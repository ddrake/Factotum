using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using DowUtils;


namespace Factotum
{
	class RsInstrument : ReportSection
	{

		public RsInstrument(MainReport report, ReportSectionRules rules, int subsections)
			: base(report, rules, subsections)
		{
		}

		public override bool IsIncluded() 
		{
			return (rpt.eDset != null);
		}

		public override bool CanFitSome(PrintPageEventArgs args, float Y) 
		{
			int padding = 5;
			int tablePadding = 2;
			return (args.MarginBounds.Bottom - Y - rpt.footerHeight > 
				rpt.regTextFont.Height * 5 + rpt.boldRegTextFont.Height * 2 + 
				padding * 5 + tablePadding * 7);
		}

		public override bool Print(PrintPageEventArgs args, float Y) 
		{
			// Todo: use measurement graphics for row height?
			Graphics g = args.Graphics;
			Graphics measure = args.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			int leftX = args.MarginBounds.X;
			int centerX = (int)(leftX + args.MarginBounds.Width / 2);
			int rightX = leftX + args.MarginBounds.Width;
			int curX = leftX;

			int padding = 5;
			int tblWidth = 225;
			float startY = Y + padding;
			float curY = startY;

			int tablePadding = 2;
			
			string[,] meter;
			string[,] ducer;
            string[,] calBlock;
            string[,] thermo;

			string InspectorNumber = rpt.eDset.DsetInsID == null ? "N/A" : 
				GetInspectorNumber((Guid)rpt.eDset.DsetInsID);
			
			GetDataArrays(out meter, out ducer, out calBlock, out thermo);

			curY = DrawStringInBox("Inspector #" + InspectorNumber, g, curX, curY, 225, tablePadding, rpt.boldRegTextFont);
			DrawTable(meter, 3, 2, 1, 0, g, curX, curY, 75, 150, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, false, false);

			curY = startY;
			curX = centerX - tblWidth / 2;
			DrawTable(ducer, 5, 2, 1, 0, g, curX, curY, 75, 150, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, false, false);

			curX = rightX - tblWidth;
            curY = DrawTable(calBlock, 4, 2, 1, 0, g, curX, curY, 75, 150, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, false, false);
            curY += padding;
            curY = DrawTable(thermo, 2, 2, 1, 0, g, curX, curY, 75, 150, tablePadding, rpt.boldRegTextFont, rpt.regTextFont, false, false);

			curY += padding;
			hr(args, curY);
			this.Y = curY;

			// We're finished if either there were no measurements or we got them all in.
			return true;
		}

        private void GetDataArrays(out string[,] meter, out string[,] ducer, out string[,] calBlock, out string[,] thermo)
		{
			meter = new string[3, 2];
			ducer = new string[5, 2];
            calBlock = new string[4, 2];
            thermo = new string[2, 2];
            int row, col;
			string meterModel = "N/A", meterSN = "N/A";
			string ducerModel = "N/A", ducerSN = "N/A", ducerFreq = "N/A", ducerSize = "N/A";
			string calBlockType = "N/A", calBlockMaterial = "N/A", calBlockSN = "N/A";
            string thermoSN = "N/A";
            if (rpt.eDset != null)
			{
				// Meter info
				if (rpt.eDset.DsetMtrID != null)
				{
					EMeter mtr = new EMeter(rpt.eDset.DsetMtrID);
					meterModel = mtr.MeterModelName;
					meterSN = mtr.MeterSerialNumber;
				}
				// Transducer info
				if (rpt.eDset.DsetDcrID != null)
				{
					EDucer dcr = new EDucer(rpt.eDset.DsetDcrID);
					// Every ducer has a model assigned so we don't need a null check
					EDucerModel dmd = new EDucerModel(dcr.DucerDmdID);
					ducerModel = dcr.DucerModelName;
					ducerSN = dcr.DucerSerialNumber;
					ducerFreq = Util.GetFormattedDecimal_NA(dmd.DucerModelFrequency);
					ducerSize = Util.GetFormattedDecimal_NA(dmd.DucerModelSize);
				}
                // Cal block info
                if (rpt.eDset.DsetCbkID != null)
                {
                    ECalBlock cbk = new ECalBlock(rpt.eDset.DsetCbkID);
                    calBlockMaterial = cbk.CalBlockMaterialName;
                    calBlockSN = cbk.CalBlockSerialNumber;
                    calBlockType = cbk.CalBlockTypeName;
                }
                // Thermo info
                if (rpt.eDset.DsetThmID != null)
                {
                    EThermo thm = new EThermo(rpt.eDset.DsetThmID);
                    thermoSN = thm.ThermoSerialNumber;
                }
			}

			// Meter Table
			row = 0;
			// Heading row
			col = 0;
			meter[row, col] = "Instrument";

			row++;
			// Model row
			col = 0;
			meter[row, col] = "Model";
			col++;
			meter[row, col] = meterModel;

			row++;
			// S/N row
			col = 0;
			meter[row, col] = "S/N";
			col++;
			meter[row, col] = meterSN;


			// Ducer Table
			row = 0;
			// Heading row
			col = 0;
			ducer[row, col] = "Transducer";

			row++;
			// S/N row
			col = 0;
			ducer[row, col] = "Model";
			col++;
			ducer[row, col] = ducerModel;

			row++;
			// S/N row
			col = 0;
			ducer[row, col] = "S/N";
			col++;
			ducer[row, col] = ducerSN;

			row++;
			// Freq row
			col = 0;
			ducer[row, col] = "Frequency";
			col++;
			ducer[row, col] = ducerFreq + " MHz";

			row++;
			// Size row
			col = 0;
			ducer[row, col] = "Size";
			col++;
			ducer[row, col] = ducerSize + " in";


			// Cal block Table
			row = 0;
			// Heading row
			col = 0;
			calBlock[row, col] = "Calibration Block";

			row++;
			// S/N row
			col = 0;
			calBlock[row, col] = "S/N";
			col++;
			calBlock[row, col] = calBlockSN;

			row++;
			// Type row
			col = 0;
			calBlock[row, col] = "Type";
			col++;
			calBlock[row, col] = calBlockType;

			row++;
			// Material row
			col = 0;
			calBlock[row, col] = "Material";
			col++;
			calBlock[row, col] = calBlockMaterial;

            // Thermo Table
            row = 0;
            // Heading row
            col = 0;
            thermo[row, col] = "Thermometer";

            row++;
            // S/N row
            col = 0;
            thermo[row, col] = "S/N";
            col++;
            thermo[row, col] = thermoSN;

        }
	}
}
