# AI-DLC Audit Log

## Workspace Detection
**Timestamp**: 2026-06-02T00:00:00Z
**User Input**: "Using AIDLC, I want to create and execute plans to update MacroEditor to support the full 40 macro sets that are available in modern FFXI. The window that contains the macro list should have a scroll bar added to it so that the user can easily view all 40 macro sets and select them. The software that parses the macro files should be updated to be aware of the additional macro files in the updated version of FFXI. In addition, I would like the macro editor to accept two different files to load from: The current mcr.ttl file that it already accepts, as well as a custom config.json file that we will generate that has a pointer to the appropriate mcr.ttl file when we save. This file will be saved in the same folder as the macros when we execute a save, and will contain custom configuration settings so that we can support future features. It should always be backwards compatible, but it may be updated in the future."
**AI Response**: Initiated AIDLC workflow. Performed workspace detection scan of MacroEditor project.
**Context**: Initial request logged. Brownfield project detected - C# WinForms application targeting .NET Framework 4.5.2 with MSBuild. Existing code found: MainForm.cs, Assessment.cs, Destination.cs, Help.cs, MacroMapForm.cs, Resizer.cs. External dependency: Yekyaa.FFXIEncoding.dll.

---

## Reverse Engineering
**Timestamp**: 2026-06-02T00:01:00Z
**AI Response**: Completed reverse engineering analysis of the MacroEditor codebase. Generated 8 artifacts documenting business context, architecture, code structure, APIs, component inventory, technology stack, dependencies, and code quality assessment. Key findings: Brownfield C# WinForms app targeting .NET 4.5.2, appears to be decompiled from VB.NET, God Object anti-pattern in MainForm (~4600 lines), currently hardcoded to 20 macro books, no automated tests. Awaiting user approval to proceed.
**Context**: INCEPTION - Reverse Engineering phase complete. Artifacts at aidlc-docs/inception/reverse-engineering/

---

## Reverse Engineering - User Approval
**Timestamp**: 2026-06-02T00:02:00Z
**User Input**: "Oh wow, whoever built this was... not very good. Let's add another step to the plan to turn the monolith into a more accessible object oriented codebase prior to working on the 40 macro sets or other improvements. Arrays should be replaced with lists or other mutable sized objects when possible to allow for future changes to the macro system. This is our step 0. Once you have finished step 0, we will run the code to make sure that we didn't break anything. At that point we can continue iterating and improving the project."
**AI Response**: User approved reverse engineering and added requirement for Step 0: refactor monolith into OOP architecture, replace fixed arrays with dynamic collections. Proceeding to Requirements Analysis.
**Context**: INCEPTION - Reverse Engineering approved. New scope: Step 0 (refactor) must complete and be verified before Step 1 (40 macro sets + config.json features).

---

## Requirements Analysis - Questions Created
**Timestamp**: 2026-06-02T00:03:00Z
**AI Response**: Created requirement-verification-questions.md with 7 project-specific questions plus 2 extension opt-in questions. Questions cover: refactoring scope, VB.NET idiom removal, data structure approach, framework target, config.json content, file open defaults, and 20/40 book backward compatibility. Awaiting user answers.
**Context**: INCEPTION - Requirements Analysis, Step 6 gate. Waiting for user to fill in [Answer]: tags.

---

## Requirements Analysis - User Question
**Timestamp**: 2026-06-02T00:04:00Z
**User Input**: "Explain the options for question 2 to me and the pros, cons, and reasons for doing each option"
**AI Response**: Providing detailed explanation of VB.NET idiom replacement options (A, B, C) with pros, cons, and rationale.
**Context**: INCEPTION - Requirements Analysis, user seeking clarification before answering questions.

---

