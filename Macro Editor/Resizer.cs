using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace MacroEditor
{
	// Token: 0x0200000D RID: 13
	public class Resizer
	{
		// Token: 0x0600013D RID: 317 RVA: 0x0000C0FB File Offset: 0x0000A2FB
		public Resizer()
		{
			this.ctrlDict = new Dictionary<string, Resizer.ControlInfo>();
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000C110 File Offset: 0x0000A310
		public void FindAllControls(Control thisCtrl)
		{
			try
			{
				foreach (object obj in thisCtrl.Controls)
				{
					Control control = (Control)obj;
					try
					{
						bool flag = !Information.IsNothing(control.Parent);
						if (flag)
						{
							int height = control.Parent.Height;
							int width = control.Parent.Width;
							Resizer.ControlInfo controlInfo = default(Resizer.ControlInfo);
							controlInfo.name = control.Name;
							controlInfo.parentName = control.Parent.Name;
							controlInfo.topOffsetPercent = Convert.ToDouble(control.Top) / Convert.ToDouble(height);
							controlInfo.leftOffsetPercent = Convert.ToDouble(control.Left) / Convert.ToDouble(width);
							controlInfo.heightPercent = Convert.ToDouble(control.Height) / Convert.ToDouble(height);
							controlInfo.widthPercent = Convert.ToDouble(control.Width) / Convert.ToDouble(width);
							controlInfo.originalFontSize = control.Font.Size;
							controlInfo.originalHeight = control.Height;
							controlInfo.originalWidth = control.Width;
							this.ctrlDict.Add(controlInfo.name, controlInfo);
						}
					}
					catch (Exception ex)
					{
						Debug.Print(ex.Message);
					}
					bool flag2 = control.Controls.Count > 0;
					if (flag2)
					{
						this.FindAllControls(control);
					}
				}
			}
			finally
			{
				IEnumerator enumerator;
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000C2D4 File Offset: 0x0000A4D4
		public void ResizeAllControls(Control thisCtrl)
		{
			try
			{
				foreach (object obj in thisCtrl.Controls)
				{
					Control control = (Control)obj;
					try
					{
						bool flag = !Information.IsNothing(control.Parent);
						if (flag)
						{
							int height = control.Parent.Height;
							int width = control.Parent.Width;
							Resizer.ControlInfo controlInfo = default(Resizer.ControlInfo);
							try
							{
								bool flag2 = this.ctrlDict.TryGetValue(control.Name, out controlInfo);
								bool flag3 = flag2;
								if (flag3)
								{
									Font font;
									float num;
									float num2;
									checked
									{
										control.Width = (int)Math.Round(Conversion.Int(unchecked((double)width * controlInfo.widthPercent)));
										control.Height = (int)Math.Round(Conversion.Int(unchecked((double)height * controlInfo.heightPercent)));
										control.Top = (int)Math.Round(Conversion.Int(unchecked((double)height * controlInfo.topOffsetPercent)));
										control.Left = (int)Math.Round(Conversion.Int(unchecked((double)width * controlInfo.leftOffsetPercent)));
										font = control.Font;
										num = (float)((double)control.Width / (double)controlInfo.originalWidth);
										num2 = (float)((double)control.Height / (double)controlInfo.originalHeight);
									}
									float num3 = (num + num2) / 2f;
									control.Font = new Font(font.FontFamily, controlInfo.originalFontSize * num3, font.Style);
								}
							}
							catch (Exception ex)
							{
							}
						}
					}
					catch (Exception ex2)
					{
					}
					bool flag4 = control.Controls.Count > 0;
					if (flag4)
					{
						this.ResizeAllControls(control);
					}
				}
			}
			finally
			{
				IEnumerator enumerator;
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
		}

		// Token: 0x04000093 RID: 147
		private Dictionary<string, Resizer.ControlInfo> ctrlDict;

		// Token: 0x02000011 RID: 17
		private struct ControlInfo
		{
			// Token: 0x0400009B RID: 155
			public string name;

			// Token: 0x0400009C RID: 156
			public string parentName;

			// Token: 0x0400009D RID: 157
			public double leftOffsetPercent;

			// Token: 0x0400009E RID: 158
			public double topOffsetPercent;

			// Token: 0x0400009F RID: 159
			public double heightPercent;

			// Token: 0x040000A0 RID: 160
			public int originalHeight;

			// Token: 0x040000A1 RID: 161
			public int originalWidth;

			// Token: 0x040000A2 RID: 162
			public double widthPercent;

			// Token: 0x040000A3 RID: 163
			public float originalFontSize;
		}
	}
}
