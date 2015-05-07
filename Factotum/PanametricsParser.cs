using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DowUtils;
using System.IO;
using System.ComponentModel;

namespace Factotum
{
	class PanametricsParser : TextFileParser
	{
		private Guid dsetID;
		private ESurveyCollection surveys;

		public PanametricsParser(EDset dset, BackgroundWorker bw)
		{
			this.filePath = dset.DsetTextFileName;
			this.dsetID = (Guid)dset.ID;
			this.bw = bw;
			surveys = new ESurveyCollection();
		}

		public override void ParseTextFile(object sender, DoWorkEventArgs e)
		{
			StreamReader sr;
			int r = 0;
			resultMessage = "";
			const int FinalLines = 7;
			int firstMeasurementRow;
			// Try to open the text file
			using (sr = new StreamReader(this.filePath))
			{
				String sData;
				sData = sr.ReadToEnd();

				int linesDone = 0;
				string[] cols;

				// Split the file into lines
				String[] sep = { "\r\n" };
				String[] lines = sData.Split(sep, StringSplitOptions.None);

				// Verify first line for valid file format
				if (lines[r].IndexOf("[SWEET 16]") < 0 && 
					lines[r].IndexOf("[CHILD]") < 0 &&
					lines[r].IndexOf("[FACTOTUM MERGED]") < 0)
				{
					// File doesn't start with the right heading line
					resultMessage = "Expected Line " + r + " to match: '[SWEET 16]' or '[CHILD]' or '[FACTOTUM MERGED]'";
					goto ParseError;
				}
				while (r < lines.Length)
				{
					if (lines[r].IndexOf("IDENTIFIER") > 0 &&
						lines[r].IndexOf("THICKNESS") > 0 &&
						lines[r].IndexOf("UNITS") > 0 &&
						lines[r].IndexOf("FLAGS") > 0 &&
						lines[r].IndexOf("SU #") > 0) break;

					r++;
				}
				if (r >= lines.Length)
				{
					resultMessage = "File does not contain a valid Measurement column header row.  Expected\n" +
						"IDENTIFIER      THICKNESS    UNITS    FLAGS    SU #";
					goto ParseError;
				}
				r++;
				firstMeasurementRow = r;
				linesDone = r;
				bw.ReportProgress((linesDone + FinalLines) * 100 / lines.Length);

				// Read past the Measurements. I.e. read and ignore lines until first "OK" found
				while (r < lines.Length && lines[r].Trim().CompareTo("OK") != 0) r++;

				if (r >= lines.Length)
				{
					resultMessage = "Expected Measurements section to be terminated by 'OK'";
					goto ParseError;
				}

				// Verify the row header for the surveys section
				while (r < lines.Length)
				{
					if (lines[r].IndexOf("SU #") > 0 &&
						lines[r].IndexOf("VEL(/uS)") > 0 &&
						lines[r].IndexOf("DIFF") > 0 &&
						lines[r].IndexOf("LO-ALM") > 0 &&
						lines[r].IndexOf("HI-ALM") > 0 &&
						lines[r].IndexOf("EXT-BLANK") > 0 &&
						lines[r].IndexOf("UNITS") > 0 &&
						lines[r].IndexOf("TRANSDUCER") > 0 &&
						lines[r].IndexOf("GAIN dB") > 0) break;

					r++;
				}
				if (r >= lines.Length)
				{
					resultMessage = "File does not contain a valid Survey column header row. Expected: \n" +
						"SU #  VEL(/uS) DIFF   LO-ALM  HI-ALM  EXT-BLANK  UNITS  TRANSDUCER  GAIN dB";
					goto ParseError;
				}
				
				r++;
				// Read and process Survey lines until second "OK" found
				while (r < lines.Length && lines[r].Trim().CompareTo("OK") != 0)
				{
					// Break up the line into data columns.  Without the RemoveEmptyEntries flag, 
					// we would have an empty array element for every space character.
					cols = lines[r].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					ESurvey survey = new ESurvey();
					survey.SurveyDstID = dsetID;
					survey.SurveyNumber = short.Parse(cols[0]);
					survey.SurveyVelocity = decimal.Parse(cols[1]);
		
					survey.SurveyUnits = cols[6];
					if (survey.SurveyUnits != "IN" && survey.SurveyUnits != "MM")
					{
						resultMessage = "Survey in line " + r + " refers to unknown units.";
						goto ParseError;
					}

					survey.SurveyTransducer = cols[7];
					survey.SurveyGainDb = short.Parse(cols[8]);
					survey.Save();
					surveys.Add(survey);
					r++;
					linesDone++;
					bw.ReportProgress((linesDone + FinalLines) * 100 / lines.Length);
				}
				
				// Now go back to the first measurement row and read measurement lines until first "OK" found
				r = firstMeasurementRow;
				while (r < lines.Length && lines[r].Trim().CompareTo("OK") != 0)
				{
					cols = lines[r].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					EMeasurement measurement = new EMeasurement();
					short surveyNumber = short.Parse(cols[4]);
					ESurvey survey = surveys.FindFirstForSurveyNumber(surveyNumber);
					if (survey == null)
					{
						resultMessage = "Measurement in line " + r + " references a survey which is not defined.";
						goto ParseError;
					}
					// Try to read the thickness
					measurement.MeasurementSvyID = survey.ID;

					if (cols[1] == "--.---")
					{
						measurement.MeasurementThickness = null;
						measurement.MeasurementIsObstruction = true;
					}
					else
					{
						measurement.MeasurementThickness = decimal.Parse(cols[1]);

						// If the units are mm, convert to inches.
						if (survey.SurveyUnits == "MM") 
							measurement.MeasurementThickness /= 25.4m;
						measurement.MeasurementIsObstruction = false;
					}

					// Try to get the row and column
					short row;
					short col;
					if (!EMeasurement.GetRowAndColForIdentifier(cols[0], out row, out col))
					{
						resultMessage = "Unable to determine row and column in line " + r + ".";
						goto ParseError;
					}
					measurement.MeasurementRow = row;
					measurement.MeasurementCol = col;

					// Set error if flag is not as expected.
					if (!measurement.MeasurementIsObstruction && cols[3].Substring(0, 1) != "M")
						measurement.MeasurementIsError = true;

					measurement.Save();

					r++;
					linesDone++;
					bw.ReportProgress((linesDone + FinalLines) * 100 / lines.Length);
				}
				resultMessage = "Panametrics text file imported OK.";
				result = true;
				return;
			}
			ParseError:
			if (resultMessage.Length == 0)
			{
				resultMessage = "Error reading line " + r + " of text file.";
				result = false;
			}
		}

	}
}
