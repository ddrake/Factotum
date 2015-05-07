using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class ReportValidator : Form, IEntityEditForm
	{
		private EInspectedComponent curReport;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curReport; }
		}

		public ReportValidator(Guid inspectedComponentID)
		{
			InitializeComponent();
			curReport = new EInspectedComponent((Guid?)inspectedComponentID);
			InitializeControls();
			DoValidation();
		}
		// Initialize the form control values
		private void InitializeControls()
		{
			SetControlValues();
			dgvInfo.AllowUserToResizeColumns = true;
		}
		// Set the form controls to the unit object values.
		private void SetControlValues()
		{
			updateHeaderLabel();
		}

		private void updateHeaderLabel()
		{
			lblReportName.Text = "Validating Report: " + curReport.InspComponentName;
			DowUtils.Util.CenterControlHorizInForm(lblReportName, this);
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnValidate_Click(object sender, EventArgs e)
		{
			DoValidation();			
		}

		private void AddLine(string inspection, string dataset, string message)
		{
			dgvInfo.Rows.Add(new object[] { inspection, dataset, message });
		}

		public void DoValidation()
		{
			// We need to do this, because we may be trying to refresh and the current
			// report data may have changed.
			Guid reportID = (Guid)curReport.ID;
			curReport = new EInspectedComponent(reportID);

			dgvInfo.Visible = true;
			dgvInfo.Rows.Clear();
			EOutage outage = new EOutage(curReport.InspComponentOtgID);
			EComponent component = new EComponent(curReport.InspComponentCmpID);
			EInspectionCollection inspections = EInspection.ListByReportOrderForInspectedComponent((Guid)curReport.ID,false);
			EInspectorCollection contribInspectors = EInspector.ListForInspectedComponent((Guid)curReport.ID,true);
			bool gridProcedureRequired = false;
            bool warnEpriRecommended = UserSettings.sets.ValidateEpriRecommended;
            bool warnTemperatures = UserSettings.sets.ValidateTemperatures;
            bool warnNoUpMain = UserSettings.sets.ValidateNoUpMain;
            bool warnNoUpExt = UserSettings.sets.ValidateNoUpExt;
            bool warnNoDnExt = UserSettings.sets.ValidateNoDnExt;
            bool gridColCount_UseCeiling = UserSettings.sets.ValidateGridColCountWithCeiling;
            bool gridDsExtRowCount_UseCeiling = UserSettings.sets.ValidateDsExtRowCountWithCeiling;

			if (outage.OutageClpID == null)
			{
				AddLine("N/A", "N/A", "No Calibration procedure was specified for the outage");
			}
			if (outage.OutageCptID == null)
			{
				AddLine("N/A", "N/A", "No couplant type was specified for the outage");
			}
			if (outage.OutageCouplantBatch == null)
			{
				AddLine("N/A", "N/A", "No couplant batch was specified for the outage");
			}
			if (outage.OutageStartedOn == null)
			{
				AddLine("N/A", "N/A", "No start date was specified for the outage");
			}
			if (outage.OutageFacPhone == null)
			{
				AddLine("N/A", "N/A", "No FAC phone # was specified for the outage");
			}
			if (curReport.InspComponentInsID == null)
			{
				AddLine("N/A", "N/A", "No reviewer was specified for the report");
			}
			else
			{
				foreach (EInspector inspector in contribInspectors)
				{
					if (curReport.InspComponentInsID == inspector.ID)
					{
						AddLine("N/A", "N/A", "The reviewer for the report should not be a participant in inspections");
						break;
					}
				}
			}
			if (!curReport.InspComponentIsUtFieldComplete)
			{
				AddLine("N/A", "N/A", "The report is not flagged as 'UT Field Complete'");
			}
			if (curReport.InspComponentPageCountOverride == null)
			{
				AddLine("N/A", "N/A", "The number of pages for the report has not been set");
			}

			if (component.ComponentLinID == null)
			{
				AddLine("N/A", "N/A", "The line has not been specified for the component");
			}
			if (component.ComponentPslID == null)
			{
				AddLine("N/A", "N/A", "No pipe schedule was set for the component");
			}
			if (component.ComponentUpMainOd == null)
			{
				AddLine("N/A", "N/A", "No Main (U/S Main) OD was set for the component");
			}
			if (component.ComponentUpMainTnom == null)
			{
				AddLine("N/A", "N/A", "No Main (U/S Main) Tnom was set for the component");
			}
			if (component.ComponentUpMainTscr == null)
			{
				AddLine("N/A", "N/A", "No Main (U/S Main) Tscr was set for the component");
			}
			if (component.ComponentHasDs && !component.DnMainThicknessesDefined)
			{
				AddLine("N/A", "N/A", "The component has been flagged as having a downstream section, but downstream main dimensions have not been set");
			}
			if (component.ComponentHasBranch && !component.BranchThicknessesDefined)
			{
				AddLine("N/A", "N/A", "The component has been flagged as having a branch, but branch dimensions have not been set");
			}
			if (component.ComponentHasBranch && !component.BranchExtThicknessesDefined)
			{
				AddLine("N/A", "N/A", "The component has been flagged as having a branch, but branch extension dimensions have not been set");
			}
			
			foreach (EInspection inspection in inspections)
			{
				EGrid grid = null;
				string inspName = inspection.InspectionName;
				if (inspection.InspectionPersonHours == null)
				{
					AddLine(inspName, "N/A", "Inspector hours were not recorded");
				}
				
				if (inspection.InspectionHasGrid)
				{
					gridProcedureRequired = true;
					grid = new EGrid(inspection.GridID);

					EpriGrid epriGrid = new EpriGrid();

					if (grid.GridAxialDistance == null)
						AddLine(inspName, "N/A", "No axial distance was specified for the grid");
			
					if (grid.GridRadialDistance == null)
						AddLine(inspName, "N/A", "No radial distance was specified for the grid");

					if (component.ComponentUpMainOd != null)
					{
						// Get the minimum od looking at all parts of the component
						decimal minComponentDiameter =
							Math.Min((decimal)component.ComponentUpMainOd,
							(component.ComponentDnMainOd == null ? decimal.MaxValue : (decimal)component.ComponentDnMainOd));

						minComponentDiameter =
							Math.Min((decimal)minComponentDiameter,
							(component.ComponentBranchOd == null ? decimal.MaxValue : (decimal)component.ComponentBranchOd));

						decimal epriMax = epriGrid.GetMaxGridForDiameter(minComponentDiameter);
						decimal epriRecommended = epriGrid.GetRecommendedGridForDiameter(minComponentDiameter);
						// Compare the axial distance to the EPRI guidelines
						if (grid.GridAxialDistance != null && grid.GridAxialDistance > epriMax)
							AddLine(inspName, "N/A", "The Grid axial distance exceeds the EPRI MAXIMUM of " + epriMax + " in. for the component min OD");
						else if (warnEpriRecommended && grid.GridAxialDistance != null 
                                && grid.GridAxialDistance > epriRecommended)
							AddLine(inspName, "N/A", "The Grid axial distance exceeds the EPRI recommended of " + epriRecommended + " in. for the component min OD, though it is below the EPRI maximum");

						// Compare the radial distance to the EPRI guidelines
						if (grid.GridRadialDistance != null && grid.GridRadialDistance > epriMax)
							AddLine(inspName, "N/A", "The Grid radial distance exceeds the EPRI MAXIMUM of " + epriMax + " in. for the component min OD");
                        else if (warnEpriRecommended && grid.GridRadialDistance != null 
                                && grid.GridRadialDistance > epriRecommended)
							AddLine(inspName, "N/A", "The Grid radial distance exceeds the EPRI recommended of " + epriRecommended + " in. for the component min OD, though it is below the EPRI maximum");
					}

					if (component.ComponentUpMainOd != null && grid.GridRadialDistance != null)
					{
						decimal minCols = (gridColCount_UseCeiling ?
                            Math.Ceiling((decimal)component.ComponentUpMainOd * (decimal)Math.PI / (decimal)grid.GridRadialDistance) :
                            Math.Round((decimal)component.ComponentUpMainOd * (decimal)Math.PI / (decimal)grid.GridRadialDistance) );

                        if ((decimal)(grid.GridEndCol - grid.GridStartCol + 1) < minCols)
							AddLine(inspName, "N/A", "Too few columns for the grid.  There are " + (grid.GridEndCol - grid.GridStartCol + 1) +
								". Expected " + minCols + " based on the component OD and the grid radial dimension.");
					}
					if (warnNoUpExt && 
                        (grid.GridUpExtStartRow == null || grid.GridUpExtEndRow == null))
						AddLine(inspName, "N/A", "No upstream extension partition rows provided for the grid");

					if (warnNoUpMain &&
                        (grid.GridUpMainStartRow == null || grid.GridUpMainEndRow == null))
						AddLine(inspName, "N/A", "No upstream main partition rows provided for the grid");

					if (warnNoDnExt && 
                        (grid.GridDnExtStartRow == null || grid.GridDnExtEndRow == null))
						AddLine(inspName, "N/A", "No downstream extension partition rows provided for the grid");

					// Check that the downstream extension rows match what is specified in the grid procedure.
					if (grid.GridDnExtStartRow != null && grid.GridDnExtEndRow != null && 
						component.ComponentUpMainOd != null && curReport.InspComponentGrpID != null &&
						grid.GridAxialDistance != null)
					{
						EGridProcedure gridProc = new EGridProcedure(curReport.InspComponentGrpID);
						if (gridProc.GridProcedureDsDiameters != null)
						{
							decimal od = (decimal)(component.ComponentDnMainOd == null ? 
								component.ComponentUpMainOd : component.ComponentDnMainOd);
                            decimal minRows = (gridDsExtRowCount_UseCeiling ?
                                Math.Ceiling((decimal)gridProc.GridProcedureDsDiameters * od / (decimal)grid.GridAxialDistance) :
                                Math.Round((decimal)gridProc.GridProcedureDsDiameters * od / (decimal)grid.GridAxialDistance));
                            if ((grid.GridDnExtEndRow - grid.GridDnExtStartRow + 1) < minRows)
								AddLine(inspName, "N/A", "Too few downstream extension rows for the grid.  There are " + 
									(grid.GridDnExtEndRow - grid.GridDnExtStartRow + 1) + ". Expected " + minRows);
						}
					}
					// Check that there are at least 3 upstream rows unless a special axial location is set
					if (grid.GridUpExtEndRow != null && grid.GridUpExtStartRow != null && 
						grid.GridAxialLocOverride == null)
						if (grid.GridUpExtEndRow - grid.GridUpExtStartRow + 1 < 3)
							AddLine(inspName, "N/A", "Too few upstream extension rows for the grid.  There are " + 
									(grid.GridUpExtEndRow - grid.GridUpExtStartRow + 1) + ". Expected 3");

					if (grid.GridRdlID == null)
						AddLine(inspName, "N/A", "No radial location specified for the grid");

					if (component.BranchThicknessesDefined && !grid.IsBranchDefined)
						AddLine(inspName, "N/A", "The component has a branch, but no branch partition information was specified for the grid");

					if (component.BranchExtThicknessesDefined && !grid.IsBranchExtDefined)
						AddLine(inspName, "N/A", "The component has a branch, but no branch extension partition information was specified for the grid");

					if (component.DnMainThicknessesDefined && !grid.IsDsMainDefined)
						AddLine(inspName, "N/A", "The component has a downstream section, but no downstream main partition information was specified for the grid");

				}
				if (curReport.InspComponentGrpID == null && gridProcedureRequired)
				{
					AddLine("N/A", "N/A", "The report includes grids but no grid procedure was specified");
				}

				EDsetCollection dsets = EDset.ListByNameForInspection((Guid)inspection.ID);
				foreach (EDset dset in dsets)
				{
					string dsetName = dset.DsetName;
					EInspector inspector = null;
					EMeter meter = null;
					EDucer ducer = null;
					ECalBlock calblock = null;
					EThermo thermo = null;
					EInspectionPeriodCollection inspPeriods = EInspectionPeriod.ListForDset(dset.ID);
					// Check if an inspector is specfied
					if (dset.DsetInsID == null) AddLine(inspName, dsetName, "No inspector specified");
					else inspector = new EInspector(dset.DsetIspID); 

					// Make sure all tools are specified
					if (dset.DsetMtrID == null) AddLine(inspName, dsetName, "No instrument specified");
					else	meter = new EMeter(dset.DsetMtrID); 

					if (dset.DsetDcrID == null) AddLine(inspName, dsetName, "No transducer specified");
					else ducer = new EDucer(dset.DsetDcrID); 

					if (dset.DsetCbkID == null) AddLine(inspName, dsetName, "No calibration block specified");
					else calblock = new ECalBlock(dset.DsetCbkID);

                    if (dset.DsetThmID == null)
                    {
                        if (warnTemperatures) AddLine(inspName, dsetName, "No thermometer specified");
                    }
                    else thermo = new EThermo(dset.DsetThmID); 
					
					// Make sure that the ducer is ok for the meter
					if (dset.DsetMtrID != null && dset.DsetDcrID != null)
					{
						if (!ducer.DucerIsOkForMeter(dset.DsetMtrID))
						{
							AddLine(inspName, dsetName, "Transducer model " + ducer.DucerModelName +
								" is not designed for use with instrument model " + meter.MeterModelName);
						}
					}
					// Make sure the meter has been calibrated recently
					if (dset.DsetMtrID != null)
					{
						if (meter.MeterCalDueDate == null)
						{
							AddLine(inspName, dsetName, "Meter " + meter.MeterModelAndSerial + 
								" does not have its calibration due date set");
						}
						else if (meter.MeterCalDueDate < DateTime.Today)
						{
							AddLine(inspName, dsetName, "Meter " + meter.MeterModelAndSerial + 
								" was due for calibration on " + meter.MeterCalDueDate);
						}
					}
					// Make sure the cal block type matches the material type.
					if (dset.DsetCbkID != null)
					{
						EComponentMaterial cmpMaterial = new EComponentMaterial(component.ComponentCmtID);

						if (calblock.CalBlockMaterialType != cmpMaterial.CmpMaterialCalBlockMaterial)
						{
							AddLine(inspName, dsetName, "Calibration block material type " + calblock.CalBlockMaterialAbbr + 
								" should not be used with component material type " + cmpMaterial.CmpMaterialName);
						}
					}
					// Make sure no inspections lasted too long without cal checks.
					if (inspPeriods.Count == 0)
					{
						AddLine(inspName, dsetName, "No inspection periods have been defined");
					}
					else
					{
						foreach (EInspectionPeriod inspPeriod in inspPeriods)
						{
							TimeSpan span1 = TimeSpan.Zero;
							TimeSpan span2 = TimeSpan.Zero;
							TimeSpan span3 = TimeSpan.Zero;

							if (inspPeriod.InspectionPeriodCalCheck1At != null)
								span1 = (DateTime)inspPeriod.InspectionPeriodCalCheck1At - (DateTime)inspPeriod.InspectionPeriodInAt;
							else
								span1 = (DateTime)inspPeriod.InspectionPeriodOutAt - (DateTime)inspPeriod.InspectionPeriodInAt;

							if (inspPeriod.InspectionPeriodCalCheck1At != null)
							{
								if (inspPeriod.InspectionPeriodCalCheck2At != null)
									span2 = (DateTime)inspPeriod.InspectionPeriodCalCheck2At - (DateTime)inspPeriod.InspectionPeriodCalCheck1At;
								else
									span2 = (DateTime)inspPeriod.InspectionPeriodOutAt - (DateTime)inspPeriod.InspectionPeriodCalCheck1At;
							}

							if (inspPeriod.InspectionPeriodCalCheck2At != null)
							{
								span3 = (DateTime)inspPeriod.InspectionPeriodOutAt - (DateTime)inspPeriod.InspectionPeriodCalCheck2At;
							}

							if (span1.TotalHours > 4.0 || span2.TotalHours > 4.0 || span3.TotalHours > 4.0)
							{
								AddLine(inspName, dsetName, "An inspection period exceeded 4 hours between calibration checks");
							}

						}
					}
					// Make sure the component temp is within 25 degrees of the cal block temp.
                    if (warnTemperatures && dset.DsetCalBlockTemp == null)
						AddLine(inspName, dsetName, "No calibration block temperature specified");
                    if (warnTemperatures && dset.DsetCompTemp == null)
						AddLine(inspName, dsetName, "No component temperature specified");

					if (dset.DsetCompTemp != null && dset.DsetCalBlockTemp != null &&
								Math.Abs((int)(dset.DsetCompTemp - dset.DsetCalBlockTemp)) > 25)
						AddLine(inspName, dsetName, "Difference between calibration block temperature and component temperature exceeds 25 degrees F");

					// Make sure the inspector is Level II or III
					if (dset.DsetInsID != null && inspector.InspectorLevel < 2)
						AddLine(inspName, dsetName, "Inspector " + inspector.InspectorName + " is not level II or III");

					// Todo: check if they really want the inspector's impression of the min and max in the system.
					if (dset.DsetMinWall != null && dset.DsetCbkID != null)
						if (dset.DsetMinWall < System.Convert.ToSingle(calblock.CalBlockCalMin))
							AddLine(inspName, dsetName, "The dataset contains readings below the minimum for the calibration block used");

					if (dset.DsetMaxWall != null && dset.DsetCbkID != null)
						if (dset.DsetMaxWall > System.Convert.ToSingle(calblock.CalBlockCalMax))
							AddLine(inspName, dsetName, "The dataset contains readings above the maximum for the calibration block used");

					if (dset.DsetThin == null)
						AddLine(inspName, dsetName, "Low calibration thickness was not recorded");

					if (dset.DsetThick == null)
						AddLine(inspName, dsetName, "High calibration thickness was not recorded");

					if (dset.DsetRange == null)
						AddLine(inspName, dsetName, "Coarse range was not recorded");

					if (dset.DsetThin != null && dset.DsetMinWall != null && dset.DsetMinWall < System.Convert.ToSingle(dset.DsetThin))
						AddLine(inspName, dsetName, "The dataset contains readings below the low calibration thickness");

					if (dset.DsetThick != null && dset.DsetMaxWall != null && dset.DsetMaxWall > System.Convert.ToSingle(dset.DsetThick))
						AddLine(inspName, dsetName, "The dataset contains readings above the high calibration thickness");

					if (dset.DsetRange != null && dset.DsetMaxWall != null && dset.DsetMaxWall > System.Convert.ToSingle(dset.DsetRange))
						AddLine(inspName, dsetName, "The dataset contains readings above the coarse range for the instrument");

					if (dset.DsetCrewDose == null)
						AddLine(inspName, dsetName, "The crew dose was not specified");

					if (dset.DsetGainDb == null)
						AddLine(inspName, dsetName, "The gain was not specified");

					if (dset.DsetVelocity == null)
						AddLine(inspName, dsetName, "The velocity was not specified");

				}
			}
			if (dgvInfo.Rows.Count == 0)
				dgvInfo.Visible = false;

		}


	}
}