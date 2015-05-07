using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;

namespace Factotum
{
	// This is the base class for finding and listing three types of changes made to a table
	// during an outage: 1) Insertions, 2) Modifications, 3) Assignment Changes.
	// Note: we are not concerned with Deletions.  The outage interface will prevent deletion of any record
	// created prior to the outage.
	// This class and its subclasses will connect to both the system master database file
	// and the database file returned from an outage.
	// They will create a new un-typed dataset and fill two tables: 'original' and 'modified'
	// They will then relate the two tables by their primary keys with the 'modified' table
	// as the parent and loop through the modified table, looking for insertions (no child records)
	// or modifications (field values different).
	// For each insertion, modification, or assignment change found, an info object will be added to a list
	// that describes this change.
	// The 
	class ChangeFinder
	{
		protected SqlCeConnection cnnOrig;
		protected SqlCeConnection cnnMod;
		protected DataSet ds;
		protected SqlCeDataAdapter daOrig;
		protected SqlCeDataAdapter daMod;

		protected List<InfoInsertion> insertions;
		protected List<InfoModification> modifications;
		protected List<InfoAssignmentChange> assignmentChanges;

		// These fields should be set by the inheriting class constructor.
		protected SqlCeCommand cmdSelOrig;
		protected SqlCeCommand cmdUpdOrig;
		protected SqlCeCommand cmdInsOrig;
		protected SqlCeCommand cmdDelOrig;
		protected SqlCeCommand cmdSelMod;
		protected string tableName;
		protected string tableName_friendly;

		// These two fields should be set if we're comparing map tables.
		protected string primaryParentTableName;
		protected string secondaryParentTableName;

		protected string primaryParentTableName_friendly;
		protected string secondaryParentTableName_friendly;

		public List<InfoInsertion> Insertions
		{
			get { return insertions;}
		}
		public List<InfoModification> Modifications
		{
			get { return modifications;}
		}
		public List<InfoAssignmentChange> AssignmentChanges
		{
			get { return assignmentChanges;}
		}

		public ChangeFinder(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)
		{
			this.cnnOrig = cnnOrig;
			this.cnnMod = cnnMod;

			ds = new DataSet("Comparer");
			daOrig = new SqlCeDataAdapter();
			daMod = new SqlCeDataAdapter();
			insertions = new List<InfoInsertion>();
			modifications = new List<InfoModification>();
			assignmentChanges = new List<InfoAssignmentChange>();

			cmdSelOrig = null;
			cmdUpdOrig = null;
			cmdInsOrig = null;
			cmdDelOrig = null;
			cmdSelMod = null;
			tableName = null;
		}

		// Note: this function should not be called until you have set the commands for the data adapters
		// This function performs comparisons of standard tables which have the following fields:
		// Guid? ID, string Name and that ID is the first column.
		public void CompareTables_std()
		{
			// These commands and their associated parameters should be set by the inheriting class.
			daOrig.SelectCommand = cmdSelOrig;
			daOrig.InsertCommand = cmdInsOrig;
			daOrig.UpdateCommand = cmdUpdOrig;

			daMod.SelectCommand = cmdSelMod;

			if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
			daOrig.Fill(ds, "Original");
			// Leave the original connection open...

			if (cnnMod.State != ConnectionState.Open) cnnMod.Open();
			daMod.Fill(ds, "Modified");
			cnnMod.Close();

			int columnCount = ds.Tables["Original"].Columns.Count;
			// Make Modified table the parent.  We should only have insertions and changed
			// records.  If we loop through the rows of the modified table, any time 
			// getChildRows() returns no rows, we'll know that it's a new record.
			DataRelation dr = new DataRelation("linkIDs", ds.Tables["Modified"].Columns[0], ds.Tables["Original"].Columns[0], false);
			ds.Relations.Add(dr);
			foreach (DataRow modRow in ds.Tables["Modified"].Rows)
			{
				DataRow[] origRows = modRow.GetChildRows("linkIDs");
				if (origRows.Length == 0)
				{
					insertions.Add(new InfoInsertion(tableName, tableName_friendly, modRow["Name"], modRow["ID"]));
					ds.Tables["Original"].Rows.Add(modRow.ItemArray);
				}
				else
				{
					// start at 1, assuming column 0 is the ID...
					for (int col = 1; col < ds.Tables["Original"].Columns.Count; col++)
					{
						if (!fieldValuesEqual(origRows[0], modRow, col))
						{
							modifications.Add(new InfoModification(tableName, tableName_friendly, 
								ds.Tables["Original"].Columns[col].ColumnName,
								modRow["Name"], modRow["ID"], origRows[0][col], modRow[col]));
							origRows[0][col] = modRow[col];
						}
					}
				}
			}
		}

