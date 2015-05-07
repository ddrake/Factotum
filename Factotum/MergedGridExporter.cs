using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DowUtils;
using System.Runtime.InteropServices;
using xlsgen;
using System.IO;
using System.ComponentModel;
using System.Data;

namespace Factotum
{
	class MergedGridExporter
	{
		[DllImport("xlsgen.dll")]
		static extern IXlsEngine Start();

		IXlsEngine engine;
		IXlsWorkbook wbk = null;
		IXlsWorksheet wks;

		public string result;
		String filePath;
		EGrid parentGrid;
		EGridCollection childGrids;
		BackgroundWorker bw;
        const int MINFRACTION = 16;  // the smallest part of an inch to concern ourselves with...
        const int HEADERROWS = 0;
        const int HEADERCOLS = 0;
        int axicell; // the number of axial units (e.g. sixteenths) represented by the height of a spreadsheet cell
        int radcell; // the number of radial units (e.g. sixteenths) represented by the width of a spreadsheet cell
        // Constructor
        public MergedGridExporter(Guid ParentGridID, string filePath, BackgroundWorker bw, bool createMergedGrid)
		{
            this.parentGrid = new EGrid(ParentGridID);

            if (createMergedGrid)
                this.childGrids = parentGrid.GetAllChildGrids();
            else
                this.childGrids = new EGridCollection();

            this.filePath = filePath;
			this.bw = bw;
			this.result = null;
            getCellDims(this.parentGrid, this.childGrids, out axicell, out radcell);
		}

        public bool CreateWorkbook()
        {
            engine = Start();
            // create a new Excel file, and retrieve a workbook to work with
            wbk = engine.New(filePath);

            // delete existing sheets last
            int initialSheetCount = wbk.WorksheetCount;

            int i = 0;
            // create a new worksheet
            string tabName = this.childGrids.Count > 0 ? "Merged Grid" : "Regular Grid";
            wks = wbk.AddWorksheet("Merged Grid");
            CreateWorksheet();

            for (i = 0; i < initialSheetCount; i++)
            {
                wbk.get_WorksheetByIndex(1).Delete();
            }
            wbk.Close();
            // try to solve file locking issue
            wbk = null;
            engine = null;
            return true;
        }

        private void CreateWorksheet()
        {
            // Note: I tried adding conditional formatting but gave it up because it caused large
            // spreadsheets to crash and did not support changing the text color, just the background color.

            // For now, I think it's best to leave the headings off.
            // The big feedwater heater started labelling letters at AA.  This caused the number of columns to exceed
            // Excel's limit of 256.

            // Add the primary grid data to the spreadsheet
            AddGridDataToWorksheet(parentGrid, true);

            // Add the child grids (if any) to the spreadsheet
            foreach (EGrid grid in childGrids)
            {
                AddGridDataToWorksheet(grid, false);
            }
        }

