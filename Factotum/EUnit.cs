using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EUnit : IEntity
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
		private Guid? UntDBid;
		private string UntName;
		private Guid? UntSitID;
		private bool UntIsActive;

		// Textbox limits
		public static int UntNameCharLimit = 10;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string UntNameErrMsg;

		// Form level validation message
		private string UntErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return UntDBid; }
		}

		public string UnitName
		{
			get { return UntName; }
			set { UntName = Util.NullifyEmpty(value); }
		}

		public Guid? UnitSitID
		{
			get { return UntSitID; }
			set { UntSitID = value; }
		}

		public bool UnitIsActive
		{
			get { return UntIsActive; }
			set { UntIsActive = value; }
		}

		public string UnitNameWithSite
		{
			get
			{
				if (UntSitID == null || UntName == null) return "";
				ESite site = new ESite(UntSitID);
				return site.SiteName + " " + UntName;
			}
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string UnitNameErrMsg
		{
			get { return UntNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string UnitErrMsg
		{
			get { return UntErrMsg; }
			set { UntErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool UnitNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > UntNameCharLimit)
			{
				UntNameErrMsg = string.Format("Unit Names cannot exceed {0} characters", UntNameCharLimit);
				return false;
			}
			else
			{
				UntNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool UnitNameValid(string name)
		{
			bool existingIsInactive;
			if (!UnitNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForSite(name, UntDBid, (Guid)UntSitID, out existingIsInactive))
			{
				UntNameErrMsg = existingIsInactive ? 
					"A Unit with that Name exists but its status has been set to inactive.":
					"That Unit Name is already in use.";
				return false;
			}
			UntNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EUnit()
		{
			this.UntIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EUnit(Guid? id) : this()
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
				UntDBid,
				UntName,
				UntSitID,
				UntIsActive
				from Units
				where UntDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				UntDBid = (Guid?)dr[0];
				UntName = (string)dr[1];
				UntSitID = (Guid?)dr[2];
				UntIsActive = (bool)dr[3];
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
				UntDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", UntDBid),
					new SqlCeParameter("@p1", UntName),
					new SqlCeParameter("@p2", UntSitID),
					new SqlCeParameter("@p3", UntIsActive)
					});
				cmd.CommandText = @"Insert Into Units (
					UntDBid,
					UntName,
					UntSitID,
					UntIsActive
				) values (@p0,@p1,@p2,@p3)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Units row");
				}
				OnAdded(ID);
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", UntDBid),
					new SqlCeParameter("@p1", UntName),
					new SqlCeParameter("@p2", UntSitID),
					new SqlCeParameter("@p3", UntIsActive)});

				cmd.CommandText =
					@"Update Units 
					set					
					UntName = @p1,					
					UntSitID = @p2,					
					UntIsActive = @p3
					Where UntDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update units row");
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
			if (!UnitNameValid(UnitName)) return false;

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
			if (UntDBid == null)
			{
				UnitErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (HasSystems())
			{
				UnitErrMsg = "Unable to delete because Systems are defined for this Unit.";
				return false;
			}

			if (HasLines())
			{
				UnitErrMsg = "Unable to delete because Lines are defined for this Unit.";
				return false;
			}

			if (HasOutages())
			{
				UnitErrMsg = "Unable to delete because Outages are defined for this Unit.";
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
					@"Delete from Units 
					where UntDBid = @p0";
				cmd.Parameters.Add("@p0", UntDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					UnitErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
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

		private bool HasSystems()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select SysDBid from Systems
					where SysUntID = @p0";
			cmd.Parameters.Add("@p0", UntDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}
		private bool HasLines()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select LinDBid from Lines
					where LinUntID = @p0";
			cmd.Parameters.Add("@p0", UntDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}
		private bool HasOutages()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OtgDBid from Outages
					where OtgUntID = @p0";
			cmd.Parameters.Add("@p0", UntDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}
		//--------------------------------------------------------------------
		// Static listing methods which return collections of units
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EUnitCollection ListByName(bool showinactive, bool addNoSelection)
		{
			EUnit unit;
			EUnitCollection units = new EUnitCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				UntDBid,
				UntName,
				UntSitID,
				UntIsActive
				from Units
				Left Outer Join Sites
				On UntSitID = SitDBid";

			if (!showinactive)
				qry += " where UntIsActive = 1";

			qry += "	order by SitName, UntName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				unit = new EUnit();
				unit.UntName = "<No Selection>";
				units.Add(unit);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				unit = new EUnit((Guid?)dr[0]);
				unit.UntName = (string)(dr[1]);
				unit.UntSitID = (Guid?)(dr[2]);
				unit.UntIsActive = (bool)(dr[3]);

				units.Add(unit);	
			}
			// Finish up
			dr.Close();
			return units;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview, you probably don't.
		public static EUnitCollection ListForSite(Guid SiteID, bool includeUnassigned,
			bool showactive, bool showinactive, bool addNoSelection)
		{
			EUnit unit;
			EUnitCollection units = new EUnitCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				UntDBid,
				UntSitID,
				UntName,
				UntIsActive
				from Units";
			if (includeUnassigned)
				qry += " where (UntSitID is Null or UntSitID = @p1)";
			else
				qry += " where UntSitID = @p1";

			if (showactive && !showinactive)
				qry += " where UntIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where UntIsActive = 0";

			qry += "	order by UntName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", SiteID));

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				unit = new EUnit();
				unit.UntName = "<No Selection>";
				units.Add(unit);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				unit = new EUnit((Guid?)dr[0]);
				unit.UntSitID = (Guid?)(dr[1]);
				unit.UntName = (string)(dr[2]);
				unit.UntIsActive = (bool)(dr[3]);

				units.Add(unit);
			}
			// Finish up
			dr.Close();
			return units;
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
					UntDBid as ID,
					UntName as UnitName,
					UntSitID as UnitSitID,
					CASE
						WHEN UntIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as UnitIsActive
					from Units";
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
				cmd.CommandText = "Select UntDBid from Units where UntName = @p1";
			}
			else
			{
				cmd.CommandText = "Select UntDBid from Units where UntName = @p1 and UntDBid != @p0";
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
		private bool NameExistsForSite(string name, Guid? id, Guid siteId, 
			out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			cmd.Parameters.Add(new SqlCeParameter("@p2", siteId));
			if (id == null)
			{
				cmd.CommandText =
					@"Select	UntIsActive from Units 
					where UntName = @p1 and UntSitID = @p2";
			}
			else
			{
				cmd.CommandText =
					@"Select UntIsActive from Units 
					where UntName = @p1 and UntSitID = @p2 and UntDBid != @p0";
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

			if (UnitName == null)
			{
				UntNameErrMsg = "A Unit Name is required.";
				allFilled = false;
			}
			else
			{
				UntNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Unit Collection class
	//--------------------------------------
	public class EUnitCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EUnitCollection()
		{ }
		//the indexer of the collection
		public EUnit this[int index]
		{
			get
			{
				return (EUnit)this.List[index];
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
			foreach (EUnit unit in InnerList)
			{
				if (unit.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EUnit item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EUnit item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EUnit item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EUnit item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
