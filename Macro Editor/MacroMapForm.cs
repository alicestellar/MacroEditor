using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MacroEditor
{
	// Token: 0x0200000B RID: 11
	public partial class MacroMapForm : Form
	{
		private Action<int, int, int> navigateCallback;

		public void SetNavigateCallback(Action<int, int, int> callback)
		{
			this.navigateCallback = callback;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000BA79 File Offset: 0x00009C79
		public MacroMapForm()
		{
			base.Load += this.MacroMap_Load;
			this.InitializeComponent();
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000BB55 File Offset: 0x00009D55
		// (set) Token: 0x06000133 RID: 307 RVA: 0x0000BB5F File Offset: 0x00009D5F
		internal virtual TextBox mTemplate { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x06000134 RID: 308 RVA: 0x0000BB68 File Offset: 0x00009D68
		public object Add(int b, int r, int m, string[] a)
		{
			Label label = new Label();
			label.BackColor = SystemColors.ControlLight;
			label.BorderStyle = BorderStyle.FixedSingle;
			label.Location = new Point(13, 13);
			label.Size = new Size(150, 100);
			label.TabIndex = 0;
			label.Tag = string.Concat(new string[]
			{
				b.ToString(),
				",",
				r.ToString(),
				",",
				m.ToString()
			});
			bool flag = m < 10;
			checked
			{
				if (flag)
				{
					label.Top = 20 + (label.Height + 20) * 2 * r;
					label.Left = 170 * m + 20;
				}
				else
				{
					label.Top = label.Height + 40 + (label.Height + 20) * 2 * r;
					label.Left = 170 * (m - 10) + 20;
				}
				label.Text = string.Join("\r\n", a);
				label.MouseDown += new MouseEventHandler(this.mbox_Click);
				base.Controls.Add(label);
				return true;
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000BCA0 File Offset: 0x00009EA0
		private void mbox_Click(object sender, EventArgs e)
		{
			string tagStr = (string)((Label)sender).Tag;
			string[] parts = tagStr.Split(',');
			base.ActiveControl = null;
			if (this.navigateCallback != null)
				this.navigateCallback(
					Convert.ToInt32(parts[0]),
					Convert.ToInt32(parts[1]),
					Convert.ToInt32(parts[2]));
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000BD4E File Offset: 0x00009F4E
		private void MacroMap_Load(object sender, EventArgs e)
		{
			this.AutoScroll = true;
			this.Width = 1740;
			base.ActiveControl = null;
		}
	}
}