## Requirements Analysis - User Answers Received
**Timestamp**: 2026-06-02T00:05:00Z
**User Input**: "I've answered the questions. Some of them are still open until we gather more data, but you can continue for now. Please follow my guidelines about what we can and cannot start."
**AI Response**: Read all answers. Key decisions: Q1=D (helper class first pass, then full MVC), Q2=A (full VB.NET idiom removal, isolated with testing), Q3=B (typed classes MacroBook/MacroRow/Macro), Q4=C (stay on .NET 4.5.2), Q5=D (mcr.ttl path + macro set count + schema version + extensible settings), Q6=D (show both filters, auto-detect config.json when .ttl loaded), Q7=D (40-book upgrade BLOCKED pending sample file analysis), Security=X (no network calls, local only), PBT=C (skipped). No contradictions detected. Proceeding to generate requirements document.
**Context**: INCEPTION - Requirements Analysis, Step 6 gate passed. Generating requirements.md.

---

## Requirements Analysis - User Addendum
**Timestamp**: 2026-06-02T00:06:00Z
**User Input**: "Be aware that we need to run an initial 'Can it run at all' before we begin code changes, since this is a decompiled program and I haven't verified that the decompiled version is working."
**AI Response**: Added Baseline phase to requirements document before Step 0a. The decompiled source must be verified to compile and run before any refactoring begins. Any compilation fixes needed for the decompiled code are part of this baseline phase (without changing behavior).
**Context**: INCEPTION - Requirements Analysis, requirements.md updated with Baseline gate.

---

## Session Pause
**Timestamp**: 2026-06-02T00:07:00Z
**User Input**: "This is all good, but right now I need to go to lunch. I'll work more when I get back."
**AI Response**: Session paused. Requirements approved implicitly ("This is all good"). Next step on return: Workflow Planning.
**Context**: INCEPTION - Requirements Analysis complete and approved. User taking break.

---

## Session Resume
**Timestamp**: 2026-06-02T01:00:00Z
**User Input**: "Alright, I'm back. Let's continue"
**AI Response**: Resuming from Requirements Analysis (approved). Proceeding to Workflow Planning phase.
**Context**: INCEPTION - Workflow Planning next. Requirements approved. User Stories will be skipped (pure refactoring with zero new user personas).

---

## Workflow Planning - User Addendum
**Timestamp**: 2026-06-02T01:01:00Z
**User Input**: "I'd like to add some steps at the end. I'd also like an option to have variables in the config.json that, on load, are searched for in the "editable" portion of the UI and replaced with a placeholder. For instance, my player Makaria might have a macro with her name. I would want to replace Makaria with {user} in the UI, and replace it when saved with the username.I would also like the ability to export saved macros to another folder, but with these variables replaced with the ones from the json in that folder. If the json doesn't exist yet, then we would create them during the export."
**AI Response**: Adding two new steps: Step 4 (template variables — config.json variable definitions, display-time substitution with {placeholders}, save-time replacement back to values) and Step 5 (export with variable substitution — export macros to another folder using that folder's config.json variables, creating config.json if it doesn't exist). Updating requirements.md and execution-plan.md.
**Context**: INCEPTION - Workflow Planning, user adding scope.

---

## Workflow Planning - Approval
**Timestamp**: 2026-06-02T01:02:00Z
**User Input**: "We can move on now that we have the full road map"
**AI Response**: Workflow plan approved. Proceeding to Application Design phase.
**Context**: INCEPTION - Workflow Planning approved. Next: Application Design.

---

## Application Design - Answers Received
**Timestamp**: 2026-06-02T01:03:00Z
**User Input**: "I have answered the questions to the best of my ability"
**AI Response**: Read answers. Q1=B (full AT extraction including menu), Q2=C (static if possible, instance if needed, move single-use to final class), Q3=B (clean up Resizer VB idioms in 0c), Q4=A (highlight variables + user-created variables with save-time prompt), Q5=D (revert via parallel list + separate undo/redo stack with Ctrl+Z/Ctrl+Shift+Z buttons). No ambiguities detected. Proceeding to generate design artifacts.
**Context**: INCEPTION - Application Design, generating artifacts.

---

