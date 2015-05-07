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
	public partial class DucerModelEdit : Form, IEntityEditForm
	{
		private EDucerModel curDucerModel;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curDucerModel; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public DucerModelEdit()
			: this(null){}

		public DucerModelEdit(Guid? ID)
		{
			InitializeComponent();
			curDucerModel = new EDucerModel();
			curDucerModel.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{

			SetControlValues();
			this.Text = newRecord ? "New Transducer Model" : "Edit Transducer Model";
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
			curDucerModel.DucerModelNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curDucerModel.DucerModelNameErrMsg);
		}

		private void txtFrequency_TextChanged(object sender, EventArgs e)
		{
			curDucerModel.DucerModelFrequencyLengthOk(txtFrequency.Text);
			errorProvider1.SetError(txtFrequency, curDucerModel.DucerModelFrequencyErrMsg);
		}

		private void txtSize_TextChanged(object sender, EventArgs e)
		{
			curDucerModel.DucerModelSizeLengthOk(txtSize.Text);
			errorProvider1.SetError(txtSize, curDucerModel.DucerModelSizeErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curDucerModel.DucerModelNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curDucerModel.DucerModelNameErrMsg);
		}

		private void txtFrequency_Validating(object sender, CancelEventArgs e)
		{
			curDucerModel.DucerModelFrequencyValid(txtFrequency.Text);
			errorProvider1.SetError(txtFrequency, curDucerModel.DucerModelFrequencyErrMsg);
		}

		private void txtSize_Validating(object sender, CancelEventArgs e)
		{
			curDucerModel.DucerModelSizeValid(txtSize.Text);
			errorProvider1.SetError(txtSize, curDucerModel.DucerModelSizeErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Transducer model should only be Activated if it has returned to service.  Continue?" :
				"A Transducer model should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Transducer Model Status Change", MessageBoxButtons.YesNo);
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
			if (!curDucerModel.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curDucerModel.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curDucerModel.DucerModelName;
			txtFrequency.Text = Util.GetFormattedDecimal(curDucerModel.DucerModelFrequency);
			txtSize.Text = Util.GetFormattedDecimal(curDucerModel.DucerModelSize);
			ckActive.Checked = curDucerModel.DucerModelIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curDucerModel.DucerModelNameErrMsg);
			errorProvider1.SetError(txtFrequency, curDucerModel.DucerModelFrequencyErrMsg);
			errorProvider1.SetError(txtSize, curDucerModel.DucerModelSizeErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
					errorProvider1.GetError(txtFrequency).Length > 0 ||
					errorProvider1.GetError(txtSize).Length > 0
					);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curDucerModel.DucerModelName = txtName.Text;
			curDucerModel.DucerModelFrequency = Util.GetNullableDecimalForString(txtFrequency.Text);
			curDucerModel.DucerModelSize = Util.GetNullableDecimalForString(txtSize.Text);
			curDucerModel.DucerModelIsActive = ckActive.Checked;
		}
	}
}