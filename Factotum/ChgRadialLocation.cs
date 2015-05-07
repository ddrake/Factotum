using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgRadialLocation : ChangeFinder
	{
		public ChgRadialLocation(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "RadialLocations";
			tableName_friendly = "Radial Locations";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					RdlDBid as ID,
					RdlName as Name,
					RdlIsLclChg,
					RdlUsedInOutage,
					CASE
						WHEN RdlIsActive = 1 THEN 'Active'
						WHEN RdlIsActive = 0 THEN 'Inactive'
					END as [Status],
					RdlIsActive
					from RadialLocations";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					RdlDBid as ID,
					RdlName as Name,
					RdlIsLclChg,
					RdlUsedInOutage,
					CASE
						WHEN RdlIsActive = 1 THEN 'Active'
						WHEN RdlIsActive = 0 THEN 'Inactive'
					END as [Status],
					RdlIsActive
					from RadialLocations";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update RadialLocations set					
					RdlName = @p1,					
					RdlIsLclChg = @p2,					
					RdlUsedInOutage = @p3,					
					RdlIsActive = @p4
					Where RdlDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.Bit, 1, "RdlIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "RdlUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "RdlIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into RadialLocations (					
				RdlDBid,					
				RdlName,					
				RdlIsLclChg,					
				RdlUsedInOutage,					
				RdlIsActive				
				) values (@p0, @p1, @p2, @p3, @p4)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.Bit, 1, "RdlIsLclChg");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "RdlUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "RdlIsActive");
		}
	}
}
