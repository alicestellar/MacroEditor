using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace MacroEditor.My
{
	// Token: 0x02000006 RID: 6
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002230 File Offset: 0x00000430
		[DebuggerNonUserCode]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		private static void AutoSaveSettings(object sender, EventArgs e)
		{
			bool saveMySettingsOnExit = MyProject.Application.SaveMySettingsOnExit;
			if (saveMySettingsOnExit)
			{
				MySettingsProperty.Settings.Save();
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000011 RID: 17 RVA: 0x0000225C File Offset: 0x0000045C
		public static MySettings Default
		{
			get
			{
				bool flag = !MySettings.addedHandler;
				if (flag)
				{
					object obj = MySettings.addedHandlerLockObject;
					ObjectFlowControl.CheckForSyncLockOnValueType(obj);
					lock (obj)
					{
						bool flag3 = !MySettings.addedHandler;
						if (flag3)
						{
							MyProject.Application.Shutdown += MySettings.AutoSaveSettings;
							MySettings.addedHandler = true;
						}
					}
				}
				return MySettings.defaultInstance;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000022E8 File Offset: 0x000004E8
		// (set) Token: 0x06000013 RID: 19 RVA: 0x0000230A File Offset: 0x0000050A
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string UserDirectory
		{
			get
			{
				return Conversions.ToString(this["UserDirectory"]);
			}
			set
			{
				this["UserDirectory"] = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000231C File Offset: 0x0000051C
		// (set) Token: 0x06000015 RID: 21 RVA: 0x0000233E File Offset: 0x0000053E
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string WindowerDirectory
		{
			get
			{
				return Conversions.ToString(this["WindowerDirectory"]);
			}
			set
			{
				this["WindowerDirectory"] = value;
			}
		}

		// Token: 0x04000008 RID: 8
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

		// Token: 0x04000009 RID: 9
		private static bool addedHandler;

		// Token: 0x0400000A RID: 10
		private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());
	}
}
