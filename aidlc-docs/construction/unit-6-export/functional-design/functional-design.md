# Functional Design: Unit 6 — Export with Variable Substitution

## Overview

This unit adds an Export feature that writes the current macro set to another character's folder, substituting variables with the destination character's values. Locked variables (`{!name}`) retain the source character's values.

## Export Flow

```text
1. User selects File → Export (or menu item)
2. Folder picker: user selects destination character's macro folder
3. Read destination's Book 40 to get destination variable values
4. Compare destination variables against source variables:
   - If destination is missing any variables, add placeholder entries
     (Title populated, values empty) to destination's Book 40
5. Warn user about overwrite with three options: Confirm, Backup, Cancel
6. If Backup: rename all existing macro files in destination with .backup suffix
7. Perform export:
   a. Clone Books 1-39 from memory
   b. Apply variable substitution:
      - {name} → destination Book 40 value
      - {!name} → source Book 40 value (locked, stays as current character)
   c. Write substituted .dat files to destination
   d. Write book names (.ttl files) to destination
   e. Write destination's Book 40 (with any new placeholder variables added)
8. Show success message
```

## Variable Resolution During Export

| Token | Resolution | Example |
|-------|-----------|---------|
| `{user}` | Destination Book 40 value | "Amaranti" |
| `{!user}` | Source Book 40 value | "Makaria" (stays locked) |
| `{user2}` | Destination Book 40 alt value 2 | (destination's alt) |
| `{!user2}` | Source Book 40 alt value 2 | (source's alt, locked) |
| No placeholder (literal text) | Written as-is | Unchanged |

## Destination Book 40 Handling

### Reading Destination Variables

1. Read destination's Book 40 .dat files (pages 1-10)
2. Scan using the same algorithm as `VariableSubstitutionEngine.LoadVariables`
3. Build a destination variable map

### Missing Variable Handling

If the source has variables that the destination doesn't:
- Add new macro slots to destination's Book 40 for each missing variable
- Set the Title = variable name (populated)
- Set Lines[0..4] = empty (user must fill in values later)
- Set Lines[5] = "VARIABLE" (marker)
- Place new variables on the first available slots in Page 10 (or next available page working backwards)

**Important**: If destination values are empty, the substitution for `{name}` produces empty string. This is expected — the user will fill in values later and re-export.

## Overwrite Warning Dialog

```text
"Export will overwrite macros in:
[destination folder path]

This will replace ALL macro files in that folder with the current set.

Choose an option:
[Confirm]  [Backup & Export]  [Cancel]"
```

- **Confirm**: Overwrite directly
- **Backup & Export**: Rename existing files with `.backup` suffix first, then export
- **Cancel**: Abort

## Backup Behavior

When "Backup & Export" is selected:

1. **Read destination Book 40 FIRST** (before renaming files) to get destination variable values
2. Rename all macro files in the destination:
   - `mcr.ttl` → `mcr.ttl.backup`
   - `mcr_2.ttl` → `mcr_2.ttl.backup`
   - `mcr.dat` → `mcr.dat.backup`
   - `mcr1.dat` → `mcr1.dat.backup`
   - ... (all .dat files)
   - `mcr.locks` → `mcr.locks.backup` (if exists)
3. Then proceed with normal export (writing fresh files)

**Critical**: Read destination Book 40 values BEFORE backup rename, since after rename the files won't be at their expected paths.

## UI Integration

### Menu Item

Add "Export to Folder..." to the File menu (below Save All, above Exit).

### Folder Picker

Use `FolderBrowserDialog` — user selects the destination character's macro directory (the folder containing their mcr.ttl).

## Engine Changes

Add to `VariableSubstitutionEngine`:

```csharp
/// <summary>
/// Resolves placeholders for export: {name} uses destination values,
/// {!name} uses source (local) values.
/// </summary>
public string ResolveForExport(string text, Dictionary<string, List<string>> destinationVars);

/// <summary>
/// Creates a deep clone of books 1-39 with export substitution applied.
/// {name} → destination value, {!name} → source value.
/// </summary>
public List<MacroBook> ResolveAllForExport(List<MacroBook> books,
    Dictionary<string, List<string>> destinationVars);

/// <summary>
/// Scans a destination folder's Book 40 and returns its variable map.
/// Uses MacroFileManager to read the .dat files.
/// </summary>
public Dictionary<string, List<string>> LoadDestinationVariables(
    string destPath, MacroFileManager fileManager);

/// <summary>
/// Returns variable names that exist in source but not in destination.
/// </summary>
public List<string> GetMissingVariables(Dictionary<string, List<string>> destinationVars);
```

## MainForm Integration

### Export Handler

```csharp
private void File_Export_Click(object sender, EventArgs e)
{
    // 1. Show folder picker
    // 2. Read destination Book 40
    // 3. Check for missing variables, add placeholders if needed
    // 4. Show overwrite warning (Confirm / Backup & Export / Cancel)
    // 5. If backup: read dest Book 40 first, then rename files
    // 6. Clone books, resolve for export, write to destination
    // 7. Write book names to destination
    // 8. Write destination Book 40 (with any new variable placeholders)
    // 9. Show success
}
```

## Edge Cases

| Scenario | Behavior |
|----------|----------|
| Destination has no Book 40 at all | Treat as empty variable map; add all source variables as placeholders |
| Destination has some but not all variables | Add only the missing ones as placeholders |
| Destination variable has empty value | Substitution produces empty string (expected) |
| Source has no variables | Export without any substitution (straight copy) |
| Destination folder doesn't exist | Show error, abort |
| Backup files already exist (.backup) | Overwrite old .backup files |
| Export to same folder as source | Should work (overwrites source macros with resolved versions — warn user) |

## Testing Scenarios

1. **Basic export**: Source has `{user}` = "Makaria". Destination has `user` = "Amaranti". Export → verify .dat files in destination have "Amaranti".
2. **Locked variables**: Source has `{!user}` at some positions. Export → verify those positions have "Makaria" (not "Amaranti").
3. **Missing variables**: Source has `user` + `server`. Destination only has `user`. Export → verify destination Book 40 gets a new `server` entry with empty value.
4. **Backup**: Select Backup & Export. Verify .backup files created, then fresh export written.
5. **No variables**: Source has no Book 40 variables. Export → straight copy of all files.
6. **Book names**: Verify .ttl files in destination are updated with source book names.
7. **Restore from backup**: After a Backup & Export, use Restore from Backup on destination folder. Verify .backup files replace current files, .backup files are removed.
8. **Restore without backup**: Try to restore on a folder with no .backup files. Verify user gets an error/info message.

## Restore from Backup

### Purpose

After a "Backup & Export," the user may want to undo the export and restore the destination character's original macros. This removes the exported macros and restores the `.backup` files.

### UI

Add "Restore from Backup..." to the File menu (below "Export to Folder...").

### Flow

```text
1. User selects File → Restore from Backup...
2. Folder picker: user selects the folder to restore
3. Check if .backup files exist in that folder
   - If none found: show info message "No backup files found in this folder." → abort
4. Show destructive warning:
   "This will DELETE the current macro files in:
   [folder path]
   
   and RESTORE from the .backup files. This cannot be undone.
   
   [Confirm]  [Cancel]"
5. If confirmed:
   a. Delete current macro files (mcr.ttl, mcr_2.ttl, all .dat files, mcr.locks)
   b. Rename .backup files back to original names (strip .backup suffix)
6. Show success message
7. If the restored folder is the currently loaded folder, reload the macros
```

### Behavior Details

- Only renames files that have a `.backup` counterpart — doesn't touch files without backups
- Deletes the current (non-backup) version of each file before renaming `.backup` back
- If the restored folder happens to be the currently open macro set, offer to reload
- The `.backup` files are consumed (removed) by the restore — they become the active files
