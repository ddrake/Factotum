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
	public partial class ActivationKeyGenerator : Form
	{
		public string key;
		private bool closeAfterGenerating;
		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------

		public ActivationKeyGenerator() : this(false) {	}

		public ActivationKeyGenerator(bool closeAfterGenerating)
		{
			InitializeComponent();
			InitializeControls();
			this.closeAfterGenerating = closeAfterGenerating;
			key = null;
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			SetControlValues();
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		// Close the form.
		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		// Generate the activation key if the number of days is valid
		private void btnGenerate_Click(object sender, EventArgs e)
		{
			string key;
			if (!Util.IsNullOrEmpty(txtNumberOfDays.Text) &&
			errorProvider1.GetError(txtNumberOfDays).Length == 0)
			{
				key =
					ActivationKey.CreateKey(dtpStartDate.Value, int.Parse(txtNumberOfDays.Text));

				bool isFuture;
				DateTime testStart;
				int testDays;
				if (!ActivationKey.IsKeyValid(key, out isFuture))
				{
					MessageBox.Show("Can't generate a key for a period in the past", "Factotum");
					lblActivationKey.Text = "Click to Generate";
				}
				else
				{
					ActivationKey.parseKey(key, out testStart, out testDays);
					if (testStart != dtpStartDate.Value || testDays != int.Parse(txtNumberOfDays.Text))
					{
						MessageBox.Show("Generated Key Date and/or Days did not match!", "Factotum");
						lblActivationKey.Text = "Click to Generate";
					}
					else
					{
						lblActivationKey.Text = key.Substring(0,3) + "  " + key.Substring(3,3) + "  " + key.Substring(6,4);
						// discourage user from seeing all possible values...
						btnGenerate.Enabled = false;
						this.key = key;
						if (this.closeAfterGenerating) Close();
					}

				}
			}
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtNumberOfDays_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			if (txtNumberOfDays.Text != null && txtNumberOfDays.Text.Length > 3)
				errorProvider1.SetError(txtNumberOfDays, "Number of days may not exceed 100");
			else if (!Util.IsNullOrEmpty(txtNumberOfDays.Text))
			{
				int value;
				if (int.TryParse(txtNumberOfDays.Text, out value))
				{
					if (value > 100)
						errorProvider1.SetError(txtNumberOfDays, "Number of days may not exceed 100");
					else if (value <= 0)
						errorProvider1.SetError(txtNumberOfDays, "Number of days must be greater than zero");
					else
						errorProvider1.SetError(txtNumberOfDays, "");
				}
				else
				{
					errorProvider1.SetError(txtNumberOfDays, "Not a valid number");
				}
			}
			else
			{
				errorProvider1.SetError(txtNumberOfDays, "");
			}
		}

		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------

		// Set the form controls to the site object values.
		private void SetControlValues()
		{
			txtNumberOfDays.Text = "30";
			dtpStartDate.Value = DateTime.Today;
		}


	}
}