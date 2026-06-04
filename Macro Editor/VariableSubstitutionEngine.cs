using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroEditor
{
    public class VariableSubstitutionEngine
    {
        /// <summary>
        /// The marker string in Line 6 that identifies a macro as a variable definition.
        /// </summary>
        public const string MARKER = "VARIABLE";

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

        public bool HasVariables { get { return _variables != null && _variables.Count > 0; } }
        public int VariableCount { get { return _variables != null ? _variables.Count : 0; } }

        public VariableSubstitutionEngine()
        {
            _variables = new Dictionary<string, List<string>>();
            _forwardPairs = new List<KeyValuePair<string, string>>();
            _reversePairs = new List<KeyValuePair<string, string>>();
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
        /// </summary>
        public string ResolvePlaceholders(string text)
        {
            if (string.IsNullOrEmpty(text) || !HasVariables)
                return text;

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
    }
}
