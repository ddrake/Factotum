using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgDucerModel : ChangeFinder
	{
		public ChgDucerModel(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "DucerModels";
			tableName_friendly = "Transducer Models";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					DmdDBid as ID,
					DmdName as Name,
					DmdFrequency as [Frequency],
					DmdSize as [Size],
					DmdIsLclChg,
					DmdUsedInOutage,
					CASE
						WHEN DmdIsActive = 1 THEN 'Active'
						WHEN DmdIsActive = 0 THEN 'Inactive'
					END as [Status],
					DmdIsActive
					from DucerModels";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					DmdDBid as ID,
					DmdName as Name,
					DmdFrequency as [Frequency],
					DmdSize as [Size],
					DmdIsLclChg,
					DmdUsedInOutage,
					CASE
						WHEN DmdIsActive = 1 THEN 'Active'
						WHEN DmdIsActive = 0 THEN 'Inactive'
					END as [Status],
					DmdIsActive
					from DucerModels";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update DucerModels set					
					DmdName = @p1,					
					DmdFrequency = @p2,					
					DmdSize = @p3,					
					DmdIsLclChg = @p4,	
					DmdUsedInOutage = @p5,				
					DmdIsActive = @p6
					Where DmdDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.Decimal, 4, "Frequency");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Decimal, 4, "Size");
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "DmdIsLclChg");
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "DmdUsedInOutage");
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "DmdIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into DucerModels (					
				DmdDBid,					
				DmdName,					
				DmdFrequency,					
				DmdSize,					
				DmdIsLclChg,
				DmdUsedInOutage,					
				DmdIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.Decimal, 4, "Frequency");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Decimal, 4, "Size");
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "DmdIsLclChg");
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "DmdUsedInOutage");
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "DmdIsActive");
		}
	}
}
