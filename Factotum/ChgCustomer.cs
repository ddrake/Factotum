using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgCustomer : ChangeFinder
	{
        public ChgCustomer(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)
            : base(cnnOrig, cnnMod)
		{
            tableName = "Customers";
            tableName_friendly = "Customers";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					CstDBid as ID,
					CstName as Name,
					CstFullName,
					CASE
						WHEN CstIsActive = 1 THEN 'Active'
						WHEN CstIsActive = 0 THEN 'Inactive'
					END as [Status],
					CstIsActive
					from Customers";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
            cmdSelOrig.CommandText = @"Select
					CstDBid as ID,
					CstName as Name,
					CstFullName,
					CASE
						WHEN CstIsActive = 1 THEN 'Active'
						WHEN CstIsActive = 0 THEN 'Inactive'
					END as [Status],
					CstIsActive
					from Customers";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
            cmdUpdOrig.CommandText = @"Update Customers set					
					CstName = @p1,					
					CstFullName = @p2,
					CstIsActive = @p3
					Where CstDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
            cmdUpdOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 100, "CstFullName");
            cmdUpdOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "CstIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
            cmdInsOrig.CommandText = @"Insert into Customers (					
				CstDBid,					
				CstName,					
				CstFullName,					
				CstIsActive				
				) values (@p0, @p1, @p2, @p3)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 20, "Name");
            cmdInsOrig.Parameters.Add("@p2", SqlDbType.NVarChar, 100, "CstFullName");
            cmdInsOrig.Parameters.Add("@p3", SqlDbType.Bit, 1, "CstIsActive");
		}
	}
}
