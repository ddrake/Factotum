using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DowUtils;

namespace Factotum
{
	public partial class InspectedComponentEdit : Form, IEntityEditForm
	{
		private EInspectedComponent curInspComponent;
		private EOutage curOutage;
		private bool savedInternally;
		private EComponentComboItemCollection _components;
		private EInspectorCollection inspectors;
		private EGridProcedureCollection gridProcedures;
		private bool newRecord;
		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curInspComponent; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide component and outage parentIDs
		public InspectedComponentEdit(Guid? ID)
			: this(ID, null){}

		public InspectedComponentEdit(Guid? ID, Guid? outageID)
		{
			InitializeComponent();
			curInspComponent = new EInspectedComponent();
			curInspComponent.Load(ID);
			if (outageID != null) curInspComponent.InspComponentOtgID = outageID;
			newRecord = (ID == null);
			InitializeControls();
			savedInternally = false;
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			curOutage = new EOutage(curInspComponent.InspComponentOtgID);

			// Components combo box
			//DataTable components =
			//   EComponent.GetComboBoxViewForUnit((Guid)curOutage.OutageUntID, true, !newRecord, true);

			_components = EComponentComboItem.ListForUnitByName((Guid)curOutage.ID, !newRecord, !newRecord, true);

			cboComponent.DataSource = _components;
			cboComponent.DisplayMember = "ComponentName";
			cboComponent.ValueMember = "ID";

			// Reviewers combo box
			inspectors = EInspector.ListForOutage((Guid)curOutage.ID, !newRecord, true);

			cboReviewer.DataSource = inspectors;
			cboReviewer.DisplayMember = "InspectorName";
			cboReviewer.ValueMember = "ID";

			// Grid Procedures combo box
			gridProcedures = EGridProcedure.ListForOutage((Guid)curOutage.ID, !newRecord, true);

			cboGridProcedure.DataSource = gridProcedures;
			cboGridProcedure.DisplayMember = "GridProcedureName";
			cboGridProcedure.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Component Report" : "Edit Component Report";

			dgvReportSections.RowHeadersVisible = false;
			dgvReportSections.AllowUserToResizeRows = false;
			dgvReportSections.AllowUserToAddRows = false;
			dgvReportSections.AllowUserToDeleteRows = false;

			dgvReportSections.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvReportSections.MultiSelect = false;
			dgvReportSections.AllowUserToOrderColumns = true;
			dgvReportSections.ReadOnly = true;
			this.ckReportSubmitted.CheckedChanged += new System.EventHandler(this.ckReportSubmitted_CheckedChanged);
			this.ckCompletionReported.CheckedChanged += new System.EventHandler(this.ckCompletionReported_CheckedChanged);
		}

		// Note: this is a little tricky, since this event will be raised by this form itself 
		// whenever it saves.  However in that case the MinCounts should always match
		void EInspectedComponent_Changed(object sender, EntityChangedEventArgs e)
		{
			if (e.ID == curInspComponent.ID)
			{
				EInspectedComponent tempReport = new EInspectedComponent(e.ID);
				if (tempReport.InspComponentMinCount != curInspComponent.InspComponentMinCount)
				{
					curInspComponent.InspComponentMinCount = tempReport.InspComponentMinCount;
					lblMinCount.Text = curInspComponent.InspComponentMinCount > 0 ?
						curInspComponent.InspComponentMinCount + " pts. below Tscreen" : "";
				}
			}
		}

