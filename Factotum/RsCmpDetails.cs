using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using DowUtils;


namespace Factotum
{
	class RsCmpDetails : ReportSection
	{
		public RsCmpDetails(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
		}

		public override bool IsIncluded() 
		{
			return true;
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			return true;
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

			// Get the dimensions table.
			int rows;
			int cols=0;
			string[,] dimensions = GetDimensionsArray(out rows);
			string[,] chrome;

			Font curFont = rpt.regTextFont;

			// Put the Schedule left-aligned .
			string s = rpt.eComponent.ComponentSchedule == null ?
				"Schedule: N/A" : rpt.eComponent.ComponentSchedule;
			g.DrawString(s, curFont, Brushes.Black, leftX, curY);
			curY += curFont.Height + padding;

			// Put the Dimensions table left-aligned
			curY = DrawTable(dimensions, rows, 4, 1, 1, g, leftX, curY, 100, 50, 2, rpt.boldRegTextFont, rpt.regTextFont, true, true);
			if (curY > maxY) maxY = curY;

			// If we have chromium data, get the chromium table and draw it right-justified.
			if (rpt.eComponent.ComponentPctChromeMain != null)
			{
				curY = startY + curFont.Height + padding;
				chrome = GetChromiumArray(out cols);
				curX = rightX - 60 * cols;
				curY = DrawTable(chrome, 2, cols, 1, 1, g, curX, curY, 60, 60, 2, rpt.boldRegTextFont, rpt.regTextFont, true, true);
				if (curY > maxY) maxY = curY;
			}
			else curX = 500;

			curY = startY;
			// Put the Material left-aligned with the chromium table which will follow.
			s = rpt.eComponent.ComponentMaterialName == null ? 
				"Material: N/A" : "Material: " + rpt.eComponent.ComponentMaterialName;
			curFont = rpt.regTextFont;
			g.DrawString(s, curFont, Brushes.Black, curX, curY);
			curY += curFont.Height;
			if (curY > maxY) maxY = curY;

			this.Y = maxY;
			
			return true;
		}

		private string[,] GetDimensionsArray(out int rows)
		{
			// First make a 2D string array to contain all the data that we want to present
			// The following rules apply:  If the downstream main dimensions are null
			// The row header for the first row should read "Main" and no ds main row should be shown, 
			// otherwise the first row header should be "Upstream Main"
			// The branch row should only show if its values are non-null
			// The us ext row should only show if its values do not match the us main row
			// The ds ext row should only show if either the ds main dimensions are null and its values 
			//		don't match the us main values or
			//		if the ds main dimensions are not null and its values don't match the ds main values.
			// The branch ext row should only show if the branch dimensions are not null
			string[,] table = new string[7, 4];

			// Heading row
			int row = 0;
			int col = 0;
			table[row, col] = "Dimensions";
			col++;
			table[row, col] = "OD";
			col++;
			table[row, col] = "Tnom";
			col++;
			table[row, col] = "Tscr";

			// US Main/Main row
			col = 0;
			row++;
			table[row, col] = rpt.eComponent.ComponentDnMainOd == null ? "Main" : "U/S Main";
			col++;
			table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpMainOd);
			col++;
			table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpMainTnom);
			col++;
			table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpMainTscr);

			// DS Main row
			if (rpt.eComponent.ComponentDnMainOd != null)
			{
				col = 0;
				row++;
				table[row, col] = "D/S Main";
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentDnMainOd);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentDnMainTnom);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentDnMainTscr);
			}

			// Branch row
			if (rpt.eComponent.ComponentBranchOd != null)
			{
				col = 0;
				row++;
				table[row, col] = "Branch";
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentBranchOd);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentBranchTnom);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentBranchTscr);
			}

			// Upstream extension row
			if (rpt.eComponent.ComponentUpExtOd != rpt.eComponent.ComponentUpMainOd ||
				rpt.eComponent.ComponentUpExtTnom != rpt.eComponent.ComponentUpMainTnom ||
				rpt.eComponent.ComponentUpExtTscr != rpt.eComponent.ComponentUpMainTscr
				)
			{
				col = 0;
				row++;
				table[row, col] = "U/S Ext.";
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpExtOd);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpExtTnom);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpExtTscr);
			}

			// Downstream extension row
			if ((rpt.eComponent.ComponentDnMainOd == null &&
					(rpt.eComponent.ComponentDnExtOd != rpt.eComponent.ComponentUpMainOd ||
						rpt.eComponent.ComponentDnExtTnom != rpt.eComponent.ComponentUpMainTnom ||
						rpt.eComponent.ComponentDnExtTscr != rpt.eComponent.ComponentUpMainTscr)) ||
				 (rpt.eComponent.ComponentDnMainOd != null &&
					(rpt.eComponent.ComponentDnExtOd != rpt.eComponent.ComponentDnMainOd ||
						rpt.eComponent.ComponentDnExtTnom != rpt.eComponent.ComponentDnMainTnom ||
						rpt.eComponent.ComponentDnExtTscr != rpt.eComponent.ComponentDnMainTscr)))
			{
				col = 0;
				row++;
				table[row, col] = "D/S Ext.";
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentDnExtOd);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentDnExtTnom);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentDnExtTscr);
			}
			// Branch extension row
			if (rpt.eComponent.ComponentBranchOd != null &&
					(rpt.eComponent.ComponentBrExtOd != rpt.eComponent.ComponentBranchOd ||
					rpt.eComponent.ComponentBrExtTnom != rpt.eComponent.ComponentBranchTnom ||
					rpt.eComponent.ComponentBrExtTscr != rpt.eComponent.ComponentBranchTscr))
			{
				col = 0;
				row++;
				table[row, col] = "Branch Ext.";
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpExtOd);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpExtTnom);
				col++;
				table[row, col] = Util.GetFormattedDecimal_NA(rpt.eComponent.ComponentUpExtTscr);
			}
			rows = row+1;
			return table;
		}
		private string[,] GetChromiumArray(out int cols)
		{
			// First make a 2D string array to contain all the data that we want to present
			// Before calling this function, make sure that Chromium data exists.
			string[,] table = new string[2, 6];

			// Flag whether the component has a branch
			bool hasBranch = rpt.eComponent.ComponentBranchOd != null;

			// Heading row
			int row = 0;
			int col = 0;
			table[row, col] = "";
			col++;
			table[row, col] = "Main";
			col++;
			table[row, col] = "U/S Ext.";
			col++;
			table[row, col] = "D/S Ext.";

			// Only show branch data if it has a branch.
			if (hasBranch)
			{
				col++;
				table[row, col] = "Branch";
				col++;
				table[row, col] = "Br. Ext.";
			}
			// Chromium row
			col = 0;
			row++;
			table[row, col] = "% Cr";
			col++;
			table[row, col] = Util.GetFormattedDecimal_Percent_NA(rpt.eComponent.ComponentPctChromeMain);
			col++;
			table[row, col] = Util.GetFormattedDecimal_Percent_NA(rpt.eComponent.ComponentPctChromeUsExt);
			col++;
			table[row, col] = Util.GetFormattedDecimal_Percent_NA(rpt.eComponent.ComponentPctChromeDsExt);

			// Only show branch data if it has a branch.
			if (hasBranch)
			{
				col++;
				table[row, col] = Util.GetFormattedDecimal_Percent_NA(rpt.eComponent.ComponentPctChromeBranch);
				col++;
				table[row, col] = Util.GetFormattedDecimal_Percent_NA(rpt.eComponent.ComponentPctChromeBrExt);
			}
			cols = col + 1;
			return table;
		}
	}
}
