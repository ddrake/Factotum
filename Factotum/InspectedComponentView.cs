using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Factotum
{
	public partial class InspectedComponentView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		
		EOutage curOutage;
		EInspectorCollection reviewers;
		// Form constructor
		public InspectedComponentView(Guid outageID)
		{
			curOutage = new EOutage(outageID);
			InitializeComponent();

			// Take care of settings that are not as easily managed in the designer.
			InitializeControls();
		}

		// Take care of the non-default DataGridView settings that are not as easily managed
		// in the designer.
		private void InitializeControls()
		{
			// Set these combos to their defaults
			cboHasMin.SelectedIndex = (int)FilterYesNoAll.ShowAll;
			cboSubmitted.SelectedIndex = (int)FilterYesNoAll.ShowAll;
			cboFinal.SelectedIndex = (int)FilterYesNoAll.ShowAll;
			cboUtFieldCplt.SelectedIndex = (int)FilterYesNoAll.ShowAll;
			cboPrepComplete.SelectedIndex = (int)FilterYesNoAll.ShowAll;
			cboStatusComplete.SelectedIndex = (int)FilterYesNoAll.ShowAll;

			reviewers = EInspector.ListForOutage((Guid)curOutage.ID, false, true);
			reviewers[0].InspectorName = "All";
			cboReviewer.DataSource = reviewers;
			cboReviewer.DisplayMember = "InspectorName";
			cboReviewer.ValueMember = "ID";
			cboReviewer.SelectedIndex = 0;

			btnAdd.Enabled = Globals.ActivationOK;
			btnDelete.Enabled = Globals.ActivationOK;
		}

		private void UpdateReviewersComboSource()
		{
			Guid? curReviewerID = null;
			if (cboReviewer.SelectedValue != null)
				curReviewerID = (Guid)cboReviewer.SelectedValue;

			reviewers = EInspector.ListForOutage((Guid)curOutage.ID, false, true);
			reviewers[0].InspectorName = "All";
			cboReviewer.DataSource = reviewers;
			cboReviewer.DisplayMember = "InspectorName";
			cboReviewer.ValueMember = "ID";

			if (curReviewerID != null)
				cboReviewer.SelectedValue = curReviewerID;
			else
				cboReviewer.SelectedIndex = 0;

		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void InspectedComponentView_Load(object sender, EventArgs e)
		{
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			// We don't want these to get triggered except by the user
			this.cboHasMin.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);
			this.txtComponentID.TextChanged += new System.EventHandler(HandleApplyFilters);
			this.txtReportID.TextChanged += new System.EventHandler(HandleApplyFilters);
			this.cboUtFieldCplt.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);
			this.cboReviewer.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);
			this.cboSubmitted.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);
			this.cboFinal.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);
			this.cboPrepComplete.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);
			this.cboStatusComplete.SelectedIndexChanged += new System.EventHandler(HandleApplyFilters);

			EInspectedComponent.Changed += new EventHandler<EntityChangedEventArgs>(EInspectedComponent_Changed);
			EInspector.InspectorOutageAssignmentsChanged += new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
		}

		private void InspectedComponentView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EInspectedComponent.Changed -= new EventHandler<EntityChangedEventArgs>(EInspectedComponent_Changed);
			EInspector.InspectorOutageAssignmentsChanged -= new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
		}
		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		void EInspector_InspectorOutageAssignmentsChanged(object sender, EventArgs e)
		{
			UpdateReviewersComboSource();		
		}

		void EInspectedComponent_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvReportList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvReportList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "InspectedComponentEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				InspectedComponentEdit frm = new InspectedComponentEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		private void btnValidate_Click(object sender, EventArgs e)
		{
            ShowValidator();
		}

        private void ShowValidator()
        {
            Form frm;
            // Make sure there's a row selected
            if (dgvReportList.SelectedRows.Count != 1) return;
            Guid? currentEditItem = (Guid?)(dgvReportList.SelectedRows[0].Cells["ID"].Value);
            // First check to see if an instance of the form set to the selected ID already exists
            if (!Globals.CanActivateForm(this, "ReportValidator", currentEditItem, out frm))
            {
                // Open the form with the currently selected ID.
                frm = new ReportValidator((Guid)currentEditItem);
                frm.MdiParent = this.MdiParent;
                frm.Show();
            }
            else
            {
                ((ReportValidator)frm).DoValidation();
            }
        }
		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvReportList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}


		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			InspectedComponentEdit frm = new InspectedComponentEdit(null, curOutage.ID);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvReportList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Block to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvReportList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "InspectedComponentEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EInspectedComponent InspectedComponent = new EInspectedComponent(currentEditItem);
			InspectedComponent.Delete(true);
			if (InspectedComponent.InspComponentErrMsg != null)
			{
				MessageBox.Show(InspectedComponent.InspComponentErrMsg, "Factotum");
				InspectedComponent.InspComponentErrMsg = null;
			}
		}

		private void HandleApplyFilters(object sender, EventArgs e)
		{
			ApplyFilters();
		}

        private void ShowStatusReport()
        {
            StatusReport frm;
            frm = new StatusReport();
            frm.filterReportID = txtReportID.Text;
            frm.filtercomponentID = txtComponentID.Text;
            frm.filterSubmitted = (FilterYesNoAll)cboSubmitted.SelectedIndex;
            frm.filterPrepComplete = (FilterYesNoAll)cboPrepComplete.SelectedIndex;
            frm.filterUtFieldComplete = (FilterYesNoAll)cboUtFieldCplt.SelectedIndex;
            frm.filterStatusComplete = (FilterYesNoAll)cboStatusComplete.SelectedIndex;
            frm.filterFinal = (FilterYesNoAll)cboFinal.SelectedIndex;
            frm.filterHasMin = (FilterYesNoAll)cboHasMin.SelectedIndex;
            if (cboReviewer.SelectedValue == null) frm.filterReviewer = null;
            else frm.filterReviewer = (Guid?)cboReviewer.SelectedValue;

            frm.ShowDialog();

            bool updateForUtFieldComplete = false;
            bool updateForSubmitted = false;
            DialogResult test;

            if (frm.printedUtFieldComplete)
            {
                test = MessageBox.Show("Update the 'Completion Reported' status of 'UT field complete' reports?",
                    "Factotum: Update 'Completion Reported' status?", MessageBoxButtons.YesNo);
                updateForUtFieldComplete = test == DialogResult.Yes;

            }
            if (frm.printedSubmitted)
            {
                test = MessageBox.Show("Update the 'Completion Reported' status of 'Submitted' reports?",
                    "Factotum: Update 'Completion Reported' status?", MessageBoxButtons.YesNo);
                updateForSubmitted = test == DialogResult.Yes;
            }
            if (updateForUtFieldComplete || updateForSubmitted)
            {
                EInspectedComponent.UpdateCompletionReported(updateForUtFieldComplete, updateForSubmitted);
            }
        }

		private void btnStatusRept_Click(object sender, EventArgs e)
		{
            ShowStatusReport();
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
            PreviewReport();
		}

        private void PreviewReport()
        {
            if (dgvReportList.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a Report to print first.", "Factotum");
                return;
            }
            EInspectedComponent inspComponent = new EInspectedComponent((Guid?)dgvReportList.SelectedRows[0].Cells["ID"].Value);
            MainReport curReport = new MainReport((Guid)inspComponent.ID);
            curReport.Print(false);
        }
        private void PrintReport()
        {
            if (dgvReportList.SelectedRows.Count != 1)
            {
                MessageBox.Show("Please select a Report to print first.", "Factotum");
                return;
            }
            EInspectedComponent inspComponent = new EInspectedComponent((Guid?)dgvReportList.SelectedRows[0].Cells["ID"].Value);
            MainReport curReport = new MainReport((Guid)inspComponent.ID);
            curReport.Print(true);
        }
		private void btnPrint_Click(object sender, EventArgs e)
		{
            PrintReport();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}


		// ----------------------------------------------------------------------
		// Private utilities
		// ----------------------------------------------------------------------		
		// Update the tool selector combo box by filling its items based on current data and filters.
		// Then set the currently displayed item to that of the supplied ID.
		// If the supplied ID isn't on the list because of the current filter state, just show the
		// first item if there is one.
		private void UpdateSelector(Guid? id)
		{
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvReportList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvReportList.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EInspectedComponent.GetDefaultDataViewForOutage(curOutage.ID);
			dgvReportList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvReportList.Sort(dgvReportList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvReportList.Sort(dgvReportList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);

		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvReportList.Sort(dgvReportList.Columns["InspComponentEdsNumber"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvReportList.Columns["InspComponentEdsNumber"].HeaderText = "EDS#";
			dgvReportList.Columns["InspComponentName"].HeaderText = "Report ID";
			dgvReportList.Columns["InspComponentComponentName"].HeaderText = "Component ID";
			dgvReportList.Columns["InspComponentWorkOrder"].HeaderText = "Work Order";
			dgvReportList.Columns["InspNumberOfInspections"].HeaderText = "Inspections";
			dgvReportList.Columns["InspComponentInspectorName"].HeaderText = "Reviewer";
			dgvReportList.Columns["InspComponentGridProcedureName"].HeaderText = "Grid Proc.";
			dgvReportList.Columns["InspComponentIsReadyToInspect"].HeaderText = "Prep Cplt.";
			dgvReportList.Columns["InspComponentIsUtFieldComplete"].HeaderText = "UT Field Cplt.";
			dgvReportList.Columns["InspComponentMinCount"].HeaderText = "Mins";
			dgvReportList.Columns["InspComponentIsFinal"].HeaderText = "Final";
			dgvReportList.Columns["InspComponentReportSubmittedOn"].HeaderText = "Submitted";
			dgvReportList.Columns["InspComponentCompletionReportedOn"].HeaderText = "Status Complete";
			dgvReportList.Columns["InspComponentAreaSpecifier"].HeaderText = "Spec. Area";
			dgvReportList.Columns["InspComponentPageCountOverride"].HeaderText = "Page Count Override";
			// Hide some columns
			dgvReportList.Columns["ID"].Visible = false;
			dgvReportList.Columns["InspComponentInsID"].Visible = false;
			dgvReportList.Columns["InspComponentPageCountOverride"].Visible = false;
			dgvReportList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvReportList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("", 255);
			if (txtReportID.Text.Length > 0)
				sb.Append(" And InspComponentName Like '" + txtReportID.Text + "*'");
			if (txtComponentID.Text.Length > 0)
				sb.Append(" And InspComponentComponentName Like '" + txtComponentID.Text + "*'");
			if (cboUtFieldCplt.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And InspComponentIsUtFieldComplete = " +
					(cboUtFieldCplt.SelectedIndex == (int)FilterYesNoAll.Yes ? "'Yes'" : "'No'"));
			if (cboPrepComplete.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And InspComponentIsReadyToInspect = " +
					(cboPrepComplete.SelectedIndex == (int)FilterYesNoAll.Yes ? "'Yes'" : "'No'"));
			if (cboSubmitted.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And InspComponentReportSubmittedOn" +
					(cboSubmitted.SelectedIndex == (int)FilterYesNoAll.Yes ? " is not NULL" : " is NULL"));
			if (cboStatusComplete.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And InspComponentCompletionReportedOn" +
					(cboStatusComplete.SelectedIndex == (int)FilterYesNoAll.Yes ? " is not NULL" : " is NULL"));
			if (cboFinal.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And InspComponentIsFinal = " +
					(cboFinal.SelectedIndex == (int)FilterYesNoAll.Yes ? "'Yes'" : "'No'"));
			if (cboHasMin.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And InspComponentMinCount " +
					(cboHasMin.SelectedIndex == (int)FilterYesNoAll.Yes ? " > 0" : " <= 0"));
			if ((Guid?)cboReviewer.SelectedValue != null)
				sb.Append(" And InspComponentInsID = '" + cboReviewer.SelectedValue + "'");

			DataView dv = (DataView)dgvReportList.DataSource;
			dv.RowFilter = sb.Length > 5 ? sb.ToString().Substring(5) : "";

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvReportList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvReportList.FirstDisplayedCell;
			if (id != null && firstCell != null )
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvReportList.Rows[r].Cells["ID"].Value == id)
					{
						dgvReportList.CurrentCell = dgvReportList[firstCell.ColumnIndex, r];
						dgvReportList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvReportList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvReportList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else if (firstCell != null)
			{
				// Select the first item
				dgvReportList.CurrentCell = firstCell;
				dgvReportList.Rows[0].Selected = true;
			}
		}

		private void InspectedComponentView_SizeChanged(object sender, EventArgs e)
		{
			// There seems to be a bug where this column becomes visible when the window is restored
			// after minimizing, then editing a report.
			if (dgvReportList != null && WindowState != FormWindowState.Minimized)
				dgvReportList.Columns["ID"].Visible = false;
		}


        private void validateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowValidator();
            dgvReportList.ContextMenuStrip = null;
        }

        private void componentReportPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewReport();
        }

        private void printComponentReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintReport();
        }

        private void editReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCurrentSelection();
        }
        private void dgvReportList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView grid = sender as DataGridView;
                Point p;
                p = grid.PointToClient(Control.MousePosition);
                dgvReportList.Rows[e.RowIndex].Selected = true;
                contextMenuStrip1.Show(dgvReportList, p.X,p.Y);
            }
        }
	}
}