using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class InspectorView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public InspectorView()
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

		// Set the status filter to show active inspectors by default
		// and update the tool selector combo box
		private void InspectorView_Load(object sender, EventArgs e)
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
			EInspector.Changed += new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EInspector.InspectorKitAssignmentsChanged += new EventHandler(EInspector_InspectorKitAssignmentsChanged);
			EInspector.InspectorOutageAssignmentsChanged += new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
		}
		private void InspectorView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EInspector.Changed -= new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EInspector.InspectorKitAssignmentsChanged -= new EventHandler(EInspector_InspectorKitAssignmentsChanged);
			EInspector.InspectorOutageAssignmentsChanged -= new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EInspector_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		void EInspector_InspectorKitAssignmentsChanged(object sender, EventArgs e)
		{
			UpdateSelector(null);
		}

		void EInspector_InspectorOutageAssignmentsChanged(object sender, EventArgs e)
		{
			UpdateSelector(null);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvInspectors.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvInspectors.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "InspectorEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				InspectorEdit frm = new InspectorEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvInspectors_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			InspectorEdit frm = new InspectorEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvInspectors.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Block to delete first.","Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvInspectors.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "InspectorEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EInspector Inspector = new EInspector(currentEditItem);
			Inspector.Delete(true);
			if (Inspector.InspectorErrMsg != null)
			{
				MessageBox.Show(Inspector.InspectorErrMsg, "Factotum");
				Inspector.InspectorErrMsg = null;
			}
		}

		// The user changed the status filter setting, so apply the filter.
		private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}
		// The user changed the outage filter setting.
		private void cboInOutageFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			// We need to refresh the data grid in this case
			if (dgvInspectors.SelectedRows.Count > 0)
				UpdateSelector((Guid?)dgvInspectors.SelectedRows[0].Cells["ID"].Value);
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
			SortOrder sortOrder = dgvInspectors.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvInspectors.SortedColumn.Index;

			// Update the grid view selector
			if (Globals.CurrentOutageID == null ||
				cboInOutageFilter.SelectedIndex == (int)FilterYesNoAll.ShowAll)
				dv = EInspector.GetDefaultDataView(null);
			else
				dv = EInspector.GetDefaultDataView(Globals.CurrentOutageID);

			dgvInspectors.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvInspectors.Sort(dgvInspectors.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvInspectors.Sort(dgvInspectors.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvInspectors.Sort(dgvInspectors.Columns["InspectorName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvInspectors.Columns["InspectorName"].HeaderText = "Name";
			dgvInspectors.Columns["InspectorLevel"].HeaderText = "Level";
			dgvInspectors.Columns["InspectorKitName"].HeaderText = "Kit";
			dgvInspectors.Columns["InspectorIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvInspectors.Columns["ID"].Visible = false;
			dgvInspectors.Columns["InspectorIsActive"].Visible = false;
			dgvInspectors.Columns["InspectorIsLclChg"].Visible = false;
			dgvInspectors.Columns["InspectorUsedInOutage"].Visible = false;
			dgvInspectors.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvInspectors.DataSource == null) return;
			StringBuilder sb = new StringBuilder("InspectorIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");

			DataView dv = (DataView)dgvInspectors.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvInspectors.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvInspectors.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvInspectors.Rows[r].Cells["ID"].Value == id)
					{
						dgvInspectors.CurrentCell = dgvInspectors[firstCell.ColumnIndex, r];
						dgvInspectors.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvInspectors.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvInspectors.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvInspectors.CurrentCell = firstCell;
				dgvInspectors.Rows[0].Selected = true;
			}
		}

	}
}