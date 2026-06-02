using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;

namespace MacroEditor.My
{
	// Token: 0x02000004 RID: 4
	[StandardModule]
	[HideModuleName]
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	internal sealed class MyProject
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x0000210C File Offset: 0x0000030C
		[HelpKeyword("My.Computer")]
		internal static MyComputer Computer
		{
			[DebuggerHidden]
			get
			{
				return MyProject.m_ComputerObjectProvider.GetInstance;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002128 File Offset: 0x00000328
		[HelpKeyword("My.Application")]
		internal static MyApplication Application
		{
			[DebuggerHidden]
			get
			{
				return MyProject.m_AppObjectProvider.GetInstance;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002144 File Offset: 0x00000344
		[HelpKeyword("My.User")]
		internal static User User
		{
			[DebuggerHidden]
			get
			{
				return MyProject.m_UserObjectProvider.GetInstance;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002160 File Offset: 0x00000360
		[HelpKeyword("My.Forms")]
		internal static MyProject.MyForms Forms
		{
			[DebuggerHidden]
			get
			{
				return MyProject.m_MyFormsObjectProvider.GetInstance;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000217C File Offset: 0x0000037C
		[HelpKeyword("My.WebServices")]
		internal static MyProject.MyWebServices WebServices
		{
			[DebuggerHidden]
			get
			{
				return MyProject.m_MyWebServicesObjectProvider.GetInstance;
			}
		}

		// Token: 0x04000001 RID: 1
		private static readonly MyProject.ThreadSafeObjectProvider<MyComputer> m_ComputerObjectProvider = new MyProject.ThreadSafeObjectProvider<MyComputer>();

		// Token: 0x04000002 RID: 2
		private static readonly MyProject.ThreadSafeObjectProvider<MyApplication> m_AppObjectProvider = new MyProject.ThreadSafeObjectProvider<MyApplication>();

		// Token: 0x04000003 RID: 3
		private static readonly MyProject.ThreadSafeObjectProvider<User> m_UserObjectProvider = new MyProject.ThreadSafeObjectProvider<User>();

		// Token: 0x04000004 RID: 4
		private static MyProject.ThreadSafeObjectProvider<MyProject.MyForms> m_MyFormsObjectProvider = new MyProject.ThreadSafeObjectProvider<MyProject.MyForms>();

		// Token: 0x04000005 RID: 5
		private static readonly MyProject.ThreadSafeObjectProvider<MyProject.MyWebServices> m_MyWebServicesObjectProvider = new MyProject.ThreadSafeObjectProvider<MyProject.MyWebServices>();

		// Token: 0x0200000E RID: 14
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		internal sealed class MyForms
		{
			// Token: 0x06000140 RID: 320 RVA: 0x0000C4F4 File Offset: 0x0000A6F4
			[DebuggerHidden]
			private static T Create__Instance__<T>(T Instance) where T : Form, new()
			{
				bool flag = Instance == null || Instance.IsDisposed;
				if (flag)
				{
					bool flag2 = MyProject.MyForms.m_FormBeingCreated != null;
					if (flag2)
					{
						bool flag3 = MyProject.MyForms.m_FormBeingCreated.ContainsKey(typeof(T));
						if (flag3)
						{
							throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
						}
					}
					else
					{
						MyProject.MyForms.m_FormBeingCreated = new Hashtable();
					}
					MyProject.MyForms.m_FormBeingCreated.Add(typeof(T), null);
					try
					{
						return Activator.CreateInstance<T>();
					}
					catch (TargetInvocationException ex) when (ex.InnerException != null)
					{
						string resourceString = Utils.GetResourceString("WinForms_SeeInnerException", new string[]
						{
							ex.InnerException.Message
						});
						throw new InvalidOperationException(resourceString, ex.InnerException);
					}
					finally
					{
						MyProject.MyForms.m_FormBeingCreated.Remove(typeof(T));
					}
				}
				return Instance;
			}

			// Token: 0x06000141 RID: 321 RVA: 0x0000C61C File Offset: 0x0000A81C
			[DebuggerHidden]
			private void Dispose__Instance__<T>(ref T instance) where T : Form
			{
				instance.Dispose();
				instance = default(T);
			}

			// Token: 0x06000142 RID: 322 RVA: 0x0000C633 File Offset: 0x0000A833
			[DebuggerHidden]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MyForms()
			{
			}

			// Token: 0x06000143 RID: 323 RVA: 0x0000C640 File Offset: 0x0000A840
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object o)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(o));
			}

			// Token: 0x06000144 RID: 324 RVA: 0x0000C660 File Offset: 0x0000A860
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000145 RID: 325 RVA: 0x0000C678 File Offset: 0x0000A878
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal new Type GetType()
			{
				return typeof(MyProject.MyForms);
			}

			// Token: 0x06000146 RID: 326 RVA: 0x0000C694 File Offset: 0x0000A894
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x17000060 RID: 96
			// (get) Token: 0x06000147 RID: 327 RVA: 0x0000C6AC File Offset: 0x0000A8AC
			// (set) Token: 0x0600014C RID: 332 RVA: 0x0000C733 File Offset: 0x0000A933
			public Assessment Assessment
			{
				[DebuggerHidden]
				get
				{
					this.m_Assessment = MyProject.MyForms.Create__Instance__<Assessment>(this.m_Assessment);
					return this.m_Assessment;
				}
				[DebuggerHidden]
				set
				{
					if (value != this.m_Assessment)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.Dispose__Instance__<Assessment>(ref this.m_Assessment);
					}
				}
			}

			// Token: 0x17000061 RID: 97
			// (get) Token: 0x06000148 RID: 328 RVA: 0x0000C6C7 File Offset: 0x0000A8C7
			// (set) Token: 0x0600014D RID: 333 RVA: 0x0000C75F File Offset: 0x0000A95F
			public Destination Destination
			{
				[DebuggerHidden]
				get
				{
					this.m_Destination = MyProject.MyForms.Create__Instance__<Destination>(this.m_Destination);
					return this.m_Destination;
				}
				[DebuggerHidden]
				set
				{
					if (value != this.m_Destination)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.Dispose__Instance__<Destination>(ref this.m_Destination);
					}
				}
			}

			// Token: 0x17000062 RID: 98
			// (get) Token: 0x06000149 RID: 329 RVA: 0x0000C6E2 File Offset: 0x0000A8E2
			// (set) Token: 0x0600014E RID: 334 RVA: 0x0000C78B File Offset: 0x0000A98B
			public Help Help
			{
				[DebuggerHidden]
				get
				{
					this.m_Help = MyProject.MyForms.Create__Instance__<Help>(this.m_Help);
					return this.m_Help;
				}
				[DebuggerHidden]
				set
				{
					if (value != this.m_Help)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.Dispose__Instance__<Help>(ref this.m_Help);
					}
				}
			}

			// Token: 0x17000063 RID: 99
			// (get) Token: 0x0600014A RID: 330 RVA: 0x0000C6FD File Offset: 0x0000A8FD
			// (set) Token: 0x0600014F RID: 335 RVA: 0x0000C7B7 File Offset: 0x0000A9B7
			public MacroMapForm MacroMapForm
			{
				[DebuggerHidden]
				get
				{
					this.m_MacroMapForm = MyProject.MyForms.Create__Instance__<MacroMapForm>(this.m_MacroMapForm);
					return this.m_MacroMapForm;
				}
				[DebuggerHidden]
				set
				{
					if (value != this.m_MacroMapForm)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.Dispose__Instance__<MacroMapForm>(ref this.m_MacroMapForm);
					}
				}
			}

			// Token: 0x17000064 RID: 100
			// (get) Token: 0x0600014B RID: 331 RVA: 0x0000C718 File Offset: 0x0000A918
			// (set) Token: 0x06000150 RID: 336 RVA: 0x0000C7E3 File Offset: 0x0000A9E3
			public MainForm MainForm
			{
				[DebuggerHidden]
				get
				{
					this.m_MainForm = MyProject.MyForms.Create__Instance__<MainForm>(this.m_MainForm);
					return this.m_MainForm;
				}
				[DebuggerHidden]
				set
				{
					if (value != this.m_MainForm)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.Dispose__Instance__<MainForm>(ref this.m_MainForm);
					}
				}
			}

			// Token: 0x04000094 RID: 148
			[ThreadStatic]
			private static Hashtable m_FormBeingCreated;

			// Token: 0x04000095 RID: 149
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Assessment m_Assessment;

			// Token: 0x04000096 RID: 150
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Destination m_Destination;

			// Token: 0x04000097 RID: 151
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Help m_Help;

			// Token: 0x04000098 RID: 152
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MacroMapForm m_MacroMapForm;

			// Token: 0x04000099 RID: 153
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MainForm m_MainForm;
		}

