# Unit 9 (Undo) - Scope & Behavior Questions

Answer each by filling the letter after `[Answer]:`. Option X = Other (describe).

## What I found in the code

All paste/clear/cut operations mutate a single in-memory list (`this.Books`). The four scopes you named map to existing context-menu handlers:

- **Book** scope -> `MenuBook_Paste`, `MenuBook_Clear`, `MenuBook_Cut`, `MenuBook_PasteClipboard`
- **Page** scope -> `MenuRow_Paste`, `MenuRow_Clear`, `MenuRow_Cut`, `MenuRow_PasteClipboard` (model class is named `MacroRow` but it is a page of 20 macros)
- **Row** scope (Ctrl-side / Alt-side, 10 macros) -> `MenuHandler_PasteSide`, `MenuHandler_ClearSide`, `MenuHandler_CutSide`
- **Macro** scope -> `MenuMacro_Paste`, `MenuMacro_Clear`, `MenuMacro_Cut`, `MenuMacro_PasteClipboard`

My planned approach: before any of these mutate the data, snapshot the affected book (cheap `Clone()`), push it on an undo stack. Undo restores the snapshot and refreshes the view. Copy alone is not undoable (it changes nothing); Cut is undoable because it clears.

## Question 1
How should Undo be triggered, given native Ctrl+Z must keep working for text editing inside a macro line?

A) Add an "Edit" menu with Undo (and Redo) items, no global hotkey

B) Use Ctrl+Z, but only when focus is NOT in a macro line textbox (textbox keeps its own Ctrl+Z); add menu items too

C) Use a separate hotkey that never conflicts (e.g. Ctrl+Shift+Z for undo), plus menu items

X) Other (describe)

[Answer]: A

## Question 2
Do you want Redo as well, or undo-only?

A) Undo only

B) Undo and Redo

X) Other (describe)

[Answer]: B

## Question 3
Besides the copy-menu paste/clear/cut, the app has other destructive overwrite actions: Broadcast (copy a page/macro/row/line to ALL books) and Text Import (overwrite a book/page/row from a file). Should those also be undoable?

A) No - only the paste/clear/cut menu actions I asked for

B) Yes - also make Broadcast undoable

C) Yes - make both Broadcast and Text Import undoable

X) Other (describe)

[Answer]: B

## Question 4
When should the undo history be cleared, and how deep should it go?

A) Clear on File Open and on full Save; unlimited-ish depth (cap at 50 actions)

B) Clear on File Open only (keep across saves); cap at 50

C) Never auto-clear during a session; cap at 50

X) Other (describe e.g. different cap or rules)

[Answer]: A

## Question 5
Confirm the scope mapping above (Book / Page = MenuRow / Row = Ctrl-Alt side / Macro) is what you mean by "books, pages, rows, and macros."

A) Yes, that mapping is correct

B) No (describe what differs)

X) Other (describe)

[Answer]: A
