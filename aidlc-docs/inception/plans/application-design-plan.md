# Application Design Plan

## Plan Overview

This plan defines the new layered architecture for the MacroEditor refactoring. We are decomposing the MainForm God Object into well-defined components with clear responsibilities.

---

## Design Execution Steps

- [x] Define data model components (Macro, MacroRow, MacroBook)
- [x] Define file I/O component (MacroFileManager)
- [x] Define auto-translate encoding component (AutoTranslateEncoder)
- [x] Define validation component (MacroValidator)
- [x] Define variable substitution component (VariableSubstitutionEngine)
- [x] Define configuration component (EditorConfig)
- [x] Define component interfaces and method signatures
- [x] Define service orchestration (how MainForm coordinates components)
- [x] Define component dependencies and data flow
- [x] Validate design completeness and consistency

---

## Design Questions

Please answer the following questions by filling in after the [Answer]: tags.

## Question 1
For the AutoTranslateEncoder — this component handles the `<HEX|PhraseName>` ↔ binary `0xFD` encoding. It currently also manages the AT phrase lookup dictionary and the context menu for AT phrase insertion. Should the UI menu part stay in MainForm, with only the encode/decode logic extracted?

A) Yes — AutoTranslateEncoder handles only encode/decode/dictionary lookup. The AT context menu stays in MainForm as UI concern.

B) No — extract the full AT system including menu building into a self-contained component that MainForm just hosts.

C) Other (please describe after [Answer]: tag below)

[Answer]: B

## Question 2
For the Helper class (Step 0a deduplication) — should it be a static utility class, or an instance class that gets injected into other components?

A) Static utility class (simple, no state, just shared methods like `fill()`, `KillZero()`, `TenToZero()`)

B) Instance class with dependency injection (more testable, follows DI patterns)

C) Other (please describe after [Answer]: tag below)

[Answer]: C
If we need to use an instance class (because some of the functions can't work as static) then we use an instance class. If we CAN use a static utility class, go ahead and use a static utility class. Everything that is only used in one of the final class files should be moved to that specific class file as a local function and removed from the utility class.

## Question 3
The Resizer class currently works fine as a standalone utility. Should it remain as-is, or should it be adapted to work with the new architecture?

A) Leave Resizer as-is — it works, it's already separated, don't touch it

B) Minor cleanup (remove VB.NET idioms from it during Step 0c) but keep its structure

C) Other (please describe after [Answer]: tag below)

[Answer]: B

## Question 4
For the VariableSubstitutionEngine — when displaying macros with variables substituted (e.g., "Makaria" → "{user}"), should the UI visually distinguish variable placeholders from regular text (e.g., different color, bold, etc.)?

A) Yes — highlight {placeholders} in a distinct color so the user knows which parts are variables

B) No — just show {user} as plain text, the curly braces are enough visual distinction

C) Other (please describe after [Answer]: tag below)

[Answer]: A
Also please note that the user should be able to create their own variables. If there is a variable that is not in the config file when we go to save, then the user should be prompted to supply the string that gets replaced by the variable in brackets.

## Question 5
The current code stores both `MacroContainer` (working copy) and `MacroPreserved` (original for revert). In the new data model, how should the revert/backup system work?

A) Each MacroBook stores its own backup copy internally (clone on load, revert restores from internal backup)

B) Keep a separate parallel `List<MacroBook>` for the preserved state (same pattern, new types)

C) Use an undo/redo stack instead of simple backup/revert

D) Other (please describe after [Answer]: tag below)

[Answer]: D
I do want an undo/redo stack, but it should be a feature that isn't about reverting. We will need to add buttons to the UI for this, as well as ctrl+z to undo, and ctrl+shift+z to redo. Reverting should be done the way it is described in B.

