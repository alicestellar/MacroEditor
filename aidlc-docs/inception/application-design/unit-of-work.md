# Units of Work

## Architecture Context
- **Project Type**: Monolith (WinForms Desktop Application)
- **Deployment Model**: Single executable
- **Unit Strategy**: Sequential phases with testing gates (not parallel development)

## Units

### Unit 1: Step 0a — Code Deduplication
- **Purpose**: Extract duplicated code patterns into a shared helper class
- **Scope**: Identify repeated patterns in MainForm.cs, extract to MacroEditorUtils static class
- **Deliverables**: MacroEditorUtils.cs with shared functions, MainForm.cs calling the utility
- **Testing Gate**: Manual run — all existing functionality preserved
- **Entry Criteria**: Baseline commit verified (DONE)
- **Exit Criteria**: Builds clean, runs identically to baseline

### Unit 2: Step 0b — MVC Separation
- **Purpose**: Decompose MainForm into layered architecture with typed data model
- **Scope**: Create Macro.cs, MacroRow.cs, MacroBook.cs, MacroFileManager.cs, MacroValidator.cs, AutoTranslateEncoder.cs. Refactor MainForm to use new classes. Move utility functions that are only called from one class into that class as private methods.
- **Deliverables**: Models/, Services/, Utilities/ folder structure. MainForm reduced to UI-only concerns.
- **Testing Gate**: Manual run — all existing functionality preserved
- **Entry Criteria**: Unit 1 complete and tested
- **Exit Criteria**: Builds clean, runs identically, no business logic in MainForm

### Unit 3: Step 0c — VB.NET Idiom Replacement
- **Purpose**: Replace all Microsoft.VisualBasic idioms with native C#
- **Scope**: Replace Conversions, Operators, NewLateBinding, Interaction, Strings across all files. Remove Microsoft.VisualBasic reference. Clean up Resizer.cs VB idioms.
- **Deliverables**: Idiomatic C# throughout. Regression test documentation.
- **Testing Gate**: Manual regression test using documented scenarios
- **Entry Criteria**: Unit 2 complete and tested
- **Exit Criteria**: Builds clean, no Microsoft.VisualBasic reference, passes all regression tests

### Unit 4: Step 1 — config.json Support
- **Purpose**: Add EditorConfig class and config.json file handling
- **Scope**: Create EditorConfig.cs. Modify file open dialog to show .json + .ttl. Auto-detect config.json when .ttl opened. Save config.json alongside macros.
- **Deliverables**: EditorConfig.cs, modified File_Open_Click, modified File_SaveAll_Click
- **Testing Gate**: Manual run — open via .ttl (should auto-detect config.json), open via .json, save creates config.json
- **Entry Criteria**: Unit 3 complete and tested
- **Exit Criteria**: Both open paths work, save produces valid config.json

### Unit 5: Step 4 — Template Variables
- **Purpose**: Add variable substitution system for portable macros
- **Scope**: Create VariableSubstitutionEngine.cs. Add variables to EditorConfig. On load: replace variable values with {placeholders}. On save: reverse substitution. Highlight {placeholders} in distinct color. Prompt for undefined variables on save. Allow user-created variables.
- **Deliverables**: VariableSubstitutionEngine.cs, UI highlighting, save-time prompting
- **Testing Gate**: Manual run — load macros with variable values, verify placeholders shown in color, save reverses correctly, new variables prompt on save
- **Entry Criteria**: Unit 4 complete and tested
- **Exit Criteria**: Variable substitution works bidirectionally, highlighting visible, undefined vars prompted

### Unit 6: Step 5 — Export with Variable Substitution
- **Purpose**: Export macros to another folder with per-destination variable values
- **Scope**: Add Export menu item. On export: check destination for config.json, read destination variables or prompt to create. Clone macros, apply destination substitution, write to destination.
- **Deliverables**: Export UI, destination config handling, substitution during export
- **Testing Gate**: Manual run — export to folder with existing config.json, export to folder without one
- **Entry Criteria**: Unit 5 complete and tested
- **Exit Criteria**: Export produces valid .dat files with destination variable values

### Unit 7: Step 2 — 40 Macro Set Support (BLOCKED)
- **Purpose**: Expand from 20 to 40 macro books, parse new file formats
- **Scope**: TBD after sample file analysis (mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, numbered dats)
- **Deliverables**: TBD
- **Testing Gate**: Manual run with both old 20-book and new 40-book file sets
- **Entry Criteria**: Unit 4 complete + sample files provided and analyzed
- **Exit Criteria**: TBD
- **Status**: BLOCKED — awaiting sample files

### Unit 8: Step 3 — Scrollbar UI
- **Purpose**: Add scrollbar to book list for 40 macro sets
- **Scope**: Modify Contents ListBox height/scrollbar behavior to accommodate 40 items
- **Deliverables**: Scrollable book list, possibly adjusted form layout
- **Testing Gate**: Manual run — verify all 40 books visible and selectable via scrolling
- **Entry Criteria**: Unit 7 complete and tested
- **Exit Criteria**: All macro sets visible and selectable

### Unit 9: Undo/Redo System
- **Purpose**: Add undo/redo stack with Ctrl+Z / Ctrl+Shift+Z support
- **Scope**: Create UndoRedoManager.cs. Track text edits, macro/row/book operations. Add Undo/Redo buttons to UI. Wire keyboard shortcuts.
- **Deliverables**: UndoRedoManager.cs, UI buttons, keyboard handling
- **Testing Gate**: Manual run — edit text, undo, redo, verify state changes correctly
- **Entry Criteria**: Unit 2 complete (needs clean data model in place)
- **Exit Criteria**: Undo/Redo works for text edits and bulk operations, buttons reflect state

### Unit 10: Editable Macro Map
- **Purpose**: Add a second, editable macro map that allows side-by-side editing of macros within a book
- **Scope**: Preserve existing read-only MacroMapForm. Add "Edit" button at top of read-only map to transfer to editable map. Create new editable macro map form with save button at top. Add "Edit in Macro Map" to book right-click context menu (below existing "Macro Map" option). Editable map allows direct text editing of macro titles and lines in the grid layout.
- **Deliverables**: EditableMacroMapForm.cs, modified MacroMapForm (Edit button), modified MenuBook context menu
- **Testing Gate**: Manual run — open macro map, click Edit to transfer to editable version, edit macros side-by-side, save, verify changes reflected in main editor
- **Entry Criteria**: Unit 2 complete (needs clean data model)
- **Exit Criteria**: Editable macro map opens, allows editing, saves correctly back to data model

## Code Organization (Target)

```text
Macro Editor/
├── Models/
│   ├── Macro.cs
│   ├── MacroRow.cs
│   ├── MacroBook.cs
│   ├── ValidationResult.cs
│   ├── UndoableAction.cs
│   └── EditorConfig.cs
├── Services/
│   ├── MacroFileManager.cs
│   ├── AutoTranslateEncoder.cs
│   ├── MacroValidator.cs
│   ├── VariableSubstitutionEngine.cs
│   └── UndoRedoManager.cs
├── Utilities/
│   ├── MacroEditorUtils.cs
│   └── Resizer.cs
├── Forms/
│   ├── MainForm.cs + .Designer.cs
│   ├── Assessment.cs + .Designer.cs
│   ├── Destination.cs + .Designer.cs
│   ├── MacroMapForm.cs + .Designer.cs
│   └── Help.cs + .Designer.cs
├── My/ (application framework)
└── Properties/
```
