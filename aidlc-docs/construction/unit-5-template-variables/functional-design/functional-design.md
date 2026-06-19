# Functional Design: Unit 5 — Template Variables

## Overview

This unit adds a variable substitution system that stores variable definitions inside Book 40's macro slots. On load, the engine scans Book 40 to build a variable map, then replaces matching values with `{placeholder}` tokens in the UI for Books 1-39. On save, the reverse substitution restores literal values before writing to disk.

## Data Model: Variable Definitions in Book 40

### Storage Format

Each variable occupies one macro slot in Book 40:

| Macro Field | Purpose | Example |
|-------------|---------|---------|
| Title | Placeholder name | `user` |
| Line 1 (Lines[0]) | Primary value → `{name}` | `Makaria` |
| Line 2 (Lines[1]) | Alt value → `{name2}` | `Makaria's` |
| Line 3 (Lines[2]) | Alt value → `{name3}` | (empty = unused) |
| Line 4 (Lines[3]) | Alt value → `{name4}` | (empty = unused) |
| Line 5 (Lines[4]) | Alt value → `{name5}` | (empty = unused) |
| Line 6 (Lines[5]) | Marker | `VARIABLE` |

### Index Mapping

- **Book 40** = `Books[39]` (0-indexed)
- **Page N** = `Rows[N-1]` (Page 1 = Rows[0], Page 10 = Rows[9])
- **Each page** = one MacroRow containing 20 macros (indices 0-9 = Ctrl, 10-19 = Alt)

### Scanning Algorithm

```text
1. Start at Book 40, Page 10 (Rows[9])
2. For the current page, check all 20 macros (indices 0-19):
   - If macro.Lines[5] == "VARIABLE" AND macro.Title is non-empty:
     → This macro defines a variable. Add to variable map.
3. If NO macros on the entire page had the "VARIABLE" marker → STOP scanning
4. Otherwise, move to the previous page (Rows[8], then Rows[7], etc.)
5. Repeat until a page with zero markers is found, or Page 1 is reached
```

**Key rules:**
- Scanning is page-by-page, backwards from Page 10
- A page with at least one `VARIABLE` marker means the entire page is part of the variable zone
- The first page (going backwards) with ZERO markers stops the scan
- Empty macro slots without the marker are simply skipped (not variable definitions)
- The user can start with any macro on Page 10 — no requirement to fill sequentially

## Variable Map Structure

The engine produces a `Dictionary<string, List<string>>` mapping:

```csharp
// Key: placeholder name (from macro Title)
// Value: list of substitution values [primary, alt2, alt3, alt4, alt5]
//        (only non-empty values are stored)
{
    "user"   → ["Makaria", "Makaria's"],
    "server" → ["Asura"],
    "ls"     → ["MyLinkshell"]
}
```

### Placeholder Token Format

- Primary value: `{name}` (e.g., `{user}`)
- Alt values: `{name2}`, `{name3}`, `{name4}`, `{name5}` (e.g., `{user2}`)

## Substitution Behavior

### On Load (Values → Placeholders)

After all 40 books are loaded from disk into `Books[]`:

1. Scan Book 40 to build the variable map
2. For each book in Books[0] through Books[38] (Books 1-39):
   - For each macro in each row:
     - For each field (Title + Lines[0..5]):
       - Replace literal values with placeholder tokens
       - Match is **case-sensitive** and **whole-word-aware** (not substring)
       - Process longer values first to avoid partial replacements
       - Primary value → `{name}`, alt values → `{name2}` through `{name5}`
3. Book 40 itself is NOT substituted — it shows raw variable definitions

### On Save (Placeholders → Values)

Before writing Books 1-39 to disk:

