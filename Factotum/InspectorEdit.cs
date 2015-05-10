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
	public partial class InspectorEdit : Form, IEntityEditForm
	{
		private EInspector curInspector;
		private EKitCollection kits;
		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curInspector; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public InspectorEdit()
			: this(null){}

		public InspectorEdit(Guid? ID)
		{
			InitializeComponent();
			curInspector = new EInspector();
			curInspector.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			InspectorLevel[] levels = EInspector.GetInspectorLevelsArray();
			cboLevel.DataSource = levels;
			cboLevel.DisplayMember = "Name";
			cboLevel.ValueMember = "ID";

			kits = EKit.ListByName(true);
			cboKit.DataSource = kits;
			cboKit.DisplayMember = "ToolKitName";
			cboKit.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Inspector" : "Edit Inspector";
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
			curInspector.InspectorNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curInspector.InspectorNameErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curInspector.InspectorNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curInspector.InspectorNameErrMsg);
		}

		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"An Inspector should only be Activated if they have returned to service.  Continue?" :
				"An Inspector should only be Inactivated if they are no longer available to do inspections.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Inspector Status Change", MessageBoxButtons.YesNo);
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
			if (!curInspector.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curInspector.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curInspector.InspectorName;
			if (curInspector.InspectorKitID != null) cboKit.SelectedValue = curInspector.InspectorKitID;
			if (curInspector.InspectorLevel != null) cboLevel.SelectedValue = curInspector.InspectorLevel;
			ckActive.Checked = curInspector.InspectorIsActive;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curInspector.InspectorNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curInspector.InspectorName = txtName.Text;
			curInspector.InspectorKitID = (Guid?)cboKit.SelectedValue;
			curInspector.InspectorLevel = (byte?)cboLevel.SelectedValue;
			curInspector.InspectorIsActive = ckActive.Checked;
		}

	}
}