# System Architecture

## System Overview

The FFXI Macro Editor is a single-process Windows Forms desktop application built on .NET Framework 4.5.2. It follows a monolithic architecture with no network dependencies or external services. All data is stored in local binary files that conform to the FFXI game client's macro file format.

## Architecture Diagram

```mermaid
flowchart TD
    subgraph UI["UI Layer (WinForms)"]
        MainForm["MainForm<br/>Primary Window"]
        Assessment["Assessment<br/>Results Display"]
        Destination["Destination<br/>Macro Relocator"]
        MacroMap["MacroMapForm<br/>Visual Overview"]
        Help["Help<br/>Documentation"]
    end

    subgraph Logic["Business Logic (Embedded in MainForm)"]
        FileIO["File I/O<br/>Read/Write .dat/.ttl"]
        ATEngine["Auto-Translate<br/>Encode/Decode"]
        Validator["Macro Validator<br/>(Evaluate)"]
        Clipboard["Clipboard Manager<br/>Internal + System"]
    end

    subgraph External["External Dependencies"]
        FFXIEncoding["Yekyaa.FFXIEncoding.dll<br/>AT Phrase Loader"]
        FileSystem["File System<br/>mcr.ttl + mcrXY.dat"]
        Registry["Windows Registry<br/>FFXI Install Path"]
    end

    MainForm --> FileIO
    MainForm --> ATEngine
    MainForm --> Validator
    MainForm --> Clipboard
    MainForm --> Assessment
    MainForm --> Destination
    MainForm --> MacroMap
    ATEngine --> FFXIEncoding
    FileIO --> FileSystem
    MainForm --> Registry
```

## Component Descriptions

### MainForm
- **Purpose**: Central application controller and primary UI
- **Responsibilities**: All macro editing, file I/O, parsing, serialization, validation, clipboard, and navigation
- **Dependencies**: Yekyaa.FFXIEncoding, System.Windows.Forms, System.Security.Cryptography
- **Type**: Application (UI + Business Logic, tightly coupled)

### Assessment
- **Purpose**: Display macro evaluation results and search results
- **Responsibilities**: Show lists of validation issues and search matches with navigation
- **Dependencies**: MainForm (for navigation callbacks)
- **Type**: Application (UI)

### Destination
- **Purpose**: Macro redirection dialog
- **Responsibilities**: Allow user to select target book/row/macro for macro relocation
- **Dependencies**: MainForm (accesses MacroContainer directly)
- **Type**: Application (UI)

### MacroMapForm
- **Purpose**: Visual macro overview
- **Responsibilities**: Display full book contents as a scrollable grid of labels
- **Dependencies**: MainForm (accesses MacroContainer for display)
- **Type**: Application (UI)

### Resizer
- **Purpose**: Dynamic proportional resizing utility
- **Responsibilities**: Track and recalculate control positions/sizes on window resize
- **Dependencies**: None (standalone utility)
- **Type**: Utility

### Yekyaa.FFXIEncoding.dll
- **Purpose**: External library for loading FFXI auto-translate phrase data
- **Responsibilities**: Parse game data files and provide AT phrase lookup
- **Dependencies**: FFXI game data files
- **Type**: External Library

## Data Flow

```mermaid
sequenceDiagram
    participant User
    participant MainForm
    participant FileSystem
    participant FFXIEncoding

    Note over User,FFXIEncoding: Open Macro Set
    User->>MainForm: File > Open (select mcr.ttl)
    MainForm->>FileSystem: Read mcr.ttl (book names)
    MainForm->>MainForm: Parse book names (16 bytes each)
    loop For each book (0..debuglimit)
        loop For each row (0..9)
            MainForm->>FileSystem: Read mcrXY.dat
            MainForm->>MainForm: Parse 20 macros (380 bytes each)
        end
    end
    MainForm->>User: Display book list and first row

    Note over User,FFXIEncoding: Save All
    User->>MainForm: File > Save All
    loop For each book (0..19)
        loop For each row (0..9)
            MainForm->>MainForm: Serialize 20 macros to 7600 bytes
            MainForm->>MainForm: Compute MD5 checksum
            MainForm->>FileSystem: Write mcrXY.dat (8-byte header + 16-byte MD5 + data)
        end
    end
    MainForm->>FileSystem: Write mcr.ttl (8-byte header + 16-byte MD5 + 320 bytes of names)
```

## Integration Points

- **External APIs**: None (offline desktop application)
- **Databases**: None
- **Third-party Services**: None
- **File System**: FFXI macro directory (USER folder under FFXI installation)
- **Windows Registry**: PlayOnline registry keys for auto-detecting FFXI installation path
