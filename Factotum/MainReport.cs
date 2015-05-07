using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Factotum
{
	// An instance of this class is created when the user selects to print an inspected component.
	// The object is maintained throughout the printing process.  Each time a PrintPage event is received,
	// the report object's OutputToGraphics method is called with the event's arguments, more report data
	// is painted to the graphics, and the HasMorePages property of the graphics is set.

	// Each time the QueryPageSettings event is received (just before the PrintPage event), 
	// the NextPageLandscape property of the report object is checked and the page orientation set 
	// accordingly.  This property will only be set by the report object if the next page(s) will contain
	// a very wide grid.  
	class MainReport
	{
		// These objects are loaded in the constructor
		public EInspectedComponent eInspectedComponent;
		public EComponent eComponent;
		public EUnit eUnit;
		public EOutage eOutage;
		public EInspectorCollection eInspectors;

		// If the Inspected component has at least one inspection, the collection is filled in the constructor
		public EInspectionCollection eInspections;
		public int curInspectionIdx;
		public EInspection eInspection; // the current inspection

		// If the Inspection has at least one dataset, the collection is filled in the constructor
		public EDsetCollection eDsets;
		public int curDsetIdx;		
		public EDset eDset; // the current dataset
		
		public EGraphic eGraphic;
		public EGrid eGrid;
		// As we output report data to a provided graphic object, we increment curReportSection
		// each time we finish a section.  We increment curRow and curCol each time we output 
		// part of a section.  We reset them to zero when a section is complete.  If a section is broken
		// across pages, we use curRow and curCol to figure out how to pick up where we left off.
		// curCol would only be non-zero after a page break if we were printing a grid with more columns than
		// would fit on a page.
		private ReportSectionEnum curReportSection;
		public bool nextPageLandscape;
		private PrintDocument printDocument = null;
		public Font titleFont;
		public Font bigTextFont;
		public Font smallTextFont;
		public Font regTextFont;
		public Font boldRegTextFont;
		public Font boldSmallTextFont;
		public Font italicTextFont;
		public int footerHeight;
		private int footerRowPadding;
		public int curPageNumber;

		private string[,] signerInfo;
		private int signers;

		private ReportSection[] sections;

		// Report constructor
		public MainReport(Guid InspectedComponentID)
		{
			// Assemble the source data for the report
			eInspectedComponent = new EInspectedComponent(InspectedComponentID);
			eComponent = new EComponent(eInspectedComponent.InspComponentCmpID);
			eInspections = EInspection.ListByReportOrderForInspectedComponent(
				(Guid)eInspectedComponent.ID, false);
			eUnit = new EUnit(eComponent.ComponentUntID);
			eOutage = new EOutage(eInspectedComponent.InspComponentOtgID);
			eInspectors = EInspector.ListForInspectedComponent((Guid)eInspectedComponent.ID,true);
				
			// Use this for the report title only
			titleFont = new Font("Times New Roman",14);
			// Used for site/unit, outage, rept#, insp date, ut proc, grid proc
			bigTextFont = new Font("Times New Roman",12);
			// Not currently used
			smallTextFont = new Font("Times New Roman", 8);
			// Used for all other text exept headings and footnotes
			regTextFont = new Font("Times New Roman", 9);
			// Used for headings
			boldRegTextFont = new Font(regTextFont, FontStyle.Bold);
			boldSmallTextFont = new Font(smallTextFont, FontStyle.Bold);

			// Used for footnotes
			italicTextFont = new Font(regTextFont, FontStyle.Italic);

			ResetReport();

			printDocument = new PrintDocument();

			// Setup the footer data.  This method will set the footerHeight and footerY
			// and also initialize the footerData array.
			SetupFooter();

			// Wire up the event handlers
			printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
			printDocument.QueryPageSettings += new QueryPageSettingsEventHandler(printDocument_QueryPageSettings);
		}

		private void ResetReport()
		{
			// Get data for the current inspection
			if (eInspections.Count > 0)
			{
				curInspectionIdx = 0;
				eInspection = eInspections.Count == 0 ? null : eInspections[curInspectionIdx];
				curDsetIdx = 0;
				if (eInspection != null)
				{
					if (eInspection.GraphicID != null) eGraphic = new EGraphic(eInspection.GraphicID);
					if (eInspection.GridID != null) eGrid = new EGrid(eInspection.GridID);
					eDsets = EDset.ListByNameForInspection((Guid)eInspection.ID);
					eDset = eDsets.Count == 0 ? null : eDsets[curDsetIdx];
				}

			}

			// Initialize the report sections
			InitializeReportSections();

			// Set the current report section to the report heading
			curReportSection = ReportSectionEnum.Heading;
			nextPageLandscape = false;
			curPageNumber = 1;
		}

		public void SetupFooter()
		{
			EInspector reviewer = null;
			if (eInspectedComponent.InspComponentInsID != null)
				reviewer = new EInspector(eInspectedComponent.InspComponentInsID);
			this.signers = eInspectors.Count + (reviewer == null ? 0 : 1);
			this.signerInfo = new string[this.signers, 3];
			int idx = 0;
			foreach (EInspector inspector in eInspectors)
			{
				signerInfo[idx, 0] = "Inspector # " + (idx+1);
				signerInfo[idx, 1] = inspector.InspectorName;
				signerInfo[idx, 2] = inspector.InspectorLevelString;
				idx++;
			}
			if (reviewer != null)
			{
				signerInfo[idx, 0] = "Reviewer:";
				signerInfo[idx, 1] = reviewer.InspectorName;
				signerInfo[idx, 2] = reviewer.InspectorLevelString;
			}
			footerRowPadding = 20;
			this.footerHeight = (this.signers) * (regTextFont.Height + footerRowPadding) +
				regTextFont.Height*2;
		}

		// Optionally print a hardcopy of the report or show a preview of it.
		public void Print(bool hardcopy)
		{
			// create a PrintDialog based on the PrintDocument
			PrintDialog pdlg = new PrintDialog();
			pdlg.Document = printDocument;
			// show the PrintDialog
			if (pdlg.ShowDialog() == DialogResult.OK)
			{
				// decide what action to take
				if (hardcopy)
				{
					// actually print hardcopy
					printDocument.Print();
				}
				else
				{
					// preview onscreen instead
					PrintPreviewDialog prvw = new PrintPreviewDialog();
					prvw.Document = printDocument;
					prvw.ShowDialog();
				}
			}
		}

		void printDocument_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
		{
			// Get the default page settings for the current printer and set the margins
			// Try changing the bottom margin to help with goofy old printer...
			e.PageSettings.Margins = new Margins(50, 50, 50, 75);
			// Ask the current report if the next page to be printed needs to be landscape.
			e.PageSettings.Landscape = nextPageLandscape;
		}

		// Get the next report section, while managing collection indices.
		// It's ok if we return a section that isn't actually included, because the 
		// function that's calling us is doing so from a loop where that's being checked.
		ReportSection GetNextSection()
		{
			ReportSection section;
			// Increment the current report section
			curReportSection++;
			// Now find the next section to include
			while (curReportSection != ReportSectionEnum.None)
			{
				section = sections[(int)curReportSection];
				if (section.IsIncluded()) break;
				curReportSection++;
			}
			// If we've reached the end, we need to check first for more datasets, then
			// more inspections.
			if (curReportSection == ReportSectionEnum.None)
			{
				if (eInspections == null)
				{
					// No inspections, so just leave it at section none. We're done.
					return null;
				}
				else if (curInspectionIdx < eInspections.Count)
				{
					if (eDsets != null && curDsetIdx < eDsets.Count - 1)
					{
						// If the current inspection has more datasets, go back to do the next
						// instrument section.
						curDsetIdx++;
						eDset = eDsets[curDsetIdx];
						curReportSection = ReportSectionEnum.Instrument;
					}
					else if (curInspectionIdx < eInspections.Count - 1)
					{
						// The current inspection has no more datasets, but we have more inspections
						curInspectionIdx++;
						eInspection = eInspections[curInspectionIdx];
						if (eInspection.GraphicID != null) eGraphic = new EGraphic(eInspection.GraphicID);
						if (eInspection.GridID != null) eGrid = new EGrid(eInspection.GridID);
						// Re-Initialize the report sections
						InitializeReportSections();
						eDsets = EDset.ListByNameForInspection((Guid)eInspection.ID);
						curDsetIdx = 0;
						if (eDsets.Count > 0)
							eDset = eDsets[curDsetIdx];
						else
							eDset = null;

						curReportSection = ReportSectionEnum.InspectionHeading;
					}
					// else just leave it at section none.
				}
			}
			if (curReportSection != ReportSectionEnum.None)
			{
				return sections[(int)curReportSection];
			}
			return null;
		}

		// Print report sections until we're finished or can't fit any more on the page
		void printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			Graphics measure = e.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			Rectangle marginBounds = e.MarginBounds;
			float curX = marginBounds.X;
			float curY = marginBounds.Y;
			bool sectionDone = false;
			bool reportDone = false;
			bool nextSectionFits = true;

			ReportSection section = sections[(int)curReportSection];

			// Print report sections until we can't fit any more
			while (nextSectionFits)
			{
				// Print whatever will fit, picking up where we left off if we're continuing from the prev. page
				// The print method returns true once all is printed.
				sectionDone = section.Print(e, curY);

				if (sectionDone)
				{
					// Set the Y coord for the page to the section's Y coord.
					curY = section.Y;
				}
				else
				{
					// If we weren't able to finish the section, break out of the loop
					break;
				}

				// Get the next section that needs to be displayed
				section = GetNextSection();
				while (curReportSection != ReportSectionEnum.None && !section.IsIncluded())
					section = GetNextSection();

				if (curReportSection == ReportSectionEnum.None)
				{
					// No more inspections -- we're really done!
					reportDone = true;
					break;
				}

				// Check whether some or all of the current section can fit on the page
				// If the current section is the Inspection heading and this is not the first
				// inspection, we always start a new page.
				nextSectionFits = 
					!(curReportSection == ReportSectionEnum.InspectionHeading && 
					curInspectionIdx > 0) && section.CanFitSome(e, curY);

			}
			// Print the page footer
			PrintFooter(e);
			e.HasMorePages = (!reportDone);
			if (reportDone)
			{
				// Reset some report variables (in case the user prints from preview mode)
				ResetReport();
			}
			else
			{
				curPageNumber++;
			}
		}

		// Print the footer
		bool PrintFooter(PrintPageEventArgs args)
		{
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			int leftX = args.MarginBounds.X;
			int centerX = (int)(leftX + args.MarginBounds.Width / 2);
			int rightX = leftX + args.MarginBounds.Width;
			float curX = leftX;

			Font curFont = regTextFont;
			float curY = args.MarginBounds.Bottom - this.footerHeight + curFont.Height;
			float startY = curY;

			string s;
			int padding = 5;

			
			// SIGNERS
			for (int i = 0; i < signers; i++)
			{
				curX = leftX;
				g.DrawString(signerInfo[i, 0], curFont, Brushes.Black, new Point((int)curX, (int)curY));
				curX += 75;
				s = Chop(measure, curFont, signerInfo[i, 1], 175);
				g.DrawString(s, curFont, Brushes.Black, curX, curY);
				curX += 175;
				g.DrawString(signerInfo[i, 2], curFont, Brushes.Black, new Point((int)curX, (int)curY));
				curX += 50;
				g.DrawLine(Pens.Black, curX, curY + curFont.Height, curX + 200, curY + curFont.Height);
				curX += 200 + padding * 4;
				g.DrawString("Date:", curFont, Brushes.Black, curX, curY);
				curX += 40;
				g.DrawLine(Pens.Black, curX, curY + curFont.Height, curX + 150, curY + curFont.Height);
				
				curY += curFont.Height + footerRowPadding;
			}
			curX = leftX;

			// Date/time
			g.DrawString("Printed: " + DateTime.Now, curFont, Brushes.Black, curX, curY);
			
			// Component ID
			s = "Component ID: " + eComponent.ComponentName;
			SizeF siz = measure.MeasureString(s, curFont);
			curX = centerX - siz.Width / 2;
			g.DrawString(s, curFont, Brushes.Black, curX, curY);
			s = String.Format("Page {0}", curPageNumber) +
				(eInspectedComponent.InspComponentPageCountOverride == null ? "" :
					String.Format(" of {0}", eInspectedComponent.InspComponentPageCountOverride));

			// Page #
			siz = measure.MeasureString(s, curFont);
			curX = rightX - siz.Width;
			g.DrawString(s, curFont, Brushes.Black, curX, curY);
			return true;
		}

		// Truncate a string unless it fits in the specified width
		public string Chop(Graphics measure, Font fnt, string s, float width)
		{
			int charsFit;
			int linesFit;
			string cs = s;
			StringFormat format = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit);
			measure.MeasureString(s, fnt, new SizeF(width, fnt.Height*1.5f), format, out charsFit, out linesFit);
			if (charsFit < s.Length) cs = s.Substring(0, charsFit - 3) + "...";
			return cs;
		}

		// Fill the array of report section rules.
		private void InitializeReportSections()
		{
			sections = new ReportSection[] {
				// these first three sections are only on the first page, near the top, so they're easy
				new RsHeading(this, 
					new ReportSectionRules(ReportSectionEnum.Heading, PageForceMode.UnlessAllFits, 0),3),

				new RsCmpDefinition(this, 
					new ReportSectionRules(ReportSectionEnum.CmpDefinition, PageForceMode.UnlessAllFits, 0),3),

				new RsCmpDetails(this,
					new ReportSectionRules(ReportSectionEnum.CmpDetails, PageForceMode.UnlessAllFits, 0),2),

				// The Inspection heading section always starts a new page, except if it falls on the first page.
				// It is not allowed to break.
				new RsInspectionHeading(this,
					new ReportSectionRules(ReportSectionEnum.InspectionHeading, PageForceMode.AlwaysExcept1stPage, 0),2),

				// The Graphic/Notes section always follows the inspection heading, so the graphic should
				// always fit.  The text may be longer than the graphic and may break if needed.
				new RsGraphicNotes(this,
					new ReportSectionRules(ReportSectionEnum.GraphicNotes, PageForceMode.UnlessMinRowsAnyPage, 0),2),

				// The Statistics and Nongrid measurements section can break, but unless we can fit 4 lines
				// we'll put it on the next page
				new RsStatsAndNonGrid(this,
					new ReportSectionRules(ReportSectionEnum.StatsAndNonGrid, PageForceMode.UnlessMinRowsAnyPage, 4),2),
				
				// The partition info section isn't allowed to break.
				new RsPartition(this,
					new ReportSectionRules(ReportSectionEnum.Partition, PageForceMode.UnlessAllFits, 0), 3),
				
				// The grid can break, but unless we can fit the whole thing, we'll put it on the next page
				new RsGrid(this,
					new ReportSectionRules(ReportSectionEnum.Grid, PageForceMode.UnlessMinRowsAnyPage, 4), 1),
				
				// The next three sections aren't allowed to break (although the CalData section is several rows.
				new RsLegend(this,
					new ReportSectionRules(ReportSectionEnum.CrewDoseAndLegend, PageForceMode.UnlessAllFits, 0), 2),
				
				new RsInstrument(this,
					new ReportSectionRules(ReportSectionEnum.Instrument, PageForceMode.UnlessAllFits, 0),3),

				new RsCalData(this,
					new ReportSectionRules(ReportSectionEnum.CalData, PageForceMode.UnlessAllFits, 0),3)
			};
		}
	}
}
