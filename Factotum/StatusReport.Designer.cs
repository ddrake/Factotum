namespace Factotum
{
	partial class StatusReport
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
			Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusReport));
			this.TmpStatusReportBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.StatusReportDataSet = new Factotum.StatusReportDataSet();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.rvStatusReport = new Microsoft.Reporting.WinForms.ReportViewer();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnReportSubmitted = new System.Windows.Forms.ToolStripButton();
			this.btnUtComplete = new System.Windows.Forms.ToolStripButton();
			this.btnCurrentView = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnIncludePrevReported = new System.Windows.Forms.ToolStripButton();
			this.TmpStatusReportTableAdapter = new Factotum.StatusReportDataSetTableAdapters.TmpStatusReportTableAdapter();
			((System.ComponentModel.ISupportInitialize)(this.TmpStatusReportBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.StatusReportDataSet)).BeginInit();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// TmpStatusReportBindingSource
			// 
			this.TmpStatusReportBindingSource.DataMember = "TmpStatusReport";
			this.TmpStatusReportBindingSource.DataSource = this.StatusReportDataSet;
			// 
			// StatusReportDataSet
			// 
			this.StatusReportDataSet.DataSetName = "StatusReportDataSet";
			this.StatusReportDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// toolStripContainer1
			// 
			this.toolStripContainer1.BottomToolStripPanelVisible = false;
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.AutoScroll = true;
			this.toolStripContainer1.ContentPanel.Controls.Add(this.rvStatusReport);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(745, 292);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.LeftToolStripPanelVisible = false;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.RightToolStripPanelVisible = false;
			this.toolStripContainer1.Size = new System.Drawing.Size(745, 317);
			this.toolStripContainer1.TabIndex = 1;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// rvStatusReport
			// 
			this.rvStatusReport.Dock = System.Windows.Forms.DockStyle.Fill;
			reportDataSource1.Name = "StatusReportDataSet_TmpStatusReport";
			reportDataSource1.Value = this.TmpStatusReportBindingSource;
			this.rvStatusReport.LocalReport.DataSources.Add(reportDataSource1);
			this.rvStatusReport.LocalReport.ReportEmbeddedResource = "Factotum.StatusReport.rdlc";
			this.rvStatusReport.Location = new System.Drawing.Point(0, 0);
			this.rvStatusReport.Name = "rvStatusReport";
			this.rvStatusReport.Size = new System.Drawing.Size(745, 292);
			this.rvStatusReport.TabIndex = 0;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnReportSubmitted,
            this.btnUtComplete,
            this.btnCurrentView,
            this.toolStripSeparator1,
            this.btnIncludePrevReported});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(478, 25);
			this.toolStrip1.TabIndex = 0;
			// 
			// btnReportSubmitted
			// 
			this.btnReportSubmitted.Checked = true;
			this.btnReportSubmitted.CheckState = System.Windows.Forms.CheckState.Checked;
			this.btnReportSubmitted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnReportSubmitted.Image = ((System.Drawing.Image)(resources.GetObject("btnReportSubmitted.Image")));
			this.btnReportSubmitted.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnReportSubmitted.Name = "btnReportSubmitted";
			this.btnReportSubmitted.Size = new System.Drawing.Size(153, 22);
			this.btnReportSubmitted.Text = "Component Report Submitted";
			this.btnReportSubmitted.Click += new System.EventHandler(this.btnReportSubmitted_Click);
			// 
			// btnUtComplete
			// 
			this.btnUtComplete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnUtComplete.Image = ((System.Drawing.Image)(resources.GetObject("btnUtComplete.Image")));
			this.btnUtComplete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.btnUtComplete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnUtComplete.Name = "btnUtComplete";
			this.btnUtComplete.Size = new System.Drawing.Size(97, 22);
			this.btnUtComplete.Text = "UT Field Complete";
			this.btnUtComplete.Click += new System.EventHandler(this.btnUtComplete_Click);
			// 
			// btnCurrentView
			// 
			this.btnCurrentView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnCurrentView.Image = ((System.Drawing.Image)(resources.GetObject("btnCurrentView.Image")));
			this.btnCurrentView.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnCurrentView.Name = "btnCurrentView";
			this.btnCurrentView.Size = new System.Drawing.Size(73, 22);
			this.btnCurrentView.Text = "Current View";
			this.btnCurrentView.Click += new System.EventHandler(this.btnCurrentView_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// btnIncludePrevReported
			// 
			this.btnIncludePrevReported.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnIncludePrevReported.Image = ((System.Drawing.Image)(resources.GetObject("btnIncludePrevReported.Image")));
			this.btnIncludePrevReported.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnIncludePrevReported.Name = "btnIncludePrevReported";
			this.btnIncludePrevReported.Size = new System.Drawing.Size(146, 22);
			this.btnIncludePrevReported.Text = "Include Previously Reported";
			this.btnIncludePrevReported.Click += new System.EventHandler(this.btnIncludePrevReported_Click);
			// 
			// TmpStatusReportTableAdapter
			// 
			this.TmpStatusReportTableAdapter.ClearBeforeFill = true;
			// 
			// StatusReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(745, 317);
			this.Controls.Add(this.toolStripContainer1);
			this.MinimumSize = new System.Drawing.Size(518, 132);
			this.Name = "StatusReport";
			this.Text = "StatusReport";
			this.Load += new System.EventHandler(this.StatusReport_Load);
			((System.ComponentModel.ISupportInitialize)(this.TmpStatusReportBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.StatusReportDataSet)).EndInit();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btnUtComplete;
		private System.Windows.Forms.ToolStripButton btnReportSubmitted;
		private System.Windows.Forms.ToolStripButton btnCurrentView;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton btnIncludePrevReported;
		private Microsoft.Reporting.WinForms.ReportViewer rvStatusReport;
		private System.Windows.Forms.BindingSource TmpStatusReportBindingSource;
		private StatusReportDataSet StatusReportDataSet;
		private Factotum.StatusReportDataSetTableAdapters.TmpStatusReportTableAdapter TmpStatusReportTableAdapter;
	}
}