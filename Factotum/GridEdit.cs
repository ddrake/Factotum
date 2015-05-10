using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DowUtils;

namespace Factotum
{
	public partial class GridEdit : Form, IEntityEditForm
	{
		private EGrid curGrid;
		private EComponent curComponent;
		private DataTable dsetAssignments;
		private DataTable curMeasurements;
		private GridDivider[] gridDividersUpExt, gridDividersUpMain, gridDividersDnMain, gridDividersDnExt,
			gridDividersBranch, gridDividersBranchExt, gridDividersPost;

		EGridSizeCollection gridSizes;
		EGridComboItemCollection parentGrids;
		private int curGridRows;
		private bool savedInternally;
		private bool newRecord;
		// Used to prevent a tab-select triggered refresh if a checked-listbox 
		// triggered refresh is already in progress.
		private bool refreshInProgress;
		// This is set if the user checks any of the boxes in the checked listbox.
		// We check to see if this is set when the user leaves the clb control.
		private bool assignmentsChanged;
		// This is set initially.  The idea is that the user may want to open the grid
		// and edit some parameters that don't require the grid to be filled or stats
		// calculated.  If this flag is set and the user then decides 
		// to click one of those tabs, we recalculate before displaying..
		private bool gridAndStatsNotYetDisplayed;
        
        // Flag whether to create a merged grid
        private bool createMergedGrid = false;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curGrid; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public GridEdit(Guid? ID)
			: this(ID, null){}

		public GridEdit(Guid? ID, Guid? inspectionID)
		{
			InitializeComponent();
			curGrid = new EGrid();
			curGrid.Load(ID);
			if (inspectionID != null) curGrid.GridIspID = inspectionID;
			curComponent = new EComponent(curGrid.ComponentID);
			newRecord = (ID == null);
			InitializeControls();
			// Hook these up after the form data is initialized.
			cboGridSize.SelectedIndexChanged += new System.EventHandler(cboGridSize_SelectedIndexChanged);
			cboParentGrid.SelectedIndexChanged += new System.EventHandler(cboParentGrid_SelectedIndexChanged);
			cboUpExtPreDivider.SelectedIndexChanged += new EventHandler(cboUpExtPreDivider_SelectedIndexChanged);
			cboUpMainPreDivider.SelectedIndexChanged += new EventHandler(cboUpMainPreDivider_SelectedIndexChanged);
			cboDnMainPreDivider.SelectedIndexChanged += new EventHandler(cboDnMainPreDivider_SelectedIndexChanged);
			cboDnExtPreDivider.SelectedIndexChanged += new EventHandler(cboDnExtPreDivider_SelectedIndexChanged);
			cboBranchPreDivider.SelectedIndexChanged += new EventHandler(cboBranchPreDivider_SelectedIndexChanged);
			cboBranchExtPreDivider.SelectedIndexChanged += new EventHandler(cboBranchExtPreDivider_SelectedIndexChanged);
			cboPostDivider.SelectedIndexChanged += new EventHandler(cboPostDivider_SelectedIndexChanged);
			clbDatasets.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbDatasets_ItemCheck);
			clbDatasets.Validated += new EventHandler(clbDatasets_Validated);
			savedInternally = false;
		}

		private void GridEdit_FormClosed(object sender, FormClosedEventArgs e)
		{
			cboGridSize.SelectedIndexChanged -= new System.EventHandler(cboGridSize_SelectedIndexChanged);
			cboParentGrid.SelectedIndexChanged -= new System.EventHandler(cboParentGrid_SelectedIndexChanged);
			cboUpExtPreDivider.SelectedIndexChanged -= new System.EventHandler(cboUpExtPreDivider_SelectedIndexChanged);
			cboUpMainPreDivider.SelectedIndexChanged -= new System.EventHandler(cboUpMainPreDivider_SelectedIndexChanged);
			cboDnMainPreDivider.SelectedIndexChanged -= new System.EventHandler(cboDnMainPreDivider_SelectedIndexChanged);
			cboDnExtPreDivider.SelectedIndexChanged -= new System.EventHandler(cboDnExtPreDivider_SelectedIndexChanged);
			cboBranchPreDivider.SelectedIndexChanged -= new System.EventHandler(cboBranchPreDivider_SelectedIndexChanged);
			cboBranchExtPreDivider.SelectedIndexChanged -= new System.EventHandler(cboBranchExtPreDivider_SelectedIndexChanged);
			cboPostDivider.SelectedIndexChanged -= new System.EventHandler(cboPostDivider_SelectedIndexChanged);

			clbDatasets.ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(this.clbDatasets_ItemCheck);
			clbDatasets.Validated -= new EventHandler(clbDatasets_Validated);
		}

		// If the user is finished assigning datasets to the grid, try to save and rebuild the grid
		// and refresh the statistics.  Note: the validated event is triggered when the focus leaves 
		// the checked listbox whether or not any of the checkboxes were clicked.
		void clbDatasets_Validated(object sender, EventArgs e)
		{
			if (assignmentsChanged)
			{
				// Update the database with the current dataset assignments to the current grid.
				// This function will try to perform a silent save if this is a new record
				if (!UpdateDsetAssignmentsToGrid()) return;

				assignmentsChanged = false;

				curGrid.UpdateSectionInfoVars();

				TryToRebuildGridAndStats();
			}
		}