		// Note: this function should not be called until you have set the commands for the data adapters
		// This function performs comparisons of map tables which have two Guid? primary foreign keys:
		public void CompareTables_map()
		{
			DataColumn[] parentColumns;
			DataColumn[] childColumns;
			DataRelation dr;
			// These commands and their associated parameters should be set by the inheriting class.
			// The select commands should join to the parent tables and include their name fields as
			// "PrimaryName" and "SecondaryName"
			daOrig.SelectCommand = cmdSelOrig;
			daMod.SelectCommand = cmdSelMod;

			daOrig.InsertCommand = cmdInsOrig;
			// The update command is not used for map tables
			// The delete command is only used for map tables (to un-assign)
			daOrig.DeleteCommand = cmdDelOrig;

			if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
			daOrig.Fill(ds, "Original");
			// Don't close the original connection...

			if (cnnMod.State != ConnectionState.Open) cnnMod.Open();
			daMod.Fill(ds, "Modified");
			cnnMod.Close();

			// Make Modified table the parent.  We should only have insertions and changed
			// records.  If we loop through the rows of the modified table, any time 
			// getChildRows() returns no rows, we'll know that it's a new record.
			parentColumns = new DataColumn[] {
				ds.Tables["Modified"].Columns[0], 
				ds.Tables["Modified"].Columns[1] };

			childColumns = new DataColumn[] {
				ds.Tables["Original"].Columns[0], 
				ds.Tables["Original"].Columns[1] };
			
			dr = new DataRelation("linkIDs", parentColumns, childColumns, false);
			ds.Relations.Add(dr);

			foreach (DataRow modRow in ds.Tables["Modified"].Rows)
			{
				DataRow[] origRows = modRow.GetChildRows("linkIDs");
				if (origRows.Length == 0)
				{
					assignmentChanges.Add(new InfoAssignmentChange(primaryParentTableName,primaryParentTableName_friendly,
						secondaryParentTableName, secondaryParentTableName_friendly,
						modRow["PrimaryName"], modRow["PrimaryID"], modRow["SecondaryName"], modRow["SecondaryID"], true));
					ds.Tables["Original"].Rows.Add(modRow.ItemArray);
				}
			}

			// Now Make Original table the parent so we can find deletions.
			// If we loop through the rows of the original table, any time 
			// getChildRows() returns no rows, we'll know we need to delete the current record.
			parentColumns = new DataColumn[] {
				ds.Tables["Original"].Columns[0], 
				ds.Tables["Original"].Columns[1] };

			childColumns = new DataColumn[] {
				ds.Tables["Modified"].Columns[0], 
				ds.Tables["Modified"].Columns[1] };

			ds.Relations.Remove("linkIDs");
			dr = new DataRelation("linkIDs", parentColumns, childColumns, false);
			ds.Relations.Add(dr);

			foreach (DataRow origRow in ds.Tables["Original"].Rows)
			{
				DataRow[] modRows = origRow.GetChildRows("linkIDs");
				if (modRows.Length == 0)
				{
					assignmentChanges.Add(new InfoAssignmentChange(primaryParentTableName, primaryParentTableName_friendly,
						secondaryParentTableName, secondaryParentTableName_friendly,
						origRow["PrimaryName"], origRow["PrimaryID"], origRow["SecondaryName"], origRow["SecondaryID"], false));
					origRow.Delete();
				}
			}


		}

		// Call Update to assert the dataset changes to the table in the original database
		public void UpdateOriginalTable()
		{
			if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
			int rowsAffected = daOrig.Update(ds,"Original");
		}

		// Check whehther two field values are "equal"
		private bool fieldValuesEqual(DataRow dr1, DataRow dr2, int idx)
		{
			if (dr1[idx] == null && dr2[idx] == null) return true;
			if (dr1[idx] != null && dr2[idx] != null && dr1[idx].Equals(dr2[idx])) return true;
			return false;
		}

