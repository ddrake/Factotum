using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgThermo : ChangeFinder
	{
		public ChgThermo(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Thermos";
			tableName_friendly = "Thermometers";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					ThmDBid as ID,
					ThmSerialNumber as Name,
					ThmKitID,
					ThmUsedInOutage,
					ThmIsLclChg,
					CASE
						WHEN ThmIsActive = 1 THEN 'Active'
						WHEN ThmIsActive = 0 THEN 'Inactive'
					END as [Status],
					ThmIsActive
					from Thermos";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					ThmDBid as ID,
					ThmSerialNumber as Name,
					ThmKitID,
					ThmUsedInOutage,
					ThmIsLclChg,
					CASE
						WHEN ThmIsActive = 1 THEN 'Active'
						WHEN ThmIsActive = 0 THEN 'Inactive'
					END as [Status],
					ThmIsActive
					from Thermos";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Thermos set					
					ThmSerialNumber = @p1,					
					ThmKitID = @p2,					
					ThmUsedInOutage = @p3,					
					ThmIsLclChg = @p4,					
					ThmIsActive = @p5
					Where ThmDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "ThmKitID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "ThmUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "ThmIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "ThmIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Thermos (					
				ThmDBid,					
				ThmSerialNumber,					
				ThmKitID,					
				ThmUsedInOutage,					
				ThmIsLclChg,					
				ThmIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "ThmKitID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "ThmUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "ThmIsLclChg");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "ThmIsActive");
		}
	}
}
