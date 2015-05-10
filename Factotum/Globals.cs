using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Reflection;

namespace Factotum
{
	static class Globals
	{
        public static string dbName = "Factotum.sdf";

		public static Guid? CurrentOutageID;

		// Beta versions show "Beta" and build date in main form title
		public static bool IsBeta = false;

		// The current database connection
		public static SqlCeConnection cnn;

		// Info about the current database
		public static int DatabaseVersion;
		public static int CompatibleDBVersion;
		public static bool IsMasterDB;
		public static bool IsNewDB;
		public static string dbPassword = "omigod";
		
		private static string versionString;

		public static string VersionString
		{
			get 
			{
				if (versionString == null)
				{
					Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
					versionString = version.Major + "." + version.Minor + (Globals.IsBeta ? " (Beta)":"");
				}
				return versionString; 
			}
		}
	

		// Broadcast changes to the current database.
		public static event EventHandler DatabaseChanged;
		public static void OnDatabaseChanged()
		{
			// If we opened an outage database, the CurrentOutageID may change
			Globals.SetCurrentOutageID();
			// Copy to a temporary variable to be thread-safe.
			EventHandler temp = DatabaseChanged;
			if (temp != null)
				temp(new object(), new EventArgs());
		}

		// Broadcast changes to the current database.
		public static event EventHandler CurrentOutageChanged;
		public static void OnCurrentOutageChanged()
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler temp = CurrentOutageChanged;
			if (temp != null)
				temp(new object(), new EventArgs());
		}

		// Initialize and open the connection to the database Factotum.sdf in the app dir
		public static void InitConnection()
		{
			if (!File.Exists(Globals.FactotumDatabaseFilePath))
			{
				File.Copy(Globals.AppFolder + "\\" + dbName,Globals.FactotumDatabaseFilePath);
			}

			cnn = new SqlCeConnection(Globals.FactotumConnectionString());
			cnn.Open();
		}

		// This method should be called when the application loads and whenever a new
		// database file is opened
		public static void ReadDatabaseInfo()
		{
			if (Globals.cnn.State != ConnectionState.Open) cnn.Open();
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Select DatabaseVersion, SiteActivationKey, MasterRegCheckedOn,
					UnverifiedSessionCount, IsMasterDB, IsNewDB, IsInactivatedDB 
				from Globals";
			SqlCeDataReader rdr = cmd.ExecuteReader();
			if (rdr.Read())
			{
				DatabaseVersion = (int)rdr["DatabaseVersion"];
				IsMasterDB = (bool)rdr["IsMasterDB"];
				IsNewDB = (bool)rdr["IsNewDB"];
			}
			rdr.Close();

			// CompatibleDBVersion was added in Database Version 4
			if (DatabaseVersion > 3)
			{
				if (Globals.cnn.State != ConnectionState.Open) cnn.Open();
				cmd = cnn.CreateCommand();
				cmd.CommandText = "Select CompatibleDBVersion from Globals";
				rdr = cmd.ExecuteReader();
				if (rdr.Read())
				{
					CompatibleDBVersion = (int)rdr["CompatibleDBVersion"];
				}
				rdr.Close();
			}
			else CompatibleDBVersion = 3; // I Don't think it matters what this is set to.
		}

		public static void UpdateActivationKey(string newKey)
		{
			if (cnn.State != ConnectionState.Open) cnn.Open();
			SqlCeCommand cmd;
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Globals set SiteActivationKey = @p0, IsInactivatedDB = 0";
			cmd.Parameters.Add("@p0", newKey);
			cmd.ExecuteNonQuery();

		}

		public static void ConvertCurrentDbToMaster()
		{
            if (Globals.IsMasterDB)
            {
                MessageBox.Show("The current database is already a master database", "Factotum");
                return;
            }

            // Update the isLocalChange and isUsedInOutage flags for the configuration tables.
            ChangeFinder.CleanUpOutageDatabase(Globals.cnn);

			// Delete all outage-specific data
			Amputate_outageSpecific();
			
			// Set the dbtype to Master
			// Also, in case this is a new database, reset the IsNewDB flag.
			if (cnn.State != ConnectionState.Open) cnn.Open();
			SqlCeCommand cmd;
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Globals set IsMasterDB = 1, IsNewDB = 0";
			int recordsAffected = cmd.ExecuteNonQuery();

		}

		public static void Amputate_outageSpecific()
		{
			SqlCeCommand cmd;
			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from GridCells";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

            cmd = cnn.CreateCommand();
            cmd.CommandText = "Update Grids Set GrdParentID = NULL where GrdParentID IS NOT NULL";
            if (cnn.State != ConnectionState.Open) cnn.Open();
            cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Grids";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Graphics";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Dsets";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Inspections";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from InspectedComponents";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Kits";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

		}

