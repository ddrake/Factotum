using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ESurvey : IEntity
	{
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? SvyDBid;
		private Guid? SvyDstID;
		private short? SvyNumber;
		private string SvyTransducer;
		private decimal? SvyVelocity;
		private short? SvyGainDb;
		private string SvyUnits;

		// Textbox limits
		public static int SvyNumberCharLimit = 6;
		public static int SvyTransducerCharLimit = 10;
		public static int SvyVelocityCharLimit = 8;
		public static int SvyGainDbCharLimit = 6;
		public static int SvyUnitsCharLimit = 2;
		
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return SvyDBid; }
		}

		public Guid? SurveyDstID
		{
			get { return SvyDstID; }
			set { SvyDstID = value; }
		}

		public short? SurveyNumber
		{
			get { return SvyNumber; }
			set { SvyNumber = value; }
		}

		public string SurveyTransducer
		{
			get { return SvyTransducer; }
			set { SvyTransducer = Util.NullifyEmpty(value); }
		}

		public decimal? SurveyVelocity
		{
			get { return SvyVelocity; }
			set { SvyVelocity = value; }
		}

		public short? SurveyGainDb
		{
			get { return SvyGainDb; }
			set { SvyGainDb = value; }
		}

		public string SurveyUnits
		{
			get { return SvyUnits; }
			set { SvyUnits = Util.NullifyEmpty(value); }
		}


		//--------------------------------------
		// NO Textbox Name Length Validation
		//--------------------------------------

		//--------------------------------------
		// NO Field-Specific Validation
		//--------------------------------------

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ESurvey()
		{
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ESurvey(Guid? id) : this()
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
				SvyDBid,
				SvyDstID,
				SvyNumber,
				SvyTransducer,
				SvyVelocity,
				SvyGainDb,
				SvyUnits
				from Surveys
				where SvyDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				SvyDBid = (Guid?)dr[0];
				SvyDstID = (Guid?)dr[1];
				SvyNumber = (short?)Util.NullForDbNull(dr[2]);
				SvyTransducer = (string)Util.NullForDbNull(dr[3]);
				SvyVelocity = (decimal?)Util.NullForDbNull(dr[4]);
				SvyGainDb = (short?)Util.NullForDbNull(dr[5]);
				SvyUnits = (string)Util.NullForDbNull(dr[6]);
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
				SvyDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", SvyDBid),
					new SqlCeParameter("@p1", SvyDstID),
					new SqlCeParameter("@p2", Util.DbNullForNull(SvyNumber)),
					new SqlCeParameter("@p3", Util.DbNullForNull(SvyTransducer)),
					new SqlCeParameter("@p4", Util.DbNullForNull(SvyVelocity)),
					new SqlCeParameter("@p5", Util.DbNullForNull(SvyGainDb)),
					new SqlCeParameter("@p6", Util.DbNullForNull(SvyUnits))
					});
				cmd.CommandText = @"Insert Into Surveys (
					SvyDBid,
					SvyDstID,
					SvyNumber,
					SvyTransducer,
					SvyVelocity,
					SvyGainDb,
					SvyUnits
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Surveys row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", SvyDBid),
					new SqlCeParameter("@p1", SvyDstID),
					new SqlCeParameter("@p2", Util.DbNullForNull(SvyNumber)),
					new SqlCeParameter("@p3", Util.DbNullForNull(SvyTransducer)),
					new SqlCeParameter("@p4", Util.DbNullForNull(SvyVelocity)),
					new SqlCeParameter("@p5", Util.DbNullForNull(SvyGainDb)),
					new SqlCeParameter("@p6", Util.DbNullForNull(SvyUnits))});

				cmd.CommandText =
					@"Update Surveys 
					set					
					SvyDstID = @p1,					
					SvyNumber = @p2,					
					SvyTransducer = @p3,					
					SvyVelocity = @p4,					
					SvyGainDb = @p5,					
					SvyUnits = @p6
					Where SvyDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update surveys row");
				}
			}
			return ID;
		}

		//--------------------------------------
		// Validate the current record
		//--------------------------------------
		// Make this public so that the UI can check validation itself
		// if it chooses to do so.  This is also called by the Save function.
		public bool Valid()
		{
			// We don't need validation.  It is the responsibility of the textfile parser.
			return true;
		}

		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{

			// If the current object doesn't reference a database record, there's nothing to do.
			if (SvyDBid == null)
			{
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
					@"Delete from Surveys 
					where SvyDBid = @p0";
				cmd.Parameters.Add("@p0", SvyDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		//--------------------------------------
		// Delete all Surveys for the given Dset
		//--------------------------------------
		public static bool DeleteAllForDset(Guid? DsetID, bool promptUser)
		{
			// If the current object doesn't reference a database record, there's nothing to do.
			// this shouldn't happen.
			if (DsetID == null)
			{
				return false;
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select SvyDBid from Surveys 
					where SvyDstID = @p1";
			cmd.Parameters.Add("@p1", DsetID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();

			// If the current dset doesn't have any surveys, there's nothing to do.
			if (cmd.ExecuteScalar() == null)
				return true;

			DialogResult rslt = DialogResult.None;
			if (promptUser)
			{
				rslt = MessageBox.Show("Are you sure you want to delete the currently imported textfile data?",
				"Factotum: Delete Imported Data?",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			}




			if (!promptUser || rslt == DialogResult.OK)
			{
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from Surveys 
					where SvyDstID = @p1";
				cmd.Parameters.Add("@p1", DsetID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					return false;
				}
				else
					return true;
			}
			else
			{
				return false;
			}
		}
		//--------------------------------------------------------------------
		// Static listing methods which return collections of surveys
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ESurveyCollection List()
		{
			ESurvey survey;
			ESurveyCollection surveys = new ESurveyCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				SvyDBid,
				SvyDstID,
				SvyNumber,
				SvyTransducer,
				SvyVelocity,
				SvyGainDb,
				SvyUnits
				from Surveys";

			cmd.CommandText = qry;
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				survey = new ESurvey((Guid?)dr[0]);
				survey.SvyDstID = (Guid?)(dr[1]);
				survey.SvyNumber = (short?)Util.NullForDbNull(dr[2]);
				survey.SvyTransducer = (string)Util.NullForDbNull(dr[3]);
				survey.SvyVelocity = (decimal?)Util.NullForDbNull(dr[4]);
				survey.SvyGainDb = (short?)Util.NullForDbNull(dr[5]);
				survey.SvyUnits = (string)Util.NullForDbNull(dr[6]);

				surveys.Add(survey);	
			}
			// Finish up
			dr.Close();
			return surveys;
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
					SvyDBid as ID,
					SvyDstID as SurveyDstID,
					SvyNumber as SurveyNumber,
					SvyTransducer as SurveyTransducer,
					SvyVelocity as SurveyVelocity,
					SvyGainDb as SurveyGainDb,
					SvyUnits as SurveyUnits
					from Surveys";
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

		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			return allFilled;
		}
	}

	//--------------------------------------
	// Survey Collection class
	//--------------------------------------
	public class ESurveyCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ESurveyCollection()
		{ }
		//the indexer of the collection
		public ESurvey this[int index]
		{
			get
			{
				return (ESurvey)this.List[index];
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
			foreach (ESurvey survey in InnerList)
			{
				if (survey.ID == ID)
					return true;
			}
			return false;
		}

		public ESurvey FindFirstForSurveyNumber(short? SurveyNumber)
		{
			if (SurveyNumber == null) return null;
			foreach (ESurvey survey in InnerList)
			{
				if (survey.SurveyNumber == SurveyNumber)
					return survey;
			}
			return null;
		}

		//returns the index of an item in the collection
		public int IndexOf(ESurvey item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ESurvey item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ESurvey item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ESurvey item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
