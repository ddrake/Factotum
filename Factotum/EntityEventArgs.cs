using System;
using System.Collections.Generic;
using System.Text;

namespace Factotum
{
	public class EntityChangedEventArgs : EventArgs
	{
		private Guid? id;

		public Guid? ID
		{
			get { return id; }
		}
		public EntityChangedEventArgs(Guid? ID)
		{
			id = ID;
		}
	}
}
