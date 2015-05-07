namespace Factotum
{
	partial class AppMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportComponentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importComponentReportDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importOutageConfigurationDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewChangesFromOutageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enterNewActivationKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateANewActivationKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreAutobackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToMasterDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemMasterPrefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.proceduresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrationProceduresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridProceduresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridSizesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radialLocationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialCalibrationParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.equipmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meterModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transducersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transducerModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thermomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrationBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.couplantTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.siteConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentMaterialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pipeScheduleLookupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentOutageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolKitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tmrBackup = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.proceduresToolStripMenuItem,
            this.equipmentToolStripMenuItem,
            this.inspectorsToolStripMenuItem,
            this.siteConfigurationToolStripMenuItem,
            this.outageToolStripMenuItem,
            this.componentReportToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.MdiWindowListItem = this.windowToolStripMenuItem;
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(616, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.utilitiesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.exportToolStripMenuItem.Text = "&Backup";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImportComponentsToolStripMenuItem,
            this.importComponentReportDefinitionToolStripMenuItem,
            this.importOutageConfigurationDataToolStripMenuItem,
            this.viewChangesFromOutageToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.importToolStripMenuItem.Text = "&Import";
            // 
            // ImportComponentsToolStripMenuItem
            // 
            this.ImportComponentsToolStripMenuItem.Name = "ImportComponentsToolStripMenuItem";
            this.ImportComponentsToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.ImportComponentsToolStripMenuItem.Text = "Components";
            this.ImportComponentsToolStripMenuItem.Click += new System.EventHandler(this.ImportComponentsToolStripMenuItem_Click);
            // 
            // importComponentReportDefinitionToolStripMenuItem
            // 
            this.importComponentReportDefinitionToolStripMenuItem.Name = "importComponentReportDefinitionToolStripMenuItem";
            this.importComponentReportDefinitionToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.importComponentReportDefinitionToolStripMenuItem.Text = "Report Names/Work Orders";
            this.importComponentReportDefinitionToolStripMenuItem.Click += new System.EventHandler(this.importComponentReportDefinitionToolStripMenuItem_Click);
            // 
            // importOutageConfigurationDataToolStripMenuItem
            // 
            this.importOutageConfigurationDataToolStripMenuItem.Name = "importOutageConfigurationDataToolStripMenuItem";
            this.importOutageConfigurationDataToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.importOutageConfigurationDataToolStripMenuItem.Text = "Outage Data File";
            this.importOutageConfigurationDataToolStripMenuItem.Click += new System.EventHandler(this.importOutageConfigurationDataToolStripMenuItem_Click);
            // 
            // viewChangesFromOutageToolStripMenuItem
            // 
            this.viewChangesFromOutageToolStripMenuItem.Name = "viewChangesFromOutageToolStripMenuItem";
            this.viewChangesFromOutageToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.viewChangesFromOutageToolStripMenuItem.Text = "View Changes From Outage";
            this.viewChangesFromOutageToolStripMenuItem.Click += new System.EventHandler(this.viewChangesFromOutageToolStripMenuItem_Click);
            // 
            // utilitiesToolStripMenuItem
            // 
            this.utilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enterNewActivationKeyToolStripMenuItem,
            this.generateANewActivationKeyToolStripMenuItem,
            this.restoreAutobackupToolStripMenuItem,
            this.convertToMasterDBToolStripMenuItem});
            this.utilitiesToolStripMenuItem.Name = "utilitiesToolStripMenuItem";
            this.utilitiesToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.utilitiesToolStripMenuItem.Text = "&Utilities";
            // 
            // enterNewActivationKeyToolStripMenuItem
            // 
            this.enterNewActivationKeyToolStripMenuItem.Name = "enterNewActivationKeyToolStripMenuItem";
            this.enterNewActivationKeyToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.enterNewActivationKeyToolStripMenuItem.Text = "&Enter a new Activation Key";
            this.enterNewActivationKeyToolStripMenuItem.Click += new System.EventHandler(this.enterANewActivationKeyToolStripMenuItem_Click);
            // 
            // generateANewActivationKeyToolStripMenuItem
            // 
            this.generateANewActivationKeyToolStripMenuItem.Name = "generateANewActivationKeyToolStripMenuItem";
            this.generateANewActivationKeyToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.generateANewActivationKeyToolStripMenuItem.Text = "&Generate a new Activation Key";
            this.generateANewActivationKeyToolStripMenuItem.Click += new System.EventHandler(this.generateANewActivationKeyToolStripMenuItem_Click);
            // 
            // restoreAutobackupToolStripMenuItem
            // 
            this.restoreAutobackupToolStripMenuItem.Name = "restoreAutobackupToolStripMenuItem";
            this.restoreAutobackupToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.restoreAutobackupToolStripMenuItem.Text = "&Restore Autobackup";
            this.restoreAutobackupToolStripMenuItem.Click += new System.EventHandler(this.restoreAutobackupToolStripMenuItem_Click);
            // 
            // convertToMasterDBToolStripMenuItem
            // 
            this.convertToMasterDBToolStripMenuItem.Name = "convertToMasterDBToolStripMenuItem";
            this.convertToMasterDBToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.convertToMasterDBToolStripMenuItem.Text = "&Convert To Master DB";
            this.convertToMasterDBToolStripMenuItem.Click += new System.EventHandler(this.convertToMasterDBToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemMasterPrefsToolStripMenuItem,
            this.generalToolStripMenuItem});
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.preferencesToolStripMenuItem.Text = "Pre&ferences";
            // 
            // systemMasterPrefsToolStripMenuItem
            // 
            this.systemMasterPrefsToolStripMenuItem.Name = "systemMasterPrefsToolStripMenuItem";
            this.systemMasterPrefsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.systemMasterPrefsToolStripMenuItem.Text = "System Master";
            this.systemMasterPrefsToolStripMenuItem.Click += new System.EventHandler(this.testConfigurationToolStripMenuItem_Click);
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.generalToolStripMenuItem.Text = "General";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.generalToolStripMenuItem_Click);
            // 
            // proceduresToolStripMenuItem
            // 
            this.proceduresToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calibrationProceduresToolStripMenuItem,
            this.gridProceduresToolStripMenuItem,
            this.gridSizesToolStripMenuItem,
            this.radialLocationsToolStripMenuItem,
            this.specialCalibrationParametersToolStripMenuItem});
            this.proceduresToolStripMenuItem.Name = "proceduresToolStripMenuItem";
            this.proceduresToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.proceduresToolStripMenuItem.Text = "&Procedures";
            // 
            // calibrationProceduresToolStripMenuItem
            // 
            this.calibrationProceduresToolStripMenuItem.Name = "calibrationProceduresToolStripMenuItem";
            this.calibrationProceduresToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.calibrationProceduresToolStripMenuItem.Text = "Calibration Procedures";
            this.calibrationProceduresToolStripMenuItem.Click += new System.EventHandler(this.calibrationProceduresToolStripMenuItem_Click);
            // 
            // gridProceduresToolStripMenuItem
            // 
            this.gridProceduresToolStripMenuItem.Name = "gridProceduresToolStripMenuItem";
            this.gridProceduresToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.gridProceduresToolStripMenuItem.Text = "Grid Procedures";
            this.gridProceduresToolStripMenuItem.Click += new System.EventHandler(this.gridProceduresToolStripMenuItem_Click);
            // 
            // gridSizesToolStripMenuItem
            // 
            this.gridSizesToolStripMenuItem.Name = "gridSizesToolStripMenuItem";
            this.gridSizesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.gridSizesToolStripMenuItem.Text = "Grid Sizes";
            this.gridSizesToolStripMenuItem.Click += new System.EventHandler(this.gridSizesToolStripMenuItem_Click);
            // 
            // radialLocationsToolStripMenuItem
            // 
            this.radialLocationsToolStripMenuItem.Name = "radialLocationsToolStripMenuItem";
            this.radialLocationsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.radialLocationsToolStripMenuItem.Text = "Radial Locations";
            this.radialLocationsToolStripMenuItem.Click += new System.EventHandler(this.radialLocationsToolStripMenuItem_Click);
            // 
            // specialCalibrationParametersToolStripMenuItem
            // 
            this.specialCalibrationParametersToolStripMenuItem.Name = "specialCalibrationParametersToolStripMenuItem";
            this.specialCalibrationParametersToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.specialCalibrationParametersToolStripMenuItem.Text = "Special Calibration Parameters";
            this.specialCalibrationParametersToolStripMenuItem.Click += new System.EventHandler(this.specialCalibrationParametersToolStripMenuItem_Click);
            // 
            // equipmentToolStripMenuItem
            // 
            this.equipmentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metersToolStripMenuItem,
            this.meterModelsToolStripMenuItem,
            this.transducersToolStripMenuItem,
            this.transducerModelsToolStripMenuItem,
            this.thermomToolStripMenuItem,
            this.calibrationBlocksToolStripMenuItem,
            this.couplantTypesToolStripMenuItem});
            this.equipmentToolStripMenuItem.Name = "equipmentToolStripMenuItem";
            this.equipmentToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.equipmentToolStripMenuItem.Text = "&Equipment";
            // 
            // metersToolStripMenuItem
            // 
            this.metersToolStripMenuItem.Name = "metersToolStripMenuItem";
            this.metersToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.metersToolStripMenuItem.Text = "&Meters";
            this.metersToolStripMenuItem.Click += new System.EventHandler(this.metersToolStripMenuItem_Click);
            // 
            // meterModelsToolStripMenuItem
            // 
            this.meterModelsToolStripMenuItem.Name = "meterModelsToolStripMenuItem";
            this.meterModelsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.meterModelsToolStripMenuItem.Text = "Meter M&odels";
            this.meterModelsToolStripMenuItem.Click += new System.EventHandler(this.meterModelsToolStripMenuItem_Click);
            // 
            // transducersToolStripMenuItem
            // 
            this.transducersToolStripMenuItem.Name = "transducersToolStripMenuItem";
            this.transducersToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.transducersToolStripMenuItem.Text = "&Transducers";
            this.transducersToolStripMenuItem.Click += new System.EventHandler(this.transducersToolStripMenuItem_Click);
            // 
            // transducerModelsToolStripMenuItem
            // 
            this.transducerModelsToolStripMenuItem.Name = "transducerModelsToolStripMenuItem";
            this.transducerModelsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.transducerModelsToolStripMenuItem.Text = "Transducer Models";
            this.transducerModelsToolStripMenuItem.Click += new System.EventHandler(this.transducerModelsToolStripMenuItem_Click);
            // 
            // thermomToolStripMenuItem
            // 
            this.thermomToolStripMenuItem.Name = "thermomToolStripMenuItem";
            this.thermomToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.thermomToolStripMenuItem.Text = "T&hermometers";
            this.thermomToolStripMenuItem.Click += new System.EventHandler(this.thermomToolStripMenuItem_Click);
            // 
            // calibrationBlocksToolStripMenuItem
            // 
            this.calibrationBlocksToolStripMenuItem.Name = "calibrationBlocksToolStripMenuItem";
            this.calibrationBlocksToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.calibrationBlocksToolStripMenuItem.Text = "&Calibration Blocks";
            this.calibrationBlocksToolStripMenuItem.Click += new System.EventHandler(this.calibrationBlocksToolStripMenuItem_Click);
            // 
            // couplantTypesToolStripMenuItem
            // 
            this.couplantTypesToolStripMenuItem.Name = "couplantTypesToolStripMenuItem";
            this.couplantTypesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.couplantTypesToolStripMenuItem.Text = "Cou&plant Types";
            this.couplantTypesToolStripMenuItem.Click += new System.EventHandler(this.couplantTypesToolStripMenuItem_Click);
            // 
            // inspectorsToolStripMenuItem
            // 
            this.inspectorsToolStripMenuItem.Name = "inspectorsToolStripMenuItem";
            this.inspectorsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.inspectorsToolStripMenuItem.Text = "Inspectors";
            this.inspectorsToolStripMenuItem.Click += new System.EventHandler(this.inspectorsToolStripMenuItem_Click);
            // 
            // siteConfigurationToolStripMenuItem
            // 
            this.siteConfigurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customerToolStripMenuItem,
            this.componentsToolStripMenuItem,
            this.componentTypesToolStripMenuItem,
            this.componentMaterialsToolStripMenuItem,
            this.pipeScheduleLookupToolStripMenuItem,
            this.linesToolStripMenuItem,
            this.systemsToolStripMenuItem});
            this.siteConfigurationToolStripMenuItem.Name = "siteConfigurationToolStripMenuItem";
            this.siteConfigurationToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.siteConfigurationToolStripMenuItem.Text = "Site &Configuration";
            // 
            // customerToolStripMenuItem
            // 
            this.customerToolStripMenuItem.Name = "customerToolStripMenuItem";
            this.customerToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.customerToolStripMenuItem.Text = "C&ustomers and Sites";
            this.customerToolStripMenuItem.Click += new System.EventHandler(this.customerToolStripMenuItem_Click);
            // 
            // componentsToolStripMenuItem
            // 
            this.componentsToolStripMenuItem.Name = "componentsToolStripMenuItem";
            this.componentsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.componentsToolStripMenuItem.Text = "&Components";
            this.componentsToolStripMenuItem.Click += new System.EventHandler(this.componentsToolStripMenuItem_Click);
            // 
            // componentTypesToolStripMenuItem
            // 
            this.componentTypesToolStripMenuItem.Name = "componentTypesToolStripMenuItem";
            this.componentTypesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.componentTypesToolStripMenuItem.Text = "Component &Types";
            this.componentTypesToolStripMenuItem.Click += new System.EventHandler(this.componentTypesToolStripMenuItem_Click);
            // 
            // componentMaterialsToolStripMenuItem
            // 
            this.componentMaterialsToolStripMenuItem.Name = "componentMaterialsToolStripMenuItem";
            this.componentMaterialsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.componentMaterialsToolStripMenuItem.Text = "Component &Materials";
            this.componentMaterialsToolStripMenuItem.Click += new System.EventHandler(this.componentMaterialsToolStripMenuItem_Click);
            // 
            // pipeScheduleLookupToolStripMenuItem
            // 
            this.pipeScheduleLookupToolStripMenuItem.Name = "pipeScheduleLookupToolStripMenuItem";
            this.pipeScheduleLookupToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.pipeScheduleLookupToolStripMenuItem.Text = "&Pipe Schedule Lookup";
            this.pipeScheduleLookupToolStripMenuItem.Click += new System.EventHandler(this.pipeScheduleLookupToolStripMenuItem_Click);
            // 
            // linesToolStripMenuItem
            // 
            this.linesToolStripMenuItem.Name = "linesToolStripMenuItem";
            this.linesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.linesToolStripMenuItem.Text = "&Lines";
            this.linesToolStripMenuItem.Click += new System.EventHandler(this.linesToolStripMenuItem_Click);
            // 
            // systemsToolStripMenuItem
            // 
            this.systemsToolStripMenuItem.Name = "systemsToolStripMenuItem";
            this.systemsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.systemsToolStripMenuItem.Text = "&Systems";
            this.systemsToolStripMenuItem.Click += new System.EventHandler(this.systemsToolStripMenuItem_Click);
            // 
            // outageToolStripMenuItem
            // 
            this.outageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentOutageToolStripMenuItem,
            this.toolKitsToolStripMenuItem});
            this.outageToolStripMenuItem.Name = "outageToolStripMenuItem";
            this.outageToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.outageToolStripMenuItem.Text = "&Outage";
            // 
            // currentOutageToolStripMenuItem
            // 
            this.currentOutageToolStripMenuItem.Name = "currentOutageToolStripMenuItem";
            this.currentOutageToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.currentOutageToolStripMenuItem.Text = "Current &Outage";
            this.currentOutageToolStripMenuItem.Click += new System.EventHandler(this.currentOutageToolStripMenuItem_Click);
            // 
            // toolKitsToolStripMenuItem
            // 
            this.toolKitsToolStripMenuItem.Name = "toolKitsToolStripMenuItem";
            this.toolKitsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.toolKitsToolStripMenuItem.Text = "Tool &Kits";
            this.toolKitsToolStripMenuItem.Click += new System.EventHandler(this.toolKitsToolStripMenuItem_Click);
            // 
            // componentReportToolStripMenuItem
            // 
            this.componentReportToolStripMenuItem.Name = "componentReportToolStripMenuItem";
            this.componentReportToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.componentReportToolStripMenuItem.Text = "&Report";
            this.componentReportToolStripMenuItem.Click += new System.EventHandler(this.componentReportToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.windowToolStripMenuItem.Text = "&Window";
            // 
            // tmrBackup
            // 
            this.tmrBackup.Enabled = true;
            this.tmrBackup.Interval = 3600000;
            this.tmrBackup.Tick += new System.EventHandler(this.tmrBackup_Tick);
            // 
            // AppMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 413);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AppMain";
            this.Text = "Factotum";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.AppMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppMain_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem siteConfigurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem customerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem componentsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem componentTypesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem componentMaterialsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pipeScheduleLookupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem equipmentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem metersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem transducersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem outageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem proceduresToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem thermomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem meterModelsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem transducerModelsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem calibrationBlocksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem couplantTypesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem calibrationProceduresToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gridProceduresToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem componentReportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gridSizesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem radialLocationsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ImportComponentsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importComponentReportDefinitionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolKitsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem currentOutageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem systemMasterPrefsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem specialCalibrationParametersToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.ToolStripMenuItem importOutageConfigurationDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem utilitiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem convertToMasterDBToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem enterNewActivationKeyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem linesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem systemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewChangesFromOutageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem generateANewActivationKeyToolStripMenuItem;
		private System.Windows.Forms.Timer tmrBackup;
		private System.Windows.Forms.ToolStripMenuItem restoreAutobackupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem inspectorsToolStripMenuItem;
	}
}