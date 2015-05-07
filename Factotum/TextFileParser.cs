using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Factotum
{
	class TextFileParser
	{
		protected string filePath;
		protected BackgroundWorker bw;
		protected string resultMessage;
		protected bool result;

		public bool Result
		{
			get { return result; }
			set { result = value; }
		}

		public string ResultMessage
		{
			get { return resultMessage; }
			set { resultMessage = value; }
		}

		public TextFileParser() { }

		public virtual void ParseTextFile(object sender, DoWorkEventArgs e) { }

	}
}
