using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ENotation : IEntity
	{
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? NtnDBid;
		private Guid? NtnDctID;
		private int NtnTop;
		private int NtnLeft;
		private int NtnWidth;
		private int NtnHeight;
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return NtnDBid; }
		}

		public Guid? NotationDctID
		{
			get { return NtnDctID; }
			set { NtnDctID = value; }
		}

		public int NotationTop
		{
			get { return NtnTop; }
			set { NtnTop = value; }
		}

		public int NotationLeft
		{
			get { return NtnLeft; }
			set { NtnLeft = value; }
		}

		public int NotationWidth
		{
			get { return NtnWidth; }
			set { NtnWidth = value; }
		}

		public int NotationHeight
		{
			get { return NtnHeight; }
			set { NtnHeight = value; }
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ENotation()
		{
			this.NtnTop = 0;
			this.NtnLeft = 0;
			this.NtnWidth = 0;
			this.NtnHeight = 0;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ENotation(Guid? id) : this()
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
				NtnDBid,
				NtnDctID,
				NtnTop,
				NtnLeft,
				NtnWidth,
				NtnHeight
				from Notations
				where NtnDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				NtnDBid = (Guid?)dr[0];
				NtnDctID = (Guid?)dr[1];
				NtnTop = (int)dr[2];
				NtnLeft = (int)dr[3];
				NtnWidth = (int)dr[4];
				NtnHeight = (int)dr[5];
			}
			dr.Close();
		}

		public void LoadForDrawingControl(Guid DrawingControlID)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
				@"Select 
				NtnDBid,
				NtnDctID,
				NtnTop,
				NtnLeft,
				NtnWidth,
				NtnHeight
				from Notations
				where NtnDctID = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", DrawingControlID));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				NtnDBid = (Guid?)dr[0];
				NtnDctID = (Guid?)dr[1];
				NtnTop = (int)dr[2];
				NtnLeft = (int)dr[3];
				NtnWidth = (int)dr[4];
				NtnHeight = (int)dr[5];
			}
			dr.Close();
		}

		//--------------------------------------
		// Save the current record if it's valid
		//--------------------------------------
		public Guid? Save()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				NtnDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", NtnDBid),
					new SqlCeParameter("@p1", NtnDctID),
					new SqlCeParameter("@p2", NtnTop),
					new SqlCeParameter("@p3", NtnLeft),
					new SqlCeParameter("@p4", NtnWidth),
					new SqlCeParameter("@p5", NtnHeight)
					});
				cmd.CommandText = @"Insert Into Notations (
					NtnDBid,
					NtnDctID,
					NtnTop,
					NtnLeft,
					NtnWidth,
					NtnHeight
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Notations row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", NtnDBid),
					new SqlCeParameter("@p1", NtnDctID),
					new SqlCeParameter("@p2", NtnTop),
					new SqlCeParameter("@p3", NtnLeft),
					new SqlCeParameter("@p4", NtnWidth),
					new SqlCeParameter("@p5", NtnHeight)});

				cmd.CommandText =
					@"Update Notations 
					set					
					NtnDctID = @p1,					
					NtnTop = @p2,					
					NtnLeft = @p3,					
					NtnWidth = @p4,					
					NtnHeight = @p5
					Where NtnDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update notations row");
				}
			}
			return ID;
		}


		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{
			// Check for any child tables that reference this record.
			// If any are found, do one of the following

			// 1. Set the form error message, prevent deletion, and return false.
			// 2. Delete the record and all child records and return true.
			// 3. Set the child records to null, delete the record and return true.
			//		note: the relationship may be defined in the schema to accomplish
			//		the setting of child records to null upon delete of the parent.

			// If an error occurs when attempting to delete...

			// Use transactions??

			// Raise an event right before the deletion?

			// In this case, there are no child tables.

			// If the current object doesn't reference a database record, there's nothing to do.
			if (NtnDBid == null)
			{
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
					@"Delete from Notations 
					where NtnDBid = @p0";
				cmd.Parameters.Add("@p0", NtnDBid);
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
		// Static listing methods which return collections of notations
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ENotationCollection List()
		{
			ENotation notation;
			ENotationCollection notations = new ENotationCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				NtnDBid,
				NtnDctID,
				NtnTop,
				NtnLeft,
				NtnWidth,
				NtnHeight
				from Notations";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				notation = new ENotation((Guid?)dr[0]);
				notation.NtnDctID = (Guid?)(dr[1]);
				notation.NtnTop = (int)(dr[2]);
				notation.NtnLeft = (int)(dr[3]);
				notation.NtnWidth = (int)(dr[4]);
				notation.NtnHeight = (int)(dr[5]);

				notations.Add(notation);	
			}
			// Finish up
			dr.Close();
			return notations;
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
					NtnDBid as ID,
					NtnDctID as NotationDctID,
					NtnTop as NotationTop,
					NtnLeft as NotationLeft,
					NtnWidth as NotationWidth,
					NtnHeight as NotationHeight
					from Notations";
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
	// Notation Collection class
	//--------------------------------------
	public class ENotationCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ENotationCollection()
		{ }
		//the indexer of the collection
		public ENotation this[int index]
		{
			get
			{
				return (ENotation)this.List[index];
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
			foreach (ENotation notation in InnerList)
			{
				if (notation.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ENotation item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ENotation item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ENotation item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ENotation item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
