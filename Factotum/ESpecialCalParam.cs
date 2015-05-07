using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum {

	public class ESpecialCalParam : IEntity
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
		private Guid? ScpDBid;
		private string ScpName;
		private string ScpUnits;
		private short? ScpReportOrder;
		private bool ScpUsedInOutage;
		private bool ScpIsLclChg;
		private bool ScpIsActive;

		// Textbox limits
		private const int ScpNameCharLimit = 25;
		private const int ScpUnitsCharLimit = 15;
		private const int ScpReportOrderCharLimit = 6;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string ScpNameErrMsg;
		private string ScpUnitsErrMsg;
		private string ScpReportOrderErrMsg;
		private string ScpUsedInOutageErrMsg;
		private string ScpIsLclChgErrMsg;
		private string ScpIsActiveErrMsg;

		// Form level validation message
		private string ScpErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return ScpDBid; }
		}

		public string SpecialCalParamName
		{
			get { return ScpName; }
			set { ScpName = Util.NullifyEmpty(value);	}
		}

		public string SpecialCalParamUnits
		{
			get { return ScpUnits; }
			set { ScpUnits = Util.NullifyEmpty(value); }
		}

		public short? SpecialCalParamReportOrder
		{
			get { return ScpReportOrder; }
			set { ScpReportOrder = value; }
		}

		public bool SpecialCalParamUsedInOutage
		{
			get { return ScpUsedInOutage; }
			set { ScpUsedInOutage = value; }
		}

		public bool SpecialCalParamIsLclChg
		{
			get { return ScpIsLclChg; }
			set { ScpIsLclChg = value; }
		}

		public bool SpecialCalParamIsActive
		{
			get { return ScpIsActive; }
			set { ScpIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string SpecialCalParamNameErrMsg
		{
			get { return ScpNameErrMsg; }
		}

		public string SpecialCalParamUnitsErrMsg
		{
			get { return ScpUnitsErrMsg; }
		}

		public string SpecialCalParamReportOrderErrMsg
		{
			get { return ScpReportOrderErrMsg; }
		}

		public string SpecialCalParamUsedInOutageErrMsg
		{
			get { return ScpUsedInOutageErrMsg; }
		}

		public string SpecialCalParamIsLclChgErrMsg
		{
			get { return ScpIsLclChgErrMsg; }
		}

		public string SpecialCalParamIsActiveErrMsg
		{
			get { return ScpIsActiveErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string SpecialCalParamErrMsg
		{
			get { return ScpErrMsg; }
			set { ScpErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool SpecialCalParamNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ScpNameCharLimit)
			{
				ScpNameErrMsg = string.Format("SpecialCalParamNames cannot exceed {0} characters", ScpNameCharLimit);
				return false;
			}
			else
			{
				ScpNameErrMsg = null;
				return true;
			}
		}

		public bool SpecialCalParamUnitsLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ScpUnitsCharLimit)
			{
				ScpUnitsErrMsg = string.Format("SpecialCalParamUnitss cannot exceed {0} characters", ScpUnitsCharLimit);
				return false;
			}
			else
			{
				ScpUnitsErrMsg = null;
				return true;
			}
		}

		public bool SpecialCalParamReportOrderLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ScpReportOrderCharLimit)
			{
				ScpReportOrderErrMsg = string.Format("SpecialCalParamReportOrders cannot exceed {0} characters", ScpReportOrderCharLimit);
				return false;
			}
			else
			{
				ScpReportOrderErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool SpecialCalParamNameValid(string name)
		{
			bool existingIsInactive;
			if (!SpecialCalParamNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, (Guid?)ScpDBid, out existingIsInactive))
			{
				ScpNameErrMsg = existingIsInactive ?
					"That SpecialCalParamName exists but its status has been set to inactive." :
					"That SpecialCalParamName is already in use.";
				return false;
			}
			ScpNameErrMsg = null;
			return true;
		}

		public bool SpecialCalParamUnitsValid(string value)
		{
			if (!SpecialCalParamUnitsLengthOk(value)) return false;

			ScpUnitsErrMsg = null;
			return true;
		}

		public bool SpecialCalParamReportOrderValid(string value)
		{
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				ScpReportOrderErrMsg = null;
				return true;
			}
			ScpReportOrderErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool SpecialCalParamUsedInOutageValid(bool value)
		{
			// Add some real validation here if needed.
			ScpUsedInOutageErrMsg = null;
			return true;
		}

		public bool SpecialCalParamIsLclChgValid(bool value)
		{
			// Add some real validation here if needed.
			ScpIsLclChgErrMsg = null;
			return true;
		}

		public bool SpecialCalParamIsActiveValid(bool value)
		{
			// Add some real validation here if needed.
			ScpIsActiveErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ESpecialCalParam()
		{
			this.ScpReportOrder = 0;
			this.ScpUsedInOutage = false;
			this.ScpIsLclChg = false;
			this.ScpIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ESpecialCalParam(Guid? id) : this()
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
				ScpDBid,
				ScpName,
				ScpUnits,
				ScpReportOrder,
				ScpUsedInOutage,
				ScpIsLclChg,
				ScpIsActive
				from SpecialCalParams
				where ScpDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				ScpDBid = (Guid?)dr[0];
				ScpName = (string)dr[1];
				ScpUnits = (string)dr[2];
				ScpReportOrder = (short?)dr[3];
				ScpUsedInOutage = (bool)dr[4];
				ScpIsLclChg = (bool)dr[5];
				ScpIsActive = (bool)dr[6];
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
				if (!Globals.IsMasterDB) ScpIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				ScpDBid = (Guid?)(cmd.ExecuteScalar());
				// Get the next parameter report order in the sequence.
				ScpReportOrder = getNewReportOrder();
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ScpDBid),
					new SqlCeParameter("@p1", ScpName),
					new SqlCeParameter("@p2", ScpUnits),
					new SqlCeParameter("@p3", ScpReportOrder),
					new SqlCeParameter("@p4", ScpUsedInOutage),
					new SqlCeParameter("@p5", ScpIsLclChg),
					new SqlCeParameter("@p6", ScpIsActive)
					});
				cmd.CommandText = @"Insert Into SpecialCalParams (
					ScpDBid,
					ScpName,
					ScpUnits,
					ScpReportOrder,
					ScpUsedInOutage,
					ScpIsLclChg,
					ScpIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert SpecialCalParams row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ScpDBid),
					new SqlCeParameter("@p1", ScpName),
					new SqlCeParameter("@p2", ScpUnits),
					new SqlCeParameter("@p3", ScpReportOrder),
					new SqlCeParameter("@p4", ScpUsedInOutage),
					new SqlCeParameter("@p5", ScpIsLclChg),
					new SqlCeParameter("@p6", ScpIsActive)});

				cmd.CommandText =
					@"Update SpecialCalParams 
					set					
					ScpName = @p1,					
					ScpUnits = @p2,					
					ScpReportOrder = @p3,					
					ScpUsedInOutage = @p4,					
					ScpIsLclChg = @p5,					
					ScpIsActive = @p6
					Where ScpDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update specialcalparams row");
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
			if (!SpecialCalParamNameValid(SpecialCalParamName)) return false;
			if (!SpecialCalParamUnitsValid(SpecialCalParamUnits)) return false;
			if (!SpecialCalParamUsedInOutageValid(SpecialCalParamUsedInOutage)) return false;
			if (!SpecialCalParamIsLclChgValid(SpecialCalParamIsLclChg)) return false;
			if (!SpecialCalParamIsActiveValid(SpecialCalParamIsActive)) return false;

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
			if (ScpDBid == null)
			{
				SpecialCalParamErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (HasValues())
			{
				SpecialCalParamErrMsg = "Unable to delete because this Parameter has Values assigned.";
				return false;
			}

			if (SpecialCalParamUsedInOutage)
			{
				SpecialCalParamErrMsg = "Unable to delete because this Parameter has been used in other outages.";
				return false;
			}

			if (!ScpIsLclChg && !Globals.IsMasterDB)
			{
				SpecialCalParamErrMsg = "Unable to delete because this Parameter was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			DialogResult rslt = DialogResult.None;
			if (promptUser)
			{
				rslt = MessageBox.Show("Are you sure?", "Factotum: Deleting...",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			}

			// If an error occurs when attempting to delete...
			// Use transactions??
			// Raise an event right before the deletion?
			if (!promptUser || rslt == DialogResult.OK)
			{
				SqlCeCommand cmd;
				int rowsAffected;				
				// First update the report order for all sections after this one.
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Update SpecialCalParams 
					set ScpReportOrder = ScpReportOrder - 1
					where ScpDBid = @p0
					and ScpReportOrder > @p1";
				cmd.Parameters.Add("@p0", ScpDBid);
				cmd.Parameters.Add("@p1", ScpReportOrder);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				rowsAffected = cmd.ExecuteNonQuery();

				// Now perform the Delete operation
				cmd = Globals.cnn.CreateCommand();				
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from SpecialCalParams 
					where ScpDBid = @p0";
				cmd.Parameters.Add("@p0", ScpDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					SpecialCalParamErrMsg = "Unable to delete.  Please try again later.";
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
				SpecialCalParamErrMsg = null;
				return false;
			}
		}

		private bool HasValues()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select ScvDBid from SpecialCalValues
					where ScvScpID = @p0";
			cmd.Parameters.Add("@p0", ScpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of specialcalparams
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ESpecialCalParamCollection ListByName(
			bool showinactive, bool addNoSelection)
		{
			ESpecialCalParam specialcalparam;
			ESpecialCalParamCollection specialcalparams = new ESpecialCalParamCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				ScpDBid,
				ScpName,
				ScpUnits,
				ScpReportOrder,
				ScpUsedInOutage,
				ScpIsLclChg,
				ScpIsActive
				from SpecialCalParams";
			if (!showinactive)
				qry += " where ScpIsActive = 1";

			qry += "	order by ScpName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				specialcalparam = new ESpecialCalParam();
				specialcalparam.ScpName = "<No Selection>";
				specialcalparams.Add(specialcalparam);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				specialcalparam = new ESpecialCalParam((Guid?)dr[0]);
				specialcalparam.ScpName = (string)(dr[1]);
				specialcalparam.ScpUnits = (string)(dr[2]);
				specialcalparam.ScpReportOrder = (short?)(dr[3]);
				specialcalparam.ScpUsedInOutage = (bool)(dr[4]);
				specialcalparam.ScpIsLclChg = (bool)(dr[5]);
				specialcalparam.ScpIsActive = (bool)(dr[6]);

				specialcalparams.Add(specialcalparam);	
			}
			// Finish up
			dr.Close();
			return specialcalparams;
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
					ScpDBid as ID,
					ScpName as SpecialCalParamName,
					ScpUnits as SpecialCalParamUnits,
					ScpReportOrder as SpecialCalParamReportOrder,
					CASE
						WHEN ScpUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as SpecialCalParamUsedInOutage,
					CASE
						WHEN ScpIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as SpecialCalParamIsLclChg,
					CASE
						WHEN ScpIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as SpecialCalParamIsActive
					from SpecialCalParams
					order by ScpReportOrder";
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
				cmd.CommandText = "Select ScpIsActive from SpecialCalParams where ScpName = @p1";
			}
			else
			{
				cmd.CommandText = "Select ScpIsActive from SpecialCalParams where ScpName = @p1 and ScpDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;
			return exists;
		}

		private short getNewReportOrder()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.CommandText = "Select Max(ScpReportOrder) from SpecialCalParams";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			short newReportOrder = (short)(val == null ? 0 : Convert.ToUInt16(val) + 1);
			return newReportOrder;
		}

		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (SpecialCalParamName == null)
			{
				ScpNameErrMsg = "A unique SpecialCalParam Name is required";
				allFilled = false;
			}
			else
			{
				ScpNameErrMsg = null;
			}
			if (SpecialCalParamUnits == null)
			{
				ScpUnitsErrMsg = "A SpecialCalParam Units is required";
				allFilled = false;
			}
			else
			{
				ScpUnitsErrMsg = null;
			}
			if (SpecialCalParamReportOrder == null)
			{
				ScpReportOrderErrMsg = "A SpecialCalParam ReportOrder is required";
				allFilled = false;
			}
			else
			{
				ScpReportOrderErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// SpecialCalParam Collection class
	//--------------------------------------
	public class ESpecialCalParamCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ESpecialCalParamCollection()
		{ }
		//the indexer of the collection
		public ESpecialCalParam this[int index]
		{
			get
			{
				return (ESpecialCalParam)this.List[index];
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
			foreach (ESpecialCalParam specialcalparam in InnerList)
			{
				if (specialcalparam.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ESpecialCalParam item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ESpecialCalParam item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ESpecialCalParam item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ESpecialCalParam item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
