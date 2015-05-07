using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

    public enum ComponentSectionEnum : short
    {
        UsMain = 0,
        UsExt = 1,
        DsMain = 2,
        DsExt = 3,
        Br = 4,
        BrExt = 5
    }
    public class EAdditionalMeasurement : IEntity
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
		private Guid? AdmDBid;
		private string AdmName;
		private Guid? AdmDstID;
		private string AdmDescription;
		private decimal? AdmThickness;
        private bool AdmIncludeInStats;
        private short? AdmComponentSection;

		// Textbox limits
		// field size is 20, but limit to 14 for nice report fit
		public static int AdmNameCharLimit = 14; 
		// field size is currently 255, but limit to 44, so it fits on the report nicely. 
		public static int AdmDescriptionCharLimit = 65; 
		public static int AdmThicknessCharLimit = 7;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string AdmNameErrMsg;
		private string AdmDescriptionErrMsg;
		private string AdmThicknessErrMsg;

		// Form level validation message
		private string AdmErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return AdmDBid; }
		}

		public string AdditionalMeasurementName
		{
			get { return AdmName; }
			set { AdmName = Util.NullifyEmpty(value); }
		}

		public Guid? AdditionalMeasurementDstID
		{
			get { return AdmDstID; }
			set { AdmDstID = value; }
		}

		public string AdditionalMeasurementDescription
		{
			get { return AdmDescription; }
			set { AdmDescription = Util.NullifyEmpty(value); }
		}

		public decimal? AdditionalMeasurementThickness
		{
			get { return AdmThickness; }
			set { AdmThickness = value; }
		}

        public bool AdditionalMeasurementIncludeInStats
        {
            get { return AdmIncludeInStats; }
            set { AdmIncludeInStats = value; }
        }

        public short? AdditionalMeasurementComponentSection
        {
            get { return AdmComponentSection; }
            set { AdmComponentSection = value; }
        }


        // Array of ComponentSections for combo box binding
        static public ComponentSection[] GetComponentSectionsArray()
        {
            return new ComponentSection[] {
            new ComponentSection(null, "N/A"),
			new ComponentSection((short?)ComponentSectionEnum.UsMain,"Upstream Main"), 
			new ComponentSection((short?)ComponentSectionEnum.UsExt,"Upstream Ext."), 
			new ComponentSection((short?)ComponentSectionEnum.DsMain,"Downstream Main"), 
			new ComponentSection((short?)ComponentSectionEnum.DsExt,"Downstream Ext."), 
			new ComponentSection((short?)ComponentSectionEnum.Br,"Branch"), 
			new ComponentSection((short?)ComponentSectionEnum.BrExt,"Branch Ext.")
            };
        }

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string AdditionalMeasurementNameErrMsg
		{
			get { return AdmNameErrMsg; }
		}

		public string AdditionalMeasurementDescriptionErrMsg
		{
			get { return AdmDescriptionErrMsg; }
		}

		public string AdditionalMeasurementThicknessErrMsg
		{
			get { return AdmThicknessErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string AdditionalMeasurementErrMsg
		{
			get { return AdmErrMsg; }
			set { AdmErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool AdditionalMeasurementNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > AdmNameCharLimit)
			{
				AdmNameErrMsg = string.Format("Additional Measurement Names cannot exceed {0} characters", AdmNameCharLimit);
				return false;
			}
			else
			{
				AdmNameErrMsg = null;
				return true;
			}
		}

		public bool AdditionalMeasurementDescriptionLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > AdmDescriptionCharLimit)
			{
				AdmDescriptionErrMsg = string.Format("Additional Measurement Descriptions cannot exceed {0} characters", AdmDescriptionCharLimit);
				return false;
			}
			else
			{
				AdmDescriptionErrMsg = null;
				return true;
			}
		}

		public bool AdditionalMeasurementThicknessLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > AdmThicknessCharLimit)
			{
				AdmThicknessErrMsg = string.Format("Additional Measurement Thicknesss cannot exceed {0} characters", AdmThicknessCharLimit);
				return false;
			}
			else
			{
				AdmThicknessErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool AdditionalMeasurementNameValid(string name)
		{
			if (!AdditionalMeasurementNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, AdmDBid))
			{
				AdmNameErrMsg = "That Additional Measurement Name is already in use.";
				return false;
			}
			AdmNameErrMsg = null;
			return true;
		}

		public bool AdditionalMeasurementDescriptionValid(string value)
		{
			if (!AdditionalMeasurementDescriptionLengthOk(value)) return false;

			AdmDescriptionErrMsg = null;
			return true;
		}

		public bool AdditionalMeasurementThicknessValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				AdmThicknessErrMsg = null;
				return true;
			}
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				AdmThicknessErrMsg = null;
				return true;
			}
			AdmThicknessErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EAdditionalMeasurement()
		{
			this.AdmIncludeInStats = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EAdditionalMeasurement(Guid? id) : this()
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
				AdmDBid,
				AdmName,
				AdmDstID,
				AdmDescription,
				AdmThickness,
				AdmIncludeInStats,
				AdmComponentSection
				from AdditionalMeasurements
				where AdmDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				AdmDBid = (Guid?)dr[0];
				AdmName = (string)dr[1];
				AdmDstID = (Guid?)dr[2];
				AdmDescription = (string)Util.NullForDbNull(dr[3]);
				AdmThickness = (decimal?)Util.NullForDbNull(dr[4]);
                AdmIncludeInStats = (bool)dr[5];
                AdmComponentSection = (short?)Util.NullForDbNull(dr[6]);
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
				AdmDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", AdmDBid),
					new SqlCeParameter("@p1", AdmName),
					new SqlCeParameter("@p2", AdmDstID),
					new SqlCeParameter("@p3", Util.DbNullForNull(AdmDescription)),
					new SqlCeParameter("@p4", Util.DbNullForNull(AdmThickness)),
					new SqlCeParameter("@p5", AdmIncludeInStats),
					new SqlCeParameter("@p6", Util.DbNullForNull(AdmComponentSection))
					});
				cmd.CommandText = @"Insert Into AdditionalMeasurements (
					AdmDBid,
					AdmName,
					AdmDstID,
					AdmDescription,
					AdmThickness,
					AdmIncludeInStats,
					AdmComponentSection
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Additional Measurements row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", AdmDBid),
					new SqlCeParameter("@p1", AdmName),
					new SqlCeParameter("@p2", AdmDstID),
					new SqlCeParameter("@p3", Util.DbNullForNull(AdmDescription)),
					new SqlCeParameter("@p4", Util.DbNullForNull(AdmThickness)),
					new SqlCeParameter("@p5", AdmIncludeInStats),
					new SqlCeParameter("@p6", Util.DbNullForNull(AdmComponentSection))});

				cmd.CommandText =
					@"Update AdditionalMeasurements 
					set					
					AdmName = @p1,					
					AdmDstID = @p2,					
					AdmDescription = @p3,					
					AdmThickness = @p4,					
					AdmIncludeInStats = @p5,
					AdmComponentSection = @p6
					Where AdmDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update Additional Measurements row");
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
			if (!AdditionalMeasurementNameValid(AdditionalMeasurementName)) return false;
			if (!AdditionalMeasurementDescriptionValid(AdditionalMeasurementDescription)) return false;

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
			if (AdmDBid == null)
			{
				AdditionalMeasurementErrMsg = "Unable to delete. Record not found.";
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
					@"Delete from AdditionalMeasurements 
					where AdmDBid = @p0";
				cmd.Parameters.Add("@p0", AdmDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					AdditionalMeasurementErrMsg = "Unable to delete.  Please try again later.";
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
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of additionalmeasurements
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EAdditionalMeasurementCollection ListByNameForInspection(Guid InspectionID)
		{
			EAdditionalMeasurement additionalmeasurement;
			EAdditionalMeasurementCollection additionalmeasurements = new EAdditionalMeasurementCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				AdmDBid,
				AdmName,
				AdmDstID,
				AdmDescription,
				AdmThickness,
				AdmIncludeInStats,
				AdmComponentSection
				from AdditionalMeasurements
				inner join Dsets on AdmDstID = DstDBid";

			qry += " where DstIspID = @p1";
			qry += "	order by AdmName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", InspectionID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				additionalmeasurement = new EAdditionalMeasurement((Guid?)dr[0]);
				additionalmeasurement.AdmName = (string)(dr[1]);
				additionalmeasurement.AdmDstID = (Guid?)(dr[2]);
				additionalmeasurement.AdmDescription = (string)Util.NullForDbNull(dr[3]);
				additionalmeasurement.AdmThickness = (decimal?)Util.NullForDbNull(dr[4]);
                additionalmeasurement.AdmIncludeInStats = (bool)(dr[5]);
                additionalmeasurement.AdmComponentSection = (short?)(dr[6]);

				additionalmeasurements.Add(additionalmeasurement);	
			}
			// Finish up
			dr.Close();
			return additionalmeasurements;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataViewForDset(Guid? dsetID)
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
					AdmDBid as ID,
					AdmName as AdditionalMeasurementName,
					AdmDstID as AdditionalMeasurementDstID,
					AdmDescription as AdditionalMeasurementDescription,
					AdmThickness as AdditionalMeasurementThickness,
					CASE
						WHEN AdmIncludeInStats = 0 THEN 'No'
						ELSE 'Yes'
					END as AdditionalMeasurementIncludeInStats,
					CASE
						WHEN AdmComponentSection = 0 THEN 'Upstream Main'
						WHEN AdmComponentSection = 1 THEN 'Upstream Ext.'
						WHEN AdmComponentSection = 2 THEN 'Downstream Main'
						WHEN AdmComponentSection = 3 THEN 'Downstream Ext.'
						WHEN AdmComponentSection = 4 THEN 'Branch'
						WHEN AdmComponentSection = 5 THEN 'Branch Ext.'
						ELSE 'N/A'
					END as AdditionalMeasurementComponentSection
					from AdditionalMeasurements
					Where AdmDstID = @p1";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", dsetID == null ? Guid.Empty : dsetID);
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
			cmd.Parameters.Add(new SqlCeParameter("@p2", AdmDstID));
			if (id == null)
			{
				cmd.CommandText = "Select AdmDBid from AdditionalMeasurements where AdmName = @p1 and AdmDstID = @p2";
			}
			else
			{
				cmd.CommandText = "Select AdmDBid from AdditionalMeasurements where AdmName = @p1 and AdmDstID = @p2 and AdmDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			return exists;
		}


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (AdditionalMeasurementName == null)
			{
				AdmNameErrMsg = "A unique Additional Measurement Name is required";
				allFilled = false;
			}
			else
			{
				AdmNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// AdditionalMeasurement Collection class
	//--------------------------------------
	public class EAdditionalMeasurementCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EAdditionalMeasurementCollection()
		{ }
		//the indexer of the collection
		public EAdditionalMeasurement this[int index]
		{
			get
			{
				return (EAdditionalMeasurement)this.List[index];
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
			foreach (EAdditionalMeasurement additionalmeasurement in InnerList)
			{
				if (additionalmeasurement.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EAdditionalMeasurement item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EAdditionalMeasurement item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EAdditionalMeasurement item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EAdditionalMeasurement item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}

	}

    public class ComponentSection
    {
        private short? id;

        public short? ID
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public ComponentSection(short? ID, string Name)
        {
            this.id = ID;
            this.name = Name;
        }
    }

}
