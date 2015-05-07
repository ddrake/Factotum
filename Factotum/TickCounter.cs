using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Factotum
{
	public class TickCounter
	{
		private EventLog log;
		private DateTime start;
		private string message;

		public TickCounter(string message)
		{
			this.message = message;
			log = new EventLog("Factotum",".","Factotum");
			start = DateTime.Now;
		}
		public void Start(string message)
		{
			this.message = message;
			start = DateTime.Now;
		}
		public void Done()
		{
			string msg = DateTime.Now.Subtract(start).Milliseconds + " " + message;
			log.WriteEntry(msg);
		}
		public void DoneStartNew(string newMessage)
		{
			string msg = DateTime.Now.Subtract(start).Milliseconds + " " + message;
			log.WriteEntry(msg);
			this.message = newMessage;
			start = DateTime.Now;
		}
	}
}
