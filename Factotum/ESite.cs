using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ESite : IEntity
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
		private Guid? SitDBid;
		private string SitName;
		private Guid? SitCstID;
		private Guid? SitClpID;
		private string SitFullName;
		private bool SitIsActive;

		// Textbox limits
		public static int SitNameCharLimit = 20;
		public static int SitFullNameCharLimit = 100;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string SitNameErrMsg;
		private string SitFullNameErrMsg;

		// Form level validation message
		private string SitErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return SitDBid; }
		}

		public string SiteName
		{
			get { return SitName; }
			set { SitName = Util.NullifyEmpty(value); }
		}

		public Guid? SiteCstID
		{
			get { return SitCstID; }
			set { SitCstID = value; }
		}

		public Guid? SiteClpID
		{
			get { return SitClpID; }
			set { SitClpID = value; }
		}

		public string SiteFullName
		{
			get { return SitFullName; }
			set { SitFullName = Util.NullifyEmpty(value); }
		}

		public bool SiteIsActive
		{
			get { return SitIsActive; }
			set { SitIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string SiteNameErrMsg
		{
			get { return SitNameErrMsg; }
		}

		public string SiteFullNameErrMsg
		{
			get { return SitFullNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string SiteErrMsg
		{
			get { return SitErrMsg; }
			set { SitErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool SiteNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > SitNameCharLimit)
			{
				SitNameErrMsg = string.Format("Site Names cannot exceed {0} characters", SitNameCharLimit);
				return false;
			}
			else
			{
				SitNameErrMsg = null;
				return true;
			}
		}

		public bool SiteFullNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > SitFullNameCharLimit)
			{
				SitFullNameErrMsg = string.Format("Site Full Names cannot exceed {0} characters", SitFullNameCharLimit);
				return false;
			}
			else
			{
				SitFullNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool SiteNameValid(string name)
		{
			bool existingIsInactive;
			if (!SiteNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, SitDBid, out existingIsInactive))
			{
				SitNameErrMsg = existingIsInactive ?
					"That Site Name exists but its status has been set to inactive." :
					"That Site Name is already in use.";
				return false;
			}
			SitNameErrMsg = null;
			return true;
		}

		public bool SiteFullNameValid(string value)
		{
			if (!SiteFullNameLengthOk(value)) return false;

			SitFullNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ESite()
		{
			this.SitIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ESite(Guid? id) : this()
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
				SitDBid,
				SitName,
				SitCstID,
				SitClpID,
				SitFullName,
				SitIsActive
				from Sites
				where SitDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				SitDBid = (Guid?)dr[0];
				SitName = (string)dr[1];
				SitCstID = (Guid?)dr[2];
				// For nullable foreign keys, set field to null for dbNull case
				SitClpID = (Guid?)Util.NullForDbNull(dr[3]);
				SitFullName = (string)dr[4];
				SitIsActive = (bool)dr[5];
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
				SitDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", SitDBid),
					new SqlCeParameter("@p1", SitName),
					new SqlCeParameter("@p2", SitCstID),
					new SqlCeParameter("@p3", Util.DbNullForNull(SitClpID)),
					new SqlCeParameter("@p4", SitFullName),
					new SqlCeParameter("@p5", SitIsActive)
					});
				cmd.CommandText = @"Insert Into Sites (
					SitDBid,
					SitName,
					SitCstID,
					SitClpID,
					SitFullName,
					SitIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Sites row");
				}
				OnAdded(ID);
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", SitDBid),
					new SqlCeParameter("@p1", SitName),
					new SqlCeParameter("@p2", SitCstID),
					new SqlCeParameter("@p3", Util.DbNullForNull(SitClpID)),
					new SqlCeParameter("@p4", SitFullName),
					new SqlCeParameter("@p5", SitIsActive)});

				cmd.CommandText =
					@"Update Sites 
					set					
					SitName = @p1,					
					SitCstID = @p2,					
					SitClpID = @p3,					
					SitFullName = @p4,					
					SitIsActive = @p5
					Where SitDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update sites row");
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
			if (!SiteNameValid(SiteName)) return false;
			if (!SiteFullNameValid(SiteFullName)) return false;

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
			if (SitDBid == null)
			{
				SiteErrMsg = "Unable to delete. Record not found.";
				return false;
			}
			if (HasChildren())
			{
				SiteErrMsg = "Unable to delete because units exist for this site.";
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
					@"Delete from Sites 
					where SitDBid = @p0";
				cmd.Parameters.Add("@p0", SitDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					SiteErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					SiteErrMsg = null;
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
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
				@"Select UntDBid from Units 
					where UntSitID = @p0";
			cmd.Parameters.Add("@p0", SitDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of sites
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ESiteCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ESite site;
			ESiteCollection sites = new ESiteCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				SitDBid,
				SitName,
				SitCstID,
				SitClpID,
				SitFullName,
				SitIsActive
				from Sites";
			if (showactive && !showinactive)
				qry += " where SitIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where SitIsActive = 0";

			qry += "	order by SitName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				site = new ESite();
				site.SitName = "<No Selection>";
				sites.Add(site);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				site = new ESite((Guid?)dr[0]);
				site.SitName = (string)(dr[1]);
				site.SitCstID = (Guid?)(dr[2]);
				site.SitClpID = (Guid?)Util.NullForDbNull(dr[3]);
				site.SitFullName = (string)(dr[4]);
				site.SitIsActive = (bool)(dr[5]);

				sites.Add(site);	
			}
			// Finish up
			dr.Close();
			return sites;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview, you probably don't.
		public static ESiteCollection ListForCustomer(Guid CustomerID, bool includeUnassigned,
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ESite site;
			ESiteCollection sites = new ESiteCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				SitDBid,
				SitCstID,
				SitClpID,
				SitName,
				SitFullName,
				SitIsActive
				from Sites";
			if (includeUnassigned)
				qry += " where (SitCstID is Null or SitCstID = @p1)";
			else
				qry += " where SitCstID = @p1";

			if (showactive && !showinactive)
				qry += " where SitIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where SitIsActive = 0";

			qry += "	order by SitName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", CustomerID));

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				site = new ESite();
				site.SitName = "<No Selection>";
				sites.Add(site);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				site = new ESite((Guid?)dr[0]);
				site.SitCstID = (Guid?)(dr[1]);
				site.SitClpID = (Guid?)Util.NullForDbNull(dr[2]);
				site.SitName = (string)(dr[3]);
				site.SitFullName = (string)(dr[4]);
				site.SitIsActive = (bool)(dr[5]);

				sites.Add(site);
			}
			// Finish up
			dr.Close();
			return sites;
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
					SitDBid as ID,
					SitName as SiteName,
					SitCstID as SiteCstID,
					SitClpID as SiteClpID,
					SitFullName as SiteFullName,
					CASE
						WHEN SitIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as SiteIsActive
					from Sites";
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
				cmd.CommandText = "Select SitIsActive from Sites where SitName = @p1";
			}
			else
			{
				cmd.CommandText = "Select SitIsActive from Sites where SitName = @p1 and SitDBid != @p0";
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

			if (SiteName == null)
			{
				SitNameErrMsg = "A unique Site Name is required";
				allFilled = false;
			}
			else
			{
				SitNameErrMsg = null;
			}
			if (SiteFullName == null)
			{
				SitFullNameErrMsg = "A Site Full Name is required";
				allFilled = false;
			}
			else
			{
				SitFullNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Site Collection class
	//--------------------------------------
	public class ESiteCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ESiteCollection()
		{ }
		//the indexer of the collection
		public ESite this[int index]
		{
			get
			{
				return (ESite)this.List[index];
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
			foreach (ESite site in InnerList)
			{
				if (site.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ESite item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ESite item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ESite item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ESite item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
