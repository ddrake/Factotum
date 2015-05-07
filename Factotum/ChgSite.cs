using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgSite : ChangeFinder
	{
		public ChgSite(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Sites";
			tableName_friendly = "Sites";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					SitDBid as ID,
					SitName as Name,
					SitCstID,
					SitClpID,
					SitFullName,
					CASE
						WHEN SitIsActive = 1 THEN 'Active'
						WHEN SitIsActive = 0 THEN 'Inactive'
					END as [Status],
					SitIsActive
					from Sites";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
            cmdSelOrig.CommandText = @"Select
					SitDBid as ID,
					SitName as Name,
					SitCstID,
					SitClpID,
					SitFullName,
					CASE
						WHEN SitIsActive = 1 THEN 'Active'
						WHEN SitIsActive = 0 THEN 'Inactive'
					END as [Status],
					SitIsActive
					from Sites";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Sites set					
					SitName = @p1,					
					SitCstID = @p2,					
					SitClpID = @p3,	
					SitFullName = @p4,
					SitIsActive = @p5
					Where SitDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
            cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "SitCstID");
            cmdUpdOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "SitClpID");
            cmdUpdOrig.Parameters.Add("@p4", SqlDbType.NVarChar, 100, "SitFullName");
            cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "SitIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Sites (					
				SitDBid,					
				SitName,					
				SitCstID,					
				SitClpID,
				SitFullName,					
				SitIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");
            cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
            cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "SitCstID");
            cmdInsOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "SitClpID");
            cmdInsOrig.Parameters.Add("@p4", SqlDbType.NVarChar, 100, "SitFullName");
            cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "SitIsActive");
		}
	}
}
