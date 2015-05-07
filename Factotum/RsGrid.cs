using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Data;


namespace Factotum
{
	class RsGrid : ReportSection
	{
		// If we couldn't fit the whole grid on a page, we need to keep track of 
		// where in the grid to start up again.  
		private int startRow;
		private int startCol;
		private int startWeldIdx; // The starting weld index for the current chunk of rows
		private int curWeldIdx; // The current weld index

		// This is about as narrow as we can make columns.
		private const float colWidth = 36;

		public RsGrid(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
			startRow = 0;
			startCol = 0;
			startWeldIdx = 0;
			curWeldIdx = 0;
		}

		// Incude the section only if we have a grid.
		public override bool IsIncluded() 
		{
			return (rpt.eInspection != null && rpt.eInspection.InspectionHasGrid
				&& rpt.eGrid.GridStartRow != null & rpt.eGrid.GridStartCol != null
				&& rpt.eGrid.GridEndRow != null & rpt.eGrid.GridEndCol != null);
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			// We only return true if we can fit the entire grid on the current page.

			// This is also the perfect place to decide about page orientation.
			// If the number of columns is such that fewer page widths will be required to cover
			// the columns in landscape than in portrait, we want to set next page to landscape here.

			// If we're not on the first page, we can definitely fit some.  
			// We'll also leave the orientation the same as prior pages.
			if (startRow > 0 || startCol > 0) return true;

			// See if we can fit ALL the columns on what's left of the current page and at least a few rows.
			// Add 2 to the end column (1 for zero-based + 1 for header row/col)
			// Changed to 15 -- it was still splitting tables too often...
			int minRows = 15;
			if (args.MarginBounds.Width > (rpt.eGrid.GridEndCol - rpt.eGrid.GridStartCol + 2) * colWidth)
			{
				if (args.MarginBounds.Bottom - Y - rpt.footerHeight > minRows * rowHeight())
					return true;
				else
					return false; // Not too many columns, so keep portrait orientation
			}
			// We can't fit any on the current page.  Decide on the orientation.
			if (IsLandscapeBetter(args)) rpt.nextPageLandscape = true;

			return false;
		}

		// Get the row height
		private float rowHeight()
		{
			return rpt.smallTextFont.Height + 2;
		}

		// Figure out how many rows will fill a page in the current orientation.
		private int rowsToFillPage(PrintPageEventArgs args, float curY)
		{
			return (int)((args.MarginBounds.Bottom - curY - rpt.footerHeight) / rowHeight());
		}

		// Figure out how many columns will fill a page in the current orientation.
		private int colsToFillPage(PrintPageEventArgs args)
		{
			return (int)(args.MarginBounds.Width / colWidth);
		}

		// We should be in portrait mode when this function is called.
		private bool IsLandscapeBetter(PrintPageEventArgs args)
		{
			int cols = (int)rpt.eGrid.GridEndCol - (int)rpt.eGrid.GridStartCol + 1; // The data columns in the grid
			int rows = (int)rpt.eGrid.GridEndRow - (int)rpt.eGrid.GridStartRow + 1; // The data rows in the grid 
			float width = args.MarginBounds.Width;
			float height = args.MarginBounds.Height;
			System.Diagnostics.Debug.Assert(height > width);
			int portraitPages;
			int landscapePages;
			// First get the number of pages to cover all the columns for a single chunk of rows.
			// Subtract off a column width in the denominator for the row heading column
			int portraitPagesToSpanCols = (int)(cols * colWidth / (width - colWidth) + 1);
			int landscapePagesToSpanCols = (int) (cols * colWidth / (height - colWidth) + 1);
			float rowsPerPortraitPage = (args.MarginBounds.Height - rpt.footerHeight - rowHeight()) / rowHeight();
			float rowsPerLandscapePage = (args.MarginBounds.Width - rpt.footerHeight - rowHeight()) / rowHeight();
			portraitPages = portraitPagesToSpanCols * (int)(rows / rowsPerPortraitPage + 1);
			landscapePages = landscapePagesToSpanCols * (int)(rows / rowsPerPortraitPage + 1);

			// If it's a tie, keep it in portrait
			return (landscapePages < portraitPages);

		}