		// Update the GridCells table, rebuild the datagridview, get the dataTable, add data from
		// the dataTable to the datagridview, update the stats.
		// This just needs to be done if: a) we just opened the form and clicked either the grid tab
		// or the stats tab --- b) we changes the dataset assignments.
		// It does NOT need to be done when the partition info changes.  
		void TryToRebuildGridAndStats()
		{
			// set this flag in case we need to prevent certain actions while we're refreshing...
			refreshInProgress = true;
			toolStripStatusLabel1.Text = "Refreshing Grid...";
			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Value = 10;

			// Update the table that is used to eliminate duplicate values for a given grid cell
			// by using the higher priority Dset.
			// We don't really need to do this when the form loads, but it's pretty fast...
			curGrid.UpdateGridCellsDbTable();

			toolStripProgressBar1.Value = 20;

			// Rebuild the DataGridView, adding the right number of rows and columns and filling in
			// row and column headings.

			SetupDataGridView();
			toolStripProgressBar1.Value = 30;

			// Fill in the stats
			GetStats();

			// Get the curMeasurements DataTable.  This takes awhile.
			// when finished, it fills in the DataGridView
			backgroundWorker1.RunWorkerAsync();
		}

		// If the user is finished changing partition info, 
		// try to save and update the section info variables in the EGrid
		// which will be used for the conditional formatting.
		// The validating event is triggered whenever the user leaves a control, whether they 
		// change anything or not.
		void handlePartitionTextboxChange()
		{

			if	(this.ActiveControl.Parent != this.Sections)
			{
				if (!performSilentSave()) return;
				curGrid.UpdateSectionInfoVars();
				// We need to update the stats because the number of mins may change by the new partitioning.
				GetStats();
			}
		}

		private void InitializeDividerCombo(ComboBox cbo, ref GridDivider[] div)
		{
            div = EGrid.GetGridDividerArray();
            cbo.DataSource = div;
			cbo.DisplayMember = "Name";
			cbo.ValueMember = "ID";
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			// Grid Sizes combo box
			gridSizes = EGridSize.ListByName(!newRecord, true);
			cboGridSize.DataSource = gridSizes;
			cboGridSize.DisplayMember = "GridSizeName";
			cboGridSize.ValueMember = "ID";

			// Radial Locations combo box
			ERadialLocationCollection radialLocations = ERadialLocation.ListByName(!newRecord, true);
			cboRadialLocation.DataSource = radialLocations;
			cboRadialLocation.DisplayMember = "RadialLocationName";
			cboRadialLocation.ValueMember = "ID";

			// Parent grid combo box
			parentGrids = EGridComboItem.ListEligibleParentGridsForReportByInspectionName(
				(Guid)curGrid.InspectedComponentID, curGrid.ID, true);
			cboParentGrid.DataSource = parentGrids;
			cboParentGrid.DisplayMember = "GridName";
			cboParentGrid.ValueMember = "ID";

			// Divider combo boxes
			InitializeDividerCombo(cboUpExtPreDivider, ref gridDividersUpExt);
			InitializeDividerCombo(cboUpMainPreDivider, ref gridDividersUpMain);
			InitializeDividerCombo(cboDnMainPreDivider, ref gridDividersDnMain);
			InitializeDividerCombo(cboDnExtPreDivider, ref gridDividersDnExt);
			InitializeDividerCombo(cboBranchPreDivider, ref gridDividersBranch);
			InitializeDividerCombo(cboBranchExtPreDivider, ref gridDividersBranchExt);
			InitializeDividerCombo(cboPostDivider, ref gridDividersPost);

			// Dset CheckedListbox
			dsetAssignments = EDset.GetWithGridAssignmentsForInspection((Guid)curGrid.GridIspID);
			int rows = dsetAssignments.Rows.Count;
			if (rows == 0)
			{
				clbDatasets.Visible = false;
			}
			else
			{
				for (int dmRow = 0; dmRow < dsetAssignments.Rows.Count; dmRow++)
				{
					string dsetName = (string)dsetAssignments.Rows[dmRow]["DsetName"];
					bool isAssigned = Convert.ToBoolean(dsetAssignments.Rows[dmRow]["IsAssigned"]);
					clbDatasets.Items.Add(dsetName, isAssigned);
				}
			}
			// set a flag to force building the grid and stats the first time the user selects them
			gridAndStatsNotYetDisplayed = true;

			DisEnableRowTextboxesForComponent();
			SetControlValues();
			this.Text = newRecord ? "New Grid" : "Edit Grid";
			if (newRecord)
			{
				EOutage curOutage = new EOutage(Globals.CurrentOutageID);
				ckGridColumnLayoutCCW.Checked = curOutage.OutageGridColDefaultCCW;
			}
		}

		// Todo: what if the user has set the thicknesses for the component, come here and entered
		// values then cleared thicknesses for the component?...
		private void DisEnableRowTextboxesForComponent()
		{
			// these three need to be enabled whether or not we have a downstream section or branch
			txtUpExtStart.Enabled = txtUpExtEnd.Enabled = curComponent.UpMainThicknessesDefined;
			txtUsMainStart.Enabled = txtUsMainEnd.Enabled = curComponent.UpMainThicknessesDefined;
			txtDsExtStart.Enabled = txtDsExtEnd.Enabled = curComponent.UpMainThicknessesDefined;

			txtDsMainStart.Enabled = txtDsMainEnd.Enabled = curComponent.DnMainThicknessesDefined;
			cboDnMainPreDivider.Enabled = curComponent.DnMainThicknessesDefined;
			txtBranchStart.Enabled = txtBranchEnd.Enabled = curComponent.BranchThicknessesDefined;
			txtBranchExtStart.Enabled = txtBranchExtEnd.Enabled = curComponent.BranchThicknessesDefined;
			cboBranchExtPreDivider.Enabled = cboBranchPreDivider.Enabled = curComponent.BranchThicknessesDefined;

		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		// If the user cancels out, just close.
		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
			DialogResult = savedInternally ? DialogResult.OK : DialogResult.Cancel;
		}

