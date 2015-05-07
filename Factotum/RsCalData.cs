using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using DowUtils;


namespace Factotum
{
	class RsCalData : ReportSection
	{
		public RsCalData(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
		}

		public override bool IsIncluded() 
		{
			return (rpt.eDset != null);
		}

		// Don't include unless we can fit it all..
		public override bool CanFitSome(PrintPageEventArgs args, float Y)
		{
			int padding = 5;
			int tablePadding = 2;
			int calTimeRows = getCalTimeRows();
			// Add one to calTimeRows for the Crew Dose...
			int maxRows = calTimeRows + 1 > 9 ? calTimeRows + 1 : 9;
			return (args.MarginBounds.Bottom - Y - rpt.footerHeight >
				rpt.regTextFont.Height * maxRows + rpt.boldRegTextFont.Height +
				padding * 3 + tablePadding * (maxRows + 2));
		}

		public override bool Print(PrintPageEventArgs args, float Y) 
		{
			// Todo: use measurement graphics for row height?
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			int leftX = args.MarginBounds.X;
			int centerX = (int)(leftX + args.MarginBounds.Width / 2);
			int rightX = leftX + args.MarginBounds.Width;
			int curX = leftX;

			int padding = 5;
			int tblWidth = 225;
			float startY = Y + padding;
			float curY = startY;
			float maxY = curY;
			int specialCalDataCount = 0;

			int tablePadding = 2;

			string[,] calData1;
			string[,] calData2;
			string[,] spCalData;
			string[,] calTimes;

			GetDataArrays(out calData1, out calData2, out spCalData, out calTimes, out specialCalDataCount);
			if (specialCalDataCount > 0)
			{
				maxY = DrawTable(calData1, 10, 2, 1, 0, g, curX, curY, 125, 100, tablePadding, rpt.boldSmallTextFont, rpt.smallTextFont, false, false);

				curX = centerX - tblWidth / 2;
				DrawTable(spCalData, specialCalDataCount, 2, 1, 0, g, curX, curY, 125, 100, tablePadding, rpt.boldSmallTextFont, rpt.smallTextFont, false, false);
			}
			else
			{
				maxY = DrawTable(calData1, 6, 2, 1, 0, g, curX, curY, 125, 100, tablePadding, rpt.boldSmallTextFont, rpt.smallTextFont, false, false);

				curX = centerX - tblWidth / 2;
				DrawTable(calData2, 5, 2, 1, 0, g, curX, curY, 125, 100, tablePadding, rpt.boldSmallTextFont, rpt.smallTextFont, false, false);
			}

			curX = rightX - tblWidth;
			curY = DrawTable(calTimes, calTimes.GetUpperBound(0), 2, 1, 0, g, curX, curY, 125, 100, tablePadding, rpt.boldSmallTextFont, rpt.smallTextFont, false, false);

			// Put a little extra space before the crew dose box.
			curY += tablePadding * 3;
			int? crewDose = rpt.eDset.DsetCrewDose;
			string s = "Crew Dose: " + (crewDose == null ? "N/A" : crewDose.ToString() + " mRem");

			curY = DrawStringInBox(s, g, curX, curY, 225, tablePadding, rpt.regTextFont);
			if (curY > maxY) maxY = curY;

			curY = maxY + padding;
			hr(args, curY);
			curY += 2;
			hr(args, curY);
			this.Y = curY;

			return true;
		}

		private int getCalTimeRows()
		{
			int i;
			int calTimeRows;
			EInspectionPeriodCollection inspectPeriods = EInspectionPeriod.ListForDset(rpt.eDset.ID);

			calTimeRows = inspectPeriods.Count > 0 ? 1 : 0; // Heading row

			for (i = 0; i < inspectPeriods.Count; i++)
			{
				calTimeRows += 2; // In and Out

				if (inspectPeriods[i].InspectionPeriodCalCheck1At != null) calTimeRows++;
				if (inspectPeriods[i].InspectionPeriodCalCheck2At != null) calTimeRows++;
			}
			return calTimeRows+1; // Need to add one extra row for the heading!
		}

