using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EOutage : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler<EntityChangedEventArgs> Added;
		public static event EventHandler<EntityChangedEventArgs> Updated;
		public static event EventHandler<EntityChangedEventArgs> Deleted;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnAdded(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Added;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnUpdated(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Updated;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnDeleted(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Deleted;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for nullable numbers
		// Strings, images, etc, are reference types already
		private Guid? OtgDBid;
		private string OtgName;
		private Guid? OtgUntID;
		private Guid? OtgClpID;
		private Guid? OtgCptID;
		private string OtgCouplantBatch;
		private string OtgFacPhone;
		private DateTime? OtgStartedOn;
		private DateTime? OtgEndedOn;
		private DateTime? OtgConfigExportedOn;
		private DateTime? OtgDataImportedOn;
		private bool OtgGridColDefaultCCW;

		// Textbox limits
		public static int OtgNameCharLimit = 50;
		public static int OtgCouplantBatchCharLimit = 50;
		public static int OtgFacPhoneCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string OtgNameErrMsg;
		private string OtgCouplantBatchErrMsg;
		private string OtgFacPhoneErrMsg;

		// Form level validation message
		private string OtgErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return OtgDBid; }
		}

		public string OutageName
		{
			get { return OtgName; }
			set { OtgName = Util.NullifyEmpty(value); }
		}

		public Guid? OutageUntID
		{
			get { return OtgUntID; }
			set { OtgUntID = value; }
		}

		public Guid? OutageClpID
		{
			get { return OtgClpID; }
			set { OtgClpID = value; }
		}

		public Guid? OutageCptID
		{
			get { return OtgCptID; }
			set { OtgCptID = value; }
		}

		public string OutageCouplantBatch
		{
			get { return OtgCouplantBatch; }
			set { OtgCouplantBatch = Util.NullifyEmpty(value); }
		}

		public string OutageFacPhone
		{
			get { return OtgFacPhone; }
			set { OtgFacPhone = Util.NullifyEmpty(value); }
		}

		public DateTime? OutageStartedOn
		{
			get { return OtgStartedOn; }
			set { OtgStartedOn = value; }
		}

		public DateTime? OutageEndedOn
		{
			get { return OtgEndedOn; }
			set { OtgEndedOn = value; }
		}

		public DateTime? OutageConfigExportedOn
		{
			get { return OtgConfigExportedOn; }
			set { OtgConfigExportedOn = value; }
		}

		public DateTime? OutageDataImportedOn
		{
			get { return OtgDataImportedOn; }
			set { OtgDataImportedOn = value; }
		}

		public bool OutageGridColDefaultCCW
		{
			get { return OtgGridColDefaultCCW; }
			set { OtgGridColDefaultCCW = value; }
		}
		
		public string OutageCalibrationProcedureName
		{
			get
			{
				if (OtgClpID == null) return null;
				ECalibrationProcedure clp = new ECalibrationProcedure(OtgClpID);
				return clp.CalibrationProcedureName;
			}
		}

		public string OutageCouplantTypeName
		{
			get
			{
				if (OtgCptID == null) return null;
				ECouplantType cpt = new ECouplantType(OtgCptID);
				return cpt.CouplantTypeName;
			}
		}

		public string OutageNameWithSiteInParens
		{
			get
			{
				EUnit unt = new EUnit(OtgUntID);
				if (Util.IsNullOrEmpty(unt.UnitNameWithSite)) return OtgName;
				return OtgName + " (" + unt.UnitNameWithSite + ")";
			}
		}
		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string OutageNameErrMsg
		{
			get { return OtgNameErrMsg; }
		}

		public string OutageCouplantBatchErrMsg
		{
			get { return OtgCouplantBatchErrMsg; }
		}

		public string OutageFacPhoneErrMsg
		{
			get { return OtgFacPhoneErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string OutageErrMsg
		{
			get { return OtgErrMsg; }
			set { OtgErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool OutageNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > OtgNameCharLimit)
			{
				OtgNameErrMsg = string.Format("Outage Names cannot exceed {0} characters", OtgNameCharLimit);
				return false;
			}
			else
			{
				OtgNameErrMsg = null;
				return true;
			}
		}

		public bool OutageCouplantBatchLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > OtgCouplantBatchCharLimit)
			{
				OtgCouplantBatchErrMsg = string.Format("Outage Couplant Batches cannot exceed {0} characters", OtgCouplantBatchCharLimit);
				return false;
			}
			else
			{
				OtgCouplantBatchErrMsg = null;
				return true;
			}
		}

		public bool OutageFacPhoneLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > OtgFacPhoneCharLimit)
			{
				OtgFacPhoneErrMsg = string.Format("Outage FAC Phones cannot exceed {0} characters", OtgFacPhoneCharLimit);
				return false;
			}
			else
			{
				OtgFacPhoneErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool OutageNameValid(string name)
		{
			if (!OutageNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForUnit(name, OtgDBid, (Guid)OtgUntID))
			{
				OtgNameErrMsg = "That Outage Name is already in use.";
				return false;
			}
			OtgNameErrMsg = null;
			return true;
		}

		public bool OutageCouplantBatchValid(string value)
		{
			if (!OutageCouplantBatchLengthOk(value)) return false;

			OtgCouplantBatchErrMsg = null;
			return true;
		}

		public bool OutageFacPhoneValid(string value)
		{
			if (!OutageFacPhoneLengthOk(value)) return false;

			OtgFacPhoneErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EOutage()
		{
			this.OtgGridColDefaultCCW = false;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EOutage(Guid? id) : this()
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
				OtgDBid,
				OtgName,
				OtgUntID,
				OtgClpID,
				OtgCptID,
				OtgCouplantBatch,
				OtgFacPhone,
				OtgStartedOn,
				OtgEndedOn,
				OtgConfigExportedOn,
				OtgDataImportedOn,
				OtgGridColDefaultCCW
				from Outages
				where OtgDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				OtgDBid = (Guid?)dr[0];
				OtgName = (string)dr[1];
				OtgUntID = (Guid?)dr[2];
				OtgClpID = (Guid?)Util.NullForDbNull(dr[3]);
				OtgCptID = (Guid?)Util.NullForDbNull(dr[4]);
				OtgCouplantBatch = (string)Util.NullForDbNull(dr[5]);
				OtgFacPhone = (string)Util.NullForDbNull(dr[6]);
				OtgStartedOn = (DateTime?)Util.NullForDbNull(dr[7]);
				OtgEndedOn = (DateTime?)Util.NullForDbNull(dr[8]);
				OtgConfigExportedOn = (DateTime?)Util.NullForDbNull(dr[9]);
				OtgDataImportedOn = (DateTime?)Util.NullForDbNull(dr[10]);
				OtgGridColDefaultCCW = (bool)dr[11];
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

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				OtgDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", OtgDBid),
					new SqlCeParameter("@p1", OtgName),
					new SqlCeParameter("@p2", OtgUntID),
					new SqlCeParameter("@p3", Util.DbNullForNull(OtgClpID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(OtgCptID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(OtgCouplantBatch)),
					new SqlCeParameter("@p6", Util.DbNullForNull(OtgFacPhone)),
					new SqlCeParameter("@p7", Util.DbNullForNull(OtgStartedOn)),
					new SqlCeParameter("@p8", Util.DbNullForNull(OtgEndedOn)),
					new SqlCeParameter("@p9", Util.DbNullForNull(OtgConfigExportedOn)),
					new SqlCeParameter("@p10", Util.DbNullForNull(OtgDataImportedOn)),
					new SqlCeParameter("@p11", OtgGridColDefaultCCW)
					
					});
				cmd.CommandText = @"Insert Into Outages (
					OtgDBid,
					OtgName,
					OtgUntID,
					OtgClpID,
					OtgCptID,
					OtgCouplantBatch,
					OtgFacPhone,
					OtgStartedOn,
					OtgEndedOn,
					OtgConfigExportedOn,
					OtgDataImportedOn,
					OtgGridColDefaultCCW
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Outages row");
				}
				OnAdded(ID);
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", OtgDBid),
					new SqlCeParameter("@p1", OtgName),
					new SqlCeParameter("@p2", OtgUntID),
					new SqlCeParameter("@p3", Util.DbNullForNull(OtgClpID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(OtgCptID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(OtgCouplantBatch)),
					new SqlCeParameter("@p6", Util.DbNullForNull(OtgFacPhone)),
					new SqlCeParameter("@p7", Util.DbNullForNull(OtgStartedOn)),
					new SqlCeParameter("@p8", Util.DbNullForNull(OtgEndedOn)),
					new SqlCeParameter("@p9", Util.DbNullForNull(OtgConfigExportedOn)),
					new SqlCeParameter("@p10", Util.DbNullForNull(OtgDataImportedOn)),
					new SqlCeParameter("@p11", OtgGridColDefaultCCW)});

				cmd.CommandText =
					@"Update Outages 
					set					
					OtgName = @p1,					
					OtgUntID = @p2,					
					OtgClpID = @p3,					
					OtgCptID = @p4,					
					OtgCouplantBatch = @p5,					
					OtgFacPhone = @p6,					
					OtgStartedOn = @p7,					
					OtgEndedOn = @p8,					
					OtgConfigExportedOn = @p9,					
					OtgDataImportedOn = @p10,					
					OtgGridColDefaultCCW = @p11
					Where OtgDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update outages row");
				}
				OnUpdated(ID);
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
			if (!OutageNameValid(OutageName)) return false;
			if (!OutageCouplantBatchValid(OutageCouplantBatch)) return false;
			if (!OutageFacPhoneValid(OutageFacPhone)) return false;

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
			if (OtgDBid == null)
			{
				OutageErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (HasChildren())
			{
				OutageErrMsg = "Unable to delete because this Outage is referenced by Component Reports.";
				return false;
			}

			if (HasDataImported())
			{
				OutageErrMsg = "Unable to delete because data has been imported for this Outage.";
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
					@"Delete from Outages 
					where OtgDBid = @p0";
				cmd.Parameters.Add("@p0", OtgDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					OutageErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					OutageErrMsg = null;
					OnChanged(ID);
					OnDeleted(ID);
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool HasDataImported()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OtgDataImportedOn from Outages
					where OtgDBid = @p0";
			cmd.Parameters.Add("@p0", OtgDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = Util.NullForDbNull(cmd.ExecuteScalar());
			return result != null;
		}

		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select IscDBid from InspectedComponents
					where IscOtgID = @p0";
			cmd.Parameters.Add("@p0", OtgDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = Util.NullForDbNull(cmd.ExecuteScalar());
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of outages
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		public static EOutageCollection ListByName(bool addNoSelection)
		{
			EOutage outage;
			EOutageCollection outages = new EOutageCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				OtgDBid,
				OtgName,
				OtgUntID,
				OtgClpID,
				OtgCptID,
				OtgCouplantBatch,
				OtgFacPhone,
				OtgStartedOn,
				OtgEndedOn,
				OtgConfigExportedOn,
				OtgDataImportedOn,
				OtgGridColDefaultCCW				
				from Outages";

			qry += "	order by OtgName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				outage = new EOutage();
				outage.OtgName = "<No Selection>";
				outages.Add(outage);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				outage = new EOutage((Guid?)dr[0]);
				outage.OtgName = (string)(dr[1]);
				outage.OtgUntID = (Guid?)(dr[2]);
				outage.OtgClpID = (Guid?)Util.NullForDbNull(dr[3]);
				outage.OtgCptID = (Guid?)Util.NullForDbNull(dr[4]);
				outage.OtgCouplantBatch = (string)Util.NullForDbNull(dr[5]);
				outage.OtgFacPhone = (string)Util.NullForDbNull(dr[6]);
				outage.OtgStartedOn = (DateTime?)Util.NullForDbNull(dr[7]);
				outage.OtgEndedOn = (DateTime?)Util.NullForDbNull(dr[8]);
				outage.OtgConfigExportedOn = (DateTime?)Util.NullForDbNull(dr[9]);
				outage.OtgDataImportedOn = (DateTime?)Util.NullForDbNull(dr[10]);
				outage.OtgGridColDefaultCCW = (bool)(dr[11]);
				
				outages.Add(outage);	
			}
			// Finish up
			dr.Close();
			return outages;
		}

		// This helper function builds the collection for you based on the flags you send it
		public static EOutageCollection ListForUnit(Guid UnitID, bool addNoSelection)
		{
			EOutage outage;
			EOutageCollection outages = new EOutageCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				OtgDBid,
				OtgUntID,
				OtgName,
				OtgClpID,
				OtgCptID,
				OtgCouplantBatch,
				OtgFacPhone,
				OtgStartedOn,
				OtgEndedOn,
				OtgConfigExportedOn,
				OtgDataImportedOn,
				OtgGridColDefaultCCW				
				from Outages";

				qry += " where OtgUntID = @p1";

			qry += "	order by OtgName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", UnitID));

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				outage = new EOutage();
				outage.OtgName = "<No Selection>";
				outages.Add(outage);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				outage = new EOutage((Guid?)dr[0]);
				outage.OtgUntID = (Guid?)(dr[1]);
				outage.OtgName = (string)(dr[2]);
				outage.OtgClpID = (Guid?)Util.NullForDbNull(dr[3]);
				outage.OtgCptID = (Guid?)Util.NullForDbNull(dr[4]);
				outage.OtgCouplantBatch = (string)Util.NullForDbNull(dr[5]);
				outage.OtgFacPhone = (string)Util.NullForDbNull(dr[6]);
				outage.OtgStartedOn = (DateTime?)Util.NullForDbNull(dr[7]);
				outage.OtgEndedOn = (DateTime?)Util.NullForDbNull(dr[8]);
				outage.OtgConfigExportedOn = (DateTime?)Util.NullForDbNull(dr[9]);
				outage.OtgDataImportedOn = (DateTime?)Util.NullForDbNull(dr[10]);
				outage.OtgGridColDefaultCCW = (bool)(dr[11]);

				outages.Add(outage);
			}
			// Finish up
			dr.Close();
			return outages;
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
					OtgDBid as ID,
					OtgName as OutageName,
					OtgUntID as OutageUntID,
					OtgClpID as OutageClpID,
					OtgCptID as OutageCptID,
					OtgCouplantBatch as OutageCouplantBatch,
					OtgFacPhone as OutageFacPhone,
					OtgStartedOn as OutageStartedOn,
					OtgEndedOn as OutageEndedOn,
					OtgConfigExportedOn as OutageConfigExportedOn,
					OtgDataImportedOn as OutageDataImportedOn,
					CASE
						WHEN OtgGridColDefaultCCW = 0 THEN 'No'
						ELSE 'Yes'
					END as OutageGridColDefaultCCW
					from Outages";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}
		//--------------------------------------
		// Private utilities
		//--------------------------------------

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExists(string name, Guid? id)
		{
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			if (id == null)
			{
				cmd.CommandText = "Select OtgDBid from Outages where OtgName = @p1";
			}
			else
			{
				cmd.CommandText = "Select OtgDBid from Outages where OtgName = @p1 and OtgDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			bool test = (cmd.ExecuteScalar() != null);
			return test;

		}

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExistsForUnit(string name, Guid? id, Guid unitId)
		{
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			cmd.Parameters.Add(new SqlCeParameter("@p2", unitId));
			if (id == null)
			{
				cmd.CommandText =
					@"Select	OtgDBid from Outages 
					where OtgName = @p1 and OtgUntID = @p2";
			}
			else
			{
				cmd.CommandText =
					@"Select OtgDBid from Outages 
					where OtgName = @p1 and OtgUntID = @p2 and OtgDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			bool test = (cmd.ExecuteScalar() != null);
			return test;

		}


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (OutageName == null)
			{
				OtgNameErrMsg = "An Outage Name is required";
				allFilled = false;
			}
			else
			{
				OtgNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Outage Collection class
	//--------------------------------------
	public class EOutageCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EOutageCollection()
		{ }
		//the indexer of the collection
		public EOutage this[int index]
		{
			get
			{
				return (EOutage)this.List[index];
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
			foreach (EOutage outage in InnerList)
			{
				if (outage.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EOutage item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EOutage item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EOutage item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EOutage item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
