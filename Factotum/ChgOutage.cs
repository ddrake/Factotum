using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgOutage : ChangeFinder
	{
		public ChgOutage(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Outages";
			tableName_friendly = "Outages";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					OtgDBid as ID,
					OtgName as Name,
					OtgUntID,
					ClpName as [Calibration Procedure],
					OtgClpID,
					CptName as [Couplant Type],
					OtgCptID,
					OtgCouplantBatch as [Couplant Batch],
					OtgFacPhone as [FAC Phone],
					OtgStartedOn as [Start Date],
					OtgEndedOn as [End Date],
					OtgConfigExportedOn as [Exported On],
					OtgDataImportedOn as [Imported On],
					CASE
						WHEN OtgGridColDefaultCCW = 1 THEN 'True'
						WHEN OtgGridColDefaultCCW = 0 THEN 'False'
					END as [Columns CCW by Default],
					OtgGridColDefaultCCW
					from Outages
					left outer join CalibrationProcedures on OtgClpID = ClpDBid
					left outer join CouplantTypes on OtgCptID = CptDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					OtgDBid as ID,
					OtgName as Name,
					OtgUntID,
					ClpName as [Calibration Procedure],
					OtgClpID,
					CptName as [Couplant Type],
					OtgCptID,
					OtgCouplantBatch as [Couplant Batch],
					OtgFacPhone as [FAC Phone],
					OtgStartedOn as [Start Date],
					OtgEndedOn as [End Date],
					OtgConfigExportedOn as [Exported On],
					OtgDataImportedOn as [Imported On],
					CASE
						WHEN OtgGridColDefaultCCW = 1 THEN 'True'
						WHEN OtgGridColDefaultCCW = 0 THEN 'False'
					END as [Columns CCW by Default],
					OtgGridColDefaultCCW
					from Outages
					left outer join CalibrationProcedures on OtgClpID = ClpDBid
					left outer join CouplantTypes on OtgCptID = CptDBid";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Outages set					
					OtgName = @p1,					
					OtgUntID = @p2,					
					OtgClpID = @p3,					
					OtgCptID = @p4,					
					OtgCouplantBatch = @p5,					
					OtgFacPhone = @p6,					
					OtgStartedOn = @p7,					
					OtgEndedOn = @p8,					
					OtgConfigExportedOn = @p9,					
					OtgDataImportedOn = @p10,					
					OtgGridColDefaultCCW = @p11
					Where OtgDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "OtgUntID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "OtgClpID");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.UniqueIdentifier, 16, "OtgCptID");
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.NVarChar, 50, "Couplant Batch");
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.NVarChar, 50, "FAC Phone");
			cmdUpdOrig.Parameters.Add("@p7", SqlDbType.DateTime, 8, "Start Date");
			cmdUpdOrig.Parameters.Add("@p8", SqlDbType.DateTime, 8, "End Date");
			cmdUpdOrig.Parameters.Add("@p9", SqlDbType.DateTime, 8, "Exported On");
			cmdUpdOrig.Parameters.Add("@p10", SqlDbType.DateTime, 8, "Imported On");
			cmdUpdOrig.Parameters.Add("@p11", SqlDbType.Bit, 1, "Columns CCW by Default");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Outages (					
				OtgDBid,					
				OtgName,					
				OtgUntID,					
				OtgClpID,					
				OtgCptID,					
				OtgCouplantBatch,					
				OtgFacPhone,					
				OtgStartedOn,					
				OtgEndedOn,					
				OtgConfigExportedOn,					
				OtgDataImportedOn,					
				OtgGridColDefaultCCW				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "OtgUntID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "OtgClpID");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.UniqueIdentifier, 16, "OtgCptID");
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.NVarChar, 50, "Couplant Batch");
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.NVarChar, 50, "FAC Phone");
			cmdInsOrig.Parameters.Add("@p7", SqlDbType.DateTime, 8, "Start Date");
			cmdInsOrig.Parameters.Add("@p8", SqlDbType.DateTime, 8, "End Date");
			cmdInsOrig.Parameters.Add("@p9", SqlDbType.DateTime, 8, "Exported On");
			cmdInsOrig.Parameters.Add("@p10", SqlDbType.DateTime, 8, "Imported On");
			cmdInsOrig.Parameters.Add("@p11", SqlDbType.Bit, 1, "Columns CCW by Default");
		}
	}
}
