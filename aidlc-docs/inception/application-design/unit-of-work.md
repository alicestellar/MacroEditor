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

### ~~Unit 4: config.json Support~~ — ELIMINATED
- **Status**: Eliminated by design pivot (2026-06-04)
- **Reason**: Variable definitions are now stored directly in the macro file (Book 40) instead of a separate config.json. No external configuration file is needed.

### Unit 5: Template Variables (Redesigned)
- **Purpose**: Add variable substitution system using Book 40 as the variable definition store
- **Scope**: Create VariableSubstitutionEngine.cs. On file load, scan Book 40 backwards from Page 10 to extract variable definitions. On display: replace variable values with `{placeholder}` in Books 1-39. On save: reverse substitution (replace placeholders with values) for Books 1-39. Highlight `{placeholders}` in distinct color in the UI. Allow users to create new variables by editing Book 40 directly.
- **Variable Definition Format**:
  - Variables live in **Book 40**, scanned **backwards from Page 10**
  - Each macro slot defines one variable group:
    - **Title** = placeholder name (e.g., `user`, `server`)
    - **Line 1** = primary value → substitutes for `{name}` on save
    - **Line 2** = alt value → substitutes for `{name2}` on save
    - **Line 3** = alt value → substitutes for `{name3}` on save
    - **Line 4** = alt value → substitutes for `{name4}` on save
    - **Line 5** = alt value → substitutes for `{name5}` on save
    - **Line 6** = `VARIABLE` (marker — identifies this macro as a variable definition)
  - **Scanning rule**: Start at Page 10, check each macro for `VARIABLE` in line 6. Continue scanning backwards page by page. Stop at the first **complete page** (all 20 macros in the Ctrl+Alt row pair) that has **no** macros with the `VARIABLE` marker.
  - **Flexibility**: User can start defining variables at any macro slot on Page 10 (not required to start at a specific position). They can use as many pages as needed working backwards.
- **Deliverables**: VariableSubstitutionEngine.cs, UI highlighting for placeholders, Book 40 variable scanning logic
- **Testing Gate**: Manual run — define variables in Book 40, load macros, verify placeholders shown in color in Books 1-39, save and verify values written correctly
- **Entry Criteria**: Unit 3 complete and tested (needs clean codebase + 40-book support in place)
- **Exit Criteria**: Variable substitution works bidirectionally, highlighting visible, Book 40 scanning works correctly with partial pages

### Unit 6: Export with Variable Substitution (Redesigned)
- **Purpose**: Export macros to another folder with per-destination variable values read from the destination's Book 40
- **Scope**: Add Export menu item. On export: read the destination folder's macro files, scan destination's Book 40 for variable definitions using the same scanning rules. Clone current Books 1-39, apply destination variable values (replacing placeholders with destination-specific values), write to destination. If destination has no Book 40 variables defined, prompt user to create them.
- **Deliverables**: Export UI (folder picker + variable confirmation), destination Book 40 reading, substitution during export
- **Testing Gate**: Manual run — export to folder with existing Book 40 variables, export to folder without any variables defined
- **Entry Criteria**: Unit 5 complete and tested
- **Exit Criteria**: Export produces valid .dat files with destination-specific variable values substituted

### Unit 7: 40 Macro Set Support — COMPLETE
- **Purpose**: Expand from 20 to 40 macro books, parse new file formats
- **Scope**: Read/write mcr_2.ttl for books 21-40, increase debuglimit to 39, fix WriteRow macroCount bug, fix focus loss on startup
- **Deliverables**: Updated MacroFileManager with mcr_2.ttl support, 40-book capacity
- **Testing Gate**: Manual run with both old 20-book and new 40-book file sets
- **Entry Criteria**: Unit 3 complete + sample files analyzed
- **Exit Criteria**: All 40 books load/save correctly
- **Status**: COMPLETE — Committed 022ce15

### Unit 8: Scrollbar UI — COMPLETE
- **Purpose**: Scrollbar for book list to accommodate 40 macro sets
- **Scope**: No code changes needed — WinForms ListBox natively provides scrollbar when items exceed visible area
- **Status**: COMPLETE — native behavior, no implementation required

