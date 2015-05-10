using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class PipeScheduleView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public PipeScheduleView()
		{
			InitializeComponent();

			// Take care of settings that are not as easily managed in the designer.
			InitializeControls();
		}

		// Take care of the non-default DataGridView settings that are not as easily managed
		// in the designer.
		private void InitializeControls() 
		{
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void PipeScheduleView_Load(object sender, EventArgs e)
		{
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			// Wire up the handler for the Entity changed event
			EPipeSchedule.Changed += new EventHandler<EntityChangedEventArgs>(EPipeSchedule_Changed);
		}
		private void PipeScheduleView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EPipeSchedule.Changed -= new EventHandler<EntityChangedEventArgs>(EPipeSchedule_Changed);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EPipeSchedule_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvPipeSchedulesList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvPipeSchedulesList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "PipeScheduleEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				PipeScheduleEdit frm = new PipeScheduleEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvPipeSchedulesList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			PipeScheduleEdit frm = new PipeScheduleEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvPipeSchedulesList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Block to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvPipeSchedulesList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "PipeScheduleEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EPipeSchedule PipeSchedule = new EPipeSchedule(currentEditItem);
			PipeSchedule.Delete(true);
			if (PipeSchedule.PipeScheduleErrMsg != null)
			{
				MessageBox.Show(PipeSchedule.PipeScheduleErrMsg, "Factotum");
				PipeSchedule.PipeScheduleErrMsg = null;
			}
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
			SortOrder sortOrder = dgvPipeSchedulesList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvPipeSchedulesList.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EPipeSchedule.GetDefaultDataView();
			dgvPipeSchedulesList.DataSource = dv;
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvPipeSchedulesList.Sort(dgvPipeSchedulesList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvPipeSchedulesList.Sort(dgvPipeSchedulesList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvPipeSchedulesList.Sort(dgvPipeSchedulesList.Columns["PipeScheduleSchedule"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvPipeSchedulesList.Columns["PipeScheduleSchedule"].HeaderText = "Schedule";
			dgvPipeSchedulesList.Columns["PipeScheduleNomDia"].HeaderText = "Nominal Diameter";
			dgvPipeSchedulesList.Columns["PipeScheduleOd"].HeaderText = "Outside Diameter";
			dgvPipeSchedulesList.Columns["PipeScheduleNomWall"].HeaderText = "Nominal Wall";
			// Hide some columns
			dgvPipeSchedulesList.Columns["ID"].Visible = false;
			dgvPipeSchedulesList.Columns["PipeScheduleIsLclChg"].Visible = false;
			dgvPipeSchedulesList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvPipeSchedulesList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvPipeSchedulesList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvPipeSchedulesList.Rows[r].Cells["ID"].Value == id)
					{
						dgvPipeSchedulesList.CurrentCell = dgvPipeSchedulesList[firstCell.ColumnIndex, r];
						dgvPipeSchedulesList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvPipeSchedulesList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvPipeSchedulesList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvPipeSchedulesList.CurrentCell = firstCell;
				dgvPipeSchedulesList.Rows[0].Selected = true;
			}
		}

	}
}