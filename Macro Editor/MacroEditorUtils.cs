using System;

namespace MacroEditor
{
    public static class MacroEditorUtils
    {
        /// <summary>
        /// Generates the numeric suffix for macro .dat filenames.
        /// book=0,row=0 → "", book=0,row=N → "N", book=M,row=N → "MN"
        /// </summary>
        public static string GetMacroFileSuffix(int book, int row)
        {
            if (book == 0)
            {
                if (row == 0)
                    return "";
                else
                    return row.ToString();
            }
            else
            {
                return book.ToString() + row.ToString();
            }
        }
    }
}
