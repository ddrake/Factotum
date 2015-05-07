using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class Preferences_General : Form
	{
		public Preferences_General()
		{
			InitializeComponent();
		}

		private void GeneralPreferences_Load(object sender, EventArgs e)
		{
            txtFactotumDataFolder.Text = UserSettings.sets.FactotumDataFolder;
            txtDefaultImageFolder.Text = UserSettings.sets.DefaultImageFolder;
            txtMeterDataFolder.Text = UserSettings.sets.MeterDataFolder;
            ckValidateEpriRecommended.Checked = UserSettings.sets.ValidateEpriRecommended;
            ckValidateTemperatures.Checked = UserSettings.sets.ValidateTemperatures;
            ckValidateNoUpMain.Checked = UserSettings.sets.ValidateNoUpMain;
            ckValidateNoUpExt.Checked = UserSettings.sets.ValidateNoUpExt;
            ckValidateNoDnExt.Checked = UserSettings.sets.ValidateNoDnExt;
            if (UserSettings.sets.ValidateGridColCountWithCeiling)
                rbValidateColsCeiling.Checked = true;
            else
                rbValidateColsRound.Checked = true;
            if (UserSettings.sets.ValidateDsExtRowCountWithCeiling)
                rbValidateDsExtRowsCeiling.Checked = true;
            else
                rbValidateDsExtRowsRound.Checked = true;
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
            UserSettings.sets.FactotumDataFolder = txtFactotumDataFolder.Text;
            UserSettings.sets.DefaultImageFolder = txtDefaultImageFolder.Text;
            UserSettings.sets.MeterDataFolder = txtMeterDataFolder.Text;
            UserSettings.sets.ValidateEpriRecommended = ckValidateEpriRecommended.Checked;
            UserSettings.sets.ValidateTemperatures = ckValidateTemperatures.Checked;
            UserSettings.sets.ValidateNoUpMain = ckValidateNoUpMain.Checked;
            UserSettings.sets.ValidateNoUpExt = ckValidateNoUpExt.Checked;
            UserSettings.sets.ValidateNoDnExt = ckValidateNoDnExt.Checked;
            UserSettings.sets.ValidateGridColCountWithCeiling = rbValidateColsCeiling.Checked;
            UserSettings.sets.ValidateDsExtRowCountWithCeiling = rbValidateDsExtRowsCeiling.Checked;
            UserSettings.Save();
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.Description = "Factotum Data Folder";
			folderBrowserDialog1.SelectedPath = UserSettings.sets.FactotumDataFolder;
			DialogResult rslt = folderBrowserDialog1.ShowDialog();
			if (rslt == DialogResult.OK)
			{
				txtFactotumDataFolder.Text = folderBrowserDialog1.SelectedPath;
			}
        }

        private void btnBrowse_Image_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Default Image Folder";
            folderBrowserDialog1.SelectedPath = UserSettings.sets.DefaultImageFolder;
            DialogResult rslt = folderBrowserDialog1.ShowDialog();
            if (rslt == DialogResult.OK)
            {
                txtDefaultImageFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnBrowse_MeterData_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Meter Data Folder";
            folderBrowserDialog1.SelectedPath = UserSettings.sets.MeterDataFolder;
            DialogResult rslt = folderBrowserDialog1.ShowDialog();
            if (rslt == DialogResult.OK)
            {
                txtMeterDataFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }


	}
}