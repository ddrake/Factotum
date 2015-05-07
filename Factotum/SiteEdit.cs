using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class SiteEdit : Form, IEntityEditForm
	{
		private ESite curSite;
		private ECustomerCollection customers;
		private ECalibrationProcedureCollection calprocs;
		private bool newRecord;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curSite; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public SiteEdit(Guid? ID)
			: this(ID, null){}

		public SiteEdit(Guid? ID, Guid? customerID)
		{
			InitializeComponent();
			curSite = new ESite();
			curSite.Load(ID);
			newRecord = (ID == null);
			if (customerID != null) curSite.SiteCstID = customerID;
			InitializeControls();
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			// Customers combo box
			customers = ECustomer.ListByName(true, !newRecord, false);

			cboCustomer.DataSource = customers;
			cboCustomer.DisplayMember = "CustomerName";
			cboCustomer.ValueMember = "ID";
			if (newRecord) cboCustomer.Enabled = false;

			// Default Calibration procedure combo box
			calprocs = ECalibrationProcedure.ListByName(true, !newRecord, true);
			cboDefaultCalProcedure.DataSource = calprocs;
			cboDefaultCalProcedure.DisplayMember = "CalibrationProcedureName";
			cboDefaultCalProcedure.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Site" : "Edit Site";
			ECustomer.Changed += new EventHandler<EntityChangedEventArgs>(ECustomer_Changed);
			ECalibrationProcedure.Changed += new EventHandler<EntityChangedEventArgs>(ECalibrationProcedure_Changed);
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		void ECalibrationProcedure_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboDefaultCalProcedure.SelectedValue;
			calprocs = ECalibrationProcedure.ListByName(true, !newRecord, true);
			cboDefaultCalProcedure.DataSource = calprocs;
			if (currentValue == null)
				cboDefaultCalProcedure.SelectedIndex = 0;
			else
				cboDefaultCalProcedure.SelectedValue = currentValue;
		}

		void ECustomer_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboCustomer.SelectedValue;
			customers = ECustomer.ListByName(true, !newRecord, false);
			cboCustomer.DataSource = customers;
			if (currentValue == null)
				cboCustomer.SelectedIndex = -1;
			else
				cboCustomer.SelectedValue = currentValue;
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
			curSite.SiteNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curSite.SiteNameErrMsg);
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtSiteFullName_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curSite.SiteFullNameLengthOk(txtSiteFullName.Text);
			errorProvider1.SetError(txtSiteFullName, curSite.SiteFullNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curSite.SiteNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curSite.SiteNameErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Site should only be Activated if it has returned to service.  Continue?" :
				"A Site should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Site Status Change", MessageBoxButtons.YesNo);
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
			if (!curSite.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curSite.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curSite.SiteName;
			txtSiteFullName.Text = curSite.SiteFullName;
			ckActive.Checked = curSite.SiteIsActive;
			cboCustomer.SelectedValue = curSite.SiteCstID;
			if (curSite.SiteClpID != null)	cboDefaultCalProcedure.SelectedValue = curSite.SiteClpID;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curSite.SiteNameErrMsg);
			errorProvider1.SetError(txtSiteFullName, curSite.SiteFullNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtSiteFullName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curSite.SiteName = txtName.Text;
			curSite.SiteFullName = txtSiteFullName.Text;
			curSite.SiteCstID = (Guid?)cboCustomer.SelectedValue;
			curSite.SiteClpID = (Guid?)cboDefaultCalProcedure.SelectedValue;
			curSite.SiteIsActive = ckActive.Checked;
		}
	}
}