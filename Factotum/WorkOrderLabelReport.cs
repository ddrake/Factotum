using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Factotum
{
	// An instance of this class is created when the user selects to print a set of work order labels.
	// The object is maintained throughout the printing process.  Each time a PrintPage event is received,
	// the report object's OutputToGraphics method is called with the event's arguments, more report data
	// is painted to the graphics, and the HasMorePages property of the graphics is set.

	class WorkOrderLabelReport
	{
		// These objects are loaded in the constructor
		public EInspectedComponentCollection reports;
		public EInspectedComponent eReport;
		public EComponent eComponent;
		public EUnit eUnit;
		public int curReportIdx;
		public int reportCount;
		private PrintDocument printDocument = null;
		public Font bigFont;
		public Font bigBoldFont;
		public Font regFont;

		public bool hasData;

		// Report constructor
		public WorkOrderLabelReport()
		{

			// Assemble the source data for the report
			reports = EInspectedComponent.ListByWorkOrderForOutage((Guid?)Globals.CurrentOutageID, false);
			reportCount = reports.Count;
			hasData = (reportCount != 0);
			if (!hasData) return;

			// Use this for the material
			bigFont = new Font("Times New Roman", 10, FontStyle.Regular);
			// Use this for the Component ID and Work Order number and material if SS
			bigBoldFont = new Font("Times New Roman", 10, FontStyle.Bold);
			// Used for all other text
			regFont = new Font("Times New Roman", 9);

			ResetReport();

			printDocument = new PrintDocument();

			// Wire up the event handlers
			printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
		}

		private void ResetReport()
		{
			curReportIdx = 0;

			// Get data for the current inspection
			eReport = reports[curReportIdx];
			eComponent = new EComponent(eReport.InspComponentCmpID);
			eUnit = new EUnit(eComponent.ComponentUntID);

		}

		// Optionally print a hardcopy of the report or show a preview of it.
		public void Print(bool hardcopy)
		{
			// create a PrintDialog based on the PrintDocument
			PrintDialog pdlg = new PrintDialog();
			pdlg.Document = printDocument;
			// show the PrintDialog
			if (pdlg.ShowDialog() == DialogResult.OK)
			{
				// decide what action to take
				if (hardcopy)
				{
					// actually print hardcopy
					printDocument.Print();
				}
				else
				{
					// preview onscreen instead
					PrintPreviewDialog prvw = new PrintPreviewDialog();
					prvw.Document = printDocument;
					prvw.ShowDialog();
				}
			}
		}


		// Print labels until we're finished or can't fit any more on the page
		void printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			Graphics measure = e.PageSettings.PrinterSettings.CreateMeasurementGraphics();

			int[,] triHtabs = new int[3,3] { { 22, 107, 192 }, { 296, 381, 467 }, {571, 656, 741} };
			int[,] dblHtabs = new int[3,2] { { 22, 149 }, { 296, 423 }, { 571, 698 } };
			// vertically, label tops are at 50, 150, 250, ... 950

			int startY = 52;
			float curX, curY;
			for (int row = 0; row < 10; row++)
			{				
				for (int col = 0; col < 3; col++)
				{
					curY = startY;
					curX = dblHtabs[col, 0];
					// Print the 1st row: Component ID
					g.DrawString("Cmp: " + eComponent.ComponentName, bigBoldFont, 
						Brushes.Black, new PointF(curX, curY));

					curY += bigBoldFont.Height;
					// Print the 2nd row: Work Order
					g.DrawString("W/O: " + eReport.InspComponentWorkOrder, bigBoldFont,
						Brushes.Black, new PointF(curX, curY));

					curX = dblHtabs[col, 1];
					g.DrawString("Rpt: " + eReport.InspComponentName, bigBoldFont,
						Brushes.Black, new PointF(curX, curY));

					curX = dblHtabs[col, 0];
					curY += bigBoldFont.Height;
					// Print the 3rd row: Line Name
					string lineName = (eComponent.ComponentLineName == null ? "N/A" : eComponent.ComponentLineName);
					g.DrawString(eComponent.ComponentLineName, regFont,
						Brushes.Black, new PointF(curX, curY));

					curY += regFont.Height;
					curX = dblHtabs[col, 0];
					string cmpType = (eComponent.ComponentTypeName == null ? "N/A" : eComponent.ComponentTypeName);
					// Print the 4th row: Geometry and Material
					g.DrawString(cmpType, bigFont,
						Brushes.Black, new PointF(curX, curY));
					curX = dblHtabs[col, 1];
					string cbkName = eComponent.ComponentCalBlockMaterialTypeName;
					g.DrawString("Material: " + cbkName, bigFont,
						Brushes.Black, new PointF(curX, curY));

					// Print the dimension rows
					string[,] dimStrings = GetDimensionStrings();

					curY += bigFont.Height;
					float refY = curY;
					for (int r = 0; r < dimStrings.GetLength(0); r++)
					{
						curY = refY + regFont.Height * r;
						curX = triHtabs[col, 0];
						// Print all diameters (black)
						g.DrawString("OD: " + dimStrings[r,0], regFont,
							Brushes.Black, new PointF(curX, curY));

						curX = triHtabs[col, 1];
						// Print all Tnoms (green)
						g.DrawString("Tnom: " + dimStrings[r, 1], regFont,
							Brushes.Green, new PointF(curX, curY));

						curX = triHtabs[col, 2];
						// Print all Tscreens (red)
						g.DrawString("Tscr: " + dimStrings[r, 2], regFont,
							Brushes.Red, new PointF(curX, curY));
					}
					curReportIdx++;
					if (curReportIdx >= reportCount)
					{
						// we've made a label for each component/report
						e.HasMorePages = false;
						ResetReport();
						return;
					}

					eReport = reports[curReportIdx];
					eComponent = new EComponent(eReport.InspComponentCmpID);
					eUnit = new EUnit(eComponent.ComponentUntID);
				}
				startY += 100;
			}
			// we've filled the page
			e.HasMorePages = true;
		}


		private string[,] GetDimensionStrings()
		{
			List<decimal> ods;
			List<decimal?> tnoms;
			List<decimal?> tscrs;

			GetDimLists(out ods, out tnoms, out tscrs);

			string[,] dimStrings = new string[ods.Count,3];

			int row = 0;
			foreach (decimal od in ods)
			{
				dimStrings[row, 0] = od.ToString("0.000");
				dimStrings[row, 1] = NaForNullDecimal(tnoms[row]);
				dimStrings[row, 2] = NaForNullDecimal(tscrs[row]);

				row++;
			}
			return dimStrings;
		}

		private string NaForNullDecimal(decimal? value)
		{
			if (value == null) return "N/A";
			return ((decimal)value).ToString("0.000");
		}

		private void GetDimLists(out List<decimal> ods, out List<decimal?> tnoms, out List<decimal?> tscrs)
		{
			ods = new List<decimal>(3);
			tnoms = new List<decimal?>(3);
			tscrs = new List<decimal?>(3);

			if (eComponent.ComponentUpMainOd != null)
			{
				ods.Add((decimal)eComponent.ComponentUpMainOd);
				tnoms.Add(eComponent.ComponentUpMainTnom);
				tscrs.Add(eComponent.ComponentUpMainTscr);
			}
			if (eComponent.ComponentDnMainOd != null)
			{
				if (!IsDecimalInList((decimal)eComponent.ComponentDnMainOd, ods))
				{
					ods.Add((decimal)eComponent.ComponentDnMainOd);
					tnoms.Add(eComponent.ComponentDnMainTnom);
					tscrs.Add(eComponent.ComponentDnMainTscr);
				}
			}
			if (eComponent.ComponentBranchOd != null)
			{
				if (!IsDecimalInList((decimal)eComponent.ComponentBranchOd, ods))
				{
					ods.Add((decimal)eComponent.ComponentBranchOd);
					tnoms.Add(eComponent.ComponentBranchTnom);
					tscrs.Add(eComponent.ComponentBranchTscr);
				}
			}
		}

		private bool IsDecimalInList(decimal value, List<decimal> list)
		{
			foreach (decimal d in list)
			{
				if (value == d) return true;
			}
			return false;
		}



	}
}
