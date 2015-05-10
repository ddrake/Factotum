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
	public partial class MeterEdit : Form, IEntityEditForm
	{
		private EMeter curMeter;
		private EMeterModelCollection models;
		private EKitCollection kits;
		private bool newRecord;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curMeter; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public MeterEdit()
			: this(null){}

		public MeterEdit(Guid? ID)
		{
			InitializeComponent();
			curMeter = new EMeter();
			curMeter.Load(ID);
			newRecord = (ID == null);
			InitializeControls();
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			models = EMeterModel.ListByName(true, !newRecord, false);
			cboModel.DataSource = models;
			cboModel.DisplayMember = "MeterModelName";
			cboModel.ValueMember = "ID";
			kits = EKit.ListByName(true);
			cboKit.DataSource = kits;
			cboKit.DisplayMember = "ToolKitName";
			cboKit.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Meter" : "Edit Meter";
			EMeterModel.Changed += new EventHandler<EntityChangedEventArgs>(EMeterModel_Changed);
			EKit.Changed += new EventHandler<EntityChangedEventArgs>(EKit_Changed);
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		void EMeterModel_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboModel.SelectedValue;
			models = EMeterModel.ListByName(true, !newRecord, false);
			cboModel.DataSource = models;
			if (currentValue == null)
				cboModel.SelectedIndex = -1;
			else
				cboModel.SelectedValue = currentValue;
		}

		void EKit_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboKit.SelectedValue;
			kits = EKit.ListByName(true);
			cboKit.DataSource = kits;
			if (currentValue == null)
				cboKit.SelectedIndex = 0;
			else
				cboKit.SelectedValue = currentValue;
		}

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
			curMeter.MeterSerialNumberLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curMeter.MeterSerialNumberErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curMeter.MeterSerialNumberValid(txtName.Text);
			errorProvider1.SetError(txtName, curMeter.MeterSerialNumberErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Meter should only be Activated if it has returned to service.  Continue?" :
				"A Meter should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Meter Status Change", MessageBoxButtons.YesNo);
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
			if (!curMeter.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curMeter.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curMeter.MeterSerialNumber;
			if (curMeter.MeterKitID != null) cboKit.SelectedValue = curMeter.MeterKitID;
			if (curMeter.MeterMmlID != null) cboModel.SelectedValue = curMeter.MeterMmlID;
			dtpCalibrationDueOn.Value = curMeter.MeterCalDueDate == null ?
				DateTime.Today : (DateTime)curMeter.MeterCalDueDate;
			ckActive.Checked = curMeter.MeterIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curMeter.MeterSerialNumberErrMsg);
			errorProvider1.SetError(cboModel, curMeter.MeterMmlIDErrMsg);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curMeter.MeterSerialNumber = txtName.Text;
			curMeter.MeterKitID = (Guid?)cboKit.SelectedValue;
			curMeter.MeterMmlID = (Guid?)cboModel.SelectedValue;
			curMeter.MeterCalDueDate = (DateTime)dtpCalibrationDueOn.Value;
			curMeter.MeterIsActive = ckActive.Checked;
		}

	}
}