		private void InspectedComponentEdit_Load(object sender, EventArgs e)
		{
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			this.cboComponent.SelectedIndexChanged += new System.EventHandler(this.cboComponent_SelectedIndexChanged);
			EInspection.Changed +=new EventHandler<EntityChangedEventArgs>(EInspection_Changed);
			EComponent.Changed += new EventHandler<EntityChangedEventArgs>(EComponent_Changed);
			EInspector.Changed += new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EGridProcedure.Changed += new EventHandler<EntityChangedEventArgs>(EGridProcedure_Changed);
			EInspector.InspectorOutageAssignmentsChanged += new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
			EGridProcedure.GridProcedureOutageAssignmentsChanged += new EventHandler(EGridProcedure_GridProcedureOutageAssignmentsChanged);
			EInspectedComponent.Changed += new EventHandler<EntityChangedEventArgs>(EInspectedComponent_Changed);
		}
		private void InspectedComponentEdit_FormClosed(object sender, FormClosedEventArgs e)
		{
			EInspection.Changed -= new EventHandler<EntityChangedEventArgs>(EInspection_Changed);
			EComponent.Changed -= new EventHandler<EntityChangedEventArgs>(EComponent_Changed);
			EInspector.Changed -= new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EGridProcedure.Changed -= new EventHandler<EntityChangedEventArgs>(EGridProcedure_Changed);
			EInspector.InspectorOutageAssignmentsChanged -= new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
			EGridProcedure.GridProcedureOutageAssignmentsChanged -= new EventHandler(EGridProcedure_GridProcedureOutageAssignmentsChanged);
			EInspectedComponent.Changed -= new EventHandler<EntityChangedEventArgs>(EInspectedComponent_Changed);
		}



		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------
		// If the entity providing a combo source changed, update the combo
		// source, while preserving the selection.
		void EInspection_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		void EComponent_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboComponent.SelectedValue;
			_components = EComponentComboItem.ListForUnitByName((Guid)curOutage.ID, !newRecord, !newRecord, true);
			cboComponent.DataSource = _components;
			if (currentValue == null)
				cboComponent.SelectedIndex = 0;
			else
				cboComponent.SelectedValue = currentValue;
		}

		void EInspector_Changed(object sender, EntityChangedEventArgs e)
		{
			updateReviewerCombo();
		}

		void EInspector_InspectorOutageAssignmentsChanged(object sender, EventArgs e)
		{
			updateReviewerCombo();
		}

		void updateReviewerCombo()
		{
			Guid? currentValue = (Guid?)cboReviewer.SelectedValue;
			inspectors = EInspector.ListForOutage((Guid)curOutage.ID, !newRecord, true);
			cboReviewer.DataSource = inspectors;
			if (currentValue == null)
				cboReviewer.SelectedIndex = 0;
			else
				cboReviewer.SelectedValue = currentValue;
		}

		void EGridProcedure_Changed(object sender, EntityChangedEventArgs e)
		{
			updateGridProcedureCombo();
		}
		void EGridProcedure_GridProcedureOutageAssignmentsChanged(object sender, EventArgs e)
		{
			updateGridProcedureCombo();
		}
		void updateGridProcedureCombo()
		{
			Guid? currentValue = (Guid?)cboGridProcedure.SelectedValue;
			gridProcedures = EGridProcedure.ListForOutage((Guid)curOutage.ID, !newRecord, true);
			cboGridProcedure.DataSource = gridProcedures;
			if (currentValue == null)
				cboGridProcedure.SelectedIndex = 0;
			else
				cboGridProcedure.SelectedValue = currentValue;
		}