        // go through all the rows in the data table, adding the measurement value to the cell.
        // if it's not the parent, additionally add a comment containing the child row and column 
        private void AddGridDataToWorksheet(EGrid grid, bool isParent)
        {
            int mrow, mcol; // measurement row and col
            int crow, ccol; // spreadsheet cell row and col
            int axigrid_cur = fractions((decimal)(grid.GridAxialDistance));
            int radgrid_cur = fractions((decimal)(grid.GridRadialDistance)); 
            int axigrid_par = (isParent? 0 : fractions((decimal)(parentGrid.GridAxialDistance)));
            int radgrid_par = (isParent? 0 : fractions((decimal)(parentGrid.GridRadialDistance))); 
            double cellValue;
            string cellText;
            bool isValue;
            string inspectionName; // used in the comment text for child grids

            // Update the section info variables for the grid.
            grid.UpdateSectionInfoVars();

            EInspection insp = new EInspection(grid.GridIspID);
            inspectionName = insp.InspectionName;
            // Get the data for the primary grid
            DataTable gridData = EMeasurement.GetForGrid(grid.ID);

            foreach (DataRow dr in gridData.Rows)
            {
                // get the measurement row and column
                getRowColForMeasurement(dr, out mrow, out mcol);  
                
                // get the spreadsheet cell row and column
                if (isParent)
                {
                    getCellLocation(mrow, mcol, axigrid_cur, radgrid_cur, out crow, out ccol);
                }
                else
                {
                    // todo: once the GridChildRefRow and GridChildRefCol are added, fix two args
                    getCellLocation(mrow, mcol, axigrid_cur, radgrid_cur, axigrid_par, radgrid_par, 
                        0,0,(int)grid.GridParentStartRow, (int)grid.GridParentStartCol, out crow, out ccol);
                }
                // get the information to be placed in the spreadsheet cell
                getInfoForCell(dr, out cellValue, out cellText, out isValue);

                // Excel only allows 256 columns, so do the best we can...
                if (isValue && ccol < 256)
                {
                    wks.set_Float(crow, ccol, cellValue);
                    //setConditionalFormatting(grid, crow, ccol, mrow);
                }
                else if (ccol < 256 && wks.get_CellType(crow, ccol) != enumDataType.datatype_double)
                {
                    wks.set_Label(crow, ccol, cellText);
                }

                if (!isParent && ccol < 256)
                {
                    xlsgen.IXlsComment c = wks.NewComment(crow, ccol);
                    c.Author = "Factotum";
                    c.Label = inspectionName + ": " + EMeasurement.GetIdentifierForRowAndCol((short)mrow, (short)mcol);
                }
            }
        }

        // Get the axial and radial distances that will be represented by 
        // the height and width of a spreadsheet cell.
        private void getCellDims(EGrid parent, EGridCollection children,
            out int axials, out int radials)
        {
            axials = fractions((decimal)(parent.GridAxialDistance));
            radials = fractions((decimal)(parent.GridRadialDistance));

            foreach (EGrid child in children)
            {
                axials = greatestCommonFactor(axials, fractions((decimal)(child.GridAxialDistance)));
                radials = greatestCommonFactor(radials, fractions((decimal)(child.GridRadialDistance)));
            }
        }

        // Get the location of the cell in the spreadsheet for a parent grid
        private void getCellLocation(int mrow, int mcol,
            int axigrid_cur, int radgrid_cur, out int row, out int col)
        {
            // 1-based array
            row = 1 + HEADERROWS + mrow * axigrid_cur / axicell;
            col = 1 + HEADERCOLS + mcol * radgrid_cur / radcell;
        }


        // Get the location of the cell in the spreadsheet for a child grid
        private void getCellLocation(int mrow, int mcol,
            int axigrid_cur, int radgrid_cur, int axigrid_par, int radgrid_par,
            int refrow_cur, int refcol_cur, int refrow_par, int refcol_par, out int row, out int col)
        {
            // The location of the cell in the spreadsheet depends on the following:
            // mrow, mcol: the local measurement row and column
            // hrows, hcols: the number of header rows and columns
            // axicell, radcell: the axial and radial distances represented by a spreadsheet cell in teenths
            // axigrid_cur, radgrid_cur: the axial and radial dimensions of the grid whose measurements are being placed
            // axigrid_par, radgrid_par: the axial and radial dimensions of the parent grid
            // refrow_par, refcol_par: the parent row and column reference
            // refrow_cur, refcol_cur: the child row and column reference
            row = 1 + HEADERROWS + (refrow_par * axigrid_par + (mrow - refrow_cur) * axigrid_cur) / axicell;
            col = 1 + HEADERCOLS + (refcol_par * radgrid_par + (mcol - refcol_cur) * radgrid_cur) / radcell;
        }

        // Parse the measurement data row, getting the row and column.
        private void getRowColForMeasurement(DataRow dr, out int row, out int col)
        {
            row = Convert.ToInt32(dr[1]);
            col = Convert.ToInt32(dr[2]);
        }

