using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using DowUtils;


namespace Factotum
{
	class RsStatsAndNonGrid : ReportSection
	{
		private int startingNonGrid;

		public RsStatsAndNonGrid(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
			startingNonGrid = 0;
		}

		public override bool IsIncluded() 
		{
			return (rpt.eInspection != null && 
				(rpt.eInspection.InspectionTextFilePoints + 
				rpt.eInspection.InspectionEmpties +
				rpt.eInspection.InspectionAdditionalMeasurements) > 0);
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			// If we're finishing up some additional measurements that we started on the
			// previous page, we're on a fresh page so some will fit.
			if (startingNonGrid > 0) return true;

			int padding = 5;
			int tablePadding = 2;
			return (args.MarginBounds.Bottom - Y - rpt.footerHeight > 
				rpt.regTextFont.Height * 3 + rpt.boldRegTextFont.Height + 
				padding * 3 + tablePadding * 4);
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
			float startY = Y + padding;
			float curY = startY;
			float maxY = startY;

			int rows; int cols;
			int colHeadRows; int rowHeadCols;
			int headColWidth; int dataColWidth;
			int tablePadding;

			int nonGridCount = (int)rpt.eInspection.InspectionAdditionalMeasurements;
			Font curFont = rpt.regTextFont;

			if (startingNonGrid == 0)
			{
				// Get the stats tables.
				string[,] stats1 = GetStats1Array();
				string[,] stats2 = GetStats2Array();

				// Put the title left-aligned.
				g.DrawString("Statistics", curFont, Brushes.Black, leftX, curY);
				curY += curFont.Height + padding;

				// Put the stats tables left-aligned side-by-side
				rows = 3; cols = 2; colHeadRows = 0; rowHeadCols = 1;
				headColWidth = 75; dataColWidth = 50; tablePadding = 2;

				DrawTable(stats1, rows, cols, colHeadRows, rowHeadCols, g, curX,
					curY, headColWidth, dataColWidth, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, true, true);

				curX = leftX + headColWidth + dataColWidth;
				curY = DrawTable(stats2, rows, cols, colHeadRows, rowHeadCols, g, curX,
					curY, headColWidth, dataColWidth, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, true, true);

				if (curY > maxY) maxY = curY;
			}

			if (nonGridCount > startingNonGrid)
			{
				// The non-grid data table
				string[,] nonGrids;

                // curX = rightX - 400;
                curX = rightX - 495;
                curY = startY;
				curFont = rpt.regTextFont;

				// Put the title.
				string s = "Additional (Non-Grid) Measurements" +
					(startingNonGrid > 0 ? " Cont'd" : "");

				g.DrawString(s, curFont, Brushes.Black, curX, curY);
				curY += curFont.Height + padding;

				cols = 3; colHeadRows = 1; rowHeadCols = 0; tablePadding = 2;

				// Figure out how many additional measurements will fit on the page.
				// Note: this calculation could become invalid if the DrawTable logic is changed.
				int dataRowsToInclude = TableRowsThatWillFitInHeight(
					args.MarginBounds.Bottom - curY - rpt.footerHeight, colHeadRows,tablePadding,
					rpt.boldRegTextFont, rpt.regTextFont);

				// If we can fit more than we actually have, just include what we have.
				if (dataRowsToInclude > nonGridCount-startingNonGrid) 
					dataRowsToInclude = nonGridCount-startingNonGrid;

				// Get the array with the non-grid measurement data
				nonGrids = GetNonGridsArray(startingNonGrid, dataRowsToInclude);

                //int[] colWidths = new int[] { 100, 50, 250 };
                int[] colWidths = new int[] { 100, 50, 345 };

				// Draw the table
				curY = DrawTable(nonGrids, dataRowsToInclude + 1, cols, colHeadRows, rowHeadCols, 
					g, curX,	curY, colWidths, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, true, true);

				if (curY > maxY) maxY = curY;
				// Set the starting non-grid measurement for next time, in case we couldn't get
				// them all on the page.
				startingNonGrid += dataRowsToInclude;
			}
			maxY += padding;
			hr(args, maxY);
			this.Y = maxY;
			// We're finished if either there were no measurements or we got them all in.
			return nonGridCount == 0 || startingNonGrid >= nonGridCount;
		}

		private string[,] GetStats1Array()
		{
			string[,] table = new string[3, 2];
			int row, col;
			// No Heading row

			row = 0;
			// Min row
			col = 0;
			table[row, col] = "Min";
			col++;
			table[row, col] = Util.GetFormattedFloat_NA(rpt.eInspection.InspectionMinWall,3);

			row++;
			// Max row
			col = 0;
			table[row, col] = "Max";
			col++;
			table[row, col] = Util.GetFormattedFloat_NA(rpt.eInspection.InspectionMaxWall,3);

			row++;
			// Obst row
			col = 0;
			table[row, col] = "Obst";
			col++;
			table[row, col] = rpt.eInspection.InspectionObstructions.ToString();
			return table;
		}

		private string[,] GetStats2Array()
		{
			string[,] table = new string[3, 2];
			int row, col;
			// No Heading row

			row = 0;
			// Mean row
			col = 0;
			table[row, col] = "Mean";
			col++;
			table[row, col] = Util.GetFormattedFloat_NA(rpt.eInspection.InspectionMeanWall,3);

			row++;
			// Std Dev row
			col = 0;
			table[row, col] = "Std Dev";
			col++;
			table[row, col] = Util.GetFormattedFloat_NA(rpt.eInspection.InspectionStdevWall,3);

			row++;
			// Empties row
			col = 0;
			table[row, col] = "Empties";
			col++;
			table[row, col] = rpt.eInspection.InspectionEmpties.ToString();
			return table;
		}

		private string[,] GetNonGridsArray(int startingNonGrid, int rows)
		{
			// First make a 2D string array to contain all the data that we want to present
			// Before calling this function, make sure that Non-grid data exists.
			string[,] table = new string[rows+1, 3];

			// Flag whether the component has a branch
			bool hasBranch = rpt.eComponent.ComponentBranchOd != null;

			// Heading row
			int row = 0;
			int col = 0;
			table[row, col] = "Name";
			col++;
			table[row, col] = "Value";
			col++;
			table[row, col] = "Note";

			EAdditionalMeasurementCollection addMeasures =
				EAdditionalMeasurement.ListByNameForInspection((Guid)rpt.eInspection.ID);
			// Data rows
			for (int i = startingNonGrid; i < startingNonGrid+rows; i++)
			{
				EAdditionalMeasurement addMeasure = addMeasures[i];
				row++;
				col = 0;
				table[row, col] = addMeasure.AdditionalMeasurementName;
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(addMeasure.AdditionalMeasurementThickness);
				col++;
				table[row, col] = addMeasure.AdditionalMeasurementDescription;
			}
			return table;
		}
	}
}