		// If the user clicks OK, first validate and set the error text 
		// for any controls with invalid values.
		// If it validates, try to save.
		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveAndClose();
		}

		// The user changed the selected Grid Size
		// If they set it to "<No Selection>", I think it's best to clear the axial and radial dists.
		// If they set it to a real Grid Size, we should fill in the axial and radial textboxes.
		private void cboGridSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboGridSize.SelectedIndex == 0)
			{
				txtAxialDistance.Text = null;
				txtRadialDistance.Text = null;
			}
			EGridSize gridSize = FindGridSizeForId((Guid?)cboGridSize.SelectedValue);
			if (gridSize != null)
			{
				txtAxialDistance.Text = GetFormattedGridSize(gridSize.GridSizeAxialDistance);
				txtRadialDistance.Text = GetFormattedGridSize(gridSize.GridSizeRadialDistance);
			}

		}

		// If the user changed the parent grid combo selection, refresh the items in the 
		// ParentColRef and ParentRowRef combos.
		private void cboParentGrid_SelectedIndexChanged(object sender, EventArgs e)
		{
			ManageParentRefCombos();
		}

		void cboPostDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void cboBranchExtPreDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void cboBranchPreDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void cboDnExtPreDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void cboDnMainPreDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void cboUpMainPreDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		void cboUpExtPreDivider_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		// Whenever a grid column is added to the DataGridView, make it not sortable
		private void dgvGrid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		// Get the curMeasurements DataTable.  This takes awhile.
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			curMeasurements = EMeasurement.GetForGrid(curGrid.ID);
		}

		// We're finished re-generating the curMeasurements DataTable, so fill in the DataGridView
		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			toolStripStatusLabel1.Text = "Refreshing Grid...";
			toolStripProgressBar1.Value = 90;
			AddMeasurementsToGrid();
			toolStripProgressBar1.Value = 100;
			toolStripStatusLabel1.Text = "Ready";
			toolStripProgressBar1.ProgressBar.Value = 0;
			EInspectedComponent curReport = new EInspectedComponent(curGrid.InspectedComponentID);
			curReport.UpdateMinCount();
			curReport.Save();
			gridAndStatsNotYetDisplayed = false;
			refreshInProgress = false;
		}

		private bool performSilentSave()
		{
			// we need to do a 'silent save'
			if (AnyControlErrors())
			{
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curGrid.Valid())
			{
				setAllErrors();
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curGrid.Save();
			if (tmpID == null) return false;
			savedInternally = true;
			return true;
		}

		// If the user just clicked the grid or stats tab, build the grid and stats if 
		// they haven't been displayed yet unless there is already a refresh operation in progress
		// Note: a refresh operation can be caused by the user clicking off of the checked listbox.
		private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
		{
			string tabName = e.TabPage.Name;
			if (gridAndStatsNotYetDisplayed && !refreshInProgress &&
				(tabName == "ViewGrid" || tabName == "Stats"))
			{
				// Update the database with the current dataset assignments to the current grid.
				// This function will try to perform a silent save if this is a new record
				if (!performSilentSave())
				{
					e.Cancel = true;
					return;
				}
				curGrid.UpdateSectionInfoVars();

				TryToRebuildGridAndStats();			
			}
		}

		// If the user checked to add or remove a dataset from the grid, set a flag
		// We'll use this flag in the Validated event handler to regenerate the grid data.
		private void clbDatasets_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			assignmentsChanged = true;
		}

		private void dgvGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			decimal val;
			// Ignore empty cells
			if (e.Value == null)
			{
				return;
			}
			// We don't care about the row and column headings
			if (e.RowIndex >= 0 && e.ColumnIndex > 0)
			{
				bool sel = (e.State & DataGridViewElementStates.Selected) > 0;
				// Special formatting for obstructed cells
				if (e.Value.ToString() == "obst.")
				{
					paintCell(ref e, Brushes.Purple, Brushes.AliceBlue);
					return;
				}
				// Some kind of mystery value...
				if (!Decimal.TryParse(e.Value.ToString(), out val))
				{
					paintCell(ref e, Brushes.Magenta, Brushes.Yellow);
					return;
				}
				ThicknessRange range = curGrid.GetThicknessRange(e.RowIndex, val);
				switch (range)
				{
					case ThicknessRange.BelowTscreen:
						paintCell(ref e, Brushes.Crimson, Brushes.Pink);
						break;
					case ThicknessRange.TscreenToTnom:
						paintCell(ref e, Brushes.Green, Brushes.Pink);
						break;
					case ThicknessRange.TnomTo120pct:
						paintCell(ref e, Brushes.Black, Brushes.Pink);
						break;
					case ThicknessRange.Above120pctTnom:
						paintCell(ref e, Brushes.Blue, Brushes.Pink);
						break;
					case ThicknessRange.Unknown:
						paintCell(ref e, Brushes.Magenta, Brushes.Yellow);
						break;
				}
			}
		}

		// Paint a grid cell with the specified foreground color.
		// The color depends on whether it's selected or not.
		private void paintCell(ref DataGridViewCellPaintingEventArgs e, Brush regColor, Brush selColor)
		{
			bool sel = (e.State & DataGridViewElementStates.Selected) > 0;
			e.PaintBackground(e.CellBounds, sel);
			e.Graphics.DrawString((String)e.Value, e.CellStyle.Font,
				sel ? selColor : regColor, e.CellBounds.X + 2,
				e.CellBounds.Y + 2, StringFormat.GenericDefault);
			e.Handled = true;
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtAxialDistance_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridAxialDistanceLengthOk(txtAxialDistance.Text);
			errorProvider1.SetError(txtAxialDistance, curGrid.GridAxialDistanceErrMsg);
		}

		private void txtRadialDistance_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridRadialDistanceLengthOk(txtRadialDistance.Text);
			errorProvider1.SetError(txtRadialDistance, curGrid.GridRadialDistanceErrMsg);
		}

		private void txtAxialLocationOverride_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridAxialLocOverrideLengthOk(txtAxialLocationOverride.Text);
			errorProvider1.SetError(txtAxialLocationOverride, curGrid.GridAxialLocOverrideErrMsg);
		}

		private void txtUpExtStart_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridUpExtStartRowLengthOk(txtUpExtStart.Text);
			errorProvider1.SetError(txtUpExtStart, curGrid.GridUpExtStartRowErrMsg);
		}

		private void txtUpExtEnd_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridUpExtEndRowLengthOk(txtUpExtEnd.Text);
			errorProvider1.SetError(txtUpExtEnd, curGrid.GridUpExtEndRowErrMsg);
		}

		private void txtUsMainStart_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridUpMainStartRowLengthOk(txtUsMainStart.Text);
			errorProvider1.SetError(txtUsMainStart, curGrid.GridUpMainStartRowErrMsg);
		}

		private void txtUsMainEnd_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridUpMainEndRowLengthOk(txtUsMainEnd.Text);
			errorProvider1.SetError(txtUsMainEnd, curGrid.GridUpMainEndRowErrMsg);
		}

		private void txtDsMainStart_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridDnMainStartRowLengthOk(txtDsMainStart.Text);
			errorProvider1.SetError(txtDsMainStart, curGrid.GridDnMainStartRowErrMsg);
		}

		private void txtDsMainEnd_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridDnMainEndRowLengthOk(txtDsMainEnd.Text);
			errorProvider1.SetError(txtDsMainEnd, curGrid.GridDnMainEndRowErrMsg);
		}

		private void txtDsExtStart_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridDnExtStartRowLengthOk(txtDsExtStart.Text);
			errorProvider1.SetError(txtDsExtStart, curGrid.GridDnExtStartRowErrMsg);
		}

		private void txtDsExtEnd_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridDnExtEndRowLengthOk(txtDsExtEnd.Text);
			errorProvider1.SetError(txtDsExtEnd, curGrid.GridDnExtEndRowErrMsg);
		}

		private void txtBranchStart_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridBranchStartRowLengthOk(txtBranchStart.Text);
			errorProvider1.SetError(txtBranchStart, curGrid.GridBranchStartRowErrMsg);
		}

		private void txtBranchEnd_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridBranchEndRowLengthOk(txtBranchEnd.Text);
			errorProvider1.SetError(txtBranchEnd, curGrid.GridBranchEndRowErrMsg);
		}

		private void txtBranchExtStart_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridBranchExtStartRowLengthOk(txtBranchExtStart.Text);
			errorProvider1.SetError(txtBranchExtStart, curGrid.GridBranchExtStartRowErrMsg);
		}

		private void txtBranchExtEnd_TextChanged(object sender, EventArgs e)
		{
			curGrid.GridBranchExtEndRowLengthOk(txtBranchExtEnd.Text);
			errorProvider1.SetError(txtBranchExtEnd, curGrid.GridBranchExtEndRowErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtAxialDistance_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridAxialDistanceValid(txtAxialDistance.Text);
			errorProvider1.SetError(txtAxialDistance, curGrid.GridAxialDistanceErrMsg);
			SetGridSizeComboForTextChange();
		}

		private void txtRadialDistance_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridRadialDistanceValid(txtRadialDistance.Text);
			errorProvider1.SetError(txtRadialDistance, curGrid.GridRadialDistanceErrMsg);
			SetGridSizeComboForTextChange();
		}

		private void txtAxialLocationOverride_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridAxialLocOverrideValid(txtAxialLocationOverride.Text);
			errorProvider1.SetError(txtAxialLocationOverride, curGrid.GridAxialLocOverrideErrMsg);
		}

		private void txtUpExtStart_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridUpExtStartRowValid(txtUpExtStart.Text);
			errorProvider1.SetError(txtUpExtStart, curGrid.GridUpExtStartRowErrMsg);
			handlePartitionTextboxChange();		
		}

		private void txtUpExtEnd_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridUpExtEndRowValid(txtUpExtEnd.Text);
			errorProvider1.SetError(txtUpExtEnd, curGrid.GridUpExtEndRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtUsMainStart_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridUpMainStartRowValid(txtUsMainStart.Text);
			errorProvider1.SetError(txtUsMainStart, curGrid.GridUpMainStartRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtUsMainEnd_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridUpMainEndRowValid(txtUsMainEnd.Text);
			errorProvider1.SetError(txtUsMainEnd, curGrid.GridUpMainEndRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtDsMainStart_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridDnMainStartRowValid(txtDsMainStart.Text);
			errorProvider1.SetError(txtDsMainStart, curGrid.GridDnMainStartRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtDsMainEnd_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridDnMainEndRowValid(txtDsMainEnd.Text);
			errorProvider1.SetError(txtDsMainEnd, curGrid.GridDnMainEndRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtDsExtStart_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridDnExtStartRowValid(txtDsExtStart.Text);
			errorProvider1.SetError(txtDsExtStart, curGrid.GridDnExtStartRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtDsExtEnd_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridDnExtEndRowValid(txtDsExtEnd.Text);
			errorProvider1.SetError(txtDsExtEnd, curGrid.GridDnExtEndRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtBranchStart_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridBranchStartRowValid(txtBranchStart.Text);
			errorProvider1.SetError(txtBranchStart, curGrid.GridBranchStartRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtBranchEnd_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridBranchEndRowValid(txtBranchEnd.Text);
			errorProvider1.SetError(txtBranchEnd, curGrid.GridBranchEndRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtBranchExtStart_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridBranchExtStartRowValid(txtBranchExtStart.Text);
			errorProvider1.SetError(txtBranchExtStart, curGrid.GridBranchExtStartRowErrMsg);
			handlePartitionTextboxChange();
		}

		private void txtBranchExtEnd_Validating(object sender, CancelEventArgs e)
		{
			curGrid.GridBranchExtEndRowValid(txtBranchExtEnd.Text);
			errorProvider1.SetError(txtBranchExtEnd, curGrid.GridBranchExtEndRowErrMsg);
			handlePartitionTextboxChange();
		}


		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------

		// No prompting is performed.  The user should understand the meanings of OK and Cancel.
		private void SaveAndClose()
		{
			if (AnyControlErrors()) return;
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curGrid.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curGrid.Save();
			if (tmpID != null)
			{
				// We need to do these updates after saving because they require a valid Grid ID

				// Update Inspectors table from checkboxes
				for (int dmRow = 0; dmRow < dsetAssignments.Rows.Count; dmRow++)
				{
					dsetAssignments.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbDatasets.CheckedIndices)
				{
					dsetAssignments.Rows[idx]["IsAssigned"] = 1;
				}
				EDset.UpdateAssignmentsToGrid((Guid)curGrid.ID, dsetAssignments);

				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the grid object values.
		private void SetControlValues()
		{
			if (curGrid.GridIspID != null)
			{
				EInspection inspection = new EInspection((Guid?)curGrid.GridIspID);
				EInspectedComponent inspectedComponent =
					new EInspectedComponent(inspection.InspectionIscID);
				lblSiteName.Text = "Grid for Inspection '" +
					inspection.InspectionName + "' of Report '" + inspectedComponent.InspComponentName + "'";
			}
			else lblSiteName.Text = "Grid for Unknown Inspection";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			txtAxialDistance.Text = GetFormattedGridSize(curGrid.GridAxialDistance);
			txtRadialDistance.Text = GetFormattedGridSize(curGrid.GridRadialDistance);
			txtAxialLocationOverride.Text = curGrid.GridAxialLocOverride;
			// User wants rows to start at 1
			txtUpExtStart.Text = GetFormattedRow(curGrid.GridUpExtStartRow);
			txtUpExtEnd.Text = GetFormattedRow(curGrid.GridUpExtEndRow);
			txtUsMainStart.Text = GetFormattedRow(curGrid.GridUpMainStartRow);
			txtUsMainEnd.Text = GetFormattedRow(curGrid.GridUpMainEndRow);
			txtDsMainStart.Text = GetFormattedRow(curGrid.GridDnMainStartRow);
			txtDsMainEnd.Text = GetFormattedRow(curGrid.GridDnMainEndRow);
			txtDsExtStart.Text = GetFormattedRow(curGrid.GridDnExtStartRow);
			txtDsExtEnd.Text = GetFormattedRow(curGrid.GridDnExtEndRow);
			txtBranchStart.Text = GetFormattedRow(curGrid.GridBranchStartRow);
			txtBranchEnd.Text = GetFormattedRow(curGrid.GridBranchEndRow);
			txtBranchExtStart.Text = GetFormattedRow(curGrid.GridBranchExtStartRow);
			txtBranchExtEnd.Text = GetFormattedRow(curGrid.GridBranchExtEndRow);

			if (curGrid.GridGszID == null) cboGridSize.SelectedIndex = 0;
			else cboGridSize.SelectedValue = curGrid.GridGszID;

			if (curGrid.GridRdlID == null) cboRadialLocation.SelectedIndex = 0;
			else cboRadialLocation.SelectedValue = curGrid.GridRdlID;

			if (curGrid.GridParentID == null) cboParentGrid.SelectedIndex = 0;
			else cboParentGrid.SelectedValue = curGrid.GridParentID;

			// Set by default to N/A for new records
			cboUpExtPreDivider.SelectedValue = curGrid.GridUpExtPreDivider;
			cboUpMainPreDivider.SelectedValue = curGrid.GridUpMainPreDivider;
			cboDnMainPreDivider.SelectedValue = curGrid.GridDnMainPreDivider;
			cboDnExtPreDivider.SelectedValue = curGrid.GridDnExtPreDivider;
			cboBranchPreDivider.SelectedValue = curGrid.GridBranchPreDivider;
			cboBranchExtPreDivider.SelectedValue = curGrid.GridBranchExtPreDivider;
			cboPostDivider.SelectedValue = curGrid.GridPostDivider;

			ManageParentRefCombos();

            if (curGrid.GridParentStartCol != null)
                cboParentColRef.SelectedIndex = (int)curGrid.GridParentStartCol;

            if (curGrid.GridParentStartRow != null) 
                cboParentRowRef.SelectedIndex = (int)curGrid.GridParentStartRow;

            ckFullScan.Checked = curGrid.GridIsFullScan;
			ckGridColumnLayoutCCW.Checked = curGrid.GridIsColumnCCW;
			ckHideColumnLayoutGraphic.Checked = curGrid.GridHideColumnLayoutGraphic;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtAxialDistance, curGrid.GridAxialDistanceErrMsg);
			errorProvider1.SetError(txtRadialDistance, curGrid.GridRadialDistanceErrMsg);
			errorProvider1.SetError(txtAxialLocationOverride, curGrid.GridAxialLocOverrideErrMsg);
			errorProvider1.SetError(txtUpExtStart, curGrid.GridUpExtStartRowErrMsg);
			errorProvider1.SetError(txtUpExtEnd, curGrid.GridUpExtEndRowErrMsg);
			errorProvider1.SetError(txtUsMainStart, curGrid.GridUpMainStartRowErrMsg);
			errorProvider1.SetError(txtUsMainEnd, curGrid.GridUpMainEndRowErrMsg);
			errorProvider1.SetError(txtDsMainStart, curGrid.GridDnMainStartRowErrMsg);
			errorProvider1.SetError(txtDsMainEnd, curGrid.GridDnMainEndRowErrMsg);
			errorProvider1.SetError(txtDsExtStart, curGrid.GridDnExtStartRowErrMsg);
			errorProvider1.SetError(txtDsExtEnd, curGrid.GridDnExtEndRowErrMsg);
			errorProvider1.SetError(txtBranchStart, curGrid.GridBranchStartRowErrMsg);
			errorProvider1.SetError(txtBranchEnd, curGrid.GridBranchEndRowErrMsg);
			errorProvider1.SetError(txtBranchExtStart, curGrid.GridBranchExtStartRowErrMsg);
			errorProvider1.SetError(txtBranchExtEnd, curGrid.GridBranchExtEndRowErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtAxialDistance).Length > 0 ||
				errorProvider1.GetError(txtRadialDistance).Length > 0 || 
				errorProvider1.GetError(txtAxialLocationOverride).Length > 0 ||
				errorProvider1.GetError(txtUpExtStart).Length > 0 ||
				errorProvider1.GetError(txtUpExtEnd).Length > 0 ||
				errorProvider1.GetError(txtUsMainStart).Length > 0 ||
				errorProvider1.GetError(txtUsMainEnd).Length > 0 ||
				errorProvider1.GetError(txtDsMainStart).Length > 0 ||
				errorProvider1.GetError(txtDsMainEnd).Length > 0 ||
				errorProvider1.GetError(txtDsExtStart).Length > 0 ||
				errorProvider1.GetError(txtDsExtEnd).Length > 0 ||
				errorProvider1.GetError(txtBranchStart).Length > 0 ||
				errorProvider1.GetError(txtBranchEnd).Length > 0 ||
				errorProvider1.GetError(txtBranchExtStart).Length > 0 ||
				errorProvider1.GetError(txtBranchExtEnd).Length > 0
				);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curGrid.GridAxialDistance = Util.GetNullableDecimalForString(txtAxialDistance.Text);
			curGrid.GridRadialDistance = Util.GetNullableDecimalForString(txtRadialDistance.Text);
			curGrid.GridAxialLocOverride = txtAxialLocationOverride.Text;
			curGrid.GridUpExtStartRow = GetRowNumberForString(txtUpExtStart.Text);
			curGrid.GridUpExtEndRow = GetRowNumberForString(txtUpExtEnd.Text);
			curGrid.GridUpMainStartRow = GetRowNumberForString(txtUsMainStart.Text);
			curGrid.GridUpMainEndRow = GetRowNumberForString(txtUsMainEnd.Text);
			curGrid.GridDnMainStartRow = GetRowNumberForString(txtDsMainStart.Text);
			curGrid.GridDnMainEndRow = GetRowNumberForString(txtDsMainEnd.Text);
			curGrid.GridDnExtStartRow = GetRowNumberForString(txtDsExtStart.Text);
			curGrid.GridDnExtEndRow = GetRowNumberForString(txtDsExtEnd.Text);
			curGrid.GridBranchStartRow = GetRowNumberForString(txtBranchStart.Text);
			curGrid.GridBranchEndRow = GetRowNumberForString(txtBranchEnd.Text);
			curGrid.GridBranchExtStartRow = GetRowNumberForString(txtBranchExtStart.Text);
			curGrid.GridBranchExtEndRow = GetRowNumberForString(txtBranchExtEnd.Text);
			curGrid.GridGszID = (Guid?)cboGridSize.SelectedValue;
			curGrid.GridRdlID = (Guid?)cboRadialLocation.SelectedValue;
			curGrid.GridParentID = (Guid?)cboParentGrid.SelectedValue;
			if (cboParentColRef.Items.Count > 0) 
				curGrid.GridParentStartCol = (short)cboParentColRef.SelectedIndex;
			if (cboParentRowRef.Items.Count > 0)
				curGrid.GridParentStartRow = (short)cboParentRowRef.SelectedIndex;

			curGrid.GridUpExtPreDivider = (byte)cboUpExtPreDivider.SelectedValue;
			curGrid.GridUpMainPreDivider = (byte)cboUpMainPreDivider.SelectedValue;
			curGrid.GridDnMainPreDivider = (byte)cboDnMainPreDivider.SelectedValue;
			curGrid.GridDnExtPreDivider = (byte)cboDnExtPreDivider.SelectedValue;
			curGrid.GridBranchPreDivider = (byte)cboBranchPreDivider.SelectedValue;
			curGrid.GridBranchExtPreDivider = (byte)cboBranchExtPreDivider.SelectedValue;
			curGrid.GridPostDivider = (byte)cboPostDivider.SelectedValue;

			curGrid.GridIsFullScan = ckFullScan.Checked;
			curGrid.GridIsColumnCCW = ckGridColumnLayoutCCW.Checked;
			curGrid.GridHideColumnLayoutGraphic = ckHideColumnLayoutGraphic.Checked;
		}

		// Update the database with the current dataset assignments to the current grid.
		private bool UpdateDsetAssignmentsToGrid()
		{
			if (curGrid.ID == null)
			{
				if (!performSilentSave()) return false;
			}
			// We need to do these updates after saving because they require a valid Grid ID

			// Update dataset assignments table from checkboxes
			for (int dmRow = 0; dmRow < dsetAssignments.Rows.Count; dmRow++)
			{
				dsetAssignments.Rows[dmRow]["IsAssigned"] = 0;
			}
			foreach (int idx in clbDatasets.CheckedIndices)
			{
				dsetAssignments.Rows[idx]["IsAssigned"] = 1;
			}
			// Update the database from the assignments table
			EDset.UpdateAssignmentsToGrid((Guid)curGrid.ID, dsetAssignments);

			return true;
		}

		// Set up the DataGridView, adding the right number of rows and columns and filling in
		// row and column headings.
		private void SetupDataGridView()
		{
			int nRows;
			int nCols;
			short startRow;
			short endRow;
			short startCol;
			short endCol;
			dgvGrid.Columns.Clear();

			// If no grid data, just clear the grid and return
			if (curGrid.GridStartRow == null || curGrid.GridStartCol == null ||
				curGrid.GridEndRow == null || curGrid.GridEndCol == null) return;

			dgvGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

			startRow = (short)curGrid.GridStartRow;
			endRow = (short)curGrid.GridEndRow;
			startCol = (short)curGrid.GridStartCol;
			endCol = (short)curGrid.GridEndCol;

			nRows = endRow - startRow + 1;
			curGridRows = nRows;
			dgvGrid.RowCount = nRows;
			nCols = endCol - startCol + 1;

			dgvGrid.ColumnCount = nCols + 1; // add an extra because column 1 is row headings
			dgvGrid.Columns[0].HeaderText = "Row #";

			for (short c = 0; c < nCols; c++)
				dgvGrid.Columns[c + 1].HeaderText = EMeasurement.GetColLabel((short)(c+startCol));

			for (short r = 0; r < nRows; r++)
				dgvGrid.Rows[r].Cells[0].Value = r+startRow + 1;

		}

		// Add all measurements from the curMeasurements DataTable to the DataGridView
		private void AddMeasurementsToGrid()
		{
			string sVal;
			int row, col;
			short startRow;
			short startCol;

			// If no grid data, just return
			if (curGrid.GridStartRow == null || curGrid.GridStartCol == null ||
				curGrid.GridEndRow == null || curGrid.GridEndCol == null) return;

			startRow = (short)curGrid.GridStartRow;
			startCol = (short)curGrid.GridStartCol;

			decimal? thick;
			bool obstr, error;
			DataRow dr;
			for (int r = 0; r < curMeasurements.Rows.Count; r++) 
			{
				dr = curMeasurements.Rows[r];
				row = Convert.ToInt32(dr[1]);
				col = Convert.ToInt32(dr[2]);
				if (dr[3] == DBNull.Value) thick = null;
				else thick = Convert.ToDecimal(dr[3]);
				obstr = (bool)dr[4];
				error = (bool)dr[5];
				if (thick == null)
				{
					if (obstr) sVal = "obst.";
					else sVal = "empty";
				}
				else sVal = GetFormattedThickness(thick);
				dgvGrid.Rows[row-startRow].Cells[col-startCol + 1].Value = sVal;
			}
		}

		// Fill in the items in the ParentColRef and ParentRowRef combo boxes.
		// These items depend on the currently selected parent grid if any.
		private void ManageParentRefCombos()
		{
			if (cboParentGrid.SelectedIndex == 0)
			{
				cboParentColRef.Items.Clear();
				cboParentRowRef.Items.Clear();
			}
			else
			{
				cboParentColRef.Items.Clear();
				cboParentRowRef.Items.Clear();
				EGrid grid = new EGrid((Guid?)cboParentGrid.SelectedValue);
				int? endRow = grid.GridEndRow;
				int? endCol = grid.GridEndCol;
				if (endRow != null)
				{
					for (int i = 0; i <= endRow; i++)
						cboParentRowRef.Items.Add((i + 1));

					cboParentRowRef.SelectedIndex = 0;
				}

				if (endCol != null)
				{
					for (int i = 0; i <= endCol; i++)
						cboParentColRef.Items.Add(EMeasurement.GetColLabel((short)i));
					cboParentColRef.SelectedIndex = 0;
				}
			}
		}

		// Gets a GridSize from a collection with the specified id if it's there.
		private EGridSize FindGridSizeForId(Guid? id)
		{
			if (id == null) return null;
			foreach (EGridSize gridSize in gridSizes)
				if (gridSize.ID == id)
					return gridSize;
			return null;
		}

		// Searches the gridsizes collection for a grid size that matches the specified axial
		// and radial distances.
		private Guid? GetGridSizeIdForAxialAndRadial(string axialDist, string radialDist)
		{
			decimal ad, rd;
			if (decimal.TryParse(axialDist, out ad) && decimal.TryParse(radialDist, out rd))
			{
				foreach (EGridSize gridSize in gridSizes)
				{
					if (ad == gridSize.GridSizeAxialDistance && rd == gridSize.GridSizeRadialDistance)
					{
						return gridSize.ID;
					}
				}
			}
			return null;
		}

		// Test if a grid size object's axial and radial distances match the specified values.
		private bool GridSizeMatchesAxialAndRadial(EGridSize gridSize, string axialDist, string radialDist)
		{
			if (gridSize == null) return false;
			decimal ad, rd;
			if (decimal.TryParse(axialDist, out ad) && decimal.TryParse(radialDist, out rd))
				if (ad == gridSize.GridSizeAxialDistance && rd == gridSize.GridSizeRadialDistance)
					return true;

			return false;
		}

		// If there are errors for either textbox, set the combo to no selection.
		// Otherwise, Check the current combo item to see if its distances match the textboxes.
		// If it does, we're done.
		// If not, check the grid size collection to see if a grid size that matches the 
		// current axial and radial distances exists.  If it does, set the combo to that item.
		// Otherwise, set it to no selection.
		private void SetGridSizeComboForTextChange()
		{
			if (errorProvider1.GetError(txtAxialDistance).Length > 0 ||
				errorProvider1.GetError(txtRadialDistance).Length > 0)
				cboGridSize.SelectedIndex = 0;
			else
			{
				EGridSize gridSize = FindGridSizeForId((Guid?)cboGridSize.SelectedValue);
				if (!GridSizeMatchesAxialAndRadial(gridSize,
					txtAxialDistance.Text, txtRadialDistance.Text))
				{
					Guid? gridSizeID = GetGridSizeIdForAxialAndRadial(txtAxialDistance.Text, txtRadialDistance.Text);
					cboGridSize.SelectedIndexChanged -= new System.EventHandler(cboGridSize_SelectedIndexChanged);
					if (gridSizeID == null) cboGridSize.SelectedIndex = 0;
					else cboGridSize.SelectedValue = gridSizeID;
					cboGridSize.SelectedIndexChanged += new System.EventHandler(cboGridSize_SelectedIndexChanged);
				}
			}
		}
		private void GetStats()
		{
			int? startRow = curGrid.GridStartRow;
			int? endRow = curGrid.GridEndRow;
			int? startCol = curGrid.GridStartCol;
			int? endCol = curGrid.GridEndCol;

			if (startRow != null && startCol != null && endRow != null && endCol != null)
			{
				lblColsIncluded.Text = EMeasurement.GetColLabel((short)startCol) + " to " +	EMeasurement.GetColLabel((short)endCol);
				lblRowsIncluded.Text = (startRow + 1) + " to " + (endRow + 1);
				lblObstructions.Text = curGrid.GridObstructions.ToString();
				lblEmpties.Text = curGrid.GridEmpties.ToString();
			}
			else
			{
				lblColsIncluded.Text = "N/A";
				lblRowsIncluded.Text = "N/A";
				lblObstructions.Text = "N/A";
				lblEmpties.Text = "N/A";
			}

			if (curGrid.GridMeasurements != null)
				lblTotalReadings.Text = curGrid.GridMeasurements.ToString();
			else
				lblTotalReadings.Text = "0";

			if ((curGrid.GridTextFilePoints != null && curGrid.GridTextFilePoints != 0))
			{
				lblMaxInfo.Text = curGrid.GridMaxWall.ToString();
				lblMinInfo.Text = curGrid.GridMinWall.ToString();
				lblMean.Text = string.Format("{0:0.000}", curGrid.GridMeanWall);
				lblStdDev.Text = string.Format("{0:0.000}", curGrid.GridStdevWall);
				lblBelowTscr.Text = string.Format("{0:0}", curGrid.GridBelowTscr);
			}
			else
			{
				lblMaxInfo.Text = "N/A";
				lblMinInfo.Text = "N/A";
				lblMean.Text = "N/A";
				lblStdDev.Text = "N/A";
				lblBelowTscr.Text = "N/A";
			}
		}

		// Given a string value which may contain a 1-based row value, 
		// try to return a 0-based short row value
		public static short? GetRowNumberForString(string text)
		{
			return Util.IsNullOrEmpty(text) ? (short?)null : (short)((short.Parse(text) - 1));
		}

		// Format a grid size value for display
		private string GetFormattedGridSize(decimal? number)
		{
			return number == null ? null :
				string.Format("{0:0.000}", number);
		}

		// Format a thickness value for display
		private string GetFormattedThickness(decimal? number)
		{
			return number == null ? null :
				string.Format("{0:0.000}", number);
		}

		// Format a generic short for display
		private string GetFormattedShort(short? number)
		{
			return number == null ? null :
				string.Format("{0}", number);
		}

		// Format a row for display
		private string GetFormattedRow(short? number)
		{
			return number == null ? null :
				string.Format("{0}", number + 1);
		}

		private void dgvGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			int rowIdx = e.RowIndex;
			int colIdx = e.ColumnIndex - 1;
			Guid? measurementID = GetMeasurementIdForGridRowAndColumn(rowIdx, colIdx);
			EMeasurement measurement = new EMeasurement(measurementID);
		}

		private Guid? GetMeasurementIdForGridRowAndColumn(int row, int column)
		{
			foreach (DataRow dr in curMeasurements.Rows)
			{
				if (Convert.ToInt32(dr[1]) == row && Convert.ToInt32(dr[2]) == column)
				{
					return (Guid?)dr[0];
				}
			}
			return null;
		}

		private void btnExportToText_Click(object sender, EventArgs e)
		{
			if (clbDatasets.CheckedItems.Count == 0)
			{
				MessageBox.Show("Nothing to Export -- The current grid does not contain any datasets","Factotum");
				return;
			}

			EInspection curInspection = new EInspection(curGrid.GridIspID);
			EInspectedComponent curReport = new EInspectedComponent(curInspection.InspectionIscID);

			saveFileDialog1.Filter = "Text files *.txt|*.txt";
			saveFileDialog1.DefaultExt = "txt";
			saveFileDialog1.FileName = Globals.MeterDataFolder + "\\" +
				curReport.InspComponentName + "_" + curInspection.InspectionName + ".txt";
			saveFileDialog1.Title = "Export Grid to Panametrics Text File";
			DialogResult rslt = saveFileDialog1.ShowDialog();
			if (rslt != DialogResult.OK) return;

			PanametricsExporter exporter = 
				new PanametricsExporter(saveFileDialog1.FileName, (Guid)curGrid.ID);
			exporter.ExportGrid();
			MessageBox.Show("Done!");
		}

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            if (!performSilentSave()) return;

            if (clbDatasets.CheckedItems.Count == 0)
            {
                MessageBox.Show("Nothing to Export -- The current grid does not contain any datasets", "Factotum");
                return;
            }
            if (curGrid.GridAxialDistance == null || curGrid.GridRadialDistance == null)
            {
                MessageBox.Show("A Grid Size must be selected before the grid can be exported", "Factotum");
                return;
            }
            EGridCollection childGrids = curGrid.GetAllChildGrids();
            if (childGrids.Count > 0)
            {
                if (MessageBox.Show("Current grid contains child grids.  Create a merged grid?",
                    "Factotum", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                    createMergedGrid = true;
            }

            EInspection curInspection = new EInspection(curGrid.GridIspID);
            EInspectedComponent curReport = new EInspectedComponent(curInspection.InspectionIscID);
            
            saveFileDialog1.Filter = "Excel files *.xls|*.xls";
			saveFileDialog1.DefaultExt = "xls";
			saveFileDialog1.FileName = Globals.MeterDataFolder + "\\" +
				curReport.InspComponentName + "_" + curInspection.InspectionName + ".xls";
			saveFileDialog1.Title = "Export Grid to Excel file";
			DialogResult rslt = saveFileDialog1.ShowDialog();
			if (rslt != DialogResult.OK) return;
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            MergedGridExporter exporter = new MergedGridExporter((Guid)curGrid.ID, saveFileDialog1.FileName, backgroundWorker2, createMergedGrid);
            exporter.CreateWorkbook();
            exporter = null;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Done!");
        }
	}
}