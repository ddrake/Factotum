using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using xlsgen;
using System.IO;
using DowUtils;

namespace Factotum
{
	public partial class ReportDefinitionImporter : Form
	{
		[DllImport("xlsgen.dll")]
		static extern IXlsEngine Start();
		EOutage curOutage;
		EUnit curUnit;
		string[] colNames;
		string[] colHeads;
		// Collections of objects that will be inserted if the user clicks to Import
		EInspectedComponentCollection reportsToInsert;

		// A list of pending report definition changes.  Each list item represents a proposed change to one
		// field of one InspectedComponent record.
		List<PendingChange> pendingChanges;

		List<string> reportNames;
		List<string> componentNames;

		// This flag tracks whether or not any Insertions or changes are required to get the database
		// in synch with the spreadsheet.  It's used to control enabling of the Import button.
		bool AnyInsertionsOrChanges = false;

		// Constructor -- We MUST have an OutageID because every InspectedComponent belongs to an Outage.
		public ReportDefinitionImporter(Guid OutageID)
		{
			curOutage = new EOutage(OutageID);
			curUnit = new EUnit(curOutage.OutageUntID);
			InitializeComponent();
			InitializeControls();
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			lblSiteName.Text = "Import for Outage: " + 
				curUnit.UnitNameWithSite + " - " + curOutage.OutageName;
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			// Initialize the array of allowed column names.  The first row of the user's spreadsheet
			// must include all these names (in any order).
			initColNames();
			btnImport.Enabled = false;
		}

		// The user clicked to select a file and preview changes
		private void btnSelectAndPreview_Click(object sender, EventArgs e)
		{
			// Setup and show the file open dialog
			openFileDialog1.InitialDirectory = Globals.FactotumDataFolder;
			openFileDialog1.FileName = null;
			openFileDialog1.Filter = "Excel files *.xls|*.xls";
			DialogResult rslt = openFileDialog1.ShowDialog();
			if (rslt == DialogResult.OK)
			{
				if (!ProcessFile(openFileDialog1.FileName))
				{
					tvwItemAdd.Nodes.Clear();
					tvwReportChange.Nodes.Clear();
				}
			}
		}

