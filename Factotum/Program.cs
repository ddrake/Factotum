using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Factotum
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.Run(new AppMain());
		}

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Exception ex = e.Exception;
			ExceptionLogger.LogException(ex);
			MessageBox.Show("An unexpected error occured.\nThe details have been recorded in 'error.log'.", "Factotum");
		}

	}
}