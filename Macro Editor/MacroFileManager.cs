using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacroEditor
{
    public class MacroFileManager
    {
        private static string _logPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            "crash.log");
        private static void Log(string msg) { System.IO.File.AppendAllText(_logPath, msg + "\n"); }

        private AutoTranslateEncoder atEncoder;
        private Regex lfs;

        public MacroFileManager(AutoTranslateEncoder encoder)
        {
            this.atEncoder = encoder;
            this.lfs = new Regex("\\r|\\n");
        }

        /// <summary>
        /// Reads book names from a mcr.ttl file.
        /// Returns a list of book name strings.
        /// </summary>
        public List<string> ReadBookNames(string ttlFilePath)
        {
            Log("MacroFileManager: ReadBookNames from " + ttlFilePath);
            byte[] array = File.ReadAllBytes(ttlFilePath);
            string text = "";
            checked
            {
                int num = array.Length - 1;
                for (int i = 0; i <= num; i++)
                {
                    text += Conversions.ToString(Convert.ToChar(array[i]));
                }
            }
            Match match = new Regex("((.{15}\\x00)+$)").Match(text);
            text = match.Groups[1].ToString();
            text = new Regex("\0+").Replace(text, ",");
            string[] array2 = text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(array2);
        }

        /// <summary>
        /// Reads a single .dat file and returns a populated MacroRow.
        /// Returns null if the file doesn't exist.
        /// </summary>
        public MacroRow ReadMacroRow(string datFilePath)
        {
            Log("MacroFileManager: ReadMacroRow from " + datFilePath);
            if (!File.Exists(datFilePath))
            {
                return null;
            }
            byte[] array = File.ReadAllBytes(datFilePath);
            string text = "";
            checked
            {
                int num = array.Length - 1;
                for (int i = 24; i <= num; i++)
                {
                    text += Conversions.ToString(Convert.ToChar(array[i]));
                }
                text = text.Replace('\0', '\n');
                MacroRow row = new MacroRow();
                int num2 = 0;
                int num3 = text.Length - 1;
                for (int j = 0; j <= num3; j += 380)
                {
                    int num4 = 1;
                    row.Macros[num2] = Macro.FromArray(new string[]
                    {
                        this.lfs.Replace(text.Substring(j + 370, 8).Trim(), ""),
                        "",
                        "",
                        "",
                        "",
                        "",
                        ""
                    });
                    int num5 = 0;
                    do
                    {
                        row.Macros[num2][num4] = this.atEncoder.Decode(this.lfs.Replace(text.Substring(j + num5, 60).Trim(), ""));
                        num4++;
                        num5 += 61;
                    }
                    while (num5 <= 360);
                    num2++;
                }
                return row;
            }
        }

        /// <summary>
        /// Writes a single row's macros to a .dat file.
        /// </summary>
        public bool WriteRow(string macroPath, int bookIndex, int rowIndex, MacroRow row, MacroRow preservedRow, int macroCount)
        {
            Log("MacroFileManager: WriteRow book=" + bookIndex + " row=" + rowIndex);
            string text = macroPath + "\\mcr" + MacroEditorUtils.GetMacroFileSuffix(bookIndex, rowIndex) + ".dat";
            bool flag = File.Exists(text);
            byte[] sourceArray;
            if (flag)
            {
                try
                {
                    sourceArray = File.ReadAllBytes(text);
                }
                catch (Exception ex)
                {
                    Interaction.MsgBox("Cannot read " + text + ".", MsgBoxStyle.OkOnly, null);
                    return false;
                }
            }
            else
            {
                byte[] array = new byte[8];
                array[0] = 1;
                sourceArray = array;
            }
            StringBuilder stringBuilder = new StringBuilder();
            int num = macroCount;
            checked
            {
                for (int i = 0; i <= num; i++)
                {
                    preservedRow.Macros[i] = row.Macros[i].Clone();
                    stringBuilder.Append(this.Fill("", 4));
                    int num2 = 1;
                    do
                    {
                        stringBuilder.Append(this.Fill(this.atEncoder.Encode(row.Macros[i][num2]), 61));
                        num2++;
                    }
                    while (num2 <= 6);
                    stringBuilder.Append(this.Fill(row.Macros[i][0].Substring(0, Math.Min(row.Macros[i][0].Length, 8)), 9));
                    stringBuilder.Append('\0');
                }
                bool flag2 = stringBuilder.Length != 7600;
                bool result;
                if (flag2)
                {
                    Interaction.MsgBox("Compilation of " + MacroEditorUtils.GetMacroFileSuffix(bookIndex, rowIndex) + "failed.", MsgBoxStyle.OkOnly, null);
                    result = false;
                }
                else
                {
                    byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
                    byte[] array2 = ComputeMD5(bytes);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    int num3 = array2.Length - 1;
                    for (int j = 0; j <= num3; j++)
                    {
                        stringBuilder2.Append(Strings.Chr((int)array2[j]));
                    }
                    byte[] array3 = new byte[8 + array2.Length + bytes.Length - 1 + 1];
                    Array.Copy(sourceArray, 0, array3, 0, 8);
                    Array.Copy(array2, 0, array3, 8, 16);
                    Array.Copy(bytes, 0, array3, 24, bytes.Length);
                    try
                    {
                        File.WriteAllBytes(text, array3);
                        result = true;
                    }
                    catch (Exception ex2)
                    {
                        result = false;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Writes book names to mcr.ttl file.
        /// </summary>
        public bool WriteBookNames(string macroPath, List<string> bookNames)
        {
            Log("MacroFileManager: WriteBookNames to " + macroPath);
            string path = macroPath + "\\mcr.ttl";
            byte[] sourceArray = File.ReadAllBytes(path);
            StringBuilder stringBuilder = new StringBuilder();
            checked
            {
                int num = bookNames.Count - 1;
                for (int i = 0; i <= num; i++)
                {
                    string name = bookNames[i].Trim();
                    string truncated = name.Substring(0, Math.Min(15, name.Length));
                    stringBuilder.Append(this.Fill(truncated, 16));
                }
                bool flag = stringBuilder.Length != 320;
                if (flag)
                {
                    Interaction.MsgBox("Compilation of Macro Book titles failed", MsgBoxStyle.OkOnly, null);
                    return false;
                }
                else
                {
                    byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
                    byte[] array4 = ComputeMD5(bytes);
                    StringBuilder stringBuilder3 = new StringBuilder();
                    int num3 = array4.Length - 1;
                    for (int j = 0; j <= num3; j++)
                    {
                        stringBuilder3.Append(Strings.Chr((int)array4[j]));
                    }
                    bool flag2 = stringBuilder.Length != 320;
                    if (flag2)
                    {
                        Interaction.MsgBox("Compilation of Macro Names file failed.", MsgBoxStyle.OkOnly, null);
                        return false;
                    }
                    else
                    {
                        byte[] array5 = new byte[8 + array4.Length + bytes.Length - 1 + 1];
                        Array.Copy(sourceArray, 0, array5, 0, 8);
                        Array.Copy(array4, 0, array5, 8, 16);
                        Array.Copy(bytes, 0, array5, 24, bytes.Length);
                        File.WriteAllBytes(path, array5);
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the .dat filename for a given book and row.
        /// </summary>
        public string GetDatFilePath(string macroPath, int bookIndex, int rowIndex)
        {
            return macroPath + "\\mcr" + MacroEditorUtils.GetMacroFileSuffix(bookIndex, rowIndex) + ".dat";
        }

        /// <summary>
        /// Computes MD5 hash for file integrity.
        /// </summary>
        private byte[] ComputeMD5(byte[] data)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(data);
            }
        }

        /// <summary>
        /// Pads a string with null characters to the specified length.
        /// </summary>
        private string Fill(string str, int targetLength)
        {
            return str + new string('\0', checked(targetLength - str.Length));
        }
    }
}
