# Requirements Verification Questions

Please answer the following questions to help clarify the requirements for the MacroEditor refactoring and enhancement project. Fill in the letter choice after each [Answer]: tag. If none of the options match, choose the last option (Other) and describe your preference.

---

## Question 1
For the Step 0 refactoring, how far should we go with separating concerns? The current MainForm.cs handles everything (file I/O, parsing, UI, validation, clipboard, AT encoding). What's the target architecture?

A) Extract file I/O and macro parsing into a separate class (e.g., MacroFileManager), keep everything else in MainForm

B) Extract file I/O, macro data model, and validation into separate classes — but leave UI event handling in MainForm

C) Full separation: data model class, file I/O class, validation class, and MainForm only handles UI — proper MVC/MVP-style layering

D) Other (please describe after [Answer]: tag below)

[Answer]: D
I want you to do C, but please be sure that while you're refactoring to look for duplicate code on the first pass. I want duplicated parts of the code to be abstracted out to a helper class first. Once that's done, we'll do a first pass run to make sure everything still works. Once we do that, we can do the full C option, and if there are functions in the helper class that are only used from one of the other new classes, we can move it to the respective new class.

## Question 2
The current code uses `Microsoft.VisualBasic` extensively (late binding, Operators, Conversions). During the refactoring, should we eliminate these VB.NET idioms and replace them with native C# equivalents?

A) Yes — replace all VB.NET idioms with idiomatic C# (cleaner but higher risk of subtle behavior changes)

B) Replace obvious ones (Conversions.ToString, Operators.CompareString) but keep late binding where it simplifies things

C) No — leave VB.NET idioms alone for now, only refactor architecture

D) Other (please describe after [Answer]: tag below)

[Answer]: A
Make sure that we do this section isolated. We make sure we've committed our latest and that the function can run properly before we start, and we don't continue with any other work until we've done a full test of the software. I'll want help documenting the regression tests I'll need to run.

## Question 3
For the macro data containers, the current structure is `string[20, 10, 20][]` (a 3D array of string arrays). When replacing with dynamic collections, what's the preferred approach?

A) Use nested `List<List<List<string[]>>>` (fully dynamic but complex)

B) Create typed classes (e.g., `MacroBook` containing `MacroRow` containing `Macro` with `Title` and `Lines` properties) and use `List<MacroBook>`

C) Use a `Dictionary<(int book, int row, int macro), Macro>` for flat lookup with a typed Macro class

D) Other (please describe after [Answer]: tag below)

[Answer]: B

## Question 4
The .NET Framework 4.5.2 target is quite old (2014). Should we upgrade the target framework during refactoring?

A) Yes — upgrade to .NET 8 (latest LTS, modern C# features, but requires migration from .NET Framework to .NET SDK-style project)

B) Yes — upgrade to .NET Framework 4.8 (minimal migration effort, stays on WinForms/.NET Framework but gets latest framework fixes)

C) No — keep .NET Framework 4.5.2 to maintain maximum compatibility with existing build environment

D) Other (please describe after [Answer]: tag below)

[Answer]: C

## Question 5
For the config.json file feature, what configuration settings (beyond the mcr.ttl path) do you envision storing initially?

A) Just the mcr.ttl file path for now — we'll add more later as features are developed

B) mcr.ttl path + number of macro sets (to support variable set counts) + a schema version field

C) mcr.ttl path + schema version + a placeholder "settings" object that we can extend later

D) Other (please describe after [Answer]: tag below)

[Answer]: D
I want the mcr.ttl path, the number of macro sets in case FFXI ever updates this again, a schema version field, and a placeholder settings object that we can extend later.

## Question 6
When opening files, should the config.json be the preferred/default option, or should mcr.ttl remain the default with config.json as an alternative?

A) config.json becomes the new default in the Open dialog, with mcr.ttl as a secondary filter option

B) mcr.ttl remains the default (backward compatible), config.json is an additional filter option

C) Show both filters equally — let the user choose each time

D) Other (please describe after [Answer]: tag below)

[Answer]: D
I want to be able to see the .json and .ttl files in the folder when I look in it. Filter other files to reduce visual clutter for the user. If the .ttl is loaded, CHECK in the same folder for the config.json file and load it anyway. This helps prevent user confusion.

## Question 7
For the 40 macro set support — FFXI expanded from 20 to 40 books. The mcr.ttl file format would need to store 40 names instead of 20. Should the editor handle both old (20-book) and new (40-book) mcr.ttl files transparently?

A) Yes — auto-detect the file size and load either 20 or 40 books accordingly

B) Always assume 40 books — if old 20-book file is loaded, create empty entries for books 21-40

C) Ask the user when opening whether to treat it as 20 or 40 book format

D) Other (please describe after [Answer]: tag below)

[Answer]: D
The mcr.ttl file still exists, but it actually has other files that contain the various macros. There is a mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, followed by mcr1.dat to mcr399.dat (though not all of these are guaranteed to exist and that should be accounted for). I will provide copies of the mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, and one of the number mcr.dat files for you to analyze to determine how we will handle this. Until that process is complete we should not begin the upgrade to 40 macro books.

## Question: Security Extensions
Should security extension rules be enforced for this project?

A) Yes — enforce all SECURITY rules as blocking constraints (recommended for production-grade applications)

B) No — skip all SECURITY rules (suitable for PoCs, prototypes, and experimental projects)

X) Other (please describe after [Answer]: tag below)

[Answer]: X
I don't want anything in the project that sends information across the internet. There should be no calls to send data while we are running the application. Other than that, I have no concern about security. This program will only be run locally.

## Question: Property-Based Testing Extension
Should property-based testing (PBT) rules be enforced for this project?

A) Yes — enforce all PBT rules as blocking constraints (recommended for projects with business logic, data transformations, serialization, or stateful components)

B) Partial — enforce PBT rules only for pure functions and serialization round-trips (suitable for projects with limited algorithmic complexity)

C) No — skip all PBT rules (suitable for simple CRUD applications, UI-only projects, or thin integration layers with no significant business logic)

X) Other (please describe after [Answer]: tag below)

[Answer]: C