1. Read the current variable map (from Book 40 in memory)
2. For each book in Books[0] through Books[38]:
   - Clone the book data (don't modify the in-memory display version)
   - For each macro in the clone:
     - Replace placeholder tokens with literal values
     - `{name}` → primary value, `{name2}` → alt value 2, etc.
3. Write the cloned (de-substituted) data to .dat files
4. Book 40 is written as-is (no substitution applied)

### On Display (UI TextBoxes)

- TextBoxes show the substituted form (with `{placeholders}`)
- Placeholders should be visually highlighted (distinct color) — **deferred to UI highlighting sub-task**
- When the user edits text, they work with placeholders directly
- The `Lines_TextChanged` handler stores whatever the user types (including placeholders)

## VariableSubstitutionEngine Class Design

```csharp
namespace MacroEditor
{
    public class VariableSubstitutionEngine
    {
        // The marker string that identifies a variable definition macro
        public const string MARKER = "VARIABLE";

        // The loaded variable map: placeholder name → list of values
        private Dictionary<string, List<string>> _variables;

        // Whether variables have been loaded
        public bool HasVariables { get; }

        // Number of variables loaded
        public int VariableCount { get; }

        /// <summary>
        /// Scans Book 40 (passed as a MacroBook) and builds the variable map.
        /// Returns the number of variables found.
        /// </summary>
        public int LoadVariables(MacroBook book40);

        /// <summary>
        /// Applies forward substitution (values → placeholders) to a single string.
        /// Used on load to convert raw file data to display form.
        /// </summary>
        public string ApplyPlaceholders(string text);

        /// <summary>
        /// Applies reverse substitution (placeholders → values) to a single string.
        /// Used on save to convert display form back to raw file data.
        /// </summary>
        public string ResolvePlaceholders(string text);

        /// <summary>
        /// Applies forward substitution to all macros in books 1-39.
        /// Modifies the book data in-place (for display).
        /// Skips Book 40 (index 39).
        /// </summary>
        public void SubstituteAll(List<MacroBook> books);

        /// <summary>
        /// Creates a deep clone of books 1-39 with reverse substitution applied.
        /// Used before saving to produce file-ready data.
        /// Does NOT modify the original books list.
        /// </summary>
        public List<MacroBook> ResolveAllForSave(List<MacroBook> books);

        /// <summary>
        /// Returns all loaded variable names (for UI display/highlighting).
        /// </summary>
        public IReadOnlyList<string> GetVariableNames();

        /// <summary>
        /// Returns the variable map for inspection (e.g., export logic).
        /// </summary>
        public IReadOnlyDictionary<string, List<string>> GetVariableMap();
    }
}
```

## Integration Points

### MainForm — File_Open_Click (after loading)

Current flow:
```text
1. Read book names from .ttl files
2. For each book 0..39: read all .dat rows into Books[]
3. Enable UI controls
```

New flow (additions marked with ►):
```text
1. Read book names from .ttl files
2. For each book 0..39: read all .dat rows into Books[]
► 3. variableEngine.LoadVariables(Books[39])
► 4. variableEngine.SubstituteAll(Books)  // modifies Books[0..38] in-place
5. Enable UI controls
```

### MainForm — File_SaveAll_Click (before writing)

Current flow:
```text
1. Confirm dialog
2. For each book 0..debuglimit, each row 0..9: WriteFile(book, row)
3. Save book names
```

New flow:
```text
1. Confirm dialog
► 2. var saveBooks = variableEngine.ResolveAllForSave(Books)
3. For each book 0..38: write from saveBooks[book] rows
4. For book 39 (Book 40): write directly from Books[39] (no substitution)
5. Save book names
```

### MainForm — File_SaveRow_Click (single row save)

Same principle: resolve placeholders for the specific row before writing, but don't modify the in-memory display data.

### MainForm — Constructor

Add field:
```csharp
private VariableSubstitutionEngine variableEngine;
```

Initialize in constructor:
```csharp
this.variableEngine = new VariableSubstitutionEngine();
```

## Substitution Order and Conflict Resolution

### Forward Substitution (values → placeholders)

1. Sort variable values by **length descending** (longest values first)
2. This prevents partial matches: if `user` = "Makaria" and `user2` = "Makaria's", process "Makaria's" before "Makaria"
3. Each replacement is done once per occurrence (standard string replace)
4. Case-sensitive matching

### Reverse Substitution (placeholders → values)

1. Sort placeholders by **length descending** (e.g., `{user2}` before `{user}`)
2. This prevents `{user}` from matching inside `{user2}`
3. Simple string replace: `{name}` → value, `{name2}` → value2, etc.

## Edge Cases

| Scenario | Behavior |
|----------|----------|
| No variables defined in Book 40 | Engine loads zero variables, no substitution occurs, app works normally |
| Variable with empty Title | Skipped (Title must be non-empty to define a variable) |
| Variable with empty Line 1 | Skipped (primary value must be non-empty) |
| Variable value appears in Book 40 itself | Book 40 is never substituted — only Books 1-39 |
| User types a placeholder manually (e.g., `{user}`) | It will be reverse-substituted on save like any other placeholder |
| Placeholder text not found in reverse substitution | Left as-is (no error) |
| Value not found in forward substitution | No placeholder inserted (no error) |
| Multiple variables with same value | First match wins (order: longer placeholder names first) |
| Book 40 edited in UI after load | Variable map should be refreshable — but for v1, variables are loaded once at file open. User must re-open to pick up Book 40 edits. |

## UI Highlighting (Deferred Sub-task)

Placeholder highlighting in TextBoxes will be addressed as a sub-task after the core engine works. Options:
- RichTextBox replacement (enables colored text)
- Custom painting over TextBox
- Simple visual indicator (e.g., tooltip showing "contains variables")

For v1, the core substitution engine is the priority. Highlighting can be added incrementally.

## File Structure

New file to create:
```text
Macro Editor/
└── VariableSubstitutionEngine.cs    (in project root, alongside other service classes)
```

## Testing Scenarios

1. **Basic substitution**: Define `user` = "Makaria" in Book 40 Page 10. Have "Makaria" in a macro in Book 1. Open file → verify UI shows `{user}`. Save → verify .dat file contains "Makaria".
2. **Multiple variables**: Define `user` + `server` + `ls`. Verify all substitute correctly.
3. **Alt values**: Define `user` with Line 1 = "Makaria", Line 2 = "Makaria's". Verify `{user}` and `{user2}` both work.
4. **No variables**: Open a file where Book 40 has no VARIABLE markers. Verify app works normally with no substitution.
5. **Partial page**: Only 3 macros on Page 10 have VARIABLE marker. Verify only those 3 are loaded as variables.
6. **Multi-page**: Variables on Pages 10 and 9. Page 8 has no markers. Verify scanning stops at Page 8.
7. **Save preserves Book 40**: Save all → verify Book 40 .dat files are unchanged (no substitution applied to them).

## Variable Locking (`{!name}` syntax)

### Purpose

Some placeholder instances should **never** be replaced during export. For example, a macro that tells Makaria to use her weaponskill should always say "Makaria" — even when exporting to Amaranti's folder. The user marks these with `{!user}` instead of `{user}`.

### Behavior Summary

| Token | On normal save | On export (to Amaranti) | Display in UI |
|-------|---------------|------------------------|---------------|
| `{user}` | Writes "Makaria" (source value) | Writes "Amaranti" (dest value) | `{user}` |
| `{!user}` | Writes "Makaria" (source value) | Writes "Makaria" (source value) | `{!user}` |

### Lock File: `mcr.locks`

A JSON file stored in the same directory as the macro .dat files. It persists which placeholder instances are locked so they survive load/save cycles.

**File format:**
```json
{
  "version": 1,
  "locks": [
    { "book": 0, "page": 2, "macro": 5, "line": 3, "variable": "user" },
    { "book": 3, "page": 0, "macro": 14, "line": 1, "variable": "user" }
  ]
}
```

Fields (all 0-indexed):
- `book`: Book index (0-38 for Books 1-39)
- `page`: Row/page index (0-9)
- `macro`: Macro index within the row (0-19)
- `line`: 0 = Title, 1-6 = Lines[0]-Lines[5]
- `variable`: The placeholder name (without braces or `!`)

### Lock Flow

#### On Load

```text
1. Read all .dat files into Books[]
2. Scan Book 40 → build variable map
3. Read mcr.locks (if it exists)
4. Apply forward substitution: all values → {placeholder} in Books 1-39
5. For each lock entry:
   a. Check if {variableName} exists at the specified position
   b. If YES → change it to {!variableName} (apply lock)
   c. If NO → delete this lock entry (stale, macro was edited/moved)
6. Save cleaned lock list in memory (stale entries removed)
```

#### On Save

```text
1. Reverse substitution for disk:
   - {user} → source value (e.g., "Makaria")
   - {!user} → source value (e.g., "Makaria") — same on normal save
2. Write .dat files
3. Rebuild lock file: scan in-memory Books 1-39 for all {!...} tokens,
   record their positions
4. Write mcr.locks to disk
```

#### On Export (Unit 6)

```text
1. Reverse substitution for export:
   - {user} → destination value (e.g., "Amaranti")
   - {!user} → SOURCE value (e.g., "Makaria") — stays locked
2. Write exported .dat files to destination
3. Do NOT write a lock file to the destination (destination has literal values)
4. Source mcr.locks is unchanged
```

### User Interaction

- **To lock**: Edit the text box, change `{user}` to `{!user}` (just add `!`)
- **To unlock**: Edit the text box, change `{!user}` to `{user}` (just remove `!`)
- **Persistence**: The lock file is automatically maintained — user never touches it directly

### Self-Healing

On every load, stale lock entries are removed. A lock becomes stale when:
- The macro at that position no longer contains the expected variable value
- The macro was moved, deleted, or overwritten by a different macro
- The variable definition was removed from Book 40

This means locks are "best effort" — if you rearrange macros, the locks for moved macros will be lost. The user would need to re-add `!` to re-lock them. This is acceptable since macro rearrangement is infrequent.

### Engine Changes

The `VariableSubstitutionEngine` needs these additions:

```csharp
/// <summary>
/// Loads lock data from a JSON file. Returns number of locks loaded.
/// </summary>
public int LoadLocks(string lockFilePath);

/// <summary>
/// Applies locks to the in-memory books after forward substitution.
/// Changes {name} → {!name} at locked positions.
/// Removes stale lock entries and returns the count of stale entries removed.
/// </summary>
public int ApplyLocks(List<MacroBook> books);

/// <summary>
/// Rebuilds the lock list by scanning books for {!...} tokens.
/// </summary>
public void RebuildLocks(List<MacroBook> books);

/// <summary>
/// Saves the lock file to disk.
/// </summary>
public void SaveLocks(string lockFilePath);

/// <summary>
/// During reverse substitution, resolves {!name} using source values
/// (same as {name} for normal save, but different for export).
/// </summary>
public string ResolvePlaceholders(string text, bool isExport = false,
    Dictionary<string, List<string>> destinationVariables = null);
```

### Testing Scenarios (Lock-specific)

8. **Basic lock**: Set `{!user}` in a macro. Save → verify .dat has "Makaria". Reload → verify `{!user}` is restored (not `{user}`).
9. **Lock survives reload**: Close and reopen file. Verify `{!user}` is still marked.
10. **Stale lock removal**: Set `{!user}`, save. Then edit that macro to remove the name entirely. Reload → lock entry should be gone from mcr.locks.
11. **Export with locks**: Export to Amaranti's folder. Verify `{user}` positions have "Amaranti" but `{!user}` positions still have "Makaria".
12. **No lock file**: Open a macro set with no mcr.locks file. Verify everything works normally (no locks applied).