		public override bool Print(PrintPageEventArgs args, float Y) 
		{
			// The page orientation has been set already..
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			int curX;
			int startY = args.MarginBounds.Top;
			int dataRows;
			int dataCols;
			bool fitAllRows;
			bool fitAllCols;
			bool sectionDone = false;

			int padding = 5;
			float curY;

			// Update the section info variables for the grid.  The weld locations and 
			// colors will depend on this.
			rpt.eGrid.UpdateSectionInfoVars();

			curY = (Y > args.MarginBounds.Top ? Y + padding : Y);

			int gridCols = (int)rpt.eGrid.GridEndCol - (int)rpt.eGrid.GridStartCol + 1; // The data columns in the grid
			int gridRows = (int)rpt.eGrid.GridEndRow - (int)rpt.eGrid.GridStartRow + 1; // The data rows in the grid 

			// Subtract 1 row and 1 col (headings)

			// The data columns that will fit on the page
			int dataColsToFit = colsToFillPage(args) - 1;

			// The data rows that will fit on the page (this includes weld rows)
			int dataRowsToFit = rowsToFillPage(args, curY) - 1; 

			// Get the weld info.  Extra rows are added to the printed grid for welds.
			int[] divRows;
			int[] divTypes;
			rpt.eGrid.GetGridDivisionInfo(out divRows, out divTypes);
			int divisions = divRows.Length;

			// Print the next chunk of columns 

			fitAllCols = (gridCols - startCol <= dataColsToFit);
			fitAllRows = (gridRows - startRow + divisions <= dataRowsToFit);

			// These will be used to size the arrays
			dataCols = fitAllCols ? gridCols - startCol : dataColsToFit;
			dataRows = fitAllRows ? gridRows - startRow : dataRowsToFit-divisions;

			// Get an array with the subset of rows and columns that will fit on the page.
			string[,] gridDataArray;
			int[,] gridColorArray;

			GetGridArray(startRow, startCol, dataRows, dataCols,  
				out gridDataArray, out gridColorArray);

			curX = args.MarginBounds.Left + (int)((args.MarginBounds.Width - (dataCols + 1) * colWidth) / 2);

			curY = DrawGridTable(gridDataArray, gridColorArray, divRows, divTypes, fitAllRows, dataRows, dataCols,
				g,	curX, curY, (int)colWidth, (int)rowHeight(), padding, rpt.boldSmallTextFont, rpt.smallTextFont);

			// If the current column + cols per page < max cols per page,
			//		reset next column to zero after printing the chunk
			if (fitAllRows)
			{
				if (fitAllCols)
				{
					// We're done.
					rpt.nextPageLandscape = false;
					sectionDone = true;
					curY += padding;
					hr(args, curY);
					this.Y = curY;
				}
				else
				{
					// Leave start row as is. 
					this.startCol += dataCols;
					this.curWeldIdx = this.startWeldIdx;
				}
			}
			else
			{
				if (fitAllCols)
				{
					// We finished all the columns for this section of rows
					this.startCol = 0;
					this.startRow += dataRows;
					this.startWeldIdx = this.curWeldIdx;
				}
				else
				{
					// We still have some columns for this section of rows
					// leave start row as is.
					this.startCol += dataCols;
					this.curWeldIdx = this.startWeldIdx;
				}
			}		
			return sectionDone;
		}

