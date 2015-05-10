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
	public partial class InspectionPeriodEdit : Form, IEntityEditForm
	{
		private EInspectionPeriod curInspectionPeriod;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curInspectionPeriod; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public InspectionPeriodEdit(Guid? ID)
			: this(ID, null) { }

		public InspectionPeriodEdit(Guid? ID, Guid? dsetID)
		{
			InitializeComponent();
			curInspectionPeriod = new EInspectionPeriod();
			curInspectionPeriod.Load(ID);
			if (dsetID != null) curInspectionPeriod.InspectionPeriodDstID = dsetID;
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{

			SetControlValues();
			this.Text = newRecord ? "New Inspection Period" : "Edit Inspection Period";
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

		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------

		// No prompting is performed.  The user should understand the meanings of OK and Cancel.
		private void SaveAndClose()
		{
			// Set the entity values to match the form values
			UpdateRecord();
			// Try to validate
			if (!curInspectionPeriod.Valid())
			{
				if (curInspectionPeriod.InspectionPeriodErrMsg != null)
				{
					MessageBox.Show(curInspectionPeriod.InspectionPeriodErrMsg, "Factotum");
					curInspectionPeriod.InspectionPeriodErrMsg = null;
				}
				//setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curInspectionPeriod.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			if (curInspectionPeriod.InspectionPeriodDstID != null)
			{
				EDset dst = new EDset(curInspectionPeriod.InspectionPeriodDstID);
				lblSiteName.Text = "Inspection Period for Dataset: '" + dst.DsetName + "'";
			}
			else lblSiteName.Text = "Inspection Period for Unknown Dataset";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			ckCheck1.Checked = curInspectionPeriod.InspectionPeriodCalCheck1At != null;
			ckCheck2.Checked = curInspectionPeriod.InspectionPeriodCalCheck2At != null;
			dtpCalCheck1.Enabled = ckCheck1.Checked;
			dtpCalCheck2.Enabled = ckCheck2.Checked;
			DateTime now = DateTime.Now;
			now = now.AddSeconds(-(now.Second + now.Millisecond/1000.0)); // clear off the seconds;
			dtpCalIn.Value = curInspectionPeriod.InspectionPeriodInAt == null ?
				now : (DateTime)curInspectionPeriod.InspectionPeriodInAt;
			dtpCalOut.Value = curInspectionPeriod.InspectionPeriodOutAt == null ?
				now : (DateTime)curInspectionPeriod.InspectionPeriodOutAt;
			dtpCalCheck1.Value = curInspectionPeriod.InspectionPeriodCalCheck1At == null ?
				now : (DateTime)curInspectionPeriod.InspectionPeriodCalCheck1At;
			dtpCalCheck2.Value = curInspectionPeriod.InspectionPeriodCalCheck2At == null ?
				now : (DateTime)curInspectionPeriod.InspectionPeriodCalCheck2At;
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curInspectionPeriod.InspectionPeriodInAt = (DateTime)dtpCalIn.Value;
			curInspectionPeriod.InspectionPeriodOutAt = (DateTime)dtpCalOut.Value;
			if (ckCheck1.Checked)
				curInspectionPeriod.InspectionPeriodCalCheck1At = (DateTime)dtpCalCheck1.Value;
			else
				curInspectionPeriod.InspectionPeriodCalCheck1At = null;

			if (ckCheck2.Checked)
				curInspectionPeriod.InspectionPeriodCalCheck2At = (DateTime)dtpCalCheck2.Value;
			else
				curInspectionPeriod.InspectionPeriodCalCheck2At = null;

		}

		private void ckCheck1_CheckedChanged(object sender, EventArgs e)
		{
			dtpCalCheck1.Enabled = ckCheck1.Checked;
		}

		private void ckCheck2_CheckedChanged(object sender, EventArgs e)
		{
			dtpCalCheck2.Enabled = ckCheck2.Checked;
		}

	}
}