## Application Design - User Clarification
**Timestamp**: 2026-06-02T01:04:00Z
**User Input**: "Please note that for Q2, I didn't mean 'single use' functions. All of the functions in the utility class should be called from multiple places. If all of the places it is called are in one class, though, then the function should be moved into that class and made private."
**AI Response**: Understood. Clarifying the utility class rule: Functions in MacroEditorUtils must be called from multiple classes to justify their presence there. If after refactoring a utility function is only called from within one class, it should be moved into that class as a private method — not because it's "single use" (it may be called many times), but because all its callers live in one class. Updating component documentation.
**Context**: INCEPTION - Application Design, clarification on utility class membership rules.

---

## Baseline Verification - Start
**Timestamp**: 2026-06-02T01:05:00Z
**User Input**: "Let's pause here. Can we try to get the application to run before we continue? I moved the dependency files into the main MacroEditor folder before I started, but I never tried to run everything. Can we do that now?"
**AI Response**: Pausing Application Design approval. Executing Baseline verification step — attempting to build the decompiled project. User has moved dependency DLLs into the MacroEditor folder.
**Context**: Baseline Verification gate — attempting first build of decompiled source.

---

## Baseline Verification - Complete
**Timestamp**: 2026-06-02T01:30:00Z
**User Input**: "Alright! It's working! Let's commit this before we continue revising our plans."
**AI Response**: Committed baseline on branch ExtendedMacroList (43c70a0). Application launches successfully. Baseline gate PASSED.
**Context**: Baseline phase complete. Committed and verified.

---

## Resuming AIDLC Workflow
**Timestamp**: 2026-06-02T01:31:00Z
**User Input**: "Go ahead and continue until you need my input or we're ready to start changing code again."
**AI Response**: Resuming from Application Design approval. Application Design was presented but not yet explicitly approved. Proceeding to finalize Application Design, then Units Generation, then into CONSTRUCTION phase.
**Context**: INCEPTION - Application Design approval pending, then Units Generation.

---

## Construction - Unit 1 Code Generation Start
**Timestamp**: 2026-06-02T01:32:00Z
**User Input**: "Go ahead. Remember that when we're done with this change, we need to run another functional test to make sure that we didn't break anything."
**AI Response**: Beginning Unit 1 implementation — extracting 5 duplicate functions to MacroEditorUtils. Will build and present for manual testing when complete.
**Context**: CONSTRUCTION - Code Generation, Unit 1 (Deduplication)

---

## Unit 1 - Complete
**Timestamp**: 2026-06-02T01:35:00Z
**User Input**: "Alright. It seems to work. For now, let's wrap up for the night. I need to sleep. We can continue work in the morning."
**AI Response**: Unit 1 functional test passed. Committed as a8e0d86. Session ending. Next step on resume: Unit 2 (MVC Separation).
**Context**: CONSTRUCTION - Unit 1 complete. Unit 2 next.

---

## Unit 2a - Data Model Extraction Start
**Timestamp**: 2026-06-03T08:00:00Z
**User Input**: "That sounds like a good place to start. Be aware that doing the data model classes will need you to replace the previous model structure in the rest of the application to have the application in a working state, so this will be a big lift."
**AI Response**: Beginning Unit 2a — creating Macro/MacroRow/MacroBook typed classes and replacing all 75 MacroContainer/MacroPreserved access patterns across MainForm.cs and Destination.cs. Will build and present for manual testing when complete.
**Context**: CONSTRUCTION - Unit 2a (Data Model extraction). Scope: 66 MacroContainer refs + 7 MacroPreserved refs in MainForm, 2 in Destination.

---

## Roadmap Addition - Editable Macro Map
**Timestamp**: 2026-06-03T09:00:00Z
**User Input**: "Add another item to the development roadmap: I want an additional version of the macromap. The current one should be preserved, but have an edit button added to allow you to transfer the macro map to the edit macro map. The edit button should be at the top of the macro map in its own row. The second macro map should be editable, and should be able to be saved with a save button, also at the top of the macro map in its own row. There should be an 'Edit in Macro Map' option added to the right click context menu below the current Macro Map option. There is currently no way to edit macros 'side by side' and the macro map is the perfect way to do this."
**AI Response**: Added Unit 10 (Editable Macro Map) to the units of work. Depends on Unit 2 (data model). Preserves existing read-only map, adds Edit button to transfer to editable version, new context menu option, save button in editable map.
**Context**: Roadmap update — new feature added as Unit 10.

