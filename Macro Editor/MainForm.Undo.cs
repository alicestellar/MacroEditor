using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MacroEditor
{
    // Unit 9: Undo/Redo for paste, clear, cut (book/page/row/macro) and broadcast.
    // In-macro line text editing is intentionally NOT handled here - it keeps the
    // native TextBox Ctrl+Z. This feature is invoked from the Edit menu only.
    public partial class MainForm
    {
        private readonly UndoManager _undoManager = new UndoManager(50);
        private ToolStripMenuItem _editMenu;
        private ToolStripMenuItem _editUndo;
        private ToolStripMenuItem _editRedo;

        // Re-entrancy guard: ensures one user action (which may internally invoke
        // other handlers via PerformClick) records exactly one undo entry capturing
        // the original state. Reset after the current UI message completes.
        private bool _suppressUndoRecord = false;

        /// <summary>Build the Edit menu (Undo/Redo) and insert it after the File menu.</summary>
        private void InitUndoUi()
        {
            _editUndo = new ToolStripMenuItem("Undo");
            _editUndo.Click += delegate { this.DoUndo(); };
            _editRedo = new ToolStripMenuItem("Redo");
            _editRedo.Click += delegate { this.DoRedo(); };

            _editMenu = new ToolStripMenuItem("Edit");
            _editMenu.DropDownItems.Add(_editUndo);
            _editMenu.DropDownItems.Add(_editRedo);

            int insertAt = this.MainMenu.Items.IndexOf(this.FileMenu) + 1;
            if (insertAt < 1 || insertAt > this.MainMenu.Items.Count) insertAt = this.MainMenu.Items.Count;
            this.MainMenu.Items.Insert(insertAt, _editMenu);

            UpdateUndoRedoMenu();
        }

        private void UpdateUndoRedoMenu()
        {
            if (_editUndo == null) return;
            _editUndo.Enabled = _undoManager.CanUndo;
            _editUndo.Text = _undoManager.CanUndo ? ("Undo " + _undoManager.UndoLabel) : "Undo";
            _editRedo.Enabled = _undoManager.CanRedo;
            _editRedo.Text = _undoManager.CanRedo ? ("Redo " + _undoManager.RedoLabel) : "Redo";
        }

        /// <summary>
        /// Snapshot the given books BEFORE a mutating action so it can be undone.
        /// Call at the very start of each paste/clear/cut handler (before it mutates Books).
        /// Nested calls within the same user action are ignored so only the original
        /// state is captured.
        /// </summary>
        private void RecordUndo(string label, params int[] bookIndices)
        {
            if (_suppressUndoRecord) return;
            UndoEntry entry = BuildUndoEntry(label, bookIndices);
            if (entry == null) return;
            _undoManager.Record(entry);
            UpdateUndoRedoMenu();

            _suppressUndoRecord = true;
            if (this.IsHandleCreated)
                this.BeginInvoke((Action)delegate { _suppressUndoRecord = false; });
            else
                _suppressUndoRecord = false;
        }

        /// <summary>Snapshot every book (used by broadcast, which writes to all books).</summary>
        private void RecordUndoAllBooks(string label)
        {
            int[] all = Enumerable.Range(0, this.Books.Count).ToArray();
            RecordUndo(label, all);
        }

        private UndoEntry BuildUndoEntry(string label, int[] bookIndices)
        {
            if (bookIndices == null || bookIndices.Length == 0) return null;
            var entry = new UndoEntry { Label = label, Books = new List<BookSnapshot>() };
            foreach (int b in bookIndices.Distinct())
            {
                if (b < 0 || b >= this.Books.Count) continue;
                entry.Books.Add(new BookSnapshot
                {
                    BookIndex = b,
                    Book = this.Books[b].Clone(),
                    DisplayName = (b < this.Contents.Items.Count) ? this.Contents.Items[b].ToString() : null
                });
            }
            return entry.Books.Count > 0 ? entry : null;
        }

        private void DoUndo()
        {
            if (!_undoManager.CanUndo) return;
            UndoEntry entry = _undoManager.TakeUndo();
            int[] indices = entry.Books.Select(b => b.BookIndex).ToArray();
            UndoEntry redoState = BuildUndoEntry(entry.Label, indices); // current state -> redo
            ApplyUndoEntry(entry);
            if (redoState != null) _undoManager.PushRedo(redoState);
            UpdateUndoRedoMenu();
            this.UpdateStatusBar("Undo", entry.Label);
        }

        private void DoRedo()
        {
            if (!_undoManager.CanRedo) return;
            UndoEntry entry = _undoManager.TakeRedo();
            int[] indices = entry.Books.Select(b => b.BookIndex).ToArray();
            UndoEntry undoState = BuildUndoEntry(entry.Label, indices); // current state -> undo
            ApplyUndoEntry(entry);
            if (undoState != null) _undoManager.PushUndoKeepRedo(undoState);
            UpdateUndoRedoMenu();
            this.UpdateStatusBar("Redo", entry.Label);
        }

        private void ApplyUndoEntry(UndoEntry entry)
        {
            this.Contents.SelectedIndexChanged -= this.Contents_SelectedIndexChanged;
            foreach (BookSnapshot snap in entry.Books)
            {
                if (snap.BookIndex < 0 || snap.BookIndex >= this.Books.Count) continue;
                // Clone so the stored memento stays pristine across repeated undo/redo.
                this.Books[snap.BookIndex] = snap.Book.Clone();
                if (snap.DisplayName != null && snap.BookIndex < this.Contents.Items.Count)
                    this.Contents.Items[snap.BookIndex] = snap.DisplayName;
            }
            this.Contents.SelectedIndexChanged += this.Contents_SelectedIndexChanged;

            this.SomethingEdited = true;
            // Repaint the currently displayed page.
            if (this.Rows != null && this.Rows.ContainsKey(this.xRow))
                this.Rows[this.xRow].PerformClick();
        }

        /// <summary>Clear undo/redo history (on File Open and on full Save).</summary>
        private void ClearUndoHistory()
        {
            _undoManager.Clear();
            UpdateUndoRedoMenu();
        }
    }
}
