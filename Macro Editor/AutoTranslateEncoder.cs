using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Yekyaa.FFXIEncoding;

namespace MacroEditor
{
    /// <summary>
    /// Encapsulates all Auto-Translate phrase functionality:
    /// loading phrases from FFXIEncoding, encoding/decoding AT markers,
    /// dictionary lookup, and menu building/filtering.
    /// </summary>
    public class AutoTranslateEncoder
    {
        private static string _logPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            "crash.log");
        private static void Log(string msg) { System.IO.File.AppendAllText(_logPath, msg + "\n"); }

        private ToolStripMenuItem ATmenu;
        private Dictionary<string, string> ATObject;
        private Regex ATReadable;
        private Regex ATWritable;
        private Regex lfs;

        /// <summary>
        /// Raised when a phrase is selected from the AT menu.
        /// The string parameter is the AT phrase tag to insert.
        /// </summary>
        public event Action<string> PhraseSelected;

        public AutoTranslateEncoder()
        {
            this.ATmenu = new ToolStripMenuItem();
            this.ATObject = new Dictionary<string, string>();
            this.ATReadable = new Regex("\\xFD(.{4})\\xFD");
            this.ATWritable = new Regex("\\<(.{8})\\|[^<>]+\\>");
            this.lfs = new Regex("\\r|\\n");
        }

        /// <summary>
        /// Loads AT phrases from FFXIEncoding, builds the menu and dictionary.
        /// Extracted from MainForm.ParseAT().
        /// </summary>
        public void LoadPhrases()
        {
            Log("AutoTranslateEncoder: LoadPhrases starting...");
            FFXIATPhraseLoader ffxiatphraseLoader = new FFXIATPhraseLoader();
            FFXIATPhrase[] atphrases = ffxiatphraseLoader.ATPhrases;
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)this.ATmenu.DropDownItems.Add("test");
            checked
            {
                int num = atphrases.Count<FFXIATPhrase>() - 1;
                for (int i = 0; i <= num; i++)
                {
                    bool flag = atphrases[i].value.Trim().Length == 0;
                    if (!flag)
                    {
                        bool flag2 = Operators.CompareString(atphrases[i].value.Substring(0, 1), "\u3010", false) == 0;
                        if (flag2)
                        {
                            toolStripMenuItem = (ToolStripMenuItem)this.ATmenu.DropDownItems.Add(atphrases[i].value);
                        }
                        else
                        {
                            ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem)toolStripMenuItem.DropDownItems.Add(atphrases[i].value);
                            toolStripMenuItem2.Tag = atphrases[i].ToString();
                            this.ATObject.Add(atphrases[i].ToString().Substring(1, 8), atphrases[i].value);
                            toolStripMenuItem2.Click += this.ATPhrase_Click;
                        }
                    }
                }
                this.ATmenu.Text = "Auto-Translate";
            }
            Log("AutoTranslateEncoder: LoadPhrases complete.");
        }

        /// <summary>
        /// Returns the built AT menu item for adding to a MenuStrip.
        /// </summary>
        public ToolStripMenuItem GetATMenu()
        {
            return this.ATmenu;
        }

        /// <summary>
        /// Decodes a raw text string by replacing binary FD markers with readable &lt;HEX|Name&gt; format.
        /// Replaces ATReadable regex usage in ReadMacroFile.
        /// </summary>
        public string Decode(string rawText)
        {
            return this.ATReadable.Replace(rawText, (Match found) => this.DecodePhrase(found.Groups[1].ToString()));
        }

        /// <summary>
        /// Encodes a display text string by replacing &lt;HEX|Name&gt; markers with binary AT format.
        /// Replaces ATWriter method. Also strips line feeds before encoding.
        /// </summary>
        public string Encode(string displayText)
        {
            return this.ATWritable.Replace(this.lfs.Replace(displayText, ""), (Match found) => this.EncodePhrase(found.Groups[1].ToString()));
        }

        /// <summary>
        /// Decodes a single AT phrase from binary chars to &lt;HEX|Name&gt; format.
        /// Extracted from MainForm.ATDecode().
        /// </summary>
        public string DecodePhrase(string phrase)
        {
            string text = "";
            foreach (char value in phrase)
            {
                text += Convert.ToString(Convert.ToInt32(value), 16).PadLeft(2, '0');
            }
            string result;
            try
            {
                result = string.Concat(new string[]
                {
                    "<",
                    text,
                    "|",
                    this.ATObject[text.ToUpper()],
                    ">"
                });
            }
            catch (Exception)
            {
                result = "<" + text + "|UnknownItem>";
            }
            return result;
        }

        /// <summary>
        /// Encodes a single AT phrase from hex string back to binary chars.
        /// Extracted from MainForm.ATEncode().
        /// </summary>
        public string EncodePhrase(string phrase)
        {
            phrase = "FD" + phrase + "FD";
            string text = "";
            checked
            {
                int num = phrase.Length - 2;
                for (int i = 0; i <= num; i += 2)
                {
                    text += Convert.ToChar(Convert.ToUInt32(phrase.Substring(i, 2), 16)).ToString();
                }
                return text;
            }
        }

        /// <summary>
        /// Filters AT menu items by the given search text (prefix match).
        /// Extracted from MainForm.OpenATMenu() filtering logic.
        /// </summary>
        public void FilterMenu(string searchText)
        {
            checked
            {
                int num2 = this.ATmenu.DropDownItems.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)this.ATmenu.DropDownItems[i];
                    toolStripMenuItem.Visible = false;
                    int num3 = toolStripMenuItem.DropDownItems.Count - 1;
                    for (int j = 0; j <= num3; j++)
                    {
                        ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem)toolStripMenuItem.DropDownItems[j];
                        bool flag3 = Operators.CompareString(toolStripMenuItem2.Text.Substring(0, Math.Min(searchText.Length, toolStripMenuItem2.Text.Length)).ToLower(), searchText.ToLower(), false) == 0;
                        if (flag3)
                        {
                            toolStripMenuItem.Visible = true;
                            toolStripMenuItem2.Visible = true;
                        }
                        else
                        {
                            toolStripMenuItem2.Visible = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Looks up a phrase name by hex code in the dictionary.
        /// </summary>
        public string LookupPhrase(string hexCode)
        {
            string result;
            if (this.ATObject.TryGetValue(hexCode.ToUpper(), out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Event handler for AT phrase menu item clicks.
        /// Raises the PhraseSelected event with the phrase tag.
        /// </summary>
        private void ATPhrase_Click(object sender, EventArgs e)
        {
            var handler = this.PhraseSelected;
            if (handler != null)
            {
                string tag = (string)((ToolStripMenuItem)sender).Tag;
                handler(tag);
            }
        }
    }
}
