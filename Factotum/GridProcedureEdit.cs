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
	public partial class GridProcedureEdit : Form, IEntityEditForm
	{
		private EGridProcedure curGridProcedure;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curGridProcedure; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public GridProcedureEdit()
			: this(null){}

		public GridProcedureEdit(Guid? ID)
		{
			InitializeComponent();
			curGridProcedure = new EGridProcedure();
			curGridProcedure.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New Grid Procedure" : "Edit Grid Procedure";
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
			curGridProcedure.GridProcedureNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curGridProcedure.GridProcedureNameErrMsg);
		}

		private void txtDescription_TextChanged(object sender, EventArgs e)
		{
			curGridProcedure.GridProcedureDescriptionLengthOk(txtDescription.Text);
			errorProvider1.SetError(txtDescription, curGridProcedure.GridProcedureDescriptionErrMsg);
		}

		private void txtDsDiameters_TextChanged(object sender, EventArgs e)
		{
			curGridProcedure.GridProcedureDsDiametersLengthOk(txtDsDiameters.Text);
			errorProvider1.SetError(txtDsDiameters, curGridProcedure.GridProcedureDsDiametersErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curGridProcedure.GridProcedureNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curGridProcedure.GridProcedureNameErrMsg);
		}

		private void txtDescription_Validating(object sender, CancelEventArgs e)
		{
			curGridProcedure.GridProcedureDescriptionValid(txtDescription.Text);
			errorProvider1.SetError(txtDescription, curGridProcedure.GridProcedureDescriptionErrMsg);
		}

		private void txtDsDiameters_Validating(object sender, CancelEventArgs e)
		{
			curGridProcedure.GridProcedureDsDiametersValid(txtDsDiameters.Text);
			errorProvider1.SetError(txtDsDiameters, curGridProcedure.GridProcedureDsDiametersErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Grid Procedure should only be Activated if it is no longer discontinued.  Continue?" :
				"A Grid Procedure should only be Inactivated if it is discontinued.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Grid Procedure Status Change", MessageBoxButtons.YesNo);
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
			if (!curGridProcedure.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curGridProcedure.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curGridProcedure.GridProcedureName;
			txtDsDiameters.Text = curGridProcedure.GridProcedureDsDiameters.ToString();
			txtDescription.Text = curGridProcedure.GridProcedureDescription;
			ckActive.Checked = curGridProcedure.GridProcedureIsActive;

		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curGridProcedure.GridProcedureNameErrMsg);
			errorProvider1.SetError(txtDescription, curGridProcedure.GridProcedureDescriptionErrMsg);
			errorProvider1.SetError(txtDsDiameters, curGridProcedure.GridProcedureDsDiametersErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtDescription).Length > 0 ||
				errorProvider1.GetError(txtDsDiameters).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curGridProcedure.GridProcedureName = txtName.Text;
			curGridProcedure.GridProcedureDsDiameters = Util.GetNullableShortForString(txtDsDiameters.Text);
			curGridProcedure.GridProcedureDescription = txtDescription.Text;
			curGridProcedure.GridProcedureIsActive = ckActive.Checked;
		}

	}
}