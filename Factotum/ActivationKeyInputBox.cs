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
	public partial class ActivationKeyInputBox : Form
	{
		int maxRetries = 5;
		int retries = 0;
		//---------------------------------------------------------
		// Initialization
		//---------------------------------------------------------
		public string newKey;

		public ActivationKeyInputBox()
		{
			InitializeComponent();
		}

		//---------------------------------------------------------
		// Event Handlers
		//---------------------------------------------------------

		// Close the form.
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		// User clicked ok -- check for a valid key
		private void btnOK_Click(object sender, EventArgs e)
		{
			bool isFuture;

			long value;
			if (long.TryParse(txtActivationKey.Text, out value))
			{
				if (value.ToString().Length != 10)
				{
					errorProvider1.SetError(txtActivationKey, "Activation key should be ten digits");
					return;
				}
			}

			if (!Util.IsNullOrEmpty(txtActivationKey.Text) &&
			errorProvider1.GetError(txtActivationKey).Length == 0)
			{
				if (!ActivationKey.IsKeyValid(txtActivationKey.Text, out isFuture) || isFuture)
				{
					MessageBox.Show("Invalid Key", "Factotum");
					retries++;
					if (retries >= maxRetries)
					{
						this.DialogResult = DialogResult.Cancel;
						Close();
					}
				}
				else
				{
					newKey = txtActivationKey.Text;
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
		}

		// Each time the text changes, check to make sure its length is ok
		// If not, set the error.
		private void txtActivationKey_TextChanged(object sender, EventArgs e)
		{
			// Calling this method sets the ErrMsg property of the Object.
			if (txtActivationKey.Text != null && txtActivationKey.Text.Length > 10)
				errorProvider1.SetError(txtActivationKey, "Activation key should not exceed 10 characters");
			else if (!Util.IsNullOrEmpty(txtActivationKey.Text))
			{
				long value;
				if (long.TryParse(txtActivationKey.Text, out value))
				{
					errorProvider1.SetError(txtActivationKey, "");
				}
				else
				{
					errorProvider1.SetError(txtActivationKey, "Activation key should be all numeric");
				}
			}
			else
			{
				errorProvider1.SetError(txtActivationKey, "");
			}
		}

		//---------------------------------------------------------
		// Helper functions
		//---------------------------------------------------------



	}
}