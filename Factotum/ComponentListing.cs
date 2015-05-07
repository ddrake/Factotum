using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data.SqlServerCe;

namespace Factotum
{
	public partial class ComponentListing : Form
	{
		private string locationInfo;
		private EUnitCollection units;

		public ComponentListing()
		{
			InitializeComponent();
		}

		private void ComponentListing_Load(object sender, EventArgs e)
		{
			units = EUnit.ListByName(false, false);
			if (units.Count == 0)
			{
				MessageBox.Show("Can't generate a Component Listing until at least one Unit has been created", "Factotum");
				return;
			}
			cboUnit.DataSource = units;
			cboUnit.DisplayMember = "UnitNameWithSite";
			cboUnit.ValueMember = "ID";
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			// First clear out the temp table
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"delete from TmpComponentListing";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Now insert the data we need
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"INSERT INTO TmpComponentListing 
				(CmpName, CtpName, CmtName, LinName, SysName, PslSchedule, PslNomDia, 
				CmpTimesInspected, CmpAvgInspectionTime, CmpAvgCrewDose, CmpHighRad, 
				CmpHardToAccess, CmpNote)
				SELECT CmpName, CtpName, CmtName, LinName, SysName, PslSchedule, PslNomDia, 
				CmpTimesInspected, CmpAvgInspectionTime, CmpAvgCrewDose, CmpHighRad, CmpHardToAccess, CmpNote 
				from Components 
				left outer join ComponentTypes on CmpCtpID = CtpDBid 
				left outer join ComponentMaterials on CmpCmtID = CmtDBid
				left outer join Lines on CmpLinID = LinDBid
				left outer join Systems on CmpSysID = SysDBid
				left outer join PipeScheduleLookup on CmpPslID = PslDBid
				where CmpUntID = @p1";
			cmd.Parameters.Add("@p1", cboUnit.SelectedValue);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			locationInfo = ((EUnit)cboUnit.SelectedItem).UnitNameWithSite;
			ReportParameter[] pars = { new ReportParameter("Location", locationInfo) };

			rvComponentListing.LocalReport.SetParameters(pars);
			this.TmpComponentListingTableAdapter.Connection = Globals.cnn;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			this.TmpComponentListingTableAdapter.Fill(this.ComponentListingDataSet.TmpComponentListing);
			this.rvComponentListing.RefreshReport();

		}
	}
}