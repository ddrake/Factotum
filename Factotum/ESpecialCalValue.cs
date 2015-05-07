using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum {

	public class ESpecialCalValue : IEntity
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
		private Guid? ScvDBid;
		private Guid? ScvScpID;
		private Guid? ScvDstID;
		private string ScvValue;

		// Textbox limits
		private const int ScvValueCharLimit = 10;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string ScvValueErrMsg;
		private string ScvScpErrMsg;

		// Form level validation message
		private string ScvErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return ScvDBid; }
		}

		public Guid? SpecialCalValueScpID
		{
			get { return ScvScpID; }
			set { ScvScpID = value; }
		}

		public Guid? SpecialCalValueDstID
		{
			get { return ScvDstID; }
			set { ScvDstID = value; }
		}

		public string SpecialCalValueValue
		{
			get { return ScvValue; }
			set { ScvValue = Util.NullifyEmpty(value); }
		}

		public string SpecialCalValueParameterName
		{
			get
			{
				if (ScvScpID == null)
				{
					return null;
				}
				else
				{
					ESpecialCalParam specialCalParam = new ESpecialCalParam(ScvScpID);
					return specialCalParam.SpecialCalParamName;
				}
			}
		}

		public string SpecialCalValueUnits
		{
			get
			{
				if (ScvScpID == null)
				{
					return null;
				}
				else
				{
					ESpecialCalParam specialCalParam = new ESpecialCalParam(ScvScpID);
					return specialCalParam.SpecialCalParamUnits;
				}
			}
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string SpecialCalValueValueErrMsg
		{
			get { return ScvValueErrMsg; }
		}

		public string SpecialCalValueScpErrMsg
		{
			get { return ScvScpErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string SpecialCalValueErrMsg
		{
			get { return ScvErrMsg; }
			set { ScvErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool SpecialCalValueValueLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > ScvValueCharLimit)
			{
				ScvValueErrMsg = string.Format("SpecialCalValueValues cannot exceed {0} characters", ScvValueCharLimit);
				return false;
			}
			else
			{
				ScvValueErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool SpecialCalValueValueValid(string value)
		{
			if (!SpecialCalValueValueLengthOk(value)) return false;

			ScvValueErrMsg = null;
			return true;
		}

		public bool SpecialCalValueScpIDValid(Guid? value)
		{
			if (CalParamAssignedToDset(value, ScvDBid))
			{
				ScvScpErrMsg = "That Parameter has already been assigned a value.";
				return false;
			}
			ScvScpErrMsg = null;
			return true;

		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ESpecialCalValue()
		{
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ESpecialCalValue(Guid? id) : this()
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
				ScvDBid,
				ScvScpID,
				ScvDstID,
				ScvValue
				from SpecialCalValues
				where ScvDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				ScvDBid = (Guid?)dr[0];
				ScvScpID = (Guid?)dr[1];
				ScvDstID = (Guid?)dr[2];
				ScvValue = (string)dr[3];
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
				ScvDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ScvDBid),
					new SqlCeParameter("@p1", ScvScpID),
					new SqlCeParameter("@p2", ScvDstID),
					new SqlCeParameter("@p3", ScvValue)
					});
				cmd.CommandText = @"Insert Into SpecialCalValues (
					ScvDBid,
					ScvScpID,
					ScvDstID,
					ScvValue
				) values (@p0,@p1,@p2,@p3)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert SpecialCalValues row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", ScvDBid),
					new SqlCeParameter("@p1", ScvScpID),
					new SqlCeParameter("@p2", ScvDstID),
					new SqlCeParameter("@p3", ScvValue)});

				cmd.CommandText =
					@"Update SpecialCalValues 
					set					
					ScvScpID = @p1,					
					ScvDstID = @p2,					
					ScvValue = @p3
					Where ScvDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update specialcalvalues row");
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
			if (!SpecialCalValueValueValid(SpecialCalValueValue)) return false;

			// First check each field to see if it's valid from the UI perspective
			if (!SpecialCalValueScpIDValid(SpecialCalValueScpID)) return false;

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
			if (ScvDBid == null)
			{
				SpecialCalValueErrMsg = "Unable to delete. Record not found.";
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
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from SpecialCalValues 
					where ScvDBid = @p0";
				cmd.Parameters.Add("@p0", ScvDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					SpecialCalValueErrMsg = "Unable to delete.  Please try again later.";
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
				SpecialCalValueErrMsg = "Deletion cancelled.";
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of specialcalvalues
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ESpecialCalValueCollection ListForDsetByReportOrder(Guid? dsetID)
		{
			ESpecialCalValue specialcalvalue;
			ESpecialCalValueCollection specialcalvalues = new ESpecialCalValueCollection();
			
			if (dsetID != null)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				string qry = @"Select 

					ScvDBid,
					ScvScpID,
					ScvDstID,
					ScvValue
					from SpecialCalValues
					inner join SpecialCalParams on ScvScpID = ScpDBid
					where ScvDstID = @p0";
				qry += "	order by ScpReportOrder";
				cmd.CommandText = qry;
				cmd.Parameters.Add("@p0", dsetID);

				SqlCeDataReader dr;
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				dr = cmd.ExecuteReader();

				// Build new objects and add them to the collection
				while (dr.Read())
				{
					specialcalvalue = new ESpecialCalValue((Guid?)dr[0]);
					specialcalvalue.ScvScpID = (Guid?)(dr[1]);
					specialcalvalue.ScvDstID = (Guid?)(dr[2]);
					specialcalvalue.ScvValue = (string)(dr[3]);

					specialcalvalues.Add(specialcalvalue);	
				}
				// Finish up
				dr.Close();
			}
			return specialcalvalues;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataViewForDset(Guid? DsetID)
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
					ScvDBid as ID,
					ScpName as SpecialCalParamName,
					ScvValue as SpecialCalValueValue,
					ScpUnits as SpecialCalParamUnits
					from SpecialCalValues
					inner join SpecialCalParams on ScvScpID = ScpDBid";
			
			qry += " where ScvDstID = @p1";
			qry += " order by ScpReportOrder";

			if (DsetID == null)
				cmd.Parameters.Add("@p1",Guid.Empty);
			else
				cmd.Parameters.Add(new SqlCeParameter("@p1", DsetID));

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

		// Check if the Calibration parameter has been assigned to any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool CalParamAssignedToDset(Guid? specialCalParamID, Guid? id)
		{
			if (specialCalParamID == null) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", ScvDstID));
			cmd.Parameters.Add(new SqlCeParameter("@p2", specialCalParamID));
			if (id == null)
			{
				cmd.CommandText = 
					@"Select ScvDBid from SpecialCalValues 
					where ScvDstID = @p1 and ScvScpID = @p2";
			}
			else
			{
				cmd.CommandText =
					@"Select ScvDBid from SpecialCalValues 
					where ScvDstID = @p1  and ScvScpID = @p2 and ScvDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			bool exists = (val != null);
			return exists;
		}

		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (SpecialCalValueValue == null)
			{
				ScvValueErrMsg = "A Value is required";
				ScvScpErrMsg = null;
				allFilled = false;
			}
			else if (SpecialCalValueScpID == null)
			{
				ScvValueErrMsg = null;
				ScvScpErrMsg = "A Parameter is required";
				allFilled = false;
			}
			else
			{
				ScvValueErrMsg = null;
				ScvScpErrMsg = null;
				ScvValueErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// SpecialCalValue Collection class
	//--------------------------------------
	public class ESpecialCalValueCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ESpecialCalValueCollection()
		{ }
		//the indexer of the collection
		public ESpecialCalValue this[int index]
		{
			get
			{
				return (ESpecialCalValue)this.List[index];
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
			foreach (ESpecialCalValue specialcalvalue in InnerList)
			{
				if (specialcalvalue.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ESpecialCalValue item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ESpecialCalValue item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ESpecialCalValue item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ESpecialCalValue item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
