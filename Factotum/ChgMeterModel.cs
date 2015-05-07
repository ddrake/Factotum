using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgMeterModel : ChangeFinder
	{
		public ChgMeterModel(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "MeterModels";
			tableName_friendly = "Meter Models";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					MmlDBid as ID,
					MmlName as Name,
					MmlManfName as [Manufacturer],
					MmlIsLclChg,
					MmlUsedInOutage,
					CASE
						WHEN MmlIsActive = 1 THEN 'Active'
						WHEN MmlIsActive = 0 THEN 'Inactive'
					END as [Status],
					MmlIsActive
					from MeterModels";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					MmlDBid as ID,
					MmlName as Name,
					MmlManfName as [Manufacturer],
					MmlIsLclChg,
					MmlUsedInOutage,
					CASE
						WHEN MmlIsActive = 1 THEN 'Active'
						WHEN MmlIsActive = 0 THEN 'Inactive'
					END as [Status],
					MmlIsActive
					from MeterModels";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update MeterModels set					
					MmlName = @p1,					
					MmlManfName = @p2,					
					MmlIsLclChg = @p3,	
					MmlUsedInOutage = @p4,
					MmlIsActive = @p5
					Where MmlDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 50, "Manufacturer");
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "MmlIsLclChg");
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "MmlUsedInOutage");
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "MmlIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into MeterModels (					
				MmlDBid,					
				MmlName,					
				MmlManfName,					
				MmlIsLclChg,
				MmlUsedInOutage,					
				MmlIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 50, "Manufacturer");
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "MmlIsLclChg");
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "MmlUsedInOutage");
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "MmlIsActive");
		}
	}
}
