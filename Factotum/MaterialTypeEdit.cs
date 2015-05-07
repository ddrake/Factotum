using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class MaterialTypeEdit : Form, IEntityEditForm
	{
		private EComponentMaterial curMaterial;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curMaterial; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public MaterialTypeEdit(Guid? ID)
			: this(ID, null){}

		public MaterialTypeEdit(Guid? ID, Guid? siteID)
		{
			InitializeComponent();
			curMaterial = new EComponentMaterial();
			curMaterial.Load(ID);
			if (siteID != null) curMaterial.CmpMaterialSitID = siteID;
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			// Calibration Block Materials combo box
			CalBlockMaterial[] calBlockMaterials = ECalBlock.GetCalBlockMaterialsArray();
			cboCalBlockMatlType.DataSource = calBlockMaterials;
			cboCalBlockMatlType.DisplayMember = "Name";
			cboCalBlockMatlType.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Component Material Type" : "Edit Component Material Type";
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
			curMaterial.CmpMaterialNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curMaterial.CmpMaterialNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curMaterial.CmpMaterialNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curMaterial.CmpMaterialNameErrMsg);
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
			if (!curMaterial.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curMaterial.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curMaterial.CmpMaterialName;
			ckActive.Checked = curMaterial.CmpMaterialIsActive;
			cboCalBlockMatlType.SelectedValue = curMaterial.CmpMaterialCalBlockMaterial;

			if (curMaterial.CmpMaterialSitID != null)
			{
				ESite sit = new ESite(curMaterial.CmpMaterialSitID);
				lblSiteName.Text = "Site: " + sit.SiteName;
			}
			else lblSiteName.Text = "Unknown Site";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curMaterial.CmpMaterialNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curMaterial.CmpMaterialName = txtName.Text;
			curMaterial.CmpMaterialCalBlockMaterial = Convert.ToByte(cboCalBlockMatlType.SelectedValue);
			curMaterial.CmpMaterialIsActive = ckActive.Checked;
		}

		private void MaterialTypeEdit_Resize(object sender, EventArgs e)
		{
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

	}
}