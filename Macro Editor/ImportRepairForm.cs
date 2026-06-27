using System;
using System.Drawing;
using System.Windows.Forms;

namespace MacroEditor
{
    /// <summary>What the user chose in the import repair dialog.</summary>
    public enum RepairChoice { Fix, Skip, SkipAll, Cancel }

    /// <summary>
    /// Modal editor used by the import repair workflow. In Stage 1 (file syntax) it shows the
    /// whole file text with a Re-check button. In Stage 2 (per-macro problem) it shows the
    /// offending snippet with Fix / Skip / Skip All buttons. Cancel aborts the whole import.
    /// </summary>
    public class ImportRepairForm : Form
    {
        private readonly TextBox _editor;
        private readonly Label _problem;

        public RepairChoice Result { get; private set; }
        public string EditedText { get { return _editor.Text; } }

        /// <param name="caption">Window title.</param>
        /// <param name="problemText">Description of the problem shown above the editor.</param>
        /// <param name="editableText">Initial text in the editor (whole file or macro snippet).</param>
        /// <param name="stage2">True for per-macro problems (shows Fix/Skip/Skip All); false for file-syntax stage (shows Re-check only).</param>
        public ImportRepairForm(string caption, string problemText, string editableText, bool stage2)
        {
            this.Text = caption;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = true;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(640, 460);
            this.MinimumSize = new Size(480, 320);
            this.Result = RepairChoice.Cancel;

            _problem = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Padding = new Padding(10, 8, 10, 4),
                Text = problemText,
                ForeColor = Color.FromArgb(160, 0, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Bold)
            };

            _editor = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                AcceptsTab = true,
                AcceptsReturn = true,
                Font = new Font(FontFamily.GenericMonospace, 9.5f),
                Text = editableText ?? ""
            };

            var buttonBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8),
                WrapContents = false
            };

            var cancel = MakeButton("Cancel Import", delegate { Choose(RepairChoice.Cancel); });
            buttonBar.Controls.Add(cancel);

            if (stage2)
            {
                buttonBar.Controls.Add(MakeButton("Fix", delegate { Choose(RepairChoice.Fix); }));
                buttonBar.Controls.Add(MakeButton("Skip This", delegate { Choose(RepairChoice.Skip); }));
                buttonBar.Controls.Add(MakeButton("Skip All Remaining", delegate { Choose(RepairChoice.SkipAll); }));
            }
            else
            {
                buttonBar.Controls.Add(MakeButton("Re-check", delegate { Choose(RepairChoice.Fix); }));
            }

            // TableLayoutPanel gives deterministic Top/Fill/Bottom layout (no docking z-order reliance).
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 68f));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 48f));
            table.Controls.Add(_problem, 0, 0);
            table.Controls.Add(_editor, 0, 1);
            table.Controls.Add(buttonBar, 0, 2);

            this.Controls.Add(table);
            this.CancelButton = cancel;
        }

        private Button MakeButton(string text, EventHandler onClick)
        {
            var b = new Button { Text = text, AutoSize = true, Margin = new Padding(6, 4, 6, 4), Padding = new Padding(6, 2, 6, 2) };
            b.Click += onClick;
            return b;
        }

        private void Choose(RepairChoice choice)
        {
            this.Result = choice;
            this.DialogResult = (choice == RepairChoice.Cancel) ? DialogResult.Cancel : DialogResult.OK;
            this.Close();
        }
    }
}
