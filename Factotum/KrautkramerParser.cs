using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DowUtils;
using System.IO;
using System.ComponentModel;

namespace Factotum
{
	class KrautkramerParser : TextFileParser
	{
		private ESurvey survey;

		public KrautkramerParser(EDset dset, BackgroundWorker bw) 
		{
			this.filePath = dset.DsetTextFileName;
			this.bw = bw;
			survey = new ESurvey();
			survey.SurveyDstID = (Guid)dset.ID;
			survey.Save();
		}

		public override void ParseTextFile(object sender, DoWorkEventArgs e)
		{
			StreamReader sr;
			resultMessage = "";
			// Try to open the text file
			using (sr = new StreamReader(this.filePath))
			{
				String sData;
				//try
				//{
					sData = sr.ReadToEnd();

					string[] cols;
					int ncols;
					int nrows;
					int r;
					int dr, dc; // index of row containing data (ignoring blank lines)

					// Split the file into lines
					String[] sep = { "\r\n" };
					String[] lines = sData.Split(sep, StringSplitOptions.None);

					nrows = lines.Length;
					r = 0; dr = 0;
					while (r < nrows)
					{
						if (lines[r].Length == 0)
						{
							r++; // increment r, but not dr
							continue;
						}
						cols = lines[r].Split(null);
						ncols = cols.Length;
						for (dc = 0; dc < ncols; dc++)
						{
							EMeasurement measurement = new EMeasurement();
							measurement.MeasurementSvyID = survey.ID;
							measurement.MeasurementRow = (short)dr;
							measurement.MeasurementCol = (short)dc;

							if (cols[dc] == "EMPTY")
							{
								measurement.MeasurementThickness = null;
							}
							else if (cols[dc] == "OBSTR")
							{
								measurement.MeasurementIsObstruction = true;
								measurement.MeasurementThickness = null;
							}
							else
							{
								decimal test;
								if (decimal.TryParse(cols[dc], out test))
								{
									measurement.MeasurementThickness = test;
								}
								else
								{
									resultMessage = "Unable to read value " + cols[dc] + "  in row " + (r + 1) + ", column " + (dc + 1);
									goto ParseError;
								}
							}
							measurement.Save();
						}
						r++;
						dr++;
						bw.ReportProgress(100 * r / nrows);
					}
					resultMessage = "Krautkramer text file imported OK.";
					result = true;
					return;
				//}
				//catch (Exception ex)
				//{
				//   // Todo: log the exception.
				//   goto ParseError;
				//}
				ParseError:
					if (resultMessage.Length == 0)
					{
						resultMessage = "Error reading line " + r + " of text file.";
						result = false;
					}
				}
		}

	}
}
