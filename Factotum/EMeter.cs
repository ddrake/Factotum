using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EMeter : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler MeterKitAssignmentsChanged;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}

		protected virtual void OnMeterKitAssignmentsChanged()
		{
			EventHandler temp = MeterKitAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}

		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? MtrDBid;
		private string MtrSerialNumber;
		private Guid? MtrMmlID;
		private Guid? MtrKitID;
		private DateTime? MtrCalDueDate;
		private bool MtrUsedInOutage;
		private bool MtrIsLclChg;
		private bool MtrIsActive;

		private string MtrModelName;
		private string MtrModelAndSerial;

		// Textbox limits
		public static int MtrSerialNumberCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string MtrSerialNumberErrMsg;
		private string MtrCalDueDateErrMsg;
		private string MtrMmlIDErrMsg;

		// Form level validation message
		private string MtrErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return MtrDBid; }
		}

		public string MeterSerialNumber
		{
			get { return MtrSerialNumber; }
			set
			{
				MtrSerialNumber = Util.NullifyEmpty(value);
				UpdateModelAndSerialNumber();
			}
		}

		public Guid? MeterMmlID
		{
			get { return MtrMmlID; }
			set 
			{ 
				MtrMmlID = value;
				if (MtrMmlID != null)
				{
					EMeterModel mml = new EMeterModel(MtrMmlID);
					MtrModelName = mml.MeterModelName;
				}
				else MtrModelName = null;
				UpdateModelAndSerialNumber();
			}
		}

		public Guid? MeterKitID
		{
			get { return MtrKitID; }
			set { MtrKitID = value; }
		}

		public DateTime? MeterCalDueDate
		{
			get { return MtrCalDueDate; }
			set { MtrCalDueDate = value; }
		}

		public bool MeterUsedInOutage
		{
			get { return MtrUsedInOutage; }
			set { MtrUsedInOutage = value; }
		}

		public bool MeterIsLclChg
		{
			get { return MtrIsLclChg; }
			set { MtrIsLclChg = value; }
		}

		public bool MeterIsActive
		{
			get { return MtrIsActive; }
			set { MtrIsActive = value; }
		}

		public string MeterModelAndSerial
		{
			get { return MtrModelAndSerial; }
		}

		public string MeterModelName
		{
			get 
			{
				if (MtrMmlID != null)
				{
					EMeterModel mml = new EMeterModel(MtrMmlID);
					MtrModelName = mml.MeterModelName;
				}
				else MtrModelName = null;
				return MtrModelName; 
			}
		}

		public string MeterNameWithStatus
		{
			get { return MtrModelAndSerial == null ? null : MtrModelAndSerial + (MtrIsActive ? "" : " (inactive)"); }
		}

		// This should be called whenever either the model or serial number changes
		private void UpdateModelAndSerialNumber()
		{
			if (MtrSerialNumber != null && MtrMmlID != null)
			{
				MtrModelAndSerial = MtrModelName + " S/N: " + MtrSerialNumber;
			}
			else
			{
				MtrModelAndSerial = null;
			}
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string MeterSerialNumberErrMsg
		{
			get { return MtrSerialNumberErrMsg; }
		}

		public string MeterMmlIDErrMsg
		{
			get { return MtrMmlIDErrMsg; }
		}

		public string MeterCalDueDateErrMsg
		{
			get { return MtrCalDueDateErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string MeterErrMsg
		{
			get { return MtrErrMsg; }
			set { MtrErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool MeterSerialNumberLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MtrSerialNumberCharLimit)
			{
				MtrSerialNumberErrMsg = string.Format("Meter Serial Numbers cannot exceed {0} characters", MtrSerialNumberCharLimit);
				return false;
			}
			else
			{
				MtrSerialNumberErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool MeterSerialNumberValid(string value)
		{
			if (!MeterSerialNumberLengthOk(value)) return false;

			MtrSerialNumberErrMsg = null;
			return true;
		}

		public bool MeterCalDueDateValid(DateTime? value)
		{
			// Add some real validation here if needed.
			MtrCalDueDateErrMsg = null;
			return true;
		}


		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EMeter()
		{
			this.MtrUsedInOutage = false;
			this.MtrIsLclChg = false;
			this.MtrIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EMeter(Guid? id) : this()
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
				MtrDBid,
				MtrSerialNumber,
				MtrMmlID,
				MtrKitID,
				MtrCalDueDate,
				MtrUsedInOutage,
				MtrIsLclChg,
				MtrIsActive
				from Meters
				where MtrDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				MtrDBid = (Guid?)dr[0];
				MtrSerialNumber = (string)dr[1];
				MtrMmlID = (Guid?)dr[2];
				MtrKitID = (Guid?)Util.NullForDbNull(dr[3]);
				MtrCalDueDate = (DateTime?)Util.NullForDbNull(dr[4]);
				MtrUsedInOutage = (bool)dr[5];
				MtrIsLclChg = (bool)dr[6];
				MtrIsActive = (bool)dr[7];
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
			if (!MtrIsActive) MtrKitID = null;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) MtrIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				MtrDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", MtrDBid),
					new SqlCeParameter("@p1", MtrSerialNumber),
					new SqlCeParameter("@p2", MtrMmlID),
					new SqlCeParameter("@p3", Util.DbNullForNull(MtrKitID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(MtrCalDueDate)),
					new SqlCeParameter("@p5", MtrUsedInOutage),
					new SqlCeParameter("@p6", MtrIsLclChg),
					new SqlCeParameter("@p7", MtrIsActive)
					});
				cmd.CommandText = @"Insert Into Meters (
					MtrDBid,
					MtrSerialNumber,
					MtrMmlID,
					MtrKitID,
					MtrCalDueDate,
					MtrUsedInOutage,
					MtrIsLclChg,
					MtrIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Meters row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", MtrDBid),
					new SqlCeParameter("@p1", MtrSerialNumber),
					new SqlCeParameter("@p2", MtrMmlID),
					new SqlCeParameter("@p3", Util.DbNullForNull(MtrKitID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(MtrCalDueDate)),
					new SqlCeParameter("@p5", MtrUsedInOutage),
					new SqlCeParameter("@p6", MtrIsLclChg),
					new SqlCeParameter("@p7", MtrIsActive)});

				cmd.CommandText =
					@"Update Meters 
					set					
					MtrSerialNumber = @p1,					
					MtrMmlID = @p2,					
					MtrKitID = @p3,					
					MtrCalDueDate = @p4,					
					MtrUsedInOutage = @p5,					
					MtrIsLclChg = @p6,					
					MtrIsActive = @p7
					Where MtrDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update meters row");
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
			if (!MeterSerialNumberValid(MeterSerialNumber)) return false;
			if (!MeterCalDueDateValid(MeterCalDueDate)) return false;
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
			if (MtrDBid == null)
			{
				MeterErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!MtrIsLclChg && !Globals.IsMasterDB)
			{
				MeterErrMsg = "Unable to delete because this Meter was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (MtrUsedInOutage)
			{
				MeterErrMsg = "Unable to delete this Meter because it has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasChildren())
			{
				MeterErrMsg = "Unable to delete because this Meter is referenced by one or more Datasets.";
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
					@"Delete from Meters 
					where MtrDBid = @p0";
				cmd.Parameters.Add("@p0", MtrDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					MeterErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					MeterErrMsg = null;
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
					where DstMtrID = @p0";
			cmd.Parameters.Add("@p0", MtrDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of meters
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EMeterCollection ListByName(
			bool showinactive, bool addNoSelection)
		{
			EMeter meter;
			EMeterCollection meters = new EMeterCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				MtrDBid,
				MtrSerialNumber,
				MtrMmlID,
				MtrKitID,
				MtrCalDueDate,
				MtrUsedInOutage,
				MtrIsLclChg,
				MtrIsActive,
				MmlName
				from Meters 
				left outer join MeterModels on MtrMmlID = MmlDBid";
			if (!showinactive)
				qry += " where MtrIsActive = 1";

			qry += "	order by MmlName, MtrSerialNumber";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				meter = new EMeter();
				meter.MtrModelAndSerial = "<No Selection>";
				meters.Add(meter);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				meter = new EMeter((Guid?)dr[0]);
				meter.MtrSerialNumber = (string)(dr[1]);
				meter.MtrMmlID = (Guid?)(dr[2]);
				meter.MtrKitID = (Guid?)Util.NullForDbNull(dr[3]);
				meter.MtrCalDueDate = (DateTime?)Util.NullForDbNull(dr[4]);
				meter.MtrUsedInOutage = (bool)(dr[5]);
				meter.MtrIsLclChg = (bool)(dr[6]);
				meter.MtrIsActive = (bool)(dr[7]);
				meter.UpdateModelAndSerialNumber();

				meters.Add(meter);	
			}
			// Finish up
			dr.Close();
			return meters;
		}

		public static EMeterCollection ListForKit(Guid? kitID, bool addNoSelection)
		{
			EMeter meter;
			EMeterCollection meters = new EMeterCollection();
			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				meter = new EMeter();
				meter.MtrModelAndSerial = "<No Selection>";
				meters.Add(meter);
			}
			if (kitID != null)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				string qry = @"Select 

				MtrDBid,
				MtrSerialNumber,
				MtrMmlID,
				MtrKitID,
				MtrCalDueDate,
				MtrUsedInOutage,
				MtrIsLclChg,
				MtrIsActive
				from Meters
				left outer join MeterModels on MtrMmlID = MmlDBid";
				qry += " where MtrKitID = @p1";

				qry += "	order by MmlName, MtrSerialNumber";
				cmd.CommandText = qry;
				cmd.Parameters.Add("@p1", kitID);
				SqlCeDataReader dr;
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				dr = cmd.ExecuteReader();

				// Build new objects and add them to the collection
				while (dr.Read())
				{
					meter = new EMeter((Guid?)dr[0]);
					meter.MtrSerialNumber = (string)(dr[1]);
					meter.MtrMmlID = (Guid?)(dr[2]);
					meter.MtrKitID = (Guid?)Util.NullForDbNull(dr[3]);
					meter.MtrCalDueDate = (DateTime?)Util.NullForDbNull(dr[4]);
					meter.MtrUsedInOutage = (bool)(dr[5]);
					meter.MtrIsLclChg = (bool)(dr[6]);
					meter.MtrIsActive = (bool)(dr[7]);
					meter.UpdateModelAndSerialNumber();

					meters.Add(meter);
				}
				// Finish up
				dr.Close();
			}
			return meters;
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
					MtrDBid as ID,
					MmlName as MeterModelName,
					MtrSerialNumber as MeterSerialNumber,
					KitName as MeterKitName,
					MtrCalDueDate as MeterCalDueDate,
					CASE
						WHEN MtrUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as MeterUsedInOutage,
					CASE
						WHEN MtrIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as MeterIsLclChg,
					CASE
						WHEN MtrIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as MeterIsActive
					from Meters
					inner join MeterModels on MtrMmlID = MmlDBid
					left outer join Kits on MtrKitID = KitDBid";
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
				MtrDBid as ID,
				MtrSerialNumber as MeterSerialNumber,
				MmlName as MeterModelName,
				MtrIsActive as MeterIsActive,
				CASE
				WHEN MtrKitID is null THEN 0 ELSE 1
				END as IsAssigned
				from Meters inner join MeterModels on MtrMmlID = MmlDBid
				left join Kits on MtrKitID = KitDBid
				where (MtrKitID is NULL or MtrKitID = @p1)";

			if (!includeInactive) qry += " and MtrIsActive = 1";

			qry += " order by MmlName, MtrSerialNumber";

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
			cmd.CommandText = @"Update Meters set MtrKitID = NULL where MtrKitID = @p1";
			cmd.Parameters.Add("@p1", KitID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Then add the selected ducers back in
			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", KitID);
			cmd.Parameters.Add("@p2", "");
			cmd.CommandText =
@"				Update Meters set MtrKitID = @p1 
				where MtrDBid = @p2";

			// Now insert the current ducer assignments
			for (int dmRow = 0; dmRow < assignments.Rows.Count; dmRow++)
			{
				if (Convert.ToBoolean(assignments.Rows[dmRow]["IsAssigned"]))
				{
					cmd.Parameters["@p2"].Value = (Guid)assignments.Rows[dmRow]["ID"];
					if (cmd.ExecuteNonQuery() != 1)
					{
						throw new Exception("Unable to assign Meter to Kit");
					}
				}
			}
			EMeter mtr = new EMeter();
			mtr.OnMeterKitAssignmentsChanged();
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
				cmd.CommandText = "Select MtrIsActive from Meters where MtrName = @p1";
			}
			else
			{
				cmd.CommandText = "Select MtrIsActive from Meters where MtrName = @p1 and MtrDBid != @p0";
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

			if (MeterSerialNumber == null)
			{
				MtrSerialNumberErrMsg = "A Meter Serial Number is required";
				allFilled = false;
			}
			else
			{
				MtrSerialNumberErrMsg = null;
			}

			if (MeterMmlID == null)
			{
				MtrMmlIDErrMsg = "A Meter Model is required.  You must add at least one Meter Model first.";
				allFilled = false;
			}
			else
			{
				MtrMmlIDErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Meter Collection class
	//--------------------------------------
	public class EMeterCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EMeterCollection()
		{ }
		//the indexer of the collection
		public EMeter this[int index]
		{
			get
			{
				return (EMeter)this.List[index];
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
			foreach (EMeter meter in InnerList)
			{
				if (meter.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EMeter item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EMeter item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EMeter item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EMeter item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
