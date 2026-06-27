# Unit 15 - Partial Load + Fix-Each-Problem Import Workflow

Answer each by filling the letter after `[Answer]:`. Option X = Other (describe).

## What the code does today

`MacroTextSerializer.Parse` handles two formats (AI JSON, Human text). Import currently behaves in two ways:

- **Hard failure** (whole import aborts): empty file, no AI markers / human header, missing BEGIN/END markers, JSON that does not parse, no `books` array. The user just gets an error message.
- **Soft problems** (import proceeds, bad items skipped, warnings summarized at end): invalid slot name, book number out of range, page number out of range, wrong line count (auto-padded to 6), title > 8 chars (auto-truncated).

Entry points: `File > Import Text` (put-back to original positions), `Import Over Book/Page/Row` (targeted), and `File > Import All` (bulk folder).

## What I think you want

A repair workflow: load the good parts, then walk the user through each problem one at a time in an in-app text editor so they can fix or skip it, re-validate, and continue — instead of an all-or-nothing failure or silent skips.

## Question 1
What should the "text editor" be?

A) An in-app dialog: shows the relevant snippet (the broken macro, or for a syntax error the whole file text), an editable box, the problem description, and Fix/Skip/Cancel buttons

B) An in-app dialog that always shows the WHOLE file text for every problem (simpler, less guided)

C) Launch the system default editor (Notepad) on the file, then re-import when the user returns

X) Other (describe)

[Answer]: A

## Question 2
Two-stage handling: a malformed JSON file cannot be partially read until it parses. Proposed flow:

- Stage 1 - File-level syntax (JSON won't parse / markers missing): open the whole-file editor showing the parser error; user edits; "Re-check" re-parses; loop until it parses or the user cancels.
- Stage 2 - Per-macro problems (invalid slot, out-of-range book/page): walk these one by one in the editor.

Is that the right model?

A) Yes - Stage 1 (file syntax) then Stage 2 (per-macro), as described

B) Only Stage 2 (per-macro problems); if the JSON itself won't parse, just fail as it does today

C) Only Stage 1 (let me fix raw text until it parses); keep today's silent-skip behavior for per-macro issues

X) Other (describe)

[Answer]: A

## Question 3
Which per-macro problems should pause for a fix in Stage 2? (Today these are skipped or auto-corrected.)

A) Only the ones currently skipped entirely: invalid slot name, book out of range, page out of range

B) Those, plus warn/pause on title > 8 chars and wrong line count (instead of silently truncating/padding)

X) Other (describe)

[Answer]: B

## Question 4
For each problem, what actions should the user have?

A) Fix (edit then re-validate), Skip this one (don't import it), Cancel entire import

B) Fix, Skip, Skip All Remaining, Cancel

X) Other (describe)

[Answer]: B

## Question 5
Partial load timing:

A) Apply the valid macros as I go / at the end, importing everything that is (or becomes) valid; only problems pause the flow

B) Do not import anything until every problem is fixed or skipped, then apply once at the end

X) Other (describe)

[Answer]: B

> CLARIFICATION (added after answering): The "do not import anything until every problem is fixed/skipped, then apply once" rule applies to **single-file import only**. If a single-file import has any problem, nothing loads until the whole file is resolved (every problem fixed or skipped), then it applies.

## Question 6
Which entry points get the repair workflow?

A) The single-file imports only (Import Text, Import Over Book/Page/Row); leave bulk folder Import All as-is (summary only)

B) All import entry points including bulk folder import

X) Other (describe)

[Answer]: X
Bulk Folder Import should tell the user which files have problems. The user can then
do a single file import on that file to engage the correction workflow.

> CLARIFICATION (added after answering): For bulk folder import, the "apply only at the
> end" rule does NOT apply. Process each file sequentially and apply clean files as you go.
> A file with ANY problem (hard parse error or per-macro problem) is NOT loaded at all; it
> is collected and reported at the end so the user can import it individually and run the
> correction workflow on it.

## Question 7
After the user fixes problems in the editor, should I offer to save the corrected text back to the original file (so it is not broken next time)?

A) Yes - after a successful repaired import, offer to overwrite the source file with the corrected text

B) No - only import into the editor; never touch the source file

X) Other (describe)

[Answer]: A

## Question 8
Which formats does this apply to?

A) Both AI JSON and Human formats

B) AI JSON only (the format most likely to be hand/AI-edited and broken)

X) Other (describe)

[Answer]: A
