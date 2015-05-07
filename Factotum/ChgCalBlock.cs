using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgCalBlock : ChangeFinder
	{
		public ChgCalBlock(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "CalBlocks";
			tableName_friendly = "Calibration Blocks";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					CbkDBid as ID,
					CbkSerialNumber as Name,
					CbkCalMin as [Min. Cal Thickness],
					CbkCalMax as [Max. Cal Thickness],
					CbkKitID,
					CASE
						WHEN CbkMaterialType = 1 THEN 'Carbon Steel'
						WHEN CbkMaterialType = 2 THEN 'Stainless Steel'
						WHEN CbkMaterialType = 3 THEN 'Inconel'
						ELSE 'Unspecified'
					END as [Cal Block Material Type],
					CbkMaterialType,
					CASE
						WHEN CbkType = 1 THEN 'Step Block'
						WHEN CbkType = 2 THEN 'IIW2 Block'
						ELSE 'Unspecified'
					END as [Cal Block Type],
					CbkType,
					CbkUsedInOutage,
					CbkIsLclChg,
					CASE
						WHEN CbkIsActive = 1 THEN 'Active'
						WHEN CbkIsActive = 0 THEN 'Inactive'
					END as [Status],
					CbkIsActive
					from CalBlocks";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					CbkDBid as ID,
					CbkSerialNumber as Name,
					CbkCalMin as [Min. Cal Thickness],
					CbkCalMax as [Max. Cal Thickness],
					CbkKitID,
					CASE
						WHEN CbkMaterialType = 1 THEN 'Carbon Steel'
						WHEN CbkMaterialType = 2 THEN 'Stainless Steel'
						WHEN CbkMaterialType = 3 THEN 'Inconel'
						ELSE 'Unspecified'
					END as [Cal Block Material Type],
					CbkMaterialType,
					CASE
						WHEN CbkType = 1 THEN 'Step Block'
						WHEN CbkType = 2 THEN 'IIW2 Block'
						ELSE 'Unspecified'
					END as [Cal Block Type],
					CbkType,
					CbkUsedInOutage,
					CbkIsLclChg,
					CASE
						WHEN CbkIsActive = 1 THEN 'Active'
						WHEN CbkIsActive = 0 THEN 'Inactive'
					END as [Status],
					CbkIsActive
					from CalBlocks";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update CalBlocks set					
					CbkSerialNumber = @p1,					
					CbkCalMin = @p2,					
					CbkCalMax = @p3,					
					CbkKitID = @p4,					
					CbkMaterialType = @p5,					
					CbkType = @p6,					
					CbkUsedInOutage = @p7,					
					CbkIsLclChg = @p8,					
					CbkIsActive = @p9
					Where CbkDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.Decimal, 5, "Min. Cal Thickness");
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Decimal, 5, "Max. Cal Thickness");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.UniqueIdentifier, 16, "CbkKitID");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.TinyInt, 1, "CbkMaterialType");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.TinyInt, 1, "CbkType");					
			cmdUpdOrig.Parameters.Add("@p7", SqlDbType.Bit, 1, "CbkUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p8", SqlDbType.Bit, 1, "CbkIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p9", SqlDbType.Bit, 1, "CbkIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into CalBlocks (					
				CbkDBid,					
				CbkSerialNumber,					
				CbkCalMin,					
				CbkCalMax,					
				CbkKitID,					
				CbkMaterialType,					
				CbkType,					
				CbkUsedInOutage,					
				CbkIsLclChg,					
				CbkIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.Decimal, 5, "Min. Cal Thickness");
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Decimal, 5, "Max. Cal Thickness");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.UniqueIdentifier, 16, "CbkKitID");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.TinyInt, 1, "CbkMaterialType");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.TinyInt, 1, "CbkType");					
			cmdInsOrig.Parameters.Add("@p7", SqlDbType.Bit, 1, "CbkUsedInOutage");					
			cmdInsOrig.Parameters.Add("@p8", SqlDbType.Bit, 1, "CbkIsLclChg");					
			cmdInsOrig.Parameters.Add("@p9", SqlDbType.Bit, 1, "CbkIsActive");
		}
	}
}
