using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using DowUtils;

namespace Factotum{

	public enum BackgroundImageFileType
	{
		Jpeg,
		Gif,
		Png,
		Bmp
	}
	public class EGraphic : IEntity
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
		private Guid? GphDBid;
		private Guid? GphIspID;
		private Image GphImage;
		private byte GphBgImageType;
		private string GphBgImageFileName;

		// Textbox limits
		private const int GphBgImageTypeCharLimit = 3;
		private const int GphBgImageFileNameCharLimit = 255;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string GphBgImageTypeErrMsg;
		private string GphBgImageFileNameErrMsg;

		// Form level validation message
		private string GphErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return GphDBid; }
		}

		public Guid? GraphicIspID
		{
			get { return GphIspID; }
			set { GphIspID = value; }
		}

		public Image GraphicImage
		{
			get { return GphImage; }
			set { GphImage = value; }
		}

		public byte GraphicBgImageType
		{
			get { return GphBgImageType; }
			set { GphBgImageType = value; }
		}

		public string GraphicBgImageFileName
		{
			get { return GphBgImageFileName; }
			set { GphBgImageFileName = Util.NullifyEmpty(value); }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string GraphicBgImageTypeErrMsg
		{
			get { return GphBgImageTypeErrMsg; }
		}

		public string GraphicBgImageFileNameErrMsg
		{
			get { return GphBgImageFileNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string GraphicErrMsg
		{
			get { return GphErrMsg; }
			set { GphErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool GraphicBgImageTypeLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GphBgImageTypeCharLimit)
			{
				GphBgImageTypeErrMsg = string.Format("GraphicBgImageTypes cannot exceed {0} characters", GphBgImageTypeCharLimit);
				return false;
			}
			else
			{
				GphBgImageTypeErrMsg = null;
				return true;
			}
		}

		public bool GraphicBgImageFileNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GphBgImageFileNameCharLimit)
			{
				GphBgImageFileNameErrMsg = string.Format("GraphicBgImageFileNames cannot exceed {0} characters", GphBgImageFileNameCharLimit);
				return false;
			}
			else
			{
				GphBgImageFileNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool GraphicBgImageTypeValid(string value)
		{
			byte result;
			if (byte.TryParse(value, out result) && result > 0)
			{
				GphBgImageTypeErrMsg = null;
				return true;
			}
			GphBgImageTypeErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GraphicBgImageFileNameValid(string value)
		{
			if (!GraphicBgImageFileNameLengthOk(value)) return false;

			GphBgImageFileNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EGraphic()
		{
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EGraphic(Guid? id) : this()
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

			Byte[] barray;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
				@"Select 
				GphDBid,
				GphIspID,
				GphImage,
				GphBgImageType,
				GphBgImageFileName
				from Graphics
				where GphDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				GphDBid = (Guid?)dr[0];
				GphIspID = (Guid?)dr[1];

				barray = (byte[])dr[2];
				MemoryStream ms = new MemoryStream(barray);
				GphImage = new Bitmap(ms);

				GphBgImageType = (byte)dr[3];
				GphBgImageFileName = (string)dr[4];
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
				GphDBid = (Guid?)(cmd.ExecuteScalar());

				MemoryStream ms = new MemoryStream();
				GphImage.Save(ms, ImageFormat.Jpeg);
				Byte[] barray = ms.ToArray();

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GphDBid),
					new SqlCeParameter("@p1", GphIspID),
					new SqlCeParameter("@p2", barray),
					new SqlCeParameter("@p3", GphBgImageType),
					new SqlCeParameter("@p4", GphBgImageFileName)
					});
				cmd.CommandText = @"Insert Into Graphics (
					GphDBid,
					GphIspID,
					GphImage,
					GphBgImageType,
					GphBgImageFileName
				) values (@p0,@p1,@p2,@p3,@p4)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Graphics row");
				}
			}
			else
			{
				// we are updating an existing record
				MemoryStream ms = new MemoryStream();
				GphImage.Save(ms, ImageFormat.Jpeg);
				Byte[] barray = ms.ToArray();
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GphDBid),
					new SqlCeParameter("@p1", GphIspID),
					new SqlCeParameter("@p2", barray),
					new SqlCeParameter("@p3", GphBgImageType),
					new SqlCeParameter("@p4", GphBgImageFileName)});

				cmd.CommandText =
					@"Update Graphics 
					set					
					GphIspID = @p1,					
					GphImage = @p2,					
					GphBgImageType = @p3,					
					GphBgImageFileName = @p4
					Where GphDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update graphics row");
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
			if (!GraphicBgImageFileNameValid(GraphicBgImageFileName)) return false;

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
			if (GphDBid == null)
			{
				GraphicErrMsg = "Unable to delete. Record not found.";
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
					@"Delete from Graphics 
					where GphDBid = @p0";
				cmd.Parameters.Add("@p0", GphDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					GraphicErrMsg = "Unable to delete.  Please try again later.";
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
				GraphicErrMsg = "Deletion cancelled.";
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of graphics
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EGraphicCollection List()
		{
			EGraphic graphic;
			EGraphicCollection graphics = new EGraphicCollection();
			Byte[] barray;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				GphDBid,
				GphIspID,
				GphImage,
				GphBgImageType,
				GphBgImageFileName
				from Graphics";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				graphic = new EGraphic((Guid?)dr[0]);
				graphic.GphIspID = (Guid?)(dr[1]);

				barray = (byte[])dr[2];
				MemoryStream ms = new MemoryStream(barray);
				graphic.GphImage = new Bitmap(ms);

				graphic.GphBgImageType = (byte)(dr[3]);
				graphic.GphBgImageFileName = (string)(dr[4]);

				graphics.Add(graphic);	
			}
			// Finish up
			dr.Close();
			return graphics;
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
					GphDBid as ID,
					GphIspID as GraphicIspID,
					GphImage as GraphicImage,
					GphBgImageType as GraphicBgImageType,
					GphBgImageFileName as GraphicBgImageFileName
					from Graphics";
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
	// Graphic Collection class
	//--------------------------------------
	public class EGraphicCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EGraphicCollection()
		{ }
		//the indexer of the collection
		public EGraphic this[int index]
		{
			get
			{
				return (EGraphic)this.List[index];
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
			foreach (EGraphic graphic in InnerList)
			{
				if (graphic.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EGraphic item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EGraphic item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EGraphic item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EGraphic item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