		// Fill three arrays, one with text strings, one with integer colors, and one with weld indices
		private void GetGridArray(int startRow, int startCol, int dataRows, int dataCols,
				out string[,] gridDataArray, out int[,] gridColorArray)
		{
			// Grid row and column indices
			int row; int col;

			// Grid starting row and column (not necessarily zero)
			int gridStartRow = (int)(rpt.eGrid.GridStartRow);
			int gridStartCol = (int)(rpt.eGrid.GridStartCol);

			// Measurement data
			decimal? thick;
			bool obstr;
			bool empty;
			bool error;

			// Initialize output arrays
			gridDataArray = new string[dataRows + 1, dataCols + 1];
			gridColorArray = new int[dataRows + 1, dataCols + 1];

			// The current datarow of the datatable and its index.
			DataRow dr;
			int drIdx = 0;

			// Array row and column indices used by gridDataArray and gridColorArray
			int aRow; int aCol;

			DataTable measurements = EMeasurement.GetForGrid(rpt.eGrid.ID);
			//	Columns: 0:ID, 1:MeasurementRow, 2:MeasurementCol, 
			// 3:MeasurementThickness, 4:MeasurementIsObstruction, 5:MeasurementIsError
			// Ordered by: MeasurementRow, MeasurementCol

			// Fill in the heading row and column.
			FillHeadings(startRow, startCol, dataRows, dataCols, gridDataArray, gridColorArray);

			// Loop through the entire datatable
			while (drIdx < measurements.Rows.Count)
			{

				dr = measurements.Rows[drIdx++];
				row = Convert.ToInt32(dr[1]);
				col = Convert.ToInt32(dr[2]);
				
				// If the datarow is not in the range we're interested in, get the next one
				if (row - gridStartRow < startRow || row - gridStartRow >= startRow + dataRows ||
					col-gridStartCol < startCol || col-gridStartCol >= startCol + dataCols)
					continue;

				// If we didn't continue, we want to include the info in the array, so get the other data.
				if (dr[3] == DBNull.Value) thick = null;
				else thick = Convert.ToDecimal(dr[3]);
				obstr = (bool)dr[4];
				error = (bool)dr[5];
				empty = (thick == null && !obstr && !error);
				aRow = row - gridStartRow - startRow + 1;
				aCol = col - gridStartCol - startCol + 1;

				// fill in the text
				if (obstr) gridDataArray[aRow, aCol] = "obst.";
				else if (error) gridDataArray[aRow, aCol] = "error";
				else if (empty) gridDataArray[aRow, aCol] = "empty";
				else gridDataArray[aRow, aCol] = GetFormattedThickness(thick);
				// fill in the color
				gridColorArray[aRow, aCol] = GetTextColor(row - gridStartRow, col, obstr, empty, error, thick);
			}
		}

		// Handle the heading row and column of the data and color arrays
		private void FillHeadings(int startRow, int startCol, int dataRows, int dataCols,
			string[,] gridDataArray, int[,] gridColorArray)
		{
			// Fill column headings
			int col = startCol;
			int? gridStartCol = rpt.eGrid.GridStartCol;
			int? gridStartRow = rpt.eGrid.GridStartRow;

			for (int c = 1; c <= dataCols; c++)
			{
				gridDataArray[0, c] = EMeasurement.GetColLabel((short)(col + gridStartCol));
				gridColorArray[0, c] = Color.Black.ToArgb();
				col++;
			}
			// Fill row headings
			int row = startRow;
			for (int r = 1; r <= dataRows; r++)
			{
				gridDataArray[r, 0] = (row+gridStartRow+1).ToString();
				row++;
				gridColorArray[r, 0] = Color.Black.ToArgb();
			}
		}

		// Format a thickness value for display
		private string GetFormattedThickness(decimal? number)
		{
			return number == null ? "N/A" :
				string.Format("{0:0.000}", number);
		}


