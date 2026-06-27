namespace MacroEditor
{
	// Token: 0x02000008 RID: 8
	public partial class Assessment : global::System.Windows.Forms.Form
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002444 File Offset: 0x00000644
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

		// Token: 0x06000019 RID: 25 RVA: 0x00002494 File Offset: 0x00000694
		[global::System.Diagnostics.DebuggerStepThrough]
		private void InitializeComponent()
		{
			this.Results = new global::System.Windows.Forms.ListView();
			this.Results_Location = new global::System.Windows.Forms.ColumnHeader();
			this.Results_Type = new global::System.Windows.Forms.ColumnHeader();
			this.Results_Description = new global::System.Windows.Forms.ColumnHeader();
			this.Results_iType = new global::System.Windows.Forms.ColumnHeader();
			this.Label1 = new global::System.Windows.Forms.Label();
			base.SuspendLayout();
			this.Results.Columns.AddRange(new global::System.Windows.Forms.ColumnHeader[]
			{
				this.Results_Location,
				this.Results_Type,
				this.Results_Description,
				this.Results_iType
			});
			this.Results.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 12f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.Results.FullRowSelect = true;
			this.Results.Location = new global::System.Drawing.Point(12, 12);
			this.Results.MultiSelect = false;
			this.Results.Name = "Results";
			this.Results.ShowItemToolTips = true;
			this.Results.Size = new global::System.Drawing.Size(760, 266);
			this.Results.TabIndex = 0;
			this.Results.UseCompatibleStateImageBehavior = false;
			this.Results.View = global::System.Windows.Forms.View.Details;
			this.Results_Location.Text = "Location";
			this.Results_Location.Width = 250;
			this.Results_Type.Text = "Type";
			this.Results_Type.Width = 150;
			this.Results_Description.Text = "Description";
			this.Results_Description.Width = 325;
			this.Results_iType.Width = 0;
			this.Label1.AutoSize = true;
			this.Label1.Font = new global::System.Drawing.Font("Microsoft Sans Serif", 12f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.Label1.Location = new global::System.Drawing.Point(13, 281);
			this.Label1.Name = "Label1";
			this.Label1.Size = new global::System.Drawing.Size(429, 20);
			this.Label1.TabIndex = 1;
			this.Label1.Text = "Double-click a row to go to macro; right click for more details";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(784, 306);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.Results);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "Assessment";
			this.Text = "Results";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x0400000B RID: 11
		private global::System.ComponentModel.IContainer components = null;
	}
}
