using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ERadialLocation : IEntity
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
		private Guid? RdlDBid;
		private string RdlName;
		private bool RdlIsLclChg;
		private bool RdlUsedInOutage;
		private bool RdlIsActive;

		// Textbox limits
		public static int RdlNameCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string RdlNameErrMsg;

		// Form level validation message
		private string RdlErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return RdlDBid; }
		}

		public string RadialLocationName
		{
			get { return RdlName; }
			set { RdlName = Util.NullifyEmpty(value); }
		}

		public bool RadialLocationIsLclChg
		{
			get { return RdlIsLclChg; }
			set { RdlIsLclChg = value; }
		}

		public bool RadialLocationUsedInOutage
		{
			get { return RdlUsedInOutage; }
			set { RdlUsedInOutage = value; }
		}

		public bool RadialLocationIsActive
		{
			get { return RdlIsActive; }
			set { RdlIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string RadialLocationNameErrMsg
		{
			get { return RdlNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string RadialLocationErrMsg
		{
			get { return RdlErrMsg; }
			set { RdlErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool RadialLocationNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > RdlNameCharLimit)
			{
				RdlNameErrMsg = string.Format("Radial Location Names cannot exceed {0} characters", RdlNameCharLimit);
				return false;
			}
			else
			{
				RdlNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool RadialLocationNameValid(string name)
		{
			bool existingIsInactive;
			if (!RadialLocationNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, RdlDBid, out existingIsInactive))
			{
				RdlNameErrMsg = existingIsInactive ?
					"That Radial Location Name exists but its status has been set to inactive." :
					"That Radial Location Name is already in use.";
				return false;
			}
			RdlNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ERadialLocation()
		{
			this.RdlIsLclChg = false;
			this.RdlUsedInOutage = false;
			this.RdlIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ERadialLocation(Guid? id) : this()
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
				RdlDBid,
				RdlName,
				RdlIsLclChg,
				RdlUsedInOutage,
				RdlIsActive
				from RadialLocations
				where RdlDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				RdlDBid = (Guid?)dr[0];
				RdlName = (string)dr[1];
				RdlIsLclChg = (bool)dr[2];
				RdlUsedInOutage = (bool)dr[3];
				RdlIsActive = (bool)dr[4];
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
				if (!Globals.IsMasterDB) RdlIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				RdlDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", RdlDBid),
					new SqlCeParameter("@p1", RdlName),
					new SqlCeParameter("@p2", RdlIsLclChg),
					new SqlCeParameter("@p3", RdlUsedInOutage),
					new SqlCeParameter("@p4", RdlIsActive)
					});
				cmd.CommandText = @"Insert Into RadialLocations (
					RdlDBid,
					RdlName,
					RdlIsLclChg,
					RdlUsedInOutage,
					RdlIsActive
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert RadialLocations row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", RdlDBid),
					new SqlCeParameter("@p1", RdlName),
					new SqlCeParameter("@p2", RdlIsLclChg),
					new SqlCeParameter("@p3", RdlUsedInOutage),
					new SqlCeParameter("@p4", RdlIsActive)});

				cmd.CommandText =
					@"Update RadialLocations 
					set					
					RdlName = @p1,					
					RdlIsLclChg = @p2,					
					RdlUsedInOutage = @p3,					
					RdlIsActive = @p4
					Where RdlDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update radiallocations row");
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
			if (!RadialLocationNameValid(RadialLocationName)) return false;

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
			if (RdlDBid == null)
			{
				RadialLocationErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (RdlUsedInOutage)
			{
				RadialLocationErrMsg = "Unable to delete this Radial Location because it has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!RdlIsLclChg && !Globals.IsMasterDB)
			{
				RadialLocationErrMsg = "Unable to delete because this Radial Location was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (HasChildren())
			{
				RadialLocationErrMsg = "Unable to delete because this Radial Location is referenced by one or more grids.";
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
					@"Delete from RadialLocations 
					where RdlDBid = @p0";
				cmd.Parameters.Add("@p0", RdlDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					RadialLocationErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					RadialLocationErrMsg = null;
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
				@"Select GrdDBid from Grids
					where GrdRdlID = @p0";
			cmd.Parameters.Add("@p0", RdlDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of radiallocations
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ERadialLocationCollection ListByName(bool showinactive, bool addNoSelection)
		{
			ERadialLocation radiallocation;
			ERadialLocationCollection radiallocations = new ERadialLocationCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				radiallocation = new ERadialLocation();
				radiallocation.RdlName = "<No Selection>";
				radiallocations.Add(radiallocation);
			}
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				RdlDBid,
				RdlName,
				RdlIsLclChg,
				RdlUsedInOutage,
				RdlIsActive
				from RadialLocations";
			if (!showinactive)
				qry += " where RdlIsActive = 1";

			qry += "	order by RdlName";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				radiallocation = new ERadialLocation((Guid?)dr[0]);
				radiallocation.RdlName = (string)(dr[1]);
				radiallocation.RdlIsLclChg = (bool)(dr[2]);
				radiallocation.RdlUsedInOutage = (bool)(dr[3]);
				radiallocation.RdlIsActive = (bool)(dr[4]);

				radiallocations.Add(radiallocation);	
			}
			// Finish up
			dr.Close();
			return radiallocations;
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
					RdlDBid as ID,
					RdlName as RadialLocationName,
					CASE
						WHEN RdlIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as RadialLocationIsLclChg,
					CASE
						WHEN RdlUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as RadialLocationUsedInOutage,
					CASE
						WHEN RdlIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as RadialLocationIsActive
					from RadialLocations";
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
				cmd.CommandText = "Select RdlIsActive from RadialLocations where RdlName = @p1";
			}
			else
			{
				cmd.CommandText = "Select RdlIsActive from RadialLocations where RdlName = @p1 and RdlDBid != @p0";
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

			if (RadialLocationName == null)
			{
				RdlNameErrMsg = "A unique RadialLocation Name is required";
				allFilled = false;
			}
			else
			{
				RdlNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// RadialLocation Collection class
	//--------------------------------------
	public class ERadialLocationCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ERadialLocationCollection()
		{ }
		//the indexer of the collection
		public ERadialLocation this[int index]
		{
			get
			{
				return (ERadialLocation)this.List[index];
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
			foreach (ERadialLocation radiallocation in InnerList)
			{
				if (radiallocation.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ERadialLocation item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ERadialLocation item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ERadialLocation item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ERadialLocation item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
