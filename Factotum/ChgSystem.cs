using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgSystem : ChangeFinder
	{
		public ChgSystem(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Systems";
			tableName_friendly = "Systems";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					SysDBid as ID,
					SysName as Name,
					SysUntID,
					SysIsLclChg,
					CASE
						WHEN SysIsActive = 1 THEN 'Active'
						WHEN SysIsActive = 0 THEN 'Inactive'
					END as [Status],
					SysIsActive
					from Systems";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					SysDBid as ID,
					SysName as Name,
					SysUntID,
					SysIsLclChg,
					CASE
						WHEN SysIsActive = 1 THEN 'Active'
						WHEN SysIsActive = 0 THEN 'Inactive'
					END as [Status],
					SysIsActive
					from Systems";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Systems set					
					SysName = @p1,					
					SysUntID = @p2,					
					SysIsLclChg = @p3,					
					SysIsActive = @p4
					Where SysDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 40, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "SysUntID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "SysIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "SysIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Systems (					
				SysDBid,					
				SysName,					
				SysUntID,					
				SysIsLclChg,					
				SysIsActive				
				) values (@p0, @p1, @p2, @p3, @p4)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 40, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "SysUntID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "SysIsLclChg");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "SysIsActive");
		}
	}
}
