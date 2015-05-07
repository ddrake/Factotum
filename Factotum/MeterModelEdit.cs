using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class MeterModelEdit : Form, IEntityEditForm
	{
		private EMeterModel curMeterModel;
		private DataTable ducerModels;

		public IEntity Entity
		{
			get { return curMeterModel; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		public MeterModelEdit()
			: this(null){}

		public MeterModelEdit(Guid? ID)
		{
			InitializeComponent();
			curMeterModel = new EMeterModel();
			curMeterModel.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			ducerModels = EDucerModel.GetWithAssignmentsForMeterModel(curMeterModel.ID, !newRecord);
			for (int dmRow = 0; dmRow < ducerModels.Rows.Count; dmRow++)
			{
				string modelName = (string)ducerModels.Rows[dmRow]["DucerModelName"];
				bool isActive = (bool)ducerModels.Rows[dmRow]["DucerModelIsActive"];
				bool isAssigned = Convert.ToBoolean(ducerModels.Rows[dmRow]["IsAssigned"]);
				clbDucerModels.Items.Add(modelName +(isActive ? "" : " (inactive)"), isAssigned);
			}
			SetControlValues();
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

		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveAndClose();
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curMeterModel.MeterModelNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curMeterModel.MeterModelNameErrMsg);
		}
		private void txtManufacturer_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curMeterModel.MeterModelManfNameLengthOk(txtManufacturer.Text);
			errorProvider1.SetError(txtManufacturer, curMeterModel.MeterModelManfNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curMeterModel.MeterModelNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curMeterModel.MeterModelNameErrMsg);
		}

		private void txtManufacturer_Validating(object sender, CancelEventArgs e)
		{
			curMeterModel.MeterModelManfNameValid(txtManufacturer.Text);
			errorProvider1.SetError(txtManufacturer, curMeterModel.MeterModelManfNameErrMsg);
		}

		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Meter model should only be Activated if it has returned to service.  Continue?" :
				"A Meter model should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Meter Model Status Change", MessageBoxButtons.YesNo);
			if (r == DialogResult.No) ckActive.Checked = !ckActive.Checked;
		}

		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------

		// No prompting is performed.  The user should understand the meanings of OK and Cancel.
		private void SaveAndClose()
		{
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curMeterModel.Valid())
			{
				setAllErrors();
				return;
			}

			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curMeterModel.Save();
			if (tmpID != null)
			{
				// We need to do these updates after saving because they require a valid Outage ID
				
				// Update ducerModels table from checkboxes
				for (int dmRow = 0; dmRow < ducerModels.Rows.Count; dmRow++)
				{
					ducerModels.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbDucerModels.CheckedIndices)
				{
					ducerModels.Rows[idx]["IsAssigned"] = 1;
				}

				EDucerModel.UpdateAssignmentsToMeterModel(curMeterModel.ID, ducerModels);

				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the outage object values.
		private void SetControlValues()
		{
			txtName.Text = curMeterModel.MeterModelName;
			txtManufacturer.Text = curMeterModel.MeterModelManfName;
			ckActive.Checked = curMeterModel.MeterModelIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curMeterModel.MeterModelNameErrMsg);
			errorProvider1.SetError(txtManufacturer, curMeterModel.MeterModelManfNameErrMsg);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curMeterModel.MeterModelName = txtName.Text;
			curMeterModel.MeterModelManfName = txtManufacturer.Text;
			curMeterModel.MeterModelIsActive = ckActive.Checked;
		}
	}
}