using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ELine : IEntity
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
		private Guid? LinDBid;
		private Guid? LinUntID;
		private string LinName;
		private bool LinIsLclChg;
		private bool LinIsActive;

		// Textbox limits
		public static int LinNameCharLimit = 40;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string LinNameErrMsg;

		// Form level validation message
		private string LinErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return LinDBid; }
		}

		public Guid? LineUntID
		{
			get { return LinUntID; }
			set { LinUntID = value; }
		}

		public string LineName
		{
			get { return LinName; }
			set { LinName = Util.NullifyEmpty(value); }
		}

		public bool LineIsLclChg
		{
			get { return LinIsLclChg; }
			set { LinIsLclChg = value; }
		}

		public bool LineIsActive
		{
			get { return LinIsActive; }
			set { LinIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string LineNameErrMsg
		{
			get { return LinNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string LineErrMsg
		{
			get { return LinErrMsg; }
			set { LinErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool LineNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > LinNameCharLimit)
			{
				LinNameErrMsg = string.Format("Line Names cannot exceed {0} characters", LinNameCharLimit);
				return false;
			}
			else
			{
				LinNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool LineNameValid(string name)
		{
			bool existingIsInactive;
			if (!LineNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForUnit(name, LinDBid, (Guid)LinUntID, out existingIsInactive))
			{
				LinNameErrMsg = existingIsInactive ?
					"A Line with that Name exists but its status has been set to inactive." :
					"That Line Name is already in use.";
				return false;
			}
			LinNameErrMsg = null;
			return true;
		}


		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ELine()
		{
			this.LinIsLclChg = false;
			this.LinIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ELine(Guid? id) : this()
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
				LinDBid,
				LinUntID,
				LinName,
				LinIsLclChg,
				LinIsActive
				from Lines
				where LinDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				LinDBid = (Guid?)dr[0];
				LinUntID = (Guid?)dr[1];
				LinName = (string)dr[2];
				LinIsLclChg = (bool)dr[3];
				LinIsActive = (bool)dr[4];
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
				if (!Globals.IsMasterDB) LinIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				LinDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", LinDBid),
					new SqlCeParameter("@p1", LinUntID),
					new SqlCeParameter("@p2", LinName),
					new SqlCeParameter("@p3", LinIsLclChg),
					new SqlCeParameter("@p4", LinIsActive)
					});
				cmd.CommandText = @"Insert Into Lines (
					LinDBid,
					LinUntID,
					LinName,
					LinIsLclChg,
					LinIsActive
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Lines row");
				}
				OnAdded(ID);
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", LinDBid),
					new SqlCeParameter("@p1", LinUntID),
					new SqlCeParameter("@p2", LinName),
					new SqlCeParameter("@p3", LinIsLclChg),
					new SqlCeParameter("@p4", LinIsActive)});

				cmd.CommandText =
					@"Update Lines 
					set					
					LinUntID = @p1,					
					LinName = @p2,					
					LinIsLclChg = @p3,					
					LinIsActive = @p4
					Where LinDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update lines row");
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
			if (!LineNameValid(LineName)) return false;

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
			if (LinDBid == null)
			{
				LineErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!LinIsLclChg && !Globals.IsMasterDB)
			{
				LineErrMsg = "Unable to delete because this Line was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (HasChildren())
			{
				LineErrMsg = "Unable to delete because this Line is referenced by components.";
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
					@"Delete from Lines 
					where LinDBid = @p0";
				cmd.Parameters.Add("@p0", LinDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					LineErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					LineErrMsg = null;
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
					where CmpLinID = @p0";
			cmd.Parameters.Add("@p0", LinDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of lines
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ELineCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ELine line;
			ELineCollection lines = new ELineCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			string qry = @"Select 

				LinDBid,
				LinUntID,
				LinName,
				LinIsLclChg,
				LinIsActive
				from Lines";
			if (showactive && !showinactive)
				qry += " where LinIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where LinIsActive = 0";

			qry += "	order by LinName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				line = new ELine();
				line.LinName = "<No Selection>";
				lines.Add(line);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				line = new ELine((Guid?)dr[0]);
				line.LinUntID = (Guid?)(dr[1]);
				line.LinName = (string)(dr[2]);
				line.LinIsLclChg = (bool)(dr[3]);
				line.LinIsActive = (bool)(dr[4]);

				lines.Add(line);	
			}
			// Finish up
			dr.Close();
			return lines;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview, you probably don't.
		public static ELineCollection ListForUnit(Guid? UnitID, 
			bool showinactive, bool addNoSelection)
		{
			ELine line;
			ELineCollection lines = new ELineCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				line = new ELine();
				line.LinName = "<No Selection>";
				lines.Add(line);
			}

			if (UnitID == null) return lines;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				LinDBid,
				LinName,
				LinUntID,
				LinIsLclChg,
				LinIsActive
				from Lines";
				qry += " where LinUntID = @p1";

			if (!showinactive)
				qry += " and LinIsActive = 1";

			qry += "	order by LinName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", UnitID));

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				line = new ELine((Guid?)dr[0]);
				line.LinName = (string)(dr[1]);
				line.LinUntID = (Guid?)(dr[2]);
				line.LinIsLclChg = (bool)(dr[3]);
				line.LinIsActive = (bool)(dr[4]);

				lines.Add(line);
			}
			// Finish up
			dr.Close();
			return lines;
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
					LinDBid as ID,
					LinName as LineName,
					CASE
						WHEN LinIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as LineIsActive
					from Lines
					where LinUntID = @p1";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", UnitID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static ELine FindForLineName(string LineName)
		{
			if (Util.IsNullOrEmpty(LineName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", LineName));
			cmd.CommandText = "Select LinDBid from Lines where LinName = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new ELine((Guid)val);
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
				cmd.CommandText = "Select LinDBid from Lines where LinName = @p1";
			}
			else
			{
				cmd.CommandText = "Select LinDBid from Lines where LinName = @p1 and LinDBid != @p0";
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
					@"Select	LinIsActive from Lines 
					where LinName = @p1 and LinUntID = @p2";
			}
			else
			{
				cmd.CommandText =
					@"Select LinIsActive from Lines 
					where LinName = @p1 and LinUntID = @p2 and LinDBid != @p0";
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

			if (LineName == null)
			{
				LinNameErrMsg = "A Line Name is required";
				allFilled = false;
			}
			else
			{
				LinNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Line Collection class
	//--------------------------------------
	public class ELineCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ELineCollection()
		{ }
		//the indexer of the collection
		public ELine this[int index]
		{
			get
			{
				return (ELine)this.List[index];
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
			foreach (ELine line in InnerList)
			{
				if (line.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ELine item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ELine item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ELine item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ELine item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
