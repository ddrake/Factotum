using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EComponentMaterial : IEntity
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
		private Guid? CmtDBid;
		private string CmtName;
		private Guid? CmtSitID;
		private byte? CmtCalBlockMaterial;
		private bool CmtIsLclChg;
		private bool CmtIsActive;

		// Textbox limits
		public static int CmtNameCharLimit = 25;
		public static int CmtCalBlockMaterialCharLimit = 3;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string CmtNameErrMsg;
		private string CmtCalBlockMaterialErrMsg;

		// Form level validation message
		private string CmtErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return CmtDBid; }
		}

		public string CmpMaterialName
		{
			get { return CmtName; }
			set { CmtName = Util.NullifyEmpty(value); }
		}

		public Guid? CmpMaterialSitID
		{
			get { return CmtSitID; }
			set { CmtSitID = value; }
		}

		public byte? CmpMaterialCalBlockMaterial
		{
			get { return CmtCalBlockMaterial; }
			set { CmtCalBlockMaterial = value; }
		}

		public bool CmpMaterialIsLclChg
		{
			get { return CmtIsLclChg; }
			set { CmtIsLclChg = value; }
		}

		public bool CmpMaterialIsActive
		{
			get { return CmtIsActive; }
			set { CmtIsActive = value; }
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string CmpMaterialNameErrMsg
		{
			get { return CmtNameErrMsg; }
		}

		public string CmpMaterialCalBlockMaterialErrMsg
		{
			get { return CmtCalBlockMaterialErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string CmpMaterialErrMsg
		{
			get { return CmtErrMsg; }
			set { CmtErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool CmpMaterialNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmtNameCharLimit)
			{
				CmtNameErrMsg = string.Format("Component Material Names cannot exceed {0} characters", CmtNameCharLimit);
				return false;
			}
			else
			{
				CmtNameErrMsg = null;
				return true;
			}
		}


		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool CmpMaterialNameValid(string name)
		{
			bool existingIsInactive;
			if (!CmpMaterialNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForSite(name, CmtDBid, (Guid)CmtSitID, out existingIsInactive))
			{
				CmtNameErrMsg = existingIsInactive ?
					"A Component Material with that name exists but its status has been set to inactive." :
					"That Component Material Name is already in use.";
				return false;
			}
			CmtNameErrMsg = null;
			return true;
		}

		public bool CmpMaterialCalBlockMaterialValid(byte? value)
		{
			// Add some real validation here if needed.
			CmtCalBlockMaterialErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EComponentMaterial()
		{
			this.CmtCalBlockMaterial = 1;
			this.CmtIsLclChg = false;
			this.CmtIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EComponentMaterial(Guid? id) : this()
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
				CmtDBid,
				CmtName,
				CmtSitID,
				CmtCalBlockMaterial,
				CmtIsLclChg,
				CmtIsActive
				from ComponentMaterials
				where CmtDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				CmtDBid = (Guid?)dr[0];
				CmtName = (string)dr[1];
				CmtSitID = (Guid?)dr[2];
				CmtCalBlockMaterial = (byte?)dr[3];
				CmtIsLclChg = (bool)dr[4];
				CmtIsActive = (bool)dr[5];
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
				if (!Globals.IsMasterDB) CmtIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				CmtDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CmtDBid),
					new SqlCeParameter("@p1", CmtName),
					new SqlCeParameter("@p2", CmtSitID),
					new SqlCeParameter("@p3", CmtCalBlockMaterial),
					new SqlCeParameter("@p4", CmtIsLclChg),
					new SqlCeParameter("@p5", CmtIsActive)
					});
				cmd.CommandText = @"Insert Into ComponentMaterials (
					CmtDBid,
					CmtName,
					CmtSitID,
					CmtCalBlockMaterial,
					CmtIsLclChg,
					CmtIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert ComponentMaterials row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CmtDBid),
					new SqlCeParameter("@p1", CmtName),
					new SqlCeParameter("@p2", CmtSitID),
					new SqlCeParameter("@p3", CmtCalBlockMaterial),
					new SqlCeParameter("@p4", CmtIsLclChg),
					new SqlCeParameter("@p5", CmtIsActive)});

				cmd.CommandText =
					@"Update ComponentMaterials 
					set					
					CmtName = @p1,					
					CmtSitID = @p2,					
					CmtCalBlockMaterial = @p3,					
					CmtIsLclChg = @p4,					
					CmtIsActive = @p5
					Where CmtDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update component materials row");
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
			if (!CmpMaterialNameValid(CmpMaterialName)) return false;
			if (!CmpMaterialCalBlockMaterialValid(CmpMaterialCalBlockMaterial)) return false;

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
			if (CmtDBid == null)
			{
				CmpMaterialErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (!CmtIsLclChg && !Globals.IsMasterDB)
			{
				CmpMaterialErrMsg = "Unable to delete because this Component Material was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasChildren())
			{
				CmpMaterialErrMsg = "Unable to delete because components exist with this material type.";
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
					@"Delete from ComponentMaterials 
					where CmtDBid = @p0";
				cmd.Parameters.Add("@p0", CmtDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					CmpMaterialErrMsg = "Unable to delete.  Please try again later.";
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
				CmpMaterialErrMsg = null;
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
					where CmpCmtID = @p0";
			cmd.Parameters.Add("@p0", CmtDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}
		//--------------------------------------------------------------------
		// Static listing methods which return collections of cmpmaterials
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EComponentMaterialCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			EComponentMaterial cmpmaterial;
			EComponentMaterialCollection cmpmaterials = new EComponentMaterialCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CmtDBid,
				CmtName,
				CmtSitID,
				CmtCalBlockMaterial,
				CmtIsLclChg,
				CmtIsActive
				from ComponentMaterials";
			if (showactive && !showinactive)
				qry += " where CmtIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where CmtIsActive = 0";

			qry += "	order by CmtName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				cmpmaterial = new EComponentMaterial();
				cmpmaterial.CmtName = "<No Selection>";
				cmpmaterials.Add(cmpmaterial);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				cmpmaterial = new EComponentMaterial((Guid?)dr[0]);
				cmpmaterial.CmtName = (string)(dr[1]);
				cmpmaterial.CmtSitID = (Guid?)(dr[2]);
				cmpmaterial.CmtCalBlockMaterial = (byte?)(dr[3]);
				cmpmaterial.CmtIsLclChg = (bool)(dr[4]);
				cmpmaterial.CmtIsActive = (bool)(dr[5]);

				cmpmaterials.Add(cmpmaterial);	
			}
			// Finish up
			dr.Close();
			return cmpmaterials;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview or combo box, you probably don't.
		public static EComponentMaterialCollection ListForSite(Guid? SiteID, 
			bool showinactive, bool addNoSelection)
		{
			EComponentMaterial cmpmaterial;
			EComponentMaterialCollection cmpmaterials = new EComponentMaterialCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				cmpmaterial = new EComponentMaterial();
				cmpmaterial.CmtName = "<No Selection>";
				cmpmaterials.Add(cmpmaterial);
			}
			if (SiteID == null) return cmpmaterials;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CmtDBid,
				CmtName,
				CmtSitID,
				CmtCalBlockMaterial,
				CmtIsLclChg,
				CmtIsActive
				from ComponentMaterials";
				qry += " where CmtSitID = @p1";

			if (!showinactive)
				qry += " and CmtIsActive = 1";

			qry += "	order by CmtName";
			cmd.CommandText = qry;
			cmd.Parameters.Add(new SqlCeParameter("@p1", SiteID));

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				cmpmaterial = new EComponentMaterial((Guid?)dr[0]);
				cmpmaterial.CmtName = (string)(dr[1]);
				cmpmaterial.CmtSitID = (Guid?)(dr[2]);
				cmpmaterial.CmtCalBlockMaterial = (byte?)(dr[3]);
				cmpmaterial.CmtIsLclChg = (bool)(dr[4]);
				cmpmaterial.CmtIsActive = (bool)(dr[5]);

				cmpmaterials.Add(cmpmaterial);
			}
			// Finish up
			dr.Close();
			return cmpmaterials;
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
					CmtDBid as ID,
					CmtName as CmpMaterialName,
					CmtSitID as CmpMaterialSitID,
					CASE 
						WHEN CmtCalBlockMaterial = 1 Then 'Carbon Steel'
						WHEN CmtCalBlockMaterial = 2 Then 'Stainless Steel'
						WHEN CmtCalBlockMaterial = 3 Then 'Inconel'
						ELSE 'Unknown Material'
					END as CmpMaterialCalBlockMaterial,
					CASE
						WHEN CmtIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as CmpMaterialIsLclChg,
					CASE
						WHEN CmtIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as CmpMaterialIsActive
					from ComponentMaterials";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static EComponentMaterial FindForComponentMaterialName(string ComponentMaterialName)
		{
			if (Util.IsNullOrEmpty(ComponentMaterialName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", ComponentMaterialName));
			cmd.CommandText = "Select CmtDBid from ComponentMaterials where CmtName = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new EComponentMaterial((Guid)val);
			else return null;
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
				cmd.CommandText = "Select CmtDBid from ComponentMaterials where CmtName = @p1";
			}
			else
			{
				cmd.CommandText = "Select CmtDBid from ComponentMaterials where CmtName = @p1 and CmtDBid != @p0";
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
					@"Select CmtIsActive from ComponentMaterials 
					where LTRIM(RTRIM(CmtName)) = @p1 and CmtSitID = @p2";
			}
			else
			{
				cmd.CommandText = 
					@"Select CmtIsActive from ComponentMaterials 
					where LTRIM(RTRIM(CmtName)) = @p1 and CmtSitID = @p2 and CmtDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
            if (val == null)
            {
                return false;
            }
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;
			return exists;

		}

		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (CmpMaterialName == null)
			{
				CmtNameErrMsg = "A unique Component Material Name is required";
				allFilled = false;
			}
			else
			{
				CmtNameErrMsg = null;
			}
			if (CmpMaterialCalBlockMaterial == null)
			{
				CmtCalBlockMaterialErrMsg = "A Calibration block material is required";
				allFilled = false;
			}
			else
			{
				CmtCalBlockMaterialErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// CmpMaterial Collection class
	//--------------------------------------
	public class EComponentMaterialCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EComponentMaterialCollection()
		{ }
		//the indexer of the collection
		public EComponentMaterial this[int index]
		{
			get
			{
				return (EComponentMaterial)this.List[index];
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
			foreach (EComponentMaterial cmpmaterial in InnerList)
			{
				if (cmpmaterial.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EComponentMaterial item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EComponentMaterial item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EComponentMaterial item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EComponentMaterial item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}

}
