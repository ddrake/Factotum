using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgCalibrationProcedure : ChangeFinder
	{
		public ChgCalibrationProcedure(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "CalibrationProcedures";
			tableName_friendly = "Calibration Procedures";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					ClpDBid as ID,
					ClpName as Name,
					ClpDescription as [Calibration Proc. Descrip.],
					ClpIsLclChg,
					CASE
						WHEN ClpIsActive = 1 THEN 'Active'
						WHEN ClpIsActive = 0 THEN 'Inactive'
					END as [Status],
					ClpIsActive
					from CalibrationProcedures";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					ClpDBid as ID,
					ClpName as Name,
					ClpDescription as [Calibration Proc. Descrip.],
					ClpIsLclChg,
					CASE
						WHEN ClpIsActive = 1 THEN 'Active'
						WHEN ClpIsActive = 0 THEN 'Inactive'
					END as [Status],
					ClpIsActive
					from CalibrationProcedures";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update CalibrationProcedures set					
					ClpName = @p1,					
					ClpDescription = @p2,					
					ClpIsLclChg = @p3,					
					ClpIsActive = @p4
					Where ClpDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 255, "Calibration Proc. Descrip.");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "ClpIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "ClpIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into CalibrationProcedures (					
				ClpDBid,					
				ClpName,					
				ClpDescription,					
				ClpIsLclChg,					
				ClpIsActive				
				) values (@p0, @p1, @p2, @p3, @p4)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 255, "Calibration Proc. Descrip.");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "ClpIsLclChg");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "ClpIsActive");
		}
	}
}
