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
	public partial class InspectionEdit : Form, IEntityEditForm
	{
		private EInspection curInspection;
		private bool savedInternally;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curInspection; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide component and outage parentIDs
		public InspectionEdit(Guid? ID)
			: this(ID, null){}

		public InspectionEdit(Guid? ID, Guid? inspectedComponentID)
		{
			InitializeComponent();
			curInspection = new EInspection();
			curInspection.Load(ID);
			if (inspectedComponentID != null) curInspection.InspectionIscID = inspectedComponentID;
			InitializeControls(ID == null);
			savedInternally = false;
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New Inspection" : "Edit Inspection";
			this.btnAddDset.Enabled = Globals.ActivationOK;
			this.btnDeleteDset.Enabled = Globals.ActivationOK;
			this.btnMoveDown.Enabled = Globals.ActivationOK;
			this.btnMoveUp.Enabled = Globals.ActivationOK;
			this.btnOK.Enabled = Globals.ActivationOK;

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
			ManageButtons();
			EDset.Changed += new EventHandler<EntityChangedEventArgs>(EDset_Changed);
			EGrid.Changed += new EventHandler<EntityChangedEventArgs>(EGrid_Changed);
			EGraphic.Changed += new EventHandler<EntityChangedEventArgs>(EGraphic_Changed);
		}
		private void InspectionEdit_FormClosed(object sender, FormClosedEventArgs e)
		{
			EDset.Changed -= new EventHandler<EntityChangedEventArgs>(EDset_Changed);
			EGrid.Changed -= new EventHandler<EntityChangedEventArgs>(EGrid_Changed);
			EGraphic.Changed -= new EventHandler<EntityChangedEventArgs>(EGraphic_Changed);
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		void EDset_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
			ManageButtons();
		}

		void EGraphic_Changed(object sender, EntityChangedEventArgs e)
		{
			ManageButtons();
		}

		void EGrid_Changed(object sender, EntityChangedEventArgs e)
		{
			ManageButtons();
		}

		// If the user cancels out, just close.
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

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvDatasets.SelectedRows.Count != 1) return;
			if (Globals.ActivationOK)
			{
				if (!performSilentSave()) return;
			}
			Guid? currentEditItem = (Guid?)(dgvDatasets.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "DsetEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				DsetEdit frm = new DsetEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEditDset_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvDatasets_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
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
			if (!curInspection.Valid())
			{
				setAllErrors();
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curInspection.Save();
			if (tmpID == null) return false;
			savedInternally = true;
			return true;
		}

		// Handle the user's decision to add a new tool
		private void btnAddDset_Click(object sender, EventArgs e)
		{
			if (!performSilentSave()) return;
			// show form etc
			DsetEdit frm = new DsetEdit(null, curInspection.ID);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDeleteDset_Click(object sender, EventArgs e)
		{
			if (dgvDatasets.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Dataset to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvDatasets.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "DsetEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EDset Dset = new EDset(currentEditItem);
			Dset.Delete(true);
			if (Dset.DsetErrMsg != null)
			{
				MessageBox.Show(Dset.DsetErrMsg, "Factotum");
				Dset.DsetErrMsg = null;
			}
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			if (dgvDatasets.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a dataset to move up first.", "Factotum");
				return;
			}
			if (dgvDatasets.SelectedRows[0].Index == 0)
			{
				MessageBox.Show("Can't move that dataset up.  It already has highest priority.", "Factotum");
				return;
			}
			int selIdx = dgvDatasets.SelectedRows[0].Index;
			EDset source = new EDset((Guid?)dgvDatasets.Rows[selIdx].Cells["ID"].Value);
			EDset dest = new EDset((Guid?)dgvDatasets.Rows[selIdx - 1].Cells["ID"].Value);
			RenumberDsets(source, dest);
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			if (dgvDatasets.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a dataset to move down first.", "Factotum");
				return;
			}
			int selIdx = dgvDatasets.SelectedRows[0].Index;
			if (selIdx >= dgvDatasets.Rows.Count - 1)
			{
				MessageBox.Show("Can't move that dataset down.  It already has lowest priority.", "Factotum");
				return;
			}
			EDset source = new EDset((Guid?)dgvDatasets.Rows[selIdx].Cells["ID"].Value);
			EDset dest = new EDset((Guid?)dgvDatasets.Rows[selIdx + 1].Cells["ID"].Value);
			RenumberDsets(source, dest);
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curInspection.InspectionNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curInspection.InspectionNameErrMsg);
		}

		// We don't have a handler for inspector hours text changed.
		// It's a float, I don't know what the character limit is...  I guess I could find out...
		private void txtNotes_TextChanged(object sender, EventArgs e)
		{
			curInspection.InspectionNotesLengthOk(txtNotes.Text);
			errorProvider1.SetError(txtNotes, curInspection.InspectionNotesErrMsg);
		}


		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curInspection.InspectionNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curInspection.InspectionNameErrMsg);
		}

		private void txtNotes_Validating(object sender, CancelEventArgs e)
		{
			curInspection.InspectionNotesValid(txtNotes.Text);
			errorProvider1.SetError(txtNotes, curInspection.InspectionNotesErrMsg);
		}

		private void txtInspectorHours_Validating(object sender, CancelEventArgs e)
		{
			curInspection.InspectionPersonHoursValid(txtInspectorHours.Text);
			errorProvider1.SetError(txtInspectorHours, curInspection.InspectionPersonHoursErrMsg);
		}
		
		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------

		private void UpdateSelector(Guid? id)
		{
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvDatasets.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvDatasets.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EDset.GetDefaultDataView(curInspection.ID);
			dgvDatasets.DataSource = dv;
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvDatasets.Sort(dgvDatasets.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvDatasets.Sort(dgvDatasets.Columns[sortCol], ListSortDirection.Descending);

			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvDatasets.Sort(dgvDatasets.Columns["DsetGridPriority"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvDatasets.Columns["DsetName"].HeaderText = "Dataset";
			dgvDatasets.Columns["DsetHasTextFile"].HeaderText = "Has Textfile";
			dgvDatasets.Columns["DsetAdditionalMeasurements"].HeaderText = "Add. Meas.";
			// Hide some columns
			dgvDatasets.Columns["ID"].Visible = false;
			dgvDatasets.Columns["DsetGridPriority"].Visible = false;
			dgvDatasets.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvDatasets.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvDatasets.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvDatasets.Rows[r].Cells["ID"].Value == id)
					{
						dgvDatasets.CurrentCell = dgvDatasets[firstCell.ColumnIndex, r];
						dgvDatasets.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvDatasets.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvDatasets.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvDatasets.CurrentCell = firstCell;
				dgvDatasets.Rows[0].Selected = true;
			}
		}

		private void ManageButtons()
		{
			bool hasGrid = curInspection.InspectionHasGrid;
			bool hasGraphic = curInspection.InspectionHasGraphic;
			btnAddEditGrid.Enabled = dgvDatasets.Rows.Count > 0 && (hasGrid || Globals.ActivationOK);
			btnAddEditGrid.Text = hasGrid ? "Edit Grid" : "Add Grid";
			btnAddEditGraphic.Enabled = Globals.ActivationOK;
			btnAddEditGraphic.Text = hasGraphic ? "Edit Graphic" : "Add Graphic";
			btnDeleteGrid.Enabled = hasGrid && Globals.ActivationOK;
			btnDeleteGraphic.Enabled = hasGraphic && Globals.ActivationOK;
		}

		// No prompting is performed.  The user should understand the meanings of OK and Cancel.
		private void SaveAndClose()
		{
			if (AnyControlErrors()) return;
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curInspection.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curInspection.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the inspected component object values.
		private void SetControlValues()
		{
			if (curInspection.InspectionIscID != null)
			{
				EInspectedComponent inspectedComponent = 
					new EInspectedComponent(curInspection.InspectionIscID);
				lblSiteName.Text = "Inspection for Report: '" + inspectedComponent.InspComponentName + "'";
			}
			else lblSiteName.Text = "Inspection for Unknown Report";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			txtName.Text = curInspection.InspectionName;
			txtInspectorHours.Text = GetFormattedFloat(curInspection.InspectionPersonHours);
			txtNotes.Text = curInspection.InspectionNotes;
		}

		private string GetFormattedFloat(float? number)
		{
			return number == null ? null :
				string.Format("{0:0.00}", number);
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtInspectorHours, curInspection.InspectionPersonHoursErrMsg);
			errorProvider1.SetError(txtName, curInspection.InspectionNameErrMsg);
			errorProvider1.SetError(txtNotes, curInspection.InspectionNotesErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtInspectorHours).Length > 0 ||
				errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtNotes).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curInspection.InspectionName = txtName.Text;
			curInspection.InspectionNotes = txtNotes.Text;
			curInspection.InspectionPersonHours = Util.GetNullableFloatForString(txtInspectorHours.Text);
		}

		private void RenumberDsets(EDset source, EDset dest)
		{
			short tmpGridPriority = (short)source.DsetGridPriority;
			source.DsetGridPriority = dest.DsetGridPriority;
			dest.DsetGridPriority = tmpGridPriority;
			source.Save();
			dest.Save();
		}

		private void btnAddEditGrid_Click(object sender, EventArgs e)
		{
			if (Globals.ActivationOK)
			{
				if (!performSilentSave()) return;
			}
			Guid? gridID = curInspection.GridID;
			GridEdit frm;
			if (gridID == null)
			{
				frm = new GridEdit(null, curInspection.ID);
			}
			else
			{
				frm = new GridEdit(gridID);
			}
			frm.ShowDialog();
		}

		private void btnDeleteGrid_Click(object sender, EventArgs e)
		{
			Guid? gridID = curInspection.GridID;
			if (gridID != null)
			{
				EGrid grid = new EGrid(gridID);
				grid.Delete(false);
			}
		}

		private void btnAddEditGraphic_Click(object sender, EventArgs e)
		{
			if (Globals.ActivationOK)
			{
				if (!performSilentSave()) return;
			}
			Guid? graphicID = curInspection.GraphicID;
			GraphicEdit frm;
			if (graphicID == null)
			{
				frm = new GraphicEdit(null, curInspection.ID);
			}
			else
			{
				frm = new GraphicEdit(graphicID);
			}
			frm.ShowDialog();
		}

		private void btnDeleteGraphic_Click(object sender, EventArgs e)
		{
			Guid? graphicID = curInspection.GraphicID;
			if (graphicID != null)
			{
				EGraphic graphic = new EGraphic(graphicID);
				graphic.Delete(false);
			}
		}


	}
}