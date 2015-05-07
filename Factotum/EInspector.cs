using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EInspector : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler InspectorKitAssignmentsChanged;
		public static event EventHandler InspectorOutageAssignmentsChanged;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}

		protected virtual void OnInspectorKitAssignmentsChanged()
		{
			EventHandler temp = InspectorKitAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}

		protected virtual void OnInspectorOutageAssignmentsChanged()
		{
			EventHandler temp = InspectorOutageAssignmentsChanged;
			if (temp != null)
				temp(this, new EventArgs());
		}
	
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? InsDBid;
		private Guid? InsKitID;
		private string InsName;
		private byte? InsLevel;
		private bool InsIsLclChg;
		private bool InsUsedInOutage;
		private bool InsIsActive;

		// This is used by ListWithAssignmentsForOutage() to indicate whether or not
		// the current grid procedure is assigned to the specified outage.
		private bool isAssignedToYourOutage;

		// Textbox limits
		public static int InsNameCharLimit = 50;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string InsNameErrMsg;

		// Form level validation message
		private string InsErrMsg;
		private EMeterCollection kitMeters;
		private EDucerCollection kitDucers;
		private ECalBlockCollection kitCalBlocks;
		private EThermoCollection kitThermos;
		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return InsDBid; }
		}

		public Guid? InspectorKitID
		{
			get { return InsKitID; }
			set { InsKitID = value; }
		}

		public string InspectorName
		{
			get { return InsName; }
			set { InsName = Util.NullifyEmpty(value); }
		}

		public byte? InspectorLevel
		{
			get { return InsLevel; }
			set { InsLevel = value; }
		}

		public bool InspectorIsLclChg
		{
			get { return InsIsLclChg; }
			set { InsIsLclChg = value; }
		}

		public bool InspectorUsedInOutage
		{
			get { return InsUsedInOutage; }
			set { InsUsedInOutage = value; }
		}

		public bool InspectorIsActive
		{
			get { return InsIsActive; }
			set { InsIsActive = value; }
		}

		public string InspectorLevelString
		{
			get
			{
				return InspectorLevelStringForLevel(InsLevel);
			}
		}

		public string InspectorNameWithStatus
		{
			get { return InsName == null ? null : InsName + (InsIsActive ? "" : " (inactive)"); }
		}

		static public string InspectorLevelStringForLevel(byte? level)
		{
			if (level == null) return "";
			switch (level)
			{
				case 1:
					return "Level I";
				case 2:
					return "Level II";
                case 3:
                    return "Level III";
                case 4:
                    return "Level IIL";
                default:
					throw new Exception("Unknown inspector level");
			}
		}
		// Array of Inspector Levels for combo box binding
		static public InspectorLevel[] GetInspectorLevelsArray()
		{
			return new InspectorLevel[] {
			new InspectorLevel(1,InspectorLevelStringForLevel(1)), 
			new InspectorLevel(2,InspectorLevelStringForLevel(2)), 
			new InspectorLevel(4,InspectorLevelStringForLevel(4)),
			new InspectorLevel(3,InspectorLevelStringForLevel(3))};
        }

		// This is used by ListWithAssignmentsForOutage() to indicate whether or not
		// the current inspector is assigned to the specified outage.
		public bool IsAssignedToYourOutage
		{
			get { return isAssignedToYourOutage; }
			set { isAssignedToYourOutage = value; }
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string InspectorNameErrMsg
		{
			get { return InsNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string InspectorErrMsg
		{
			get { return InsErrMsg; }
			set { InsErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool InspectorNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > InsNameCharLimit)
			{
				InsNameErrMsg = string.Format("Inspector Names cannot exceed {0} characters", InsNameCharLimit);
				return false;
			}
			else
			{
				InsNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool InspectorNameValid(string name)
		{
			bool existingIsInactive;
			if (!InspectorNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, InsDBid, out existingIsInactive))
			{
				InsNameErrMsg = existingIsInactive ?
					"That InspectorName exists but its status has been set to inactive." :
					"That InspectorName is already in use.";
				return false;
			}
			InsNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EInspector()
		{
			this.InsIsLclChg = false;
			this.InsUsedInOutage = false;
			this.InsIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EInspector(Guid? id) : this()
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
				InsDBid,
				InsKitID,
				InsName,
				InsLevel,
				InsIsLclChg,
				InsUsedInOutage,
				InsIsActive
				from Inspectors
				where InsDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				InsDBid = (Guid?)dr[0];
				InsKitID = (Guid?)Util.NullForDbNull(dr[1]);
				InsName = (string)dr[2];
				InsLevel = (byte?)dr[3];
				InsIsLclChg = (bool)dr[4];
				InsUsedInOutage = (bool)dr[5];
				InsIsActive = (bool)dr[6];
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
			if (!InsIsActive) InsKitID = null;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) InsIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				InsDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", InsDBid),
					new SqlCeParameter("@p1", Util.DbNullForNull(InsKitID)),
					new SqlCeParameter("@p2", InsName),
					new SqlCeParameter("@p3", InsLevel),
					new SqlCeParameter("@p4", InsIsLclChg),
					new SqlCeParameter("@p5", InsUsedInOutage),
					new SqlCeParameter("@p6", InsIsActive)
					});
				cmd.CommandText = @"Insert Into Inspectors (
					InsDBid,
					InsKitID,
					InsName,
					InsLevel,
					InsIsLclChg,
					InsUsedInOutage,
					InsIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Inspectors row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", InsDBid),
					new SqlCeParameter("@p1", Util.DbNullForNull(InsKitID)),
					new SqlCeParameter("@p2", InsName),
					new SqlCeParameter("@p3", InsLevel),
					new SqlCeParameter("@p4", InsIsLclChg),
					new SqlCeParameter("@p5", InsUsedInOutage),
					new SqlCeParameter("@p6", InsIsActive)});

				cmd.CommandText =
					@"Update Inspectors 
					set					
					InsKitID = @p1,					
					InsName = @p2,					
					InsLevel = @p3,					
					InsIsLclChg = @p4,					
					InsUsedInOutage = @p5,					
					InsIsActive = @p6
					Where InsDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update inspectors row");
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
			if (!InspectorNameValid(InspectorName)) return false;

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
			if (InsDBid == null)
			{
				InspectorErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (InsUsedInOutage)
			{
				InspectorErrMsg = "Unable to delete this Inspector because of participation in past outages.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (!InsIsLclChg && !Globals.IsMasterDB)
			{
				InspectorErrMsg = "Unable to delete because this Inspector was not added during this outage.\r\nYou may wish to inactivate instead.";
				return false;
			}

			if (HasOutages())
			{
				InspectorErrMsg = "Unable to delete this Inspector because they are assigned to one or more outages.\r\n";
				return false;
			}

			if (HasDsets())
			{
				InspectorErrMsg = "Unable to delete because this Inspector is referenced by one or more Datasets.";
				return false;
			}

			if (HasInspectedComponents())
			{
				InspectorErrMsg = "Unable to delete because this Inspector reviewed one or more Component Reports.";
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
					@"Delete from Inspectors 
					where InsDBid = @p0";
				cmd.Parameters.Add("@p0", InsDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					InspectorErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					InspectorErrMsg = null;
					OnChanged(ID);
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool HasDsets()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select DstDBid from Dsets
					where DstInsID = @p0";
			cmd.Parameters.Add("@p0", InsDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		private bool HasInspectedComponents()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select IscDBid from InspectedComponents
					where IscInsID = @p0";
			cmd.Parameters.Add("@p0", InsDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		private bool HasOutages()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OinOtgID from OutageInspectors
					where OinInsID = @p0";
			cmd.Parameters.Add("@p0", InsDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of inspectors
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EInspectorCollection ListByName(
			bool showinactive, bool addNoSelection)
		{
			EInspector inspector;
			EInspectorCollection inspectors = new EInspectorCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				InsDBid,
				InsKitID,
				InsName,
				InsLevel,
				InsIsLclChg,
				InsUsedInOutage,
				InsIsActive
				from Inspectors";
			if (!showinactive)
				qry += " where InsIsActive = 1";

			qry += "	order by InsName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				inspector = new EInspector();
				inspector.InsName = "<No Selection>";
				inspectors.Add(inspector);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				inspector = new EInspector((Guid?)dr[0]);
				inspector.InsKitID = (Guid?)Util.NullForDbNull(dr[1]);
				inspector.InsName = (string)(dr[2]);
				inspector.InsLevel = (byte?)(dr[3]);
				inspector.InsIsLclChg = (bool)(dr[4]);
				inspector.InsUsedInOutage = (bool)(dr[5]);
				inspector.InsIsActive = (bool)(dr[6]);

				inspectors.Add(inspector);	
			}
			// Finish up
			dr.Close();
			return inspectors;
		}

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EInspectorCollection ListForOutage(Guid outageID,
			bool showinactive, bool addNoSelection)
		{
			EInspector inspector;
			EInspectorCollection inspectors = new EInspectorCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				InsDBid,
				InsKitID,
				InsName,
				InsLevel,
				InsIsLclChg,
				InsUsedInOutage,
				InsIsActive
				from Inspectors
				left outer join OutageInspectors
				on InsDBid = OinInsID";
			qry += " where OinOtgID = @p1";
			if (!showinactive)
				qry += " and InsIsActive = 1";

			qry += "	order by InsName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", outageID);

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				inspector = new EInspector();
				inspector.InsName = "<No Selection>";
				inspectors.Add(inspector);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				inspector = new EInspector((Guid?)dr[0]);
				inspector.InsKitID = (Guid?)Util.NullForDbNull(dr[1]);
				inspector.InsName = (string)(dr[2]);
				inspector.InsLevel = (byte?)(dr[3]);
				inspector.InsIsLclChg = (bool)(dr[4]);
				inspector.InsUsedInOutage = (bool)(dr[5]);
				inspector.InsIsActive = (bool)(dr[6]);

				inspectors.Add(inspector);
			}
			// Finish up
			dr.Close();
			return inspectors;
		}

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EInspectorCollection ListForInspectedComponent(Guid inspectedComponentID,
			bool excludeDuplicates)
		{
			EInspector inspector;
			EInspectorCollection inspectors = new EInspectorCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				InsDBid,
				InsKitID,
				InsName,
				InsLevel,
				InsIsLclChg,
				InsUsedInOutage,
				InsIsActive
				from Inspections
				inner join Dsets on IspDBid = DstIspID
				inner join Inspectors on DstInsID = InsDBid";
			qry += " where IspIscID = @p1";

			qry += "	order by InsName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", inspectedComponentID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				inspector = new EInspector((Guid?)dr[0]);
				inspector.InsKitID = (Guid?)Util.NullForDbNull(dr[1]);
				inspector.InsName = (string)(dr[2]);
				inspector.InsLevel = (byte?)(dr[3]);
				inspector.InsIsLclChg = (bool)(dr[4]);
				inspector.InsUsedInOutage = (bool)(dr[5]);
				inspector.InsIsActive = (bool)(dr[6]);

				inspectors.Add(inspector,excludeDuplicates);
			}
			// Finish up
			dr.Close();
			return inspectors;
		}

		// This helper function builds the collection for you based on the flags you send it
		// To fill a checked listbox you may want to set 'includeUnassigned'
		// To fill a treeview, you probably don't.
		public static EInspectorCollection ListWithAssignmentsForOutage(Guid? OutageID,
			bool showactive, bool showinactive)
		{
			EInspector ins;
			EInspectorCollection inss = new EInspectorCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry =
@"
				Select 
				InsDBid, 
				InsKitID,
				InsName,
				InsLevel,
				InsIsLclChg,
				InsUsedInOutage,
				InsIsActive, 
				Max(Case OutageInspectors.OinOtgID
					when @p1 then 1
					else 0
					end) as IsAssigned
				From Inspectors 
				left outer join OutageInspectors
				on Inspectors.InsDBid = OutageInspectors.OinInsID";
			if (showactive && !showinactive)
				qry +=
@" 
				where InsIsActive = 1";
			else if (!showactive && showinactive)
				qry +=
@"				where InsIsActive = 0";

			qry +=
@"			
				Group by InsName, InsDBid, InsKitID, 
				InsLevel, InsIsLclChg, InsUsedInOutage, 
				InsIsActive";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", OutageID == null ? Guid.Empty : OutageID);
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				ins = new EInspector((Guid?)dr[0]);
				ins.InspectorKitID = (Guid?)Util.NullForDbNull(dr[1]);
				ins.InspectorName = (string)dr[2];
				ins.InspectorLevel = (byte)dr[3];
				ins.InspectorIsLclChg = (bool)dr[4];
				ins.InspectorUsedInOutage = (bool)dr[5];
				ins.InspectorIsActive = (bool)dr[6];
				ins.IsAssignedToYourOutage = Convert.ToBoolean(dr[7]);

				inss.Add(ins);
			}
			// Finish up
			dr.Close();
			return inss;
		}

		public EMeterCollection ListKitMeters(bool addNoSelection)
		{
			if (kitMeters == null)
				kitMeters = EMeter.ListForKit(InsKitID, addNoSelection);
			return kitMeters;
		}

		
		public EDucerCollection ListKitDucers(bool addNoSelection)
		{
			if (kitDucers == null)
				kitDucers = EDucer.ListForKit(InsKitID, addNoSelection);
			return kitDucers;
		}

		public ECalBlockCollection ListKitCalBlocks(bool addNoSelection)
		{
			if (kitCalBlocks == null)
				kitCalBlocks = ECalBlock.ListForKit(InsKitID, addNoSelection);
			return kitCalBlocks;
		}

		public EThermoCollection ListKitThermos(bool addNoSelection)
		{
			if (kitThermos == null)
				kitThermos = EThermo.ListForKit(InsKitID, addNoSelection);
			return kitThermos;
		}

		public static void UpdateAssignmentsToOutage(Guid? OutageID, EInspectorCollection inss)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			// First delete any existing inspector assignments (except those that are 
			// used in reports or datasets)
			cmd.CommandText = 
				@"Delete from OutageInspectors 
				where OinOtgID = @p1
				and not exists(
					select IscDBid from InspectedComponents
					where IscInsID = OinInsID)
				and not exists(
					select DstDBid from Dsets
					inner join Inspections on DstIspID = IspDBid
					inner join InspectedComponents on IspIscID = IscDBid
					where DstInsID = OinInsID)";
			cmd.Parameters.Add("@p1", OutageID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select OinOtgID from OutageInspectors
					where OinOtgID = @p1 and OinInsID = @p2";
			cmd.Parameters.Add("@p1", OutageID);
			cmd.Parameters.Add("@p2", "");

			SqlCeCommand cmd2 = Globals.cnn.CreateCommand();
			cmd2.CommandText =
				@"Insert Into OutageInspectors (OinOtgID, OinInsID)
				values (@p1, @p2)";
			cmd2.Parameters.Add("@p1", OutageID);
			cmd2.Parameters.Add("@p2", "");

			SqlCeCommand cmd3 = Globals.cnn.CreateCommand();
			cmd3.CommandText =
				@"Update Inspectors set InsKitID = null
					where InsDBid = @p2";
			cmd3.Parameters.Add("@p2", "");

			// Now insert the current inspector assignments
			foreach (EInspector ins in inss)
			{
				if (ins.isAssignedToYourOutage)
				{
					cmd.Parameters["@p2"].Value = ins.ID;
					cmd2.Parameters["@p2"].Value = ins.ID;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();

					object val = cmd.ExecuteScalar();

					if (val == null)
					{
						if (cmd2.ExecuteNonQuery() != 1)
						{
							throw new Exception("Unable to insert Outage Inspectors row");
						}
					}
				}
				else
				{
					// Make sure the kit assignments are null for inspectors who are removed from
					// the outage.
					cmd3.Parameters["@p2"].Value = ins.ID;
					cmd3.ExecuteNonQuery();
					ins.OnInspectorKitAssignmentsChanged();
				}
			}
			// Just throw one event for all
			EInspector ins2 = new EInspector();
			ins2.OnInspectorOutageAssignmentsChanged();
		}

		// Fill a datatable for binding to a combobox with ID's and names of
		// inspectors.
		public static DataTable GetComboBoxView(bool includeInactive, bool addNoSelection)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			// Changing the booleans to 'Yes' and 'No' eliminates the silly checkboxes and
			// makes the column sortable.
			// You'll likely want to modify this query further, joining in other tables, etc.
			string qry =
				@"Select 
				InsDBid as ID,
				InsName as Name,
				InsKitID as KitID
				from Inspectors";

			if (!includeInactive)
				qry += " and CmpIsActive = 1";

			qry += " order by Name";

			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			if (addNoSelection)
			{
				DataRow dr = ds.Tables[0].NewRow();
				dr[0] = DBNull.Value;
				dr[1] = "<No Selection>";
				dr[2] = DBNull.Value;
				ds.Tables[0].Rows.InsertAt(dr, 0);
			}
			return ds.Tables[0];
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.

		// Pass a null outage id if you don't want to limit to the current outage.
		public static DataView GetDefaultDataView(Guid? OutageID)
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
					InsDBid as ID,
					InsName as InspectorName,
					CASE
						WHEN InsLevel = 1 THEN 'Level I'
						WHEN InsLevel = 2 THEN 'Level II'
						WHEN InsLevel = 3 THEN 'Level III'
						WHEN InsLevel = 4 THEN 'Level IIL'
						ELSE 'Unknown Level'
					END as InspectorLevel,
					KitName as InspectorKitName,
					CASE
						WHEN InsIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as InspectorIsLclChg,
					CASE
						WHEN InsUsedInOutage = 0 THEN 'No'
						ELSE 'Yes'
					END as InspectorUsedInOutage,
					CASE
						WHEN InsIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as InspectorIsActive
					from Inspectors left outer join kits on InsKitID = KitDBid";
			if (OutageID != null)
			{
				qry +=
					@" Inner join OutageInspectors on InsDBid = OinInsID
					where OinOtgID = @p1";
				cmd.Parameters.Add("@p1", OutageID);
			}
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}
		
		public static DataTable GetWithAssignmentsForOutageAndKit(
			Guid OutageID, Guid? KitID, bool includeInactive)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			string qry =
				@"select 
				InsDBid as ID,
				InsName as InspectorName,
				InsLevel as InspectorLevel,
				InsIsActive as InspectorIsActive,
				CASE
				WHEN InsKitID is null THEN 0 ELSE 1
				END as IsAssigned
				from Inspectors left join Kits on InsKitID = KitDBid
				inner join OutageInspectors on InsDBid = OinInsID
				where (InsKitID is NULL or InsKitID = @p1)
				and OinOtgID = @p2";

			if (!includeInactive) qry += " and InsIsActive = 1";

			qry += " order by InsName";

			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", KitID == null ? Guid.Empty : KitID);
			cmd.Parameters.Add("@p2", OutageID);
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
			cmd.CommandText = @"Update Inspectors set InsKitID = NULL where InsKitID = @p1";
			cmd.Parameters.Add("@p1", KitID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Then add the selected ducers back in
			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", KitID);
			cmd.Parameters.Add("@p2", "");
			cmd.CommandText =
@"				Update Inspectors set InsKitID = @p1 
				where InsDBid = @p2";

			// Now insert the current assignments
			for (int dmRow = 0; dmRow < assignments.Rows.Count; dmRow++)
			{
				if (Convert.ToBoolean(assignments.Rows[dmRow]["IsAssigned"]))
				{
					cmd.Parameters["@p2"].Value = (Guid)assignments.Rows[dmRow]["ID"];
					if (cmd.ExecuteNonQuery() != 1)
					{
						throw new Exception("Unable to assign Kit to Inspector");
					}
				}
			}
			EInspector insp = new EInspector();
			insp.OnInspectorKitAssignmentsChanged();
		}

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
				cmd.CommandText = "Select InsIsActive from Inspectors where InsName = @p1";
			}
			else
			{
				cmd.CommandText = "Select InsIsActive from Inspectors where InsName = @p1 and InsDBid != @p0";
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

			if (InspectorName == null)
			{
				InsNameErrMsg = "A unique Inspector Name is required";
				allFilled = false;
			}
			else
			{
				InsNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Inspector Collection class
	//--------------------------------------
	public class EInspectorCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EInspectorCollection()
		{ }
		//the indexer of the collection
		public EInspector this[int index]
		{
			get
			{
				return (EInspector)this.List[index];
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
			foreach (EInspector inspector in InnerList)
			{
				if (inspector.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EInspector item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EInspector item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		public void Add(EInspector item, bool unique)
		{
			if (unique) this.AddUnique(item);
			else this.Add(item);
		}
		//adds a item to the collection only if it is not in the collection already
		public void AddUnique(EInspector item)
		{
			bool inList = false;
			foreach (object ins in this.List)
			{
				if (((EInspector)ins).ID == item.ID)
				{
					inList = true;
					break;
				}
			}

			if (!inList)
			{
				this.List.Add(item);
				OnChanged(EventArgs.Empty);
			}
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EInspector item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EInspector item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}

	public class InspectorLevel
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
		public InspectorLevel(byte ID, string Name)
		{
			this.id = ID;
			this.name = Name;
		}
	}

}
