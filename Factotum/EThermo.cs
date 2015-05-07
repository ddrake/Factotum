using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EThermo : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler ThermoKitAssignmentsChanged;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnThermoKitAssignmentsChanged()
		{
			EventHandler temp = ThermoKitAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}

		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? ThmDBid;
		private string ThmSerialNumber;
		private Guid? ThmKitID;
		private bool ThmUsedInOutage;
		private bool ThmIsLclChg;
		private bool ThmIsActive;

		// Textbox limits
		public static int ThmSerialNumberCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string ThmSerialNumberErrMsg;

		// Form level validation message
		private string ThmErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return ThmDBid; }
		}

		public string ThermoSerialNumber
		{
			get { return ThmSerialNumber; }
			set { ThmSerialNumber = Util.NullifyEmpty(value); }
		}

		public Guid? ThermoKitID
		{
			get { return ThmKitID; }
			set { ThmKitID = value; }
		}

		public bool ThermoUsedInOutage
		{
			get { return ThmUsedInOutage; }
			set { ThmUsedInOutage = value; }
		}

		public bool ThermoIsLclChg
		{
			get { return ThmIsLclChg; }
			set { ThmIsLclChg = value; }
		}

		public bool ThermoIsActive
		{
			get { return ThmIsActive; }
			set { ThmIsActive = value; }
		}

		public string ThermoNameWithStatus
		{
			get { return ThmSerialNumber == null ? null : ThmSerialNumber + (ThmIsActive ? "" : " (inactive)"); }
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string ThermoSerialNumberErrMsg
		{
			get { return ThmSerialNumberErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string ThermoErrMsg
		{
			get { return ThmErrMsg; }
			set { ThmErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool ThermoSerialNumberLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ThmSerialNumberCharLimit)
			{
				ThmSerialNumberErrMsg = string.Format("Thermometer Serial Numbers cannot exceed {0} characters", ThmSerialNumberCharLimit);
				return false;
			}
			else
			{
				ThmSerialNumberErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool ThermoSerialNumberValid(string value)
		{
			if (!ThermoSerialNumberLengthOk(value)) return false;

			ThmSerialNumberErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EThermo()
		{
			this.ThmUsedInOutage = false;
			this.ThmIsLclChg = false;
			this.ThmIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EThermo(Guid? id) : this()
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
				ThmDBid,
				ThmSerialNumber,
				ThmKitID,
				ThmUsedInOutage,
				ThmIsLclChg,
				ThmIsActive
				from Thermos
				where ThmDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				ThmDBid = (Guid?)dr[0];
				ThmSerialNumber = (string)dr[1];
				ThmKitID = (Guid?)Util.NullForDbNull(dr[2]);
				ThmUsedInOutage = (bool)dr[3];
				ThmIsLclChg = (bool)dr[4];
				ThmIsActive = (bool)dr[5];
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
			if (!ThmIsActive) ThmKitID = null;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) ThmIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				ThmDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ThmDBid),
					new SqlCeParameter("@p1", ThmSerialNumber),
					new SqlCeParameter("@p2", Util.DbNullForNull(ThmKitID)),
					new SqlCeParameter("@p3", ThmUsedInOutage),
					new SqlCeParameter("@p4", ThmIsLclChg),
					new SqlCeParameter("@p5", ThmIsActive)
					});
				cmd.CommandText = @"Insert Into Thermos (
					ThmDBid,
					ThmSerialNumber,
					ThmKitID,
					ThmUsedInOutage,
					ThmIsLclChg,
					ThmIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Thermometer row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ThmDBid),
					new SqlCeParameter("@p1", ThmSerialNumber),
					new SqlCeParameter("@p2", Util.DbNullForNull(ThmKitID)),
					new SqlCeParameter("@p3", ThmUsedInOutage),
					new SqlCeParameter("@p4", ThmIsLclChg),
					new SqlCeParameter("@p5", ThmIsActive)});

				cmd.CommandText =
					@"Update Thermos 
					set					
					ThmSerialNumber = @p1,					
					ThmKitID = @p2,					
					ThmUsedInOutage = @p3,					
					ThmIsLclChg = @p4,					
					ThmIsActive = @p5
					Where ThmDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update Thermometer row");
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
			if (!ThermoSerialNumberValid(ThermoSerialNumber)) return false;

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
			if (ThmDBid == null)
			{
				ThermoErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (ThmUsedInOutage)
			{
				ThermoErrMsg = "Unable to delete this Thermometer because it has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!ThmIsLclChg && !Globals.IsMasterDB)
			{
				ThermoErrMsg = "Unable to delete because this Thermometer was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (HasChildren())
			{
				ThermoErrMsg = "Unable to delete because this Thermometer is referenced by datasets.";
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
					@"Delete from Thermos 
					where ThmDBid = @p0";
				cmd.Parameters.Add("@p0", ThmDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					ThermoErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					ThermoErrMsg = null;
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
					where DstThmID = @p0";
			cmd.Parameters.Add("@p0", ThmDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of thermos
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EThermoCollection ListByName(
			bool showinactive, bool addNoSelection)
		{
			EThermo thermo;
			EThermoCollection thermos = new EThermoCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				ThmDBid,
				ThmSerialNumber,
				ThmKitID,
				ThmUsedInOutage,
				ThmIsLclChg,
				ThmIsActive
				from Thermos";
			if (!showinactive)
				qry += " where ThmIsActive = 1";

			qry += "	order by ThmSerialNumber";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				thermo = new EThermo();
				thermo.ThmSerialNumber = "<No Selection>";
				thermos.Add(thermo);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				thermo = new EThermo((Guid?)dr[0]);
				thermo.ThmSerialNumber = (string)(dr[1]);
				thermo.ThmKitID = (Guid?)Util.NullForDbNull(dr[2]);
				thermo.ThmUsedInOutage = (bool)(dr[3]);
				thermo.ThmIsLclChg = (bool)(dr[4]);
				thermo.ThmIsActive = (bool)(dr[5]);

				thermos.Add(thermo);
			}
			// Finish up
			dr.Close();
			return thermos;
		}

		public static EThermoCollection ListForKit(Guid? kitID, bool addNoSelection)
		{
			EThermo thermo;
			EThermoCollection thermos = new EThermoCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				thermo = new EThermo();
				thermo.ThmSerialNumber = "<No Selection>";
				thermos.Add(thermo);
			}
			if (kitID != null)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				string qry = @"Select 

				ThmDBid,
				ThmSerialNumber,
				ThmKitID,
				ThmUsedInOutage,
				ThmIsLclChg,
				ThmIsActive
				from Thermos";
				qry += " where ThmKitID = @p1";

				qry += "	order by ThmSerialNumber";
				cmd.CommandText = qry;
				cmd.Parameters.Add("@p1", kitID);

				SqlCeDataReader dr;
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				dr = cmd.ExecuteReader();

				// Build new objects and add them to the collection
				while (dr.Read())
				{
					thermo = new EThermo((Guid?)dr[0]);
					thermo.ThmSerialNumber = (string)(dr[1]);
					thermo.ThmKitID = (Guid?)Util.NullForDbNull(dr[2]);
					thermo.ThmUsedInOutage = (bool)(dr[3]);
					thermo.ThmIsLclChg = (bool)(dr[4]);
					thermo.ThmIsActive = (bool)(dr[5]);

					thermos.Add(thermo);
				}
				// Finish up
				dr.Close();
			}
			return thermos;
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
					ThmDBid as ID,
					ThmSerialNumber as ThermoSerialNumber,
					KitName as ThermoKitName,
					CASE
						WHEN ThmUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as ThermoUsedInOutage,
					CASE
						WHEN ThmIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as ThermoIsLclChg,
					CASE
						WHEN ThmIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as ThermoIsActive
					from Thermos
					left outer join Kits on ThmKitID = KitDBid";
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
				ThmDBid as ID,
				ThmSerialNumber as ThermoSerialNumber,
				ThmIsActive as ThermoIsActive,
				CASE
				WHEN ThmKitID is null THEN 0 ELSE 1
				END as IsAssigned
				from Thermos left join Kits on ThmKitID = KitDBid
				where (ThmKitID is NULL or ThmKitID = @p1)";

			if (!includeInactive) qry += " and ThmIsActive = 1";

			qry += " order by ThmSerialNumber";

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
			cmd.CommandText = @"Update Thermos set ThmKitID = NULL where ThmKitID = @p1";
			cmd.Parameters.Add("@p1", KitID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Then add the selected ducers back in
			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", KitID);
			cmd.Parameters.Add("@p2", "");
			cmd.CommandText =
@"				Update Thermos set ThmKitID = @p1 
				where ThmDBid = @p2";

			// Now insert the current ducer assignments
			for (int dmRow = 0; dmRow < assignments.Rows.Count; dmRow++)
			{
				if (Convert.ToBoolean(assignments.Rows[dmRow]["IsAssigned"]))
				{
					cmd.Parameters["@p2"].Value = (Guid)assignments.Rows[dmRow]["ID"];
					if (cmd.ExecuteNonQuery() != 1)
					{
						throw new Exception("Unable to assign Thermometer to Kit");
					}
				}
			}
			EThermo thm = new EThermo();
			thm.OnThermoKitAssignmentsChanged();
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
				cmd.CommandText = "Select ThmIsActive from Thermos where ThmName = @p1";
			}
			else
			{
				cmd.CommandText = "Select ThmIsActive from Thermos where ThmName = @p1 and ThmDBid != @p0";
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

			if (ThermoSerialNumber == null)
			{
				ThmSerialNumberErrMsg = "A Thermometer Serial Number is required";
				allFilled = false;
			}
			else
			{
				ThmSerialNumberErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Thermo Collection class
	//--------------------------------------
	public class EThermoCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EThermoCollection()
		{ }
		//the indexer of the collection
		public EThermo this[int index]
		{
			get
			{
				return (EThermo)this.List[index];
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
			foreach (EThermo thermo in InnerList)
			{
				if (thermo.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EThermo item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EThermo item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EThermo item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EThermo item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
