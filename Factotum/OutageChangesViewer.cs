using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.Drawing.Printing;

namespace Factotum
{
	public partial class OutageChangesViewer : Form
	{
		private DataSet ds;
		// The DataGridView Control which will be printed.
		SqlCeDataAdapter daUpdates, daAssignmentChanges, daInsertions;

		PrintDocument MyPrintDocument;
		// The class that will do the printing process.

		DataGridViewPrinter MyDataGridViewPrinter;
			
		public OutageChangesViewer()
		{
			InitializeComponent();
		}

		private void OutageChangesViewer_Load(object sender, EventArgs e)
		{
			// Check whether or not the temp tables exist.  If any exist, they all should,
			// so just check for one.
			SqlCeConnection cnn = Globals.cnn;
			SqlCeCommand cmd;
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"SELECT Count(*) 
				FROM Information_schema.tables 
				WHERE table_name = 'tmpOutageUpdates'";
			if (cnn.State != ConnectionState.Open) cnn.Open();
			bool tableExists = ((int)cmd.ExecuteScalar() != 0);
			if (!tableExists)
			{
				MessageBox.Show("No Outages have been imported yet, so there are no changes to display","Factotum: No Changes To Display");
				return;
			}
			ds = new DataSet("OutageMods");
			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"SELECT recordGuid, tableName, tableName_friendly, recordName, 
					fieldName, oldValue, newValue
				FROM tmpOutageUpdates";

			daUpdates = new SqlCeDataAdapter(cmd);
			daUpdates.Fill(ds, "Updates");

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"SELECT primaryTableRecordGuid, primaryTableName, 
					primaryTableName_friendly,	primaryTableRecordName, 
					secondaryTableName_friendly, secondaryTableRecordName, isNowAssigned
				FROM tmpOutageAssignmentChanges";

			daAssignmentChanges = new SqlCeDataAdapter(cmd);
			daAssignmentChanges.Fill(ds, "AssignmentChanges");

			cmd = cnn.CreateCommand();
			cmd.CommandText =
				@"SELECT recordGuid, tableName, tableName_friendly, recordName
				FROM tmpOutageInsertions";

			daInsertions = new SqlCeDataAdapter(cmd);
			daInsertions.Fill(ds, "Insertions");

			this.dgvChanges.RowHeadersVisible = false;
			this.dgvChanges.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgvChanges.AllowUserToResizeRows = false;

			this.dgvAdditions.RowHeadersVisible = false;
			this.dgvAdditions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgvAdditions.AllowUserToResizeRows = false;

			this.dgvReassignments.RowHeadersVisible = false;
			this.dgvReassignments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgvReassignments.AllowUserToResizeRows = false;

			this.dgvChanges.DataSource = ds;
			this.dgvChanges.DataMember = "Updates";
			this.dgvReassignments.DataSource = ds;
			this.dgvReassignments.DataMember = "AssignmentChanges";
			this.dgvAdditions.DataSource = ds;
			this.dgvAdditions.DataMember = "Insertions";

			dgvChanges.Columns["tableName_friendly"].HeaderText = "Table";
			dgvChanges.Columns["recordName"].HeaderText = "Record";
			dgvChanges.Columns["fieldName"].HeaderText = "Changed Item";
			dgvChanges.Columns["oldValue"].HeaderText = "Original Value";
			dgvChanges.Columns["newValue"].HeaderText = "New Value";

			dgvChanges.Columns["recordGuid"].Visible = false;
			dgvChanges.Columns["tableName"].Visible = false;

			dgvReassignments.Columns["primaryTableName_friendly"].HeaderText = "Main Table";
			dgvReassignments.Columns["primaryTableRecordName"].HeaderText = "Main Item";
			dgvReassignments.Columns["secondaryTableName_friendly"].HeaderText = "Related Table";
			dgvReassignments.Columns["secondaryTableRecordName"].HeaderText = "Related Item";
			dgvReassignments.Columns["isNowAssigned"].HeaderText = "Currently Assigned";

			dgvReassignments.Columns["primaryTableRecordGuid"].Visible = false;
			dgvReassignments.Columns["primaryTableName"].Visible = false;

			dgvAdditions.Columns["tableName_friendly"].HeaderText = "Table";
			dgvAdditions.Columns["recordName"].HeaderText = "Record";

			dgvAdditions.Columns["recordGuid"].Visible = false;
			dgvAdditions.Columns["tableName"].Visible = false;

			dgvChanges.CellDoubleClick += new DataGridViewCellEventHandler(dgv_CellDoubleClick);
			dgvReassignments.CellDoubleClick += new DataGridViewCellEventHandler(dgv_CellDoubleClick);
			dgvAdditions.CellDoubleClick += new DataGridViewCellEventHandler(dgv_CellDoubleClick);
		}


