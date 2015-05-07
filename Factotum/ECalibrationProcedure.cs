using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ECalibrationProcedure : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for nullable numbers
		// Strings, images, etc, are reference types already
		private Guid? ClpDBid;
		private string ClpName;
		private string ClpDescription;
		private bool ClpIsLclChg;
		private bool ClpIsActive;

		// Textbox limits
		public static int ClpNameCharLimit = 50;
		public static int ClpDescriptionCharLimit = 255;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string ClpNameErrMsg;
		private string ClpDescriptionErrMsg;

		// Form level validation message
		private string ClpErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return ClpDBid; }
		}

		public string CalibrationProcedureName
		{
			get { return ClpName; }
			set { ClpName = Util.NullifyEmpty(value); }
		}

		public string CalibrationProcedureDescription
		{
			get { return ClpDescription; }
			set { ClpDescription = Util.NullifyEmpty(value); }
		}

		public bool CalibrationProcedureIsLclChg
		{
			get { return ClpIsLclChg; }
			set { ClpIsLclChg = value; }
		}

		public bool CalibrationProcedureIsActive
		{
			get { return ClpIsActive; }
			set { ClpIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string CalibrationProcedureNameErrMsg
		{
			get { return ClpNameErrMsg; }
		}

		public string CalibrationProcedureDescriptionErrMsg
		{
			get { return ClpDescriptionErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string CalibrationProcedureErrMsg
		{
			get { return ClpErrMsg; }
			set { ClpErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool CalibrationProcedureNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ClpNameCharLimit)
			{
				ClpNameErrMsg = string.Format("Calibration Procedure Names cannot exceed {0} characters", ClpNameCharLimit);
				return false;
			}
			else
			{
				ClpNameErrMsg = null;
				return true;
			}
		}

		public bool CalibrationProcedureDescriptionLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ClpDescriptionCharLimit)
			{
				ClpDescriptionErrMsg = string.Format("Calibration Procedure Descriptions cannot exceed {0} characters", ClpDescriptionCharLimit);
				return false;
			}
			else
			{
				ClpDescriptionErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool CalibrationProcedureNameValid(string name)
		{
			bool existingIsInactive;
			if (!CalibrationProcedureNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, ClpDBid, out existingIsInactive))
			{
				ClpNameErrMsg = existingIsInactive ?
					"That Calibration Procedure Name exists but its status has been set to inactive." :
					"That Calibration Procedure Name is already in use.";
				return false;
			}
			ClpNameErrMsg = null;
			return true;
		}

		public bool CalibrationProcedureDescriptionValid(string value)
		{
			if (!CalibrationProcedureDescriptionLengthOk(value)) return false;

			ClpDescriptionErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ECalibrationProcedure()
		{
			this.ClpIsLclChg = false;
			this.ClpIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ECalibrationProcedure(Guid? id) : this()
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
				ClpDBid,
				ClpName,
				ClpDescription,
				ClpIsLclChg,
				ClpIsActive
				from CalibrationProcedures
				where ClpDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				ClpDBid = (Guid?)dr[0];
				ClpName = (string)dr[1];
				ClpDescription = (string)Util.NullForDbNull(dr[2]);
				ClpIsLclChg = (bool)dr[3];
				ClpIsActive = (bool)dr[4];
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
				if (!Globals.IsMasterDB) ClpIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				ClpDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ClpDBid),
					new SqlCeParameter("@p1", ClpName),
					new SqlCeParameter("@p2", Util.DbNullForNull(ClpDescription)),
					new SqlCeParameter("@p3", ClpIsLclChg),
					new SqlCeParameter("@p4", ClpIsActive)
					});
				cmd.CommandText = @"Insert Into CalibrationProcedures (
					ClpDBid,
					ClpName,
					ClpDescription,
					ClpIsLclChg,
					ClpIsActive
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Calibration Procedure row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ClpDBid),
					new SqlCeParameter("@p1", ClpName),
					new SqlCeParameter("@p2", Util.DbNullForNull(ClpDescription)),
					new SqlCeParameter("@p3", ClpIsLclChg),
					new SqlCeParameter("@p4", ClpIsActive)});

				cmd.CommandText =
					@"Update CalibrationProcedures 
					set					
					ClpName = @p1,					
					ClpDescription = @p2,					
					ClpIsLclChg = @p3,					
					ClpIsActive = @p4
					Where ClpDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update calibration procedure row");
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
			if (!CalibrationProcedureNameValid(CalibrationProcedureName)) return false;
			if (!CalibrationProcedureDescriptionValid(CalibrationProcedureDescription)) return false;

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
			if (ClpDBid == null)
			{
				CalibrationProcedureErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!ClpIsLclChg && !Globals.IsMasterDB)
			{
				CalibrationProcedureErrMsg = "Unable to delete because this Calibration Procedure was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasSites())
			{
				CalibrationProcedureErrMsg = "Unable to delete because this Calibration Procedure is the default for one or more sites.";
				return false;
			}

			if (HasOutages())
			{
				CalibrationProcedureErrMsg = "Unable to delete because this Calibration Procedure is referenced by one or more outages.";
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
					@"Delete from CalibrationProcedures 
					where ClpDBid = @p0";
				cmd.Parameters.Add("@p0", ClpDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					CalibrationProcedureErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					OnChanged(ID);
					return true;
				}
			}
			else
			{
				CalibrationProcedureErrMsg = null;
				return false;
			}
		}

		private bool HasSites()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select SitDBid from Sites
					where SitClpID = @p0";
			cmd.Parameters.Add("@p0", ClpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		private bool HasOutages()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OtgDBid from Outages
					where OtgClpID = @p0";
			cmd.Parameters.Add("@p0", ClpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}


		//--------------------------------------------------------------------
		// Static listing methods which return collections of calibrationprocedures
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ECalibrationProcedureCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ECalibrationProcedure calibrationprocedure;
			ECalibrationProcedureCollection calibrationprocedures = new ECalibrationProcedureCollection();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				ClpDBid,
				ClpName,
				ClpDescription,
				ClpIsLclChg,
				ClpIsActive
				from CalibrationProcedures";
			if (showactive && !showinactive)
				qry += " where ClpIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where ClpIsActive = 0";

			qry += "	order by ClpName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				calibrationprocedure = new ECalibrationProcedure();
				calibrationprocedure.ClpName = "<No Selection>";
				calibrationprocedures.Add(calibrationprocedure);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				calibrationprocedure = new ECalibrationProcedure((Guid?)dr[0]);
				calibrationprocedure.ClpName = (string)(dr[1]);
				calibrationprocedure.ClpDescription = (string)Util.NullForDbNull(dr[2]);
				calibrationprocedure.ClpIsLclChg = (bool)(dr[3]);
				calibrationprocedure.ClpIsActive = (bool)(dr[4]);

				calibrationprocedures.Add(calibrationprocedure);	
			}
			// Finish up
			dr.Close();
			return calibrationprocedures;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataView()
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
					ClpDBid as ID,
					ClpName as CalibrationProcedureName,
					ClpDescription as CalibrationProcedureDescription,
					CASE
						WHEN ClpIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as CalibrationProcedureIsLclChg,
					CASE
						WHEN ClpIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as CalibrationProcedureIsActive
					from CalibrationProcedures";
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
				cmd.CommandText = "Select ClpIsActive from CalibrationProcedures where ClpName = @p1";
			}
			else
			{
				cmd.CommandText = "Select ClpIsActive from CalibrationProcedures where ClpName = @p1 and ClpDBid != @p0";
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

			if (CalibrationProcedureName == null)
			{
				ClpNameErrMsg = "A unique Calibration Procedure Name is required";
				allFilled = false;
			}
			else
			{
				ClpNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// CalibrationProcedure Collection class
	//--------------------------------------
	public class ECalibrationProcedureCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ECalibrationProcedureCollection()
		{ }
		//the indexer of the collection
		public ECalibrationProcedure this[int index]
		{
			get
			{
				return (ECalibrationProcedure)this.List[index];
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
			foreach (ECalibrationProcedure calibrationprocedure in InnerList)
			{
				if (calibrationprocedure.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ECalibrationProcedure item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ECalibrationProcedure item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ECalibrationProcedure item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ECalibrationProcedure item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
