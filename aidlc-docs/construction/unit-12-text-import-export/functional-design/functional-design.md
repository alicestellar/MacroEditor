# Functional Design: Unit 12 — Text File Import/Export

## Overview

Allow exporting macros to editable text files and importing them back. The primary use case is sending a file to an AI (or hand-editing it), then re-importing. Two formats are produced:

- **Human format** — labeled plain text, no escaping, with editing guardrails
- **AI format** — a leading instruction prompt followed by a JSON payload between markers

Export granularity: **book**, **page**, or **row** (Ctrl or Alt half-row), via right-click context menus. "All books" exports one file per book into a folder. Placeholders (e.g. `{Name}`) are exported as-is, never resolved.

## Scope and Addressing

Every exported macro is fully addressable so it can be put back exactly where it came from:

- **Book**: number (1-40) and name
- **Page**: number (1-10)
- **Slot**: `Ctrl1`-`Ctrl10`, `Alt1`-`Alt10` (maps to macro index 0-9 / 10-19)

Each file records its **scope** (`book` / `page` / `row` / `all`) and the source book number + name.

## Human Format

```text
================================================================
 FFXI MACRO EXPORT  (Human-Editable Format v1)
 Character: Makaria
 Scope: BOOK
 Source: Book 1  "Combat"
================================================================
 HOW TO EDIT WITHOUT BREAKING THIS FILE:
  * Do NOT change lines starting with  =  #  -  or  [ .
  * Keep the "  N:" prefixes (1-6) on each macro line. To make a
    line empty, leave it blank after the colon.
  * Titles are max 8 characters; extra characters are dropped on import.
  * Tokens like {Name} are variables - leave them as-is unless you
    intend to change them.
  * Do NOT reorder, add, or remove books, pages, or slots.
================================================================

#### BOOK 1: "Combat" ####

-- Page 1 --

[Ctrl 1]  Title: MaWS
  1: /console send {Name} /ws "Seraph Blade" <t>
  2:
  3:
  4:
  5:
  6:

[Ctrl 2]  Title: Provoke
  1: /ja "Provoke" <t>
  2:
  ...
```

**Parse rules:**
- Header lines (between the `===` rules) are read for Character / Scope / Source book.
- `#### BOOK n: "name" ####` → book index n-1
- `-- Page p --` → row (page) index p-1
- `[Ctrl m]` → macro index m-1; `[Alt m]` → macro index m+9
- `Title: xxx` (on the slot line) → macro title (truncated to 8 on import)
- `  N: text` → line index N-1 (1-6)
- Blank/unknown lines between macros are ignored.

## AI Format

```text
=== INSTRUCTIONS FOR THE AI EDITING THIS FILE — READ FIRST ===
This file contains Final Fantasy XI macros exported from "Macro Editor".

STRUCTURE:
  books -> pages (1-10) -> macros. Each macro has a "slot" (Ctrl1-Ctrl10,
  Alt1-Alt10), a "title" (max 8 characters), and exactly 6 "lines"
  (strings; use "" for an empty line).

RULES — FOLLOW EXACTLY OR THE FILE WILL FAIL TO IMPORT:
  * Edit ONLY the "title" and "lines" string values.
  * Do NOT add, remove, rename, or reorder any keys, slots, pages, or books.
  * Keep titles to 8 characters or fewer.
  * Keep exactly 6 entries in every "lines" array.
  * Tokens like {Name} or {!Name} are variables. Leave them exactly as
    written unless explicitly asked to change them.
  * Output the ENTIRE file back, including this instruction block and the
    BEGIN/END markers, with your edits applied. The text between the markers
    must be valid JSON.
=== END INSTRUCTIONS ===

<<<MACRO_JSON_BEGIN>>>
{
  "format": "macro-editor-ai-v1",
  "character": "Makaria",
  "scope": "book",
  "books": [
    { "book": 1, "name": "Combat", "pages": [
      { "page": 1, "macros": [
        { "slot": "Ctrl1", "title": "MaWS",
          "lines": ["/console send {Name} /ws \"Seraph Blade\" <t>","","","","",""] }
      ]}
    ]}
  ]
}
<<<MACRO_JSON_END>>>
```

**Parse rules:**
- Locate `<<<MACRO_JSON_BEGIN>>>` and `<<<MACRO_JSON_END>>>`; parse the text between them as JSON (ignore everything else, including any AI chatter outside the markers).
- Walk books → pages → macros; map `slot` to a macro index.

## Format & Scope Auto-Detection (on import)

- If the content contains `<<<MACRO_JSON_BEGIN>>>` → AI format.
- Else if it contains the `FFXI MACRO EXPORT` header / `#### BOOK` → Human format.
- Else → error: "Unrecognized macro text file."
- Scope is read from the file's metadata (Scope line / `"scope"` field).

