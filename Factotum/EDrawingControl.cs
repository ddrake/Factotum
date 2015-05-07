using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public enum EDrawingControlType
	{
		Arrow,
		Notation,
		Boundary
	}
	public class EDrawingControl : IEntity
	{
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? DctDBid;
		private Guid? DctGphID;
		private byte DctType;
		private bool DctHasText;
		private bool DctHasStroke;
		private bool DctHasFill;
		private bool DctHasTranspBackground;
		private int DctFillColor;
		private int DctStrokeColor;
		private int DctTextColor;
		private int DctStroke;
		private int DctZindex;
		private string DctText;
		private string DctFontFamily;
		private float DctFontPoints;
		private bool DctFontIsBold;
		private bool DctFontIsItalic;
		private bool DctFontIsUnderlined;
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return DctDBid; }
		}

		public Guid? DrawingControlGphID
		{
			get { return DctGphID; }
			set { DctGphID = value; }
		}

		public byte DrawingControlType
		{
			get { return DctType; }
			set { DctType = value; }
		}

		public bool DrawingControlHasText
		{
			get { return DctHasText; }
			set { DctHasText = value; }
		}

		public bool DrawingControlHasStroke
		{
			get { return DctHasStroke; }
			set { DctHasStroke = value; }
		}

		public bool DrawingControlHasFill
		{
			get { return DctHasFill; }
			set { DctHasFill = value; }
		}

		public bool DrawingControlHasTranspBackground
		{
			get { return DctHasTranspBackground; }
			set { DctHasTranspBackground = value; }
		}

		public int DrawingControlFillColor
		{
			get { return DctFillColor; }
			set { DctFillColor = value; }
		}

		public int DrawingControlStrokeColor
		{
			get { return DctStrokeColor; }
			set { DctStrokeColor = value; }
		}

		public int DrawingControlTextColor
		{
			get { return DctTextColor; }
			set { DctTextColor = value; }
		}

		public int DrawingControlStroke
		{
			get { return DctStroke; }
			set { DctStroke = value; }
		}

		public int DrawingControlZindex
		{
			get { return DctZindex; }
			set { DctZindex = value; }
		}

		public string DrawingControlText
		{
			get { return DctText; }
			set { DctText = Util.NullifyEmpty(value); }
		}

		public string DrawingControlFontFamily
		{
			get { return DctFontFamily; }
			set { DctFontFamily = Util.NullifyEmpty(value); }
		}

		public float DrawingControlFontPoints
		{
			get { return DctFontPoints; }
			set { DctFontPoints = value; }
		}

		public bool DrawingControlFontIsBold
		{
			get { return DctFontIsBold; }
			set { DctFontIsBold = value; }
		}

		public bool DrawingControlFontIsItalic
		{
			get { return DctFontIsItalic; }
			set { DctFontIsItalic = value; }
		}

		public bool DrawingControlFontIsUnderlined
		{
			get { return DctFontIsUnderlined; }
			set { DctFontIsUnderlined = value; }
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EDrawingControl()
		{
			this.DctHasText = false;
			this.DctHasStroke = false;
			this.DctHasFill = false;
			this.DctHasTranspBackground = false;
			this.DctFillColor = 0;
			this.DctStrokeColor = 0;
			this.DctTextColor = 0;
			this.DctStroke = 1;
			this.DctZindex = 0;
			this.DctFontPoints = 0;
			this.DctFontIsBold = false;
			this.DctFontIsItalic = false;
			this.DctFontIsUnderlined = false;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EDrawingControl(Guid? id) : this()
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
				DctDBid,
				DctGphID,
				DctType,
				DctHasText,
				DctHasStroke,
				DctHasFill,
				DctHasTranspBackground,
				DctFillColor,
				DctStrokeColor,
				DctTextColor,
				DctStroke,
				DctZindex,
				DctText,
				DctFontFamily,
				DctFontPoints,
				DctFontIsBold,
				DctFontIsItalic,
				DctFontIsUnderlined
				from DrawingControls
				where DctDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				DctDBid = (Guid?)dr[0];
				DctGphID = (Guid?)dr[1];
				DctType = (byte)dr[2];
				DctHasText = (bool)dr[3];
				DctHasStroke = (bool)dr[4];
				DctHasFill = (bool)dr[5];
				DctHasTranspBackground = (bool)dr[6];
				DctFillColor = (int)dr[7];
				DctStrokeColor = (int)dr[8];
				DctTextColor = (int)dr[9];
				DctStroke = (int)dr[10];
				DctZindex = (int)dr[11];
				DctText = (string)Util.NullForDbNull(dr[12]);
				DctFontFamily = (string)dr[13];
				DctFontPoints = Convert.ToSingle(dr[14]);
				DctFontIsBold = (bool)dr[15];
				DctFontIsItalic = (bool)dr[16];
				DctFontIsUnderlined = (bool)dr[17];
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
				DctDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", DctDBid),
					new SqlCeParameter("@p1", DctGphID),
					new SqlCeParameter("@p2", DctType),
					new SqlCeParameter("@p3", DctHasText),
					new SqlCeParameter("@p4", DctHasStroke),
					new SqlCeParameter("@p5", DctHasFill),
					new SqlCeParameter("@p6", DctHasTranspBackground),
					new SqlCeParameter("@p7", DctFillColor),
					new SqlCeParameter("@p8", DctStrokeColor),
					new SqlCeParameter("@p9", DctTextColor),
					new SqlCeParameter("@p10", DctStroke),
					new SqlCeParameter("@p11", DctZindex),
					new SqlCeParameter("@p12", Util.DbNullForNull(DctText)),
					new SqlCeParameter("@p13", DctFontFamily),
					new SqlCeParameter("@p14", DctFontPoints),
					new SqlCeParameter("@p15", DctFontIsBold),
					new SqlCeParameter("@p16", DctFontIsItalic),
					new SqlCeParameter("@p17", DctFontIsUnderlined)
					});
				cmd.CommandText = @"Insert Into DrawingControls (
					DctDBid,
					DctGphID,
					DctType,
					DctHasText,
					DctHasStroke,
					DctHasFill,
					DctHasTranspBackground,
					DctFillColor,
					DctStrokeColor,
					DctTextColor,
					DctStroke,
					DctZindex,
					DctText,
					DctFontFamily,
					DctFontPoints,
					DctFontIsBold,
					DctFontIsItalic,
					DctFontIsUnderlined
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert DrawingControls row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", DctDBid),
					new SqlCeParameter("@p1", DctGphID),
					new SqlCeParameter("@p2", DctType),
					new SqlCeParameter("@p3", DctHasText),
					new SqlCeParameter("@p4", DctHasStroke),
					new SqlCeParameter("@p5", DctHasFill),
					new SqlCeParameter("@p6", DctHasTranspBackground),
					new SqlCeParameter("@p7", DctFillColor),
					new SqlCeParameter("@p8", DctStrokeColor),
					new SqlCeParameter("@p9", DctTextColor),
					new SqlCeParameter("@p10", DctStroke),
					new SqlCeParameter("@p11", DctZindex),
					new SqlCeParameter("@p12", Util.DbNullForNull(DctText)),
					new SqlCeParameter("@p13", DctFontFamily),
					new SqlCeParameter("@p14", DctFontPoints),
					new SqlCeParameter("@p15", DctFontIsBold),
					new SqlCeParameter("@p16", DctFontIsItalic),
					new SqlCeParameter("@p17", DctFontIsUnderlined)});

				cmd.CommandText =
					@"Update DrawingControls 
					set					
					DctGphID = @p1,					
					DctType = @p2,					
					DctHasText = @p3,					
					DctHasStroke = @p4,					
					DctHasFill = @p5,					
					DctHasTranspBackground = @p6,					
					DctFillColor = @p7,					
					DctStrokeColor = @p8,					
					DctTextColor = @p9,					
					DctStroke = @p10,					
					DctZindex = @p11,					
					DctText = @p12,					
					DctFontFamily = @p13,					
					DctFontPoints = @p14,					
					DctFontIsBold = @p15,					
					DctFontIsItalic = @p16,					
					DctFontIsUnderlined = @p17
					Where DctDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update drawingcontrols row");
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
			if (DctDBid == null)
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
					@"Delete from DrawingControls 
					where DctDBid = @p0";
				cmd.Parameters.Add("@p0", DctDBid);
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

		public static bool DeleteAllForGraphic(Guid GraphicID)
		{

			// If an error occurs when attempting to delete...
			// Use transactions??
			// Raise an event right before the deletion?
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
				@"Delete from DrawingControls 
				where DctGphID = @p0";
			cmd.Parameters.Add("@p0", GraphicID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			int rowsAffected = cmd.ExecuteNonQuery();
			
			return true;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of drawingcontrols
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EDrawingControlCollection ListForGraphicByZindex(Guid? GraphicID)
		{
			EDrawingControl drawingcontrol;
			EDrawingControlCollection drawingcontrols = new EDrawingControlCollection();

			if (GraphicID == null) return drawingcontrols;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				DctDBid,
				DctGphID,
				DctType,
				DctHasText,
				DctHasStroke,
				DctHasFill,
				DctHasTranspBackground,
				DctFillColor,
				DctStrokeColor,
				DctTextColor,
				DctStroke,
				DctZindex,
				DctText,
				DctFontFamily,
				DctFontPoints,
				DctFontIsBold,
				DctFontIsItalic,
				DctFontIsUnderlined
				from DrawingControls
				Where DctGphID = @p1
				Order by DctZindex";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", (Guid)GraphicID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				drawingcontrol = new EDrawingControl((Guid?)dr[0]);
				drawingcontrol.DctGphID = (Guid?)(dr[1]);
				drawingcontrol.DctType = (byte)(dr[2]);
				drawingcontrol.DctHasText = (bool)(dr[3]);
				drawingcontrol.DctHasStroke = (bool)(dr[4]);
				drawingcontrol.DctHasFill = (bool)(dr[5]);
				drawingcontrol.DctHasTranspBackground = (bool)(dr[6]);
				drawingcontrol.DctFillColor = (int)(dr[7]);
				drawingcontrol.DctStrokeColor = (int)(dr[8]);
				drawingcontrol.DctTextColor = (int)(dr[9]);
				drawingcontrol.DctStroke = (int)(dr[10]);
				drawingcontrol.DctZindex = (int)(dr[11]);
				drawingcontrol.DctText = (string)Util.NullForDbNull(dr[12]);
				drawingcontrol.DctFontFamily = (string)(dr[13]);
				drawingcontrol.DctFontPoints = Convert.ToSingle(dr[14]);
				drawingcontrol.DctFontIsBold = (bool)(dr[15]);
				drawingcontrol.DctFontIsItalic = (bool)(dr[16]);
				drawingcontrol.DctFontIsUnderlined = (bool)(dr[17]);

				drawingcontrols.Add(drawingcontrol);	
			}
			// Finish up
			dr.Close();
			return drawingcontrols;
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
					DctDBid as ID,
					DctGphID as DrawingControlGphID,
					DctType as DrawingControlType,
					CASE
						WHEN DctHasText = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlHasText,
					CASE
						WHEN DctHasStroke = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlHasStroke,
					CASE
						WHEN DctHasFill = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlHasFill,
					CASE
						WHEN DctHasTranspBackground = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlHasTranspBackground,
					DctFillColor as DrawingControlFillColor,
					DctStrokeColor as DrawingControlStrokeColor,
					DctTextColor as DrawingControlTextColor,
					DctStroke as DrawingControlStroke,
					DctZindex as DrawingControlZindex,
					DctText as DrawingControlText,
					DctFontFamily as DrawingControlFontFamily,
					DctFontPoints as DrawingControlFontPoints,
					CASE
						WHEN DctFontIsBold = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlFontIsBold,
					CASE
						WHEN DctFontIsItalic = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlFontIsItalic,
					CASE
						WHEN DctFontIsUnderlined = 0 THEN 'No'
						ELSE 'Yes'
					END as DrawingControlFontIsUnderlined
					from DrawingControls";
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
	// DrawingControl Collection class
	//--------------------------------------
	public class EDrawingControlCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EDrawingControlCollection()
		{ }
		//the indexer of the collection
		public EDrawingControl this[int index]
		{
			get
			{
				return (EDrawingControl)this.List[index];
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
			foreach (EDrawingControl drawingcontrol in InnerList)
			{
				if (drawingcontrol.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EDrawingControl item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EDrawingControl item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EDrawingControl item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EDrawingControl item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