		// Get the color for the text at the given grid row/col
		private int GetTextColor(int row, int col, bool obstr, bool empty, bool error, decimal? thick)
		{
			Color curColor = Color.Black;
			if (error) curColor = Color.Black;
			else if (obstr) curColor = Color.Black;
			else if (empty) curColor = Color.Black;
			else
			{
				ThicknessRange range =
					rpt.eGrid.GetThicknessRange(row, (decimal)thick);
				switch (range)
				{
					case ThicknessRange.BelowTscreen:
						curColor = Color.Crimson;
						break;
					case ThicknessRange.TscreenToTnom:
						curColor = Color.Green;
						break;
					case ThicknessRange.TnomTo120pct:
						curColor = Color.Black;
						break;
					case ThicknessRange.Above120pctTnom:
						curColor = Color.Blue;
						break;
					case ThicknessRange.Unknown:
						curColor = Color.Magenta;
						break;
				}
			}
			return curColor.ToArgb();
		}

		// Draw a formatted table of UT grid data.
		private float DrawGridTable(string[,] table, int[,] colors, 
			int[] divRows, int[] divTypes, bool fitAllRows, int dataRows, int dataCols,
			Graphics g, float X, float Y, int colWidth, int rowHeight,
			int padding, Font headingFont, Font dataFont)
		{
			// Note: the divRows[] array contains the indices of the rows immediately following a division
			int gridStartRow = (int)rpt.eGrid.GridStartRow;
			float curX;
			float curY = Y;
			Font curFont;
			int totCols = dataCols + 1;
			Brush tmpBrush;

			for (int r = 0; r <= dataRows; r++)
			{
				// Reset the X coord each row.
				curX = X;
				// Insert a weld if needed -- Note: We need to subtract 1 because of the col header row
				if ((curWeldIdx < divRows.Length) && r + startRow + gridStartRow - 1 == divRows[curWeldIdx])
				{
					tmpBrush = ((GridDividerTypeEnum)divTypes[curWeldIdx] == GridDividerTypeEnum.Weld ?
						Brushes.Gray : Brushes.SlateBlue);
					g.FillRectangle(tmpBrush, X, curY, colWidth * totCols, rowHeight / 2);
					// Draw the text "Weld" roughly centered.
					//g.DrawString("Weld", dataFont, Brushes.White, X + colWidth * (int)(totCols / 2), curY);
					curY += rowHeight / 2;
					curWeldIdx++;
				}				
				// Alternate row background
				if (r % 2 > 0) g.FillRectangle(Brushes.Beige, curX, curY, colWidth * totCols, rowHeight);

				// horizontal grid line
				g.DrawLine(Pens.Black, curX, curY, curX + colWidth * totCols, curY);

				// Draw the conditionally formatted text
				for (int c = 0; c <= dataCols; c++)
				{
					curFont = (r == 0 || c == 0 ? headingFont : dataFont);
					
					Brush b = new SolidBrush(Color.FromArgb(colors[r, c]));
					if (r == 0)
						// center column headings
						g.DrawString(table[r, c], curFont, b, curX + 
							(colWidth - g.MeasureString(table[r,c], headingFont).Width) / 2 - 2, curY);
					else
						g.DrawString(table[r, c], curFont, b, curX, curY);

					curX += colWidth;
				}

				curY += rowHeight;

			}
			// Insert a weld if needed -- Note: We need to subtract 1 because of the col header row
			if (curWeldIdx < divRows.Length && fitAllRows )
			{
				tmpBrush = ((GridDividerTypeEnum)divTypes[curWeldIdx] == GridDividerTypeEnum.Weld ?
					Brushes.Gray : Brushes.SlateBlue);
				g.FillRectangle(tmpBrush, X, curY, colWidth * totCols, rowHeight / 2);
				// Draw the text "Weld" roughly centered.
				//g.DrawString("Weld", dataFont, Brushes.White, X + colWidth * (int)(totCols / 2), curY);
				curY += rowHeight / 2;
				curWeldIdx++;
			}

			// horizontal grid line
			g.DrawLine(Pens.Black, X, curY, X + colWidth * totCols, curY);

			// vertical lines
			g.DrawLine(Pens.Black, X, Y, X, curY);
			g.DrawLine(Pens.Black, X + colWidth * totCols, Y, X + colWidth * totCols, curY);
			return curY;
		}
	}
}
