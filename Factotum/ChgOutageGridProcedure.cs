using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgOutageGridProcedure : ChangeFinder
	{
		public ChgOutageGridProcedure(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "OutageGridProcedures";
			tableName_friendly = "Outage Grid Procedures";
			primaryParentTableName = "Outages";
			primaryParentTableName_friendly = "Outages";
			secondaryParentTableName = "GridProcedures";
			secondaryParentTableName_friendly = "Grid Procedures";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = 
					@"Select
					OgpOtgID as PrimaryID,
					OgpGrpID as SecondaryID,
					Outages.OtgName as PrimaryName,
					GridProcedures.GrpName as SecondaryName
					from (OutageGridProcedures
					inner join Outages on OutageGridProcedures.OgpOtgID = Outages.OtgDBid)
					inner join GridProcedures on OutageGridProcedures.OgpGrpID = GridProcedures.GrpDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText =
					@"Select
					OgpOtgID as PrimaryID,
					OgpGrpID as SecondaryID,
					Outages.OtgName as PrimaryName,
					GridProcedures.GrpName as SecondaryName
					from (OutageGridProcedures
					inner join Outages on OutageGridProcedures.OgpOtgID = Outages.OtgDBid)
					inner join GridProcedures on OutageGridProcedures.OgpGrpID = GridProcedures.GrpDBid";	

			// No Update Command for Original Table needed

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = 
				@"Insert into OutageGridProcedures (
				OgpOtgID,
				OgpGrpID
				) values (@p0, @p1)";

			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "PrimaryID");
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.UniqueIdentifier, 16, "SecondaryID");
			
			// Delete Command for Original Table
			cmdDelOrig = cnnOrig.CreateCommand();
			cmdDelOrig.CommandType = CommandType.Text;
			cmdDelOrig.CommandText = 
				@"Delete from OutageGridProcedures
				where (OgpOtgID = @p0 and OgpGrpID = @p1)";

			cmdDelOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "PrimaryID");
			cmdDelOrig.Parameters.Add("@p1", SqlDbType.UniqueIdentifier, 16, "SecondaryID");

		}
	}
}