		// Append insertion information to a temporary table, creating or clearing the table first
		// as required.  This table will contain changes for the last outage imported and will be
		// used to report to the master user which items have been inserted at the outage.
		public void AppendInsertionInfo(bool createNew)
		{
			// Check whether or not the temp table exists.
			SqlCeCommand cmd;
			cmd = cnnOrig.CreateCommand();
			cmd.CommandText =
				@"SELECT Count(*) 
				FROM Information_schema.tables 
				WHERE table_name = 'tmpOutageInsertions'";
			if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
			bool tableExists = ((int)cmd.ExecuteScalar() != 0);
			if (tableExists && createNew)
			{
				// the table exists, but we're supposed to start fresh, so drop the table
				// Note: I think it's better to drop than just delete records because it makes
				// it easy to change the table structure later if need be.
				cmd = cnnOrig.CreateCommand();
				cmd.CommandText = "drop table tmpOutageInsertions";
				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
				tableExists = false;
			}
			if (!tableExists)
			{
				// the table doesn't exist, so create it
				cmd = cnnOrig.CreateCommand();
				cmd.CommandText =
					"create table tmpOutageInsertions" +
					string.Format(" (tableName Nvarchar({0}) NOT NULL,", InfoInsertion.TableNameSize) +
					string.Format(" tableName_friendly Nvarchar({0}) NOT NULL,", InfoInsertion.TableNameSize) +
					string.Format(" recordName Nvarchar({0}) NOT NULL,", InfoInsertion.RecordNameSize) +
									" recordGuid UniqueIdentifier NOT NULL)";

				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
			}

			// Fill the temp table with any insertions in our collection.
			cmd = cnnOrig.CreateCommand();
			cmd.CommandText =
				@"insert into tmpOutageInsertions (tableName, tableName_friendly, recordName, recordGuid)
				values (@p0, @p1, @p2, @p3)";
			cmd.Parameters.Add("@p0", "");
			cmd.Parameters.Add("@p1", "");
			cmd.Parameters.Add("@p2", "");
			cmd.Parameters.Add("@p3", Guid.Empty);
			foreach (InfoInsertion insertion in insertions)
			{
				cmd.Parameters["@p0"].Value = insertion.tableName;
				cmd.Parameters["@p1"].Value = insertion.tableName_friendly;
				cmd.Parameters["@p2"].Value = insertion.recordName;
				cmd.Parameters["@p3"].Value = insertion.recordGuid;
				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
			}
		}

