namespace MacroEditor
{
	// Token: 0x02000009 RID: 9
	public partial class Destination : global::System.Windows.Forms.Form
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00002C9C File Offset: 0x00000E9C
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

		// Token: 0x0600002D RID: 45 RVA: 0x00002CEC File Offset: 0x00000EEC
		[global::System.Diagnostics.DebuggerStepThrough]
		private void InitializeComponent()
		{
			this.ControlTItles = new global::System.Windows.Forms.TextBox();
			this.DestContents = new global::System.Windows.Forms.ComboBox();
			this.AlternateTitles = new global::System.Windows.Forms.TextBox();
			base.SuspendLayout();
			this.ControlTItles.BackColor = global::System.Drawing.SystemColors.ButtonFace;
			this.ControlTItles.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.ControlTItles.Font = new global::System.Drawing.Font("Courier New", 11f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.ControlTItles.Location = new global::System.Drawing.Point(13, 71);
			this.ControlTItles.Name = "ControlTItles";
			this.ControlTItles.Size = new global::System.Drawing.Size(949, 17);
			this.ControlTItles.TabIndex = 1;
			this.DestContents.Font = new global::System.Drawing.Font("Courier New", 14.25f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.DestContents.FormattingEnabled = true;
			this.DestContents.Location = new global::System.Drawing.Point(13, 13);
			this.DestContents.Name = "DestContents";
			this.DestContents.Size = new global::System.Drawing.Size(160, 29);
			this.DestContents.TabIndex = 0;
			this.AlternateTitles.BackColor = global::System.Drawing.SystemColors.ButtonFace;
			this.AlternateTitles.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.AlternateTitles.Font = new global::System.Drawing.Font("Courier New", 11f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.AlternateTitles.Location = new global::System.Drawing.Point(13, 117);
			this.AlternateTitles.Name = "AlternateTitles";
			this.AlternateTitles.Size = new global::System.Drawing.Size(949, 17);
			this.AlternateTitles.TabIndex = 2;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(974, 180);
			base.Controls.Add(this.AlternateTitles);
			base.Controls.Add(this.ControlTItles);
			base.Controls.Add(this.DestContents);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Destination";
			this.Text = "Form1";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000016 RID: 22
		private global::System.ComponentModel.IContainer components;
	}
}
