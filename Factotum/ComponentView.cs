using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class ComponentView : Form
	{
		// ----------------------------------------------------------------------
		// Initialization
		// ----------------------------------------------------------------------		
		ESystemCollection systems;
		ELineCollection lines;
		EUnitCollection units;
		// Form constructor
		public ComponentView()
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
			cboDataCompleteFilter.SelectedIndex = (int)FilterYesNoAll.ShowAll;
            if (Globals.CurrentOutageID != null)
            {
                EOutage otg = new EOutage((Guid?)Globals.CurrentOutageID);
                cboUnitFilter.SelectedValue = otg.OutageUntID;
            }

			FillUnits();
			HandleEnablingForPropertySettings();			
			FillSystems();
			FillLines();

		}

		// Set the status filter to show active tools by default
		// and update the tool selector combo box
		private void ComponentView_Load(object sender, EventArgs e)
		{
			if (units.Count == 0)
			{
				MessageBox.Show("Can't Add Components until there is at Least one Site and Unit", "Factotum");
				btnAdd.Enabled = false;
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnComponentListing.Enabled = false;
				return;
			}
            // set the text for the tool tip for btnAdd
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
			this.cboSystemFilter.SelectedIndexChanged += new System.EventHandler(this.cboSystemFilter_SelectedIndexChanged);
			this.cboLineFilter.SelectedIndexChanged += new System.EventHandler(this.cboLineFilter_SelectedIndexChanged);
			this.cboDataCompleteFilter.SelectedIndexChanged += new System.EventHandler(this.cboDataCompleteFilter_SelectedIndexChanged);
			this.cboInOutageFilter.SelectedIndexChanged += new System.EventHandler(this.cboInOutageFilter_SelectedIndexChanged);
			EComponent.Changed += new EventHandler<EntityChangedEventArgs>(EComponent_Changed);
			EUnit.Changed += new EventHandler<EntityChangedEventArgs>(EUnit_Changed);
			ESystem.Changed += new EventHandler<EntityChangedEventArgs>(ESystem_Changed);
			ELine.Changed += new EventHandler<EntityChangedEventArgs>(ELine_Changed);
			Globals.CurrentOutageChanged += new EventHandler(Globals_CurrentOutageChanged);

		}
		private void ComponentView_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.cboStatusFilter.SelectedIndexChanged -= new System.EventHandler(this.cboStatus_SelectedIndexChanged);
			this.txtNameFilter.TextChanged -= new System.EventHandler(this.txtNameFilter_TextChanged);
			this.cboUnitFilter.SelectedIndexChanged -= new System.EventHandler(this.cboUnitFilter_SelectedIndexChanged);
			this.cboSystemFilter.SelectedIndexChanged -= new System.EventHandler(this.cboSystemFilter_SelectedIndexChanged);
			this.cboLineFilter.SelectedIndexChanged -= new System.EventHandler(this.cboLineFilter_SelectedIndexChanged);
			this.cboDataCompleteFilter.SelectedIndexChanged -= new System.EventHandler(this.cboDataCompleteFilter_SelectedIndexChanged);
			this.cboInOutageFilter.SelectedIndexChanged -= new System.EventHandler(this.cboInOutageFilter_SelectedIndexChanged);
			EComponent.Changed -= new EventHandler<EntityChangedEventArgs>(EComponent_Changed);
			EUnit.Changed -= new EventHandler<EntityChangedEventArgs>(EUnit_Changed);
			ESystem.Changed -= new EventHandler<EntityChangedEventArgs>(ESystem_Changed);
			ELine.Changed -= new EventHandler<EntityChangedEventArgs>(ELine_Changed);
			Globals.CurrentOutageChanged -= new EventHandler(Globals_CurrentOutageChanged);
		}


		// ----------------------------------------------------------------------
		// Event Handlers
		// ----------------------------------------------------------------------		
		// If any of this type of entity object was saved or deleted, we want to update the selector
		// The event args contain the ID of the entity that was added, mofified or deleted.
		void EComponent_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector(e.ID);
		}

		void EUnit_Changed(object sender, EntityChangedEventArgs e)
		{
			FillUnits();
			FillSystems();
			FillLines();
			UpdateSelector(null);
		}

		void ELine_Changed(object sender, EntityChangedEventArgs e)
		{
			FillLines();
			UpdateSelector(null);
		}

		void ESystem_Changed(object sender, EntityChangedEventArgs e)
		{
			FillSystems();
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
			if (dgvComponentList.SelectedRows.Count != 1) return;

			Guid? currentEditItem = (Guid?)(dgvComponentList.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "ComponentEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				ComponentEdit frm = new ComponentEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		// This handles the datagridview double-click as well as button click
		void btnEdit_Click(object sender, System.EventArgs e)
		{
			EditCurrentSelection();
		}

		private void dgvComponentList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection();
		}

		// Handle the user's decision to add a new tool
		private void btnAdd_Click(object sender, EventArgs e)
		{

			ComponentEdit frm = new ComponentEdit(null, (Guid?)cboUnitFilter.SelectedValue);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Handle the user's decision to delete the selected tool
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (dgvComponentList.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Component to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvComponentList.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "ComponentEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EComponent Component = new EComponent(currentEditItem);
			Component.Delete(true);
			if (Component.ComponentErrMsg != null)
			{
				MessageBox.Show(Component.ComponentErrMsg, "Factotum");
				Component.ComponentErrMsg = null;
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

		private void cboDataCompleteFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}

		private void cboLineFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFilters();
		}

		private void cboSystemFilter_SelectedIndexChanged(object sender, EventArgs e)
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
			SortOrder sortOrder = dgvComponentList.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvComponentList.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EComponent.GetDefaultDataViewForUnit((Guid?)cboUnitFilter.SelectedValue);
			dgvComponentList.DataSource = dv;
			ApplyFilters();
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvComponentList.Sort(dgvComponentList.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvComponentList.Sort(dgvComponentList.Columns[sortCol], ListSortDirection.Descending);
			
			// Select the current row
			SelectGridRow(id);

		}

		private void CustomizeGrid()
		{
			// Apply a default sort
			dgvComponentList.Sort(dgvComponentList.Columns["ComponentName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvComponentList.Columns["ComponentName"].HeaderText = "Component Name";
			dgvComponentList.Columns["ComponentDrawing"].HeaderText = "Drawing";
			dgvComponentList.Columns["ComponentUpMainOd"].HeaderText = "U/S Main OD";
			dgvComponentList.Columns["ComponentUpMainTnom"].HeaderText = "U/S Main Tnom";
			dgvComponentList.Columns["ComponentUpMainTscr"].HeaderText = "U/S Main Tscr";
			dgvComponentList.Columns["ComponentDnMainOd"].HeaderText = "D/S Main OD";
			dgvComponentList.Columns["ComponentDnMainTnom"].HeaderText = "D/S Main Tnom";
			dgvComponentList.Columns["ComponentDnMainTscr"].HeaderText = "D/S Main Tscr";
			dgvComponentList.Columns["ComponentBranchOd"].HeaderText = "Branch OD";
			dgvComponentList.Columns["ComponentBranchTnom"].HeaderText = "Branch Tnom";
			dgvComponentList.Columns["ComponentBranchTscr"].HeaderText = "Branch Tscr";
			dgvComponentList.Columns["ComponentTimesInspected"].HeaderText = "Times Inspected";
			dgvComponentList.Columns["ComponentAvgInspectionTime"].HeaderText = "Avg Insp. Time";
			dgvComponentList.Columns["ComponentAvgInspectionTime"].DefaultCellStyle.Format = "0.00";
			dgvComponentList.Columns["ComponentAvgCrewDose"].HeaderText = "Avg Crew Dose";
			dgvComponentList.Columns["ComponentAvgCrewDose"].DefaultCellStyle.Format = "0.00";
			dgvComponentList.Columns["ComponentPctChromeMain"].HeaderText = "% Cr Main";
			dgvComponentList.Columns["ComponentPctChromeBranch"].HeaderText = "% Cr Branch";
			dgvComponentList.Columns["ComponentMisc1"].HeaderText = "Misc 1";
			dgvComponentList.Columns["ComponentMisc2"].HeaderText = "Misc 2";
			dgvComponentList.Columns["ComponentHighRad"].HeaderText = "High Rad";
			dgvComponentList.Columns["ComponentHardToAccess"].HeaderText = "Hard Access";
			dgvComponentList.Columns["ComponentIsActive"].HeaderText = "Active";
			dgvComponentList.Columns["ComponentDataComplete"].HeaderText = "Data Complete";
			dgvComponentList.Columns["ComponentHasDs"].HeaderText = "Has D/S Section";
			dgvComponentList.Columns["ComponentHasBranch"].HeaderText = "Has Branch";
			dgvComponentList.Columns["ComponentMaterial"].HeaderText = "Material";
			dgvComponentList.Columns["ComponentType"].HeaderText = "Type";
			dgvComponentList.Columns["ComponentLine"].HeaderText = "Line";
			dgvComponentList.Columns["ComponentSystem"].HeaderText = "System";
			dgvComponentList.Columns["ComponentSchedule"].HeaderText = "Schedule";
			dgvComponentList.Columns["ComponentReportName"].HeaderText = "Report Name";
			dgvComponentList.Columns["ComponentWorkOrder"].HeaderText = "Work Order";
			dgvComponentList.Columns["ComponentInOutage"].HeaderText = "In This Outage";
			// Hide some columns
			dgvComponentList.Columns["ID"].Visible = false;
			dgvComponentList.Columns["ComponentSystemID"].Visible = false;
			dgvComponentList.Columns["ComponentLineID"].Visible = false;
			dgvComponentList.Columns["ComponentIsActive"].Visible = false;
			dgvComponentList.Columns["ComponentInOutage"].Visible = false;
			dgvComponentList.Columns["ComponentDataComplete"].Visible = false;
			dgvComponentList.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Apply the current filters to the DataView.  The DataGridView will auto-refresh.
		private void ApplyFilters()
		{
			if (dgvComponentList.DataSource == null) return;
			StringBuilder sb = new StringBuilder("ComponentIsActive = ", 255);
			sb.Append(cboStatusFilter.SelectedIndex == (int)FilterActiveStatus.ShowActive ? "'Yes'" : "'No'");
			if (txtNameFilter.Text.Length > 0)
				sb.Append(" And ComponentName Like '" + txtNameFilter.Text + "*'");
			if (cboDataCompleteFilter.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And ComponentDataComplete = " +
					(cboDataCompleteFilter.SelectedIndex == (int)FilterYesNoAll.Yes ? "'Yes'" : "'No'"));
			if (cboInOutageFilter.SelectedIndex != (int)FilterYesNoAll.ShowAll)
				sb.Append(" And ComponentInOutage = " +
					(cboInOutageFilter.SelectedIndex == (int)FilterYesNoAll.Yes ? "'Yes'" : "'No'"));
			if ((Guid?)cboSystemFilter.SelectedValue != null)
				sb.Append(" And ComponentSystemID = '" + cboSystemFilter.SelectedValue + "'");
			if ((Guid?)cboLineFilter.SelectedValue != null)
				sb.Append(" And ComponentLineID = '" + cboLineFilter.SelectedValue + "'");

			DataView dv = (DataView)dgvComponentList.DataSource;
			dv.RowFilter = sb.ToString();

		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(Guid? id)
		{
			bool found = false;
			int rows = dgvComponentList.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgvComponentList.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgvComponentList.Rows[r].Cells["ID"].Value == id)
					{
						dgvComponentList.CurrentCell = dgvComponentList[firstCell.ColumnIndex, r];
						dgvComponentList.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgvComponentList.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgvComponentList.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgvComponentList.CurrentCell = firstCell;
				dgvComponentList.Rows[0].Selected = true;
			}
		}

		private void FillUnits()
		{
			units = EUnit.ListByName(false, false);
			cboUnitFilter.DataSource = units;
			cboUnitFilter.DisplayMember = "UnitNameWithSite";
			cboUnitFilter.ValueMember = "ID";

		}

		private void FillSystems()
		{
			systems = ESystem.ListForUnit((Guid?)cboUnitFilter.SelectedValue, false, true);
			systems[0].SystemName = "All";
			cboSystemFilter.DataSource = systems;
			cboSystemFilter.DisplayMember = "SystemName";
			cboSystemFilter.ValueMember = "ID";
		}

		private void FillLines()
		{
			lines = ELine.ListForUnit((Guid?)cboUnitFilter.SelectedValue, false, true);
			lines[0].LineName = "All";
			cboLineFilter.DataSource = lines;
			cboLineFilter.DisplayMember = "LineName";
			cboLineFilter.ValueMember = "ID";
		}

        private void UpdateAddToolTip()
        {
            if (cboUnitFilter.Text == null)
                toolTip1.SetToolTip(btnAdd, "");
            else
                toolTip1.SetToolTip(btnAdd, "Add a component for unit: " + cboUnitFilter.Text);
        }

        private void HandleUnitFilterChanged()
		{
			FillSystems();
			FillLines();
			UpdateSelector(null);
            UpdateAddToolTip();
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
				cboInOutageFilter.SelectedIndex = (int)FilterYesNoAll.ShowAll;
			}
			else
			{
				cboUnitFilter.Enabled = false;
				cboInOutageFilter.SelectedIndex = (int)FilterYesNoAll.Yes;
			}
		}

		private void btnComponentListing_Click(object sender, EventArgs e)
		{
			ComponentListing frm = new ComponentListing();
			frm.Show();
		}


	}
}