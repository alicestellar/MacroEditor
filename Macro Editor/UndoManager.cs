using System.Collections.Generic;

namespace MacroEditor
{
    /// <summary>
    /// Snapshot of a single book's full contents plus the name shown for it in the
    /// book list. Used as a memento for undo/redo of paste, clear, cut and broadcast.
    /// </summary>
    public class BookSnapshot
    {
        public int BookIndex { get; set; }
        public MacroBook Book { get; set; }
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// A single undoable user action. Holds a snapshot of every book the action
    /// affected (one book for scoped paste/clear/cut, all books for broadcast).
    /// </summary>
    public class UndoEntry
    {
        public string Label { get; set; }
        public List<BookSnapshot> Books { get; set; }
    }

    /// <summary>
    /// Fixed-depth undo/redo history of <see cref="UndoEntry"/> mementos.
    /// Backed by lists used as stacks (append/remove at the end) so the oldest
    /// entry can be trimmed when the depth cap is exceeded.
    /// </summary>
    public class UndoManager
    {
        private readonly List<UndoEntry> _undo = new List<UndoEntry>();
        private readonly List<UndoEntry> _redo = new List<UndoEntry>();
        private readonly int _maxDepth;

        public UndoManager(int maxDepth = 50)
        {
            _maxDepth = maxDepth > 0 ? maxDepth : 50;
        }

        public bool CanUndo { get { return _undo.Count > 0; } }
        public bool CanRedo { get { return _redo.Count > 0; } }
        public string UndoLabel { get { return CanUndo ? _undo[_undo.Count - 1].Label : ""; } }
        public string RedoLabel { get { return CanRedo ? _redo[_redo.Count - 1].Label : ""; } }

        /// <summary>Record a new user action. Clears the redo stack (a new edit branch).</summary>
        public void Record(UndoEntry entry)
        {
            if (entry == null) return;
            _undo.Add(entry);
            TrimToDepth(_undo);
            _redo.Clear();
        }

        public UndoEntry TakeUndo()
        {
            if (!CanUndo) return null;
            UndoEntry e = _undo[_undo.Count - 1];
            _undo.RemoveAt(_undo.Count - 1);
            return e;
        }

        public UndoEntry TakeRedo()
        {
            if (!CanRedo) return null;
            UndoEntry e = _redo[_redo.Count - 1];
            _redo.RemoveAt(_redo.Count - 1);
            return e;
        }

        /// <summary>Push the pre-undo state onto the redo stack (during an undo).</summary>
        public void PushRedo(UndoEntry entry)
        {
            if (entry == null) return;
            _redo.Add(entry);
            TrimToDepth(_redo);
        }

        /// <summary>Push onto the undo stack during a redo, WITHOUT clearing the redo stack.</summary>
        public void PushUndoKeepRedo(UndoEntry entry)
        {
            if (entry == null) return;
            _undo.Add(entry);
            TrimToDepth(_undo);
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
        }

        private void TrimToDepth(List<UndoEntry> list)
        {
            while (list.Count > _maxDepth)
                list.RemoveAt(0);
        }
    }
}
