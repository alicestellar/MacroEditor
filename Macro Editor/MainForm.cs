using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MacroEditor.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using Yekyaa.FFXIEncoding;

namespace MacroEditor
{
	// Token: 0x0200000A RID: 10
	[DesignerGenerated]
	public partial class MainForm : Form
	{
		// Token: 0x0600003C RID: 60 RVA: 0x000033E8 File Offset: 0x000015E8
		private static string _logPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "crash.log");
		private static void Log(string msg) { System.IO.File.AppendAllText(_logPath, msg + "\n"); }

		public MainForm()
		{
			Log("Constructor: start");
			base.KeyDown += this.Form1_KeyDown;
			base.Shown += this.Form1_Shown;
			base.Load += this.Form1_Load;
			base.Closing += this.Form1_Closing;
			base.Resize += this.Form1_Resize;
			base.FormClosing += this.MainForm_FormClosing;
			base.MouseWheel += this.MainForm_MouseWheel;
			Log("Constructor: events wired");
			this.Ctrls = new Dictionary<int, Button>();
			this.Rows = new Dictionary<int, Button>();
			this.Lines = new Dictionary<int, TextBox>();
			Log("Constructor: before GetFFXIDirectory");
			this.macropath = Conversions.ToString(Operators.ConcatenateObject(this.GetFFXIDirectory(), "USER\\"));
			this.importpath = Conversions.ToString(Operators.ConcatenateObject(this.GetFFXIDirectory(), "USER\\"));
			Log("Constructor: after GetFFXIDirectory");
			this.MacroContainer = new string[20, 10, 20][];
			this.MacroPreserved = new string[20, 10, 20][];
			this.Contents = new ListBox();
			this.bWidth = 60;
			this.bHeight = 50;
			this.cbook = 0;
			this.xBook = 0;
			this.xRow = 0;
			this.xMacro = 0;
			this.handlerStart = 0;
			this.handlerEnd = 9;
			this.RowHolder = new string[20][];
			this.BookHolder = new string[10, 20][];
			this.debuglimit = 19;
			this.copiedbookname = "";
			this.ATmenu = new ToolStripMenuItem();
			this.ATObject = new Dictionary<string, string>();
			this.CurrentLine = -1;
			this.InternalClipboardMethod = "";
			this.SomethingEdited = false;
			Log("Constructor: before Resizer/Regex");
			Log("Constructor: before Resizer/Regex");
			this.rs = new Resizer();
			this.ATReadable = new Regex("\\xFD(.{4})\\xFD");
			this.ATWritable = new Regex("\\<(.{8})\\|[^<>]+\\>");
			this.lfs = new Regex("\\r|\\n");
			this.Evaluation = null;
			this.SearchResults = null;
			this.MacroMap = null;
			Log("Constructor: before InitializeComponent");
			this.InitializeComponent();
			Log("Constructor: done");
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000367C File Offset: 0x0000187C
		public bool FindMacro(int b, int r, int m)
		{
			this.Contents.SelectedIndex = b;
			this.xRow = r;
			this.xMacro = m;
			this.Rows[r].PerformClick();
			return true;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003700 File Offset: 0x00001900
		private bool FindFFXIDirectory()
		{
			this.FolderBrowserDialog1.SelectedPath = this.macropath;
			DialogResult dialogResult = this.FolderBrowserDialog1.ShowDialog();
			bool flag = dialogResult == DialogResult.OK;
			if (flag)
			{
				MySettingsProperty.Settings.UserDirectory = this.FolderBrowserDialog1.SelectedPath;
			}
			MySettingsProperty.Settings.Save();
			return true;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000375C File Offset: 0x0000195C
		public bool ParseAT()
		{
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
						bool flag2 = Operators.CompareString(atphrases[i].value.Substring(0, 1), "【", false) == 0;
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
				this.MenuText.Items.Add(this.ATmenu);
				return true;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000038B4 File Offset: 0x00001AB4
		public string ATWriter(string macroline)
		{
			return this.ATWritable.Replace(this.lfs.Replace(macroline, ""), (Match found) => this.ATEncode(found.Groups[1].ToString()));
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000038F0 File Offset: 0x00001AF0
		public bool WriteFile(int Book, int Row)
		{
			string text = this.macropath + "\\mcr" + MacroEditorUtils.GetMacroFileSuffix(Book, Row) + ".dat";
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
			int num = this.debuglimit;
			checked
			{
				for (int i = 0; i <= num; i++)
				{
					this.MacroPreserved[Book, Row, i] = (string[])this.MacroContainer[Book, Row, i].Clone();
					stringBuilder.Append(MacroEditorUtils.Fill("", 4));
					int num2 = 1;
					do
					{
						stringBuilder.Append(MacroEditorUtils.Fill(this.ATWriter(this.MacroContainer[Book, Row, i][num2]), 61));
						num2++;
					}
					while (num2 <= 6);
					stringBuilder.Append(MacroEditorUtils.Fill(this.MacroContainer[Book, Row, i][0].Substring(0, Math.Min(this.MacroContainer[Book, Row, i][0].Length, 8)), 9));
					stringBuilder.Append('\0');
				}
				bool flag2 = stringBuilder.Length != 7600;
				bool result;
				if (flag2)
				{
					Interaction.MsgBox("Compilation of " + MacroEditorUtils.GetMacroFileSuffix(Book, Row) + "failed.", MsgBoxStyle.OkOnly, null);
					result = false;
				}
				else
				{
					byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
					MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
					byte[] array2 = md5CryptoServiceProvider.ComputeHash(bytes);
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

		// Token: 0x06000045 RID: 69 RVA: 0x00003B70 File Offset: 0x00001D70
		public string ATDecode(string phrase)
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
			catch (Exception ex)
			{
				result = "<" + text + "|UnknownItem>";
			}
			return result;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003C34 File Offset: 0x00001E34
		public string ATEncode(string phrase)
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

		// Token: 0x06000047 RID: 71 RVA: 0x00003C98 File Offset: 0x00001E98
		public string CleanClipBoard()
		{
			bool flag = Clipboard.ContainsText(TextDataFormat.Text);
			string result;
			if (flag)
			{
				Regex regex = new Regex("(\\r\\n|\\r|\\n)");
				result = regex.Replace(Clipboard.GetText(), "\n");
			}
			else
			{
				Interaction.MsgBox("Clipboard does not contain regular text.", MsgBoxStyle.OkOnly, null);
				result = "";
			}
			return result;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003DAC File Offset: 0x00001FAC
		public bool ReadMacroFile(int mbook, int mset, string filename, bool PreserveMacros)
		{
			byte[] array = File.ReadAllBytes(filename);
			string text = "";
			checked
			{
				int num = array.Length - 1;
				for (int i = 24; i <= num; i++)
				{
					text += Conversions.ToString(Convert.ToChar(array[i]));
				}
				text = text.Replace('\0', '\n');
				int num2 = 0;
				bool result = true;
				int num3 = text.Length - 1;
				for (int j = 0; j <= num3; j += 380)
				{
					int num4 = 1;
					this.MacroContainer[mbook, mset, num2] = new string[]
					{
						this.lfs.Replace(text.Substring(j + 370, 8).Trim(), ""),
						"",
						"",
						"",
						"",
						"",
						""
					};
					if (PreserveMacros)
					{
						this.MacroPreserved[mbook, mset, num2] = (string[])this.MacroContainer[mbook, mset, num2].Clone();
					}
					int num5 = 0;
					do
					{
						this.MacroContainer[mbook, mset, num2][num4] = this.ATReadable.Replace(this.lfs.Replace(text.Substring(j + num5, 60).Trim(), ""), (Match found) => this.ATDecode(found.Groups[1].ToString()));
						this.MacroPreserved[mbook, mset, num2][num4] = Conversions.ToString(this.MacroContainer[mbook, mset, num2][num4].Clone());
						num4++;
						num5 += 61;
					}
					while (num5 <= 360);
					num2++;
				}
				return result;
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003F64 File Offset: 0x00002164
		public bool OpenATMenu()
		{
			bool flag = !this.MenuMacro.Enabled;
			checked
			{
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					TextBox textBox = this.Lines[this.CurrentLine];
					bool flag2 = textBox.SelectionLength == 0;
					if (flag2)
					{
						int selectionStart = textBox.SelectionStart;
						int num = textBox.Text.Substring(0, textBox.SelectionStart).LastIndexOf(" ") + 1;
						textBox.Select(num, selectionStart - num);
					}
					string selectedText = textBox.SelectedText;
					int num2 = this.ATmenu.DropDownItems.Count - 1;
					for (int i = 0; i <= num2; i++)
					{
						ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)this.ATmenu.DropDownItems[i];
						toolStripMenuItem.Visible = false;
						int num3 = toolStripMenuItem.DropDownItems.Count - 1;
						for (int j = 0; j <= num3; j++)
						{
							ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem)toolStripMenuItem.DropDownItems[j];
							bool flag3 = Operators.CompareString(toolStripMenuItem2.Text.Substring(0, Math.Min(selectedText.Length, toolStripMenuItem2.Text.Length)).ToLower(), selectedText.ToLower(), false) == 0;
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
					result = true;
				}
				return result;
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000040D8 File Offset: 0x000022D8
		public object GetFFXIDirectory()
		{
			string text = string.Empty;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\PlayOnlineUS\\InstallFolder");
			bool flag = registryKey == null;
			if (flag)
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\PlayOnlineEU\\InstallFolder");
			}
			bool flag2 = registryKey == null;
			if (flag2)
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\PlayOnline\\InstallFolder");
			}
			bool flag3 = registryKey == null;
			if (flag3)
			{
				registryKey = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\PlayOnlineUS\\InstallFolder");
			}
			bool flag4 = registryKey == null;
			if (flag4)
			{
				registryKey = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\PlayOnlineEU\\InstallFolder");
			}
			bool flag5 = registryKey == null;
			if (flag5)
			{
				registryKey = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\PlayOnline\\InstallFolder");
			}
			bool flag6 = registryKey != null && registryKey.GetValue("0001") != null;
			object result;
			if (flag6)
			{
				text = (string)registryKey.GetValue("0001");
				result = string.Format("{0}\\", text.TrimEnd(new char[]
				{
					'\\'
				}));
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000041DC File Offset: 0x000023DC
		public object UpdateStatusBar(string h, string d = "")
		{
			bool flag = Operators.CompareString(h, "0", false) != 0;
			if (flag)
			{
				this.StatusH.Text = h;
			}
			this.StatusD.Text = d;
			return true;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004224 File Offset: 0x00002424
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			bool flag = keyData == (Keys)131158;
			if (flag)
			{
				bool flag2 = base.ActiveControl.Name.StartsWith("Lines");
				if (flag2)
				{
					this.menuText_Paste.PerformClick();
					return true;
				}
				bool flag3 = base.ActiveControl.Name.StartsWith("Ctrl") & this.CleanClipBoard().StartsWith("Type: Macro");
				if (flag3)
				{
					this.menuText_Paste.PerformClick();
					return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000042B0 File Offset: 0x000024B0
		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			bool flag = !this.Contents.Enabled;
			checked
			{
				if (!flag)
				{
					bool control = e.Control;
					if (control)
					{
						bool flag2 = e.KeyCode == Keys.Left;
						if (flag2)
						{
							bool flag3 = this.xMacro >= 10;
							if (flag3)
							{
								this.xMacro = -1;
							}
							this.Ctrls[Math.Max(0, this.xMacro - 1)].PerformClick();
							e.SuppressKeyPress = true;
						}
						else
						{
							bool flag4 = e.KeyCode == Keys.Right;
							if (flag4)
							{
								bool flag5 = this.xMacro >= 10;
								if (flag5)
								{
									this.xMacro = -1;
								}
								this.Ctrls[Math.Min(9, this.xMacro + 1)].PerformClick();
								e.SuppressKeyPress = true;
							}
							else
							{
								bool flag6 = e.KeyCode == Keys.Up;
								if (flag6)
								{
									this.Rows[Math.Max(this.xRow - 1, 0)].PerformClick();
									e.SuppressKeyPress = true;
								}
								else
								{
									bool flag7 = e.KeyCode == Keys.Down;
									if (flag7)
									{
										this.Rows[Math.Min(this.xRow + 1, 9)].PerformClick();
										e.SuppressKeyPress = true;
									}
									else
									{
										bool flag8 = e.KeyCode >= Keys.D0 & e.KeyCode <= Keys.D9;
										if (flag8)
										{
											bool flag9 = e.KeyCode > Keys.D0;
											if (flag9)
											{
												this.Ctrls[e.KeyCode - Keys.D1].PerformClick();
											}
											else
											{
												bool flag10 = e.KeyCode == Keys.D0;
												if (flag10)
												{
													this.Ctrls[9].PerformClick();
												}
											}
											e.SuppressKeyPress = true;
										}
										else
										{
											bool flag11 = e.Shift & (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return);
											if (flag11)
											{
												this.Ctrls[Math.Max(0, this.xMacro - 1)].PerformClick();
												e.SuppressKeyPress = true;
											}
											else
											{
												bool flag12 = e.KeyCode == Keys.Tab || e.KeyCode == Keys.Return;
												if (flag12)
												{
													this.Ctrls[Math.Min(19, this.xMacro + 1)].PerformClick();
													e.SuppressKeyPress = true;
												}
											}
										}
									}
								}
							}
						}
					}
					else
					{
						bool alt = e.Alt;
						if (alt)
						{
							e.SuppressKeyPress = true;
							bool flag13 = e.KeyCode == Keys.Left;
							if (flag13)
							{
								bool flag14 = this.xMacro < 10;
								if (flag14)
								{
									this.xMacro = 9;
								}
								this.Ctrls[Math.Max(10, this.xMacro - 1)].PerformClick();
							}
							else
							{
								bool flag15 = e.KeyCode == Keys.Right;
								if (flag15)
								{
									bool flag16 = this.xMacro < 10;
									if (flag16)
									{
										this.xMacro = 9;
									}
									this.Ctrls[Math.Min(19, this.xMacro + 1)].PerformClick();
								}
								else
								{
									bool flag17 = e.KeyCode == Keys.Up;
									if (flag17)
									{
										this.Contents.SelectedIndex = Math.Max(Math.Min(this.xBook - 1, this.Contents.Items.Count - 1), 0);
									}
									else
									{
										bool flag18 = e.KeyCode == Keys.Down;
										if (flag18)
										{
											this.Contents.SelectedIndex = Math.Min(Math.Max(this.xBook + 1, 0), this.Contents.Items.Count - 1);
										}
										else
										{
											bool flag19 = e.KeyCode >= Keys.D0 & e.KeyCode <= Keys.D9;
											if (flag19)
											{
												bool flag20 = e.KeyCode > Keys.D0;
												if (flag20)
												{
													this.Ctrls[e.KeyCode - Keys.Right].PerformClick();
												}
												else
												{
													bool flag21 = e.KeyCode == Keys.D0;
													if (flag21)
													{
														this.Ctrls[19].PerformClick();
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000046D4 File Offset: 0x000028D4
		private void Form1_Shown(object sender, EventArgs e)
		{
			bool flag = !Directory.Exists(MySettingsProperty.Settings.UserDirectory);
			if (flag)
			{
				this.GetFFXIDirectory();
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004704 File Offset: 0x00002904
		private void Form1_Load(object sender, EventArgs e)
		{
			this.Contents.Top = 40;
			this.Contents.Left = 20;
			this.Contents.Height = 400;
			this.Contents.Width = 115;
			this.Contents.Name = "Contents";
			this.Contents.Font = new Font(this.Font.FontFamily, 11f, this.Font.Style);
			checked
			{
				base.Height = this.Contents.Top + this.Contents.Height + this.Contents.Top + 20;
				base.Width = 990;
				this.Contents.SelectedIndexChanged += this.Contents_SelectedIndexChanged;
				this.Contents.MouseDown += this.Contents_MouseDown;
				base.Controls.Add(this.Contents);
				int num = 0;
				do
				{
					Button button = new Button();
					base.Controls.Add(button);
					button.Height = 30;
					button.Width = 40;
					button.Left = 145;
					button.Top = 40 * (num + 1);
					button.Tag = num;
					button.Font = this.Font;
					button.Text = (num + 1).ToString();
					button.Name = "Row" + Conversions.ToString(num);
					button.BackColor = this.BackColor;
					this.Rows.Add(num, button);
					button.Click += this.Row_Click;
					button.MouseDown += this.Row_Mousedown;
					num++;
				}
				while (num <= 9);
				int num2 = 0;
				do
				{
					Button button2 = new Button();
					base.Controls.Add(button2);
					button2.Height = this.bHeight;
					button2.Width = this.bWidth;
					button2.Tag = num2;
					button2.BackColor = this.BackColor;
					button2.Name = "Ctrl" + Conversions.ToString(num2);
					bool flag = num2 <= 9;
					if (flag)
					{
						button2.Left = 195 + (num2 + 1) * (this.bWidth + 10);
						button2.Top = 40;
						button2.Text = "C" + MacroEditorUtils.FormatMacroIndex(num2 + 1);
						this.Ctrls.Add(num2, button2);
					}
					else
					{
						button2.Left = 195 + (num2 + 1 - 10) * (this.bWidth + 10);
						button2.Top = this.bHeight + 50;
						button2.Text = "A" + MacroEditorUtils.FormatMacroIndex(num2 - 9);
						this.Ctrls.Add(num2, button2);
					}
					button2.Click += this.Control_Click;
					button2.MouseEnter += this.Control_MouseEnter;
					button2.MouseDown += this.Control_Mousedown;
					num2++;
				}
				while (num2 <= 19);
				Button button3 = new Button();
				base.Controls.Add(button3);
				button3.Left = 195;
				button3.Top = 40;
				button3.Width = this.bWidth;
				button3.Height = 30;
				button3.Name = "LeftHandler";
				button3.BackColor = this.BackColor;
				button3.Text = "● ● ●";
				button3.MouseDown += this.RowHandler_Mousedown;
				button3 = new Button();
				base.Controls.Add(button3);
				button3.Left = 195;
				button3.Top = 80;
				button3.Width = this.bWidth;
				button3.Height = 30;
				button3.Name = "FlipHandler";
				button3.BackColor = this.BackColor;
				button3.Text = "▲ ▼";
				button3.MouseDown += this.RowHandler_Mousedown;
				button3 = new Button();
				base.Controls.Add(button3);
				button3.Left = 195;
				button3.Top = 120;
				button3.Width = this.bWidth;
				button3.Height = 30;
				button3.Name = "RightHandler";
				button3.BackColor = this.BackColor;
				button3.Text = "● ● ●";
				button3.MouseDown += this.RowHandler_Mousedown;
				int num3 = 0;
				do
				{
					TextBox textBox = new TextBox();
					bool flag2 = num3 == 0;
					if (flag2)
					{
						textBox.Left = button3.Left;
						textBox.MaxLength = 8;
					}
					else
					{
						textBox.Left = button3.Left + 150;
						textBox.MaxLength = 300;
						textBox.ContextMenuStrip = this.MenuText;
					}
					textBox.Width = this.Ctrls[9].Left + this.Ctrls[9].Width - textBox.Left;
					textBox.Top = this.Rows[3 + num3].Top;
					textBox.Tag = num3;
					textBox.Font = new Font(this.Font.FontFamily, 18f, this.Font.Style);
					textBox.Name = "Lines" + Conversions.ToString(num3);
					this.Lines.Add(num3, textBox);
					base.Controls.Add(textBox);
					textBox.KeyDown += this.lines_KeyDown;
					textBox.GotFocus += this.Lines_GotFocus;
					textBox.TextChanged += this.Lines_TextChanged;
					num3++;
				}
				while (num3 <= 6);
				foreach (object obj in base.Controls)
				{
					object objectValue = RuntimeHelpers.GetObjectValue(obj);
					bool flag3 = objectValue is Button | objectValue is ListBox;
					if (flag3)
					{
						NewLateBinding.LateSet(objectValue, null, "tabstop", new object[]
						{
							false
						}, null, null);
					}
					bool flag4 = Conversions.ToBoolean(Operators.NotObject(Operators.CompareObjectEqual(NewLateBinding.LateGet(objectValue, null, "name", new object[0], null, null, null), "MainMenu", false)));
					if (flag4)
					{
						NewLateBinding.LateSet(objectValue, null, "enabled", new object[]
						{
							false
						}, null, null);
					}
				}
				this.File_SaveRow.Enabled = false;
				this.File_SaveAll.Enabled = false;
				this.StatusBar.Enabled = true;
				this.StatusH.Enabled = true;
				this.StatusD.Enabled = true;
				try
				{
					this.ParseAT();
				}
				catch (Exception ex)
				{
					// AT phrases unavailable - FFXI data files not found. Non-fatal.
				}
				this.rs.FindAllControls(this);
				ListBox contents = this.Contents;
				this.Warning.Top = contents.Top;
				this.Warning.Left = contents.Left;
				this.Warning.Height = contents.Height;
				this.Warning.Width = contents.Width;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004EB0 File Offset: 0x000030B0
		private void ATPhrase_Click(object sender, EventArgs e)
		{
			this.Lines[this.CurrentLine].SelectedText = Conversions.ToString(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null));
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004EE4 File Offset: 0x000030E4
		private void Lines_TextChanged(object sender, EventArgs e)
		{
			bool flag = Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null), 0, false);
			checked
			{
				if (flag)
				{
					string text = this.Lines[0].Text.Trim() + " ";
					bool flag2 = this.xMacro < 10;
					if (flag2)
					{
						this.Ctrls[this.xMacro].Text = "C" + MacroEditorUtils.FormatMacroIndex(this.xMacro + 1) + "\n" + text.Substring(0, Math.Min(5, text.Length));
					}
					else
					{
						this.Ctrls[this.xMacro].Text = "A" + MacroEditorUtils.FormatMacroIndex(this.xMacro - 9) + "\n" + text.Substring(0, Math.Min(5, text.Length));
					}
					bool flag3 = Operators.ConditionalCompareObjectGreater(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "text", new object[0], null, null, null), null, "length", new object[0], null, null, null), 8, false);
					if (flag3)
					{
						int val = Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "selectionstart", new object[0], null, null, null));
						NewLateBinding.LateSet(sender, null, "text", new object[]
						{
							NewLateBinding.LateGet(sender, null, "text", new object[0], null, null, null).ToString().Substring(0, 8)
						}, null, null);
						NewLateBinding.LateSet(sender, null, "selectionstart", new object[]
						{
							Math.Min(val, 8)
						}, null, null);
					}
				}
				this.SomethingEdited = true;
				bool flag4 = Strings.Trim(Conversions.ToString(NewLateBinding.LateGet(sender, null, "text", new object[0], null, null, null))).Length == 0;
				if (flag4)
				{
					this.MacroContainer[this.xBook, this.xRow, this.xMacro][Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null))] = "";
				}
				else
				{
					this.MacroContainer[this.xBook, this.xRow, this.xMacro][Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null))] = Conversions.ToString(NewLateBinding.LateGet(sender, null, "text", new object[0], null, null, null));
				}
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000515C File Offset: 0x0000335C
		private void lines_KeyDown(object sender, KeyEventArgs e)
		{
			bool flag = e.KeyCode == Keys.F2 & this.CurrentLine > 0;
			if (flag)
			{
				e.Handled = true;
				this.MenuText_Opening(RuntimeHelpers.GetObjectValue(sender), new CancelEventArgs());
				this.MenuText.Show(new Point(Conversions.ToInteger(Operators.AddObject(Operators.AddObject(base.Left, NewLateBinding.LateGet(sender, null, "left", new object[0], null, null, null)), NewLateBinding.LateGet(sender, null, "width", new object[0], null, null, null))), Conversions.ToInteger(Operators.AddObject(base.Top, NewLateBinding.LateGet(sender, null, "top", new object[0], null, null, null)))));
			}
			else
			{
				bool flag2 = e.KeyCode == Keys.Return & this.CurrentLine < 6;
				if (flag2)
				{
					bool flag3 = Operators.ConditionalCompareObjectGreater(NewLateBinding.LateGet(sender, null, "selectionlength", new object[0], null, null, null), 0, false);
					if (flag3)
					{
						NewLateBinding.LateSet(sender, null, "selectionstart", new object[]
						{
							Operators.AddObject(NewLateBinding.LateGet(sender, null, "selectionstart", new object[0], null, null, null), NewLateBinding.LateGet(sender, null, "selectionlength", new object[0], null, null, null))
						}, null, null);
						NewLateBinding.LateSet(sender, null, "selectionlength", new object[]
						{
							0
						}, null, null);
						e.SuppressKeyPress = true;
					}
					else
					{
						bool flag4 = this.CurrentLine < 6;
						if (flag4)
						{
							e.SuppressKeyPress = true;
							this.Lines[checked(this.CurrentLine + 1)].Focus();
						}
					}
				}
				else
				{
					bool flag5 = e.KeyCode == Keys.Return & this.CurrentLine == 6;
					if (flag5)
					{
						this.Lines[0].Focus();
						e.SuppressKeyPress = true;
					}
					else
					{
						bool flag6 = e.KeyCode == Keys.Escape;
						if (flag6)
						{
							bool flag7 = Operators.ConditionalCompareObjectGreater(NewLateBinding.LateGet(sender, null, "selectionlength", new object[0], null, null, null), 0, false);
							if (flag7)
							{
								NewLateBinding.LateSet(sender, null, "selectionlength", new object[]
								{
									0
								}, null, null);
								e.SuppressKeyPress = true;
							}
							else
							{
								bool flag8 = this.CurrentLine < 6;
								if (flag8)
								{
									e.SuppressKeyPress = true;
									this.Lines[0].Focus();
								}
								else
								{
									e.SuppressKeyPress = true;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000053E1 File Offset: 0x000035E1
		private void Lines_GotFocus(object sender, EventArgs e)
		{
			this.CurrentLine = Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null));
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005404 File Offset: 0x00003604
		private void Contents_MouseDown(object sender, MouseEventArgs e)
		{
			int num = this.Contents.IndexFromPoint(new Point(e.X, e.Y));
			bool flag = num >= 0;
			if (flag)
			{
				this.cbook = num;
				bool flag2 = e.Button == MouseButtons.Right;
				if (flag2)
				{
					this.MenuBook.Show((Control)sender, checked(new Point(e.X + 10, this.cbook * this.Contents.ItemHeight + 10)));
				}
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000548B File Offset: 0x0000368B
		private void Contents_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.xBook = this.Contents.SelectedIndex;
			this.Rows[this.xRow].PerformClick();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000054B8 File Offset: 0x000036B8
		private void Row_Click(object sender, EventArgs e)
		{
			this.xRow = Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null));
			int num = 0;
			checked
			{
				do
				{
					string text = this.MacroContainer[this.xBook, this.xRow, num][0].Trim() + " ";
					bool flag = num < 10;
					if (flag)
					{
						this.Ctrls[num].Text = "C" + MacroEditorUtils.FormatMacroIndex(num + 1) + "\n" + text.Substring(0, Math.Min(5, text.Length));
					}
					else
					{
						this.Ctrls[num].Text = "A" + MacroEditorUtils.FormatMacroIndex(num - 9) + "\n" + text.Substring(0, Math.Min(5, text.Length));
					}
					num++;
				}
				while (num <= 19);
				foreach (object obj in base.Controls)
				{
					object objectValue = RuntimeHelpers.GetObjectValue(obj);
					bool flag2 = objectValue is Button & NewLateBinding.LateGet(objectValue, null, "name", new object[0], null, null, null).ToString().StartsWith("Row");
					if (flag2)
					{
						NewLateBinding.LateSet(objectValue, null, "backcolor", new object[]
						{
							this.BackColor
						}, null, null);
					}
				}
				NewLateBinding.LateSet(sender, null, "BackColor", new object[]
				{
					Color.LightGray
				}, null, null);
				this.Ctrls[this.xMacro].PerformClick();
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000568C File Offset: 0x0000388C
		private void Control_MouseEnter(object sender, EventArgs e)
		{
			this.ControlTip.SetToolTip((Control)sender, string.Join("\n", this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null))]));
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000056E8 File Offset: 0x000038E8
		private void Control_Click(object sender, EventArgs e)
		{
			bool somethingEdited = this.SomethingEdited;
			this.xMacro = Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null));
			checked
			{
				int num = this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null))].Length - 1;
				for (int i = 0; i <= num; i++)
				{
					this.Lines[i].Text = this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null))][i];
				}
				foreach (object obj in base.Controls)
				{
					object objectValue = RuntimeHelpers.GetObjectValue(obj);
					bool flag = objectValue is Button & !NewLateBinding.LateGet(objectValue, null, "name", new object[0], null, null, null).ToString().StartsWith("Row");
					if (flag)
					{
						NewLateBinding.LateSet(objectValue, null, "backcolor", new object[]
						{
							this.BackColor
						}, null, null);
					}
				}
				NewLateBinding.LateSet(sender, null, "BackColor", new object[]
				{
					Color.LightGray
				}, null, null);
				this.SomethingEdited = somethingEdited;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000587C File Offset: 0x00003A7C
		private void Control_Mousedown(object sender, MouseEventArgs e)
		{
			bool flag = e.Button == MouseButtons.Right;
			if (flag)
			{
				this.MenuMacro.Show((Control)sender, new Point(40, 40));
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000058B8 File Offset: 0x00003AB8
		private void Row_Mousedown(object sender, MouseEventArgs e)
		{
			bool flag = e.Button == MouseButtons.Right;
			if (flag)
			{
				this.MenuRow.Show((Control)sender, new Point(20, 20));
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000058F4 File Offset: 0x00003AF4
		private void RowHandler_Mousedown(object sender, MouseEventArgs e)
		{
			bool flag = Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, null, "name", new object[0], null, null, null), "LeftHandler", false);
			checked
			{
				if (flag)
				{
					this.handlerStart = 0;
					this.handlerEnd = 9;
					this.MenuHandler.Show((Control)sender, new Point(20, 20));
				}
				else
				{
					bool flag2 = Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, null, "name", new object[0], null, null, null), "RightHandler", false);
					if (flag2)
					{
						this.handlerStart = 10;
						this.handlerEnd = 19;
						this.MenuHandler.Show((Control)sender, new Point(20, 20));
					}
					else
					{
						bool flag3 = Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, null, "name", new object[0], null, null, null), "FlipHandler", false);
						if (flag3)
						{
							this.handlerStart = 0;
							this.handlerEnd = 0;
							string[][] array = new string[20][];
							int num = 0;
							do
							{
								array[num] = this.MacroContainer[this.xBook, this.xRow, num];
								this.MacroContainer[this.xBook, this.xRow, num] = this.MacroContainer[this.xBook, this.xRow, num + 10];
								num++;
							}
							while (num <= 9);
							int num2 = 10;
							do
							{
								this.MacroContainer[this.xBook, this.xRow, num2] = array[num2 - 10];
								num2++;
							}
							while (num2 <= 19);
							this.SomethingEdited = true;
							this.Rows[this.xRow].PerformClick();
						}
					}
				}
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005A98 File Offset: 0x00003C98
		private void MenuMacro_Paste_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(objectValue)] = this.Macroholder;
			string text = this.Macroholder[0] + " ";
			bool flag = Operators.ConditionalCompareObjectLess(objectValue, 10, false);
			if (flag)
			{
				this.Ctrls[Conversions.ToInteger(objectValue)].Text = "C" + MacroEditorUtils.FormatMacroIndex(Conversions.ToInteger(Operators.AddObject(objectValue, 1))) + "\n" + text.Substring(0, Math.Min(5, text.Length));
			}
			else
			{
				this.Ctrls[Conversions.ToInteger(objectValue)].Text = "A" + MacroEditorUtils.FormatMacroIndex(Conversions.ToInteger(Operators.SubtractObject(objectValue, 9))) + "\n" + text.Substring(0, Math.Min(5, text.Length));
			}
			this.SomethingEdited = true;
			this.Ctrls[Conversions.ToInteger(objectValue)].PerformClick();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005BF8 File Offset: 0x00003DF8
		private void MenuBook_Paste_Click(object sender, EventArgs e)
		{
			int num = 0;
			checked
			{
				do
				{
					int num2 = 0;
					do
					{
						try
						{
							this.MacroContainer[this.cbook, num, num2] = this.BookHolder[num, num2];
						}
						catch (Exception ex)
						{
							int num3 = 0;
							do
							{
								try
								{
									this.MacroContainer[this.cbook, num, num2][num3] = this.BookHolder[num, num2][num3];
								}
								catch (Exception ex2)
								{
									this.MacroContainer[this.cbook, num, num2][num3] = "";
								}
								num3++;
							}
							while (num3 <= 6);
						}
						num2++;
					}
					while (num2 <= 19);
					this.SomethingEdited = true;
					num++;
				}
				while (num <= 9);
				this.Contents.SelectedIndexChanged -= this.Contents_SelectedIndexChanged;
				this.Contents.Items[this.cbook] = this.copiedbookname;
				this.Contents.SelectedIndexChanged += this.Contents_SelectedIndexChanged;
				this.Contents.SelectedIndex = this.cbook;
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005D48 File Offset: 0x00003F48
		private void MenuBook_Revert_Click(object sender, EventArgs e)
		{
			int num = 0;
			checked
			{
				do
				{
					int num2 = 0;
					do
					{
						this.MacroContainer[this.cbook, num, num2] = (string[])this.MacroPreserved[this.cbook, num, num2].Clone();
						num2++;
					}
					while (num2 <= 19);
					num++;
				}
				while (num <= 9);
				this.SomethingEdited = true;
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00005DBC File Offset: 0x00003FBC
		private void MenuBook_Copy_Click(object sender, EventArgs e)
		{
			int num = 0;
			checked
			{
				do
				{
					int num2 = 0;
					do
					{
						this.BookHolder[num, num2] = this.MacroContainer[this.cbook, num, num2];
						num2++;
					}
					while (num2 <= 19);
					num++;
				}
				while (num <= 9);
				this.InternalClipboardMethod = "Book";
				this.copiedbookname = Conversions.ToString(this.Contents.Items[this.cbook]);
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005E43 File Offset: 0x00004043
		private void MenuBook_Cut_Click(object sender, EventArgs e)
		{
			this.MenuBook_Copy_Click(RuntimeHelpers.GetObjectValue(sender), e);
			this.MenuBook_Clear_Click(RuntimeHelpers.GetObjectValue(sender), e);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005E64 File Offset: 0x00004064
		private void MenuBook_Clear_Click(object sender, EventArgs e)
		{
			int num = 0;
			checked
			{
				do
				{
					int num2 = 0;
					do
					{
						this.MacroContainer[this.cbook, num, num2] = new string[]
						{
							"",
							"",
							"",
							"",
							"",
							"",
							""
						};
						num2++;
					}
					while (num2 <= 19);
					num++;
				}
				while (num <= 9);
				this.SomethingEdited = true;
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005EF8 File Offset: 0x000040F8
		private void MenuBook_CopyClipboard_Click(object sender, EventArgs e)
		{
			string[][] array = new string[21][];
			string[] array2 = new string[12];
			array2[0] = Conversions.ToString(Operators.ConcatenateObject("Type: Book ", this.Contents.Items[this.cbook]));
			int num = 0;
			checked
			{
				do
				{
					array[num] = new string[]
					{
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						"",
						""
					};
					int num2 = 0;
					do
					{
						array[num][num2] = string.Join("\n", this.MacroContainer[this.cbook, num, num2]).TrimEnd(new char[0]);
						num2++;
					}
					while (num2 <= 19);
					array2[num + 1] = string.Join("\n\nMacro:\n", array[num]);
					num++;
				}
				while (num <= 9);
				array2[11] = "EndBook (Please do not remove empty lines as they're part of the sharing format). If you experience problems pasting, please make sure to download an up to date version.";
				Clipboard.SetText(string.Join("\n\nRow, Macro:\n", array2));
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000607C File Offset: 0x0000427C
		private void MenuBook_PasteClipboard_Click(object sender, EventArgs e, string clp = "")
		{
			bool flag = clp == "";
			string[] array;
			if (flag)
			{
				array = Strings.Split(this.CleanClipBoard(), "\n\nRow, Macro:\n", -1, CompareMethod.Binary);
			}
			else
			{
				array = Strings.Split(clp, "\n\nRow, Macro:\n", -1, CompareMethod.Binary);
			}
			bool flag2 = !MacroEditorUtils.VerifyClipboardFormat("Book", array);
			checked
			{
				if (!flag2)
				{
					this.Contents.SelectedIndexChanged -= this.Contents_SelectedIndexChanged;
					bool flag3 = Operators.CompareString(array[0].Substring(0, 10), "Type: Book", false) == 0;
					if (flag3)
					{
						this.Contents.Items[this.cbook] = array[0].Substring(11, Math.Min(10, array[0].Length - 11));
						array = array.Skip(1).ToArray<string>();
						Array.Resize<string>(ref array, array.Length - 1);
						int num = 0;
						do
						{
							string[] array2 = Strings.Split(array[num], "\n\nMacro:\n", -1, CompareMethod.Binary);
							int num2 = 0;
							do
							{
								this.MacroContainer[this.xBook, num, num2] = new string[]
								{
									"",
									"",
									"",
									"",
									"",
									"",
									""
								};
								string[] array3 = array2[num2].Split(new char[]
								{
									'\n'
								});
								int num3 = Math.Min(array3.Length - 1, 6);
								for (int i = 0; i <= num3; i++)
								{
									this.MacroContainer[this.cbook, num, num2][i] = array3[i];
								}
								num2++;
							}
							while (num2 <= 19);
							num++;
						}
						while (num <= 9);
					}
					this.SomethingEdited = true;
					this.Contents.SelectedIndexChanged += this.Contents_SelectedIndexChanged;
					this.Rows[this.xRow].PerformClick();
				}
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006270 File Offset: 0x00004470
		private void MenuMacro_Revert_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(objectValue)] = (string[])this.MacroPreserved[this.xBook, this.xRow, Conversions.ToInteger(objectValue)].Clone();
			this.SomethingEdited = true;
			this.Ctrls[Conversions.ToInteger(objectValue)].PerformClick();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00006324 File Offset: 0x00004524
		private void MenuMacro_Copy_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			this.Macroholder = this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(objectValue)];
			this.InternalClipboardMethod = "Macro";
			this.Ctrls[Conversions.ToInteger(objectValue)].PerformClick();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000063BA File Offset: 0x000045BA
		private void MenuMacro_Cut_Click(object sender, EventArgs e)
		{
			this.MenuMacro_Copy_Click(RuntimeHelpers.GetObjectValue(sender), e);
			this.MenuMacro_Clear_Click(RuntimeHelpers.GetObjectValue(sender), e);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000063DC File Offset: 0x000045DC
		private void MenuMacro_Clear_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(objectValue)] = new string[]
			{
				"",
				"",
				"",
				"",
				"",
				"",
				""
			};
			this.SomethingEdited = true;
			this.Ctrls[Conversions.ToInteger(objectValue)].PerformClick();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000064A8 File Offset: 0x000046A8
		private void MenuMacro_CopyClipboard_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			string text = "Type: Macro\n" + string.Join("\n", this.MacroContainer[this.xBook, this.xRow, Conversions.ToInteger(objectValue)]).TrimEnd(new char[0]) + "\nEndMacro (Please do not remove empty lines as they're part of the sharing format). If you experience problems pasting, please make sure to download an up to date version.";
			Clipboard.SetText(text);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006544 File Offset: 0x00004744
		private void MenuMacro_PasteClipboard_Click(object sender, EventArgs e, int x = -1)
		{
			bool flag = x == -1;
			if (flag)
			{
				x = Conversions.ToInteger(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			}
			string[] array = this.CleanClipBoard().Split(new char[]
			{
				'\n'
			});
			bool flag2 = array[0].StartsWith("Type: Macro");
			checked
			{
				if (flag2)
				{
					this.MacroContainer[this.xBook, this.xRow, x] = new string[]
					{
						"",
						"",
						"",
						"",
						"",
						"",
						""
					};
					int num = Math.Min(array.Length - 2, 7);
					for (int i = 1; i <= num; i++)
					{
						this.MacroContainer[this.xBook, this.xRow, x][i - 1] = array[i];
					}
				}
				else
				{
					bool flag3 = array[0].StartsWith("Type: ");
					if (flag3)
					{
						Interaction.MsgBox("Clipboard data contains " + array[0] + ", canceling paste to this macro.", MsgBoxStyle.OkOnly, null);
					}
					else
					{
						bool flag4 = array.Length == 7;
						if (flag4)
						{
							this.MenuMacro_Clear.PerformClick();
							this.MacroContainer[this.xBook, this.xRow, x] = array;
							this.SomethingEdited = true;
						}
						else
						{
							bool flag5 = array.Length < 7;
							if (flag5)
							{
								this.MenuMacro_Clear.PerformClick();
								int num2 = (int)MessageBox.Show("Is the first line the title?", "Macro Editor", MessageBoxButtons.YesNo);
								bool flag6 = num2 == 6;
								if (flag6)
								{
									this.MacroContainer[this.xBook, this.xRow, x] = new string[]
									{
										"",
										"",
										"",
										"",
										"",
										"",
										""
									};
									int num3 = array.Length - 1;
									for (int j = 0; j <= num3; j++)
									{
										this.MacroContainer[this.xBook, this.xRow, x][j] = array[j];
									}
								}
								else
								{
									string text = Interaction.InputBox("Enter the title for the macro", "Macro Editor", Conversions.ToString(0), -1, -1);
									this.MacroContainer[this.xBook, this.xRow, x] = new string[]
									{
										text,
										"",
										"",
										"",
										"",
										"",
										""
									};
									int num4 = array.Length - 1;
									for (int k = 0; k <= num4; k++)
									{
										this.MacroContainer[this.xBook, this.xRow, x][k + 1] = array[k];
									}
								}
								this.SomethingEdited = true;
							}
						}
					}
				}
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000686C File Offset: 0x00004A6C
		private void MenuRow_Paste_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			int num = 0;
			checked
			{
				do
				{
					this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num] = this.RowHolder[num];
					num++;
				}
				while (num <= 19);
				this.SomethingEdited = true;
				this.Rows[Conversions.ToInteger(objectValue)].PerformClick();
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00006908 File Offset: 0x00004B08
		private void MenuRow_Revert_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			int num = 0;
			checked
			{
				do
				{
					this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num] = (string[])this.MacroPreserved[this.xBook, Conversions.ToInteger(objectValue), num].Clone();
					num++;
				}
				while (num <= 19);
				this.SomethingEdited = true;
				this.Rows[Conversions.ToInteger(objectValue)].PerformClick();
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000069BC File Offset: 0x00004BBC
		private void MenuRow_Copy_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			int num = 0;
			checked
			{
				do
				{
					this.RowHolder[num] = this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num];
					num++;
				}
				while (num <= 19);
				this.InternalClipboardMethod = "Row";
				this.Rows[Conversions.ToInteger(objectValue)].PerformClick();
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006A5C File Offset: 0x00004C5C
		private void MenuRow_Cut_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			int num = 0;
			checked
			{
				do
				{
					this.RowHolder[num] = this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num];
					this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num] = new string[]
					{
						"",
						"",
						"",
						"",
						"",
						"",
						""
					};
					num++;
				}
				while (num <= 19);
				this.SomethingEdited = true;
				this.Rows[Conversions.ToInteger(objectValue)].PerformClick();
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00006B4C File Offset: 0x00004D4C
		private void MenuRow_Clear_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			int num = 0;
			checked
			{
				do
				{
					this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num] = new string[]
					{
						"",
						"",
						"",
						"",
						"",
						"",
						""
					};
					num++;
				}
				while (num <= 19);
				this.SomethingEdited = true;
				this.Rows[Conversions.ToInteger(objectValue)].PerformClick();
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006C1C File Offset: 0x00004E1C
		private void MenuRow_CopyClipboard_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			string[] array = new string[22];
			array[0] = "Type: Row";
			int num = 0;
			checked
			{
				do
				{
					array[num + 1] = string.Join("\n", this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num]).TrimEnd(new char[0]);
					num++;
				}
				while (num <= 19);
				array[21] = "EndRow (Please do not remove empty lines as they're part of the sharing format). If you experience problems pasting, please make sure to download an up to date version.";
				Clipboard.SetText(string.Join("\n\nMacro:\n", array));
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00006CD4 File Offset: 0x00004ED4
		private void MenuRow_PasteClipboard_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			string[] array = Strings.Split(this.CleanClipBoard(), "\n\nMacro:\n", -1, CompareMethod.Binary);
			bool flag = !MacroEditorUtils.VerifyClipboardFormat("Row", array);
			checked
			{
				if (!flag)
				{
					array = array.Skip(1).ToArray<string>();
					Array.Resize<string>(ref array, array.Length - 1);
					int num = 0;
					do
					{
						string[] array2 = array[num].Split(new char[]
						{
							'\n'
						});
						this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num] = new string[]
						{
							"",
							"",
							"",
							"",
							"",
							"",
							""
						};
						int num2 = Math.Min(array2.Length - 1, 6);
						for (int i = 0; i <= num2; i++)
						{
							try
							{
								this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num][i] = array2[i];
							}
							catch (Exception ex)
							{
								this.MacroContainer[this.xBook, Conversions.ToInteger(objectValue), num][i] = "";
							}
						}
						num++;
					}
					while (num <= 19);
					this.SomethingEdited = true;
					this.Rows[Conversions.ToInteger(objectValue)].PerformClick();
				}
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006E84 File Offset: 0x00005084
		private void MenuMain_evaluate_Click(object sender, EventArgs e)
		{
			this.Rows[this.xRow].Focus();
			Dictionary<int, string[]> dictionary = new Dictionary<int, string[]>();
			Dictionary<int, string[]> dictionary2 = new Dictionary<int, string[]>();
			int num = this.debuglimit;
			checked
			{
				for (int i = 0; i <= num; i++)
				{
					int num2 = 0;
					do
					{
						int num3 = 0;
						do
						{
							bool flag = this.MacroContainer[i, num2, num3][0].Length > 8;
							if (flag)
							{
								dictionary[dictionary.Count] = new string[]
								{
									Conversions.ToString(i),
									Conversions.ToString(num2),
									Conversions.ToString(num3),
									Conversions.ToString(0),
									"8 characters max.",
									this.MacroContainer[i, num2, num3][0]
								};
							}
							int num4 = 1;
							do
							{
								string text = this.MacroContainer[i, num2, num3][num4];
								string text2 = this.ATWriter(text);
								Match match = new Regex("[^\\x00-\\x7f]").Match(text);
								bool flag2 = text.Length == 0;
								if (!flag2)
								{
									bool flag3 = text2.Length > 60;
									if (flag3)
									{
										dictionary[dictionary.Count] = new string[]
										{
											Conversions.ToString(i),
											Conversions.ToString(num2),
											Conversions.ToString(num3),
											Conversions.ToString(num4),
											Conversions.ToString(2),
											"After auto-translate, the line is " + Conversions.ToString(text2.Length) + " characters."
										};
									}
									else
									{
										bool flag4 = match.Length != 0;
										if (flag4)
										{
											dictionary2[dictionary2.Count] = new string[]
											{
												Conversions.ToString(i),
												Conversions.ToString(num2),
												Conversions.ToString(num3),
												Conversions.ToString(num4),
												Conversions.ToString(3),
												text
											};
										}
										else
										{
											bool flag5 = text.StartsWith("//");
											if (flag5)
											{
												dictionary2[dictionary2.Count] = new string[]
												{
													Conversions.ToString(i),
													Conversions.ToString(num2),
													Conversions.ToString(num3),
													Conversions.ToString(num4),
													Conversions.ToString(4),
													text
												};
											}
											else
											{
												bool flag6 = !text.StartsWith("/");
												if (flag6)
												{
													dictionary2[dictionary2.Count] = new string[]
													{
														Conversions.ToString(i),
														Conversions.ToString(num2),
														Conversions.ToString(num3),
														Conversions.ToString(num4),
														Conversions.ToString(5),
														text
													};
												}
												else
												{
													bool flag7 = text.StartsWith("/wait");
													if (flag7)
													{
														dictionary2[dictionary2.Count] = new string[]
														{
															Conversions.ToString(i),
															Conversions.ToString(num2),
															Conversions.ToString(num3),
															Conversions.ToString(num4),
															Conversions.ToString(6),
															text
														};
													}
													else
													{
														bool flag8 = text.Contains("|UnknownItem>");
														if (flag8)
														{
															dictionary2[dictionary2.Count] = new string[]
															{
																Conversions.ToString(i),
																Conversions.ToString(num2),
																Conversions.ToString(num3),
																Conversions.ToString(num4),
																Conversions.ToString(7),
																text
															};
														}
													}
												}
											}
										}
									}
								}
								num4++;
							}
							while (num4 <= 6);
							num3++;
						}
						while (num3 <= 19);
						num2++;
					}
					while (num2 <= 9);
				}
				this.Evaluation.Hide();
				this.Evaluation = new Assessment();
				bool flag9 = dictionary.Count + dictionary2.Count > 0;
				if (flag9)
				{
					foreach (KeyValuePair<int, string[]> keyValuePair in dictionary)
					{
						this.Evaluation.AddResult(keyValuePair.Value[0], keyValuePair.Value[1], keyValuePair.Value[2], keyValuePair.Value[3], keyValuePair.Value[4], keyValuePair.Value[5], Color.Red, Color.White);
					}
					foreach (KeyValuePair<int, string[]> keyValuePair2 in dictionary2)
					{
						this.Evaluation.AddResult(keyValuePair2.Value[0], keyValuePair2.Value[1], keyValuePair2.Value[2], keyValuePair2.Value[3], keyValuePair2.Value[4], keyValuePair2.Value[5], Color.White, Color.Black);
					}
				}
				else
				{
					Interaction.MsgBox("No warnings or errors found.", MsgBoxStyle.OkOnly, null);
				}
				this.Evaluation.Show(this);
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00007390 File Offset: 0x00005590
		private void MenuHandler_CutSide_Click(object sender, EventArgs e)
		{
			this.MenuHandler_CopySide_Click(RuntimeHelpers.GetObjectValue(sender), e);
			this.MenuHandler_ClearSide_Click(RuntimeHelpers.GetObjectValue(sender), e);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000073B0 File Offset: 0x000055B0
		private void MenuHandler_CopySide_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			string[] array = new string[12];
			array[0] = "Type: Side";
			bool flag = this.handlerStart == 10;
			checked
			{
				if (flag)
				{
					int num = this.handlerStart;
					int num2 = this.handlerEnd;
					for (int i = num; i <= num2; i++)
					{
						array[i - 10 + 1] = string.Join("\n", this.MacroContainer[this.xBook, this.xRow, i]);
					}
				}
				else
				{
					int num3 = this.handlerStart;
					int num4 = this.handlerEnd;
					for (int j = num3; j <= num4; j++)
					{
						array[j + 1] = string.Join("\n", this.MacroContainer[this.xBook, this.xRow, j]);
					}
				}
				array[11] = "EndSide (Please do not remove empty lines as they're part of the sharing format). If you experience problems pasting, please make sure to download an up to date version.";
				Clipboard.SetText(string.Join("\n\nMacro:\n", array));
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000074D8 File Offset: 0x000056D8
		private void MenuHandler_PasteSide_Click(object sender, EventArgs e)
		{
			object objectValue = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
			string[] array = Strings.Split(this.CleanClipBoard(), "\n\nMacro:\n", -1, CompareMethod.Binary);
			bool flag = Operators.CompareString(array[0], "Type: Side", false) == 0;
			checked
			{
				if (flag)
				{
					bool flag2 = !MacroEditorUtils.VerifyClipboardFormat("Side", array);
					if (!flag2)
					{
						array = array.Skip(1).ToArray<string>();
						Array.Resize<string>(ref array, array.Length - 1);
						bool flag3 = this.handlerStart == 10;
						if (flag3)
						{
							int num = this.handlerStart;
							int num2 = this.handlerEnd;
							for (int i = num; i <= num2; i++)
							{
								string[] array2 = array[i - 10].Split(new char[]
								{
									'\n'
								});
								int num3 = 0;
								do
								{
									this.MacroContainer[this.xBook, this.xRow, i][num3] = array2[num3];
									num3++;
								}
								while (num3 <= 6);
							}
						}
						else
						{
							int num4 = this.handlerStart;
							int num5 = this.handlerEnd;
							for (int j = num4; j <= num5; j++)
							{
								string[] array3 = array[j].Split(new char[]
								{
									'\n'
								});
								int num6 = 0;
								do
								{
									this.MacroContainer[this.xBook, this.xRow, j][num6] = array3[num6];
									num6++;
								}
								while (num6 <= 6);
							}
						}
						this.SomethingEdited = true;
						this.Rows[this.xRow].PerformClick();
					}
				}
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00007690 File Offset: 0x00005890
		private void MenuHandler_ClearSide_Click(object sender, EventArgs e)
		{
			int num = this.handlerStart;
			int num2 = this.handlerEnd;
			checked
			{
				for (int i = num; i <= num2; i++)
				{
					this.MacroContainer[this.xBook, this.xRow, i] = new string[]
					{
						"",
						"",
						"",
						"",
						"",
						"",
						""
					};
				}
				this.SomethingEdited = true;
				this.Rows[this.xRow].PerformClick();
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000772C File Offset: 0x0000592C
		private void File_Open_Click(object sender, EventArgs e)
		{
			this.OpenDialog.InitialDirectory = this.macropath;
			this.OpenDialog.Filter = "Macro Title Files|mcr.ttl";
			this.OpenDialog.FileName = "mcr.ttl";
			this.OpenDialog.Multiselect = false;
			bool flag = this.OpenDialog.ShowDialog() != DialogResult.Cancel;
			checked
			{
				if (flag)
				{
					this.macropath = this.OpenDialog.FileName.Substring(0, this.OpenDialog.FileName.LastIndexOf("\\"));
					this.Warning.Visible = false;
					string path = this.macropath + "\\mcr.ttl";
					byte[] array = File.ReadAllBytes(path);
					string text = "";
					int num = array.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						text += Conversions.ToString(Convert.ToChar(array[i]));
					}
					Match match = new Regex("((.{15}\\x00)+$)").Match(text);
					text = match.Groups[1].ToString();
					text = new Regex("\0+").Replace(text, ",");
					string[] array2 = text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					this.Contents.Items.Clear();
					int num2 = this.debuglimit;
					for (int j = 0; j <= num2; j++)
					{
						this.UpdateStatusBar("Extracting...", array2[j]);
						this.Contents.Items.Add(array2[j]);
						this.Contents.Refresh();
						this.StatusBar.Refresh();
						int num3 = 0;
						do
						{
							string text2 = this.macropath + "\\mcr" + MacroEditorUtils.GetMacroFileSuffix(j, num3) + ".dat";
							bool flag2 = !File.Exists(text2);
							if (flag2)
							{
								int num4 = 0;
								do
								{
									this.MacroContainer[j, num3, num4] = new string[]
									{
										"",
										"",
										"",
										"",
										"",
										"",
										""
									};
									this.MacroPreserved[j, num3, num4] = (string[])this.MacroContainer[j, num3, num4].Clone();
									num4++;
								}
								while (num4 <= 19);
							}
							else
							{
								this.ReadMacroFile(j, num3, text2, true);
							}
							num3++;
						}
						while (num3 <= 9);
					}
					this.UpdateStatusBar("Extraction complete", "");
					foreach (object obj in base.Controls)
					{
						object objectValue = RuntimeHelpers.GetObjectValue(obj);
						NewLateBinding.LateSet(objectValue, null, "enabled", new object[]
						{
							true
						}, null, null);
					}
					this.MenuMain_evaluate.Enabled = true;
					this.MenuMain_Search.Enabled = true;
					this.Contents.SelectedIndex = 0;
					this.File_SaveRow.Enabled = true;
					this.File_SaveAll.Enabled = true;
					this.SomethingEdited = false;
				}
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00007A8C File Offset: 0x00005C8C
		private void File_SaveRow_Click(object sender, EventArgs e)
		{
			this.MenuRow_Save_Click(this.Rows[this.xRow], e);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00007AA8 File Offset: 0x00005CA8
		private void File_SaveAll_Click(object sender, EventArgs e)
		{
			int num = (int)MessageBox.Show("This will save all macros to file.\n\nProceed?\n\nThis will also update the in-memory backup that can\nbe restored with Revert from Main, Book, Row, & Macro menus.", "Macro Editor", MessageBoxButtons.YesNo);
			bool flag = num == 6;
			checked
			{
				if (flag)
				{
					int num2 = 0;
					for (;;)
					{
						int num3 = 0;
						do
						{
							bool flag2 = !this.WriteFile(num2, num3);
							if (flag2)
							{
								goto Block_2;
							}
							this.UpdateStatusBar(Conversions.ToString(this.Contents.Items[num2]), "Writing " + MacroEditorUtils.GetMacroFileSuffix(num2, num3) + ".dat");
							this.StatusBar.Refresh();
							num3++;
						}
						while (num3 <= 9);
						num2++;
						if (num2 > 19)
						{
							goto IL_B3;
						}
					}
					Block_2:
					this.StatusH.Text = "Error";
					this.StatusD.Text = "An error ocurred writing files.";
					return;
				}
				IL_B3:
				num = (int)MessageBox.Show("Now, save macro titles?", "Macro Editor", MessageBoxButtons.YesNo);
				bool flag3 = num == 6;
				if (flag3)
				{
					this.MenuBook_SaveBookNames.PerformClick();
					this.UpdateStatusBar("Titles saved.", "Save Complete.");
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00007BA4 File Offset: 0x00005DA4
		private void MenuHelp_Help_Click(object sender, EventArgs e)
		{
			Help help = new Help();
			help.ShowDialog();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00007BC0 File Offset: 0x00005DC0
		private void MenuBook_SaveBookNames_Click(object sender, EventArgs e)
		{
			string path = this.macropath + "\\mcr.ttl";
			byte[] sourceArray = File.ReadAllBytes(path);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			checked
			{
				do
				{
					StringBuilder stringBuilder2 = stringBuilder;
					object instance = NewLateBinding.LateGet(this.Contents.Items[num], null, "trim", new object[0], null, null, null);
					Type type = null;
					string memberName = "Substring";
					object[] array = new object[2];
					array[0] = 0;
					int num2 = 1;
					object instance2;
					object[] array2;
					bool[] array3;
					object obj = NewLateBinding.LateGet(null, typeof(Math), "Min", array2 = new object[]
					{
						15,
						NewLateBinding.LateGet(instance2 = this.Contents.Items[num], null, "length", new object[0], null, null, null)
					}, null, null, array3 = new bool[]
					{
						default(bool),
						true
					});
					if (array3[1])
					{
						NewLateBinding.LateSetComplex(instance2, null, "length", new object[]
						{
							array2[1]
						}, null, null, true, true);
					}
					array[num2] = obj;
					stringBuilder2.Append(MacroEditorUtils.Fill(Conversions.ToString(NewLateBinding.LateGet(instance, type, memberName, array, null, null, null)), 16));
					num++;
				}
				while (num <= 19);
				bool flag = stringBuilder.Length != 320;
				if (flag)
				{
					Interaction.MsgBox("Compilation of Macro Book titles failed", MsgBoxStyle.OkOnly, null);
				}
				else
				{
					byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
					MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
					byte[] array4 = md5CryptoServiceProvider.ComputeHash(bytes);
					StringBuilder stringBuilder3 = new StringBuilder();
					int num3 = array4.Length - 1;
					for (int i = 0; i <= num3; i++)
					{
						stringBuilder3.Append(Strings.Chr((int)array4[i]));
					}
					bool flag2 = stringBuilder.Length != 320;
					if (flag2)
					{
						Interaction.MsgBox("Compilation of Macro Names file failed.", MsgBoxStyle.OkOnly, null);
					}
					else
					{
						byte[] array5 = new byte[8 + array4.Length + bytes.Length - 1 + 1];
						Array.Copy(sourceArray, 0, array5, 0, 8);
						Array.Copy(array4, 0, array5, 8, 16);
						Array.Copy(bytes, 0, array5, 24, bytes.Length);
						File.WriteAllBytes(path, array5);
					}
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00007DD5 File Offset: 0x00005FD5
		private void Form1_Closing(object sender, CancelEventArgs e)
		{
			MySettingsProperty.Settings.Save();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00007DE4 File Offset: 0x00005FE4
		private void MenuText_Opening(object sender, CancelEventArgs e)
		{
			bool flag = Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, null, "name", new object[0], null, null, null), "MenuText", false);
			if (flag)
			{
				this.OpenATMenu();
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00007E20 File Offset: 0x00006020
		private void MenuBook_Opening(object sender, CancelEventArgs e)
		{
			bool enabled = this.MenuBook.Enabled;
			if (enabled)
			{
				this.MenuBook_Header.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(this.Contents.Items[this.cbook], " (#"), checked(this.cbook + 1)), ")"));
				this.MenuBook_Paste.Enabled = (Operators.CompareString(this.InternalClipboardMethod, "Book", false) == 0);
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00007EAC File Offset: 0x000060AC
		private void MenuRow_Opening(object sender, CancelEventArgs e)
		{
			this.MenuRow_Paste.Enabled = (Operators.CompareString(this.InternalClipboardMethod, "Row", false) == 0);
			bool enabled = this.MenuRow.Enabled;
			if (enabled)
			{
				this.MenuRow_Save.Text = Conversions.ToString(Operators.ConcatenateObject("Save Row ", Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "sourcecontrol", new object[0], null, null, null), null, "tag", new object[0], null, null, null), 1)));
				this.MenuRow_CopyLocation.Text = "Copy Location (mcr" + MacroEditorUtils.GetMacroFileSuffix(this.xBook, Conversions.ToInteger(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "sourcecontrol", new object[0], null, null, null), null, "tag", new object[0], null, null, null))) + ".dat)";
				this.MenuRow_CopyLocation.Tag = "mcr" + MacroEditorUtils.GetMacroFileSuffix(this.xBook, Conversions.ToInteger(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "sourcecontrol", new object[0], null, null, null), null, "tag", new object[0], null, null, null))) + ".dat";
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00007FE7 File Offset: 0x000061E7
		private void MenuMacro_Opening(object sender, CancelEventArgs e)
		{
			this.MenuMacro_Paste.Enabled = (Operators.CompareString(this.InternalClipboardMethod, "Macro", false) == 0);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000800C File Offset: 0x0000620C
		private void MenuBook_Import_Click(object sender, EventArgs e)
		{
			int num = (int)MessageBox.Show("You will need to open 10 macro files ranging from (0-9, 10-19, 120-129, et). Proceed?\r\n\r\nThe files will not be saved until you save and the originals can be recovered by Reverting the Book or individual Rows.", "Macro Editor", MessageBoxButtons.YesNo);
			bool flag = num == 7;
			checked
			{
				if (!flag)
				{
					this.OpenDialog.InitialDirectory = this.importpath;
					this.OpenDialog.Filter = "Macro Dat Files|*.dat";
					this.OpenDialog.FileName = "";
					this.OpenDialog.Multiselect = true;
					bool flag2 = this.OpenDialog.ShowDialog() != DialogResult.Cancel;
					if (flag2)
					{
						bool flag3 = this.OpenDialog.FileNames.Length == 10;
						if (flag3)
						{
							int num2 = this.OpenDialog.FileNames.Length - 1;
							for (int i = 0; i <= num2; i++)
							{
								this.ReadMacroFile(this.xBook, i, this.OpenDialog.FileNames[i], false);
							}
							this.Contents.SelectedIndex = this.cbook;
							this.Rows[this.xRow].PerformClick();
							this.importpath = this.OpenDialog.FileName.Substring(0, this.OpenDialog.FileName.LastIndexOf("\\"));
						}
						else
						{
							Interaction.MsgBox("You must select 10 files.", MsgBoxStyle.OkOnly, null);
						}
					}
				}
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00008158 File Offset: 0x00006358
		private void MenuRow_Import_Click(object sender, EventArgs e)
		{
			int num = (int)MessageBox.Show("You will need to open another macro dat file. Do you want to proceed? \r\n\r\nThe file will not be saved until you save and the original can be recovered by Reverting the Row.", "Macro Editor", MessageBoxButtons.YesNo);
			bool flag = num == 7;
			if (!flag)
			{
				this.OpenDialog.InitialDirectory = this.importpath;
				this.OpenDialog.Filter = "Macro Dat Files|*.dat";
				this.OpenDialog.FileName = "";
				this.OpenDialog.Multiselect = false;
				bool flag2 = this.OpenDialog.ShowDialog() != DialogResult.Cancel;
				if (flag2)
				{
					this.importpath = this.OpenDialog.FileName.Substring(0, this.OpenDialog.FileName.LastIndexOf("\\"));
					this.ReadMacroFile(this.xBook, this.xRow, this.OpenDialog.FileName, false);
					this.Rows[this.xRow].PerformClick();
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00008244 File Offset: 0x00006444
		private void menuText_Paste_Click(object sender, EventArgs e, int x = 0)
		{
			string[] array = this.CleanClipBoard().Trim().Split(new char[]
			{
				'\n'
			});
			bool flag = Operators.CompareString(array[0], "Type: Macro", false) == 0;
			checked
			{
				if (flag)
				{
					this.MenuMacro_PasteClipboard_Click(this.Ctrls[this.xMacro], e, this.xMacro);
				}
				else
				{
					bool flag2 = array.Length == 1;
					if (flag2)
					{
						this.Lines[this.CurrentLine].SelectedText = this.CleanClipBoard();
					}
					else
					{
						bool flag3 = this.CurrentLine == 0;
						if (flag3)
						{
							this.MacroContainer[this.xBook, this.xRow, this.xMacro] = new string[]
							{
								"",
								"",
								"",
								"",
								"",
								"",
								""
							};
							int num = array.Length - 1;
							for (int i = 0; i <= num; i++)
							{
								this.Lines[i].Text = array[i];
								bool flag4 = i == 6;
								if (flag4)
								{
									break;
								}
							}
						}
						else
						{
							bool flag5 = array.Length > 1;
							if (flag5)
							{
								int num2 = (int)MessageBox.Show("The clipboard contains multiple lines.\nMacro editor will begin pasting at the currently selected line. Ok?\nPress Yes for to start here, No to start at at the macro title, and cancel to abort.", "Macro Editor", MessageBoxButtons.YesNoCancel);
								bool flag6 = num2 == 6;
								if (flag6)
								{
									this.MacroContainer[this.xBook, this.xRow, this.xMacro] = new string[]
									{
										"",
										"",
										"",
										"",
										"",
										"",
										""
									};
									int num3 = array.Length - 1;
									for (int j = 0; j <= num3; j++)
									{
										this.Lines[this.CurrentLine + j].Text = array[j];
										bool flag7 = this.CurrentLine + j == 6;
										if (flag7)
										{
											break;
										}
									}
								}
								else
								{
									bool flag8 = num2 == 7;
									if (flag8)
									{
										int num4 = array.Length - 1;
										for (int k = 0; k <= num4; k++)
										{
											this.Lines[k].Text = array[k];
											bool flag9 = k == 6;
											if (flag9)
											{
												break;
											}
										}
									}
								}
							}
						}
					}
				}
				this.SomethingEdited = true;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000084B0 File Offset: 0x000066B0
		private void menuText_Copy_Click(object sender, EventArgs e)
		{
			bool flag = Conversions.ToBoolean(this.Lines[this.CurrentLine].SelectedText);
			if (flag)
			{
				Clipboard.SetText(this.Lines[this.CurrentLine].SelectedText);
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000084FB File Offset: 0x000066FB
		private void menuText_Cut_Click(object sender, EventArgs e)
		{
			this.menuText_Copy_Click(RuntimeHelpers.GetObjectValue(sender), e);
			this.Lines[this.CurrentLine].SelectedText = "";
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00008528 File Offset: 0x00006728
		private void Form1_Resize(object sender, EventArgs e)
		{
			this.rs.ResizeAllControls(this);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00008538 File Offset: 0x00006738
		private void MenuMain_Search_Click(object sender, EventArgs e)
		{
			Dictionary<int, string[]> dictionary = new Dictionary<int, string[]>();
			string text = Interaction.InputBox("Enter search phrase:", "Macro Editor", "", -1, -1);
			bool flag = text.Length > 0;
			checked
			{
				if (flag)
				{
					string value = text.ToLower();
					int num = this.debuglimit;
					for (int i = 0; i <= num; i++)
					{
						int num2 = 0;
						do
						{
							int num3 = 0;
							do
							{
								bool flag2 = this.MacroContainer[i, num2, num3][0].ToLower().Contains(value);
								if (flag2)
								{
									dictionary[dictionary.Count] = new string[]
									{
										Conversions.ToString(i),
										Conversions.ToString(num2),
										Conversions.ToString(num3),
										Conversions.ToString(0),
										Conversions.ToString(0),
										this.MacroContainer[i, num2, num3][0]
									};
								}
								int num4 = 1;
								do
								{
									string text2 = this.MacroContainer[i, num2, num3][num4];
									bool flag3 = text2.ToLower().Contains(value);
									if (flag3)
									{
										dictionary[dictionary.Count] = new string[]
										{
											Conversions.ToString(i),
											Conversions.ToString(num2),
											Conversions.ToString(num3),
											Conversions.ToString(num4),
											Conversions.ToString(0),
											this.MacroContainer[i, num2, num3][num4]
										};
									}
									num4++;
								}
								while (num4 <= 6);
								num3++;
							}
							while (num3 <= 19);
							num2++;
						}
						while (num2 <= 9);
					}
					bool flag4 = dictionary.Count > 1;
					if (flag4)
					{
						this.SearchResults.Hide();
						this.SearchResults = new Assessment();
						foreach (KeyValuePair<int, string[]> keyValuePair in dictionary)
						{
							this.SearchResults.AddResult(keyValuePair.Value[0], keyValuePair.Value[1], keyValuePair.Value[2], keyValuePair.Value[3], keyValuePair.Value[4], keyValuePair.Value[5], Color.White, Color.Black);
						}
						this.SearchResults.Show(this);
					}
					else
					{
						Interaction.MsgBox("No matches found in any macro.", MsgBoxStyle.OkOnly, null);
					}
				}
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000087B8 File Offset: 0x000069B8
		private void MenuBook_Wizard_BLM_Click(object sender, EventArgs e)
		{
			string input = "Type: Book BLM\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizzaja\n/ma \"Blizzaja\" <stnpc>\n\nMacro:\nFiraja\n/ma \"Firaja\" <stnpc>\n\nMacro:\nWaterja\n/ma \"Waterja\" <stnpc>\n\nMacro:\nThundaja\n/ma \"Thundaja\" <stnpc>\n\nMacro:\nStoneja\n/ma \"Stoneja\" <stnpc>\n\nMacro:\nAeroja\n/ma \"Aeroja\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\n\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizard6\n/ma \"Blizzard VI\" <stnpc>\n\nMacro:\nFire6\n/ma \"Fire VI\" <stnpc>\n\nMacro:\nWater6\n/ma \"Water VI\" <stnpc>\n\nMacro:\nThunder6\n/ma \"Thunder VI\" <stnpc>\n\nMacro:\nStone6\n/ma \"Stone VI\" <stnpc>\n\nMacro:\nAero6\n/ma \"Aero VI\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\n\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizard5\n/ma \"Blizzard V\" <stnpc>\n\nMacro:\nFire5\n/ma \"Fire V\" <stnpc>\n\nMacro:\nWater5\n/ma \"Water V\" <stnpc>\n\nMacro:\nThunder5\n/ma \"Thunder V\" <stnpc>\n\nMacro:\nStone5\n/ma \"Stone V\" <stnpc>\n\nMacro:\nAero5\n/ma \"Aero V\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizard4\n/ma \"Blizzard IV\" <stnpc>\n\nMacro:\nFire4\n/ma \"Fire IV\" <stnpc>\n\nMacro:\nWater4\n/ma \"Water IV\" <stnpc>\n\nMacro:\nThunder4\n/ma \"Thunder IV\" <stnpc>\n\nMacro:\nStone4\n/ma \"Stone IV\" <stnpc>\n\nMacro:\nAero4\n/ma \"Aero IV\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizard3\n/ma \"Blizzard III\" <stnpc>\n\nMacro:\nFire3\n/ma \"Fire III\" <stnpc>\n\nMacro:\nWater3\n/ma \"Water III\" <stnpc>\n\nMacro:\nThunder3\n/ma \"Thunder III\" <stnpc>\n\nMacro:\nStone 3\n/ma \"Stone III\" <stnpc>\n\nMacro:\nAero3\n/ma \"Aero III\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBliz2\n/ma \"Blizzard II\" <stnpc>\n\nMacro:\nFire2\n/ma \"Fire II\" <stnpc>\n\nMacro:\nWater2\n/ma \"Water II\" <stnpc>\n\nMacro:\nThun2\n/ma \"Thunder II\" <stnpc>\n\nMacro:\nStone2\n/ma \"Stone II\" <stnpc>\n\nMacro:\nAero2\n/ma \"Aero II\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nSleep2\n/ma \"Sleep II\" <stnpc>\n\nMacro:\nBreak\n/ma \"Break\" <stnpc>\n\nMacro:\nSleep\n/ma \"Sleep\" <stnpc>\n\nMacro:\nGravity\n/ma \"Gravity\" <stnpc>\n\nMacro:\nDistract\n/ma \"Distract\" <stnpc>\n\nMacro:\nFrazzle\n/ma \"Frazzle\" <stnpc>\n\nMacro:\nSleepga\n/ma \"Sleepga\" <stnpc>\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nSleep2\n/ma \"Sleep II\" <stnpc>\n\nMacro:\nFiraga\n/ma \"Firaga\" <stnpc>\n\nMacro:\nWaterga\n/ma \"Waterga\" <stnpc>\n\nMacro:\nGravity\n/ma \"Gravity\" <stnpc>\n\nMacro:\nStonega\n/ma \"Stonega\" <stnpc>\n\nMacro:\nAeroga\n/ma \"Aeroga\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizzag\n/ma \"Blizzaga II\" <stnpc>\n\nMacro:\nFiraga2\n/ma \"Firaga II\" <stnpc>\n\nMacro:\nWaterga2\n/ma \"Waterga II\" <stnpc>\n\nMacro:\nThundag2\n/ma \"Thundaga II\" <stnpc>\n\nMacro:\nStonega2\n/ma \"Stonega II\" <stnpc>\n\nMacro:\nAeroga2\n/ma \"Aeroga II\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nHome\n/macro set 1\n\nMacro:\nBlizzag3\n/ma \"Blizzaga III\" <stnpc>\n\nMacro:\nFiraga3\n/ma \"Firaga III\" <stnpc>\n\nMacro:\nWaterga3\n/ma \"Waterga III\" <stnpc>\n\nMacro:\nThundag3\n/ma \"Thundaga III\" <stnpc>\n\nMacro:\nStonega3\n/ma \"Stonega III\" <stnpc>\n\nMacro:\nAeroga3\n/ma \"Aeroga III\" <stnpc>\n\nMacro:\nCCs\n/macro set 7\n\nMacro:\nBreakga\n/ma \"Breakga\" <stnpc>\n\nMacro:\nSleepga2\n/ma \"Sleepga II\" <stnpc>\n\nMacro:\nCure IV\n/ma \"Cure IV\" <stal>\n\nMacro:\nCure III\n/ma \"Cure III\" <stal>\n\nMacro:\n\n\nMacro:\nAquaveil\n/ma \"Aquaveil\" <me>\n\nMacro:\nBlink\n/ma \"Blink\" <me>\n\nMacro:\nStoneski\n/ma \"Stoneskin\" <me>\n\nMacro:\nRefresh\n/ma \"Refresh\" <stpt>\n\nMacro:\nHaste\n/ma \"Haste\" <stal>\n\nMacro:\nAspirs\n/ma \"Aspir III\" <stnpc>\n/ma \"Aspir II\" <lastst>\n/ma \"Aspir\" <lastst>\n\nMacro:\nStun\n/ma \"Stun\" <stnpc>\n\nRow, Macro:\nEndBook (Please do not remove empty lines as they're part of the sharing format). If you experience problems pasting, please make sure to download an up to date version.";
			Regex regex = new Regex("(\\r\\n|\\r|\\n)");
			this.MenuBook_PasteClipboard_Click(RuntimeHelpers.GetObjectValue(sender), e, regex.Replace(input, "\n"));
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000087F4 File Offset: 0x000069F4
		private void MenuBook_SaveFiles_Click(object sender, EventArgs e)
		{
			int num = (int)MessageBox.Show(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("This will save all macros for Book ", this.Contents.Items[this.cbook]), " to file."), '\n'), '\n'), "Proceed?"), '\n'), '\n'), "This will also update the in-memory backup that can"), '\n'), "be restored with Revert from Main, Book, Row, & Macro menus.")), "Macro Editor", MessageBoxButtons.YesNo);
			bool flag = num == 6;
			checked
			{
				if (flag)
				{
					int num2 = 0;
					for (;;)
					{
						bool flag2 = !this.WriteFile(this.cbook, num2);
						if (flag2)
						{
							break;
						}
						this.UpdateStatusBar(Conversions.ToString(this.Contents.Items[this.cbook]), "Writing " + MacroEditorUtils.GetMacroFileSuffix(this.cbook, num2) + ".dat");
						this.StatusBar.Refresh();
						num2++;
						if (num2 > 9)
						{
							return;
						}
					}
					this.UpdateStatusBar("Error!", "An error occurred while writing " + MacroEditorUtils.GetMacroFileSuffix(this.cbook, num2) + ".dat");
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000893C File Offset: 0x00006B3C
		private void MenuHelp_FeatureTour_Click(object sender, EventArgs e)
		{
			base.TopMost = true;
			checked
			{
				base.Top = (int)Math.Round((double)(MyProject.Computer.Screen.Bounds.Size.Height - base.Height) / 2.0);
				base.Left = (int)Math.Round((double)(MyProject.Computer.Screen.Bounds.Size.Width - base.Width) / 2.0);
				this.UpdateStatusBar("", "Always On Top Set While Feature Tour Is running.");
				this.MenuBook.Enabled = false;
				this.MenuBook.Show(new Point(base.Left + 50, base.Top + 100));
				Interaction.MsgBox("This Is the macro book menu, operations that can be performed on any book. You can interact with books without selecting them.", MsgBoxStyle.OkOnly, null);
				this.MenuRow.Enabled = false;
				this.MenuRow.Show(new Point(base.Left + 160, base.Top + 400));
				Interaction.MsgBox("The row menu, you can also interact with rows without selecting them.", MsgBoxStyle.OkOnly, null);
				this.MenuHandler.Enabled = false;
				this.MenuHandler.Show(new Point(base.Left + 230, base.Top + 85));
				Interaction.MsgBox("Individual menus Control-side and Alternate side.", MsgBoxStyle.OkOnly, null);
				this.MenuMacro.Enabled = false;
				this.MenuMacro.Show(new Point(base.Left + 800, base.Top + 100));
				Interaction.MsgBox("The macro menu. If you copy to clipboard, you can share online or select a macro and immediately press Ctrl+V.", MsgBoxStyle.OkOnly, null);
				this.MenuText.Enabled = false;
				this.MenuText.Show(new Point(base.Left + 800, base.Top + 285));
				Interaction.MsgBox("This menu can be accessed by right-clicking a text box or pressing F2 While inside a textbox.", MsgBoxStyle.OkOnly, null);
				this.MenuText.Hide();
				this.MenuBook.Enabled = true;
				this.MenuRow.Enabled = true;
				this.MenuHandler.Enabled = true;
				this.MenuMacro.Enabled = true;
				this.MenuText.Enabled = true;
				base.TopMost = false;
				this.UpdateStatusBar("", "");
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00008B88 File Offset: 0x00006D88
		private void MenuBook_RenameBook_Click(object sender, EventArgs e)
		{
			this.Contents.SelectedIndexChanged -= this.Contents_SelectedIndexChanged;
			string text = Interaction.InputBox("Enter book name.", "Macro Editor", Conversions.ToString(this.Contents.SelectedItem), -1, -1);
			bool flag = Operators.CompareString(text, "", false) != 0;
			if (flag)
			{
				this.Contents.Items[this.cbook] = text.Substring(0, Math.Min(15, text.Length));
			}
			this.Contents.SelectedIndexChanged += this.Contents_SelectedIndexChanged;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00008C28 File Offset: 0x00006E28
		private void MenuBook_MacroMap_Click(object sender, EventArgs e)
		{
			bool enabled = this.Contents.Enabled;
			checked
			{
				if (enabled)
				{
					this.MacroMap.Hide();
					this.MacroMap = new MacroMapForm();
					int num = 0;
					do
					{
						int num2 = 0;
						do
						{
							this.MacroMap.Add(this.cbook, num, num2, this.MacroContainer[this.cbook, num, num2]);
							num2++;
						}
						while (num2 <= 19);
						num++;
					}
					while (num <= 9);
					this.MacroMap.Show();
				}
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00008CA8 File Offset: 0x00006EA8
		private void MenuBook_CopyMacroMap_Click(object sender, EventArgs e)
		{
			bool enabled = this.Contents.Enabled;
			checked
			{
				if (enabled)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("[table style='width:1500px;']");
					string[] array = new string[6];
					int num = 0;
					do
					{
						stringBuilder.Append("[tr][td]Row " + Conversions.ToString(num + 1) + "[/td][/tr][tr]");
						int num2 = 0;
						do
						{
							stringBuilder.Append("[td][b]" + this.MacroContainer[this.cbook, num, num2][0] + "[/b]\n");
							Array.Copy(this.MacroContainer[this.cbook, num, num2], 1, array, 0, 1);
							stringBuilder.Append(Strings.Join(array, "\n") + "[/td]\n");
							bool flag = num2 == 9;
							if (flag)
							{
								stringBuilder.Append("[/tr][tr]");
							}
							num2++;
						}
						while (num2 <= 19);
						stringBuilder.Append("[/tr]");
						num++;
					}
					while (num <= 9);
					stringBuilder.Append("[/table]");
					Clipboard.SetText(stringBuilder.ToString());
				}
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00008DC4 File Offset: 0x00006FC4
		private void MenuRow_Save_Click(object sender, EventArgs e)
		{
			int num = (int)MessageBox.Show("This will save this macro to file.\n\nProceed?\n\nThis will also update the in-memory backup that can\nbe restored with Revert from Main, Book, Row, & Macro menus.", "Macro Editor", MessageBoxButtons.YesNo);
			bool flag = num == 6;
			if (flag)
			{
				bool flag2 = !this.WriteFile(this.xBook, this.xRow);
				if (flag2)
				{
					this.UpdateStatusBar("Error", "An error occurred while writing files.");
				}
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00008E1A File Offset: 0x0000701A
		private void MenuRow_CopyLocation_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(Conversions.ToString(Operators.ConcatenateObject(this.macropath + "\\", NewLateBinding.LateGet(sender, null, "tag", new object[0], null, null, null))));
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00008E54 File Offset: 0x00007054
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			bool flag = e.CloseReason == CloseReason.UserClosing & this.SomethingEdited;
			if (flag)
			{
				DialogResult dialogResult = MessageBox.Show("You have made unsaved changes. Do you really want to exit and lose all changes?", "Macro Editor", MessageBoxButtons.YesNo);
				bool flag2 = dialogResult == DialogResult.Yes;
				if (!flag2)
				{
					e.Cancel = true;
				}
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00008EA0 File Offset: 0x000070A0
		private void MenuMacro_Destination_Click(object sender, EventArgs e)
		{
			Destination destination = MyProject.Forms.Destination;
			bool flag = this.xBook >= 0 & this.xRow >= 0;
			if (flag)
			{
				destination.xBook = this.xBook;
				destination.xRow = this.xRow;
				destination.tMacro = Conversions.ToInteger(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "GetCurrentParent", new object[0], null, null, null), null, "SourceControl", new object[0], null, null, null), null, "tag", new object[0], null, null, null));
				destination.ShowDialog();
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00008F3F File Offset: 0x0000713F
		private void File_Exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00008F4C File Offset: 0x0000714C
		private void MainForm_MouseWheel(object sender, MouseEventArgs e)
		{
			bool flag = e.Delta > 0;
			checked
			{
				if (flag)
				{
					this.Rows[Math.Max(this.xRow - 1, 0)].PerformClick();
				}
				else
				{
					this.Rows[Math.Min(this.xRow + 1, 9)].PerformClick();
				}
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00008FAC File Offset: 0x000071AC
		private void MenuBook_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			bool flag = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
			if (flag)
			{
				string name = this.MenuBook.GetItemAt(checked(new Point(Cursor.Position.X - this.MenuBook.Left, Cursor.Position.Y - this.MenuBook.Top))).Name;
				bool flag2 = Operators.CompareString(name, "MenuBook_Header", false) == 0;
				if (flag2)
				{
					e.Cancel = true;
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000A861 File Offset: 0x00008A61
		// (set) Token: 0x06000099 RID: 153 RVA: 0x0000A86B File Offset: 0x00008A6B
		internal virtual MenuStrip MainMenu { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600009A RID: 154 RVA: 0x0000A874 File Offset: 0x00008A74
		// (set) Token: 0x0600009B RID: 155 RVA: 0x0000A880 File Offset: 0x00008A80
		internal virtual ContextMenuStrip MenuMacro
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				CancelEventHandler value2 = new CancelEventHandler(this.MenuMacro_Opening);
				ContextMenuStrip menuMacro = this._MenuMacro;
				if (menuMacro != null)
				{
					menuMacro.Opening -= value2;
				}
				this._MenuMacro = value;
				menuMacro = this._MenuMacro;
				if (menuMacro != null)
				{
					menuMacro.Opening += value2;
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600009C RID: 156 RVA: 0x0000A8C3 File Offset: 0x00008AC3
		// (set) Token: 0x0600009D RID: 157 RVA: 0x0000A8D0 File Offset: 0x00008AD0
		internal virtual ToolStripMenuItem MenuMacro_Cut
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_Cut;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_Cut_Click);
				ToolStripMenuItem menuMacro_Cut = this._MenuMacro_Cut;
				if (menuMacro_Cut != null)
				{
					menuMacro_Cut.Click -= value2;
				}
				this._MenuMacro_Cut = value;
				menuMacro_Cut = this._MenuMacro_Cut;
				if (menuMacro_Cut != null)
				{
					menuMacro_Cut.Click += value2;
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600009E RID: 158 RVA: 0x0000A913 File Offset: 0x00008B13
		// (set) Token: 0x0600009F RID: 159 RVA: 0x0000A920 File Offset: 0x00008B20
		internal virtual ToolStripMenuItem MenuMacro_Copy
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_Copy;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_Copy_Click);
				ToolStripMenuItem menuMacro_Copy = this._MenuMacro_Copy;
				if (menuMacro_Copy != null)
				{
					menuMacro_Copy.Click -= value2;
				}
				this._MenuMacro_Copy = value;
				menuMacro_Copy = this._MenuMacro_Copy;
				if (menuMacro_Copy != null)
				{
					menuMacro_Copy.Click += value2;
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000A963 File Offset: 0x00008B63
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x0000A970 File Offset: 0x00008B70
		internal virtual ToolStripMenuItem MenuMacro_Paste
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_Paste;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_Paste_Click);
				ToolStripMenuItem menuMacro_Paste = this._MenuMacro_Paste;
				if (menuMacro_Paste != null)
				{
					menuMacro_Paste.Click -= value2;
				}
				this._MenuMacro_Paste = value;
				menuMacro_Paste = this._MenuMacro_Paste;
				if (menuMacro_Paste != null)
				{
					menuMacro_Paste.Click += value2;
				}
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x0000A9B3 File Offset: 0x00008BB3
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x0000A9C0 File Offset: 0x00008BC0
		internal virtual ToolStripMenuItem MenuMacro_Revert
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_Revert;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_Revert_Click);
				ToolStripMenuItem menuMacro_Revert = this._MenuMacro_Revert;
				if (menuMacro_Revert != null)
				{
					menuMacro_Revert.Click -= value2;
				}
				this._MenuMacro_Revert = value;
				menuMacro_Revert = this._MenuMacro_Revert;
				if (menuMacro_Revert != null)
				{
					menuMacro_Revert.Click += value2;
				}
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x0000AA03 File Offset: 0x00008C03
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x0000AA0D File Offset: 0x00008C0D
		internal virtual ToolStripSeparator h1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000AA16 File Offset: 0x00008C16
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x0000AA20 File Offset: 0x00008C20
		internal virtual ToolStripMenuItem MenuMacro_Clear
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_Clear;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_Clear_Click);
				ToolStripMenuItem menuMacro_Clear = this._MenuMacro_Clear;
				if (menuMacro_Clear != null)
				{
					menuMacro_Clear.Click -= value2;
				}
				this._MenuMacro_Clear = value;
				menuMacro_Clear = this._MenuMacro_Clear;
				if (menuMacro_Clear != null)
				{
					menuMacro_Clear.Click += value2;
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000AA63 File Offset: 0x00008C63
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x0000AA70 File Offset: 0x00008C70
		internal virtual ContextMenuStrip MenuRow
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				CancelEventHandler value2 = new CancelEventHandler(this.MenuRow_Opening);
				ContextMenuStrip menuRow = this._MenuRow;
				if (menuRow != null)
				{
					menuRow.Opening -= value2;
				}
				this._MenuRow = value;
				menuRow = this._MenuRow;
				if (menuRow != null)
				{
					menuRow.Opening += value2;
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000AAB3 File Offset: 0x00008CB3
		// (set) Token: 0x060000AB RID: 171 RVA: 0x0000AAC0 File Offset: 0x00008CC0
		internal virtual ToolStripMenuItem MenuRow_Cut
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Cut;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Cut_Click);
				ToolStripMenuItem menuRow_Cut = this._MenuRow_Cut;
				if (menuRow_Cut != null)
				{
					menuRow_Cut.Click -= value2;
				}
				this._MenuRow_Cut = value;
				menuRow_Cut = this._MenuRow_Cut;
				if (menuRow_Cut != null)
				{
					menuRow_Cut.Click += value2;
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000AB03 File Offset: 0x00008D03
		// (set) Token: 0x060000AD RID: 173 RVA: 0x0000AB10 File Offset: 0x00008D10
		internal virtual ToolStripMenuItem MenuRow_Copy
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Copy;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Copy_Click);
				ToolStripMenuItem menuRow_Copy = this._MenuRow_Copy;
				if (menuRow_Copy != null)
				{
					menuRow_Copy.Click -= value2;
				}
				this._MenuRow_Copy = value;
				menuRow_Copy = this._MenuRow_Copy;
				if (menuRow_Copy != null)
				{
					menuRow_Copy.Click += value2;
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000AB53 File Offset: 0x00008D53
		// (set) Token: 0x060000AF RID: 175 RVA: 0x0000AB60 File Offset: 0x00008D60
		internal virtual ToolStripMenuItem MenuRow_Paste
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Paste;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Paste_Click);
				ToolStripMenuItem menuRow_Paste = this._MenuRow_Paste;
				if (menuRow_Paste != null)
				{
					menuRow_Paste.Click -= value2;
				}
				this._MenuRow_Paste = value;
				menuRow_Paste = this._MenuRow_Paste;
				if (menuRow_Paste != null)
				{
					menuRow_Paste.Click += value2;
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x0000ABA3 File Offset: 0x00008DA3
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x0000ABAD File Offset: 0x00008DAD
		internal virtual ToolStripSeparator h2 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000ABB6 File Offset: 0x00008DB6
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x0000ABC0 File Offset: 0x00008DC0
		internal virtual ToolStripMenuItem MenuRow_Clear
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Clear;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Clear_Click);
				ToolStripMenuItem menuRow_Clear = this._MenuRow_Clear;
				if (menuRow_Clear != null)
				{
					menuRow_Clear.Click -= value2;
				}
				this._MenuRow_Clear = value;
				menuRow_Clear = this._MenuRow_Clear;
				if (menuRow_Clear != null)
				{
					menuRow_Clear.Click += value2;
				}
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000AC03 File Offset: 0x00008E03
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x0000AC10 File Offset: 0x00008E10
		internal virtual ToolStripMenuItem MenuRow_Revert
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Revert;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Revert_Click);
				ToolStripMenuItem menuRow_Revert = this._MenuRow_Revert;
				if (menuRow_Revert != null)
				{
					menuRow_Revert.Click -= value2;
				}
				this._MenuRow_Revert = value;
				menuRow_Revert = this._MenuRow_Revert;
				if (menuRow_Revert != null)
				{
					menuRow_Revert.Click += value2;
				}
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000AC53 File Offset: 0x00008E53
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x0000AC60 File Offset: 0x00008E60
		internal virtual ToolStripMenuItem MenuMacro_CopyClipboard
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_CopyClipboard;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_CopyClipboard_Click);
				ToolStripMenuItem menuMacro_CopyClipboard = this._MenuMacro_CopyClipboard;
				if (menuMacro_CopyClipboard != null)
				{
					menuMacro_CopyClipboard.Click -= value2;
				}
				this._MenuMacro_CopyClipboard = value;
				menuMacro_CopyClipboard = this._MenuMacro_CopyClipboard;
				if (menuMacro_CopyClipboard != null)
				{
					menuMacro_CopyClipboard.Click += value2;
				}
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000ACA3 File Offset: 0x00008EA3
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x0000ACB0 File Offset: 0x00008EB0
		internal virtual ToolStripMenuItem MenuMacro_PasteClipboard
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_PasteClipboard;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = delegate(object a0, EventArgs a1)
				{
					this.MenuMacro_PasteClipboard_Click(RuntimeHelpers.GetObjectValue(a0), a1, -1);
				};
				ToolStripMenuItem menuMacro_PasteClipboard = this._MenuMacro_PasteClipboard;
				if (menuMacro_PasteClipboard != null)
				{
					menuMacro_PasteClipboard.Click -= value2;
				}
				this._MenuMacro_PasteClipboard = value;
				menuMacro_PasteClipboard = this._MenuMacro_PasteClipboard;
				if (menuMacro_PasteClipboard != null)
				{
					menuMacro_PasteClipboard.Click += value2;
				}
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000ACF3 File Offset: 0x00008EF3
		// (set) Token: 0x060000BB RID: 187 RVA: 0x0000AD00 File Offset: 0x00008F00
		internal virtual ToolStripMenuItem MenuRow_CopyClipboard
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_CopyClipboard;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_CopyClipboard_Click);
				ToolStripMenuItem menuRow_CopyClipboard = this._MenuRow_CopyClipboard;
				if (menuRow_CopyClipboard != null)
				{
					menuRow_CopyClipboard.Click -= value2;
				}
				this._MenuRow_CopyClipboard = value;
				menuRow_CopyClipboard = this._MenuRow_CopyClipboard;
				if (menuRow_CopyClipboard != null)
				{
					menuRow_CopyClipboard.Click += value2;
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000BC RID: 188 RVA: 0x0000AD43 File Offset: 0x00008F43
		// (set) Token: 0x060000BD RID: 189 RVA: 0x0000AD50 File Offset: 0x00008F50
		internal virtual ToolStripMenuItem MenuRow_PasteClipboard
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_PasteClipboard;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_PasteClipboard_Click);
				ToolStripMenuItem menuRow_PasteClipboard = this._MenuRow_PasteClipboard;
				if (menuRow_PasteClipboard != null)
				{
					menuRow_PasteClipboard.Click -= value2;
				}
				this._MenuRow_PasteClipboard = value;
				menuRow_PasteClipboard = this._MenuRow_PasteClipboard;
				if (menuRow_PasteClipboard != null)
				{
					menuRow_PasteClipboard.Click += value2;
				}
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000BE RID: 190 RVA: 0x0000AD93 File Offset: 0x00008F93
		// (set) Token: 0x060000BF RID: 191 RVA: 0x0000ADA0 File Offset: 0x00008FA0
		internal virtual ToolStripMenuItem MenuMain_evaluate
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMain_evaluate;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMain_evaluate_Click);
				ToolStripMenuItem menuMain_evaluate = this._MenuMain_evaluate;
				if (menuMain_evaluate != null)
				{
					menuMain_evaluate.Click -= value2;
				}
				this._MenuMain_evaluate = value;
				menuMain_evaluate = this._MenuMain_evaluate;
				if (menuMain_evaluate != null)
				{
					menuMain_evaluate.Click += value2;
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x0000ADE3 File Offset: 0x00008FE3
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x0000ADF0 File Offset: 0x00008FF0
		internal virtual ContextMenuStrip MenuBook
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				CancelEventHandler value2 = new CancelEventHandler(this.MenuBook_Opening);
				ToolStripDropDownClosingEventHandler value3 = new ToolStripDropDownClosingEventHandler(this.MenuBook_Closing);
				ContextMenuStrip menuBook = this._MenuBook;
				if (menuBook != null)
				{
					menuBook.Opening -= value2;
					menuBook.Closing -= value3;
				}
				this._MenuBook = value;
				menuBook = this._MenuBook;
				if (menuBook != null)
				{
					menuBook.Opening += value2;
					menuBook.Closing += value3;
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000AE4E File Offset: 0x0000904E
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x0000AE58 File Offset: 0x00009058
		internal virtual ToolStripMenuItem MenuBook_Cut
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Cut;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Cut_Click);
				ToolStripMenuItem menuBook_Cut = this._MenuBook_Cut;
				if (menuBook_Cut != null)
				{
					menuBook_Cut.Click -= value2;
				}
				this._MenuBook_Cut = value;
				menuBook_Cut = this._MenuBook_Cut;
				if (menuBook_Cut != null)
				{
					menuBook_Cut.Click += value2;
				}
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x0000AE9B File Offset: 0x0000909B
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x0000AEA8 File Offset: 0x000090A8
		internal virtual ToolStripMenuItem MenuBook_Copy
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Copy;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Copy_Click);
				ToolStripMenuItem menuBook_Copy = this._MenuBook_Copy;
				if (menuBook_Copy != null)
				{
					menuBook_Copy.Click -= value2;
				}
				this._MenuBook_Copy = value;
				menuBook_Copy = this._MenuBook_Copy;
				if (menuBook_Copy != null)
				{
					menuBook_Copy.Click += value2;
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x0000AEEB File Offset: 0x000090EB
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x0000AEF8 File Offset: 0x000090F8
		internal virtual ToolStripMenuItem MenuBook_Paste
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Paste;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Paste_Click);
				ToolStripMenuItem menuBook_Paste = this._MenuBook_Paste;
				if (menuBook_Paste != null)
				{
					menuBook_Paste.Click -= value2;
				}
				this._MenuBook_Paste = value;
				menuBook_Paste = this._MenuBook_Paste;
				if (menuBook_Paste != null)
				{
					menuBook_Paste.Click += value2;
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x0000AF3B File Offset: 0x0000913B
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x0000AF45 File Offset: 0x00009145
		internal virtual ToolStripSeparator MenuBook_h1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000CA RID: 202 RVA: 0x0000AF4E File Offset: 0x0000914E
		// (set) Token: 0x060000CB RID: 203 RVA: 0x0000AF58 File Offset: 0x00009158
		internal virtual ToolStripMenuItem MenuBook_Clear
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Clear;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Clear_Click);
				ToolStripMenuItem menuBook_Clear = this._MenuBook_Clear;
				if (menuBook_Clear != null)
				{
					menuBook_Clear.Click -= value2;
				}
				this._MenuBook_Clear = value;
				menuBook_Clear = this._MenuBook_Clear;
				if (menuBook_Clear != null)
				{
					menuBook_Clear.Click += value2;
				}
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000CC RID: 204 RVA: 0x0000AF9B File Offset: 0x0000919B
		// (set) Token: 0x060000CD RID: 205 RVA: 0x0000AFA8 File Offset: 0x000091A8
		internal virtual ToolStripMenuItem MenuBook_Revert
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Revert;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Revert_Click);
				ToolStripMenuItem menuBook_Revert = this._MenuBook_Revert;
				if (menuBook_Revert != null)
				{
					menuBook_Revert.Click -= value2;
				}
				this._MenuBook_Revert = value;
				menuBook_Revert = this._MenuBook_Revert;
				if (menuBook_Revert != null)
				{
					menuBook_Revert.Click += value2;
				}
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000AFEB File Offset: 0x000091EB
		// (set) Token: 0x060000CF RID: 207 RVA: 0x0000AFF5 File Offset: 0x000091F5
		internal virtual ToolStripSeparator MenuBook_h2 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000AFFE File Offset: 0x000091FE
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x0000B008 File Offset: 0x00009208
		internal virtual ToolStripMenuItem MenuBook_CopyClipboard
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_CopyClipboard;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_CopyClipboard_Click);
				ToolStripMenuItem menuBook_CopyClipboard = this._MenuBook_CopyClipboard;
				if (menuBook_CopyClipboard != null)
				{
					menuBook_CopyClipboard.Click -= value2;
				}
				this._MenuBook_CopyClipboard = value;
				menuBook_CopyClipboard = this._MenuBook_CopyClipboard;
				if (menuBook_CopyClipboard != null)
				{
					menuBook_CopyClipboard.Click += value2;
				}
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x0000B04B File Offset: 0x0000924B
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x0000B058 File Offset: 0x00009258
		internal virtual ToolStripMenuItem MenuBook_PasteClipboard
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_PasteClipboard;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = delegate(object a0, EventArgs a1)
				{
					this.MenuBook_PasteClipboard_Click(RuntimeHelpers.GetObjectValue(a0), a1, "");
				};
				ToolStripMenuItem menuBook_PasteClipboard = this._MenuBook_PasteClipboard;
				if (menuBook_PasteClipboard != null)
				{
					menuBook_PasteClipboard.Click -= value2;
				}
				this._MenuBook_PasteClipboard = value;
				menuBook_PasteClipboard = this._MenuBook_PasteClipboard;
				if (menuBook_PasteClipboard != null)
				{
					menuBook_PasteClipboard.Click += value2;
				}
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x0000B09B File Offset: 0x0000929B
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x0000B0A5 File Offset: 0x000092A5
		internal virtual ContextMenuStrip MenuHandler { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x0000B0AE File Offset: 0x000092AE
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x0000B0B8 File Offset: 0x000092B8
		internal virtual ToolStripMenuItem MenuHandler_ClearSide
		{
			[CompilerGenerated]
			get
			{
				return this._MenuHandler_ClearSide;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuHandler_ClearSide_Click);
				ToolStripMenuItem menuHandler_ClearSide = this._MenuHandler_ClearSide;
				if (menuHandler_ClearSide != null)
				{
					menuHandler_ClearSide.Click -= value2;
				}
				this._MenuHandler_ClearSide = value;
				menuHandler_ClearSide = this._MenuHandler_ClearSide;
				if (menuHandler_ClearSide != null)
				{
					menuHandler_ClearSide.Click += value2;
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x0000B0FB File Offset: 0x000092FB
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x0000B108 File Offset: 0x00009308
		internal virtual ToolStripMenuItem MenuHandler_CutSide
		{
			[CompilerGenerated]
			get
			{
				return this._MenuHandler_CutSide;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuHandler_CutSide_Click);
				ToolStripMenuItem menuHandler_CutSide = this._MenuHandler_CutSide;
				if (menuHandler_CutSide != null)
				{
					menuHandler_CutSide.Click -= value2;
				}
				this._MenuHandler_CutSide = value;
				menuHandler_CutSide = this._MenuHandler_CutSide;
				if (menuHandler_CutSide != null)
				{
					menuHandler_CutSide.Click += value2;
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000DA RID: 218 RVA: 0x0000B14B File Offset: 0x0000934B
		// (set) Token: 0x060000DB RID: 219 RVA: 0x0000B158 File Offset: 0x00009358
		internal virtual ToolStripMenuItem MenuHandler_CopySide
		{
			[CompilerGenerated]
			get
			{
				return this._MenuHandler_CopySide;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuHandler_CopySide_Click);
				ToolStripMenuItem menuHandler_CopySide = this._MenuHandler_CopySide;
				if (menuHandler_CopySide != null)
				{
					menuHandler_CopySide.Click -= value2;
				}
				this._MenuHandler_CopySide = value;
				menuHandler_CopySide = this._MenuHandler_CopySide;
				if (menuHandler_CopySide != null)
				{
					menuHandler_CopySide.Click += value2;
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000DC RID: 220 RVA: 0x0000B19B File Offset: 0x0000939B
		// (set) Token: 0x060000DD RID: 221 RVA: 0x0000B1A8 File Offset: 0x000093A8
		internal virtual ToolStripMenuItem MenuHandler_PasteSide
		{
			[CompilerGenerated]
			get
			{
				return this._MenuHandler_PasteSide;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuHandler_PasteSide_Click);
				ToolStripMenuItem menuHandler_PasteSide = this._MenuHandler_PasteSide;
				if (menuHandler_PasteSide != null)
				{
					menuHandler_PasteSide.Click -= value2;
				}
				this._MenuHandler_PasteSide = value;
				menuHandler_PasteSide = this._MenuHandler_PasteSide;
				if (menuHandler_PasteSide != null)
				{
					menuHandler_PasteSide.Click += value2;
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000DE RID: 222 RVA: 0x0000B1EB File Offset: 0x000093EB
		// (set) Token: 0x060000DF RID: 223 RVA: 0x0000B1F5 File Offset: 0x000093F5
		internal virtual OpenFileDialog OpenDialog { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000B1FE File Offset: 0x000093FE
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x0000B208 File Offset: 0x00009408
		internal virtual ToolStripMenuItem FileMenu { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x0000B211 File Offset: 0x00009411
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x0000B21B File Offset: 0x0000941B
		internal virtual ToolStripSeparator ToolStripMenuItem2 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x0000B224 File Offset: 0x00009424
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000B230 File Offset: 0x00009430
		internal virtual ToolStripMenuItem File_Exit
		{
			[CompilerGenerated]
			get
			{
				return this._File_Exit;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.File_Exit_Click);
				ToolStripMenuItem file_Exit = this._File_Exit;
				if (file_Exit != null)
				{
					file_Exit.Click -= value2;
				}
				this._File_Exit = value;
				file_Exit = this._File_Exit;
				if (file_Exit != null)
				{
					file_Exit.Click += value2;
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000B273 File Offset: 0x00009473
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x0000B280 File Offset: 0x00009480
		internal virtual ToolStripMenuItem File_Open
		{
			[CompilerGenerated]
			get
			{
				return this._File_Open;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.File_Open_Click);
				ToolStripMenuItem file_Open = this._File_Open;
				if (file_Open != null)
				{
					file_Open.Click -= value2;
				}
				this._File_Open = value;
				file_Open = this._File_Open;
				if (file_Open != null)
				{
					file_Open.Click += value2;
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000B2C3 File Offset: 0x000094C3
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x0000B2D0 File Offset: 0x000094D0
		internal virtual ToolStripMenuItem File_SaveRow
		{
			[CompilerGenerated]
			get
			{
				return this._File_SaveRow;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.File_SaveRow_Click);
				ToolStripMenuItem file_SaveRow = this._File_SaveRow;
				if (file_SaveRow != null)
				{
					file_SaveRow.Click -= value2;
				}
				this._File_SaveRow = value;
				file_SaveRow = this._File_SaveRow;
				if (file_SaveRow != null)
				{
					file_SaveRow.Click += value2;
				}
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000EA RID: 234 RVA: 0x0000B313 File Offset: 0x00009513
		// (set) Token: 0x060000EB RID: 235 RVA: 0x0000B320 File Offset: 0x00009520
		internal virtual ToolStripMenuItem File_SaveAll
		{
			[CompilerGenerated]
			get
			{
				return this._File_SaveAll;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.File_SaveAll_Click);
				ToolStripMenuItem file_SaveAll = this._File_SaveAll;
				if (file_SaveAll != null)
				{
					file_SaveAll.Click -= value2;
				}
				this._File_SaveAll = value;
				file_SaveAll = this._File_SaveAll;
				if (file_SaveAll != null)
				{
					file_SaveAll.Click += value2;
				}
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000EC RID: 236 RVA: 0x0000B363 File Offset: 0x00009563
		// (set) Token: 0x060000ED RID: 237 RVA: 0x0000B36D File Offset: 0x0000956D
		internal virtual ToolStripStatusLabel StatusH { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000B376 File Offset: 0x00009576
		// (set) Token: 0x060000EF RID: 239 RVA: 0x0000B380 File Offset: 0x00009580
		internal virtual ToolStripStatusLabel StatusD { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x0000B389 File Offset: 0x00009589
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x0000B393 File Offset: 0x00009593
		internal virtual ToolStripMenuItem MenuHelp { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x0000B39C File Offset: 0x0000959C
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x0000B3A8 File Offset: 0x000095A8
		internal virtual ToolStripMenuItem MenuBook_SaveBookNames
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_SaveBookNames;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_SaveBookNames_Click);
				ToolStripMenuItem menuBook_SaveBookNames = this._MenuBook_SaveBookNames;
				if (menuBook_SaveBookNames != null)
				{
					menuBook_SaveBookNames.Click -= value2;
				}
				this._MenuBook_SaveBookNames = value;
				menuBook_SaveBookNames = this._MenuBook_SaveBookNames;
				if (menuBook_SaveBookNames != null)
				{
					menuBook_SaveBookNames.Click += value2;
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000B3EB File Offset: 0x000095EB
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x0000B3F8 File Offset: 0x000095F8
		internal virtual ContextMenuStrip MenuText
		{
			[CompilerGenerated]
			get
			{
				return this._MenuText;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				CancelEventHandler value2 = new CancelEventHandler(this.MenuText_Opening);
				ContextMenuStrip menuText = this._MenuText;
				if (menuText != null)
				{
					menuText.Opening -= value2;
				}
				this._MenuText = value;
				menuText = this._MenuText;
				if (menuText != null)
				{
					menuText.Opening += value2;
				}
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000B43B File Offset: 0x0000963B
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x0000B448 File Offset: 0x00009648
		internal virtual ToolStripMenuItem menuText_Cut
		{
			[CompilerGenerated]
			get
			{
				return this._menuText_Cut;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.menuText_Cut_Click);
				ToolStripMenuItem menuText_Cut = this._menuText_Cut;
				if (menuText_Cut != null)
				{
					menuText_Cut.Click -= value2;
				}
				this._menuText_Cut = value;
				menuText_Cut = this._menuText_Cut;
				if (menuText_Cut != null)
				{
					menuText_Cut.Click += value2;
				}
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000B48B File Offset: 0x0000968B
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000B498 File Offset: 0x00009698
		internal virtual ToolStripMenuItem menuText_Copy
		{
			[CompilerGenerated]
			get
			{
				return this._menuText_Copy;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.menuText_Copy_Click);
				ToolStripMenuItem menuText_Copy = this._menuText_Copy;
				if (menuText_Copy != null)
				{
					menuText_Copy.Click -= value2;
				}
				this._menuText_Copy = value;
				menuText_Copy = this._menuText_Copy;
				if (menuText_Copy != null)
				{
					menuText_Copy.Click += value2;
				}
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000B4DB File Offset: 0x000096DB
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000B4E8 File Offset: 0x000096E8
		internal virtual ToolStripMenuItem menuText_Paste
		{
			[CompilerGenerated]
			get
			{
				return this._menuText_Paste;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = delegate(object a0, EventArgs a1)
				{
					this.menuText_Paste_Click(RuntimeHelpers.GetObjectValue(a0), a1, 0);
				};
				ToolStripMenuItem menuText_Paste = this._menuText_Paste;
				if (menuText_Paste != null)
				{
					menuText_Paste.Click -= value2;
				}
				this._menuText_Paste = value;
				menuText_Paste = this._menuText_Paste;
				if (menuText_Paste != null)
				{
					menuText_Paste.Click += value2;
				}
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000B52B File Offset: 0x0000972B
		// (set) Token: 0x060000FD RID: 253 RVA: 0x0000B535 File Offset: 0x00009735
		internal virtual ToolStripSeparator ToolStripMenuItem7 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000FE RID: 254 RVA: 0x0000B53E File Offset: 0x0000973E
		// (set) Token: 0x060000FF RID: 255 RVA: 0x0000B548 File Offset: 0x00009748
		internal virtual FolderBrowserDialog FolderBrowserDialog1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000100 RID: 256 RVA: 0x0000B551 File Offset: 0x00009751
		// (set) Token: 0x06000101 RID: 257 RVA: 0x0000B55B File Offset: 0x0000975B
		internal virtual ToolStripSeparator ToolStripMenuItem1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000102 RID: 258 RVA: 0x0000B564 File Offset: 0x00009764
		// (set) Token: 0x06000103 RID: 259 RVA: 0x0000B570 File Offset: 0x00009770
		internal virtual ToolStripMenuItem MenuBook_Import
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Import;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Import_Click);
				ToolStripMenuItem menuBook_Import = this._MenuBook_Import;
				if (menuBook_Import != null)
				{
					menuBook_Import.Click -= value2;
				}
				this._MenuBook_Import = value;
				menuBook_Import = this._MenuBook_Import;
				if (menuBook_Import != null)
				{
					menuBook_Import.Click += value2;
				}
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000104 RID: 260 RVA: 0x0000B5B3 File Offset: 0x000097B3
		// (set) Token: 0x06000105 RID: 261 RVA: 0x0000B5BD File Offset: 0x000097BD
		internal virtual ToolStripSeparator ToolStripMenuItem3 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000106 RID: 262 RVA: 0x0000B5C6 File Offset: 0x000097C6
		// (set) Token: 0x06000107 RID: 263 RVA: 0x0000B5D0 File Offset: 0x000097D0
		internal virtual ToolStripMenuItem MenuRow_Import
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Import;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Import_Click);
				ToolStripMenuItem menuRow_Import = this._MenuRow_Import;
				if (menuRow_Import != null)
				{
					menuRow_Import.Click -= value2;
				}
				this._MenuRow_Import = value;
				menuRow_Import = this._MenuRow_Import;
				if (menuRow_Import != null)
				{
					menuRow_Import.Click += value2;
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000108 RID: 264 RVA: 0x0000B613 File Offset: 0x00009813
		// (set) Token: 0x06000109 RID: 265 RVA: 0x0000B61D File Offset: 0x0000981D
		public virtual StatusStrip StatusBar { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000B626 File Offset: 0x00009826
		// (set) Token: 0x0600010B RID: 267 RVA: 0x0000B630 File Offset: 0x00009830
		internal virtual ToolTip ControlTip { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600010C RID: 268 RVA: 0x0000B639 File Offset: 0x00009839
		// (set) Token: 0x0600010D RID: 269 RVA: 0x0000B644 File Offset: 0x00009844
		internal virtual ToolStripMenuItem MenuMain_Search
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMain_Search;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMain_Search_Click);
				ToolStripMenuItem menuMain_Search = this._MenuMain_Search;
				if (menuMain_Search != null)
				{
					menuMain_Search.Click -= value2;
				}
				this._MenuMain_Search = value;
				menuMain_Search = this._MenuMain_Search;
				if (menuMain_Search != null)
				{
					menuMain_Search.Click += value2;
				}
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600010E RID: 270 RVA: 0x0000B687 File Offset: 0x00009887
		// (set) Token: 0x0600010F RID: 271 RVA: 0x0000B691 File Offset: 0x00009891
		internal virtual ToolStripMenuItem MenuBook_Wizard { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000110 RID: 272 RVA: 0x0000B69A File Offset: 0x0000989A
		// (set) Token: 0x06000111 RID: 273 RVA: 0x0000B6A4 File Offset: 0x000098A4
		internal virtual ToolStripMenuItem MenuBook_Wizard_BLM
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_Wizard_BLM;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_Wizard_BLM_Click);
				ToolStripMenuItem menuBook_Wizard_BLM = this._MenuBook_Wizard_BLM;
				if (menuBook_Wizard_BLM != null)
				{
					menuBook_Wizard_BLM.Click -= value2;
				}
				this._MenuBook_Wizard_BLM = value;
				menuBook_Wizard_BLM = this._MenuBook_Wizard_BLM;
				if (menuBook_Wizard_BLM != null)
				{
					menuBook_Wizard_BLM.Click += value2;
				}
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000B6E7 File Offset: 0x000098E7
		// (set) Token: 0x06000113 RID: 275 RVA: 0x0000B6F4 File Offset: 0x000098F4
		internal virtual ToolStripMenuItem MenuBook_SaveFiles
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_SaveFiles;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_SaveFiles_Click);
				ToolStripMenuItem menuBook_SaveFiles = this._MenuBook_SaveFiles;
				if (menuBook_SaveFiles != null)
				{
					menuBook_SaveFiles.Click -= value2;
				}
				this._MenuBook_SaveFiles = value;
				menuBook_SaveFiles = this._MenuBook_SaveFiles;
				if (menuBook_SaveFiles != null)
				{
					menuBook_SaveFiles.Click += value2;
				}
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000B737 File Offset: 0x00009937
		// (set) Token: 0x06000115 RID: 277 RVA: 0x0000B744 File Offset: 0x00009944
		internal virtual ToolStripMenuItem MenuHelp_Help
		{
			[CompilerGenerated]
			get
			{
				return this._MenuHelp_Help;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuHelp_Help_Click);
				ToolStripMenuItem menuHelp_Help = this._MenuHelp_Help;
				if (menuHelp_Help != null)
				{
					menuHelp_Help.Click -= value2;
				}
				this._MenuHelp_Help = value;
				menuHelp_Help = this._MenuHelp_Help;
				if (menuHelp_Help != null)
				{
					menuHelp_Help.Click += value2;
				}
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000B787 File Offset: 0x00009987
		// (set) Token: 0x06000117 RID: 279 RVA: 0x0000B794 File Offset: 0x00009994
		internal virtual ToolStripMenuItem MenuHelp_FeatureTour
		{
			[CompilerGenerated]
			get
			{
				return this._MenuHelp_FeatureTour;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuHelp_FeatureTour_Click);
				ToolStripMenuItem menuHelp_FeatureTour = this._MenuHelp_FeatureTour;
				if (menuHelp_FeatureTour != null)
				{
					menuHelp_FeatureTour.Click -= value2;
				}
				this._MenuHelp_FeatureTour = value;
				menuHelp_FeatureTour = this._MenuHelp_FeatureTour;
				if (menuHelp_FeatureTour != null)
				{
					menuHelp_FeatureTour.Click += value2;
				}
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000118 RID: 280 RVA: 0x0000B7D7 File Offset: 0x000099D7
		// (set) Token: 0x06000119 RID: 281 RVA: 0x0000B7E4 File Offset: 0x000099E4
		internal virtual ToolStripMenuItem MenuBook_RenameBook
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_RenameBook;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_RenameBook_Click);
				ToolStripMenuItem menuBook_RenameBook = this._MenuBook_RenameBook;
				if (menuBook_RenameBook != null)
				{
					menuBook_RenameBook.Click -= value2;
				}
				this._MenuBook_RenameBook = value;
				menuBook_RenameBook = this._MenuBook_RenameBook;
				if (menuBook_RenameBook != null)
				{
					menuBook_RenameBook.Click += value2;
				}
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600011A RID: 282 RVA: 0x0000B827 File Offset: 0x00009A27
		// (set) Token: 0x0600011B RID: 283 RVA: 0x0000B834 File Offset: 0x00009A34
		internal virtual ToolStripMenuItem MenuBook_MacroMap
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_MacroMap;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_MacroMap_Click);
				ToolStripMenuItem menuBook_MacroMap = this._MenuBook_MacroMap;
				if (menuBook_MacroMap != null)
				{
					menuBook_MacroMap.Click -= value2;
				}
				this._MenuBook_MacroMap = value;
				menuBook_MacroMap = this._MenuBook_MacroMap;
				if (menuBook_MacroMap != null)
				{
					menuBook_MacroMap.Click += value2;
				}
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000B877 File Offset: 0x00009A77
		// (set) Token: 0x0600011D RID: 285 RVA: 0x0000B884 File Offset: 0x00009A84
		internal virtual ToolStripMenuItem MenuRow_Save
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_Save;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_Save_Click);
				ToolStripMenuItem menuRow_Save = this._MenuRow_Save;
				if (menuRow_Save != null)
				{
					menuRow_Save.Click -= value2;
				}
				this._MenuRow_Save = value;
				menuRow_Save = this._MenuRow_Save;
				if (menuRow_Save != null)
				{
					menuRow_Save.Click += value2;
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600011E RID: 286 RVA: 0x0000B8C7 File Offset: 0x00009AC7
		// (set) Token: 0x0600011F RID: 287 RVA: 0x0000B8D4 File Offset: 0x00009AD4
		internal virtual ToolStripMenuItem MenuRow_CopyLocation
		{
			[CompilerGenerated]
			get
			{
				return this._MenuRow_CopyLocation;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuRow_CopyLocation_Click);
				ToolStripMenuItem menuRow_CopyLocation = this._MenuRow_CopyLocation;
				if (menuRow_CopyLocation != null)
				{
					menuRow_CopyLocation.Click -= value2;
				}
				this._MenuRow_CopyLocation = value;
				menuRow_CopyLocation = this._MenuRow_CopyLocation;
				if (menuRow_CopyLocation != null)
				{
					menuRow_CopyLocation.Click += value2;
				}
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000B917 File Offset: 0x00009B17
		// (set) Token: 0x06000121 RID: 289 RVA: 0x0000B924 File Offset: 0x00009B24
		internal virtual ToolStripMenuItem MenuBook_CopyMacroMap
		{
			[CompilerGenerated]
			get
			{
				return this._MenuBook_CopyMacroMap;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuBook_CopyMacroMap_Click);
				ToolStripMenuItem menuBook_CopyMacroMap = this._MenuBook_CopyMacroMap;
				if (menuBook_CopyMacroMap != null)
				{
					menuBook_CopyMacroMap.Click -= value2;
				}
				this._MenuBook_CopyMacroMap = value;
				menuBook_CopyMacroMap = this._MenuBook_CopyMacroMap;
				if (menuBook_CopyMacroMap != null)
				{
					menuBook_CopyMacroMap.Click += value2;
				}
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000122 RID: 290 RVA: 0x0000B967 File Offset: 0x00009B67
		// (set) Token: 0x06000123 RID: 291 RVA: 0x0000B971 File Offset: 0x00009B71
		internal virtual ToolStripSeparator ToolStripMenuItem4 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000B97A File Offset: 0x00009B7A
		// (set) Token: 0x06000125 RID: 293 RVA: 0x0000B984 File Offset: 0x00009B84
		internal virtual ToolStripMenuItem MenuMacro_Destination
		{
			[CompilerGenerated]
			get
			{
				return this._MenuMacro_Destination;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.MenuMacro_Destination_Click);
				ToolStripMenuItem menuMacro_Destination = this._MenuMacro_Destination;
				if (menuMacro_Destination != null)
				{
					menuMacro_Destination.Click -= value2;
				}
				this._MenuMacro_Destination = value;
				menuMacro_Destination = this._MenuMacro_Destination;
				if (menuMacro_Destination != null)
				{
					menuMacro_Destination.Click += value2;
				}
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000126 RID: 294 RVA: 0x0000B9C7 File Offset: 0x00009BC7
		// (set) Token: 0x06000127 RID: 295 RVA: 0x0000B9D1 File Offset: 0x00009BD1
		internal virtual ToolStripMenuItem MenuBook_Header { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0000B9DA File Offset: 0x00009BDA
		// (set) Token: 0x06000129 RID: 297 RVA: 0x0000B9E4 File Offset: 0x00009BE4
		internal virtual TextBox Warning { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }


		// Backing fields for WithEvents properties
		[CompilerGenerated]
		private ContextMenuStrip _MenuMacro;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_Cut;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_Copy;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_Paste;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_Revert;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_Clear;

		[CompilerGenerated]
		private ContextMenuStrip _MenuRow;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Cut;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Copy;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Paste;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Clear;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Revert;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_CopyClipboard;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_PasteClipboard;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_CopyClipboard;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_PasteClipboard;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMain_evaluate;

		[CompilerGenerated]
		private ContextMenuStrip _MenuBook;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Cut;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Copy;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Paste;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Clear;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Revert;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_CopyClipboard;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_PasteClipboard;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuHandler_ClearSide;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuHandler_CutSide;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuHandler_CopySide;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuHandler_PasteSide;

		[CompilerGenerated]
		private ToolStripMenuItem _File_Exit;

		[CompilerGenerated]
		private ToolStripMenuItem _File_Open;

		[CompilerGenerated]
		private ToolStripMenuItem _File_SaveRow;

		[CompilerGenerated]
		private ToolStripMenuItem _File_SaveAll;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_SaveBookNames;

		[CompilerGenerated]
		private ContextMenuStrip _MenuText;

		[CompilerGenerated]
		private ToolStripMenuItem _menuText_Cut;

		[CompilerGenerated]
		private ToolStripMenuItem _menuText_Copy;

		[CompilerGenerated]
		private ToolStripMenuItem _menuText_Paste;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Import;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Import;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMain_Search;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_Wizard_BLM;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_SaveFiles;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuHelp_Help;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuHelp_FeatureTour;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_RenameBook;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_MacroMap;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_Save;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuRow_CopyLocation;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuBook_CopyMacroMap;

		[CompilerGenerated]
		private ToolStripMenuItem _MenuMacro_Destination;

		// Token: 0x0400001F RID: 31
		private const int WM_CUT = 768;

		// Token: 0x04000020 RID: 32
		private const int WM_COPY = 769;

		// Token: 0x04000021 RID: 33
		private const int WM_PASTE = 770;

		// Token: 0x04000022 RID: 34
		private const int WM_CLEAR = 771;

		// Token: 0x04000023 RID: 35
		private const int WM_UNDO = 772;

		// Token: 0x04000024 RID: 36
		public Dictionary<int, Button> Ctrls;

		// Token: 0x04000025 RID: 37
		public Dictionary<int, Button> Rows;

		// Token: 0x04000026 RID: 38
		public Dictionary<int, TextBox> Lines;

		// Token: 0x04000027 RID: 39
		public string macropath;

		// Token: 0x04000028 RID: 40
		public string importpath;

		// Token: 0x04000029 RID: 41
		public string[,,][] MacroContainer;

		// Token: 0x0400002A RID: 42
		public string[,,][] MacroPreserved;

		// Token: 0x0400002B RID: 43
		public ListBox Contents;

		// Token: 0x0400002C RID: 44
		public int bWidth;

		// Token: 0x0400002D RID: 45
		public int bHeight;

		// Token: 0x0400002E RID: 46
		public int cbook;

		// Token: 0x0400002F RID: 47
		public int xBook;

		// Token: 0x04000030 RID: 48
		public int xRow;

		// Token: 0x04000031 RID: 49
		public int xMacro;

		// Token: 0x04000032 RID: 50
		public int handlerStart;

		// Token: 0x04000033 RID: 51
		public int handlerEnd;

		// Token: 0x04000034 RID: 52
		public string[] Macroholder;

		// Token: 0x04000035 RID: 53
		public string[][] RowHolder;

		// Token: 0x04000036 RID: 54
		public string[,][] BookHolder;

		// Token: 0x04000037 RID: 55
		public int debuglimit;

		// Token: 0x04000038 RID: 56
		public string copiedbookname;

		// Token: 0x04000039 RID: 57
		public ToolStripMenuItem ATmenu;

		// Token: 0x0400003A RID: 58
		public Dictionary<string, string> ATObject;

		// Token: 0x0400003B RID: 59
		public int CurrentLine;

		// Token: 0x0400003C RID: 60
		public string InternalClipboardMethod;

		// Token: 0x0400003D RID: 61
		public bool SomethingEdited;

		// Token: 0x0400003E RID: 62
		public Resizer rs;

		// Token: 0x0400003F RID: 63
		public Regex ATReadable;

		// Token: 0x04000040 RID: 64
		public Regex ATWritable;

		// Token: 0x04000041 RID: 65
		public Regex lfs;

		// Token: 0x04000042 RID: 66
		public Assessment Evaluation;

		// Token: 0x04000043 RID: 67
		public Assessment SearchResults;

		// Token: 0x04000044 RID: 68
		public MacroMapForm MacroMap;
	}
}