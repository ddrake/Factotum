using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class KitView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public KitView()
		{
			InitializeComponent();

			// Take care of settings that are not as easily managed in the designer.
			InitializeControls();
		}

		// Take care of the non-default DataGridView settings that are not as easily managed
		// in the designer.
		private void InitializeControls() 
		{
			btnAdd.Enabled = Globals.ActivationOK;
			btnDelete.Enabled = Globals.ActivationOK;
		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void KitView_Load(object sender, EventArgs e)
		{
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			// Wire up the handler for the Entity changed event
			EKit.Changed += new EventHandler<EntityChangedEventArgs>(EKit_Changed);
			EInspector.InspectorKitAssignmentsChanged += new EventHandler(EKitAssignment_Changed);
			EMeter.MeterKitAssignmentsChanged += new EventHandler(EKitAssignment_Changed);
			EDucer.DucerKitAssignmentsChanged += new EventHandler(EKitAssignment_Changed);
			ECalBlock.CalBlockKitAssignmentsChanged += new EventHandler(EKitAssignment_Changed);
			EThermo.ThermoKitAssignmentsChanged += new EventHandler(EKitAssignment_Changed);
		}
		private void KitView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EKit.Changed -= new EventHandler<EntityChangedEventArgs>(EKit_Changed);
			EInspector.InspectorKitAssignmentsChanged -= new EventHandler(EKitAssignment_Changed);
			EMeter.MeterKitAssignmentsChanged -= new EventHandler(EKitAssignment_Changed);
			EDucer.DucerKitAssignmentsChanged -= new EventHandler(EKitAssignment_Changed);
			ECalBlock.CalBlockKitAssignmentsChanged -= new EventHandler(EKitAssignment_Changed);
			EThermo.ThermoKitAssignmentsChanged -= new EventHandler(EKitAssignment_Changed);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EKit_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}
		void EKitAssignment_Changed(object sender, EventArgs e)
		{
			if (dgvKits.SelectedRows.Count != 1) return;
			Guid? currentEditItem = (Guid?)(dgvKits.SelectedRows[0].Cells["ID"].Value);
			UpdateSelector(currentEditItem);
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvKits.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvKits.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "KitEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				KitEdit frm = new KitEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvKits_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			KitEdit frm = new KitEdit();
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvKits.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Kit to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvKits.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "KitEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EKit Kit = new EKit(currentEditItem);
			Kit.Delete(true);
			if (Kit.ToolKitErrMsg != null)
			{
				MessageBox.Show(Kit.ToolKitErrMsg, "Factotum");
				Kit.ToolKitErrMsg = null;
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
			SortOrder sortOrder = dgvKits.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvKits.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EKit.GetDefaultDataView();
			dgvKits.DataSource = dv;
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvKits.Sort(dgvKits.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvKits.Sort(dgvKits.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvKits.Sort(dgvKits.Columns["KitName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvKits.Columns["KitName"].HeaderText = "Kit Name";
			dgvKits.Columns["KitInspectorCt"].HeaderText = "# Inspectors";
			dgvKits.Columns["KitMeterCt"].HeaderText = "# Meters";
			dgvKits.Columns["KitDucerCt"].HeaderText = "# Transducers";
			dgvKits.Columns["KitCalBlockCt"].HeaderText = "# Cal Blocks";
			dgvKits.Columns["KitThermoCt"].HeaderText = "# Thermometers";
			// Hide some columns
			dgvKits.Columns["ID"].Visible = false;
			dgvKits.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvKits.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvKits.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvKits.Rows[r].Cells["ID"].Value == id)
					{
						dgvKits.CurrentCell = dgvKits[firstCell.ColumnIndex, r];
						dgvKits.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvKits.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvKits.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvKits.CurrentCell = firstCell;
				dgvKits.Rows[0].Selected = true;
			}

		}

	}
}