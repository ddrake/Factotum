namespace Factotum
{
	partial class ReportDefinitionImporter
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportDefinitionImporter));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnSelectAndPreview = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.lblSiteName = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.tvwReportChange = new System.Windows.Forms.TreeView();
			this.tvwItemAdd = new System.Windows.Forms.TreeView();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 369);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(609, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(38, 17);
			this.toolStripStatusLabel1.Text = "Ready";
			// 
			// btnSelectAndPreview
			// 
			this.btnSelectAndPreview.Location = new System.Drawing.Point(12, 331);
			this.btnSelectAndPreview.Name = "btnSelectAndPreview";
			this.btnSelectAndPreview.Size = new System.Drawing.Size(283, 23);
			this.btnSelectAndPreview.TabIndex = 2;
			this.btnSelectAndPreview.Text = "Select File For Import and Preview Changes";
			this.btnSelectAndPreview.UseVisualStyleBackColor = true;
			this.btnSelectAndPreview.Click += new System.EventHandler(this.btnSelectAndPreview_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(236, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(157, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Report Definitions To Be Added";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(169, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Report Definitions To Be Changed";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(494, 331);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(102, 23);
			this.btnCancel.TabIndex = 15;
			this.btnCancel.Text = "Cancel Import";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnImport
			// 
			this.btnImport.Location = new System.Drawing.Point(390, 331);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(98, 23);
			this.btnImport.TabIndex = 16;
			this.btnImport.Text = "Perform Import";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// lblSiteName
			// 
			this.lblSiteName.AutoSize = true;
			this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
			this.lblSiteName.Location = new System.Drawing.Point(215, 7);
			this.lblSiteName.Name = "lblSiteName";
			this.lblSiteName.Size = new System.Drawing.Size(53, 16);
			this.lblSiteName.TabIndex = 17;
			this.lblSiteName.Text = "Facility:";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// tvwReportChange
			// 
			this.tvwReportChange.CheckBoxes = true;
			this.tvwReportChange.Location = new System.Drawing.Point(12, 55);
			this.tvwReportChange.Name = "tvwReportChange";
			this.tvwReportChange.Size = new System.Drawing.Size(221, 270);
			this.tvwReportChange.TabIndex = 18;
			this.tvwReportChange.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwComponentChange_AfterCheck);
			// 
			// tvwItemAdd
			// 
			this.tvwItemAdd.Location = new System.Drawing.Point(239, 55);
			this.tvwItemAdd.Name = "tvwItemAdd";
			this.tvwItemAdd.Size = new System.Drawing.Size(358, 270);
			this.tvwItemAdd.TabIndex = 19;
			// 
			// ReportDefinitionImporter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(609, 391);
			this.Controls.Add(this.tvwItemAdd);
			this.Controls.Add(this.tvwReportChange);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSelectAndPreview);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ReportDefinitionImporter";
			this.Text = "Report Names/Work Orders Importer";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Button btnSelectAndPreview;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.Label lblSiteName;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TreeView tvwReportChange;
		private System.Windows.Forms.TreeView tvwItemAdd;
	}
}