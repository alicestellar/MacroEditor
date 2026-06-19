# Component Definitions

## Data Layer

### Macro
- **Purpose**: Represents a single macro (one of 20 per row)
- **Responsibilities**:
  - Store macro title (max 8 characters)
  - Store 6 command lines (max 60 encoded characters each)
  - Provide deep clone for backup/revert
  - Provide empty/default state factory

### MacroRow
- **Purpose**: Represents a numbered row (1-10) within a book, containing 20 macros
- **Responsibilities**:
  - Store ordered list of 20 Macros (indices 0-9 = Ctrl, 10-19 = Alt)
  - Provide access to Ctrl-side and Alt-side subsets
  - Provide deep clone for backup/revert

### MacroBook
- **Purpose**: Represents a named macro book (one of up to 40 sets)
- **Responsibilities**:
  - Store book name (max 15 characters)
  - Store ordered list of 10 MacroRows
  - Provide deep clone for backup/revert

---

## File I/O Layer

### MacroFileManager
- **Purpose**: Handle all reading and writing of FFXI macro binary files
- **Responsibilities**:
  - Read mcr.ttl file (book names) and return List of MacroBooks
  - Read individual mcrXY.dat files (macro data) into MacroRow objects
  - Write mcr.ttl file from List of MacroBooks
  - Write individual mcrXY.dat files from MacroRow data
  - Generate MD5 checksums for file integrity
  - Generate file name from book/row indices (KillZero logic)
  - Handle missing .dat files gracefully (create empty macros)
  - Detect file format (20-book vs 40-book) from mcr.ttl size

---

## Auto-Translate Layer

### AutoTranslateEncoder
- **Purpose**: Full auto-translate phrase system including encoding, decoding, dictionary, and menu
- **Responsibilities**:
  - Load AT phrase data from Yekyaa.FFXIEncoding on initialization
  - Build AT phrase dictionary (hex code → phrase name)
  - Encode human-readable AT format `<HEX|Name>` to binary `0xFD` format for saving
  - Decode binary `0xFD` format to human-readable `<HEX|Name>` for display
  - Build the AT phrase context menu (hierarchical by category)
  - Filter AT menu items by search text (for F2 lookup)
  - Provide the built ToolStripMenuItem for MainForm to host

---

## Validation Layer

### MacroValidator
- **Purpose**: Validate macro content for FFXI compatibility
- **Responsibilities**:
  - Validate macro title length (max 8 characters)
  - Validate macro line length after AT encoding (max 60 characters)
  - Detect invalid/non-ASCII characters in macro lines
  - Detect commented-out lines (// prefix)
  - Detect non-command lines (missing / prefix)
  - Detect /wait commands (informational)
  - Detect unknown AT items
  - Return structured validation results (errors vs warnings with location info)

---

## Configuration Layer

### EditorConfig
- **Purpose**: Manage the config.json file format and settings
- **Responsibilities**:
  - Define config.json schema (mcrTtlPath, macroSetCount, schemaVersion, settings, variables)
  - Read config.json from a folder
  - Write config.json to a folder
  - Provide default configuration when no config.json exists
  - Handle backward compatibility across schema versions
  - Manage the `variables` dictionary (name → value mappings)

---

## Variable Substitution Layer

### VariableSubstitutionEngine
- **Purpose**: Handle template variable replacement for display and save operations
- **Responsibilities**:
  - On load: scan macro text for variable values and replace with `{placeholder}` for UI display
  - On save: replace `{placeholder}` tokens with actual values before writing to .dat files
  - On save: detect undefined variables (user-created `{newvar}` in UI) and prompt for value
  - On export: apply destination folder's variable values during substitution
  - Manage variable registry (from config.json `variables` object)
  - Case-sensitive matching of variable values in text

---

## Undo/Redo Layer

### UndoRedoManager
- **Purpose**: Track editing changes and support undo/redo operations
- **Responsibilities**:
  - Maintain undo stack of state snapshots or change deltas
  - Maintain redo stack
  - Support Ctrl+Z (undo) and Ctrl+Shift+Z (redo)
  - Clear redo stack when new edits occur after an undo
  - Provide state for UI buttons (undo/redo enabled/disabled)

---

## UI Layer

### MainForm (refactored)
- **Purpose**: Thin UI shell — coordinates components, handles user interactions
- **Responsibilities**:
  - Form layout and control creation
  - Event handling (clicks, key presses, menu selections)
  - Display macro data from data model
  - Coordinate between components (file I/O, validation, substitution)
  - Clipboard operations (internal copy/paste using data model)
  - Status bar updates
  - Hosting the AT context menu (provided by AutoTranslateEncoder)
  - Undo/Redo button and keyboard shortcut handling

### Assessment (unchanged structure)
- **Purpose**: Display validation and search results
- **Responsibilities**: Show clickable result lists that navigate to specific macros

### Destination (updated references)
- **Purpose**: Macro relocation dialog
- **Responsibilities**: Select target book/row/macro — uses new data model instead of MainForm.MacroContainer directly

### MacroMapForm (updated references)
- **Purpose**: Visual macro overview
- **Responsibilities**: Display all macros in a book as clickable grid — uses new data model

### Resizer (cleanup only)
- **Purpose**: Proportional control resizing utility
- **Responsibilities**: Unchanged — VB.NET idioms cleaned up in Step 0c

---

## Utility Layer

### MacroEditorUtils (static utility class)
- **Purpose**: Shared helper functions that are called from multiple classes
- **Responsibilities**:
  - String padding/fill operations
  - Any truly cross-cutting utilities identified during deduplication
  - **Membership rule**: A function belongs here ONLY if it is called from multiple different classes. If all callers of a function reside in a single class (even if called many times within that class), the function must be moved into that class as a private method.