		// Token: 0x0200000F RID: 15
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class MyWebServices
		{
			// Token: 0x06000151 RID: 337 RVA: 0x0000C810 File Offset: 0x0000AA10
			[EditorBrowsable(EditorBrowsableState.Never)]
			[DebuggerHidden]
			public override bool Equals(object o)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(o));
			}

			// Token: 0x06000152 RID: 338 RVA: 0x0000C830 File Offset: 0x0000AA30
			[EditorBrowsable(EditorBrowsableState.Never)]
			[DebuggerHidden]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000153 RID: 339 RVA: 0x0000C848 File Offset: 0x0000AA48
			[EditorBrowsable(EditorBrowsableState.Never)]
			[DebuggerHidden]
			internal new Type GetType()
			{
				return typeof(MyProject.MyWebServices);
			}

			// Token: 0x06000154 RID: 340 RVA: 0x0000C864 File Offset: 0x0000AA64
			[EditorBrowsable(EditorBrowsableState.Never)]
			[DebuggerHidden]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000155 RID: 341 RVA: 0x0000C87C File Offset: 0x0000AA7C
			[DebuggerHidden]
			private static T Create__Instance__<T>(T instance) where T : new()
			{
				bool flag = instance == null;
				T result;
				if (flag)
				{
					result = Activator.CreateInstance<T>();
				}
				else
				{
					result = instance;
				}
				return result;
			}

			// Token: 0x06000156 RID: 342 RVA: 0x0000C8A5 File Offset: 0x0000AAA5
			[DebuggerHidden]
			private void Dispose__Instance__<T>(ref T instance)
			{
				instance = default(T);
			}

			// Token: 0x06000157 RID: 343 RVA: 0x0000C633 File Offset: 0x0000A833
			[DebuggerHidden]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MyWebServices()
			{
			}
		}

		// Token: 0x02000010 RID: 16
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ComVisible(false)]
		internal sealed class ThreadSafeObjectProvider<T> where T : new()
		{
			// Token: 0x17000065 RID: 101
			// (get) Token: 0x06000158 RID: 344 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
			internal T GetInstance
			{
				[DebuggerHidden]
				get
				{
					bool flag = MyProject.ThreadSafeObjectProvider<T>.m_ThreadStaticValue == null;
					if (flag)
					{
						MyProject.ThreadSafeObjectProvider<T>.m_ThreadStaticValue = Activator.CreateInstance<T>();
					}
					return MyProject.ThreadSafeObjectProvider<T>.m_ThreadStaticValue;
				}
			}

			// Token: 0x06000159 RID: 345 RVA: 0x0000C633 File Offset: 0x0000A833
			[DebuggerHidden]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ThreadSafeObjectProvider()
			{
			}

			// Token: 0x0400009A RID: 154
			[CompilerGenerated]
			[ThreadStatic]
			private static T m_ThreadStaticValue;
		}
	}
}