		// Check to see if the version stored in the DB matches that stored in the settings.
		public static bool IsDatabaseOk(out string message)
		{
			message = null;
			// If the app is expecting a database older than the compatible version of the current database, 
			// we never want to try to open it.
			if (CompatibleDBVersion > UserSettings.sets.DbVersion)
			{
				message = "The version of the selected data file (" + DatabaseVersion + ") is not supported by this version of Factotum.\n" +
					"Please upgrade to the latest version of Factotum";
				return false;
			}
			else if (DatabaseVersion < UserSettings.sets.DbVersion)
			{
				// Upgrade the database to the currently supported version 
				DatabaseUpdater updater = new DatabaseUpdater(DatabaseVersion, Globals.cnn);
				updater.UpdateToCurrent();
			}
			return true;
		}
        
		// This is the REAL connection string for the database.
		// Leave the application property set - it's being used by some designers
        public static string FactotumConnectionString()
        {
            return ConnectionStringForPath(AppDataFolder,dbName);
        }

		// Get a database connection string for a given database file path
        public static string ConnectionStringForPath(string path, string dbname)
        {
            return ConnectionStringForPath(path + "\\" + dbName);
        }
        public static string ConnectionStringForPath(string path)
        {
            return "Datasource = \"" + path + "\"; Password = \"" + dbPassword + "\";";
        }
        // We need this function because the property cnn.ConnectionString removes the password
        public static string ConnectionStringFromConnection(SqlCeConnection cnn)
        {
            return cnn.ConnectionString + " Password = \"" + dbPassword + "\";";
        }
		// The application folder
		public static string AppFolder = System.Windows.Forms.Application.StartupPath;

		// The application data folder
        // The App Domain property "DataDirectory" is set to this in AppMain upon startup,
        // so |DataDirectory| and Globals.AppDataFolder should always have the same value.
        // Some of the .NET designers want to use the |DataDirectory| property.
		public static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Application.CompanyName + "\\" + Application.ProductName;

		// This is used by various file copy operations
		public static string FactotumDatabaseFilePath =
			AppDataFolder + "\\Factotum.sdf";

        // Set the Factotum data folder by default
        public static void SetDefaultFactotumDataFolder()
        {
            if (UserSettings.sets.FactotumDataFolder.Length == 0)
            {
                UserSettings.sets.FactotumDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Factotum";
                UserSettings.Save();
            }
        }

        // Set the Image folder by default
        public static void SetDefaultImageFolder()
        {
            if (UserSettings.sets.DefaultImageFolder.Length == 0)
            {
                UserSettings.sets.DefaultImageFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Factotum\\Images";
                UserSettings.Save();
            }
        }
        // Set the Meter Text files folder by default
        public static void SetDefaultMeterDataFolder()
        {
            if (UserSettings.sets.MeterDataFolder.Length == 0)
            {
                UserSettings.sets.MeterDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Factotum\\Meter text files";
                UserSettings.Save();
            }
        }
        
        // The Factotum Data Folder
		public static string FactotumDataFolder
		{
			get
			{
				return UserSettings.sets.FactotumDataFolder;
			}
		}
		// The Factotum Backup file path
		public static string FactotumBackupFilePath
		{
			get
			{
				return UserSettings.sets.FactotumDataFolder + "\\_Autobackup.sdf";
			}
		}

		// The meter data folder inside the desktop factotum folder
		public static string MeterDataFolder
		{
			get
			{
                return UserSettings.sets.MeterDataFolder;
			}
		}

		// The image data folder inside the desktop factotum folder
		public static string ImageFolder
		{
			get
			{
				return UserSettings.sets.DefaultImageFolder;
			}
		}

		// Convert the string property CurrentOutageID to Guid
		public static void SetCurrentOutageID()
		{

			if (Globals.IsMasterDB)
			{
				// If a Master Database is loaded, we should use the property setting for the 
				// current outage id if it's set to the id of a valid outage in the database.
				// Otherwise (if it's null or invalid) we should set it to the id of the 
				// first outage in the database.
				// If there are no outages in the database (i.e. a new install, we set it to null)
				// Set the property to whatever we come up with...
				Guid? outageID;

				if (UserSettings.sets.CurrentOutageID.Length == 0)
					outageID = null;
				else
					outageID = new Guid(UserSettings.sets.CurrentOutageID);

				if (outageID != null)
				{
					// Check if it's valid
					SqlCeCommand cmd = cnn.CreateCommand();
					cmd.CommandText = "Select OtgDBid from Outages where OtgDBid = @p0";
					cmd.Parameters.Add("@p0", outageID);
					object result = DowUtils.Util.NullForDbNull(cmd.ExecuteScalar());
					if (result == null)
					{
						// It was invalid, so try to get the first
						cmd = cnn.CreateCommand();
						cmd.CommandText = "Select OtgDBid from Outages";
						result = DowUtils.Util.NullForDbNull(cmd.ExecuteScalar());

						if (result == null) outageID = null;
						else outageID = (Guid?)result;

					}
					// Else -- It was found, so outageID is ok as is.

					UserSettings.sets.CurrentOutageID = outageID.ToString();
					UserSettings.Save();
					CurrentOutageID = outageID;
				}
				else
				{
					CurrentOutageID = null;
				}
			}
			else
			{
				// An Outage database is loaded, so the current outage is simply the only outage
				// in the database.  Don't worry about setting the property to this.
				SqlCeCommand cmd = cnn.CreateCommand();
				cmd.CommandText = "Select OtgDBid from Outages";
				object result = DowUtils.Util.NullForDbNull(cmd.ExecuteScalar());
				if (result != null) CurrentOutageID = (Guid?)result;
				
				// An outage data file must contain an outage... unless it's a new db!!!
				else if (!Globals.IsNewDB) throw new Exception("Outage Data File must contain an Outage");
			}
			OnCurrentOutageChanged();		
		}

