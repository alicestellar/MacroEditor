# Component Inventory

## Application Packages
- **MacroEditor** (single package) - FFXI Macro Editor WinForms application

## Application Components
- **MainForm** - Primary application window and all business logic
- **Assessment** - Macro validation results and search results display form
- **Destination** - Macro relocation dialog form
- **MacroMapForm** - Visual macro map display form
- **Help** - Help documentation display form
- **Resizer** - Proportional control resizing utility

## Infrastructure Packages
- None (standalone desktop application, no infrastructure-as-code)

## Shared Packages
- **My/** - VB.NET application framework utilities (MyApplication, MyComputer, MyProject, MySettings)

## Test Packages
- None (no automated tests exist)

## External Libraries
- **Yekyaa.FFXIEncoding.dll** - FFXI auto-translate phrase data loader

## Total Count
- **Total Components**: 6 (application classes)
- **Application**: 5 (forms) + 1 (utility)
- **Infrastructure**: 0
- **Shared**: 1 (My/ namespace)
- **Test**: 0
- **External Libraries**: 1
