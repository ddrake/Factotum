using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgComponentMaterial : ChangeFinder
	{
		public ChgComponentMaterial(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "ComponentMaterials";
			tableName_friendly = "Component Materials";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					CmtDBid as ID,
					CmtName as Name,
					CmtSitID,
					CASE
						WHEN CmtCalBlockMaterial = 1 THEN 'Carbon Steel'
						WHEN CmtCalBlockMaterial = 2 THEN 'Stainless Steel'
						WHEN CmtCalBlockMaterial = 3 THEN 'Inconel'
						ELSE 'Unspecified'
					END as [Cal Block Material Type],
					CmtCalBlockMaterial,
					CmtIsLclChg,
					CASE
						WHEN CmtIsActive = 1 THEN 'Active'
						WHEN CmtIsActive = 0 THEN 'Inactive'
					END as [Status],
					CmtIsActive
					from ComponentMaterials";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					CmtDBid as ID,
					CmtName as Name,
					CmtSitID,
					CASE
						WHEN CmtCalBlockMaterial = 1 THEN 'Carbon Steel'
						WHEN CmtCalBlockMaterial = 2 THEN 'Stainless Steel'
						WHEN CmtCalBlockMaterial = 3 THEN 'Inconel'
						ELSE 'Unspecified'
					END as [Cal Block Material Type],
					CmtCalBlockMaterial,
					CmtIsLclChg,
					CASE
						WHEN CmtIsActive = 1 THEN 'Active'
						WHEN CmtIsActive = 0 THEN 'Inactive'
					END as [Status],
					CmtIsActive
					from ComponentMaterials";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update ComponentMaterials set					
					CmtName = @p1,					
					CmtSitID = @p2,					
					CmtCalBlockMaterial = @p3,					
					CmtIsLclChg = @p4,					
					CmtIsActive = @p5
					Where CmtDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "CmtSitID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.TinyInt, 1, "CmtCalBlockMaterial");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "CmtIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "CmtIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into ComponentMaterials (					
				CmtDBid,					
				CmtName,					
				CmtSitID,					
				CmtCalBlockMaterial,					
				CmtIsLclChg,					
				CmtIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 25, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "CmtSitID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.TinyInt, 1, "CmtCalBlockMaterial");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Bit, 1, "CmtIsLclChg");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "CmtIsActive");
		}
	}
}
