using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class CustomerEdit : Form, IEntityEditForm
	{
		private ECustomer curCustomer;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curCustomer; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		public CustomerEdit(Guid? ID)
		{
			InitializeComponent();
			curCustomer = new ECustomer();
			curCustomer.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New Customer" : "Edit Customer";
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
			curCustomer.CustomerNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curCustomer.CustomerNameErrMsg);
		}
		private void txtFullName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curCustomer.CustomerFullNameLengthOk(txtFullName.Text);
			errorProvider1.SetError(txtFullName, curCustomer.CustomerFullNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curCustomer.CustomerNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curCustomer.CustomerNameErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Customer should only be Activated if it has returned to service.  Continue?" :
				"A Customer should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Customer Status Change", MessageBoxButtons.YesNo);
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
			if (!curCustomer.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curCustomer.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the customer object values.
		private void SetControlValues()
		{
			txtName.Text = curCustomer.CustomerName;
			txtFullName.Text = curCustomer.CustomerFullName;
			ckActive.Checked = curCustomer.CustomerIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curCustomer.CustomerNameErrMsg);
			errorProvider1.SetError(txtFullName, curCustomer.CustomerFullNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtFullName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curCustomer.CustomerName = txtName.Text;
			curCustomer.CustomerFullName = txtFullName.Text;
			curCustomer.CustomerIsActive = ckActive.Checked;
		}

	}
}