		// Append update information to a temporary table, creating or clearing the table first
		// as required.  This table will contain changes for the last outage imported and will be
		// used to report to the master user which items have been inserted at the outage.
		public void AppendModificationInfo(bool createNew)
		{
			// Check whether or not the temp table exists.
			SqlCeCommand cmd;
			cmd = cnnOrig.CreateCommand();
			cmd.CommandText =
				@"SELECT Count(*) 
				FROM Information_schema.tables 
				WHERE table_name = 'tmpOutageUpdates'";
			if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
			bool tableExists = ((int)cmd.ExecuteScalar() != 0);
			if (tableExists && createNew)
			{
				// the table exists, but we're supposed to start fresh, so drop the table
				// Note: I think it's better to drop than just delete records because it makes
				// it easy to change the table structure later if need be.
				cmd = cnnOrig.CreateCommand();
				cmd.CommandText = "drop table tmpOutageUpdates";
				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
				tableExists = false;
			}
			if (!tableExists)
			{
				// the table doesn't exist, so create it
				cmd = cnnOrig.CreateCommand();
				cmd.CommandText =
					"create table tmpOutageUpdates" +
					string.Format(" (tableName Nvarchar({0}) NOT NULL,", InfoModification.TableNameSize) +
					string.Format(" tableName_friendly Nvarchar({0}) NOT NULL,", InfoModification.TableNameSize) +
					string.Format(" recordName Nvarchar({0}) NOT NULL,", InfoModification.RecordNameSize) +
									" recordGuid UniqueIdentifier NOT NULL," +
					string.Format(" fieldName Nvarchar({0}) NOT NULL,", InfoModification.FieldNameSize) +
					string.Format(" oldValue Nvarchar({0}) NOT NULL,", InfoModification.ValueSize) +
					string.Format(" newValue Nvarchar({0}) NOT NULL)", InfoModification.ValueSize);

				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
			}

			// Fill the temp table with any insertions in our collection.
			cmd = cnnOrig.CreateCommand();
			cmd.CommandText =
				@"insert into tmpOutageUpdates 
				(tableName, 
				tableName_friendly,
				recordName,
				recordGuid,
				fieldName,
				oldValue,
				newValue)
				values (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";

			cmd.Parameters.Add("@p0", "");
			cmd.Parameters.Add("@p1", "");
			cmd.Parameters.Add("@p2", "");
			cmd.Parameters.Add("@p3", Guid.Empty);
			cmd.Parameters.Add("@p4", "");
			cmd.Parameters.Add("@p5", "");
			cmd.Parameters.Add("@p6", "");

			foreach (InfoModification modification in modifications)
			{
				if (excludeField(modification)) continue;
				cmd.Parameters["@p0"].Value = modification.tableName;
				cmd.Parameters["@p1"].Value = modification.tableName_friendly;
				cmd.Parameters["@p2"].Value = modification.recordName;
				cmd.Parameters["@p3"].Value = modification.recordGuid;
				cmd.Parameters["@p4"].Value = modification.fieldName;
				cmd.Parameters["@p5"].Value = modification.oldValue;
				cmd.Parameters["@p6"].Value = modification.newValue;

				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
			}
		}

		private bool excludeField(InfoModification mod)
		{
			// Shouldn't have to worry about IsLclChange flags, they should all be reset before
			// starting the import.
			string fld = mod.fieldName;
			// Exclude foreign keys.  If they can change, they'll have associated string fields.
			if (fld.Length > 2 && fld.EndsWith("ID")) return true;
			// The user doesn't need to know about these changes.
			if (fld.EndsWith("UsedInOutage")) return true;
			// We do want the user to know about active/inactive status changes, just not with this
			if (fld.EndsWith("IsActive")) return true;
			// Miscellaneous flags and byte enums to exclude
			string[] excludes = new string[] { "CbkMaterialType", "CbkType", 
				"CmpHighRad", "CmpHardToAccess","CmpHasDs","CmpHasBranch",
				"CmtCalBlockMaterial","InsLevel","OtgGridColDefaultCCW"};
			foreach (string exFld in excludes)
				if (fld == exFld) return true;

			return false;
		}

		// Append assignment change information to a temporary table, creating or clearing the table first
		// as required.  This table will contain changes for the last outage imported and will be
		// used to report to the master user which items have been inserted at the outage.
		public void AppendAssignmentChangeInfo(bool createNew)
		{
			// Check whether or not the temp table exists.
			SqlCeCommand cmd;
			cmd = cnnOrig.CreateCommand();
			cmd.CommandText =
				@"SELECT Count(*) 
				FROM Information_schema.tables 
				WHERE table_name = 'tmpOutageAssignmentChanges'";
			if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
			bool tableExists = ((int)cmd.ExecuteScalar() != 0);
			if (tableExists && createNew)
			{
				// the table exists, but we're supposed to start fresh, so drop the table
				// Note: I think it's better to drop than just delete records because it makes
				// it easy to change the table structure later if need be.
				cmd = cnnOrig.CreateCommand();
				cmd.CommandText = "drop table tmpOutageAssignmentChanges";
				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
				tableExists = false;
			}
			if (!tableExists)
			{
				// the table doesn't exist, so create it
				cmd = cnnOrig.CreateCommand();
				cmd.CommandText =
					"create table tmpOutageAssignmentChanges" +
					string.Format(" (primaryTableName Nvarchar({0}) NOT NULL,", InfoAssignmentChange.TableNameSize) +
					string.Format(" primaryTableName_friendly Nvarchar({0}) NOT NULL,", InfoAssignmentChange.TableNameSize) +
					string.Format(" primaryTableRecordName Nvarchar({0}) NOT NULL,", InfoAssignmentChange.RecordNameSize) +
									" primaryTableRecordGuid UniqueIdentifier NOT NULL," +
					string.Format(" secondaryTableName Nvarchar({0}) NOT NULL,", InfoAssignmentChange.TableNameSize) +
					string.Format(" secondaryTableName_friendly Nvarchar({0}) NOT NULL,", InfoAssignmentChange.TableNameSize) +
					string.Format(" secondaryTableRecordName Nvarchar({0}) NOT NULL,", InfoAssignmentChange.RecordNameSize) +
									" secondaryTableRecordGuid UniqueIdentifier NOT NULL," +
					string.Format(" isNowAssigned Bit NOT NULL)", InfoAssignmentChange.RecordNameSize);

				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
			}

			// Fill the temp table with any insertions in our collection.
			cmd = cnnOrig.CreateCommand();
			cmd.CommandText =
				@"insert into tmpOutageAssignmentChanges 
				(primaryTableName, 
				primaryTableName_friendly, 
				primaryTableRecordName, 
				primaryTableRecordGuid, 
				secondaryTableName,
				secondaryTableName_friendly,
				secondaryTableRecordName,
				secondaryTableRecordGuid,
				isNowAssigned)
				values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

			cmd.Parameters.Add("@p0", "");
			cmd.Parameters.Add("@p1", "");
			cmd.Parameters.Add("@p2", "");
			cmd.Parameters.Add("@p3", Guid.Empty);
			cmd.Parameters.Add("@p4", "");
			cmd.Parameters.Add("@p5", "");
			cmd.Parameters.Add("@p6", "");
			cmd.Parameters.Add("@p7", Guid.Empty);
			cmd.Parameters.Add("@p8", false);

			foreach (InfoAssignmentChange assignmentChange in assignmentChanges)
			{
				cmd.Parameters["@p0"].Value = assignmentChange.primaryTableName;
				cmd.Parameters["@p1"].Value = assignmentChange.primaryTableName_friendly;
				cmd.Parameters["@p2"].Value = assignmentChange.primaryTableRecordName;
				cmd.Parameters["@p3"].Value = assignmentChange.primaryTableRecordGuid;
				cmd.Parameters["@p4"].Value = assignmentChange.secondaryTableName;
				cmd.Parameters["@p5"].Value = assignmentChange.secondaryTableName_friendly;
				cmd.Parameters["@p6"].Value = assignmentChange.secondaryTableRecordName;
				cmd.Parameters["@p7"].Value = assignmentChange.secondaryTableRecordGuid;
				cmd.Parameters["@p8"].Value = assignmentChange.isNowAssigned;

				if (cnnOrig.State != ConnectionState.Open) cnnOrig.Open();
				cmd.ExecuteNonQuery();
			}
		}

		// Update the number of times inspected for the component
		// Do this after updating both the average time to Inspect and 
		// the average Crew Dose
		public static void UpdateTimesInspected(SqlCeConnection cnn)
		{
			SqlCeCommand cmd;

			// Increment the Times inspected for components which were included in reports.
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Components set CmpTimesInspected = (CmpTimesInspected + 1)
				where CmpDBid IN (Select IscCmpID from InspectedComponents)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

		}

		// This query is a bit too complicated for SQL CE, so use a cursor
		public static void UpdateTimeToInspect(SqlCeConnection cnn)
		{
			SqlCeCommand cmd;

			// Get a data reader with the time to inspect each component, summing over inspections.
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"select IscCmpID as ComponentID, sum(IspPersonHours) as TotalHours
				from InspectedComponents
				inner join Inspections on IscDBid = IspIscID
				where IspPersonHours is not NULL
				group by IscCmpID";

			Guid componentID;
			float totalHours;
			
			SqlCeConnection cnn2 = new SqlCeConnection(Globals.ConnectionStringFromConnection(cnn));
			SqlCeCommand cmd2;

			if (cnn.State != ConnectionState.Open) cnn.Open();
			SqlCeDataReader rdr = cmd.ExecuteReader();
			while (rdr.Read())
			{
				componentID = (Guid)rdr[0];
				totalHours = Convert.ToSingle(rdr[1]);
				cmd2 = cnn2.CreateCommand();
				cmd2.CommandText =
				@"Update Components set 
				CmpAvgInspectionTime = ((CmpAvgInspectionTime * CmpTimesInspected) + @p1) / (CmpTimesInspected + 1)
				where CmpDBid = @p0";
				cmd2.Parameters.Add("@p0", componentID);
				cmd2.Parameters.Add("@p1", totalHours);
				if (cnn2.State != ConnectionState.Open) cnn2.Open();
				cmd2.ExecuteNonQuery();
			}
			cnn2.Close();
			rdr.Close();

		}

		// This query is a bit too complicated for SQL CE, so use a cursor
		public static void UpdateCrewDose(SqlCeConnection cnn)
		{
			SqlCeCommand cmd;

			// Get a data reader with the time to inspect each component, summing over inspections.
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"select IscCmpID as ComponentID, sum(DstCrewDose) as TotalCrewDose
				from InspectedComponents
				inner join Inspections on IscDBid = IspIscID
				inner join Dsets on IspDBid = DstIspID
				where DstCrewDose is not NULL
				group by IscCmpID";

			Guid componentID;
			float totalCrewDose;

            SqlCeConnection cnn2 = new SqlCeConnection(Globals.ConnectionStringFromConnection(cnn));
			SqlCeCommand cmd2;

			if (cnn.State != ConnectionState.Open) cnn.Open();
			SqlCeDataReader rdr = cmd.ExecuteReader();
			while (rdr.Read())
			{
				componentID = (Guid)rdr[0];
				totalCrewDose = Convert.ToSingle(rdr[1]);
				cmd2 = cnn2.CreateCommand();
				cmd2.CommandText =
				@"Update Components set 
				CmpAvgCrewDose = ((CmpAvgCrewDose * CmpTimesInspected) + @p1) / (CmpTimesInspected + 1)
				where CmpDBid = @p0";
				cmd2.Parameters.Add("@p0", componentID);
				cmd2.Parameters.Add("@p1", totalCrewDose);
				if (cnn2.State != ConnectionState.Open) cnn2.Open();
				cmd2.ExecuteNonQuery();
			}
			cnn2.Close();
			rdr.Close();
		}

		// Used by the info classes below
		public static string truncateAndNA(object input, int max)
		{
			if (input == DBNull.Value || input == null) return "N/A";
			string strInput = input.ToString();
			if (strInput.Length > max)
				return strInput.Substring(0, max - 3) + "...";
			else
				return strInput;

		}

		public static void CleanUpOutageDatabase(SqlCeConnection cnn)
		{
			SqlCeCommand cmd;

			// Update the components table with the avg time to inspect,
			// avg crew dose and times inspected.
			UpdateTimeToInspect(cnn);
			UpdateCrewDose(cnn);
			UpdateTimesInspected(cnn);

			cmd = cnn.CreateCommand();
			// This will take care of clearing any references to kits.
			cmd.CommandText = "Delete from Kits";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Reset all IsLclChg flags -- records that were inserted locally.
			// These flags are only relevant in the sphere of the outage.
			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update CalBlocks set CbkIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update CalibrationProcedures set ClpIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Components set CmpIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update ComponentMaterials set CmtIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update ComponentTypes set CtpIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update CouplantTypes set CptIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Ducers set DcrIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update DucerModels set DmdIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update GridProcedures set GrpIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update GridSizes set GszIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Inspectors set InsIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Lines set LinIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update MeterModels set MmlIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Meters set MtrIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update PipeScheduleLookup set PslIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update RadialLocations set RdlIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update SpecialCalParams set ScpIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Systems set SysIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Thermos set ThmIsLclChg = 0";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Update the Outage DataImportedOn date
			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Outages set OtgDataImportedOn = @p0";
			cmd.Parameters.Add("@p0", DateTime.Today);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			
			// Update the UsedInOutage flags for all records which are referenced by another record

			// Cal blocks
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update CalBlocks set CbkUsedInOutage = 1
					where CbkDBid IN(Select DstCbkID from Dsets)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Components
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Components set CmpUsedInOutage = 1
					where CmpDBid IN(Select IscCmpID from InspectedComponents)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Couplant Types
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update CouplantTypes set CptUsedInOutage = 1
					where CptDBid IN(Select OtgCptID from Outages)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Ducers
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Ducers set DcrUsedInOutage = 1
					where DcrDBid IN(Select DstDcrID from Dsets)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Ducer Models
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update DucerModels set DmdUsedInOutage = 1
					where DmdDBid IN(Select DcrDmdID from Ducers
					inner join Dsets on DcrDBid = DstDcrID)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();


			// Grid Procedures
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update GridProcedures set GrpUsedInOutage = 1
					where GrpDBid IN(Select IscGrpID from InspectedComponents)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Grid Sizes
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update GridSizes set GszUsedInOutage = 1
					where GszDBid IN(Select GrdGszID from Grids)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Inspectors
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Inspectors set InsUsedInOutage = 1
					where InsDBid IN(Select DstInsID from Dsets)
					or InsDBid IN(Select IscInsID from InspectedComponents)
					or InsDBid IN(Select OinInsID from OutageInspectors)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Meters
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Meters set MtrUsedInOutage = 1
					where MtrDBid IN(Select DstMtrID from Dsets)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Meter Models
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update MeterModels set MmlUsedInOutage = 1
					where MmlDBid IN(Select MtrMmlID from Meters
					inner join Dsets on MtrDBid = DstMtrID)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Radial Locations
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update RadialLocations set RdlUsedInOutage = 1
					where RdlDBid IN(Select GrdRdlID from Grids)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Special Cal Params
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update SpecialCalParams set ScpUsedInOutage = 1
					where ScpDBid IN(Select ScvScpID from SpecialCalValues)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// Thermos
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"Update Thermos set ThmUsedInOutage = 1
					where ThmDBid IN(Select DstThmID from Dsets)";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

		}
	} // End ChangeFinder Class


