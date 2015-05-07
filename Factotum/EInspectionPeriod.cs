using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EInspectionPeriod : IEntity
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
		private Guid? IpdDBid;
		private Guid? IpdDstID;
		private DateTime? IpdInAt;
		private DateTime? IpdOutAt;
		private DateTime? IpdCalCheck1At;
		private DateTime? IpdCalCheck2At;
	
		// Form level validation message
		private string IpdErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return IpdDBid; }
		}

		public Guid? InspectionPeriodDstID
		{
			get { return IpdDstID; }
			set { IpdDstID = value; }
		}

		public DateTime? InspectionPeriodInAt
		{
			get { return IpdInAt; }
			set { IpdInAt = value; }
		}

		public DateTime? InspectionPeriodOutAt
		{
			get { return IpdOutAt; }
			set { IpdOutAt = value; }
		}

		public DateTime? InspectionPeriodCalCheck1At
		{
			get { return IpdCalCheck1At; }
			set { IpdCalCheck1At = value; }
		}

		public DateTime? InspectionPeriodCalCheck2At
		{
			get { return IpdCalCheck2At; }
			set { IpdCalCheck2At = value; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string InspectionPeriodErrMsg
		{
			get { return IpdErrMsg; }
			set { IpdErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EInspectionPeriod()
		{
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EInspectionPeriod(Guid? id) : this()
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
				IpdDBid,
				IpdDstID,
				IpdInAt,
				IpdOutAt,
				IpdCalCheck1At,
				IpdCalCheck2At
				from InspectionPeriods
				where IpdDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				IpdDBid = (Guid?)dr[0];
				IpdDstID = (Guid?)dr[1];
				IpdInAt = (DateTime?)dr[2];
				IpdOutAt = (DateTime?)dr[3];
				IpdCalCheck1At = (DateTime?)Util.NullForDbNull(dr[4]);
				IpdCalCheck2At = (DateTime?)Util.NullForDbNull(dr[5]);
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
				IpdDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", IpdDBid),
					new SqlCeParameter("@p1", IpdDstID),
					new SqlCeParameter("@p2", IpdInAt),
					new SqlCeParameter("@p3", IpdOutAt),
					new SqlCeParameter("@p4", Util.DbNullForNull(IpdCalCheck1At)),
					new SqlCeParameter("@p5", Util.DbNullForNull(IpdCalCheck2At))
					});
				cmd.CommandText = @"Insert Into InspectionPeriods (
					IpdDBid,
					IpdDstID,
					IpdInAt,
					IpdOutAt,
					IpdCalCheck1At,
					IpdCalCheck2At
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Inspection Periods row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", IpdDBid),
					new SqlCeParameter("@p1", IpdDstID),
					new SqlCeParameter("@p2", IpdInAt),
					new SqlCeParameter("@p3", IpdOutAt),
					new SqlCeParameter("@p4", Util.DbNullForNull(IpdCalCheck1At)),
					new SqlCeParameter("@p5", Util.DbNullForNull(IpdCalCheck2At))});

				cmd.CommandText =
					@"Update InspectionPeriods 
					set					
					IpdDstID = @p1,					
					IpdInAt = @p2,					
					IpdOutAt = @p3,					
					IpdCalCheck1At = @p4,					
					IpdCalCheck2At = @p5
					Where IpdDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update inspection periods row");
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
			// Check form to make sure all required fields have been filled in
			if (!RequiredFieldsFilled()) return false; // The UI should prevent this..

			// Check for incorrect field interactions...
			if (!SequenceOK())
			{
				IpdErrMsg = "The Inspection Dates/Times are not in the correct sequence.";
				return false;
			}

			return true;
		}

		private bool SequenceOK()
		{
			if (IpdInAt > IpdOutAt) return false;
			if (IpdCalCheck1At != null &&
				(IpdCalCheck1At < IpdInAt || IpdCalCheck1At > IpdOutAt ||
					(IpdCalCheck2At != null && IpdCalCheck1At > IpdCalCheck2At))) return false;
			if (IpdCalCheck2At != null &&
				(IpdCalCheck2At < IpdInAt || IpdCalCheck2At > IpdOutAt)) return false;
			return true;
		}

		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{
			// If the current object doesn't reference a database record, there's nothing to do.
			if (IpdDBid == null)
			{
				InspectionPeriodErrMsg = "Unable to delete. Record not found.";
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
					@"Delete from InspectionPeriods 
					where IpdDBid = @p0";
				cmd.Parameters.Add("@p0", IpdDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					InspectionPeriodErrMsg = "Unable to delete.  Please try again later.";
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
				InspectionPeriodErrMsg = null;
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of inspectionperiods
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EInspectionPeriodCollection ListForDset(Guid? DsetID)
		{
			EInspectionPeriod inspectionperiod;
			EInspectionPeriodCollection inspectionperiods = new EInspectionPeriodCollection();

			if (DsetID == null) return inspectionperiods;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				IpdDBid,
				IpdDstID,
				IpdInAt,
				IpdOutAt,
				IpdCalCheck1At,
				IpdCalCheck2At
				from InspectionPeriods";

			qry += " where IpdDstID = @p1";
			qry += " order by IpdInAt";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", DsetID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				inspectionperiod = new EInspectionPeriod((Guid?)dr[0]);
				inspectionperiod.IpdDstID = (Guid?)(dr[1]);
				inspectionperiod.IpdInAt = (DateTime?)(dr[2]);
				inspectionperiod.IpdOutAt = (DateTime?)(dr[3]);
				inspectionperiod.IpdCalCheck1At = (DateTime?)Util.NullForDbNull(dr[4]);
				inspectionperiod.IpdCalCheck2At = (DateTime?)Util.NullForDbNull(dr[5]);

				inspectionperiods.Add(inspectionperiod);	
			}
			// Finish up
			dr.Close();
			return inspectionperiods;
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
					IpdDBid as ID,
					IpdDstID as InspectionPeriodDstID,
					IpdInAt as InspectionPeriodInAt,
					IpdOutAt as InspectionPeriodOutAt,
					IpdCalCheck1At as InspectionPeriodCalCheck1At,
					IpdCalCheck2At as InspectionPeriodCalCheck2At
					from InspectionPeriods
					where IpdDstID = @p1";
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


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = (IpdInAt != null && IpdOutAt != null);
			return allFilled;
		}
	}

	//--------------------------------------
	// InspectionPeriod Collection class
	//--------------------------------------
	public class EInspectionPeriodCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EInspectionPeriodCollection()
		{ }
		//the indexer of the collection
		public EInspectionPeriod this[int index]
		{
			get
			{
				return (EInspectionPeriod)this.List[index];
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
			foreach (EInspectionPeriod inspectionperiod in InnerList)
			{
				if (inspectionperiod.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EInspectionPeriod item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EInspectionPeriod item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EInspectionPeriod item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EInspectionPeriod item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
