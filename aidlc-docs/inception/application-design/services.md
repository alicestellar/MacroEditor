# Service Orchestration

## Overview

There is no separate "service layer" in the traditional sense — this is a desktop WinForms application, not a multi-tier service architecture. Instead, **MainForm acts as the orchestrator** that coordinates between components. This document defines how MainForm (as the thin UI shell) coordinates component interactions.

## Orchestration Patterns

### File Open Flow

```
User clicks File > Open
    → MainForm shows OpenFileDialog (filter: .json + .ttl)
    → IF .ttl selected:
        → EditorConfig.Load(same folder) — check for config.json
        → MacroFileManager.LoadFromDirectory(folder)
    → IF .json selected:
        → EditorConfig.Load(json path)
        → MacroFileManager.LoadFromDirectory(config.McrTtlPath folder)
    → VariableSubstitutionEngine.ApplyDisplaySubstitution(books, config.Variables)
    → MainForm displays books in Contents ListBox
    → UndoRedoManager.Clear()
```

### File Save Flow

```
User clicks File > Save All
    → VariableSubstitutionEngine.ApplySaveSubstitution(books, config.Variables)
    → MacroFileManager.SaveAll(macroPath, books)
    → EditorConfig.Save(macroPath + "config.json")
    → VariableSubstitutionEngine.ApplyDisplaySubstitution(books, config.Variables)
        (re-apply display substitution after save)
    → Update MacroPreserved (parallel backup list)
```

### Export Flow

```
User clicks Export
    → MainForm shows FolderBrowserDialog for destination
    → IF config.json exists in destination:
        → destConfig = EditorConfig.Load(destination)
    → ELSE:
        → Prompt user for variable values for destination
        → destConfig = EditorConfig.CreateDefault(...) with user-supplied variables
    → Clone current books
    → VariableSubstitutionEngine.ApplyExportSubstitution(clonedBooks, sourceVars, destVars)
    → MacroFileManager.SaveAll(destination, clonedBooks)
    → destConfig.Save(destination + "config.json")
```

### Edit Flow (Text Change)

```
User edits a TextBox
    → UndoRedoManager.RecordChange(previous value, new value, location)
    → Update data model: books[xBook].Rows[xRow].Macros[xMacro].Lines[lineIndex] = newText
    → MainForm.SomethingEdited = true
```

### Undo/Redo Flow

```
User presses Ctrl+Z (or clicks Undo button)
    → UndoRedoManager.Undo()
    → Apply reversed change to data model
    → Refresh UI display for affected macro

User presses Ctrl+Shift+Z (or clicks Redo button)
    → UndoRedoManager.Redo()
    → Apply forward change to data model
    → Refresh UI display for affected macro
```

### Validate Flow

```
User clicks Evaluate menu
    → MacroValidator.ValidateAll(books, autoTranslateEncoder)
    → Assessment form displays results
    → User clicks result → MainForm.FindMacro(book, row, macro)
```

### Revert Flow

```
User right-clicks > Revert (book, row, or macro level)
    → Copy from preserved list back to working list at appropriate level
    → Refresh UI display
    → UndoRedoManager.Clear() — revert is not undoable
```

### Variable Save with Undefined Variables

```
User clicks Save (and macros contain {newvar} not in config.Variables)
    → VariableSubstitutionEngine.FindUndefinedVariables(text, variables)
    → IF undefined variables found:
        → Prompt user: "Variable {newvar} is not defined. Enter the value:"
        → Add to config.Variables
    → Continue with normal save flow
```

## Component Lifecycle

| Component | Created | Lifespan |
|-----------|---------|----------|
| MacroFileManager | On app start | Application lifetime |
| AutoTranslateEncoder | On app start (loads phrases) | Application lifetime |
| MacroValidator | On demand (Evaluate click) | Per-validation |
| EditorConfig | On file open | Until next file open |
| VariableSubstitutionEngine | On file open (after config loaded) | Until next file open |
| UndoRedoManager | On file open | Until next file open (cleared on open) |
| Data model (List of MacroBook) | On file open | Until next file open |
| Preserved backup (List of MacroBook) | On file open / after save | Until next file open |
