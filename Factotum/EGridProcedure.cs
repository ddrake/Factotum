using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EGridProcedure : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler GridProcedureOutageAssignmentsChanged;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}

		protected virtual void OnGridProcedureOutageAssignmentsChanged()
		{
			EventHandler temp = GridProcedureOutageAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for nullable numbers
		// Strings, images, etc, are reference types already
		private Guid? GrpDBid;
		private string GrpName;
		private string GrpDescription;
		private short? GrpDsDiameters;
		private bool GrpIsLclChg;
		private bool GrpUsedInOutage;
		private bool GrpIsActive;

		// This is used by ListWithAssignmentsForOutage() to indicate whether or not
		// the current grid procedure is assigned to the specified outage.
		private bool isAssignedToYourOutage;

		// Textbox limits
		public static int GrpNameCharLimit = 50;
		public static int GrpDescriptionCharLimit = 255;
		public static int GrpDsDiametersCharLimit = 6;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string GrpNameErrMsg;
		private string GrpDescriptionErrMsg;
		private string GrpDsDiametersErrMsg;

		// Form level validation message
		private string GrpErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return GrpDBid; }
		}

		public string GridProcedureName
		{
			get { return GrpName; }
			set { GrpName = Util.NullifyEmpty(value); }
		}

		public string GridProcedureDescription
		{
			get { return GrpDescription; }
			set { GrpDescription = Util.NullifyEmpty(value); }
		}

		public short? GridProcedureDsDiameters
		{
			get { return GrpDsDiameters; }
			set { GrpDsDiameters = value; }
		}

		public bool GridProcedureIsLclChg
		{
			get { return GrpIsLclChg; }
			set { GrpIsLclChg = value; }
		}

		public bool GridProcedureUsedInOutage
		{
			get { return GrpUsedInOutage; }
			set { GrpUsedInOutage = value; }
		}

		public bool GridProcedureIsActive
		{
			get { return GrpIsActive; }
			set { GrpIsActive = value; }
		}

		// This is used by ListWithAssignmentsForOutage() to indicate whether or not
		// the current grid procedure is assigned to the specified outage.
		public bool IsAssignedToYourOutage
		{
			get { return isAssignedToYourOutage; }
			set { isAssignedToYourOutage = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string GridProcedureNameErrMsg
		{
			get { return GrpNameErrMsg; }
		}

		public string GridProcedureDescriptionErrMsg
		{
			get { return GrpDescriptionErrMsg; }
		}

		public string GridProcedureDsDiametersErrMsg
		{
			get { return GrpDsDiametersErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string GridProcedureErrMsg
		{
			get { return GrpErrMsg; }
			set { GrpErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool GridProcedureNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrpNameCharLimit)
			{
				GrpNameErrMsg = string.Format("Grid Procedure Names cannot exceed {0} characters", GrpNameCharLimit);
				return false;
			}
			else
			{
				GrpNameErrMsg = null;
				return true;
			}
		}

		public bool GridProcedureDescriptionLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrpDescriptionCharLimit)
			{
				GrpDescriptionErrMsg = string.Format("Grid Procedure Descriptions cannot exceed {0} characters", GrpDescriptionCharLimit);
				return false;
			}
			else
			{
				GrpDescriptionErrMsg = null;
				return true;
			}
		}

		public bool GridProcedureDsDiametersLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrpDsDiametersCharLimit)
			{
				GrpDsDiametersErrMsg = string.Format("Grid Procedure D/S Diameters cannot exceed {0} characters", GrpDsDiametersCharLimit);
				return false;
			}
			else
			{
				GrpDsDiametersErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool GridProcedureNameValid(string name)
		{
			bool existingIsInactive;
			if (!GridProcedureNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, GrpDBid, out existingIsInactive))
			{
				GrpNameErrMsg = existingIsInactive ?
					"A Grid Procedure with that Name exists but its status has been set to inactive." :
					"That Grid Procedure Name is already in use.";
				return false;
			}
			GrpNameErrMsg = null;
			return true;
		}

		public bool GridProcedureDescriptionValid(string value)
		{
			if (!GridProcedureDescriptionLengthOk(value)) return false;

			GrpNameErrMsg = null;
			return true;
		}

		public bool GridProcedureDsDiametersValid(string value)
		{
			// Add some real validation here if needed.

			short result;
			if (Util.IsNullOrEmpty(value))
			{
				GrpDsDiametersErrMsg = null;
				return true;
			}
			if (short.TryParse(value, out result) && result > 0 && result < 100)
			{
				GrpDsDiametersErrMsg = null;
				return true;
			}
			GrpDsDiametersErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EGridProcedure()
		{
			this.GrpIsLclChg = false;
			this.GrpUsedInOutage = false;
			this.GrpIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EGridProcedure(Guid? id) : this()
		{
			Load(id);
		}

		//--------------------------------------
		// Public Methods
		//--------------------------------------

		//----------------------------------------------------
		// Load the object from the database given a Guid?
		//----------------------------------------------------
		public void Load(Guid? id)
		{
			if (id == null) return;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
				@"Select 
				GrpDBid,
				GrpName,
				GrpDescription,
				GrpDsDiameters,
				GrpIsLclChg,
				GrpUsedInOutage,
				GrpIsActive
				from GridProcedures
				where GrpDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				GrpDBid = (Guid?)dr[0];
				GrpName = (string)dr[1];
				GrpDescription = (string)Util.NullForDbNull(dr[2]);
				GrpDsDiameters = (short?)Util.NullForDbNull(dr[3]);
				GrpIsLclChg = (bool)dr[4];
				GrpUsedInOutage = (bool)dr[5];
				GrpIsActive = (bool)dr[6];
			}
			dr.Close();
		}

		//--------------------------------------
		// Save the current record if it's valid
		//--------------------------------------
		public Guid? Save()
		{
			if (!Valid())
			{
				// Note: We're returning null if we fail,
				// so don't just assume you're going to get your id back 
				// and set your id to the result of this function call.
				return null;
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) GrpIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				GrpDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GrpDBid),
					new SqlCeParameter("@p1", GrpName),
					new SqlCeParameter("@p2", Util.DbNullForNull(GrpDescription)),
					new SqlCeParameter("@p3", Util.DbNullForNull(GrpDsDiameters)),
					new SqlCeParameter("@p4", GrpIsLclChg),
					new SqlCeParameter("@p5", GrpUsedInOutage),
					new SqlCeParameter("@p6", GrpIsActive)
					});
				cmd.CommandText = @"Insert Into GridProcedures (
					GrpDBid,
					GrpName,
					GrpDescription,
					GrpDsDiameters,
					GrpIsLclChg,
					GrpUsedInOutage,
					GrpIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Grid Procedures row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GrpDBid),
					new SqlCeParameter("@p1", GrpName),
					new SqlCeParameter("@p2", Util.DbNullForNull(GrpDescription)),
					new SqlCeParameter("@p3", Util.DbNullForNull(GrpDsDiameters)),
					new SqlCeParameter("@p4", GrpIsLclChg),
					new SqlCeParameter("@p5", GrpUsedInOutage),
					new SqlCeParameter("@p6", GrpIsActive)});

				cmd.CommandText =
					@"Update GridProcedures 
					set					
					GrpName = @p1,					
					GrpDescription = @p2,					
					GrpDsDiameters = @p3,					
					GrpIsLclChg = @p4,					
					GrpUsedInOutage = @p5,					
					GrpIsActive = @p6
					Where GrpDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update grid procedures row");
				}
			}
			OnChanged(ID);
			return ID;
		}

		//--------------------------------------
		// Validate the current record
		//--------------------------------------
		// Make this public so that the UI can check validation itself
		// if it chooses to do so.  This is also called by the Save function.
		public bool Valid()
		{
			// First check each field to see if it's valid from the UI perspective
			if (!GridProcedureNameValid(GridProcedureName)) return false;
			if (!GridProcedureDescriptionValid(GridProcedureDescription)) return false;

			// Check form to make sure all required fields have been filled in
			if (!RequiredFieldsFilled()) return false;

			// Check for incorrect field interactions...

			return true;
		}

		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{

			// If the current object doesn't reference a database record, there's nothing to do.
			if (GrpDBid == null)
			{
				GridProcedureErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (GrpUsedInOutage)
			{
				GridProcedureErrMsg = "Unable to delete because this Grid Procedure has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!GrpIsLclChg && !Globals.IsMasterDB)
			{
				GridProcedureErrMsg = "Unable to delete this Grid Procedure because it was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasInspectedComponents())
			{
				GridProcedureErrMsg = "Unable to delete because this Grid Procedure is referenced by one or more Component Reports.";
				return false;
			}

			if (HasOutages())
			{
				GridProcedureErrMsg = "Unable to delete because this Grid Procedure is referenced by one or more outages.";
				return false;
			}

			DialogResult rslt = DialogResult.None;
			if (promptUser)
			{
				rslt = MessageBox.Show("Are you sure?", "Factotum: Deleting...",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			}

			if (!promptUser || rslt == DialogResult.OK)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from GridProcedures 
					where GrpDBid = @p0";
				cmd.Parameters.Add("@p0", GrpDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					GridProcedureErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					GridProcedureErrMsg = null;
					OnChanged(ID);
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool HasInspectedComponents()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select IscDBid from InspectedComponents
					where IscGrpID = @p0";
			cmd.Parameters.Add("@p0", GrpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		private bool HasOutages()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OgpOtgID from OutageGridProcedures
					where OgpGrpID = @p0";
			cmd.Parameters.Add("@p0", GrpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of gridprocedures
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EGridProcedureCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			EGridProcedure gridprocedure;
			EGridProcedureCollection gridprocedures = new EGridProcedureCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				GrpDBid,
				GrpName,
				GrpDescription,
				GrpDsDiameters,
				GrpIsLclChg,
				GrpUsedInOutage,
				GrpIsActive
				from GridProcedures";
			if (showactive && !showinactive)
				qry += " where GrpIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where GrpIsActive = 0";

			qry += "	order by GrpName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				gridprocedure = new EGridProcedure();
				gridprocedure.GrpName = "<No Selection>";
				gridprocedures.Add(gridprocedure);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				gridprocedure = new EGridProcedure((Guid?)dr[0]);
				gridprocedure.GrpName = (string)(dr[1]);
				gridprocedure.GrpDescription = (string)Util.NullForDbNull(dr[2]);
				gridprocedure.GrpDsDiameters = (short?)(dr[3]);
				gridprocedure.GrpIsLclChg = (bool)(dr[4]);
				gridprocedure.GrpUsedInOutage = (bool)(dr[5]);
				gridprocedure.GrpIsActive = (bool)(dr[6]);

				gridprocedures.Add(gridprocedure);	
			}
			// Finish up
			dr.Close();
			return gridprocedures;
		}

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EGridProcedureCollection ListForOutage(Guid outageID, 
			bool showinactive, bool addNoSelection)
		{
			EGridProcedure gridprocedure;
			EGridProcedureCollection gridprocedures = new EGridProcedureCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				GrpDBid,
				GrpName,
				GrpDescription,
				GrpDsDiameters,
				GrpIsLclChg,
				GrpUsedInOutage,
				GrpIsActive
				from GridProcedures
				left outer join OutageGridProcedures
				on GrpDBid = OgpGrpID";
			qry += " where OgpOtgID = @p1";
			if (!showinactive)
				qry += " and GrpIsActive = 1";

			qry += "	order by GrpName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", outageID);

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				gridprocedure = new EGridProcedure();
				gridprocedure.GrpName = "<No Selection>";
				gridprocedures.Add(gridprocedure);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				gridprocedure = new EGridProcedure((Guid?)dr[0]);
				gridprocedure.GrpName = (string)(dr[1]);
				gridprocedure.GrpDescription = (string)Util.NullForDbNull(dr[2]);
				gridprocedure.GrpDsDiameters = (short?)Util.NullForDbNull(dr[3]);
				gridprocedure.GrpIsLclChg = (bool)(dr[4]);
				gridprocedure.GrpUsedInOutage = (bool)(dr[5]);
				gridprocedure.GrpIsActive = (bool)(dr[6]);

				gridprocedures.Add(gridprocedure);
			}
			// Finish up
			dr.Close();
			return gridprocedures;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview, you probably don't.
		public static EGridProcedureCollection ListWithAssignmentsForOutage(Guid? OutageID,
			bool showactive, bool showinactive)
		{
			EGridProcedure grp;
			EGridProcedureCollection grps = new EGridProcedureCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry =
@"
				Select 
				GrpDBid, 
				GrpName,
				GrpDescription,
				GrpDsDiameters,
				GrpIsLclChg,
				GrpUsedInOutage,
				GrpIsActive, 
				Max(Case OutageGridProcedures.OgpOtgID
					when @p1 then 1
					else 0
					end) as IsAssigned
				From GridProcedures 
				left outer join OutageGridProcedures
				on GridProcedures.GrpDBid = OutageGridProcedures.OgpGrpID"; 
			if (showactive && !showinactive)
				qry += 
@" 
				where GrpIsActive = 1";
			else if (!showactive && showinactive)
				qry += 
@"				where GrpIsActive = 0";

			qry +=
@"			
				Group by GrpName, GrpDBid, GrpDescription, 
				GrpDsDiameters, GrpIsLclChg, GrpUsedInOutage, 
				GrpIsActive";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", OutageID == null ? Guid.Empty : OutageID);
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				grp = new EGridProcedure((Guid?)dr[0]);
				grp.GridProcedureName = (string)dr[1];
				grp.GridProcedureDescription = (string)Util.NullForDbNull(dr[2]);
				grp.GridProcedureDsDiameters = (short?)Util.NullForDbNull(dr[3]);
				grp.GridProcedureIsLclChg = (bool)dr[4];
				grp.GridProcedureUsedInOutage = (bool)dr[5];
				grp.GridProcedureIsActive = (bool)dr[6];
				grp.IsAssignedToYourOutage = Convert.ToBoolean(dr[7]);

				grps.Add(grp);
			}
			// Finish up
			dr.Close();
			return grps;
		}

		public static void UpdateAssignmentsToOutage(Guid? OutageID, EGridProcedureCollection grps)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			// First delete any existing grid procedure assignments (except those that are 
			// used in reports)
			cmd.CommandText = 
				@"Delete from OutageGridProcedures 
				where OgpOtgID = @p1
				and not exists(
					select IscDBid from InspectedComponents 
					where IscGrpID = OgpGrpID)";
			cmd.Parameters.Add("@p1", OutageID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OgpOtgID from OutageGridProcedures
					where OgpOtgID = @p1 and OgpGrpID = @p2";
			cmd.Parameters.Add("@p1", OutageID);
			cmd.Parameters.Add("@p2", "");

			SqlCeCommand cmd2 = Globals.cnn.CreateCommand();
			cmd2.CommandText = 
				@"Insert Into OutageGridProcedures (OgpOtgID, OgpGrpID)
				values (@p1, @p2)";
			cmd2.Parameters.Add("@p1", OutageID);
			cmd2.Parameters.Add("@p2", "");

			// Now insert the current grid procedure assignments (except those that are
			// used in reports)
			foreach (EGridProcedure grp in grps)
			{
				if (grp.isAssignedToYourOutage)
				{
					cmd.Parameters["@p2"].Value = grp.ID;
					cmd2.Parameters["@p2"].Value = grp.ID;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();

					if (cmd.ExecuteScalar() == null)
					{
						if (cmd2.ExecuteNonQuery() != 1)
						{
							throw new Exception("Unable to insert Outage Grid Procedures row");
						}
					}
				}
			}
			// Just throw one event for all
			EGridProcedure gridproc = new EGridProcedure();
			gridproc.OnGridProcedureOutageAssignmentsChanged();
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.

		// Pass a null outage id if you don't want to limit to the current outage.
		public static DataView GetDefaultDataView(Guid? OutageID)
		{
			DataSet ds = new DataSet();
			DataView dv;
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			// Changing the booleans to 'Yes' and 'No' eliminates the silly checkboxes and
			// makes the column sortable.
			// You'll likely want to modify this query further, joining in other tables, etc.
			string qry = @"Select 
					GrpDBid as ID,
					GrpName as GridProcedureName,
					GrpDsDiameters as GridProcedureDsDiameters,
					GrpDescription as GridProcedureDescription,
					CASE
						WHEN GrpIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as GridProcedureIsLclChg,
					CASE
						WHEN GrpUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as GridProcedureUsedInOutage,
					CASE
						WHEN GrpIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as GridProcedureIsActive
					from GridProcedures";
			if (OutageID != null)
			{
				qry +=
					@" Inner join OutageGridProcedures on GrpDBid = OgpGrpID
					where OgpOtgID = @p1";
				cmd.Parameters.Add("@p1", OutageID);
			}
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}
		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExists(string name, Guid? id, out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			if (id == null)
			{
				cmd.CommandText = "Select GrpIsActive from GridProcedures where GrpName = @p1";
			}
			else
			{
				cmd.CommandText = "Select GrpIsActive from GridProcedures where GrpName = @p1 and GrpDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;
			return exists;

		}


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (GridProcedureName == null)
			{
				GrpNameErrMsg = "A unique Grid Procedure Name is required";
				allFilled = false;
			}
			else
			{
				GrpNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// GridProcedure Collection class
	//--------------------------------------
	public class EGridProcedureCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EGridProcedureCollection()
		{ }
		//the indexer of the collection
		public EGridProcedure this[int index]
		{
			get
			{
				return (EGridProcedure)this.List[index];
			}
		}
		//this method fires the Changed event.
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
			{
				Changed(this, e);
			}
		}

		public bool ContainsID(Guid? ID)
		{
			if (ID == null) return false;
			foreach (EGridProcedure gridprocedure in InnerList)
			{
				if (gridprocedure.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EGridProcedure item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EGridProcedure item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EGridProcedure item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EGridProcedure item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
