using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Factotum
{
	public interface IEntityEditForm
	{
		IEntity Entity
		{
			get;
		}
	}
}
