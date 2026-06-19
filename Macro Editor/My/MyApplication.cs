using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace MacroEditor.My
{
	// Token: 0x02000002 RID: 2
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class MyApplication : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		[STAThread]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static void Main(string[] Args)
		{
			string logPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "crash.log");
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			{
				System.IO.File.AppendAllText(logPath, "UNHANDLED: " + e.ExceptionObject.ToString() + "\n");
			};
			try
			{
				System.IO.File.WriteAllText(logPath, "Starting...\n");
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				System.IO.File.AppendAllText(logPath, "Creating MainForm...\n");
				var form = new MainForm();
				System.IO.File.AppendAllText(logPath, "Running...\n");
				Application.Run(form);
				System.IO.File.AppendAllText(logPath, "Exited normally.\n");
			}
			catch (Exception ex)
			{
				System.IO.File.AppendAllText(logPath, "EXCEPTION: " + ex.ToString() + "\n");
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000208C File Offset: 0x0000028C
		[DebuggerStepThrough]
		public MyApplication() : base(AuthenticationMode.Windows)
		{
			base.IsSingleInstance = false;
			base.EnableVisualStyles = true;
			base.SaveMySettingsOnExit = true;
			base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020B7 File Offset: 0x000002B7
		[DebuggerStepThrough]
		protected override void OnCreateMainForm()
		{
			base.MainForm = MyProject.Forms.MainForm;
		}
	}
}
