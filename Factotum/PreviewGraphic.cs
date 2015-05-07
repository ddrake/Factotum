using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class PreviewGraphic : Form
	{
		public PreviewGraphic()
		{
			InitializeComponent();
		}

		public void SetPreviewImage(Image previewImage)
		{
			panel1.BackgroundImage = previewImage;
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}	
	}
}