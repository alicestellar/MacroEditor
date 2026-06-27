using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MacroEditor
{
	/// <summary>
	/// Editable variant of the macro map. Each macro is shown as a multiline TextBox
	/// (line 1 = title, lines 2-7 = the macro's 6 command lines). A Save button writes
	/// all edits back to the data model via a save callback.
	/// </summary>
	public class EditableMacroMapForm : Form
	{
		// Faded color scheme matching the read-only map
		private static readonly Color CtrlColor = Color.FromArgb(210, 225, 255);
		private static readonly Color AltColor = Color.FromArgb(255, 215, 215);
		private static readonly Color CtrlHeaderColor = Color.FromArgb(170, 200, 255);
		private static readonly Color AltHeaderColor = Color.FromArgb(255, 180, 180);

		private const int CellHeight = 100;
		private const int CellWidth = 150;
		private const int Gap = 20;
		private const int HeaderHeight = 70;

		private Panel headerPanel;
		private Panel bodyPanel;
		private int currentPage;
		private int maxPage;
		private int bookIndex = -1;

		// Maps "row,macro" -> the editing TextBox for that macro slot.
		private readonly Dictionary<string, TextBox> cells = new Dictionary<string, TextBox>();

		// Save callback: (book, row, macro, [title, line1..line6])
		private Action<int, int, int, string[]> saveCallback;

		public EditableMacroMapForm()
		{
			this.Text = "Edit Macro Map";
			this.Load += this.Form_Load;
			this.BuildLayout();
		}

		public void SetSaveCallback(Action<int, int, int, string[]> callback)
		{
			this.saveCallback = callback;
		}

		private void BuildLayout()
		{
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

			var saveButton = new Button();
			saveButton.Text = "Save";
			saveButton.Size = new Size(100, 30);
			saveButton.Location = new Point(430, 20);
			saveButton.Click += this.Save_Click;

			this.headerPanel.Controls.Add(ctrlHeaderLabel);
			this.headerPanel.Controls.Add(altHeaderLabel);
			this.headerPanel.Controls.Add(prevPageButton);
			this.headerPanel.Controls.Add(nextPageButton);
			this.headerPanel.Controls.Add(saveButton);

			var headerDivider = new Panel();
			headerDivider.Dock = DockStyle.Bottom;
			headerDivider.Height = 2;
			headerDivider.BackColor = Color.FromArgb(120, 120, 120);
			this.headerPanel.Controls.Add(headerDivider);

			this.bodyPanel = new Panel();
			this.bodyPanel.Dock = DockStyle.Fill;
			this.bodyPanel.AutoScroll = true;

			base.Controls.Add(this.bodyPanel);
			base.Controls.Add(this.headerPanel);

			this.currentPage = 0;
			this.maxPage = 0;
		}

		/// <summary>
		/// Adds an editable cell for a macro. 'a' is [title, line1..line6].
		/// </summary>
		public void Add(int b, int r, int m, string[] a)
		{
			TextBox box = new TextBox();
			box.Multiline = true;
			box.BackColor = (m < 10) ? CtrlColor : AltColor;
			box.BorderStyle = BorderStyle.FixedSingle;
			box.Size = new Size(CellWidth, CellHeight);
			box.ScrollBars = ScrollBars.Vertical;
			box.Font = new Font(this.Font.FontFamily, 8.25f);

			checked
			{
				if (m < 10)
				{
					box.Top = Gap + (CellHeight + Gap) * 2 * r;
					box.Left = (CellWidth + Gap) * m + Gap;
				}
				else
				{
					box.Top = CellHeight + Gap * 2 + (CellHeight + Gap) * 2 * r;
					box.Left = (CellWidth + Gap) * (m - 10) + Gap;
				}
				box.Text = string.Join("\r\n", a);
				this.bodyPanel.Controls.Add(box);
				this.cells[r.ToString() + "," + m.ToString()] = box;
				this.bookIndex = b;
				if (r > this.maxPage)
					this.maxPage = r;
			}
		}

		private void Save_Click(object sender, EventArgs e)
		{
			if (this.saveCallback == null || this.bookIndex < 0)
				return;

			foreach (var kvp in this.cells)
			{
				string[] rm = kvp.Key.Split(',');
				int r = Convert.ToInt32(rm[0]);
				int m = Convert.ToInt32(rm[1]);

				// Split the textbox content into title + up to 6 lines.
				string[] rawLines = kvp.Value.Text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
				string[] parsed = new string[7];
				for (int i = 0; i < 7; i++)
					parsed[i] = (i < rawLines.Length) ? rawLines[i] : "";

				// Title (index 0) truncated to 8 characters.
				if (parsed[0].Length > 8)
					parsed[0] = parsed[0].Substring(0, 8);

				this.saveCallback(this.bookIndex, r, m, parsed);
			}

			MessageBox.Show("Macro map changes saved to the editor.\n\nUse File > Save All to write them to disk.",
				"Edit Macro Map", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void GoToPage(int page)
		{
			if (page < 0) page = 0;
			if (page > this.maxPage) page = this.maxPage;
			this.currentPage = page;
			int targetY = (CellHeight + Gap) * 2 * page;
			this.bodyPanel.AutoScrollPosition = new Point(0, targetY);
		}

		private void PrevPage_Click(object sender, EventArgs e)
		{
			this.GoToPage(this.currentPage - 1);
		}

		private void NextPage_Click(object sender, EventArgs e)
		{
			this.GoToPage(this.currentPage + 1);
		}

		private void Form_Load(object sender, EventArgs e)
		{
			int onePageBottom = CellHeight + Gap * 2 + CellHeight + Gap;
			base.ClientSize = new Size(1740, HeaderHeight + onePageBottom);
		}
	}
}
