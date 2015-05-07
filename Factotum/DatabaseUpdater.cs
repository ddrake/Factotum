using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;

namespace Factotum
{
	class DatabaseUpdater
	{
		int dbVersion;
		SqlCeConnection cnn;

		public DatabaseUpdater(int dbVersion, SqlCeConnection cnn)
		{
			this.dbVersion = dbVersion;
			this.cnn = cnn;
		}

		public void UpdateToCurrent()
		{
			int appDbVersion = UserSettings.sets.DbVersion;
			int curDbVersion = dbVersion;
			while (curDbVersion < appDbVersion)
			{
				switch (curDbVersion)
				{	
					case 1:
						Upgrade1To2();
						break;
					case 2:
						Upgrade2To3();
						break;
					case 3:
						Upgrade3To4();
						break;
					case 4:
						Upgrade4To5();
						break;
                    case 5:
                        Upgrade5To6();
                        break;
                    case 6:
                        Upgrade6To7();
                        break;
                    case 7:
						break;
					default:
						break;
				}
				curDbVersion++;
			}

			// Update the database version number in the Globals table.
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.Parameters.Add("@p0", curDbVersion);
			cmd.CommandText = "Update Globals Set DatabaseVersion = @p0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

		}
		private void Upgrade1To2()
		{
			// Add the cmpAvgCrewDose column to the Components table.
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.CommandText = "Alter table Components Add [CmpAvgCrewDose] Float Default 0 NOT NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

		}
		private void Upgrade2To3()
		{
			// Add the TmpComponentListing and TmpStatusReport tables.
			// This is an attempt to simplify the data source definitions for the reports which 
			// have been difficult to modify and inflexible with regard to subqueries, etc...
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Create table [TmpComponentListing]
				(
					[CmpName] Nvarchar(50) NULL,
					[CtpName] Nvarchar(25) NULL,
					[CmtName] Nvarchar(25) NULL,
					[LinName] Nvarchar(40) NULL,
					[SysName] Nvarchar(40) NULL,
					[PslSchedule] Nvarchar(20) NULL,
					[PslNomDia] Numeric(5,3) NULL,
					[CmpTimesInspected] Integer Default 0 NULL,
					[CmpAvgInspectionTime] Float Default 0 NULL,
					[CmpAvgCrewDose] Float Default 0 NULL,
					[CmpHighRad] Bit Default 0 NULL,
					[CmpHardToAccess] Bit Default 0 NULL,
					[CmpNote] Nvarchar(256) NULL
				)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Create table [TmpStatusReport]
				(
					[IscName] Nvarchar(50) NULL,
					[CmpName] Nvarchar(50) NULL,
					[IscIsReadyToInspect] Bit Default 0 NULL,
					[IscInsID] Uniqueidentifier NULL,
					[IscOtgID] Uniqueidentifier NULL,
					[IscMinCount] Smallint Default 0 NULL,
					[IscIsFinal] Bit Default 0 NULL,
					[IscIsUtFieldComplete] Bit Default 0 NULL,
					[IscReportSubmittedOn] Datetime NULL,
					[IscCompletionReportedOn] Datetime NULL,
					[IscWorkOrder] Nvarchar(50) NULL,
					[IscEdsNumber] Integer Default 0 NULL,
					[TotalCrewDose] Float NULL,
					[TotalPersonHours] Float NULL
				) ";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

		}
		private void Upgrade3To4()
		{
			// Add divider type columns to the Grids table.
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.CommandText = 
				@"Alter table Grids Add 	
					[GrdUpMainPreDivider] Tinyint Default 0 NOT NULL,
					[GrdDnMainPreDivider] Tinyint Default 0 NOT NULL,
					[GrdUpExtPreDivider] Tinyint Default 0 NOT NULL,
					[GrdDnExtPreDivider] Tinyint Default 0 NOT NULL,
					[GrdBranchPreDivider] Tinyint Default 0 NOT NULL,
					[GrdBranchExtPreDivider] Tinyint Default 0 NOT NULL,
					[GrdPostDivider] Tinyint Default 0 NOT NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = 
				@"Alter table Globals Add 	
					[CompatibleDBVersion] Integer Default 0 NOT NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Back end updates may not always break older front ends
			// The CompatibleDBVersion is used to determine whether or not an older front end version 
			// can work with a newer back end.  If the db version specified in the front end's system properties
			// is greater or equal the CompatibleDBVersion stored in the globals table of the DB, we'll let 
			// the front end use it.

			// This will need to be considered for possible updating in all future upgrades as well
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Globals Set	[CompatibleDBVersion] = 4";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();
		}
		private void Upgrade4To5()
		{
			// Add divider type columns to the Grids table.
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"ALTER TABLE RadialLocations ALTER COLUMN RdlName nvarchar(50)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"ALTER TABLE Grids ALTER COLUMN GrdAxialLocOverride nvarchar(50)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// This will need to be considered for possible updating in all future upgrades as well
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Globals Set	[CompatibleDBVersion] = 4";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();
		}

		private void Upgrade5To6()
		{
			// Add the GrdHideColumnLayoutGraphic column to the Grids table.
			SqlCeCommand cmd = cnn.CreateCommand();
			cmd.CommandText = "Alter table Grids Add [GrdHideColumnLayoutGraphic] Bit Default 0 NOT NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"ALTER TABLE GridSizes ALTER COLUMN GszAxialDistance numeric(6,3) NOT NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"ALTER TABLE GridSizes ALTER COLUMN GszRadialDistance numeric(6,3) NOT NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"ALTER TABLE Grids ALTER COLUMN GrdAxialDistance numeric(6,3) NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"ALTER TABLE Grids ALTER COLUMN GrdRadialDistance numeric(6,3) NULL";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Any app that works with version 4 db will work with this version.
			// the following would not have been necessary, except that I just 
			// fixed a mistake in the 4To5 updater where I had the compatible version as 5.
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Globals Set	[CompatibleDBVersion] = 4";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();
		}

        private void Upgrade6To7()
        {
            // Add an optional component section to additional measurements so we can compute Tscr.
            SqlCeCommand cmd = cnn.CreateCommand();
            cmd.CommandText = "Alter Table AdditionalMeasurements ADD AdmComponentSection smallint NULL";
            if (cnn.State != ConnectionState.Open) cnn.Open();
            cmd.ExecuteNonQuery();

            cmd = cnn.CreateCommand();
            cmd.CommandText =
                @"ALTER TABLE Components ALTER COLUMN CmpMisc1 nvarchar(50) NULL;";
            if (cnn.State != ConnectionState.Open) cnn.Open();
            cmd.ExecuteNonQuery();

            cmd = cnn.CreateCommand();
            cmd.CommandText =
                @"ALTER TABLE Components ALTER COLUMN CmpMisc2 nvarchar(50) NULL;";
            if (cnn.State != ConnectionState.Open) cnn.Open();
            cmd.ExecuteNonQuery();

        }

	}
}
