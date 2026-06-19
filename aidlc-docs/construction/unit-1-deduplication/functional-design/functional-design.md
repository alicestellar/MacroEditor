# Functional Design: Unit 1 — Code Deduplication

## Overview

This unit identifies duplicated code patterns in MainForm.cs and extracts them to a `MacroEditorUtils` static class. The goal is to reduce repetition before the larger MVC separation in Unit 2.

## Identified Duplicate Patterns

Based on reverse engineering analysis, these patterns are repeated:

### 1. String Padding (`fill`)
- **Current**: `this.fill(str, length, "\0")` — pads a string with null characters
- **Usage**: WriteFile (6 lines per macro, title padding), MenuBook_SaveBookNames_Click (book names)
- **Target**: `MacroEditorUtils.Fill(string str, int targetLength)`

### 2. File Name Generation (`KillZero`)
- **Current**: `this.KillZero(book, row)` — generates numeric suffix for .dat filenames
- **Usage**: WriteFile, File_Open_Click, MenuRow_Opening (copy location), MenuRow_Save_Click
- **Target**: `MacroEditorUtils.GetMacroFileSuffix(int book, int row)`

### 3. Index Display Conversion (`TenToZero`)
- **Current**: `this.TenToZero(num)` — converts 10 to "0" for display
- **Usage**: Form1_Load (button creation), Row_Click, Lines_TextChanged, Control_Click
- **Target**: `MacroEditorUtils.FormatMacroIndex(int index)`

### 4. Macro Label Formatting (`AsMacro`)
- **Current**: `this.AsMacro(m)` — returns "Ctl X" or "Alt X" from macro index
- **Usage**: Search results display, potentially other locations
- **Target**: `MacroEditorUtils.FormatMacroLabel(int macroIndex)`

### 5. Status Bar Update (`UpdateStatusBar`)
- **Current**: `this.UpdateStatusBar(header, detail)` — updates status bar labels
- **Usage**: File_Open_Click (extracting), File_SaveAll_Click (writing), other locations
- **Decision**: Stays in MainForm (UI concern, updates form controls directly)

### 6. Clipboard Verification (`verifyclipboard`)
- **Current**: `this.verifyclipboard(pasteType, array)` — validates clipboard format
- **Usage**: MenuBook_PasteClipboard_Click, MenuRow_PasteClipboard_Click, MenuHandler_PasteSide_Click
- **Target**: `MacroEditorUtils.VerifyClipboardFormat(string expectedType, Array data)`

### 7. Clipboard Cleaning (`CleanClipBoard`)
- **Current**: `this.CleanClipBoard()` — gets clipboard text and normalizes line endings
- **Usage**: Multiple paste operations, ProcessCmdKey
- **Decision**: Stays in MainForm (accesses Clipboard which is UI-thread-bound, shows MsgBox on failure)

## Extraction Rules

1. A function goes into `MacroEditorUtils` ONLY if called from multiple places across what will become multiple classes after Unit 2
2. Functions that will end up being called only from MainForm (UI concerns) stay in MainForm
3. Functions that will end up being called only from MacroFileManager stay — but we extract them now and relocate later in Unit 2

## Functions to Extract

| Function | New Name | Rationale |
|----------|----------|-----------|
| `fill(str, rpad, padder)` | `MacroEditorUtils.Fill(str, length)` | Used by file writing (future MacroFileManager) AND book name saving (same) — but called from MainForm in multiple save contexts |
| `KillZero(book, row)` | `MacroEditorUtils.GetMacroFileSuffix(book, row)` | Used by file read, file write, UI display of file locations — crosses file I/O and UI boundaries |
| `TenToZero(ten)` | `MacroEditorUtils.FormatMacroIndex(index)` | Used across UI button creation and display — potentially UI-only after refactoring. Extract now, may move to private in Unit 2 |
| `AsMacro(m)` | `MacroEditorUtils.FormatMacroLabel(macroIndex)` | Used in search/evaluation display — may end up UI-only. Extract now, evaluate in Unit 2 |
| `verifyclipboard(type, array)` | `MacroEditorUtils.VerifyClipboardFormat(type, data)` | Used across multiple paste operations — clipboard format is not UI-specific |

## Functions Staying in MainForm

| Function | Reason |
|----------|--------|
| `UpdateStatusBar` | Directly manipulates form controls |
| `CleanClipBoard` | Uses System.Windows.Forms.Clipboard + shows MsgBox |
| `FindMacro` | Navigates UI controls |
| `OpenATMenu` | Manipulates UI menu visibility |
| `GetFFXIDirectory` | Used only in constructor, will move to MacroFileManager in Unit 2 |
| `FindFFXIDirectory` | Shows FolderBrowserDialog (UI) |

## Implementation Plan

1. Create `MacroEditorUtils.cs` in project root (will move to Utilities/ folder in Unit 2)
2. Add the 5 static methods listed above
3. Replace all call sites in MainForm.cs with `MacroEditorUtils.Method(...)` calls
4. Remove the instance methods from MainForm
5. Build and verify
