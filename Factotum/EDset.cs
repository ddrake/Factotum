using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;
using System.IO;

namespace Factotum{

	public enum MeterTextFileType
	{
		Unknown,
		Panametrics,
		Krautkramer
	}
	public class EDset : IEntity
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
		private Guid? DstDBid;
		private string DstName;
		private Guid? DstIspID;
		private Guid? DstGrdID;
		private Guid? DstInsID;
		private Guid? DstDcrID;
		private Guid? DstMtrID;
		private Guid? DstCbkID;
		private Guid? DstThmID;
		private short? DstGridPriority;
		private string DstTextFileName;
		private byte? DstTextFileFormat;
		private short? DstCompTemp;
		private short? DstCalBlockTemp;
		private decimal? DstRange;
		private decimal? DstThin;
		private decimal? DstThick;
		private decimal? DstVelocity;
		private decimal? DstGainDb;
		private short? DstCrewDose;

		// Textbox limits
		private const int DstNameCharLimit = 20;
		private const int DstTextFileNameCharLimit = 512;
		// positive short values must be < 32000, let's make them < 10000 by limiting to 4 chars
		private const int DstCompTempCharLimit = 4; 
		private const int DstCalBlockTempCharLimit = 4;
		private const int DstCrewDoseCharLimit = 4;
		// for positive decimals, we don't need to allow a space for a minus sign, just a decimal point
		// for decimals that can be negative, we need to allow an extra space but add some special 
		// logic to make sure they're aren't too many decimal places or the db will throw an exception
		private const int DstRangeCharLimit = 6;
		private const int DstThinCharLimit = 6;
		private const int DstThickCharLimit = 6;
		private const int DstVelocityCharLimit = 6; // eg 9.9999
		private const int DstGainDbCharLimit = 4; // eg 99.9
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string DstNameErrMsg;
		private string DstTextFileNameErrMsg;
		private string DstCompTempErrMsg;
		private string DstCalBlockTempErrMsg;
		private string DstRangeErrMsg;
		private string DstThinErrMsg;
		private string DstThickErrMsg;
		private string DstVelocityErrMsg;
		private string DstGainDbErrMsg;
		private string DstCrewDoseErrMsg;

