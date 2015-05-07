using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgLine : ChangeFinder
	{
		public ChgLine(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Lines";
			tableName_friendly = "Lines";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					LinDBid as ID,
					LinName as Name,
					LinUntID,
					LinIsLclChg,
					CASE
						WHEN LinIsActive = 1 THEN 'Active'
						WHEN LinIsActive = 0 THEN 'Inactive'
					END as [Status],
					LinIsActive
					from Lines";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					LinDBid as ID,
					LinName as Name,
					LinUntID,
					LinIsLclChg,
					CASE
						WHEN LinIsActive = 1 THEN 'Active'
						WHEN LinIsActive = 0 THEN 'Inactive'
					END as [Status],
					LinIsActive
					from Lines";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Lines set					
					LinName = @p1,					
					LinUntID = @p2,					
					LinIsLclChg = @p3,					
					LinIsActive = @p4
					Where LinDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 40, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "LinUntID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "LinIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "LinIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Lines (					
				LinDBid,					
				LinName,					
				LinUntID,					
				LinIsLclChg,					
				LinIsActive				
				) values (@p0, @p1, @p2, @p3, @p4)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 40, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "LinUntID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "LinIsLclChg");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "LinIsActive");
		}
	}
}
