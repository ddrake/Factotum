using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class SystemView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		
		EUnitCollection units;

		// Form constructor
		public SystemView()
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
            if (Globals.CurrentOutageID != null)
            {
                EOutage otg = new EOutage((Guid?)Globals.CurrentOutageID);
                cboUnitFilter.SelectedValue = otg.OutageUntID;
            }

			FillUnits();
			HandleEnablingForPropertySettings();			

			btnAdd.Enabled = Globals.ActivationOK;
			btnDelete.Enabled = Globals.ActivationOK;
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void SystemView_Load(object sender, EventArgs e)
		{
			if (units.Count == 0)
			{
				MessageBox.Show("Can't Add Systems until there is at Least one Unit", "Factotum");
				btnAdd.Enabled = false;
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				return;
			}
            UpdateAddToolTip();
            // Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			// We don't want these to get triggered except by the user
			this.cboStatusFilter.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
			this.txtNameFilter.TextChanged += new System.EventHandler(this.txtNameFilter_TextChanged);
			this.cboUnitFilter.SelectedIndexChanged += new System.EventHandler(this.cboUnitFilter_SelectedIndexChanged);
			EUnit.Changed += new EventHandler<EntityChangedEventArgs>(EUnit_Changed);
			ESystem.Changed += new EventHandler<EntityChangedEventArgs>(ESystem_Changed);
			Globals.CurrentOutageChanged += new EventHandler(Globals_CurrentOutageChanged);
		}

		private void SystemView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EUnit.Changed -= new EventHandler<EntityChangedEventArgs>(EUnit_Changed);
			ESystem.Changed -= new EventHandler<EntityChangedEventArgs>(ESystem_Changed);
			Globals.CurrentOutageChanged -= new EventHandler(Globals_CurrentOutageChanged);
		}
		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void ESystem_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		void EUnit_Changed(object sender, EntityChangedEventArgs e)
		{
			FillUnits();
			UpdateSelector(null);
		}

		void Globals_CurrentOutageChanged(object sender, EventArgs e)
		{
			HandleEnablingForPropertySettings();
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvSystemList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvSystemList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "SystemEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				SystemEdit frm = new SystemEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvSystemList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{

			SystemEdit frm = new SystemEdit(null, (Guid?)cboUnitFilter.SelectedValue);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvSystemList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a System to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvSystemList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "SystemEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			ESystem System = new ESystem(currentEditItem);
			System.Delete(true);
			if (System.SystemErrMsg != null)
			{
				MessageBox.Show(System.SystemErrMsg, "Factotum");
				System.SystemErrMsg = null;
			}
		}

		// The user changed the status filter setting, so update the selector combo.
		private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}
		// This one is special, the other combos and the data view depend on it...
		private void cboUnitFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			HandleUnitFilterChanged();
		}

		private void txtNameFilter_TextChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}

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
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvSystemList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvSystemList.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = ESystem.GetDefaultDataViewForUnit((Guid?)cboUnitFilter.SelectedValue);
			dgvSystemList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvSystemList.Sort(dgvSystemList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvSystemList.Sort(dgvSystemList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvSystemList.Sort(dgvSystemList.Columns["SystemName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvSystemList.Columns["SystemName"].HeaderText = "System Name";
			// Hide some columns
			dgvSystemList.Columns["ID"].Visible = false;
			dgvSystemList.Columns["SystemIsActive"].Visible = false;
			dgvSystemList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvSystemList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("SystemIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			if (txtNameFilter.Text.Length > 0)
				sb.Append(" And SystemName Like '" + txtNameFilter.Text + "*'");

			DataView dv = (DataView)dgvSystemList.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvSystemList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvSystemList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvSystemList.Rows[r].Cells["ID"].Value == id)
					{
						dgvSystemList.CurrentCell = dgvSystemList[firstCell.ColumnIndex, r];
						dgvSystemList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvSystemList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvSystemList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvSystemList.CurrentCell = firstCell;
				dgvSystemList.Rows[0].Selected = true;
			}
		}

		private void FillUnits()
		{
			units = EUnit.ListByName(false, false);
			cboUnitFilter.DataSource = units;
			cboUnitFilter.DisplayMember = "UnitNameWithSite";
			cboUnitFilter.ValueMember = "ID";

		}

		private void HandleUnitFilterChanged()
		{
			UpdateSelector(null);
            UpdateAddToolTip();
        }

        private void UpdateAddToolTip()
        {
            if (cboUnitFilter.Text == null)
                toolTip1.SetToolTip(btnAdd, "");
            else
                toolTip1.SetToolTip(btnAdd, "Add a system for unit: " + cboUnitFilter.Text);
        }

        private void HandleEnablingForPropertySettings()
		{
			if (Globals.CurrentOutageID != null)
			{
				Guid OutageID = (Guid)Globals.CurrentOutageID;
				EOutage outage = new EOutage(OutageID);
				if (outage.OutageUntID != (Guid)cboUnitFilter.SelectedValue)
					cboUnitFilter.SelectedValue = outage.OutageUntID;
				// do we need to call HandleUnitFilterChanged???
			}
			if (Globals.IsMasterDB)
			{
				cboUnitFilter.Enabled = true;
			}
			else
			{
				cboUnitFilter.Enabled = false;
			}
		}


	}
}