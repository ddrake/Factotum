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
	public partial class ThermoEdit : Form, IEntityEditForm
	{
		private EThermo curThermo;
		private EKitCollection kits;
		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curThermo; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public ThermoEdit()
			: this(null){}

		public ThermoEdit(Guid? ID)
		{
			InitializeComponent();
			curThermo = new EThermo();
			curThermo.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			kits = EKit.ListByName(true);
			cboKit.DataSource = kits;
			cboKit.DisplayMember = "ToolKitName";
			cboKit.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Thermometer" : "Edit Thermometer";
			this.btnOK.Enabled = Globals.ActivationOK;
			EKit.Changed += new EventHandler<EntityChangedEventArgs>(EKit_Changed);
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

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
			curThermo.ThermoSerialNumberLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curThermo.ThermoSerialNumberErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curThermo.ThermoSerialNumberValid(txtName.Text);
			errorProvider1.SetError(txtName, curThermo.ThermoSerialNumberErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Thermometer should only be Activated if it has returned to service.  Continue?" :
				"A Thermometer should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Thermometer Status Change", MessageBoxButtons.YesNo);
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
			if (!curThermo.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curThermo.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curThermo.ThermoSerialNumber;
			if (curThermo.ThermoKitID != null) cboKit.SelectedValue = curThermo.ThermoKitID;
			ckActive.Checked = curThermo.ThermoIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curThermo.ThermoSerialNumberErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curThermo.ThermoSerialNumber = txtName.Text;
			curThermo.ThermoKitID = (Guid?)cboKit.SelectedValue;
			curThermo.ThermoIsActive = ckActive.Checked;
		}

	}
}