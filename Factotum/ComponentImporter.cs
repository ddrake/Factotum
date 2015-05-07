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
	public partial class ComponentImporter : Form
	{
		[DllImport("xlsgen.dll")]
		static extern IXlsEngine Start();
		EUnit curUnit;
		string[] colNames;
		string[] colHeads;
		// Collections of objects that will be inserted if the user clicks to Import
		EComponentMaterialCollection materialsToInsert;
		EComponentTypeCollection typesToInsert;
		ELineCollection linesToInsert;
		ESystemCollection systemsToInsert;
		// Components that will be inserted if the user clicks to import.
		// These need to have their foreign key ids looked up after the other collections are
		// inserted because they may reference items in those collections 
		EComponentCollection componentsToInsert;

		// A list of pending component changes.  Each list item represents a proposed change to one
		// field of one Component record.
		List<PendingChange> pendingChanges;
		List<string> componentNames;

		// A list of parent key names (display values).  This list is necessary because it's not 
		// possible to set the parent key names in the EComponent, only their key ids, which may 
		// not exist yet.  So what we do is add items to this list at the same time EComponents to their
		// collection so the indices match up.  It's ugly but it works.
		List<ParentKeyName> parentKeyNames;

		// This flag tracks whether or not any Insertions or changes are required to get the database
		// to get the database in synch with the spreadsheet.  It's used to control enabling of the 
		// Import button.
		bool AnyInsertionsOrChanges = false;

		// Constructor -- We MUST have a UnitID because every component belongs to a unit.
		public ComponentImporter(Guid UnitID)
		{
			curUnit = new EUnit(UnitID);
			InitializeComponent();
			InitializeControls();
		}

		// Initialize the form control values
		private void InitializeControls()
		{
			lblSiteName.Text = "Import Components for Facility: " + curUnit.UnitNameWithSite;
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			// Initialize the array of allowed column names.  The first row of the user's spreadsheet
			// must be a subset of this set of names.
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
					tvwComponentChange.Nodes.Clear();
				}
			}
		}

		private bool ProcessFile(string xlsFile)
		{
			IXlsEngine engine = null;
			IXlsWorkbook wbk = null;
			IXlsWorksheet wks = null;
			// Xlsgen needs to create a spreadsheet file even though we're really only using it to read
			// data from the spreadsheet so we'll create a temporary file in the user's local settings temp
			// folder and delete it when we're finished.
			string tempFile = null;
			string tempXlsFile = null;

			// The last row of the spreadsheet that contains a component name
			int lastRow = 0;

			// The indices of the primary and foreign key columns
			int nameIdx = 0;
			int materialIdx = 0;
			int typeIdx = 0;
			int systemIdx = 0;
			int lineIdx = 0;

			// Flags for the foreign key columns
			bool hasMaterial = false;
			bool hasType = false;
			bool hasSystem = false;
			bool hasLine = false;
			
			tempFile = Path.GetTempFileName();
			// XlsGen needs the file to have an xls suffix or it won't work.
			tempXlsFile = tempFile.Substring(0, tempFile.Length - 3) + "xls";
			engine = Start();
			wbk = engine.Open(xlsFile, tempXlsFile);

			// If the dialog result is OK, try to open the workbook and get the first sheet.
			try
			{
				// Get the first sheet.  Any other sheets are ignored.
				wks = wbk.get_WorksheetByIndex(1);

				// Read the first row of the spreadsheet for column headings.
				readColHeadings(wks);

				// We have to have a component name column.
				if (!IsInArray(colHeads, "ComponentName", out nameIdx))
				{
					MessageBox.Show("A column called 'ComponentName' for the component name is required", 
						"Factotum Component Importer", MessageBoxButtons.OK);
					return false;
				}
				// Set flags and get indices for foreign key columns.
				hasMaterial = IsInArray(colHeads, "Material", out materialIdx);
				hasType = IsInArray(colHeads, "Type", out typeIdx);
				hasSystem = IsInArray(colHeads, "System", out systemIdx);
				hasLine = IsInArray(colHeads, "Line", out lineIdx);

				// Read through rows until a row is found that lacks a component name.
				lastRow = GetLastRow(wks, (int)nameIdx);
				if (lastRow <= 1)
				{
					MessageBox.Show("No data rows found in the selected file.", 
						"Factotum Component Importer", MessageBoxButtons.OK);
					return false;
				}

				// Prepare the tree views
				tvwItemAdd.Nodes.Clear();
				tvwComponentChange.Nodes.Clear();

				// Initialize the collections and lists
				materialsToInsert = new EComponentMaterialCollection();
				typesToInsert = new EComponentTypeCollection();
				linesToInsert = new ELineCollection();
				systemsToInsert = new ESystemCollection();
				componentsToInsert = new EComponentCollection();
				pendingChanges = new List<PendingChange>();
				componentNames = new List<string>();
				parentKeyNames = new List<ParentKeyName>();
				// Process each row of the spreadsheet
				for (int row = 2; row <= lastRow; row++)
				{
					if (!ProcessRow(wks, row, hasMaterial, hasType, hasSystem, hasLine,
						(int)nameIdx, materialIdx, typeIdx, systemIdx, lineIdx)) return false;
				}
				if (AnyInsertionsOrChanges)
				{
					btnImport.Enabled = true;
					foreach (TreeNode tn in tvwComponentChange.Nodes) tn.Checked = true;
					tvwComponentChange.ExpandAll();
					tvwItemAdd.ExpandAll();
				}
				else
				{
					MessageBox.Show("All component data in the selected file is already in the system.", 
						"Factotum Component Importer", MessageBoxButtons.OK);
				}
			}
			catch (Exception ex)
			{
				ExceptionLogger.LogException(ex);
				MessageBox.Show("An error occurred while trying to process the selected file.\nThe details have been recorded in 'error.log'.", 
					"Factotum Component Importer", MessageBoxButtons.OK);
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

				// if (wbk != null) wbk.Close();
				// Note: when we called Path.GetTempFileName(), tempFile was actually created, so delete it.
				//if (File.Exists(tempFile)) File.Delete(tempFile);
				//if (File.Exists(tempXlsFile)) File.Delete(tempXlsFile);
			}
			return true;
		}

		private string TruncateTo(string s, int limit)
		{
			if (s == null) return null;
			if (s.Length > limit) return s.Substring(0,limit);
			return s;
		}

		// Process one row of the worksheet.
		private bool ProcessRow(IXlsWorksheet wks, int row,
			bool hasMaterial, bool hasType, bool hasSystem, bool hasLine,
			int nameIdx, int materialIdx, int typeIdx, int systemIdx, int lineIdx)
		{
			string componentName = null, materialName = null, typeName = null; 
			string systemName = null, lineName = null;
			bool existingIsInactive;

			componentName = wks.get_Label(row, nameIdx+1); // should be no way this can be null.
			componentName = TruncateTo(componentName, EComponent.CmpNameCharLimit);
			// Notify the user if we have a duplicate component name.
			if (IsInComponentsCollection(componentName))
			{
				MessageBox.Show("The Component Name " + componentName + " appears more than once in the file.", 
					"Factotum Component Importer", MessageBoxButtons.OK);
				return false;
			}
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// Check for existence of foreign key columns.  If a name is referenced that doesn't
			// exist yet, make an entity object for it and add it to the to-be-inserted collection.
			if (hasMaterial)
			{
				materialName = Util.NullifyEmpty(wks.get_Label(row, materialIdx + 1));
				materialName = TruncateTo(materialName, EComponentMaterial.CmtNameCharLimit);
				if (materialName != null && 
					!EComponentMaterial.NameExistsForSite(
					materialName, null, (Guid)curUnit.UnitSitID, out existingIsInactive) &&
					!IsInMaterialsCollection(materialName))
				{
					//	create a new entity object
					EComponentMaterial eMaterial = new EComponentMaterial();
					//	fill in the value(s)
					eMaterial.CmpMaterialSitID = curUnit.UnitSitID;
					eMaterial.CmpMaterialName = materialName;
					//	add the object to a (to-be-inserted) collection
					materialsToInsert.Add(eMaterial);
					//	add a subnode to the treeview node of the corresponding type.
					AddInsertItemToTreeView("Materials",materialName);
					AnyInsertionsOrChanges = true;
				}
			}
			if (hasType)
			{
				typeName = Util.NullifyEmpty(wks.get_Label(row, typeIdx + 1));
				typeName = TruncateTo(typeName, EComponentType.CtpNameCharLimit);
				if (typeName != null && 
					!EComponentType.NameExistsForSite(
					typeName, null, (Guid)curUnit.UnitSitID, out existingIsInactive) && 
					!IsInTypesCollection(typeName))
				{
					//	create a new entity object
					EComponentType eType = new EComponentType();
					//	fill in the value(s)
					eType.ComponentTypeSitID = curUnit.UnitSitID;
					eType.ComponentTypeName = typeName;
					//	add the object to a (to-be-inserted) collection
					typesToInsert.Add(eType);
					//	add a subnode to the treeview node of the corresponding type.
					AddInsertItemToTreeView("Types",typeName);
					AnyInsertionsOrChanges = true;
				}
			}
			if (hasSystem)
			{
				systemName = Util.NullifyEmpty(wks.get_Label(row, systemIdx + 1));
				systemName = TruncateTo(systemName, ESystem.SysNameCharLimit);
				if (systemName != null && 
					!ESystem.NameExistsForUnit(
					systemName, null, (Guid)curUnit.ID, out existingIsInactive) &&
					!IsInSystemsCollection(systemName))
				{
					//	create a new entity object
					ESystem eSystem = new ESystem();
					//	fill in the value(s)
					eSystem.SystemUntID = curUnit.ID;
					eSystem.SystemName = systemName;
					//	add the object to a (to-be-inserted) collection
					systemsToInsert.Add(eSystem);
					//	add a subnode to the treeview node of the corresponding type.
					AddInsertItemToTreeView("Systems",systemName);
					AnyInsertionsOrChanges = true;
				}
			}
			if (hasLine)
			{
				lineName = Util.NullifyEmpty(wks.get_Label(row, lineIdx + 1));
				lineName = TruncateTo(lineName, ELine.LinNameCharLimit);
				if (lineName != null && 
					!ELine.NameExistsForUnit(
					lineName, null, (Guid)curUnit.ID, out existingIsInactive) && 
					!IsInLinesCollection(lineName))
				{
					//	create a new entity object
					ELine eLine = new ELine();
					//	fill in the value(s)
					eLine.LineUntID = curUnit.ID;
					eLine.LineName = lineName;
					//	add the object to a (to-be-inserted) collection
					linesToInsert.Add(eLine);
					//	add a subnode to the treeview node of the corresponding type.
					AddInsertItemToTreeView("Lines",lineName);
					AnyInsertionsOrChanges = true;
				}
			}
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// New component case -- the component name doesn't exist yet.
			if (!EComponent.NameExistsForUnit(componentName, null, (Guid)curUnit.ID, out existingIsInactive))
			{
				decimal? test;
				//	create a new entity object
				EComponent eComponent = new EComponent();
				
				eComponent.ComponentUntID = curUnit.ID;
				eComponent.ComponentName = componentName;
				eComponent.ComponentDrawing = TruncateTo(TryToGetString(wks, "Drawing", row), EComponent.CmpDrawingCharLimit);
				eComponent.ComponentMisc1 = TruncateTo(TryToGetString(wks, "Misc1", row), EComponent.CmpMisc1CharLimit);
				eComponent.ComponentMisc2 = TruncateTo(TryToGetString(wks, "Misc2", row), EComponent.CmpMisc2CharLimit);
				if (!TryToGetDecimal(wks, "UpMainOd", row, out test)) return false;
				eComponent.ComponentUpMainOd = test;
				if (!TryToGetDecimal(wks, "UpMainTnom", row, out test)) return false;
				eComponent.ComponentUpMainTnom = test;
				if (!TryToGetDecimal(wks, "UpMainTscr", row, out test)) return false;
				eComponent.ComponentUpMainTscr = test;
				if (!TryToGetDecimal(wks, "DnMainOd", row, out test)) return false;
				eComponent.ComponentDnMainOd = test;
				if (!TryToGetDecimal(wks, "DnMainTnom", row, out test)) return false;
				eComponent.ComponentDnMainTnom = test;
				if (!TryToGetDecimal(wks, "DnMainTscr", row, out test)) return false;
				eComponent.ComponentDnMainTscr = test;
				if (!TryToGetDecimal(wks, "BranchOd", row, out test)) return false;
				eComponent.ComponentBranchOd = test;
				if (!TryToGetDecimal(wks, "BranchTnom", row, out test)) return false;
				eComponent.ComponentBranchTnom = test;
				if (!TryToGetDecimal(wks, "BranchTscr", row, out test)) return false;
				eComponent.ComponentBranchTscr = test;
				if (!TryToGetDecimal(wks, "UpExtOd", row, out test)) return false;
				eComponent.ComponentUpExtOd = test;
				if (!TryToGetDecimal(wks, "UpExtTnom", row, out test)) return false;
				eComponent.ComponentUpExtTnom = test;
				if (!TryToGetDecimal(wks, "UpExtTscr", row, out test)) return false;
				eComponent.ComponentUpExtTscr = test;
				if (!TryToGetDecimal(wks, "DnExtOd", row, out test)) return false;
				eComponent.ComponentDnExtOd = test;
				if (!TryToGetDecimal(wks, "DnExtTnom", row, out test)) return false;
				eComponent.ComponentDnExtTnom = test;
				if (!TryToGetDecimal(wks, "DnExtTscr", row, out test)) return false;
				eComponent.ComponentDnExtTscr = test;
				if (!TryToGetDecimal(wks, "BranchExtOd", row, out test)) return false;
				eComponent.ComponentBrExtOd = test;
				if (!TryToGetDecimal(wks, "BranchExtTnom", row, out test)) return false;
				eComponent.ComponentBrExtTnom = test;
				if (!TryToGetDecimal(wks, "BranchExtTscr", row, out test)) return false;
				eComponent.ComponentBrExtTscr = test;
				if (!eComponent.Valid())
				{
					MessageBox.Show(eComponent.ComponentErrMsg, "Factotum");
					return false;
				}
				//	add a subnode to the treeview node of the corresponding type.
				AddInsertItemToTreeView("Components", componentName);

				//	add the object to a (to-be-inserted) collection
				componentsToInsert.Add(eComponent);
				componentNames.Add(componentName);
				// add the parent key names to a list whose indices correspond to those of the
				// to-be-inserted collection so we can get the primary key ids for them once all the
				// parent table records have been inserted.
				parentKeyNames.Add(new ParentKeyName(systemName, lineName, materialName, typeName));
				AnyInsertionsOrChanges = true;
			}
			else
			{
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// Modify component case -- That component already exists.
				EComponent eComponent = EComponent.FindForComponentNameAndUnit(componentName, (Guid)curUnit.ID);

				HandlePossibleComponentChanges_FK(componentName, "System", "System",
					eComponent.ComponentSystemName, systemName);

				HandlePossibleComponentChanges_FK(componentName, "Line", "Line",
					eComponent.ComponentLineName, lineName);

				HandlePossibleComponentChanges_FK(componentName, "Type", "Type",
					eComponent.ComponentTypeName, typeName);

				HandlePossibleComponentChanges_FK(componentName, "Material", "Material",
					eComponent.ComponentMaterialName, materialName);

				HandlePossibleComponentChanges_NonFK_String(componentName, "Drawing", "Drawing",
					eComponent.ComponentDrawing, wks, row);

				HandlePossibleComponentChanges_NonFK_String(componentName, "Misc1", "Misc1",
					eComponent.ComponentMisc1, wks, row);

				HandlePossibleComponentChanges_NonFK_String(componentName, "Misc2", "Misc2",
					eComponent.ComponentMisc2, wks, row);

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "UpMainOd", "U/S Main OD",
					eComponent.ComponentUpMainOd, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "UpMainTnom", "U/S Main Tnom",
					eComponent.ComponentUpMainTnom, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "UpMainTscr", "U/S Main Tscr",
					eComponent.ComponentUpMainTscr, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "DnMainOd", "D/S Main OD",
					eComponent.ComponentDnMainOd, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "DnMainTnom", "D/S Main Tnom",
					eComponent.ComponentDnMainTnom, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "DnMainTscr", "D/S Main Tscr",
					eComponent.ComponentDnMainTscr, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "BranchOd", "Branch OD",
					eComponent.ComponentBranchOd, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "BranchTnom", "Branch Tnom",
					eComponent.ComponentBranchTnom, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "BranchTscr", "Branch Tscr",
					eComponent.ComponentBranchTscr, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "UpExtOd", "U/S Ext OD",
					eComponent.ComponentUpExtOd, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "UpExtTnom", "U/S Ext Tnom",
					eComponent.ComponentUpExtTnom, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "UpExtTscr", "U/S Ext Tscr",
					eComponent.ComponentUpExtTscr, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "DnExtOd", "D/S Ext OD",
					eComponent.ComponentDnExtOd, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "DnExtTnom", "D/S Ext Tnom",
					eComponent.ComponentDnExtTnom, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "DnExtTscr", "D/S Ext Tscr",
					eComponent.ComponentDnExtTscr, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "BranchExtOd", "Branch Ext OD",
					eComponent.ComponentBrExtOd, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "BranchExtTnom", "Branch Ext Tnom",
					eComponent.ComponentBrExtTnom, wks, row)) return false;

				if (!HandlePossibleComponentChanges_NonFK_Decimal(componentName, "BranchExtTscr", "Branch Ext Tscr",
					eComponent.ComponentBrExtTscr, wks, row)) return false;

				componentNames.Add(componentName);
			}
			return true;
		}

		// Try to get a string from the specified column and row of the spreadsheet
		private string TryToGetString(IXlsWorksheet wks, string colHead, int row)
		{
			int itemIdx;
			if (IsInArray(colHeads, colHead, out itemIdx))
				return Util.NullifyEmpty(wks.get_Label(row, itemIdx + 1));
			else return null;
		}

		// Try to get a decimal from the specified column and row of the spreadsheet
		private bool TryToGetDecimal(IXlsWorksheet wks, string colHead, int row, out decimal? result)
		{
			int itemIdx;
			decimal tmpDecimal;
			string tmpString;
			result = null;
			if (IsInArray(colHeads, colHead, out itemIdx))
			{
				tmpString = wks.get_Label(row, itemIdx + 1);
				if (tmpString.Length == 0) return true;

				if (decimal.TryParse(wks.get_Label(row, itemIdx + 1), out tmpDecimal))
					result = tmpDecimal;
				else
				{
					MessageBox.Show("The value in the " + colHead +
					  " column of row " + row + " is not a valid number", "Factotum Component Importer", MessageBoxButtons.OK);					
					return false;
				}
			}
			return true;
		}

		// Add an insert item to the tree view under the appropriate item type.  
		// If this is the first item of that type, add the type node first.
		private void AddInsertItemToTreeView(string TypeKey, string ItemName)
		{
			TreeNode[] nodes;
			string TypeText;
			switch (TypeKey)
			{
				case "Types":
					TypeText = "Component Types";
					break;
				case "Materials":
					TypeText = "Component Materials";
					break;
				default:
					TypeText = TypeKey;
					break;
			}
			nodes = tvwItemAdd.Nodes.Find(TypeKey, false);
			if (nodes.Length > 0)
			{
				// Found the node, so add the child
				nodes[0].Nodes.Add(ItemName);
			}
			else
			{
				// Create the node, then add the child
				TreeNode tn = tvwItemAdd.Nodes.Add(TypeKey, TypeText);
				tn.Nodes.Add(ItemName);
			}
			AnyInsertionsOrChanges = true;
		}

		// Add a component change item to the tree view under the appropriate item type.  
		// If this is the first changed field for that component, add the component node first.
		private void AddComponentChangeToTreeView(PendingChange pc)
		{
			TreeNode[] nodes;
			nodes = tvwComponentChange.Nodes.Find(pc.ComponentName, false);
			if (nodes.Length > 0)
			{
				// Found the node, so add the child
				nodes[0].Nodes.Add(pc.TreeViewNodeText);
			}
			else
			{
				// Create the node, then add the child
				TreeNode tn = tvwComponentChange.Nodes.Add(pc.ComponentName, pc.ComponentName);
				tn.Nodes.Add(pc.TreeViewNodeText);
			}
			AnyInsertionsOrChanges = true;
		}

		// For Non-Foreign key string columns
		// Check if the spreadsheet value matches the current value.  If not, create a pending change
		// and add it to the pending changes list, also add the change to the tree view
		private void HandlePossibleComponentChanges_NonFK_String(string componentName,
			string colHead, string colDisplay, string oldDisplayValue, IXlsWorksheet wks, int row)
		{
			int itemIdx;
			string tempStr;
			if (IsInArray(colHeads, colHead, out itemIdx))
			{
				tempStr = Util.NullifyEmpty(wks.get_Label(row, itemIdx + 1));
				if (!Matches(oldDisplayValue, tempStr))
				{
					PendingChange pc = new PendingChange(componentName, colHead, colDisplay,
						oldDisplayValue, tempStr);
					pendingChanges.Add(pc);
					AddComponentChangeToTreeView(pc);
				}
			}
		}
		// For Non-Foreign key decimal columns
		// Check if the spreadsheet value matches the current value.  If not, create a pending change
		// and add it to the pending changes list, also add the change to the tree view
		private bool HandlePossibleComponentChanges_NonFK_Decimal(string componentName,
			string colHead, string colDisplay, decimal? oldValue, IXlsWorksheet wks, int row)
		{
			int itemIdx;
			string tempStr;
			decimal tmpDecimal = 0;
			if (IsInArray(colHeads, colHead, out itemIdx))
			{
				tempStr = Util.NullifyEmpty(wks.get_Label(row, itemIdx + 1));

				if (tempStr == null && oldValue == null) return true;

				if (tempStr != null && !decimal.TryParse(tempStr, out tmpDecimal))
				{
					MessageBox.Show("The value in the " + colHead +
						" column of row " + row + " is not a valid number", "Factotum Component Importer", MessageBoxButtons.OK);
					return false;
				}
				
				if (tempStr == null)
				{
					PendingChange pc = new PendingChange(componentName, colHead, colDisplay,
						oldValue, null);
					pendingChanges.Add(pc);
					AddComponentChangeToTreeView(pc);
				}
				else if (oldValue == null || Math.Abs((double)(oldValue - tmpDecimal)) >= .001)
				{
					PendingChange pc = new PendingChange(componentName, colHead, colDisplay,
						oldValue, tmpDecimal);
					pendingChanges.Add(pc);
					AddComponentChangeToTreeView(pc);
				}
			}
			return true;
		}

		// For Foreign key columns
		// Check if the spreadsheet value matches the current value.  If not, create a pending change
		// and add it to the pending changes list, also add the change to the tree view
		private void HandlePossibleComponentChanges_FK(string componentName,
			string colHead, string colDisplay, string oldDisplayValue, string newDisplayValue)
		{
			if (!Matches(oldDisplayValue, newDisplayValue))
			{
				PendingChange pc = new PendingChange(componentName, colHead, colDisplay,
					oldDisplayValue, newDisplayValue);
				pendingChanges.Add(pc);
				AddComponentChangeToTreeView(pc);
			}
		}

		// Get the last data row of the spreadsheet.  This is simply the last row that has a 
		// Component name.
		private int GetLastRow(IXlsWorksheet wks, int nameColunm)
		{
			int row = 2;
			while (wks.get_Label(row, nameColunm+1).Length > 0) row++;
			return row - 1;
		}

		// Read the column headings into an array
		private void readColHeadings(IXlsWorksheet wks)
		{
			int col = 0;
			string heading;

			List<string> tempHeadings;
			tempHeadings = new List<string>(20);
			while ((heading = wks.get_Label(1, col + 1)).Length > 0)
			{
				tempHeadings.Add(heading);
				col++;
			}
			colHeads = tempHeadings.ToArray();

		}

		// Check if a string is in an array.  If so, send out the index.
		// Note: this function assumes a 0-based array 
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

		// Check if a Line is in our collection of Lines to be added.
		private bool IsInLinesCollection(string LineName)
		{
			foreach (ELine eline in linesToInsert)
				if (eline.LineName == LineName) return true;
			return false;
		}

		// Check if a System is in our collection of Systems to be added.
		private bool IsInSystemsCollection(string SystemName)
		{
			foreach (ESystem esystem in systemsToInsert)
				if (esystem.SystemName == SystemName) return true;
			return false;
		}

		// Check if a Component Type is in our collection of Types to be added.
		private bool IsInTypesCollection(string TypeName)
		{
			foreach (EComponentType etype in typesToInsert)
				if (etype.ComponentTypeName == TypeName) return true;
			return false;
		}

		// Check if a Component Material is in our collection of Materials to be added.
		private bool IsInMaterialsCollection(string MaterialName)
		{
			foreach (EComponentMaterial ematerial in materialsToInsert)
				if (ematerial.CmpMaterialName == MaterialName) return true;
			return false;
		}

		// Check if a Component is in our collection of Components to be added.
		private bool IsInComponentsCollection(string ComponentName)
		{
			foreach (string cname in componentNames)
				if (cname == ComponentName) return true;
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
				"Name",
				"Material",
				"Type",
				"Line",
				"System",
				"Drawing",
				"Misc1",
				"Misc2",
				"UpMainOd",
				"UpMainTnom",
				"UpMainTscr",
				"DnMainOd",
				"DnMainTnom",
				"DnMainTscr",
				"BranchOd",
				"BranchTnom",
				"BranchTscr",
				"UpExtOd",
				"UpExtTnom",
				"UpExtTscr",
				"DnExtOd",
				"DnExtTnom",
				"DnExtTscr",
				"BranchExtOd",
				"BranchExtTnom",
				"BranchExtTscr"
			};
		}

		// Handle the user's click on the Import button.  This button should only be enabled if
		// a file has been selected and some changes proposed.
		private void btnImport_Click(object sender, EventArgs e)
		{
			ParentKeyName names;

			// Save any parent entities that have been placed on 'to-insert' lists.
			foreach (ESystem eSystem in systemsToInsert) eSystem.Save();
			foreach (ELine eLine in linesToInsert) eLine.Save();
			foreach (EComponentType eType in typesToInsert) eType.Save();
			foreach (EComponentMaterial eMaterial in materialsToInsert) eMaterial.Save();

			// Go through the collection of components to be inserted
			for (int i = 0; i < componentsToInsert.Count; i++)
			{
				// Create a new component
				EComponent eComponent = componentsToInsert[i];
				names = parentKeyNames[i];

				// Get entity objects for each foreign key.
				ESystem eSystem = ESystem.FindForSystemName(names.SystemName);
				ELine eLine = ELine.FindForLineName(names.LineName);
				EComponentType eType = EComponentType.FindForComponentTypeName(names.TypeName);
				EComponentMaterial eMaterial = EComponentMaterial.FindForComponentMaterialName(names.MaterialName);

				// Get the IDs for entity object, if found and assign them to the foreign key field
				// of the component
				if (eSystem != null) eComponent.ComponentSysID = eSystem.ID;
				if (eLine != null) eComponent.ComponentLinID = eLine.ID;
				if (eType != null) eComponent.ComponentCtpID = eType.ID;
				if (eMaterial != null) eComponent.ComponentCmtID = eMaterial.ID;

				DoComponentSave(eComponent);
			}
			// Go through all the nodes in the Component Changes tree to determine which items were checked.
			// Simultaneously step through the pendingChanges list.
			int change = 0;
			foreach (TreeNode node in tvwComponentChange.Nodes)
			{
				// For each component node...
				string componentName = node.Name;
				EComponent eComponent = EComponent.FindForComponentNameAndUnit(
					pendingChanges[change].ComponentName, (Guid)curUnit.ID);
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
							case "Material":
								EComponentMaterial eMaterial = EComponentMaterial.FindForComponentMaterialName(
									pendingChanges[change].NewDisplayValue);
								eComponent.ComponentCmtID = (eMaterial == null ? null : eMaterial.ID);
								break;
							case "Type":
								EComponentType eType = EComponentType.FindForComponentTypeName(
									pendingChanges[change].NewDisplayValue);
								eComponent.ComponentCtpID = (eType == null ? null : eType.ID);
								break;
							case "System":
								ESystem eSystem = ESystem.FindForSystemName(
									pendingChanges[change].NewDisplayValue);
								eComponent.ComponentSysID = (eSystem == null ? null : eSystem.ID);
								break;
							case "Line":
								ELine eLine = ELine.FindForLineName(
									pendingChanges[change].NewDisplayValue);
								eComponent.ComponentLinID = (eLine == null ? null : eLine.ID);
								break;
							case "Drawing":
								eComponent.ComponentDrawing = pendingChanges[change].NewDisplayValue;
								break;
							case "Misc1":
								eComponent.ComponentMisc1 = pendingChanges[change].NewDisplayValue;
								break;
							case "Misc2":
								eComponent.ComponentMisc2 = pendingChanges[change].NewDisplayValue;
								break;
							case "UpMainOd":
								eComponent.ComponentUpMainOd = pendingChanges[change].NewDecimalValue;
								break;
							case "UpMainTnom":
								eComponent.ComponentUpMainTnom = pendingChanges[change].NewDecimalValue;
								break;
							case "UpMainTscr":
								eComponent.ComponentUpMainTscr = pendingChanges[change].NewDecimalValue;
								break;
							case "DnMainOd":
								eComponent.ComponentDnMainOd = pendingChanges[change].NewDecimalValue;
								break;
							case "DnMainTnom":
								eComponent.ComponentDnMainTnom = pendingChanges[change].NewDecimalValue;
								break;
							case "DnMainTscr":
								eComponent.ComponentDnMainTscr = pendingChanges[change].NewDecimalValue;
								break;
							case "BranchOd":
								eComponent.ComponentBranchOd = pendingChanges[change].NewDecimalValue;
								break;
							case "BranchTnom":
								eComponent.ComponentBranchTnom = pendingChanges[change].NewDecimalValue;
								break;
							case "BranchTscr":
								eComponent.ComponentBranchTscr = pendingChanges[change].NewDecimalValue;
								break;
							case "UpExtOd":
								eComponent.ComponentUpExtOd = pendingChanges[change].NewDecimalValue;
								break;
							case "UpExtTnom":
								eComponent.ComponentUpExtTnom = pendingChanges[change].NewDecimalValue;
								break;
							case "UpExtTscr":
								eComponent.ComponentUpExtTscr = pendingChanges[change].NewDecimalValue;
								break;
							case "DnExtOd":
								eComponent.ComponentDnExtOd = pendingChanges[change].NewDecimalValue;
								break;
							case "DnExtTnom":
								eComponent.ComponentDnExtTnom = pendingChanges[change].NewDecimalValue;
								break;
							case "DnExtTscr":
								eComponent.ComponentDnExtTscr = pendingChanges[change].NewDecimalValue;
								break;
							case "BranchExtOd":
								eComponent.ComponentBrExtOd = pendingChanges[change].NewDecimalValue;
								break;
							case "BranchExtTnom":
								eComponent.ComponentBrExtTnom = pendingChanges[change].NewDecimalValue;
								break;
							case "BranchExtTscr":
								eComponent.ComponentBrExtTscr = pendingChanges[change].NewDecimalValue;
								break;
							default:
								throw new Exception("Unexpected Column name '" + pendingChanges[change].ColHead +
									"' found while processing component updates.");
						}
					}
					change++;
				}
				if (somethingChanged)
				{
					DoComponentSave(eComponent);
				}
			}
			MessageBox.Show("Components Imported Successfully", "Factotum Component Importer", MessageBoxButtons.OK);
			Close();
		}

		private void DoComponentSave(EComponent eComponent)
		{
			if (eComponent.ComponentUpMainTnom != null && eComponent.ComponentUpMainOd != null)
			{
				EPipeSchedule ps = EPipeSchedule.FindForOdAndTnom(
					(decimal)eComponent.ComponentUpMainOd, (decimal)eComponent.ComponentUpMainTnom);
				if (ps == null) eComponent.ComponentPslID = null;
				else eComponent.ComponentPslID = ps.ID;
			}
			// Insert the component
			eComponent.Save();
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
				tvwComponentChange.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(tvwComponentChange_AfterCheck);
				foreach (TreeNode child in e.Node.Nodes)
					child.Checked = isChecked;

				tvwComponentChange.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(tvwComponentChange_AfterCheck);
			}
		}


		// A SUBCLASS for managing parent key names for use during insertion of components
		private class ParentKeyName
		{
			public string SystemName;
			public string LineName;
			public string MaterialName;
			public string TypeName;
			public ParentKeyName(string systemName, string lineName, string materialName, string typeName)
			{
				SystemName = systemName;
				LineName = lineName;
				MaterialName = materialName;
				TypeName = typeName;
			}
		}

		// A SUBCLASS for managing pending changes to components.
		private class PendingChange
		{
			public string ComponentName;
			public string ColHead;
			public string ColumnDisplayName;
			public string OldDisplayValue;
			public string NewDisplayValue;
			public decimal? OldDecimalValue;
			public decimal? NewDecimalValue;

			// We have two public constructors which call a private one, as a sort of cheesy way
			// to use this object for both changes to string values and changes to decimal values.
			public PendingChange(string componentName, string colHead,
				string columnDisplayName, string oldDisplayValue, string newDisplayValue)
				: this(componentName, colHead, columnDisplayName, oldDisplayValue, newDisplayValue, null, null)
			{ }

			public PendingChange(string componentName, string colHead,
				string columnDisplayName, decimal? oldDecimalValue, decimal? newDecimalValue)
				: this(componentName, colHead, columnDisplayName, null, null, oldDecimalValue, newDecimalValue)
			{
				OldDisplayValue = (oldDecimalValue == null ? null : oldDecimalValue.ToString());
				NewDisplayValue = (newDecimalValue == null ? null : newDecimalValue.ToString());
			}

			private PendingChange(string componentName, string colHead, string columnDisplayName,
				string oldDisplayValue, string newDisplayValue, decimal? oldDecimalValue, decimal? newDecimalValue)
			{
				ComponentName = componentName;
				ColHead = colHead;
				ColumnDisplayName = columnDisplayName;
				OldDecimalValue = oldDecimalValue;
				NewDecimalValue = newDecimalValue;
				OldDisplayValue = oldDisplayValue;
				NewDisplayValue = newDisplayValue;
			}

			// Get the text to insert in a component change TreeNode
			public string TreeViewNodeText
			{
				get
				{
					string tempOld = (OldDisplayValue != null && OldDisplayValue.Length > 0 ?
						OldDisplayValue : "<Blank>");
					string tempNew = (NewDisplayValue != null && NewDisplayValue.Length > 0 ?
						NewDisplayValue : "<Blank>");
					return ColumnDisplayName + ": " + tempOld + " --> " + tempNew;
				}
			}
		}

		// End Main class
	}

}