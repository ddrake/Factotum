using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgUnit : ChangeFinder
	{
        public ChgUnit(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)
            : base(cnnOrig, cnnMod)
		{
            tableName = "Units";
            tableName_friendly = "Units";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					UntDBid as ID,
					UntName as Name,
					UntSitID,
					CASE
						WHEN UntIsActive = 1 THEN 'Active'
						WHEN UntIsActive = 0 THEN 'Inactive'
					END as [Status],
					UntIsActive
					from Units";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
            cmdSelOrig.CommandText = @"Select
					UntDBid as ID,
					UntName as Name,
					UntSitID,
					CASE
						WHEN UntIsActive = 1 THEN 'Active'
						WHEN UntIsActive = 0 THEN 'Inactive'
					END as [Status],
					UntIsActive
					from Units";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
            cmdUpdOrig.CommandText = @"Update Units set					
					UntName = @p1,					
					UntSitID = @p2,					
					UntIsActive = @p3
					Where UntDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
            cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "UntSitID");
            cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "UntIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
            cmdInsOrig.CommandText = @"Insert into Units (					
				UntDBid,					
				UntName,					
				UntSitID,					
				UntIsActive				
				) values (@p0, @p1, @p2, @p3)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");
            cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
            cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "UntSitID");
            cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "UntIsActive");
		}
	}
}