## Import Targeting

- **File menu → Import from Text...**: "put it back" — write each macro to the book/page/slot recorded in the file (matching by book number). Auto-detects format.
- **Context-menu imports** ("Import Text Over This Book/Page/Row"): remap the file's content onto the *clicked* target, ignoring the file's original book/page numbers. Used to overwrite a target with content that "doesn't belong" to it.
  - The file's scope must match the target (book-file → book target, etc.). Mismatch → clear error.

## UI Integration

**Book context menu (MenuBook):**
- `Export Book to Text ▸ Human Format / AI Format`
- `Import Text Over This Book...`

**Page/row context menu (MenuRow):**
- `Export Page to Text ▸ Human Format / AI Format`
- `Import Text Over This Page...`

**Handler context menu (MenuHandler — Ctrl/Alt half-row):**
- `Export Ctrl/Alt Row to Text ▸ Human Format / AI Format`
- `Import Text Over This Row...`

**File menu:**
- `Export All Books to Text...` → choose Human/AI + a folder; writes one file per book (e.g. `book-01-Combat.human.txt`).
- `Import from Text...` → pick a file; auto-detect; put back where it came from.

All export/import dialogs reuse the same `OpenFileDialog`/`SaveFileDialog`/`FolderBrowserDialog` style as the rest of the app.

## New Class: MacroTextSerializer.cs

```csharp
public enum MacroTextFormat { Human, Ai }
public enum MacroTextScope { Book, Page, Row, All }

public class MacroImportResult
{
    public MacroTextScope Scope;
    public int SourceBookNumber;      // 1-based, from file
    public string SourceBookName;
    public List<MacroImportEntry> Entries; // each: page index, macro index, string[7]
    public List<string> Warnings;     // non-fatal issues
    public bool Success;
    public string ErrorMessage;       // set when parse fails
}

public static class MacroTextSerializer
{
    // Export
    public static string ExportHuman(MacroTextScope scope, string character,
        int bookNumber, string bookName, /* data */ ...);
    public static string ExportAi(MacroTextScope scope, string character,
        int bookNumber, string bookName, /* data */ ...);

    // Import (auto-detect format)
    public static MacroImportResult Parse(string fileContent);
}
```

## Error Handling (critical, especially for multi-file all-books import)

| Problem | Handling |
|---------|----------|
| Unrecognized format | Abort with clear message |
| Malformed JSON (AI) | Report JSON error position; abort that file only |
| Missing BEGIN/END markers | Abort with message naming the markers |
| Title > 8 chars | Truncate, add a warning |
| `lines` array ≠ 6 entries | Pad/clamp to 6, add a warning |
| Unknown/out-of-range slot, page, or book | Skip that macro, add a warning |
| Scope mismatch on context-menu import | Abort with explanation |
| Folder import: one file fails | Continue with the rest; summary lists per-file success/failure |
| Import would overwrite data | Confirmation dialog before applying (same pattern as broadcast/export) |

After a successful import, set `SomethingEdited = true` and refresh the current view. Imports affect in-memory data only; the user still does File → Save All to persist (consistent with the rest of the app).

## Phase 2 (Deferred) — Partial Import with Correction Editor

After base functionality works, add resilient bulk import:

- On a multi-file (or multi-section) import, **import everything that parses successfully** immediately.
- For each **broken section/file**, open a **correction editor** window showing the broken content as raw plain text, with the parse error explained (what failed and, where possible, the line/marker involved).
- The user fixes the text in place and confirms; the corrected content is re-parsed and imported.
- If multiple sections are broken, open them **in sequence** — fix one, move to the next — until all are resolved or the user cancels the remainder.
- A final summary reports what imported, what was corrected, and what was skipped.

This is explicitly out of scope for the first implementation pass; the base import will report errors via dialog/summary. The correction-editor flow is layered on afterward.

## Testing Scenarios

1. Export a book (Human) → file is readable, guardrails present, placeholders intact.
2. Export a book (AI) → prompt + valid JSON between markers.
3. Round-trip: export book → reimport via File menu → data unchanged.
4. Edit a few titles/lines in the Human file → import → only those change; title >8 chars truncated.
5. Edit the AI JSON → import → changes applied.
6. Export page / row → import back → correct placement.
7. Context-menu "Import Over This Book" with a file from a different book → overwrites the clicked book.
8. Scope mismatch (page file onto a book target) → clear error, no changes.
9. All-books export → folder with 40 files; reimport the folder → all restored; a deliberately corrupted file is reported but others still import.
10. Malformed JSON → specific error, no partial corruption.
