using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EGridSize : IEntity
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
		private Guid? GszDBid;
		private string GszName;
		private decimal? GszAxialDistance;
		private decimal? GszRadialDistance;
		private decimal? GszMaxDiameter;
		private bool GszIsLclChg;
		private bool GszUsedInOutage;
		private bool GszIsActive;

		// Textbox limits
		public static int GszNameCharLimit = 20;
		public static int GszAxialDistanceCharLimit = 8;
		public static int GszRadialDistanceCharLimit = 8;
		public static int GszMaxDiameterCharLimit = 10;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string GszNameErrMsg;
		private string GszAxialDistanceErrMsg;
		private string GszRadialDistanceErrMsg;
		private string GszMaxDiameterErrMsg;

		// Form level validation message
		private string GszErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return GszDBid; }
		}

		public string GridSizeName
		{
			get { return GszName; }
			set { GszName = Util.NullifyEmpty(value); }
		}

		public decimal? GridSizeAxialDistance
		{
			get { return GszAxialDistance; }
			set { GszAxialDistance = value; }
		}

		public decimal? GridSizeRadialDistance
		{
			get { return GszRadialDistance; }
			set { GszRadialDistance = value; }
		}

		public decimal? GridSizeMaxDiameter
		{
			get { return GszMaxDiameter; }
			set { GszMaxDiameter = value; }
		}

		public bool GridSizeIsLclChg
		{
			get { return GszIsLclChg; }
			set { GszIsLclChg = value; }
		}

		public bool GridSizeUsedInOutage
		{
			get { return GszUsedInOutage; }
			set { GszUsedInOutage = value; }
		}

		public bool GridSizeIsActive
		{
			get { return GszIsActive; }
			set { GszIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string GridSizeNameErrMsg
		{
			get { return GszNameErrMsg; }
		}

		public string GridSizeAxialDistanceErrMsg
		{
			get { return GszAxialDistanceErrMsg; }
		}

		public string GridSizeRadialDistanceErrMsg
		{
			get { return GszRadialDistanceErrMsg; }
		}

		public string GridSizeMaxDiameterErrMsg
		{
			get { return GszMaxDiameterErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string GridSizeErrMsg
		{
			get { return GszErrMsg; }
			set { GszErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool GridSizeNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GszNameCharLimit)
			{
				GszNameErrMsg = string.Format("Grid Size Names cannot exceed {0} characters", GszNameCharLimit);
				return false;
			}
			else
			{
				GszNameErrMsg = null;
				return true;
			}
		}

		public bool GridSizeAxialDistanceLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GszAxialDistanceCharLimit)
			{
				GszAxialDistanceErrMsg = string.Format("Grid Size Axial Distances cannot exceed {0} characters", GszAxialDistanceCharLimit);
				return false;
			}
			else
			{
				GszAxialDistanceErrMsg = null;
				return true;
			}
		}

		public bool GridSizeRadialDistanceLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GszRadialDistanceCharLimit)
			{
				GszRadialDistanceErrMsg = string.Format("Grid Size Radial Distances cannot exceed {0} characters", GszRadialDistanceCharLimit);
				return false;
			}
			else
			{
				GszRadialDistanceErrMsg = null;
				return true;
			}
		}

		public bool GridSizeMaxDiameterLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GszMaxDiameterCharLimit)
			{
				GszMaxDiameterErrMsg = string.Format("Grid Size Max Diameters cannot exceed {0} characters", GszMaxDiameterCharLimit);
				return false;
			}
			else
			{
				GszMaxDiameterErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool GridSizeNameValid(string name)
		{
			bool existingIsInactive;
			if (!GridSizeNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, GszDBid, out existingIsInactive))
			{
				GszNameErrMsg = existingIsInactive ?
					"A Grid Size with that Name exists but its status has been set to inactive." :
					"That Grid Size Name is already in use.";
				return false;
			}
			GszNameErrMsg = null;
			return true;
		}

		public bool GridSizeAxialDistanceValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				GszAxialDistanceErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0)
			{
				GszAxialDistanceErrMsg = null;
				return true;
			}
			GszAxialDistanceErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridSizeRadialDistanceValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				GszRadialDistanceErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0)
			{
				GszRadialDistanceErrMsg = null;
				return true;
			}
			GszRadialDistanceErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridSizeMaxDiameterValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				GszMaxDiameterErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0)
			{
				GszMaxDiameterErrMsg = null;
				return true;
			}
			GszMaxDiameterErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EGridSize()
		{
			this.GszIsLclChg = false;
			this.GszUsedInOutage = false;
			this.GszIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EGridSize(Guid? id) : this()
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
				GszDBid,
				GszName,
				GszAxialDistance,
				GszRadialDistance,
				GszMaxDiameter,
				GszIsLclChg,
				GszUsedInOutage,
				GszIsActive
				from GridSizes
				where GszDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				GszDBid = (Guid?)dr[0];
				GszName = (string)dr[1];
				GszAxialDistance = (decimal?)dr[2];
				GszRadialDistance = (decimal?)dr[3];
				GszMaxDiameter = (decimal?)dr[4];
				GszIsLclChg = (bool)dr[5];
				GszUsedInOutage = (bool)dr[6];
				GszIsActive = (bool)dr[7];
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
				if (!Globals.IsMasterDB) GszIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				GszDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GszDBid),
					new SqlCeParameter("@p1", GszName),
					new SqlCeParameter("@p2", GszAxialDistance),
					new SqlCeParameter("@p3", GszRadialDistance),
					new SqlCeParameter("@p4", GszMaxDiameter),
					new SqlCeParameter("@p5", GszIsLclChg),
					new SqlCeParameter("@p6", GszUsedInOutage),
					new SqlCeParameter("@p7", GszIsActive)
					});
				cmd.CommandText = @"Insert Into GridSizes (
					GszDBid,
					GszName,
					GszAxialDistance,
					GszRadialDistance,
					GszMaxDiameter,
					GszIsLclChg,
					GszUsedInOutage,
					GszIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Grid Sizes row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GszDBid),
					new SqlCeParameter("@p1", GszName),
					new SqlCeParameter("@p2", GszAxialDistance),
					new SqlCeParameter("@p3", GszRadialDistance),
					new SqlCeParameter("@p4", GszMaxDiameter),
					new SqlCeParameter("@p5", GszIsLclChg),
					new SqlCeParameter("@p6", GszUsedInOutage),
					new SqlCeParameter("@p7", GszIsActive)});

				cmd.CommandText =
					@"Update GridSizes 
					set					
					GszName = @p1,					
					GszAxialDistance = @p2,					
					GszRadialDistance = @p3,					
					GszMaxDiameter = @p4,					
					GszIsLclChg = @p5,					
					GszUsedInOutage = @p6,					
					GszIsActive = @p7
					Where GszDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update grid sizes row");
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
			if (!GridSizeNameValid(GridSizeName)) return false;

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
			if (GszDBid == null)
			{
				GridSizeErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (GszUsedInOutage)
			{
				GridSizeErrMsg = "Unable to delete because this Grid Size has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!GszIsLclChg && !Globals.IsMasterDB)
			{
				GridSizeErrMsg = "Unable to delete this Grid Size because it was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasChildren())
			{
				GridSizeErrMsg = "Unable to delete because this Grid Size is referenced by one or more Grids.";
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
					@"Delete from GridSizes 
					where GszDBid = @p0";
				cmd.Parameters.Add("@p0", GszDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					GridSizeErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					GridSizeErrMsg = null;
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
					where GrdGszID = @p0";
			cmd.Parameters.Add("@p0", GszDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of gridsizes
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EGridSizeCollection ListByName(bool showinactive, bool addNoSelection)
		{
			EGridSize gridsize;
			EGridSizeCollection gridsizes = new EGridSizeCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				gridsize = new EGridSize();
				gridsize.GszName = "<No Selection>";
				gridsizes.Add(gridsize);
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				GszDBid,
				GszName,
				GszAxialDistance,
				GszRadialDistance,
				GszMaxDiameter,
				GszIsLclChg,
				GszUsedInOutage,
				GszIsActive
				from GridSizes";
			if (!showinactive)
				qry += " where GszIsActive = 1";

			qry += "	order by GszName";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				gridsize = new EGridSize((Guid?)dr[0]);
				gridsize.GszName = (string)(dr[1]);
				gridsize.GszAxialDistance = (decimal?)(dr[2]);
				gridsize.GszRadialDistance = (decimal?)(dr[3]);
				gridsize.GszMaxDiameter = (decimal?)(dr[4]);
				gridsize.GszIsLclChg = (bool)(dr[5]);
				gridsize.GszUsedInOutage = (bool)(dr[6]);
				gridsize.GszIsActive = (bool)(dr[7]);

				gridsizes.Add(gridsize);	
			}
			// Finish up
			dr.Close();
			return gridsizes;
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
					GszDBid as ID,
					GszName as GridSizeName,
					GszAxialDistance as GridSizeAxialDistance,
					GszRadialDistance as GridSizeRadialDistance,
					GszMaxDiameter as GridSizeMaxDiameter,
					CASE
						WHEN GszIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as GridSizeIsLclChg,
					CASE
						WHEN GszUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as GridSizeUsedInOutage,
					CASE
						WHEN GszIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as GridSizeIsActive
					from GridSizes";
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
				cmd.CommandText = "Select GszIsActive from GridSizes where GszName = @p1";
			}
			else
			{
				cmd.CommandText = "Select GszIsActive from GridSizes where GszName = @p1 and GszDBid != @p0";
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

			if (GridSizeName == null)
			{
				GszNameErrMsg = "A unique Grid Size Name is required";
				allFilled = false;
			}
			else
			{
				GszNameErrMsg = null;
			}
			if (GridSizeAxialDistance == null)
			{
				GszAxialDistanceErrMsg = "A Grid Size Axial Distance is required";
				allFilled = false;
			}
			else
			{
				GszAxialDistanceErrMsg = null;
			}
			if (GridSizeRadialDistance == null)
			{
				GszRadialDistanceErrMsg = "A Grid Size Radial Distance is required";
				allFilled = false;
			}
			else
			{
				GszRadialDistanceErrMsg = null;
			}
			if (GridSizeMaxDiameter == null)
			{
				GszMaxDiameterErrMsg = "A Grid Size Max Diameter is required";
				allFilled = false;
			}
			else
			{
				GszMaxDiameterErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// GridSize Collection class
	//--------------------------------------
	public class EGridSizeCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EGridSizeCollection()
		{ }
		//the indexer of the collection
		public EGridSize this[int index]
		{
			get
			{
				return (EGridSize)this.List[index];
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
			foreach (EGridSize gridsize in InnerList)
			{
				if (gridsize.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EGridSize item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EGridSize item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EGridSize item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EGridSize item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
