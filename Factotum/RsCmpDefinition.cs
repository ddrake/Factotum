using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;


namespace Factotum
{
	class RsCmpDefinition : ReportSection
	{
		public RsCmpDefinition(MainReport report, ReportSectionRules rules, int subsections)
			:
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
			int centerX = (int)(leftX + args.MarginBounds.Width / 2);
			int rightX = leftX + args.MarginBounds.Width;
			float curX = leftX;

			int padding = 5;
			float startY = Y + padding;
			float curY = startY;
			float maxY = startY;

			SizeF sSize;
			string s;

			Font curFont = rpt.regTextFont;

			// COMPONENT ID
			s = Chop(measure, curFont, "Component ID: " + rpt.eComponent.ComponentName, 250);
			sSize = measure.MeasureString(s, curFont);
			curX = leftX;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			// WORK ORDER
			s = Chop(measure, curFont, "WO #: " + rpt.eInspectedComponent.InspComponentWorkOrder, 250);
			sSize = measure.MeasureString(s, curFont);
			curX = leftX;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			if (curY > maxY) maxY = curY;

			// COMPONENT TYPE
			curY = startY;
			s = Chop(measure, curFont, "Type: " + rpt.eComponent.ComponentTypeName, 250);
			sSize = measure.MeasureString(s, curFont);
			curX = centerX - sSize.Width / 2;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			// AREA SPECIFIER
			if (rpt.eInspectedComponent.InspComponentAreaSpecifier != null)
			{
				s = Chop(measure, curFont, rpt.eInspectedComponent.InspComponentAreaSpecifier, 250);
				sSize = measure.MeasureString(s, curFont);
				curX = centerX - sSize.Width / 2;
				g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
				curY += sSize.Height;
			}

			// MISC1
			if (rpt.eComponent.ComponentMisc1 != null)
			{
				s = Chop(measure, curFont, rpt.eComponent.ComponentMisc1, 250);
				sSize = measure.MeasureString(s, curFont);
				curX = centerX - sSize.Width / 2;
				g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
				curY += sSize.Height;
			}

			// MISC2
			if (rpt.eComponent.ComponentMisc2 != null)
			{
				s = Chop(measure, curFont, rpt.eComponent.ComponentMisc2, 250);
				sSize = measure.MeasureString(s, curFont);
				curX = centerX - sSize.Width / 2;
				g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
				curY += sSize.Height;
			}

			if (curY > maxY) maxY = curY;

			curY = startY;
			// LINE
			s = Chop(measure, curFont, "Line: " + NaForEmpty(rpt.eComponent.ComponentLineName), 250);
			sSize = measure.MeasureString(s, curFont);
			curX = rightX - sSize.Width;
			g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
			curY += sSize.Height;

			// SYSTEM
			if (rpt.eComponent.ComponentSystemName != null)
			{
				s = Chop(measure, curFont, "System: " + rpt.eComponent.ComponentSystemName, 250);
				sSize = measure.MeasureString(s, curFont);
				curX = rightX - sSize.Width;
				g.DrawString(s, curFont, Brushes.Black, new Point((int)curX, (int)curY));
				curY += sSize.Height;
			}

			if (curY > maxY) maxY = curY;
			
			curY = maxY + 5;
			hr(args,curY);
			this.Y = curY;

			return true;
		}


	}
}
