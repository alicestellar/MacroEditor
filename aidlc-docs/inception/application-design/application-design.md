# Application Design Summary

## Architecture Overview

The MacroEditor is being refactored from a monolithic God Object (MainForm ~4600 lines) into a layered architecture with clear separation of concerns:

```
+--------------------------------------------------+
|                   UI Layer                        |
|  MainForm | Assessment | Destination | MacroMap  |
+--------------------------------------------------+
         |          |           |           |
+--------------------------------------------------+
|              Core Components                      |
|  MacroFileManager | AutoTranslateEncoder         |
|  MacroValidator   | VariableSubstitutionEngine   |
|  EditorConfig     | UndoRedoManager              |
+--------------------------------------------------+
         |          |           |           |
+--------------------------------------------------+
|               Data Model                          |
|       MacroBook > MacroRow > Macro               |
+--------------------------------------------------+
         |
+--------------------------------------------------+
|               Utility                             |
|       MacroEditorUtils | Resizer                 |
+--------------------------------------------------+
```

## Key Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Data model | Typed classes with Lists | Type-safe, extensible, readable |
| Utility class | Static, multi-caller only | Simple, no state; private if single-class callers |
| AT system | Full extraction including menu | Self-contained component, MainForm just hosts the menu |
| Revert system | Parallel `List<MacroBook>` backup | Preserves familiar behavior |
| Undo/Redo | Separate stack-based system | New feature, orthogonal to revert |
| Variable highlighting | Distinct color in UI | Clarity for user about which parts are templates |
| User-created variables | Prompt on save for undefined | Flexible, user-driven |
| Sub-form coupling | Data model + callback, no MainForm reference | Breaks circular dependencies |

## Component Count

| Layer | Components |
|-------|-----------|
| Data Model | 3 (Macro, MacroRow, MacroBook) |
| Core | 6 (FileManager, ATEncoder, Validator, VarEngine, Config, UndoRedo) |
| UI | 4 (MainForm, Assessment, Destination, MacroMapForm) |
| Utility | 2 (MacroEditorUtils, Resizer) |
| **Total** | **15** |

## File Organization (Target)

```
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
│   ├── MainForm.cs
│   ├── MainForm.Designer.cs
│   ├── Assessment.cs
│   ├── Assessment.Designer.cs
│   ├── Destination.cs
│   ├── Destination.Designer.cs
│   ├── MacroMapForm.cs
│   ├── MacroMapForm.Designer.cs
│   └── Help.cs
├── My/
│   └── (application framework files — unchanged)
└── Properties/
    └── AssemblyInfo.cs
```

## Design Artifacts Reference

- **[components.md](components.md)** — Component definitions and responsibilities
- **[component-methods.md](component-methods.md)** — Method signatures for each component
- **[services.md](services.md)** — Service orchestration and coordination flows
- **[component-dependency.md](component-dependency.md)** — Dependencies and data flow
