using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;

namespace CleanupAppData
{
	[RunInstaller(true)]
	public partial class Installer1 : Installer
	{
		public Installer1()
		{
			InitializeComponent();
			string factotumFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AgedMan\\Factotum";
			if (Directory.Exists(factotumFolder))
				Directory.Delete(factotumFolder, true);
		}
	}
}