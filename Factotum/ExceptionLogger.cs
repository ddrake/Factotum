using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Factotum
{
	class ExceptionLogger
	{
		public static void LogException(Exception ex)
		{
            StringBuilder sb = new StringBuilder();
            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            sb.AppendLine("Factotum Version " + strVersion);
            sb.AppendLine(DateTime.Now.ToString("g"));
            sb.AppendLine(new String('-', 80));
            sb.AppendLine(ex.ToString());
            sb.AppendLine(new String('-', 80));
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            sb.AppendLine(st.ToString());
            string msg = sb.ToString();
			string path = Globals.FactotumDataFolder + "\\error.log";
			StreamWriter sw = new StreamWriter(path,true);
			sw.Write(msg);
			sw.Close();
		}
	}
}
