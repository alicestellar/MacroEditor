using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MacroEditor
{
    /// <summary>
    /// Represents a single variable lock entry (a placeholder instance that should
    /// not be replaced with destination values during export).
    /// </summary>
    public class VariableLock
    {
        public int Book { get; set; }
        public int Page { get; set; }
        public int MacroIndex { get; set; }
        public int Line { get; set; }
        public string Variable { get; set; }
    }

    public class VariableSubstitutionEngine
    {
        /// <summary>
        /// The marker string in Line 6 that identifies a macro as a variable definition.
        /// </summary>
        public const string MARKER = "VARIABLE";

        /// <summary>
        /// The lock file name stored in the macro directory.
        /// </summary>
        public const string LOCK_FILENAME = "mcr.locks";

        /// <summary>
        /// Variable map: placeholder name → list of values (primary + alts).
        /// Index 0 = primary ({name}), index 1 = {name2}, etc.
        /// </summary>
        private Dictionary<string, List<string>> _variables;

        /// <summary>
        /// Sorted replacement pairs for forward substitution (values → placeholders).
        /// Sorted by value length descending to prevent partial matches.
        /// </summary>
        private List<KeyValuePair<string, string>> _forwardPairs;

        /// <summary>
        /// Sorted replacement pairs for reverse substitution (placeholders → values).
        /// Sorted by placeholder length descending to prevent partial matches.
        /// </summary>
        private List<KeyValuePair<string, string>> _reversePairs;

        /// <summary>
        /// Current lock entries in memory.
        /// </summary>
        private List<VariableLock> _locks;

        public bool HasVariables { get { return _variables != null && _variables.Count > 0; } }
        public int VariableCount { get { return _variables != null ? _variables.Count : 0; } }
        public int LockCount { get { return _locks != null ? _locks.Count : 0; } }

        public VariableSubstitutionEngine()
        {
            _variables = new Dictionary<string, List<string>>();
            _forwardPairs = new List<KeyValuePair<string, string>>();
            _reversePairs = new List<KeyValuePair<string, string>>();
            _locks = new List<VariableLock>();
        }

        /// <summary>
        /// Scans Book 40 (passed as a MacroBook) and builds the variable map.
        /// Scans backwards from Page 10 (Rows[9]) to Page 1 (Rows[0]).
        /// Stops at the first complete page with no VARIABLE markers.
        /// Returns the number of variables found.
        /// </summary>
        public int LoadVariables(MacroBook book40)
        {
            _variables.Clear();
            _forwardPairs.Clear();
            _reversePairs.Clear();

            if (book40 == null)
                return 0;

            // Scan backwards from Page 10 (index 9) to Page 1 (index 0)
            for (int rowIndex = 9; rowIndex >= 0; rowIndex--)
            {
                MacroRow row = book40.Rows[rowIndex];
                bool pageHasMarker = false;

                for (int macroIndex = 0; macroIndex < 20; macroIndex++)
                {
                    Macro macro = row.Macros[macroIndex];

                    // Check if this macro has the VARIABLE marker in Line 6 (Lines[5])
                    if (string.Equals(macro.Lines[5].Trim(), MARKER, StringComparison.OrdinalIgnoreCase))
                    {
                        pageHasMarker = true;

                        // Title must be non-empty to define a variable
                        string name = macro.Title.Trim();
                        if (string.IsNullOrEmpty(name))
                            continue;

                        // Primary value (Line 1) must be non-empty
                        string primaryValue = macro.Lines[0].Trim();
                        if (string.IsNullOrEmpty(primaryValue))
                            continue;

                        // Build value list: primary + up to 4 alt values
                        var values = new List<string>();
                        values.Add(primaryValue);

                        for (int lineIdx = 1; lineIdx <= 4; lineIdx++)
                        {
                            string altValue = macro.Lines[lineIdx].Trim();
                            if (!string.IsNullOrEmpty(altValue))
                                values.Add(altValue);
                            else
                                break; // Stop at first empty alt value
                        }

                        // Add to map (later definitions don't overwrite earlier ones)
                        if (!_variables.ContainsKey(name))
                        {
                            _variables[name] = values;
                        }
                    }
                }

                // If this entire page had no markers, stop scanning
                if (!pageHasMarker)
                    break;
            }

            // Build sorted replacement pairs
            BuildReplacementPairs();

            return _variables.Count;
        }

        /// <summary>
        /// Builds the forward and reverse replacement pair lists,
        /// sorted by length descending to prevent partial matches.
        /// </summary>
        private void BuildReplacementPairs()
        {
            _forwardPairs.Clear();
            _reversePairs.Clear();

            foreach (var kvp in _variables)
            {
                string name = kvp.Key;
                List<string> values = kvp.Value;

                // Primary: value → {name}
                string placeholder = "{" + name + "}";
                _forwardPairs.Add(new KeyValuePair<string, string>(values[0], placeholder));
                _reversePairs.Add(new KeyValuePair<string, string>(placeholder, values[0]));

                // Alt values: value → {name2}, {name3}, etc.
                for (int i = 1; i < values.Count; i++)
                {
                    string altPlaceholder = "{" + name + (i + 1) + "}";
                    _forwardPairs.Add(new KeyValuePair<string, string>(values[i], altPlaceholder));
                    _reversePairs.Add(new KeyValuePair<string, string>(altPlaceholder, values[i]));
                }
            }

            // Sort forward pairs by value length descending (longer values replaced first)
            _forwardPairs.Sort((a, b) => b.Key.Length.CompareTo(a.Key.Length));

            // Sort reverse pairs by placeholder length descending (longer placeholders replaced first)
            _reversePairs.Sort((a, b) => b.Key.Length.CompareTo(a.Key.Length));
        }

        /// <summary>
        /// Applies forward substitution (values → placeholders) to a single string.
        /// </summary>
        public string ApplyPlaceholders(string text)
        {
            if (string.IsNullOrEmpty(text) || !HasVariables)
                return text;

            foreach (var pair in _forwardPairs)
            {
                if (text.Contains(pair.Key))
                {
                    text = text.Replace(pair.Key, pair.Value);
                }
            }

            return text;
        }

        /// <summary>
        /// Applies reverse substitution (placeholders → values) to a single string.
        /// Handles both {name} and {!name} (locked) tokens.
        /// For normal save, both resolve to the source value.
        /// </summary>
        public string ResolvePlaceholders(string text)
        {
            if (string.IsNullOrEmpty(text) || !HasVariables)
                return text;

            // First resolve locked placeholders {!name} → source value
            foreach (var pair in _reversePairs)
            {
                // Build the locked version: {name} → {!name}
                string lockedPlaceholder = "{!" + pair.Key.Substring(1); // "{name}" → "{!name}"
                if (text.Contains(lockedPlaceholder))
                {
                    text = text.Replace(lockedPlaceholder, pair.Value);
                }
            }

            // Then resolve normal placeholders {name} → source value
            foreach (var pair in _reversePairs)
            {
                if (text.Contains(pair.Key))
                {
                    text = text.Replace(pair.Key, pair.Value);
                }
            }

            return text;
        }

        /// <summary>
        /// Applies forward substitution to all macros in Books 1-39 (indices 0-38).
        /// Modifies the book data in-place for display.
        /// Skips Book 40 (index 39).
        /// </summary>
        public void SubstituteAll(List<MacroBook> books)
        {
            if (!HasVariables || books == null)
                return;

            int limit = Math.Min(books.Count - 1, 38); // Process indices 0-38 (Books 1-39)
            for (int bookIdx = 0; bookIdx <= limit; bookIdx++)
            {
                MacroBook book = books[bookIdx];
                foreach (MacroRow row in book.Rows)
                {
                    foreach (Macro macro in row.Macros)
                    {
                        macro.Title = ApplyPlaceholders(macro.Title);
                        for (int lineIdx = 0; lineIdx < macro.Lines.Count; lineIdx++)
                        {
                            macro.Lines[lineIdx] = ApplyPlaceholders(macro.Lines[lineIdx]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a deep clone of books 1-39 with reverse substitution applied.
        /// Used before saving to produce file-ready data.
        /// Does NOT modify the original books list.
        /// Returns a list of 39 cloned books (indices 0-38 correspond to Books 1-39).
        /// </summary>
        public List<MacroBook> ResolveAllForSave(List<MacroBook> books)
        {
            var resolved = new List<MacroBook>();

            int limit = Math.Min(books.Count - 1, 38);
            for (int bookIdx = 0; bookIdx <= limit; bookIdx++)
            {
                MacroBook clone = books[bookIdx].Clone();

                if (HasVariables)
                {
                    foreach (MacroRow row in clone.Rows)
                    {
                        foreach (Macro macro in row.Macros)
                        {
                            macro.Title = ResolvePlaceholders(macro.Title);
                            for (int lineIdx = 0; lineIdx < macro.Lines.Count; lineIdx++)
                            {
                                macro.Lines[lineIdx] = ResolvePlaceholders(macro.Lines[lineIdx]);
                            }
                        }
                    }
                }

                resolved.Add(clone);
            }

            return resolved;
        }

        /// <summary>
        /// Returns all loaded variable names (placeholder names).
        /// </summary>
        public IReadOnlyList<string> GetVariableNames()
        {
            return _variables.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// Returns the variable map for inspection.
        /// </summary>
        public IReadOnlyDictionary<string, List<string>> GetVariableMap()
        {
            return _variables;
        }

        // ===== LOCK FILE SUPPORT =====

        /// <summary>
        /// Loads lock entries from a JSON lock file.
        /// Returns the number of locks loaded.
        /// </summary>
        public int LoadLocks(string lockFilePath)
        {
            _locks.Clear();

            if (!File.Exists(lockFilePath))
                return 0;

            try
            {
                string json = File.ReadAllText(lockFilePath);
                ParseLocksJson(json);
            }
            catch
            {
                // If lock file is corrupt, start fresh
                _locks.Clear();
            }

            return _locks.Count;
        }

        /// <summary>
        /// Applies locks to in-memory books after forward substitution.
        /// For each lock entry, checks if {variableName} exists at that position.
        /// If yes → changes to {!variableName}. If no → removes the stale lock.
        /// Returns the number of stale entries removed.
        /// </summary>
        public int ApplyLocks(List<MacroBook> books)
        {
            if (_locks.Count == 0 || books == null)
                return 0;

            int staleCount = 0;
            var validLocks = new List<VariableLock>();

            foreach (var lockEntry in _locks)
            {
                // Bounds check
                if (lockEntry.Book < 0 || lockEntry.Book >= Math.Min(books.Count, 39))
                    { staleCount++; continue; }
                if (lockEntry.Page < 0 || lockEntry.Page >= books[lockEntry.Book].Rows.Count)
                    { staleCount++; continue; }
                if (lockEntry.MacroIndex < 0 || lockEntry.MacroIndex >= 20)
                    { staleCount++; continue; }
                if (lockEntry.Line < 0 || lockEntry.Line > 6)
                    { staleCount++; continue; }

                Macro macro = books[lockEntry.Book].Rows[lockEntry.Page].Macros[lockEntry.MacroIndex];
                string fieldValue = (lockEntry.Line == 0) ? macro.Title : macro.Lines[lockEntry.Line - 1];

                // Check if the expected placeholder exists at this position
                // Look for {variable} or {variable2}, {variable3}, etc.
                string basePlaceholder = "{" + lockEntry.Variable + "}";
                bool found = false;

                if (fieldValue.Contains(basePlaceholder))
                {
                    // Replace {name} with {!name} at this position
                    string lockedPlaceholder = "{!" + lockEntry.Variable + "}";
                    string newValue = fieldValue.Replace(basePlaceholder, lockedPlaceholder);
                    if (lockEntry.Line == 0)
                        macro.Title = newValue;
                    else
                        macro.Lines[lockEntry.Line - 1] = newValue;
                    found = true;
                }
                else
                {
                    // Check alt placeholders {name2}, {name3}, etc.
                    for (int alt = 2; alt <= 5; alt++)
                    {
                        string altPlaceholder = "{" + lockEntry.Variable + alt + "}";
                        if (fieldValue.Contains(altPlaceholder))
                        {
                            string lockedAlt = "{!" + lockEntry.Variable + alt + "}";
                            string newValue = fieldValue.Replace(altPlaceholder, lockedAlt);
                            if (lockEntry.Line == 0)
                                macro.Title = newValue;
                            else
                                macro.Lines[lockEntry.Line - 1] = newValue;
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                    validLocks.Add(lockEntry);
                else
                    staleCount++;
            }

            _locks = validLocks;
            return staleCount;
        }

        /// <summary>
        /// Rebuilds the lock list by scanning Books 1-39 for {!...} tokens.
        /// Call this before saving the lock file.
        /// </summary>
        public void RebuildLocks(List<MacroBook> books)
        {
            _locks.Clear();

            if (books == null || !HasVariables)
                return;

            var lockedPattern = new Regex(@"\{!([^}]+)\}");
            int limit = Math.Min(books.Count - 1, 38);

            for (int bookIdx = 0; bookIdx <= limit; bookIdx++)
            {
                MacroBook book = books[bookIdx];
                for (int pageIdx = 0; pageIdx < book.Rows.Count; pageIdx++)
                {
                    MacroRow row = book.Rows[pageIdx];
                    for (int macroIdx = 0; macroIdx < row.Macros.Count; macroIdx++)
                    {
                        Macro macro = row.Macros[macroIdx];

                        // Check Title (line index 0)
                        CheckFieldForLocks(macro.Title, bookIdx, pageIdx, macroIdx, 0, lockedPattern);

                        // Check Lines 1-6 (line indices 1-6)
                        for (int lineIdx = 0; lineIdx < macro.Lines.Count; lineIdx++)
                        {
                            CheckFieldForLocks(macro.Lines[lineIdx], bookIdx, pageIdx, macroIdx, lineIdx + 1, lockedPattern);
                        }
                    }
                }
            }
        }

        private void CheckFieldForLocks(string text, int book, int page, int macroIdx, int line, Regex pattern)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var matches = pattern.Matches(text);
            foreach (Match match in matches)
            {
                string varName = match.Groups[1].Value;
                // Strip trailing number for alt placeholders (e.g., "user2" → "user")
                string baseVar = Regex.Replace(varName, @"\d+$", "");
                // Only create lock if this is a known variable
                if (_variables.ContainsKey(baseVar))
                {
                    _locks.Add(new VariableLock
                    {
                        Book = book,
                        Page = page,
                        MacroIndex = macroIdx,
                        Line = line,
                        Variable = baseVar
                    });
                }
            }
        }

        /// <summary>
        /// Saves the current lock list to a JSON file.
        /// </summary>
        public void SaveLocks(string lockFilePath)
        {
            if (_locks.Count == 0)
            {
                // Delete lock file if no locks exist
                if (File.Exists(lockFilePath))
                    File.Delete(lockFilePath);
                return;
            }

            string json = BuildLocksJson();
            File.WriteAllText(lockFilePath, json);
        }

        /// <summary>
        /// Simple JSON parser for the lock file (avoids dependency on JSON library).
        /// </summary>
        private void ParseLocksJson(string json)
        {
            // Minimal parser: find "locks" array and extract objects
            int locksStart = json.IndexOf("\"locks\"");
            if (locksStart < 0) return;

            int arrayStart = json.IndexOf('[', locksStart);
            if (arrayStart < 0) return;

            int arrayEnd = json.LastIndexOf(']');
            if (arrayEnd < 0) return;

            string arrayContent = json.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);

            // Split by "}" to find individual objects
            var objectPattern = new Regex(@"\{[^{}]+\}");
            var matches = objectPattern.Matches(arrayContent);

            foreach (Match match in matches)
            {
                string obj = match.Value;
                var lockEntry = new VariableLock();

                // Extract integer fields
                var bookMatch = Regex.Match(obj, @"""book""\s*:\s*(\d+)");
                var pageMatch = Regex.Match(obj, @"""page""\s*:\s*(\d+)");
                var macroMatch = Regex.Match(obj, @"""macro""\s*:\s*(\d+)");
                var lineMatch = Regex.Match(obj, @"""line""\s*:\s*(\d+)");
                var varMatch = Regex.Match(obj, @"""variable""\s*:\s*""([^""]+)""");

                if (bookMatch.Success && pageMatch.Success && macroMatch.Success &&
                    lineMatch.Success && varMatch.Success)
                {
                    lockEntry.Book = int.Parse(bookMatch.Groups[1].Value);
                    lockEntry.Page = int.Parse(pageMatch.Groups[1].Value);
                    lockEntry.MacroIndex = int.Parse(macroMatch.Groups[1].Value);
                    lockEntry.Line = int.Parse(lineMatch.Groups[1].Value);
                    lockEntry.Variable = varMatch.Groups[1].Value;
                    _locks.Add(lockEntry);
                }
            }
        }

        /// <summary>
        /// Builds JSON string for the lock file.
        /// </summary>
        private string BuildLocksJson()
        {
            var lines = new List<string>();
            lines.Add("{");
            lines.Add("  \"version\": 1,");
            lines.Add("  \"locks\": [");

            for (int i = 0; i < _locks.Count; i++)
            {
                var l = _locks[i];
                string comma = (i < _locks.Count - 1) ? "," : "";
                lines.Add(string.Format(
                    "    {{ \"book\": {0}, \"page\": {1}, \"macro\": {2}, \"line\": {3}, \"variable\": \"{4}\" }}{5}",
                    l.Book, l.Page, l.MacroIndex, l.Line, l.Variable, comma));
            }

            lines.Add("  ]");
            lines.Add("}");
            return string.Join("\r\n", lines);
        }

        // ===== EXPORT SUPPORT =====

        /// <summary>
        /// Scans a destination folder's Book 40 .dat files and returns the variable map.
        /// Returns empty dictionary if Book 40 doesn't exist or has no variables.
        /// </summary>
        public Dictionary<string, List<string>> LoadDestinationVariables(string destPath, MacroFileManager fileManager)
        {
            var destVars = new Dictionary<string, List<string>>();

            // Build a temporary Book 40 by reading its .dat files
            MacroBook destBook40 = new MacroBook("Variables");
            for (int rowIdx = 0; rowIdx < 10; rowIdx++)
            {
                string datPath = destPath + "\\mcr" + MacroEditorUtils.GetMacroFileSuffix(39, rowIdx) + ".dat";
                MacroRow row = fileManager.ReadMacroRow(datPath);
                if (row != null)
                    destBook40.Rows[rowIdx] = row;
            }

            // Scan using same algorithm
            for (int rowIndex = 9; rowIndex >= 0; rowIndex--)
            {
                MacroRow row = destBook40.Rows[rowIndex];
                bool pageHasMarker = false;

                for (int macroIndex = 0; macroIndex < 20; macroIndex++)
                {
                    Macro macro = row.Macros[macroIndex];
                    if (string.Equals(macro.Lines[5].Trim(), MARKER, StringComparison.OrdinalIgnoreCase))
                    {
                        pageHasMarker = true;
                        string name = macro.Title.Trim();
                        if (string.IsNullOrEmpty(name)) continue;

                        var values = new List<string>();
                        for (int lineIdx = 0; lineIdx <= 4; lineIdx++)
                        {
                            string val = macro.Lines[lineIdx].Trim();
                            values.Add(val); // Include even if empty for export
                        }

                        if (!destVars.ContainsKey(name))
                            destVars[name] = values;
                    }
                }

                if (!pageHasMarker)
                    break;
            }

            return destVars;
        }

        /// <summary>
        /// Returns variable names that exist in source but not in destination.
        /// </summary>
        public List<string> GetMissingVariables(Dictionary<string, List<string>> destinationVars)
        {
            var missing = new List<string>();
            foreach (var name in _variables.Keys)
            {
                if (!destinationVars.ContainsKey(name))
                    missing.Add(name);
            }
            return missing;
        }

        /// <summary>
        /// Creates placeholder variable macros for missing variables and adds them to
        /// a destination Book 40. Returns the modified book.
        /// </summary>
        public MacroBook AddMissingVariablesToBook40(MacroBook destBook40, List<string> missingVars)
        {
            if (missingVars == null || missingVars.Count == 0)
                return destBook40;

            if (destBook40 == null)
                destBook40 = new MacroBook("Variables");

            int added = 0;
            // Find empty slots starting from Page 10, macro 0
            for (int rowIdx = 9; rowIdx >= 0 && added < missingVars.Count; rowIdx--)
            {
                for (int macroIdx = 0; macroIdx < 20 && added < missingVars.Count; macroIdx++)
                {
                    Macro macro = destBook40.Rows[rowIdx].Macros[macroIdx];
                    // Skip if this slot already has a VARIABLE marker
                    if (string.Equals(macro.Lines[5].Trim(), MARKER, StringComparison.OrdinalIgnoreCase))
                        continue;
                    // Skip if slot is non-empty (has content)
                    if (!macro.IsEmpty())
                        continue;

                    // Place the missing variable here
                    macro.Title = missingVars[added];
                    macro.Lines[0] = ""; // Empty - user must fill in
                    macro.Lines[1] = "";
                    macro.Lines[2] = "";
                    macro.Lines[3] = "";
                    macro.Lines[4] = "";
                    macro.Lines[5] = MARKER;
                    added++;
                }
            }

            return destBook40;
        }

        /// <summary>
        /// Resolves a single string for export:
        /// {name} → destination value, {!name} → source value.
        /// </summary>
        public string ResolveForExport(string text, Dictionary<string, List<string>> destinationVars)
        {
            if (string.IsNullOrEmpty(text) || !HasVariables)
                return text;

            // First resolve locked placeholders {!name} → source value
            foreach (var kvp in _variables)
            {
                string name = kvp.Key;
                List<string> sourceValues = kvp.Value;

                // Primary locked: {!name} → source value
                string lockedPlaceholder = "{!" + name + "}";
                if (text.Contains(lockedPlaceholder))
                {
                    text = text.Replace(lockedPlaceholder, sourceValues[0]);
                }

                // Alt locked: {!name2}, {!name3}, etc. → source alt values
                for (int i = 1; i < sourceValues.Count; i++)
                {
                    string lockedAlt = "{!" + name + (i + 1) + "}";
                    if (text.Contains(lockedAlt))
                    {
                        text = text.Replace(lockedAlt, sourceValues[i]);
                    }
                }
            }

            // Then resolve normal placeholders {name} → destination value
            foreach (var kvp in _variables)
            {
                string name = kvp.Key;
                List<string> sourceValues = kvp.Value;

                List<string> destValues = null;
                if (destinationVars.ContainsKey(name))
                    destValues = destinationVars[name];

                // Primary: {name} → dest value (or source value if no dest value)
                string placeholder = "{" + name + "}";
                if (text.Contains(placeholder))
                {
                    string destVal = (destValues != null && destValues.Count > 0 && !string.IsNullOrEmpty(destValues[0]))
                        ? destValues[0] : sourceValues[0];
                    text = text.Replace(placeholder, destVal);
                }

                // Alt values: {name2} → dest value 2, etc.
                for (int i = 1; i < sourceValues.Count; i++)
                {
                    string altPlaceholder = "{" + name + (i + 1) + "}";
                    if (text.Contains(altPlaceholder))
                    {
                        string destVal = (destValues != null && destValues.Count > i && !string.IsNullOrEmpty(destValues[i]))
                            ? destValues[i] : sourceValues[i];
                        text = text.Replace(altPlaceholder, destVal);
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Creates a deep clone of books 1-39 with export substitution applied.
        /// {name} → destination value, {!name} → source value.
        /// </summary>
        public List<MacroBook> ResolveAllForExport(List<MacroBook> books, Dictionary<string, List<string>> destinationVars)
        {
            var resolved = new List<MacroBook>();

            int limit = Math.Min(books.Count - 1, 38);
            for (int bookIdx = 0; bookIdx <= limit; bookIdx++)
            {
                MacroBook clone = books[bookIdx].Clone();

                if (HasVariables)
                {
                    foreach (MacroRow row in clone.Rows)
                    {
                        foreach (Macro macro in row.Macros)
                        {
                            macro.Title = ResolveForExport(macro.Title, destinationVars);
                            for (int lineIdx = 0; lineIdx < macro.Lines.Count; lineIdx++)
                            {
                                macro.Lines[lineIdx] = ResolveForExport(macro.Lines[lineIdx], destinationVars);
                            }
                        }
                    }
                }

                resolved.Add(clone);
            }

            return resolved;
        }
    }
}