---

## Roadmap Addition - Macro Map Visual Enhancements
**Timestamp**: 2026-06-03T10:00:00Z
**User Input**: "I would like the macro map to have divider lines between each 'page,' as well as two small icons in the macro maps at the top of the window that say 'CTRL' and 'ALT.' The CTRL icon should be a faded blue. The ALT icon should be a faded red. The Macros that are activated with ctrl should be shaded the same way as ctrl, and vice versa, instead of being grey. The top row of each page is CTRL. The bottom row is ALT. I would also like buttons at the top and bottom of the scroll bar in the macro map. They should move the macro map to the next 'page' in that direction and center the window around it. The scroll bar should function in sections like this, scrolling and showing one page at a time. It should still take the same amount of scrolling with the mouse wheel to get to a new page as it does now (i.e. it tracks the location using the same units) but the actual view doesn't change until the scroll value moves to a new page."
**AI Response**: Added Unit 10 (Macro Map Visual Enhancements) and renumbered Editable Macro Map to Unit 11 (depends on Unit 10 visual enhancements being in place first).
**Context**: Roadmap update — new Unit 10 for visual enhancements, Unit 11 for editable map.

---

## 40-Book File Format Analysis
**Timestamp**: 2026-06-03T10:15:00Z
**User Input**: Provided sample files: mcr.sys, mcr.ttl, mcr_2.ttl, mcr.dat, mcr1.dat, mcr399.dat
**AI Response**: Analysis complete. Format is straightforward:
- mcr.sys: 28 bytes (8 header + 16 MD5 + 4 data = 0xDC/220). Preserve as-is.
- mcr.ttl: Books 1-20 names (same format as before)
- mcr_2.ttl: Books 21-40 names (identical format to mcr.ttl)
- .dat files: Unchanged format. Numbering extends linearly: books 0-19 = mcr.dat through mcr199.dat, books 20-39 = mcr200.dat through mcr399.dat
- GetMacroFileSuffix already generates correct filenames for books > 19
- Changes needed: read/write mcr_2.ttl, increase debuglimit to 39, add scrollbar for 40 items
**Context**: Unit 7 (40 Macro Sets) is now UNBLOCKED. Format analysis complete.

---

## Session Resume
**Timestamp**: 2026-06-04T00:00:00Z
**User Input**: "Using AIDLC, I'd like to continue work on MacroEditor"
**AI Response**: Detected existing project. Loaded aidlc-state.md (stale), checked git history. Found Units 1-3, 7, and 8 all complete. Presented status.
**Context**: Session resumption. State file was out of date.

---

## State File Correction
**Timestamp**: 2026-06-04T00:01:00Z
**User Input**: "Update state, but I also need to inform you that the scrollbar UI appeared without any necessary code changes due to how windows applications work natively. Unit 8 also needs to be marked done."
**AI Response**: Updated aidlc-state.md to reflect actual progress: Units 1, 2 (a-e), 3 (a-b), 7, and 8 all complete. Unit 8 required no code changes — WinForms ListBox provides native scrollbar when items exceed visible area. Updated project structure from Monolith to Layered OOP. Current stage set to CONSTRUCTION - Unit 4 pending.
**Context**: State file corrected. Remaining units: 4, 5, 6, 9, 10, 11, 12.

---

## Session Pause
**Timestamp**: 2026-06-04T00:02:00Z
**User Input**: "I forgot to update my macros with the names of the characters that they are related to so that I could reliably put a name to a config. I will need to do that before continuing. I'm going to go eat lunch, then I can do that and we can continue."
**AI Response**: Session paused. User needs to rename macro books with character names before starting Unit 4 (config.json) so each config can be associated with a character. Next step on return: Unit 4 after user completes macro naming.
**Context**: CONSTRUCTION - Unit 4 pending. User doing prep work (renaming macros) before we proceed.

---

