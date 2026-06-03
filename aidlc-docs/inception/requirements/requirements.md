# Requirements Document: MacroEditor Refactoring & Enhancement

## Intent Analysis

- **User Request**: Refactor monolithic MacroEditor into proper OOP architecture, then enhance to support 40 macro sets, scrollable book list, updated file parser, and config.json support
- **Request Type**: Refactoring + Enhancement (phased)
- **Scope Estimate**: System-wide (touches all components)
- **Complexity Estimate**: Complex — multi-phase refactoring with behavioral preservation requirements

## Phasing Strategy

The work is explicitly phased with testing gates between each phase:

| Phase | Description | Gate |
|-------|-------------|------|
| Baseline | Verify decompiled code compiles and runs at all | Build + launch confirmation |
| Step 0a | Extract duplicate code to helper class | Manual test run |
| Step 0b | Full MVC separation (data model, file I/O, validation, UI) | Manual test run |
| Step 0c | Replace VB.NET idioms with idiomatic C# | Manual test run + regression docs |
| Step 1 | config.json support + file open improvements | Manual test run |
| Step 2 | 40 macro set support (BLOCKED — awaiting sample files) | Manual test run |
| Step 3 | Scrollbar UI improvements | Manual test run |
| Step 4 | Template variables (load/save substitution) | Manual test run |
| Step 5 | Export with variable substitution | Manual test run |

**CRITICAL RULE**: No phase may begin until the previous phase passes manual testing. Each phase must start from a clean commit.

**BASELINE REQUIREMENT**: The decompiled source has never been verified to compile or run. Before ANY code changes begin, we must:
1. Attempt to build the project as-is
2. Fix any compilation errors in the decompiled code (without changing behavior)
3. Confirm the application launches and can open/display macro files
4. Commit this verified baseline as the starting point for all subsequent work

---

## Functional Requirements

### FR-1: Code Deduplication (Step 0a)

- **FR-1.1**: Identify all duplicated code patterns across MainForm.cs
- **FR-1.2**: Extract duplicated logic into a shared helper class
- **FR-1.3**: All existing functionality must be preserved — no behavioral changes
- **FR-1.4**: Resulting code must compile and run identically to the original

### FR-2: Architecture Separation (Step 0b)

- **FR-2.1**: Create a typed data model:
  - `Macro` class: Title (string, max 8 chars) + Lines (List of 6 strings, max 60 encoded chars each)
  - `MacroRow` class: List of Macros (20 per row — 10 Ctrl + 10 Alt)
  - `MacroBook` class: Name (string, max 15 chars) + List of MacroRows (10 per book)
  - Top-level: `List<MacroBook>` replacing `string[20, 10, 20][]`
- **FR-2.2**: Create a file I/O class responsible for:
  - Reading mcr.ttl (book names)
  - Reading mcrXY.dat files (macro data)
  - Writing mcr.ttl
  - Writing mcrXY.dat files
  - MD5 checksum generation
- **FR-2.3**: Create a validation class responsible for:
  - Macro title length validation
  - Macro line length validation (post-AT-encode)
  - Character set validation
  - Comment detection
- **FR-2.4**: MainForm handles ONLY UI concerns:
  - Control creation and layout
  - Event handling
  - Display/render of macro data
  - User interaction (dialogs, status bar, menus)
- **FR-2.5**: Replace all fixed-size arrays with `List<T>` or typed collections
- **FR-2.6**: If helper class functions are only used by one of the new classes, move them to that class
- **FR-2.7**: Preserve backup/revert system (currently MacroPreserved) using the new data model

### FR-3: VB.NET Idiom Replacement (Step 0c)

- **FR-3.1**: Replace all `Conversions.ToString()` with `.ToString()` or string interpolation
- **FR-3.2**: Replace all `Operators.CompareString()` with `string.Equals()` or `==`
- **FR-3.3**: Replace all `NewLateBinding.LateGet/LateSet` with direct typed property access
- **FR-3.4**: Replace all `Interaction.MsgBox()` with `MessageBox.Show()`
- **FR-3.5**: Replace all `Strings.Split/Join/Chr` with native C# equivalents
- **FR-3.6**: Replace all `Operators.ConcatenateObject` with string concatenation or interpolation
- **FR-3.7**: Remove the `Microsoft.VisualBasic` assembly reference from the project
- **FR-3.8**: This step is isolated — commit clean before starting, full test before continuing
- **FR-3.9**: Produce regression test documentation listing all scenarios to manually verify

### FR-4: config.json Support (Step 1)

- **FR-4.1**: Define a config.json schema containing:
  - `mcrTtlPath` (string): relative or absolute path to the mcr.ttl file
  - `macroSetCount` (integer): number of macro sets (default: current detected count)
  - `schemaVersion` (integer): starts at 1, incremented on breaking format changes
  - `settings` (object): empty extensible object for future features
