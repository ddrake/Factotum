using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EMeasurement : IEntity
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
		private Guid? MsrDBid;
		private Guid? MsrSvyID;
		private short? MsrRow;
		private short? MsrCol;
		private decimal? MsrThickness;
		private decimal? MsrRowOffset;
		private decimal? MsrColOffset;
		private decimal? MsrDiameter;
		private short? MsrCount;
		private bool MsrIsObstruction;
		private bool MsrIsError;

		// Textbox limits
		public static int MsrRowCharLimit = 6;
		public static int MsrColCharLimit = 6;
		public static int MsrThicknessCharLimit = 7;
		public static int MsrRowOffsetCharLimit = 8;
		public static int MsrColOffsetCharLimit = 8;
		public static int MsrDiameterCharLimit = 7;
		public static int MsrCountCharLimit = 6;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string MsrRowErrMsg;
		private string MsrColErrMsg;
		private string MsrThicknessErrMsg;
		private string MsrRowOffsetErrMsg;
		private string MsrColOffsetErrMsg;
		private string MsrDiameterErrMsg;
		private string MsrCountErrMsg;
		private string MsrIsObstructionErrMsg;
		private string MsrIsErrorErrMsg;

		// Form level validation message
		private string MsrErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return MsrDBid; }
		}

		public Guid? MeasurementSvyID
		{
			get { return MsrSvyID; }
			set { MsrSvyID = value; }
		}

		public short? MeasurementRow
		{
			get { return MsrRow; }
			set { MsrRow = value; }
		}

		public short? MeasurementCol
		{
			get { return MsrCol; }
			set { MsrCol = value; }
		}

		public decimal? MeasurementThickness
		{
			get { return MsrThickness; }
			set { MsrThickness = value; }
		}

		public decimal? MeasurementRowOffset
		{
			get { return MsrRowOffset; }
			set { MsrRowOffset = value; }
		}

		public decimal? MeasurementColOffset
		{
			get { return MsrColOffset; }
			set { MsrColOffset = value; }
		}

		public decimal? MeasurementDiameter
		{
			get { return MsrDiameter; }
			set { MsrDiameter = value; }
		}

		public short? MeasurementCount
		{
			get { return MsrCount; }
			set { MsrCount = value; }
		}

		public bool MeasurementIsObstruction
		{
			get { return MsrIsObstruction; }
			set { MsrIsObstruction = value; }
		}

		public bool MeasurementIsError
		{
			get { return MsrIsError; }
			set { MsrIsError = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string MeasurementRowErrMsg
		{
			get { return MsrRowErrMsg; }
		}

		public string MeasurementColErrMsg
		{
			get { return MsrColErrMsg; }
		}

		public string MeasurementThicknessErrMsg
		{
			get { return MsrThicknessErrMsg; }
		}

		public string MeasurementRowOffsetErrMsg
		{
			get { return MsrRowOffsetErrMsg; }
		}

		public string MeasurementColOffsetErrMsg
		{
			get { return MsrColOffsetErrMsg; }
		}

		public string MeasurementDiameterErrMsg
		{
			get { return MsrDiameterErrMsg; }
		}

		public string MeasurementCountErrMsg
		{
			get { return MsrCountErrMsg; }
		}

		public string MeasurementIsObstructionErrMsg
		{
			get { return MsrIsObstructionErrMsg; }
		}

		public string MeasurementIsErrorErrMsg
		{
			get { return MsrIsErrorErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string MeasurementErrMsg
		{
			get { return MsrErrMsg; }
			set { MsrErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool MeasurementThicknessLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MsrThicknessCharLimit)
			{
				MsrThicknessErrMsg = string.Format("MeasurementThicknesss cannot exceed {0} characters", MsrThicknessCharLimit);
				return false;
			}
			else
			{
				MsrThicknessErrMsg = null;
				return true;
			}
		}

		public bool MeasurementRowOffsetLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MsrRowOffsetCharLimit)
			{
				MsrRowOffsetErrMsg = string.Format("MeasurementRowOffsets cannot exceed {0} characters", MsrRowOffsetCharLimit);
				return false;
			}
			else
			{
				MsrRowOffsetErrMsg = null;
				return true;
			}
		}

		public bool MeasurementColOffsetLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MsrColOffsetCharLimit)
			{
				MsrColOffsetErrMsg = string.Format("MeasurementColOffsets cannot exceed {0} characters", MsrColOffsetCharLimit);
				return false;
			}
			else
			{
				MsrColOffsetErrMsg = null;
				return true;
			}
		}

		public bool MeasurementDiameterLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MsrDiameterCharLimit)
			{
				MsrDiameterErrMsg = string.Format("MeasurementDiameters cannot exceed {0} characters", MsrDiameterCharLimit);
				return false;
			}
			else
			{
				MsrDiameterErrMsg = null;
				return true;
			}
		}

		public bool MeasurementCountLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > MsrCountCharLimit)
			{
				MsrCountErrMsg = string.Format("MeasurementCounts cannot exceed {0} characters", MsrCountCharLimit);
				return false;
			}
			else
			{
				MsrCountErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool MeasurementThicknessValid(string value)
		{
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0)
			{
				MsrThicknessErrMsg = null;
				return true;
			}
			MsrThicknessErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool MeasurementRowOffsetValid(string value)
		{
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0)
			{
				MsrRowOffsetErrMsg = null;
				return true;
			}
			MsrRowOffsetErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool MeasurementColOffsetValid(string value)
		{
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0)
			{
				MsrColOffsetErrMsg = null;
				return true;
			}
			MsrColOffsetErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool MeasurementDiameterValid(string value)
		{
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0)
			{
				MsrDiameterErrMsg = null;
				return true;
			}
			MsrDiameterErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool MeasurementCountValid(string value)
		{
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				MsrCountErrMsg = null;
				return true;
			}
			MsrCountErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool MeasurementIsObstructionValid(bool value)
		{
			// Add some real validation here if needed.
			MsrIsObstructionErrMsg = null;
			return true;
		}

		public bool MeasurementIsErrorValid(bool value)
		{
			// Add some real validation here if needed.
			MsrIsErrorErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EMeasurement()
		{
			this.MsrRow = 0;
			this.MsrCol = 0;
			this.MsrThickness = 0;
			this.MsrRowOffset = 0;
			this.MsrColOffset = 0;
			this.MsrDiameter = 0;
			this.MsrCount = 1;
			this.MsrIsObstruction = false;
			this.MsrIsError = false;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EMeasurement(Guid? id) : this()
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
				MsrDBid,
				MsrSvyID,
				MsrRow,
				MsrCol,
				MsrThickness,
				MsrRowOffset,
				MsrColOffset,
				MsrDiameter,
				MsrCount,
				MsrIsObstruction,
				MsrIsError
				from Measurements
				where MsrDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				MsrDBid = (Guid?)dr[0];
				MsrSvyID = (Guid?)dr[1];
				MsrRow = (short?)dr[2];
				MsrCol = (short?)dr[3];
				MsrThickness = (decimal?)Util.NullForDbNull(dr[4]);
				MsrRowOffset = (decimal?)dr[5];
				MsrColOffset = (decimal?)dr[6];
				MsrDiameter = (decimal?)dr[7];
				MsrCount = (short?)dr[8];
				MsrIsObstruction = (bool)dr[9];
				MsrIsError = (bool)dr[10];
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
				MsrDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", MsrDBid),
					new SqlCeParameter("@p1", MsrSvyID),
					new SqlCeParameter("@p2", MsrRow),
					new SqlCeParameter("@p3", MsrCol),
					new SqlCeParameter("@p4", Util.DbNullForNull(MsrThickness)),
					new SqlCeParameter("@p5", MsrRowOffset),
					new SqlCeParameter("@p6", MsrColOffset),
					new SqlCeParameter("@p7", MsrDiameter),
					new SqlCeParameter("@p8", MsrCount),
					new SqlCeParameter("@p9", MsrIsObstruction),
					new SqlCeParameter("@p10", MsrIsError)
					});
				cmd.CommandText = @"Insert Into Measurements (
					MsrDBid,
					MsrSvyID,
					MsrRow,
					MsrCol,
					MsrThickness,
					MsrRowOffset,
					MsrColOffset,
					MsrDiameter,
					MsrCount,
					MsrIsObstruction,
					MsrIsError
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Measurements row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", MsrDBid),
					new SqlCeParameter("@p1", MsrSvyID),
					new SqlCeParameter("@p2", MsrRow),
					new SqlCeParameter("@p3", MsrCol),
					new SqlCeParameter("@p4", Util.DbNullForNull(MsrThickness)),
					new SqlCeParameter("@p5", MsrRowOffset),
					new SqlCeParameter("@p6", MsrColOffset),
					new SqlCeParameter("@p7", MsrDiameter),
					new SqlCeParameter("@p8", MsrCount),
					new SqlCeParameter("@p9", MsrIsObstruction),
					new SqlCeParameter("@p10", MsrIsError)});

				cmd.CommandText =
					@"Update Measurements 
					set					
					MsrSvyID = @p1,					
					MsrRow = @p2,					
					MsrCol = @p3,					
					MsrThickness = @p4,					
					MsrRowOffset = @p5,					
					MsrColOffset = @p6,					
					MsrDiameter = @p7,					
					MsrCount = @p8,					
					MsrIsObstruction = @p9,					
					MsrIsError = @p10
					Where MsrDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update measurements row");
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
			if (!MeasurementIsObstructionValid(MeasurementIsObstruction)) return false;
			if (!MeasurementIsErrorValid(MeasurementIsError)) return false;

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
			if (MsrDBid == null)
			{
				MeasurementErrMsg = "Unable to delete. Record not found.";
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
					@"Delete from Measurements 
					where MsrDBid = @p0";
				cmd.Parameters.Add("@p0", MsrDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					MeasurementErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
					return true;
			}
			else
			{
				MeasurementErrMsg = null;
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of measurements
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EMeasurementCollection ListForGrid(Guid? GridID)
		{
			EMeasurement measurement;
			EMeasurementCollection measurements = new EMeasurementCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				MsrDBid,
				MsrSvyID,
				MsrRow,
				MsrCol,
				MsrThickness,
				MsrRowOffset,
				MsrColOffset,
				MsrDiameter,
				MsrCount,
				MsrIsObstruction,
				MsrIsError
				from Measurements
				inner join Surveys on MsrSvyID = SvyDBid
				inner join Dsets on SvyDstID = DstDbid
				inner join GridCells on 
					(DstGridPriority = GclGridPriority and MsrRow = GclRow and MsrCol = GclCol)
				where DstGrdID = @p1";

			qry += "	order by MsrRow, MsrCol";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", GridID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				measurement = new EMeasurement((Guid?)dr[0]);
				measurement.MsrSvyID = (Guid?)(dr[1]);
				measurement.MsrRow = (short?)(dr[2]);
				measurement.MsrCol = (short?)(dr[3]);
				measurement.MsrThickness = (decimal?)Util.NullForDbNull(dr[4]);
				measurement.MsrRowOffset = (decimal?)(dr[5]);
				measurement.MsrColOffset = (decimal?)(dr[6]);
				measurement.MsrDiameter = (decimal?)(dr[7]);
				measurement.MsrCount = (short?)(dr[8]);
				measurement.MsrIsObstruction = (bool)(dr[9]);
				measurement.MsrIsError = (bool)(dr[10]);

				measurements.Add(measurement);	
			}
			// Finish up
			dr.Close();
			return measurements;
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
					MsrDBid as ID,
					MsrSvyID as MeasurementSvyID,
					MsrRow as MeasurementRow,
					MsrCol as MeasurementCol,
					MsrThickness as MeasurementThickness,
					MsrRowOffset as MeasurementRowOffset,
					MsrColOffset as MeasurementColOffset,
					MsrDiameter as MeasurementDiameter,
					MsrCount as MeasurementCount,
					CASE
						WHEN MsrIsObstruction = 0 THEN 'No'
						ELSE 'Yes'
					END as MeasurementIsObstruction,
					CASE
						WHEN MsrIsError = 0 THEN 'No'
						ELSE 'Yes'
					END as MeasurementIsError
					from Measurements";
			qry += "	order by MsrRow, MsrCol";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		// Putting 700 measurements into a collection turned out to be extremely slow.
		// Getting them from the collection to the Datagrid cells was pretty fast
		// Hopefully this will be faster.
		// If you pass a null grid id, you should get back a table with the right schema, just
		// no rows.
		public static DataTable GetForGrid(Guid? GridID)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
					MsrDBid as ID,
					MsrRow as MeasurementRow,
					MsrCol as MeasurementCol,
					MsrThickness as MeasurementThickness,
					MsrIsObstruction as MeasurementIsObstruction,
					MsrIsError as MeasurementIsError
					from Measurements
					inner join Surveys on MsrSvyID = SvyDBid
					inner join Dsets on SvyDstID = DstDbid
					inner join GridCells on MsrDBid = GclMsrID
					where DstGrdID = @p1
					order by MsrRow, MsrCol";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", GridID == null ? Guid.Empty : GridID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			return ds.Tables[0];
		}
		
		// Get a data table arranged for writing back out to a Panametrics text file
		public static DataTable GetForExport(Guid? GridID)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				MsrCol,
				MsrRow,
				MsrThickness,
				MsrIsObstruction, 
				SvyUnits,
				SvyNumber
				from Measurements
				inner join Surveys on MsrSvyID = SvyDBid
				inner join Dsets on SvyDstID = DstDbid
				inner join GridCells on MsrDBid = GclMsrID
				where DstGrdID = @p1";

			qry += "	order by MsrCol, MsrRow";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", GridID == null ? Guid.Empty : GridID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			return ds.Tables[0];
		}

		
		
