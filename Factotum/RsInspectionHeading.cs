using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;



namespace Factotum
{
	class RsInspectionHeading : ReportSection
	{
		public RsInspectionHeading(MainReport report, ReportSectionRules rules, int subsections) :
			base(report, rules, subsections)
		{
		}

		public override bool IsIncluded() 
		{
			return (rpt.eInspection != null);
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			// Except for the first page, this section always begins a new page.
			// This logic is handled in the main report.
			return true;
		}

		public override bool Print(PrintPageEventArgs args, float Y) 
		{
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			float curX = args.MarginBounds.X;

			int leftX = args.MarginBounds.X;
			int centerX = (int)(curX + args.MarginBounds.Width/2);
			int rightX = args.MarginBounds.X + args.MarginBounds.Width;
			string s;
			int padding = 5;
			float startY = Y + 2 * padding;
			float curY = startY;
			// HORIZONTAL LINE
			hr(args, curY);

			curY += 2;

			Font curFont = rpt.bigTextFont;

			// INSPECTION NAME
            int gridMinCount = 0;
            int nonGridMinCount = 0;
            if (rpt.eGrid != null)
			{
				gridMinCount = (rpt.eGrid.GridBelowTscr == null ? 0 : (int)rpt.eGrid.GridBelowTscr);
			}
            nonGridMinCount = rpt.eInspection.NonGridBelowTscreen;
			s = rpt.eInspection.InspectionName;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			if (gridMinCount > 0 || nonGridMinCount > 0)
			{
				curX += g.MeasureString(s, curFont).Width + 5;
				s = "(Includes readings below Tscreen)";
				g.DrawString(s, curFont, Brushes.Red, new Point((int)curX, (int)curY));
			}
			curY += g.MeasureString(s, curFont).Height;

			this.Y = curY;
			return true;
		}

	}
}