## Design Pivot - Variables in Macro Book 40
**Timestamp**: 2026-06-04T01:00:00Z
**User Input**: "Let's change the config file... instead of a config file, let's include the variable substitutions IN the macro file. They will be included in Macro book 40. The title of the macro is the value for the placeholders. The first line is the value to replace the placeholders with on save."
**AI Response**: Asked clarifying questions about config.json removal, Book 40 reservation, load/save/export behavior, and unused lines.
**Context**: CONSTRUCTION - Design pivot. config.json being replaced with in-macro-file variable storage.

---

## Design Pivot - Clarification 1
**Timestamp**: 2026-06-04T01:01:00Z
**User Input**: "1. Dropped entirely for now. We probably won't need it.

2. We work backwards from Book 40 Page 10. The last line of a macro should have a value to let the program know that the macro is a placeholder. The other five lines can be alternative values, and each one would be used with "placeholder#" So the top line is replaced with {name}, but the second line is replaced with {name2}. If an entire page is found without the "marker" in line 6, then we stop checking for variables. This way the user can define as many variables as they need, and group them logically, but they aren't locked in to a set number.

3. Yes, but with the changes I listed in item 2.

4. Correct, but with the changes in item 2.

5. I explained in item 2."
**AI Response**: Captured flexible variable system design. Asked about marker value and scanning direction confirmation.
**Context**: Variable system design: Book 40 pages 10→1, marker in line 6, title=placeholder name, lines 1-5=values for {name} through {name5}.

---

## Design Pivot - Clarification 2
**Timestamp**: 2026-06-04T01:02:00Z
**User Input**: "1. I don't want special characters. VARIABLE in all caps is probably best.

2. That is correct. Please note that the user is free to start with any individual macro in page 10. They don't HAVE to start with alt 0. they can start with ctrl 1. They just HAVE to start with SOME macro in page 10."
**AI Response**: Final design confirmed. Marker = "VARIABLE" (all caps, line 6). Scanning: Book 40 Page 10 → backwards, stops at first full page with no markers. User can start with any macro on Page 10. config.json dropped entirely. Updating roadmap.
**Context**: Variable system design finalized. Unit 4 (config.json) eliminated. Units 5/6 redesigned around Book 40 variable storage.

---

## Roadmap Update - Design Pivot Applied
**Timestamp**: 2026-06-04T01:10:00Z
**User Input**: "Go ahead an update the unit of work file. Once that's done I'll decide what to do next"
**AI Response**: Updated unit-of-work.md and unit-of-work-dependency.md to reflect design pivot:
- Unit 4 (config.json) marked ELIMINATED
- Unit 5 (Template Variables) redesigned: variables stored in Book 40 macros, scanned backwards from Page 10, marker "VARIABLE" in line 6, title=placeholder name, lines 1-5=values for {name} through {name5}
- Unit 6 (Export) redesigned: reads destination's Book 40 for variable values instead of config.json
- Units 7/8 marked DONE with commit references
- Dependency diagram updated: Unit 4 removed from chain, Unit 5 now depends on Unit 3 + Unit 7
- EditorConfig.cs removed from planned file structure
- Shared resources updated: Book 40 replaces EditorConfig as variable store
**Context**: CONSTRUCTION - Roadmap documentation updated. Ready for user to decide next step.

---

## Unit 5 - Template Variables Start
**Timestamp**: 2026-06-04T01:15:00Z
**User Input**: "Alright. Let's start on the Template Variables functionality. I have a couple of characters ready, and I've managed to identify them prior to this. That will allow us to test."
**AI Response**: Beginning Unit 5 (Template Variables). Per AIDLC Construction workflow: Functional Design first (since this involves new business logic — variable scanning, substitution engine, UI highlighting), then Code Generation. Loading existing code to understand current state before designing.
**Context**: CONSTRUCTION - Unit 5 (Template Variables). Starting with Functional Design stage.

---

