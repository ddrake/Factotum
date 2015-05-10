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
	public partial class ComponentEdit : Form, IEntityEditForm
	{
		private EComponent curComponent;
		private EUnit curUnit;
		private ESite curSite;
		private EPipeSchedule curPipeSchedule;

		private ESystemCollection systems;
		private ELineCollection lines;
		private EComponentTypeCollection componentTypes;
		private EComponentMaterialCollection componentMaterials;

		private bool newRecord;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curComponent; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public ComponentEdit(Guid? ID)
			: this(ID, null){}

		public ComponentEdit(Guid? ID, Guid? unitID)
		{
			InitializeComponent();
			curComponent = new EComponent();
			curComponent.Load(ID);
			if (unitID != null) curComponent.ComponentUntID = unitID;
			// Note: We're assuming that either you are creating a new component and 
			// specifying the unitID OR editing an existing component.  Either way there 
			// needs to be a unit ID for the component at this point.
			curUnit = new EUnit(curComponent.ComponentUntID);
			curSite = new ESite(curUnit.UnitSitID);
			newRecord = (ID == null);
			InitializeControls();
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			// Systems combo box
			systems = ESystem.ListForUnit(curComponent.ComponentUntID, !newRecord, true);
			cboSystem.DataSource = systems;
			cboSystem.DisplayMember = "SystemName";
			cboSystem.ValueMember = "ID";

			// Lines combo box
			lines = ELine.ListForUnit(curComponent.ComponentUntID, !newRecord, true);
			cboLine.DataSource = lines;
			cboLine.DisplayMember = "LineName";
			cboLine.ValueMember = "ID";

			// Component Types combo box
			componentTypes = EComponentType.ListForSite(curSite.ID, !newRecord, true);
			cboType.DataSource = componentTypes;
			cboType.DisplayMember = "ComponentTypeName";
			cboType.ValueMember = "ID";

			// Component Materials combo box
			componentMaterials = EComponentMaterial.ListForSite(curSite.ID, !newRecord, true);
			cboMaterial.DataSource = componentMaterials;
			cboMaterial.DisplayMember = "CmpMaterialName";
			cboMaterial.ValueMember = "ID";

			SetControlValues();
			this.Text = newRecord ? "New Component" : "Edit Component";
			ESystem.Changed += new EventHandler<EntityChangedEventArgs>(ESystem_Changed);
			ELine.Changed += new EventHandler<EntityChangedEventArgs>(ELine_Changed);
			EComponentType.Changed += new EventHandler<EntityChangedEventArgs>(EComponentType_Changed);
			EComponentMaterial.Changed += new EventHandler<EntityChangedEventArgs>(EComponentMaterial_Changed);
		}

		void EComponentMaterial_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboMaterial.SelectedValue;
			componentMaterials = componentMaterials = EComponentMaterial.ListForSite(curSite.ID, !newRecord, true);
			cboMaterial.DataSource = componentMaterials;
			if (currentValue == null)
				cboMaterial.SelectedIndex = 0;
			else
				cboMaterial.SelectedValue = currentValue;
		}

		void EComponentType_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboType.SelectedValue;
			componentTypes = EComponentType.ListForSite(curSite.ID, !newRecord, true);
			cboType.DataSource = componentTypes;
			if (currentValue == null)
				cboType.SelectedIndex = 0;
			else
				cboType.SelectedValue = currentValue;
		}

		void ELine_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboLine.SelectedValue;
			lines = ELine.ListForUnit(curComponent.ComponentUntID, !newRecord, true);
			cboLine.DataSource = lines;
			if (currentValue == null)
				cboLine.SelectedIndex = 0;
			else
				cboLine.SelectedValue = currentValue;
		}

		void ESystem_Changed(object sender, EntityChangedEventArgs e)
		{
			Guid? currentValue = (Guid?)cboSystem.SelectedValue;
			systems = ESystem.ListForUnit(curComponent.ComponentUntID, !newRecord, true);
			cboSystem.DataSource = systems;
			if (currentValue == null)
				cboSystem.SelectedIndex = 0;
			else
				cboSystem.SelectedValue = currentValue;
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
			curComponent.ComponentNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curComponent.ComponentNameErrMsg);
		}
		private void txtPctChromeMain_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentPctChromeMainLengthOk(txtPctChromeMain.Text);
			errorProvider1.SetError(txtPctChromeMain, curComponent.ComponentPctChromeMainErrMsg);
		}

		private void txtPctChromeBranch_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentPctChromeBranchLengthOk(txtPctChromeBranch.Text);
			errorProvider1.SetError(txtPctChromeBranch, curComponent.ComponentPctChromeBranchErrMsg);
		}

		private void txtUpMainOD_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentUpMainOdLengthOk(txtUpMainOD.Text);
			errorProvider1.SetError(txtUpMainOD, curComponent.ComponentUpMainOdErrMsg);
			txtUsExtOD.Text = txtUpMainOD.Text;
		}
		private void txtUpMainTnom_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentUpMainTnomLengthOk(txtUpMainTnom.Text);
			errorProvider1.SetError(txtUpMainTnom, curComponent.ComponentUpMainTnomErrMsg);
			txtUsExtTnom.Text = txtUpMainTnom.Text;
		}

		private void txtUpMainTscreen_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentUpMainTscrLengthOk(txtUpMainTscreen.Text);
			errorProvider1.SetError(txtUpMainTscreen, curComponent.ComponentUpMainTscrErrMsg);
			txtUsExtTscr.Text = txtUpMainTscreen.Text;
		}

		private void txtDnMainOD_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDnMainOdLengthOk(txtDnMainOD.Text);
			errorProvider1.SetError(txtDnMainOD, curComponent.ComponentDnMainOdErrMsg);
			txtDsExtOD.Text = txtDnMainOD.Text;
		}

		private void txtDnMainTnom_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDnMainTnomLengthOk(txtDnMainTnom.Text);
			errorProvider1.SetError(txtDnMainTnom, curComponent.ComponentDnMainTnomErrMsg);
			txtDsExtTnom.Text = txtDnMainTnom.Text;
		}

		private void txtDnMainTscr_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDnMainTscrLengthOk(txtDnMainTscr.Text);
			errorProvider1.SetError(txtDnMainTscr, curComponent.ComponentDnMainTscrErrMsg);
			txtDsExtTscr.Text = txtDnMainTscr.Text;
		}

		private void txtBranchOD_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentBranchOdLengthOk(txtBranchOD.Text);
			errorProvider1.SetError(txtBranchOD, curComponent.ComponentBranchOdErrMsg);
			txtBranchExtOD.Text = txtBranchOD.Text;
		}

		private void txtBranchTnom_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentBranchTnomLengthOk(txtBranchTnom.Text);
			errorProvider1.SetError(txtBranchTnom, curComponent.ComponentBranchTnomErrMsg);
			txtBranchExtTnom.Text = txtBranchTnom.Text;
		}

		private void txtBranchTscr_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentBranchTscrLengthOk(txtBranchTscr.Text);
			errorProvider1.SetError(txtBranchTscr, curComponent.ComponentBranchTscrErrMsg);
			txtBranchExtTscr.Text = txtBranchTscr.Text;
		}
		private void txtDrawingID_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDrawingLengthOk(txtDrawingID.Text);
			errorProvider1.SetError(txtDrawingID, curComponent.ComponentDrawingErrMsg);
		}

		private void txtMisc1_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentMisc1LengthOk(txtMisc1.Text);
			errorProvider1.SetError(txtMisc1, curComponent.ComponentMisc1ErrMsg);
		}

		private void txtMisc2_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentMisc2LengthOk(txtMisc2.Text);
			errorProvider1.SetError(txtMisc2, curComponent.ComponentMisc2ErrMsg);
		}

		private void txtPctChromeUsExt_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentPctChromeUsExtLengthOk(txtPctChromeUsExt.Text);
			errorProvider1.SetError(txtPctChromeUsExt, curComponent.ComponentPctChromeUsExtErrMsg);
		}

		private void txtPctChromeDsExt_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentPctChromeDsExtLengthOk(txtPctChromeDsExt.Text);
			errorProvider1.SetError(txtPctChromeDsExt, curComponent.ComponentPctChromeDsExtErrMsg);
		}

		private void txtPctChromeBrExt_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentPctChromeBrExtLengthOk(txtPctChromeBrExt.Text);
			errorProvider1.SetError(txtPctChromeBrExt, curComponent.ComponentPctChromeBrExtErrMsg);
		}

		private void txtNotes_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentNoteLengthOk(txtNotes.Text);
			errorProvider1.SetError(txtNotes, curComponent.ComponentNoteErrMsg);
		}

		private void txtUsExtOD_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentUpExtOdLengthOk(txtUsExtOD.Text);
			errorProvider1.SetError(txtUsExtOD, curComponent.ComponentUpExtOdErrMsg);
		}

		private void txtUsExtTnom_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentUpExtTnomLengthOk(txtUsExtTnom.Text);
			errorProvider1.SetError(txtUsExtTnom, curComponent.ComponentUpExtTnomErrMsg);
		}

		private void txtUsExtTscr_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentUpExtTscrLengthOk(txtUsExtTscr.Text);
			errorProvider1.SetError(txtUsExtTscr, curComponent.ComponentUpExtTscrErrMsg);
		}

		private void txtDsExtOD_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDnExtOdLengthOk(txtDsExtOD.Text);
			errorProvider1.SetError(txtDsExtOD, curComponent.ComponentDnExtOdErrMsg);
		}

		private void txtDsExtTnom_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDnExtTnomLengthOk(txtDsExtTnom.Text);
			errorProvider1.SetError(txtDsExtTnom, curComponent.ComponentDnExtTnomErrMsg);
		}

		private void txtDsExtTscr_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentDnExtTscrLengthOk(txtDsExtTscr.Text);
			errorProvider1.SetError(txtDsExtTscr, curComponent.ComponentDnExtTscrErrMsg);
		}

		private void txtBranchExtOD_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentBrExtOdLengthOk(txtBranchExtOD.Text);
			errorProvider1.SetError(txtBranchExtOD, curComponent.ComponentBrExtOdErrMsg);
		}

		private void txtBranchExtTnom_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentBrExtTnomLengthOk(txtBranchExtTnom.Text);
			errorProvider1.SetError(txtBranchExtTnom, curComponent.ComponentBrExtTnomErrMsg);
		}

		private void txtBranchExtTscr_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentBrExtTscrLengthOk(txtBranchExtTscr.Text);
			errorProvider1.SetError(txtBranchExtTscr, curComponent.ComponentBrExtTscrErrMsg);
		}
		private void txtTimesInspected_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentTimesInspectedLengthOk(txtTimesInspected.Text);
			errorProvider1.SetError(txtTimesInspected, curComponent.ComponentTimesInspectedErrMsg);
		}
		private void txtAvgTimeToInspect_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentAvgInspectionTimeLengthOk(txtAvgTimeToInspect.Text);
			errorProvider1.SetError(txtAvgTimeToInspect, curComponent.ComponentAvgInspectionTimeErrMsg);
		}
		private void txtAvgCrewDose_TextChanged(object sender, EventArgs e)
		{
			curComponent.ComponentAvgCrewDoseLengthOk(txtAvgCrewDose.Text);
			errorProvider1.SetError(txtAvgCrewDose, curComponent.ComponentAvgCrewDoseErrMsg);
		}

		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curComponent.ComponentNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curComponent.ComponentNameErrMsg);
		}
		private void txtPctChromeMain_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentPctChromeMainValid(txtPctChromeMain.Text);
			errorProvider1.SetError(txtPctChromeMain, curComponent.ComponentPctChromeMainErrMsg);
		}

		private void txtPctChromeBranch_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentPctChromeBranchValid(txtPctChromeBranch.Text);
			errorProvider1.SetError(txtPctChromeBranch, curComponent.ComponentPctChromeBranchErrMsg);
		}
		private void txtUpMainTnom_Validating(object sender, CancelEventArgs e)
		{
			bool valid;
			valid = curComponent.ComponentUpMainTnomValid(txtUpMainTnom.Text);
			errorProvider1.SetError(txtUpMainTnom, curComponent.ComponentUpMainTnomErrMsg);
			GetCurrentPipeSchedule();
		}

		private void txtUpMainOD_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentUpMainOdValid(txtUpMainOD.Text);
			errorProvider1.SetError(txtUpMainOD, curComponent.ComponentUpMainOdErrMsg);
			GetCurrentPipeSchedule();
		}

		private void txtUpMainTscreen_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentUpMainTscrValid(txtUpMainTscreen.Text);
			errorProvider1.SetError(txtUpMainTscreen, curComponent.ComponentUpMainTscrErrMsg);
		}

		private void txtDnMainOD_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentDnMainOdValid(txtDnMainOD.Text);
			errorProvider1.SetError(txtDnMainOD, curComponent.ComponentDnMainOdErrMsg);
		}

		private void txtDnMainTnom_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentDnMainTnomValid(txtDnMainTnom.Text);
			errorProvider1.SetError(txtDnMainTnom, curComponent.ComponentDnMainTnomErrMsg);
		}

		private void txtDnMainTscr_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentDnMainTscrValid(txtDnMainTscr.Text);
			errorProvider1.SetError(txtDnMainTscr, curComponent.ComponentDnMainTscrErrMsg);
		}

		private void txtBranchOD_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentBranchOdValid(txtBranchOD.Text);
			errorProvider1.SetError(txtBranchOD, curComponent.ComponentBranchOdErrMsg);
		}

		private void txtBranchTnom_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentBranchTnomValid(txtBranchTnom.Text);
			errorProvider1.SetError(txtBranchTnom, curComponent.ComponentBranchTnomErrMsg);
		}

		private void txtBranchTscr_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentBranchTscrValid(txtBranchTscr.Text);
			errorProvider1.SetError(txtBranchTscr, curComponent.ComponentBranchTscrErrMsg);
		}

		private void txtMisc1_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentMisc1Valid(txtMisc1.Text);
			errorProvider1.SetError(txtMisc1, curComponent.ComponentMisc1ErrMsg);
		}

		private void txtMisc2_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentMisc1Valid(txtMisc2.Text);
			errorProvider1.SetError(txtMisc2, curComponent.ComponentMisc2ErrMsg);
		}
		private void txtDrawingID_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentMisc2Valid(txtDrawingID.Text);
			errorProvider1.SetError(txtDrawingID, curComponent.ComponentDrawingErrMsg);
		}
		
		private void txtPctChromeUsExt_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentPctChromeUsExtValid(txtPctChromeUsExt.Text);
			errorProvider1.SetError(txtPctChromeUsExt, curComponent.ComponentPctChromeUsExtErrMsg);
		}

		private void txtPctChromeDsExt_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentPctChromeDsExtValid(txtPctChromeDsExt.Text);
			errorProvider1.SetError(txtPctChromeDsExt, curComponent.ComponentPctChromeDsExtErrMsg);
		}

		private void txtPctChromeBrExt_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentPctChromeBrExtValid(txtPctChromeBrExt.Text);
			errorProvider1.SetError(txtPctChromeBrExt, curComponent.ComponentPctChromeBrExtErrMsg);
		}

		private void txtNotes_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentNoteValid(txtNotes.Text);
			errorProvider1.SetError(txtNotes, curComponent.ComponentNoteErrMsg);
		}

		private void txtUsExtOD_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentUpExtOdValid(txtUsExtOD.Text);
			errorProvider1.SetError(txtUsExtOD, curComponent.ComponentUpExtOdErrMsg);
		}

		private void txtUsExtTnom_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentUpExtTnomValid(txtUsExtTnom.Text);
			errorProvider1.SetError(txtUsExtTnom, curComponent.ComponentUpExtTnomErrMsg);
		}

		private void txtUsExtTscr_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentUpExtTscrValid(txtUsExtTscr.Text);
			errorProvider1.SetError(txtUsExtTscr, curComponent.ComponentUpExtTscrErrMsg);
		}

		private void txtDsExtOD_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentDnExtOdValid(txtDsExtOD.Text);
			errorProvider1.SetError(txtDsExtOD, curComponent.ComponentDnExtOdErrMsg);
		}

		private void txtDsExtTnom_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentDnExtTnomValid(txtDsExtTnom.Text);
			errorProvider1.SetError(txtDsExtTnom, curComponent.ComponentDnExtTnomErrMsg);
		}

		private void txtDsExtTscr_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentDnExtTscrValid(txtDsExtTscr.Text);
			errorProvider1.SetError(txtDsExtTscr, curComponent.ComponentDnExtTscrErrMsg);
		}

		private void txtBranchExtOD_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentBrExtOdValid(txtBranchExtOD.Text);
			errorProvider1.SetError(txtBranchExtOD, curComponent.ComponentBrExtOdErrMsg);
		}

		private void txtBranchExtTnom_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentBrExtTnomValid(txtBranchExtTnom.Text);
			errorProvider1.SetError(txtBranchExtTnom, curComponent.ComponentBrExtTnomErrMsg);
		}
		private void txtBranchExtTscr_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentBrExtTscrValid(txtBranchExtTscr.Text);
			errorProvider1.SetError(txtBranchExtTscr, curComponent.ComponentBrExtTscrErrMsg);
		}
		private void txtAvgCrewDose_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentAvgCrewDoseValid(txtAvgCrewDose.Text);
			errorProvider1.SetError(txtAvgCrewDose, curComponent.ComponentAvgCrewDoseErrMsg);
		}
		private void txtAvgTimeToInspect_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentAvgInspectionTimeValid(txtAvgTimeToInspect.Text);
			errorProvider1.SetError(txtAvgTimeToInspect, curComponent.ComponentAvgInspectionTimeErrMsg);
		}
		private void txtTimesInspected_Validating(object sender, CancelEventArgs e)
		{
			curComponent.ComponentTimesInspectedValid(txtTimesInspected.Text);
			errorProvider1.SetError(txtTimesInspected, curComponent.ComponentTimesInspectedErrMsg);
		}


		// Handle the user clicking the "Is Active" checkbox
		private void ckActive_Click(object sender, EventArgs e)
		{
			string msg = ckActive.Checked ?
				"A Component should only be Activated if it has returned to service.  Continue?" :
				"A Component Type should only be Inactivated if it is out of service.  Continue?";
			DialogResult r = MessageBox.Show(msg, "Factotum: Confirm Component Status Change", MessageBoxButtons.YesNo);
			if (r == DialogResult.No) ckActive.Checked = !ckActive.Checked;
		}

		private void ComponentEdit_Resize(object sender, EventArgs e)
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
			if (!curComponent.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curComponent.Save();
			if (tmpID != null)
			{
				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtAvgTimeToInspect.Text = string.Format("{0:0.00}", curComponent.ComponentAvgInspectionTime);
			txtAvgCrewDose.Text = string.Format("{0:0.00}", curComponent.ComponentAvgCrewDose);
			txtTimesInspected.Text = string.Format("{0:0}", curComponent.ComponentTimesInspected);
			txtName.Text = curComponent.ComponentName;
			if (curComponent.ComponentSysID != null) cboSystem.SelectedValue = curComponent.ComponentSysID;
			if (curComponent.ComponentLinID != null) cboLine.SelectedValue = curComponent.ComponentLinID;
			if (curComponent.ComponentCtpID != null) cboType.SelectedValue = curComponent.ComponentCtpID;
			if (curComponent.ComponentCmtID != null) cboMaterial.SelectedValue = curComponent.ComponentCmtID;
			txtPctChromeMain.Text = Util.GetFormattedDecimal(curComponent.ComponentPctChromeMain);
			txtPctChromeBranch.Text = Util.GetFormattedDecimal(curComponent.ComponentPctChromeBranch);
			txtUpMainOD.Text = Util.GetFormattedDecimal(curComponent.ComponentUpMainOd);
			txtUpMainTnom.Text = Util.GetFormattedDecimal(curComponent.ComponentUpMainTnom);
			txtUpMainTscreen.Text = Util.GetFormattedDecimal(curComponent.ComponentUpMainTscr);
			txtDnMainOD.Text = Util.GetFormattedDecimal(curComponent.ComponentDnMainOd);
			txtDnMainTnom.Text = Util.GetFormattedDecimal(curComponent.ComponentDnMainTnom);
			txtDnMainTscr.Text = Util.GetFormattedDecimal(curComponent.ComponentDnMainTscr);
			txtBranchOD.Text = Util.GetFormattedDecimal(curComponent.ComponentBranchOd);
			txtBranchTnom.Text = Util.GetFormattedDecimal(curComponent.ComponentBranchTnom);
			txtBranchTscr.Text = Util.GetFormattedDecimal(curComponent.ComponentBranchTscr);
			txtDrawingID.Text = curComponent.ComponentDrawing;
			txtMisc1.Text = curComponent.ComponentMisc1;
			txtMisc2.Text = curComponent.ComponentMisc2;
			txtNotes.Text = curComponent.ComponentNote;
			txtPctChromeBrExt.Text = Util.GetFormattedDecimal(curComponent.ComponentPctChromeBrExt);
			txtPctChromeDsExt.Text = Util.GetFormattedDecimal(curComponent.ComponentPctChromeDsExt);
			txtPctChromeUsExt.Text = Util.GetFormattedDecimal(curComponent.ComponentPctChromeUsExt);
			txtUsExtOD.Text = Util.GetFormattedDecimal(curComponent.ComponentUpExtOd);
			txtUsExtTnom.Text = Util.GetFormattedDecimal(curComponent.ComponentUpExtTnom);
			txtUsExtTscr.Text = Util.GetFormattedDecimal(curComponent.ComponentUpExtTscr);
			txtDsExtOD.Text = Util.GetFormattedDecimal(curComponent.ComponentDnExtOd);
			txtDsExtTnom.Text = Util.GetFormattedDecimal(curComponent.ComponentDnExtTnom);
			txtDsExtTscr.Text = Util.GetFormattedDecimal(curComponent.ComponentDnExtTscr);
			txtBranchExtOD.Text = Util.GetFormattedDecimal(curComponent.ComponentBrExtOd);
			txtBranchExtTnom.Text = Util.GetFormattedDecimal(curComponent.ComponentBrExtTnom);
			txtBranchExtTscr.Text = Util.GetFormattedDecimal(curComponent.ComponentBrExtTscr);
			
			// Try to match a pipe schedule from the main OD and Tnom
			// This must be done after those textboxes have their values set.
			GetCurrentPipeSchedule();
			ckActive.Checked = curComponent.ComponentIsActive;
			ckHardAccess.Checked = curComponent.ComponentHardToAccess;
			ckHighRad.Checked = curComponent.ComponentHighRad;
			ckHasDownstream.Checked = curComponent.ComponentHasDs;
			ckHasBranch.Checked = curComponent.ComponentHasBranch;

			if (curComponent.ComponentUntID != null)
			{
				lblSiteName.Text = "Facilty: " + curSite.SiteName + " " + curUnit.UnitName;
			}
			else lblSiteName.Text = "Unknown Facility";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
		}

		private void GetCurrentPipeSchedule()
		{
			if (Util.IsNullOrEmpty(txtUpMainOD.Text) ||
				Util.IsNullOrEmpty(txtUpMainTnom.Text) ||
				!Util.IsNullOrEmpty(errorProvider1.GetError(txtUpMainOD)) ||
				!Util.IsNullOrEmpty(errorProvider1.GetError(txtUpMainTnom)))
			{
				// One or both of the required fields are blank or has errors, 
				// so leave the pipe schedule blank
				txtPipeSchedule.Text = null;
				curPipeSchedule = null;
			}
			else
			{
				curPipeSchedule = EPipeSchedule.FindForOdAndTnom(txtUpMainOD.Text, txtUpMainTnom.Text);
				txtPipeSchedule.Text = curPipeSchedule == null ? "No Match Found" :
					curPipeSchedule.PipeScheduleAndNomDiaText;
			}
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtBranchOD, curComponent.ComponentBranchOdErrMsg);
			errorProvider1.SetError(txtBranchTnom, curComponent.ComponentBranchTnomErrMsg);
			errorProvider1.SetError(txtBranchTscr, curComponent.ComponentBranchTscrErrMsg);
			errorProvider1.SetError(txtDnMainOD, curComponent.ComponentDnMainOdErrMsg);
			errorProvider1.SetError(txtDnMainTnom, curComponent.ComponentDnMainTnomErrMsg);
			errorProvider1.SetError(txtDnMainTscr, curComponent.ComponentDnMainTscrErrMsg);
			errorProvider1.SetError(txtName, curComponent.ComponentNameErrMsg);
			errorProvider1.SetError(txtPctChromeMain, curComponent.ComponentPctChromeMainErrMsg);
			errorProvider1.SetError(txtPctChromeBranch, curComponent.ComponentPctChromeBranchErrMsg);
			errorProvider1.SetError(txtUpMainOD, curComponent.ComponentUpMainOdErrMsg);
			errorProvider1.SetError(txtUpMainTnom, curComponent.ComponentUpMainTnomErrMsg);
			errorProvider1.SetError(txtUpMainTscreen, curComponent.ComponentUpMainTscrErrMsg);
			errorProvider1.SetError(txtDrawingID, curComponent.ComponentDrawingErrMsg);
			errorProvider1.SetError(txtMisc1, curComponent.ComponentMisc1ErrMsg);
			errorProvider1.SetError(txtMisc2, curComponent.ComponentMisc2ErrMsg);
			errorProvider1.SetError(txtNotes, curComponent.ComponentNoteErrMsg);
			errorProvider1.SetError(txtPctChromeBrExt, curComponent.ComponentPctChromeBrExtErrMsg);
			errorProvider1.SetError(txtPctChromeDsExt, curComponent.ComponentPctChromeDsExtErrMsg);
			errorProvider1.SetError(txtPctChromeUsExt, curComponent.ComponentPctChromeUsExtErrMsg);
			errorProvider1.SetError(txtUsExtOD, curComponent.ComponentUpExtOdErrMsg);
			errorProvider1.SetError(txtUsExtTnom, curComponent.ComponentUpExtTnomErrMsg);
			errorProvider1.SetError(txtUsExtTscr, curComponent.ComponentUpExtTscrErrMsg);
			errorProvider1.SetError(txtDsExtOD, curComponent.ComponentDnExtOdErrMsg);
			errorProvider1.SetError(txtDsExtTnom, curComponent.ComponentDnExtTnomErrMsg);
			errorProvider1.SetError(txtDsExtTscr, curComponent.ComponentDnExtTscrErrMsg);
			errorProvider1.SetError(txtBranchExtOD, curComponent.ComponentBrExtOdErrMsg);
			errorProvider1.SetError(txtBranchExtTnom, curComponent.ComponentBrExtTnomErrMsg);
			errorProvider1.SetError(txtBranchExtTscr, curComponent.ComponentBrExtTscrErrMsg);
			errorProvider1.SetError(txtTimesInspected, curComponent.ComponentTimesInspectedErrMsg);
			errorProvider1.SetError(txtAvgCrewDose, curComponent.ComponentAvgCrewDoseErrMsg);
			errorProvider1.SetError(txtAvgTimeToInspect, curComponent.ComponentAvgInspectionTimeErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtBranchOD).Length > 0 ||
				errorProvider1.GetError(txtBranchTnom).Length > 0 ||
				errorProvider1.GetError(txtBranchTscr).Length > 0 ||
				errorProvider1.GetError(txtDnMainOD).Length > 0 ||
				errorProvider1.GetError(txtDnMainTnom).Length > 0 ||
				errorProvider1.GetError(txtDnMainTscr).Length > 0 ||
				errorProvider1.GetError(txtName).Length > 0 ||
				errorProvider1.GetError(txtPctChromeMain).Length > 0 ||
				errorProvider1.GetError(txtPctChromeBranch).Length > 0 ||
				errorProvider1.GetError(txtUpMainOD).Length > 0 ||
				errorProvider1.GetError(txtUpMainTnom).Length > 0 ||
				errorProvider1.GetError(txtUpMainTscreen).Length > 0 ||
				errorProvider1.GetError(txtDrawingID).Length > 0 ||
				errorProvider1.GetError(txtMisc1).Length > 0 ||
				errorProvider1.GetError(txtMisc2).Length > 0 ||
				errorProvider1.GetError(txtNotes).Length > 0 ||
				errorProvider1.GetError(txtPctChromeBrExt).Length > 0 ||
				errorProvider1.GetError(txtPctChromeDsExt).Length > 0 ||
				errorProvider1.GetError(txtPctChromeUsExt).Length > 0 ||
				errorProvider1.GetError(txtUsExtOD).Length > 0 ||
				errorProvider1.GetError(txtUsExtTnom).Length > 0 ||
				errorProvider1.GetError(txtUsExtTscr).Length > 0 ||
				errorProvider1.GetError(txtDsExtOD).Length > 0 ||
				errorProvider1.GetError(txtDsExtTnom).Length > 0 ||
				errorProvider1.GetError(txtDsExtTscr).Length > 0 ||
				errorProvider1.GetError(txtBranchExtOD).Length > 0 ||
				errorProvider1.GetError(txtBranchExtTnom).Length > 0 ||
				errorProvider1.GetError(txtBranchExtTscr).Length > 0 ||
				errorProvider1.GetError(txtTimesInspected).Length > 0 ||
				errorProvider1.GetError(txtAvgCrewDose).Length > 0 ||
				errorProvider1.GetError(txtAvgTimeToInspect).Length > 0
				);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curComponent.ComponentName = txtName.Text;
			curComponent.ComponentSysID = (Guid?)cboSystem.SelectedValue;
			curComponent.ComponentIsActive = ckActive.Checked;
			curComponent.ComponentLinID = (Guid?)cboLine.SelectedValue;
			curComponent.ComponentCtpID = (Guid?)cboType.SelectedValue;
			curComponent.ComponentCmtID = (Guid?)cboMaterial.SelectedValue;
			curComponent.ComponentPslID = curPipeSchedule == null ? null : curPipeSchedule.ID;
			curComponent.ComponentPctChromeMain = Util.GetNullableDecimalForString(txtPctChromeMain.Text);
			curComponent.ComponentPctChromeBranch = Util.GetNullableDecimalForString(txtPctChromeBranch.Text);
			curComponent.ComponentUpMainOd = Util.GetNullableDecimalForString(txtUpMainOD.Text);
			curComponent.ComponentUpMainTnom = Util.GetNullableDecimalForString(txtUpMainTnom.Text);
			curComponent.ComponentUpMainTscr = Util.GetNullableDecimalForString(txtUpMainTscreen.Text);
			curComponent.ComponentDnMainOd = Util.GetNullableDecimalForString(txtDnMainOD.Text);
			curComponent.ComponentDnMainTnom = Util.GetNullableDecimalForString(txtDnMainTnom.Text);
			curComponent.ComponentDnMainTscr = Util.GetNullableDecimalForString(txtDnMainTscr.Text);
			curComponent.ComponentBranchOd = Util.GetNullableDecimalForString(txtBranchOD.Text);
			curComponent.ComponentBranchTnom = Util.GetNullableDecimalForString(txtBranchTnom.Text);
			curComponent.ComponentBranchTscr = Util.GetNullableDecimalForString(txtBranchTscr.Text);
			curComponent.ComponentPctChromeBrExt = Util.GetNullableDecimalForString(txtPctChromeBrExt.Text);
			curComponent.ComponentPctChromeDsExt = Util.GetNullableDecimalForString(txtPctChromeDsExt.Text);
			curComponent.ComponentPctChromeUsExt = Util.GetNullableDecimalForString(txtPctChromeUsExt.Text);
			curComponent.ComponentUpExtOd = Util.GetNullableDecimalForString(txtUsExtOD.Text);
			curComponent.ComponentUpExtTnom = Util.GetNullableDecimalForString(txtUsExtTnom.Text);
			curComponent.ComponentUpExtTscr = Util.GetNullableDecimalForString(txtUsExtTscr.Text);
			curComponent.ComponentDnExtOd = Util.GetNullableDecimalForString(txtDsExtOD.Text);
			curComponent.ComponentDnExtTnom = Util.GetNullableDecimalForString(txtDsExtTnom.Text);
			curComponent.ComponentDnExtTscr = Util.GetNullableDecimalForString(txtDsExtTscr.Text);
			curComponent.ComponentBrExtOd = Util.GetNullableDecimalForString(txtBranchExtOD.Text);
			curComponent.ComponentBrExtTnom = Util.GetNullableDecimalForString(txtBranchExtTnom.Text);
			curComponent.ComponentBrExtTscr = Util.GetNullableDecimalForString(txtBranchExtTscr.Text);
			curComponent.ComponentNote = txtNotes.Text;
			curComponent.ComponentDrawing = txtDrawingID.Text;
			curComponent.ComponentMisc1 = txtMisc1.Text;
			curComponent.ComponentMisc2 = txtMisc2.Text;
			curComponent.ComponentIsActive = ckActive.Checked;
			curComponent.ComponentHardToAccess = ckHardAccess.Checked;
			curComponent.ComponentHighRad = ckHighRad.Checked;
			curComponent.ComponentHasDs = ckHasDownstream.Checked;
			curComponent.ComponentHasBranch = ckHasBranch.Checked;
			curComponent.ComponentTimesInspected = Util.GetIntForString(txtTimesInspected.Text);
			curComponent.ComponentAvgInspectionTime = Util.GetFloatForString(txtAvgTimeToInspect.Text);
			curComponent.ComponentAvgCrewDose = Util.GetFloatForString(txtAvgCrewDose.Text);
		}

	}
}