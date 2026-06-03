namespace MacroEditor
{
	// Token: 0x0200000B RID: 11
	public partial class MacroMapForm : global::System.Windows.Forms.Form
	{
		// Token: 0x06000130 RID: 304 RVA: 0x0000BA9C File Offset: 0x00009C9C
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

		// Token: 0x06000131 RID: 305 RVA: 0x0000BAEC File Offset: 0x00009CEC
		[global::System.Diagnostics.DebuggerStepThrough]
		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(284, 261);
			base.Name = "MacroMapForm";
			this.Text = "MacroMap";
			base.ResumeLayout(false);
		}

		// Token: 0x0400008F RID: 143
		private global::System.ComponentModel.IContainer components;
	}
}
