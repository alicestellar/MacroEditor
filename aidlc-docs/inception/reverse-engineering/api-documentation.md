# API Documentation

## REST APIs

Not applicable — this is a standalone desktop application with no network APIs.

## Internal APIs

### MainForm - File Operations

#### ReadMacroFile(int mbook, int mset, string filename, bool PreserveMacros) : bool
- **Purpose**: Read and parse a single macro .dat file into MacroContainer
- **Parameters**:
  - `mbook`: Book index (0-based)
  - `mset`: Row index (0-based, 0-9)
  - `filename`: Full path to the .dat file
  - `PreserveMacros`: Whether to also copy into MacroPreserved for revert
- **Returns**: true on success
- **File Format**: 8-byte header + 16-byte MD5 + 7600 bytes data (20 macros x 380 bytes each)
- **Macro Format**: 4 bytes padding + 6 lines x 61 bytes + 9 bytes title + 1 byte null = 380 bytes

#### WriteFile(int Book, int Row) : bool
- **Purpose**: Serialize and write a single row of macros to its .dat file
- **Parameters**:
  - `Book`: Book index (0-based)
  - `Row`: Row index (0-based, 0-9)
- **Returns**: true on success, false on error
- **Output Format**: 8-byte header (preserved from original) + 16-byte MD5 + 7600 bytes serialized data
- **Validation**: Fails if serialized data is not exactly 7600 bytes

### MainForm - Auto-Translate Operations

#### ATDecode(string phrase) : string
- **Purpose**: Decode a raw AT phrase (4-char binary) into human-readable format
- **Parameters**: `phrase` — raw 4-character AT phrase from .dat file
- **Returns**: String in format `<HEXHEXHEXHEX|PhraseName>` or `<HEXHEX|UnknownItem>`

#### ATEncode(string phrase) : string
- **Purpose**: Encode a hex AT phrase string back to binary format for saving
- **Parameters**: `phrase` — 8-character hex string (e.g., "01020304")
- **Returns**: Binary string with FD markers for .dat file writing

#### ATWriter(string macroline) : string
- **Purpose**: Process a macro line for writing — convert human-readable AT phrases back to binary
- **Parameters**: `macroline` — macro line text with `<HEX|Name>` AT markers
- **Returns**: Line with AT markers converted to binary FD-wrapped format

#### ParseAT() : bool
- **Purpose**: Load all auto-translate phrases from FFXIEncoding and build menu + lookup dictionary
- **Returns**: true on completion
- **Side Effects**: Populates ATmenu (context menu) and ATObject (hex-to-name dictionary)

### MainForm - File Naming Helpers

#### KillZero(int book, int row) : string
- **Purpose**: Generate the numeric suffix for macro filenames
- **Logic**: book=0,row=0 → "", book=0,row=N → "N", book=M,row=N → "MN"
- **Examples**: KillZero(0,0)="" → mcr.dat, KillZero(0,5)="5" → mcr5.dat, KillZero(1,3)="13" → mcr13.dat

#### TenToZero(int ten) : string
- **Purpose**: Convert value 10 to "0" for display (FFXI uses 0 to represent the 10th macro)
- **Returns**: "0" if input is 10, otherwise the number as string

### MainForm - Navigation

#### FindMacro(int b, int r, int m) : bool
- **Purpose**: Navigate to a specific macro by book/row/macro index
- **Parameters**: b=book index, r=row index, m=macro index (0-19)
- **Used by**: MacroMapForm click handler, search results

## Data Models

### MacroContainer Structure
- **Type**: `string[20, 10, 20][]` (3D jagged array)
- **Dimensions**: [book 0-19][row 0-9][macro 0-19]
- **Element**: string array of 7 elements: [title, line1, line2, line3, line4, line5, line6]
- **Title**: Max 8 characters
- **Lines**: Max 60 encoded characters each (may be longer when displaying AT phrases in readable form)

### mcr.ttl File Format
- **Header**: 8 bytes (first byte = 0x01, rest = 0x00)
- **MD5**: 16 bytes (hash of the name data)
- **Data**: 16 bytes per book name (null-padded), max 15 visible characters
- **Total for 20 books**: 8 + 16 + 320 = 344 bytes

### mcrXY.dat File Format
- **Header**: 8 bytes (first byte = 0x01, rest = 0x00)
- **MD5**: 16 bytes (hash of the macro data)
- **Data**: 7600 bytes (20 macros x 380 bytes)
- **Per Macro**: 4 bytes padding + 6 lines x 61 bytes + 9 bytes title + 1 byte null = 380 bytes
- **Total**: 8 + 16 + 7600 = 7624 bytes
