using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class PipeScheduleEdit : Form, IEntityEditForm
	{
		private EPipeSchedule curPipeSched;
		
		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curPipeSched; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public PipeScheduleEdit()
			: this(null){}

		public PipeScheduleEdit(Guid? ID)
		{
			InitializeComponent();
			curPipeSched = new EPipeSchedule();
			curPipeSched.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			SetControlValues();
			this.Text = newRecord ? "New Pipe Schedule Item" : "Edit Pipe Schedule Item";
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
		private void txtOD_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			curPipeSched.PipeScheduleOdLengthOk(txtOD.Text);
			errorProvider1.SetError(txtOD, curPipeSched.PipeScheduleOdErrMsg);
		}

		private void txtNomWall_TextChanged(object sender, EventArgs e)
		{
			curPipeSched.PipeScheduleNomWallLengthOk(txtNomWall.Text);
			errorProvider1.SetError(txtNomWall, curPipeSched.PipeScheduleNomWallErrMsg);

		}

		private void txtNomDia_TextChanged(object sender, EventArgs e)
		{
			curPipeSched.PipeScheduleNomDiaLengthOk(txtNomDia.Text);
			errorProvider1.SetError(txtNomDia, curPipeSched.PipeScheduleNomDiaErrMsg);
		}

		private void txtPipeSchedule_TextChanged(object sender, EventArgs e)
		{
			curPipeSched.PipeScheduleScheduleLengthOk(txtPipeSchedule.Text);
			errorProvider1.SetError(txtPipeSchedule, curPipeSched.PipeScheduleScheduleErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtOD_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curPipeSched.PipeScheduleOdValid(txtOD.Text);
			errorProvider1.SetError(txtOD, curPipeSched.PipeScheduleOdErrMsg);
		}
		private void txtPipeSchedule_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curPipeSched.PipeScheduleScheduleValid(txtPipeSchedule.Text);
			errorProvider1.SetError(txtPipeSchedule, curPipeSched.PipeScheduleScheduleErrMsg);
		}

		private void txtNomDia_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curPipeSched.PipeScheduleNomDiaValid(txtNomDia.Text);
			errorProvider1.SetError(txtNomDia, curPipeSched.PipeScheduleNomDiaErrMsg);
		}

		private void txtNomWall_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curPipeSched.PipeScheduleNomWallValid(txtNomWall.Text);
			errorProvider1.SetError(txtNomWall, curPipeSched.PipeScheduleNomWallErrMsg);
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
			if (!curPipeSched.Valid())
			{
				setAllErrors();
				return;
			}

			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curPipeSched.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the line object values.
		private void SetControlValues()
		{
			txtOD.Text = curPipeSched.PipeScheduleOd == null ? null :
				string.Format("{0:0.000}",curPipeSched.PipeScheduleOd);

			txtNomWall.Text = curPipeSched.PipeScheduleNomWall == null ? null :
				string.Format("{0:0.000}", curPipeSched.PipeScheduleNomWall);

			txtPipeSchedule.Text = curPipeSched.PipeScheduleSchedule;

			txtNomDia.Text = curPipeSched.PipeScheduleNomDia == null ? null :
				string.Format("{0:0.000}", curPipeSched.PipeScheduleNomDia);

		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtOD, curPipeSched.PipeScheduleOdErrMsg);
			errorProvider1.SetError(txtNomWall, curPipeSched.PipeScheduleNomWallErrMsg);
			errorProvider1.SetError(txtPipeSchedule, curPipeSched.PipeScheduleScheduleErrMsg);
			errorProvider1.SetError(txtNomDia, curPipeSched.PipeScheduleNomDiaErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtOD).Length > 0 ||
				errorProvider1.GetError(txtNomWall).Length > 0 ||
				errorProvider1.GetError(txtPipeSchedule).Length > 0 ||
				errorProvider1.GetError(txtNomDia).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curPipeSched.PipeScheduleOd = DowUtils.Util.IsNullOrEmpty(txtOD.Text) ? 
				(decimal?)null :
				decimal.Parse(txtOD.Text);
			curPipeSched.PipeScheduleNomWall = DowUtils.Util.IsNullOrEmpty(txtNomWall.Text) ?
				(decimal?)null :
				decimal.Parse(txtNomWall.Text);
			curPipeSched.PipeScheduleNomDia = DowUtils.Util.IsNullOrEmpty(txtNomDia.Text) ?
				(decimal?)null :
				decimal.Parse(txtNomDia.Text);
			curPipeSched.PipeScheduleSchedule = txtPipeSchedule.Text;
		}

	}
}