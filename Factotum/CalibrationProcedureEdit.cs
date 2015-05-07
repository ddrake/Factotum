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
	public partial class CalibrationProcedureEdit : Form, IEntityEditForm
	{
		private ECalibrationProcedure curCalibrationProcedure;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curCalibrationProcedure; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public CalibrationProcedureEdit()
			: this(null){}

		public CalibrationProcedureEdit(Guid? ID)
		{
			InitializeComponent();
			curCalibrationProcedure = new ECalibrationProcedure();
			curCalibrationProcedure.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New Calibration Procedure" : "Edit Calibration Procedure";
			this.btnOK.Enabled = Globals.ActivationOK;
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		// If the user cancels out, just close.
		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
			DialogResult = DialogResult.Cancel;
		}

		// If the user clicks OK, first validate and set the error text 
		// for any controls with invalid values.
		// If it validates, try to save.
		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveAndClose();
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curCalibrationProcedure.CalibrationProcedureNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curCalibrationProcedure.CalibrationProcedureNameErrMsg);
		}

		private void txtDescription_TextChanged(object sender, EventArgs e)
		{
			curCalibrationProcedure.CalibrationProcedureDescriptionLengthOk(txtDescription.Text);
			errorProvider1.SetError(txtDescription, curCalibrationProcedure.CalibrationProcedureDescriptionErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curCalibrationProcedure.CalibrationProcedureNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curCalibrationProcedure.CalibrationProcedureNameErrMsg);
		}

		private void txtDescription_Validating(object sender, CancelEventArgs e)
		{
			curCalibrationProcedure.CalibrationProcedureDescriptionValid(txtDescription.Text);
			errorProvider1.SetError(txtDescription, curCalibrationProcedure.CalibrationProcedureDescriptionErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Calibration Procedure should only be Activated if it is no longer discontinued.  Continue?" :
				"A Calibration Procedure should only be Inactivated if it is discontinued.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Calibration Procedure Status Change", MessageBoxButtons.YesNo);
			if (r == DialogResult.No) ckActive.Checked = !ckActive.Checked;
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
			if (!curCalibrationProcedure.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curCalibrationProcedure.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curCalibrationProcedure.CalibrationProcedureName;
			txtDescription.Text = curCalibrationProcedure.CalibrationProcedureDescription;
			ckActive.Checked = curCalibrationProcedure.CalibrationProcedureIsActive;

		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curCalibrationProcedure.CalibrationProcedureNameErrMsg);
			errorProvider1.SetError(txtDescription, curCalibrationProcedure.CalibrationProcedureDescriptionErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtDescription).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curCalibrationProcedure.CalibrationProcedureName = txtName.Text;
			curCalibrationProcedure.CalibrationProcedureDescription = txtDescription.Text;
			curCalibrationProcedure.CalibrationProcedureIsActive = ckActive.Checked;
		}

	}
}