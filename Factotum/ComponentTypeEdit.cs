using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class ComponentTypeEdit : Form, IEntityEditForm
	{
		private EComponentType curType;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curType; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public ComponentTypeEdit(Guid? ID)
			: this(ID, null){}

		public ComponentTypeEdit(Guid? ID, Guid? siteID)
		{
			InitializeComponent();
			curType = new EComponentType();
			curType.Load(ID);
			if (siteID != null) curType.ComponentTypeSitID = siteID;
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{

			SetControlValues();
			this.Text = newRecord ? "New Component Type" : "Edit Component Type";
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
			curType.ComponentTypeNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curType.ComponentTypeNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curType.ComponentTypeNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curType.ComponentTypeNameErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Material Type should only be Activated if it has returned to service.  Continue?" :
				"A Material Type should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Material Type Status Change", MessageBoxButtons.YesNo);
			if (r == DialogResult.No) ckActive.Checked = !ckActive.Checked;
		}

		// If the user resizes the form, keep the site label centered and in the form
		private void MaterialTypeEdit_Resize(object sender, EventArgs e)
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
			if (!curType.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curType.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curType.ComponentTypeName;
			ckActive.Checked = curType.ComponentTypeIsActive;

			if (curType.ComponentTypeSitID != null)
			{
				ESite sit = new ESite((Guid?)curType.ComponentTypeSitID);
				lblSiteName.Text = "Site: " + sit.SiteName;
			}
			else lblSiteName.Text = "Unknown Site";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curType.ComponentTypeNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curType.ComponentTypeName = txtName.Text;
			curType.ComponentTypeIsActive = ckActive.Checked;
		}

	}
}