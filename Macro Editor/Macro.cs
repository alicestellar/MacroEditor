using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroEditor
{
    public class Macro
    {
        public string Title { get; set; }
        public List<string> Lines { get; set; }

        public Macro()
        {
            Title = "";
            Lines = new List<string> { "", "", "", "", "", "" };
        }

        public Macro Clone()
        {
            return new Macro
            {
                Title = this.Title,
                Lines = new List<string>(this.Lines)
            };
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Title) && Lines.All(l => string.IsNullOrEmpty(l));
        }

        /// <summary>
        /// Access macro data by index: 0 = Title, 1-6 = Lines[0]-Lines[5]
        /// This provides backward-compatible indexed access during migration.
        /// </summary>
        public string this[int index]
        {
            get
            {
                if (index == 0) return Title;
                if (index >= 1 && index <= 6) return Lines[index - 1];
                throw new IndexOutOfRangeException("Macro index must be 0 (title) or 1-6 (lines)");
            }
            set
            {
                if (index == 0) Title = value;
                else if (index >= 1 && index <= 6) Lines[index - 1] = value;
                else throw new IndexOutOfRangeException("Macro index must be 0 (title) or 1-6 (lines)");
            }
        }

        /// <summary>
        /// Returns the macro as a string array [title, line1..line6] for backward compatibility.
        /// </summary>
        public string[] ToArray()
        {
            var arr = new string[7];
            arr[0] = Title;
            for (int i = 0; i < 6; i++)
                arr[i + 1] = Lines[i];
            return arr;
        }

        /// <summary>
        /// Creates a Macro from a string array [title, line1..line6].
        /// </summary>
        public static Macro FromArray(string[] data)
        {
            var m = new Macro();
            if (data == null || data.Length == 0) return m;
            m.Title = data.Length > 0 ? (data[0] ?? "") : "";
            for (int i = 0; i < 6 && i + 1 < data.Length; i++)
                m.Lines[i] = data[i + 1] ?? "";
            return m;
        }
    }
}
