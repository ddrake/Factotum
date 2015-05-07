using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgMeterDucer : ChangeFinder
	{
		public ChgMeterDucer(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "MeterDucers";
			tableName_friendly = "Meter Transducers";
			primaryParentTableName = "MeterModels";
			primaryParentTableName_friendly = "Meter Models";
			secondaryParentTableName = "DucerModels";
			secondaryParentTableName_friendly = "Transducer Models";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = 
					@"Select
					MtdMmlID as PrimaryID,
					MtdDmdID as SecondaryID,
					MeterModels.MmlName as PrimaryName,
					DucerModels.DmdName as SecondaryName
					from (MeterDucers
					inner join MeterModels on MeterDucers.MtdMmlID = MeterModels.MmlDBid)
					inner join DucerModels on MeterDucers.MtdDmdID = DucerModels.DmdDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText =
					@"Select
					MtdMmlID as PrimaryID,
					MtdDmdID as SecondaryID,
					MeterModels.MmlName as PrimaryName,
					DucerModels.DmdName as SecondaryName
					from (MeterDucers
					inner join MeterModels on MeterDucers.MtdMmlID = MeterModels.MmlDBid)
					inner join DucerModels on MeterDucers.MtdDmdID = DucerModels.DmdDBid";	

			// No Update Command for Original Table needed

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = 
				@"Insert into MeterDucers (
				MtdMmlID,
				MtdDmdID
				) values (@p0, @p1)";

			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "PrimaryID");
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.UniqueIdentifier, 16, "SecondaryID");
			
			// Delete Command for Original Table
			cmdDelOrig = cnnOrig.CreateCommand();
			cmdDelOrig.CommandType = CommandType.Text;
			cmdDelOrig.CommandText = 
				@"Delete from MeterDucers
				where (MtdMmlID = @p0 and MtdDmdID = @p1)";

			cmdDelOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "PrimaryID");
			cmdDelOrig.Parameters.Add("@p1", SqlDbType.UniqueIdentifier, 16, "SecondaryID");

		}
	}
}
