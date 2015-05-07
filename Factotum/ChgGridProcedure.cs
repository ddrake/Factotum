using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgGridProcedure : ChangeFinder
	{
		public ChgGridProcedure(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "GridProcedures";
			tableName_friendly = "Grid Procedures";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					GrpDBid as ID,
					GrpName as Name,
					GrpDescription as [Description],
					GrpDsDiameters as [D/S Diameters],
					GrpIsLclChg,
					GrpUsedInOutage,
					CASE
						WHEN GrpIsActive = 1 THEN 'Active'
						WHEN GrpIsActive = 0 THEN 'Inactive'
					END as [Status],
					GrpIsActive
					from GridProcedures";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					GrpDBid as ID,
					GrpName as Name,
					GrpDescription as [Description],
					GrpDsDiameters as [D/S Diameters],
					GrpIsLclChg,
					GrpUsedInOutage,
					CASE
						WHEN GrpIsActive = 1 THEN 'Active'
						WHEN GrpIsActive = 0 THEN 'Inactive'
					END as [Status],
					GrpIsActive
					from GridProcedures";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update GridProcedures set					
					GrpName = @p1,					
					GrpDescription = @p2,					
					GrpDsDiameters = @p3,					
					GrpIsLclChg = @p4,					
					GrpUsedInOutage = @p5,					
					GrpIsActive = @p6
					Where GrpDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 255, "Description");
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.SmallInt, 2, "D/S Diameters");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "GrpIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "GrpUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "GrpIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into GridProcedures (					
				GrpDBid,					
				GrpName,					
				GrpDescription,					
				GrpDsDiameters,					
				GrpIsLclChg,					
				GrpUsedInOutage,					
				GrpIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 255, "Description");
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.SmallInt, 2, "D/S Diameters");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "GrpIsLclChg");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "GrpUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "GrpIsActive");
		}
	}
}
