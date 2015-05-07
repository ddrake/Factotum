using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using DowUtils;


namespace Factotum
{
	class RsPartition : ReportSection
	{
		public RsPartition(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
		}

		public override bool IsIncluded() 
		{
			return (rpt.eInspection != null && rpt.eInspection.InspectionHasGrid &&
				rpt.eGrid.GridStartRow != null & rpt.eGrid.GridStartCol != null &&
				rpt.eGrid.GridEndRow != null & rpt.eGrid.GridEndCol != null);
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			// If we can't fit the whole thing, put it on the next page.
			int padding = 5;
			int tablePadding = 2;
			int partitions = getPartitionCount();
			return (args.MarginBounds.Bottom - Y - rpt.footerHeight > 
				rpt.regTextFont.Height * partitions + rpt.boldRegTextFont.Height + 
				padding * 3 + tablePadding * (partitions + 1));
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

			// Always include rows for U/S Ext, U/S Main, D/S Ext
			int partitionCount = getPartitionCount();
			string[,] partitions;
			
			Font curFont = rpt.regTextFont;

			// Get the partition table.
			if (!GetPartitionArray(partitionCount, out partitions, out rows))
			{
				g.DrawString("No Partition Information", curFont, Brushes.Black, leftX, curY);
			}
			else
			{
				// Put the title left-aligned.
				g.DrawString("Partition Information", curFont, Brushes.Black, leftX, curY);
				curY += curFont.Height + padding;

				// Put the partition table left-aligned
				cols = 5; colHeadRows = 1; rowHeadCols = 1;
				headColWidth = 75; dataColWidth = 40; tablePadding = 2;

				curY = DrawTable(partitions, rows, cols, colHeadRows, rowHeadCols, g, curX,
					curY, headColWidth, dataColWidth, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, true, true);

				if (curY > maxY) maxY = curY;
			}

			// Insert the appropriate graphic based on the column layout for the grid.
			curX = 305;
			curY = startY;
			if (!rpt.eGrid.GridHideColumnLayoutGraphic)
			{
				g.DrawImage((rpt.eGrid.GridIsColumnCCW ?
					Properties.Resources.orientation_ccw : Properties.Resources.orientation_cw), curX, curY, 75, 75);
				curY += 75;
				if (curY > maxY) maxY = curY;
			}

			// The gridInfo table
			string[,] gridInfo = GetGridInfoArray();

			curX = rightX - 400;
			curY = startY;
			curFont = rpt.regTextFont;

			// Put the title.
			string s = "Grid Information";

			g.DrawString(s, curFont, Brushes.Black, curX, curY);
			curY += curFont.Height + padding;

			rows = 3;  cols = 4; colHeadRows = 1; rowHeadCols = 1; tablePadding = 2;

			int[] colWidths = new int[] { 40, 50, 50, 260 };

			// Draw the table
			curY = DrawTable(gridInfo, rows, cols, colHeadRows, rowHeadCols, 
				g, curX,	curY, colWidths, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, true, true);

			if (curY > maxY) maxY = curY;
			
			maxY += padding;
			hr(args, maxY);
			this.Y = maxY;

			return true;
		}

		private bool GetPartitionArray(int partitionCount, out string[,] table, out int rows)
		{
			
			table = new string[partitionCount + 1, 5];
			rows = 0;
			if (rpt.eGrid.GridUpExtStartRow == null && rpt.eGrid.GridUpExtEndRow == null &&
				rpt.eGrid.GridUpMainStartRow == null && rpt.eGrid.GridUpMainEndRow == null &&
				rpt.eGrid.GridDnMainStartRow == null && rpt.eGrid.GridDnMainEndRow == null &&
				rpt.eGrid.GridDnExtStartRow == null && rpt.eGrid.GridDnExtEndRow == null &&
				rpt.eGrid.GridBranchStartRow == null && rpt.eGrid.GridBranchEndRow == null &&
				rpt.eGrid.GridBranchExtStartRow == null && rpt.eGrid.GridBranchExtEndRow == null)
				return false;

			int row, col;
			int gridStartRow = (int)rpt.eGrid.GridStartRow;
			int gridEndRow = (int)rpt.eGrid.GridEndRow;
			int gridStartCol = (int)rpt.eGrid.GridStartCol;
			int gridEndCol = (int)rpt.eGrid.GridEndCol;

			int gridRows = gridEndRow - gridStartRow + 1;
			int gridCols = gridEndCol - gridStartCol + 1;

			// No Heading row

			row = 0;
			// Min row
			col = 0;
			table[row, col] = "";
			col++;
			table[row, col] = "S row";
			col++;
			table[row, col] = "E row";
			col++;
			table[row, col] = "S col";
			col++;
			table[row, col] = "E col";

			if (rpt.eGrid.GridUpExtStartRow != null && rpt.eGrid.GridUpExtEndRow != null)
			{
				row++;
				// U/S Ext row
				col = 0;
				table[row, col] = "U/S Ext";
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridUpExtStartRow);
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridUpExtEndRow);
				col++;
				table[row, col] = formatCol(gridStartCol);
				col++;
				table[row, col] = formatCol(gridEndCol);
			}

