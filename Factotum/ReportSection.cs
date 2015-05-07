using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;

namespace Factotum
{
	// Base class for report sections.
	// Each report section needs to be able to
	// 1. Figure out whether or not it should be included in the report
	// 2. Figure out whether or not it can fit any of itself on the current page
	// 3. Print itself (or part of itself) on the current page, returning true if it completes and 
	//		the current Y location on the page after printing.
	// 4. Keep track of its current row for each subsection. 

	// Each report section should add whatever whitespace it needs before printing itself
	// but not add whitespace after itself.
	class ReportSection
	{
		// The main report to which this section belongs
		protected MainReport rpt;

		// The ID of the current inspection
		// The rules to apply to this section
		protected ReportSectionRules rules;

		// The number of sub-sections
		protected int subsections;

		// The current row for each sub-section
		protected int[] curRow;

		// The current y-position on the page
		public float Y;

		// Constructor
		public ReportSection(MainReport report, ReportSectionRules rules, int subsections)
		{
			this.rpt = report;
			this.rules = rules;
			this.subsections = subsections;
			this.curRow = new int[subsections];
		}

		// Virtual methods to override
		public virtual bool IsIncluded() 
		{
			return true;
		}

		public virtual bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			return true;
		}

		public virtual bool Print(PrintPageEventArgs args, float Y)
		{
			return true;
		}

		// Helper methods

		protected float DrawStringInBox(string s, Graphics g, float X, float Y, 
			int boxWidth, int padding, Font dataFont)
		{
			float curX = X + padding;
			float curY = Y + padding;
			float height = dataFont.Height + padding * 2;
			g.DrawString(BlankForNull(s), dataFont, Brushes.Black, new PointF(curX, curY));
			g.DrawRectangle(Pens.Black, X,Y, boxWidth, height);
			return Y + height;
		}

		// Draws a basic table of data with grid lines and different fonts for row/col headings and data.
		protected float DrawTable(string[,] table, int rows, int cols, int colHeadRows, int rowHeadCols,
			Graphics g, float X, float Y, int rowHeadColWidth, int dataColWidth, int padding,
			Font headingFont, Font dataFont, bool verticalLines, bool horizontalLines)
		{
			int[] colWidths = new int[cols];
			colWidths[0] = rowHeadColWidth;
			for (int c = 1; c < cols; c++)
				colWidths[c] = dataColWidth;

			return DrawTable(table, rows, cols, colHeadRows, rowHeadCols, g, X, Y, colWidths, padding,
				headingFont, dataFont, verticalLines, horizontalLines);
		}

		// Draws a basic table of data with grid lines and different fonts for row/col headings and data.
		protected float DrawTable(string[,] table, int rows, int cols, int colHeadRows, int rowHeadCols,
			Graphics g, float X, float Y, int[] colWidths, int padding,
			Font headingFont, Font dataFont, bool verticalLines, bool horizontalLines)
		{
			// Todo: use measurement graphics for row height?
			// Calculate the table width by adding column widths
			int tableWidth = 0;
			for (int i = 0; i < cols; i++) tableWidth += colWidths[i];

			int colHeadRowHeight = headingFont.Height;
			int dataRowHeight = headingFont.Height;
			int tableHeight = colHeadRowHeight + (rows - 1) * dataRowHeight + 2 * rows * padding;
			float leftX = X;
			float curY = Y;
			float curX = leftX;
			for (int r = 0; r < rows; r++)
			{
				curX = leftX + padding;
				if (horizontalLines || r == 0 || r == colHeadRows)
					g.DrawLine(Pens.Black, leftX, curY, leftX + tableWidth, curY);
				curY += padding;
				for (int c = 0; c < cols; c++)
				{
					Font fnt = (r < colHeadRows || c < rowHeadCols ? headingFont : dataFont);
					g.DrawString(BlankForNull(table[r, c]), fnt, Brushes.Black, new PointF(curX, curY));
					curX += colWidths[c];
				}
				curY += (r < colHeadRows ? colHeadRowHeight : dataRowHeight);
				curY += padding;
			}
			g.DrawLine(Pens.Black, leftX, curY, leftX + tableWidth, curY);

			curY = Y;
			curX = leftX;
			for (int c = 0; c < cols; c++)
			{
				// verticalLines=false suppresses internal verticals.
				if (verticalLines || c == 0)
				{
					g.DrawLine(Pens.Black, curX, curY, curX, curY + tableHeight);
				}
				curX += colWidths[c];
			}
			g.DrawLine(Pens.Black, curX, curY, curX, curY + tableHeight);
			return curY + tableHeight;
		}

