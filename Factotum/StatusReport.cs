using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;


namespace Factotum
{
	public partial class StatusReport : Form
	{
		public string filterReportID = null;
		public string filtercomponentID = null;
		public FilterYesNoAll filterSubmitted = FilterYesNoAll.ShowAll;
		public FilterYesNoAll filterPrepComplete = FilterYesNoAll.ShowAll;
		public FilterYesNoAll filterUtFieldComplete = FilterYesNoAll.ShowAll;
		public FilterYesNoAll filterStatusComplete = FilterYesNoAll.ShowAll;
		public FilterYesNoAll filterFinal = FilterYesNoAll.ShowAll;
		public FilterYesNoAll filterHasMin = FilterYesNoAll.ShowAll;
		public Guid? filterReviewer = null;
		private string locationInfo;

		// These two flags are set in the print handler and used by the InspectedComponent 
		// view form to propt the user to update the CompletionReported flags 
		// for these reports.
		public bool printedUtFieldComplete;
		public bool printedSubmitted;

		public StatusReport()
		{
			InitializeComponent();
		}

		private void FillTempTable()
		{
			SqlCeCommand cmd;
			// First construct REALLY temporary tables with the totals we need.

			// Create the Crew Dose Temp table
			Globals.DeleteTempTableIfExists("tmpTotalCrewDose");
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"create table tmpTotalCrewDose 
				(IscID UniqueIdentifier NOT NULL, 
				TotalCrewDose float NOT NULL)";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Fill the Crew Dose temp table.
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"insert into tmpTotalCrewDose (IscID, TotalCrewDose)
				select 				
				IscDBid, 
				Sum(DstCrewDose)
				from InspectedComponents
				inner join Inspections on IscDBid = IspIscID
				inner join Dsets on IspDBid = DstIspID
				where DstCrewDose is not NULL
				and IscOtgID = @p0
				group by IscDBid";
			cmd.Parameters.Add("@p0", Globals.CurrentOutageID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Create the Person Hours Temp table
			Globals.DeleteTempTableIfExists("tmpTotalPersonHours");
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"create table tmpTotalPersonHours 
				(IscID UniqueIdentifier NOT NULL, 
				TotalPersonHours float NOT NULL)";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Fill the Person Hours temp table.
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"insert into tmpTotalPersonHours (IscID, TotalPersonHours)
				select 				
				IscDBid, 
				Sum(IspPersonHours)
				from InspectedComponents
				inner join Inspections on IscDBid = IspIscID
				where IspPersonHours is not NULL
				and IscOtgID = @p0
				group by IscDBid";
			cmd.Parameters.Add("@p0", Globals.CurrentOutageID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Now we're ready to fill the TmpStatusReport table

			// First clear existing data
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"delete from TmpStatusReport";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Now insert new data
			cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"insert into TmpStatusReport (
				IscName, CmpName, IscIsReadyToInspect
				, IscInsID, IscOtgID, IscMinCount, IscIsFinal
				, IscIsUtFieldComplete, IscReportSubmittedOn, IscCompletionReportedOn
				, IscWorkOrder, IscEdsNumber, TotalCrewDose, TotalPersonHours
				)
				select 				
				IscName, CmpName, IscIsReadyToInspect
				, IscInsID, IscOtgID, IscMinCount, IscIsFinal
				, IscIsUtFieldComplete, IscReportSubmittedOn, IscCompletionReportedOn
				, IscWorkOrder, IscEdsNumber, TotalCrewDose, TotalPersonHours
				from InspectedComponents
				inner join Components on IscCmpID = CmpDBid
				left outer join tmpTotalCrewDose on IscDBid = tmpTotalCrewDose.IscID
				left outer join tmpTotalPersonHours on IscDBid = tmpTotalPersonHours.IscID
				where IscOtgID = @p0";
			cmd.Parameters.Add("@p0", Globals.CurrentOutageID);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();

			// Remove temp tables.
			Globals.DeleteTempTableIfExists("tmpTotalCrewDose");
			Globals.DeleteTempTableIfExists("tmpTotalPersonHours");

		}

		private void StatusReport_Load(object sender, EventArgs e)
		{
			EOutage outage = new EOutage((Guid)Globals.CurrentOutageID);
			EUnit unit = new EUnit(outage.OutageUntID);
			locationInfo = unit.UnitNameWithSite + " - " + outage.OutageName;
			ReportParameter[] pars = { new ReportParameter("Location", locationInfo) };
			rvStatusReport.LocalReport.SetParameters(pars);

			if (this.TmpStatusReportTableAdapter.Connection.State == ConnectionState.Open)
				this.TmpStatusReportTableAdapter.Connection.Close();

			// Fill TmpStatusReport with the data to be used by the report
			FillTempTable();

			this.TmpStatusReportTableAdapter.Connection = Globals.cnn;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			this.TmpStatusReportTableAdapter.Fill(this.StatusReportDataSet.TmpStatusReport);
			updateFilter();
			rvStatusReport.Print +=new ReportPrintEventHandler(rvStatusReport_Print);
			this.rvStatusReport.RefreshReport();
		}

		void  rvStatusReport_Print(object sender, CancelEventArgs e)
		{
			if (btnReportSubmitted.Checked && !btnIncludePrevReported.Checked)
			{
				printedSubmitted = true;
			}
			if (btnUtComplete.Checked && !btnIncludePrevReported.Checked)
			{
				printedUtFieldComplete = true;
			}
		}

		private void btnReportSubmitted_Click(object sender, EventArgs e)
		{
			if (!btnReportSubmitted.Checked)
			{
				btnReportSubmitted.Checked = true;
				btnUtComplete.Checked = false;
				btnCurrentView.Checked = false;
				btnIncludePrevReported.Enabled = true;
				updateFilter();
			}
		}

		private void btnUtComplete_Click(object sender, EventArgs e)
		{
			if (!btnUtComplete.Checked)
			{
				btnReportSubmitted.Checked = false;
				btnUtComplete.Checked = true;
				btnCurrentView.Checked = false;
				btnIncludePrevReported.Enabled = true;
				updateFilter();
			}

		}

		private void btnCurrentView_Click(object sender, EventArgs e)
		{
			if (!btnCurrentView.Checked)
			{
				btnReportSubmitted.Checked = false;
				btnUtComplete.Checked = false;
				btnCurrentView.Checked = true;
				btnIncludePrevReported.Checked = false;
				btnIncludePrevReported.Enabled = false;
				updateFilter();
			}

		}

		private void btnIncludePrevReported_Click(object sender, EventArgs e)
		{
			btnIncludePrevReported.Checked = !btnIncludePrevReported.Checked;
			updateFilter();
		}

		private void updateFilter()
		{
			StringBuilder sb = new StringBuilder("", 255);
			if (btnCurrentView.Checked)
			{
				// Set the filter according to the global filters passed in
				if (filterReportID.Length > 0)
					sb.Append(" And IscName Like '" + filterReportID + "*'");
				if (filtercomponentID.Length > 0)
					sb.Append(" And CmpName Like '" + filtercomponentID + "*'");
				if (filterUtFieldComplete != FilterYesNoAll.ShowAll)
					sb.Append(" And IscIsUtFieldComplete = " +
						(filterUtFieldComplete == FilterYesNoAll.Yes ? "1" : "0"));
				// todo: need to add ready to inspect to the dataset query.
				if (filterPrepComplete != FilterYesNoAll.ShowAll)
					sb.Append(" And IscIsReadyToInspect = " +
						(filterPrepComplete == FilterYesNoAll.Yes ? "1" : "0" ));
				if (filterSubmitted != FilterYesNoAll.ShowAll)
					sb.Append(" And IscReportSubmittedOn" +
						(filterSubmitted == FilterYesNoAll.Yes ? " is not NULL" : " is NULL"));
				if (filterStatusComplete != FilterYesNoAll.ShowAll)
					sb.Append(" And IscCompletionReportedOn" +
						(filterStatusComplete == FilterYesNoAll.Yes ? " is not NULL" : " is NULL"));
				if (filterFinal != FilterYesNoAll.ShowAll)
					sb.Append(" And IscIsFinal = " +
						(filterFinal == FilterYesNoAll.Yes ? "1" : "0"));
				if (filterHasMin != FilterYesNoAll.ShowAll)
					sb.Append(" And IscMinCount " +
						(filterHasMin == FilterYesNoAll.Yes ? " > 0" : " <= 0"));
				if (filterReviewer != null)
					sb.Append(" And IscInsID = '" + filterReviewer + "'");

				//int count = StatusReportDataset.StatusInfo.Rows.Count;
			}

			else
			{
				// Set the filter according to the checked buttons.
				if (btnReportSubmitted.Checked)
					sb.Append(" And IscReportSubmittedOn is not NULL");
				else
					sb.Append(" And IscIsUtFieldComplete <> 0");
				if (!btnIncludePrevReported.Checked)
					sb.Append(" And IscCompletionReportedOn is NULL");
				
			}
			StatusReportDataSet.TmpStatusReport.DefaultView.RowFilter = 
				sb.Length > 5 ? sb.ToString().Substring(5) : "";

			TmpStatusReportBindingSource.DataSource = StatusReportDataSet.TmpStatusReport.DefaultView;

			rvStatusReport.RefreshReport();
		}


	}
}