        // Parse the measurement data row, getting the local row, column and text for the spreadsheet cell.
        private void getInfoForCell(DataRow dr, out double cellValue, out string cellText, out bool isValue)
        {
            //dr[0] ID,
            //dr[1] MeasurementRow,
            //dr[2] MeasurementCol,
            //dr[3] MeasurementThickness,
            //dr[4] MeasurementIsObstruction,
            //dr[5] MeasurementIsError
        	decimal? thick;
			bool obstr, error;
            cellText = "";
            cellValue = -1.0;
            if (dr[3] == DBNull.Value) thick = null;
			else thick = Convert.ToDecimal(dr[3]);
			obstr = (bool)dr[4];
			error = (bool)dr[5]; // not used?
            if (thick == null)
            {
                if (obstr) cellText = "obst.";
                else cellText = "empty";
                isValue = false;
            }
            else
            {
                cellValue = (double)thick;
                isValue = true;
            }
		}


		private string GetExcelRange(int sRow, int sCol, int rows, int cols)
		{
			return string.Format("R{0}C{1}:R{2}C{3}", new object[] { sRow, sCol, sRow + rows - 1, sCol + cols - 1 });
		}

		private void CreateMergedCellsLabel(IXlsWorksheet wks, IXlsStyle style,
			int sRow, int sCol, int rows, int cols, string label)
		{
			style.Apply();
			wks.set_Label(sRow, sCol, label);
			xlsgen.IXlsRange range = wks.NewRange(GetExcelRange(sRow,sCol,rows, cols));
			xlsgen.IXlsMergedCells mc = range.NewMergedCells();
		}

		private void BorderAroundRange(IXlsWorksheet wks, int sRow, int sCol, int rows, int cols)
		{
			int cRow, cCol;
			cRow = sRow;
			cCol = sCol;
			AddBorder(wks, cRow, cCol, 1, cols, "top");
			cRow = sRow + rows - 1;
			AddBorder(wks, cRow, cCol, 1, cols, "bottom");
			cRow = sRow;
			cCol = sCol;
			AddBorder(wks, cRow, cCol, rows, 1, "left");
			cCol = sCol + cols - 1;
			AddBorder(wks, cRow, cCol, rows, 1, "right");
		}

		private void AddBorder(IXlsWorksheet wks, int cRow, int cCol, int rows, int cols, string position)
		{
			IXlsRange range = wks.NewRange(GetExcelRange(cRow, cCol, rows, cols));
			switch (position)
			{
				case "top":
					range.Style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
					range.Style.Borders.Top.Color = (int)xlsgen.enumColorPalette.colorBlack;
					break;
				case "bottom":
					range.Style.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
					range.Style.Borders.Bottom.Color = (int)xlsgen.enumColorPalette.colorBlack;
					break;
				case "left":
					range.Style.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
					range.Style.Borders.Left.Color = (int)xlsgen.enumColorPalette.colorBlack;
					break;
				case "right":
					range.Style.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
					range.Style.Borders.Right.Color = (int)xlsgen.enumColorPalette.colorBlack;
					break;
				default:
					throw new Exception("Unknown border position");
			}
			range.Apply();
		}

        // Euclid's method
        // given two positive decimal numbers, find their greatest common factor
        private int greatestCommonFactor(int big, int small)
        {
            if (big < 0 || small < 0) throw new InvalidDataException("Both arguments must be positive");
            if (big < small) return greatestCommonFactor(small, big);
            if (small == 0) return big;
            if (big == 0) return small; // shouldn't normally happen.
            return greatestCommonFactor(small, big % small);
        }


        // returns the number of (e.g. sixteenths) of an inch for the distance, rounded to the
        // nearest (e.g. sixteenth)
        private int fractions(decimal distance)
        {
            return (int)Math.Round(distance * MINFRACTION);
        }

	}
}
