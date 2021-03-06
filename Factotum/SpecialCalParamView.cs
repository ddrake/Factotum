using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class SpecialCalParamView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public SpecialCalParamView()
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
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void SpecialCalParamView_Load(object sender, EventArgs e)
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
			ESpecialCalParam.Changed += new EventHandler<EntityChangedEventArgs>(ESpecialCalParam_Changed);
		}

		private void SpecialCalParamView_FormClosed(object sender, FormClosedEventArgs e)
		{
			ESpecialCalParam.Changed -= new EventHandler<EntityChangedEventArgs>(ESpecialCalParam_Changed);
		}
		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void ESpecialCalParam_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}
		private void dgvSpecialCalParamsList_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvSpecialCalParamsList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvSpecialCalParamsList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "SpecialCalParamEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				SpecialCalParamEdit frm = new SpecialCalParamEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvSpecialCalParamsList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			SpecialCalParamEdit frm = new SpecialCalParamEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvSpecialCalParamsList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Special Calibration Parameter to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvSpecialCalParamsList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "SpecialCalParamEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			ESpecialCalParam SpecialCalParam = new ESpecialCalParam(currentEditItem);
			SpecialCalParam.Delete(true);
			if (SpecialCalParam.SpecialCalParamErrMsg != null)
			{
				MessageBox.Show(SpecialCalParam.SpecialCalParamErrMsg, "Factotum");
				SpecialCalParam.SpecialCalParamErrMsg = null;
			}
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			if (dgvSpecialCalParamsList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a calibration parameter to move up first.", "Factotum");
				return;
			}
			if (dgvSpecialCalParamsList.SelectedRows[0].Index == 0)
			{
				MessageBox.Show("Can't move that calibration parameter up.  It's already first in the report.", "Factotum");
				return;
			}
			int selIdx = dgvSpecialCalParamsList.SelectedRows[0].Index;
			ESpecialCalParam source = new ESpecialCalParam((Guid?)dgvSpecialCalParamsList.Rows[selIdx].Cells["ID"].Value);
			ESpecialCalParam dest = new ESpecialCalParam((Guid?)dgvSpecialCalParamsList.Rows[selIdx - 1].Cells["ID"].Value);
			RenumberCalParameters(source, dest);
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			if (dgvSpecialCalParamsList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a calibration parameter to move down first.", "Factotum");
				return;
			}
			int selIdx = dgvSpecialCalParamsList.SelectedRows[0].Index;
			if (selIdx >= dgvSpecialCalParamsList.Rows.Count - 1)
			{
				MessageBox.Show("Can't move that calibration parameter down.  It's already last in the report.", "Factotum");
				return;
			}
			ESpecialCalParam source = new ESpecialCalParam((Guid?)dgvSpecialCalParamsList.Rows[selIdx].Cells["ID"].Value);
			ESpecialCalParam dest = new ESpecialCalParam((Guid?)dgvSpecialCalParamsList.Rows[selIdx + 1].Cells["ID"].Value);
			RenumberCalParameters(source, dest);
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
		// Update the selector DataGridView by filling its items based on current data and filters.
		// Then set the currently displayed item to that of the supplied ID.
		// If the supplied ID isn't on the list because of the current filter state, just show the
		// first item if there is one.
		private void UpdateSelector(Guid? id)
		{
			DataView dv;
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvSpecialCalParamsList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvSpecialCalParamsList.SortedColumn.Index;

			dv = ESpecialCalParam.GetDefaultDataView();

			dgvSpecialCalParamsList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvSpecialCalParamsList.Sort(dgvSpecialCalParamsList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvSpecialCalParamsList.Sort(dgvSpecialCalParamsList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
//			dgvSpecialCalParamsList.Sort(dgvSpecialCalParamsList.Columns["SpecialCalParamName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvSpecialCalParamsList.Columns["SpecialCalParamName"].HeaderText = "Parameter Name";
			dgvSpecialCalParamsList.Columns["SpecialCalParamUnits"].HeaderText = "Units";
			dgvSpecialCalParamsList.Columns["SpecialCalParamIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvSpecialCalParamsList.Columns["ID"].Visible = false;
			dgvSpecialCalParamsList.Columns["SpecialCalParamIsActive"].Visible = false;
			dgvSpecialCalParamsList.Columns["SpecialCalParamIsLclChg"].Visible = false;
			dgvSpecialCalParamsList.Columns["SpecialCalParamUsedInOutage"].Visible = false;
			dgvSpecialCalParamsList.Columns["SpecialCalParamReportOrder"].Visible = false;
			dgvSpecialCalParamsList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvSpecialCalParamsList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("SpecialCalParamIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			DataView dv = (DataView)dgvSpecialCalParamsList.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvSpecialCalParamsList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvSpecialCalParamsList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvSpecialCalParamsList.Rows[r].Cells["ID"].Value == id)
					{
						dgvSpecialCalParamsList.CurrentCell = dgvSpecialCalParamsList[firstCell.ColumnIndex, r];
						dgvSpecialCalParamsList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvSpecialCalParamsList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvSpecialCalParamsList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvSpecialCalParamsList.CurrentCell = firstCell;
				dgvSpecialCalParamsList.Rows[0].Selected = true;
			}
		}

		// Swap the report order for two special calibration parameters.
		private void RenumberCalParameters(ESpecialCalParam source, ESpecialCalParam dest)
		{
			short tmpReportOrder = (short)source.SpecialCalParamReportOrder;
			source.SpecialCalParamReportOrder = dest.SpecialCalParamReportOrder;
			dest.SpecialCalParamReportOrder = tmpReportOrder;
			source.Save();
			dest.Save();
		}


	}
}