- **FR-4.2**: config.json is saved in the same folder as the macros when the user executes Save
- **FR-4.3**: config.json must always be backward compatible — older versions of the file must still load in newer versions of the editor
- **FR-4.4**: When opening a file:
  - Open dialog shows both .json and .ttl files (filter shows both, hides other file types)
  - If a .ttl file is selected, check the same folder for config.json and auto-load it
  - If config.json is selected directly, read it and load the referenced mcr.ttl
- **FR-4.5**: When saving: always write/update config.json alongside macro files

### FR-5: 40 Macro Set Support (Step 2 — BLOCKED)

- **FR-5.1**: BLOCKED — requires sample files (mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, numbered mcr.dat) from user for format analysis
- **FR-5.2**: Parser must handle both old (20-book) and new (40-book) file structures
- **FR-5.3**: Not all mcrXXX.dat files are guaranteed to exist — handle gracefully
- **FR-5.4**: New file format includes: mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, mcr1.dat through mcr399.dat
- **FR-5.5**: Specific parsing behavior TBD after file analysis

### FR-6: Scrollbar UI Enhancement (Step 3)

- **FR-6.1**: The Contents ListBox (book list) must have a visible scrollbar when items exceed visible area
- **FR-6.2**: User must be able to scroll through all macro sets (up to 40) and select them
- **FR-6.3**: ListBox height may need adjustment or the form layout may need to accommodate more items

### FR-7: Template Variables (Step 4)

- **FR-7.1**: config.json gains a `variables` object that maps placeholder names to values (e.g., `{ "user": "Makaria", "server": "Asura" }`)
- **FR-7.2**: On load, after macros are read from .dat files, scan all editable text (macro titles and lines) for variable values and replace them with `{placeholder}` syntax in the UI (e.g., "Makaria" → "{user}")
- **FR-7.3**: On save, replace all `{placeholder}` occurrences in macro text with the corresponding variable values from config.json before writing to .dat files
- **FR-7.4**: Variable substitution is display-only — the .dat files always contain the real values, never placeholders
- **FR-7.5**: Variable names use `{name}` syntax in the UI (curly braces)
- **FR-7.6**: If a variable value appears in macro text but the user has edited it to something else, that edit is preserved (substitution only replaces exact matches)
- **FR-7.7**: Variables are case-sensitive for matching

### FR-8: Export with Variable Substitution (Step 5)

- **FR-8.1**: Add an Export function that saves macros to a user-selected destination folder
- **FR-8.2**: When exporting, check the destination folder for an existing config.json
- **FR-8.3**: If config.json exists in the destination, read its `variables` object and substitute the destination's variable values into the macro text before writing
- **FR-8.4**: If config.json does not exist in the destination, create one during export — prompt the user to define variable values for the destination (e.g., "What is {user} for this folder?")
- **FR-8.5**: The exported .dat files contain the destination's variable values (not placeholders, not the source's values)
- **FR-8.6**: The source macros remain unchanged after export

---

## Non-Functional Requirements

### NFR-1: Behavioral Preservation
- All refactoring steps must maintain identical external behavior
- File format compatibility must be preserved — files written by the refactored editor must be readable by the FFXI client
- MD5 checksums must remain identical for unchanged data

### NFR-2: No Network Access
- The application must NEVER make network calls
- No telemetry, update checks, or data transmission of any kind
- Purely local file system operations only

### NFR-3: Target Framework
- Remain on .NET Framework 4.5.2
- Do not introduce dependencies that require a newer framework

### NFR-4: Testing Strategy
- Each phase produces a clean, committed state before the next begins
- Manual regression testing between each phase
- Regression test documentation produced with Step 0c (FR-3.9)

### NFR-5: Backward Compatibility
- config.json format must be forward-compatible (newer editor reads older configs)
- mcr.ttl/dat reading must continue to work with existing 20-book macro sets
- Existing clipboard sharing format must be preserved

---

## Constraints & Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Framework | .NET Framework 4.5.2 | Maintain existing build compatibility |
| Data model | Typed classes (MacroBook/Row/Macro) | Readable, extensible, type-safe |
| VB.NET idioms | Full removal | Clean codebase for future development |
| Architecture | Full MVC separation (via helper class first) | Reduce risk through incremental refactoring |
| Security | No network access | Local-only desktop tool |
| PBT | Skipped | UI-focused desktop app with manual testing |
| 40 macro sets | BLOCKED | Pending sample file analysis |

---

## Open Items

1. **Sample file analysis** — User will provide mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, and numbered mcr.dat files for format reverse engineering before Step 2 can begin
2. **Exact config.json schema** — Final schema will be confirmed during Step 1 implementation
3. **Regression test scenarios** — Will be documented during Step 0c
