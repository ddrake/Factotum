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
	public partial class KitEdit : Form, IEntityEditForm
	{
		private EKit curKit;
		private DataTable inspectorAssignments;
		private DataTable meterAssignments;
		private DataTable ducerAssignments;
		private DataTable calBlockAssignments;
		private DataTable thermoAssignments;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curKit; }
		}

		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		// If you are creating a new record, the ID should be null
		// Normally in this case, you will want to provide a parentID
		public KitEdit()
			: this(null){}

		public KitEdit(Guid? ID)
		{
			InitializeComponent();
			curKit = new EKit();
			curKit.Load(ID);
			InitializeControls(ID == null);
		}

		// Initialize the form control values
		private void InitializeControls(bool newRecord)
		{
			// Note: We only need to show active items are shown in the checked list boxes 
			// because Items are removed from kits whenever they are inactivated.
			// For the special case of inspectors, we also want to exclude all inspectors
			// who are not assigned to the current outage.  If an inspector is first assigned
			// to an outage and given a toolkit, then unassigned, his tool kit will be unassigned.
			GetInspectorAssignments();
			GetMeterAssignments();
			GetDucerAssignments();
			GetCalBlockAssignments();
			GetThermoAssignments();

			SetControlValues();
			this.Text = newRecord ? "New Kit" : "Edit Kit";
			EInspector.Changed += new EventHandler<EntityChangedEventArgs>(EInspector_Changed);
			EMeter.Changed += new EventHandler<EntityChangedEventArgs>(EMeter_Changed);
			EDucer.Changed += new EventHandler<EntityChangedEventArgs>(EDucer_Changed);
			ECalBlock.Changed += new EventHandler<EntityChangedEventArgs>(ECalBlock_Changed);
			EThermo.Changed += new EventHandler<EntityChangedEventArgs>(EThermo_Changed);
		}

		void EThermo_Changed(object sender, EntityChangedEventArgs e)
		{
			GetThermoAssignments();
		}

		void ECalBlock_Changed(object sender, EntityChangedEventArgs e)
		{
			GetCalBlockAssignments();
		}

		void EDucer_Changed(object sender, EntityChangedEventArgs e)
		{
			GetDucerAssignments();
		}

		void EMeter_Changed(object sender, EntityChangedEventArgs e)
		{
			GetMeterAssignments();
		}

		void EInspector_Changed(object sender, EntityChangedEventArgs e)
		{
			GetInspectorAssignments();
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
			curKit.ToolKitNameLengthOk(txtName.Text);
			errorProvider1.SetError(txtName, curKit.ToolKitNameErrMsg);
		}


		// The validating event gets called when the user leaves the control.
		// We handle it to perform all validation for the value.
		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			// Calling this function will set the ErrMsg property of the object.
			curKit.ToolKitNameValid(txtName.Text);
			errorProvider1.SetError(txtName, curKit.ToolKitNameErrMsg);
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
			if (!curKit.Valid())
			{
				setAllErrors();
				return;
			}
			// The Save function returns a the Guid? of the record created or updated.
			Guid? tmpID = curKit.Save();
			if (tmpID != null)
			{
				// We need to do these updates after saving because they require a valid Kit ID

				// Update Inspectors table from checkboxes
				for (int dmRow = 0; dmRow < inspectorAssignments.Rows.Count; dmRow++)
				{
					inspectorAssignments.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbInspectors.CheckedIndices)
				{
					inspectorAssignments.Rows[idx]["IsAssigned"] = 1;
				}
				EInspector.UpdateAssignmentsToKit((Guid)curKit.ID, inspectorAssignments);

				// Update Meters table from checkboxes
				for (int dmRow = 0; dmRow < meterAssignments.Rows.Count; dmRow++)
				{
					meterAssignments.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbMeters.CheckedIndices)
				{
					meterAssignments.Rows[idx]["IsAssigned"] = 1;
				}
				EMeter.UpdateAssignmentsToKit((Guid)curKit.ID, meterAssignments);

				// Update Ducers table from checkboxes
				for (int dmRow = 0; dmRow < ducerAssignments.Rows.Count; dmRow++)
				{
					ducerAssignments.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbTransducers.CheckedIndices)
				{
					ducerAssignments.Rows[idx]["IsAssigned"] = 1;
				}
				EDucer.UpdateAssignmentsToKit((Guid)curKit.ID, ducerAssignments);

				// Update Thermos table from checkboxes
				for (int dmRow = 0; dmRow < thermoAssignments.Rows.Count; dmRow++)
				{
					thermoAssignments.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbThermos.CheckedIndices)
				{
					thermoAssignments.Rows[idx]["IsAssigned"] = 1;
				}
				EThermo.UpdateAssignmentsToKit((Guid)curKit.ID, thermoAssignments);

				// Update CalBlocks table from checkboxes
				for (int dmRow = 0; dmRow < calBlockAssignments.Rows.Count; dmRow++)
				{
					calBlockAssignments.Rows[dmRow]["IsAssigned"] = 0;
				}
				foreach (int idx in clbCalBlocks.CheckedIndices)
				{
					calBlockAssignments.Rows[idx]["IsAssigned"] = 1;
				}
				ECalBlock.UpdateAssignmentsToKit((Guid)curKit.ID, calBlockAssignments);

				Close();
				DialogResult = DialogResult.OK;
			}
		}

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtName.Text = curKit.ToolKitName;
		}

		// Set the error provider text for all controls that use it.
		private void setAllErrors()
		{
			errorProvider1.SetError(txtName, curKit.ToolKitNameErrMsg);
		}

		// Check all controls to see if any have errors.
		private bool AnyControlErrors()
		{
			return (errorProvider1.GetError(txtName).Length > 0);
		}

		// Update the object values from the form control values.
		private void UpdateRecord()
		{
			curKit.ToolKitName = txtName.Text;
		}

		private void GetInspectorAssignments()
		{
			// Inspector CheckedListbox
			inspectorAssignments = EInspector.GetWithAssignmentsForOutageAndKit(
				(Guid)Globals.CurrentOutageID, curKit.ID, false);
			clbInspectors.Items.Clear();
			int rows = inspectorAssignments.Rows.Count;
			if (rows == 0)
			{
				clbInspectors.Visible = false;
			}
			else
			{
				for (int dmRow = 0; dmRow < inspectorAssignments.Rows.Count; dmRow++)
				{
					string inspectorName = (string)inspectorAssignments.Rows[dmRow]["InspectorName"];
					string level = EInspector.InspectorLevelStringForLevel(
						(byte)inspectorAssignments.Rows[dmRow]["InspectorLevel"]);
					bool isActive = (bool)inspectorAssignments.Rows[dmRow]["InspectorIsActive"];
					bool isAssigned = Convert.ToBoolean(inspectorAssignments.Rows[dmRow]["IsAssigned"]);
					clbInspectors.Items.Add(inspectorName + " " + level, isAssigned);
				}
			}

		}
		private void GetMeterAssignments()
		{
			// Meter CheckedListbox
			meterAssignments = EMeter.GetWithAssignmentsForKit(curKit.ID, false);
			clbMeters.Items.Clear();
			int rows = meterAssignments.Rows.Count;
			if (rows == 0)
			{
				clbMeters.Visible = false;
			}
			else
			{
				for (int dmRow = 0; dmRow < meterAssignments.Rows.Count; dmRow++)
				{
					string modelName = (string)meterAssignments.Rows[dmRow]["MeterModelName"];
					string serialNumber = (string)meterAssignments.Rows[dmRow]["MeterSerialNumber"];
					bool isActive = (bool)meterAssignments.Rows[dmRow]["MeterIsActive"];
					bool isAssigned = Convert.ToBoolean(meterAssignments.Rows[dmRow]["IsAssigned"]);
					clbMeters.Items.Add(modelName + " - S/N: " + serialNumber, isAssigned);
				}
			}

		}
		private void GetDucerAssignments()
		{
			// Ducer CheckedListbox
			ducerAssignments = EDucer.GetWithAssignmentsForKit(curKit.ID, false);
			clbTransducers.Items.Clear();
			int rows = ducerAssignments.Rows.Count;
			if (rows == 0)
			{
				clbTransducers.Visible = false;
			}
			else
			{
				for (int dmRow = 0; dmRow < ducerAssignments.Rows.Count; dmRow++)
				{
					string modelName = (string)ducerAssignments.Rows[dmRow]["DucerModelName"];
					string serialNumber = (string)ducerAssignments.Rows[dmRow]["DucerSerialNumber"];
					bool isActive = (bool)ducerAssignments.Rows[dmRow]["DucerIsActive"];
					bool isAssigned = Convert.ToBoolean(ducerAssignments.Rows[dmRow]["IsAssigned"]);
					clbTransducers.Items.Add(modelName + " - S/N: " + serialNumber, isAssigned);
				}
			}
		}
		private void GetCalBlockAssignments()
		{
			// Cal Block CheckedListbox
			calBlockAssignments = ECalBlock.GetWithAssignmentsForKit(curKit.ID, false);
			clbCalBlocks.Items.Clear();
			int rows = calBlockAssignments.Rows.Count;
			if (rows == 0)
			{
				clbCalBlocks.Visible = false;
			}
			else
			{
				for (int dmRow = 0; dmRow < calBlockAssignments.Rows.Count; dmRow++)
				{
					string serialNumber = (string)calBlockAssignments.Rows[dmRow]["CalBlockSerialNumber"];
					bool isActive = (bool)calBlockAssignments.Rows[dmRow]["CalBlockIsActive"];
					bool isAssigned = Convert.ToBoolean(calBlockAssignments.Rows[dmRow]["IsAssigned"]);
					clbCalBlocks.Items.Add("S/N: " + serialNumber, isAssigned);
				}
			}
		}
		private void GetThermoAssignments()
		{
			// Thermos CheckedListbox
			thermoAssignments = EThermo.GetWithAssignmentsForKit(curKit.ID, false);
			clbThermos.Items.Clear();
			int rows = thermoAssignments.Rows.Count;
			if (rows == 0)
			{
				clbThermos.Visible = false;
			}
			else
			{
				for (int dmRow = 0; dmRow < thermoAssignments.Rows.Count; dmRow++)
				{
					string serialNumber = (string)thermoAssignments.Rows[dmRow]["ThermoSerialNumber"];
					bool isActive = (bool)thermoAssignments.Rows[dmRow]["ThermoIsActive"];
					bool isAssigned = Convert.ToBoolean(thermoAssignments.Rows[dmRow]["IsAssigned"]);
					clbThermos.Items.Add("S/N: " + serialNumber, isAssigned);
				}
			}
		}
	}
}