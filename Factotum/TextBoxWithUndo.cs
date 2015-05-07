using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public class TextBoxWithUndo : TextBox
	{
		public TextBoxWithUndo()
			: base()
		{
			KeyDown += new KeyEventHandler(TextBoxWithUndo_KeyDown);
		}

		// Consider making a custom textbox control with this functionality.
		// We probably want it on every textbox control on a data entry form.
		private void TextBoxWithUndo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				// Determine if last operation can be undone in text box.   
				if (CanUndo)
				{
					// Undo the last operation.
					Undo();
					// Clear the undo buffer to prevent last action from being redone.
					ClearUndo();
				}
			}
		}
	}
}
