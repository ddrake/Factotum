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
	public partial class AdditionalMeasurementEdit : Form, IEntityEditForm
	{
		private EAdditionalMeasurement curAdditionalMeasurement;
        private ComponentSection[] componentSections;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curAdditionalMeasurement; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public AdditionalMeasurementEdit(Guid? ID)
			: this(ID, null){}

		public AdditionalMeasurementEdit(Guid? ID, Guid? dsetID)
		{
			InitializeComponent();
			curAdditionalMeasurement = new EAdditionalMeasurement();
			curAdditionalMeasurement.Load(ID);
			if (dsetID != null) curAdditionalMeasurement.AdditionalMeasurementDstID = dsetID;
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
            componentSections = EAdditionalMeasurement.GetComponentSectionsArray();
            cboComponentSection.DataSource = componentSections;
            cboComponentSection.DisplayMember = "Name";
            cboComponentSection.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Additional Measurement" : "Edit Additional Measurement";
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
			curAdditionalMeasurement.AdditionalMeasurementNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curAdditionalMeasurement.AdditionalMeasurementNameErrMsg);
		}

		private void txtThickness_TextChanged(object sender, EventArgs e)
		{
			curAdditionalMeasurement.AdditionalMeasurementThicknessLengthOk(txtThickness.Text);
			errorProvider1.SetError(txtThickness, curAdditionalMeasurement.AdditionalMeasurementThicknessErrMsg);
		}

		private void txtDescription_TextChanged(object sender, EventArgs e)
		{
			curAdditionalMeasurement.AdditionalMeasurementDescriptionLengthOk(txtDescription.Text);
			errorProvider1.SetError(txtDescription, curAdditionalMeasurement.AdditionalMeasurementDescriptionErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curAdditionalMeasurement.AdditionalMeasurementNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curAdditionalMeasurement.AdditionalMeasurementNameErrMsg);
		}

		private void txtThickness_Validating(object sender, CancelEventArgs e)
		{
			curAdditionalMeasurement.AdditionalMeasurementThicknessValid(txtThickness.Text);
			errorProvider1.SetError(txtThickness, curAdditionalMeasurement.AdditionalMeasurementThicknessErrMsg);
		}

		private void txtDescription_Validating(object sender, CancelEventArgs e)
		{
			curAdditionalMeasurement.AdditionalMeasurementDescriptionValid(txtDescription.Text);
			errorProvider1.SetError(txtDescription, curAdditionalMeasurement.AdditionalMeasurementDescriptionErrMsg);
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
			if (!curAdditionalMeasurement.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curAdditionalMeasurement.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			if (curAdditionalMeasurement.AdditionalMeasurementDstID != null)
			{
				EDset dst = new EDset(curAdditionalMeasurement.AdditionalMeasurementDstID);
				lblSiteName.Text = "Additional Measurement for Dataset: '" + dst.DsetName + "'";
			}
			else lblSiteName.Text = "Additional Measurement for Unknown Dataset";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			txtName.Text = curAdditionalMeasurement.AdditionalMeasurementName;
			txtDescription.Text = curAdditionalMeasurement.AdditionalMeasurementDescription;
			txtThickness.Text = Util.GetFormattedDecimal(curAdditionalMeasurement.AdditionalMeasurementThickness);
            ckIncludeInStats.Checked = curAdditionalMeasurement.AdditionalMeasurementIncludeInStats;
            if (curAdditionalMeasurement.AdditionalMeasurementComponentSection != null)
            {
                cboComponentSection.SelectedValue = curAdditionalMeasurement.AdditionalMeasurementComponentSection;
            }
        }

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curAdditionalMeasurement.AdditionalMeasurementNameErrMsg);
			errorProvider1.SetError(txtDescription, curAdditionalMeasurement.AdditionalMeasurementDescriptionErrMsg);
			errorProvider1.SetError(txtThickness, curAdditionalMeasurement.AdditionalMeasurementThicknessErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtDescription).Length > 0 ||
				errorProvider1.GetError(txtThickness).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curAdditionalMeasurement.AdditionalMeasurementName = txtName.Text;
			curAdditionalMeasurement.AdditionalMeasurementDescription = txtDescription.Text;
			curAdditionalMeasurement.AdditionalMeasurementThickness = Util.GetNullableDecimalForString(txtThickness.Text);
            curAdditionalMeasurement.AdditionalMeasurementIncludeInStats = ckIncludeInStats.Checked;
            curAdditionalMeasurement.AdditionalMeasurementComponentSection = (short?)cboComponentSection.SelectedValue;
        }

	}
}