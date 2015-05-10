using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DowUtils;
using System.Collections;
using System.IO;


namespace Factotum
{
	public partial class DsetEdit : Form, IEntityEditForm
	{
		private EDset curDset;
		private ESpecialCalValue curSpecialCalValue;
		private ESpecialCalParamCollection specialCalParams;
		private EInspection inspection;
		private EInspectedComponent inspectedComponent;
		private bool newRecord;
		private EInspectorCollection inspectors;
		private EMeterCollection metersAll;
		private EMeterCollection metersKit;
		private EDucerCollection ducersAll;
		private EDucerCollection ducersKit;
		private ECalBlockCollection calblocksAll;
		private ECalBlockCollection calblocksKit;
		private EThermoCollection thermosAll;
		private EThermoCollection thermosKit;
		TextFileParser parser;
		private bool savedInternally;
		private bool statsNeedRefresh;
		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curDset; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public DsetEdit(Guid? ID)
			: this(ID, null){}

		public DsetEdit(Guid? ID, Guid? inspectionID)
		{
			InitializeComponent();
			this.backgroundParser.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundParser_RunWorkerCompleted);
			this.backgroundParser.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundParser_ProgressChanged);
			curDset = new EDset();
			curDset.Load(ID);
			if (inspectionID != null) curDset.DsetIspID = inspectionID;
			newRecord = (ID == null);
			InitializeControls();
			savedInternally = false;
			// This will initially get set when the user clicks on the SpecialFieldData tab
			curSpecialCalValue = null;
		}

		// Initialize the form control values
		// If we have a new form, we should populate the inspectors combo box and set it to
		// no selection.  
		// We should also populate the tools combo boxes with all and set them to no selection.
		// Disable the checkboxes.
		// If the user selects an inspector, we should do the following
		// 1. See if the inspector has a tool kit.  
		//    If the inspector is 'no selection' or an actual inspector who 
		//			does not have a tool kit, uncheck and disable all the checkboxes.  
		//			Leave the tool combos with all items.
		//		If the inspector has a tool kit, fill the kit tables for each tool
		//       and activate all the checkboxes.
		//			Don't change the combo bindings yet.
		//	2.	Next, take a look at each tool combo's current selection.  
		//			If it is 'no selection' or an item in the inspector's kit, check the checkbox
		//				and set the tool combo source to the kit table.
		//			If it is an item that is not in the inspector's kit,
		//				explain to the user
		//				and ask them if they want to clear the current selection.
		//				If the user says to clear the current selection, 
		//					check the checkbox and set the tool combo source to the kit table.
		//					set the combo box to 'no selection'
		//				If the user says to keep the current selection, uncheck the kit box
		//					leave the combo bound to the all table.

		// If the user checks the checkbox, we should do step 2. above for that tool only.
		// If the user unchecks the checkbox, set the combo source to the all table

		private void InitializeControls()
		{
			inspection = new EInspection(curDset.DsetIspID);
			inspectedComponent = new EInspectedComponent(inspection.InspectionIscID);

			inspectors = EInspector.ListForOutage(
				(Guid)inspectedComponent.InspComponentOtgID, !newRecord, true);
			cboInspector.DataSource = inspectors;
			cboInspector.DisplayMember = "InspectorNameWithStatus";
			cboInspector.ValueMember = "ID";

			// I don't think we ever want to see inactive cal params here -- whether or not it's
			// a new Dset...
			specialCalParams = ESpecialCalParam.ListByName(false, true);
			cboSpecialCalParameter.DataSource = specialCalParams;
			cboSpecialCalParameter.DisplayMember = "SpecialCalParamName";
			cboSpecialCalParameter.ValueMember = "ID";

			// Get a list for each type of tool
			metersAll = EMeter.ListByName(!newRecord, true);
			ducersAll = EDucer.ListByName(!newRecord, true);
			calblocksAll = ECalBlock.ListByName(!newRecord, true);
			thermosAll = EThermo.ListByName(!newRecord, true);


			// We'll set the data source in SetControlValues
			cboMeter.DisplayMember = "MeterNameWithStatus";
			cboMeter.ValueMember = "ID";
			cboTransducer.DisplayMember = "DucerNameWithStatus";
			cboTransducer.ValueMember = "ID";
			cboCalBlock.DisplayMember = "CalBlockNameWithStatus";
			cboCalBlock.ValueMember = "ID";
			cboThermo.DisplayMember = "ThermoNameWithStatus";
			cboThermo.ValueMember = "ID";


			SetControlValues(newRecord);
			this.Text = newRecord ? "New Dataset" : "Edit Dataset";

			statsNeedRefresh = true;
		}

		private void DsetEdit_Load(object sender, EventArgs e)
		{
			// Apply the current filters and set the selector row.  
			// Passing a null selects the first row if there are any rows.
			UpdateSelector_NonGridMeasurements(null);
			// Now that we have some rows and columns, we can do some customization.
			CustomizeGrid_NonGridMeasurements();
			// Need to do this because the customization clears the row selection.
			SelectGridRow(dgvNonGridMeasurements, null);

			UpdateSelector_InspPeriods(null);
			CustomizeGrid_InspPeriods();
			SelectGridRow(dgvInspPeriods, null);

			UpdateSelector_SpecialCalData(null);
			CustomizeGrid_SpecialCalData();
			SelectGridRow(dgvSpecialFieldData, null);

			EAdditionalMeasurement.Changed += new EventHandler<EntityChangedEventArgs>(EAdditionalMeasurement_Changed);
			EInspectionPeriod.Changed += new EventHandler<EntityChangedEventArgs>(EInspectionPeriod_Changed);
			EInspector.Changed += new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EInspector.InspectorOutageAssignmentsChanged += new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
			EInspector.InspectorKitAssignmentsChanged += new EventHandler(EInspector_InspectorKitAssignmentsChanged);
			EMeter.Changed += new EventHandler<EntityChangedEventArgs>(EMeter_Changed);
			EMeter.MeterKitAssignmentsChanged += new EventHandler(EMeter_MeterKitAssignmentsChanged);
			EDucer.Changed += new EventHandler<EntityChangedEventArgs>(EDucer_Changed);
			EDucer.DucerKitAssignmentsChanged += new EventHandler(EDucer_DucerKitAssignmentsChanged);
			ECalBlock.Changed += new EventHandler<EntityChangedEventArgs>(ECalBlock_Changed);
			ECalBlock.CalBlockKitAssignmentsChanged += new EventHandler(ECalBlock_CalBlockKitAssignmentsChanged);
			EThermo.Changed += new EventHandler<EntityChangedEventArgs>(EThermo_Changed);
			EThermo.ThermoKitAssignmentsChanged += new EventHandler(EThermo_ThermoKitAssignmentsChanged);

			ESpecialCalParam.Changed += new EventHandler<EntityChangedEventArgs>(ESpecialCalParam_Changed);
			ESpecialCalValue.Changed += new EventHandler<EntityChangedEventArgs>(ESpecialCalValue_Changed);
		}

		private void DsetEdit_FormClosed(object sender, FormClosedEventArgs e)
		{
			EAdditionalMeasurement.Changed -= new EventHandler<EntityChangedEventArgs>(EAdditionalMeasurement_Changed);
			EInspectionPeriod.Changed -= new EventHandler<EntityChangedEventArgs>(EInspectionPeriod_Changed);
			EInspector.Changed -= new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EInspector.InspectorOutageAssignmentsChanged -= new EventHandler(EInspector_InspectorOutageAssignmentsChanged);
			EInspector.InspectorKitAssignmentsChanged -= new EventHandler(EInspector_InspectorKitAssignmentsChanged);
			EMeter.Changed -= new EventHandler<EntityChangedEventArgs>(EMeter_Changed);
			EMeter.MeterKitAssignmentsChanged -= new EventHandler(EMeter_MeterKitAssignmentsChanged);
			EDucer.Changed -= new EventHandler<EntityChangedEventArgs>(EDucer_Changed);
			EDucer.DucerKitAssignmentsChanged -= new EventHandler(EDucer_DucerKitAssignmentsChanged);
			ECalBlock.Changed -= new EventHandler<EntityChangedEventArgs>(ECalBlock_Changed);
			ECalBlock.CalBlockKitAssignmentsChanged -= new EventHandler(ECalBlock_CalBlockKitAssignmentsChanged);
			EThermo.Changed -= new EventHandler<EntityChangedEventArgs>(EThermo_Changed);
			EThermo.ThermoKitAssignmentsChanged -= new EventHandler(EThermo_ThermoKitAssignmentsChanged);

			ESpecialCalParam.Changed -= new EventHandler<EntityChangedEventArgs>(ESpecialCalParam_Changed);
			ESpecialCalValue.Changed -= new EventHandler<EntityChangedEventArgs>(ESpecialCalValue_Changed);
		}


		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		void EInspectionPeriod_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector_InspPeriods(e.ID);
		}

		void EAdditionalMeasurement_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector_NonGridMeasurements(e.ID);
			statsNeedRefresh = true;
		}

		void EInspector_Changed(object sender, EntityChangedEventArgs e)
		{
			HandleEInspectorChange();
		}

		void EInspector_InspectorOutageAssignmentsChanged(object sender, EventArgs e)
		{
			HandleEInspectorChange();
		}

		void EInspector_InspectorKitAssignmentsChanged(object sender, EventArgs e)
		{
			HandleEInspectorChange();
		}

		void ESpecialCalParam_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? curValue = (Guid?)cboSpecialCalParameter.SelectedValue;
			specialCalParams = ESpecialCalParam.ListByName(!newRecord, true);
			cboSpecialCalParameter.DataSource = specialCalParams;
			if (curValue == null) cboSpecialCalParameter.SelectedIndex = 0;
			else cboSpecialCalParameter.SelectedValue = curValue;
		}

		void ESpecialCalValue_Changed(object sender, EntityChangedEventArgs e)
		{
			UpdateSelector_SpecialCalData(e.ID);
		}

		private void HandleEInspectorChange()
		{
			Guid? curValue = (Guid?)cboInspector.SelectedValue;
			inspectors = EInspector.ListForOutage(
				(Guid)inspectedComponent.InspComponentOtgID, !newRecord, true);
			cboInspector.DataSource = inspectors;
		}

		void EMeter_Changed(object sender, EntityChangedEventArgs e)
		{
			HandleEMeterChange();
		}

		void EMeter_MeterKitAssignmentsChanged(object sender, EventArgs e)
		{
			HandleEMeterChange();
		}

		private void HandleEMeterChange()
		{
			Guid? curValue = (Guid?)cboMeter.SelectedValue;
			metersAll = EMeter.ListByName(!newRecord, true);
			if (cboInspector.SelectedValue != null)
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)cboInspector.SelectedValue);
				metersKit = EMeter.ListForKit(inspector.InspectorKitID, true);
			}
			cboMeter.DataSource = (ckKitMeter.Checked == true ? metersKit : metersAll);
			if (curValue == null) cboMeter.SelectedIndex = 0;
			else cboMeter.SelectedValue = curValue;
		}

		void EDucer_Changed(object sender, EntityChangedEventArgs e)
		{
			HandleEDucerChange();
		}

		void EDucer_DucerKitAssignmentsChanged(object sender, EventArgs e)
		{
			HandleEDucerChange();
		}

		private void HandleEDucerChange()
		{
			Guid? curValue = (Guid?)cboTransducer.SelectedValue;
			ducersAll = EDucer.ListByName(!newRecord, true);
			if (cboInspector.SelectedValue != null)
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)cboInspector.SelectedValue);
				ducersKit = EDucer.ListForKit(inspector.InspectorKitID, true);
			}
			cboTransducer.DataSource = (ckKitTransducer.Checked == true ? ducersKit : ducersAll);
			if (curValue == null) cboTransducer.SelectedIndex = 0;
			else cboTransducer.SelectedValue = curValue;
		}

		void ECalBlock_Changed(object sender, EntityChangedEventArgs e)
		{
			HandleECalBlockChange();
		}

		void ECalBlock_CalBlockKitAssignmentsChanged(object sender, EventArgs e)
		{
			HandleECalBlockChange();
		}

		private void HandleECalBlockChange()
		{
			Guid? curValue = (Guid?)cboCalBlock.SelectedValue;
			calblocksAll = ECalBlock.ListByName(!newRecord, true);
			if (cboInspector.SelectedValue != null)
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)cboInspector.SelectedValue);
				calblocksKit = ECalBlock.ListForKit(inspector.InspectorKitID, true);
			}
			cboCalBlock.DataSource = (ckKitCalBlock.Checked == true ? calblocksKit : calblocksAll);
			if (curValue == null) cboCalBlock.SelectedIndex = 0;
			else cboCalBlock.SelectedValue = curValue;
		}

		void EThermo_Changed(object sender, EntityChangedEventArgs e)
		{
			HandleEThermoChange();
		}

		void EThermo_ThermoKitAssignmentsChanged(object sender, EventArgs e)
		{
			HandleEThermoChange();
		}

		private void HandleEThermoChange()
		{
			Guid? curValue = (Guid?)cboThermo.SelectedValue;
			thermosAll = EThermo.ListByName(!newRecord, true);
			if (cboInspector.SelectedValue != null)
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)cboInspector.SelectedValue);
				thermosKit = EThermo.ListForKit(inspector.InspectorKitID, true);
			}
			cboThermo.DataSource = (ckKitThermometer.Checked == true ? thermosKit : thermosAll);
			if (curValue == null) cboThermo.SelectedIndex = 0;
			else cboThermo.SelectedValue = curValue;
		}

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

		private void EditCurrentSelection_NonGridMeasurements()
		{
			// Make sure there's a row selected
			if (dgvNonGridMeasurements.SelectedRows.Count != 1) return;
			if (!performSilentSave()) return;

			Guid? currentEditItem = (Guid?)(dgvNonGridMeasurements.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "AdditionalMeasurementEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				AdditionalMeasurementEdit frm = new AdditionalMeasurementEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}
		private void btnEditNonGridMeas_Click(object sender, EventArgs e)
		{
			EditCurrentSelection_NonGridMeasurements();
		}

		private void dgvNonGridMeasurements_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection_NonGridMeasurements();
		}

		private void btnAddNonGridMeas_Click(object sender, EventArgs e)
		{
			if (!performSilentSave()) return;
			AdditionalMeasurementEdit frm = new AdditionalMeasurementEdit(null, curDset.ID);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		private void btnDeleteNonGridMeas_Click(object sender, EventArgs e)
		{
			if (dgvNonGridMeasurements.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select an Additional Measurement to delete first.","Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvNonGridMeasurements.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "AdditionalMeasurementEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EAdditionalMeasurement additionalMeasurement = 
				new EAdditionalMeasurement(currentEditItem);

			additionalMeasurement.Delete(true);
			if (additionalMeasurement.AdditionalMeasurementErrMsg != null)
			{
				MessageBox.Show(additionalMeasurement.AdditionalMeasurementErrMsg, "Factotum");
				additionalMeasurement.AdditionalMeasurementErrMsg = null;
			}
		}

		private void EditCurrentSelection_InspPeriods()
		{
			// Make sure there's a row selected
			if (dgvInspPeriods.SelectedRows.Count != 1) return;
			if (!performSilentSave()) return;
			Guid? currentEditItem = (Guid?)(dgvInspPeriods.SelectedRows[0].Cells["ID"].Value);
			// First check to see if an instance of the form set to the selected ID already exists
			if (!Globals.CanActivateForm(this, "InspectionPeriodEdit", currentEditItem))
			{
				// Open the edit form with the currently selected ID.
				InspectionPeriodEdit frm = new InspectionPeriodEdit(currentEditItem);
				frm.MdiParent = this.MdiParent;
				frm.Show();
			}
		}

		private void btnEditInspPeriod_Click(object sender, EventArgs e)
		{
			EditCurrentSelection_InspPeriods();
		}

		private void dgvInspPeriods_DoubleClick(object sender, EventArgs e)
		{
			EditCurrentSelection_InspPeriods();
		}

		private void dgvInspPeriods_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				EditCurrentSelection_InspPeriods();
		}

		private void btnAddInspPeriod_Click(object sender, EventArgs e)
		{
			if (!performSilentSave()) return;
			InspectionPeriodEdit frm = new InspectionPeriodEdit(null, curDset.ID);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		private void btnDeleteInspPeriod_Click(object sender, EventArgs e)
		{
			if (dgvInspPeriods.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select an Inspection Period to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvInspPeriods.SelectedRows[0].Cells["ID"].Value);

			if (Globals.IsFormOpen(this, "InspectionPeriodEdit", currentEditItem))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return;
			}

			EInspectionPeriod inspectionPeriod =
				new EInspectionPeriod(currentEditItem);
			inspectionPeriod.Delete(true);
			if (inspectionPeriod.InspectionPeriodErrMsg != null)
			{
				MessageBox.Show(inspectionPeriod.InspectionPeriodErrMsg, "Factotum");
				inspectionPeriod.InspectionPeriodErrMsg = null;
			}
		}

		private void btnEditSpecialCalItem_Click(object sender, EventArgs e)
		{
			// If we're already editing, exit
			if (lblEditModeIndicator.Visible) return;

			// If no row is selected, exit
			if (dgvSpecialFieldData.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Item to edit first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvSpecialFieldData.SelectedRows[0].Cells["ID"].Value);
			curSpecialCalValue = new ESpecialCalValue(currentEditItem);
			
			// Get the parameter and value for the selected row and set the edit combo and textbox
			// to those values.  Make the edit mode label visible.
			txtSpecialCalValue.Text = curSpecialCalValue.SpecialCalValueValue;
			cboSpecialCalParameter.SelectedValue = curSpecialCalValue.SpecialCalValueScpID;

			// Make the edit mode label visible.
			lblEditModeIndicator.Visible = true;
		}

		private void btnAddSaveSpecialCalItem_Click(object sender, EventArgs e)
		{			
			// Set the entity values to match the form values
			UpdateRecord_SpecialCal();
			// Try to validate
			if (!curSpecialCalValue.Valid())
			{
				setAllErrors_SpecialCal();
				return;
			}
			// Save
			curSpecialCalValue.Save();

			// Clear the entry fields and get a new object.
			ClearSpecialCalItem();
		}

		private void btnDeleteSpecialCalItem_Click(object sender, EventArgs e)
		{
			// If no row is selected, exit
			if (dgvSpecialFieldData.SelectedRows.Count != 1)
			{
				MessageBox.Show("Please select a Calibration Item to delete first.", "Factotum");
				return;
			}
			Guid? currentEditItem = (Guid?)(dgvSpecialFieldData.SelectedRows[0].Cells["ID"].Value);
			curSpecialCalValue = new ESpecialCalValue(currentEditItem);
			curSpecialCalValue.Delete(true);

			if (curSpecialCalValue.SpecialCalValueErrMsg != null)
			{
				MessageBox.Show(curSpecialCalValue.SpecialCalValueErrMsg, "Factotum");
				curSpecialCalValue.SpecialCalValueErrMsg = null;
			}
			// Clear the entry fields and get a new object.
			ClearSpecialCalItem();
		}

		private void btnClearSpecialCalItem_Click(object sender, EventArgs e)
		{
			ClearSpecialCalItem();		
		}

		private void ClearSpecialCalItem()
		{
			lblEditModeIndicator.Visible = false;
			// clear out the control values
			cboSpecialCalParameter.SelectedIndex = 0;
			txtSpecialCalValue.Text = null;

			// Get a new object to work with
			curSpecialCalValue = new ESpecialCalValue();
			curSpecialCalValue.SpecialCalValueDstID = curDset.ID;
			errorProvider1.SetError(txtSpecialCalValue, "");
			errorProvider1.SetError(cboSpecialCalParameter, "");
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curDset.DsetNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curDset.DsetNameErrMsg);
		}
		private void txtComponentTemp_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetCompTempLengthOk(txtComponentTemp.Text);
			errorProvider1.SetError(txtComponentTemp, curDset.DsetCompTempErrMsg);
		}

		private void txtCalBlockTemp_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetCalBlockTempLengthOk(txtCalBlockTemp.Text);
			errorProvider1.SetError(txtCalBlockTemp, curDset.DsetCalBlockTempErrMsg);
		}

		private void txtCoarseRange_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetRangeLengthOk(txtCoarseRange.Text);
			errorProvider1.SetError(txtCoarseRange, curDset.DsetRangeErrMsg);
		}

		private void txtVelocity_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetVelocityLengthOk(txtVelocity.Text);
			errorProvider1.SetError(txtVelocity, curDset.DsetVelocityErrMsg);
		}

		private void txtGain_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetGainDbLengthOk(txtGain.Text);
			errorProvider1.SetError(txtGain, curDset.DsetGainDbErrMsg);
		}

		private void txtMinReading_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetThinLengthOk(txtMinReading.Text);
			errorProvider1.SetError(txtMinReading, curDset.DsetThinErrMsg);
		}

		private void txtMaxReading_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetThickLengthOk(txtMaxReading.Text);
			errorProvider1.SetError(txtMaxReading, curDset.DsetThickErrMsg);
		}
		private void txtCrewDose_TextChanged(object sender, EventArgs e)
		{
			curDset.DsetCrewDoseLengthOk(txtCrewDose.Text);
			errorProvider1.SetError(txtCrewDose, curDset.DsetCrewDoseErrMsg);
		}
		private void txtSpecialCalValue_TextChanged(object sender, EventArgs e)
		{
			curSpecialCalValue.SpecialCalValueValueLengthOk(txtSpecialCalValue.Text);
			errorProvider1.SetError(txtSpecialCalValue, curSpecialCalValue.SpecialCalValueValueErrMsg);
		}


		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curDset.DsetNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curDset.DsetNameErrMsg);
		}

		private void txtComponentTemp_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetCompTempValid(txtComponentTemp.Text);
			errorProvider1.SetError(txtComponentTemp, curDset.DsetCompTempErrMsg);
		}

		private void txtCalBlockTemp_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetCalBlockTempValid(txtCalBlockTemp.Text);
			errorProvider1.SetError(txtCalBlockTemp, curDset.DsetCalBlockTempErrMsg);
		}

		private void txtCoarseRange_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetRangeValid(txtCoarseRange.Text);
			errorProvider1.SetError(txtCoarseRange, curDset.DsetRangeErrMsg);
		}

		private void txtVelocity_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetVelocityValid(txtVelocity.Text);
			errorProvider1.SetError(txtVelocity, curDset.DsetVelocityErrMsg);
		}

		private void txtGain_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetGainDbValid(txtGain.Text);
			errorProvider1.SetError(txtGain, curDset.DsetGainDbErrMsg);
		}

		private void txtMinReading_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetThinValid(txtMinReading.Text);
			errorProvider1.SetError(txtMinReading, curDset.DsetThinErrMsg);
		}

		private void txtMaxReading_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetThickValid(txtMaxReading.Text);
			errorProvider1.SetError(txtMaxReading, curDset.DsetThickErrMsg);
		}

		private void txtCrewDose_Validating(object sender, CancelEventArgs e)
		{
			curDset.DsetCrewDoseValid(txtCrewDose.Text);
			errorProvider1.SetError(txtCrewDose, curDset.DsetCrewDoseErrMsg);
		}

		private void txtSpecialCalValue_Validating(object sender, CancelEventArgs e)
		{
			curSpecialCalValue.SpecialCalValueValueValid(txtSpecialCalValue.Text);
			errorProvider1.SetError(txtSpecialCalValue, curSpecialCalValue.SpecialCalValueValueErrMsg);
		}

		private void cboSpecialCalParameter_Validating(object sender, CancelEventArgs e)
		{
			curSpecialCalValue.SpecialCalValueScpIDValid((Guid?)cboSpecialCalParameter.SelectedValue);
			errorProvider1.SetError(cboSpecialCalParameter, curSpecialCalValue.SpecialCalValueScpErrMsg);
		}

		// If the user resizes the form, keep the site label centered and in the form
		private void MaterialTypeEdit_Resize(object sender, EventArgs e)
		{
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

		private void cboInspector_SelectedIndexChanged(object sender, EventArgs e)
		{
			HandleMeterForInspectorSelection(
				(Guid?)cboInspector.SelectedValue, (Guid?)cboMeter.SelectedValue);
			HandleDucerForInspectorSelection(
				(Guid?)cboInspector.SelectedValue, (Guid?)cboTransducer.SelectedValue);
			HandleCalBlockForInspectorSelection(
				(Guid?)cboInspector.SelectedValue, (Guid?)cboCalBlock.SelectedValue);
			HandleThermoForInspectorSelection(
				(Guid?)cboInspector.SelectedValue, (Guid?)cboThermo.SelectedValue);
		}

		private void ckKitMeter_Click(object sender, EventArgs e)
		{
			HandleCheckboxClick(cboMeter, ckKitMeter,
				"Meter", metersKit, metersAll);
		}
		private void ckKitTransducer_Click(object sender, EventArgs e)
		{
			HandleCheckboxClick(cboTransducer, ckKitTransducer,
				"Transducer", ducersKit, ducersAll);
		}

		private void ckKitCalBlock_Click(object sender, EventArgs e)
		{
			HandleCheckboxClick(cboCalBlock, ckKitCalBlock,
				"Calibration Block", calblocksKit, calblocksAll);
		}

		private void ckKitThermometer_Click(object sender, EventArgs e)
		{
			HandleCheckboxClick(cboThermo, ckKitThermometer,
				"Thermometer", thermosKit, thermosAll);
		}

		//---------------------------------------------------------
		// High Level Helper functions
		//---------------------------------------------------------

		private void UpdateSelector_NonGridMeasurements(Guid? id)
		{
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvNonGridMeasurements.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvNonGridMeasurements.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EAdditionalMeasurement.GetDefaultDataViewForDset(curDset.ID);
			dgvNonGridMeasurements.DataSource = dv;
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvNonGridMeasurements.Sort(dgvNonGridMeasurements.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvNonGridMeasurements.Sort(dgvNonGridMeasurements.Columns[sortCol], ListSortDirection.Descending);

			// Select the current row
			SelectGridRow(dgvNonGridMeasurements, id);
		}

		private void CustomizeGrid_NonGridMeasurements()
		{
			// Apply a default sort
			dgvNonGridMeasurements.Sort(dgvNonGridMeasurements.Columns["AdditionalMeasurementName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvNonGridMeasurements.Columns["AdditionalMeasurementName"].HeaderText = "Measurement";
			dgvNonGridMeasurements.Columns["AdditionalMeasurementDescription"].HeaderText = "Description";
			dgvNonGridMeasurements.Columns["AdditionalMeasurementThickness"].HeaderText = "Thickness";
            dgvNonGridMeasurements.Columns["AdditionalMeasurementIncludeInStats"].HeaderText = "Include In Stats";
            dgvNonGridMeasurements.Columns["AdditionalMeasurementComponentSection"].HeaderText = "Section";
            // Hide some columns
			dgvNonGridMeasurements.Columns["ID"].Visible = false;
			dgvNonGridMeasurements.Columns["AdditionalMeasurementDstID"].Visible = false;
			dgvNonGridMeasurements.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		private void UpdateSelector_InspPeriods(Guid? id)
		{
			// Save the sort specs if there are any, so we can re-apply them
			SortOrder sortOrder = dgvInspPeriods.SortOrder;
			int sortCol = -1;
			if (sortOrder != SortOrder.None)
				sortCol = dgvInspPeriods.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = EInspectionPeriod.GetDefaultDataViewForDset(curDset.ID);
			dgvInspPeriods.DataSource = dv;
			// Re-apply the sort specs
			if (sortOrder == SortOrder.Ascending)
				dgvInspPeriods.Sort(dgvInspPeriods.Columns[sortCol], ListSortDirection.Ascending);
			else if (sortOrder == SortOrder.Descending)
				dgvInspPeriods.Sort(dgvInspPeriods.Columns[sortCol], ListSortDirection.Descending);

			// Select the current row
			SelectGridRow(dgvInspPeriods, id);
		}

		private void CustomizeGrid_InspPeriods()
		{
			// Apply a default sort
			dgvInspPeriods.Sort(dgvInspPeriods.Columns["InspectionPeriodInAt"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvInspPeriods.Columns["InspectionPeriodInAt"].HeaderText = "Calibration In";
			dgvInspPeriods.Columns["InspectionPeriodOutAt"].HeaderText = "Calibration Out";
			dgvInspPeriods.Columns["InspectionPeriodCalCheck1At"].HeaderText = "Cal Check 1";
			dgvInspPeriods.Columns["InspectionPeriodCalCheck2At"].HeaderText = "Cal Check 2";
			// Hide some columns
			dgvInspPeriods.Columns["ID"].Visible = false;
			dgvInspPeriods.Columns["InspectionPeriodDstID"].Visible = false;
			dgvInspPeriods.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		private void UpdateSelector_SpecialCalData(Guid? id)
		{
			// Save the sort specs if there are any, so we can re-apply them
			// SortOrder sortOrder = dgvSpecialFieldData.SortOrder;
			//int sortCol = -1;
			//if (sortOrder != SortOrder.None)
			//   sortCol = dgvSpecialFieldData.SortedColumn.Index;

			// Update the grid view selector
			DataView dv = ESpecialCalValue.GetDefaultDataViewForDset(curDset.ID);
			dgvSpecialFieldData.DataSource = dv;
			// Re-apply the sort specs
			//if (sortOrder == SortOrder.Ascending)
			//   dgvSpecialFieldData.Sort(dgvSpecialFieldData.Columns[sortCol], ListSortDirection.Ascending);
			//else if (sortOrder == SortOrder.Descending)
			//   dgvSpecialFieldData.Sort(dgvSpecialFieldData.Columns[sortCol], ListSortDirection.Descending);

			// Select the current row
			SelectGridRow(dgvSpecialFieldData, id);
		}

		private void CustomizeGrid_SpecialCalData()
		{
			// Apply a default sort
			// dgvSpecialFieldData.Sort(dgvSpecialFieldData.Columns["SpecialCalParamName"], ListSortDirection.Ascending);
			// Fix up the column headings
			dgvSpecialFieldData.Columns["SpecialCalParamName"].HeaderText = "Parameter";
			dgvSpecialFieldData.Columns["SpecialCalValueValue"].HeaderText = "Value";
			dgvSpecialFieldData.Columns["SpecialCalParamUnits"].HeaderText = "Units";
			// Hide some columns
			dgvSpecialFieldData.Columns["ID"].Visible = false;
			dgvSpecialFieldData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
		}

		// Select the row with the specified ID if it is currently displayed and scroll to it.
		// If the ID is not in the list, 
		private void SelectGridRow(DataGridView dgv, Guid? id)
		{
			bool found = false;
			int rows = dgv.Rows.Count;
			if (rows == 0) return;
			int r = 0;
			DataGridViewCell firstCell = dgv.FirstDisplayedCell;
			if (id != null)
			{
				// Find the row with the specified key id and select it.
				for (r = 0; r < rows; r++)
				{
					if ((Guid?)dgv.Rows[r].Cells["ID"].Value == id)
					{
						dgv.CurrentCell = dgv[firstCell.ColumnIndex, r];
						dgv.Rows[r].Selected = true;
						found = true;
						break;
					}
				}
			}
			if (found)
			{
				if (!dgv.Rows[r].Displayed)
				{
					// Scroll to the selected row if the ID was in the list.
					dgv.FirstDisplayedScrollingRowIndex = r;
				}
			}
			else
			{
				// Select the first item
				dgv.CurrentCell = firstCell;
				dgv.Rows[0].Selected = true;
			}
		}

		// No prompting is performed.  The user should understand the meanings of OK and Cancel.
		private void SaveAndClose()
		{
			if (AnyControlErrors()) return;
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curDset.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curDset.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues(bool newRecord)
		{
			txtName.Text = curDset.DsetName;
			txtCalBlockTemp.Text = Util.GetFormattedShort(curDset.DsetCalBlockTemp);
			txtCoarseRange.Text = Util.GetFormattedDecimal(curDset.DsetRange);
			txtComponentTemp.Text = Util.GetFormattedShort(curDset.DsetCompTemp);
			txtGain.Text = Util.GetFormattedDecimal(curDset.DsetGainDb, 1);
			txtMaxReading.Text = Util.GetFormattedDecimal(curDset.DsetThick);
			txtMinReading.Text = Util.GetFormattedDecimal(curDset.DsetThin);
			txtVelocity.Text = Util.GetFormattedDecimal(curDset.DsetVelocity,4);
			txtCrewDose.Text = Util.GetFormattedShort(curDset.DsetCrewDose);
			lblEditModeIndicator.Visible = false;

			if (!Util.IsNullOrEmpty(curDset.DsetTextFileName))
			{
				FileInfo fi = new FileInfo(curDset.DsetTextFileName);
				ofdTextfile.FileName = fi.Name;
				txtFileName.Text = fi.Name;
				ofdTextfile.InitialDirectory = fi.DirectoryName;
			}
			else
			{
				ofdTextfile.FileName = null;
				ofdTextfile.InitialDirectory = Globals.MeterDataFolder;
			}
			
			HandleMeterForInspectorSelection(curDset.DsetInsID, curDset.DsetMtrID);
			HandleDucerForInspectorSelection(curDset.DsetInsID, curDset.DsetDcrID);
			HandleCalBlockForInspectorSelection(curDset.DsetInsID, curDset.DsetCbkID);
			HandleThermoForInspectorSelection(curDset.DsetInsID, curDset.DsetThmID);

			if (curDset.DsetInsID == null) cboInspector.SelectedIndex = 0;
			else cboInspector.SelectedValue = curDset.DsetInsID;

			if (curDset.DsetIspID != null)
			{
				EInspection inspection = new EInspection((Guid?)curDset.DsetIspID);
				EInspectedComponent inspectedComponent = 
					new EInspectedComponent(inspection.InspectionIscID);
				lblSiteName.Text = "Dataset for Inspection '" + 
					inspection.InspectionName + "' of Report '" + inspectedComponent.InspComponentName + "'";
			}
			else lblSiteName.Text = "Dataset for Unknown Inspection";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			cboInspector.SelectedIndexChanged += new System.EventHandler(this.cboInspector_SelectedIndexChanged);
			
			// Default enabling of buttons on Utilities tab.
			btnApplyRowShift.Enabled = false;
			btnApplyColShift.Enabled = false;
			btnApplyDeleteCols.Enabled = false;
			btnApplyDeleteRows.Enabled = false;
			int? hasTextFilePoints = curDset.DsetTextFilePoints;
			btnApplyReverseCols.Enabled = hasTextFilePoints != null && hasTextFilePoints > 0;
			btnApplyReverseRows.Enabled = hasTextFilePoints != null && hasTextFilePoints > 0;
			btnApplyTranspose.Enabled = hasTextFilePoints != null && hasTextFilePoints > 0;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curDset.DsetNameErrMsg);
			errorProvider1.SetError(txtCalBlockTemp, curDset.DsetCalBlockTempErrMsg);
			errorProvider1.SetError(txtCoarseRange, curDset.DsetRangeErrMsg);
			errorProvider1.SetError(txtComponentTemp, curDset.DsetCompTempErrMsg);
			errorProvider1.SetError(txtGain, curDset.DsetGainDbErrMsg);
			errorProvider1.SetError(txtMaxReading, curDset.DsetThickErrMsg);
			errorProvider1.SetError(txtMinReading, curDset.DsetThinErrMsg);
			errorProvider1.SetError(txtVelocity, curDset.DsetVelocityErrMsg);
			errorProvider1.SetError(txtCrewDose, curDset.DsetCrewDoseErrMsg);
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors_SpecialCal()
		{
			errorProvider1.SetError(txtSpecialCalValue, curSpecialCalValue.SpecialCalValueValueErrMsg);
			errorProvider1.SetError(cboSpecialCalParameter, curSpecialCalValue.SpecialCalValueScpErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtCalBlockTemp).Length > 0 ||
				errorProvider1.GetError(txtCoarseRange).Length > 0 ||
				errorProvider1.GetError(txtComponentTemp).Length > 0 ||
				errorProvider1.GetError(txtGain).Length > 0 ||
				errorProvider1.GetError(txtMaxReading).Length > 0 ||
				errorProvider1.GetError(txtMinReading).Length > 0 ||
				errorProvider1.GetError(txtVelocity).Length > 0 ||
				errorProvider1.GetError(txtCrewDose).Length > 0
				);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors_SpecialCal()
		{
			return (errorProvider1.GetError(txtSpecialCalValue).Length > 0 ||
				errorProvider1.GetError(cboSpecialCalParameter).Length > 0 
				);
		}
		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curDset.DsetName = txtName.Text;
			curDset.DsetCalBlockTemp = Util.GetNullableShortForString(txtCalBlockTemp.Text);
			curDset.DsetRange = Util.GetNullableDecimalForString(txtCoarseRange.Text);
			curDset.DsetCompTemp = Util.GetNullableShortForString(txtComponentTemp.Text);
			curDset.DsetGainDb = Util.GetNullableDecimalForString(txtGain.Text);
			curDset.DsetThick = Util.GetNullableDecimalForString(txtMaxReading.Text);
			curDset.DsetThin = Util.GetNullableDecimalForString(txtMinReading.Text);
			curDset.DsetVelocity = Util.GetNullableDecimalForString(txtVelocity.Text);
			curDset.DsetCrewDose = Util.GetNullableShortForString(txtCrewDose.Text);

			curDset.DsetInsID = (Guid?)cboInspector.SelectedValue;
			curDset.DsetMtrID = (Guid?)cboMeter.SelectedValue;
			curDset.DsetDcrID = (Guid?)cboTransducer.SelectedValue;
			curDset.DsetCbkID = (Guid?)cboCalBlock.SelectedValue;
			curDset.DsetThmID = (Guid?)cboThermo.SelectedValue;
		}

		// Update the object values from the form control values.
		private void UpdateRecord_SpecialCal()
		{
			curSpecialCalValue.SpecialCalValueValue = txtSpecialCalValue.Text;
			curSpecialCalValue.SpecialCalValueScpID = (Guid?)cboSpecialCalParameter.SelectedValue;
		}

		//--------------------------------------------------------------------
		// Inspector Selection Handlers
		//--------------------------------------------------------------------
		// Note: If an inspector or tool is assigned to a kit and then inactivated, 
		// they will automatically be removed from the kit, so when we get the kit 
		private void HandleMeterForInspectorSelection(Guid? inspectorID, Guid? meterID)
		{
			if (inspectorID == null)
			{
				metersKit = null;
				HandleToolForNullInspector(meterID, cboMeter, ckKitMeter, metersAll);
			}
			else
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)inspectorID);
				metersKit = inspector.ListKitMeters(true);
				HandleToolForNonNullInspector((Guid)inspectorID, meterID, 
					cboMeter, ckKitMeter, metersKit, metersAll);
			}
			SetCurrentSelection(cboMeter, meterID);
		}

		private void HandleDucerForInspectorSelection(Guid? inspectorID, Guid? ducerID)
		{
			if (inspectorID == null)
			{
				ducersKit = null;
				HandleToolForNullInspector(ducerID, cboTransducer, ckKitTransducer, ducersAll);
			}
			else
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)inspectorID);
				ducersKit = inspector.ListKitDucers(true);
				HandleToolForNonNullInspector((Guid)inspectorID, ducerID,
					cboTransducer, ckKitTransducer, ducersKit, ducersAll);
			}
			SetCurrentSelection(cboTransducer, ducerID);
		}

		private void HandleCalBlockForInspectorSelection(Guid? inspectorID, Guid? calblockID)
		{
			if (inspectorID == null)
			{
				calblocksKit = null;
				HandleToolForNullInspector(calblockID, cboCalBlock, ckKitCalBlock, calblocksAll);
			}
			else
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)inspectorID);
				calblocksKit = inspector.ListKitCalBlocks(true);
				HandleToolForNonNullInspector((Guid)inspectorID, calblockID,
					cboCalBlock, ckKitCalBlock, calblocksKit, calblocksAll);
			}
			SetCurrentSelection(cboCalBlock, calblockID);
		}

		private void HandleThermoForInspectorSelection(Guid? inspectorID, Guid? thermoID)
		{
			if (inspectorID == null)
			{
				thermosKit = null;
				HandleToolForNullInspector(thermoID, cboThermo, ckKitThermometer, thermosAll);
			}
			else
			{
				// There is an inspector selected, so get the list of meters in his kit.
				EInspector inspector = GetInspectorForID((Guid)inspectorID);
				thermosKit = inspector.ListKitThermos(true);
				HandleToolForNonNullInspector((Guid)inspectorID, thermoID,
					cboThermo, ckKitThermometer, thermosKit, thermosAll);
			}
			SetCurrentSelection(cboThermo, thermoID);
		}

		//--------------------------------------------------------------------
		// Restrict to Kit Checkbox Handler
		//--------------------------------------------------------------------
		private void HandleCheckboxClick(ComboBox cboTool, CheckBox ckKitTool, 
			string toolName, object kitList, object allList)
		{
			Guid? curValue = (Guid?)cboTool.SelectedValue;
			if (ckKitTool.Checked == false)
			{
				cboTool.DataSource = allList;
				SetCurrentSelection(cboTool, curValue);
			}
			else if (curValue == null || ToolInKit((Guid)curValue,(CollectionBase)kitList))
			{
				cboTool.DataSource = kitList;
				SetCurrentSelection(cboTool, curValue);
			}
			else
			{
				string msg = String.Format("Restricting the {0} list to the items in the current inspector's kit\r\nrequires the current {0} selection to be cleared.  Continue?", toolName);
				string caption = String.Format("Factotum: Clear Current {0} Selection?", toolName);
				DialogResult rslt = MessageBox.Show(msg,caption, MessageBoxButtons.YesNo);

				if (rslt == DialogResult.Yes)
				{
					cboTool.DataSource = kitList;
					cboTool.SelectedIndex = 0;
				}
				else
				{
					ckKitTool.Checked = false;
				}

			}
		}

		//--------------------------------------------------------------------
		// Lower Level Helper functions
		//--------------------------------------------------------------------

		private EInspector GetInspectorForID(Guid inspectorID)
		{
			foreach (EInspector inspector in inspectors)
			{
				if (inspector.ID == inspectorID)
					return inspector;
			}
			return null;
		}

		private bool ToolInKit(Guid toolID, System.Collections.CollectionBase toolKit)
		{
			if (toolKit == null) return false;

			foreach (object item in toolKit)
			{
				if (item is EMeter && ((EMeter)item).ID == toolID) return true;
				if (item is EDucer && ((EDucer)item).ID == toolID) return true;
				if (item is ECalBlock && ((ECalBlock)item).ID == toolID) return true;
				if (item is EThermo && ((EThermo)item).ID == toolID) return true;
			}
			return false;
		}

		private void HandleToolForNullInspector(Guid? toolID,
			ComboBox cboTool, CheckBox ckKitTool, object allList)
		{
			cboTool.DataSource = allList;
			if (toolID == null) cboTool.SelectedIndex = 0;
			else cboTool.SelectedValue = toolID;

			ckKitTool.Checked = false;
			ckKitTool.Enabled = false;
		}

		private void HandleToolForNonNullInspector(Guid InspectorID, Guid? toolID,
			ComboBox cboTool, CheckBox ckKitTool, object kitList, object allList)
		{
			ckKitTool.Enabled = true;

			if (toolID == null || ToolInKit((Guid)toolID, (CollectionBase)kitList))
			{
				// If either no meter selected or the selected meter is in 
				// the current inspector's kit - limit to kit
				cboTool.DataSource = kitList;
				ckKitTool.Checked = true;
			}
			else
			{
				// There is a meter selected which is not in the inspector's kit - Don't limit
				cboTool.DataSource = allList;
				ckKitTool.Checked = false;
			}
		}

		private void SetCurrentSelection(ComboBox cbo, Guid? id)
		{
			if (id == null) cbo.SelectedIndex = 0;
			else cbo.SelectedValue = id;
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
			if (!curDset.Valid())
			{
				setAllErrors();
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curDset.Save();
			if (tmpID == null) return false;
			savedInternally = true;
			return true;
		}

		private void btnSelectFile_Click(object sender, EventArgs e)
		{
			if (AnyControlErrors()) return;

			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curDset.Valid())
			{
				setAllErrors();
				MessageBox.Show("The text file cannot be imported until required fields are filled.", "Factotum");
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curDset.Save();

			if (tmpID == null)
			{
				MessageBox.Show("The text file cannot be imported because this dataset could not be saved.", "Factotum");
				return;
			}
			savedInternally = true;
			ofdTextfile.Filter = "Win37DL+ or Ultramate Text files *.txt|*.txt|All Files *.*|*.*";
			DialogResult rslt = ofdTextfile.ShowDialog();
			if (rslt == DialogResult.OK)
			{
				FileInfo fi = new FileInfo(ofdTextfile.FileName);
				ofdTextfile.InitialDirectory = fi.DirectoryName;

				if (curDset.PrepareToLoadTextfile(ofdTextfile.FileName))
				{
					curDset.DsetTextFileName = ofdTextfile.FileName;
					curDset.Save();  // Make sure the file name is also saved since we already saved current data.
					txtFileName.Text = fi.Name;
					switch ((MeterTextFileType)curDset.DsetTextFileFormat)
					{
						case MeterTextFileType.Panametrics:
							parser = new PanametricsParser(curDset, backgroundParser);
							backgroundParser.DoWork += new System.ComponentModel.DoWorkEventHandler(parser.ParseTextFile);
							backgroundParser.RunWorkerAsync();
							break;
						case MeterTextFileType.Krautkramer:
							// Note: Krautkramer meter software allows the user to specify starting rows and
							// columns other than 1A, however the text files that can be exported do not
							// specify starting rows and columns. They do however allow cells to be empty.
							// If more than one tech is working on a grid using Krautkramer meters, it is
							// recommended that either the whole grid be set up for both techs
							// (each will have lots of empties) or the data person use
							// the shift utility after importing to set the starting row and column.
							parser = new KrautkramerParser(curDset, backgroundParser);
							backgroundParser.DoWork += new System.ComponentModel.DoWorkEventHandler(parser.ParseTextFile);
							backgroundParser.RunWorkerAsync();
							break;
						default:
							throw new Exception("Undefined file type");
					}
				}

			}
		}

		private void backgroundParser_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			toolStripProgressBar1.Value = e.ProgressPercentage;
		}

		private void backgroundParser_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			MessageBox.Show(parser.ResultMessage, "Factotum");
		
			toolStripProgressBar1.Value = 0;
			backgroundParser.DoWork -= parser.ParseTextFile;
			parser = null;
			statsNeedRefresh = true;
		}

		private void GetStats()
		{
			if (curDset.DsetTextFilePoints != null && curDset.DsetTextFilePoints != 0)
			{
				lblColsIncluded.Text = EMeasurement.GetColLabel((short)curDset.DsetStartCol) + " to " +
					EMeasurement.GetColLabel((short)curDset.DsetEndCol);

				lblRowsIncluded.Text = (curDset.DsetStartRow + 1) + " to " + (curDset.DsetEndRow + 1);
				lblObstructions.Text = curDset.DsetObstructions.ToString();
				lblEmpties.Text = curDset.DsetEmpties.ToString();
			}
			else
			{
				lblColsIncluded.Text = "N/A";
				lblRowsIncluded.Text = "N/A";
				lblObstructions.Text = "N/A";
				lblEmpties.Text = "N/A";
			}
			lblTotalReadings.Text = curDset.DsetMeasurements.ToString();

			if ((curDset.DsetTextFilePoints != null && curDset.DsetTextFilePoints != 0) ||
				(curDset.DsetAdditionalMeasurementsWithStats != null && curDset.DsetAdditionalMeasurementsWithStats != 0))
			{
				lblMaxInfo.Text = curDset.DsetMaxWall.ToString();
				lblMinInfo.Text = curDset.DsetMinWall.ToString();
				lblMean.Text = string.Format("{0:0.000}", curDset.DsetMeanWall);
				lblStdDev.Text = string.Format("{0:0.000}", curDset.DsetStdevWall);
			}
			else
			{
				lblMaxInfo.Text = "N/A";
				lblMinInfo.Text = "N/A";
				lblMean.Text = "N/A";
				lblStdDev.Text = "N/A";
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab.Name == "Stats" && statsNeedRefresh)
			{
				GetStats();
				statsNeedRefresh = false;
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (ESurvey.DeleteAllForDset(curDset.ID, true))
			{
				curDset.DsetTextFileName = null;
				curDset.DsetTextFileFormat = null;
				txtFileName.Text = null;
				// I'm not sure how to handle this.  We need to force a save if the textfile data
				// is deleted, but how to explain to the user?
				curDset.Save();
				savedInternally = true;
				statsNeedRefresh = true;
			}
		}

		private void txtRowShift_TextChanged(object sender, EventArgs e)
		{
			bool enableBtn = false;
			int rowShift;
			int? startRow = curDset.DsetStartRow;
			if (int.TryParse(txtRowShift.Text, out rowShift))
			{
				if (startRow != null && rowShift != 0 && rowShift + startRow >= 0)
					enableBtn = true;
			}
			btnApplyRowShift.Enabled = enableBtn;
		}

		private void txtColShift_TextChanged(object sender, EventArgs e)
		{
			bool enableBtn = false;
			int colShift;
			int? startCol = curDset.DsetStartCol;
			if (int.TryParse(txtColShift.Text, out colShift))
			{
				if (startCol != null && colShift != 0 && colShift + startCol >= 0)
					enableBtn = true;
			}
			btnApplyColShift.Enabled = enableBtn;
		}

		private void txtDeleteRow_TextChanged(object sender, EventArgs e)
		{
			bool enableBtn = false;
			int delStart;
			int delEnd;
			int? startRow = curDset.DsetStartRow;
			int? endRow = curDset.DsetEndRow;
			if (int.TryParse(txtDeleteStartRow.Text, out delStart) &&
				int.TryParse(txtDeleteEndRow.Text, out delEnd))
			{
                // Note: we have to subtract 1 for zero-based rows in back end
				if (startRow != null && endRow != null
					&& delStart-1 >= startRow && delEnd-1 <= endRow+1 && delEnd >= delStart)
					enableBtn = true;
			}
			btnApplyDeleteRows.Enabled = enableBtn;
		}

		private void txtDeleteCol_TextChanged(object sender, EventArgs e)
		{
			bool enableBtn = false;
			int? startCol = curDset.DsetStartCol;
			int? endCol = curDset.DsetEndCol;
			int? delStartCol = EMeasurement.GetColForLabel(txtDeleteStartCol.Text.ToUpper());
			int? delEndCol = EMeasurement.GetColForLabel(txtDeleteEndCol.Text.ToUpper());
			if (startCol != null && endCol != null && delStartCol != null && delEndCol != null
				&& startCol <= delStartCol && delStartCol <= delEndCol && delEndCol <= endCol)
			{
					enableBtn = true;
			}
			btnApplyDeleteCols.Enabled = enableBtn;
		}

		private void btnApplyRowShift_Click(object sender, EventArgs e)
		{
			curDset.ShiftRows(int.Parse(txtRowShift.Text));
			MessageBox.Show("Row shift complete.", "Factotum");
			statsNeedRefresh = true;
		}

		private void btnApplyColShift_Click(object sender, EventArgs e)
		{
			curDset.ShiftCols(int.Parse(txtColShift.Text));
			MessageBox.Show("Column shift complete.", "Factotum");
			statsNeedRefresh = true;
		}

		private void btnApplyReverseRows_Click(object sender, EventArgs e)
		{
			curDset.ReverseRows();
			MessageBox.Show("Row reversal complete.", "Factotum");
			statsNeedRefresh = true;
		}

		private void btnApplyReverseCols_Click(object sender, EventArgs e)
		{
			curDset.ReverseCols();
			MessageBox.Show("Column reversal complete.", "Factotum");
			statsNeedRefresh = true;
		}

        private void btnApplyReverseColOrientation_Click(object sender, EventArgs e)
        {
            if (curDset.DsetStartCol != 0) 
            {
                MessageBox.Show("Before proceeding, the data must be shifted so that the first column is A","Factotum");
                return;
            }
            // First reverse the columns: A, B, C, D => D, C, B, A
            curDset.ReverseCols();
            // Next shift the columns right by 1
            curDset.ShiftCols(1);
            // Next move the last column to the zero position
            curDset.MoveLastColumnToZero();
            MessageBox.Show("Column orientation reversal complete.","Factotum");
        }
        private void btnApplyTranspose_Click(object sender, EventArgs e)
		{
			curDset.Transpose();
			MessageBox.Show("Transpose complete.", "Factotum");
			statsNeedRefresh = true;
		}

		private void btnApplyDeleteRows_Click(object sender, EventArgs e)
		{
			DialogResult resp = MessageBox.Show("Measurement Data may be lost.  Continue?",
				"Factotum: Confirm Row Delete",MessageBoxButtons.YesNo);
			if (resp == DialogResult.Yes)
			{
                // need to subtract 1 for zero-based rows in back end
				curDset.DeleteRows(int.Parse(txtDeleteStartRow.Text)-1, int.Parse(txtDeleteEndRow.Text)-1);
				MessageBox.Show("Row Deletion complete.", "Factotum");
				statsNeedRefresh = true;
			}
		}

		private void btnApplyDeleteCols_Click(object sender, EventArgs e)
		{
			int delStartCol = (int)EMeasurement.GetColForLabel(txtDeleteStartCol.Text.ToUpper());
			int delEndCol = (int)EMeasurement.GetColForLabel(txtDeleteEndCol.Text.ToUpper());
			curDset.DeleteCols(delStartCol, delEndCol);
			MessageBox.Show("Column Deletion complete.", "Factotum");
			statsNeedRefresh = true;
		}

		private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPage.Name == "SpecialFieldData")
			{
				if (!performSilentSave())
				{
					e.Cancel = true;
					return;
				}
				if (curSpecialCalValue == null)
				{
					curSpecialCalValue = new ESpecialCalValue();
					curSpecialCalValue.SpecialCalValueDstID = curDset.ID;
				}
			}
		}

		private void dgvSpecialFieldData_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
		}





	}
}