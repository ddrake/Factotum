using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgInspector : ChangeFinder
	{
		public ChgInspector(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Inspectors";
			tableName_friendly = "Inspectors";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					InsDBid as ID,
					InsName as Name,
					InsKitID,
					CASE
						WHEN InsLevel = 1 THEN 'Level I'
						WHEN InsLevel = 2 THEN 'Level II'
						WHEN InsLevel = 3 THEN 'Level III'
					END as [Level],
					InsLevel,
					InsIsLclChg,
					InsUsedInOutage,
					CASE
						WHEN InsIsActive = 1 THEN 'Active'
						WHEN InsIsActive = 0 THEN 'Inactive'
					END as [Status],
					InsIsActive
					from Inspectors";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					InsDBid as ID,
					InsName as Name,
					InsKitID,
					CASE
						WHEN InsLevel = 1 THEN 'Level I'
						WHEN InsLevel = 2 THEN 'Level II'
						WHEN InsLevel = 3 THEN 'Level III'
					END as [Level],
					InsLevel,
					InsIsLclChg,
					InsUsedInOutage,
					CASE
						WHEN InsIsActive = 1 THEN 'Active'
						WHEN InsIsActive = 0 THEN 'Inactive'
					END as [Status],
					InsIsActive
					from Inspectors";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Inspectors set					
					InsName = @p1,					
					InsKitID = @p2,					
					InsLevel = @p3,					
					InsIsLclChg = @p4,					
					InsUsedInOutage = @p5,					
					InsIsActive = @p6
					Where InsDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "InsKitID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.TinyInt, 1, "InsLevel");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "InsIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "InsUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "InsIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Inspectors (					
				InsDBid,					
				InsName,					
				InsKitID,					
				InsLevel,					
				InsIsLclChg,					
				InsUsedInOutage,					
				InsIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "InsKitID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.TinyInt, 1, "InsLevel");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "InsIsLclChg");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "InsUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "InsIsActive");
		}
	}
}
