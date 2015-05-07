using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using DowUtils;

namespace Factotum
{
	class PanametricsExporter
	{
		string outFilePath;
		Guid gridID;

		public PanametricsExporter(string outFilePath, Guid gridID)
		{
			this.outFilePath = outFilePath;
			this.gridID = gridID;
		}

		public void ExportGrid()
		{
			StreamWriter sw = new StreamWriter(outFilePath, false);
			sw.WriteLine("[FACTOTUM MERGED]");
			sw.WriteLine();
			sw.WriteLine("    IDENTIFIER      THICKNESS    UNITS    FLAGS    SU #");
			EGrid grid = new EGrid(gridID);
			DataTable dt = EMeasurement.GetForExport(gridID);
			short col;
			short row;
			string identifier;
			decimal? thickness;
			short? svyNumber;
			string units;
			StringBuilder outString;
			short gfirstrow = (short)(grid.GridStartRow == null ? 0 : grid.GridStartRow);
			short gfirstcol = (short)(grid.GridStartCol == null ? 0 : grid.GridStartCol);
			short glastrow = (short)(grid.GridEndRow == null ? 0 : grid.GridEndRow);
			short glastcol = (short)(grid.GridEndCol == null ? 0 : grid.GridEndCol);
			string reading;
			int dtRowCount = dt.Rows.Count;
			if (glastrow > 0 && glastcol > 0 && dtRowCount > 0)
			{
				// Set the data row index to zero initially
				int drIdx = 0;
				DataRow dr = dt.Rows[drIdx];
				col = (short)dr["MsrCol"];
				row = (short)dr["MsrRow"];
				// Cover the entire grid, even empties, which will not have measurements (i.e. data rows)
				for (short gcol = gfirstcol; gcol <= glastcol; gcol++)
				{
					if (gcol > col && drIdx < dtRowCount-1)
					{
						// If the last measurement in a given column is empty, we need to get a new datarow
						drIdx++;
						dr = dt.Rows[drIdx];
						col = (short)dr["MsrCol"];
					}
					for (short grow = gfirstrow; grow <= glastrow; grow++)
					{
						outString = new StringBuilder(80);
						identifier = EMeasurement.GetIdentifierForRowAndCol(grow, gcol);
						// If both the row and column match, we have a non-empty measurement at that grid cell
						if (grow == row && gcol == col)
						{
							thickness = (decimal?)Util.NullForDbNull(dr["MsrThickness"]);
							units = (string)Util.NullForDbNull(dr["SvyUnits"]);
							units = (units == null ? "IN" : units);
							svyNumber = (short?)Util.NullForDbNull(dr["SvyNumber"]);
							svyNumber = (svyNumber == null ? 0 : svyNumber);

							outString.Append(identifier.PadRight(7)); // 2 letters + 5 numbers max
							reading = (thickness == null ?
								"--.---".PadLeft(20) : String.Format("{0:0.000}",thickness).PadLeft(20));
							outString.Append(reading);
							outString.Append(units.PadLeft(9));
							outString.Append("      " + (thickness == null ? "L" : "M") + "-----");
							outString.Append(String.Format("{0:00}",svyNumber).PadLeft(5));
							outString.Append("  ");
							sw.WriteLine(outString.ToString());

							if (drIdx < dtRowCount - 1)
							{
								// Get the next measurement data row and its row and column numbers
								drIdx++;
								dr = dt.Rows[drIdx];
								col = (short)dr["MsrCol"];
								row = (short)dr["MsrRow"];
							}
						}
						else
						{
							// Add an obstruction line at the current grid row/column
							outString.Append(identifier.PadRight(7)); // 2 letters + 5 numbers max
							reading = "--.---".PadLeft(20);
							outString.Append(reading);
							outString.Append("IN".PadLeft(9));
							outString.Append("      " + "L-----");
							outString.Append("00".PadLeft(5));
							outString.Append("  ");
							sw.WriteLine(outString.ToString());
						}
					}
				}
			}
			sw.WriteLine("OK");
			sw.WriteLine();
			sw.Close();
		}
	}
}
