# Technology Stack

## Programming Languages
- **C#** - .NET Framework 4.5.2 - Primary application language (likely converted from VB.NET)

## Frameworks
- **Windows Forms** - .NET Framework 4.5.2 - UI framework for desktop application
- **Microsoft.VisualBasic** - .NET Framework 4.5.2 - String operations, message boxes, conversions (legacy VB.NET compat)

## Infrastructure
- **Windows File System** - Local macro file storage (mcr.ttl + mcrXY.dat files)
- **Windows Registry** - FFXI installation path auto-detection

## Build Tools
- **MSBuild** - via .csproj - Project build system
- **Visual Studio** - IDE (solution file present)

## Testing Tools
- None currently in use

## Cryptography
- **MD5CryptoServiceProvider** - .NET Framework - Checksum generation for macro file integrity (matches FFXI client expectations)

## Key Libraries
- **Yekyaa.FFXIEncoding.dll** - Unknown version - FFXI auto-translate phrase data loading and lookup
- **System.Text.RegularExpressions** - .NET Framework - AT phrase pattern matching in macro text
- **System.IO** - .NET Framework - File read/write operations
- **Microsoft.Win32** - .NET Framework - Registry access for FFXI path detection
