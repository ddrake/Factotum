using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;

namespace Factotum
{
	class OutageExporter
	{
		public static void Amputate(string path, Guid? OutageID, string activationKey)
		{
			if (path == null || OutageID == null) return;
			string cnnString = Globals.ConnectionStringForPath(path);
			EOutage outage = new EOutage(OutageID);
			EUnit unit = new EUnit(outage.OutageUntID);
			ESite site = new ESite(unit.UnitSitID);
			// Set the dbtype to Master

			SqlCeConnection cnn = new SqlCeConnection(cnnString);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			SqlCeCommand cmd;

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Outages where OtgDBid != @p0";
			cmd.Parameters.Add("@p0", outage.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Components where CmpUntID != @p0";
			cmd.Parameters.Add("@p0", unit.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Systems where SysUntID != @p0";
			cmd.Parameters.Add("@p0", unit.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Lines where LinUntID != @p0";
			cmd.Parameters.Add("@p0", unit.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Units where UntDBid != @p0";
			cmd.Parameters.Add("@p0", unit.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from ComponentTypes where CtpSitID != @p0";
			cmd.Parameters.Add("@p0", site.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from ComponentMaterials where CmtSitID != @p0";
			cmd.Parameters.Add("@p0", site.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Sites where SitDBid != @p0";
			cmd.Parameters.Add("@p0", site.ID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			cmd = cnn.CreateCommand();
			cmd.CommandText = "Delete from Customers where CstDBid != @p0";
			cmd.Parameters.Add("@p0", site.SiteCstID);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();

			// The database version will just be copied with the database, but
			// we need to copy the key.  
			// Also clear the master db flag to make it an outage type database.
			cmd = cnn.CreateCommand();
			cmd.CommandText = "Update Globals set SiteActivationKey = @p0, IsMasterDB = 0";
			cmd.Parameters.Add("@p0", activationKey);
			if (cnn.State != ConnectionState.Open) cnn.Open();
			cmd.ExecuteNonQuery();


			Globals.OnDatabaseChanged();
			cnn.Close();
		}
	}
}
