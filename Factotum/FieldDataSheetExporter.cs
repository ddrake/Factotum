using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DowUtils;
using System.Runtime.InteropServices;
using xlsgen;
using System.IO;
using System.ComponentModel;

namespace Factotum
{
	class FieldDataSheetExporter
	{
		[DllImport("xlsgen.dll")]
		static extern IXlsEngine Start();

		IXlsEngine engine;
		IXlsWorkbook wbk = null;
		IXlsWorksheet wks;
		
		public string result;
		String filePath;
		EOutage curOutage;
		EUnit curUnit;
		EInspectorCollection inspectors;
		EGridProcedureCollection gridProcedures;
		BackgroundWorker bw;

		public FieldDataSheetExporter(Guid OutageID, string filePath, BackgroundWorker bw)
		{
			this.curOutage = new EOutage(OutageID);
			this.filePath = filePath;
			this.bw = bw;
			this.curUnit = new EUnit(curOutage.OutageUntID);
			this.inspectors = EInspector.ListForOutage((Guid)OutageID, false, false);
			this.gridProcedures = EGridProcedure.ListForOutage((Guid)OutageID, false, false);
			this.result = null;
		}

		private void CreateWorksheet(IXlsWorksheet wks, EInspector inspector)
		{
			Guid? kitID = inspector.InspectorKitID;
			EMeterCollection meters = EMeter.ListForKit(kitID,false);
			EDucerCollection ducers = EDucer.ListForKit(kitID, false);
			EThermoCollection thermos = EThermo.ListForKit(kitID, false);
			ECalBlockCollection calBlocks = ECalBlock.ListForKit(kitID, false);
			IXlsStyle titleStyle, labelStyle, dataStyle, ckBoxStyle, labelBoxStyle;
			int lastRow;

			dataStyle = wks.NewStyle();
			dataStyle.Font.Name = "Arial";
			dataStyle.Font.Size = 9;
			dataStyle.Font.Bold = 0;
			dataStyle.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_left;

			labelStyle = dataStyle.Duplicate();
			labelStyle.Font.Bold = 1;

			titleStyle = dataStyle.Duplicate();
			titleStyle.Font.Size = 12;
			titleStyle.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

			labelBoxStyle = labelStyle.Duplicate();

			labelBoxStyle.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
			labelBoxStyle.Borders.Top.Color = (int)xlsgen.enumColorPalette.colorBlack;
			labelBoxStyle.Borders.Left.Style = xlsgen.enumBorderStyle.border_thin;
			labelBoxStyle.Borders.Left.Color = (int)xlsgen.enumColorPalette.colorBlack;
			labelBoxStyle.Borders.Right.Style = xlsgen.enumBorderStyle.border_thin;
			labelBoxStyle.Borders.Right.Color = (int)xlsgen.enumColorPalette.colorBlack;
			labelBoxStyle.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_thin;
			labelBoxStyle.Borders.Bottom.Color = (int)xlsgen.enumColorPalette.colorBlack;

			ckBoxStyle = labelBoxStyle.Duplicate();
			ckBoxStyle.Borders.Top.Style = xlsgen.enumBorderStyle.border_medium;
			ckBoxStyle.Borders.Left.Style = xlsgen.enumBorderStyle.border_medium;
			ckBoxStyle.Borders.Right.Style = xlsgen.enumBorderStyle.border_medium;
			ckBoxStyle.Borders.Bottom.Style = xlsgen.enumBorderStyle.border_medium;

//			SetupStyles(wks, out titleStyle, out labelStyle, out dataStyle, out ckBoxStyle, out labelBoxStyle);

			wks.PageSetup.PageOrientation = 1;
			wks.PageSetup.PaperSize = "LETTER";
			wks.PageSetup.SetFitWidth(1, 1);
			wks.PageSetup.SetMargins(1.75, 1.75, 1.75, 1.25, 0, 0);
			wks.PageSetup.CenterHorizontally = 1;
			wks.PageSetup.PrintGridlines = 0;
			// approximate (col width in pixels)/(char width ~ 8) some trial and error.
			wks.get_Columns("A1:W1").Width = 4.3;

			// Assemble the data for the header section
			string vndName = UserSettings.sets.VendorName;
			string unitName = curUnit.UnitNameWithSite;
			string outageName = curOutage.OutageName;
			if (Util.IsNullOrEmpty(outageName)) outageName = "N/A";
			outageName = "Refueling Outage " + outageName;
			string calProc = curOutage.OutageCalibrationProcedureName;
			if (Util.IsNullOrEmpty(calProc)) calProc = "N/A";
			string inspectorName = inspector.InspectorName + " - " + inspector.InspectorLevelString;
			string facPhone = curOutage.OutageFacPhone;
			if (Util.IsNullOrEmpty(facPhone)) facPhone = "N/A";
			lastRow = AddHeader(wks, vndName, unitName, outageName, calProc, inspectorName, gridProcedures,
				titleStyle, labelStyle, dataStyle, ckBoxStyle, labelBoxStyle);
			lastRow = AddTools(wks, meters, ducers, thermos, calBlocks, 
				labelBoxStyle, dataStyle, ckBoxStyle, lastRow+2);
			AddBoilerPlate(wks, facPhone, labelStyle, labelBoxStyle, dataStyle, lastRow+2);
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

		private int AddHeader(IXlsWorksheet wks, string vendorName, string unitName, string outageName,
			string calProcName, string inspectorName, EGridProcedureCollection gridProcs,
			IXlsStyle titleStyle, IXlsStyle labelStyle, IXlsStyle dataStyle, 
			IXlsStyle ckBoxStyle, IXlsStyle labelBoxStyle)
		{
			int sRow = 1;
			int sCol = 1;
			int cRow, cCol, rows, cols;
			int maxRow = 0;
			// Add the report title
			cRow = sRow;	cCol = sCol;
			rows = 1;	cols = 23;
			CreateMergedCellsLabel(wks, titleStyle, cRow, cCol, rows, cols,
				"Ultrasonic Thickness Measurement Data Sheet");

			// Add the vendor, unit and outage info
			sRow++; // sRow = 2
			cRow = sRow;
			cols = 5;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, vendorName);
			cRow++;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, unitName);
			cRow++;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, outageName);
			if (cRow > maxRow) maxRow = cRow;

