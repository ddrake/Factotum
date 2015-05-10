using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class CalibrationProcedureView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public CalibrationProcedureView()
		{
			InitializeComponent();

			// Take care of settings that are not easily managed in the designer.
			InitializeControls();
		}

		// Take care of settings that are not easily managed in the designer.
		private void InitializeControls() 
		{
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void CalibrationProcedureView_Load(object sender, EventArgs e)
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
			ECalibrationProcedure.Changed += new EventHandler<EntityChangedEventArgs>(ECalibrationProcedure_Changed);
		}
		private void CalibrationProcedureView_FormClosing(object sender, FormClosingEventArgs e)
		{
			ECalibrationProcedure.Changed -= new EventHandler<EntityChangedEventArgs>(ECalibrationProcedure_Changed);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void ECalibrationProcedure_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvCalibrationProcedureList.SelectedRows.Count != 1) return;
			
			Guid? currentEditItem = (Guid?)(dgvCalibrationProcedureList.SelectedRows[0].Cells["ID"].Value);			
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "CalibrationProcedureEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				CalibrationProcedureEdit frm = new CalibrationProcedureEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}
		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvCalibrationProcedureList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			CalibrationProcedureEdit frm = new CalibrationProcedureEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}
		
		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvCalibrationProcedureList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Procedure to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvCalibrationProcedureList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "CalibrationProcedureEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}
			ECalibrationProcedure CalibrationProcedure = new ECalibrationProcedure(currentEditItem);
			CalibrationProcedure.Delete(true);
			if (CalibrationProcedure.CalibrationProcedureErrMsg != null)
			{
				MessageBox.Show(CalibrationProcedure.CalibrationProcedureErrMsg, "Factotum");
				CalibrationProcedure.CalibrationProcedureErrMsg = null;
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
			SortOrder sortOrder = dgvCalibrationProcedureList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvCalibrationProcedureList.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = ECalibrationProcedure.GetDefaultDataView();
			dgvCalibrationProcedureList.DataSource = dv;
			ApplyFilters();
		
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvCalibrationProcedureList.Sort(dgvCalibrationProcedureList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvCalibrationProcedureList.Sort(dgvCalibrationProcedureList.Columns[sortCol], ListSortDirection.Descending);

			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvCalibrationProcedureList.Sort(dgvCalibrationProcedureList.Columns["CalibrationProcedureName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvCalibrationProcedureList.Columns["CalibrationProcedureName"].HeaderText = "Calibration Proc. Name";
			dgvCalibrationProcedureList.Columns["CalibrationProcedureDescription"].HeaderText = "Description";
			dgvCalibrationProcedureList.Columns["CalibrationProcedureIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvCalibrationProcedureList.Columns["ID"].Visible = false;
			dgvCalibrationProcedureList.Columns["CalibrationProcedureIsActive"].Visible = false;
			dgvCalibrationProcedureList.Columns["CalibrationProcedureIsLclChg"].Visible = false;
			dgvCalibrationProcedureList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvCalibrationProcedureList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("CalibrationProcedureIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			DataView dv = (DataView)dgvCalibrationProcedureList.DataSource;
			dv.RowFilter = sb.ToString();
		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvCalibrationProcedureList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvCalibrationProcedureList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvCalibrationProcedureList.Rows[r].Cells["ID"].Value == id)
					{
						dgvCalibrationProcedureList.CurrentCell = dgvCalibrationProcedureList[firstCell.ColumnIndex,r];
						dgvCalibrationProcedureList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvCalibrationProcedureList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvCalibrationProcedureList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvCalibrationProcedureList.CurrentCell = firstCell ;
				dgvCalibrationProcedureList.Rows[0].Selected = true;
			}
		}

	}
}