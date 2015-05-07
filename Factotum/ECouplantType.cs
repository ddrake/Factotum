using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ECouplantType : IEntity
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
		// Use int?, decimal?, etc for nullable numbers
		// Strings, images, etc, are reference types already
		private Guid? CptDBid;
		private string CptName;
		private bool CptIsLclChg;
		private bool CptUsedInOutage;
		private bool CptIsActive;

		// Textbox limits
		public static int CptNameCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string CptNameErrMsg;

		// Form level validation message
		private string CptErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return CptDBid; }
		}

		public string CouplantTypeName
		{
			get { return CptName; }
			set { CptName = Util.NullifyEmpty(value); }
		}

		public bool CouplantTypeIsLclChg
		{
			get { return CptIsLclChg; }
			set { CptIsLclChg = value; }
		}

		public bool CouplantTypeUsedInOutage
		{
			get { return CptUsedInOutage; }
			set { CptUsedInOutage = value; }
		}

		public bool CouplantTypeIsActive
		{
			get { return CptIsActive; }
			set { CptIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string CouplantTypeNameErrMsg
		{
			get { return CptNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string CouplantTypeErrMsg
		{
			get { return CptErrMsg; }
			set { CptErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool CouplantTypeNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CptNameCharLimit)
			{
				CptNameErrMsg = string.Format("Couplant Type Names cannot exceed {0} characters", CptNameCharLimit);
				return false;
			}
			else
			{
				CptNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool CouplantTypeNameValid(string name)
		{
			bool existingIsInactive;
			if (!CouplantTypeNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, CptDBid, out existingIsInactive))
			{
				CptNameErrMsg = existingIsInactive ?
					"A Couplant Type with that name exists but its status has been set to inactive." :
					"That Couplant Type Name is already in use.";
				return false;
			}
			CptNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ECouplantType()
		{
			this.CptIsLclChg = false;
			this.CptUsedInOutage = false;
			this.CptIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ECouplantType(Guid? id) : this()
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
				CptDBid,
				CptName,
				CptIsLclChg,
				CptUsedInOutage,
				CptIsActive
				from CouplantTypes
				where CptDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				CptDBid = (Guid?)dr[0];
				CptName = (string)dr[1];
				CptIsLclChg = (bool)dr[2];
				CptUsedInOutage = (bool)dr[3];
				CptIsActive = (bool)dr[4];
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
				if (!Globals.IsMasterDB) CptIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				CptDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CptDBid),
					new SqlCeParameter("@p1", CptName),
					new SqlCeParameter("@p2", CptIsLclChg),
					new SqlCeParameter("@p3", CptUsedInOutage),
					new SqlCeParameter("@p4", CptIsActive)
					});
				cmd.CommandText = @"Insert Into CouplantTypes (
					CptDBid,
					CptName,
					CptIsLclChg,
					CptUsedInOutage,
					CptIsActive
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Couplant Types row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CptDBid),
					new SqlCeParameter("@p1", CptName),
					new SqlCeParameter("@p2", CptIsLclChg),
					new SqlCeParameter("@p3", CptUsedInOutage),
					new SqlCeParameter("@p4", CptIsActive)});

				cmd.CommandText =
					@"Update CouplantTypes 
					set					
					CptName = @p1,					
					CptIsLclChg = @p2,					
					CptUsedInOutage = @p3,					
					CptIsActive = @p4
					Where CptDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update couplant types row");
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
			if (!CouplantTypeNameValid(CouplantTypeName)) return false;

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
			if (CptDBid == null)
			{
				CouplantTypeErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (CptUsedInOutage)
			{
				CouplantTypeErrMsg = "Unable to delete because this Couplant Type has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!CptIsLclChg && !Globals.IsMasterDB)
			{
				CouplantTypeErrMsg = "Unable to delete because this Couplant Type was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasOutages())
			{
				CouplantTypeErrMsg = "Unable to delete because this Couplant Type is referenced by one or more outages.";
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
					@"Delete from CouplantTypes 
					where CptDBid = @p0";
				cmd.Parameters.Add("@p0", CptDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					CouplantTypeErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					CouplantTypeErrMsg = null;
					OnChanged(ID); 
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool HasOutages()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OtgDBid from Outages
					where OtgCptID = @p0";
			cmd.Parameters.Add("@p0", CptDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of couplanttypes
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ECouplantTypeCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ECouplantType couplanttype;
			ECouplantTypeCollection couplanttypes = new ECouplantTypeCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CptDBid,
				CptName,
				CptIsLclChg,
				CptUsedInOutage,
				CptIsActive
				from CouplantTypes";
			if (showactive && !showinactive)
				qry += " where CptIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where CptIsActive = 0";

			qry += "	order by CptName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				couplanttype = new ECouplantType();
				couplanttype.CptName = "<No Selection>";
				couplanttypes.Add(couplanttype);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				couplanttype = new ECouplantType((Guid?)dr[0]);
				couplanttype.CptName = (string)(dr[1]);
				couplanttype.CptIsLclChg = (bool)(dr[2]);
				couplanttype.CptUsedInOutage = (bool)(dr[3]);
				couplanttype.CptIsActive = (bool)(dr[4]);

				couplanttypes.Add(couplanttype);	
			}
			// Finish up
			dr.Close();
			return couplanttypes;
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
					CptDBid as ID,
					CptName as CouplantTypeName,
					CASE
						WHEN CptIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as CouplantTypeIsLclChg,
					CASE
						WHEN CptUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as CouplantTypeUsedInOutage,
					CASE
						WHEN CptIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as CouplantTypeIsActive
					from CouplantTypes";
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
				cmd.CommandText = "Select CptIsActive from CouplantTypes where CptName = @p1";
			}
			else
			{
				cmd.CommandText = "Select CptIsActive from CouplantTypes where CptName = @p1 and CptDBid != @p0";
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

			if (CouplantTypeName == null)
			{
				CptNameErrMsg = "A unique Couplant Type Name is required";
				allFilled = false;
			}
			else
			{
				CptNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// CouplantType Collection class
	//--------------------------------------
	public class ECouplantTypeCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ECouplantTypeCollection()
		{ }
		//the indexer of the collection
		public ECouplantType this[int index]
		{
			get
			{
				return (ECouplantType)this.List[index];
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
			foreach (ECouplantType couplanttype in InnerList)
			{
				if (couplanttype.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ECouplantType item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ECouplantType item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ECouplantType item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ECouplantType item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
