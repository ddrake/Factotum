using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;



namespace Factotum
{
	class RsGraphicNotes : ReportSection
	{
		private int notesPosition;

		public RsGraphicNotes(MainReport report, ReportSectionRules rules, int subsections) :
			base(report, rules, subsections)
		{
			notesPosition = 0;
		}

		public override bool IsIncluded() 
		{
			return (rpt.eInspection != null && 
				(rpt.eInspection.InspectionHasGraphic || rpt.eInspection.InspectionHasGrid));
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
			float startY = Y + padding;
			float curY = startY;
			float maxY = startY;
			float notesHeight;
			bool hasGraphic = rpt.eInspection.InspectionHasGraphic;
			// Either we are printing this section for the first time or we've already printed
			// the graphic and are finishing up the text.
			bool graphicDone = this.notesPosition > 0;
			bool sectionDone = false;

			// GRAPHIC IMAGE
			// If we have a graphic image, and haven't drawn it yet, draw it.
			if (hasGraphic && !graphicDone)
			{
				g.DrawImage(rpt.eGraphic.GraphicImage, leftX, curY,480,360);
				maxY += 360 + padding;
			}

			// NOTES
			if (rpt.eInspection.InspectionNotes == null)
			{
				curY = maxY;
				sectionDone = true;
				goto Finish;
			}
			//	If we have notes, fill the area available with text, beginning at the current
			// position in the notes string.  
			else
			{
				// set the font
				Font curFont = rpt.regTextFont;
				bool notesDone = false;
				int textPosition = 0;
				int linesFit = 0;
				Rectangle rec;
				// get the text string (or what's left of it)
				s = rpt.eInspection.InspectionNotes.Substring(this.notesPosition);
				
				// construct the rectangle to the right of the graphic
				if (hasGraphic && !graphicDone)
				{
					// If we just added a graphic, first fill the area to the right of the graphic, 
					// then fill the area below it until all the text is done or the page is full. 
					rec = new Rectangle(leftX + 480 + padding, (int)startY, 
						args.MarginBounds.Width - 480 - padding, 360);

					notesDone = DispenseText(g, measure, rec, Brushes.Black, s, curFont, 
						out textPosition, out linesFit, out notesHeight);

					if (notesDone)
					{
						// the notes fit in the rectangle to the right of the graphic
						curY = maxY;
						sectionDone = true;
						goto Finish;
					}
					else
					{
						// continue the notes below the graphic until all the text is done or the 
						// page is full
						int remainingHeight = (int)(args.MarginBounds.Height - rpt.footerHeight - (360 + padding));
						if (remainingHeight < curFont.Height)
						{
							this.notesPosition = textPosition;
							return false;
						}
						curY = maxY;
						rec = new Rectangle(leftX, (int)curY, args.MarginBounds.Width, 
							(int)(args.MarginBounds.Bottom - curY - rpt.footerHeight));

						notesDone = DispenseText(g, measure, rec, Brushes.Black, 
							s.Substring(textPosition), curFont, out textPosition, out linesFit, out notesHeight);
						if (notesDone)
						{
							// we were able to fit the rest of the notes on the page
							curY += notesHeight;
							sectionDone = true;
							goto Finish;
						}
						else 
						{
							// the notes didn't all fit on the page
							this.notesPosition = textPosition;
							sectionDone = false;
							goto Finish;
						}
					}
				}
				else
				{
					// No Graphic or we added the graphic on a prior page, so we can use the whole width.
					int remainingHeight = (int)(args.MarginBounds.Bottom - startY - rpt.footerHeight);
					if (remainingHeight < curFont.Height)
					{
						// We can't fit even a single line
						this.notesPosition = textPosition;
						sectionDone = false;
						goto Finish;
					}
					// We can fit some, so just fill as much as we can on the page.  
					rec = new Rectangle(leftX, (int)startY, args.MarginBounds.Width, remainingHeight);

					notesDone = DispenseText(g, measure, rec, Brushes.Black, 
						s.Substring(textPosition), curFont, out textPosition, out linesFit, out notesHeight);
					if (notesDone)
					{
						// we were able to fit the rest of the notes on the page
						curY = startY + notesHeight + padding;
						sectionDone = true;
						goto Finish;
					}
					else 
					{
						// the notes didn't all fit on the page
						this.notesPosition = textPosition;
						sectionDone = false;
						goto Finish;
					}
				}
			}
			Finish:
			if (sectionDone)
			{
				curY += padding;
				hr(args, curY);
				this.Y = curY;
			}
			return sectionDone;
			
		}

		public bool DispenseText(Graphics target, Graphics measurer, RectangleF r,
			Brush brsh, string text, Font fnt, out int textPosition, out int linesFit, out float height)
		{
			if (r.Height < fnt.Height)
				throw new ArgumentException(
				"The rectangle is not tall enough to fit a single line of text inside.");
			int charsFit = 0;
			int cut = 0;
			linesFit = 0;
			textPosition = 0;
			string temp = text;

			//measure how much of the string we can fit into the rectangle
			StringFormat format = new StringFormat(StringFormatFlags.FitBlackBox |
				StringFormatFlags.LineLimit);

			SizeF stringSize = new SizeF();
			stringSize = measurer.MeasureString(temp, fnt, r.Size, format, out charsFit, out linesFit);
			height = stringSize.Height;

			// Get the portion of the string that will fit
			cut = BreakText(temp, charsFit);
			if (cut != charsFit) temp = temp.Substring(0, cut);
			temp = temp.Trim(' ');

			// Draw the string
			target.DrawString(temp, fnt, brsh, r, format);

			textPosition += cut;
			if (textPosition == text.Length)
			{
				textPosition = 0; //reset the location so we can repeat the document
				return true; //finished printing
			}
			else
				return false;
		}

		private static int BreakText(string text, int approx)
		{
			if (approx == 0)
				throw new ArgumentException();
			if (approx < text.Length)
			{
				//are we in the middle of a word?
				if (char.IsLetterOrDigit(text[approx]) &&
				char.IsLetterOrDigit(text[approx - 1]))
				{
					int temp = text.LastIndexOf(' ', approx, approx + 1);
					if (temp >= 0)
						return temp;
				}
			}
			return approx;
		}


	}
}
