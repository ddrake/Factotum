using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public class DataGridViewStd : DataGridView
	{
		protected override bool ProcessDialogKey(Keys keyData)
		{
			Keys key = (keyData & Keys.KeyCode);
			if (key == Keys.Enter)
			{
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override bool ProcessDataGridViewKey(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				return true;
			}
			return base.ProcessDataGridViewKey(e);
		}

		public DataGridViewStd()
		{
			this.RowHeadersVisible = false;
			this.AllowUserToResizeRows = false;
			this.AllowUserToAddRows = false;
			this.AllowUserToDeleteRows = false;

			this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.MultiSelect = false;
			this.AllowUserToOrderColumns = true;
			this.ReadOnly = true;
			this.StandardTab = true;

		}
	}

}
