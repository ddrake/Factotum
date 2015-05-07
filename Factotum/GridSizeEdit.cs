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
	public partial class GridSizeEdit : Form, IEntityEditForm
	{
		private EGridSize curGridSize;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curGridSize; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public GridSizeEdit()
			: this(null){}

		public GridSizeEdit(Guid? ID)
		{
			InitializeComponent();
			curGridSize = new EGridSize();
			curGridSize.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New Grid Size" : "Edit Grid Size";
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
			curGridSize.GridSizeNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curGridSize.GridSizeNameErrMsg);
		}

		private void txtAxialDistance_TextChanged(object sender, EventArgs e)
		{
			curGridSize.GridSizeAxialDistanceLengthOk(txtAxialDistance.Text);
			errorProvider1.SetError(txtAxialDistance, curGridSize.GridSizeAxialDistanceErrMsg);
		}

		private void txtRadialDistance_TextChanged(object sender, EventArgs e)
		{
			curGridSize.GridSizeRadialDistanceLengthOk(txtRadialDistance.Text);
			errorProvider1.SetError(txtRadialDistance, curGridSize.GridSizeRadialDistanceErrMsg);
		}

		private void txtMaxOD_TextChanged(object sender, EventArgs e)
		{
			curGridSize.GridSizeMaxDiameterLengthOk(txtMaxOD.Text);
			errorProvider1.SetError(txtMaxOD, curGridSize.GridSizeMaxDiameterErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curGridSize.GridSizeNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curGridSize.GridSizeNameErrMsg);
		}

		private void txtAxialDistance_Validating(object sender, CancelEventArgs e)
		{
			curGridSize.GridSizeAxialDistanceValid(txtAxialDistance.Text);
			errorProvider1.SetError(txtAxialDistance, curGridSize.GridSizeAxialDistanceErrMsg);
		}

		private void txtRadialDistance_Validating(object sender, CancelEventArgs e)
		{
			curGridSize.GridSizeRadialDistanceValid(txtRadialDistance.Text);
			errorProvider1.SetError(txtRadialDistance, curGridSize.GridSizeRadialDistanceErrMsg);
		}

		private void txtMaxOD_Validating(object sender, CancelEventArgs e)
		{
			curGridSize.GridSizeMaxDiameterValid(txtMaxOD.Text);
			errorProvider1.SetError(txtMaxOD, curGridSize.GridSizeMaxDiameterErrMsg);
		}


		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Grid Size should only be Activated if it is no longer discontinued.  Continue?" :
				"A Grid Size should only be Inactivated if it is discontinued.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Grid Size Status Change", MessageBoxButtons.YesNo);
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
			if (!curGridSize.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curGridSize.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curGridSize.GridSizeName;
			txtAxialDistance.Text = Util.GetFormattedDecimal(curGridSize.GridSizeAxialDistance,3);
			txtRadialDistance.Text = Util.GetFormattedDecimal(curGridSize.GridSizeRadialDistance,3);
			txtMaxOD.Text = Util.GetFormattedDecimal(curGridSize.GridSizeMaxDiameter,3);
			ckActive.Checked = curGridSize.GridSizeIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curGridSize.GridSizeNameErrMsg);
			errorProvider1.SetError(txtAxialDistance, curGridSize.GridSizeAxialDistanceErrMsg);
			errorProvider1.SetError(txtRadialDistance, curGridSize.GridSizeRadialDistanceErrMsg);
			errorProvider1.SetError(txtMaxOD, curGridSize.GridSizeMaxDiameterErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtAxialDistance).Length > 0 ||
				errorProvider1.GetError(txtRadialDistance).Length > 0 ||
				errorProvider1.GetError(txtMaxOD).Length > 0
				);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curGridSize.GridSizeName = txtName.Text;
			curGridSize.GridSizeAxialDistance = Util.GetNullableDecimalForString(txtAxialDistance.Text);
			curGridSize.GridSizeRadialDistance = Util.GetNullableDecimalForString(txtRadialDistance.Text);
			curGridSize.GridSizeMaxDiameter = Util.GetNullableDecimalForString(txtMaxOD.Text);
			curGridSize.GridSizeIsActive = ckActive.Checked;
		}
	}
}