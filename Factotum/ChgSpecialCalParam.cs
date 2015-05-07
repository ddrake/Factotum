using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgSpecialCalParam : ChangeFinder
	{
		public ChgSpecialCalParam(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "SpecialCalParams";
			tableName_friendly = "Special Calibration Parameters";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					ScpDBid as ID,
					ScpName as Name,
					ScpUnits as [Units],
					ScpReportOrder as [Report Order],
					ScpUsedInOutage,
					ScpIsLclChg,
					CASE
						WHEN ScpIsActive = 1 THEN 'Active'
						WHEN ScpIsActive = 0 THEN 'Inactive'
					END as [Status],
					ScpIsActive
					from SpecialCalParams";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					ScpDBid as ID,
					ScpName as Name,
					ScpUnits as [Units],
					ScpReportOrder as [Report Order],
					ScpUsedInOutage,
					ScpIsLclChg,
					CASE
						WHEN ScpIsActive = 1 THEN 'Active'
						WHEN ScpIsActive = 0 THEN 'Inactive'
					END as [Status],
					ScpIsActive
					from SpecialCalParams";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update SpecialCalParams set					
					ScpName = @p1,					
					ScpUnits = @p2,					
					ScpReportOrder = @p3,					
					ScpUsedInOutage = @p4,					
					ScpIsLclChg = @p5,					
					ScpIsActive = @p6
					Where ScpDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 15, "Units");
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.SmallInt, 2, "Report Order");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "ScpUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "ScpIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "ScpIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into SpecialCalParams (					
				ScpDBid,					
				ScpName,					
				ScpUnits,					
				ScpReportOrder,					
				ScpUsedInOutage,					
				ScpIsLclChg,					
				ScpIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 15, "Units");
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.SmallInt, 2, "Report Order");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "ScpUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "ScpIsLclChg");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "ScpIsActive");
		}
	}
}
