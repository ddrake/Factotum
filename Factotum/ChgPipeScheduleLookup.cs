using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgPipeScheduleLookup : ChangeFinder
	{
		public ChgPipeScheduleLookup(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "PipeScheduleLookup";
			tableName_friendly = "Pipe Schedule Lookup";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					PslDBid as ID,
					'OD: ' + STR(PslOd,6,3) + ', Tnom: ' + STR(PslNomWall,5,3) as Name,
					PslOd as [OD],
					PslNomWall as [Tnom],
					PslSchedule as [Schedule],
					PslNomDia as [Nom Dia],
					CASE
						WHEN PslIsLclChg = 1 THEN 'Active'
						WHEN PslIsLclChg = 0 THEN 'Inactive'
					END as [Status],
					PslIsLclChg
					from PipeScheduleLookup";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					PslDBid as ID,
					'OD: ' + STR(PslOd,6,3) + ', Tnom: ' + STR(PslNomWall,5,3) as Name,
					PslOd as [OD],
					PslNomWall as [Tnom],
					PslSchedule as [Schedule],
					PslNomDia as [Nom Dia],
					CASE
						WHEN PslIsLclChg = 1 THEN 'Active'
						WHEN PslIsLclChg = 0 THEN 'Inactive'
					END as [Status],
					PslIsLclChg
					from PipeScheduleLookup";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update PipeScheduleLookup set					
					PslOd = @p1,					
					PslNomWall = @p2,					
					PslSchedule = @p3,					
					PslNomDia = @p4,					
					PslIsLclChg = @p5
					Where PslDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.Decimal, 6, "OD");
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.Decimal, 5, "Tnom");
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.NVarChar, 20, "Schedule");
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.Decimal, 5, "Nom Dia");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "PslIsLclChg");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into PipeScheduleLookup (					
				PslDBid,					
				PslOd,					
				PslNomWall,					
				PslSchedule,					
				PslNomDia,					
				PslIsLclChg				
				) values (@p0, @p1, @p2, @p3, @p4, @p5)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.Decimal, 6, "OD");
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.Decimal, 5, "Tnom");
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.NVarChar, 20, "Schedule");
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.Decimal, 5, "Nom Dia");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.Bit, 1, "PslIsLclChg");
		}
	}
}