		private void GetDataArrays(out string[,] calData1, out string[,] calData2, 
			out string[,] spCalData, out string[,] calTimes, out int specialCalDataCount)
		{
			int row, col;
			int inspectPeriodCount;
			int i;
			int calTimeRows;
			string componentTemp = "N/A", calBlockTemp = "N/A";
			string coarseRange = "N/A", gain = "N/A", couplantType = "N/A";
			string batch = "N/A", velocity = "N/A", loCalThickness = "N/A", hiCalThickness = "N/A";
			string calInTime = "N/A", calOutTime = "N/A";
			string[] calCheckTimes = new string[2];
			int calChecks = 0;

			calData1 = null;
			calData2 = null;
			spCalData = null;
			calTimes = null;
			
			// Main calibration data
			if (rpt.eDset.DsetCompTemp != null)
			{
				componentTemp = rpt.eDset.DsetCompTemp.ToString() + " ° F";
			}
			if (rpt.eDset.DsetCalBlockTemp != null)
			{
				calBlockTemp = rpt.eDset.DsetCalBlockTemp.ToString() + " ° F";
			}
			if (rpt.eDset.DsetRange != null)
			{
				coarseRange = Util.GetFormattedDecimal_NA(rpt.eDset.DsetRange) + " in";
			}
			if (rpt.eDset.DsetGainDb != null)
			{
				gain = Util.GetFormattedDecimal_NA(rpt.eDset.DsetGainDb,1) + " dB";
			}
			if (rpt.eOutage.OutageCouplantTypeName != null)
			{
				couplantType = rpt.eOutage.OutageCouplantTypeName;
			}
			if (rpt.eOutage.OutageCouplantBatch != null)
			{
				batch = rpt.eOutage.OutageCouplantBatch;
			}
			if (rpt.eDset.DsetVelocity != null)
			{
				velocity = Util.GetFormattedDecimal_NA(rpt.eDset.DsetVelocity,4) + " in/µs";
			}
			if (rpt.eDset.DsetThin != null)
			{
				loCalThickness = Util.GetFormattedDecimal_NA(rpt.eDset.DsetThin) + " in";
			}
			if (rpt.eDset.DsetThick != null)
			{
				hiCalThickness = Util.GetFormattedDecimal_NA(rpt.eDset.DsetThick) + " in";
			}

			EInspectionPeriodCollection inspectPeriods = EInspectionPeriod.ListForDset(rpt.eDset.ID);
			inspectPeriodCount = inspectPeriods.Count;

			calTimeRows = getCalTimeRows();  // value returned includes the extra row for the heading...

			calTimes = new string[calTimeRows, 2];

			ESpecialCalValueCollection specialCalValues = 
				ESpecialCalValue.ListForDsetByReportOrder(rpt.eDset.ID);

			// Todo: Fill Ultrasonic (special) calibration data if we have it...
			specialCalDataCount = specialCalValues.Count;
			if (specialCalDataCount > 0)
			{
				// add one for the title
				specialCalDataCount++;
				calData1 = new string[10, 2];
				spCalData = new string[specialCalDataCount, 2];
			}
			else
			{
				calData1 = new string[6, 2];
				calData2 = new string[5, 2];
			}

			// Cal Data Table
			row = 0;
			// Heading row
			col = 0;
			calData1[row, col] = "Calibration Data";

			row++;
			// Component Temp
			col = 0;
			calData1[row, col] = "Component Temp.";
			col++;
			calData1[row, col] = componentTemp;

			row++;
			// Cal out
			col = 0;
			calData1[row, col] = "Cal. Block Temp.";
			col++;
			calData1[row, col] = calBlockTemp;

			row++;
			// Coarse Range
			col = 0;
			calData1[row, col] = "Coarse Range";
			col++;
			calData1[row, col] = coarseRange;

			row++;
			// Low Calibration Thickness
			col = 0;
			calData1[row, col] = "Low Cal. Thickness";
			col++;
			calData1[row, col] = loCalThickness;

			row++;
			// High Calibration Thickness
			col = 0;
			calData1[row, col] = "High Cal. Thickness";
			col++;
			calData1[row, col] = hiCalThickness;
			row++;

			if (specialCalDataCount > 0)
			{
				// We'll put the special cal data in the middle, so keep this info on the left
				// Velocity
				col = 0;
				calData1[row, col] = "Velocity";
				col++;
				calData1[row, col] = velocity;

				row++;
				// Gain
				col = 0;
				calData1[row, col] = "Gain";
				col++;
				calData1[row, col] = gain;

				row++;
				// Couplant Type
				col = 0;
				calData1[row, col] = "Couplant Type";
				col++;
				calData1[row, col] = couplantType;

				row++;
				// Couplant Batch
				col = 0;
				calData1[row, col] = "Couplant Batch";
				col++;
				calData1[row, col] = batch;

				row = 0;
				col = 0;
				spCalData[row, col] = "Special Calibration Data";
				row++;
				foreach (ESpecialCalValue spcal in specialCalValues)
				{
					col = 0;
					spCalData[row, col] = spcal.SpecialCalValueParameterName;
					col++;
					spCalData[row, col] = spcal.SpecialCalValueValue + " " + spcal.SpecialCalValueUnits;
					row++;
				}
			}
			else
			{
				// No special cal data, so put the rest of the regular cal data in the middle
				row = 0;
				// Heading row
				col = 0;
				calData2[row, col] = "Calibration Data -- Cont'd.";

				row++;
				// Velocity
				col = 0;
				calData2[row, col] = "Velocity";
				col++;
				calData2[row, col] = velocity;

				row++;
				// Gain
				col = 0;
				calData2[row, col] = "Gain";
				col++;
				calData2[row, col] = gain;

				row++;
				// Couplant Type
				col = 0;
				calData2[row, col] = "Couplant Type";
				col++;
				calData2[row, col] = couplantType;

				row++;
				// Couplant Batch
				col = 0;
				calData2[row, col] = "Couplant Batch";
				col++;
				calData2[row, col] = batch;

			}

			// Calibration Times
			row = 0;

			// Heading row
			col = 0;
			calTimes[row, col] = "Calibration Times";

			// Add a loop here to cover all times


			for (i=0; i < inspectPeriodCount; i++) 
			{
				EInspectionPeriod firstInspPeriod = inspectPeriods[i];

				if (firstInspPeriod.InspectionPeriodInAt != null)
					calInTime = string.Format("{0:MM-dd-yyyy H:mm}",
							firstInspPeriod.InspectionPeriodInAt);
				if (firstInspPeriod.InspectionPeriodOutAt != null)
					calOutTime = string.Format("{0:MM-dd-yyyy H:mm}",
							firstInspPeriod.InspectionPeriodOutAt);

				calChecks = 0;
				if (firstInspPeriod.InspectionPeriodCalCheck1At != null)
					calCheckTimes[calChecks++] = string.Format("{0:MM-dd-yyyy H:mm}",
						firstInspPeriod.InspectionPeriodCalCheck1At);
				if (firstInspPeriod.InspectionPeriodCalCheck2At != null)
					calCheckTimes[calChecks++] = string.Format("{0:MM-dd-yyyy H:mm}",
						firstInspPeriod.InspectionPeriodCalCheck2At);
			
				row++;
				// Cal in
				col = 0;
				calTimes[row, col] = "Initial Cal. In Time";
				col++;
				calTimes[row, col] = calInTime;

				for (int t = 0; t < calChecks; t++)
				{
					row++;
					// Component Temp
					col = 0;
					calTimes[row, col] = "Cal. Verification";
					col++;
					calTimes[row, col] = calCheckTimes[t];
				}

				row++;
				// Cal out
				col = 0;
				calTimes[row, col] = "Final Cal. Out Time";
				col++;
				calTimes[row, col] = calOutTime;
			}

		// end caltimes loop.
		}

	}
}
