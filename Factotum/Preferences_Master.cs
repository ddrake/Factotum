using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class Preferences_Master : Form
	{
		public Preferences_Master()
		{
			InitializeComponent();
		}

		private void MasterPreferences_Load(object sender, EventArgs e)
		{
			if (Globals.IsMasterDB)
			{
				// Add the "No Selection" option, so the user can have no current outage if
				// so desired.
				EOutageCollection outages = EOutage.ListByName(true);
				cboCurrentOutage.DataSource = outages;
				cboCurrentOutage.DisplayMember = "OutageNameWithSiteInParens";
				cboCurrentOutage.ValueMember = "ID";
				// show the current property setting
				if (UserSettings.sets.CurrentOutageID.Length != 0)
					cboCurrentOutage.SelectedValue = new Guid(UserSettings.sets.CurrentOutageID);
			}
			else
			{
				// This is an outage database so...
				// Don't add the "No Selection" Option
				// The list should contain only one item in this case, the current outage.
				EOutageCollection outages = EOutage.ListByName(false);
				cboCurrentOutage.DataSource = outages;
				cboCurrentOutage.DisplayMember = "OutageNameWithSiteInParens";
				cboCurrentOutage.ValueMember = "ID";
				// Just show the current outage id
				cboCurrentOutage.SelectedValue = Globals.CurrentOutageID;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			// Don't bother changing properties or global settings unless this is a master db.
			if (Globals.IsMasterDB)
			{
                string outageID = (cboCurrentOutage.SelectedValue != null ?
						cboCurrentOutage.SelectedValue.ToString() : "");

				UserSettings.sets.CurrentOutageID = outageID;
                UserSettings.Save();
						
				Globals.SetCurrentOutageID();

                // Handle enabling of the outage menu.
                AppMain main = (AppMain)this.MdiParent;
                main.handleMenuEnabling();
            }
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

	}
}