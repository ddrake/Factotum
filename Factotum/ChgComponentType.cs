using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgComponentType : ChangeFinder
	{
		public ChgComponentType(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "ComponentTypes";
			tableName_friendly = "Component Types";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					CtpDBid as ID,
					CtpName as Name,
					CtpSitID,
					CtpIsLclChg,
					CASE
						WHEN CtpIsActive = 1 THEN 'Active'
						WHEN CtpIsActive = 0 THEN 'Inactive'
					END as [Status],
					CtpIsActive
					from ComponentTypes";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					CtpDBid as ID,
					CtpName as Name,
					CtpSitID,
					CtpIsLclChg,
					CASE
						WHEN CtpIsActive = 1 THEN 'Active'
						WHEN CtpIsActive = 0 THEN 'Inactive'
					END as [Status],
					CtpIsActive
					from ComponentTypes";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update ComponentTypes set					
					CtpName = @p1,					
					CtpSitID = @p2,					
					CtpIsLclChg = @p3,					
					CtpIsActive = @p4
					Where CtpDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "CtpSitID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "CtpIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "CtpIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into ComponentTypes (					
				CtpDBid,					
				CtpName,					
				CtpSitID,					
				CtpIsLclChg,					
				CtpIsActive				
				) values (@p0, @p1, @p2, @p3, @p4)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "CtpSitID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "CtpIsLclChg");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "CtpIsActive");
		}
	}
}
