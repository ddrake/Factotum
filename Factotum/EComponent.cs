using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EComponent : IEntity
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
		private Guid? CmpDBid;
		private string CmpName;
		private Guid? CmpUntID;
		private Guid? CmpCmtID;
		private Guid? CmpLinID;
		private Guid? CmpSysID;
		private Guid? CmpCtpID;
		private Guid? CmpPslID;
		private string CmpDrawing;
		private decimal? CmpUpMainOd;
		private decimal? CmpUpMainTnom;
		private decimal? CmpUpMainTscr;
		private decimal? CmpDnMainOd;
		private decimal? CmpDnMainTnom;
		private decimal? CmpDnMainTscr;
		private decimal? CmpBranchOd;
		private decimal? CmpBranchTnom;
		private decimal? CmpBranchTscr;
		private decimal? CmpUpExtOd;
		private decimal? CmpUpExtTnom;
		private decimal? CmpUpExtTscr;
		private decimal? CmpDnExtOd;
		private decimal? CmpDnExtTnom;
		private decimal? CmpDnExtTscr;
		private decimal? CmpBrExtOd;
		private decimal? CmpBrExtTnom;
		private decimal? CmpBrExtTscr;
		private int? CmpTimesInspected;
		private float? CmpAvgInspectionTime;
		private float? CmpAvgCrewDose;
		private decimal? CmpPctChromeMain;
		private decimal? CmpPctChromeBranch;
		private decimal? CmpPctChromeUsExt;
		private decimal? CmpPctChromeDsExt;
		private decimal? CmpPctChromeBrExt;
		private string CmpMisc1;
		private string CmpMisc2;
		private string CmpNote;
		private bool CmpHighRad;
		private bool CmpHardToAccess;
		private bool CmpHasDs;
		private bool CmpHasBranch;
		private bool CmpIsLclChg;
		private bool CmpUsedInOutage;
		private bool CmpIsActive;

		// Textbox limits
		public static int CmpNameCharLimit = 50;
		public static int CmpDrawingCharLimit = 50;
		public static int CmpUpMainOdCharLimit = 8;
		public static int CmpUpMainTnomCharLimit = 7;
		public static int CmpUpMainTscrCharLimit = 7;
		public static int CmpDnMainOdCharLimit = 8;
		public static int CmpDnMainTnomCharLimit = 7;
		public static int CmpDnMainTscrCharLimit = 7;
		public static int CmpBranchOdCharLimit = 8;
		public static int CmpBranchTnomCharLimit = 7;
		public static int CmpBranchTscrCharLimit = 7;
		public static int CmpUpExtOdCharLimit = 8;
		public static int CmpUpExtTnomCharLimit = 7;
		public static int CmpUpExtTscrCharLimit = 7;
		public static int CmpDnExtOdCharLimit = 8;
		public static int CmpDnExtTnomCharLimit = 7;
		public static int CmpDnExtTscrCharLimit = 7;
		public static int CmpBrExtOdCharLimit = 8;
		public static int CmpBrExtTnomCharLimit = 7;
		public static int CmpBrExtTscrCharLimit = 7;
		public static int CmpPctChromeMainCharLimit = 7;
		public static int CmpPctChromeBranchCharLimit = 7;
		public static int CmpPctChromeUsExtCharLimit = 7;
		public static int CmpPctChromeDsExtCharLimit = 7;
		public static int CmpPctChromeBrExtCharLimit = 7;
		public static int CmpMisc1CharLimit = 50;
		public static int CmpMisc2CharLimit = 50;
		public static int CmpNoteCharLimit = 256;
		public static int CmpTimesInspectedCharLimit = 4;
		public static int CmpAvgInspectionTimeCharLimit = 6;
		public static int CmpAvgCrewDoseCharLimit = 6;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string CmpNameErrMsg;
		private string CmpDrawingErrMsg;
		private string CmpUpMainOdErrMsg;
		private string CmpUpMainTnomErrMsg;
		private string CmpUpMainTscrErrMsg;
		private string CmpDnMainOdErrMsg;
		private string CmpDnMainTnomErrMsg;
		private string CmpDnMainTscrErrMsg;
		private string CmpBranchOdErrMsg;
		private string CmpBranchTnomErrMsg;
		private string CmpBranchTscrErrMsg;
		private string CmpUpExtOdErrMsg;
		private string CmpUpExtTnomErrMsg;
		private string CmpUpExtTscrErrMsg;
		private string CmpDnExtOdErrMsg;
		private string CmpDnExtTnomErrMsg;
		private string CmpDnExtTscrErrMsg;
		private string CmpBrExtOdErrMsg;
		private string CmpBrExtTnomErrMsg;
		private string CmpBrExtTscrErrMsg;
		private string CmpPctChromeMainErrMsg;
		private string CmpPctChromeBranchErrMsg;
		private string CmpPctChromeUsExtErrMsg;
		private string CmpPctChromeDsExtErrMsg;
		private string CmpPctChromeBrExtErrMsg;
		private string CmpMisc1ErrMsg;
		private string CmpMisc2ErrMsg;
		private string CmpNoteErrMsg;
		private string CmpTimesInspectedErrMsg;
		private string CmpAvgInspectionTimeErrMsg;
		private string CmpAvgCrewDoseErrMsg;

		// Form level validation message
		private string CmpErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return CmpDBid; }
		}

		public string ComponentName
		{
			get { return CmpName; }
			set { CmpName = Util.NullifyEmpty(value); }
		}

		public Guid? ComponentUntID
		{
			get { return CmpUntID; }
			set { CmpUntID = value; }
		}

		public Guid? ComponentCmtID
		{
			get { return CmpCmtID; }
			set { CmpCmtID = value; }
		}

		public Guid? ComponentLinID
		{
			get { return CmpLinID; }
			set { CmpLinID = value; }
		}

		public Guid? ComponentSysID
		{
			get { return CmpSysID; }
			set { CmpSysID = value; }
		}

		public Guid? ComponentCtpID
		{
			get { return CmpCtpID; }
			set { CmpCtpID = value; }
		}

		public Guid? ComponentPslID
		{
			get { return CmpPslID; }
			set { CmpPslID = value; }
		}

		public string ComponentDrawing
		{
			get { return CmpDrawing; }
			set { CmpDrawing = Util.NullifyEmpty(value); }
		}

		// Setting the Upstream main dimensions also sets the upstream extensions whether or not they
		// are null.
		// Further, it also sets the downstream extensions if the downstream main dimensions are null
		public decimal? ComponentUpMainOd
		{
			get { return CmpUpMainOd; }
			set 
			{
				if (value > 0)
				{
					CmpUpMainOd = value;
					CmpUpExtOd = value;
					if (CmpDnMainOd == null) CmpDnExtOd = value;
				}
				else
					CmpUpMainOd = null; // Convert zero to null
			}
		}

		public decimal? ComponentUpMainTnom
		{
			get { return CmpUpMainTnom; }
			set 
			{
				if (value > 0)
				{
					CmpUpMainTnom = value;
					CmpUpExtTnom = value;
					if (CmpDnMainTnom == null) CmpDnExtTnom = value;
				}
				else
					CmpUpMainTnom = null;
			}
		}

		public decimal? ComponentUpMainTscr
		{
			get { return CmpUpMainTscr; }
			set 
			{
				if (value > 0)
				{
					CmpUpMainTscr = value;
					CmpUpExtTscr = value;
					if (CmpDnMainTscr == null) CmpDnExtTscr = value;
				}
				else
					CmpUpMainTscr = null;
			}
		}

		// Whenever the Downstream main dimensions are set, the downstream extensions are also set
		// even if they are not null.  We need to do this because setting the Upstream main dimensions
		// also sets the downstream extensions if the downstream main is null and we want this 
		// logic to work regardless of the order in which dimensions are entered.
		// The only quirk now is that if non-null extensions are entered, this should be done
		// after setting the main dimensions.
		public decimal? ComponentDnMainOd
		{
			get { return CmpDnMainOd; }
			set 
			{
				if (value > 0)
				{
					CmpDnMainOd = value;
					CmpDnExtOd = value;
				}
				else
					CmpDnMainOd = null;
			}
		}

		public decimal? ComponentDnMainTnom
		{
			get { return CmpDnMainTnom; }
			set 
			{
				if (value > 0)
				{
					CmpDnMainTnom = value;
					CmpDnExtTnom = value;
				}
				else
					CmpDnMainTnom = null;
			}
		}

		public decimal? ComponentDnMainTscr
		{
			get { return CmpDnMainTscr; }
			set 
			{
				if (value > 0)
				{
					CmpDnMainTscr = value;
					CmpDnExtTscr = value;
				}
				else
					CmpDnMainTscr = null;
			}
		}

		// Setting the branch dimensions also sets the branch extension dimensions 
		// even if they are not null
		public decimal? ComponentBranchOd
		{
			get { return CmpBranchOd; }
			set 
			{
				if (value > 0)
				{
					CmpBranchOd = value;
					CmpBrExtOd = value;
				}
				else
					CmpBranchOd = null;
			}
		}

		public decimal? ComponentBranchTnom
		{
			get { return CmpBranchTnom; }
			set 
			{
				if (value > 0)
				{
					CmpBranchTnom = value;
					CmpBrExtTnom = value;
				}
				else
					CmpBranchTnom = null;
			}
		}

		public decimal? ComponentBranchTscr
		{
			get { return CmpBranchTscr; }
			set 
			{
				if (value > 0)
				{
					CmpBranchTscr = value;
					CmpBrExtTscr = value;
				}
				else
					CmpBranchTscr = null;
			}
		}

		// Component Extension dimensions are ONLY set if a positive value is provided.
		// otherwise the set is ignored.  This is because they generally should match the 
		// corresponding main dimensions.  We don't want them set to null just because no value is
		// provided.
		public decimal? ComponentUpExtOd
		{
			get { return CmpUpExtOd; }
			set 
			{
				if (value > 0)	CmpUpExtOd = value;
			}
		}

		public decimal? ComponentUpExtTnom
		{
			get { return CmpUpExtTnom; }
			set 
			{
				if (value > 0)	CmpUpExtTnom = value;
			}
		}

		public decimal? ComponentUpExtTscr
		{
			get { return CmpUpExtTscr; }
			set 
			{
				if (value > 0)	CmpUpExtTscr = value;
			}
		}

		public decimal? ComponentDnExtOd
		{
			get { return CmpDnExtOd; }
			set 
			{
				if (value > 0)	CmpDnExtOd = value;
			}
		}

		public decimal? ComponentDnExtTnom
		{
			get { return CmpDnExtTnom; }
			set 
			{
				if (value > 0)	CmpDnExtTnom = value;
			}
		}

		public decimal? ComponentDnExtTscr
		{
			get { return CmpDnExtTscr; }
			set
			{
				if (value > 0)	CmpDnExtTscr = value;
			}
		}

		public decimal? ComponentBrExtOd
		{
			get { return CmpBrExtOd; }
			set
			{
				if (value > 0) CmpBrExtOd = value;
			}
		}

		public decimal? ComponentBrExtTnom
		{
			get { return CmpBrExtTnom; }
			set
			{
				if (value > 0)	CmpBrExtTnom = value;
			}
		}

		public decimal? ComponentBrExtTscr
		{
			get { return CmpBrExtTscr; }
			set
			{
				if (value > 0)	CmpBrExtTscr = value;
			}
		}

		public int? ComponentTimesInspected
		{
			get { return CmpTimesInspected; }
			set { CmpTimesInspected = value; }
		}

		public float? ComponentAvgInspectionTime
		{
			get { return CmpAvgInspectionTime; }
			set { CmpAvgInspectionTime = value; }
		}

		public float? ComponentAvgCrewDose
		{
			get { return CmpAvgCrewDose; }
			set { CmpAvgCrewDose = value; }
		}

		public decimal? ComponentPctChromeMain
		{
			get { return CmpPctChromeMain; }
			set { CmpPctChromeMain = value; }
		}

		public decimal? ComponentPctChromeBranch
		{
			get { return CmpPctChromeBranch; }
			set { CmpPctChromeBranch = value; }
		}

		public decimal? ComponentPctChromeUsExt
		{
			get { return CmpPctChromeUsExt; }
			set { CmpPctChromeUsExt = value; }
		}

		public decimal? ComponentPctChromeDsExt
		{
			get { return CmpPctChromeDsExt; }
			set { CmpPctChromeDsExt = value; }
		}

		public decimal? ComponentPctChromeBrExt
		{
			get { return CmpPctChromeBrExt; }
			set { CmpPctChromeBrExt = value; }
		}

		public string ComponentMisc1
		{
			get { return CmpMisc1; }
			set { CmpMisc1 = Util.NullifyEmpty(value); }
		}

		public string ComponentMisc2
		{
			get { return CmpMisc2; }
			set { CmpMisc2 = Util.NullifyEmpty(value); }
		}

		public string ComponentNote
		{
			get { return CmpNote; }
			set { CmpNote = Util.NullifyEmpty(value); }
		}

		public bool ComponentHighRad
		{
			get { return CmpHighRad; }
			set { CmpHighRad = value; }
		}

		public bool ComponentHardToAccess
		{
			get { return CmpHardToAccess; }
			set { CmpHardToAccess = value; }
		}

		public bool ComponentHasDs
		{
			get { return CmpHasDs; }
			set { CmpHasDs = value; }
		}

		public bool ComponentHasBranch
		{
			get { return CmpHasBranch; }
			set { CmpHasBranch = value; }
		}

		public bool ComponentIsLclChg
		{
			get { return CmpIsLclChg; }
			set { CmpIsLclChg = value; }
		}

		public bool ComponentUsedInOutage
		{
			get { return CmpUsedInOutage; }
			set { CmpUsedInOutage = value; }
		}

		public bool ComponentIsActive
		{
			get { return CmpIsActive; }
			set { CmpIsActive = value; }
		}

		public bool UpMainThicknessesDefined
		{
			get { return CmpUpMainTnom != null && CmpUpMainTscr != null; }
		}

		public bool DnMainThicknessesDefined
		{
			get { return CmpDnMainTnom != null && CmpDnMainTscr != null; }
		}

		public bool BranchThicknessesDefined
		{
			get { return CmpBranchTnom != null && CmpBranchTscr != null; }
		}

		// Need to use the properties here so that the defaults get handled correctly...
		public bool UpExtThicknessesDefined
		{
			get { return ComponentUpExtTnom != null && ComponentUpExtTscr != null; }
		}

		public bool DnExtThicknessesDefined
		{
			get { return ComponentDnExtTnom != null && ComponentDnExtTscr != null; }
		}

		public bool BranchExtThicknessesDefined
		{
			get { return ComponentBrExtTnom != null && ComponentBrExtTscr != null; }
		}

		// Used by Component Importer
		public string ComponentMaterialName
		{
			get
			{
				if (CmpCmtID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select CmtName from ComponentMaterials 
					where CmtDBid = @p1";

				cmd.Parameters.Add("@p1", CmpCmtID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		public string ComponentTypeName
		{
			get
			{
				if (CmpCtpID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select CtpName from ComponentTypes 
					where CtpDBid = @p1";

				cmd.Parameters.Add("@p1", CmpCtpID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		public string ComponentSystemName
		{
			get
			{
				if (CmpSysID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select SysName from Systems 
					where SysDBid = @p1";

				cmd.Parameters.Add("@p1", CmpSysID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		public string ComponentLineName
		{
			get
			{
				if (CmpLinID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select LinName from Lines 
					where LinDBid = @p1";

				cmd.Parameters.Add("@p1", CmpLinID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		public string ComponentCalBlockMaterialTypeName
		{
			get
			{
				if (CmpCmtID == null) return "N/A";
				EComponentMaterial material = new EComponentMaterial(CmpCmtID);
				CalBlockMaterialTypeEnum cbkMaterial = (CalBlockMaterialTypeEnum)(material.CmpMaterialCalBlockMaterial);
				if (cbkMaterial == CalBlockMaterialTypeEnum.StainlessSteel)
					return "S/S";
				else if (cbkMaterial == CalBlockMaterialTypeEnum.CarbonSteel)
					return "C/S";
				else if (cbkMaterial == CalBlockMaterialTypeEnum.Inconel)
					return "Inc";
				else
					return "N/A";
			}
		}
		public string ComponentSchedule
		{
			get
			{
				if (CmpPslID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select PslSchedule from PipeScheduleLookup
					where PslDBid = @p1";

				cmd.Parameters.Add("@p1", CmpPslID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return (string)cmd.ExecuteScalar();
			}
		}

		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string ComponentNameErrMsg
		{
			get { return CmpNameErrMsg; }
		}

		public string ComponentDrawingErrMsg
		{
			get { return CmpDrawingErrMsg; }
		}

		public string ComponentUpMainOdErrMsg
		{
			get { return CmpUpMainOdErrMsg; }
		}

		public string ComponentUpMainTnomErrMsg
		{
			get { return CmpUpMainTnomErrMsg; }
		}

		public string ComponentUpMainTscrErrMsg
		{
			get { return CmpUpMainTscrErrMsg; }
		}

		public string ComponentDnMainOdErrMsg
		{
			get { return CmpDnMainOdErrMsg; }
		}

		public string ComponentDnMainTnomErrMsg
		{
			get { return CmpDnMainTnomErrMsg; }
		}

		public string ComponentDnMainTscrErrMsg
		{
			get { return CmpDnMainTscrErrMsg; }
		}

		public string ComponentBranchOdErrMsg
		{
			get { return CmpBranchOdErrMsg; }
		}

		public string ComponentBranchTnomErrMsg
		{
			get { return CmpBranchTnomErrMsg; }
		}

		public string ComponentBranchTscrErrMsg
		{
			get { return CmpBranchTscrErrMsg; }
		}

		public string ComponentUpExtOdErrMsg
		{
			get { return CmpUpExtOdErrMsg; }
		}

		public string ComponentUpExtTnomErrMsg
		{
			get { return CmpUpExtTnomErrMsg; }
		}

		public string ComponentUpExtTscrErrMsg
		{
			get { return CmpUpExtTscrErrMsg; }
		}

		public string ComponentDnExtOdErrMsg
		{
			get { return CmpDnExtOdErrMsg; }
		}

		public string ComponentDnExtTnomErrMsg
		{
			get { return CmpDnExtTnomErrMsg; }
		}

		public string ComponentDnExtTscrErrMsg
		{
			get { return CmpDnExtTscrErrMsg; }
		}

		public string ComponentBrExtOdErrMsg
		{
			get { return CmpBrExtOdErrMsg; }
		}

		public string ComponentBrExtTnomErrMsg
		{
			get { return CmpBrExtTnomErrMsg; }
		}

		public string ComponentBrExtTscrErrMsg
		{
			get { return CmpBrExtTscrErrMsg; }
		}

		public string ComponentPctChromeMainErrMsg
		{
			get { return CmpPctChromeMainErrMsg; }
		}

		public string ComponentPctChromeBranchErrMsg
		{
			get { return CmpPctChromeBranchErrMsg; }
		}

		public string ComponentPctChromeUsExtErrMsg
		{
			get { return CmpPctChromeUsExtErrMsg; }
		}

		public string ComponentPctChromeDsExtErrMsg
		{
			get { return CmpPctChromeDsExtErrMsg; }
		}

		public string ComponentPctChromeBrExtErrMsg
		{
			get { return CmpPctChromeBrExtErrMsg; }
		}

		public string ComponentMisc1ErrMsg
		{
			get { return CmpMisc1ErrMsg; }
		}

		public string ComponentMisc2ErrMsg
		{
			get { return CmpMisc2ErrMsg; }
		}

		public string ComponentNoteErrMsg
		{
			get { return CmpNoteErrMsg; }
		}

		public string ComponentTimesInspectedErrMsg
		{
			get { return CmpTimesInspectedErrMsg; }
		}

		public string ComponentAvgInspectionTimeErrMsg
		{
			get { return CmpAvgInspectionTimeErrMsg; }
		}

		public string ComponentAvgCrewDoseErrMsg
		{
			get { return CmpAvgCrewDoseErrMsg; }
		}


		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string ComponentErrMsg
		{
			get { return CmpErrMsg; }
			set { CmpErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool ComponentNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpNameCharLimit)
			{
				CmpNameErrMsg = string.Format("Component Names cannot exceed {0} characters", CmpNameCharLimit);
				return false;
			}
			else
			{
				CmpNameErrMsg = null;
				return true;
			}
		}

		public bool ComponentDrawingLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDrawingCharLimit)
			{
				CmpDrawingErrMsg = string.Format("Component Drawing Names cannot exceed {0} characters", CmpDrawingCharLimit);
				return false;
			}
			else
			{
				CmpDrawingErrMsg = null;
				return true;
			}
		}

		public bool ComponentUpMainOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpUpMainOdCharLimit)
			{
				CmpUpMainOdErrMsg = string.Format("Component Upstream Main OD's cannot exceed {0} characters", CmpUpMainOdCharLimit);
				return false;
			}
			else
			{
				CmpUpMainOdErrMsg = null;
				return true;
			}
		}

		public bool ComponentUpMainTnomLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpUpMainTnomCharLimit)
			{
				CmpUpMainTnomErrMsg = string.Format("Component Upstream Main Tnom's cannot exceed {0} characters", CmpUpMainTnomCharLimit);
				return false;
			}
			else
			{
				CmpUpMainTnomErrMsg = null;
				return true;
			}
		}

		public bool ComponentUpMainTscrLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpUpMainTscrCharLimit)
			{
				CmpUpMainTscrErrMsg = string.Format("Component Upstream Main Tscreen's cannot exceed {0} characters", CmpUpMainTscrCharLimit);
				return false;
			}
			else
			{
				CmpUpMainTscrErrMsg = null;
				return true;
			}
		}

		public bool ComponentDnMainOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDnMainOdCharLimit)
			{
				CmpDnMainOdErrMsg = string.Format("Component Downstream Main OD's cannot exceed {0} characters", CmpDnMainOdCharLimit);
				return false;
			}
			else
			{
				CmpDnMainOdErrMsg = null;
				return true;
			}
		}

		public bool ComponentDnMainTnomLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDnMainTnomCharLimit)
			{
				CmpDnMainTnomErrMsg = string.Format("Component Downstream Main Tnom's cannot exceed {0} characters", CmpDnMainTnomCharLimit);
				return false;
			}
			else
			{
				CmpDnMainTnomErrMsg = null;
				return true;
			}
		}

		public bool ComponentDnMainTscrLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDnMainTscrCharLimit)
			{
				CmpDnMainTscrErrMsg = string.Format("Component Downstream Main Tscreens cannot exceed {0} characters", CmpDnMainTscrCharLimit);
				return false;
			}
			else
			{
				CmpDnMainTscrErrMsg = null;
				return true;
			}
		}

		public bool ComponentBranchOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpBranchOdCharLimit)
			{
				CmpBranchOdErrMsg = string.Format("Component Branch OD's cannot exceed {0} characters", CmpBranchOdCharLimit);
				return false;
			}
			else
			{
				CmpBranchOdErrMsg = null;
				return true;
			}
		}

		public bool ComponentBranchTnomLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpBranchTnomCharLimit)
			{
				CmpBranchTnomErrMsg = string.Format("Component Branch Tnom's cannot exceed {0} characters", CmpBranchTnomCharLimit);
				return false;
			}
			else
			{
				CmpBranchTnomErrMsg = null;
				return true;
			}
		}

		public bool ComponentBranchTscrLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpBranchTscrCharLimit)
			{
				CmpBranchTscrErrMsg = string.Format("Component Branch Tscreens cannot exceed {0} characters", CmpBranchTscrCharLimit);
				return false;
			}
			else
			{
				CmpBranchTscrErrMsg = null;
				return true;
			}
		}

		public bool ComponentUpExtOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpUpExtOdCharLimit)
			{
				CmpUpExtOdErrMsg = string.Format("Component U/S Ext OD's cannot exceed {0} characters", CmpUpExtOdCharLimit);
				return false;
			}
			else
			{
				CmpUpExtOdErrMsg = null;
				return true;
			}
		}

		public bool ComponentUpExtTnomLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpUpExtTnomCharLimit)
			{
				CmpUpExtTnomErrMsg = string.Format("Component U/S Ext Tnoms cannot exceed {0} characters", CmpUpExtTnomCharLimit);
				return false;
			}
			else
			{
				CmpUpExtTnomErrMsg = null;
				return true;
			}
		}

		public bool ComponentUpExtTscrLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpUpExtTscrCharLimit)
			{
				CmpUpExtTscrErrMsg = string.Format("Component U/S Ext Tscreens cannot exceed {0} characters", CmpUpExtTscrCharLimit);
				return false;
			}
			else
			{
				CmpUpExtTscrErrMsg = null;
				return true;
			}
		}

		public bool ComponentDnExtOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDnExtOdCharLimit)
			{
				CmpDnExtOdErrMsg = string.Format("Component D/S Ext OD's cannot exceed {0} characters", CmpDnExtOdCharLimit);
				return false;
			}
			else
			{
				CmpDnExtOdErrMsg = null;
				return true;
			}
		}

		public bool ComponentDnExtTnomLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDnExtTnomCharLimit)
			{
				CmpDnExtTnomErrMsg = string.Format("Component D/S Ext Tnoms cannot exceed {0} characters", CmpDnExtTnomCharLimit);
				return false;
			}
			else
			{
				CmpDnExtTnomErrMsg = null;
				return true;
			}
		}

		public bool ComponentDnExtTscrLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpDnExtTscrCharLimit)
			{
				CmpDnExtTscrErrMsg = string.Format("Component D/S Ext Tscreens cannot exceed {0} characters", CmpDnExtTscrCharLimit);
				return false;
			}
			else
			{
				CmpDnExtTscrErrMsg = null;
				return true;
			}
		}

		public bool ComponentBrExtOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpBrExtOdCharLimit)
			{
				CmpBrExtOdErrMsg = string.Format("Component Branch Ext OD's cannot exceed {0} characters", CmpBrExtOdCharLimit);
				return false;
			}
			else
			{
				CmpBrExtOdErrMsg = null;
				return true;
			}
		}

		public bool ComponentBrExtTnomLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpBrExtTnomCharLimit)
			{
				CmpBrExtTnomErrMsg = string.Format("Component Branch Ext Tnoms cannot exceed {0} characters", CmpBrExtTnomCharLimit);
				return false;
			}
			else
			{
				CmpBrExtTnomErrMsg = null;
				return true;
			}
		}

		public bool ComponentBrExtTscrLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpBrExtTscrCharLimit)
			{
				CmpBrExtTscrErrMsg = string.Format("Component Branch Ext Tscreens cannot exceed {0} characters", CmpBrExtTscrCharLimit);
				return false;
			}
			else
			{
				CmpBrExtTscrErrMsg = null;
				return true;
			}
		}

		public bool ComponentPctChromeMainLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpPctChromeMainCharLimit)
			{
				CmpPctChromeMainErrMsg = string.Format("Component Pct. Chrome cannot exceed {0} characters", CmpPctChromeMainCharLimit);
				return false;
			}
			else
			{
				CmpPctChromeMainErrMsg = null;
				return true;
			}
		}

		public bool ComponentPctChromeBranchLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpPctChromeBranchCharLimit)
			{
				CmpPctChromeBranchErrMsg = string.Format("Component Pct. Chrome Branch cannot exceed {0} characters", CmpPctChromeBranchCharLimit);
				return false;
			}
			else
			{
				CmpPctChromeBranchErrMsg = null;
				return true;
			}
		}

		public bool ComponentPctChromeUsExtLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpPctChromeUsExtCharLimit)
			{
				CmpPctChromeUsExtErrMsg = string.Format("Component Pct. Chrome U/S Ext cannot exceed {0} characters", CmpPctChromeUsExtCharLimit);
				return false;
			}
			else
			{
				CmpPctChromeUsExtErrMsg = null;
				return true;
			}
		}

		public bool ComponentPctChromeDsExtLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpPctChromeDsExtCharLimit)
			{
				CmpPctChromeDsExtErrMsg = string.Format("Component Pct. Chrome D/S Ext cannot exceed {0} characters", CmpPctChromeDsExtCharLimit);
				return false;
			}
			else
			{
				CmpPctChromeDsExtErrMsg = null;
				return true;
			}
		}

		public bool ComponentPctChromeBrExtLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpPctChromeBrExtCharLimit)
			{
				CmpPctChromeBrExtErrMsg = string.Format("Component Pct. Chrome Branch Ext cannot exceed {0} characters", CmpPctChromeBrExtCharLimit);
				return false;
			}
			else
			{
				CmpPctChromeBrExtErrMsg = null;
				return true;
			}
		}

		public bool ComponentMisc1LengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpMisc1CharLimit)
			{
				CmpMisc1ErrMsg = string.Format("Component Misc1 cannot exceed {0} characters", CmpMisc1CharLimit);
				return false;
			}
			else
			{
				CmpMisc1ErrMsg = null;
				return true;
			}
		}

		public bool ComponentMisc2LengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpMisc2CharLimit)
			{
				CmpMisc2ErrMsg = string.Format("Component Misc2 cannot exceed {0} characters", CmpMisc2CharLimit);
				return false;
			}
			else
			{
				CmpMisc2ErrMsg = null;
				return true;
			}
		}

		public bool ComponentNoteLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpNoteCharLimit)
			{
				CmpNoteErrMsg = string.Format("Component Notes cannot exceed {0} characters", CmpNoteCharLimit);
				return false;
			}
			else
			{
				CmpNoteErrMsg = null;
				return true;
			}
		}

		public bool ComponentTimesInspectedLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpTimesInspectedCharLimit)
			{
				CmpTimesInspectedErrMsg = string.Format("Times Inspected cannot exceed {0} characters", CmpTimesInspectedCharLimit);
				return false;
			}
			else
			{
				CmpTimesInspectedErrMsg = null;
				return true;
			}
		}

		public bool ComponentAvgInspectionTimeLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpAvgInspectionTimeCharLimit)
			{
				CmpAvgInspectionTimeErrMsg = string.Format("Component Avg Inspection Time cannot exceed {0} characters", CmpAvgInspectionTimeCharLimit);
				return false;
			}
			else
			{
				CmpAvgInspectionTimeErrMsg = null;
				return true;
			}
		}

		public bool ComponentAvgCrewDoseLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CmpAvgCrewDoseCharLimit)
			{
				CmpAvgCrewDoseErrMsg = string.Format("Component Avg Crew Dose cannot exceed {0} characters", CmpAvgCrewDoseCharLimit);
				return false;
			}
			else
			{
				CmpAvgCrewDoseErrMsg = null;
				return true;
			}
		}


		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool ComponentNameValid(string name)
		{
			bool existingIsInactive;
			if (!ComponentNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExistsForUnit(name, CmpDBid, (Guid)CmpUntID, out existingIsInactive))
			{
				CmpNameErrMsg = existingIsInactive ?
					"A Component with that Name exists but its status has been set to inactive." :
					"That Component Name is already in use.";
				return false;
			}
			CmpNameErrMsg = null;
			return true;
		}

		public bool ComponentDrawingValid(string value)
		{
			if (!ComponentDrawingLengthOk(value)) return false;

			CmpDrawingErrMsg = null;
			return true;
		}

		public bool ComponentUpMainOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpUpMainOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1000)
			{
				CmpUpMainOdErrMsg = null;
				return true;
			}
			CmpUpMainOdErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		public bool ComponentUpMainTnomValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpUpMainTnomErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpUpMainTnomErrMsg = null;
				return true;
			}
			CmpUpMainTnomErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentUpMainTscrValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpUpMainTscrErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpUpMainTscrErrMsg = null;
				return true;
			}
			CmpUpMainTscrErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentDnMainOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpDnMainOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1000)
			{
				CmpDnMainOdErrMsg = null;
				return true;
			}
			CmpDnMainOdErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		public bool ComponentDnMainTnomValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpDnMainTnomErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpDnMainTnomErrMsg = null;
				return true;
			}
			CmpDnMainTnomErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentDnMainTscrValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpDnMainTscrErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpDnMainTscrErrMsg = null;
				return true;
			}
			CmpDnMainTscrErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentBranchOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpBranchOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1000)
			{
				CmpBranchOdErrMsg = null;
				return true;
			}
			CmpBranchOdErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		public bool ComponentBranchTnomValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpBranchTnomErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpBranchTnomErrMsg = null;
				return true;
			}
			CmpBranchTnomErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentBranchTscrValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpBranchTscrErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpBranchTscrErrMsg = null;
				return true;
			}
			CmpBranchTscrErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}


		public bool ComponentUpExtOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpUpExtOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1000)
			{
				CmpUpExtOdErrMsg = null;
				return true;
			}
			CmpUpExtOdErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		public bool ComponentUpExtTnomValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpUpExtTnomErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpUpExtTnomErrMsg = null;
				return true;
			}
			CmpUpExtTnomErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentUpExtTscrValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpUpExtTscrErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpUpExtTscrErrMsg = null;
				return true;
			}
			CmpUpExtTscrErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentDnExtOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpDnExtOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1000)
			{
				CmpDnExtOdErrMsg = null;
				return true;
			}
			CmpDnExtOdErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		public bool ComponentDnExtTnomValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpDnExtTnomErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpDnExtTnomErrMsg = null;
				return true;
			}
			CmpDnExtTnomErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentDnExtTscrValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpDnExtTscrErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpDnExtTscrErrMsg = null;
				return true;
			}
			CmpDnExtTscrErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentBrExtOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpBrExtOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1000)
			{
				CmpBrExtOdErrMsg = null;
				return true;
			}
			CmpBrExtOdErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		public bool ComponentBrExtTnomValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpBrExtTnomErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpBrExtTnomErrMsg = null;
				return true;
			}
			CmpBrExtTnomErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentBrExtTscrValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpBrExtTscrErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				CmpBrExtTscrErrMsg = null;
				return true;
			}
			CmpBrExtTscrErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentPctChromeMainValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpPctChromeMainErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result >= 0 && result <= 100)
			{
				CmpPctChromeMainErrMsg = null;
				return true;
			}
			CmpPctChromeMainErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentPctChromeBranchValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpPctChromeBranchErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result >= 0 && result <= 100)
			{
				CmpPctChromeBranchErrMsg = null;
				return true;
			}
			CmpPctChromeBranchErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentPctChromeUsExtValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpPctChromeUsExtErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result >= 0 && result <= 100)
			{
				CmpPctChromeUsExtErrMsg = null;
				return true;
			}
			CmpPctChromeUsExtErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentPctChromeDsExtValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpPctChromeDsExtErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result >= 0 && result <= 100)
			{
				CmpPctChromeDsExtErrMsg = null;
				return true;
			}
			CmpPctChromeDsExtErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentPctChromeBrExtValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpPctChromeBrExtErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result >= 0 && result <= 100)
			{
				CmpPctChromeBrExtErrMsg = null;
				return true;
			}
			CmpPctChromeBrExtErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool ComponentMisc1Valid(string value)
		{
			if (!ComponentMisc1LengthOk(value)) return false;

			CmpMisc1ErrMsg = null;
			return true;
		}

		public bool ComponentMisc2Valid(string value)
		{
			if (!ComponentMisc2LengthOk(value)) return false;

			CmpMisc2ErrMsg = null;
			return true;
		}

		public bool ComponentNoteValid(string value)
		{
			if (!ComponentNoteLengthOk(value)) return false;

			CmpNoteErrMsg = null;
			return true;
		}
		public bool ComponentTimesInspectedValid(string value)
		{
			int result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpTimesInspectedErrMsg = null;
				return true;
			}
			if (int.TryParse(value, out result) && result >= 0 && result < 10000)
			{
				CmpTimesInspectedErrMsg = null;
				return true;
			}
			CmpTimesInspectedErrMsg = string.Format("Please enter a positive number less than 10000");
			return false;
		}
		public bool ComponentAvgInspectionTimeValid(string value)
		{
			float result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpAvgInspectionTimeErrMsg = null;
				return true;
			}
			if (float.TryParse(value, out result) && result >= 0 && result < 1000)
			{
				CmpAvgInspectionTimeErrMsg = null;
				return true;
			}
			CmpAvgInspectionTimeErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}
		public bool ComponentAvgCrewDoseValid(string value)
		{
			float result;
			if (Util.IsNullOrEmpty(value))
			{
				CmpAvgCrewDoseErrMsg = null;
				return true;
			}
			if (float.TryParse(value, out result) && result >= 0 && result < 1000)
			{
				CmpAvgCrewDoseErrMsg = null;
				return true;
			}
			CmpAvgCrewDoseErrMsg = string.Format("Please enter a positive number less than 1000");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EComponent()
		{
			this.CmpTimesInspected = 0;
			this.CmpAvgInspectionTime = 0;
			this.CmpAvgCrewDose = 0;
			this.CmpHighRad = false;
			this.CmpHardToAccess = false;
			this.CmpHasDs = false;
			this.CmpHasBranch = false;
			this.CmpIsLclChg = false;
			this.CmpUsedInOutage = false;
			this.CmpIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EComponent(Guid? id) : this()
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
				CmpDBid,
				CmpName,
				CmpUntID,
				CmpCmtID,
				CmpLinID,
				CmpSysID,
				CmpCtpID,
				CmpPslID,
				CmpDrawing,
				CmpUpMainOd,
				CmpUpMainTnom,
				CmpUpMainTscr,
				CmpDnMainOd,
				CmpDnMainTnom,
				CmpDnMainTscr,
				CmpBranchOd,
				CmpBranchTnom,
				CmpBranchTscr,
				CmpUpExtOd,
				CmpUpExtTnom,
				CmpUpExtTscr,
				CmpDnExtOd,
				CmpDnExtTnom,
				CmpDnExtTscr,
				CmpBrExtOd,
				CmpBrExtTnom,
				CmpBrExtTscr,
				CmpTimesInspected,
				CmpAvgInspectionTime,
				CmpAvgCrewDose,
				CmpPctChromeMain,
				CmpPctChromeBranch,
				CmpPctChromeUsExt,
				CmpPctChromeDsExt,
				CmpPctChromeBrExt,
				CmpMisc1,
				CmpMisc2,
				CmpNote,
				CmpHighRad,
				CmpHardToAccess,
				CmpHasDs,
				CmpHasBranch,
				CmpIsLclChg,
				CmpUsedInOutage,
				CmpIsActive
				from Components
				where CmpDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				CmpDBid = (Guid?)dr[0];
				CmpName = (string)dr[1];
				CmpUntID = (Guid?)dr[2];
				CmpCmtID = (Guid?)Util.NullForDbNull(dr[3]);
				CmpLinID = (Guid?)Util.NullForDbNull(dr[4]);
				CmpSysID = (Guid?)Util.NullForDbNull(dr[5]);
				CmpCtpID = (Guid?)Util.NullForDbNull(dr[6]);
				CmpPslID = (Guid?)Util.NullForDbNull(dr[7]);
				CmpDrawing = (string)Util.NullForDbNull(dr[8]);
				CmpUpMainOd = (decimal?)Util.NullForDbNull(dr[9]);
				CmpUpMainTnom = (decimal?)Util.NullForDbNull(dr[10]);
				CmpUpMainTscr = (decimal?)Util.NullForDbNull(dr[11]);
				CmpDnMainOd = (decimal?)Util.NullForDbNull(dr[12]);
				CmpDnMainTnom = (decimal?)Util.NullForDbNull(dr[13]);
				CmpDnMainTscr = (decimal?)Util.NullForDbNull(dr[14]);
				CmpBranchOd = (decimal?)Util.NullForDbNull(dr[15]);
				CmpBranchTnom = (decimal?)Util.NullForDbNull(dr[16]);
				CmpBranchTscr = (decimal?)Util.NullForDbNull(dr[17]);
				CmpUpExtOd = (decimal?)Util.NullForDbNull(dr[18]);
				CmpUpExtTnom = (decimal?)Util.NullForDbNull(dr[19]);
				CmpUpExtTscr = (decimal?)Util.NullForDbNull(dr[20]);
				CmpDnExtOd = (decimal?)Util.NullForDbNull(dr[21]);
				CmpDnExtTnom = (decimal?)Util.NullForDbNull(dr[22]);
				CmpDnExtTscr = (decimal?)Util.NullForDbNull(dr[23]);
				CmpBrExtOd = (decimal?)Util.NullForDbNull(dr[24]);
				CmpBrExtTnom = (decimal?)Util.NullForDbNull(dr[25]);
				CmpBrExtTscr = (decimal?)Util.NullForDbNull(dr[26]);
				CmpTimesInspected = (int?)dr[27];
				CmpAvgInspectionTime = Convert.ToSingle(dr[28]);
				CmpAvgCrewDose = Convert.ToSingle(dr[29]);
				CmpPctChromeMain = (decimal?)Util.NullForDbNull(dr[30]);
				CmpPctChromeBranch = (decimal?)Util.NullForDbNull(dr[31]);
				CmpPctChromeUsExt = (decimal?)Util.NullForDbNull(dr[32]);
				CmpPctChromeDsExt = (decimal?)Util.NullForDbNull(dr[33]);
				CmpPctChromeBrExt = (decimal?)Util.NullForDbNull(dr[34]);
				CmpMisc1 = (string)Util.NullForDbNull(dr[35]);
				CmpMisc2 = (string)Util.NullForDbNull(dr[36]);
				CmpNote = (string)Util.NullForDbNull(dr[37]);
				CmpHighRad = (bool)dr[38];
				CmpHardToAccess = (bool)dr[39];
				CmpHasDs = (bool)dr[40];
				CmpHasBranch = (bool)dr[41];
				CmpIsLclChg = (bool)dr[42];
				CmpUsedInOutage = (bool)dr[43];
				CmpIsActive = (bool)dr[44];
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
			// Try to set dimemsions for the downstream extension if they're not set.
			if (ComponentDnExtOd == null)
			{
				if (ComponentDnMainOd != null)
					ComponentDnExtOd = ComponentDnMainOd;
				else if (ComponentUpMainOd != null)
					ComponentDnExtOd = ComponentUpMainOd;
			}
			if (ComponentDnExtTnom == null)
			{
				if (ComponentDnMainTnom != null)
					ComponentDnExtTnom = ComponentDnMainTnom;
				else if (ComponentUpMainTnom != null)
					ComponentDnExtTnom = ComponentUpMainTnom;
			}
			if (ComponentDnExtTscr == null)
			{
				if (ComponentDnMainTscr != null)
					ComponentDnExtTscr = ComponentDnMainTscr;
				else if (ComponentUpMainTscr != null)
					ComponentDnExtTscr = ComponentUpMainTscr;
			}
			// Try to set dimensions for the upstream extension if they're not set.
			if (ComponentUpExtOd == null)
			{
				if (ComponentUpMainOd != null)
					ComponentUpExtOd = ComponentUpMainOd;
			}
			if (ComponentUpExtTnom == null)
			{
				if (ComponentUpMainTnom != null)
					ComponentUpExtTnom = ComponentUpMainTnom;
			}
			if (ComponentUpExtTscr == null)
			{
				if (ComponentUpMainTscr != null)
					ComponentUpExtTscr = ComponentUpMainTscr;
			}
			// Try to set dimensions for the branch extension if they're not set.
			if (ComponentBrExtOd == null)
			{
				if (ComponentBranchOd != null)
					ComponentBrExtOd = ComponentBranchOd;
			}
			if (ComponentBrExtTnom == null)
			{
				if (ComponentBranchTnom != null)
					ComponentBrExtTnom = ComponentBranchTnom;
			}
			if (ComponentBrExtTscr == null)
			{
				if (ComponentBranchTscr != null)
					ComponentBrExtTscr = ComponentBranchTscr;
			}

			if (ComponentDnMainOd != null && ComponentDnMainTnom != null &&
				ComponentDnMainTscr != null)
				ComponentHasDs = true;

			if (ComponentBranchOd != null && ComponentBranchTnom != null &&
				ComponentBranchTscr != null)
				ComponentHasBranch = true;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) CmpIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				CmpDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CmpDBid),
					new SqlCeParameter("@p1", CmpName),
					new SqlCeParameter("@p2", CmpUntID),
					new SqlCeParameter("@p3", Util.DbNullForNull(CmpCmtID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(CmpLinID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(CmpSysID)),
					new SqlCeParameter("@p6", Util.DbNullForNull(CmpCtpID)),
					new SqlCeParameter("@p7", Util.DbNullForNull(CmpPslID)),
					new SqlCeParameter("@p8", Util.DbNullForNull(CmpDrawing)),
					new SqlCeParameter("@p9", Util.DbNullForNull(CmpUpMainOd)),
					new SqlCeParameter("@p10", Util.DbNullForNull(CmpUpMainTnom)),
					new SqlCeParameter("@p11", Util.DbNullForNull(CmpUpMainTscr)),
					new SqlCeParameter("@p12", Util.DbNullForNull(CmpDnMainOd)),
					new SqlCeParameter("@p13", Util.DbNullForNull(CmpDnMainTnom)),
					new SqlCeParameter("@p14", Util.DbNullForNull(CmpDnMainTscr)),
					new SqlCeParameter("@p15", Util.DbNullForNull(CmpBranchOd)),
					new SqlCeParameter("@p16", Util.DbNullForNull(CmpBranchTnom)),
					new SqlCeParameter("@p17", Util.DbNullForNull(CmpBranchTscr)),
					new SqlCeParameter("@p18", Util.DbNullForNull(CmpUpExtOd)),
					new SqlCeParameter("@p19", Util.DbNullForNull(CmpUpExtTnom)),
					new SqlCeParameter("@p20", Util.DbNullForNull(CmpUpExtTscr)),
					new SqlCeParameter("@p21", Util.DbNullForNull(CmpDnExtOd)),
					new SqlCeParameter("@p22", Util.DbNullForNull(CmpDnExtTnom)),
					new SqlCeParameter("@p23", Util.DbNullForNull(CmpDnExtTscr)),
					new SqlCeParameter("@p24", Util.DbNullForNull(CmpBrExtOd)),
					new SqlCeParameter("@p25", Util.DbNullForNull(CmpBrExtTnom)),
					new SqlCeParameter("@p26", Util.DbNullForNull(CmpBrExtTscr)),
					new SqlCeParameter("@p27", CmpTimesInspected),
					new SqlCeParameter("@p28", CmpAvgInspectionTime),
					new SqlCeParameter("@p29", CmpAvgCrewDose),
					new SqlCeParameter("@p30", Util.DbNullForNull(CmpPctChromeMain)),
					new SqlCeParameter("@p31", Util.DbNullForNull(CmpPctChromeBranch)),
					new SqlCeParameter("@p32", Util.DbNullForNull(CmpPctChromeUsExt)),
					new SqlCeParameter("@p33", Util.DbNullForNull(CmpPctChromeDsExt)),
					new SqlCeParameter("@p34", Util.DbNullForNull(CmpPctChromeBrExt)),
					new SqlCeParameter("@p35", Util.DbNullForNull(CmpMisc1)),
					new SqlCeParameter("@p36", Util.DbNullForNull(CmpMisc2)),
					new SqlCeParameter("@p37", Util.DbNullForNull(CmpNote)),
					new SqlCeParameter("@p38", CmpHighRad),
					new SqlCeParameter("@p39", CmpHardToAccess),
					new SqlCeParameter("@p40", CmpHasDs),
					new SqlCeParameter("@p41", CmpHasBranch),
					new SqlCeParameter("@p42", CmpIsLclChg),
					new SqlCeParameter("@p43", CmpUsedInOutage),
					new SqlCeParameter("@p44", CmpIsActive)
					});
				cmd.CommandText = @"Insert Into Components (
					CmpDBid,
					CmpName,
					CmpUntID,
					CmpCmtID,
					CmpLinID,
					CmpSysID,
					CmpCtpID,
					CmpPslID,
					CmpDrawing,
					CmpUpMainOd,
					CmpUpMainTnom,
					CmpUpMainTscr,
					CmpDnMainOd,
					CmpDnMainTnom,
					CmpDnMainTscr,
					CmpBranchOd,
					CmpBranchTnom,
					CmpBranchTscr,
					CmpUpExtOd,
					CmpUpExtTnom,
					CmpUpExtTscr,
					CmpDnExtOd,
					CmpDnExtTnom,
					CmpDnExtTscr,
					CmpBrExtOd,
					CmpBrExtTnom,
					CmpBrExtTscr,
					CmpTimesInspected,
					CmpAvgInspectionTime,
					CmpAvgCrewDose,
					CmpPctChromeMain,
					CmpPctChromeBranch,
					CmpPctChromeUsExt,
					CmpPctChromeDsExt,
					CmpPctChromeBrExt,
					CmpMisc1,
					CmpMisc2,
					CmpNote,
					CmpHighRad,
					CmpHardToAccess,
					CmpHasDs,
					CmpHasBranch,
					CmpIsLclChg,
					CmpUsedInOutage,
					CmpIsActive
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20,@p21,@p22,@p23,@p24,@p25,@p26,@p27,@p28,@p29,@p30,@p31,@p32,@p33,@p34,@p35,@p36,@p37,@p38,@p39,@p40,@p41,@p42,@p43,@p44)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Components row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CmpDBid),
					new SqlCeParameter("@p1", CmpName),
					new SqlCeParameter("@p2", CmpUntID),
					new SqlCeParameter("@p3", Util.DbNullForNull(CmpCmtID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(CmpLinID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(CmpSysID)),
					new SqlCeParameter("@p6", Util.DbNullForNull(CmpCtpID)),
					new SqlCeParameter("@p7", Util.DbNullForNull(CmpPslID)),
					new SqlCeParameter("@p8", Util.DbNullForNull(CmpDrawing)),
					new SqlCeParameter("@p9", Util.DbNullForNull(CmpUpMainOd)),
					new SqlCeParameter("@p10", Util.DbNullForNull(CmpUpMainTnom)),
					new SqlCeParameter("@p11", Util.DbNullForNull(CmpUpMainTscr)),
					new SqlCeParameter("@p12", Util.DbNullForNull(CmpDnMainOd)),
					new SqlCeParameter("@p13", Util.DbNullForNull(CmpDnMainTnom)),
					new SqlCeParameter("@p14", Util.DbNullForNull(CmpDnMainTscr)),
					new SqlCeParameter("@p15", Util.DbNullForNull(CmpBranchOd)),
					new SqlCeParameter("@p16", Util.DbNullForNull(CmpBranchTnom)),
					new SqlCeParameter("@p17", Util.DbNullForNull(CmpBranchTscr)),
					new SqlCeParameter("@p18", Util.DbNullForNull(CmpUpExtOd)),
					new SqlCeParameter("@p19", Util.DbNullForNull(CmpUpExtTnom)),
					new SqlCeParameter("@p20", Util.DbNullForNull(CmpUpExtTscr)),
					new SqlCeParameter("@p21", Util.DbNullForNull(CmpDnExtOd)),
					new SqlCeParameter("@p22", Util.DbNullForNull(CmpDnExtTnom)),
					new SqlCeParameter("@p23", Util.DbNullForNull(CmpDnExtTscr)),
					new SqlCeParameter("@p24", Util.DbNullForNull(CmpBrExtOd)),
					new SqlCeParameter("@p25", Util.DbNullForNull(CmpBrExtTnom)),
					new SqlCeParameter("@p26", Util.DbNullForNull(CmpBrExtTscr)),
					new SqlCeParameter("@p27", CmpTimesInspected),
					new SqlCeParameter("@p28", CmpAvgInspectionTime),
					new SqlCeParameter("@p29", CmpAvgCrewDose),
					new SqlCeParameter("@p30", Util.DbNullForNull(CmpPctChromeMain)),
					new SqlCeParameter("@p31", Util.DbNullForNull(CmpPctChromeBranch)),
					new SqlCeParameter("@p32", Util.DbNullForNull(CmpPctChromeUsExt)),
					new SqlCeParameter("@p33", Util.DbNullForNull(CmpPctChromeDsExt)),
					new SqlCeParameter("@p34", Util.DbNullForNull(CmpPctChromeBrExt)),
					new SqlCeParameter("@p35", Util.DbNullForNull(CmpMisc1)),
					new SqlCeParameter("@p36", Util.DbNullForNull(CmpMisc2)),
					new SqlCeParameter("@p37", Util.DbNullForNull(CmpNote)),
					new SqlCeParameter("@p38", CmpHighRad),
					new SqlCeParameter("@p39", CmpHardToAccess),
					new SqlCeParameter("@p40", CmpHasDs),
					new SqlCeParameter("@p41", CmpHasBranch),
					new SqlCeParameter("@p42", CmpIsLclChg),
					new SqlCeParameter("@p43", CmpUsedInOutage),
					new SqlCeParameter("@p44", CmpIsActive)});

				cmd.CommandText =
					@"Update Components 
					set					
					CmpName = @p1,					
					CmpUntID = @p2,					
					CmpCmtID = @p3,					
					CmpLinID = @p4,					
					CmpSysID = @p5,					
					CmpCtpID = @p6,					
					CmpPslID = @p7,					
					CmpDrawing = @p8,					
					CmpUpMainOd = @p9,					
					CmpUpMainTnom = @p10,					
					CmpUpMainTscr = @p11,					
					CmpDnMainOd = @p12,					
					CmpDnMainTnom = @p13,					
					CmpDnMainTscr = @p14,					
					CmpBranchOd = @p15,					
					CmpBranchTnom = @p16,					
					CmpBranchTscr = @p17,					
					CmpUpExtOd = @p18,					
					CmpUpExtTnom = @p19,					
					CmpUpExtTscr = @p20,					
					CmpDnExtOd = @p21,					
					CmpDnExtTnom = @p22,					
					CmpDnExtTscr = @p23,					
					CmpBrExtOd = @p24,					
					CmpBrExtTnom = @p25,					
					CmpBrExtTscr = @p26,					
					CmpTimesInspected = @p27,					
					CmpAvgInspectionTime = @p28,					
					CmpAvgCrewDose= @p29,					
					CmpPctChromeMain = @p30,					
					CmpPctChromeBranch = @p31,					
					CmpPctChromeUsExt = @p32,					
					CmpPctChromeDsExt = @p33,					
					CmpPctChromeBrExt = @p34,					
					CmpMisc1 = @p35,					
					CmpMisc2 = @p36,					
					CmpNote = @p37,					
					CmpHighRad = @p38,					
					CmpHardToAccess = @p39,					
					CmpHasDs = @p40,					
					CmpHasBranch = @p41,					
					CmpIsLclChg = @p42,					
					CmpUsedInOutage = @p43,					
					CmpIsActive = @p44
					Where CmpDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update components row");
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
			if (!ComponentNameValid(ComponentName)) return false;
			if (!ComponentDrawingValid(ComponentDrawing)) return false;
			if (!ComponentMisc1Valid(ComponentMisc1)) return false;
			if (!ComponentMisc2Valid(ComponentMisc2)) return false;
			if (!ComponentNoteValid(ComponentNote)) return false;

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
			if (CmpDBid == null)
			{
				ComponentErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (CmpUsedInOutage)
			{
				ComponentErrMsg = "Unable to delete because this Component has been used in past outages.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (!CmpIsLclChg && !Globals.IsMasterDB)
			{
				ComponentErrMsg = "Unable to delete because this Component was not added during this outage.\r\nYou may wish to inactivate it instead.";
				return false;
			}

			if (HasInspectedComponents())
			{
				ComponentErrMsg = "Unable to delete because this Component is referenced by a Report.";
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
					@"Delete from Components 
					where CmpDBid = @p0";
				cmd.Parameters.Add("@p0", CmpDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					ComponentErrMsg = "Unable to delete.  Please try again later.";
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
				ComponentErrMsg = null;
				return false;
			}
		}

		private bool HasInspectedComponents()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select IscDBid from InspectedComponents
					where IscCmpID = @p0";
			cmd.Parameters.Add("@p0", CmpDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of components
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EComponentCollection ListByNameForOutage(Guid? OutageID,
			bool showinactive, bool addNoSelection)
		{
			EComponent component;
			EComponentCollection components = new EComponentCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				component = new EComponent();
				component.CmpName = "<No Selection>";
				components.Add(component);
			}

			if (OutageID == null) return components;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CmpDBid,
				CmpName,
				CmpUntID,
				CmpCmtID,
				CmpLinID,
				CmpSysID,
				CmpCtpID,
				CmpPslID,
				CmpDrawing,
				CmpUpMainOd,
				CmpUpMainTnom,
				CmpUpMainTscr,
				CmpDnMainOd,
				CmpDnMainTnom,
				CmpDnMainTscr,
				CmpBranchOd,
				CmpBranchTnom,
				CmpBranchTscr,
				CmpUpExtOd,
				CmpUpExtTnom,
				CmpUpExtTscr,
				CmpDnExtOd,
				CmpDnExtTnom,
				CmpDnExtTscr,
				CmpBrExtOd,
				CmpBrExtTnom,
				CmpBrExtTscr,
				CmpTimesInspected,
				CmpAvgInspectionTime,
				CmpAvgCrewDose,
				CmpPctChromeMain,
				CmpPctChromeBranch,
				CmpPctChromeUsExt,
				CmpPctChromeDsExt,
				CmpPctChromeBrExt,
				CmpMisc1,
				CmpMisc2,
				CmpNote,
				CmpHighRad,
				CmpHardToAccess,
				CmpHasDs,
				CmpHasBranch,
				CmpIsLclChg,
				CmpUsedInOutage,
				CmpIsActive
				from Components
				inner join InspectedComponents on CmpDBid = IscCmpID
				where IscOtgID = @p1";
			if (!showinactive)
				qry += " where CmpIsActive = 1";

			qry += "	order by CmpName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", OutageID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				component = new EComponent((Guid?)dr[0]);
				component.CmpName = (string)(dr[1]);
				component.CmpUntID = (Guid?)(dr[2]);
				component.CmpCmtID = (Guid?)Util.NullForDbNull(dr[3]);
				component.CmpLinID = (Guid?)Util.NullForDbNull(dr[4]);
				component.CmpSysID = (Guid?)Util.NullForDbNull(dr[5]);
				component.CmpCtpID = (Guid?)Util.NullForDbNull(dr[6]);
				component.CmpPslID = (Guid?)Util.NullForDbNull(dr[7]);
				component.CmpDrawing = (string)Util.NullForDbNull(dr[8]);
				component.CmpUpMainOd = (decimal?)Util.NullForDbNull(dr[9]);
				component.CmpUpMainTnom = (decimal?)Util.NullForDbNull(dr[10]);
				component.CmpUpMainTscr = (decimal?)Util.NullForDbNull(dr[11]);
				component.CmpDnMainOd = (decimal?)Util.NullForDbNull(dr[12]);
				component.CmpDnMainTnom = (decimal?)Util.NullForDbNull(dr[13]);
				component.CmpDnMainTscr = (decimal?)Util.NullForDbNull(dr[14]);
				component.CmpBranchOd = (decimal?)Util.NullForDbNull(dr[15]);
				component.CmpBranchTnom = (decimal?)Util.NullForDbNull(dr[16]);
				component.CmpBranchTscr = (decimal?)Util.NullForDbNull(dr[17]);
				component.CmpUpExtOd = (decimal?)Util.NullForDbNull(dr[18]);
				component.CmpUpExtTnom = (decimal?)Util.NullForDbNull(dr[19]);
				component.CmpUpExtTscr = (decimal?)Util.NullForDbNull(dr[20]);
				component.CmpDnExtOd = (decimal?)Util.NullForDbNull(dr[21]);
				component.CmpDnExtTnom = (decimal?)Util.NullForDbNull(dr[22]);
				component.CmpDnExtTscr = (decimal?)Util.NullForDbNull(dr[23]);
				component.CmpBrExtOd = (decimal?)Util.NullForDbNull(dr[24]);
				component.CmpBrExtTnom = (decimal?)Util.NullForDbNull(dr[25]);
				component.CmpBrExtTscr = (decimal?)Util.NullForDbNull(dr[26]);
				component.CmpTimesInspected = (int?)(dr[27]);
				component.CmpAvgInspectionTime = (float?)(dr[28]);
				component.CmpAvgCrewDose = (float?)(dr[29]);
				component.CmpPctChromeMain = (decimal?)Util.NullForDbNull(dr[30]);
				component.CmpPctChromeBranch = (decimal?)Util.NullForDbNull(dr[31]);
				component.CmpPctChromeUsExt = (decimal?)Util.NullForDbNull(dr[32]);
				component.CmpPctChromeDsExt = (decimal?)Util.NullForDbNull(dr[33]);
				component.CmpPctChromeBrExt = (decimal?)Util.NullForDbNull(dr[34]);
				component.CmpMisc1 = (string)Util.NullForDbNull(dr[35]);
				component.CmpMisc2 = (string)Util.NullForDbNull(dr[36]);
				component.CmpNote = (string)Util.NullForDbNull(dr[37]);
				component.CmpHighRad = (bool)(dr[38]);
				component.CmpHardToAccess = (bool)(dr[39]);
				component.CmpHasDs = (bool)(dr[40]);
				component.CmpHasBranch = (bool)(dr[41]);
				component.CmpIsLclChg = (bool)(dr[42]);
				component.CmpUsedInOutage = (bool)(dr[43]);
				component.CmpIsActive = (bool)(dr[44]);

				components.Add(component);	
			}
			// Finish up
			dr.Close();
			return components;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataViewForUnit(Guid? UnitID)
		{
			DataSet ds = new DataSet();
			DataView dv;
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			// Changing the booleans to 'Yes' and 'No' eliminates the silly checkboxes and
			// makes the column sortable.
			// You'll likely want to modify this query further, joining in other tables, etc.
			string qry =
				@"Select 
				CmpDBid as ID,
				CmpSysID as ComponentSystemID,
				CmpLinID as ComponentLineID,
				CmpName as ComponentName,
				CtpName as ComponentType,
				CmpUpMainOd as ComponentUpMainOd,
				CmpUpMainTnom as ComponentUpMainTnom,
				CmpUpMainTscr as ComponentUpMainTscr,
				PslSchedule as ComponentSchedule,
				CASE
					WHEN CmpHasDs = 0 THEN 'No'
					ELSE 'Yes'
				END as ComponentHasDs,
				CASE
					WHEN CmpHasBranch = 0 THEN 'No'
					ELSE 'Yes'
				END as ComponentHasBranch,
				CmtName as ComponentMaterial,
				CmpPctChromeMain as ComponentPctChromeMain,
				CmpPctChromeBranch as ComponentPctChromeBranch,
				LinName as ComponentLine,
				SysName as ComponentSystem,
				IscName as ComponentReportName,
				IscWorkOrder as ComponentWorkOrder,
				CmpDrawing as ComponentDrawing,
				CmpDnMainOd as ComponentDnMainOd,
				CmpDnMainTnom as ComponentDnMainTnom,
				CmpDnMainTscr as ComponentDnMainTscr,
				CmpBranchOd as ComponentBranchOd,
				CmpBranchTnom as ComponentBranchTnom,
				CmpBranchTscr as ComponentBranchTscr,
				CmpTimesInspected as ComponentTimesInspected,
				CmpAvgInspectionTime as ComponentAvgInspectionTime,
				CmpAvgCrewDose as ComponentAvgCrewDose,
				CASE
					WHEN CmpHighRad = 0 THEN 'No'
					ELSE 'Yes'
				END as ComponentHighRad,
				CASE
					WHEN CmpHardToAccess = 0 THEN 'No'
					ELSE 'Yes'
				END as ComponentHardToAccess,
				CmpMisc1 as ComponentMisc1,
				CmpMisc2 as ComponentMisc2,
				CASE
					WHEN CmpIsActive = 0 THEN 'No'
					ELSE 'Yes'
				END as ComponentIsActive,
				CASE 
					WHEN CmpCmtID is not NULL and
						CmpLinID is not NULL and 
						CmpCtpID is not NULL and 
						CmpPslID is not NULL and
						CmpUpMainOd is not NULL and
						CmpUpMainTnom is not NULL and
						CmpUpMainTscr is not NULL and
						(CmpHasDs = 0 or 
							(CmpDnMainOd is not NULL and 
							CmpDnMainTnom is not NULL and 
							CmpDnMainTscr is not NULL))
					THEN 'Yes'
					ELSE 'No' 
				END as ComponentDataComplete,
				CASE 
					WHEN IscCmpID is NULL THEN 'No'
					ELSE 'Yes'
				END as ComponentInOutage
				from Components
				left outer join ComponentMaterials on CmpCmtID = CmtDBid
				left outer join ComponentTypes on CmpCtpID = CtpDBid
				left outer join Lines on CmpLinID = LinDBid
				left outer join Systems on CmpSysID = SysDBid
				left outer join PipeScheduleLookup on CmpPslID = PslDBid
				left outer join InspectedComponents on CmpDBid = IscCmpID
				where CmpUntID = @p1";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", UnitID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static EComponent FindForComponentNameAndUnit(string ComponentName, Guid UnitID)
		{
			if (Util.IsNullOrEmpty(ComponentName)) return null;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.Parameters.Add(new SqlCeParameter("@p1", ComponentName));
			cmd.Parameters.Add(new SqlCeParameter("@p2", UnitID));
			cmd.CommandText = 
				@"Select CmpDBid from Components 
				where CmpName = @p1
				and CmpUntID = @p2";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) return new EComponent((Guid)val);
			else return null;
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
				cmd.CommandText = "Select CmpIsActive from Components where CmpName = @p1";
			}
			else
			{
				cmd.CommandText = "Select CmpIsActive from Components where CmpName = @p1 and CmpDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;
			return exists;
		}

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		public static bool NameExistsForUnit(string name, Guid? id, Guid unitId, 
			out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			cmd.Parameters.Add(new SqlCeParameter("@p2", unitId));
			if (id == null)
			{
				cmd.CommandText = 
					@"Select CmpIsActive from Components 
					where CmpName = @p1 and CmpUntID = @p2";
			}
			else
			{
				cmd.CommandText = 
					@"Select CmpIsActive from Components 
					where CmpName = @p1 and CmpUntID = @p2 and CmpDBid != @p0";
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

			if (ComponentName == null)
			{
				CmpNameErrMsg = "A unique Component Name is required";
				allFilled = false;
			}
			else
			{
				CmpNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Component Collection class
	//--------------------------------------
	public class EComponentCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EComponentCollection()
		{ }
		//the indexer of the collection
		public EComponent this[int index]
		{
			get
			{
				return (EComponent)this.List[index];
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
			foreach (EComponent component in InnerList)
			{
				if (component.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EComponent item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EComponent item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EComponent item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EComponent item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}

	public class EComponentComboItem
	{
		private Guid? CmpDBid;
		private string CmpName;

		public Guid? ID
		{
			get { return CmpDBid; }
			set { CmpDBid = value; }
		}

		public string ComponentName
		{
			get { return CmpName; }
			set { CmpName = value; }
		}

		public EComponentComboItem()	{}

		public EComponentComboItem(Guid? id)
		{
			this.CmpDBid = id;
		}

		public static EComponentComboItemCollection ListForUnitByName(Guid outageID,
			bool showInactive, bool showAssignedToReport, bool addNoSelection)
		{
			EOutage outage = new EOutage(outageID);
			Guid unitID = (Guid)outage.OutageUntID;
			EComponentComboItem component;
			EComponentComboItemCollection components = new EComponentComboItemCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				component = new EComponentComboItem();
				component.CmpName = "<No Selection>";
				components.Add(component);
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.Add("@p1", unitID);
			string qry = @"Select 
				CmpDBid,
				CmpName
				from Components
				left outer join InspectedComponents on CmpDBid = IscCmpID";

			qry += " where CmpUntID = @p1";

			if (!showInactive) qry += " and CmpIsActive = 1";

			// Exclude components that already have reports assigned for the current outage
			if (!showAssignedToReport)
			{
				qry += " and (IscDBid is NULL or IscOtgId <> @p2)";
				cmd.Parameters.Add("@p2", outageID);
			}
			qry += "	order by CmpName";
			
			cmd.CommandText = qry;
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				component = new EComponentComboItem((Guid?)dr[0]);
				component.CmpName = (string)(dr[1]);
				components.Add(component);
			}
			// Finish up
			dr.Close();
			return components;
		}

	}
	//--------------------------------------
	// Component Collection class
	//--------------------------------------
	public class EComponentComboItemCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EComponentComboItemCollection()
		{ }
		//the indexer of the collection
		public EComponentComboItem this[int index]
		{
			get
			{
				return (EComponentComboItem)this.List[index];
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
			foreach (EComponentComboItem component in InnerList)
			{
				if (component.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EComponentComboItem item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EComponentComboItem item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EComponentComboItem item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EComponentComboItem item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
