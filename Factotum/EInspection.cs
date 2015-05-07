using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EInspection : IEntity
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
		private Guid? IspDBid;
		private string IspName;
		private Guid? IspIscID;
		private string IspNotes;
		private short? IspReportOrder;
		private float? IspPersonHours;

		// Textbox limits
		public static int IspNameCharLimit = 25;
		public static int IspNotesCharLimit = 4000;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string IspNameErrMsg;
		private string IspNotesErrMsg;
		private string IspPersonHoursErrMsg;

		// Form level validation message
		private string IspErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return IspDBid; }
		}

		public string InspectionName
		{
			get { return IspName; }
			set { IspName = Util.NullifyEmpty(value); }
		}

		public Guid? InspectionIscID
		{
			get { return IspIscID; }
			set 
			{
				if (IspIscID == null && value != null)
				{
					// We're setting the parent for a new record, so get the next report
					// order in the sequence.
					IspReportOrder = getNewReportOrder((Guid)value);
					IspName = GetDefaultInspectionName((Guid)value);
				}
				IspIscID = value; 
			}
		}

		public string InspectionNotes
		{
			get { return IspNotes; }
			set { IspNotes = Util.NullifyEmpty(value); }
		}

		public short? InspectionReportOrder
		{
			get { return IspReportOrder; }
			set { IspReportOrder = value; }
		}

		public float? InspectionPersonHours
		{
			get { return IspPersonHours; }
			set { IspPersonHours = value; }
		}

		public bool InspectionHasGrid
		{
			get
			{
				if (IspDBid == null) return false;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "Select GrdDBid from Grids where GrdIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", IspDBid));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = cmd.ExecuteScalar();
				bool exists = (val != null);
				return exists;
			}
		}

		public Guid? GridID
		{
			get
			{
				if (IspDBid == null) return null;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "Select GrdDBid from Grids where GrdIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", IspDBid));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = cmd.ExecuteScalar();
				if (val != null) return (Guid?)val;
				else return null;
			}
		}

		public Guid? GraphicID
		{
			get
			{
				if (IspDBid == null) return null;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "Select GphDBid from Graphics where GphIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", IspDBid));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = cmd.ExecuteScalar();
				if (val != null) return (Guid?)val;
				else return null;
			}
		}

		public bool InspectionHasGraphic
		{
			get
			{
				if (IspDBid == null) return false;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "Select GphDBid from Graphics where GphIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", IspDBid));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = cmd.ExecuteScalar();
				bool exists = (val != null);
				return exists;
			}
		}

		public short? InspectionCrewDose
		{
			get
			{
				if (IspDBid == null) return null;
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "Select Sum(DstCrewDose) from Dsets where DstIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", ID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				if (val != null) return Convert.ToInt16(val);
				else return null;
			}
		}

		// Get the ID of the "Main" dataset in the inspection, based on total measurements.
		// We have to use temp tables because SQL CE doesn't allow subqueries.
		public Guid? InspectionMainDsetID
		{
			get
			{
				if (IspDBid == null) return null;

				SqlCeCommand cmd;

				// Delete the temp tables if they exist -- they shouldn't
				Globals.DeleteTempTableIfExists("tmpDset");
				Globals.DeleteTempTableIfExists("tmpSmry");

				
				// Create the temp table which unions all measurements for the inspection
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"create table tmpDset (dst uniqueidentifier, msr uniqueidentifier)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				// Create the temp summary table, which includes totals for each inspection
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"create table tmpSmry (dst uniqueidentifier, count int)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				// Add the regular measurement count
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"insert into tmpDset (dst, msr)
					select DstDBid, MsrDbid
					from Dsets
					inner join Surveys on DstDBid = SvyDstID
					inner join Measurements on SvyDBid = MsrSvyID
					where DstIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", ID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				// Add the additional measurement counts
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"insert into tmpDset (dst, msr)
					select DstDBid, AdmDBid
					from Dsets
					inner join AdditionalMeasurements on DstDBid = AdmDstID
					where DstIspID = @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", ID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				// Add the totals for each dataset
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"insert into tmpSmry (dst, count)
					select dst, Count(msr)
					from tmpDset
					group by dst";
				cmd.Parameters.Add(new SqlCeParameter("@p0", ID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				// Get the id of the dset with the highest count
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select dst, count
					from tmpSmry
					order by count desc";
				cmd.Parameters.Add(new SqlCeParameter("@p0", ID));
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				object val = Util.NullForDbNull(cmd.ExecuteScalar());
				
				// Drop the temp tables
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "drop table tmpDset";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText = "drop table tmpSmry";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				cmd.ExecuteNonQuery();

				if (val != null) return (Guid)val;
				else return null;
			}
		}

		// The points in the textfile including obstructions, but excluding empties.
		public int? InspectionTextFilePoints
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(MsrDBid) from Measurements 
							inner join Surveys on MsrSvyID = SvyDBid
							inner join Dsets on SvyDstID = DstDBid
							where DstIspID = @p0 and 
							(MsrThickness is not NULL or MsrIsObstruction = 1)";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		// Total number of measurements used by statistics.  
		// I.e. all textfile measurements (excluding empties and obstructions)
		// plus any additional measurements whose 'IncludeInStats' flag is set.
		public int? InspectionMeasurements
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
						inner join Dsets on SvyDstID = DstDBid
						where DstIspID = @p0 and MsrThickness is not NULL
						union select count (AdmDBid) from AdditionalMeasurements
						inner join Dsets on AdmDstID = DstDBid
						where DstIspID = @p1 and AdmIncludeInStats = 1 and AdmThickness is not null";

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
		public int? InspectionObstructions
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(MsrDBid) from Measurements 
					inner join Surveys on MsrSvyID = SvyDBid
					inner join Dsets on SvyDstID = DstDBid
					where DstIspID = @p0 and MsrIsObstruction = 1";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}
		// The total number of cells in all the rows and columns spanned by the textfile
		// less any measurements or obstructions.
		// Note InspectionTextFilePoints includes obstructions, so we don't need to subtract them...
		public int? InspectionEmpties
		{
			get
			{
				if (ID == null) return null;
				int empties = 0;
				EDsetCollection dsets = EDset.ListByNameForInspection((Guid)ID);
				foreach (EDset dset in dsets)
				{
					empties += (int)dset.DsetEmpties;
				}
				return empties;
			}
		}

		// The total number of additional measurements
		public int? InspectionAdditionalMeasurements
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(AdmDBid) from AdditionalMeasurements 
					inner join Dsets on AdmDstID = DstDBid
					where DstIspID = @p0";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}

		// The total number of additional measurements which are to be included in statistics.
		public int? InspectionAdditionalMeasurementsWithStats
		{
			get
			{
				if (ID == null) return null;
				SqlCeCommand cmd;
				cmd = Globals.cnn.CreateCommand();
				cmd.CommandText =
					@"select count(AdmDBid) from AdditionalMeasurements 
					inner join Dsets on AdmDstID = DstDBid
					where DstIspID = @p0 and AdmIncludeInStats = 1 and AdmThickness is not null";

				cmd.Parameters.Add("@p0", ID);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				return Convert.ToInt32(cmd.ExecuteScalar());
			}
		}
		// The minimum of all measurements (textfile or additional with IncludeInStats set)
		public float? InspectionMinWall
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
					inner join Dsets on SvyDstID = DstDBid
					where DstIspID = @p0
					union select min(AdmThickness) from AdditionalMeasurements
					inner join Dsets on AdmDstID = DstDBid
					where DstIspID = @p1 and AdmIncludeInStats = 1 and AdmThickness is not null";

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
		public float? InspectionMaxWall
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
					inner join Dsets on SvyDstID = DstDBid
					where DstIspID = @p0
					union select max(AdmThickness) from AdditionalMeasurements
					inner join Dsets on AdmDstID = DstDBid
					where DstIspID = @p1 and AdmIncludeInStats = 1 and AdmThickness is not null";

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
		public float? InspectionMeanWall
		{
			get
			{
				if (ID == null) return null;
				double sum = 0d;
				object test;
				int? totMeas = InspectionMeasurements;
				int? addlMeas = InspectionAdditionalMeasurementsWithStats;
				if (totMeas == null || totMeas == 0) return null;
				SqlCeCommand cmd;
				if (addlMeas != null && addlMeas > 0) // We have some additional measurements so add up the thicknesses
				{
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
					@"select sum(AdmThickness) from AdditionalMeasurements 
					inner join Dsets on AdmDstID = DstDBid
					where DstIspID = @p0 and AdmIncludeInStats = 1 and AdmThickness is not null";

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
					inner join Dsets on SvyDstID = DstDBid
					where DstIspID = @p0";

					cmd.Parameters.Add("@p0", ID);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					test = Util.NullForDbNull(cmd.ExecuteScalar());
					if (test != null) sum += Convert.ToDouble(test);
				}
				return (float)(sum / totMeas);

			}
		}
		// The standard deviation of all measurements (textfile or additional with IncludeInStats set)
		public float? InspectionStdevWall
		{
			get
			{
				if (ID == null) return null;
				float? meanWall = InspectionMeanWall;
				if (meanWall == null) return null;

				double sum = 0d;
				object test;
				int? totMeas = InspectionMeasurements;
				int? addlMeas = InspectionAdditionalMeasurementsWithStats;
				if (totMeas == null || totMeas == 0) return null;
				if (totMeas == 1) return 0;
				SqlCeCommand cmd;
				if (addlMeas != null && addlMeas > 0) // We have some additional measurements so add up the thicknes error squares
				{
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
					@"select sum((@p1 - AdmThickness)*(@p2 - AdmThickness)) from AdditionalMeasurements 
					inner join Dsets on AdmDstID = DstDBid
					where DstIspID = @p0 and AdmIncludeInStats = 1 and AdmThickness is not null";

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
					inner join Dsets on SvyDstID = DstDBid
					where DstIspID = @p0";

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

		// Count of non grid measurements below tScreen
		public int NonGridBelowTscreen
		{
			get
			{
				if (ID == null) return 0;
				{
					SqlCeCommand cmd;
                    SqlCeDataReader dr;
                    ArrayList tscrs = new ArrayList(6);
 					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText = 
                    @"select cmp.CmpUpMainTscr,cmp.CmpUpExtTscr,cmp.CmpDnMainTscr,cmp.CmpDnExtTscr,cmp.CmpBranchTscr,cmp.CmpBrExtTscr
                    from inspections isp 
                    inner join InspectedComponents isc on isp.IspIscID = isc.IscDBid
                    inner join Components cmp on isc.IscCmpID = cmp.CmpDBid
                    where isp.IspDBid = @p0";

					cmd.Parameters.Add("@p0", ID);
                    if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        tscrs.Add((decimal?)Util.NullForDbNull(dr[0]));
                        tscrs.Add((decimal?)Util.NullForDbNull(dr[1]));
                        tscrs.Add((decimal?)Util.NullForDbNull(dr[2]));
                        tscrs.Add((decimal?)Util.NullForDbNull(dr[3]));
                        tscrs.Add((decimal?)Util.NullForDbNull(dr[4]));
                        tscrs.Add((decimal?)Util.NullForDbNull(dr[5]));
                    }
                    dr.Close();
                  
					int count = 0;
					cmd = Globals.cnn.CreateCommand();
					cmd.CommandText =
                        @"select adm.AdmComponentSection, adm.AdmThickness from Inspections isp
                        inner join Dsets dst on isp.IspDBid = dst.DstIspID
                        inner join AdditionalMeasurements adm on dst.DstDBid = adm.AdmDstID
                        where adm.AdmIncludeInStats = 1 and adm.AdmThickness is not null
                        and adm.AdmComponentSection is not null
                        and isp.IspDBid = @p0";

					cmd.Parameters.Add("@p0", ID);
					if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
					dr = cmd.ExecuteReader();
                    decimal? thickness;
                    ComponentSectionEnum section;
                    while (dr.Read())
                    {
                        section = (ComponentSectionEnum)Util.NullForDbNull(dr[0]);
                        thickness = (decimal?)Util.NullForDbNull(dr[1]);
                        if (tscrs[(int)section] != null && thickness < (decimal?)tscrs[(int)section])
                        {
                            count += 1;
                        }
                    }
					dr.Close();
					return count;
				}
			}
		}




		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string InspectionNameErrMsg
		{
			get { return IspNameErrMsg; }
		}

		public string InspectionNotesErrMsg
		{
			get { return IspNotesErrMsg; }
		}

		public string InspectionPersonHoursErrMsg
		{
			get { return IspPersonHoursErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string InspectionErrMsg
		{
			get { return IspErrMsg; }
			set { IspErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool InspectionNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > IspNameCharLimit)
			{
				IspNameErrMsg = string.Format("Inspection Names cannot exceed {0} characters", IspNameCharLimit);
				return false;
			}
			else
			{
				IspNameErrMsg = null;
				return true;
			}
		}

		public bool InspectionNotesLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > IspNotesCharLimit)
			{
				IspNotesErrMsg = string.Format("Inspection Notes cannot exceed {0} characters", IspNotesCharLimit);
				return false;
			}
			else
			{
				IspNotesErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool InspectionNameValid(string name)
		{
			if (!InspectionNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			if (NameExists(name, (Guid?)IspDBid))
			{
				IspNameErrMsg = "That Inspection Name is already in use.";
				return false;
			}
			IspNameErrMsg = null;
			return true;
		}

		public bool InspectionNotesValid(string value)
		{
			if (!InspectionNotesLengthOk(value)) return false;

			IspNotesErrMsg = null;
			return true;
		}

		public bool InspectionPersonHoursValid(string value)
		{
			float result;
			if (Util.IsNullOrEmpty(value))
			{
				IspPersonHoursErrMsg = null;
				return true;
			}
			if (float.TryParse(value, out result) && result >= 0)
			{
				IspPersonHoursErrMsg = null;
				return true;
			}
			IspPersonHoursErrMsg = string.Format("Please enter a number greater than or equal to zero, or leave blank");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EInspection()
		{
			this.IspReportOrder = 0;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EInspection(Guid? id) : this()
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
				IspDBid,
				IspName,
				IspIscID,
				IspNotes,
				IspReportOrder,
				IspPersonHours
				from Inspections
				where IspDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				IspDBid = (Guid?)dr[0];
				IspName = (string)dr[1];
				IspIscID = (Guid?)dr[2];
				IspNotes = (string)Util.NullForDbNull(dr[3]);
				IspReportOrder = (short?)dr[4];
				// I tried using a cast here in the usual way, but got an InvalidCast exception
				// because the NullForDbNull function is returning a boxed double not a boxed float
				if (Util.NullForDbNull(dr[5]) == null) IspPersonHours = null;
				else IspPersonHours = Convert.ToSingle(dr[5]);
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
				IspDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", IspDBid),
					new SqlCeParameter("@p1", IspName),
					new SqlCeParameter("@p2", IspIscID),
					new SqlCeParameter("@p3", Util.DbNullForNull(IspNotes)),
					new SqlCeParameter("@p4", IspReportOrder),
					new SqlCeParameter("@p5", Util.DbNullForNull(IspPersonHours))
					});
				cmd.CommandText = @"Insert Into Inspections (
					IspDBid,
					IspName,
					IspIscID,
					IspNotes,
					IspReportOrder,
					IspPersonHours
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Inspections row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", IspDBid),
					new SqlCeParameter("@p1", IspName),
					new SqlCeParameter("@p2", IspIscID),
					new SqlCeParameter("@p3", Util.DbNullForNull(IspNotes)),
					new SqlCeParameter("@p4", IspReportOrder),
					new SqlCeParameter("@p5", Util.DbNullForNull(IspPersonHours))});

				cmd.CommandText =
					@"Update Inspections 
					set					
					IspName = @p1,					
					IspIscID = @p2,					
					IspNotes = @p3,					
					IspReportOrder = @p4,					
					IspPersonHours = @p5
					Where IspDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update inspections row");
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
			if (!InspectionNameValid(InspectionName)) return false;
			if (!InspectionNotesValid(InspectionNotes)) return false;

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
			if (IspDBid == null)
			{
				InspectionErrMsg = "Unable to delete. Record not found.";
				return false;
			}

			if (HasGraphic())
			{
				InspectionErrMsg = "Unable to delete because this Inspection has a Graphic.";
				return false;
			}

			if (HasGrid())
			{
				InspectionErrMsg = "Unable to delete because this Inspection has a Grid.";
				return false;
			}

			if (HasDsets())
			{
				InspectionErrMsg = "Unable to delete because this Inspection has one or more Datasets.";
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
				// First update the report order for all sections after this one.
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Update Inspections 
					set IspReportOrder = IspReportOrder - 1
					where IspIscID = @p1
					and IspReportOrder > @p2";
				cmd.Parameters.Add("@p1", IspIscID);
				cmd.Parameters.Add("@p2", IspReportOrder);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from Inspections 
					where IspDBid = @p0";
				cmd.Parameters.Add("@p0", IspDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					InspectionErrMsg = "Unable to delete.  Please try again later.";
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
				InspectionErrMsg = null;
				return false;
			}
		}

		private bool HasGraphic()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select GphDBid from Graphics
					where GphIspID = @p0";
			cmd.Parameters.Add("@p0", IspDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		private bool HasGrid()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select GrdDBid from Grids
					where GrdIspID = @p0";
			cmd.Parameters.Add("@p0", IspDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		private bool HasDsets()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select DstDBid from Dsets
					where DstIspID = @p0";
			cmd.Parameters.Add("@p0", IspDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of inspections
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EInspectionCollection ListByReportOrderForInspectedComponent(
			Guid InspectedComponentID, bool addNoSelection)
		{
			EInspection inspection;
			EInspectionCollection inspections = new EInspectionCollection();

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				inspection = new EInspection();
				inspection.IspName = "<No Selection>";
				inspections.Add(inspection);
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				IspDBid,
				IspName,
				IspIscID,
				IspNotes,
				IspReportOrder,
				IspPersonHours
				from Inspections";

			qry += "	where IspIscID = @p1";
			qry += "	order by IspReportOrder";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", InspectedComponentID);
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				inspection = new EInspection((Guid?)dr[0]);
				inspection.IspName = (string)(dr[1]);
				inspection.IspIscID = (Guid?)(dr[2]);
				inspection.IspNotes = (string)Util.NullForDbNull(dr[3]);
				inspection.IspReportOrder = (short?)(dr[4]);
				// I tried using a cast here in the usual way, but got an InvalidCast exception
				// because the NullForDbNull function is returning a boxed double not a boxed float
				if (Util.NullForDbNull(dr[5]) == null) inspection.IspPersonHours = null;
				else inspection.IspPersonHours = Convert.ToSingle(dr[5]);

				inspections.Add(inspection);	
			}
			// Finish up
			dr.Close();
			return inspections;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataViewForInspectedComponent(Guid? inspectedComponent)
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
				IspDBid as ID, 
				IspReportOrder as InspectionReportOrder,
				IspName as InspectionName, 
				CASE 
					WHEN Count(GrdIspID) = 0 THEN	'No'
					ELSE 'Yes'
				END as InspectionHasGrid,
				CASE 
					WHEN Count(GphIspID) = 0 THEN	'No'
					ELSE 'Yes'
				END as InspectionHasGraphic,
				Count(DstIspID) as InspectionDsets,
				IspNotes as InspectionNotes
				from Inspections left outer join Grids on IspDBid = GrdIspID 
				left outer join Graphics on IspDBid = GphIspID
				left outer join Dsets on IspDBid = DstIspID
				where IspIscID = @p1
				group by IspDBid, IspReportOrder, IspName, IspNotes
				order by IspReportOrder";
			cmd.CommandText = qry;
			cmd.Parameters.Add("@p1", inspectedComponent == null ? Guid.Empty : inspectedComponent);
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
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
			cmd.Parameters.Add(new SqlCeParameter("@p2", IspIscID));
			if (id == null)
			{
				cmd.CommandText = "Select IspDBid from Inspections where IspName = @p1 and IspIscID = @p2";
			}
			else
			{
				cmd.CommandText = "Select IspDbid from Inspections where IspName = @p1 and IspIscID = @p2 and IspDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			return exists;
		}

		private short getNewReportOrder(Guid inspectedComponentID)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", inspectedComponentID));
			cmd.CommandText = "Select Max(IspReportOrder) from Inspections where IspIscID = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = Util.NullForDbNull(cmd.ExecuteScalar());
			short newReportOrder = (short)(val == null ? 0 : Convert.ToUInt16(val) + 1);
			return newReportOrder;
		}

		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (InspectionName == null)
			{
				IspNameErrMsg = "A unique Inspection Name is required";
				allFilled = false;
			}
			else
			{
				IspNameErrMsg = null;
			}
			return allFilled;
		}

		private string GetDefaultInspectionName(Guid ReportID)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			string result = null;

			cmd.CommandText = 
				@"Select Count(IspDBid) from Inspections
				where IspIscID = @p1";
			cmd.Parameters.Add("@p1",ReportID);

			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			if ((int)cmd.ExecuteScalar() == 0)
				result = "Primary Inspection";

			return result;
		}
	}

	//--------------------------------------
	// Inspection Collection class
	//--------------------------------------
	public class EInspectionCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EInspectionCollection()
		{ }
		//the indexer of the collection
		public EInspection this[int index]
		{
			get
			{
				return (EInspection)this.List[index];
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
			foreach (EInspection inspection in InnerList)
			{
				if (inspection.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EInspection item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EInspection item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EInspection item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EInspection item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