		private bool ProcessFile(string xlsFile)
		{
			IXlsEngine engine;
			IXlsWorkbook wbk = null;
			IXlsWorksheet wks;
			// Xlsgen needs to create a spreadsheet file even though we're really only using it to read
			// data from the spreadsheet so we'll create a temporary file in the user's local settings temp
			// folder and delete it when we're finished.
			string tempFile = null;
			string tempXlsFile = null;

			// The last row of the spreadsheet that contains a report name
			int lastRow;

			// The indices of the columns
			int reportNameIdx = 0;
			int workOrderIdx = 0;
			int componentNameIdx = 0;

			// If the dialog result is OK, try to open the workbook and get the first sheet.
			try
			{
				tempFile = Path.GetTempFileName();
				// XlsGen needs the file to have an xls suffix or it won't work.
				tempXlsFile = tempFile.Substring(0, tempFile.Length - 3) + "xls";
				engine = Start();
				wbk = engine.Open(xlsFile, tempXlsFile);

				// Get the first sheet.  Any other sheets are ignored.
				wks = wbk.get_WorksheetByIndex(1);

				// Read the first row of the spreadsheet for column headings.
				readColHeadings(wks);

				// We have to have all three columns.
				if (!IsInArray(colHeads, "ReportName", out reportNameIdx))
				{
					MessageBox.Show("A column called 'ReportName' for the Report Name is required",
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
					return false;
				}

				if (!IsInArray(colHeads, "WorkOrder", out workOrderIdx))
				{
					MessageBox.Show("A column called 'WorkOrder' for the Work Order Number is required",
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
					return false;
				}
				if (!IsInArray(colHeads, "ComponentName", out componentNameIdx))
				{
					MessageBox.Show("A column called 'ComponentName' for the Component Name is required",
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
					return false;
				}

				// Read through rows until the component name is an empty string.
				// This function will return false if a partial row is found.
				if(!GetLastRow(wks, reportNameIdx, workOrderIdx, componentNameIdx,out lastRow)) return false;
				
				if (lastRow <= 1)
				{
					MessageBox.Show("No data rows found in the selected file.",
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
					return false;
				}

				// Prepare the tree views
				tvwItemAdd.Nodes.Clear();
				tvwReportChange.Nodes.Clear();

				// Initialize the collections and lists
				reportsToInsert = new EInspectedComponentCollection();
				pendingChanges = new List<PendingChange>();
				reportNames = new List<string>();
				componentNames = new List<string>();

				// Process each row of the spreadsheet
				for (int row = 2; row <= lastRow; row++)
				{
					if (!ProcessRow(wks, row, reportNameIdx, workOrderIdx, componentNameIdx)) return false;
				}
				if (AnyInsertionsOrChanges)
				{
					btnImport.Enabled = true;
					foreach (TreeNode tn in tvwReportChange.Nodes) tn.Checked = true;
					tvwReportChange.ExpandAll();
					tvwItemAdd.ExpandAll();
				}
				else
				{
					MessageBox.Show("All Report Definition data in the selected file is already in the system.",
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
				}
			}
			catch (Exception ex)
			{
				ExceptionLogger.LogException(ex);
				MessageBox.Show("An error occurred while trying to process the selected file.\nThe details have been recorded in 'error.log'.",
					"Factotum Report Definition Importer", MessageBoxButtons.OK);
			}
			finally
			{
				// In Xlsgen 2.0 and above, 
				// Workbooks are automatically closed when the engine goes out of scope.

				// When I explicitly closed the workbook here, I was getting a sporadic run-time error
				// System.AccessViolationException: Attempted to read or write protected memory. 
				// This is often an indication that other memory is corrupt.
				// at xlsgen.IXlsWorkbook.Close()

				// I wasn't able to generate this error in debug mode, just in release mode.
				// I can't reproduce the error now that the explicit Close() is removed.
				// Note: since we're not explicitly closing, we can't delete the tempfiles. Oh well...

				//if (wbk != null) wbk.Close();
				// Note: when we called Path.GetTempFileName(), tempFile was actually created, so delete it.
				//if (File.Exists(tempFile)) File.Delete(tempFile);
				//if (File.Exists(tempXlsFile)) File.Delete(tempXlsFile);
			}
			return true;
		}

		private string TruncateTo(string s, int limit)
		{
			if (s == null) return null;
			if (s.Length > limit) return s.Substring(0, limit);
			return s;
		}

		// -----------------------------------------------------
		// Process one row of the worksheet.
		// -----------------------------------------------------
		private bool ProcessRow(IXlsWorksheet wks, int row,
			int reportNameIdx, int workOrderIdx, int componentNameIdx)
		{
			string reportName, workOrder, componentName; 
			bool existingIsInactive;

			componentName = wks.get_Label(row, componentNameIdx + 1); // should be no way this can be null.
			componentName = TruncateTo(componentName, EComponent.CmpNameCharLimit);
			reportName = wks.get_Label(row, reportNameIdx + 1); // should be no way this can be null.
			reportName = TruncateTo(reportName, EInspectedComponent.IscNameCharLimit);
			workOrder = wks.get_Label(row, workOrderIdx + 1); // should be no way this can be null.
			workOrder = TruncateTo(workOrder, EInspectedComponent.IscWorkOrderCharLimit);

			// Notify the user if we have a duplicate component name.
			if (IsReportNameInList(reportName))
			{
				MessageBox.Show("The Report Name " + reportName + " appears more than once in the file.",
					"Factotum Report Definition Importer", MessageBoxButtons.OK);
				return false;
			}
			// Notify the user if we have a duplicate component name.
			if (IsComponentNameInList(componentName))
			{
				MessageBox.Show("The Component Name " + componentName + " appears more than once in the file.",
					"Factotum Report Definition Importer", MessageBoxButtons.OK);
				return false;
			}
			// If a component name is referenced that doesn't exist yet
			if (!EComponent.NameExistsForUnit(
				componentName, null, (Guid)curUnit.ID, out existingIsInactive))
			{
				MessageBox.Show("Component Name " + componentName + " in row " + row + " is not defined for this facility.",
					"Factotum Report Definition Importer", MessageBoxButtons.OK);
				return false;
			}

			// Try to get an existing report for modification
			EInspectedComponent eReport = EInspectedComponent.FindForComponentName((Guid)curOutage.ID, componentName);
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// New report case -- no report definition exists for that component yet.
			if (eReport == null)
			{
				if (EInspectedComponent.NameExistsForOutage(reportName, curOutage.ID, null))
				{
					MessageBox.Show("Unable to add a report definition for component '" +
								componentName + "' in row " + row + "\r\nbecause the Report Name specified '" +
								reportName + "' is already being used for a different component.",
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
					return false;
				}

				//	create a new entity object
				eReport = new EInspectedComponent();
				EComponent eComponent = EComponent.FindForComponentNameAndUnit(componentName, (Guid)curUnit.ID);
				// Don't set the outage ID yet.  It's used by the entity to get a new EDS number, 
				// so we need to set it right after the previous entity was saved.
				eReport.InspComponentCmpID = eComponent.ID;
				eReport.InspComponentName = reportName;
				eReport.InspComponentWorkOrder = workOrder;

				//	add a subnode to the treeview node.
				AddInsertItemToTreeView("Report: " + reportName + " for Component: " + componentName);

				//	add the object to a (to-be-inserted) collection
				reportsToInsert.Add(eReport);

				AnyInsertionsOrChanges = true;
			}
			else
			{
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// Modify report case -- A report definition already exists for that component.

				int itemIdx;
				string tempStr;
				string colHead;
				string colDisplay;
				string oldDisplayValue;

				colHead = "ReportName";
				colDisplay = "Report Name";
				oldDisplayValue = eReport.InspComponentName;
				if (IsInArray(colHeads, colHead, out itemIdx))
				{
					tempStr = wks.get_Label(row, itemIdx + 1);
					if (!Matches(oldDisplayValue, tempStr))
					{
						// The user is trying to change the report name for the component.
						if (EInspectedComponent.NameExistsForOutage(reportName, curOutage.ID, null))
						{
							MessageBox.Show("Unable to modify the report definition for component '" +
								componentName + "' in row " + row + "\r\nbecause the Report Name specified '" +
								reportName + "' is already being used for a different component.",
								"Factotum Report Definition Importer", MessageBoxButtons.OK);
							return false;
						}
 
						PendingChange pc = new PendingChange(componentName, colHead, colDisplay,
							oldDisplayValue, tempStr);
						pendingChanges.Add(pc);
						AddReportChangeToTreeView(pc);
					}
				}

				colHead = "WorkOrder";
				colDisplay = "Work Order";
				oldDisplayValue = eReport.InspComponentWorkOrder;
				if (IsInArray(colHeads, colHead, out itemIdx))
				{
					tempStr = wks.get_Label(row, itemIdx + 1);
					if (!Matches(oldDisplayValue, tempStr))
					{
						PendingChange pc = new PendingChange(componentName, colHead, colDisplay,
							oldDisplayValue, tempStr);
						pendingChanges.Add(pc);
						AddReportChangeToTreeView(pc);
					}
				}
			}
			componentNames.Add(componentName);
			reportNames.Add(reportName);
			return true;
		}

		// Add an insert item to the tree view under the appropriate item type.  
		// If this is the first item of that type, add the type node first.
		private void AddInsertItemToTreeView(string ItemName)
		{
			// Found the node, so add the child
			tvwItemAdd.Nodes.Add(ItemName);
			AnyInsertionsOrChanges = true;
		}

		// Add a report change item to the tree view under the appropriate component name.  
		// If this is the first changed field for that report, add the component name node first.
		private void AddReportChangeToTreeView(PendingChange pc)
		{
			TreeNode[] nodes;
			nodes = tvwReportChange.Nodes.Find(pc.ComponentName, false);
			if (nodes.Length > 0)
			{
				// Found the node, so add the child
				nodes[0].Nodes.Add(pc.TreeViewNodeText);
			}
			else
			{
				// Create the node, then add the child
				TreeNode tn = tvwReportChange.Nodes.Add(pc.ComponentName, pc.ComponentName);
				tn.Nodes.Add(pc.TreeViewNodeText);
			}
			AnyInsertionsOrChanges = true;
		}

		// Get the last data row of the spreadsheet.  This is simply the last row that has all three  
		// required values.  If a row is found that is missing any of the three, an exception is raised.
		private bool GetLastRow(IXlsWorksheet wks, int reportNameColumn, int workOrderColumn, 
			int componentNameColumn, out int lastRow)
		{

			int row = 2;
			lastRow = 0;

			string reportName = wks.get_Label(row, reportNameColumn + 1);
			string workOrder = wks.get_Label(row, workOrderColumn + 1);
			string componentName = wks.get_Label(row, componentNameColumn + 1);
			while (reportName.Length > 0 || workOrder.Length > 0 || componentName.Length > 0)
			{
				if (reportName.Length == 0 || workOrder.Length == 0 || componentName.Length == 0)
				{
					MessageBox.Show("Missing required information in row " + row,
						"Factotum Report Definition Importer", MessageBoxButtons.OK);
					return false;
				}
				row++;
				reportName = wks.get_Label(row, reportNameColumn + 1);
				workOrder = wks.get_Label(row, workOrderColumn + 1);
				componentName = wks.get_Label(row, componentNameColumn + 1);
			}
			lastRow = row - 1;
			return true;
		}

		// Read the column headings into an array
		private void readColHeadings(IXlsWorksheet wks)
		{
			int col = 0;
			//int idx = 0;
			string heading;

			List<string> tempHeadings;
			tempHeadings = new List<string>(20);
			while ((heading = wks.get_Label(1, col + 1)).Length > 0)
			{
				tempHeadings.Add(heading);
				col++;
			}
			colHeads = tempHeadings.ToArray();


			//int idx = 0;
			//string heading;
			//// First check to be sure that all column headings are valid.
			//while ((heading = wks.get_Label(1,col+1)).Length > 0) 
			//{
			//   if (!IsInArray(colNames, heading, out idx)) 
			//   {
			//      return false;
			//   }
			//   col++;
			//}
			//// Add the column headings to the array
			//int cols = col;
			//colHeads = new string[cols];
			//for (col = 0; col < cols; col++) colHeads[col] = wks.get_Label(1, col+1);
			//return true;
		}

		// Check if a string is in an array.  If so, send out the index.
		// Note: this function assumes a 1-based array
		private bool IsInArray(string[] ar, string item, out int index)
		{
			for (int i = 0; i < ar.Length; i++)
			{
				if (ar[i].ToUpper() == item.ToUpper())
				{
					index = i;
					return true;
				}
			}
			index = 0;
			return false;
		}

		// Check if a Report Name is already in our collection of Reports to be added.
		private bool IsReportNameInList(string ReportName)
		{
			foreach (string report in reportNames)
			{
				if (report == ReportName) return true;
			}

			return false;
		}

		// Check if a Component Name is already in our collection of Reports to be added.
		private bool IsComponentNameInList(string ComponentName)
		{
			foreach (string component in componentNames)
			{
				if (component == ComponentName) return true;
			}

			return false;
		}

		// Check if two strings 'match'.  They 'match' if they are both null or are equal except for 
		// possible casing differences.
		private bool Matches(string s1, string s2)
		{
			if (s1 == null && s2 == null) return true;
			else if (s1 == null || s2 == null) return false;
			else if (s1.ToUpper() == s2.ToUpper()) return true;
			else return false;
		}

		// Initialize the column names array that contains all allowed column names
		private void initColNames()
		{
			colNames = new string[] { 
				"ReportName",
				"WorkOrder",
				"ComponentName"
			};
		}

		// Handle the user's click on the Import button.  This button should only be enabled if
		// a file has been selected and some changes proposed.
		private void btnImport_Click(object sender, EventArgs e)
		{
			// Save any parent entities that have been placed on 'to-insert' lists.
			// Go through the collection of components to be inserted
			for (int i = 0; i < reportsToInsert.Count; i++)
			{
				// Create a new report
				EInspectedComponent eReport = reportsToInsert[i];

				// Set the Outage ID -- this causes a new sequential EDS number to be created.
				eReport.InspComponentOtgID = curOutage.ID;

				// Insert the report 
				eReport.Save();

			}
			// Go through all the nodes in the Component Changes tree to determine which items were checked.
			// Simultaneously step through the pendingChanges list.
			int change = 0;
			foreach (TreeNode node in tvwReportChange.Nodes)
			{
				// For each component node...
				string componentName = node.Name;
				EInspectedComponent eReport = EInspectedComponent.FindForComponentName((Guid)curOutage.ID, pendingChanges[change].ComponentName);
				bool somethingChanged = false;
				foreach (TreeNode child in node.Nodes)
				{
					// For each change previewed for the current component
					if (child.Checked)
					{
						somethingChanged = true;

						// Update the entity object with the values stored in each pending change.
						switch (pendingChanges[change].ColHead)
						{
							case "ReportName":
								eReport.InspComponentName = pendingChanges[change].NewDisplayValue;
								break;
							case "WorkOrder":
								eReport.InspComponentWorkOrder = pendingChanges[change].NewDisplayValue;
								break;
							default:
								throw new Exception("Unexpected Column name");
						}
					}
					change++;
				}
				if (somethingChanged)
				{
					eReport.Save();
				}
			}
			MessageBox.Show("Reports Imported Successfully", "Factotum");
			Close();
		}

		// Close the form
		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		// If a component is checked or un-checked, check or un-check all change items for that component.
		private void tvwComponentChange_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Level == 0)
			{
				bool isChecked = e.Node.Checked;
				tvwReportChange.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(tvwComponentChange_AfterCheck);
				foreach (TreeNode child in e.Node.Nodes)
					child.Checked = isChecked;

				tvwReportChange.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(tvwComponentChange_AfterCheck);
			}
		}

		// A subclass for managing pending changes to components.
		private class PendingChange
		{
			public string ComponentName;
			public string ColHead;
			public string ColumnDisplayName;
			public string OldDisplayValue;
			public string NewDisplayValue;

			// We have two public constructors which call a private one, as a sort of cheesy way
			// to use this object for both changes to string values and changes to decimal values.

			public PendingChange(string componentName, string colHead, string columnDisplayName,
				string oldDisplayValue, string newDisplayValue)
			{
				ComponentName = componentName;
				ColHead = colHead;
				ColumnDisplayName = columnDisplayName;
				OldDisplayValue = oldDisplayValue;
				NewDisplayValue = newDisplayValue;
			}

			// Get the text to insert in a component change TreeNode
			public string TreeViewNodeText
			{
				get
				{
					return ColumnDisplayName + ": " + OldDisplayValue + " --> " + NewDisplayValue;
				}
			}
		}
		// End Main class
	}

}