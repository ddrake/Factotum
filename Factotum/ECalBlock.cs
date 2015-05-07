using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public enum CalBlockMaterialTypeEnum : byte
	{
		Unspecified = 0,
		CarbonSteel = 1,
		StainlessSteel = 2,
		Inconel = 3
	}
	public enum CalBlockPhysicalTypeEnum : byte
	{
		Unspecified = 0,
		StepBlock = 1,
		IIW2Block = 2
	}

	public class ECalBlock : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler CalBlockKitAssignmentsChanged;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}

		protected virtual void OnCalBlockKitAssignmentsChanged()
		{
			EventHandler temp = CalBlockKitAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}

		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? CbkDBid;
		private string CbkSerialNumber;
		private decimal? CbkCalMin;
		private decimal? CbkCalMax;
		private Guid? CbkKitID;
		private byte? CbkMaterialType;
		private byte? CbkType;
		private bool CbkUsedInOutage;
		private bool CbkIsLclChg;
		private bool CbkIsActive;

		// Textbox limits
		public static int CbkSerialNumberCharLimit = 25;
		public static int CbkCalMinCharLimit = 7;
		public static int CbkCalMaxCharLimit = 7;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string CbkSerialNumberErrMsg;
		private string CbkCalMinErrMsg;
		private string CbkCalMaxErrMsg;

		// Form level validation message
		private string CbkErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return CbkDBid; }
		}

		public string CalBlockSerialNumber
		{
			get { return CbkSerialNumber; }
			set { CbkSerialNumber = Util.NullifyEmpty(value); }
		}

		public decimal? CalBlockCalMin
		{
			get { return CbkCalMin; }
			set { CbkCalMin = value; }
		}

		public decimal? CalBlockCalMax
		{
			get { return CbkCalMax; }
			set { CbkCalMax = value; }
		}

		public Guid? CalBlockKitID
		{
			get { return CbkKitID; }
			set { CbkKitID = value; }
		}

		public byte? CalBlockMaterialType
		{
			get { return CbkMaterialType; }
			set { CbkMaterialType = value; }
		}

		public byte? CalBlockType
		{
			get { return CbkType; }
			set { CbkType = value; }
		}

		public bool CalBlockUsedInOutage
		{
			get { return CbkUsedInOutage; }
			set { CbkUsedInOutage = value; }
		}

		public bool CalBlockIsLclChg
		{
			get { return CbkIsLclChg; }
			set { CbkIsLclChg = value; }
		}

		public bool CalBlockIsActive
		{
			get { return CbkIsActive; }
			set { CbkIsActive = value; }
		}

		public string CalBlockNameWithStatus
		{
			get { return CbkSerialNumber == null ? null : CbkSerialNumber + (CbkIsActive ? "" : " (inactive)"); }
		}

		// Array of CalBlock Materials for combo box binding
		static public CalBlockMaterial[] GetCalBlockMaterialsArray()
		{
			return new CalBlockMaterial[] {
			new CalBlockMaterial((byte)CalBlockMaterialTypeEnum.CarbonSteel,"Carbon Steel"), 
			new CalBlockMaterial((byte)CalBlockMaterialTypeEnum.StainlessSteel,"Stainless Steel"), 
			new CalBlockMaterial((byte)CalBlockMaterialTypeEnum.Inconel,"Inconel")};
		}

		// Array of CalBlock Materials for combo box binding
		static public CalBlockType[] GetCalBlockTypesArray()
		{
			return new CalBlockType[] {
			new CalBlockType((byte)CalBlockPhysicalTypeEnum.StepBlock,"Step Block"), 
			new CalBlockType((byte)CalBlockPhysicalTypeEnum.IIW2Block,"IIW2 Block")}; 
		}

		public string CalBlockMaterialName
		{
			get
			{
				string name;

				if (CbkMaterialType == null) return "N/A";

				switch ((CalBlockMaterialTypeEnum)CbkMaterialType)
				{
					case CalBlockMaterialTypeEnum.CarbonSteel:
						name = "Carbon Steel";
						break;
					case CalBlockMaterialTypeEnum.Inconel:
						name = "Inconel";
						break;
					case CalBlockMaterialTypeEnum.StainlessSteel:
						name = "Stainless Steel";
						break;
					default:
						name = "N/A";
						break;
				}
				return name;
			}
		}

		public string CalBlockMaterialAbbr
		{
			get
			{
				string name;

				if (CbkMaterialType == null) return "N/A";

				switch ((CalBlockMaterialTypeEnum)CbkMaterialType)
				{
					case CalBlockMaterialTypeEnum.CarbonSteel:
						name = "C/S";
						break;
					case CalBlockMaterialTypeEnum.Inconel:
						name = "Inc";
						break;
					case CalBlockMaterialTypeEnum.StainlessSteel:
						name = "S/S";
						break;
					default:
						name = "N/A";
						break;
				}
				return name;
			}
		}

		public string CalBlockTypeName
		{
			get
			{
				string name;
				if (CbkType == null) return "N/A";

				switch ((CalBlockPhysicalTypeEnum)CbkType)
				{
					case CalBlockPhysicalTypeEnum.IIW2Block:
						name = "IIW2";
						break;
					case CalBlockPhysicalTypeEnum.StepBlock:
						name = "Step Block";
						break;
					default:
						name = "N/A";
						break;
				}
				return name;
			}
		}

		public string CalBlockTypeAbbr
		{
			get
			{
				string name;
				if (CbkType == null) return "N/A";

				switch ((CalBlockPhysicalTypeEnum)CbkType)
				{
					case CalBlockPhysicalTypeEnum.IIW2Block:
						name = "IIW2";
						break;
					case CalBlockPhysicalTypeEnum.StepBlock:
						name = "Step";
						break;
					default:
						name = "N/A";
						break;
				}
				return name;
			}
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string CalBlockSerialNumberErrMsg
		{
			get { return CbkSerialNumberErrMsg; }
		}

		public string CalBlockCalMinErrMsg
		{
			get { return CbkCalMinErrMsg; }
		}

		public string CalBlockCalMaxErrMsg
		{
			get { return CbkCalMaxErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string CalBlockErrMsg
		{
			get { return CbkErrMsg; }
			set { CbkErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool CalBlockSerialNumberLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CbkSerialNumberCharLimit)
			{
				CbkSerialNumberErrMsg = string.Format("Calibration Block Serial Numbers cannot exceed {0} characters", CbkSerialNumberCharLimit);
				return false;
			}
			else
			{
				CbkSerialNumberErrMsg = null;
				return true;
			}
		}

		public bool CalBlockCalMinLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CbkCalMinCharLimit)
			{
				CbkCalMinErrMsg = string.Format("Calibration Block Calibration Minimums cannot exceed {0} characters", CbkCalMinCharLimit);
				return false;
			}
			else
			{
				CbkCalMinErrMsg = null;
				return true;
			}
		}

		public bool CalBlockCalMaxLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CbkCalMaxCharLimit)
			{
				CbkCalMaxErrMsg = string.Format("Calibration Block Calibration Maximums cannot exceed {0} characters", CbkCalMaxCharLimit);
				return false;
			}
			else
			{
				CbkCalMaxErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool CalBlockSerialNumberValid(string value)
		{
			if (!CalBlockSerialNumberLengthOk(value)) return false;

			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			bool existingIsInactive;
			if (NameExists(value, CbkDBid, out existingIsInactive))
			{
				CbkSerialNumberErrMsg = existingIsInactive ?
					"That Calibration block serial number exists but its status has been set to inactive." :
					"That Calibration block serial number is already in use.";

				return false;
			}
			CbkSerialNumberErrMsg = null;
			return true;
		}

		public bool CalBlockCalMinValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CbkCalMinErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CbkCalMinErrMsg = null;
				return true;
			}
			CbkCalMinErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool CalBlockCalMaxValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CbkCalMaxErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CbkCalMaxErrMsg = null;
				return true;
			}
			CbkCalMaxErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}


		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ECalBlock()
		{
			this.CbkUsedInOutage = false;
			this.CbkIsLclChg = false;
			this.CbkIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ECalBlock(Guid? id) : this()
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
				CbkDBid,
				CbkSerialNumber,
				CbkCalMin,
				CbkCalMax,
				CbkKitID,
				CbkMaterialType,
				CbkType,
				CbkUsedInOutage,
				CbkIsLclChg,
				CbkIsActive
				from CalBlocks
				where CbkDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				CbkDBid = (Guid?)dr[0];
				CbkSerialNumber = (string)dr[1];
				CbkCalMin = (decimal?)dr[2];
				CbkCalMax = (decimal?)dr[3];
				CbkKitID = (Guid?)Util.NullForDbNull(dr[4]);
				CbkMaterialType = (byte?)dr[5];
				CbkType = (byte?)dr[6];
				CbkUsedInOutage = (bool)dr[7];
				CbkIsLclChg = (bool)dr[8];
				CbkIsActive = (bool)dr[9];
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

			// If the status has been set to inactive, make sure the Kit is null
			if (!CbkIsActive) CbkKitID = null;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) CbkIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				CbkDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CbkDBid),
					new SqlCeParameter("@p1", CbkSerialNumber),
					new SqlCeParameter("@p2", CbkCalMin),
					new SqlCeParameter("@p3", CbkCalMax),
					new SqlCeParameter("@p4", Util.DbNullForNull(CbkKitID)),
					new SqlCeParameter("@p5", CbkMaterialType),
					new SqlCeParameter("@p6", CbkType),
					new SqlCeParameter("@p7", CbkUsedInOutage),
					new SqlCeParameter("@p8", CbkIsLclChg),
					new SqlCeParameter("@p9", CbkIsActive)
					});
				cmd.CommandText = @"Insert Into CalBlocks (
					CbkDBid,
					CbkSerialNumber,
					CbkCalMin,
					CbkCalMax,
					CbkKitID,
					CbkMaterialType,
					CbkType,
					CbkUsedInOutage,
					CbkIsLclChg,
					CbkIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Calibration Block row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CbkDBid),
					new SqlCeParameter("@p1", CbkSerialNumber),
					new SqlCeParameter("@p2", CbkCalMin),
					new SqlCeParameter("@p3", CbkCalMax),
					new SqlCeParameter("@p4", Util.DbNullForNull(CbkKitID)),
					new SqlCeParameter("@p5", CbkMaterialType),
					new SqlCeParameter("@p6", CbkType),
					new SqlCeParameter("@p7", CbkUsedInOutage),
					new SqlCeParameter("@p8", CbkIsLclChg),
					new SqlCeParameter("@p9", CbkIsActive)});

				cmd.CommandText =
					@"Update CalBlocks 
					set					
					CbkSerialNumber = @p1,					
					CbkCalMin = @p2,					
					CbkCalMax = @p3,					
					CbkKitID = @p4,					
					CbkMaterialType = @p5,					
					CbkType = @p6,					
					CbkUsedInOutage = @p7,					
					CbkIsLclChg = @p8,					
					CbkIsActive = @p9
					Where CbkDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update Calibration Block row");
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
			if (!CalBlockSerialNumberValid(CalBlockSerialNumber)) return false;

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
			if (CbkDBid == null)
			{
				CalBlockErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (CbkUsedInOutage)
			{
				CalBlockErrMsg = "Unable to delete this Calibration Block because it has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!CbkIsLclChg && !Globals.IsMasterDB)
			{
				CalBlockErrMsg = "Unable to delete this Calibration Block because it was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasChildren())
			{
				CalBlockErrMsg = "Unable to delete because this Calibration Block is referenced by datasets.";
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
					@"Delete from CalBlocks 
					where CbkDBid = @p0";
				cmd.Parameters.Add("@p0", CbkDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					CalBlockErrMsg = "Unable to delete.  Please try again later.";
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
				CalBlockErrMsg = null;
				return false;
			}
		}

		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select DstDBid from Dsets
					where DstCbkID = @p0";
			cmd.Parameters.Add("@p0", CbkDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of calblocks
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ECalBlockCollection ListByName(
			bool showinactive, bool addNoSelection)
		{
			ECalBlock calblock;
			ECalBlockCollection calblocks = new ECalBlockCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CbkDBid,
				CbkSerialNumber,
				CbkCalMin,
				CbkCalMax,
				CbkKitID,
				CbkMaterialType,
				CbkType,
				CbkUsedInOutage,
				CbkIsLclChg,
				CbkIsActive
				from CalBlocks";
			if (!showinactive)
				qry += " where CbkIsActive = 1";

			qry += "	order by CbkSerialNumber";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				calblock = new ECalBlock();
				calblock.CbkSerialNumber = "<No Selection>";
				calblocks.Add(calblock);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				calblock = new ECalBlock((Guid?)dr[0]);
				calblock.CbkSerialNumber = (string)(dr[1]);
				calblock.CbkCalMin = (decimal?)(dr[2]);
				calblock.CbkCalMax = (decimal?)(dr[3]);
				calblock.CbkKitID = (Guid?)Util.NullForDbNull(dr[4]);
				calblock.CbkMaterialType = (byte?)(dr[5]);
				calblock.CbkType = (byte?)(dr[6]);
				calblock.CbkUsedInOutage = (bool)(dr[7]);
				calblock.CbkIsLclChg = (bool)(dr[8]);
				calblock.CbkIsActive = (bool)(dr[9]);

				calblocks.Add(calblock);	
			}
			// Finish up
			dr.Close();
			return calblocks;
		}

		public static ECalBlockCollection ListForKit(Guid? kitID, bool addNoSelection)
		{
			ECalBlock calblock;
			ECalBlockCollection calblocks = new ECalBlockCollection();
			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				calblock = new ECalBlock();
				calblock.CbkSerialNumber = "<No Selection>";
				calblocks.Add(calblock);
			}
			if (kitID != null)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				string qry = @"Select 

				CbkDBid,
				CbkSerialNumber,
				CbkCalMin,
				CbkCalMax,
				CbkKitID,
				CbkMaterialType,
				CbkType,
				CbkUsedInOutage,
				CbkIsLclChg,
				CbkIsActive
				from CalBlocks";
				qry += " where CbkKitID = @p1";

				qry += "	order by CbkSerialNumber";
				cmd.CommandText = qry;
				cmd.Parameters.Add("@p1", kitID);

				SqlCeDataReader dr;
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				dr = cmd.ExecuteReader();

				// Build new objects and add them to the collection
				while (dr.Read())
				{
					calblock = new ECalBlock((Guid?)dr[0]);
					calblock.CbkSerialNumber = (string)(dr[1]);
					calblock.CbkCalMin = (decimal?)(dr[2]);
					calblock.CbkCalMax = (decimal?)(dr[3]);
					calblock.CbkKitID = (Guid?)Util.NullForDbNull(dr[4]);
					calblock.CbkMaterialType = (byte?)(dr[5]);
					calblock.CbkType = (byte?)(dr[6]);
					calblock.CbkUsedInOutage = (bool)(dr[7]);
					calblock.CbkIsLclChg = (bool)(dr[8]);
					calblock.CbkIsActive = (bool)(dr[9]);

					calblocks.Add(calblock);
				}
				// Finish up
				dr.Close();
			}
			return calblocks;
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
					CbkDBid as ID,
					CbkSerialNumber as CalBlockSerialNumber,
					CbkCalMin as CalBlockCalMin,
					CbkCalMax as CalBlockCalMax,
					KitName as CalBlockKitName,
					CASE 
						WHEN CbkMaterialType = 1 THEN 'Carbon Steel'
						WHEN CbkMaterialType = 2 THEN 'Stainless Steel'
						WHEN CbkMaterialType = 3 THEN 'Inconel'
						ELSE ''
					END as CalBlockMaterialType,
					CASE
						WHEN CbkType = 1 THEN 'Step Block'
						WHEN CbkType = 2 THEN 'IIW2 Block'
						ELSE ''
					END as CalBlockType,
					CASE
						WHEN CbkUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as CalBlockUsedInOutage,
					CASE
						WHEN CbkIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as CalBlockIsLclChg,
					CASE
						WHEN CbkIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as CalBlockIsActive
					from CalBlocks
					left outer join Kits on CbkKitID = KitDBid";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static DataTable GetWithAssignmentsForKit(Guid? KitID, bool includeInactive)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			string qry =
				@"select 
				CbkDBid as ID,
				CbkSerialNumber as CalBlockSerialNumber,
				CbkIsActive as CalBlockIsActive,
				CASE
				WHEN CbkKitID is null THEN 0 ELSE 1
				END as IsAssigned
				from CalBlocks left join Kits on CbkKitID = KitDBid
				where (CbkKitID is NULL or CbkKitID = @p1)";

			if (!includeInactive) qry += " and CbkIsActive = 1";

			qry += " order by CbkSerialNumber";

			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", KitID == null ? Guid.Empty : KitID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			return ds.Tables[0];
		}

		public static void UpdateAssignmentsToKit(Guid KitID, DataTable assignments)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			// First remove all ducers from the kit
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = @"Update CalBlocks set CbkKitID = NULL where CbkKitID = @p1";
			cmd.Parameters.Add("@p1", KitID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Then add the selected ducers back in
			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", KitID);
			cmd.Parameters.Add("@p2", "");
			cmd.CommandText =
@"				Update CalBlocks set CbkKitID = @p1 
				where CbkDBid = @p2";

			// Now insert the current ducer assignments
			for (int dmRow = 0; dmRow < assignments.Rows.Count; dmRow++)
			{
				if (Convert.ToBoolean(assignments.Rows[dmRow]["IsAssigned"]))
				{
					cmd.Parameters["@p2"].Value = (Guid)assignments.Rows[dmRow]["ID"];
					if (cmd.ExecuteNonQuery() != 1)
					{
						throw new Exception("Unable to assign Calibration Block to Kit");
					}
				}
			}
			ECalBlock cbk = new ECalBlock();
			cbk.OnCalBlockKitAssignmentsChanged();
		}
		
		//--------------------------------------
		// Private utilities
		//--------------------------------------

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExists(string name, Guid? id, out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			if (id == null)
			{
				cmd.CommandText = "Select CbkIsActive from CalBlocks where CbkSerialNumber = @p1";
			}
			else
			{
				cmd.CommandText = "Select CbkIsActive from CalBlocks where CbkSerialNumber = @p1 and CbkDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;
			return exists;
		}


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (CalBlockSerialNumber == null)
			{
				CbkSerialNumberErrMsg = "A Calibration Block SerialNumber is required";
				allFilled = false;
			}
			else
			{
				CbkSerialNumberErrMsg = null;
			}
			if (CalBlockCalMin == null)
			{
				CbkCalMinErrMsg = "A Calibration Block Calibration Minimum is required";
				allFilled = false;
			}
			else
			{
				CbkCalMinErrMsg = null;
			}
			if (CalBlockCalMax == null)
			{
				CbkCalMaxErrMsg = "A Calibration Block Calibration Maximum is required";
				allFilled = false;
			}
			else
			{
				CbkCalMaxErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// CalBlock Collection class
	//--------------------------------------
	public class ECalBlockCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ECalBlockCollection()
		{ }
		//the indexer of the collection
		public ECalBlock this[int index]
		{
			get
			{
				return (ECalBlock)this.List[index];
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
			foreach (ECalBlock calblock in InnerList)
			{
				if (calblock.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ECalBlock item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ECalBlock item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ECalBlock item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ECalBlock item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}

	public class CalBlockMaterial
	{
		private byte id;

		public byte ID
		{
			get { return id; }
			set { id = value; }
		}
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		public CalBlockMaterial(byte ID, string Name)
		{
			this.id = ID;
			this.name = Name;
		}
	}

	public class CalBlockType
	{
		private byte id;

		public byte ID
		{
			get { return id; }
			set { id = value; }
		}
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		public CalBlockType(byte ID, string Name)
		{
			this.id = ID;
			this.name = Name;
		}
	}

}
