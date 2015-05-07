using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgGridSize : ChangeFinder
	{
		public ChgGridSize(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "GridSizes";
			tableName = "Grid Sizes";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					GszDBid as ID,
					GszName as Name,
					GszAxialDistance as [Axial Distance],
					GszRadialDistance as [Radial Distance],
					GszMaxDiameter as [Max Diameter],
					GszIsLclChg,
					GszUsedInOutage,
					CASE
						WHEN GszIsActive = 1 THEN 'Active'
						WHEN GszIsActive = 0 THEN 'Inactive'
					END as [Status],
					GszIsActive
					from GridSizes";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					GszDBid as ID,
					GszName as Name,
					GszAxialDistance as [Axial Distance],
					GszRadialDistance as [Radial Distance],
					GszMaxDiameter as [Max Diameter],
					GszIsLclChg,
					GszUsedInOutage,
					CASE
						WHEN GszIsActive = 1 THEN 'Active'
						WHEN GszIsActive = 0 THEN 'Inactive'
					END as [Status],
					GszIsActive
					from GridSizes";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update GridSizes set					
					GszName = @p1,					
					GszAxialDistance = @p2,					
					GszRadialDistance = @p3,					
					GszMaxDiameter = @p4,					
					GszIsLclChg = @p5,					
					GszUsedInOutage = @p6,					
					GszIsActive = @p7
					Where GszDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.Decimal, 4, "Axial Distance");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Decimal, 4, "Radial Distance");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Decimal, 6, "Max Diameter");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "GszIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "GszUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p7", SqlDbType.Bit, 1, "GszIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into GridSizes (					
				GszDBid,					
				GszName,					
				GszAxialDistance,					
				GszRadialDistance,					
				GszMaxDiameter,					
				GszIsLclChg,					
				GszUsedInOutage,					
				GszIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.Decimal, 4, "Axial Distance");
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.Decimal, 4, "Radial Distance");
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Decimal, 6, "Max Diameter");
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "GszIsLclChg");
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.Bit, 1, "GszUsedInOutage");
			cmdInsOrig.Parameters.Add("@p7", SqlDbType.Bit, 1, "GszIsActive");
		}
	}
}
