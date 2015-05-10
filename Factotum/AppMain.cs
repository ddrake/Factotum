using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.IO;
using DowUtils;

namespace Factotum
{
	public partial class AppMain : Form
	{
		// Allow 3 seconds to connect to webservice
		const int BlacklistCheckTimeout = 3000;

		public AppMain()
		{
			InitializeComponent();
		}

		private void customerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CustomerConfigView ccv = new CustomerConfigView();
			ccv.MdiParent = this;
			ccv.Show();
		}

		private void AppMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			Globals.cnn.Close();
			Globals.DatabaseChanged -= new EventHandler(Globals_DatabaseChanged);
			Globals.CurrentOutageChanged -= new EventHandler(Globals_CurrentOutageChanged);
			backupCurrentDB();
		}

		private void AppMain_Load(object sender, EventArgs e)
		{
            UserSettings.Load();
            AppDomain.CurrentDomain.SetData("DataDirectory", Globals.AppDataFolder);
            Globals.SetDefaultFactotumDataFolder();
            Globals.SetDefaultImageFolder();
            Globals.SetDefaultMeterDataFolder();
            Globals.InitConnection();
			// Get the Database Version and type (IsMasterDB). 
			// We need to know what kind of database Factotum.sdf is in case the user double-clicks
			// and we need to back up.
			Globals.ReadDatabaseInfo();

			// It's startup of a fresh install
			if (Globals.IsNewDB)
			{
				// No need to back up first.
				Globals.ConvertCurrentDbToMaster();
			}

			if (!Globals.IsNewDB)
			{
				// This needs to be set to do the caption right
				Globals.SetCurrentOutageID();
			}

			// If the user has specified a database by clicking on a database file,
			// Let them backup and open it.
			bool isOpenedWithFile = false;
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length == 2)
			{
				DialogResult resp = MessageBox.Show(
					"Would you like to open this data file: \n" +
					args[1], "Factotum: Open Data File?", MessageBoxButtons.YesNo);
				if (resp == DialogResult.Yes)
				{
					// Open the selected file.  Prompt the user to backup first, unless
					// its a new (empty) database.
					isOpenedWithFile = handleFileOpenRequest(args[1], !Globals.IsNewDB);
				}
			}
			// If the current database was changed by the previous block, 
			// The OpenDatabaseFile method will take care of getting and acting on the
			// new database info.


			// Wire up these handlers for keeping the caption and menus refreshed if the user
			// opens a different database later.
			Globals.DatabaseChanged += new EventHandler(Globals_DatabaseChanged);
			Globals.CurrentOutageChanged += new EventHandler(Globals_CurrentOutageChanged);
			// Handle menus and form caption
			handleMenuEnabling();
			UpdateFormCaption();
		}

		private void UpdateFormCaption()
		{
			this.Text = "Factotum " + Globals.VersionString;
			string database = Globals.IsMasterDB ? " - Master Data File - " : " - Outage Data File - ";
			
			string outage;
			if (Globals.CurrentOutageID != null)
				outage = "Current Outage: " + new EOutage(Globals.CurrentOutageID).OutageName;
			else
				outage = "No Current Outage";
			this.Text += database + " (" + outage + ")";
		}

		void Globals_CurrentOutageChanged(object sender, EventArgs e)
		{
			handleMenuEnabling();
			UpdateFormCaption();
		}

		void Globals_DatabaseChanged(object sender, EventArgs e)
		{
			handleMenuEnabling();
			UpdateFormCaption();
		}

		public void handleMenuEnabling()
		{
			bool masterDB = Globals.IsMasterDB;
			bool newDB = Globals.IsNewDB;
			//bool inactiveDB = !Globals.ActivationOK;
			bool outageOK = Globals.CurrentOutageID != null;
			this.componentReportToolStripMenuItem.Visible = !masterDB;
			this.importComponentReportDefinitionToolStripMenuItem.Visible = !masterDB;
			this.importOutageConfigurationDataToolStripMenuItem.Visible = masterDB;
			this.customerToolStripMenuItem.Visible = masterDB;
			this.toolKitsToolStripMenuItem.Visible = !masterDB;
			this.viewChangesFromOutageToolStripMenuItem.Visible = masterDB;

			this.convertToMasterDBToolStripMenuItem.Enabled = !masterDB && !newDB;
			this.importOutageConfigurationDataToolStripMenuItem.Enabled = !newDB;
			this.equipmentToolStripMenuItem.Enabled = !newDB;
			this.proceduresToolStripMenuItem.Enabled = !newDB;
			this.componentMaterialsToolStripMenuItem.Enabled = !newDB;
			this.componentsToolStripMenuItem.Enabled = !newDB;
			this.componentTypesToolStripMenuItem.Enabled = !newDB;
			this.importToolStripMenuItem.Enabled = !newDB;
			this.outageToolStripMenuItem.Enabled = !newDB;
			this.componentReportToolStripMenuItem.Enabled = !newDB;
			this.utilitiesToolStripMenuItem.Enabled = !newDB;
			this.siteConfigurationToolStripMenuItem.Enabled = !newDB;
			this.preferencesToolStripMenuItem.Enabled = !newDB;
			this.exportToolStripMenuItem.Enabled = !newDB;
			this.inspectorsToolStripMenuItem.Enabled = !newDB;
			this.ImportComponentsToolStripMenuItem.Enabled = outageOK;
			this.currentOutageToolStripMenuItem.Enabled = outageOK;

		}

		private void componentMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MaterialTypeView frm = new MaterialTypeView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void componentTypesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ComponentTypeView frm = new ComponentTypeView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void pipeScheduleLookupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PipeScheduleView frm = new PipeScheduleView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void componentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ComponentView frm = new ComponentView();
			frm.MdiParent = this;
			frm.Show();
		}

		// This menu option is disabled for a Master database if we don't have a CurrentOutageID
		private void componentReportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InspectedComponentView frm = 
				new InspectedComponentView((Guid)Globals.CurrentOutageID);
			frm.MdiParent = this;
			frm.Show();
		}

		private void meterModelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MeterModelView frm =	new MeterModelView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void transducerModelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DucerModelView frm =	new DucerModelView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void transducersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DucerView frm = new DucerView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void metersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MeterView frm = new MeterView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void thermomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ThermoView frm = new ThermoView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void calibrationBlocksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CalBlockView frm = new CalBlockView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void gridProceduresToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GridProcedureView frm = new GridProcedureView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void calibrationProceduresToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CalibrationProcedureView frm = new CalibrationProcedureView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void couplantTypesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CouplantTypeView frm = new CouplantTypeView();
			frm.MdiParent = this;
			frm.Show();
		}

		// This menu option is disabled for a master database if we don't have a CurrentOutageID.
		private void currentOutageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Guid? OutageID = Globals.CurrentOutageID;
			if (OutageID != null)
			{
				OutageEdit frm = new OutageEdit(OutageID);
				frm.MdiParent = this;
				frm.Show();
			}
		}

		private void inspectorsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			InspectorView frm = new InspectorView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void toolKitsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			KitView frm = new KitView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void gridSizesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GridSizeView frm = new GridSizeView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void radialLocationsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RadialLocationView frm = new RadialLocationView();
			frm.MdiParent = this;
			frm.Show();
		}

		// This menu option is disabled for a master database if we don't have a CurrentOutageID.
		private void ImportComponentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Guid? OutageID = Globals.CurrentOutageID;
			EOutage curOutage = new EOutage(OutageID);
			ComponentImporter frm = new ComponentImporter((Guid)curOutage.OutageUntID);
			frm.MdiParent = this;
			frm.Show();
		}

		// This menu option is hidden for a master database.  Report definitions are Outage-Specific data.
		private void importComponentReportDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ReportDefinitionImporter frm = new ReportDefinitionImporter((Guid)Globals.CurrentOutageID);
			frm.MdiParent = this;
			frm.Show();
		}

		private void testConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Preferences_Master frm = new Preferences_Master();
			frm.MdiParent = this;
			frm.Show();
		}


		private void specialCalibrationParametersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SpecialCalParamView frm = new SpecialCalParamView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string filePath;
			BackupCurrentDatabase(true, out filePath);
		}

		private bool BackupCurrentDatabase(bool IsUserBackupRequired, out string filePath)
		{
			filePath = null;
			if (IsUserBackupRequired)
			{
				saveFileDialog1.InitialDirectory = Globals.FactotumDataFolder;
				saveFileDialog1.Filter = (Globals.IsMasterDB ?
					"Factotum Master Data Files *.mfac | *.mfac" :
					"Factotum Outage Data Files *.ofac | *.ofac");
				saveFileDialog1.DefaultExt = (Globals.IsMasterDB ? ".mfac" : ".ofac");

				saveFileDialog1.FileName = Globals.getUniqueBackupFileName(Globals.FactotumDataFolder);
				DialogResult rslt = saveFileDialog1.ShowDialog();
				// Note: If the user selects a file that already exists, the Save Dialog
				// will handle warning the user
				if (rslt == DialogResult.OK)
				{
					// Overwrite flag is specified -- could anything go wrong??
					filePath = saveFileDialog1.FileName;
					File.Copy(Globals.FactotumDatabaseFilePath, filePath, true);
				}
				return (rslt == DialogResult.OK);
			}
			else
			{
				filePath = Path.GetTempPath() + "Factotum.sdf";
				File.Copy(Globals.FactotumDatabaseFilePath, filePath, true);
				return true;
			}
		}

		// To open a new file, we must first backup the current file,
		// then open the new file and test for validity.  If it's invalid, we need to 
		// explain the problem and reopen the original.
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string filePathToOpen = null;
			if (!SelectFileToOpen(out filePathToOpen)) return;
			// Unless it's a new db, ask the user for a backup location first
			handleFileOpenRequest(filePathToOpen, !Globals.IsNewDB);
		}

		private void handleFileOpenRequest(string filePathToOpen)
		{
			handleFileOpenRequest(filePathToOpen, true);
		}

		private bool handleFileOpenRequest(string filePathToOpen, bool IsUserBackupRequired)
		{
			string backupPath = null;
			string message;
			
			// We always back up before opening, because the open operation may fail.
			// But in certain situations, like a new install, we'd rather not bother the user to
			// backup before opening.  As far as they're concerned there's nothing to back up.
			// So in this case, back up to a temp file.
			if (!BackupBeforeFileOpen(IsUserBackupRequired, out backupPath)) return false;

			if (!OpenDatabaseFile(filePathToOpen, out message))
			{
				// We always need to restore the original database if the Open attempt fails.
				if (Globals.cnn.State != ConnectionState.Closed) Globals.cnn.Close();
				File.Copy(backupPath, Globals.FactotumDatabaseFilePath, true);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				Globals.ReadDatabaseInfo();
				MessageBox.Show(message + "\nFile Open Operation was Cancelled", "Factotum: Cancelled File Open");
				return false;
			}
			return true;
		}

		private bool SelectFileToOpen(out string filePath)
		{
			filePath = null;
			openFileDialog1.InitialDirectory = Globals.FactotumDataFolder;
			openFileDialog1.Filter = "All Factotum Data Files *.mfac, *.ofac|*.mfac;*.ofac|Factotum Master Data Files *.mfac|*.mfac|Factotum Outage Data Files *.ofac|*.ofac";
			openFileDialog1.Title = "Select a Data File to Open";
			openFileDialog1.DefaultExt = ".ofac";
			DialogResult rslt = openFileDialog1.ShowDialog();
			filePath = openFileDialog1.FileName;
			return (rslt == DialogResult.OK);
		}

		private bool BackupBeforeFileOpen(bool IsUserBackupRequired, out string filePath)
		{
			filePath = null;
			if (IsUserBackupRequired)
			{
				MessageBox.Show("The current data file must be backed up first.", "Factotum: Backup Required");
			}
			if (!BackupCurrentDatabase(IsUserBackupRequired, out filePath))
			{
				MessageBox.Show("File Open operation was cancelled", "Factotum: User Cancelled");
				return false;
			}
			return true;
		}

		// Copy the file from the given path to the application directory, then 
		// connect to it.
		private bool OpenDatabaseFile(string filePathToOpen, out string message)
		{
			// Close all the windows first
			foreach (Form mdiChild in this.MdiChildren)
			{
				mdiChild.Close();
			}

			message = null;
			if (Globals.cnn.State != ConnectionState.Closed) Globals.cnn.Close();

			// Overwrite flag is specified -- could anything go wrong??
			File.Copy(filePathToOpen, Globals.FactotumDatabaseFilePath, true);

			try
			{
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			}
			catch (Exception ex)
			{
				ExceptionLogger.LogException(ex);
				message = "Unable to open data file.";
				return false;
			}
			Globals.ReadDatabaseInfo();
			if (!Globals.IsDatabaseOk(out message))
			{
				return false;
			}
			return true;
		}

		private void importOutageConfigurationDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<ChangeFinder> configTables = new List<ChangeFinder>();
			openFileDialog1.InitialDirectory = Globals.FactotumDataFolder;
			openFileDialog1.Filter = "Outage Data Files *.ofac | *.ofac";
			openFileDialog1.Title = "Select an Outage Data File to Import";
			DialogResult rslt = openFileDialog1.ShowDialog();
			if (rslt == DialogResult.OK)
			{
				// Copy the selected file to a temporary location so we can do some cleanup.
				string tempFile = Path.GetTempFileName();
				string tempDbPath = tempFile.Substring(0,tempFile.Length - 4) + ".ofac";

				// Overwrite flag is specified
				File.Copy(openFileDialog1.FileName, tempDbPath, true);

				string outageConnectionString = Globals.ConnectionStringForPath(tempDbPath);
				SqlCeConnection cnnOutage = new SqlCeConnection(outageConnectionString);

				// Check the database version.  If it's too old, the user needs to open it and save it 
				if (cnnOutage.State != ConnectionState.Open) cnnOutage.Open();
				SqlCeCommand cmd = cnnOutage.CreateCommand();
				cmd.CommandText =
					@"Select DatabaseVersion, IsMasterDB from Globals";
				SqlCeDataReader rdr = cmd.ExecuteReader();
				int DatabaseVersion = 0;
				bool isMasterDB = false;
				if (rdr.Read())
				{
					DatabaseVersion = (int)rdr["DatabaseVersion"];
					isMasterDB = (bool)rdr["IsMasterDB"];
				}
				rdr.Close();

				if (isMasterDB)
				{
					MessageBox.Show("The selected file does not contain outage data.","Factotum");
					return;
				}
				if (DatabaseVersion < UserSettings.sets.DbVersion)
				{
					MessageBox.Show("The selected Outage Data File is an older version.  The following must be done:\nOpen this file and Back it up to a new Outage Data file, then Import that new Outage Data File instead.","Factotum");
					return;
				}

				// delete tool kit assignments, set UsedInOutage flags
				// reset IsLclChange flags, and update the OutageImportedOn date for the outage.
				ChangeFinder.CleanUpOutageDatabase(cnnOutage);

				// Do the table comparisons for each configuration table
				// The order of that the tables are added to the list is important. Add parent tables 
				// first.
                ChgCustomer customerChanges = new ChgCustomer(Globals.cnn, cnnOutage);
                customerChanges.CompareTables_std();
                configTables.Add(customerChanges);

                ChgCalibrationProcedure calibrationProcedureChanges = new ChgCalibrationProcedure(Globals.cnn, cnnOutage);
                calibrationProcedureChanges.CompareTables_std();
                configTables.Add(calibrationProcedureChanges);

                ChgSite siteChanges = new ChgSite(Globals.cnn, cnnOutage);
                siteChanges.CompareTables_std();
                configTables.Add(siteChanges);

                ChgUnit unitChanges = new ChgUnit(Globals.cnn, cnnOutage);
                unitChanges.CompareTables_std();
                configTables.Add(unitChanges);

                ChgMeterModel meterModelChanges = new ChgMeterModel(Globals.cnn, cnnOutage);
                meterModelChanges.CompareTables_std();
                configTables.Add(meterModelChanges);

                ChgDucerModel ducerModelChanges = new ChgDucerModel(Globals.cnn, cnnOutage);
				ducerModelChanges.CompareTables_std();
				configTables.Add(ducerModelChanges);

				ChgDucer ducerChanges = new ChgDucer(Globals.cnn, cnnOutage);
				ducerChanges.CompareTables_std();
				configTables.Add(ducerChanges);

				ChgMeter meterChanges = new ChgMeter(Globals.cnn, cnnOutage);
				meterChanges.CompareTables_std();
				configTables.Add(meterChanges);

				ChgInspector inspectorChanges = new ChgInspector(Globals.cnn, cnnOutage);
				inspectorChanges.CompareTables_std();
				configTables.Add(inspectorChanges);

				ChgCalBlock calBlockChanges = new ChgCalBlock(Globals.cnn, cnnOutage);
				calBlockChanges.CompareTables_std();
				configTables.Add(calBlockChanges);

				ChgThermo thermoChanges = new ChgThermo(Globals.cnn, cnnOutage);
				thermoChanges.CompareTables_std();
				configTables.Add(thermoChanges);

				ChgSpecialCalParam specialCalParamChanges = new ChgSpecialCalParam(Globals.cnn, cnnOutage);
				specialCalParamChanges.CompareTables_std();
				configTables.Add(specialCalParamChanges);

				ChgCouplantType couplantTypeChanges = new ChgCouplantType(Globals.cnn, cnnOutage);
				couplantTypeChanges.CompareTables_std();
				configTables.Add(couplantTypeChanges);

				ChgGridProcedure gridProcedureChanges = new ChgGridProcedure(Globals.cnn, cnnOutage);
				gridProcedureChanges.CompareTables_std();
				configTables.Add(gridProcedureChanges);

				ChgComponentMaterial componentMaterialChanges = new ChgComponentMaterial(Globals.cnn, cnnOutage);
				componentMaterialChanges.CompareTables_std();
				configTables.Add(componentMaterialChanges);

				ChgComponentType componentTypeChanges = new ChgComponentType(Globals.cnn, cnnOutage);
				componentTypeChanges.CompareTables_std();
				configTables.Add(componentTypeChanges);

				ChgSystem systemChanges = new ChgSystem(Globals.cnn, cnnOutage);
				systemChanges.CompareTables_std();
				configTables.Add(systemChanges);

				ChgLine lineChanges = new ChgLine(Globals.cnn, cnnOutage);
				lineChanges.CompareTables_std();
				configTables.Add(lineChanges);

				ChgPipeScheduleLookup pipeScheduleLookupChanges = new ChgPipeScheduleLookup(Globals.cnn, cnnOutage);
				pipeScheduleLookupChanges.CompareTables_std();
				configTables.Add(pipeScheduleLookupChanges);

				ChgRadialLocation radialLocationChanges = new ChgRadialLocation(Globals.cnn, cnnOutage);
				radialLocationChanges.CompareTables_std();
				configTables.Add(radialLocationChanges);

				ChgGridSize gridSizeChanges = new ChgGridSize(Globals.cnn, cnnOutage);
				gridSizeChanges.CompareTables_std();
				configTables.Add(gridSizeChanges);

				ChgComponent componentChanges = new ChgComponent(Globals.cnn, cnnOutage);
				componentChanges.CompareTables_std();
				configTables.Add(componentChanges);

				ChgOutage outageChanges = new ChgOutage(Globals.cnn, cnnOutage);
				outageChanges.CompareTables_std();
				configTables.Add(outageChanges);

				// Map tables
				ChgMeterDucer meterDucerChanges = new ChgMeterDucer(Globals.cnn, cnnOutage);
				meterDucerChanges.CompareTables_map();
				configTables.Add(meterDucerChanges);

				ChgOutageGridProcedure outageGridProcedureChanges = new ChgOutageGridProcedure(Globals.cnn, cnnOutage);
				outageGridProcedureChanges.CompareTables_map();
				configTables.Add(outageGridProcedureChanges);

				ChgOutageInspector outageInspectorChanges = new ChgOutageInspector(Globals.cnn, cnnOutage);
				outageInspectorChanges.CompareTables_map();
				configTables.Add(outageInspectorChanges);


				bool first = true;
				foreach (ChangeFinder tbl in configTables)
				{
					tbl.AppendInsertionInfo(first);
					tbl.AppendModificationInfo(first);
					tbl.AppendAssignmentChangeInfo(first);
					first = false;
					
					tbl.UpdateOriginalTable();
				}

				MessageBox.Show("Outage Data File Imported OK", "Factotum: Import Outage");
			}
		}

		private void convertToMasterDBToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string backupPath;
			if (BackupCurrentDatabase(true, out backupPath))
			{
				Globals.ConvertCurrentDbToMaster();
			}
		}

		private void linesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LineView frm = new LineView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void systemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SystemView frm = new SystemView();
			frm.MdiParent = this;
			frm.Show();
		}

		private void generalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Preferences_General frm = new Preferences_General();
			frm.MdiParent = this;
			frm.Show();
		}

		private void viewChangesFromOutageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OutageChangesViewer frm = new OutageChangesViewer();
			frm.MdiParent = this;
			frm.Show();
		}


		private void generateANewActivationKeyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActivationKeyGenerator frm = new ActivationKeyGenerator(false);
			frm.MdiParent = this;
			frm.Show();
		}

		private void tmrBackup_Tick(object sender, EventArgs e)
		{
			backupCurrentDB();
		}

		private void backupCurrentDB()
		{
			File.Copy(Globals.FactotumDatabaseFilePath, Globals.FactotumBackupFilePath, true);
		}

		private void restoreAutobackupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!File.Exists(Globals.FactotumBackupFilePath))
			{
				MessageBox.Show("No backup file exists","Factotum");
				return;
			}
			handleFileOpenRequest(Globals.FactotumBackupFilePath, !Globals.IsNewDB);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

	}
}