		private string BlankForNull(string val)
		{
			return (val == null ? "" : val);
		}

		protected int TableRowsThatWillFitInHeight(float height, int colHeadRows,
			int padding, Font headingFont, Font dataFont)
		{
			return
			(int)((height -	colHeadRows * headingFont.Height - (colHeadRows + 1) * padding) /
				(dataFont.Height + padding));

		}

		// Truncate a string unless it fits in the specified width
		public string Chop(Graphics measure, Font fnt, string s, float width)
		{
			int charsFit;
			int linesFit;
			string cs = s;
			StringFormat format = new StringFormat(StringFormatFlags.FitBlackBox|StringFormatFlags.LineLimit);
			measure.MeasureString(s, fnt, new SizeF(width, 1.5f*fnt.Height), format, out charsFit, out linesFit);
			if (charsFit < s.Length) cs = s.Substring(0, charsFit - 3) + "...";
			return cs;
		}

		public string NaForEmpty(string s)
		{
			if (s == null || s.Length == 0) return "N/A";
			else return s;
		}

		protected void hr(PrintPageEventArgs args, float Y)
		{
			args.Graphics.DrawLine(Pens.Gray, args.MarginBounds.Left, Y, args.MarginBounds.Right, Y); 
		}

		protected string GetInspectorNumber(Guid InspectorID)
		{
			int iCount = rpt.eInspectors.Count;

			for (int i = 0; i < iCount; i++)
			{
				if (InspectorID == rpt.eInspectors[i].ID)
					return "" + (i + 1);

			}
			return "N/A";
		}
	}


	// Class for managing the rules around the various report sections
	// The footer is not considered a report section.
	class ReportSectionRules
	{
		public ReportSectionEnum Section;
		public PageForceMode ForceMode;
		public int MinRowsBeforeForce;
		public ReportSectionRules(ReportSectionEnum section,
			PageForceMode forceMode, int minRowsBeforeForce)
		{
			this.Section = section;
			this.ForceMode = forceMode;
			this.MinRowsBeforeForce = minRowsBeforeForce;
		}
	}

	// Enumeration of page force modes used to determine whether or not a new page is forced before the 
	// section is started. 
	enum PageForceMode
	{
		// force a page before the section unless the section starts on the first page, 
		// regardless of the number of rows in the section.
		AlwaysExcept1stPage,
		// force a page before the section unless the entire section will fit on the page.
		UnlessAllFits,
		// force a page unless the section contains at least a minimum number of rows
		UnlessMinRowsAnyPage
	}

	// Enumeration of report sections in the order in which they appear on the report.
	// Each report section has a section print method, a height tester, and a set of rules for 
	// controlling its pagination.
	enum ReportSectionEnum :int
	{
		Heading, // logo, facility, outage, title, report #, inspection date, ut proc, grid proc
		CmpDefinition, // component name, work order, area spec, misc1, misc2, line, system
		CmpDetails, // dimensions, type, schedule, material, chromium
		InspectionHeading, // Just the inspection name and a dividing line (if it's the first page)
		GraphicNotes, // Graphic and/or notes
		StatsAndNonGrid, // Statistics and non-grid measurements
		Partition, // Partition info, and grid definition
		Grid, // Data grid
		CrewDoseAndLegend, // Legend, note.
		Instrument, // Meter, Ducer, Cal Block
		CalData, // Calibration Data and times.
		None
	}

}