//--------------------------------------
		// Private utilities
		//--------------------------------------


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (MeasurementRow == null)
			{
				MsrRowErrMsg = "A Measurement Row is required";
				allFilled = false;
			}
			else
			{
				MsrRowErrMsg = null;
			}
			if (MeasurementCol == null)
			{
				MsrColErrMsg = "A Measurement Col is required";
				allFilled = false;
			}
			else
			{
				MsrColErrMsg = null;
			}
			return allFilled;
		}

		// Get a column label (like AJ) for a zero-based column number
		public static string GetColLabel(short col)
		{
			int first = col / 26;
			int second = col % 26;
			if (first > 0) return new string(new char[] { (char)(first - 1 + 'A'), (char)(second + 'A') });
			else return new string((char)(second + 'A'), 1);
		}

		// Given a column label (like AJ) return a zero-based column number
		public static short? GetColForLabel(string label)
		{
			int val, val2;
			if (label.Length == 0 || label.Length > 2) return null;
			val = char.Parse(label.Substring(0, 1)) - 'A';
			if (val < 0 || val >= 26) return null;
			if (label.Length == 2)
			{
				val2 = char.Parse(label.Substring(1, 1)) - 'A';
				if (val2 < 0 || val2 >= 26) return null;
				val = (val + 1) * 26 + val2;
			}
			return (short)val;
		}
		public static string GetIdentifierForRowAndCol(short row, short col)
		{
			if (col < 0 || col >= 27 * 26 || row < 0)
			{
				return "N/A";
			}
			else
			{
				int colHi = col / 26;
				int colLo = col % 26;

				char hi, lo;
				hi = (char)0;
				if (colHi > 0) hi = (char)(colHi - 1 + 'A');
				lo = (char)(colLo + 'A');
				char[] ca = new char[2];
				ca[0] = colHi > 0 ? hi : ' ';
				ca[1] = lo;
				return (new string(ca) + (row+1).ToString("00"));
			}
		}
		// Given a cell identifier (like AJ45) return a zero-based row and column
		public static bool GetRowAndColForIdentifier(String ident, out short row, out short col)
		{
			int c = 0;
			int val = 0;
			row = 0;
			col = 0;
			while (char.IsLetter(ident, c)) c++;
			switch (c)
			{
				// Just one letter for the column
				case 1:
					val = char.Parse(ident.Substring(0, 1)) - 'A';
					break;
				// Two-letter column
				case 2:
					val = (char.Parse(ident.Substring(0, 1)) - 'A' + 1) * 26;
					val += char.Parse(ident.Substring(1, 1)) - 'A';
					break;
				default:
					return false;
			}
			col = System.Convert.ToInt16(val);
			String s = ident.Substring(c, ident.Length - c);
			if (s.Length > 0)
			{
				val = int.Parse(s) - 1; // parse and subtract 1 for zero-based
				row = System.Convert.ToInt16(val);
			}
			else return false;
			return true;
		}

	}

	//--------------------------------------
	// Measurement Collection class
	//--------------------------------------
	public class EMeasurementCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EMeasurementCollection()
		{ }
		//the indexer of the collection
		public EMeasurement this[int index]
		{
			get
			{
				return (EMeasurement)this.List[index];
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
			foreach (EMeasurement measurement in InnerList)
			{
				if (measurement.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EMeasurement item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EMeasurement item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EMeasurement item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EMeasurement item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
