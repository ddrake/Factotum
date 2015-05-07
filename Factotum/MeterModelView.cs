using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class MeterModelView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public MeterModelView()
		{
			InitializeComponent();

			// Take care of settings that are not easily managed in the designer.
			InitializeControls();
		}

		// Take care of settings that are not easily managed in the designer.
		private void InitializeControls() 
		{
			btnAdd.Enabled = Globals.ActivationOK;
			btnDelete.Enabled = Globals.ActivationOK;
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void MeterModelView_Load(object sender, EventArgs e)
		{
			// Set the status combo first.  The selector DataGridView depends on it.
			cboStatusFilter.SelectedIndex = (int)FilterActiveStatus.ShowActive;
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			// Wire up the handler for the Entity changed event
			EMeterModel.Changed += new EventHandler<EntityChangedEventArgs>(EMeterModel_Changed);
		}
		private void MeterModelView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EMeterModel.Changed -= new EventHandler<EntityChangedEventArgs>(EMeterModel_Changed);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EMeterModel_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvMeterModels.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvMeterModels.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "MeterModelEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				MeterModelEdit frm = new MeterModelEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvMeterModels_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			MeterModelEdit frm = new MeterModelEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvMeterModels.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Block to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvMeterModels.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "MeterModelEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EMeterModel MeterModel = new EMeterModel(currentEditItem);
			MeterModel.Delete(true);
			if (MeterModel.MeterModelErrMsg != null)
			{
				MessageBox.Show(MeterModel.MeterModelErrMsg, "Factotum");
				MeterModel.MeterModelErrMsg = null;
			}
		}

		// The user changed the status filter setting, so update the selector combo.
		private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
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
			SortOrder sortOrder = dgvMeterModels.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvMeterModels.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EMeterModel.GetDefaultDataView();
			dgvMeterModels.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvMeterModels.Sort(dgvMeterModels.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvMeterModels.Sort(dgvMeterModels.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvMeterModels.Sort(dgvMeterModels.Columns["MeterModelName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvMeterModels.Columns["MeterModelName"].HeaderText = "Model Name";
			dgvMeterModels.Columns["MeterModelManfName"].HeaderText = "Manufacturer";
			dgvMeterModels.Columns["MeterModelIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvMeterModels.Columns["ID"].Visible = false;
			dgvMeterModels.Columns["MeterModelIsActive"].Visible = false;
			dgvMeterModels.Columns["MeterModelUsedInOUtage"].Visible = false;
			dgvMeterModels.Columns["MeterModelIsLclChg"].Visible = false;
			dgvMeterModels.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvMeterModels.DataSource == null) return;
			StringBuilder sb = new StringBuilder("MeterModelIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");

			DataView dv = (DataView)dgvMeterModels.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvMeterModels.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvMeterModels.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvMeterModels.Rows[r].Cells["ID"].Value == id)
					{
						dgvMeterModels.CurrentCell = dgvMeterModels[firstCell.ColumnIndex, r];
						dgvMeterModels.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvMeterModels.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvMeterModels.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvMeterModels.CurrentCell = firstCell;
				dgvMeterModels.Rows[0].Selected = true;
			}
		}


	}
}