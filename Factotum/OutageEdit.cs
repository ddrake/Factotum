using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Factotum
{
	public partial class OutageEdit : Form, IEntityEditForm
	{
		private EOutage curOutage;
		private EGridProcedureCollection gridProcedures;
		private EInspectorCollection inspectors;
		//private TickCounter tc;
		private ECalibrationProcedureCollection calProcs;
		private ECouplantTypeCollection couplantTypes;
		private bool newRecord;

			public IEntity Entity
		{
			get { return curOutage; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public OutageEdit(Guid? ID)
			: this(ID, null){}

		public OutageEdit(Guid? ID, Guid? unitID)
		{
			InitializeComponent();
			curOutage = new EOutage();
			curOutage.Load(ID);
			if (unitID != null) curOutage.OutageUntID = unitID;
			//tc = new TickCounter("initializing controls");
			newRecord = (ID == null);
			InitializeControls();
			//tc.Done();
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			calProcs = ECalibrationProcedure.ListByName(true, !newRecord, true);
			cboCalibrationProc.DataSource = calProcs;
			cboCalibrationProc.DisplayMember = "CalibrationProcedureName";
			cboCalibrationProc.ValueMember = "ID";

			couplantTypes = ECouplantType.ListByName(true, !newRecord, true);
			cboCouplantType.DataSource = couplantTypes;
			cboCouplantType.DisplayMember = "CouplantTypeName";
			cboCouplantType.ValueMember = "ID";

			getGridProceduresWithAssignments();
			getInspectorsWithAssignments();

			SetControlValues();
			this.btnExportConfig.Enabled = Globals.IsMasterDB;

			ESite.Changed += new EventHandler<EntityChangedEventArgs>(ESiteOrEUnit_Changed);
			EUnit.Changed += new EventHandler<EntityChangedEventArgs>(ESiteOrEUnit_Changed);
			ECalibrationProcedure.Changed += new EventHandler<EntityChangedEventArgs>(ECalibrationProcedure_Changed);
			ECouplantType.Changed += new EventHandler<EntityChangedEventArgs>(ECouplantType_Changed);
			EGridProcedure.Changed += new EventHandler<EntityChangedEventArgs>(EGridProcedure_Changed);
			EInspector.Changed += new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			btnGridColLayoutCCW.CheckedChanged += new System.EventHandler(btnGridColLayoutCCW_CheckedChanged);
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		void ECouplantType_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboCouplantType.SelectedValue;
			couplantTypes = ECouplantType.ListByName(true, !newRecord, true);
			cboCouplantType.DataSource = couplantTypes;
			if (currentValue == null)
				cboCouplantType.SelectedIndex = 0;
			else
				cboCouplantType.SelectedValue = currentValue;
		}

		void ECalibrationProcedure_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboCalibrationProc.SelectedValue;
			calProcs = ECalibrationProcedure.ListByName(true, !newRecord, true);
			cboCalibrationProc.DataSource = calProcs;
			if (currentValue == null)
				cboCalibrationProc.SelectedIndex = 0;
			else
				cboCalibrationProc.SelectedValue = currentValue;
		}

		void ESiteOrEUnit_Changed(object sender, EntityChangedEventArgs e)
		{
			updateHeaderLabel();
		}

		// This is not ideal, but maybe a reasonable compromise.
		// If any inspector or grid procedure record changed while this form is open,
		// we abandon any changes that the user may have made to the inspector assignments or
		// grid procedure assignmenets.
		void EInspector_Changed(object sender, EntityChangedEventArgs e)
		{
			getInspectorsWithAssignments();
		}

		void EGridProcedure_Changed(object sender, EntityChangedEventArgs e)
		{
			getGridProceduresWithAssignments();
		}

		// If the user cancels out, just close.
		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
			DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveAndClose();
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curOutage.OutageNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curOutage.OutageNameErrMsg);
		}
		private void txtFacPhone_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curOutage.OutageFacPhoneLengthOk(txtFacPhone.Text);
			errorProvider1.SetError(txtFacPhone, curOutage.OutageFacPhoneErrMsg);
		}

		private void txtCouplantBatch_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curOutage.OutageCouplantBatchLengthOk(txtCouplantBatch.Text);
			errorProvider1.SetError(txtCouplantBatch, curOutage.OutageCouplantBatchErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curOutage.OutageNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curOutage.OutageNameErrMsg);
		}

		// Re-center the label when the form is resized.
		private void OutageEdit_Resize(object sender, EventArgs e)
		{
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
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
			if (!curOutage.Valid())
			{
				setAllErrors();
				return;
			}

			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curOutage.Save();
			if (tmpID != null)
			{
				UpdateAssignments();
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		private void UpdateAssignments()
		{
			// We need to do these updates after saving because they require a valid Outage ID

			// Update grid procedures
			foreach (EGridProcedure grp in gridProcedures)
				grp.IsAssignedToYourOutage = false;

			foreach (int idx in clbGridProcedures.CheckedIndices)
				gridProcedures[idx].IsAssignedToYourOutage = true;

			EGridProcedure.UpdateAssignmentsToOutage(curOutage.ID, gridProcedures);

			// Update inspectors
			foreach (EInspector ins in inspectors)
				ins.IsAssignedToYourOutage = false;

			foreach (int idx in clbInspectors.CheckedIndices)
				inspectors[idx].IsAssignedToYourOutage = true;

			EInspector.UpdateAssignmentsToOutage(curOutage.ID, inspectors);

		}
		// Set the form controls to the outage object values.
		private void SetControlValues()
		{
			updateHeaderLabel();

			txtName.Text = curOutage.OutageName;
			txtCouplantBatch.Text = curOutage.OutageCouplantBatch;
			txtFacPhone.Text = curOutage.OutageFacPhone;
			if (curOutage.OutageClpID != null) cboCalibrationProc.SelectedValue = curOutage.OutageClpID;
			if (curOutage.OutageCptID != null) cboCouplantType.SelectedValue = curOutage.OutageCptID;
			if (curOutage.OutageStartedOn == null)	dtpStartDate.Value = DateTime.Today;
			else	dtpStartDate.Value = (DateTime)curOutage.OutageStartedOn;
			if (curOutage.OutageEndedOn == null) dtpEndDate.Value = DateTime.Today;
			else	dtpEndDate.Value = (DateTime)curOutage.OutageEndedOn;
			btnGridColLayoutCCW.Checked = curOutage.OutageGridColDefaultCCW;
			lblOutageDataImportedOn.Text = (curOutage.OutageDataImportedOn == null ? "" : string.Format("Outage Data Imported {0:d}",curOutage.OutageDataImportedOn));

		}

		private void updateHeaderLabel()
		{
			if (curOutage.OutageUntID != null)
			{
				EUnit unt = new EUnit(curOutage.OutageUntID);
				lblSiteName.Text = "Facilty: " + unt.UnitNameWithSite;
			}
			else lblSiteName.Text = "Unknown Facility";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curOutage.OutageNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curOutage.OutageName = txtName.Text;
			curOutage.OutageCouplantBatch =txtCouplantBatch.Text;
			curOutage.OutageFacPhone = txtFacPhone.Text;
			curOutage.OutageClpID = (Guid?)cboCalibrationProc.SelectedValue;
			curOutage.OutageCptID = (Guid?)cboCouplantType.SelectedValue;
			curOutage.OutageStartedOn = dtpStartDate.Value;
			curOutage.OutageEndedOn = dtpEndDate.Value;
			curOutage.OutageGridColDefaultCCW = btnGridColLayoutCCW.Checked;
		}
		private void getInspectorsWithAssignments()
		{
			inspectors = EInspector.ListWithAssignmentsForOutage(curOutage.ID, true, !newRecord);
			clbInspectors.Items.Clear();
			foreach (EInspector ins in inspectors)
			{
				clbInspectors.Items.Add(ins.InspectorName + ", " + ins.InspectorLevelString +
					(ins.InspectorIsActive ? "" : " (inactive)"), ins.IsAssignedToYourOutage);
			}
		}
		private void getGridProceduresWithAssignments()
		{
			gridProcedures = EGridProcedure.ListWithAssignmentsForOutage(curOutage.ID, true, !newRecord);
			clbGridProcedures.Items.Clear();
			foreach (EGridProcedure grp in gridProcedures)
			{
				clbGridProcedures.Items.Add(grp.GridProcedureName +
					(grp.GridProcedureIsActive ? "" : " (inactive)"), grp.IsAssignedToYourOutage);
			}
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
			if (!curOutage.Valid())
			{
				setAllErrors();
				MessageBox.Show("Make sure all errors are cleared first", "Factotum");
				return false;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curOutage.Save();
			if (tmpID != null)
			{
				UpdateAssignments();
				return true;
			}
			else return false;
		}

		// If the user clicks to Make Field Data Sheets, perform a silent save, 
		// then try to create the workbook.
		private void btnMakeFieldDataSheets_Click(object sender, EventArgs e)
		{
			if (!performSilentSave()) return;
			string outFilePath = Globals.FactotumDataFolder + "\\" + UserSettings.sets.FieldDataSheetName;

			if (System.IO.File.Exists(outFilePath))
			{
				DialogResult rslt = MessageBox.Show("The file " + UserSettings.sets.FieldDataSheetName +
					" exists.  Overwrite it?", "Factotum: Overwrite Existing File?", MessageBoxButtons.YesNo);
				if (rslt != DialogResult.Yes) return;
			}

			fieldDataSheetsWorker.RunWorkerAsync(outFilePath);
		}

		private void fieldDataSheetsWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			FieldDataSheetExporter exporter = new FieldDataSheetExporter((Guid)this.curOutage.ID,
				(string)e.Argument, fieldDataSheetsWorker);
			exporter.CreateWorkbook();
			e.Result = exporter.result;
		}

		private void fieldDataSheetsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			toolStripProgressBar1.Value = e.ProgressPercentage;
		}

		private void fieldDataSheetsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (e.Error.Message.StartsWith("A lock violation"))
				{
					MessageBox.Show("Please make sure the Field Data Sheet workbook is closed and retry.", "Factotum");
				}
				else
				{
					MessageBox.Show("An error occurred creating the Field Data Sheet workbook.", "Factotum");
				}
				ExceptionLogger.LogException(e.Error);
			}
			else if (e.Result != null)
			{
				MessageBox.Show((string)e.Result, "Factotum");
			}
			else
			{
				MessageBox.Show("'Field Data Sheets.xls' has been created in the Factotum Data Folder.", "Factotum");
			}
			toolStripProgressBar1.Value = 0;
			toolStripStatusLabel1.Text = "Ready";
		}

		private void btnGridColLayoutCCW_CheckedChanged(object sender, EventArgs e)
		{
			MessageBox.Show("New Grids will have their column layouts set to " + (btnGridColLayoutCCW.Checked ? "CCW" : "CW") + " by default.\n" +
				"The column layout settings of existing grids will not be affected.","Factotum: Default Column Layouts Changed"); 
		}

		//private void btnPrintLabels_Click(object sender, EventArgs e)
		//{
		//   WorkOrderLabelReport curReport = new WorkOrderLabelReport();
		//   if (curReport.hasData) curReport.Print(true);
		//}

		private void btnPreviewLabels_Click(object sender, EventArgs e)
		{
			WorkOrderLabelReport curReport = new WorkOrderLabelReport();
			if (curReport.hasData) curReport.Print(false);
		}

		private void btnExportConfig_Click(object sender, EventArgs e)
		{
			if (!performSilentSave()) return;
			// If we've managed to save the outage, we should have both a name and an ID

			ActivationKeyGenerator frm = new ActivationKeyGenerator(true);
			frm.ShowDialog();
			if (frm.key == null) return;

			string activationKey = frm.key;

			saveFileDialog1.InitialDirectory = Globals.FactotumDataFolder;
			saveFileDialog1.Filter = "Factotum Outage Data Files *.ofac | *.ofac";
			saveFileDialog1.DefaultExt = ".ofac";
			// Get a default file name based on the outage.
			saveFileDialog1.FileName = Globals.getUniqueBackupFileName(
				Globals.FactotumDataFolder, txtName.Text, false, false, false);

			DialogResult rslt = saveFileDialog1.ShowDialog();
			// Note: If the user selects a file that already exists, the Save Dialog
			// will handle warning the user
			if (rslt == DialogResult.OK)
			{
				// first close the global connection
				Globals.cnn.Close();
				// Overwrite flag is specified
				File.Copy(Globals.FactotumDatabaseFilePath, saveFileDialog1.FileName, true);
				Globals.cnn.Open();
				// Connect to the database copy we just created in the desktop data folder
				// Delete all data from it that doesn't pertain to the currently selected outage
				// Set the activation key in that database
				OutageExporter.Amputate(saveFileDialog1.FileName, curOutage.ID, activationKey);
			}

		}

	}
}