		// Form level validation message
		private string DstErrMsg;

		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return DstDBid; }
		}

		public string DsetName
		{
			get { return DstName; }
			set { DstName = Util.NullifyEmpty(value); }
		}

		public Guid? DsetIspID
		{
			get { return DstIspID; }
			set 
			{
				if (DstIspID == null && value != null)
				{
					// We're setting the parent for a new record, so get the next grid priority
					// in the sequence.
					DstGridPriority = getNewGridPriority((Guid)value);
					DstName = GetUniqueDsetName((Guid)value);
				}
				DstIspID = value; 
			}
		}

		public Guid? DsetGrdID
		{
			get { return DstGrdID; }
			set { DstGrdID = value; }
		}

		public Guid? DsetInsID
		{
			get { return DstInsID; }
			set { DstInsID = value; }
		}

		public Guid? DsetDcrID
		{
			get { return DstDcrID; }
			set { DstDcrID = value; }
		}

		public Guid? DsetMtrID
		{
			get { return DstMtrID; }
			set { DstMtrID = value; }
		}

		public Guid? DsetCbkID
		{
			get { return DstCbkID; }
			set { DstCbkID = value; }
		}

		public Guid? DsetThmID
		{
			get { return DstThmID; }
			set { DstThmID = value; }
		}

		public short? DsetGridPriority
		{
			get { return DstGridPriority; }
			set { DstGridPriority = value; }
		}

		public string DsetTextFileName
		{
			get { return DstTextFileName; }
			set { DstTextFileName = Util.NullifyEmpty(value); }
		}

		public byte? DsetTextFileFormat
		{
			get { return DstTextFileFormat; }
			set { DstTextFileFormat = value; }
		}

		public short? DsetCompTemp
		{
			get { return DstCompTemp; }
			set { DstCompTemp = value; }
		}

		public short? DsetCalBlockTemp
		{
			get { return DstCalBlockTemp; }
			set { DstCalBlockTemp = value; }
		}

		public decimal? DsetRange
		{
			get { return DstRange; }
			set { DstRange = value; }
		}

		public decimal? DsetThin
		{
			get { return DstThin; }
			set { DstThin = value; }
		}

		public decimal? DsetThick
		{
			get { return DstThick; }
			set { DstThick = value; }
		}

		public decimal? DsetVelocity
		{
			get { return DstVelocity; }
			set { DstVelocity = value; }
		}

		public decimal? DsetGainDb
		{
			get { return DstGainDb; }
			set { DstGainDb = value; }
		}


		public short? DsetCrewDose
		{
			get { return DstCrewDose; }
			set { DstCrewDose = value; }
		}

		// The points in the textfile including obstructions, but excluding empties.
		public int? DsetTextFilePoints
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(MsrDBid) from Measurements 
							inner join Surveys on MsrSvyID = SvyDBid
							where SvyDstID = @p0 and 
							(MsrThickness is not NULL or MsrIsObstruction = 1)";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		// Total number of measurements used by statistics.  
		// I.e. all textfile measurements (excluding empties and obstructions)
		// plus any additional measurements whose 'IncludeInStats' flag is set.
		public int? DsetMeasurements
		{
			get
			{
				if (ID == null) return null;
				{
					int count = 0;
					SqlCeCommand cmd;
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
						@"select count(MsrDBid) from Measurements 
						inner join Surveys on MsrSvyID = SvyDBid
						where SvyDstID = @p0 and MsrThickness is not NULL
						union select count (AdmDBid) from AdditionalMeasurements
						where AdmDstID = @p1 and AdmIncludeInStats = 1 and AdmThickness is not null";

					cmd.Parameters.Add("@p0", ID);
					cmd.Parameters.Add("@p1", ID);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					SqlCeDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
						count += (int)dr[0];
					dr.Close();
					return count;
				}
			}
		}

		// Total number of textfile measurements for which the 'IsObstruction' flag is set
		public int? DsetObstructions
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(MsrDBid) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0 and MsrIsObstruction = 1";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}
		// The total number of cells in all the rows and columns spanned by the textfile
		// less any measurements or obstructions.
		// Note DsetTextFilePoints includes obstructions, so we don't need to subtract them...
		public int? DsetEmpties
		{
			get
			{
				if (ID == null) return null;
				if (DsetTextFilePoints > 0)
					return (DsetEndCol - DsetStartCol + 1) * (DsetEndRow - DsetStartRow + 1) 
						- DsetTextFilePoints;
				else return 0;
			}
		}

		// The total number of additional measurements
		public int? DsetAdditionalMeasurements
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(AdmDBid) from AdditionalMeasurements 
					where AdmDstID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		// The total number of additional measurements which are to be included in statistics.
		public int? DsetAdditionalMeasurementsWithStats
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(AdmDBid) from AdditionalMeasurements 
					where AdmDstID = @p0 and AdmIncludeInStats = 1 and AdmThickness is not null";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		// The starting row
		public int? DsetStartRow
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select min(MsrRow) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test == null) return null;
				return Convert.ToInt32(test);
			}
		}

		// The ending row
		public int? DsetEndRow
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select max(MsrRow) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test == null) return null;
				return Convert.ToInt32(test);
			}
		}

		// The starting column
		public int? DsetStartCol
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select min(MsrCol) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test == null) return null;
				return Convert.ToInt32(test);
			}
		}
		
		// The ending column
		public int? DsetEndCol
		{
			get
			{
				if (ID == null) return null;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select max(MsrCol) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				test = Util.NullForDbNull(cmd.ExecuteScalar());
				if (test == null) return null;
				return Convert.ToInt32(test);
			}
		}

		// The minimum of all measurements (textfile or additional with IncludeInStats set)
		public float? DsetMinWall
		{
			get
			{
				if (ID == null) return null;
				float minWall = float.MaxValue;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select min(MsrThickness) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0
					union select min(AdmThickness) from AdditionalMeasurements
					where AdmDstID = @p1 and AdmIncludeInStats = 1 and AdmThickness is not null";

				cmd.Parameters.Add("@p0", ID);
				cmd.Parameters.Add("@p1", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				SqlCeDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					test = Util.NullForDbNull(dr[0]);
					if (test != null)
						minWall = Convert.ToSingle(test) < minWall ? Convert.ToSingle(test) : minWall;
				}
				dr.Close();
				if (minWall == float.MaxValue) return null;
				else return minWall;

			}
		}
		// The maximum of all measurements (textfile or additional with IncludeInStats set)
		public float? DsetMaxWall
		{
			get
			{
				if (ID == null) return null;
				float maxWall = 0;
				object test;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select max(MsrThickness) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0
					union select max(AdmThickness) from AdditionalMeasurements
					where AdmDstID = @p1 and AdmIncludeInStats = 1 and AdmThickness is not null";

				cmd.Parameters.Add("@p0", ID);
				cmd.Parameters.Add("@p1", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				SqlCeDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					test = Util.NullForDbNull(dr[0]);
					if (test != null)
						maxWall = Convert.ToSingle(test) > maxWall ? Convert.ToSingle(test) : maxWall;
				}
				dr.Close();
				if (maxWall == 0) return null;
				else return maxWall;

			}
		}
		// The arithmetic mean of all measurements (textfile or additional with IncludeInStats set)
		public float? DsetMeanWall
		{
			get
			{
				if (ID == null) return null;
				double sum = 0d;
				object test;
				int? totMeas = DsetMeasurements;
				int? addlMeas = DsetAdditionalMeasurementsWithStats;
				if (totMeas == null || totMeas == 0) return null;
				SqlCeCommand cmd;
				if (addlMeas != null && addlMeas > 0) // We have some additional measurements so add up the thicknesses
				{
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
						@"select sum(AdmThickness) from AdditionalMeasurements 
					where AdmDstID = @p0 and AdmIncludeInStats = 1 and AdmThickness is not null";

					cmd.Parameters.Add("@p0", ID);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					test = Util.NullForDbNull(cmd.ExecuteScalar());
					if (test != null) sum += Convert.ToDouble(test);
				}
				if (totMeas > addlMeas) // we have some grid measurements so add up the thicknesses
				{
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
						@"select sum(MsrThickness) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0";

					cmd.Parameters.Add("@p0", ID);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					test = Util.NullForDbNull(cmd.ExecuteScalar());
					if (test != null) sum += Convert.ToDouble(test);
				}
				return (float)(sum / totMeas);

			}
		}
		// The standard deviation of all measurements (textfile or additional with IncludeInStats set)
		public float? DsetStdevWall
		{
			get
			{
				if (ID == null) return null;				
				float? meanWall = DsetMeanWall;
				if (meanWall == null) return null;

				double sum = 0d;
				object test;
				int? totMeas = DsetMeasurements;
				int? addlMeas = DsetAdditionalMeasurementsWithStats;
				if (totMeas == null || totMeas == 0) return null;
				if (totMeas == 1) return 0;

				SqlCeCommand cmd;
				if (addlMeas != null && addlMeas > 0) // We have some additional measurements so add up the thicknes error squares
				{
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
					@"select sum((@p1 - AdmThickness)*(@p2 - AdmThickness)) from AdditionalMeasurements 
					where AdmDstID = @p0 and AdmIncludeInStats = 1 and AdmThickness is not null";

					cmd.Parameters.Add("@p0", ID);
					cmd.Parameters.Add("@p1", meanWall);
					cmd.Parameters.Add("@p2", meanWall);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					test = Util.NullForDbNull(cmd.ExecuteScalar());
					if (test != null) sum += Convert.ToDouble(test);
				}
				if (totMeas > addlMeas) // we have some grid measurements so add up the thickness error squares
				{
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
					@"select sum((@p1 - MsrThickness)*(@p2 - MsrThickness)) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p0";

					cmd.Parameters.Add("@p0", ID);
					cmd.Parameters.Add("@p1", meanWall);
					cmd.Parameters.Add("@p2", meanWall);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					test = Util.NullForDbNull(cmd.ExecuteScalar());
					if (test != null) sum += Convert.ToDouble(test);
				}
				return Convert.ToSingle(Math.Sqrt(sum / (float)(totMeas-1)));
			}
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string DsetNameErrMsg
		{
			get { return DstNameErrMsg; }
		}

		public string DsetTextFileNameErrMsg
		{
			get { return DstTextFileNameErrMsg; }
		}

		public string DsetCompTempErrMsg
		{
			get { return DstCompTempErrMsg; }
		}

		public string DsetCalBlockTempErrMsg
		{
			get { return DstCalBlockTempErrMsg; }
		}

		public string DsetRangeErrMsg
		{
			get { return DstRangeErrMsg; }
		}

		public string DsetThinErrMsg
		{
			get { return DstThinErrMsg; }
		}

		public string DsetThickErrMsg
		{
			get { return DstThickErrMsg; }
		}

		public string DsetVelocityErrMsg
		{
			get { return DstVelocityErrMsg; }
		}

		public string DsetGainDbErrMsg
		{
			get { return DstGainDbErrMsg; }
		}

		public string DsetCrewDoseErrMsg
		{
			get { return DstCrewDoseErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string DsetErrMsg
		{
			get { return DstErrMsg; }
			set { DstErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool DsetNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstNameCharLimit)
			{
				DstNameErrMsg = string.Format("Dataset Names cannot exceed {0} characters", DstNameCharLimit);
				return false;
			}
			else
			{
				DstNameErrMsg = null;
				return true;
			}
		}

		public bool DsetTextFileNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstTextFileNameCharLimit)
			{
				DstTextFileNameErrMsg = string.Format("Dataset Textfile paths cannot exceed {0} characters", DstTextFileNameCharLimit);
				return false;
			}
			else
			{
				DstTextFileNameErrMsg = null;
				return true;
			}
		}


		public bool DsetCompTempLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstCompTempCharLimit)
			{
				DstCompTempErrMsg = string.Format("Component Temperature cannot exceed {0} characters", DstCompTempCharLimit);
				return false;
			}
			else
			{
				DstCompTempErrMsg = null;
				return true;
			}
		}

		public bool DsetCalBlockTempLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstCalBlockTempCharLimit)
			{
				DstCalBlockTempErrMsg = string.Format("Cal Block Temperature cannot exceed {0} characters", DstCalBlockTempCharLimit);
				return false;
			}
			else
			{
				DstCalBlockTempErrMsg = null;
				return true;
			}
		}

		public bool DsetRangeLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstRangeCharLimit)
			{
				DstRangeErrMsg = string.Format("Range cannot exceed {0} characters", DstRangeCharLimit);
				return false;
			}
			else
			{
				DstRangeErrMsg = null;
				return true;
			}
		}

		public bool DsetThinLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstThinCharLimit)
			{
				DstThinErrMsg = string.Format("Min wall cannot exceed {0} characters", DstThinCharLimit);
				return false;
			}
			else
			{
				DstThinErrMsg = null;
				return true;
			}
		}

		public bool DsetThickLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstThickCharLimit)
			{
				DstThickErrMsg = string.Format("Max Wall cannot exceed {0} characters", DstThickCharLimit);
				return false;
			}
			else
			{
				DstThickErrMsg = null;
				return true;
			}
		}

		public bool DsetVelocityLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstVelocityCharLimit)
			{
				DstVelocityErrMsg = string.Format("Velocity cannot exceed {0} characters", DstVelocityCharLimit);
				return false;
			}
			else
			{
				DstVelocityErrMsg = null;
				return true;
			}
		}

		public bool DsetGainDbLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstGainDbCharLimit)
			{
				DstGainDbErrMsg = string.Format("Gain cannot exceed {0} characters", DstGainDbCharLimit);
				return false;
			}
			else
			{
				DstGainDbErrMsg = null;
				return true;
			}
		}

		public bool DsetCrewDoseLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > DstCrewDoseCharLimit)
			{
				DstCrewDoseErrMsg = string.Format("Crew Dose cannot exceed {0} characters", DstCrewDoseCharLimit);
				return false;
			}
			else
			{
				DstCrewDoseErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool DsetNameValid(string name)
		{
			if (!DsetNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, (Guid?)DstDBid))
			{
				DstNameErrMsg = "That Dataset Name is already in use.";
				return false;
			}
			DstNameErrMsg = null;
			return true;
		}

		public bool DsetTextFileNameValid(string value)
		{
			if (!DsetTextFileNameLengthOk(value)) return false;

			DstTextFileNameErrMsg = null;
			return true;
		}

		public bool DsetCompTempValid(string value)
		{
			short result;
			if (Util.IsNullOrEmpty(value))
			{
				DstCompTempErrMsg = null;
				return true;
			}
			if (short.TryParse(value, out result) && result > 0 && result < 10000)
			{
				DstCompTempErrMsg = null;
				return true;
			}
			DstCompTempErrMsg = string.Format("Please enter a positive whole number less than 10000");
			return false;
		}

		public bool DsetCalBlockTempValid(string value)
		{
			short result;
			if (Util.IsNullOrEmpty(value))
			{
				DstCalBlockTempErrMsg = null;
				return true;
			}
			if (short.TryParse(value, out result) && result > 0 && result < 10000)
			{
				DstCalBlockTempErrMsg = null;
				return true;
			}
			DstCalBlockTempErrMsg = string.Format("Please enter a positive whole number less than 10000");
			return false;
		}

		public bool DsetRangeValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				DstRangeErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				DstRangeErrMsg = null;
				return true;
			}
			DstRangeErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool DsetThinValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				DstThinErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				DstThinErrMsg = null;
				return true;
			}
			DstThinErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool DsetThickValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				DstThickErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				DstThickErrMsg = null;
				return true;
			}
			DstThickErrMsg = string.Format("Please enter a positive number less than 100");
			return false;
		}

		public bool DsetVelocityValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				DstVelocityErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 1)
			{
				DstVelocityErrMsg = null;
				return true;
			}
			DstVelocityErrMsg = string.Format("Please enter a number between 0 and 0.9999");
			return false;
		}

		public bool DsetGainDbValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				DstGainDbErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0 && result < 100)
			{
				DstGainDbErrMsg = null;
				return true;
			}
			DstGainDbErrMsg = string.Format("Please enter a number between 0.0 and 99.9");
			return false;
		}

		public bool DsetCrewDoseValid(string value)
		{
			short result;
			if (Util.IsNullOrEmpty(value))
			{
				DstCrewDoseErrMsg = null;
				return true;
			}
			if (short.TryParse(value, out result) && result >= 0)
			{
				DstCrewDoseErrMsg = null;
				return true;
			}
			DstCrewDoseErrMsg = string.Format("Please enter a non-negative whole number less than 10000");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EDset()
		{
			this.DstGridPriority = 0;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EDset(Guid? id) : this()
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
			if (id == null)
			{
				return;
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
				@"Select 
				DstDBid,
				DstName,
				DstIspID,
				DstGrdID,
				DstInsID,
				DstDcrID,
				DstMtrID,
				DstCbkID,
				DstThmID,
				DstGridPriority,
				DstTextFileName,
				DstTextFileFormat,
				DstCompTemp,
				DstCalBlockTemp,
				DstRange,
				DstThin,
				DstThick,
				DstVelocity,
				DstGainDb,
				DstCrewDose
				from Dsets
				where DstDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				DstDBid = (Guid?)dr[0];
				DstName = (string)dr[1];
				DstIspID = (Guid?)dr[2];
				DstGrdID = (Guid?)Util.NullForDbNull(dr[3]);
				DstInsID = (Guid?)Util.NullForDbNull(dr[4]);
				DstDcrID = (Guid?)Util.NullForDbNull(dr[5]);
				DstMtrID = (Guid?)Util.NullForDbNull(dr[6]);
				DstCbkID = (Guid?)Util.NullForDbNull(dr[7]);
				DstThmID = (Guid?)Util.NullForDbNull(dr[8]);
				DstGridPriority = (short?)Util.NullForDbNull(dr[9]);
				DstTextFileName = (string)Util.NullForDbNull(dr[10]);
				DstTextFileFormat = (byte?)Util.NullForDbNull(dr[11]);
				DstCompTemp = (short?)Util.NullForDbNull(dr[12]);
				DstCalBlockTemp = (short?)Util.NullForDbNull(dr[13]);
				DstRange = (decimal?)Util.NullForDbNull(dr[14]);
				DstThin = (decimal?)Util.NullForDbNull(dr[15]);
				DstThick = (decimal?)Util.NullForDbNull(dr[16]);
				DstVelocity = (decimal?)Util.NullForDbNull(dr[17]);
				DstGainDb = (decimal?)Util.NullForDbNull(dr[18]);
				DstCrewDose = (short?)Util.NullForDbNull(dr[19]);
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
				DstDBid = (Guid?)(Util.NullForDbNull(cmd.ExecuteScalar()));

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", DstDBid),
					new SqlCeParameter("@p1", DstName),
					new SqlCeParameter("@p2", DstIspID),
					new SqlCeParameter("@p3", Util.DbNullForNull(DstGrdID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(DstInsID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(DstDcrID)),
					new SqlCeParameter("@p6", Util.DbNullForNull(DstMtrID)),
					new SqlCeParameter("@p7", Util.DbNullForNull(DstCbkID)),
					new SqlCeParameter("@p8", Util.DbNullForNull(DstThmID)),
					new SqlCeParameter("@p9", Util.DbNullForNull(DstGridPriority)),
					new SqlCeParameter("@p10", Util.DbNullForNull(DstTextFileName)),
					new SqlCeParameter("@p11", Util.DbNullForNull(DstTextFileFormat)),
					new SqlCeParameter("@p12", Util.DbNullForNull(DstCompTemp)),
					new SqlCeParameter("@p13", Util.DbNullForNull(DstCalBlockTemp)),
					new SqlCeParameter("@p14", Util.DbNullForNull(DstRange)),
					new SqlCeParameter("@p15", Util.DbNullForNull(DstThin)),
					new SqlCeParameter("@p16", Util.DbNullForNull(DstThick)),
					new SqlCeParameter("@p17", Util.DbNullForNull(DstVelocity)),
					new SqlCeParameter("@p18", Util.DbNullForNull(DstGainDb)),
					new SqlCeParameter("@p19", Util.DbNullForNull(DstCrewDose))
					});
				cmd.CommandText = @"Insert Into Dsets (
					DstDBid,
					DstName,
					DstIspID,
					DstGrdID,
					DstInsID,
					DstDcrID,
					DstMtrID,
					DstCbkID,
					DstThmID,
					DstGridPriority,
					DstTextFileName,
					DstTextFileFormat,
					DstCompTemp,
					DstCalBlockTemp,
					DstRange,
					DstThin,
					DstThick,
					DstVelocity,
					DstGainDb,
					DstCrewDose
				) values (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Dsets row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", DstDBid),
					new SqlCeParameter("@p1", DstName),
					new SqlCeParameter("@p2", DstIspID),
					new SqlCeParameter("@p3", Util.DbNullForNull(DstGrdID)),
					new SqlCeParameter("@p4", Util.DbNullForNull(DstInsID)),
					new SqlCeParameter("@p5", Util.DbNullForNull(DstDcrID)),
					new SqlCeParameter("@p6", Util.DbNullForNull(DstMtrID)),
					new SqlCeParameter("@p7", Util.DbNullForNull(DstCbkID)),
					new SqlCeParameter("@p8", Util.DbNullForNull(DstThmID)),
					new SqlCeParameter("@p9", Util.DbNullForNull(DstGridPriority)),
					new SqlCeParameter("@p10", Util.DbNullForNull(DstTextFileName)),
					new SqlCeParameter("@p11", Util.DbNullForNull(DstTextFileFormat)),
					new SqlCeParameter("@p12", Util.DbNullForNull(DstCompTemp)),
					new SqlCeParameter("@p13", Util.DbNullForNull(DstCalBlockTemp)),
					new SqlCeParameter("@p14", Util.DbNullForNull(DstRange)),
					new SqlCeParameter("@p15", Util.DbNullForNull(DstThin)),
					new SqlCeParameter("@p16", Util.DbNullForNull(DstThick)),
					new SqlCeParameter("@p17", Util.DbNullForNull(DstVelocity)),
					new SqlCeParameter("@p18", Util.DbNullForNull(DstGainDb)),
					new SqlCeParameter("@p19", Util.DbNullForNull(DstCrewDose))});

				cmd.CommandText =
					@"Update Dsets 
					set					
					DstName = @p1,					
					DstIspID = @p2,					
					DstGrdID = @p3,					
					DstInsID = @p4,					
					DstDcrID = @p5,					
					DstMtrID = @p6,					
					DstCbkID = @p7,					
					DstThmID = @p8,					
					DstGridPriority = @p9,					
					DstTextFileName = @p10,					
					DstTextFileFormat = @p11,					
					DstCompTemp = @p12,					
					DstCalBlockTemp = @p13,					
					DstRange = @p14,					
					DstThin = @p15,					
					DstThick = @p16,					
					DstVelocity = @p17,					
					DstGainDb = @p18,					
					DstCrewDose = @p19
					Where DstDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update dsets row");
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
			if (!DsetNameValid(DsetName)) return false;
			if (!DsetTextFileNameValid(DsetTextFileName)) return false;

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
			if (DstDBid == null)
			{
				DsetErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			// I'm currently allowing a Dset to be deleted even if it is contributing to a Grid
			// i.e. its GridID is not null.  Cascading deletes will cause all children to be deleted
			// including any rows in GridSizes that are associated with measurements associated with
			// the current Dset.

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
					@"Delete from Dsets 
					where DstDBid = @p0";
				cmd.Parameters.Add("@p0", DstDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					DsetErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					DsetErrMsg = null;
					OnChanged(ID); 
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		public void ShiftRows(int shift)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
		    cmd.CommandText =
			    @"update Measurements
			    set MsrRow = MsrRow + @p1
			    where MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
            cmd.Parameters.Add("@p0", DstDBid);
            cmd.Parameters.Add("@p1", shift);

			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}

        public void MoveLastColumnToZero()
        {
            int endCol;
            endCol = (int)(this.DsetEndCol);
            SqlCeCommand cmd = Globals.cnn.CreateCommand();
            cmd.CommandText =
                @"UPDATE Measurements
			    SET MsrCol = 0
			    WHERE MsrCol = @p1 AND MsrSvyID IN (SELECT DISTINCT SvyDBid FROM Surveys WHERE SvyDstID = @p0)";
            cmd.Parameters.Add("@p0", DstDBid);
            cmd.Parameters.Add("@p1", endCol);
            if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
            cmd.ExecuteNonQuery();
        }

		public void ShiftCols(int shift)
		{
            // a single leftward shift is allowed for dealing with reversal of column orientation
            // the left-most column is moved to the right-most position
            // this is not necessary for rows.
            int endCol;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
            if (shift == -1 && DsetStartCol == 0)
            {
                endCol = (int)(this.DsetEndCol);
                // Stupid compact sql won't allow a CASE in the SET, so we have to use two queries
                cmd.CommandText =
                    @"update Measurements
				    set MsrCol = MsrCol - 1
				    where MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
                cmd.Parameters.Add("@p0", DstDBid);
                if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
                cmd.ExecuteNonQuery();
                
                cmd = Globals.cnn.CreateCommand();
                cmd.CommandText =
                    @"update Measurements
				    set MsrCol = @p1
				    where MsrCol = -1 AND MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
                cmd.Parameters.Add("@p0", DstDBid);
                cmd.Parameters.Add("@p1", endCol);
                if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText =
                    @"update Measurements
				    set MsrCol = MsrCol + @p1
				    where MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
                cmd.Parameters.Add("@p0", DstDBid);
                cmd.Parameters.Add("@p1", shift);
                if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
                cmd.ExecuteNonQuery();
            }
		}

		public void ReverseRows()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"update Measurements
				set MsrRow = @p1 - MsrRow
				where MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
			cmd.Parameters.Add("@p0", DstDBid);
			cmd.Parameters.Add("@p1", DsetEndRow);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}

		public void ReverseCols()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"update Measurements
				set MsrCol = @p1 - MsrCol
				where MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
			cmd.Parameters.Add("@p0", DstDBid);
			cmd.Parameters.Add("@p1", DsetEndCol);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}

		public void Transpose()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"update Measurements
				set MsrCol = MsrRow, MsrRow = MsrCol
				where MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
			cmd.Parameters.Add("@p0", DstDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}

		public void DeleteRows(int startRow, int endRow)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Delete From Measurements
				where MsrRow >= @p1 and MsrRow <= @p2
				and MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
			cmd.Parameters.Add("@p0", DstDBid);
			cmd.Parameters.Add("@p1", startRow);
			cmd.Parameters.Add("@p2", endRow);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}

		public void DeleteCols(int startCol, int endCol)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Delete From Measurements
				where MsrCol >= @p1 and MsrCol <= @p2
				and MsrSvyID in (Select Distinct SvyDBid from Surveys where SvyDstID = @p0)";
			cmd.Parameters.Add("@p0", DstDBid);
			cmd.Parameters.Add("@p1", startCol);
			cmd.Parameters.Add("@p2", endCol);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
		}



		//--------------------------------------------------------------------
		// Static listing methods which return collections of dsets
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EDsetCollection ListByNameForInspection(Guid InspectionID)
		{
			EDset dset;
			EDsetCollection dsets = new EDsetCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				DstDBid,
				DstName,
				DstIspID,
				DstGrdID,
				DstInsID,
				DstDcrID,
				DstMtrID,
				DstCbkID,
				DstThmID,
				DstGridPriority,
				DstTextFileName,
				DstTextFileFormat,
				DstCompTemp,
				DstCalBlockTemp,
				DstRange,
				DstThin,
				DstThick,
				DstVelocity,
				DstGainDb,
				DstCrewDose
				from Dsets";

			qry += " where DstIspID = @p1";
			qry += "	order by DstName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", InspectionID);

			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				dset = new EDset((Guid?)dr[0]);
				dset.DstName = (string)(dr[1]);
				dset.DstIspID = (Guid?)(dr[2]);
				dset.DstGrdID = (Guid?)Util.NullForDbNull(dr[3]);
				dset.DstInsID = (Guid?)Util.NullForDbNull(dr[4]);
				dset.DstDcrID = (Guid?)Util.NullForDbNull(dr[5]);
				dset.DstMtrID = (Guid?)Util.NullForDbNull(dr[6]);
				dset.DstCbkID = (Guid?)Util.NullForDbNull(dr[7]);
				dset.DstThmID = (Guid?)Util.NullForDbNull(dr[8]);
				dset.DstGridPriority = (short?)Util.NullForDbNull(dr[9]);
				dset.DstTextFileName = (string)Util.NullForDbNull(dr[10]);
				dset.DstTextFileFormat = (byte?)Util.NullForDbNull(dr[11]);
				dset.DstCompTemp = (short?)Util.NullForDbNull(dr[12]);
				dset.DstCalBlockTemp = (short?)Util.NullForDbNull(dr[13]);
				dset.DstRange = (decimal?)Util.NullForDbNull(dr[14]);
				dset.DstThin = (decimal?)Util.NullForDbNull(dr[15]);
				dset.DstThick = (decimal?)Util.NullForDbNull(dr[16]);
				dset.DstVelocity = (decimal?)Util.NullForDbNull(dr[17]);
				dset.DstGainDb = (decimal?)Util.NullForDbNull(dr[18]);
				dset.DstCrewDose = (short?)Util.NullForDbNull(dr[19]);

				dsets.Add(dset);	
			}
			// Finish up
			dr.Close();
			return dsets;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataView(Guid? InspectionID)
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
					DstDBid as ID,
					DstGridPriority as DsetGridPriority,
					DstName as DsetName,
					CASE 
						WHEN Count(SvyDBid) > 0 THEN 'Yes'
						ELSE 'No'
					END as DsetHasTextFile,
					Count(AdmDBid) as DsetAdditionalMeasurements
					from Dsets 
					left outer join AdditionalMeasurements on DstDBid = AdmDstID
					left outer join Surveys on DstDBid = SvyDstID
					where DstIspID = @p1
					group by DstDBid, DstGridPriority, DstName";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", InspectionID == null ? Guid.Empty : InspectionID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}

		public static DataTable GetWithGridAssignmentsForInspection(Guid InspectionID)
		{
			DataSet ds = new DataSet();
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			// Get names and ids for dsets that contain at least one surveys 
			// I noticed something very peculiar here with SqlCE.  If I have a query without any 
			// aggregation: e.g. Select DstDBid from Dsets inner join Surveys
			// It works as expected: Dsets are only returned if they are referenced by a Survey.
			// However, if aggregation is added: e.g. Select DstDbid, DstName from Dsets inner join surveys
			// It lists ALL Dsets, even ones which are not referenced by a survey.
			// To my utter amazement, even after adding a 'where SvyDstID is not NULL' clause,
			// ALL Dsets still show up!
			// I think this is a bug.  I'm working around it by adding a 'group by SvyDstID' which
			// should not do anything, but forces the join to work right.
			string qry = @"Select 
				DstDBid as ID,
				DstName as DsetName,
				DstGridPriority,
				SvyDstID,
				Max(
					CASE
						WHEN DstGrdID is null THEN 0 
						ELSE 1
					END
				)	as IsAssigned
				from Dsets 
				inner join Surveys on DstDBid = SvyDstID 
				where DstIspID = @p1
				group by DstGridPriority, DstDBid, DstName, SvyDstID
				order by DstGridPriority";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", InspectionID);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			return ds.Tables[0];
		}

		public static void UpdateAssignmentsToGrid(Guid GridID, DataTable assignments)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			// First remove all dsets from the grid
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = @"Update Dsets set DstGrdID = NULL where DstGrdID = @p1";
			cmd.Parameters.Add("@p1", GridID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Then add the selected dsets back in
			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", GridID);
			cmd.Parameters.Add("@p2", "");
			cmd.CommandText =
@"				Update Dsets set DstGrdID = @p1 
				where DstDBid = @p2";

			// Now insert the current grid assignments
			for (int dmRow = 0; dmRow < assignments.Rows.Count; dmRow++)
			{
				if (Convert.ToBoolean(assignments.Rows[dmRow]["IsAssigned"]))
				{
					cmd.Parameters["@p2"].Value = (Guid)assignments.Rows[dmRow]["ID"];
					if (cmd.ExecuteNonQuery() != 1)
					{
						throw new Exception("Unable to assign Dataset to Grid");
					}
				}
			}
		}

		// The UI should save the current dset before calling this so we at least
		// have an ID for the current dset and hopefully a file name.

		// It should also pass us a background worker that has its report progress
		// property set.
		public bool PrepareToLoadTextfile(string filePath)
		{
			// If no file name has been set for the dset show a messagebox and return false

			if (filePath == null)
			{
				MessageBox.Show("No file path was specified", "Factotum");
				return false;
			}

			// Try to identify the textfile type
			// If we fail give a message box and return false.
			MeterTextFileType type = IdentifyTextfileType(filePath);
			DsetTextFileFormat = (byte?)IdentifyTextfileType(filePath);
			if (type == MeterTextFileType.Unknown)
			{
				MessageBox.Show("The specified file type is not supported", "Factotum");
				return false;
			}

			// Check if the current dset has any measurements.  If it does, ask the user if 
			// they still want to proceed.
			SqlCeCommand cmd;
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText = 
				@"Select MsrDBid from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					where SvyDstID = @p1";

			cmd.Parameters.Add("@p1", ID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			if (Util.NullForDbNull(cmd.ExecuteScalar()) != null)
			{
				DialogResult rslt = MessageBox.Show(
					"If you continue, any existing measurements will be deleted\nbefore the text file is imported.  Continue?",
					"Factotum: Replace Existing Measurements?",MessageBoxButtons.YesNo);
				if (rslt != DialogResult.Yes) return false;

			}
			// Delete all surveys for the dataset and any associated measurements
			ESurvey.DeleteAllForDset(ID, false);

			// If we succeed, update this dset with the textfile type and save.

			// Delete any surveys (and measurements) that belong to this Dset. 

			// Create a new parser of the appropriate type and call its ParseTextFile()
			// method.  This method will parse the file directly into the database.  
			// It will return true if it succeeds and will display a messagebox and 
			// return false if it runs into trouble.

			// Note: I had considered having the parse build a dataset instead,
			// but that approach didn't work because I want to stay consistent with Guids 
			// instead of autonumbered ints and datasets can't generate new Guids.
 

			// Then create a new Parser object of the appropriate type.
			// Then tell the parser to parse the file into the dataset.

			// The parser will add rows into the dataset that is provided and return it.

			// If the parser didn't report errors, update the database with the dataset.


			return true;
		}

		private MeterTextFileType IdentifyTextfileType(string filePath)
		{
			StreamReader sw;
			MeterTextFileType result = MeterTextFileType.Unknown;

			// Try to open the text file
			using (sw = new StreamReader(filePath))
			{
				try
				{
					string firstLine = sw.ReadLine();
					if (firstLine == null)
					{
						MessageBox.Show("No data was found in the specified file", "Factotum");
						return MeterTextFileType.Unknown;
					}
					if (firstLine == "[SWEET 16]" || 
						firstLine == "[CHILD]" ||
						firstLine == "[FACTOTUM MERGED]")
						result = MeterTextFileType.Panametrics;
					else
					{
						string[] sa = firstLine.Split(null);
						if (sa.Length == 0)
							result = MeterTextFileType.Unknown;
						else
						{
							decimal d;
							foreach (string s in sa)
							{
								result = MeterTextFileType.Krautkramer;
								if (s != "EMPTY" && s != "OBSTR" && !decimal.TryParse(s, out d))
								{
									result = MeterTextFileType.Unknown;
									break;
								}
							}
						}
					}
				}
				catch
				{
					MessageBox.Show("Unable to read from the specified file.", "Factotum");
					return MeterTextFileType.Unknown;
				}
			}
			return result;
		}


		//--------------------------------------
		// Private utilities
		//--------------------------------------

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExists(string name, Guid? id)
		{
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			cmd.Parameters.Add(new SqlCeParameter("@p2", DstIspID));
			if (id == null)
			{
				cmd.CommandText = "Select DstDBid from Dsets where DstName = @p1 and DstIspID = @p2";
			}
			else
			{
				cmd.CommandText = "Select DstDBid from Dsets where DstName = @p1 and DstIspID = @p2 and DstDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			bool exists = (val != null);
			return exists;
		}

		private short getNewGridPriority(Guid inspectionID)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			
			cmd.Parameters.Add(new SqlCeParameter("@p1", inspectionID));
			cmd.CommandText = "Select Max(DstGridPriority) from Dsets where DstIspID = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			short newReportOrder = (short)(val == null ? 0 : Convert.ToUInt16(val) + 1);
			return newReportOrder;
		}



		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (DsetName == null)
			{
				DstNameErrMsg = "A unique Dataset Name is required";
				allFilled = false;
			}
			else
			{
				DstNameErrMsg = null;
			}
			return allFilled;
		}

		private string GetUniqueDsetName(Guid InspectionID)
		{
			int i;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.CommandText = 
				@"Select Count(DstDBid) from Dsets
				where DstIspID = @p1";
			cmd.Parameters.Add("@p1", InspectionID);

			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			i = (int)cmd.ExecuteScalar() + 1; // Start with 'Dataset 1'

			cmd = Globals.cnn.CreateCommand();
			cmd.Parameters.Add("@p1", "Dataset " + i);
			cmd.Parameters.Add("@p2", InspectionID);
			cmd.CommandText = 
				@"Select DstName from Dsets 
				where DstName = @p1
				and DstIspID = @p2";

			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			while (val != null)
			{
				i++;
				cmd.Parameters["@p1"].Value = "Dataset " + i;
				val = cmd.ExecuteScalar();
			}
			return "Dataset " + i;
		}

	}

	//--------------------------------------
	// Dset Collection class
	//--------------------------------------
	public class EDsetCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EDsetCollection()
		{ }
		//the indexer of the collection
		public EDset this[int index]
		{
			get
			{
				return (EDset)this.List[index];
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
			foreach (EDset dset in InnerList)
			{
				if (dset.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EDset item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EDset item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EDset item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EDset item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
