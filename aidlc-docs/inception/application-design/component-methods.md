# Component Methods

## Macro

```csharp
public class Macro
{
    // Properties
    string Title { get; set; }          // Max 8 characters
    List<string> Lines { get; set; }    // 6 lines, max 60 encoded chars each

    // Methods
    Macro Clone()                        // Deep copy for backup/revert
    static Macro CreateEmpty()           // Factory: empty macro with "" title and 6 empty lines
    bool IsEmpty()                       // True if title and all lines are empty
}
```

## MacroRow

```csharp
public class MacroRow
{
    // Properties
    List<Macro> Macros { get; set; }    // 20 macros (indices 0-9 Ctrl, 10-19 Alt)

    // Methods
    MacroRow Clone()                     // Deep copy all macros
    static MacroRow CreateEmpty()        // Factory: 20 empty macros
    List<Macro> GetCtrlMacros()          // Macros[0..9]
    List<Macro> GetAltMacros()           // Macros[10..19]
}
```

## MacroBook

```csharp
public class MacroBook
{
    // Properties
    string Name { get; set; }           // Max 15 characters
    List<MacroRow> Rows { get; set; }   // 10 rows

    // Methods
    MacroBook Clone()                    // Deep copy name + all rows
    static MacroBook CreateEmpty(string name)  // Factory: named book with 10 empty rows
}
```

## MacroFileManager

```csharp
public class MacroFileManager
{
    // Reading
    List<MacroBook> LoadFromDirectory(string macroPath)
        // Reads mcr.ttl + all mcrXY.dat files, returns full macro set

    List<string> ReadBookNames(string ttlFilePath)
        // Parse mcr.ttl, return list of book names

    MacroRow ReadMacroRow(string datFilePath)
        // Parse a single mcrXY.dat file into a MacroRow

    // Writing
    bool SaveAll(string macroPath, List<MacroBook> books)
        // Write all .dat files + mcr.ttl

    bool WriteRow(string macroPath, int bookIndex, int rowIndex, MacroRow row)
        // Write a single mcrXY.dat file

    bool WriteBookNames(string macroPath, List<MacroBook> books)
        // Write mcr.ttl

    // Utilities
    string GetDatFileName(int bookIndex, int rowIndex)
        // Generate "mcr" + KillZero(book, row) + ".dat"

    byte[] ComputeMD5(byte[] data)
        // MD5 checksum for file integrity
}
```

## AutoTranslateEncoder

```csharp
public class AutoTranslateEncoder
{
    // Initialization
    void LoadPhrases()
        // Load AT phrases from FFXIEncoding, build dictionary + menu

    // Encoding/Decoding
    string Decode(string rawText)
        // Replace binary 0xFD markers with <HEX|Name> readable format

    string Encode(string displayText)
        // Replace <HEX|Name> format back to binary 0xFD format for saving

    // Dictionary Lookup
    string LookupPhrase(string hexCode)
        // Get phrase name from hex code, or "UnknownItem"

    // Menu
    ToolStripMenuItem GetATMenu()
        // Return the built AT phrase menu for MainForm to host

    void FilterMenu(string searchText)
        // Show/hide menu items matching search text
}
```

## MacroValidator

```csharp
public class MacroValidator
{
    // Validation
    List<ValidationResult> ValidateAll(List<MacroBook> books, AutoTranslateEncoder encoder)
        // Validate all macros, return errors and warnings

    List<ValidationResult> ValidateMacro(Macro macro, int book, int row, int macroIndex, AutoTranslateEncoder encoder)
        // Validate a single macro
}

public class ValidationResult
{
    int BookIndex { get; set; }
    int RowIndex { get; set; }
    int MacroIndex { get; set; }
    int LineIndex { get; set; }     // 0 = title, 1-6 = lines
    ValidationSeverity Severity { get; set; }  // Error or Warning
    string Message { get; set; }
    string Content { get; set; }    // The offending text
}

public enum ValidationSeverity { Error, Warning }
```

## EditorConfig

```csharp
public class EditorConfig
{
    // Properties
    string McrTtlPath { get; set; }
    int MacroSetCount { get; set; }
    int SchemaVersion { get; set; }
    Dictionary<string, string> Variables { get; set; }
    Dictionary<string, object> Settings { get; set; }  // Extensible

    // Methods
    static EditorConfig Load(string configPath)
        // Read config.json, return parsed config

    void Save(string configPath)
        // Write config.json

    static EditorConfig CreateDefault(string mcrTtlPath, int macroSetCount)
        // Factory: default config with given path and count
}
```

## VariableSubstitutionEngine

```csharp
public class VariableSubstitutionEngine
{
    // Core Operations
    string SubstituteForDisplay(string text, Dictionary<string, string> variables)
        // Replace variable values with {placeholder} for UI display

    string SubstituteForSave(string text, Dictionary<string, string> variables)
        // Replace {placeholder} with variable values for .dat writing

    List<string> FindUndefinedVariables(string text, Dictionary<string, string> variables)
        // Find {name} tokens in text that aren't in the variables dict

    // Batch Operations
    void ApplyDisplaySubstitution(List<MacroBook> books, Dictionary<string, string> variables)
        // Apply substitution across all loaded macros for display

    void ApplySaveSubstitution(List<MacroBook> books, Dictionary<string, string> variables)
        // Reverse substitution across all macros before save

    // Export
    void ApplyExportSubstitution(List<MacroBook> books, Dictionary<string, string> sourceVars, Dictionary<string, string> destVars)
        // Replace source placeholders with destination values for export
}
```

## UndoRedoManager

```csharp
public class UndoRedoManager
{
    // State
    bool CanUndo { get; }
    bool CanRedo { get; }

    // Operations
    void RecordChange(UndoableAction action)
        // Push action onto undo stack, clear redo stack

    void Undo()
        // Pop undo stack, apply reverse, push to redo stack

    void Redo()
        // Pop redo stack, apply forward, push to undo stack

    void Clear()
        // Reset both stacks (e.g., on file load)
}

public class UndoableAction
{
    int BookIndex { get; set; }
    int RowIndex { get; set; }
    int MacroIndex { get; set; }
    int LineIndex { get; set; }     // -1 for full macro operations
    string PreviousValue { get; set; }
    string NewValue { get; set; }
    UndoActionType Type { get; set; }
}

public enum UndoActionType { TextEdit, MacroClear, MacroPaste, RowClear, RowPaste, BookClear, BookPaste }
```

## MacroEditorUtils (Static Utility)

```csharp
public static class MacroEditorUtils
{
    static string Fill(string str, int length)
        // Pad string with null characters to specified length

    // Additional shared utilities discovered during deduplication
    // MEMBERSHIP RULE: A function belongs here ONLY if it is called from
    // multiple different classes. If all callers reside in a single class
    // (even if called many times), move the function into that class as
    // a private method.
}
```

**Note**: Detailed business rules (exact byte offsets, encoding edge cases, validation thresholds) will be specified during Functional Design in the CONSTRUCTION phase.
