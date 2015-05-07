using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EComponentType : IEntity
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
		private Guid? CtpDBid;
		private string CtpName;
		private Guid? CtpSitID;
		private bool CtpIsLclChg;
		private bool CtpIsActive;

		// Textbox limits
		public static int CtpNameCharLimit = 25;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string CtpNameErrMsg;

		// Form level validation message
		private string CtpErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return CtpDBid; }
		}

		public string ComponentTypeName
		{
			get { return CtpName; }
			set { CtpName = Util.NullifyEmpty(value); }
		}

		public Guid? ComponentTypeSitID
		{
			get { return CtpSitID; }
			set { CtpSitID = value; }
		}

		public bool ComponentTypeIsLclChg
		{
			get { return CtpIsLclChg; }
			set { CtpIsLclChg = value; }
		}

		public bool ComponentTypeIsActive
		{
			get { return CtpIsActive; }
			set { CtpIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string ComponentTypeNameErrMsg
		{
			get { return CtpNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string ComponentTypeErrMsg
		{
			get { return CtpErrMsg; }
			set { CtpErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool ComponentTypeNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CtpNameCharLimit)
			{
				CtpNameErrMsg = string.Format("Component Type Names cannot exceed {0} characters", CtpNameCharLimit);
				return false;
			}
			else
			{
				CtpNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool ComponentTypeNameValid(string name)
		{
			bool existingIsInactive;
			if (!ComponentTypeNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForSite(name, CtpDBid, (Guid)CtpSitID, out existingIsInactive))
			{
				CtpNameErrMsg = existingIsInactive ?
					"A Component Type with that name exists but its status has been set to inactive." :
					"That Component Type Name is already in use.";
				return false;
			}
			CtpNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EComponentType()
		{
			this.CtpIsLclChg = false;
			this.CtpIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EComponentType(Guid? id) : this()
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
				CtpDBid,
				CtpName,
				CtpSitID,
				CtpIsLclChg,
				CtpIsActive
				from ComponentTypes
				where CtpDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				CtpDBid = (Guid?)dr[0];
				CtpName = (string)dr[1];
				CtpSitID = (Guid?)dr[2];
				CtpIsLclChg = (bool)dr[3];
				CtpIsActive = (bool)dr[4];
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
				if (!Globals.IsMasterDB) CtpIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				CtpDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CtpDBid),
					new SqlCeParameter("@p1", CtpName),
					new SqlCeParameter("@p2", CtpSitID),
					new SqlCeParameter("@p3", CtpIsLclChg),
					new SqlCeParameter("@p4", CtpIsActive)
					});
				cmd.CommandText = @"Insert Into ComponentTypes (
					CtpDBid,
					CtpName,
					CtpSitID,
					CtpIsLclChg,
					CtpIsActive
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Component Types row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CtpDBid),
					new SqlCeParameter("@p1", CtpName),
					new SqlCeParameter("@p2", CtpSitID),
					new SqlCeParameter("@p3", CtpIsLclChg),
					new SqlCeParameter("@p4", CtpIsActive)});

				cmd.CommandText =
					@"Update ComponentTypes 
					set					
					CtpName = @p1,					
					CtpSitID = @p2,					
					CtpIsLclChg = @p3,					
					CtpIsActive = @p4
					Where CtpDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update component types row");
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
			if (!ComponentTypeNameValid(ComponentTypeName)) return false;

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
			if (CtpDBid == null)
			{
				ComponentTypeErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!CtpIsLclChg && !Globals.IsMasterDB)
			{
				ComponentTypeErrMsg = "Unable to delete because this Component Type was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasChildren())
			{
				ComponentTypeErrMsg = "Unable to delete because components exist with this component type.";
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
					@"Delete from ComponentTypes 
					where CtpDBid = @p0";
				cmd.Parameters.Add("@p0", CtpDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					ComponentTypeErrMsg = "Unable to delete.  Please try again later.";
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
				ComponentTypeErrMsg = null;
				return false;
			}
		}

		// Check whether the current record is referenced by other tables.
		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
				@"Select CmpDBid from Components 
					where CmpCtpID = @p0";
			cmd.Parameters.Add("@p0", CtpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of componenttypes
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EComponentTypeCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			EComponentType componenttype;
			EComponentTypeCollection componenttypes = new EComponentTypeCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CtpDBid,
				CtpName,
				CtpSitID,
				CtpIsLclChg,
				CtpIsActive
				from ComponentTypes";
			if (showactive && !showinactive)
				qry += " where CtpIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where CtpIsActive = 0";

			qry += "	order by CtpName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				componenttype = new EComponentType();
				componenttype.CtpName = "<No Selection>";
				componenttypes.Add(componenttype);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				componenttype = new EComponentType((Guid?)dr[0]);
				componenttype.CtpName = (string)(dr[1]);
				componenttype.CtpSitID = (Guid?)(dr[2]);
				componenttype.CtpIsLclChg = (bool)(dr[3]);
				componenttype.CtpIsActive = (bool)(dr[4]);

				componenttypes.Add(componenttype);	
			}
			// Finish up
			dr.Close();
			return componenttypes;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview or combo box, you probably don't.
		public static EComponentTypeCollection ListForSite(Guid? SiteID, 
			bool showinactive, bool addNoSelection)
		{
			EComponentType componenttype;
			EComponentTypeCollection componenttypes = new EComponentTypeCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				componenttype = new EComponentType();
				componenttype.CtpName = "<No Selection>";
				componenttypes.Add(componenttype);
			}
			if (SiteID == null) return componenttypes;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CtpDBid,
				CtpName,
				CtpSitID,
				CtpIsLclChg,
				CtpIsActive
				from ComponentTypes";
				qry += " where CtpSitID = @p1";

			if (!showinactive)
				qry += " and CtpIsActive = 1";

			qry += "	order by CtpName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", SiteID));

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				componenttype = new EComponentType((Guid?)dr[0]);
				componenttype.CtpName = (string)(dr[1]);
				componenttype.CtpSitID = (Guid?)(dr[2]);
				componenttype.CtpIsLclChg = (bool)(dr[3]);
				componenttype.CtpIsActive = (bool)(dr[4]);

				componenttypes.Add(componenttype);
			}
			// Finish up
			dr.Close();
			return componenttypes;
		}

		public static EComponentType FindForComponentTypeName(string ComponentTypeName)
		{
			if (Util.IsNullOrEmpty(ComponentTypeName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", ComponentTypeName));
			cmd.CommandText = "Select CtpDBid from ComponentTypes where CtpName = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new EComponentType((Guid)val);
			else return null;
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
					CtpDBid as ID,
					CtpName as ComponentTypeName,
					CtpSitID as ComponentTypeSitID,
					CASE
						WHEN CtpIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as ComponentTypeIsLclChg,
					CASE
						WHEN CtpIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as ComponentTypeIsActive
					from ComponentTypes";
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
				cmd.CommandText = "Select CtpIsActive from ComponentTypes where CtpName = @p1";
			}
			else
			{
				cmd.CommandText = "Select CtpIsActive from ComponentTypes where CtpName = @p1 and CtpDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;
			return exists;
		}

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		public static bool NameExistsForSite(string name, Guid? id, Guid siteId,
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
					@"Select CtpIsActive from ComponentTypes
					where LTRIM(RTRIM(CtpName)) = @p1 and CtpSitID = @p2";
			}
			else
			{
				cmd.CommandText =
					@"Select CtpIsActive from ComponentTypes 
					where LTRIM(RTRIM(CtpName)) = @p1 and CtpSitID = @p2 and CtpDBid != @p0";
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

			if (ComponentTypeName == null)
			{
				CtpNameErrMsg = "A unique Component Type Name is required";
				allFilled = false;
			}
			else
			{
				CtpNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// ComponentType Collection class
	//--------------------------------------
	public class EComponentTypeCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EComponentTypeCollection()
		{ }
		//the indexer of the collection
		public EComponentType this[int index]
		{
			get
			{
				return (EComponentType)this.List[index];
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
			foreach (EComponentType componenttype in InnerList)
			{
				if (componenttype.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EComponentType item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EComponentType item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EComponentType item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EComponentType item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
