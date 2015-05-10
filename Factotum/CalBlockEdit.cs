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
	public partial class CalBlockEdit : Form, IEntityEditForm
	{
		private ECalBlock curCalBlock;
		private CalBlockType[] calBlockTypes;
		private CalBlockMaterial[] calBlockMaterials;
		private EKitCollection kits;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curCalBlock; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public CalBlockEdit()
			: this(null){}

		public CalBlockEdit(Guid? ID)
		{
			InitializeComponent();
			curCalBlock = new ECalBlock();
			curCalBlock.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			kits = EKit.ListByName(true);
			cboKit.DataSource = kits;
			cboKit.DisplayMember = "ToolKitName";
			cboKit.ValueMember = "ID";

			calBlockMaterials = ECalBlock.GetCalBlockMaterialsArray();
			cboMaterialType.DataSource = calBlockMaterials;
			cboMaterialType.DisplayMember = "Name";
			cboMaterialType.ValueMember = "ID";

			calBlockTypes = ECalBlock.GetCalBlockTypesArray();
			cboType.DataSource = calBlockTypes;
			cboType.DisplayMember = "Name";
			cboType.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Calibration Block" : "Edit Calibration Block";
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
			curCalBlock.CalBlockSerialNumberLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curCalBlock.CalBlockSerialNumberErrMsg);
		}

		private void txtMin_TextChanged(object sender, EventArgs e)
		{
			curCalBlock.CalBlockCalMinLengthOk(txtMin.Text);
			errorProvider1.SetError(txtMin, curCalBlock.CalBlockCalMinErrMsg);
		}

		private void txtMax_TextChanged(object sender, EventArgs e)
		{
			curCalBlock.CalBlockCalMaxLengthOk(txtMax.Text);
			errorProvider1.SetError(txtMax, curCalBlock.CalBlockCalMaxErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curCalBlock.CalBlockSerialNumberValid(txtName.Text);
			errorProvider1.SetError(txtName, curCalBlock.CalBlockSerialNumberErrMsg);
		}

		private void txtMin_Validating(object sender, CancelEventArgs e)
		{
			curCalBlock.CalBlockCalMinValid(txtMin.Text);
			errorProvider1.SetError(txtMin, curCalBlock.CalBlockCalMinErrMsg);
		}

		private void txtMax_Validating(object sender, CancelEventArgs e)
		{
			curCalBlock.CalBlockCalMaxValid(txtMax.Text);
			errorProvider1.SetError(txtMax, curCalBlock.CalBlockCalMaxErrMsg);
		}
		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Calibration Block should only be Activated if it has returned to service.  Continue?" :
				"A Calibration Block should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Calibration Block Status Change", MessageBoxButtons.YesNo);
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
			if (!curCalBlock.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curCalBlock.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curCalBlock.CalBlockSerialNumber;
			txtMax.Text = Util.GetFormattedDecimal(curCalBlock.CalBlockCalMax);
			txtMin.Text = Util.GetFormattedDecimal(curCalBlock.CalBlockCalMin);
			if (curCalBlock.CalBlockKitID != null) cboKit.SelectedValue = curCalBlock.CalBlockKitID;
			if (curCalBlock.CalBlockMaterialType != null) cboMaterialType.SelectedValue = curCalBlock.CalBlockMaterialType;
			if (curCalBlock.CalBlockType != null) cboType.SelectedValue = curCalBlock.CalBlockType;
			ckActive.Checked = curCalBlock.CalBlockIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curCalBlock.CalBlockSerialNumberErrMsg);
			errorProvider1.SetError(txtMax, curCalBlock.CalBlockCalMaxErrMsg);
			errorProvider1.SetError(txtMin, curCalBlock.CalBlockCalMinErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtMax).Length > 0 ||
				errorProvider1.GetError(txtMin).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curCalBlock.CalBlockSerialNumber = txtName.Text;
			curCalBlock.CalBlockCalMax = Util.GetNullableDecimalForString(txtMax.Text);
			curCalBlock.CalBlockCalMin = Util.GetNullableDecimalForString(txtMin.Text);
			curCalBlock.CalBlockSerialNumber = txtName.Text;
			curCalBlock.CalBlockKitID = (Guid?)cboKit.SelectedValue;
			curCalBlock.CalBlockMaterialType = (byte?)cboMaterialType.SelectedValue;
			curCalBlock.CalBlockType = (byte?)cboType.SelectedValue;
			curCalBlock.CalBlockIsActive = ckActive.Checked;
		}


	}
}