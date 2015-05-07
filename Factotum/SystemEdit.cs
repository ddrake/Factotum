using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class SystemEdit : Form, IEntityEditForm
	{
		private ESystem curSystem;

		public IEntity Entity
		{
			get { return curSystem; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public SystemEdit(Guid? ID)
			: this(ID, null){}

		public SystemEdit(Guid? ID, Guid? unitID)
		{
			InitializeComponent();
			curSystem = new ESystem();
			curSystem.Load(ID);
			if (unitID != null) curSystem.SystemUntID = unitID;
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New System" : "Edit System";
			btnOK.Enabled = Globals.ActivationOK;
			ESite.Changed += new EventHandler<EntityChangedEventArgs>(ESiteOrEUnit_Changed);
			EUnit.Changed += new EventHandler<EntityChangedEventArgs>(ESiteOrEUnit_Changed);
		}

		void ESiteOrEUnit_Changed(object sender, EntityChangedEventArgs e)
		{
			updateHeaderLabel();
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
			curSystem.SystemNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curSystem.SystemNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curSystem.SystemNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curSystem.SystemNameErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A System should only be Activated if it has returned to service.  Continue?" :
				"A System should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm System Status Change", MessageBoxButtons.YesNo);
			if (r == DialogResult.No) ckActive.Checked = !ckActive.Checked;
		}

		// Re-center the label when the form is resized.
		private void SystemEdit_Resize(object sender, EventArgs e)
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
			if (!curSystem.Valid())
			{
				setAllErrors();
				return;
			}

			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curSystem.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}
		// Set the form controls to the system object values.
		private void SetControlValues()
		{
			updateHeaderLabel();			
			txtName.Text = curSystem.SystemName;
			ckActive.Checked = curSystem.SystemIsActive;
		}

		private void updateHeaderLabel()
		{
			if (curSystem.SystemUntID != null)
			{
				EUnit unt = new EUnit(curSystem.SystemUntID);
				lblSiteName.Text = "Facilty: " + unt.UnitNameWithSite;
			}
			else lblSiteName.Text = "Unknown Facility";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curSystem.SystemNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curSystem.SystemName = txtName.Text;
			curSystem.SystemIsActive = ckActive.Checked;
		}
	}
}