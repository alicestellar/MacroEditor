using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MacroEditor
{
	// Token: 0x0200000B RID: 11
	public partial class MacroMapForm : Form
	{
		private Action<int, int, int> navigateCallback;
		private Action<int> editCallback;
		private int bookIndex = -1;

		// Faded color scheme for Ctrl (blue) and Alt (red) macros
		private static readonly Color CtrlColor = Color.FromArgb(210, 225, 255);
		private static readonly Color AltColor = Color.FromArgb(255, 215, 215);
		private static readonly Color CtrlHeaderColor = Color.FromArgb(170, 200, 255);
		private static readonly Color AltHeaderColor = Color.FromArgb(255, 180, 180);

		// Layout constants
		private const int LabelHeight = 100;
		private const int LabelWidth = 150;
		private const int Gap = 20;
		private const int HeaderHeight = 70;
		private int PageHeight = (LabelHeight + Gap) * 2; // Ctrl row + Alt row

		private Panel headerPanel;
		private Panel bodyPanel;
		private int currentPage;
		private int maxPage; // highest page index that has content

		public void SetNavigateCallback(Action<int, int, int> callback)
		{
			this.navigateCallback = callback;
		}

		public void SetEditCallback(Action<int> callback)
		{
			this.editCallback = callback;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000BA79 File Offset: 0x00009C79
		public MacroMapForm()
		{
			base.Load += this.MacroMap_Load;
			this.InitializeComponent();
			this.BuildLayout();
		}

		private void BuildLayout()
		{
			// Header panel (fixed, pinned at top)
			this.headerPanel = new Panel();
			this.headerPanel.Dock = DockStyle.Top;
			this.headerPanel.Height = HeaderHeight;
			this.headerPanel.BackColor = SystemColors.Control;

			var ctrlHeaderLabel = new Label();
			ctrlHeaderLabel.Text = "CTRL";
			ctrlHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
			ctrlHeaderLabel.BackColor = CtrlHeaderColor;
			ctrlHeaderLabel.BorderStyle = BorderStyle.FixedSingle;
			ctrlHeaderLabel.Font = new Font(this.Font.FontFamily, 11f, FontStyle.Bold);
			ctrlHeaderLabel.Location = new Point(20, 20);
			ctrlHeaderLabel.Size = new Size(70, 30);

			var altHeaderLabel = new Label();
			altHeaderLabel.Text = "ALT";
			altHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
			altHeaderLabel.BackColor = AltHeaderColor;
			altHeaderLabel.BorderStyle = BorderStyle.FixedSingle;
			altHeaderLabel.Font = new Font(this.Font.FontFamily, 11f, FontStyle.Bold);
			altHeaderLabel.Location = new Point(100, 20);
			altHeaderLabel.Size = new Size(70, 30);

			var prevPageButton = new Button();
			prevPageButton.Text = "▲ Prev Page";
			prevPageButton.Size = new Size(100, 30);
			prevPageButton.Location = new Point(200, 20);
			prevPageButton.Click += this.PrevPage_Click;

			var nextPageButton = new Button();
			nextPageButton.Text = "Next Page ▼";
			nextPageButton.Size = new Size(100, 30);
			nextPageButton.Location = new Point(310, 20);
			nextPageButton.Click += this.NextPage_Click;

			var editButton = new Button();
			editButton.Text = "Edit";
			editButton.Size = new Size(100, 30);
			editButton.Location = new Point(430, 20);
			editButton.Click += this.Edit_Click;

			this.headerPanel.Controls.Add(ctrlHeaderLabel);
			this.headerPanel.Controls.Add(altHeaderLabel);
			this.headerPanel.Controls.Add(prevPageButton);
			this.headerPanel.Controls.Add(nextPageButton);
			this.headerPanel.Controls.Add(editButton);

			// Divider at the bottom of the header to separate it from the scrolling content
			var headerDivider = new Panel();
			headerDivider.Dock = DockStyle.Bottom;
			headerDivider.Height = 2;
			headerDivider.BackColor = Color.FromArgb(120, 120, 120);
			this.headerPanel.Controls.Add(headerDivider);

			// Body panel (scrolling content)
			this.bodyPanel = new Panel();
			this.bodyPanel.Dock = DockStyle.Fill;
			this.bodyPanel.AutoScroll = true;

			// Add body first then header so header stays on top of dock order
			base.Controls.Add(this.bodyPanel);
			base.Controls.Add(this.headerPanel);

			this.currentPage = 0;
			this.maxPage = 0;
		}

		internal virtual TextBox mTemplate { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

		// Token: 0x06000134 RID: 308 RVA: 0x0000BB68 File Offset: 0x00009D68
		public object Add(int b, int r, int m, string[] a)
		{
			Label label = new Label();
			label.BackColor = (m < 10) ? CtrlColor : AltColor;
			label.BorderStyle = BorderStyle.FixedSingle;
			label.Size = new Size(LabelWidth, LabelHeight);
			label.TabIndex = 0;
			label.Tag = string.Concat(new string[]
			{
				b.ToString(),
				",",
				r.ToString(),
				",",
				m.ToString()
			});
			checked
			{
				bool flag = m < 10;
				if (flag)
				{
					label.Top = Gap + (LabelHeight + Gap) * 2 * r;
					label.Left = (LabelWidth + Gap) * m + Gap;
				}
				else
				{
					label.Top = LabelHeight + Gap * 2 + (LabelHeight + Gap) * 2 * r;
					label.Left = (LabelWidth + Gap) * (m - 10) + Gap;
				}
				label.Text = string.Join("\r\n", a);
				label.MouseDown += new MouseEventHandler(this.mbox_Click);
				this.bodyPanel.Controls.Add(label);
				this.bookIndex = b;
				if (r > this.maxPage)
					this.maxPage = r;
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

		private void GoToPage(int page)
		{
			if (page < 0) page = 0;
			if (page > this.maxPage) page = this.maxPage;
			this.currentPage = page;
			// Scroll the body so the target page's top aligns with the viewport top.
			int targetY = (LabelHeight + Gap) * 2 * page;
			this.bodyPanel.AutoScrollPosition = new Point(0, targetY);
			base.ActiveControl = null;
		}

		private void PrevPage_Click(object sender, EventArgs e)
		{
			this.GoToPage(this.currentPage - 1);
		}

		private void NextPage_Click(object sender, EventArgs e)
		{
			this.GoToPage(this.currentPage + 1);
		}

		private void Edit_Click(object sender, EventArgs e)
		{
			if (this.editCallback != null && this.bookIndex >= 0)
			{
				this.editCallback(this.bookIndex);
				this.Hide();
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000BD4E File Offset: 0x00009F4E
		private void MacroMap_Load(object sender, EventArgs e)
		{
			// Size so the body shows exactly one page (down to just below the first Alt row).
			// First page: Ctrl row top=Gap(20), Alt row bottom = LabelHeight + Gap*2 + LabelHeight = 240.
			int onePageBottom = LabelHeight + Gap * 2 + LabelHeight + Gap; // ~260, small margin below Alt row
			base.ClientSize = new Size(1740, HeaderHeight + onePageBottom);
			base.ActiveControl = null;
		}
	}
}