	class InfoInsertion
	{
		public const int TableNameSize = 100;
		public const int RecordNameSize = 100;
		public string tableName;
		public string recordName;
		public string tableName_friendly;
		public Guid recordGuid;
		public InfoInsertion(object tableName, object tableName_friendly, object recordName, object recordGuid)
		{
			this.tableName = ChangeFinder.truncateAndNA(tableName, TableNameSize);
			this.tableName_friendly = ChangeFinder.truncateAndNA(tableName_friendly, TableNameSize);
			this.recordName = ChangeFinder.truncateAndNA(recordName, RecordNameSize);
			this.recordGuid = (Guid)recordGuid;
		}
	}

	class InfoModification
	{
		public const int TableNameSize = 100;
		public const int FieldNameSize = 100;
		public const int RecordNameSize = 100;
		public const int ValueSize = 255;
		public string tableName;
		public string tableName_friendly;
		public string fieldName;
		public string recordName;
		public Guid recordGuid;
		public string oldValue;
		public string newValue;
		public InfoModification(object tableName, object tableName_friendly, 
			object fieldName, object recordName, object recordGuid, object oldValue, object newValue)
		{
			this.tableName = ChangeFinder.truncateAndNA(tableName, TableNameSize);
			this.tableName_friendly = ChangeFinder.truncateAndNA(tableName_friendly, TableNameSize);
			this.fieldName = ChangeFinder.truncateAndNA(fieldName, FieldNameSize);
			this.recordName = ChangeFinder.truncateAndNA(recordName, RecordNameSize);
			this.recordGuid = (Guid)recordGuid;
			this.oldValue = ChangeFinder.truncateAndNA(oldValue, ValueSize);
			this.newValue = ChangeFinder.truncateAndNA(newValue, ValueSize);
		}

	}

