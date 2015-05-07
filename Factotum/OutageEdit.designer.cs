namespace Factotum
{
	partial class OutageEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutageEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblSiteName = new System.Windows.Forms.Label();
			this.cboCalibrationProc = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cboCouplantType = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
			this.label7 = new System.Windows.Forms.Label();
			this.btnExportConfig = new System.Windows.Forms.Button();
			this.clbGridProcedures = new System.Windows.Forms.CheckedListBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.clbInspectors = new System.Windows.Forms.CheckedListBox();
			this.btnMakeFieldDataSheets = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.fieldDataSheetsWorker = new System.ComponentModel.BackgroundWorker();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnGridColLayoutCCW = new System.Windows.Forms.RadioButton();
			this.btnGridColLayoutCW = new System.Windows.Forms.RadioButton();
			this.btnPreviewLabels = new System.Windows.Forms.Button();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.txtFacPhone = new Factotum.TextBoxWithUndo();
			this.txtCouplantBatch = new Factotum.TextBoxWithUndo();
			this.txtName = new Factotum.TextBoxWithUndo();
			this.lblOutageDataImportedOn = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(397, 337);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 16;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Outage Name";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(467, 337);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 17;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lblSiteName
			// 
			this.lblSiteName.AutoSize = true;
			this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
			this.lblSiteName.Location = new System.Drawing.Point(116, 9);
			this.lblSiteName.Name = "lblSiteName";
			this.lblSiteName.Size = new System.Drawing.Size(53, 16);
			this.lblSiteName.TabIndex = 0;
			this.lblSiteName.Text = "Facility:";
			// 
			// cboCalibrationProc
			// 
			this.cboCalibrationProc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCalibrationProc.FormattingEnabled = true;
			this.cboCalibrationProc.Location = new System.Drawing.Point(119, 61);
			this.cboCalibrationProc.Name = "cboCalibrationProc";
			this.cboCalibrationProc.Size = new System.Drawing.Size(172, 21);
			this.cboCalibrationProc.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Calibration Procedure";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(37, 91);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Couplant Type";
			// 
			// cboCouplantType
			// 
			this.cboCouplantType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCouplantType.FormattingEnabled = true;
			this.cboCouplantType.Location = new System.Drawing.Point(119, 88);
			this.cboCouplantType.Name = "cboCouplantType";
			this.cboCouplantType.Size = new System.Drawing.Size(172, 21);
			this.cboCouplantType.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(346, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Couplant Batch";
			// 
			// dtpStartDate
			// 
			this.dtpStartDate.Location = new System.Drawing.Point(119, 116);
			this.dtpStartDate.Name = "dtpStartDate";
			this.dtpStartDate.Size = new System.Drawing.Size(200, 20);
			this.dtpStartDate.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(58, 120);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Start Date";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(61, 146);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(52, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "End Date";
			// 
			// dtpEndDate
			// 
			this.dtpEndDate.Location = new System.Drawing.Point(119, 142);
			this.dtpEndDate.Name = "dtpEndDate";
			this.dtpEndDate.Size = new System.Drawing.Size(200, 20);
			this.dtpEndDate.TabIndex = 14;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(355, 38);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(71, 13);
			this.label7.TabIndex = 5;
			this.label7.Text = "FAC Phone #";
			// 
			// btnExportConfig
			// 
			this.btnExportConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnExportConfig.Location = new System.Drawing.Point(191, 314);
			this.btnExportConfig.Name = "btnExportConfig";
			this.btnExportConfig.Size = new System.Drawing.Size(84, 45);
			this.btnExportConfig.TabIndex = 15;
			this.btnExportConfig.Text = "Export Outage Data File";
			this.btnExportConfig.UseVisualStyleBackColor = true;
			this.btnExportConfig.Click += new System.EventHandler(this.btnExportConfig_Click);
			// 
			// clbGridProcedures
			// 
			this.clbGridProcedures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)));
			this.clbGridProcedures.FormattingEnabled = true;
			this.clbGridProcedures.Location = new System.Drawing.Point(12, 199);
			this.clbGridProcedures.Name = "clbGridProcedures";
			this.clbGridProcedures.Size = new System.Drawing.Size(248, 94);
			this.clbGridProcedures.TabIndex = 18;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(12, 183);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(139, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "Grid Procedures For Outage";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(281, 183);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(112, 13);
			this.label9.TabIndex = 21;
			this.label9.Text = "Inspectors For Outage";
			// 
			// clbInspectors
			// 
			this.clbInspectors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.clbInspectors.FormattingEnabled = true;
			this.clbInspectors.Location = new System.Drawing.Point(284, 199);
			this.clbInspectors.Name = "clbInspectors";
			this.clbInspectors.Size = new System.Drawing.Size(248, 94);
			this.clbInspectors.TabIndex = 20;
			// 
			// btnMakeFieldDataSheets
			// 
			this.btnMakeFieldDataSheets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnMakeFieldDataSheets.Location = new System.Drawing.Point(11, 314);
			this.btnMakeFieldDataSheets.Name = "btnMakeFieldDataSheets";
			this.btnMakeFieldDataSheets.Size = new System.Drawing.Size(84, 45);
			this.btnMakeFieldDataSheets.TabIndex = 22;
			this.btnMakeFieldDataSheets.Text = "Export Field Data Sheets";
			this.btnMakeFieldDataSheets.UseVisualStyleBackColor = true;
			this.btnMakeFieldDataSheets.Click += new System.EventHandler(this.btnMakeFieldDataSheets_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 367);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(545, 22);
			this.statusStrip1.TabIndex = 23;
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
			// fieldDataSheetsWorker
			// 
			this.fieldDataSheetsWorker.WorkerReportsProgress = true;
			this.fieldDataSheetsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fieldDataSheetsWorker_DoWork);
			this.fieldDataSheetsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.fieldDataSheetsWorker_RunWorkerCompleted);
			this.fieldDataSheetsWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.fieldDataSheetsWorker_ProgressChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnGridColLayoutCCW);
			this.groupBox1.Controls.Add(this.btnGridColLayoutCW);
			this.groupBox1.Location = new System.Drawing.Point(363, 91);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(169, 68);
			this.groupBox1.TabIndex = 24;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Default grid column layout";
			// 
			// btnGridColLayoutCCW
			// 
			this.btnGridColLayoutCCW.AutoSize = true;
			this.btnGridColLayoutCCW.Location = new System.Drawing.Point(6, 42);
			this.btnGridColLayoutCCW.Name = "btnGridColLayoutCCW";
			this.btnGridColLayoutCCW.Size = new System.Drawing.Size(147, 17);
			this.btnGridColLayoutCCW.TabIndex = 1;
			this.btnGridColLayoutCCW.Text = "CCW looking downstream";
			this.btnGridColLayoutCCW.UseVisualStyleBackColor = true;
			// 
			// btnGridColLayoutCW
			// 
			this.btnGridColLayoutCW.AutoSize = true;
			this.btnGridColLayoutCW.Checked = true;
			this.btnGridColLayoutCW.Location = new System.Drawing.Point(6, 19);
			this.btnGridColLayoutCW.Name = "btnGridColLayoutCW";
			this.btnGridColLayoutCW.Size = new System.Drawing.Size(140, 17);
			this.btnGridColLayoutCW.TabIndex = 0;
			this.btnGridColLayoutCW.TabStop = true;
			this.btnGridColLayoutCW.Text = "CW looking downstream";
			this.btnGridColLayoutCW.UseVisualStyleBackColor = true;
			// 
			// btnPreviewLabels
			// 
			this.btnPreviewLabels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPreviewLabels.Location = new System.Drawing.Point(101, 314);
			this.btnPreviewLabels.Name = "btnPreviewLabels";
			this.btnPreviewLabels.Size = new System.Drawing.Size(84, 45);
			this.btnPreviewLabels.TabIndex = 25;
			this.btnPreviewLabels.Text = "Preview WO Labels";
			this.btnPreviewLabels.UseVisualStyleBackColor = true;
			this.btnPreviewLabels.Click += new System.EventHandler(this.btnPreviewLabels_Click);
			// 
			// txtFacPhone
			// 
			this.txtFacPhone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFacPhone.Location = new System.Drawing.Point(432, 35);
			this.txtFacPhone.Name = "txtFacPhone";
			this.txtFacPhone.Size = new System.Drawing.Size(100, 20);
			this.txtFacPhone.TabIndex = 6;
			this.txtFacPhone.TextChanged += new System.EventHandler(this.txtFacPhone_TextChanged);
			// 
			// txtCouplantBatch
			// 
			this.txtCouplantBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCouplantBatch.Location = new System.Drawing.Point(432, 61);
			this.txtCouplantBatch.Name = "txtCouplantBatch";
			this.txtCouplantBatch.Size = new System.Drawing.Size(100, 20);
			this.txtCouplantBatch.TabIndex = 10;
			this.txtCouplantBatch.TextChanged += new System.EventHandler(this.txtCouplantBatch_TextChanged);
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(119, 35);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(100, 20);
			this.txtName.TabIndex = 2;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// lblOutageDataImportedOn
			// 
			this.lblOutageDataImportedOn.AutoSize = true;
			this.lblOutageDataImportedOn.Location = new System.Drawing.Point(320, 313);
			this.lblOutageDataImportedOn.Name = "lblOutageDataImportedOn";
			this.lblOutageDataImportedOn.Size = new System.Drawing.Size(148, 13);
			this.lblOutageDataImportedOn.TabIndex = 26;
			this.lblOutageDataImportedOn.Text = "Outage Data was imported on";
			// 
			// OutageEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(197)))), ((int)(((byte)(196)))));
			this.ClientSize = new System.Drawing.Size(545, 389);
			this.Controls.Add(this.lblOutageDataImportedOn);
			this.Controls.Add(this.btnPreviewLabels);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.btnMakeFieldDataSheets);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.clbInspectors);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.clbGridProcedures);
			this.Controls.Add(this.btnExportConfig);
			this.Controls.Add(this.txtFacPhone);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.dtpEndDate);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.dtpStartDate);
			this.Controls.Add(this.txtCouplantBatch);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cboCouplantType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cboCalibrationProc);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(537, 381);
			this.Name = "OutageEdit";
			this.Text = "Edit Outage";
			this.Resize += new System.EventHandler(this.OutageEdit_Resize);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnCancel;
		private TextBoxWithUndo txtName;
		private System.Windows.Forms.Label lblSiteName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.DateTimePicker dtpEndDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.DateTimePicker dtpStartDate;
		private TextBoxWithUndo txtCouplantBatch;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cboCouplantType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cboCalibrationProc;
		private System.Windows.Forms.Button btnExportConfig;
		private TextBoxWithUndo txtFacPhone;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckedListBox clbGridProcedures;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckedListBox clbInspectors;
		private System.Windows.Forms.Button btnMakeFieldDataSheets;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.ComponentModel.BackgroundWorker fieldDataSheetsWorker;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton btnGridColLayoutCCW;
		private System.Windows.Forms.RadioButton btnGridColLayoutCW;
		private System.Windows.Forms.Button btnPreviewLabels;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Label lblOutageDataImportedOn;
	}
}

