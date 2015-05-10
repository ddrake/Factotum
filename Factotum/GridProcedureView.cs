using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class GridProcedureView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public GridProcedureView()
		{
			InitializeComponent();

			// Take care of settings that are not easily managed in the designer.
			InitializeControls();
		}

		// Take care of settings that are not easily managed in the designer.
		private void InitializeControls()
		{
			// Set these combos to their defaults
			cboStatusFilter.SelectedIndex = (int)FilterActiveStatus.ShowActive;
			cboInOutageFilter.SelectedIndex = Globals.IsMasterDB ?
				(int)FilterYesNoAll.ShowAll : (int)FilterYesNoAll.Yes;
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void GridProcedureView_Load(object sender, EventArgs e)
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
			// We don't want these to get triggered except by the user
			this.cboInOutageFilter.SelectedIndexChanged += new System.EventHandler(this.cboInOutageFilter_SelectedIndexChanged);
			// Wire up the handler for the Entity changed event
			EGridProcedure.Changed += new EventHandler<EntityChangedEventArgs>(EGridProcedure_Changed);
			EGridProcedure.GridProcedureOutageAssignmentsChanged += new EventHandler(EGridProcedure_GridProcedureOutageAssignmentsChanged);
		}

		private void GridProcedureView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EGridProcedure.Changed -= new EventHandler<EntityChangedEventArgs>(EGridProcedure_Changed);
			EGridProcedure.GridProcedureOutageAssignmentsChanged -= new EventHandler(EGridProcedure_GridProcedureOutageAssignmentsChanged);
		}


		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EGridProcedure_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}
		void EGridProcedure_GridProcedureOutageAssignmentsChanged(object sender, EventArgs e)
		{
			UpdateSelector(null);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvGridProcedureList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvGridProcedureList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "GridProcedureEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				GridProcedureEdit frm = new GridProcedureEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvGridProcedureList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			GridProcedureEdit frm = new GridProcedureEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvGridProcedureList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Grid Procedure to delete first.","Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvGridProcedureList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "GridProcedureEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EGridProcedure GridProcedure = new EGridProcedure(currentEditItem);
			GridProcedure.Delete(true);
			if (GridProcedure.GridProcedureErrMsg != null)
			{
				MessageBox.Show(GridProcedure.GridProcedureErrMsg, "Factotum");
				GridProcedure.GridProcedureErrMsg = null;
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
			// We need to refresh the data grid in this case
			if (dgvGridProcedureList.SelectedRows.Count > 0)
				UpdateSelector((Guid?)dgvGridProcedureList.SelectedRows[0].Cells["ID"].Value);
			else
				UpdateSelector(null);
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
			SortOrder sortOrder = dgvGridProcedureList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvGridProcedureList.SortedColumn.Index;

			// Update the grid view selector
			if (Globals.CurrentOutageID == null ||
				cboInOutageFilter.SelectedIndex == (int)FilterYesNoAll.ShowAll)
				dv = EGridProcedure.GetDefaultDataView(null);
			else
				dv = EGridProcedure.GetDefaultDataView(Globals.CurrentOutageID);

			dgvGridProcedureList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvGridProcedureList.Sort(dgvGridProcedureList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvGridProcedureList.Sort(dgvGridProcedureList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvGridProcedureList.Sort(dgvGridProcedureList.Columns["GridProcedureName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvGridProcedureList.Columns["GridProcedureName"].HeaderText = "Procedure Name";
			dgvGridProcedureList.Columns["GridProcedureDsDiameters"].HeaderText = "D/S Dias";
			dgvGridProcedureList.Columns["GridProcedureDescription"].HeaderText = "Procedure Description";
			dgvGridProcedureList.Columns["GridProcedureIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvGridProcedureList.Columns["ID"].Visible = false;
			dgvGridProcedureList.Columns["GridProcedureIsActive"].Visible = false;
			dgvGridProcedureList.Columns["GridProcedureIsLclChg"].Visible = false;
			dgvGridProcedureList.Columns["GridProcedureUsedInOutage"].Visible = false;
			dgvGridProcedureList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvGridProcedureList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("GridProcedureIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			DataView dv = (DataView)dgvGridProcedureList.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvGridProcedureList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvGridProcedureList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvGridProcedureList.Rows[r].Cells["ID"].Value == id)
					{
						dgvGridProcedureList.CurrentCell = dgvGridProcedureList[firstCell.ColumnIndex, r];
						dgvGridProcedureList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvGridProcedureList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvGridProcedureList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvGridProcedureList.CurrentCell = firstCell;
				dgvGridProcedureList.Rows[0].Selected = true;
			}
		}

	}
}