	class InfoAssignmentChange
	{
		public const int TableNameSize = 100;
		public const int RecordNameSize = 100;
		public string primaryTableName;
		public string secondaryTableName;
		public string primaryTableName_friendly;
		public string secondaryTableName_friendly;
		public string primaryTableRecordName;
		public string secondaryTableRecordName;
		public Guid primaryTableRecordGuid;
		public Guid secondaryTableRecordGuid;
		public bool isNowAssigned;
		public InfoAssignmentChange(object primaryTableName, object primaryTableName_friendly,
			object secondaryTableName, object secondaryTableName_friendly,
			object primaryTableRecordName, object primaryTableRecordGuid,
			object secondaryTableRecordName, object secondaryTableRecordGuid, bool isNowAssigned)
		{
			this.primaryTableName = ChangeFinder.truncateAndNA(primaryTableName, TableNameSize);
			this.secondaryTableName = ChangeFinder.truncateAndNA(secondaryTableName, TableNameSize);
			this.primaryTableName_friendly = ChangeFinder.truncateAndNA(primaryTableName_friendly, TableNameSize);
			this.secondaryTableName_friendly = ChangeFinder.truncateAndNA(secondaryTableName_friendly, TableNameSize);
			this.primaryTableRecordName = ChangeFinder.truncateAndNA(primaryTableRecordName, RecordNameSize);
			this.secondaryTableRecordName = ChangeFinder.truncateAndNA(secondaryTableRecordName, RecordNameSize);
			this.primaryTableRecordGuid = (Guid)primaryTableRecordGuid;
			this.secondaryTableRecordGuid = (Guid)secondaryTableRecordGuid;
			this.isNowAssigned = isNowAssigned;
		}
	}
}