### Unit 9: Undo/Redo System
- **Purpose**: Add undo/redo stack with Ctrl+Z / Ctrl+Shift+Z support
- **Scope**: Create UndoRedoManager.cs. Track text edits, macro/row/book operations. Add Undo/Redo buttons to UI. Wire keyboard shortcuts.
- **Deliverables**: UndoRedoManager.cs, UI buttons, keyboard handling
- **Testing Gate**: Manual run — edit text, undo, redo, verify state changes correctly
- **Entry Criteria**: Unit 2 complete (needs clean data model in place)
- **Exit Criteria**: Undo/Redo works for text edits and bulk operations, buttons reflect state

### Unit 10: Macro Map Visual Enhancements
- **Purpose**: Improve macro map readability with color coding, page navigation, and dividers
- **Scope**:
  - Add a fixed header/toolbar area at the top of the macro map window containing CTRL/ALT labels and action buttons (shared with Unit 11's Edit/Save buttons)
  - Add divider lines between each "page" (row pair: Ctrl row + Alt row)
  - "CTRL" and "ALT" labels pinned in the header — always visible regardless of scroll position (CTRL = faded blue, ALT = faded red)
  - Color-code macro labels: Ctrl macros = faded blue background, Alt macros = faded red background (instead of grey)
  - Add page navigation buttons at top and bottom of scrollbar (move to next/prev page, center window)
  - Implement paged scrolling: scroll position tracks continuously (same mouse wheel delta units) but the view snaps to show one page at a time, only changing when scroll value crosses a page boundary
- **Deliverables**: Modified MacroMapForm with color coding, dividers, page labels, and paged scroll behavior
- **Testing Gate**: Manual run — verify color coding, dividers visible, page buttons navigate correctly, scroll snaps between pages
- **Entry Criteria**: Unit 2 complete (needs clean data model)
- **Exit Criteria**: Macro map shows colored Ctrl/Alt rows with dividers, paged scrolling works

### Unit 11: Editable Macro Map
- **Purpose**: Add a second, editable macro map that allows side-by-side editing of macros within a book
- **Scope**: Preserve existing read-only MacroMapForm. Add "Edit" button at top of read-only map to transfer to editable map. Create new editable macro map form with save button at top. Add "Edit in Macro Map" to book right-click context menu (below existing "Macro Map" option). Editable map allows direct text editing of macro titles and lines in the grid layout.
- **Deliverables**: EditableMacroMapForm.cs, modified MacroMapForm (Edit button), modified MenuBook context menu
- **Testing Gate**: Manual run — open macro map, click Edit to transfer to editable version, edit macros side-by-side, save, verify changes reflected in main editor
- **Entry Criteria**: Unit 10 complete (visual enhancements should be in place first)
- **Exit Criteria**: Editable macro map opens, allows editing, saves correctly back to data model

### Unit 12: Text File Import/Export for Macro Sets
- **Purpose**: Allow users to export a macro set (book) to a human-readable text file and import it back
- **Scope**: Add Export to Text and Import from Text options. The text format should be readable and editable by humans in a text editor. Export writes one book's macros to a .txt file. Import reads a .txt file and loads it into the current book (or a selected book).
- **Deliverables**: Export/Import menu items, text file format definition, MacroTextSerializer.cs
- **Testing Gate**: Manual run — export a book to text, edit the text file externally, import it back, verify macros loaded correctly
- **Entry Criteria**: Unit 2 complete (needs clean data model)
- **Exit Criteria**: Round-trip export→edit→import works, format is human-readable

### Unit 13: Compiler Warning Cleanup
- **Purpose**: Eliminate all compiler warnings for a clean build output
- **Scope**: Fix unused variable warnings (CS0168) in Resizer.cs, MacroFileManager.cs, MainForm.cs. Fix unassigned field warnings (CS0649) in Designer files. Address any other warnings introduced by later units.
- **Deliverables**: Zero-warning build
- **Testing Gate**: `dotnet build` produces 0 warnings
- **Entry Criteria**: All other units complete
- **Exit Criteria**: Clean build with no warnings
- **Priority**: Low — cosmetic cleanup, does not affect functionality

```text
Macro Editor/
├── Models/
│   ├── Macro.cs
│   ├── MacroRow.cs
│   ├── MacroBook.cs
│   ├── ValidationResult.cs
│   └── UndoableAction.cs
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
