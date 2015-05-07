using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;



namespace Factotum
{
	class RsHeading : ReportSection
	{
		public RsHeading(MainReport report, ReportSectionRules rules, int subsections) :
			base(report, rules, subsections)
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
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			int leftX = args.MarginBounds.X;
			int centerX = (int)(leftX + args.MarginBounds.Width/2);
			int rightX = leftX + args.MarginBounds.Width;
			float curX = leftX;

			float curY = Y;
			float startY = Y;
			float maxY = Y;

			SizeF sSize;
			string s;
			int padding = 5;

			// Report Generated by Factotum
			Font genByFont = new Font("Arial", 6);
			g.DrawString("Factotum� Report Generator " + Globals.VersionString, 
				genByFont, Brushes.Black, curX, curY);
			curY += genByFont.Height;

			// LOGO
            Image logo;
            switch (UserSettings.sets.VendorName)
            {
                case "Westinghouse":
                    logo = Properties.Resources.vendor_logo1;
                    break;
                default:
                    logo = Properties.Resources.vendor_logo0;
                    break;
            }
			g.DrawImage(logo, new Point((int)curX, (int)curY));
			curY += logo.Height * 100 / logo.VerticalResolution;
			logo.Dispose();

			Font curFont = rpt.regTextFont;

			// SITE/UNIT
			s = Chop(measure, curFont, NaForEmpty(rpt.eUnit.UnitNameWithSite),250);
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += g.MeasureString(s, curFont).Height;

			// OUTAGE
			s = Chop(measure, curFont, "Outage: " + NaForEmpty(rpt.eOutage.OutageName), 250);
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += g.MeasureString(s, curFont).Height;
			if (curY > maxY) maxY = curY;

			curY = startY;

			// TITLE
			curFont = rpt.titleFont;
			s = "Component Inspection";
			sSize = measure.MeasureString(s, curFont);
			curX = centerX - sSize.Width / 2;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			curFont = rpt.bigTextFont;
			s = "UT for FAC";
			sSize = measure.MeasureString(s, curFont);
			curX = centerX - sSize.Width / 2;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			s = (rpt.eInspectedComponent.InspComponentIsFinal ? "Final " : "Preliminary ") + "Report";
			sSize = measure.MeasureString(s, curFont);
			curX = centerX - sSize.Width / 2;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;
			if (curY > maxY) maxY = curY;

			curX = 550;
			curY = startY;
			curFont = rpt.regTextFont;

			s = Chop(measure, curFont, "Report No: " + rpt.eInspectedComponent.InspComponentName, 250);
			sSize = measure.MeasureString(s, curFont);
			curX = rightX - sSize.Width;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			// The date of the last inspection period if any, otherwise the current date.
			s = Chop(measure, curFont, "Inspected: " + String.Format("{0:d}", rpt.eInspectedComponent.InspComponentInspectedOn), 250);
			sSize = measure.MeasureString(s, curFont);
			curX = rightX - sSize.Width;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			curFont = rpt.smallTextFont;

			s = Chop(measure, curFont, "UT Proc: " + NaForEmpty(rpt.eOutage.OutageCalibrationProcedureName), 250);
			sSize = measure.MeasureString(s, curFont);
			curX = rightX - sSize.Width;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			s = Chop(measure, curFont, "Grid Proc: " + NaForEmpty(rpt.eInspectedComponent.InspComponentGridProcedureName), 250);
			sSize = measure.MeasureString(s, curFont);
			curX = rightX - sSize.Width;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;
			if (curY > maxY) maxY = curY;

			curY = maxY + padding;
			hr(args, curY);
			this.Y = curY;
			return true;
		}

	}
}
