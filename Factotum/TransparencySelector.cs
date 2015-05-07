using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class TransparencySelector : Form
	{
		private int alphaLevel;

		public int AlphaLevel
		{
			get { return alphaLevel; }
			set
			{
				if (alphaLevel != value)
				{
					alphaLevel = value;
					trackBar1.Value = alphaLevel;
				}
			}
		}
	
		public TransparencySelector()
		{
			InitializeComponent();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			alphaLevel = trackBar1.Value;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}