		// If the user cancels out, just close.
		// Any changes the user made to the report will not be saved. 
		// However, any changes made to the report sections will be!!!
		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
			DialogResult = savedInternally ? DialogResult.OK : DialogResult.Cancel;
		}

		// If the user clicks OK, first validate and set the error text 
		// for any controls with invalid values.
		// If it validates, try to save.
		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveAndClose();
		}

		private bool performSilentSave()
		{
			// we need to do a 'silent save'
			if (AnyControlErrors())
			{
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curInspComponent.Valid())
			{
				setAllErrors();
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curInspComponent.Save();
			if (tmpID == null) return false;
			savedInternally = true;
			return true;
		}

		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvReportSections.SelectedRows.Count != 1) return;
			if (!performSilentSave()) return;
			Guid? currentEditItem = (Guid?)(dgvReportSections.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "InspectionEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				InspectionEdit frm = new InspectionEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}
		private void btnEdit_Click(object sender, EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvReportSections_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (!performSilentSave()) return;
			InspectionEdit frm = new InspectionEdit(null, curInspComponent.ID);
			frm.MdiParent = this.MdiParent;
			frm.Show();

		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvReportSections.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Report Section to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvReportSections.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "InspectionEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EInspection inspection = new EInspection(currentEditItem);
			inspection.Delete(true);
			if (inspection.InspectionErrMsg != null)
			{
				MessageBox.Show(inspection.InspectionErrMsg, "Factotum");
				inspection.InspectionErrMsg = null;
			}
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			if (dgvReportSections.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a report section to move up first.", "Factotum");
				return;
			}
			if (dgvReportSections.SelectedRows[0].Index == 0)
			{
				MessageBox.Show("Can't move that report section up.  It's already first in the report.", "Factotum");
				return;
			}
			int selIdx = dgvReportSections.SelectedRows[0].Index;
			EInspection source = new EInspection((Guid?)dgvReportSections.Rows[selIdx].Cells["ID"].Value);
			EInspection dest = new EInspection((Guid?)dgvReportSections.Rows[selIdx - 1].Cells["ID"].Value);
			RenumberInspections(source, dest);
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			if (dgvReportSections.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a report section to move down first.", "Factotum");
				return;
			}
			int selIdx = dgvReportSections.SelectedRows[0].Index;
			if (selIdx >= dgvReportSections.Rows.Count - 1)
			{
				MessageBox.Show("Can't move that report section down.  It's already last in the report.", "Factotum");
				return;
			}
			EInspection source = new EInspection((Guid?)dgvReportSections.Rows[selIdx].Cells["ID"].Value);
			EInspection dest = new EInspection((Guid?)dgvReportSections.Rows[selIdx + 1].Cells["ID"].Value);
			RenumberInspections(source, dest);
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtReportID_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curInspComponent.InspComponentNameLengthOk(txtReportID.Text);
			errorProvider1.SetError(txtReportID, curInspComponent.InspComponentNameErrMsg);
		}

		private void txtPageCountOverride_TextChanged(object sender, EventArgs e)
		{
			curInspComponent.InspComponentPageCountOverrideLengthOk(txtPageCountOverride.Text);
			errorProvider1.SetError(txtPageCountOverride, curInspComponent.InspComponentPageCountOverrideErrMsg);
		}

		private void txtWorkOrder_TextChanged(object sender, EventArgs e)
		{
			curInspComponent.InspComponentWorkOrderLengthOk(txtWorkOrder.Text);
			errorProvider1.SetError(txtWorkOrder, curInspComponent.InspComponentWorkOrderErrMsg);
		}

		private void txtSpecificArea_TextChanged(object sender, EventArgs e)
		{
			curInspComponent.InspComponentAreaSpecifierLengthOk(txtSpecificArea.Text);
			errorProvider1.SetError(txtSpecificArea, curInspComponent.InspComponentAreaSpecifierErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtReportID_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curInspComponent.InspComponentNameValid(txtReportID.Text, (Guid)curOutage.ID);
			errorProvider1.SetError(txtReportID, curInspComponent.InspComponentNameErrMsg);
		}

		private void txtWorkOrder_Validating(object sender, CancelEventArgs e)
		{
			curInspComponent.InspComponentWorkOrderValid(txtWorkOrder.Text);
			errorProvider1.SetError(txtWorkOrder, curInspComponent.InspComponentWorkOrderErrMsg);
		}

		private void txtSpecificArea_Validating(object sender, CancelEventArgs e)
		{
			curInspComponent.InspComponentAreaSpecifierValid(txtSpecificArea.Text);
			errorProvider1.SetError(txtSpecificArea, curInspComponent.InspComponentAreaSpecifierErrMsg);
		}

		private void txtPageCountOverride_Validating(object sender, CancelEventArgs e)
		{
			curInspComponent.InspComponentPageCountOverrideValid(txtPageCountOverride.Text);
			errorProvider1.SetError(txtPageCountOverride, curInspComponent.InspComponentPageCountOverrideErrMsg);
		}
		private void cboComponent_SelectedIndexChanged(object sender, EventArgs e)
		{
			curInspComponent.InspComponentCmpIDValid((Guid?)cboComponent.SelectedValue);
			errorProvider1.SetError(cboComponent, curInspComponent.InspComponentCmpIDErrMsg);
		}
		
		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------

		private void UpdateSelector(Guid? id)
		{
			// Save the sort specs if there are any, so we can re-apply them
			//SortOrder sortOrder = dgvReportSections.SortOrder;
			//int sortCol = -1;
			//if (sortOrder != SortOrder.None)
			//   sortCol = dgvReportSections.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EInspection.GetDefaultDataViewForInspectedComponent(curInspComponent.ID);
			dgvReportSections.DataSource = dv;
			// Re-apply the sort specs
			//if (sortOrder == SortOrder.Ascending)
			//   dgvReportSections.Sort(dgvReportSections.Columns[sortCol], ListSortDirection.Ascending);
			//else if (sortOrder == SortOrder.Descending)
			//   dgvReportSections.Sort(dgvReportSections.Columns[sortCol], ListSortDirection.Descending);

			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			//dgvReportSections.Sort(dgvReportSections.Columns["InspectionName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvReportSections.Columns["InspectionName"].HeaderText = "Inspection Name";
			dgvReportSections.Columns["InspectionHasGrid"].HeaderText = "Has Grid";
			dgvReportSections.Columns["InspectionHasGraphic"].HeaderText = "Has Graphic";
			dgvReportSections.Columns["InspectionDsets"].HeaderText = "Datasets";
			dgvReportSections.Columns["InspectionNotes"].HeaderText = "Notes";
			// Hide some columns
			dgvReportSections.Columns["ID"].Visible = false;
			dgvReportSections.Columns["InspectionReportOrder"].Visible = false;
			dgvReportSections.Columns["InspectionNotes"].Visible = false;
			dgvReportSections.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvReportSections.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvReportSections.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvReportSections.Rows[r].Cells["ID"].Value == id)
					{
						dgvReportSections.CurrentCell = dgvReportSections[firstCell.ColumnIndex, r];
						dgvReportSections.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvReportSections.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvReportSections.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvReportSections.CurrentCell = firstCell;
				dgvReportSections.Rows[0].Selected = true;
			}
		}


		// No prompting is performed.  The user should understand the meanings of OK and Cancel.
		private void SaveAndClose()
		{
			if (AnyControlErrors()) return;
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curInspComponent.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curInspComponent.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the inspected component object values.
		private void SetControlValues()
		{
			if (curOutage.OutageUntID != null)
			{
				EUnit unt = new EUnit(curOutage.OutageUntID);
				lblSiteName.Text = "Report for Outage: '" + unt.UnitNameWithSite + 
					" - " + curOutage.OutageName + "'";
			}
			else lblSiteName.Text = "Unknown Outage";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			txtReportID.Text = curInspComponent.InspComponentName;
			txtPageCountOverride.Text = curInspComponent.InspComponentPageCountOverride.ToString();
			txtSpecificArea.Text = curInspComponent.InspComponentAreaSpecifier;
			txtWorkOrder.Text = curInspComponent.InspComponentWorkOrder;
			ckFinal.Checked = curInspComponent.InspComponentIsFinal;
			ckPrepComplete.Checked = curInspComponent.InspComponentIsReadyToInspect;
			ckUtFieldComplete.Checked = curInspComponent.InspComponentIsUtFieldComplete;
			if (curInspComponent.InspComponentCmpID != null) cboComponent.SelectedValue = curInspComponent.InspComponentCmpID;
			if (curInspComponent.InspComponentInsID != null) cboReviewer.SelectedValue = curInspComponent.InspComponentInsID;
			if (curInspComponent.InspComponentGrpID != null) cboGridProcedure.SelectedValue = curInspComponent.InspComponentGrpID;
			lblMinCount.Text = curInspComponent.InspComponentMinCount > 0 ?
				curInspComponent.InspComponentMinCount + " pts. below Tscreen" : "";
			ckCompletionReported.Checked = curInspComponent.InspComponentCompletionReportedOn != null;
			ckReportSubmitted.Checked = curInspComponent.InspComponentReportSubmittedOn != null;
			lblCompletionReportedOn.Text = formatDateLabel(curInspComponent.InspComponentCompletionReportedOn);
			lblReportSubmittedOn.Text = formatDateLabel(curInspComponent.InspComponentReportSubmittedOn);
		}

		private string formatDateLabel(DateTime? stamp)
		{
			if (stamp == null) return null;
			else return ((DateTime)stamp).ToString("MM/dd/yy HH:mm");
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtPageCountOverride, curInspComponent.InspComponentPageCountOverrideErrMsg);
			errorProvider1.SetError(txtReportID, curInspComponent.InspComponentNameErrMsg);
			errorProvider1.SetError(txtSpecificArea, curInspComponent.InspComponentAreaSpecifierErrMsg);
			errorProvider1.SetError(txtWorkOrder, curInspComponent.InspComponentWorkOrderErrMsg);
			errorProvider1.SetError(cboComponent, curInspComponent.InspComponentCmpIDErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtPageCountOverride).Length > 0 ||
				errorProvider1.GetError(txtReportID).Length > 0 ||
				errorProvider1.GetError(txtSpecificArea).Length > 0 ||
				errorProvider1.GetError(txtWorkOrder).Length > 0 ||
				errorProvider1.GetError(cboComponent).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curInspComponent.InspComponentName = txtReportID.Text;
			curInspComponent.InspComponentCmpID = (Guid?)cboComponent.SelectedValue;
			curInspComponent.InspComponentInsID = (Guid?)cboReviewer.SelectedValue;
			curInspComponent.InspComponentGrpID = (Guid?)cboGridProcedure.SelectedValue;
			curInspComponent.InspComponentAreaSpecifier = txtSpecificArea.Text;
			// Note: don't need to do anything with EDS number
			// Todo: how should we handle this.. probably monitor the click event to decide if 
			// it can really be finalized...
			curInspComponent.InspComponentIsFinal = ckFinal.Checked;
			curInspComponent.InspComponentIsReadyToInspect = ckPrepComplete.Checked;
			curInspComponent.InspComponentIsUtFieldComplete = ckUtFieldComplete.Checked;
			curInspComponent.InspComponentPageCountOverride = Util.GetNullableShortForString(txtPageCountOverride.Text);
			curInspComponent.InspComponentWorkOrder = txtWorkOrder.Text;
			if (ckReportSubmitted.Checked)
				curInspComponent.InspComponentReportSubmittedOn = DateTime.Parse(lblReportSubmittedOn.Text);
			else
				curInspComponent.InspComponentReportSubmittedOn = null;

			if (ckCompletionReported.Checked)
				curInspComponent.InspComponentCompletionReportedOn = DateTime.Parse(lblCompletionReportedOn.Text);
			else
				curInspComponent.InspComponentCompletionReportedOn = null;
		}

		private void RenumberInspections(EInspection source, EInspection dest)
		{
			short tmpReportOrder = (short)source.InspectionReportOrder;
			source.InspectionReportOrder = dest.InspectionReportOrder;
			dest.InspectionReportOrder = tmpReportOrder;
			source.Save();
			dest.Save();
		}

		private void dgvReportSections_DoubleClick(object sender, EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvReportSections_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		private void ckReportSubmitted_CheckedChanged(object sender, EventArgs e)
		{
			lblReportSubmittedOn.Text = 
				(ckReportSubmitted.Checked ? formatDateLabel(DateTime.Now) : "");
			
		}

		private void ckCompletionReported_CheckedChanged(object sender, EventArgs e)
		{
			lblCompletionReportedOn.Text =
				(ckCompletionReported.Checked ? formatDateLabel(DateTime.Now) : "");
		}

	}
}