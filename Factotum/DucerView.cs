using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class DucerView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public DucerView()
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
		private void DucerView_Load(object sender, EventArgs e)
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
			EDucer.Changed += new EventHandler<EntityChangedEventArgs>(EDucer_Changed);
		}
		private void DucerView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EDucer.Changed -= new EventHandler<EntityChangedEventArgs>(EDucer_Changed);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EDucer_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvTransducers.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvTransducers.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "DucerEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				DucerEdit frm = new DucerEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvTransducers_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			DucerEdit frm = new DucerEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvTransducers.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Transducer to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvTransducers.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "DucerEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EDucer Ducer = new EDucer(currentEditItem);
			Ducer.Delete(true);
			if (Ducer.DucerErrMsg != null)
			{
				MessageBox.Show(Ducer.DucerErrMsg, "Factotum");
				Ducer.DucerErrMsg = null;
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
			SortOrder sortOrder = dgvTransducers.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvTransducers.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EDucer.GetDefaultDataView();
			dgvTransducers.DataSource = dv;
			ApplyFilters();

			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvTransducers.Sort(dgvTransducers.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvTransducers.Sort(dgvTransducers.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvTransducers.Sort(dgvTransducers.Columns["DucerSerialNumber"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvTransducers.Columns["DucerSerialNumber"].HeaderText = "Serial Number";
			dgvTransducers.Columns["DucerModelName"].HeaderText = "Model Name";
			dgvTransducers.Columns["DucerKitName"].HeaderText = "Kit";
			dgvTransducers.Columns["DucerIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvTransducers.Columns["ID"].Visible = false;
			dgvTransducers.Columns["DucerIsActive"].Visible = false;
			dgvTransducers.Columns["DucerIsLclChg"].Visible = false;
			dgvTransducers.Columns["DucerUsedInOutage"].Visible = false;
			dgvTransducers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvTransducers.DataSource == null) return;
			StringBuilder sb = new StringBuilder("DucerIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");

			DataView dv = (DataView)dgvTransducers.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvTransducers.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvTransducers.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvTransducers.Rows[r].Cells["ID"].Value == id)
					{
						dgvTransducers.CurrentCell = dgvTransducers[firstCell.ColumnIndex, r];
						dgvTransducers.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvTransducers.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvTransducers.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvTransducers.CurrentCell = firstCell;
				dgvTransducers.Rows[0].Selected = true;
			}
		}


	}
}