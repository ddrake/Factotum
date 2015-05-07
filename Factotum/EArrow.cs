using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EArrow : IEntity
	{
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? AroDBid;
		private Guid? AroDctID;
		private int AroShaftWidth;
		private int AroBarb;
		private int AroTip;
		private int AroStartX;
		private int AroStartY;
		private int AroEndX;
		private int AroEndY;
		private byte AroHeadCount;
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return AroDBid; }
		}

		public Guid? ArrowDctID
		{
			get { return AroDctID; }
			set { AroDctID = value; }
		}

		public int ArrowShaftWidth
		{
			get { return AroShaftWidth; }
			set { AroShaftWidth = value; }
		}

		public int ArrowBarb
		{
			get { return AroBarb; }
			set { AroBarb = value; }
		}

		public int ArrowTip
		{
			get { return AroTip; }
			set { AroTip = value; }
		}

		public int ArrowStartX
		{
			get { return AroStartX; }
			set { AroStartX = value; }
		}

		public int ArrowStartY
		{
			get { return AroStartY; }
			set { AroStartY = value; }
		}

		public int ArrowEndX
		{
			get { return AroEndX; }
			set { AroEndX = value; }
		}

		public int ArrowEndY
		{
			get { return AroEndY; }
			set { AroEndY = value; }
		}

		public byte ArrowHeadCount
		{
			get { return AroHeadCount; }
			set { AroHeadCount = value; }
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EArrow()
		{
			this.AroShaftWidth = 0;
			this.AroBarb = 0;
			this.AroTip = 0;
			this.AroStartX = 0;
			this.AroStartY = 0;
			this.AroEndX = 0;
			this.AroEndY = 0;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EArrow(Guid? id) : this()
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
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
				@"Select 
				AroDBid,
				AroDctID,
				AroShaftWidth,
				AroBarb,
				AroTip,
				AroStartX,
				AroStartY,
				AroEndX,
				AroEndY,
				AroHeadCount
				from Arrows
				where AroDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				AroDBid = (Guid?)dr[0];
				AroDctID = (Guid?)dr[1];
				AroShaftWidth = (int)dr[2];
				AroBarb = (int)dr[3];
				AroTip = (int)dr[4];
				AroStartX = (int)dr[5];
				AroStartY = (int)dr[6];
				AroEndX = (int)dr[7];
				AroEndY = (int)dr[8];
				AroHeadCount = (byte)dr[9];
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
				AroDBid,
				AroDctID,
				AroShaftWidth,
				AroBarb,
				AroTip,
				AroStartX,
				AroStartY,
				AroEndX,
				AroEndY,
				AroHeadCount
				from Arrows
				where AroDctID = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", DrawingControlID));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				AroDBid = (Guid?)dr[0];
				AroDctID = (Guid?)dr[1];
				AroShaftWidth = (int)dr[2];
				AroBarb = (int)dr[3];
				AroTip = (int)dr[4];
				AroStartX = (int)dr[5];
				AroStartY = (int)dr[6];
				AroEndX = (int)dr[7];
				AroEndY = (int)dr[8];
				AroHeadCount = (byte)dr[9];
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
				AroDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", AroDBid),
					new SqlCeParameter("@p1", AroDctID),
					new SqlCeParameter("@p2", AroShaftWidth),
					new SqlCeParameter("@p3", AroBarb),
					new SqlCeParameter("@p4", AroTip),
					new SqlCeParameter("@p5", AroStartX),
					new SqlCeParameter("@p6", AroStartY),
					new SqlCeParameter("@p7", AroEndX),
					new SqlCeParameter("@p8", AroEndY),
					new SqlCeParameter("@p9", AroHeadCount)
					});
				cmd.CommandText = @"Insert Into Arrows (
					AroDBid,
					AroDctID,
					AroShaftWidth,
					AroBarb,
					AroTip,
					AroStartX,
					AroStartY,
					AroEndX,
					AroEndY,
					AroHeadCount
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Arrows row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", AroDBid),
					new SqlCeParameter("@p1", AroDctID),
					new SqlCeParameter("@p2", AroShaftWidth),
					new SqlCeParameter("@p3", AroBarb),
					new SqlCeParameter("@p4", AroTip),
					new SqlCeParameter("@p5", AroStartX),
					new SqlCeParameter("@p6", AroStartY),
					new SqlCeParameter("@p7", AroEndX),
					new SqlCeParameter("@p8", AroEndY),
					new SqlCeParameter("@p9", AroHeadCount)});

				cmd.CommandText =
					@"Update Arrows 
					set					
					AroDctID = @p1,					
					AroShaftWidth = @p2,					
					AroBarb = @p3,					
					AroTip = @p4,					
					AroStartX = @p5,					
					AroStartY = @p6,					
					AroEndX = @p7,					
					AroEndY = @p8,					
					AroHeadCount = @p9
					Where AroDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update arrows row");
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
			if (AroDBid == null)
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
					@"Delete from Arrows 
					where AroDBid = @p0";
				cmd.Parameters.Add("@p0", AroDBid);
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
		// Static listing methods which return collections of arrows
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EArrowCollection List()
		{
			EArrow arrow;
			EArrowCollection arrows = new EArrowCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				AroDBid,
				AroDctID,
				AroShaftWidth,
				AroBarb,
				AroTip,
				AroStartX,
				AroStartY,
				AroEndX,
				AroEndY,
				AroHeadCount
				from Arrows";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				arrow = new EArrow((Guid?)dr[0]);
				arrow.AroDctID = (Guid?)(dr[1]);
				arrow.AroShaftWidth = (int)(dr[2]);
				arrow.AroBarb = (int)(dr[3]);
				arrow.AroTip = (int)(dr[4]);
				arrow.AroStartX = (int)(dr[5]);
				arrow.AroStartY = (int)(dr[6]);
				arrow.AroEndX = (int)(dr[7]);
				arrow.AroEndY = (int)(dr[8]);
				arrow.AroHeadCount = (byte)(dr[9]);

				arrows.Add(arrow);	
			}
			// Finish up
			dr.Close();
			return arrows;
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
					AroDBid as ID,
					AroDctID as ArrowDctID,
					AroShaftWidth as ArrowShaftWidth,
					AroBarb as ArrowBarb,
					AroTip as ArrowTip,
					AroStartX as ArrowStartX,
					AroStartY as ArrowStartY,
					AroEndX as ArrowEndX,
					AroEndY as ArrowEndY,
					AroHeadCount as ArrowHeadCount
					from Arrows";
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
	// Arrow Collection class
	//--------------------------------------
	public class EArrowCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EArrowCollection()
		{ }
		//the indexer of the collection
		public EArrow this[int index]
		{
			get
			{
				return (EArrow)this.List[index];
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
			foreach (EArrow arrow in InnerList)
			{
				if (arrow.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EArrow item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EArrow item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EArrow item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EArrow item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
