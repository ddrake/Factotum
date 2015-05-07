using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;
using System.Drawing;

namespace Factotum{

	public class EBoundaryPoint : IEntity
	{
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? BptDBid;
		private Guid? BptBdrID;
		private int BptX;
		private int BptY;
		private byte BptIdx;
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return BptDBid; }
		}

		public Guid? BoundaryPointBdrID
		{
			get { return BptBdrID; }
			set { BptBdrID = value; }
		}

		public int BoundaryPointX
		{
			get { return BptX; }
			set { BptX = value; }
		}

		public int BoundaryPointY
		{
			get { return BptY; }
			set { BptY = value; }
		}

		public byte BoundaryPointIdx
		{
			get { return BptIdx; }
			set { BptIdx = value; }
		}



		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EBoundaryPoint()
		{
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EBoundaryPoint(Guid? id) : this()
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
				BptDBid,
				BptBdrID,
				BptX,
				BptY,
				BptIdx
				from BoundaryPoints
				where BptDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				BptDBid = (Guid?)dr[0];
				BptBdrID = (Guid?)dr[1];
				BptX = (int)dr[2];
				BptY = (int)dr[3];
				BptIdx = (byte)dr[4];
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
				BptDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", BptDBid),
					new SqlCeParameter("@p1", BptBdrID),
					new SqlCeParameter("@p2", BptX),
					new SqlCeParameter("@p3", BptY),
					new SqlCeParameter("@p4", BptIdx)
					});
				cmd.CommandText = @"Insert Into BoundaryPoints (
					BptDBid,
					BptBdrID,
					BptX,
					BptY,
					BptIdx
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert BoundaryPoints row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", BptDBid),
					new SqlCeParameter("@p1", BptBdrID),
					new SqlCeParameter("@p2", BptX),
					new SqlCeParameter("@p3", BptY),
					new SqlCeParameter("@p4", BptIdx)});

				cmd.CommandText =
					@"Update BoundaryPoints 
					set					
					BptBdrID = @p1,					
					BptX = @p2,					
					BptY = @p3,					
					BptIdx = @p4
					Where BptDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update boundarypoints row");
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
			if (BptDBid == null)
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
					@"Delete from BoundaryPoints 
					where BptDBid = @p0";
				cmd.Parameters.Add("@p0", BptDBid);
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
		// Static listing methods which return collections of boundarypoints
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EBoundaryPointCollection ListByIndex()
		{
			EBoundaryPoint boundarypoint;
			EBoundaryPointCollection boundarypoints = new EBoundaryPointCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				BptDBid,
				BptBdrID,
				BptX,
				BptY,
				BptIdx
				from BoundaryPoints";
			qry += "	order by BptIdx";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				boundarypoint = new EBoundaryPoint((Guid?)dr[0]);
				boundarypoint.BptBdrID = (Guid?)(dr[1]);
				boundarypoint.BptX = (int)(dr[2]);
				boundarypoint.BptY = (int)(dr[3]);
				boundarypoint.BptIdx = (byte)(dr[4]);

				boundarypoints.Add(boundarypoint);
			}
			// Finish up
			dr.Close();
			return boundarypoints;
		}

		public static PointF[] GetArrayByIndexForBoundary(Guid BoundaryID)
		{
			ArrayList al = new ArrayList(10);

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				BptX,
				BptY
				from BoundaryPoints
				where BptBdrID = @p1";
			qry += "	order by BptIdx";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", BoundaryID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				PointF bPoint = new PointF(Convert.ToSingle(dr[0]), Convert.ToSingle(dr[1]));
				al.Add(bPoint);
			}
			// Finish up
			dr.Close();
			return (PointF[])al.ToArray(typeof(PointF));
		}

		public static bool SaveForBoundaryFromArray(Guid BoundaryID, PointF[] ptArray)
		{

			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			string qry = @"Insert into BoundaryPoints 
				(BptBdrID, BptX, BptY, BptIdx)
				values (@p1, @p2, @p3, @p4)";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", BoundaryID);
			cmd.Parameters.Add("@p2", 0.0);
			cmd.Parameters.Add("@p3", 0.0);
			cmd.Parameters.Add("@p4", 0);


			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();

			// Build new objects and add them to the collection
			int nPoints = ptArray.Length;
			for (int i=0;i < nPoints; i++)
			{
				cmd.Parameters[1].Value = ptArray[i].X;
				cmd.Parameters[2].Value = ptArray[i].Y;
				cmd.Parameters[3].Value = i;
				cmd.ExecuteNonQuery();
			}
			return true;
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
					BptDBid as ID,
					BptBdrID as BoundaryPointBdrID,
					BptX as BoundaryPointX,
					BptY as BoundaryPointY,
					BptIdx as BoundaryPointIdx
					from BoundaryPoints";
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
	// BoundaryPoint Collection class
	//--------------------------------------
	public class EBoundaryPointCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EBoundaryPointCollection()
		{ }
		//the indexer of the collection
		public EBoundaryPoint this[int index]
		{
			get
			{
				return (EBoundaryPoint)this.List[index];
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
			foreach (EBoundaryPoint boundarypoint in InnerList)
			{
				if (boundarypoint.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EBoundaryPoint item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EBoundaryPoint item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EBoundaryPoint item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EBoundaryPoint item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
