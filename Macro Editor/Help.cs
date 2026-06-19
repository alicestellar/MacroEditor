using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MacroEditor
{
	// Token: 0x0200000C RID: 12
	public partial class Help : Form
	{
		// Token: 0x06000137 RID: 311 RVA: 0x0000BD61 File Offset: 0x00009F61
		public Help()
		{
			base.Load += this.Help_Load;
			this.InitializeComponent();
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600013A RID: 314 RVA: 0x0000BF2D File Offset: 0x0000A12D
		// (set) Token: 0x0600013B RID: 315 RVA: 0x0000BF37 File Offset: 0x0000A137
		internal virtual RichTextBox HelpInfo { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x0600013C RID: 316 RVA: 0x0000BF40 File Offset: 0x0000A140
		private void Help_Load(object sender, EventArgs e)
		{
			Match match = new Regex("(##.*)").Match(this.HelpInfo.Text);
			int num = 0;
			checked
			{
				while (match.Success)
				{
					this.HelpInfo.SelectionStart = match.Groups[0].Index - num;
					this.HelpInfo.SelectionLength = match.Groups[0].Length;
					this.HelpInfo.SelectionFont = new Font(this.Font.FontFamily, 25f, this.Font.Style);
					this.HelpInfo.SelectedText = this.HelpInfo.SelectedText.Substring(2);
					num += 2;
					match = match.NextMatch();
				}
				this.HelpInfo.BackColor = Color.White;
				Match match2 = new Regex("@([^@]+)@").Match(this.HelpInfo.Text);
				num = 0;
				while (match2.Success)
				{
					this.HelpInfo.SelectionStart = match2.Groups[0].Index - num;
					this.HelpInfo.SelectionLength = match2.Groups[0].Length;
					this.HelpInfo.SelectionColor = Color.Red;
					this.HelpInfo.SelectedText = match2.Groups[1].Value;
					num += 2;
					match2 = match2.NextMatch();
				}
				this.HelpInfo.BackColor = this.BackColor;
				this.HelpInfo.SelectionStart = 0;
				this.HelpInfo.SelectionLength = 0;
				this.HelpInfo.ScrollToCaret();
			}
		}
	}
}