		// Check to see if an instance of the form set to the selected ID already exists
		// If so, activate it.
		public static bool CanActivateForm(Form curForm, string formToOpen, Guid? ID)
		{
			Form[] siblings = curForm.MdiParent.MdiChildren;
			bool activated = false;
			foreach (Form sibling in siblings)
			{
				if (sibling.GetType().Name == formToOpen)
				{
					if ((sibling as IEntityEditForm).Entity.ID == ID)
					{
						sibling.Activate();
						activated = true;
						break;
					}
				}
			}
			return activated;
		}

		// Check to see if an instance of the form set to the selected ID already exists
		// If so, activate it and send out a reference to the form.
		public static bool CanActivateForm(Form curForm, string formToOpen, Guid? ID, out Form frmFound)
		{
			frmFound = null;
			Form[] siblings = curForm.MdiParent.MdiChildren;
			bool activated = false;
			foreach (Form sibling in siblings)
			{
				if (sibling.GetType().Name == formToOpen)
				{
					if ((sibling as IEntityEditForm).Entity.ID == ID)
					{
						sibling.Activate();
						activated = true;
						frmFound = sibling;
						break;
					}
				}
			}
			return activated;
		}

		// Check to see if an instance of the form set to the selected ID exists
		public static bool IsFormOpen(Form curForm, string testForm, Guid? ID)
		{
			Form[] siblings = curForm.MdiParent.MdiChildren;
			bool open = false;
			foreach (Form sibling in siblings)
			{
				if (sibling.GetType().Name == testForm)
				{
					if ((sibling as IEntityEditForm).Entity.ID == ID)
					{
						open = true;
						break;
					}
				}
			}
			return open;
		}

		// Delete the temp table if it exists.  
		// For temp tables that shouldn't exist under normal circumstances.
		public static void DeleteTempTableIfExists(string tableName)
		{
			SqlCeCommand cmd;
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"SELECT Count(*) 
				FROM Information_schema.tables 
				WHERE table_name = '" + tableName + "'";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			if ((int)cmd.ExecuteScalar() > 0)
			{
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "drop table " + tableName;
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		// Get a unique name for backing up the current database.
		public static string getUniqueBackupFileName(string folder)
		{
			string baseName;
			if (Globals.IsMasterDB) baseName = "Master";
			// An outage database must have an outage so if we're not a master, CurrentOutageID should be set.
			else baseName = new EOutage(Globals.CurrentOutageID).OutageName;
			return getUniqueBackupFileName(folder, baseName, Globals.IsMasterDB, true, false);
		}

		public static string getUniqueBackupFileName(string folder, string inPrefix, bool forMaster, bool includeDate, bool isFinal)
		{
			string suffix = (forMaster ? ".mfac" : ".ofac");
			DirectoryInfo di = new DirectoryInfo(folder);
			FileInfo[] dbFiles = di.GetFiles("*" + suffix);
			string base_prefix = inPrefix +
				(includeDate ? "_" + DateTime.Today.ToString("yyyy-MM-dd") : "");
			base_prefix += (isFinal ? "_" + "Final" : "");
			string prefix = base_prefix;
			int foundCount = 1;
			bool found = true;
			while (found)
			{
				found = false;
				foreach (FileInfo fi in dbFiles)
				{
					if (fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) == prefix)
					{
						prefix = base_prefix + "_" + foundCount;
						found = true;
						foundCount++;
						break;
					}
				}
			}
			return prefix + suffix;
		}

	}


	// Enums used by lots of view forms
	public enum FilterActiveStatus
	{
		ShowActive,
		ShowInactive
	}
	public enum FilterYesNoAll
	{
		ShowAll,
		Yes,
		No
	}

}
