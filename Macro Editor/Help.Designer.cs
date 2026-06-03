namespace MacroEditor
{
	// Token: 0x0200000C RID: 12
	public partial class Help : global::System.Windows.Forms.Form
	{
		// Token: 0x06000138 RID: 312 RVA: 0x0000BD84 File Offset: 0x00009F84
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

		// Token: 0x06000139 RID: 313 RVA: 0x0000BDD4 File Offset: 0x00009FD4
		[global::System.Diagnostics.DebuggerStepThrough]
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::MacroEditor.Help));
			this.HelpInfo = new global::System.Windows.Forms.RichTextBox();
			base.SuspendLayout();
			this.HelpInfo.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.HelpInfo.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 16f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.HelpInfo.Location = new global::System.Drawing.Point(13, 13);
			this.HelpInfo.Name = "HelpInfo";
			this.HelpInfo.ReadOnly = true;
			this.HelpInfo.ScrollBars = global::System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.HelpInfo.Size = new global::System.Drawing.Size(733, 490);
			this.HelpInfo.TabIndex = 0;
			this.HelpInfo.Text = componentResourceManager.GetString("HelpInfo.Text");
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(758, 515);
			base.Controls.Add(this.HelpInfo);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Help";
			this.Text = "Help";
			base.ResumeLayout(false);
		}

		// Token: 0x04000091 RID: 145
		private global::System.ComponentModel.IContainer components;
	}
}
