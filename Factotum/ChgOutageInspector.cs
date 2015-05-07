using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgOutageInspector : ChangeFinder
	{
		public ChgOutageInspector(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "OutageInspectors";
			tableName_friendly = "Outage Inspectors";
			primaryParentTableName = "Outages";
			primaryParentTableName_friendly = "Outages";
			secondaryParentTableName = "Inspectors";
			secondaryParentTableName_friendly = "Inspectors";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = 
					@"Select
					OinOtgID as PrimaryID,
					OinInsID as SecondaryID,
					Outages.OtgName as PrimaryName,
					Inspectors.InsName as SecondaryName
					from (OutageInspectors
					inner join Outages on OutageInspectors.OinOtgID = Outages.OtgDBid)
					inner join Inspectors on OutageInspectors.OinInsID = Inspectors.InsDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = 
					@"Select
					OinOtgID as PrimaryID,
					OinInsID as SecondaryID,
					Outages.OtgName as PrimaryName,
					Inspectors.InsName as SecondaryName
					from (OutageInspectors
					inner join Outages on OutageInspectors.OinOtgID = Outages.OtgDBid)
					inner join Inspectors on OutageInspectors.OinInsID = Inspectors.InsDBid";	

			// No Update Command for Original Table needed

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = 
				@"Insert into OutageInspectors (
				OinOtgID,
				OinInsID
				) values (@p0, @p1)";

			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "PrimaryID");
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.UniqueIdentifier, 16, "SecondaryID");
			
			// Delete Command for Original Table
			cmdDelOrig = cnnOrig.CreateCommand();
			cmdDelOrig.CommandType = CommandType.Text;
			cmdDelOrig.CommandText = 
				@"Delete from OutageInspectors
				where (OinOtgID = @p0 and OinInsID = @p1)";

			cmdDelOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "PrimaryID");
			cmdDelOrig.Parameters.Add("@p1", SqlDbType.UniqueIdentifier, 16, "SecondaryID");

		}
	}
}
