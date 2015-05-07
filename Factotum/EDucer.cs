using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EDucer : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler DucerKitAssignmentsChanged;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}

		protected virtual void OnDucerKitAssignmentsChanged()
		{
			EventHandler temp = DucerKitAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}

		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? DcrDBid;
		private string DcrSerialNumber;
		private Guid? DcrDmdID;
		private Guid? DcrKitID;
		private bool DcrUsedInOutage;
		private bool DcrIsLclChg;
		private bool DcrIsActive;

		private string DcrModelAndSerial;

		// Textbox limits
		public static int DcrSerialNumberCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string DcrSerialNumberErrMsg;
		private string DcrUsedInOutageErrMsg;
		private string DcrDmdIDErrMsg;
		// Form level validation message
		private string DcrErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return DcrDBid; }
		}

		public string DucerSerialNumber
		{
			get { return DcrSerialNumber; }
			set { 
				DcrSerialNumber = Util.NullifyEmpty(value);
				UpdateModelAndSerialNumber();
			}
		}

		public Guid? DucerDmdID
		{
			get { return DcrDmdID; }
			set 
			{ 
				DcrDmdID = value;
				UpdateModelAndSerialNumber();
			}
		}

		public Guid? DucerKitID
		{
			get { return DcrKitID; }
			set { DcrKitID = value; }
		}

		public bool DucerUsedInOutage
		{
			get { return DcrUsedInOutage; }
			set { DcrUsedInOutage = value; }
		}

		public bool DucerIsLclChg
		{
			get { return DcrIsLclChg; }
			set { DcrIsLclChg = value; }
		}

		public bool DucerIsActive
		{
			get { return DcrIsActive; }
			set { DcrIsActive = value; }
		}

		public string DucerModelAndSerial
		{
			get { return DcrModelAndSerial; }
		}

		public string DucerNameWithStatus
		{
			get { return DcrModelAndSerial == null ? null : DcrModelAndSerial + (DcrIsActive ? "" : " (inactive)"); }
		}

		public string DucerModelName
		{
			get
			{
				if (DcrDmdID == null) return null;
				EDucerModel mdl = new EDucerModel(DcrDmdID);
				return mdl.DucerModelName;
			}
		}

		public decimal? DucerSize
		{
			get
			{
				if (DcrDmdID == null) return null;
				EDucerModel mdl = new EDucerModel(DcrDmdID);
				return mdl.DucerModelSize;
			}
		}
	
		public decimal? DucerFrequency
		{
			get
			{
				if (DcrDmdID == null) return null;
				EDucerModel mdl = new EDucerModel(DcrDmdID);
				return mdl.DucerModelFrequency;
			}
		}

		// This should be called whenever either the model or serial number changes
		// Trying to improve efficiency for combos, etc...
		private void UpdateModelAndSerialNumber()
		{
			if (DcrSerialNumber != null && DcrDmdID != null)
			{
				EDucerModel dmd = new EDucerModel(DcrDmdID);
				DcrModelAndSerial = dmd.DucerModelName + " S/N: " + DcrSerialNumber;
			}
			else
			{
				DcrModelAndSerial = null;
			}
		}

		// Check if the model of this ducer works with the model of the specified meter.
		public bool DucerIsOkForMeter(Guid? meterID)
		{
			if (meterID == null) return false;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = 
				@"Select MtdMmlID from MeterDucers 
					inner join Meters on MtdMmlID = MtrMmlID 
					where MtdDmdID = @p0 AND MtrDBid = @p1";
			cmd.Parameters.Add(new SqlCeParameter("@p0", DcrDmdID));
			cmd.Parameters.Add(new SqlCeParameter("@p1", meterID));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			return (val != null);

		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string DucerSerialNumberErrMsg
		{
			get { return DcrSerialNumberErrMsg; }
		}

		public string DucerUsedInOutageErrMsg
		{
			get { return DcrUsedInOutageErrMsg; }
		}

		public string DucerDmdIDErrMsg
		{
			get { return DcrDmdIDErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string DucerErrMsg
		{
			get { return DcrErrMsg; }
			set { DcrErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool DucerSerialNumberLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DcrSerialNumberCharLimit)
			{
				DcrSerialNumberErrMsg = string.Format("Transducer Serial Numbers cannot exceed {0} characters", DcrSerialNumberCharLimit);
				return false;
			}
			else
			{
				DcrSerialNumberErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool DucerSerialNumberValid(string value)
		{
			bool existingIsInactive;
			if (!DucerSerialNumberLengthOk(value)) return false;

			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(value, DcrDBid, out existingIsInactive))
			{
				DcrSerialNumberErrMsg = existingIsInactive ?
					"A Transducer  with that Serial Number exists but its status has been set to inactive." :
					"A Transducer  with that Serial Number is already in use.";
				return false;
			}
			DcrSerialNumberErrMsg = null;
			return true;

		}

		public bool DucerUsedInOutageValid(bool value)
		{
			// Add some real validation here if needed.
			DcrUsedInOutageErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EDucer()
		{
			this.DcrUsedInOutage = false;
			this.DcrIsLclChg = false;
			this.DcrIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EDucer(Guid? id) : this()
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
				DcrDBid,
				DcrSerialNumber,
				DcrDmdID,
				DcrKitID,
				DcrUsedInOutage,
				DcrIsLclChg,
				DcrIsActive
				from Ducers
				where DcrDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				DcrDBid = (Guid?)dr[0];
				DcrSerialNumber = (string)dr[1];
				DcrDmdID = (Guid?)dr[2];
				DcrKitID = (Guid?)Util.NullForDbNull(dr[3]);
				DcrUsedInOutage = (bool)dr[4];
				DcrIsLclChg = (bool)dr[5];
				DcrIsActive = (bool)dr[6];
				UpdateModelAndSerialNumber();
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

			// If the status has been set to inactive, make sure the Kit is null
			if (!DcrIsActive) DcrKitID = null;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) DcrIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				DcrDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", DcrDBid),
					new SqlCeParameter("@p1", DcrSerialNumber),
					new SqlCeParameter("@p2", DcrDmdID),
					new SqlCeParameter("@p3", Util.DbNullForNull(DcrKitID)),
					new SqlCeParameter("@p4", DcrUsedInOutage),
					new SqlCeParameter("@p5", DcrIsLclChg),
					new SqlCeParameter("@p6", DcrIsActive)
					});
				cmd.CommandText = @"Insert Into Ducers (
					DcrDBid,
					DcrSerialNumber,
					DcrDmdID,
					DcrKitID,
					DcrUsedInOutage,
					DcrIsLclChg,
					DcrIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Transducer row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", DcrDBid),
					new SqlCeParameter("@p1", DcrSerialNumber),
					new SqlCeParameter("@p2", DcrDmdID),
					new SqlCeParameter("@p3", Util.DbNullForNull(DcrKitID)),
					new SqlCeParameter("@p4", DcrUsedInOutage),
					new SqlCeParameter("@p5", DcrIsLclChg),
					new SqlCeParameter("@p6", DcrIsActive)});

				cmd.CommandText =
					@"Update Ducers 
					set					
					DcrSerialNumber = @p1,					
					DcrDmdID = @p2,					
					DcrKitID = @p3,					
					DcrUsedInOutage = @p4,					
					DcrIsLclChg = @p5,					
					DcrIsActive = @p6
					Where DcrDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update Transducer row");
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
			if (!DucerSerialNumberValid(DucerSerialNumber)) return false;
			if (!DucerUsedInOutageValid(DucerUsedInOutage)) return false;

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
			if (DcrDBid == null)
			{
				DucerErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (DcrUsedInOutage)
			{
				DucerErrMsg = "Unable to delete because this Transducer has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!DcrIsLclChg && !Globals.IsMasterDB)
			{
				DucerErrMsg = "Unable to delete this Transducer because it was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasChildren())
			{
				DucerErrMsg = "Unable to delete because this Transducer is referenced by datasets.";
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
					@"Delete from Ducers 
					where DcrDBid = @p0";
				cmd.Parameters.Add("@p0", DcrDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					DucerErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					DucerErrMsg = null;
					OnChanged(ID); 
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select DstDBid from Dsets
					where DstDcrID = @p0";
			cmd.Parameters.Add("@p0", DcrDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of ducers
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EDucerCollection ListByName(
			bool showinactive, bool addNoSelection)
		{
			EDucer ducer;
			EDucerCollection ducers = new EDucerCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				DcrDBid,
				DcrSerialNumber,
				DcrDmdID,
				DcrKitID,
				DcrUsedInOutage,
				DcrIsLclChg,
				DcrIsActive
				from Ducers
				left outer join DucerModels on DcrDmdID = DmdDBid";
			if (!showinactive)
				qry += " where DcrIsActive = 1";

			qry += "	order by DmdName, DcrSerialNumber";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				ducer = new EDucer();
				ducer.DcrModelAndSerial = "<No Selection>";
				ducers.Add(ducer);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				ducer = new EDucer((Guid?)dr[0]);
				ducer.DcrSerialNumber = (string)(dr[1]);
				ducer.DcrDmdID = (Guid?)(dr[2]);
				ducer.DcrKitID = (Guid?)Util.NullForDbNull(dr[3]);
				ducer.DcrUsedInOutage = (bool)(dr[4]);
				ducer.DcrIsLclChg = (bool)(dr[5]);
				ducer.DcrIsActive = (bool)(dr[6]);
				ducer.UpdateModelAndSerialNumber();

				ducers.Add(ducer);	
			}
			// Finish up
			dr.Close();
			return ducers;
		}

		public static EDucerCollection ListForKit(Guid? kitID, bool addNoSelection)
		{
			EDucer ducer;
			EDucerCollection ducers = new EDucerCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				ducer = new EDucer();
				ducer.DcrModelAndSerial = "<No Selection>";
				ducers.Add(ducer);
			}
			if (kitID != null)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				string qry = @"Select 

				DcrDBid,
				DcrSerialNumber,
				DcrDmdID,
				DcrKitID,
				DcrUsedInOutage,
				DcrIsLclChg,
				DcrIsActive,
				DmdName
				from Ducers
				left outer join DucerModels on DcrDmdID = DmdDBid";
				qry += " where DcrKitID = @p1";

				qry += "	order by DmdName, DcrSerialNumber";
				cmd.CommandText = qry;
				cmd.Parameters.Add("@p1", kitID);

				SqlCeDataReader dr;
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				dr = cmd.ExecuteReader();

				// Build new objects and add them to the collection
				while (dr.Read())
				{
					ducer = new EDucer((Guid?)dr[0]);
					ducer.DcrSerialNumber = (string)(dr[1]);
					ducer.DcrDmdID = (Guid?)(dr[2]);
					ducer.DcrKitID = (Guid?)Util.NullForDbNull(dr[3]);
					ducer.DcrUsedInOutage = (bool)(dr[4]);
					ducer.DcrIsLclChg = (bool)(dr[5]);
					ducer.DcrIsActive = (bool)(dr[6]);
					ducer.UpdateModelAndSerialNumber();

					ducers.Add(ducer);
				}
				// Finish up
				dr.Close();
			}
			return ducers;
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
					DcrDBid as ID,
					DmdName as DucerModelName,
					DcrSerialNumber as DucerSerialNumber,
					KitName as DucerKitName,
					CASE
						WHEN DcrUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as DucerUsedInOutage,
					CASE
						WHEN DcrIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as DucerIsLclChg,
					CASE
						WHEN DcrIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as DucerIsActive
					from Ducers 
					inner join DucerModels on DcrDmdID = DmdDBid
					left outer join Kits on DcrKitID = KitDbid";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static DataTable GetWithAssignmentsForKit(Guid? KitID, bool includeInactive)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			string qry = 
				@"select 
				DcrDBid as ID,
				DcrSerialNumber as DucerSerialNumber,
				DmdName as DucerModelName,
				DcrIsActive as DucerIsActive,
				CASE
				WHEN DcrKitID is null THEN 0 ELSE 1
				END as IsAssigned
				from Ducers inner join DucerModels on DcrDmdID = DmdDBid
				left join Kits on DcrKitID = KitDBid
				where (DcrKitID is NULL or DcrKitID = @p1)";
			
			if (!includeInactive) qry += " and DcrIsActive = 1";

			qry += " order by DmdName, DcrSerialNumber";

			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", KitID == null ? Guid.Empty : KitID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			return ds.Tables[0];
		}

		public static void UpdateAssignmentsToKit(Guid KitID, DataTable assignments)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			// First remove all ducers from the kit
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = @"Update Ducers set DcrKitID = NULL where DcrKitID = @p1";
			cmd.Parameters.Add("@p1", KitID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Then add the selected ducers back in
			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", KitID);
			cmd.Parameters.Add("@p2", "");
			cmd.CommandText =
@"				Update Ducers set DcrKitID = @p1 
				where DcrDBid = @p2";

			// Now insert the current ducer assignments
			for (int dmRow = 0; dmRow < assignments.Rows.Count; dmRow++)
			{
				if (Convert.ToBoolean(assignments.Rows[dmRow]["IsAssigned"]))
				{
					cmd.Parameters["@p2"].Value = (Guid)assignments.Rows[dmRow]["ID"];
					if (cmd.ExecuteNonQuery() != 1)
					{
						throw new Exception("Unable to assign Transducer to Kit");
					}
				}
			}
			EDucer dcr = new EDucer();
			dcr.OnDucerKitAssignmentsChanged();
		}
		//--------------------------------------
		// Private utilities
		//--------------------------------------

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
				cmd.CommandText = "Select DcrIsActive from Ducers where DcrSerialNumber = @p1";
			}
			else
			{
				cmd.CommandText = "Select DcrIsActive from Ducers where DcrSerialNumber = @p1 and DcrDBid != @p0";
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

			if (DucerSerialNumber == null)
			{
				DcrSerialNumberErrMsg = "A Transducer Serial Number is required";
				allFilled = false;
			}
			else
			{
				DcrSerialNumberErrMsg = null;
			}
			if (DucerDmdID == null)
			{
				DcrDmdIDErrMsg = "A Transducer Model is required";
				allFilled = false;
			}
			else
			{
				DcrDmdIDErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Ducer Collection class
	//--------------------------------------
	public class EDucerCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EDucerCollection()
		{ }
		//the indexer of the collection
		public EDucer this[int index]
		{
			get
			{
				return (EDucer)this.List[index];
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
			foreach (EDucer ducer in InnerList)
			{
				if (ducer.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EDucer item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EDucer item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EDucer item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EDucer item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
