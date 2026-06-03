using System;
using Microsoft.VisualBasic.CompilerServices;

namespace MacroEditor
{
    public static class MacroEditorUtils
    {
        /// <summary>
        /// Pads a string with null characters to the specified length.
        /// </summary>
        public static string Fill(string str, int targetLength)
        {
            return str + new string('\0', checked(targetLength - str.Length));
        }

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
                return Conversions.ToString(book) + Conversions.ToString(row);
            }
        }

        /// <summary>
        /// Converts index 10 to "0" for FFXI display (10th macro shows as 0).
        /// </summary>
        public static string FormatMacroIndex(int index)
        {
            if (index != 10)
                return index.ToString();
            else
                return "0";
        }

        /// <summary>
        /// Returns "Ctl X" or "Alt X" label from macro index (0-19).
        /// </summary>
        public static string FormatMacroLabel(int macroIndex)
        {
            if (macroIndex > 9)
                return "Alt " + Conversions.ToString(checked(macroIndex - 9));
            else
                return "Ctl " + Conversions.ToString(checked(macroIndex + 1));
        }

        /// <summary>
        /// Validates that clipboard data matches the expected paste format.
        /// Returns true if valid, false otherwise.
        /// </summary>
        public static bool VerifyClipboardFormat(string expectedType, Array data)
        {
            bool valid = NewLateBinding.LateIndexGet(data, new object[] { 0 }, null)
                .ToString().Trim().StartsWith("Type: " + expectedType)
                & NewLateBinding.LateIndexGet(data, new object[] { checked(data.Length - 1) }, null)
                .ToString().Trim().StartsWith("End" + expectedType);

            if (valid)
            {
                return true;
            }
            else
            {
                Microsoft.VisualBasic.Interaction.MsgBox(
                    "Clipboard does not contain a " + expectedType + ".",
                    Microsoft.VisualBasic.MsgBoxStyle.OkOnly, null);
                return false;
            }
        }
    }
}
