using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MacroEditor.My;

namespace MacroEditor
{
	// Token: 0x02000009 RID: 9
	public partial class Destination : Form
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002C28 File Offset: 0x00000E28
		public Destination()
		{
			base.Load += this.Form1_Load;
			base.Paint += this.Destination_Paint;
			this.mainWin = MyProject.Forms.MainForm;
			this.Rows = new Dictionary<int, Button>();
			this.xBook = 0;
			this.xRow = 0;
			this.tMacro = 0;
			this.InitializeComponent();
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002F42 File Offset: 0x00001142
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002F4C File Offset: 0x0000114C
		internal virtual TextBox ControlTItles { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002F55 File Offset: 0x00001155
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002F5F File Offset: 0x0000115F
		internal virtual ComboBox DestContents { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002F68 File Offset: 0x00001168
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002F72 File Offset: 0x00001172
		internal virtual TextBox AlternateTitles { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x06000034 RID: 52 RVA: 0x00002F7C File Offset: 0x0000117C
		private void Form1_Load(object sender, EventArgs e)
		{
			checked
			{
				int num = this.mainWin.Contents.Items.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					this.DestContents.Items.Add(this.mainWin.Contents.Items[i]);
				}
				int num2 = 0;
				do
				{
					Button button = new Button();
					button = new Button();
					button.Height = 30;
					button.Width = 40;
					button.Left = 135 + 50 * (num2 + 1);
					button.Top = 11;
					button.Tag = num2;
					button.Text = (num2 + 1).ToString();
					button.Name = "Row" + num2.ToString();
					button.BackColor = this.BackColor;
					this.Rows.Add(num2, button);
					base.Controls.Add(button);
					button.Click += this.Row_Click;
					num2++;
				}
				while (num2 <= 9);
				Button button2 = new Button();
				button2.Top = 11;
				button2.Left = 700;
				button2.Height = 30;
				button2.Width = 40;
				button2.Text = "OK";
				base.Controls.Add(button2);
				button2.Click += delegate(object a0, EventArgs a1)
				{
					this.DialogResult = DialogResult.OK;
					this.Close();
				};
				Button button3 = new Button();
				button3.Top = 11;
				button3.Left = 750;
				button3.Height = 30;
				button3.Width = 100;
				button3.Text = "Cancel";
				base.Controls.Add(button3);
				button3.Click += delegate(object a0, EventArgs a1)
				{
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				};
				this.DestContents.SelectedIndexChanged += this.DestContents_SelectedIndexChanged;
				this.DestContents.SelectedIndex = this.xBook;
				base.Left = this.mainWin.Left + 5;
				base.Top = this.mainWin.Top + 50;
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000031AF File Offset: 0x000013AF
		private void DestContents_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.xBook = this.DestContents.SelectedIndex;
			this.Rows[this.xRow].PerformClick();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000031DC File Offset: 0x000013DC
		private void Row_Click(object sender, EventArgs e)
		{
			string[] array = new string[10];
			string[] array2 = new string[10];
			this.xRow = Convert.ToInt32(((Button)sender).Tag);
			int num = 0;
			checked
			{
				do
				{
					array[num] = "[" + this.mainWin.Books[this.xBook].Rows[this.xRow].Macros[num][0].PadRight(8, ' ') + "]";
					num++;
				}
				while (num <= 9);
				int num2 = 10;
				do
				{
					array2[num2 - 10] = "[" + this.mainWin.Books[this.xBook].Rows[this.xRow].Macros[num2 - 10][0].PadRight(8, ' ') + "]";
					num2++;
				}
				while (num2 <= 19);
				this.ControlTItles.Text = string.Join("", array);
				this.AlternateTitles.Text = string.Join("", array2);
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000032D7 File Offset: 0x000014D7
		private void Destination_Paint(object sender, PaintEventArgs e)
		{
			ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Black, ButtonBorderStyle.Solid);
		}

		// Token: 0x0400001A RID: 26
		private MainForm mainWin;

		// Token: 0x0400001B RID: 27
		public Dictionary<int, Button> Rows;

		// Token: 0x0400001C RID: 28
		public int xBook;

		// Token: 0x0400001D RID: 29
		public int xRow;

		// Token: 0x0400001E RID: 30
		public int tMacro;
	}
}
