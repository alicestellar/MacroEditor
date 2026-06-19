namespace MacroEditor
{
	// Token: 0x0200000A RID: 10
	public partial class MainForm : global::System.Windows.Forms.Form
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00009030 File Offset: 0x00007230
		[global::System.Diagnostics.DebuggerNonUserCode]
		protected override void Dispose(bool disposing)
		{
			try
			{
				bool flag = disposing && this.components != null;
				if (flag)
				{
					this.components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00009080 File Offset: 0x00007280
		[global::System.Diagnostics.DebuggerStepThrough]
		private void InitializeComponent()
		{
			this.components = new global::System.ComponentModel.Container();
			this.MainMenu = new global::System.Windows.Forms.MenuStrip();
			this.FileMenu = new global::System.Windows.Forms.ToolStripMenuItem();
			this.File_Open = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.File_SaveRow = new global::System.Windows.Forms.ToolStripMenuItem();
			this.File_SaveAll = new global::System.Windows.Forms.ToolStripMenuItem();
			this.File_Exit = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMain_evaluate = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMain_Search = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHelp = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHelp_Help = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHelp_FeatureTour = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMacro = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.MenuMacro_Cut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMacro_Copy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMacro_Paste = new global::System.Windows.Forms.ToolStripMenuItem();
			this.h1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuMacro_Clear = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMacro_Revert = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMacro_CopyClipboard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuMacro_PasteClipboard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem4 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuMacro_Destination = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.MenuRow_Cut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_Copy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_Paste = new global::System.Windows.Forms.ToolStripMenuItem();
			this.h2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuRow_Clear = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_Revert = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_CopyClipboard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_PasteClipboard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem3 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuRow_Save = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_Import = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuRow_CopyLocation = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.MenuBook_Header = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Cut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Copy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Paste = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_h1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuBook_Clear = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Revert = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_RenameBook = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_SaveBookNames = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_h2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuBook_CopyClipboard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_PasteClipboard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MenuBook_SaveFiles = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Import = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Wizard = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_Wizard_BLM = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_MacroMap = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuBook_CopyMacroMap = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHandler = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.MenuHandler_ClearSide = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHandler_CutSide = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHandler_CopySide = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MenuHandler_PasteSide = new global::System.Windows.Forms.ToolStripMenuItem();
			this.OpenDialog = new global::System.Windows.Forms.OpenFileDialog();
			this.StatusBar = new global::System.Windows.Forms.StatusStrip();
			this.StatusH = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.StatusD = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.MenuText = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuText_Cut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.menuText_Copy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.menuText_Paste = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem7 = new global::System.Windows.Forms.ToolStripSeparator();
			this.FolderBrowserDialog1 = new global::System.Windows.Forms.FolderBrowserDialog();
			this.ControlTip = new global::System.Windows.Forms.ToolTip(this.components);
			this.Warning = new global::System.Windows.Forms.TextBox();
			this.MainMenu.SuspendLayout();
			this.MenuMacro.SuspendLayout();
			this.MenuRow.SuspendLayout();
			this.MenuBook.SuspendLayout();
			this.MenuHandler.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.MenuText.SuspendLayout();
			base.SuspendLayout();
			this.MainMenu.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.FileMenu,
				this.MenuMain_evaluate,
				this.MenuMain_Search,
				this.MenuHelp
			});
			this.MainMenu.Location = new global::System.Drawing.Point(0, 0);
			this.MainMenu.Name = "MainMenu";
			this.MainMenu.Size = new global::System.Drawing.Size(284, 24);
			this.MainMenu.TabIndex = 0;
			this.MainMenu.Text = "MenuMain";
			this.FileMenu.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.File_Open,
				this.ToolStripMenuItem2,
				this.File_SaveRow,
				this.File_SaveAll,
				this.File_Exit
			});
			this.FileMenu.Name = "FileMenu";
			this.FileMenu.Size = new global::System.Drawing.Size(37, 20);
			this.FileMenu.Text = "File";
			this.File_Open.Name = "File_Open";
			this.File_Open.Size = new global::System.Drawing.Size(161, 22);
			this.File_Open.Text = "Open";
			this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
			this.ToolStripMenuItem2.Size = new global::System.Drawing.Size(158, 6);
			this.File_SaveRow.Name = "File_SaveRow";
			this.File_SaveRow.Size = new global::System.Drawing.Size(161, 22);
			this.File_SaveRow.Text = "Save Macro Row";
			this.File_SaveAll.Name = "File_SaveAll";
			this.File_SaveAll.Size = new global::System.Drawing.Size(161, 22);
			this.File_SaveAll.Text = "Save All";
			this.File_Exit.Name = "File_Exit";
			this.File_Exit.Size = new global::System.Drawing.Size(161, 22);
			this.File_Exit.Text = "Exit";
			this.MenuMain_evaluate.Enabled = false;
			this.MenuMain_evaluate.Name = "MenuMain_evaluate";
			this.MenuMain_evaluate.Size = new global::System.Drawing.Size(63, 20);
			this.MenuMain_evaluate.Text = "Evaluate";
			this.MenuMain_Search.Enabled = false;
			this.MenuMain_Search.Name = "MenuMain_Search";
			this.MenuMain_Search.Size = new global::System.Drawing.Size(42, 20);
			this.MenuMain_Search.Text = "Find";
			this.MenuHelp.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MenuHelp_Help,
				this.MenuHelp_FeatureTour
			});
			this.MenuHelp.Name = "MenuHelp";
			this.MenuHelp.Size = new global::System.Drawing.Size(44, 20);
			this.MenuHelp.Text = "Help";
			this.MenuHelp_Help.Name = "MenuHelp_Help";
			this.MenuHelp_Help.Size = new global::System.Drawing.Size(152, 22);
			this.MenuHelp_Help.Text = "Help";
			this.MenuHelp_FeatureTour.Name = "MenuHelp_FeatureTour";
			this.MenuHelp_FeatureTour.Size = new global::System.Drawing.Size(152, 22);
			this.MenuHelp_FeatureTour.Text = "Feature Tour";
			this.MenuMacro.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MenuMacro_Cut,
				this.MenuMacro_Copy,
				this.MenuMacro_Paste,
				this.h1,
				this.MenuMacro_Clear,
				this.MenuMacro_Revert,
				this.MenuMacro_CopyClipboard,
				this.MenuMacro_PasteClipboard,
				this.ToolStripMenuItem4,
				this.MenuMacro_Destination
			});
			this.MenuMacro.Name = "MenuMacro";
			this.MenuMacro.Size = new global::System.Drawing.Size(220, 192);
			this.MenuMacro_Cut.Name = "MenuMacro_Cut";
			this.MenuMacro_Cut.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_Cut.Text = "Cut";
			this.MenuMacro_Copy.Name = "MenuMacro_Copy";
			this.MenuMacro_Copy.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_Copy.Text = "Copy";
			this.MenuMacro_Paste.Name = "MenuMacro_Paste";
			this.MenuMacro_Paste.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_Paste.Text = "Paste";
			this.h1.Name = "h1";
			this.h1.Size = new global::System.Drawing.Size(216, 6);
			this.MenuMacro_Clear.Name = "MenuMacro_Clear";
			this.MenuMacro_Clear.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_Clear.Text = "Clear Macro";
			this.MenuMacro_Revert.Name = "MenuMacro_Revert";
			this.MenuMacro_Revert.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_Revert.Text = "Revert";
			this.MenuMacro_CopyClipboard.Name = "MenuMacro_CopyClipboard";
			this.MenuMacro_CopyClipboard.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_CopyClipboard.Text = "Copy to Clipboard";
			this.MenuMacro_PasteClipboard.Name = "MenuMacro_PasteClipboard";
			this.MenuMacro_PasteClipboard.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_PasteClipboard.Text = "Paste from Clipboard";
			this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
			this.ToolStripMenuItem4.Size = new global::System.Drawing.Size(216, 6);
			this.MenuMacro_Destination.Name = "MenuMacro_Destination";
			this.MenuMacro_Destination.Size = new global::System.Drawing.Size(219, 22);
			this.MenuMacro_Destination.Text = "Point to another macro line";
			this.MenuRow.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MenuRow_Cut,
				this.MenuRow_Copy,
				this.MenuRow_Paste,
				this.h2,
				this.MenuRow_Clear,
				this.MenuRow_Revert,
				this.MenuRow_CopyClipboard,
				this.MenuRow_PasteClipboard,
				this.ToolStripMenuItem3,
				this.MenuRow_Save,
				this.MenuRow_Import,
				this.MenuRow_CopyLocation
			});
			this.MenuRow.Name = "ContextMenuStrip1";
			this.MenuRow.Size = new global::System.Drawing.Size(187, 236);
			this.MenuRow_Cut.Name = "MenuRow_Cut";
			this.MenuRow_Cut.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Cut.Text = "Cut";
			this.MenuRow_Copy.Name = "MenuRow_Copy";
			this.MenuRow_Copy.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Copy.Text = "Copy";
			this.MenuRow_Paste.Name = "MenuRow_Paste";
			this.MenuRow_Paste.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Paste.Text = "Paste";
			this.h2.Name = "h2";
			this.h2.Size = new global::System.Drawing.Size(183, 6);
			this.MenuRow_Clear.Name = "MenuRow_Clear";
			this.MenuRow_Clear.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Clear.Text = "Clear Row";
			this.MenuRow_Revert.Name = "MenuRow_Revert";
			this.MenuRow_Revert.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Revert.Text = "Revert";
			this.MenuRow_CopyClipboard.Name = "MenuRow_CopyClipboard";
			this.MenuRow_CopyClipboard.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_CopyClipboard.Text = "Copy to Clipboard";
			this.MenuRow_PasteClipboard.Name = "MenuRow_PasteClipboard";
			this.MenuRow_PasteClipboard.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_PasteClipboard.Text = "Paste from Clipboard";
			this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
			this.ToolStripMenuItem3.Size = new global::System.Drawing.Size(183, 6);
			this.MenuRow_Save.Name = "MenuRow_Save";
			this.MenuRow_Save.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Save.Text = "Save";
			this.MenuRow_Import.Name = "MenuRow_Import";
			this.MenuRow_Import.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_Import.Text = "Import...";
			this.MenuRow_CopyLocation.Name = "MenuRow_CopyLocation";
			this.MenuRow_CopyLocation.Size = new global::System.Drawing.Size(186, 22);
			this.MenuRow_CopyLocation.Text = "Copy File Location";
			this.MenuBook.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MenuBook_Header,
				this.MenuBook_Cut,
				this.MenuBook_Copy,
				this.MenuBook_Paste,
				this.MenuBook_h1,
				this.MenuBook_Clear,
				this.MenuBook_Revert,
				this.MenuBook_RenameBook,
				this.MenuBook_SaveBookNames,
				this.MenuBook_h2,
				this.MenuBook_CopyClipboard,
				this.MenuBook_PasteClipboard,
				this.ToolStripMenuItem1,
				this.MenuBook_SaveFiles,
				this.MenuBook_Import,
				this.MenuBook_Wizard,
				this.MenuBook_MacroMap,
				this.MenuBook_CopyMacroMap
			});
			this.MenuBook.Name = "MenuBook";
			this.MenuBook.Size = new global::System.Drawing.Size(248, 472);
			this.MenuBook_Header.Font = new global::System.Drawing.Font("Segoe UI", 14f, global::System.Drawing.FontStyle.Bold);
			this.MenuBook_Header.Name = "MenuBook_Header";
			this.MenuBook_Header.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Header.Text = "Header";
			this.MenuBook_Cut.Name = "MenuBook_Cut";
			this.MenuBook_Cut.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Cut.Text = "Cut";
			this.MenuBook_Copy.Name = "MenuBook_Copy";
			this.MenuBook_Copy.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Copy.Text = "Copy";
			this.MenuBook_Paste.Name = "MenuBook_Paste";
			this.MenuBook_Paste.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Paste.Text = "Paste";
			this.MenuBook_h1.Name = "MenuBook_h1";
			this.MenuBook_h1.Size = new global::System.Drawing.Size(244, 6);
			this.MenuBook_Clear.Name = "MenuBook_Clear";
			this.MenuBook_Clear.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Clear.Text = "Clear";
			this.MenuBook_Revert.Name = "MenuBook_Revert";
			this.MenuBook_Revert.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Revert.Text = "Revert";
			this.MenuBook_RenameBook.Name = "MenuBook_RenameBook";
			this.MenuBook_RenameBook.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_RenameBook.Text = "Rename Book";
			this.MenuBook_SaveBookNames.Name = "MenuBook_SaveBookNames";
			this.MenuBook_SaveBookNames.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_SaveBookNames.Text = "Save Booknames";
			this.MenuBook_h2.Name = "MenuBook_h2";
			this.MenuBook_h2.Size = new global::System.Drawing.Size(244, 6);
			this.MenuBook_CopyClipboard.Name = "MenuBook_CopyClipboard";
			this.MenuBook_CopyClipboard.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_CopyClipboard.Text = "Copy to Clipboard";
			this.MenuBook_PasteClipboard.Name = "MenuBook_PasteClipboard";
			this.MenuBook_PasteClipboard.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_PasteClipboard.Text = "Paste from Clipboard";
			this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
			this.ToolStripMenuItem1.Size = new global::System.Drawing.Size(244, 6);
			this.MenuBook_SaveFiles.Name = "MenuBook_SaveFiles";
			this.MenuBook_SaveFiles.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_SaveFiles.Text = "Save";
			this.MenuBook_Import.Name = "MenuBook_Import";
			this.MenuBook_Import.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Import.Text = "Import...";
			this.MenuBook_Wizard.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MenuBook_Wizard_BLM
			});
			this.MenuBook_Wizard.Name = "MenuBook_Wizard";
			this.MenuBook_Wizard.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_Wizard.Text = "Wizard";
			this.MenuBook_Wizard_BLM.Name = "MenuBook_Wizard_BLM";
			this.MenuBook_Wizard_BLM.Size = new global::System.Drawing.Size(135, 22);
			this.MenuBook_Wizard_BLM.Text = "Black Mage";
			this.MenuBook_MacroMap.Name = "MenuBook_MacroMap";
			this.MenuBook_MacroMap.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_MacroMap.Text = "Macro Map";
			this.MenuBook_CopyMacroMap.Name = "MenuBook_CopyMacroMap";
			this.MenuBook_CopyMacroMap.Size = new global::System.Drawing.Size(247, 30);
			this.MenuBook_CopyMacroMap.Text = "Copy Macro Map (FFXIAH Table)";
			this.MenuHandler.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MenuHandler_ClearSide,
				this.MenuHandler_CutSide,
				this.MenuHandler_CopySide,
				this.MenuHandler_PasteSide
			});
			this.MenuHandler.Name = "MenuHandler";
			this.MenuHandler.Size = new global::System.Drawing.Size(187, 92);
			this.MenuHandler_ClearSide.Name = "MenuHandler_ClearSide";
			this.MenuHandler_ClearSide.Size = new global::System.Drawing.Size(186, 22);
			this.MenuHandler_ClearSide.Text = "Clear";
			this.MenuHandler_CutSide.Name = "MenuHandler_CutSide";
			this.MenuHandler_CutSide.Size = new global::System.Drawing.Size(186, 22);
			this.MenuHandler_CutSide.Text = "Cut to Clipboard";
			this.MenuHandler_CopySide.Name = "MenuHandler_CopySide";
			this.MenuHandler_CopySide.Size = new global::System.Drawing.Size(186, 22);
			this.MenuHandler_CopySide.Text = "Copy to Clipboard";
			this.MenuHandler_PasteSide.Name = "MenuHandler_PasteSide";
			this.MenuHandler_PasteSide.Size = new global::System.Drawing.Size(186, 22);
			this.MenuHandler_PasteSide.Text = "Paste from Clipboard";
			this.OpenDialog.FileName = "mcr.ttl";
			this.OpenDialog.Filter = "Macro Title Files|mcr.ttl";
			this.OpenDialog.InitialDirectory = "C:\\Program Files (x86)\\PlayOnline\\SquareEnix\\FINAL FANTASY XI\\USER";
			this.OpenDialog.Title = "Find Macro Files";
			this.StatusBar.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.StatusH,
				this.StatusD
			});
			this.StatusBar.Location = new global::System.Drawing.Point(0, 239);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.Size = new global::System.Drawing.Size(284, 22);
			this.StatusBar.TabIndex = 4;
			this.StatusH.Name = "StatusH";
			this.StatusH.Size = new global::System.Drawing.Size(0, 17);
			this.StatusD.Name = "StatusD";
			this.StatusD.Size = new global::System.Drawing.Size(0, 17);
			this.MenuText.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.menuText_Cut,
				this.menuText_Copy,
				this.menuText_Paste,
				this.ToolStripMenuItem7
			});
			this.MenuText.Name = "MenuText";
			this.MenuText.Size = new global::System.Drawing.Size(103, 76);
			this.menuText_Cut.Name = "menuText_Cut";
			this.menuText_Cut.Size = new global::System.Drawing.Size(102, 22);
			this.menuText_Cut.Text = "Cut";
			this.menuText_Copy.Name = "menuText_Copy";
			this.menuText_Copy.Size = new global::System.Drawing.Size(102, 22);
			this.menuText_Copy.Text = "Copy";
			this.menuText_Paste.Name = "menuText_Paste";
			this.menuText_Paste.Size = new global::System.Drawing.Size(102, 22);
			this.menuText_Paste.Text = "Paste";
			this.ToolStripMenuItem7.Name = "ToolStripMenuItem7";
			this.ToolStripMenuItem7.Size = new global::System.Drawing.Size(99, 6);
			this.Warning.Font = new global::System.Drawing.Font("Arial", 10f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 0);
			this.Warning.Location = new global::System.Drawing.Point(80, 97);
			this.Warning.Multiline = true;
			this.Warning.Name = "Warning";
			this.Warning.Size = new global::System.Drawing.Size(100, 20);
			this.Warning.TabIndex = 6;
			this.Warning.Text = "Use of this program may constitute a violation of the PlayOnline / FFXI User Agreement";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(284, 261);
			base.Controls.Add(this.Warning);
			base.Controls.Add(this.StatusBar);
			base.Controls.Add(this.MainMenu);
			base.KeyPreview = true;
			base.MainMenuStrip = this.MainMenu;
			base.Name = "MainForm";
			this.Text = "FFXI Macro Editor";
			this.MainMenu.ResumeLayout(false);
			this.MainMenu.PerformLayout();
			this.MenuMacro.ResumeLayout(false);
			this.MenuRow.ResumeLayout(false);
			this.MenuBook.ResumeLayout(false);
			this.MenuHandler.ResumeLayout(false);
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.MenuText.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000045 RID: 69
		private global::System.ComponentModel.IContainer components;
	}
}
