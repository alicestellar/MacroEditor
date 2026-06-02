using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacroEditor.My
{
	// Token: 0x02000007 RID: 7
	[StandardModule]
	[HideModuleName]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal sealed class MySettingsProperty
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002350 File Offset: 0x00000550
		[HelpKeyword("My.Settings")]
		internal static MySettings Settings
		{
			get
			{
				return MySettings.Default;
			}
		}
	}
}
