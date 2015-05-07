using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgCouplantType : ChangeFinder
	{
		public ChgCouplantType(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "CouplantTypes";
			tableName_friendly = "Couplant Types";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					CptDBid as ID,
					CptName as Name,
					CptIsLclChg,
					CptUsedInOutage,
					CASE
						WHEN CptIsActive = 1 THEN 'Active'
						WHEN CptIsActive = 0 THEN 'Inactive'
					END as [Status],
					CptIsActive
					from CouplantTypes";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					CptDBid as ID,
					CptName as Name,
					CptIsLclChg,
					CptUsedInOutage,
					CASE
						WHEN CptIsActive = 1 THEN 'Active'
						WHEN CptIsActive = 0 THEN 'Inactive'
					END as [Status],
					CptIsActive
					from CouplantTypes";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update CouplantTypes set					
					CptName = @p1,					
					CptIsLclChg = @p2,					
					CptUsedInOutage = @p3,					
					CptIsActive = @p4
					Where CptDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.Bit, 1, "CptIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "CptUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "CptIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into CouplantTypes (					
				CptDBid,					
				CptName,					
				CptIsLclChg,					
				CptUsedInOutage,					
				CptIsActive				
				) values (@p0, @p1, @p2, @p3, @p4)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.Bit, 1, "CptIsLclChg");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "CptUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "CptIsActive");
		}
	}
}
