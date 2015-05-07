using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EBoundary : IEntity
	{
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? BdrDBid;
		private Guid? BdrDctID;
		private byte BdrAlpha;
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return BdrDBid; }
		}

		public Guid? BoundaryDctID
		{
			get { return BdrDctID; }
			set { BdrDctID = value; }
		}

		public byte BoundaryAlpha
		{
			get { return BdrAlpha; }
			set { BdrAlpha = value; }
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EBoundary()
		{
			this.BdrAlpha = 0;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EBoundary(Guid? id) : this()
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
				BdrDBid,
				BdrDctID,
				BdrAlpha
				from Boundaries
				where BdrDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				BdrDBid = (Guid?)dr[0];
				BdrDctID = (Guid?)dr[1];
				BdrAlpha = (byte)dr[2];
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
				BdrDBid,
				BdrDctID,
				BdrAlpha
				from Boundaries
				where BdrDctID = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", DrawingControlID));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				BdrDBid = (Guid?)dr[0];
				BdrDctID = (Guid?)dr[1];
				BdrAlpha = (byte)dr[2];
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
				BdrDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", BdrDBid),
					new SqlCeParameter("@p1", BdrDctID),
					new SqlCeParameter("@p2", BdrAlpha)
					});
				cmd.CommandText = @"Insert Into Boundaries (
					BdrDBid,
					BdrDctID,
					BdrAlpha
				) values (@p0,@p1,@p2)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Boundaries row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", BdrDBid),
					new SqlCeParameter("@p1", BdrDctID),
					new SqlCeParameter("@p2", BdrAlpha)
				});

				cmd.CommandText =
					@"Update Boundaries 
					set					
					BdrDctID = @p1,					
					BdrAlpha = @p2					
					Where BdrDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update boundaries row");
				}
			}
			return ID;
		}

		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{
			// If the current object doesn't reference a database record, there's nothing to do.
			if (BdrDBid == null)
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
					@"Delete from Boundaries 
					where BdrDBid = @p0";
				cmd.Parameters.Add("@p0", BdrDBid);
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
		// Static listing methods which return collections of boundaries
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EBoundaryCollection List()
		{
			EBoundary boundary;
			EBoundaryCollection boundarys = new EBoundaryCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				BdrDBid,
				BdrDctID,
				BdrAlpha
				from Boundaries";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				boundary = new EBoundary((Guid?)dr[0]);
				boundary.BdrDctID = (Guid?)(dr[1]);
				boundary.BdrAlpha = (byte)(dr[2]);

				boundarys.Add(boundary);	
			}
			// Finish up
			dr.Close();
			return boundarys;
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
					BdrDBid as ID,
					BdrDctID as BoundaryDctID,
					BdrAlpha as BoundaryAlpha
					from Boundaries";
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
	// Boundary Collection class
	//--------------------------------------
	public class EBoundaryCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EBoundaryCollection()
		{ }
		//the indexer of the collection
		public EBoundary this[int index]
		{
			get
			{
				return (EBoundary)this.List[index];
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
			foreach (EBoundary boundary in InnerList)
			{
				if (boundary.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EBoundary item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EBoundary item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EBoundary item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EBoundary item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
