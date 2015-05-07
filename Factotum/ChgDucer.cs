using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgDucer : ChangeFinder
	{
		public ChgDucer(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Ducers";
			tableName_friendly = "Transducers";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					DcrDBid as ID,
					DcrSerialNumber as Name,
					DmdName as [Model],
					DcrDmdID,
					DcrKitID,
					DcrUsedInOutage,
					DcrIsLclChg,
					CASE
						WHEN DcrIsActive = 1 THEN 'Active'
						WHEN DcrIsActive = 0 THEN 'Inactive'
					END as [Status],
					DcrIsActive
					from Ducers
					inner join DucerModels on DcrDmdID = DmdDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					DcrDBid as ID,
					DcrSerialNumber as Name,
					DmdName as [Model],
					DcrDmdID,
					DcrKitID,
					DcrUsedInOutage,
					DcrIsLclChg,
					CASE
						WHEN DcrIsActive = 1 THEN 'Active'
						WHEN DcrIsActive = 0 THEN 'Inactive'
					END as [Status],
					DcrIsActive
					from Ducers
					inner join DucerModels on DcrDmdID = DmdDBid";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Ducers set					
					DcrSerialNumber = @p1,					
					DcrDmdID = @p2,					
					DcrKitID = @p3,					
					DcrUsedInOutage = @p4,					
					DcrIsLclChg = @p5,					
					DcrIsActive = @p6
					Where DcrDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "DcrDmdID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "DcrKitID");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "DcrUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "DcrIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "DcrIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Ducers (					
				DcrDBid,					
				DcrSerialNumber,					
				DcrDmdID,					
				DcrKitID,					
				DcrUsedInOutage,					
				DcrIsLclChg,					
				DcrIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "DcrDmdID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "DcrKitID");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "DcrUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "DcrIsLclChg");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "DcrIsActive");
		}
	}
}
