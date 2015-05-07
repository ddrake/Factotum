using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public enum ThicknessRange
	{
		Unknown,
		BelowTscreen,
		TscreenToTnom,
		TnomTo120pct,
		Above120pctTnom
	}
	public enum GridDividerTypeEnum
	{
		None,
		Weld,
		Transition 
	}
    public enum GridSectionEnum
    {
        None,
        UpstreamExtension,
        UpstreamMain,
        DownstreamMain,
        DownstreamExtension,
        BranchMain,
        BranchExtension
    }
	public class EGrid : IEntity
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
		private Guid? GrdDBid;
		private Guid? GrdIspID;
		private Guid? GrdParentID;
		private Guid? GrdGszID;
		private Guid? GrdRdlID;
		private string GrdAxialLocOverride;
		private short? GrdParentStartRow;
		private short? GrdParentStartCol;
		private bool GrdIsFullScan;
		private bool GrdIsColumnCCW;
		private bool GrdHideColumnLayoutGraphic;
		private decimal? GrdAxialDistance;
		private decimal? GrdRadialDistance;
		private short? GrdUpMainStartRow;
		private short? GrdUpMainEndRow;
		private short? GrdDnMainStartRow;
		private short? GrdDnMainEndRow;
		private short? GrdUpExtStartRow;
		private short? GrdUpExtEndRow;
		private short? GrdDnExtStartRow;
		private short? GrdDnExtEndRow;
		private short? GrdBranchStartRow;
		private short? GrdBranchEndRow;
		private short? GrdBranchExtStartRow;
		private short? GrdBranchExtEndRow;
		private byte? GrdUpMainPreDivider;
		private byte? GrdDnMainPreDivider;
		private byte? GrdUpExtPreDivider;
		private byte? GrdDnExtPreDivider;
		private byte? GrdBranchPreDivider;
		private byte? GrdBranchExtPreDivider;
		private byte? GrdPostDivider;

		// Textbox limits
		public static int GrdAxialLocOverrideCharLimit = 50;
		public static int GrdParentStartRowCharLimit = 6;
		public static int GrdParentStartColCharLimit = 6;
		public static int GrdAxialDistanceCharLimit = 8;
		public static int GrdRadialDistanceCharLimit = 8;
		public static int GrdUpMainStartRowCharLimit = 6;
		public static int GrdUpMainEndRowCharLimit = 6;
		public static int GrdDnMainStartRowCharLimit = 6;
		public static int GrdDnMainEndRowCharLimit = 6;
		public static int GrdUpExtStartRowCharLimit = 6;
		public static int GrdUpExtEndRowCharLimit = 6;
		public static int GrdDnExtStartRowCharLimit = 6;
		public static int GrdDnExtEndRowCharLimit = 6;
		public static int GrdBranchStartRowCharLimit = 6;
		public static int GrdBranchEndRowCharLimit = 6;
		public static int GrdBranchExtStartRowCharLimit = 6;
		public static int GrdBranchExtEndRowCharLimit = 6;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string GrdAxialLocOverrideErrMsg;
		private string GrdParentStartRowErrMsg;
		private string GrdParentStartColErrMsg;
		private string GrdAxialDistanceErrMsg;
		private string GrdRadialDistanceErrMsg;
		private string GrdUpMainStartRowErrMsg;
		private string GrdUpMainEndRowErrMsg;
		private string GrdDnMainStartRowErrMsg;
		private string GrdDnMainEndRowErrMsg;
		private string GrdUpExtStartRowErrMsg;
		private string GrdUpExtEndRowErrMsg;
		private string GrdDnExtStartRowErrMsg;
		private string GrdDnExtEndRowErrMsg;
		private string GrdBranchStartRowErrMsg;
		private string GrdBranchEndRowErrMsg;
		private string GrdBranchExtStartRowErrMsg;
		private string GrdBranchExtEndRowErrMsg;

		// Form level validation message
		private string GrdErrMsg;

		// Private variables set by calling UpdateSectionInfoVars()
		// and used by GetThicknessRange()
		private int upExtStart, upExtEnd, upMainStart, upMainEnd, dnMainStart, dnMainEnd,
			dnExtStart, dnExtEnd, branchStart, branchEnd, branchExtStart, branchExtEnd,
			gridStartRow;

		private bool hasUpExt, hasUpMain, hasDnMain, hasDnExt, hasBranch, hasBranchExt;
		private EComponent curComponent;

		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return GrdDBid; }
		}

		public Guid? GridIspID
		{
			get { return GrdIspID; }
			set { GrdIspID = value; }
		}

		public Guid? GridParentID
		{
			get { return GrdParentID; }
			set { GrdParentID = value; }
		}

		public Guid? GridGszID
		{
			get { return GrdGszID; }
			set { GrdGszID = value; }
		}

		public Guid? GridRdlID
		{
			get { return GrdRdlID; }
			set { GrdRdlID = value; }
		}

		public string GridAxialLocOverride
		{
			get { return GrdAxialLocOverride; }
			set { GrdAxialLocOverride = Util.NullifyEmpty(value); }
		}

		public short? GridParentStartRow
		{
			get { return GrdParentStartRow; }
			set { GrdParentStartRow = value; }
		}

		public short? GridParentStartCol
		{
			get { return GrdParentStartCol; }
			set { GrdParentStartCol = value; }
		}

		public bool GridIsFullScan
		{
			get { return GrdIsFullScan; }
			set { GrdIsFullScan = value; }
		}

		public bool GridIsColumnCCW
		{
			get { return GrdIsColumnCCW; }
			set { GrdIsColumnCCW = value; }
		}
		public bool GridHideColumnLayoutGraphic
		{
			get { return GrdHideColumnLayoutGraphic; }
			set { GrdHideColumnLayoutGraphic = value; }
		}
		
		public decimal? GridAxialDistance
		{
			get { return GrdAxialDistance; }
			set { GrdAxialDistance = value; }
		}

		public decimal? GridRadialDistance
		{
			get { return GrdRadialDistance; }
			set { GrdRadialDistance = value; }
		}

		public short? GridUpMainStartRow
		{
			get { return GrdUpMainStartRow; }
			set { GrdUpMainStartRow = value; }
		}

		public short? GridUpMainEndRow
		{
			get { return GrdUpMainEndRow; }
			set { GrdUpMainEndRow = value; }
		}

		public short? GridDnMainStartRow
		{
			get { return GrdDnMainStartRow; }
			set { GrdDnMainStartRow = value; }
		}

		public short? GridDnMainEndRow
		{
			get { return GrdDnMainEndRow; }
			set { GrdDnMainEndRow = value; }
		}

		public short? GridUpExtStartRow
		{
			get { return GrdUpExtStartRow; }
			set { GrdUpExtStartRow = value; }
		}

		public short? GridUpExtEndRow
		{
			get { return GrdUpExtEndRow; }
			set { GrdUpExtEndRow = value; }
		}

		public short? GridDnExtStartRow
		{
			get { return GrdDnExtStartRow; }
			set { GrdDnExtStartRow = value; }
		}

		public short? GridDnExtEndRow
		{
			get { return GrdDnExtEndRow; }
			set { GrdDnExtEndRow = value; }
		}

		public short? GridBranchStartRow
		{
			get { return GrdBranchStartRow; }
			set { GrdBranchStartRow = value; }
		}

		public short? GridBranchEndRow
		{
			get { return GrdBranchEndRow; }
			set { GrdBranchEndRow = value; }
		}

		public short? GridBranchExtStartRow
		{
			get { return GrdBranchExtStartRow; }
			set { GrdBranchExtStartRow = value; }
		}

		public short? GridBranchExtEndRow
		{
			get { return GrdBranchExtEndRow; }
			set { GrdBranchExtEndRow = value; }
		}

		public byte? GridUpMainPreDivider
		{
			get { return GrdUpMainPreDivider; }
			set { GrdUpMainPreDivider = value; }
		}

		public byte? GridDnMainPreDivider
		{
			get { return GrdDnMainPreDivider; }
			set { GrdDnMainPreDivider = value; }
		}

		public byte? GridUpExtPreDivider
		{
			get { return GrdUpExtPreDivider; }
			set { GrdUpExtPreDivider = value; }
		}

		public byte? GridDnExtPreDivider
		{
			get { return GrdDnExtPreDivider; }
			set { GrdDnExtPreDivider = value; }
		}

		public byte? GridBranchPreDivider
		{
			get { return GrdBranchPreDivider; }
			set { GrdBranchPreDivider = value; }
		}

		public byte? GridBranchExtPreDivider
		{
			get { return GrdBranchExtPreDivider; }
			set { GrdBranchExtPreDivider = value; }
		}

		public byte? GridPostDivider
		{
			get { return GrdPostDivider; }
			set { GrdPostDivider = value; }
		}

		// Array of CalBlock Materials for combo box binding
		static public GridDivider[] GetGridDividerArray()
		{
			return new GridDivider[] {
			new GridDivider((byte)GridDividerTypeEnum.None,"None"), 
			new GridDivider((byte)GridDividerTypeEnum.Weld,"Weld"), 
			new GridDivider((byte)GridDividerTypeEnum.Transition,"Internal Partition")};
		}

		public Guid? InspectedComponentID
		{
			get
			{
				if (GrdIspID == null) return null;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "Select IspIscID from Inspections where IspDBid = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", GrdIspID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return (Guid?)val;
				else return null;
			}
		}

		public Guid? ComponentID
		{
			get
			{
				if (GrdIspID == null) return null;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = 
					@"Select IscCmpID from InspectedComponents
					inner join Inspections on IscDBid = IspIscID
					where IspDBid = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", GrdIspID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return (Guid?)val;
				return null;
			}
		}

		// Check if the Upstream Extension section is defined
		public bool IsUsExtDefined
		{
			get { return (GrdUpExtStartRow != null && GrdUpExtEndRow != null); }
		}

		public bool IsUsMainDefined
		{
			get { return (GrdUpMainStartRow != null && GrdUpMainEndRow != null); }
		}

		public bool IsDsMainDefined
		{
			get { return (GrdDnMainStartRow != null && GrdDnMainEndRow != null); }
		}

		public bool IsDsExtDefined
		{
			get { return (GrdDnExtStartRow != null && GrdDnExtEndRow != null); }
		}

		public bool IsBranchDefined
		{
			get { return (GrdBranchStartRow != null && GrdBranchEndRow != null); }
		}

		public bool IsBranchExtDefined
		{
			get { return (GrdBranchExtStartRow != null && GrdBranchExtEndRow != null); }
		}

		// The points in the textfile including obstructions, but excluding empties.
		public int? GridTextFilePoints
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(MsrDBid) from Measurements 
					inner join GridCells on MsrDBid = GclMsrID
					where GclGrdID = @p0 and 
					(MsrThickness is not NULL or MsrIsObstruction = 1)";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		// Total number of measurements used by statistics.  
		// I.e. all textfile measurements (excluding empties and obstructions)
		public int? GridMeasurements
		{
			get
			{
				if (ID == null) return null;
				{
					SqlCeCommand cmd;
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
						@"select count(MsrDBid) from Measurements 
						inner join GridCells on MsrDBid = GclMsrID
						where GclGrdID = @p0 and MsrThickness is not NULL";
					cmd.Parameters.Add("@p0", ID);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					return Convert.ToInt32(cmd.ExecuteScalar());
				}
			}
		}

		// Total number of textfile measurements for which the 'IsObstruction' flag is set
		public int? GridObstructions
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(MsrDBid) from Measurements 
					inner join GridCells on MsrDBid = GclMsrID
					where GclGrdID = @p0 and MsrIsObstruction = 1";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}
		// The total number of cells in all the rows and columns spanned by the textfile
		// less any measurements or obstructions.
		// Note GridTextFilePoints includes obstructions, so we don't need to subtract them...
		public int? GridEmpties
		{
			get
			{
				if (ID == null) return null;
				if (GridTextFilePoints != null && GridTextFilePoints > 0)
					return (GridEndCol - GridStartCol + 1) * (GridEndRow - GridStartRow + 1)
						- GridTextFilePoints;
				else return null;
			}
		}

		// The starting row
		public int? GridStartRow
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select min(MsrRow) from GridCells 
					inner join Measurements on GclMsrID = MsrDBid
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return Convert.ToInt32(val);
				return null;
			}
		}

		// The ending row
		public int? GridEndRow
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select max(MsrRow) from GridCells 
					inner join Measurements on GclMsrID = MsrDBid
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return Convert.ToInt32(val);
				return null;
			}
		}

		// The starting column
		public int? GridStartCol
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select min(MsrCol) from GridCells 
					inner join Measurements on GclMsrID = MsrDBid
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return Convert.ToInt32(val);
				return null;
			}
		}

		// The ending column
		public int? GridEndCol
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select max(MsrCol) from GridCells 
					inner join Measurements on GclMsrID = MsrDBid
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return Convert.ToInt32(val);
				return null;
			}
		}

		// The minimum of all measurements (textfile or additional with IncludeInStats set)
		public float? GridMinWall
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select Min(MsrThickness) from Measurements 
					inner join GridCells on MsrDBid = GclMsrID
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test != null) return Convert.ToSingle(test);
				return null;
			}
		}
		// The maximum of all measurements (textfile or additional with IncludeInStats set)
		public float? GridMaxWall
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select Max(MsrThickness) from Measurements 
					inner join GridCells on MsrDBid = GclMsrID
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test != null) return Convert.ToSingle(test);
				return null;
			}
		}
		// The arithmetic mean of all measurements (textfile or additional with IncludeInStats set)
		public float? GridMeanWall
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select Avg(MsrThickness) from Measurements 
					inner join GridCells on MsrDBid = GclMsrID
					where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test != null) return Convert.ToSingle(test);
				return null;
			}
		}
		// The standard deviation of all measurements (textfile or additional with IncludeInStats set)
		public float? GridStdevWall
		{
			get
			{
				if (ID == null) return null;
				float? meanWall = GridMeanWall;
				if (meanWall == null) return null;

				double sum = 0d;
				object test;
				int? totMeas = GridMeasurements;
				if (totMeas == null || totMeas == 0) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
				@"select sum((@p1 - MsrThickness)*(@p2 - MsrThickness)) from Measurements 
				inner join GridCells on MsrDBid = GclMsrID
				where GclGrdID = @p0";

				cmd.Parameters.Add("@p0", ID);
				cmd.Parameters.Add("@p1", meanWall);
				cmd.Parameters.Add("@p2", meanWall);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test != null) sum = Convert.ToDouble(test);
				return Convert.ToSingle(Math.Sqrt(sum / (float)totMeas));
			}
		}
		public int? GridBelowTscr
		{
			get
			{
				if (ID == null) return null;
				Guid? cmpID = ComponentID;
				if (cmpID == null) return null;
				EComponent component = new EComponent(cmpID);
				int totalBelowTscr = 0;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
				@"select Count(MsrThickness) from Measurements
				inner join GridCells on MsrDBid = GclMsrID
				where GclGrdID = @p0 
				and MsrRow >= @p1 
				and MsrRow <= @p2 
				and MsrThickness < @p3";

				cmd.Parameters.Add("@p0", ID);
				cmd.Parameters.Add("@p1", 0);
				cmd.Parameters.Add("@p2", 0);
				cmd.Parameters.Add("@p3", 0m);

				if (IsUsExtDefined)
				{
					cmd.Parameters[1].Value = GrdUpExtStartRow;
					cmd.Parameters[2].Value = GrdUpExtEndRow;
					cmd.Parameters[3].Value = component.ComponentUpExtTscr;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					totalBelowTscr += Convert.ToInt32(cmd.ExecuteScalar());
				}

				if (IsUsMainDefined)
				{
					cmd.Parameters[1].Value = GrdUpMainStartRow;
					cmd.Parameters[2].Value = GrdUpMainEndRow;
					cmd.Parameters[3].Value = component.ComponentUpMainTscr;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					totalBelowTscr += Convert.ToInt32(cmd.ExecuteScalar());
				}

				if (IsDsMainDefined)
				{
					cmd.Parameters[1].Value = GrdDnMainStartRow;
					cmd.Parameters[2].Value = GrdDnMainEndRow;
					cmd.Parameters[3].Value = component.ComponentDnMainTscr;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					totalBelowTscr += Convert.ToInt32(cmd.ExecuteScalar());
				}

				if (IsDsExtDefined)
				{
					cmd.Parameters[1].Value = GrdDnExtStartRow;
					cmd.Parameters[2].Value = GrdDnExtEndRow;
					cmd.Parameters[3].Value = component.ComponentDnExtTscr;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					totalBelowTscr += Convert.ToInt32(cmd.ExecuteScalar());
				}

				if (IsBranchDefined)
				{
					cmd.Parameters[1].Value = GrdBranchStartRow;
					cmd.Parameters[2].Value = GrdBranchEndRow;
					cmd.Parameters[3].Value = component.ComponentBranchTscr;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					totalBelowTscr += Convert.ToInt32(cmd.ExecuteScalar());
				}

				if (IsBranchExtDefined)
				{
					cmd.Parameters[1].Value = GrdBranchExtStartRow;
					cmd.Parameters[2].Value = GrdBranchExtEndRow;
					cmd.Parameters[3].Value = component.ComponentBrExtTscr;
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					totalBelowTscr += Convert.ToInt32(cmd.ExecuteScalar());
				}
							
				return totalBelowTscr;
			}
		}

		public string GridRadialLocation
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select RdlName from RadialLocations
					inner join Grids on RdlDBid = GrdRdlID
					where GrdDBid = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test != null) return test.ToString();
				return null;
			}
		}
		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string GridAxialLocOverrideErrMsg
		{
			get { return GrdAxialLocOverrideErrMsg; }
		}

		public string GridParentStartRowErrMsg
		{
			get { return GrdParentStartRowErrMsg; }
		}

		public string GridParentStartColErrMsg
		{
			get { return GrdParentStartColErrMsg; }
		}

		public string GridAxialDistanceErrMsg
		{
			get { return GrdAxialDistanceErrMsg; }
		}

		public string GridRadialDistanceErrMsg
		{
			get { return GrdRadialDistanceErrMsg; }
		}

		public string GridUpMainStartRowErrMsg
		{
			get { return GrdUpMainStartRowErrMsg; }
		}

		public string GridUpMainEndRowErrMsg
		{
			get { return GrdUpMainEndRowErrMsg; }
		}

		public string GridDnMainStartRowErrMsg
		{
			get { return GrdDnMainStartRowErrMsg; }
		}

		public string GridDnMainEndRowErrMsg
		{
			get { return GrdDnMainEndRowErrMsg; }
		}

		public string GridUpExtStartRowErrMsg
		{
			get { return GrdUpExtStartRowErrMsg; }
		}

		public string GridUpExtEndRowErrMsg
		{
			get { return GrdUpExtEndRowErrMsg; }
		}

		public string GridDnExtStartRowErrMsg
		{
			get { return GrdDnExtStartRowErrMsg; }
		}

		public string GridDnExtEndRowErrMsg
		{
			get { return GrdDnExtEndRowErrMsg; }
		}

		public string GridBranchStartRowErrMsg
		{
			get { return GrdBranchStartRowErrMsg; }
		}

		public string GridBranchEndRowErrMsg
		{
			get { return GrdBranchEndRowErrMsg; }
		}

		public string GridBranchExtStartRowErrMsg
		{
			get { return GrdBranchExtStartRowErrMsg; }
		}

		public string GridBranchExtEndRowErrMsg
		{
			get { return GrdBranchExtEndRowErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string GridErrMsg
		{
			get { return GrdErrMsg; }
			set { GrdErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool GridAxialLocOverrideLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdAxialLocOverrideCharLimit)
			{
				GrdAxialLocOverrideErrMsg = string.Format("GridAxialLocOverrides cannot exceed {0} characters", GrdAxialLocOverrideCharLimit);
				return false;
			}
			else
			{
				GrdAxialLocOverrideErrMsg = null;
				return true;
			}
		}

		public bool GridParentStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdParentStartRowCharLimit)
			{
				GrdParentStartRowErrMsg = string.Format("GridParentStartRows cannot exceed {0} characters", GrdParentStartRowCharLimit);
				return false;
			}
			else
			{
				GrdParentStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridParentStartColLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdParentStartColCharLimit)
			{
				GrdParentStartColErrMsg = string.Format("GridParentStartCols cannot exceed {0} characters", GrdParentStartColCharLimit);
				return false;
			}
			else
			{
				GrdParentStartColErrMsg = null;
				return true;
			}
		}

		public bool GridAxialDistanceLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdAxialDistanceCharLimit)
			{
				GrdAxialDistanceErrMsg = string.Format("GridAxialDistances cannot exceed {0} characters", GrdAxialDistanceCharLimit);
				return false;
			}
			else
			{
				GrdAxialDistanceErrMsg = null;
				return true;
			}
		}

		public bool GridRadialDistanceLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdRadialDistanceCharLimit)
			{
				GrdRadialDistanceErrMsg = string.Format("GridRadialDistances cannot exceed {0} characters", GrdRadialDistanceCharLimit);
				return false;
			}
			else
			{
				GrdRadialDistanceErrMsg = null;
				return true;
			}
		}

		public bool GridUpMainStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdUpMainStartRowCharLimit)
			{
				GrdUpMainStartRowErrMsg = string.Format("GridUpMainStartRows cannot exceed {0} characters", GrdUpMainStartRowCharLimit);
				return false;
			}
			else
			{
				GrdUpMainStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridUpMainEndRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdUpMainEndRowCharLimit)
			{
				GrdUpMainEndRowErrMsg = string.Format("GridUpMainEndRows cannot exceed {0} characters", GrdUpMainEndRowCharLimit);
				return false;
			}
			else
			{
				GrdUpMainEndRowErrMsg = null;
				return true;
			}
		}

		public bool GridDnMainStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdDnMainStartRowCharLimit)
			{
				GrdDnMainStartRowErrMsg = string.Format("GridDnMainStartRows cannot exceed {0} characters", GrdDnMainStartRowCharLimit);
				return false;
			}
			else
			{
				GrdDnMainStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridDnMainEndRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdDnMainEndRowCharLimit)
			{
				GrdDnMainEndRowErrMsg = string.Format("GridDnMainEndRows cannot exceed {0} characters", GrdDnMainEndRowCharLimit);
				return false;
			}
			else
			{
				GrdDnMainEndRowErrMsg = null;
				return true;
			}
		}

		public bool GridUpExtStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdUpExtStartRowCharLimit)
			{
				GrdUpExtStartRowErrMsg = string.Format("GridUpExtStartRows cannot exceed {0} characters", GrdUpExtStartRowCharLimit);
				return false;
			}
			else
			{
				GrdUpExtStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridUpExtEndRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdUpExtEndRowCharLimit)
			{
				GrdUpExtEndRowErrMsg = string.Format("GridUpExtEndRows cannot exceed {0} characters", GrdUpExtEndRowCharLimit);
				return false;
			}
			else
			{
				GrdUpExtEndRowErrMsg = null;
				return true;
			}
		}

		public bool GridDnExtStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdDnExtStartRowCharLimit)
			{
				GrdDnExtStartRowErrMsg = string.Format("GridDnExtStartRows cannot exceed {0} characters", GrdDnExtStartRowCharLimit);
				return false;
			}
			else
			{
				GrdDnExtStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridDnExtEndRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdDnExtEndRowCharLimit)
			{
				GrdDnExtEndRowErrMsg = string.Format("GridDnExtEndRows cannot exceed {0} characters", GrdDnExtEndRowCharLimit);
				return false;
			}
			else
			{
				GrdDnExtEndRowErrMsg = null;
				return true;
			}
		}

		public bool GridBranchStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdBranchStartRowCharLimit)
			{
				GrdBranchStartRowErrMsg = string.Format("GridBranchStartRows cannot exceed {0} characters", GrdBranchStartRowCharLimit);
				return false;
			}
			else
			{
				GrdBranchStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridBranchEndRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdBranchEndRowCharLimit)
			{
				GrdBranchEndRowErrMsg = string.Format("GridBranchEndRows cannot exceed {0} characters", GrdBranchEndRowCharLimit);
				return false;
			}
			else
			{
				GrdBranchEndRowErrMsg = null;
				return true;
			}
		}

		public bool GridBranchExtStartRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdBranchExtStartRowCharLimit)
			{
				GrdBranchExtStartRowErrMsg = string.Format("GridBranchExtStartRows cannot exceed {0} characters", GrdBranchExtStartRowCharLimit);
				return false;
			}
			else
			{
				GrdBranchExtStartRowErrMsg = null;
				return true;
			}
		}

		public bool GridBranchExtEndRowLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > GrdBranchExtEndRowCharLimit)
			{
				GrdBranchExtEndRowErrMsg = string.Format("GridBranchExtEndRows cannot exceed {0} characters", GrdBranchExtEndRowCharLimit);
				return false;
			}
			else
			{
				GrdBranchExtEndRowErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool GridAxialLocOverrideValid(string value)
		{
			if (!GridAxialLocOverrideLengthOk(value)) return false;

			GrdAxialLocOverrideErrMsg = null;
			return true;
		}

		public bool GridParentStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdParentStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdParentStartRowErrMsg = null;
				return true;
			}
			GrdParentStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridParentStartColValid(string value)
		{
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdParentStartColErrMsg = null;
				return true;
			}
			GrdParentStartColErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridAxialDistanceValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdAxialDistanceErrMsg = null;
				return true;
			}
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0)
			{
				GrdAxialDistanceErrMsg = null;
				return true;
			}
			GrdAxialDistanceErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridRadialDistanceValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdRadialDistanceErrMsg = null;
				return true;
			}
			decimal result;
			if (decimal.TryParse(value, out result) && result > 0)
			{
				GrdRadialDistanceErrMsg = null;
				return true;
			}
			GrdRadialDistanceErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridUpMainStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdUpMainStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdUpMainStartRowErrMsg = null;
				return true;
			}
			GrdUpMainStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridUpMainEndRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdUpMainEndRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdUpMainEndRowErrMsg = null;
				return true;
			}
			GrdUpMainEndRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridDnMainStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdDnMainStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdDnMainStartRowErrMsg = null;
				return true;
			}
			GrdDnMainStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridDnMainEndRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdDnMainEndRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdDnMainEndRowErrMsg = null;
				return true;
			}
			GrdDnMainEndRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridUpExtStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdUpExtStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdUpExtStartRowErrMsg = null;
				return true;
			}
			GrdUpExtStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridUpExtEndRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdUpExtEndRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdUpExtEndRowErrMsg = null;
				return true;
			}
			GrdUpExtEndRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridDnExtStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdDnExtStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdDnExtStartRowErrMsg = null;
				return true;
			}
			GrdDnExtStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridDnExtEndRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdDnExtEndRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdDnExtEndRowErrMsg = null;
				return true;
			}
			GrdDnExtEndRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridBranchStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdBranchStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdBranchStartRowErrMsg = null;
				return true;
			}
			GrdBranchStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridBranchEndRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdBranchEndRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdBranchEndRowErrMsg = null;
				return true;
			}
			GrdBranchEndRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridBranchExtStartRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdBranchExtStartRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdBranchExtStartRowErrMsg = null;
				return true;
			}
			GrdBranchExtStartRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool GridBranchExtEndRowValid(string value)
		{
			if (Util.IsNullOrEmpty(value))
			{
				GrdBranchExtEndRowErrMsg = null;
				return true;
			}
			short result;
			if (short.TryParse(value, out result) && result > 0)
			{
				GrdBranchExtEndRowErrMsg = null;
				return true;
			}
			GrdBranchExtEndRowErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EGrid()
		{
			this.GrdIsFullScan = false;
			this.GrdIsColumnCCW = false;
			this.GrdHideColumnLayoutGraphic = false;
			this.GrdUpMainPreDivider = 0;
			this.GrdDnMainPreDivider = 0;
			this.GrdUpExtPreDivider = 0;
			this.GrdDnExtPreDivider = 0;
			this.GrdBranchPreDivider = 0;
			this.GrdBranchExtPreDivider = 0;
			this.GrdPostDivider = 0;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EGrid(Guid? id) : this()
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
				GrdDBid,
				GrdIspID,
				GrdParentID,
				GrdGszID,
				GrdRdlID,
				GrdAxialLocOverride,
				GrdParentStartRow,
				GrdParentStartCol,
				GrdIsFullScan,
				GrdIsColumnCCW,
				GrdHideColumnLayoutGraphic,
				GrdAxialDistance,
				GrdRadialDistance,
				GrdUpMainStartRow,
				GrdUpMainEndRow,
				GrdDnMainStartRow,
				GrdDnMainEndRow,
				GrdUpExtStartRow,
				GrdUpExtEndRow,
				GrdDnExtStartRow,
				GrdDnExtEndRow,
				GrdBranchStartRow,
				GrdBranchEndRow,
				GrdBranchExtStartRow,
				GrdBranchExtEndRow,
				GrdUpMainPreDivider,
				GrdDnMainPreDivider,
				GrdUpExtPreDivider,
				GrdDnExtPreDivider,
				GrdBranchPreDivider,
				GrdBranchExtPreDivider,
				GrdPostDivider
				from Grids
				where GrdDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				GrdDBid = (Guid?)dr[0];
				GrdIspID = (Guid?)dr[1];
				GrdParentID = (Guid?)Util.NullForDbNull(dr[2]);
				GrdGszID = (Guid?)Util.NullForDbNull(dr[3]);
				GrdRdlID = (Guid?)Util.NullForDbNull(dr[4]);
				GrdAxialLocOverride = (string)Util.NullForDbNull(dr[5]);
				GrdParentStartRow = (short?)Util.NullForDbNull(dr[6]);
				GrdParentStartCol = (short?)Util.NullForDbNull(dr[7]);
				GrdIsFullScan = (bool)dr[8];
				GrdIsColumnCCW = (bool)dr[9];
				GrdHideColumnLayoutGraphic = (bool)dr[10];
				GrdAxialDistance = (decimal?)Util.NullForDbNull(dr[11]);
				GrdRadialDistance = (decimal?)Util.NullForDbNull(dr[12]);
				GrdUpMainStartRow = (short?)Util.NullForDbNull(dr[13]);
				GrdUpMainEndRow = (short?)Util.NullForDbNull(dr[14]);
				GrdDnMainStartRow = (short?)Util.NullForDbNull(dr[15]);
				GrdDnMainEndRow = (short?)Util.NullForDbNull(dr[16]);
				GrdUpExtStartRow = (short?)Util.NullForDbNull(dr[17]);
				GrdUpExtEndRow = (short?)Util.NullForDbNull(dr[18]);
				GrdDnExtStartRow = (short?)Util.NullForDbNull(dr[19]);
				GrdDnExtEndRow = (short?)Util.NullForDbNull(dr[20]);
				GrdBranchStartRow = (short?)Util.NullForDbNull(dr[21]);
				GrdBranchEndRow = (short?)Util.NullForDbNull(dr[22]);
				GrdBranchExtStartRow = (short?)Util.NullForDbNull(dr[23]);
				GrdBranchExtEndRow = (short?)Util.NullForDbNull(dr[24]);
				GrdUpMainPreDivider = (byte?)dr[25];
				GrdDnMainPreDivider = (byte?)dr[26];
				GrdUpExtPreDivider = (byte?)dr[27];
				GrdDnExtPreDivider = (byte?)dr[28];
				GrdBranchPreDivider = (byte?)dr[29];
				GrdBranchExtPreDivider = (byte?)dr[30];
				GrdPostDivider = (byte?)dr[31];
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
				GrdDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GrdDBid),
					new SqlCeParameter("@p1", GrdIspID),
					new SqlCeParameter("@p2", Util.DbNullForNull(GrdParentID)),
					new SqlCeParameter("@p3", Util.DbNullForNull(GrdGszID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(GrdRdlID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(GrdAxialLocOverride)),
					new SqlCeParameter("@p6", Util.DbNullForNull(GrdParentStartRow)),
					new SqlCeParameter("@p7", Util.DbNullForNull(GrdParentStartCol)),
					new SqlCeParameter("@p8", GrdIsFullScan),
					new SqlCeParameter("@p9", GrdIsColumnCCW),
					new SqlCeParameter("@p10", GrdHideColumnLayoutGraphic),
					new SqlCeParameter("@p11", Util.DbNullForNull(GrdAxialDistance)),
					new SqlCeParameter("@p12", Util.DbNullForNull(GrdRadialDistance)),
					new SqlCeParameter("@p13", Util.DbNullForNull(GrdUpMainStartRow)),
					new SqlCeParameter("@p14", Util.DbNullForNull(GrdUpMainEndRow)),
					new SqlCeParameter("@p15", Util.DbNullForNull(GrdDnMainStartRow)),
					new SqlCeParameter("@p16", Util.DbNullForNull(GrdDnMainEndRow)),
					new SqlCeParameter("@p17", Util.DbNullForNull(GrdUpExtStartRow)),
					new SqlCeParameter("@p18", Util.DbNullForNull(GrdUpExtEndRow)),
					new SqlCeParameter("@p19", Util.DbNullForNull(GrdDnExtStartRow)),
					new SqlCeParameter("@p20", Util.DbNullForNull(GrdDnExtEndRow)),
					new SqlCeParameter("@p21", Util.DbNullForNull(GrdBranchStartRow)),
					new SqlCeParameter("@p22", Util.DbNullForNull(GrdBranchEndRow)),
					new SqlCeParameter("@p23", Util.DbNullForNull(GrdBranchExtStartRow)),
					new SqlCeParameter("@p24", Util.DbNullForNull(GrdBranchExtEndRow)),
					new SqlCeParameter("@p25", GrdUpMainPreDivider),
					new SqlCeParameter("@p26", GrdDnMainPreDivider),
					new SqlCeParameter("@p27", GrdUpExtPreDivider),
					new SqlCeParameter("@p28", GrdDnExtPreDivider),
					new SqlCeParameter("@p29", GrdBranchPreDivider),
					new SqlCeParameter("@p30", GrdBranchExtPreDivider),
					new SqlCeParameter("@p31", GrdPostDivider)
					});
				cmd.CommandText = @"Insert Into Grids (
					GrdDBid,
					GrdIspID,
					GrdParentID,
					GrdGszID,
					GrdRdlID,
					GrdAxialLocOverride,
					GrdParentStartRow,
					GrdParentStartCol,
					GrdIsFullScan,
					GrdIsColumnCCW,
					GrdHideColumnLayoutGraphic,
					GrdAxialDistance,
					GrdRadialDistance,
					GrdUpMainStartRow,
					GrdUpMainEndRow,
					GrdDnMainStartRow,
					GrdDnMainEndRow,
					GrdUpExtStartRow,
					GrdUpExtEndRow,
					GrdDnExtStartRow,
					GrdDnExtEndRow,
					GrdBranchStartRow,
					GrdBranchEndRow,
					GrdBranchExtStartRow,
					GrdBranchExtEndRow,
					GrdUpMainPreDivider,
					GrdDnMainPreDivider,
					GrdUpExtPreDivider,
					GrdDnExtPreDivider,
					GrdBranchPreDivider,
					GrdBranchExtPreDivider,
					GrdPostDivider
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20,@p21,@p22,@p23,@p24,@p25,@p26,@p27,@p28,@p29,@p30,@p31)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Grids row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", GrdDBid),
					new SqlCeParameter("@p1", GrdIspID),
					new SqlCeParameter("@p2", Util.DbNullForNull(GrdParentID)),
					new SqlCeParameter("@p3", Util.DbNullForNull(GrdGszID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(GrdRdlID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(GrdAxialLocOverride)),
					new SqlCeParameter("@p6", Util.DbNullForNull(GrdParentStartRow)),
					new SqlCeParameter("@p7", Util.DbNullForNull(GrdParentStartCol)),
					new SqlCeParameter("@p8", GrdIsFullScan),
					new SqlCeParameter("@p9", GrdIsColumnCCW),
					new SqlCeParameter("@p10", GrdHideColumnLayoutGraphic),	
					new SqlCeParameter("@p11", Util.DbNullForNull(GrdAxialDistance)),
					new SqlCeParameter("@p12", Util.DbNullForNull(GrdRadialDistance)),
					new SqlCeParameter("@p13", Util.DbNullForNull(GrdUpMainStartRow)),
					new SqlCeParameter("@p14", Util.DbNullForNull(GrdUpMainEndRow)),
					new SqlCeParameter("@p15", Util.DbNullForNull(GrdDnMainStartRow)),
					new SqlCeParameter("@p16", Util.DbNullForNull(GrdDnMainEndRow)),
					new SqlCeParameter("@p17", Util.DbNullForNull(GrdUpExtStartRow)),
					new SqlCeParameter("@p18", Util.DbNullForNull(GrdUpExtEndRow)),
					new SqlCeParameter("@p19", Util.DbNullForNull(GrdDnExtStartRow)),
					new SqlCeParameter("@p20", Util.DbNullForNull(GrdDnExtEndRow)),
					new SqlCeParameter("@p21", Util.DbNullForNull(GrdBranchStartRow)),
					new SqlCeParameter("@p22", Util.DbNullForNull(GrdBranchEndRow)),
					new SqlCeParameter("@p23", Util.DbNullForNull(GrdBranchExtStartRow)),
					new SqlCeParameter("@p24", Util.DbNullForNull(GrdBranchExtEndRow)),
					new SqlCeParameter("@p25", GrdUpMainPreDivider),
					new SqlCeParameter("@p26", GrdDnMainPreDivider),
					new SqlCeParameter("@p27", GrdUpExtPreDivider),
					new SqlCeParameter("@p28", GrdDnExtPreDivider),
					new SqlCeParameter("@p29", GrdBranchPreDivider),
					new SqlCeParameter("@p30", GrdBranchExtPreDivider),
					new SqlCeParameter("@p31", GrdPostDivider)});

				cmd.CommandText =
					@"Update Grids 
					set					
					GrdIspID = @p1,					
					GrdParentID = @p2,					
					GrdGszID = @p3,					
					GrdRdlID = @p4,					
					GrdAxialLocOverride = @p5,					
					GrdParentStartRow = @p6,					
					GrdParentStartCol = @p7,					
					GrdIsFullScan = @p8,					
					GrdIsColumnCCW = @p9,	
					GrdHideColumnLayoutGraphic = @p10,	
					GrdAxialDistance = @p11,					
					GrdRadialDistance = @p12,					
					GrdUpMainStartRow = @p13,					
					GrdUpMainEndRow = @p14,					
					GrdDnMainStartRow = @p15,					
					GrdDnMainEndRow = @p16,					
					GrdUpExtStartRow = @p17,					
					GrdUpExtEndRow = @p18,					
					GrdDnExtStartRow = @p19,					
					GrdDnExtEndRow = @p20,					
					GrdBranchStartRow = @p21,					
					GrdBranchEndRow = @p22,					
					GrdBranchExtStartRow = @p23,					
					GrdBranchExtEndRow = @p24,					
					GrdUpMainPreDivider = @p25,					
					GrdDnMainPreDivider = @p26,					
					GrdUpExtPreDivider = @p27,					
					GrdDnExtPreDivider = @p28,					
					GrdBranchPreDivider = @p29,					
					GrdBranchExtPreDivider = @p30,					
					GrdPostDivider = @p31
					Where GrdDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update grids row");
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
			if (!GridAxialLocOverrideValid(GridAxialLocOverride)) return false;

			// Make sure the sequence is ok.  Note: this doesn't prevent gaps.  Should it???
			if (!SequenceOk(GrdUpExtStartRow, GrdUpExtEndRow) ||
				!SequenceOk(GrdUpExtEndRow, GrdUpMainStartRow) ||
				!SequenceOk(GrdUpMainStartRow, GrdUpMainEndRow) ||
				!SequenceOk(GrdUpMainEndRow, GrdDnMainStartRow) ||
				!SequenceOk(GrdDnMainStartRow, GrdDnExtStartRow) ||
				!SequenceOk(GrdDnExtStartRow, GrdDnExtEndRow) ||
				!SequenceOk(GrdBranchStartRow, GrdBranchEndRow) ||
                // if the flow is into the branch, the extension rows will be lower than the branch rows. 7/2013
				//!SequenceOk(GrdBranchEndRow, GrdBranchExtStartRow) || 
				!SequenceOk(GrdBranchExtStartRow, GrdBranchExtEndRow))
			{
				MessageBox.Show("Some Section Rows are Out of Sequence", "Factotum");
				return false;
			}

			if ((GrdUpExtPreDivider > 0 && (GrdUpExtStartRow == null || GrdUpExtEndRow == null)) ||
				(GrdUpMainPreDivider > 0 && (GrdUpMainStartRow == null || GrdUpMainEndRow == null)) ||
				(GrdDnMainPreDivider > 0 && (GrdDnMainStartRow == null || GrdDnMainEndRow == null)) ||
				(GrdDnExtPreDivider > 0 && (GrdDnExtStartRow == null || GrdDnExtEndRow == null)) ||
				(GrdBranchPreDivider > 0 && (GrdBranchStartRow == null || GrdBranchEndRow == null)) ||
				(GrdBranchExtPreDivider > 0 && (GrdBranchExtStartRow == null || GrdBranchExtEndRow == null)))
			{
				MessageBox.Show("Unless start and end rows are specified for a section, the grid divider before the section should be 'None'", "Factotum");
				return false;
			}


			// Check form to make sure all required fields have been filled in
			if (!RequiredFieldsFilled()) return false;

			// Check for incorrect field interactions...

			return true;
		}

		public bool SequenceOk(short? first, short? second)
		{
			if (first == null || second == null) return true;
			if (first > second) return false;
			return true;
		}

		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{


			// If the current object doesn't reference a database record, there's nothing to do.
			if (GrdDBid == null)
			{
				GridErrMsg = "Unable to delete. Record not found.";
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
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"Select GrdDBid from Grids 
					where GrdParentID = @p0";
				cmd.Parameters.Add("@p0", GrdDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object result = Util.NullForDbNull(cmd.ExecuteScalar());
				if (result != null)
				{
					MessageBox.Show("The current Grid could not be deleted\r\nbecause it is referenced by another grid in this report.", "Factotum");
					return false;
				}

				// We need to manually delete from Grid Cells.  Cascading deletes would be great,
				// But the database complained about cyclical references (without justification).
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from GridCells 
					where GclGrdID = @p0";
				cmd.Parameters.Add("@p0", GrdDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				// Now delete the current grid.  
				// Note: Any Dsets that have been assigned to the current
				// grid will have their DstGrdID set to null.
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from Grids 
					where GrdDBid = @p0";
				cmd.Parameters.Add("@p0", GrdDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					GridErrMsg = "Unable to delete.  Please try again later.";
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
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of grids
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EGridCollection List(bool showinactive)
		{
			EGrid grid;
			EGridCollection grids = new EGridCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				GrdDBid,
				GrdIspID,
				GrdParentID,
				GrdGszID,
				GrdRdlID,
				GrdAxialLocOverride,
				GrdParentStartRow,
				GrdParentStartCol,
				GrdIsFullScan,
				GrdIsColumnCCW,
				GrdHideColumnLayoutGraphic,
				GrdAxialDistance,
				GrdRadialDistance,
				GrdUpMainStartRow,
				GrdUpMainEndRow,
				GrdDnMainStartRow,
				GrdDnMainEndRow,
				GrdUpExtStartRow,
				GrdUpExtEndRow,
				GrdDnExtStartRow,
				GrdDnExtEndRow,
				GrdBranchStartRow,
				GrdBranchEndRow,
				GrdBranchExtStartRow,
				GrdBranchExtEndRow,
				GrdUpMainPreDivider,
				GrdDnMainPreDivider,
				GrdUpExtPreDivider,
				GrdDnExtPreDivider,
				GrdBranchPreDivider,
				GrdBranchExtPreDivider,
				GrdPostDivider
				from Grids";
			if (!showinactive)
				qry += " where GrdIsActive = 1";

			qry += "	order by GrdDBid";
			cmd.CommandText = qry;

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				grid = new EGrid((Guid?)dr[0]);
				grid.GrdIspID = (Guid?)(dr[1]);
				grid.GrdParentID = (Guid?)Util.NullForDbNull(dr[2]);
				grid.GrdGszID = (Guid?)Util.NullForDbNull(dr[3]);
				grid.GrdRdlID = (Guid?)Util.NullForDbNull(dr[4]);
				grid.GrdAxialLocOverride = (string)Util.NullForDbNull(dr[5]);
				grid.GrdParentStartRow = (short?)Util.NullForDbNull(dr[6]);
				grid.GrdParentStartCol = (short?)Util.NullForDbNull(dr[7]);
				grid.GrdIsFullScan = (bool)(dr[8]);
				grid.GrdIsColumnCCW = (bool)(dr[9]);
				grid.GrdHideColumnLayoutGraphic = (bool)(dr[10]);
				grid.GrdAxialDistance = (decimal?)Util.NullForDbNull(dr[11]);
				grid.GrdRadialDistance = (decimal?)Util.NullForDbNull(dr[12]);
				grid.GrdUpMainStartRow = (short?)Util.NullForDbNull(dr[13]);
				grid.GrdUpMainEndRow = (short?)Util.NullForDbNull(dr[14]);
				grid.GrdDnMainStartRow = (short?)Util.NullForDbNull(dr[15]);
				grid.GrdDnMainEndRow = (short?)Util.NullForDbNull(dr[16]);
				grid.GrdUpExtStartRow = (short?)Util.NullForDbNull(dr[17]);
				grid.GrdUpExtEndRow = (short?)Util.NullForDbNull(dr[18]);
				grid.GrdDnExtStartRow = (short?)Util.NullForDbNull(dr[19]);
				grid.GrdDnExtEndRow = (short?)Util.NullForDbNull(dr[20]);
				grid.GrdBranchStartRow = (short?)Util.NullForDbNull(dr[21]);
				grid.GrdBranchEndRow = (short?)Util.NullForDbNull(dr[22]);
				grid.GrdBranchExtStartRow = (short?)Util.NullForDbNull(dr[23]);
				grid.GrdBranchExtEndRow = (short?)Util.NullForDbNull(dr[24]);
				grid.GrdUpMainPreDivider = (byte?)(dr[25]);
				grid.GrdDnMainPreDivider = (byte?)(dr[26]);
				grid.GrdUpExtPreDivider = (byte?)(dr[27]);
				grid.GrdDnExtPreDivider = (byte?)(dr[28]);
				grid.GrdBranchPreDivider = (byte?)(dr[29]);
				grid.GrdBranchExtPreDivider = (byte?)(dr[30]);
				grid.GrdPostDivider = (byte?)(dr[31]);

				grids.Add(grid);	
			}
			// Finish up
			dr.Close();
			return grids;
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
					GrdDBid as ID,
					GrdIspID as GridIspID,
					GrdParentID as GridParentID,
					GrdGszID as GridGszID,
					GrdRdlID as GridRdlID,
					GrdAxialLocOverride as GridAxialLocOverride,
					GrdParentStartRow as GridParentStartRow,
					GrdParentStartCol as GridParentStartCol,
					CASE
						WHEN GrdIsFullScan = 0 THEN 'No'
						ELSE 'Yes'
					END as GridIsFullScan,
					CASE
						WHEN GrdIsColumnCCW = 0 THEN 'No'
						ELSE 'Yes'
					END as GridIsColumnCCW,
					GrdAxialDistance as GridAxialDistance,
					GrdRadialDistance as GridRadialDistance,
					GrdUpMainStartRow as GridUpMainStartRow,
					GrdUpMainEndRow as GridUpMainEndRow,
					GrdDnMainStartRow as GridDnMainStartRow,
					GrdDnMainEndRow as GridDnMainEndRow,
					GrdUpExtStartRow as GridUpExtStartRow,
					GrdUpExtEndRow as GridUpExtEndRow,
					GrdDnExtStartRow as GridDnExtStartRow,
					GrdDnExtEndRow as GridDnExtEndRow,
					GrdBranchStartRow as GridBranchStartRow,
					GrdBranchEndRow as GridBranchEndRow,
					GrdBranchExtStartRow as GridBranchExtStartRow,
					GrdBranchExtEndRow as GridBranchExtEndRow,
					GrdUpMainPreDivider as GridUpMainPreDivider,
					GrdDnMainPreDivider as GridDnMainPreDivider,
					GrdUpExtPreDivider as GridUpExtPreDivider,
					GrdDnExtPreDivider as GridDnExtPreDivider,
					GrdBranchPreDivider as GridBranchPreDivider,
					GrdBranchExtPreDivider as GridBranchExtPreDivider,
					GrdPostDivider as GridPostDivider
					from Grids";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		// Update the table that is used to eliminate duplicate values for a given grid cell
		// by using the higher priority Dset.
		public void UpdateGridCellsDbTable()
		{
			SqlCeCommand cmd;

			// Delete the temp table if it exists.  It shouldn't under normal circumstances.
			Globals.DeleteTempTableIfExists("tmpCells");

			// Create the temp table, which will aggregate by priority
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"create table tmpCells 
				(row smallint NOT NULL, 
				col smallint NOT NULL, 
				priority smallint NOT NULL)";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// I had some logic here that gave higher priority to non-empties, etc,
			// but that just muddies the water.  Let the priority be strictly determined by the
			// stacking order, or the user will get confused (and so will I).
			// Fill the temp table.
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"insert into tmpCells (row, col, priority)
				select 				
				MsrRow as row, 
				MsrCol as col, 
				Min(DstGridPriority) as priority
				from Dsets 
				inner join Surveys on DstDBid = SvyDstID
				inner join Measurements on SvyDBid = MsrSvyID
				where DstGrdID = @p0
				group by MsrRow, MsrCol";
			cmd.Parameters.Add("@p0", GrdDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
			
			// Now delete from the GridCells table for the current grid
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Delete From GridCells 
				where GclGrdID = @p0";
			cmd.Parameters.Add("@p0", GrdDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Now insert into the GridCells table for the current grid, joining with the temp table
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"insert into GridCells (GclGrdID, GclMsrID)
				select 				
				DstGrdID, 
				MsrDBid
				from Dsets 
				inner join Surveys on DstDBid = SvyDstID
				inner join Measurements on SvyDBid = MsrSvyID
				inner join tmpCells on (
				MsrRow = row and 
				MsrCol = col and 
				DstGridPriority = priority)
				where DstGrdID = @p0";
			cmd.Parameters.Add("@p0", GrdDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
			
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = "drop table tmpCells";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}

        public EGridCollection GetAllChildGrids()
        {
            EGridCollection children = new EGridCollection();
            EGrid child;
		    SqlCeCommand cmd;
            SqlCeDataReader dr;
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = "Select GrdDBid from Grids where GrdParentID = @p0" ;
			cmd.Parameters.Add(new SqlCeParameter("@p0", (Guid)ID));

			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
            while (dr.Read())
            {
                child = new EGrid((Guid?)dr[0]);
                children.Add(child);
            }
            // Finish up
            dr.Close();
            return children;
        }

        // BE SURE TO CALL UpdateSectionInfoVars() BEFORE USING THIS FUNCTION
        public GridSectionEnum GetGridSectionForRow(int row)
        {
            if (hasUpExt && row >= upExtStart && row <= upExtEnd)
                return GridSectionEnum.UpstreamExtension;
            if (hasUpMain && row >= upMainStart && row <= upMainEnd)
                return GridSectionEnum.UpstreamMain;
            if (hasDnExt && row >= dnExtStart && row <= dnExtEnd)
                return GridSectionEnum.DownstreamExtension;
            if (hasDnMain && row >= dnMainStart && row <= dnMainEnd)
                return GridSectionEnum.DownstreamMain;
            if (hasBranch && row >= branchStart && row <= branchEnd)
                return GridSectionEnum.BranchMain;
            if (hasBranchExt && row >= branchExtStart && row <= branchExtEnd)
                return GridSectionEnum.BranchExtension;

            return GridSectionEnum.None;
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

		// BE SURE TO CALL UpdateSectionInfoVars() BEFORE USING THIS FUNCTION TO GET A THICKNESS RANGE

		// Given a DataGridView row number and a value, return the appropriate thickness range.
		// We compare the row to the global section information vars to find out which section
		// of the component it's in, then compare the thickness value provided with the Tnom and Tscr
		// for that section of the component.
		public ThicknessRange GetThicknessRange(int dgvRow, decimal value)
		{
			decimal tScr, tNom;
			int row = dgvRow + gridStartRow;
			if (hasUpExt && row >= upExtStart && row <= upExtEnd)
			{
				tScr = (decimal)curComponent.ComponentUpExtTscr;
				tNom = (decimal)curComponent.ComponentUpExtTnom;

				if (value < tScr) return ThicknessRange.BelowTscreen;
				if (value >= tScr && value < tNom) return ThicknessRange.TscreenToTnom;
				if (value >= tNom && value < tNom * 1.2m) return ThicknessRange.TnomTo120pct;
				else return ThicknessRange.Above120pctTnom;
			}
			if (hasUpMain && row >= upMainStart && row <= upMainEnd)
			{
				tScr = (decimal)curComponent.ComponentUpMainTscr;
				tNom = (decimal)curComponent.ComponentUpMainTnom;

				if (value < tScr) return ThicknessRange.BelowTscreen;
				if (value >= tScr && value < tNom) return ThicknessRange.TscreenToTnom;
				if (value >= tNom && value < tNom * 1.2m) return ThicknessRange.TnomTo120pct;
				else return ThicknessRange.Above120pctTnom;
			}
			if (hasDnExt && row >= dnExtStart && row <= dnExtEnd)
			{
				tScr = (decimal)curComponent.ComponentDnExtTscr;
				tNom = (decimal)curComponent.ComponentDnExtTnom;

				if (value < tScr) return ThicknessRange.BelowTscreen;
				if (value >= tScr && value < tNom) return ThicknessRange.TscreenToTnom;
				if (value >= tNom && value < tNom * 1.2m) return ThicknessRange.TnomTo120pct;
				else return ThicknessRange.Above120pctTnom;
			}
			if (hasDnMain && row >= dnMainStart && row <= dnMainEnd)
			{
				tScr = (decimal)curComponent.ComponentDnMainTscr;
				tNom = (decimal)curComponent.ComponentDnMainTnom;

				if (value < tScr) return ThicknessRange.BelowTscreen;
				if (value >= tScr && value < tNom) return ThicknessRange.TscreenToTnom;
				if (value >= tNom && value < tNom * 1.2m) return ThicknessRange.TnomTo120pct;
				else return ThicknessRange.Above120pctTnom;
			}
			if (hasBranch && row >= branchStart && row <= branchEnd)
			{
				tScr = (decimal)curComponent.ComponentBranchTscr;
				tNom = (decimal)curComponent.ComponentBranchTnom;

				if (value < tScr) return ThicknessRange.BelowTscreen;
				if (value >= tScr && value < tNom) return ThicknessRange.TscreenToTnom;
				if (value >= tNom && value < tNom * 1.2m) return ThicknessRange.TnomTo120pct;
				else return ThicknessRange.Above120pctTnom;
			}
			if (hasBranchExt && row >= branchExtStart && row <= branchExtEnd)
			{
				tScr = (decimal)curComponent.ComponentBrExtTscr;
				tNom = (decimal)curComponent.ComponentBrExtTnom;

				if (value < tScr) return ThicknessRange.BelowTscreen;
				if (value >= tScr && value < tNom) return ThicknessRange.TscreenToTnom;
				if (value >= tNom && value < tNom * 1.2m) return ThicknessRange.TnomTo120pct;
				else return ThicknessRange.Above120pctTnom;
			}
			return ThicknessRange.Unknown;
		}

		// Returns the grid rows which follow gridDivisions and the type of division.
		public bool GetGridDivisionInfo(out int[] divRows, out int[] divTypes)
		{
			List<int> rowList = new List<int>(7);
			List<int> typeList = new List<int>(7);

			if (GridUpExtPreDivider > (byte)GridDividerTypeEnum.None && GrdUpExtStartRow != null)
			{
				rowList.Add((int)GrdUpExtStartRow);
				typeList.Add((int)GridUpExtPreDivider);
			}
			if (GridUpMainPreDivider > (byte)GridDividerTypeEnum.None && GrdUpMainStartRow != null)
			{
				rowList.Add((int)GrdUpMainStartRow);
				typeList.Add((int)GridUpMainPreDivider);
			}
			if (GridDnMainPreDivider > (byte)GridDividerTypeEnum.None && GrdDnMainStartRow != null)
			{
				rowList.Add((int)GrdDnMainStartRow);
				typeList.Add((int)GridDnMainPreDivider);
			}
			if (GridDnExtPreDivider > (byte)GridDividerTypeEnum.None && GrdDnExtStartRow != null)
			{
				rowList.Add((int)GrdDnExtStartRow);
				typeList.Add((int)GridDnExtPreDivider);
			}
			if (GridBranchPreDivider > (byte)GridDividerTypeEnum.None && GrdBranchStartRow != null)
			{
				rowList.Add((int)GrdBranchStartRow);
				typeList.Add((int)GridBranchPreDivider);
			}
			if (GridBranchExtPreDivider > (byte)GridDividerTypeEnum.None && GrdBranchExtStartRow != null)
			{
				rowList.Add((int)GrdBranchExtStartRow);
				typeList.Add((int)GridBranchExtPreDivider);
			}
			if (GridPostDivider > (byte)GridDividerTypeEnum.None)
			{
				rowList.Add((int)GridEndRow + 1);
				typeList.Add((int)GridPostDivider);
			}
			divRows = rowList.ToArray();
			divTypes = typeList.ToArray();

			Array.Sort(divRows, divTypes);
			return true;
		}

		// Update the global Section information variables based on the component's thickness data
		// and the current grid partition settings.
		// This is for efficiency in displaying the grid. We don't want to have to query the db each time
		// the dgvGrid_CellPainting event is called.

		public void UpdateSectionInfoVars()
		{
			int? temp;

			if (curComponent == null)	curComponent = new EComponent(ComponentID);
			
			hasUpExt = curComponent.UpExtThicknessesDefined && IsUsExtDefined;
			hasUpMain = curComponent.UpMainThicknessesDefined && IsUsMainDefined;
			hasDnMain = curComponent.DnMainThicknessesDefined && IsDsMainDefined;
			hasDnExt = curComponent.DnExtThicknessesDefined && IsDsExtDefined;
			hasBranch = curComponent.BranchThicknessesDefined && IsBranchDefined;
			hasBranchExt = curComponent.BranchExtThicknessesDefined && IsBranchExtDefined;
			upExtStart = hasUpExt ? (int)GridUpExtStartRow : 0;
			upExtEnd = hasUpExt ? (int)GridUpExtEndRow : 0;
			upMainStart = hasUpMain ? (int)GridUpMainStartRow : 0;
			upMainEnd = hasUpMain ? (int)GridUpMainEndRow : 0;
			dnMainStart = hasDnMain ? (int)GridDnMainStartRow : 0;
			dnMainEnd = hasDnMain ? (int)GridDnMainEndRow : 0;
			dnExtStart = hasDnExt ? (int)GridDnExtStartRow : 0;
			dnExtEnd = hasDnExt ? (int)GridDnExtEndRow : 0;
			branchStart = hasBranch ? (int)GridBranchStartRow : 0;
			branchEnd = hasBranch ? (int)GridBranchEndRow : 0;
			branchExtStart = hasBranchExt ? (int)GridBranchExtStartRow : 0;
			branchExtEnd = hasBranchExt ? (int)GridBranchExtEndRow : 0;
			temp = this.GridStartRow;
			gridStartRow = temp==null ? 0 : (int)temp;
		}
	}

	//--------------------------------------
	// Grid Collection class
	//--------------------------------------
	public class EGridCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EGridCollection()
		{ }
		//the indexer of the collection
		public EGrid this[int index]
		{
			get
			{
				return (EGrid)this.List[index];
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
			foreach (EGrid grid in InnerList)
			{
				if (grid.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EGrid item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EGrid item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EGrid item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EGrid item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}

	public class EGridComboItem
	{
		private Guid? GrdDBid;
		private string GrdName;

		public Guid? ID
		{
			get { return GrdDBid; }
			set { GrdDBid = value; }
		}

		public string GridName
		{
			get { return GrdName; }
			set { GrdName = value; }
		}

		public EGridComboItem()	{}

		public EGridComboItem(Guid? id)
		{
			this.GrdDBid = id;
		}

		// List possible parent grids for the specified grid.
		// We want to exclude the current grid and any children of the current grid.
		public static EGridComboItemCollection ListEligibleParentGridsForReportByInspectionName(Guid reportID,
			Guid? currentGridIdToExclude, bool addNoSelection)
		{
			EGridComboItem Grid;
			EGridComboItemCollection Grids = new EGridComboItemCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				Grid = new EGridComboItem();
				Grid.GrdName = "<No Selection>";
				Grids.Add(Grid);
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 
				GrdDBid as ID,
				IspName as GridName
				from Grids
				inner join Inspections on GrdIspID = IspDBid
				where IspIscID = @p1";
			if (currentGridIdToExclude != null)
			{
				qry += 
					@" and GrdDBid != @p0 
					and (GrdParentID is NULL or GrdParentID != @p2)";
				cmd.Parameters.Add("@p0", currentGridIdToExclude);
				cmd.Parameters.Add("@p2", currentGridIdToExclude);
			}
		
			qry += "	order by IspName";
			
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", reportID);
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				Grid = new EGridComboItem((Guid?)dr[0]);
				Grid.GrdName = (string)(dr[1]);
				Grids.Add(Grid);
			}
			// Finish up
			dr.Close();
			return Grids;
		}

	}
	//--------------------------------------
	// Grid Collection class
	//--------------------------------------
	public class EGridComboItemCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EGridComboItemCollection()
		{ }
		//the indexer of the collection
		public EGridComboItem this[int index]
		{
			get
			{
				return (EGridComboItem)this.List[index];
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
			foreach (EGridComboItem Grid in InnerList)
			{
				if (Grid.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EGridComboItem item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EGridComboItem item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EGridComboItem item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EGridComboItem item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}

	public class GridDivider
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
		public GridDivider(byte ID, string Name)
		{
			this.id = ID;
			this.name = Name;
		}
	}



}