		private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView dgv = (DataGridView)sender;
			Guid? ID = (Guid)dgv.Rows[e.RowIndex].Cells[0].Value;
			string tableName = (string)dgv.Rows[e.RowIndex].Cells[1].Value;
			switch (tableName)
			{
				case "CalBlocks":

					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "CalBlockEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						CalBlockEdit frm = new CalBlockEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "CalibrationProcedures":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "CalibrationProcedureEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						CalibrationProcedureEdit frm = new CalibrationProcedureEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Components":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "ComponentEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						ComponentEdit frm = new ComponentEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "ComponentMaterials":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "MaterialTypeEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						MaterialTypeEdit frm = new MaterialTypeEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "ComponentTypes":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "ComponentTypeEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						ComponentTypeEdit frm = new ComponentTypeEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "CouplantTypes":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "CouplantTypeEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						CouplantTypeEdit frm = new CouplantTypeEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Ducers":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "DucerEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						DucerEdit frm = new DucerEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "DucerModels":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "DucerModelEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						DucerModelEdit frm = new DucerModelEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "GridProcedures":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "GridProcedureEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						GridProcedureEdit frm = new GridProcedureEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "GridSizes":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "GridSizeEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						GridSizeEdit frm = new GridSizeEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Inspectors":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "InspectorEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						InspectorEdit frm = new InspectorEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Lines":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "LineEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						LineEdit frm = new LineEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Meters":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "MeterEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						MeterEdit frm = new MeterEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "MeterModels":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "MeterModelEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						MeterModelEdit frm = new MeterModelEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Outages":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "OutageEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						OutageEdit frm = new OutageEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "PipeScheduleLookup":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "PipeScheduleEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						PipeScheduleEdit frm = new PipeScheduleEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "RadialLocations":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "RadialLocationEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						RadialLocationEdit frm = new RadialLocationEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "SpecialCalParams":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "SpecialCalParamEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						SpecialCalParamEdit frm = new SpecialCalParamEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Systems":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "SystemEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						SystemEdit frm = new SystemEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				case "Thermos":
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "ThermoEdit", ID))
					{
						// Open the edit form with the currently selected ID.
						ThermoEdit frm = new ThermoEdit(ID);
						frm.MdiParent = this.MdiParent;
						frm.Show();
					}
					break;

				default:
					break;
			}
		}


		// The printing setup function

		private bool SetupThePrinting()
		{
			DataGridView dgvToPrint;
			string title;
			PrintDialog MyPrintDialog = new PrintDialog();

			MyPrintDialog.AllowCurrentPage = false;
			MyPrintDialog.AllowPrintToFile = false;
			MyPrintDialog.AllowSelection = false;
			MyPrintDialog.AllowSomePages = false;
			MyPrintDialog.PrintToFile = false;
			MyPrintDialog.ShowHelp = false;
			MyPrintDialog.ShowNetwork = false;

			if (MyPrintDialog.ShowDialog() != DialogResult.OK)
				return false;

			if (tabControl1.SelectedTab == tabChanges)
			{
				dgvToPrint = dgvChanges;
				title = "Configuration Changes from Last Imported Outage";
			}
			else if (tabControl1.SelectedTab == tabReassignments)
			{
				dgvToPrint = dgvReassignments;
				title = "Reassignments from Last Imported Outage";
			}
			else
			{
				dgvToPrint = dgvAdditions;
				title = "Additions from Last Imported Outage";
			}

			MyPrintDocument = new PrintDocument();

			MyPrintDocument.DocumentName = "Outage Changes Report";
			MyPrintDocument.PrinterSettings =
									  MyPrintDialog.PrinterSettings;
			MyPrintDocument.DefaultPageSettings =
			MyPrintDialog.PrinterSettings.DefaultPageSettings;
			MyPrintDocument.DefaultPageSettings.Margins =
								  new Margins(40, 40, 40, 40);

			MyPrintDocument.DefaultPageSettings.Landscape = (dgvToPrint == dgvChanges);
			MyPrintDocument.PrintPage +=new PrintPageEventHandler(MyPrintDocument_PrintPage);

			MyDataGridViewPrinter = new DataGridViewPrinter(dgvToPrint,
			MyPrintDocument, true, true, title, new Font("Tahoma", 18,
			FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);

			return true;
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			if (SetupThePrinting())
			{
				PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
				MyPrintPreviewDialog.Document = MyPrintDocument;
				MyPrintPreviewDialog.ShowDialog();
			}
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			if (SetupThePrinting())
				MyPrintDocument.Print();
		}

		private void MyPrintDocument_PrintPage(object sender, 
			System.Drawing.Printing.PrintPageEventArgs e)
		{
			bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
			if (more == true)
				e.HasMorePages = true;
		}


		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}