using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class GridSizeView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public GridSizeView()
		{
			InitializeComponent();

			// Take care of settings that are not as easily managed in the designer.
			InitializeControls();
		}

		// Take care of the non-default DataGridView settings that are not as easily managed
		// in the designer.
		private void InitializeControls()
		{
			// Set these combos to their defaults
			cboStatusFilter.SelectedIndex = (int)FilterActiveStatus.ShowActive;
			btnAdd.Enabled = Globals.ActivationOK;
			btnDelete.Enabled = Globals.ActivationOK;
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void GridSizeView_Load(object sender, EventArgs e)
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
			EGridSize.Changed += new EventHandler<EntityChangedEventArgs>(EGridSize_Changed);
		}

		private void GridSizeView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EGridSize.Changed -= new EventHandler<EntityChangedEventArgs>(EGridSize_Changed);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EGridSize_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvGridSizeList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvGridSizeList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "GridSizeEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				GridSizeEdit frm = new GridSizeEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvGridSizeList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			GridSizeEdit frm = new GridSizeEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvGridSizeList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Grid Size to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvGridSizeList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "GridSizeEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EGridSize GridSize = new EGridSize(currentEditItem);
			GridSize.Delete(true);
			if (GridSize.GridSizeErrMsg != null)
			{
				MessageBox.Show(GridSize.GridSizeErrMsg, "Factotum");
				GridSize.GridSizeErrMsg = null;
			}
		}

		// The user changed the status filter setting, so update the selector combo.
		private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}
		// The user changed the outage filter setting.
		private void cboInOutageFilter_SelectedIndexChanged(object sender, EventArgs e)
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
			DataView dv;
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvGridSizeList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvGridSizeList.SortedColumn.Index;

			// Update the grid view selector
			dv = EGridSize.GetDefaultDataView();

			dgvGridSizeList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvGridSizeList.Sort(dgvGridSizeList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvGridSizeList.Sort(dgvGridSizeList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvGridSizeList.Sort(dgvGridSizeList.Columns["GridSizeName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvGridSizeList.Columns["GridSizeName"].HeaderText = "Size Name";
			dgvGridSizeList.Columns["GridSizeAxialDistance"].HeaderText = "Axial Dist.";
			dgvGridSizeList.Columns["GridSizeRadialDistance"].HeaderText = "Radial Dist.";
			dgvGridSizeList.Columns["GridSizeMaxDiameter"].HeaderText = "Max Pipe OD";
			dgvGridSizeList.Columns["GridSizeIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvGridSizeList.Columns["ID"].Visible = false;
			dgvGridSizeList.Columns["GridSizeIsActive"].Visible = false;
			dgvGridSizeList.Columns["GridSizeIsLclChg"].Visible = false;
			dgvGridSizeList.Columns["GridSizeUsedInOutage"].Visible = false;
			dgvGridSizeList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvGridSizeList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("GridSizeIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			DataView dv = (DataView)dgvGridSizeList.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvGridSizeList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvGridSizeList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvGridSizeList.Rows[r].Cells["ID"].Value == id)
					{
						dgvGridSizeList.CurrentCell = dgvGridSizeList[firstCell.ColumnIndex, r];
						dgvGridSizeList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvGridSizeList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvGridSizeList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvGridSizeList.CurrentCell = firstCell;
				dgvGridSizeList.Rows[0].Selected = true;
			}
		}

	}
}