			// Add the procedures
			sCol = 7;
			cRow = sRow;
			cCol = sCol;
			cols = 2;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, "Cal. Proc.");
			cCol += cols;
			cols = 6;
			CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, calProcName);

			cRow++;
			cCol = sCol;
			cols = 8;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, "UT Grid Procedure (Check one)");
			foreach (EGridProcedure gProc in gridProcedures)
			{
				cRow++;
				cCol = sCol;
				ckBoxStyle.Apply();
				wks.set_Label(cRow, cCol, "");
				cCol++;
				cols = 7;
				CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, gProc.GridProcedureName);
			}
			if (cRow > maxRow) maxRow = cRow;
			// Add the right column info: Inspector, report, w/o, date
			sCol = 16;
			cRow = sRow;
			cCol = sCol;
			cols = 2;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, "Inspector:");
			cRow++;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, "Report #:");
			cRow++;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, "W/O #:");
			cRow++;
			CreateMergedCellsLabel(wks, labelStyle, cRow, cCol, rows, cols, "Date:");

			cCol+=2;
			cRow = sRow;
			cols = 6;
			CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, inspectorName);
			cRow++;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cRow++;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cRow++;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			if (cRow > maxRow) maxRow = cRow;

			return maxRow;			
		}

		private int AddTools(IXlsWorksheet wks, EMeterCollection meters, EDucerCollection ducers,
			EThermoCollection thermos, ECalBlockCollection calBlocks,
			IXlsStyle labelBoxStyle, IXlsStyle dataStyle, IXlsStyle ckBoxStyle, int startRow)
		{
			IXlsStyle centeredLabelStyle = labelBoxStyle.Duplicate();
			centeredLabelStyle.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;
			int sRow = startRow;
			int sCol = 1;
			int cRow, cCol, rows, cols;
			string data;
			// Total of collection counts, forcing at least one row for each tool.
			int totalTools = Math.Max(meters.Count,1) + Math.Max(ducers.Count,1) + 
				Math.Max(thermos.Count,1) + Math.Max(calBlocks.Count,1);
			// Add the section label
			cRow = sRow; cCol = sCol;
			rows = 1; cols = 14;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols,
				"Kit Items Used");
			// -------------------
			// METERS
			// -------------------
			if (meters.Count > 0)
			{
				foreach (EMeter mtr in meters)
				{
					cRow++;
					cCol = sCol;
					ckBoxStyle.Apply();
					wks.set_Label(cRow, cCol, "");
					cCol++;
					cols = 3;
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Instrument");
					cCol += 3;
					cols = 10;
					data = mtr.MeterModelName + ", S/N: " + mtr.MeterSerialNumber;
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, data);
				}
			}
			else
			{
				// At least supply one line with the tool name
				cRow++;
				cCol = sCol;
				ckBoxStyle.Apply();
				wks.set_Label(cRow, cCol, "");
				cCol++;
				cols = 3;
				CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Instrument");
			}
			// -------------------
			// DUCERS
			// -------------------
			if (ducers.Count > 0)
			{
				foreach (EDucer dcr in ducers)
				{
					cRow++;
					cCol = sCol;
					ckBoxStyle.Apply();
					wks.set_Label(cRow, cCol, "");
					cCol++;
					cols = 3;
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Transducer");
					cCol += 3;
					cols = 10;
					data = dcr.DucerModelName + ", S/N: " + dcr.DucerSerialNumber + 
						", Size: " + Util.GetFormattedDecimal(dcr.DucerSize) + "\"" + 
						", Freq: " + Util.GetFormattedDecimal(dcr.DucerFrequency,2) + " mHz";
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, data);
				}
			}
			else
			{
				// At least supply one line with the tool name
				cRow++;
				cCol = sCol;
				ckBoxStyle.Apply();
				wks.set_Label(cRow, cCol, "");
				cCol++;
				cols = 3;
				CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Transducer");
			}
			// -------------------
			// CAL BLOCKS
			// -------------------
			if (calBlocks.Count > 0)
			{
				foreach (ECalBlock cbk in calBlocks)
				{
					cRow++;
					cCol = sCol;
					ckBoxStyle.Apply();
					wks.set_Label(cRow, cCol, "");
					cCol++;
					cols = 3;
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Cal. Block");
					cCol += 3;
					cols = 10;
					data = "S/N: " + cbk.CalBlockSerialNumber +
						", " + cbk.CalBlockMaterialAbbr + " " + cbk.CalBlockTypeAbbr + 
						", " + Util.GetFormattedDecimal(cbk.CalBlockCalMin) + "\" - " + 
						cbk.CalBlockCalMax + "\"";
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, data);
				}
			}
			else
			{
				// At least supply one line with the tool name
				cRow++;
				cCol = sCol;
				ckBoxStyle.Apply();
				wks.set_Label(cRow, cCol, "");
				cCol++;
				cols = 3;
				CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Cal. Block");
			}
			// -------------------
			// THERMOS
			// -------------------
			if (thermos.Count > 0)
			{
				foreach (EThermo thm in thermos)
				{
					cRow++;
					cCol = sCol;
					ckBoxStyle.Apply();
					wks.set_Label(cRow, cCol, "");
					cCol++;
					cols = 3;
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Thermo.");
					cCol += 3;
					cols = 10;
					data = "S/N: " + thm.ThermoSerialNumber;
					CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, data);
				}
			}
			else
			{
				// At least supply one line with the tool name
				cRow++;
				cCol = sCol;
				ckBoxStyle.Apply();
				wks.set_Label(cRow, cCol, "");
				cCol++;
				cols = 3;
				CreateMergedCellsLabel(wks, dataStyle, cRow, cCol, rows, cols, "Thermo.");
			}

			// Now add the Other (specify) section
			cRow = sRow;
			cCol = sCol + 14;
			cols = 9;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Other (specify)");
			for (int i = 0; i < totalTools; i++)
			{
				cRow++;
				CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			}
			cCol = 2;
			cols = 13;
			IXlsRange range = wks.NewRange(GetExcelRange(cRow+1, cCol, rows, cols));
			range.Style.Borders.Top.Style = xlsgen.enumBorderStyle.border_thin;
			range.Style.Borders.Top.Color = (int)xlsgen.enumColorPalette.colorBlack;
			range.Apply();
			return cRow;
		}

		private int AddBoilerPlate(IXlsWorksheet wks, string facPhone,
			IXlsStyle labelStyle, IXlsStyle labelBoxStyle, IXlsStyle dataStyle, int startRow)
		{
			int sRow = startRow;
			int sCol = 1;
			int cRow, cCol, rows, cols;
			int tmpRow, tmpCol;
			cRow = sRow; cCol = sCol;
			rows = 1;

			IXlsStyle centeredLabelStyle = labelBoxStyle.Duplicate();
			centeredLabelStyle.Alignment.Horizontal = xlsgen.enumHorizontalAlignment.halign_center;

			// ---------------------
			// Component info
			// ---------------------
			// Component
			cols = 4; 
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Component");
			cCol += cols;
			cols = 6;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Component Type
			cCol = sCol;
			cols = 4;
			cRow++;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Component Type");
			cCol += cols;
			cols = 6;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Component Material
			cCol = sCol;
			cols = 4;
			cRow++;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Component Material");
			cCol += cols;
			cols = 6;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// ---------------------
			// Dimensions
			// ---------------------
			cCol = sCol;
			cRow++;
			tmpRow = cRow;
			cols = 2;

			// OD
			CreateMergedCellsLabel(wks, centeredLabelStyle, cRow, cCol, rows, cols, "OD");
			cRow++;
			rows = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			cRow = tmpRow;
			rows = 1;

			// Tnom
			CreateMergedCellsLabel(wks, centeredLabelStyle, cRow, cCol, rows, cols, "Tnom");
			cRow++;
			rows = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			cRow = tmpRow;
			rows = 1;

			// Tscreen
			CreateMergedCellsLabel(wks, centeredLabelStyle, cRow, cCol, rows, cols, "Tscreen");
			cRow++;
			rows = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			cRow = tmpRow;
			rows = 1;

			// Min
			CreateMergedCellsLabel(wks, centeredLabelStyle, cRow, cCol, rows, cols, "Min");
			cRow++;
			rows = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			cRow = tmpRow;
			rows = 1;

			// Max
			CreateMergedCellsLabel(wks, centeredLabelStyle, cRow, cCol, rows, cols, "Max");
			cRow++;
			rows = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");


			// ---------------------
			// Calibration data
			// ---------------------
			cCol += cols;
			tmpCol = cCol;
			rows = 1;

			// Component temp
			cRow = sRow;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Component Temp");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "°F");

			// Cal Block temp
			cRow++;
			cCol = tmpCol;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Cal. Block Temp");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "°F");

			// Coarse Range
			cRow++;
			cCol = tmpCol;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Coarse Range");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "in");

			// Sweep Range Min
			cRow++;
			cCol = tmpCol;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Sweep Range Min.");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "in");

			// Sweep Range Max
			cRow++;
			cCol = tmpCol;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Sweep Range Max.");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "in");

			// Velocity
			cRow++;
			cCol = tmpCol;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Velocity");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "in/µs");

			// Gain
			cRow++;
			cCol = tmpCol;
			cols = 4;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Gain");
			cCol += cols;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, "dB");

			// ---------------------
			// Calibration Times
			// ---------------------
			cCol += cols;
			tmpCol = cCol;
			rows = 1;

			// Cal In
			cRow = sRow;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Cal In");
			cCol += cols;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Cal Check 1
			cRow++;
			cCol = tmpCol;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Chk 1");
			cCol += cols;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Cal Check 2
			cRow++;
			cCol = tmpCol;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Chk 2");
			cCol += cols;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Cal Out
			cRow++;
			cCol = tmpCol;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Cal Out");
			cCol += cols;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Dose
			cRow++;
			cCol = tmpCol;
			cols = 2;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Dose");
			cCol += cols;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// X-Y Location
			sRow += 8;
			cRow = sRow;
			cCol = sCol;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "X-Y Location");
			cCol += cols;
			cols = 11;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Grid Size
			cRow++;
			cCol = sCol;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Grid Size");
			cCol += cols;
			cols = 11;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// ---------------------
			// Partition Info
			// ---------------------
			// Headings
			cRow = sRow;
			cCol += cols;
			tmpCol = cCol;
			rows = 2;
			cols = 3;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Section");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Starting Row");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Ending Row");

			rows = 1;
			// U/S Ext
			cRow += 2;
			cCol = tmpCol;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "U/S Ext.");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Main
			cRow++;
			cCol = tmpCol;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Main");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// D/S Ext
			cRow++;
			cCol = tmpCol;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "D/S Ext.");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Branch
			cRow++;
			cCol = tmpCol;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Branch");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			// Branch Ext
			cRow++;
			cCol = tmpCol;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Branch Ext.");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
			cCol += cols;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");

			//-------------------
			// Other measurements 
			//-------------------
			cRow += 2;
			cCol = tmpCol;
			cols = 9;
			CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "Other Measurements");

			cRow++;
			labelStyle.Apply();
			wks.set_Label(cRow, cCol, "Thickness");
			cCol += 3;
			wks.set_Label(cRow, cCol, "Description");

			for (int i = 0; i < 12; i++)
			{
				cRow++;
				cCol = tmpCol;
				cols = 2;
				CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
				cCol += cols;
				cols = 7;
				CreateMergedCellsLabel(wks, labelBoxStyle, cRow, cCol, rows, cols, "");
				cCol += cols;
			}

			// Border around sketch area
			cRow = sRow + 2;
			cCol = sCol;
			rows = 20;
			cols = 14;
			BorderAroundRange(wks, cRow, cCol, rows, cols);

			labelStyle.Apply();
			cRow += rows;
			wks.set_Label(cRow, cCol, "Notes");
			rows = 8;
			cols = 23;
			BorderAroundRange(wks, cRow, cCol, rows, cols);

			cRow += rows + 1;
			labelStyle.Apply();
			wks.set_Label(cRow, cCol, "FAC Phone #");
			cCol += 3;
			dataStyle.Apply();
			wks.set_Label(cRow, cCol, facPhone);

			return cRow;
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


		public bool CreateWorkbook()
		{
			if (inspectors.Count == 0)
			{
				this.result = "Can't generate Field Data Sheets -- No inspectors assigned to the current outage.";
				return false;
			}

			string outFilePath = Globals.FactotumDataFolder + "\\" + UserSettings.sets.FieldDataSheetName;

			engine = Start();
			// create a new Excel file, and retrieve a workbook to work with
			wbk = engine.New(outFilePath);

			// delete existing sheets first
			int initialSheetCount = wbk.WorksheetCount;

			int i = 0;
			int inspCount = inspectors.Count;
			foreach (EInspector curInspector in inspectors)
			{
				// create a new worksheet named after the inspector
				wks = wbk.AddWorksheet(curInspector.InspectorName);
				CreateWorksheet(wks, curInspector);
				i++;
				this.bw.ReportProgress(i * 100 / inspCount);
			}

			for (i = 0; i < initialSheetCount; i++)
			{
				wbk.get_WorksheetByIndex(1).Delete();
			}
			wbk.Close();
			return true;
		}
	}
}
