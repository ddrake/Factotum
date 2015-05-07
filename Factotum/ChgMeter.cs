using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgMeter : ChangeFinder
	{
		public ChgMeter(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Meters";
			tableName_friendly = "Meters";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					MtrDBid as ID,
					MtrSerialNumber as Name,
					MmlName as [Model],
					MtrMmlID,
					MtrKitID,
					MtrCalDueDate as [Due Date],
					MtrUsedInOutage,
					MtrIsLclChg,
					CASE
						WHEN MtrIsActive = 1 THEN 'Active'
						WHEN MtrIsActive = 0 THEN 'Inactive'
					END as [Status],
					MtrIsActive
					from Meters
					inner join MeterModels on MtrMmlID = MmlDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					MtrDBid as ID,
					MtrSerialNumber as Name,
					MmlName as [Model],
					MtrMmlID,
					MtrKitID,
					MtrCalDueDate as [Due Date],
					MtrUsedInOutage,
					MtrIsLclChg,
					CASE
						WHEN MtrIsActive = 1 THEN 'Active'
						WHEN MtrIsActive = 0 THEN 'Inactive'
					END as [Status],
					MtrIsActive
					from Meters
					inner join MeterModels on MtrMmlID = MmlDBid";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Meters set					
					MtrSerialNumber = @p1,					
					MtrMmlID = @p2,					
					MtrKitID = @p3,					
					MtrCalDueDate = @p4,					
					MtrUsedInOutage = @p5,					
					MtrIsLclChg = @p6,					
					MtrIsActive = @p7
					Where MtrDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "MtrMmlID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "MtrKitID");
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.DateTime, 8, "Due Date");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "MtrUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "MtrIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p7", SqlDbType.Bit, 1, "MtrIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Meters (					
				MtrDBid,					
				MtrSerialNumber,					
				MtrMmlID,					
				MtrKitID,					
				MtrCalDueDate,					
				MtrUsedInOutage,					
				MtrIsLclChg,					
				MtrIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "MtrMmlID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "MtrKitID");
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.DateTime, 8, "Due Date");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "MtrUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "MtrIsLclChg");					
			cmdInsOrig.Parameters.Add("@p7", SqlDbType.Bit, 1, "MtrIsActive");
		}
	}
}
