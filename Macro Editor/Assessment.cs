using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MacroEditor
{
	// Token: 0x02000008 RID: 8
	public partial class Assessment : Form
	{
		private Action<int, int, int> navigateCallback;

		public void SetNavigateCallback(Action<int, int, int> callback)
		{
			this.navigateCallback = callback;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002368 File Offset: 0x00000568
		public Assessment()
		{
			base.Load += this.Assessment_Load;
			this.output = new Panel();
			this.output_row = new Dictionary<int, GroupBox>();
			this.iTypes = new string[]
			{
				"Match Found",
				"Title Length",
				"Line Length",
				"Invalid characters",
				"Starts with '//'",
				"Doesn't start with '/'",
				"Old Style /wait",
				"Item Auto-Translate"
			};
			this.itypes_explanation = new string[]
			{
				"",
				"Macro titles can only be 8 characters.",
				"Macro line length, after auto-translate conversion, can only be 60 characters.",
				"Line contains invalid, likely invisible, characters try retyping the line. This is usually the result of a corrupt clipboard paste.",
				"If you're trying to execute a windower command, try /console [command] rather than //[command].",
				"Line doesn't start with '/'",
				"It may be useful to free up a line and append <waitX> to the previous line, rather than /wait X.",
				"Autotranslate phrases for items are not deciphered, in part due to the sheer number of items. FFXI Macro Editor interacts with these macro lines fine."
			};
			this.InitializeComponent();
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000275D File Offset: 0x0000095D
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002768 File Offset: 0x00000968
		internal virtual ListView Results
		{
			[CompilerGenerated]
			get
			{
				return this._Results;
			}
			[CompilerGenerated]
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				EventHandler value2 = new EventHandler(this.Results_DoubleClick);
				MouseEventHandler value3 = new MouseEventHandler(this.Results_MouseDown);
				ColumnClickEventHandler value4 = new ColumnClickEventHandler(this.Results_ColumnClick);
				ListView results = this._Results;
				if (results != null)
				{
					results.DoubleClick -= value2;
					results.MouseDown -= value3;
					results.ColumnClick -= value4;
				}
				this._Results = value;
				results = this._Results;
				if (results != null)
				{
					results.DoubleClick += value2;
					results.MouseDown += value3;
					results.ColumnClick += value4;
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000027E1 File Offset: 0x000009E1
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000027EB File Offset: 0x000009EB
		internal virtual ColumnHeader Results_Location { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000027F4 File Offset: 0x000009F4
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000027FE File Offset: 0x000009FE
		internal virtual ColumnHeader Results_Type { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002807 File Offset: 0x00000A07
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002811 File Offset: 0x00000A11
		internal virtual ColumnHeader Results_Description { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000022 RID: 34 RVA: 0x0000281A File Offset: 0x00000A1A
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002824 File Offset: 0x00000A24
		internal virtual ColumnHeader Results_iType { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000282D File Offset: 0x00000A2D
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002837 File Offset: 0x00000A37
		internal virtual Label Label1 { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x06000026 RID: 38 RVA: 0x00002840 File Offset: 0x00000A40
		private void Assessment_Load(object sender, EventArgs e)
		{
			this.AutoScroll = true;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000284C File Offset: 0x00000A4C
		public object AddResult(object b, object r, object m, object l, object itype, object description, object bgcolor, object fcolor)
		{
			string bookName = "Book " + (Convert.ToInt32(b) + 1);

			bool flag = object.Equals(l, 0);
			if (flag)
			{
				this.Results.Items.Add(new ListViewItem(new string[]
				{
					string.Format("B: {0} ({1}), R: {2}, M: {3}, L: {4}", new object[]
					{
						bookName,
						(Convert.ToInt32(b) + 1),
						(Convert.ToInt32(r) + 1),
						(Convert.ToInt32(m) + 1),
						"Title"
					}),
					this.iTypes[Convert.ToInt32(itype)],
					description.ToString(),
					itype.ToString()
				}));
			}
			else
			{
				this.Results.Items.Add(new ListViewItem(new string[]
				{
					string.Format("B: {0} ({1}), R: {2}, M: {3}, L: {4}", new object[]
					{
						bookName,
						(Convert.ToInt32(b) + 1),
						(Convert.ToInt32(r) + 1),
						(Convert.ToInt32(m) + 1),
						(Convert.ToInt32(l) + 1)
					}),
					this.iTypes[Convert.ToInt32(itype)],
					description.ToString(),
					itype.ToString()
				}));
			}
			checked
			{
				this.Results.Items[this.Results.Items.Count - 1].Tag = new object[]
				{
					b,
					r,
					m,
					l
				};
				this.Results.Items[this.Results.Items.Count - 1].BackColor = ((bgcolor != null) ? ((Color)bgcolor) : default(Color));
				this.Results.Items[this.Results.Items.Count - 1].ForeColor = ((fcolor != null) ? ((Color)fcolor) : default(Color));
				return true;
			}
		}

		/// <summary>
		/// Overload that accepts the book name directly (no MyProject.Forms lookup needed).
		/// </summary>
		public object AddResult(string bookName, int bookIndex, int rowIndex, int macroIndex, int lineIndex, object itype, object description, object bgcolor, object fcolor)
		{
			bool flag = lineIndex == 0;
			if (flag)
			{
				this.Results.Items.Add(new ListViewItem(new string[]
				{
					string.Format("B: {0} ({1}), R: {2}, M: {3}, L: {4}", new object[]
					{
						bookName,
						bookIndex + 1,
						rowIndex + 1,
						macroIndex + 1,
						"Title"
					}),
					this.iTypes[Convert.ToInt32(itype)],
					description.ToString(),
					itype.ToString()
				}));
			}
			else
			{
				this.Results.Items.Add(new ListViewItem(new string[]
				{
					string.Format("B: {0} ({1}), R: {2}, M: {3}, L: {4}", new object[]
					{
						bookName,
						bookIndex + 1,
						rowIndex + 1,
						macroIndex + 1,
						lineIndex + 1
					}),
					this.iTypes[Convert.ToInt32(itype)],
					description.ToString(),
					itype.ToString()
				}));
			}
			checked
			{
				this.Results.Items[this.Results.Items.Count - 1].Tag = new object[]
				{
					bookIndex,
					rowIndex,
					macroIndex,
					lineIndex
				};
				this.Results.Items[this.Results.Items.Count - 1].BackColor = ((bgcolor != null) ? ((Color)bgcolor) : default(Color));
				this.Results.Items[this.Results.Items.Count - 1].ForeColor = ((fcolor != null) ? ((Color)fcolor) : default(Color));
				return true;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A9C File Offset: 0x00000C9C
		private void Results_DoubleClick(object sender, EventArgs e)
		{
			object[] tagArray = (object[])this.Results.Items[this.Results.SelectedIndices[0]].Tag;
			if (this.navigateCallback != null)
				this.navigateCallback(
					Convert.ToInt32(tagArray[0]),
					Convert.ToInt32(tagArray[1]),
					Convert.ToInt32(tagArray[2]));
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002B38 File Offset: 0x00000D38
		private void Results_MouseDown(object sender, MouseEventArgs e)
		{
			bool flag = e.Button == MouseButtons.Right;
			if (flag)
			{
				int index = this.Results.GetItemAt(e.X, e.Y).Index;
				bool flag2 = index >= 0 & Convert.ToDouble(this.Results.Items[index].SubItems[3].Text) > 0.0;
				if (flag2)
				{
					MessageBox.Show(this.Results.Items[index].SubItems[1].Text + "\n\n" + this.itypes_explanation[Convert.ToInt32(this.Results.Items[index].SubItems[3].Text)], "Macro Editor", MessageBoxButtons.OK);
				}
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002C18 File Offset: 0x00000E18
		private void Results_ColumnClick(object sender, ColumnClickEventArgs e)
		{
		}

		// Token: 0x04000011 RID: 17
		[CompilerGenerated]
		private ListView _Results;

		// Token: 0x04000012 RID: 18
		private Panel output;

		// Token: 0x04000013 RID: 19
		private Dictionary<int, GroupBox> output_row;

		// Token: 0x04000014 RID: 20
		private string[] iTypes;

		// Token: 0x04000015 RID: 21
		private string[] itypes_explanation;
	}
}
