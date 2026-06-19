# Code Quality Assessment

## Test Coverage
- **Overall**: None — no automated tests exist
- **Unit Tests**: None
- **Integration Tests**: None

## Code Quality Indicators
- **Linting**: Not configured (no .editorconfig, no analyzers)
- **Code Style**: Inconsistent — mixture of VB.NET idioms in C# code (late binding, Operators class, Conversions class)
- **Documentation**: Poor — no XML doc comments, no inline documentation beyond token/RVA markers (suggests decompiled code)

## Technical Debt

### High Priority
- **God Object**: MainForm.cs contains ALL business logic (~4600 lines) — file I/O, parsing, serialization, validation, clipboard, UI, AT encoding are all in one class
- **Hardcoded Limits**: `debuglimit = 19` and array dimensions `[20, 10, 20]` hardcoded throughout — no constants or configuration
- **No Separation of Concerns**: Data model, file format handling, and UI are tightly coupled in MainForm
- **Decompiled Code**: The codebase appears to be decompiled from a VB.NET binary — contains token/RVA markers, uses Microsoft.VisualBasic late binding, and uses patterns not typical of hand-written C#

### Medium Priority
- **Bidirectional Dependencies**: Forms reference each other directly (Destination→MainForm, MacroMapForm→MainForm) creating circular coupling
- **No Error Handling Strategy**: Inconsistent try/catch blocks, some silently swallow exceptions
- **Magic Numbers**: File format constants (380, 7600, 61, 16, 8) scattered throughout without named constants
- **VB.NET Idioms in C#**: Heavy use of Microsoft.VisualBasic.CompilerServices (NewLateBinding, Operators, Conversions) instead of native C# patterns

### Low Priority
- **MD5 Usage**: Uses MD5CryptoServiceProvider (obsolete/weak) — though this is required by FFXI's file format
- **No Settings Validation**: User settings (UserDirectory) not validated for existence or permissions
- **Single-Threaded I/O**: All file operations are synchronous on the UI thread

## Patterns and Anti-patterns

### Good Patterns
- Backup/revert system (MacroPreserved mirrors MacroContainer)
- Consistent file format handling matching FFXI client expectations
- Resizer utility for responsive layout
- Context menus providing rich interaction at every level (book, row, macro, text)

### Anti-patterns
- **God Object** (MainForm) — all logic in one class
- **Decompiled Code** — not idiomatic C#, hard to maintain
- **Late Binding** — NewLateBinding.LateGet/LateSet used where direct property access would work
- **Circular References** — forms reference each other bidirectionally
- **No Constants** — magic numbers for file format layout
- **No Interfaces/Abstraction** — everything is concrete with no testable boundaries