## Unit 5 - Functional Design Complete
**Timestamp**: 2026-06-04T01:20:00Z
**AI Response**: Created functional design for Unit 5 (Template Variables). Document covers:
- Variable storage format in Book 40 (Title=placeholder name, Lines[0-4]=values, Lines[5]=VARIABLE marker)
- Scanning algorithm (backwards from Page 10, stops at first full page with no markers)
- VariableSubstitutionEngine class API (LoadVariables, ApplyPlaceholders, ResolvePlaceholders, SubstituteAll, ResolveAllForSave)
- Integration points in MainForm (File_Open_Click, File_SaveAll_Click, File_SaveRow_Click)
- Substitution order (length-descending to prevent partial matches)
- Edge cases and conflict resolution
- Testing scenarios
- UI highlighting deferred to sub-task after core engine works
Awaiting user review.
**Context**: CONSTRUCTION - Unit 5, Functional Design stage. Awaiting approval.

---

## Unit 5 - Functional Design Approved
**Timestamp**: 2026-06-04T01:25:00Z
**User Input**: "Looks good. Let's continue."
**AI Response**: Functional design approved. Proceeding to Code Generation stage (Part 1: Planning).
**Context**: CONSTRUCTION - Unit 5, Functional Design approved. Moving to Code Generation.

---

## Unit 5 - Code Generation Plan Approved
**Timestamp**: 2026-06-04T01:26:00Z
**User Input**: "Go ahead"
**AI Response**: Code generation plan approved. Beginning implementation: Step 1 (VariableSubstitutionEngine.cs), Step 2 (MainForm load integration), Step 3 (MainForm save integration), Step 4 (build/verify).
**Context**: CONSTRUCTION - Unit 5, Code Generation Part 2 (implementation).

---

## New Requirement - Variable Exclusion
**Timestamp**: 2026-06-04T01:30:00Z
**User Input**: "We have a new requirement. We need to be able to designate when we don't want an instance of a variable to be replaced at export. Ideally we would do this once and never have to do so again. Can you run me through some options?"
**AI Response**: Presenting options for marking specific placeholder instances as non-replaceable during export.
**Context**: CONSTRUCTION - Unit 5/6 scope addition. User wants to lock certain placeholder occurrences from export substitution.

---

## New Requirement - Variable Lock Mechanism
**Timestamp**: 2026-06-04T01:35:00Z
**User Input**: "I think we should probably create a dedicated lock file. The file should store the Book, Page, Macro, line, and variable that is locked for each locked variable. On load it looks for that variable in that position. If that variable doesn't exist in that position, the data for that position is deleted (the lock no longer applies). If that variable does exist in that position, then it appends the !."
**AI Response**: Presented summary of lock file mechanism. Asked about file format and lock creation UX.
**Context**: CONSTRUCTION - Unit 5/6 scope addition. Lock mechanism for non-replaceable variable instances.

---

## Variable Lock - Format Decision
**Timestamp**: 2026-06-04T01:36:00Z
**User Input**: "JSON" (for lock file format), "Yes, typing {!user} is how you lock it" (for lock creation mechanism)
**AI Response**: Confirmed design:
- Lock file: JSON format, stored as mcr.locks in macro directory
- Entries: book, page, macro, line, variableName
- User creates locks by typing {!user} directly in the text box
- On load: read lock file, apply forward substitution, then for each lock entry check if {variableName} exists at that position → if yes, change to {!variableName}; if no, delete stale lock entry
- On save: {!user} resolves to source value (Makaria), {user} resolves to source value too (same on normal save). Rebuild lock file from all {!...} tokens in memory.
- On export: {user} → destination value, {!user} → source value (Makaria stays Makaria)
- Self-healing: stale lock entries removed automatically
Updating functional design.
**Context**: Lock mechanism finalized. Updating Unit 5 functional design and noting Unit 6 export impact.

---

## Unit 13 - Broadcast Edit Start
**Timestamp**: 2026-06-04T01:45:00Z
**User Input**: "Let's implement unit 13. It is an easy feature to test, and is an important quality of life feature."
**AI Response**: Beginning Unit 13 implementation. Simple feature: add context menu items for "Apply to All Books" at page, macro, and line levels. Skipping formal functional design since this is straightforward UI + clone logic.
**Context**: CONSTRUCTION - Unit 13 (Broadcast Edit). Direct to code generation since logic is simple.

