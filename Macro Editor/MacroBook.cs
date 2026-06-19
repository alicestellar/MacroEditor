using System.Collections.Generic;

namespace MacroEditor
{
    public class MacroBook
    {
        public string Name { get; set; }
        public List<MacroRow> Rows { get; set; }

        public MacroBook()
        {
            Name = "";
            Rows = new List<MacroRow>();
            for (int i = 0; i < 10; i++)
                Rows.Add(new MacroRow());
        }

        public MacroBook(string name) : this()
        {
            Name = name;
        }

        public MacroBook Clone()
        {
            var clone = new MacroBook();
            clone.Name = this.Name;
            clone.Rows.Clear();
            foreach (var r in Rows)
                clone.Rows.Add(r.Clone());
            return clone;
        }
    }
}
