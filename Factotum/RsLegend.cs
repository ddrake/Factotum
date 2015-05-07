using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;


namespace Factotum
{
	class RsLegend : ReportSection
	{

		public RsLegend(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
		}

		public override bool IsIncluded() 
		{
			return (rpt.eInspection != null && 
				(rpt.eInspection.InspectionTextFilePoints > 0 || rpt.eInspection.InspectionAdditionalMeasurements > 0));
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			int padding = 5;
			int tablePadding = 2;
			return (args.MarginBounds.Bottom - Y - rpt.footerHeight > 
				rpt.regTextFont.Height * 4 + rpt.boldRegTextFont.Height + 
				padding * 3 + tablePadding * 5);
		}

		public override bool Print(PrintPageEventArgs args, float Y) 
		{
			char cLessOrEqual = '\u2264';
			string sLessOrEqual = cLessOrEqual.ToString();

			// Todo: use measurement graphics for row height?
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			int leftX = args.MarginBounds.X;
			int centerX = (int)(leftX + args.MarginBounds.Width / 2);
			int rightX = leftX + args.MarginBounds.Width;
			int curX = leftX;

			int padding = 5;
			int tblWidth = 300;
			int tblLeftX = rightX - tblWidth;
			int[] tabStop = new int[] { tblLeftX, tblLeftX + 50, tblLeftX + 100, tblLeftX + 115, tblLeftX + 180 }; 
			float startY = Y + padding;
			float curY = startY;

			int tablePadding = 2;

			Font curFont = rpt.regTextFont;
			g.DrawString("Note: All Measured Thickness readings (Tmeas) are in inches.",
				curFont, Brushes.Black, curX, curY);
			curY += curFont.Height + padding;
			g.DrawString("Gray bands (if present) represent welds.",
				curFont, Brushes.Black, curX, curY);
			curY += curFont.Height + padding;
			g.DrawString("Blue bands (if present) represent internal component partitions.",
				curFont, Brushes.Black, curX, curY);

			curX = tabStop[0] + tablePadding;
			curY = startY;
			// horizontal line
			g.DrawLine(Pens.Black, tblLeftX, curY, tblLeftX + tblWidth, curY);
			curY += tablePadding;
			g.DrawString("Legend", rpt.boldRegTextFont, Brushes.Black, curX, curY);
			curY += (curFont.Height + tablePadding);
			g.DrawLine(Pens.Black, tblLeftX, curY, tblLeftX + tblWidth, curY);
			curY += tablePadding;
			g.DrawString("Red", rpt.boldRegTextFont, Brushes.Red, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Green", rpt.boldRegTextFont, Brushes.Green, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Black", rpt.boldRegTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Blue", rpt.boldRegTextFont, Brushes.Blue, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawLine(Pens.Black, tblLeftX, curY, tblLeftX + tblWidth, curY);
			g.DrawLine(Pens.Black, tblLeftX, startY, tblLeftX, curY);
			g.DrawLine(Pens.Black, tblLeftX + tblWidth, startY, tblLeftX + tblWidth, curY);

			curX = tabStop[1] + tablePadding;
			curY = startY;
			curY += (curFont.Height + 3 * tablePadding);
			curY += (curFont.Height + tablePadding);
			g.DrawString("Tscreen", rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Tnom", rpt.regTextFont, Brushes.Black, curX, curY);


			curX = tabStop[2] + tablePadding;
			curY = startY;
			curY += (curFont.Height + 3 * tablePadding);
			curY += (curFont.Height + tablePadding);
			g.DrawString(sLessOrEqual, rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("<", rpt.regTextFont, Brushes.Black, curX, curY);


			curX = tabStop[3] + tablePadding;
			curY = startY;
			curY += (curFont.Height + 3 * tablePadding);
			g.DrawString("Tmeas <", rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Tmeas " + sLessOrEqual, rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Tmeas " + sLessOrEqual, rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Tmeas >", rpt.regTextFont, Brushes.Black, curX, curY);

			curX = tabStop[4] + tablePadding;
			curY = startY;
			curY += (curFont.Height + 3 * tablePadding);
			g.DrawString("Tscreen", rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("Tnom", rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("120% of Tnom", rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			g.DrawString("120% of Tnom", rpt.regTextFont, Brushes.Black, curX, curY);

			curY += (curFont.Height + tablePadding);
			curY += padding;
			hr(args, curY);
			this.Y = curY;

			// We're finished if either there were no measurements or we got them all in.
			return true;
		}
	}
}
