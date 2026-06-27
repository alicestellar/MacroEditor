# MacroEditor

A Windows desktop editor for **Final Fantasy XI** macro files. It reads and writes the
game's `mcr.ttl` / `.dat` macro data and adds bulk-editing, sharing, templating, and
multiboxing conveniences that the in-game macro UI lacks.

- **Platform:** Windows, .NET Framework 4.8 (WinForms)
- **Macro layout:** up to **40 books**, each with **10 pages**, each page holding **20
  macros** (Ctrl1-Ctrl10 and Alt1-Alt10). Every macro has a **title** (max 8 characters)
  and **6 lines** of commands.

> Use of macro editing tools may fall under the PlayOnline / FFXI User Agreement. Use at
> your own discretion.

---

## Getting Started

1. **Build / launch** the app (`Macro Editor.exe`), or open the solution in Visual Studio
   and run.
2. **File > Open** and point it at your FFXI `USER\<id>\` folder containing `mcr.ttl`.
   The app loads all books, reads each page's macros, and (if present) applies template
   variables from Book 40.
3. Pick a book from the list on the left, pick a page, then click a macro slot to edit it.
4. When finished, **File > Save All** writes everything back to disk (and refreshes the
   in-memory backup used by **Revert**).

---

## Features

### Browsing books and pages

- The **book list** on the left shows all loaded books (up to 40) and scrolls to reach
  every set.
- Selecting a book loads its pages; the page buttons switch among the 10 pages.
- Each page shows 20 macro slots: **Ctrl 1-10** and **Alt 1-10**.

### Editing a macro

- Click a slot to load its **title** and **6 command lines** into the editor.
- Titles are capped at **8 characters** (extra characters are dropped on save).
- **In-line text editing supports native Ctrl+Z** (standard textbox undo) for the line you
  are typing in. This is independent of the app-level Undo described below.

### Undo / Redo (Edit menu)

App-level **Undo** and **Redo** for the structural clipboard operations (it does **not**
cover in-macro typing, which keeps its own Ctrl+Z):

- Covers **Paste**, **Clear**, and **Cut** at every scope — **macro, row (Ctrl/Alt side),
  page, and book** — as well as **Broadcast** operations.
- Invoked from the **Edit** menu only (Undo / Redo entries show the action they will undo,
  e.g. "Undo Clear Page").
- History holds up to 50 actions and is cleared when you **Open** a file or do a full
  **Save All**.

### Copy / Cut / Paste / Clear / Revert (right-click menus)

Right-click a macro, a Ctrl/Alt row handle, a page, or the book to get a context menu with:

- **Copy / Cut / Paste** — move content between slots, rows, pages, or books using an
  internal clipboard.
- **Clear** — empty the target.
- **Revert** — restore the target to the last saved (on-disk) state.
- **Copy to Clipboard / Paste from Clipboard** — share macros as plain text using a
  portable text format (good for forums and chat).

### Broadcast (apply to all books)

Quickly push content from the current book to the **same position in every other book**:

- **Broadcast Page** — copy all 20 macros of the current page to that page in all books.
- **Broadcast Macro** — copy one macro slot to that slot in all books.
- **Broadcast Ctrl/Alt Row** — copy the current 10-macro half-row to all books.
- **Broadcast Line** — copy a single macro line (or title) to all books.

Each broadcast asks for confirmation (it overwrites) and is **undoable**.

### Template variables (Book 40)

Book 40 acts as a **variables book**. Tokens like `{Name}` or `{!Name}` in your macros are
substituted from Book 40 when a file is loaded, so you can keep one set of macros and
retarget them per character/instance:

- On **Open**, variables are loaded from Book 40 and applied to Books 1-39.
- **Locks** (`{!name}`) pin a substituted value at specific positions; locks are loaded and
  re-applied on open and saved alongside your macros.
- On **Save All**, variables are resolved/de-substituted appropriately before writing, and
  the lock file is rebuilt and saved.

### Export to text

Share or back up macros as human-readable or machine-readable text:

- **Scopes:** export a single **row**, a **page**, a **book**, or **all books**.
- **Formats:**
  - **Human** — a readable layout with `[slot] Title:` and numbered lines, plus editing
    rules at the top.
  - **AI** — a JSON block between `<<<MACRO_JSON_BEGIN>>>` / `<<<MACRO_JSON_END>>>` markers,
    designed to be safely edited by hand or by an AI assistant and re-imported.

### Import from text

Bring exported text back into the editor (changes are in-memory until you Save All):

- **Import Text** — put each macro back at the book/page/slot it was exported from.
- **Import Over Book / Page / Row** — import a file's content onto a specific target you
  choose, ignoring the file's original position.
- **Import All** (folder) — scan a folder of `.txt` exports and import the clean ones in one
  pass. Files with problems are **not** imported; they are listed at the end so you can fix
  them individually (see below).

### Import Repair Workflow

When a single-file import has problems, the editor walks you through fixing them instead of
failing outright. Nothing is loaded until every problem is resolved.

- **Stage 1 - file syntax:** if the text won't parse (e.g. broken JSON or missing markers),
  an editor opens with the whole file and the parser error. Edit it and click **Re-check**;
  it loops until the file parses or you cancel.
- **Stage 2 - per-item problems:** each problem is presented one at a time in a focused
  editor. Detected problems include: **invalid slot name**, **book number out of range**,
  **page number out of range**, **title longer than 8 characters**, and **wrong line count**
  (not exactly 6 lines).
- For each problem you can **Fix** (edit, then re-validate), **Skip This**, **Skip All
  Remaining**, or **Cancel Import**.
- After a repaired import, you're offered the option to **save the corrected text back** to
  the source file (with a warning if any skipped items would be omitted).

For bulk folder import, the repair workflow is not run inline — problem files are reported so
you can import them one at a time to use it.

### Macro Map

- **Macro Map** opens a visual overview of a book's macros laid out as a grid.
- **Editable Macro Map** lets you edit titles/content directly from the map view.
- **Copy Macro Map (FFXIAH Table)** copies the book layout as a forum-ready table.

### Validation (Evaluate) and Search

- **Evaluate** checks your macros for issues and lets you jump straight to a flagged macro.
- **Find** searches across your macros and navigates to matches.

### Saving, backups, and book names

- **Save Macro Row** writes just the current page; **Save All** writes everything.
- **Save Booknames** persists the book/list names.
- **Rename Book** changes a book's display name.
- **Backup** and **Restore Backup** protect against accidental overwrites; backups are
  written before bulk export.

### Other conveniences

- **Point to another macro line** (macro menu) links a macro to another macro's line.
- **Copy File Location** copies the on-disk path of the current page's macro file.
- **Help** and **Feature Tour** provide in-app guidance.

---

## The AI / Text File Format

Exported AI-format files contain a fixed instruction header, then a JSON block between
`<<<MACRO_JSON_BEGIN>>>` and `<<<MACRO_JSON_END>>>`. The JSON shape is:

```json
{
  "format": "macro-editor-ai-v1",
  "character": "<id>",
  "scope": "book",
  "books": [
    {
      "book": 1,
      "name": "BookName",
      "pages": [
        {
          "page": 1,
          "macros": [
            { "slot": "Ctrl1", "title": "Title", "lines": ["line1", "", "", "", "", ""] }
          ]
        }
      ]
    }
  ]
}
```

Rules for safe hand/AI editing:

- Edit only `title` and `lines` string values.
- Keep titles to **8 characters or fewer**.
- Keep exactly **6 entries** in every `lines` array.
- Use slot names `Ctrl1`-`Ctrl10` and `Alt1`-`Alt10`.
- Leave variable tokens like `{Name}` / `{!Name}` as-is unless you intend to change them.
- Keep book numbers within `1-40` and page numbers within `1-10`.

If you break one of these, the **Import Repair Workflow** will help you correct it on import.

---

## Building from source

```cmd
dotnet build "Macro Editor\Macro Editor.csproj"
```

Requires the .NET Framework 4.8 developer pack (or Visual Studio with the .NET desktop
workload). The build depends on `Yekyaa.FFXIEncoding.dll` (included at the repo root).
