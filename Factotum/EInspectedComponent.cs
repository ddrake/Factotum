using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EInspectedComponent : IEntity
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
		private Guid? IscDBid;
		private string IscName;
		private string IscWorkOrder;
		private Guid? IscCmpID;
		private int? IscEdsNumber;
		private Guid? IscOtgID;
		private Guid? IscGrpID;
		private Guid? IscInsID;
		private bool IscIsReadyToInspect;
		private short? IscMinCount;
		private bool IscIsFinal;
		private bool IscIsUtFieldComplete;
		private DateTime? IscReportPrintedOn;
		private DateTime? IscReportSubmittedOn;
		private DateTime? IscCompletionReportedOn;
		private string IscAreaSpecifier;
		private short? IscPageCountOverride;

		// Textbox limits
		public static int IscNameCharLimit = 50;
		public static int IscWorkOrderCharLimit = 50;
		public static int IscAreaSpecifierCharLimit = 25;
		public static int IscPageCountOverrideCharLimit = 6;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string IscNameErrMsg;
		private string IscWorkOrderErrMsg;
		private string IscIsReadyToInspectErrMsg;
		private string IscAreaSpecifierErrMsg;
		private string IscPageCountOverrideErrMsg;
		private string IscCmpIdErrMsg;

		// Form level validation message
		private string IscErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return IscDBid; }
		}

		public string InspComponentName
		{
			get { return IscName; }
			set { IscName = Util.NullifyEmpty(value); }
		}

		public string InspComponentWorkOrder
		{
			get { return IscWorkOrder; }
			set { IscWorkOrder = Util.NullifyEmpty(value); }
		}

		public Guid? InspComponentCmpID
		{
			get { return IscCmpID; }
			set { IscCmpID = value; }
		}

		public int? InspComponentEdsNumber
		{
			get { return IscEdsNumber; }
			set { IscEdsNumber = value; }
		}

		public Guid? InspComponentOtgID
		{
			get { return IscOtgID; }
			set 
			{
				// We're setting the parent for a new record, so get the next EDS #
				// in the sequence.
				IscEdsNumber = getNewEdsNumber((Guid)value);
				IscOtgID = value; 
			}
		}

		public Guid? InspComponentGrpID
		{
			get { return IscGrpID; }
			set { IscGrpID = value; }
		}

		public Guid? InspComponentInsID
		{
			get { return IscInsID; }
			set { IscInsID = value; }
		}

		public bool InspComponentIsReadyToInspect
		{
			get { return IscIsReadyToInspect; }
			set { IscIsReadyToInspect = value; }
		}

		public short? InspComponentMinCount
		{
			get { return IscMinCount; }
			set { IscMinCount = value; }
		}

		public bool InspComponentIsFinal
		{
			get { return IscIsFinal; }
			set { IscIsFinal = value; }
		}

		public bool InspComponentIsUtFieldComplete
		{
			get { return IscIsUtFieldComplete; }
			set { IscIsUtFieldComplete = value; }
		}

		public DateTime? InspComponentReportPrintedOn
		{
			get { return IscReportPrintedOn; }
			set { IscReportPrintedOn = value; }
		}

		public DateTime? InspComponentReportSubmittedOn
		{
			get { return IscReportSubmittedOn; }
			set { IscReportSubmittedOn = value; }
		}
		public DateTime? InspComponentCompletionReportedOn
		{
			get { return IscCompletionReportedOn; }
			set { IscCompletionReportedOn = value; }
		}

		public string InspComponentAreaSpecifier
		{
			get { return IscAreaSpecifier; }
			set { IscAreaSpecifier = Util.NullifyEmpty(value); }
		}

		public short? InspComponentPageCountOverride
		{
			get { return IscPageCountOverride; }
			set { IscPageCountOverride = value; }
		}

		public string InspComponentComponentName
		{
			get
			{
				if (IscCmpID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select CmpName from Components 
					where CmpDBid = @p1";

				cmd.Parameters.Add("@p1", IscCmpID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		public string InspComponentGridProcedureName
		{
			get
			{
				if (IscGrpID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select GrpName from GridProcedures 
					where GrpDBid = @p1";

				cmd.Parameters.Add("@p1", IscGrpID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		public DateTime InspComponentInspectedOn
		{
			get
			{
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"Select Max(IpdOutAt) from 
					((InspectionPeriods Inner Join Dsets on IpdDstID = DstDBid) 
						inner join Inspections on DstIspID = IspDBid)
						where IspIscID =  @p1";

				cmd.Parameters.Add("@p1", IscDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return Convert.ToDateTime(val);
				return DateTime.Now;
			}
		}

		
		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string InspComponentNameErrMsg
		{
			get { return IscNameErrMsg; }
		}

		public string InspComponentWorkOrderErrMsg
		{
			get { return IscWorkOrderErrMsg; }
		}

		public string InspComponentIsReadyToInspectErrMsg
		{
			get { return IscIsReadyToInspectErrMsg; }
		}

		public string InspComponentAreaSpecifierErrMsg
		{
			get { return IscAreaSpecifierErrMsg; }
		}

		public string InspComponentPageCountOverrideErrMsg
		{
			get { return IscPageCountOverrideErrMsg; }
		}
		public string InspComponentCmpIDErrMsg
		{
			get { return IscCmpIdErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string InspComponentErrMsg
		{
			get { return IscErrMsg; }
			set { IscErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool InspComponentNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > IscNameCharLimit)
			{
				IscNameErrMsg = string.Format("Report Names cannot exceed {0} characters", IscNameCharLimit);
				return false;
			}
			else
			{
				IscNameErrMsg = null;
				return true;
			}
		}

		public bool InspComponentWorkOrderLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > IscWorkOrderCharLimit)
			{
				IscWorkOrderErrMsg = string.Format("Report Work Orders cannot exceed {0} characters", IscWorkOrderCharLimit);
				return false;
			}
			else
			{
				IscWorkOrderErrMsg = null;
				return true;
			}
		}

		public bool InspComponentAreaSpecifierLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > IscAreaSpecifierCharLimit)
			{
				IscAreaSpecifierErrMsg = string.Format("Report Area Specifiers cannot exceed {0} characters", IscAreaSpecifierCharLimit);
				return false;
			}
			else
			{
				IscAreaSpecifierErrMsg = null;
				return true;
			}
		}

		public bool InspComponentPageCountOverrideLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > IscPageCountOverrideCharLimit)
			{
				IscPageCountOverrideErrMsg = string.Format("Report Page Count Overrides cannot exceed {0} characters", IscPageCountOverrideCharLimit);
				return false;
			}
			else
			{
				IscPageCountOverrideErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------


		public bool InspComponentNameValid(string name, Guid? outageID)
		{
			if (!InspComponentNameLengthOk(name)) return false;

			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForOutage(name, outageID, IscDBid))
			{
				IscNameErrMsg = "That Report Name is being used by another report in this outage.";
				return false;
			}
			IscNameErrMsg = null;
			return true;
		}

		public bool InspComponentCmpIDValid(Guid? componentID)
		{
			if (componentID == null || componentID == Guid.Empty)
			{
				IscCmpIdErrMsg = "Every report must specify a component.";
				return false;
			}

			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (InspectedComponentExistsForComponent(componentID, IscDBid))
			{
				IscCmpIdErrMsg = "A report already exists for that component.";
				return false;
			}
			IscCmpIdErrMsg = null;
			return true;
		}

		public bool InspComponentWorkOrderValid(string value)
		{
			if (!InspComponentWorkOrderLengthOk(value)) return false;

			IscWorkOrderErrMsg = null;
			return true;
		}

		public bool InspComponentIsReadyToInspectValid(bool value)
		{
			// Add some real validation here if needed.
			IscIsReadyToInspectErrMsg = null;
			return true;
		}

		public bool InspComponentAreaSpecifierValid(string value)
		{
			if (!InspComponentAreaSpecifierLengthOk(value)) return false;

			IscAreaSpecifierErrMsg = null;
			return true;
		}

		public bool InspComponentPageCountOverrideValid(string value)
		{
			short result;
			if (Util.IsNullOrEmpty(value))
			{
				IscPageCountOverrideErrMsg = null;
				return true;
			}
			if (short.TryParse(value, out result) && result > 0)
			{
				IscPageCountOverrideErrMsg = null;
				return true;
			}
			IscPageCountOverrideErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EInspectedComponent()
		{
			this.IscEdsNumber = 0;
			this.IscIsReadyToInspect = false;
			this.IscMinCount = 0;
			this.IscIsFinal = false;
			this.IscIsUtFieldComplete = false;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EInspectedComponent(Guid? id) : this()
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
				IscDBid,
				IscName,
				IscWorkOrder,
				IscCmpID,
				IscEdsNumber,
				IscOtgID,
				IscGrpID,
				IscInsID,
				IscIsReadyToInspect,
				IscMinCount,
				IscIsFinal,
				IscIsUtFieldComplete,
				IscReportPrintedOn,
				IscReportSubmittedOn,
				IscCompletionReportedOn,				
				IscAreaSpecifier,
				IscPageCountOverride
				from InspectedComponents
				where IscDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				IscDBid = (Guid?)dr[0];
				IscName = (string)dr[1];
				IscWorkOrder = (string)dr[2];
				IscCmpID = (Guid?)dr[3];
				IscEdsNumber = (int?)dr[4];
				IscOtgID = (Guid?)Util.NullForDbNull(dr[5]);
				IscGrpID = (Guid?)Util.NullForDbNull(dr[6]);
				IscInsID = (Guid?)Util.NullForDbNull(dr[7]);
				IscIsReadyToInspect = (bool)dr[8];
				IscMinCount = (short?)dr[9];
				IscIsFinal = (bool)dr[10];
				IscIsUtFieldComplete = (bool)dr[11];
				IscReportPrintedOn = (DateTime?)Util.NullForDbNull(dr[12]);
				IscReportSubmittedOn = (DateTime?)Util.NullForDbNull(dr[13]);
				IscCompletionReportedOn = (DateTime?)Util.NullForDbNull(dr[14]);
				IscAreaSpecifier = (string)Util.NullForDbNull(dr[15]);
				IscPageCountOverride = (short?)Util.NullForDbNull(dr[16]);
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
				IscDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", IscDBid),
					new SqlCeParameter("@p1", IscName),
					new SqlCeParameter("@p2", IscWorkOrder),
					new SqlCeParameter("@p3", IscCmpID),
					new SqlCeParameter("@p4", IscEdsNumber),
					new SqlCeParameter("@p5", Util.DbNullForNull(IscOtgID)),
					new SqlCeParameter("@p6", Util.DbNullForNull(IscGrpID)),
					new SqlCeParameter("@p7", Util.DbNullForNull(IscInsID)),
					new SqlCeParameter("@p8", IscIsReadyToInspect),
					new SqlCeParameter("@p9", IscMinCount),
					new SqlCeParameter("@p10", IscIsFinal),
					new SqlCeParameter("@p11", IscIsUtFieldComplete),
					new SqlCeParameter("@p12", Util.DbNullForNull(IscReportPrintedOn)),
					new SqlCeParameter("@p13", Util.DbNullForNull(IscReportSubmittedOn)),
					new SqlCeParameter("@p14", Util.DbNullForNull(IscCompletionReportedOn)),					
					new SqlCeParameter("@p15", Util.DbNullForNull(IscAreaSpecifier)),
					new SqlCeParameter("@p16", Util.DbNullForNull(IscPageCountOverride))
					});
				cmd.CommandText = @"Insert Into InspectedComponents (
					IscDBid,
					IscName,
					IscWorkOrder,
					IscCmpID,
					IscEdsNumber,
					IscOtgID,
					IscGrpID,
					IscInsID,
					IscIsReadyToInspect,
					IscMinCount,
					IscIsFinal,
					IscIsUtFieldComplete,
					IscReportPrintedOn,
					IscReportSubmittedOn,
					IscCompletionReportedOn,
					IscAreaSpecifier,
					IscPageCountOverride
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Report row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", IscDBid),
					new SqlCeParameter("@p1", IscName),
					new SqlCeParameter("@p2", IscWorkOrder),
					new SqlCeParameter("@p3", IscCmpID),
					new SqlCeParameter("@p4", IscEdsNumber),
					new SqlCeParameter("@p5", Util.DbNullForNull(IscOtgID)),
					new SqlCeParameter("@p6", Util.DbNullForNull(IscGrpID)),
					new SqlCeParameter("@p7", Util.DbNullForNull(IscInsID)),
					new SqlCeParameter("@p8", IscIsReadyToInspect),
					new SqlCeParameter("@p9", IscMinCount),
					new SqlCeParameter("@p10", IscIsFinal),
					new SqlCeParameter("@p11", IscIsUtFieldComplete),
					new SqlCeParameter("@p12", Util.DbNullForNull(IscReportPrintedOn)),
					new SqlCeParameter("@p13", Util.DbNullForNull(IscReportSubmittedOn)),
					new SqlCeParameter("@p14", Util.DbNullForNull(IscCompletionReportedOn)),
					new SqlCeParameter("@p15", Util.DbNullForNull(IscAreaSpecifier)),
					new SqlCeParameter("@p16", Util.DbNullForNull(IscPageCountOverride))});

				cmd.CommandText =
					@"Update InspectedComponents 
					set					
					IscName = @p1,					
					IscWorkOrder = @p2,					
					IscCmpID = @p3,					
					IscEdsNumber = @p4,					
					IscOtgID = @p5,					
					IscGrpID = @p6,					
					IscInsID = @p7,					
					IscIsReadyToInspect = @p8,					
					IscMinCount = @p9,					
					IscIsFinal = @p10,					
					IscIsUtFieldComplete = @p11,					
					IscReportPrintedOn = @p12,					
					IscReportSubmittedOn = @p13,					
					IscCompletionReportedOn = @p14,					
					IscAreaSpecifier = @p15,					
					IscPageCountOverride = @p16
					Where IscDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update Report row");
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
			if (!InspComponentNameValid(InspComponentName,InspComponentOtgID)) return false;
			if (!InspComponentWorkOrderValid(InspComponentWorkOrder)) return false;
			if (!InspComponentIsReadyToInspectValid(InspComponentIsReadyToInspect)) return false;
			if (!InspComponentAreaSpecifierValid(InspComponentAreaSpecifier)) return false;
			if (!InspComponentCmpIDValid(InspComponentCmpID)) return false;

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
			if (IscDBid == null)
			{
				InspComponentErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (HasChildren())
			{
				InspComponentErrMsg = "Unable to delete because this Report contains Inspections (Report Sections).";
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
					@"Delete from InspectedComponents 
					where IscDBid = @p0";
				cmd.Parameters.Add("@p0", IscDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					InspComponentErrMsg = "Unable to delete.  Please try again later.";
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
				InspComponentErrMsg = null;
				return false;
			}
		}

		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select IspDBid from Inspections
					where IspIscID = @p0";
			cmd.Parameters.Add("@p0", IscDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of inspcomponents
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EInspectedComponentCollection ListByWorkOrderForOutage(Guid? OutageID,
			bool addNoSelection)
		{
			EInspectedComponent inspcomponent;
			EInspectedComponentCollection inspcomponents = new EInspectedComponentCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				inspcomponent = new EInspectedComponent();
				inspcomponent.IscName = "<No Selection>";
				inspcomponents.Add(inspcomponent);
			}

			if (OutageID == null) return inspcomponents;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				IscDBid,
				IscName,
				IscWorkOrder,
				IscCmpID,
				IscEdsNumber,
				IscOtgID,
				IscGrpID,
				IscInsID,
				IscIsReadyToInspect,
				IscMinCount,
				IscIsFinal,
				IscIsUtFieldComplete,
				IscReportPrintedOn,
				IscReportSubmittedOn,
				IscCompletionReportedOn,
				IscAreaSpecifier,
				IscPageCountOverride
				from InspectedComponents
				inner join Components on IscCmpID = CmpDBid";

			qry += "	order by IscWorkOrder, CmpName";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				inspcomponent = new EInspectedComponent((Guid?)dr[0]);
				inspcomponent.IscName = (string)(dr[1]);
				inspcomponent.IscWorkOrder = (string)(dr[2]);
				inspcomponent.IscCmpID = (Guid?)(dr[3]);
				inspcomponent.IscEdsNumber = (int?)(dr[4]);
				inspcomponent.IscOtgID = (Guid?)Util.NullForDbNull(dr[5]);
				inspcomponent.IscGrpID = (Guid?)Util.NullForDbNull(dr[6]);
				inspcomponent.IscInsID = (Guid?)Util.NullForDbNull(dr[7]);
				inspcomponent.IscIsReadyToInspect = (bool)(dr[8]);
				inspcomponent.IscMinCount = (short?)(dr[9]);
				inspcomponent.IscIsFinal = (bool)(dr[10]);
				inspcomponent.IscIsUtFieldComplete = (bool)(dr[11]);
				inspcomponent.IscReportPrintedOn = (DateTime?)Util.NullForDbNull(dr[12]);
				inspcomponent.IscReportSubmittedOn = (DateTime?)Util.NullForDbNull(dr[13]);
				inspcomponent.IscCompletionReportedOn = (DateTime?)Util.NullForDbNull(dr[14]);				
				inspcomponent.IscAreaSpecifier = (string)Util.NullForDbNull(dr[15]);
				inspcomponent.IscPageCountOverride = (short?)Util.NullForDbNull(dr[16]);

				inspcomponents.Add(inspcomponent);	
			}
			// Finish up
			dr.Close();
			return inspcomponents;
		}

		private short getNewEdsNumber(Guid outageID)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			
			cmd.Parameters.Add(new SqlCeParameter("@p1", outageID));
			cmd.CommandText = "Select Max(IscEdsNumber) from InspectedComponents where IscOtgID = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			short newReportOrder = (short)(val == null ? 1 : Convert.ToUInt16(val) + 1);
			return newReportOrder;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataViewForOutage(Guid? outageID)
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
					IscDBid as ID,
					IscEdsNumber as InspComponentEdsNumber,
					IscInsID as InspComponentInsID,
					IscName as InspComponentName,
					CmpName as InspComponentComponentName,
					IscWorkOrder as InspComponentWorkOrder,
					Count(IspDBid) as InspNumberOfInspections,
					InsName as InspComponentInspectorName,
					GrpName as InspComponentGridProcedureName,
					CASE
						WHEN IscIsReadyToInspect = 0 THEN 'No'
						ELSE 'Yes'
					END as InspComponentIsReadyToInspect,
					CASE
						WHEN IscIsUtFieldComplete = 0 THEN 'No'
						ELSE 'Yes'
					END as InspComponentIsUtFieldComplete,
					IscMinCount as InspComponentMinCount,
					CASE
						WHEN IscIsFinal = 0 THEN 'No'
						ELSE 'Yes'
					END as InspComponentIsFinal,
					IscReportSubmittedOn as InspComponentReportSubmittedOn,
					IscCompletionReportedOn as InspComponentCompletionReportedOn,
					IscAreaSpecifier as InspComponentAreaSpecifier,
					IscPageCountOverride as InspComponentPageCountOverride
					from InspectedComponents
					left outer join Inspections on IscDBid = IspIscID
					left outer join Inspectors on IscInsID = InsDBid
					left outer join GridProcedures on IscGrpID = GrpDBid
					left outer join Components on IscCmpID = CmpDBid
					where IscOtgID = @p1
					group by IscDBid, IscInsID, IscName, CmpName, IscWorkOrder, 
						IscEdsNumber, InsName, GrpName, IscIsReadyToInspect, IscIsUtFieldComplete,
						IscMinCount, IscIsFinal, IscReportSubmittedOn, IscCompletionReportedOn, IscAreaSpecifier, IscPageCountOverride";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", outageID == null ? Guid.Empty : outageID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static EInspectedComponent FindForInspectedComponentName(string InspectedComponentName)
		{
			if (Util.IsNullOrEmpty(InspectedComponentName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", InspectedComponentName));
			cmd.CommandText = "Select IscDBid from InspectedComponents where IscName = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new EInspectedComponent((Guid)val);
			else return null;
		}

		public static EInspectedComponent FindForComponentName(Guid OutageID, string ComponentName)
		{			
			if (Util.IsNullOrEmpty(ComponentName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", ComponentName));
			cmd.Parameters.Add(new SqlCeParameter("@p2", OutageID));
			cmd.CommandText = 
				@"Select IscDBid from InspectedComponents
				inner join Components on IscCmpID = CmpDBid 
				where CmpName = @p1 and IscOtgID = @p2";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new EInspectedComponent((Guid)val);
			else return null;
		}

		//--------------------------------------
		// Utilities
		//--------------------------------------

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		public static bool NameExistsForOutage(string name, Guid? outageID, Guid? id)
		{
			if (outageID == null || outageID == Guid.Empty) return false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			cmd.Parameters.Add(new SqlCeParameter("@p2", outageID));
			if (id == null)
			{
				cmd.CommandText = "Select IscDBid from InspectedComponents where IscName = @p1 and IscOtgID = @p2";
			}
			else
			{
				cmd.CommandText = "Select IscDBid from InspectedComponents where IscName = @p1 and IscOtgID = @p2 and IscDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			return exists;
		}

		private bool InspectedComponentExistsForComponent(Guid? componentID, Guid? id)
		{
			// Note: the outageID should always be set before calling this function.
			// This is being done correctly in the InspectedComponentEdit form.
			if (IscOtgID == null) return false;
			if (componentID == null) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", componentID));
			cmd.Parameters.Add(new SqlCeParameter("@p2", IscOtgID));
			if (id == null)
			{
				// this is a new InspectedComponent
				cmd.CommandText = "Select IscDBid from InspectedComponents where IscCmpID = @p1 and IscOtgID = @p2";
			}
			else
			{
				// we're editing an InspectedComponent so we need to ignore the current id in the query
				cmd.CommandText = "Select IscDBid from InspectedComponents where IscCmpID = @p1 and IscOtgID = @p2 and IscDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			return exists;
		}



		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (InspComponentName == null)
			{
				IscNameErrMsg = "A unique Report Name is required";
				allFilled = false;
			}
			else
			{
				IscNameErrMsg = null;
			}
			if (InspComponentWorkOrder == null)
			{
				IscWorkOrderErrMsg = "A Report Work Order is required";
				allFilled = false;
			}
			else
			{
				IscWorkOrderErrMsg = null;
			}
			return allFilled;
		}

		public static void UpdateCompletionReported(bool utComplete, bool rptSubmitted)
		{
			List<Guid> reportsToUpdate = new List<Guid>(10);
			EInspectedComponent inspcomponent;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry;

			qry = @"Select IscDBid from InspectedComponents
				where IscCompletionReportedOn is NULL";

			if (utComplete || rptSubmitted)
			{
				string where2 = "";
				qry += " and (";
				if (utComplete) where2 += "or IscIsUtFieldComplete = 1";
				if (rptSubmitted) where2 += "or IscReportSubmittedOn is not NULL";
				where2 = where2.Substring(3);
				qry += where2 + ")";
			}
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				reportsToUpdate.Add((Guid)dr[0]);
			}
			// Finish up
			dr.Close();

			foreach (Guid rptID in reportsToUpdate)
			{
				inspcomponent = new EInspectedComponent(rptID);
				inspcomponent.InspComponentCompletionReportedOn = DateTime.Now;
				inspcomponent.Save();
			}
		}

		public void UpdateMinCount()
		{
			EInspectionCollection inspections = EInspection.ListByReportOrderForInspectedComponent((Guid)this.ID,false);
			EGrid grid;
			int? minCount;
			int totalMins = 0;
			foreach (EInspection insp in inspections)
			{
				Guid? gridID = insp.GridID;
				if (gridID != null)
				{
					grid = new EGrid(gridID);
					minCount = grid.GridBelowTscr;
					if (minCount != null) totalMins += (int)minCount;
				}
			}
			this.InspComponentMinCount = (short)totalMins;
		}
	}


	//--------------------------------------
	// InspComponent Collection class
	//--------------------------------------
	public class EInspectedComponentCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EInspectedComponentCollection()
		{ }
		//the indexer of the collection
		public EInspectedComponent this[int index]
		{
			get
			{
				return (EInspectedComponent)this.List[index];
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
			foreach (EInspectedComponent inspcomponent in InnerList)
			{
				if (inspcomponent.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EInspectedComponent item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EInspectedComponent item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EInspectedComponent item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EInspectedComponent item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
