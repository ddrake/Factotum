using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EMeterModel : IEntity
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
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? MmlDBid;
		private string MmlName;
		private string MmlManfName;
		private bool MmlIsLclChg;
		private bool MmlUsedInOutage;
		private bool MmlIsActive;

		// Textbox limits
		public static int MmlNameCharLimit = 50;
		public static int MmlManfNameCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string MmlNameErrMsg;
		private string MmlManfNameErrMsg;

		// Form level validation message
		private string MmlErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return MmlDBid; }
		}

		public string MeterModelName
		{
			get { return MmlName; }
			set { MmlName = Util.NullifyEmpty(value); }
		}

		public string MeterModelManfName
		{
			get { return MmlManfName; }
			set { MmlManfName = Util.NullifyEmpty(value); }
		}

		public bool MeterModelIsLclChg
		{
			get { return MmlIsLclChg; }
			set { MmlIsLclChg = value; }
		}

		public bool MeterModelUsedInOutage
		{
			get { return MmlUsedInOutage; }
			set { MmlUsedInOutage = value; }
		}

		public bool MeterModelIsActive
		{
			get { return MmlIsActive; }
			set { MmlIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string MeterModelNameErrMsg
		{
			get { return MmlNameErrMsg; }
		}

		public string MeterModelManfNameErrMsg
		{
			get { return MmlManfNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string MeterModelErrMsg
		{
			get { return MmlErrMsg; }
			set { MmlErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool MeterModelNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MmlNameCharLimit)
			{
				MmlNameErrMsg = string.Format("Meter Model Names cannot exceed {0} characters", MmlNameCharLimit);
				return false;
			}
			else
			{
				MmlNameErrMsg = null;
				return true;
			}
		}

		public bool MeterModelManfNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MmlManfNameCharLimit)
			{
				MmlManfNameErrMsg = string.Format("Meter Model Manufacturer Names cannot exceed {0} characters", MmlManfNameCharLimit);
				return false;
			}
			else
			{
				MmlManfNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool MeterModelNameValid(string name)
		{
			bool existingIsInactive;
			if (!MeterModelNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, MmlDBid, out existingIsInactive))
			{
				MmlNameErrMsg = existingIsInactive ?
					"A Meter Model with that Name exists but its status has been set to inactive." :
					"That Meter Model Name is already in use.";
				return false;
			}
			MmlNameErrMsg = null;
			return true;
		}

		public bool MeterModelManfNameValid(string value)
		{
			if (!MeterModelManfNameLengthOk(value)) return false;

			MmlManfNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EMeterModel()
		{
			this.MmlIsLclChg = false;
			this.MmlUsedInOutage = true;
			this.MmlIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EMeterModel(Guid? id) : this()
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
				MmlDBid,
				MmlName,
				MmlManfName,
				MmlIsLclChg,
				MmlUsedInOutage,
				MmlIsActive
				from MeterModels
				where MmlDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				MmlDBid = (Guid?)dr[0];
				MmlName = (string)dr[1];
				MmlManfName = (string)dr[2];
				MmlIsLclChg = (bool)dr[3];
				MmlUsedInOutage = (bool)dr[4];
				MmlIsActive = (bool)dr[5];
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
				if (!Globals.IsMasterDB) MmlIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				MmlDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", MmlDBid),
					new SqlCeParameter("@p1", MmlName),
					new SqlCeParameter("@p2", MmlManfName),
					new SqlCeParameter("@p3", MmlIsLclChg),	
					new SqlCeParameter("@p4", MmlUsedInOutage),
					new SqlCeParameter("@p5", MmlIsActive)
					});
				cmd.CommandText = @"Insert Into MeterModels (
					MmlDBid,
					MmlName,
					MmlManfName,
					MmlIsLclChg,
					MmlUsedInOutage,
					MmlIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Meter Models row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", MmlDBid),
					new SqlCeParameter("@p1", MmlName),
					new SqlCeParameter("@p2", MmlManfName),
					new SqlCeParameter("@p3", MmlIsLclChg),
					new SqlCeParameter("@p4", MmlUsedInOutage),			
					new SqlCeParameter("@p5", MmlIsActive)});

				cmd.CommandText =
					@"Update MeterModels 
					set					
					MmlName = @p1,					
					MmlManfName = @p2,					
					MmlIsLclChg = @p3,	
					MmlUsedInOutage = @p4,
					MmlIsActive = @p5
					Where MmlDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update meter models row");
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
			if (!MeterModelNameValid(MeterModelName)) return false;
			if (!MeterModelManfNameValid(MeterModelManfName)) return false;

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
			if (MmlDBid == null)
			{
				MeterModelErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!MmlIsLclChg && !Globals.IsMasterDB)
			{
				MeterModelErrMsg = "Unable to delete because this Meter Model was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (HasChildren())
			{
				MeterModelErrMsg = "Unable to delete because this Meter Model is referenced by Meters.";
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
					@"Delete from MeterModels 
					where MmlDBid = @p0";
				cmd.Parameters.Add("@p0", MmlDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					MeterModelErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					MeterModelErrMsg = null;
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
				@"Select MtrDBid from Meters
					where MtrMmlID = @p0";
			cmd.Parameters.Add("@p0", MmlDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of metermodels
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EMeterModelCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			EMeterModel metermodel;
			EMeterModelCollection metermodels = new EMeterModelCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				MmlDBid,
				MmlName,
				MmlManfName,
				MmlIsLclChg,
				MmlUsedInOutage,
				MmlIsActive
				from MeterModels";
			if (showactive && !showinactive)
				qry += " where MmlIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where MmlIsActive = 0";

			qry += "	order by MmlName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				metermodel = new EMeterModel();
				metermodel.MmlName = "<No Selection>";
				metermodels.Add(metermodel);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				metermodel = new EMeterModel((Guid?)dr[0]);
				metermodel.MmlName = (string)(dr[1]);
				metermodel.MmlManfName = (string)(dr[2]);
				metermodel.MmlIsLclChg = (bool)(dr[3]);
				metermodel.MmlUsedInOutage = (bool)(dr[4]);				
				metermodel.MmlIsActive = (bool)(dr[5]);

				metermodels.Add(metermodel);	
			}
			// Finish up
			dr.Close();
			return metermodels;
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
					MmlDBid as ID,
					MmlName as MeterModelName,
					MmlManfName as MeterModelManfName,
					CASE
						WHEN MmlIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as MeterModelIsLclChg,
					CASE
						WHEN MmlUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as MeterModelUsedInOutage,
					CASE
						WHEN MmlIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as MeterModelIsActive
					from MeterModels";
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
		private bool NameExists(string name, Guid? id, out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			if (id == null)
			{
				cmd.CommandText = "Select MmlIsActive from MeterModels where MmlName = @p1";
			}
			else
			{
				cmd.CommandText = "Select MmlIsActive from MeterModels where MmlName = @p1 and MmlDBid != @p0";
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

			if (MeterModelName == null)
			{
				MmlNameErrMsg = "A unique Meter Model Name is required";
				allFilled = false;
			}
			else
			{
				MmlNameErrMsg = null;
			}
			if (MeterModelManfName == null)
			{
				MmlManfNameErrMsg = "A Meter Model Manufacturer Name is required";
				allFilled = false;
			}
			else
			{
				MmlManfNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// MeterModel Collection class
	//--------------------------------------
	public class EMeterModelCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EMeterModelCollection()
		{ }
		//the indexer of the collection
		public EMeterModel this[int index]
		{
			get
			{
				return (EMeterModel)this.List[index];
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
			foreach (EMeterModel metermodel in InnerList)
			{
				if (metermodel.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EMeterModel item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EMeterModel item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EMeterModel item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EMeterModel item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
