using System.Collections.Generic;

namespace MacroEditor
{
    public class MacroRow
    {
        public List<Macro> Macros { get; set; }

        public MacroRow()
        {
            Macros = new List<Macro>();
            for (int i = 0; i < 20; i++)
                Macros.Add(new Macro());
        }

        public MacroRow Clone()
        {
            var clone = new MacroRow();
            clone.Macros.Clear();
            foreach (var m in Macros)
                clone.Macros.Add(m.Clone());
            return clone;
        }
    }
}
