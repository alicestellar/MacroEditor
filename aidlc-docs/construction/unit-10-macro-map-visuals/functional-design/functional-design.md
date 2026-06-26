# Functional Design: Unit 10 — Macro Map Visual Enhancements

## Overview

Enhance the existing read-only Macro Map (`MacroMapForm`) with color coding, a pinned header showing CTRL/ALT labels, page-navigation buttons, and paged scrolling. Dividers (from the original plan) are dropped. The window is made taller to accommodate the header toolbar.

## Current State

- `MacroMapForm` is an `AutoScroll` form.
- `MainForm.MenuBook_MacroMap_Click` builds it by calling `Add(book, row, macro, lines[])` for all 200 macros (10 pages × 20 macros).
- Each macro is a grey `Label` (150×100) positioned absolutely:
  - Ctrl macros (m < 10): top row of the page
  - Alt macros (m ≥ 10): bottom row of the page
  - Each "page" = one row pair (Ctrl row + Alt row), stacked vertically by row index `r`
- Clicking a label navigates the main editor to that macro via a callback.

## Layout Restructure

To support a pinned header that doesn't scroll, split the form into two regions:

```text
┌─────────────────────────────────────────────┐
│ HEADER PANEL (docked top, fixed)             │
│  [CTRL] (faded blue)   [ALT] (faded red)     │
│  [▲ Prev Page]            [Next Page ▼]      │  ← page nav buttons
├─────────────────────────────────────────────┤
│ BODY PANEL (docked fill, AutoScroll)         │
│   ┌────┐ ┌────┐ ... (Ctrl row, faded blue)   │
│   ┌────┐ ┌────┐ ... (Alt row, faded red)     │
│   (page 2 ...)                               │
│   ...                                        │
└─────────────────────────────────────────────┘
```

- **Header panel**: `Panel` docked `Top`, fixed height (~70px). Not scrolled. Holds:
  - A `CTRL` label with faded blue background
  - An `ALT` label with faded red background
  - Page navigation buttons (Prev/Next)
  - (Reserved space for Unit 11's Edit/Save buttons — same toolbar)
- **Body panel**: `Panel` docked `Fill`, `AutoScroll = true`. The macro labels move here instead of directly on the form.

The window height increases to account for the header (current content + ~70px header).

## Color Coding

| Element | Background color | Approx RGB |
|---------|------------------|-----------|
| Ctrl macro labels | faded blue | `Color.FromArgb(210, 225, 255)` |
| Alt macro labels | faded red | `Color.FromArgb(255, 215, 215)` |
| CTRL header label | faded blue (same family, slightly stronger) | `Color.FromArgb(170, 200, 255)` |
| ALT header label | faded red (same family, slightly stronger) | `Color.FromArgb(255, 180, 180)` |

Empty macro slots keep the faded color (still indicates Ctrl vs Alt position). Border stays `FixedSingle`.

## Page Navigation Buttons

- **Prev Page** (top of body / in header): scroll up by one page; clamp at first page.
- **Next Page** (bottom or header): scroll down by one page; clamp at last page.
- Each click moves the view to the target page and positions it so the page is at the top (or centered) of the body viewport.
- A page's vertical extent = the Ctrl row + Alt row pair for a given `r`.

## Paged Scrolling — DROPPED

Snap-to-page mouse-wheel scrolling is dropped (nice-to-have only). The mouse wheel scrolls normally via the body panel's `AutoScroll`. Page navigation is handled solely by the Prev/Next buttons, which jump the view to a target page.

## MacroMapForm API Changes

```csharp
// New/changed members
private Panel headerPanel;     // fixed top toolbar
private Panel bodyPanel;       // AutoScroll content host
private Label ctrlHeaderLabel; // "CTRL" pinned, faded blue
private Label altHeaderLabel;  // "ALT" pinned, faded red
private Button prevPageButton;
private Button nextPageButton;
private int currentPage;       // 0-9
private int pageHeight;        // computed page stride in px
private int totalPages;        // number of populated pages (default 10)

// Add() places labels into bodyPanel instead of the form,
// and sets faded blue/red background based on Ctrl vs Alt.
public object Add(int b, int r, int m, string[] a);

// Page navigation
private void GoToPage(int page);
private void PrevPage_Click(object sender, EventArgs e);
private void NextPage_Click(object sender, EventArgs e);
// Mouse wheel scrolls normally (default AutoScroll behavior).
```

## Behavior Preservation

- Clicking a macro label still navigates the main editor (existing callback unchanged).
- The read-only nature is preserved (Unit 11 adds the editable variant later).
- `MenuBook_MacroMap_Click` in MainForm stays the same (still calls `Add` 200 times) — the coloring is decided inside `Add` based on `m`.
- The "Copy Macro Map" feature (`MenuBook_CopyMacroMap_Click`) is unaffected.

## Edge Cases

| Scenario | Behavior |
|----------|----------|
| Fewer than 10 populated pages | `totalPages` reflects actual; nav clamps accordingly |
| Mouse wheel at first/last page | No movement past bounds |
| Window resized | Header stays docked top; body re-flows with AutoScroll |
| Clicking nav rapidly | Each click moves exactly one page; clamped |

## Testing Scenarios

1. Open macro map → Ctrl rows are faded blue, Alt rows faded red, no grey.
2. CTRL/ALT header labels visible and stay pinned while scrolling.
3. Page navigation buttons jump the view one page at a time; mouse wheel scrolls normally.
4. Prev/Next page buttons move exactly one page and clamp at ends.
5. Click a macro → main editor navigates to it (unchanged).
6. Window is tall enough that the header toolbar + first page are visible without overlap.
