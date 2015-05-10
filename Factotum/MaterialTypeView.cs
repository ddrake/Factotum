using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class MaterialTypeView : Form
	{

		ESiteCollection sites;

		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		

		// Form constructor
		public MaterialTypeView()
		{
			InitializeComponent();

			// Take care of settings that are not easily managed in the designer.
			InitializeControls();
		}

		// Take care of settings that are not easily managed in the designer.
		private void InitializeControls()
		{
			sites = ESite.ListByName(true, false, false);
			cboSiteFilter.DataSource = sites;
			cboSiteFilter.DisplayMember = "SiteName";
			cboSiteFilter.ValueMember = "ID";
			HandleEnablingForPropertySettings();
		}

		// Set the status filter to show active types by default
		// and update the status combo box
		private void MaterialTypeView_Load(object sender, EventArgs e)
		{
			if (sites.Count == 0)
			{
				MessageBox.Show("Can't Add Component Materials until there is at Least one Site", "Factotum");
				btnAdd.Enabled = false;
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				return;
			}
			// Set the status combo first.  The selector DataGridView depends on it.
			cboStatusFilter.SelectedIndex = (int)FilterActiveStatus.ShowActive;
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(null);
			this.cboSiteFilter.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
			// Wire up the handler for the Entity changed event
			EComponentMaterial.Changed += new EventHandler<EntityChangedEventArgs>(EComponentMaterial_Changed);
			Globals.CurrentOutageChanged += new EventHandler(Globals_CurrentOutageChanged);
		}
		private void MaterialTypeView_FormClosed(object sender, FormClosedEventArgs e)
		{
			EComponentMaterial.Changed -= new EventHandler<EntityChangedEventArgs>(EComponentMaterial_Changed);
			Globals.CurrentOutageChanged -= new EventHandler(Globals_CurrentOutageChanged);
		}

		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EComponentMaterial_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		void Globals_CurrentOutageChanged(object sender, EventArgs e)
		{
			HandleEnablingForPropertySettings();
		}

		// Handle the user's decision to edit the current tool
		private void EditCurrentSelection()
		{
			// Make sure there's a row selected
			if (dgvMaterialTypeList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvMaterialTypeList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "MaterialTypeEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				MaterialTypeEdit frm = new MaterialTypeEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvMaterialTypeList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{
			MaterialTypeEdit frm = new MaterialTypeEdit(null,
				new Guid(cboSiteFilter.SelectedValue.ToString()));

			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvMaterialTypeList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a MaterialType to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvMaterialTypeList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "MaterialTypeEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EComponentMaterial ComponentMaterial = new EComponentMaterial(currentEditItem);
			ComponentMaterial.Delete(true);
			if (ComponentMaterial.CmpMaterialErrMsg != null)
			{
				MessageBox.Show(ComponentMaterial.CmpMaterialErrMsg, "Factotum");
				ComponentMaterial.CmpMaterialErrMsg = null;
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
			SortOrder sortOrder = dgvMaterialTypeList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvMaterialTypeList.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EComponentMaterial.GetDefaultDataView();
			dgvMaterialTypeList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvMaterialTypeList.Sort(dgvMaterialTypeList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvMaterialTypeList.Sort(dgvMaterialTypeList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);
		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvMaterialTypeList.Sort(dgvMaterialTypeList.Columns["CmpMaterialName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvMaterialTypeList.Columns["CmpMaterialCalBlockMaterial"].HeaderText = "Cal Block Material";
			dgvMaterialTypeList.Columns["CmpMaterialName"].HeaderText = "Material Type";
			dgvMaterialTypeList.Columns["CmpMaterialIsActive"].HeaderText = "Active";
			// Hide some columns
			dgvMaterialTypeList.Columns["ID"].Visible = false;
			dgvMaterialTypeList.Columns["CmpMaterialIsActive"].Visible = false;
			dgvMaterialTypeList.Columns["CmpMaterialIsLclChg"].Visible = false;
			dgvMaterialTypeList.Columns["CmpMaterialSitID"].Visible = false;
			dgvMaterialTypeList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvMaterialTypeList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("CmpMaterialIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			sb.Append(" And CmpMaterialSitID = '" + cboSiteFilter.SelectedValue +"'");
			if (txtNameFilter.Text.Length > 0)
				sb.Append(" And CmpMaterialName Like '" + txtNameFilter.Text + "*'");

			DataView dv = (DataView)dgvMaterialTypeList.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvMaterialTypeList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvMaterialTypeList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvMaterialTypeList.Rows[r].Cells["ID"].Value == id)
					{
						dgvMaterialTypeList.CurrentCell = dgvMaterialTypeList[firstCell.ColumnIndex, r];
						dgvMaterialTypeList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvMaterialTypeList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvMaterialTypeList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvMaterialTypeList.CurrentCell = firstCell;
				dgvMaterialTypeList.Rows[0].Selected = true;
			}
		}

		private void txtNameFilter_TextChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}

		private void cboSiteFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}

		private void HandleEnablingForPropertySettings()
		{
			if (Globals.CurrentOutageID != null)
			{
				Guid OutageID = (Guid)Globals.CurrentOutageID;
				EOutage outage = new EOutage(OutageID);
				EUnit unit = new EUnit(outage.OutageUntID);

				if (unit.UnitSitID != (Guid)cboSiteFilter.SelectedValue)
					cboSiteFilter.SelectedValue = unit.UnitSitID;
			}
			cboSiteFilter.Enabled = (Globals.IsMasterDB);
		}


	}
}