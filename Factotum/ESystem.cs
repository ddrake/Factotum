using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ESystem : IEntity
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
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? SysDBid;
		private string SysName;
		private Guid? SysUntID;
		private bool SysIsLclChg;
		private bool SysIsActive;

		// Textbox limits
		public static int SysNameCharLimit = 40;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string SysNameErrMsg;

		// Form level validation message
		private string SysErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return SysDBid; }
		}

		public string SystemName
		{
			get { return SysName; }
			set { SysName = Util.NullifyEmpty(value); }
		}

		public Guid? SystemUntID
		{
			get { return SysUntID; }
			set { SysUntID = value; }
		}

		public bool SystemIsLclChg
		{
			get { return SysIsLclChg; }
			set { SysIsLclChg = value; }
		}

		public bool SystemIsActive
		{
			get { return SysIsActive; }
			set { SysIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string SystemNameErrMsg
		{
			get { return SysNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string SystemErrMsg
		{
			get { return SysErrMsg; }
			set { SysErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool SystemNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > SysNameCharLimit)
			{
				SysNameErrMsg = string.Format("System Names cannot exceed {0} characters", SysNameCharLimit);
				return false;
			}
			else
			{
				SysNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool SystemNameValid(string name)
		{
			bool existingIsInactive;
			if (!SystemNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForUnit(name, SysDBid, (Guid)SysUntID, out existingIsInactive))
			{
				SysNameErrMsg = existingIsInactive ?
					"A System with that Name exists but its status has been set to inactive." :
					"That System Name is already in use.";
				return false;
			}
			SysNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ESystem()
		{
			this.SysIsLclChg = false;
			this.SysIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ESystem(Guid? id) : this()
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
				SysDBid,
				SysName,
				SysUntID,
				SysIsLclChg,
				SysIsActive
				from Systems
				where SysDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				SysDBid = (Guid?)dr[0];
				SysName = (string)dr[1];
				SysUntID = (Guid?)dr[2];
				SysIsLclChg = (bool)dr[3];
				SysIsActive = (bool)dr[4];
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
				if (!Globals.IsMasterDB) SysIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				SysDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", SysDBid),
					new SqlCeParameter("@p1", SysName),
					new SqlCeParameter("@p2", SysUntID),
					new SqlCeParameter("@p3", SysIsLclChg),
					new SqlCeParameter("@p4", SysIsActive)
					});
				cmd.CommandText = @"Insert Into Systems (
					SysDBid,
					SysName,
					SysUntID,
					SysIsLclChg,
					SysIsActive
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Systems row");
				}
				OnAdded(ID);
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", SysDBid),
					new SqlCeParameter("@p1", SysName),
					new SqlCeParameter("@p2", SysUntID),
					new SqlCeParameter("@p3", SysIsLclChg),
					new SqlCeParameter("@p4", SysIsActive)});

				cmd.CommandText =
					@"Update Systems 
					set					
					SysName = @p1,					
					SysUntID = @p2,					
					SysIsLclChg = @p3,					
					SysIsActive = @p4
					Where SysDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update systems row");
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
			if (!SystemNameValid(SystemName)) return false;

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
			if (SysDBid == null)
			{
				SystemErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!SysIsLclChg && !Globals.IsMasterDB)
			{
				SystemErrMsg = "Unable to delete because this System was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (HasChildren())
			{
				SystemErrMsg = "Unable to delete because this System is referenced by components.";
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
					@"Delete from Systems 
					where SysDBid = @p0";
				cmd.Parameters.Add("@p0", SysDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					SystemErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					SystemErrMsg = null;
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

		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select CmpDBid from Components
					where CmpSysID = @p0";
			cmd.Parameters.Add("@p0", SysDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of systems
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ESystemCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ESystem system;
			ESystemCollection systems = new ESystemCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				SysDBid,
				SysName,
				SysUntID,
				SysIsLclChg,
				SysIsActive
				from Systems";
			if (showactive && !showinactive)
				qry += " where SysIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where SysIsActive = 0";

			qry += "	order by SysName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				system = new ESystem();
				system.SysName = "<No Selection>";
				systems.Add(system);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				system = new ESystem((Guid?)dr[0]);
				system.SysName = (string)(dr[1]);
				system.SysUntID = (Guid?)(dr[2]);
				system.SysIsLclChg = (bool)(dr[3]);
				system.SysIsActive = (bool)(dr[4]);

				systems.Add(system);	
			}
			// Finish up
			dr.Close();
			return systems;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview, you probably don't.
		public static ESystemCollection ListForUnit(Guid? UnitID, 
			bool showinactive, bool addNoSelection)
		{
			ESystem system;
			ESystemCollection systems = new ESystemCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				system = new ESystem();
				system.SysName = "<No Selection>";
				systems.Add(system);
			}
			if (UnitID == null) return systems;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				SysDBid,
				SysName,
				SysUntID,
				SysIsLclChg,
				SysIsActive
				from Systems";
				qry += " where SysUntID = @p1";

			if (!showinactive)
				qry += " and SysIsActive = 1";

			qry += "	order by SysName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", UnitID));

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				system = new ESystem((Guid?)dr[0]);
				system.SysName = (string)(dr[1]);
				system.SysUntID = (Guid?)(dr[2]);
				system.SysIsLclChg = (bool)(dr[3]);
				system.SysIsActive = (bool)(dr[4]);

				systems.Add(system);
			}
			// Finish up
			dr.Close();
			return systems;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataViewForUnit(Guid? UnitID)
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
					SysDBid as ID,
					SysName as SystemName,
					CASE
						WHEN SysIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as SystemIsActive
					from Systems
					where SysUntID = @p1";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", UnitID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}
		public static ESystem FindForSystemName(string SystemName)
		{
			if (Util.IsNullOrEmpty(SystemName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", SystemName));
			cmd.CommandText = "Select SysDBid from Systems where SysName = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new ESystem((Guid)val);
			else return null;
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
				cmd.CommandText = "Select SysDBid from Systems where SysName = @p1";
			}
			else
			{
				cmd.CommandText = "Select SysDBid from Systems where SysName = @p1 and SysDBid != @p0";
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
		public static bool NameExistsForUnit(string name, Guid? id, Guid unitId,
			out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			cmd.Parameters.Add(new SqlCeParameter("@p2", unitId));
			if (id == null)
			{
				cmd.CommandText =
					@"Select	SysIsActive from Systems 
					where SysName = @p1 and SysUntID = @p2";
			}
			else
			{
				cmd.CommandText =
					@"Select SysIsActive from Systems 
					where SysName = @p1 and SysUntID = @p2 and SysDBid != @p0";
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

			if (SystemName == null)
			{
				SysNameErrMsg = "A System Name is required";
				allFilled = false;
			}
			else
			{
				SysNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// System Collection class
	//--------------------------------------
	public class ESystemCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ESystemCollection()
		{ }
		//the indexer of the collection
		public ESystem this[int index]
		{
			get
			{
				return (ESystem)this.List[index];
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
			foreach (ESystem system in InnerList)
			{
				if (system.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ESystem item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ESystem item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ESystem item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ESystem item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