---

## Unit 13 - Clarification on "Line"
**Timestamp**: 2026-06-04T01:46:00Z
**User Input**: "Please be aware of the following: When I said 'line' I meant like a row of CTRL macros from the same page, or a row of ALT macros from the same page, but single lines in a macro should ALSO have this feature, including the title."
**AI Response**: Understood. "Line" means TWO things for broadcast:
1. A Ctrl row (macros 0-9) or Alt row (macros 10-19) from a page — broadcast that half-row to all books
2. A single text line within a macro (Title, or Lines 1-6) — broadcast that specific text to the same macro position/line in all books
Both features will be implemented.
**Context**: CONSTRUCTION - Unit 13, clarification on scope. Four levels of broadcast: Page (full 20 macros), Ctrl/Alt row (10 macros), single macro, single text line (including title).

---

## Unit 6 - Export with Variable Substitution Start
**Timestamp**: 2026-06-04T02:00:00Z
**User Input**: "Alright. Let's do the export first."
**AI Response**: Beginning Unit 6 (Export with Variable Substitution). This involves new business logic (reading destination Book 40, resolving with destination variables, handling {!name} locks). Starting with functional design.
**Context**: CONSTRUCTION - Unit 6 (Export). Starting with Functional Design stage.

---

## Unit 6 - Functional Design Approved
**Timestamp**: 2026-06-04T02:05:00Z
**User Input**: "Alright, awesome. Proceed."
**AI Response**: Functional design approved. Beginning code generation for Unit 6 (Export + Restore from Backup).
**Context**: CONSTRUCTION - Unit 6, Code Generation.

---

## Bug Fix - ReadMacroRow Line Offset (Q¥ garble)
**Timestamp**: 2026-06-26T12:00:00Z
**User Input**: "Okay, let's go ahead and implement the fix you suggested."
**AI Response**: Diagnosed via hex inspection of mcr399.dat / mcr.dat.backup in folder 167bf9b. Root cause: ReadMacroRow reads each macro's 6 line fields starting at offset 0,61,122,183,244,305 but WriteRow (and FFXI itself) place line content at offset 4,65,126,187,248,309 (after a 4-byte header). The 4-byte misalignment caused stray header bytes (e.g. 60 51 A4 = "Q¥") to be pulled into line 1 and baked into text on save. Confirmed FFXI 'Follow' macro stores "/target <me>" at offset 4. Fix: change line read start offset from 0 to 4 in ReadMacroRow. Header is null for all real macros (verified), so no metadata is lost by the existing zero-on-write behavior.
**Context**: CONSTRUCTION - Bug fix in MacroFileManager.ReadMacroRow. Logged as Unit 15.

## Unit 10 - Macro Map Visual Enhancements Start
**Timestamp**: 2026-06-26T12:30:00Z
**User Input**: "Let's start on unit 10. We'll table unit 9 for now. I want to make sure all the other features are done before we worry about undo, redo, and what COUNTS as something we should add to the stack."
**AI Response**: Beginning Unit 10 (Macro Map Visual Enhancements). Unit 9 (Undo/Redo) deferred until other features complete. Reading current MacroMapForm to plan functional design.
**Context**: CONSTRUCTION - Unit 10. Unit 9 deferred.

## Unit 10 - Complete
**Timestamp**: 2026-06-26T13:30:00Z
**User Input**: "Perfect. Go ahead and commit and push. This unit is done. I have to go get ready for dinner."
**AI Response**: Unit 10 (Macro Map Visual Enhancements) complete and tested. Color-coded Ctrl (blue)/Alt (red) labels, pinned CTRL/ALT header with divider, Prev/Next page buttons, window sized to one page. Snap-scrolling dropped per user; dividers between pages dropped. Committing and pushing.
**Context**: CONSTRUCTION - Unit 10 done. Remaining: Unit 11 (Editable Macro Map), Unit 12 (Text Import/Export), Unit 14 (Warning Cleanup). Unit 9 (Undo/Redo) deferred.