			if (rpt.eGrid.GridUpMainStartRow != null && rpt.eGrid.GridUpMainEndRow != null)
			{
				row++;
				// U/S Main Row
				col = 0;
				table[row, col] = (
					rpt.eGrid.GridDnMainStartRow != null && rpt.eGrid.GridDnMainEndRow != null ? 
					"U/S Main" : "Main");
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridUpMainStartRow);
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridUpMainEndRow);
				col++;
				table[row, col] = formatCol(gridStartCol);
				col++;
				table[row, col] = formatCol(gridEndCol);
			}

			if (rpt.eGrid.GridDnMainStartRow != null && rpt.eGrid.GridDnMainEndRow != null)
			{
				row++;
				// D/S Main
				col = 0;
				table[row, col] = "D/S Main";
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridDnMainStartRow);
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridDnMainEndRow);
				col++;
				table[row, col] = formatCol(gridStartCol);
				col++;
				table[row, col] = formatCol(gridEndCol);
			}
			if (rpt.eGrid.GridDnExtStartRow != null && rpt.eGrid.GridDnExtEndRow != null)
			{
				row++;
				// D/S Ext
				col = 0;
				table[row, col] = "D/S Ext";
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridDnExtStartRow);
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridDnExtEndRow);
				col++;
				table[row, col] = formatCol(gridStartCol);
				col++;
				table[row, col] = formatCol(gridEndCol);
			}

			if (rpt.eGrid.GridBranchStartRow != null && rpt.eGrid.GridBranchEndRow != null)
			{
				row++;
				// Branch
				col = 0;
				table[row, col] = "Branch";
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridBranchStartRow);
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridBranchEndRow);
				col++;
				table[row, col] = formatCol(gridStartCol);
				col++;
				table[row, col] = formatCol(gridEndCol);
			}

			if (rpt.eGrid.GridBranchExtStartRow != null && rpt.eGrid.GridBranchExtEndRow != null)
			{
				row++;
				// Branch Ext
				col = 0;
				table[row, col] = "Br. Ext";
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridBranchExtStartRow);
				col++;
				table[row, col] = formatRow(rpt.eGrid.GridBranchExtEndRow);
				col++;
				table[row, col] = formatCol(gridStartCol);
				col++;
				table[row, col] = formatCol(gridEndCol);
			}
			rows = row+1;
			return true;
		}

		private string[,] GetGridInfoArray()
		{
			// First make a 2D string array to contain all the data that we want to present
			string[,] table = new string[3, 4];
			string xy;
			// Heading row
			int row = 0;
			int col = 0;
			int gridStartRow = (int)rpt.eGrid.GridStartRow;
			int gridEndRow = (int)rpt.eGrid.GridEndRow;
			int gridStartCol = (int)rpt.eGrid.GridStartCol;
			int gridEndCol = (int)rpt.eGrid.GridEndCol;

			int gridRows = gridEndRow - gridStartRow + 1;
			int gridCols = gridEndCol - gridStartCol + 1;

			table[row, col] = "";
			col++;
			table[row, col] = "Size";
			col++;
			table[row, col] = "Ct.";
			col++;
			table[row, col] = "Zero Reference";

			row++;
			col = 0;
			table[row, col] = "Row";
			col++;
			table[row, col] = Util.GetFormattedDecimal_NA(rpt.eGrid.GridAxialDistance);
			col++;
			table[row, col] = Util.GetFormattedInt_NA(gridRows);
			col++;

			if (rpt.eGrid.GridAxialLocOverride == null)
			{
				if (rpt.eGrid.GridAxialDistance == null)
					xy = "N/A - Axial Distance not set";
				else
					// Unless we have an override set, the start location is 3 grid bands (2 grid widths) U/S Weld 1
					// Note: the first grid band is just slightly U/S of the weld.
					xy = String.Format("{0}\" U/S Weld 1", 2 * rpt.eGrid.GridAxialDistance);
			}
			else xy = rpt.eGrid.GridAxialLocOverride;
			
			table[row, col] = xy;

			row++;
			col = 0;
			table[row, col] = "Col";
			col++;
			table[row, col] = Util.GetFormattedDecimal_NA(rpt.eGrid.GridRadialDistance);
			col++;
			table[row, col] = Util.GetFormattedInt_NA(gridCols);
			col++;
			xy = rpt.eGrid.GridRadialLocation;
			if (xy == null) xy = "N/A";
			table[row, col] = xy;

			return table;
		}

		private int getPartitionCount()
		{
			// Always include rows for U/S Ext, U/S Main, D/S Ext
			int partitionCount = 3;
			// add one for D/S Main
			if (rpt.eComponent.ComponentHasDs) partitionCount++;
			// add for Branch and BranchExt
			if (rpt.eComponent.ComponentHasBranch) partitionCount += 2;
			return partitionCount;
		}

		private string formatRow(short? i)
		{
			if (i == null) return "N/A";
			return (i+1).ToString();
		}
		private string formatCol(short? i)
		{
			if (i == null) return "N/A";
			return EMeasurement.GetColLabel((short)i);
		}
		private string formatCol(int? i)
		{
			if (i == null) return "N/A";
			return EMeasurement.GetColLabel((short)i);
		}

	}
}
