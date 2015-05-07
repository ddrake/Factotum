namespace Factotum
{
	partial class Preferences_General
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnBrowse_MeterData = new System.Windows.Forms.Button();
            this.txtMeterDataFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowse_Image = new System.Windows.Forms.Button();
            this.txtDefaultImageFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFactotumDataFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ckValidateTemperatures = new System.Windows.Forms.CheckBox();
            this.ckValidateEpriRecommended = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ckValidateNoUpMain = new System.Windows.Forms.CheckBox();
            this.ckValidateNoUpExt = new System.Windows.Forms.CheckBox();
            this.ckValidateNoDnExt = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbValidateColsCeiling = new System.Windows.Forms.RadioButton();
            this.rbValidateColsRound = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbValidateDsExtRowsRound = new System.Windows.Forms.RadioButton();
            this.rbValidateDsExtRowsCeiling = new System.Windows.Forms.RadioButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(217, 359);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(298, 359);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(361, 341);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnBrowse_MeterData);
            this.tabPage1.Controls.Add(this.txtMeterDataFolder);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.btnBrowse_Image);
            this.tabPage1.Controls.Add(this.txtDefaultImageFolder);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnBrowse);
            this.tabPage1.Controls.Add(this.txtFactotumDataFolder);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(353, 315);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Default Folders";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnBrowse_MeterData
            // 
            this.btnBrowse_MeterData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse_MeterData.Location = new System.Drawing.Point(272, 102);
            this.btnBrowse_MeterData.Name = "btnBrowse_MeterData";
            this.btnBrowse_MeterData.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse_MeterData.TabIndex = 21;
            this.btnBrowse_MeterData.Text = "Browse...";
            this.btnBrowse_MeterData.UseVisualStyleBackColor = true;
            this.btnBrowse_MeterData.Click += new System.EventHandler(this.btnBrowse_MeterData_Click);
            // 
            // txtMeterDataFolder
            // 
            this.txtMeterDataFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMeterDataFolder.Location = new System.Drawing.Point(17, 104);
            this.txtMeterDataFolder.Multiline = true;
            this.txtMeterDataFolder.Name = "txtMeterDataFolder";
            this.txtMeterDataFolder.ReadOnly = true;
            this.txtMeterDataFolder.Size = new System.Drawing.Size(249, 47);
            this.txtMeterDataFolder.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Meter Data Folder";
            // 
            // btnBrowse_Image
            // 
            this.btnBrowse_Image.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse_Image.Location = new System.Drawing.Point(272, 187);
            this.btnBrowse_Image.Name = "btnBrowse_Image";
            this.btnBrowse_Image.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse_Image.TabIndex = 18;
            this.btnBrowse_Image.Text = "Browse...";
            this.btnBrowse_Image.UseVisualStyleBackColor = true;
            this.btnBrowse_Image.Click += new System.EventHandler(this.btnBrowse_Image_Click);
            // 
            // txtDefaultImageFolder
            // 
            this.txtDefaultImageFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefaultImageFolder.Location = new System.Drawing.Point(17, 187);
            this.txtDefaultImageFolder.Multiline = true;
            this.txtDefaultImageFolder.Name = "txtDefaultImageFolder";
            this.txtDefaultImageFolder.ReadOnly = true;
            this.txtDefaultImageFolder.Size = new System.Drawing.Size(249, 47);
            this.txtDefaultImageFolder.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Default Image Folder";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(272, 25);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 15;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFactotumDataFolder
            // 
            this.txtFactotumDataFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFactotumDataFolder.Location = new System.Drawing.Point(17, 25);
            this.txtFactotumDataFolder.Multiline = true;
            this.txtFactotumDataFolder.Name = "txtFactotumDataFolder";
            this.txtFactotumDataFolder.ReadOnly = true;
            this.txtFactotumDataFolder.Size = new System.Drawing.Size(249, 47);
            this.txtFactotumDataFolder.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Factotum Data Folder";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.ckValidateNoDnExt);
            this.tabPage2.Controls.Add(this.ckValidateNoUpExt);
            this.tabPage2.Controls.Add(this.ckValidateNoUpMain);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.ckValidateEpriRecommended);
            this.tabPage2.Controls.Add(this.ckValidateTemperatures);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(353, 315);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Validation";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ckValidateTemperatures
            // 
            this.ckValidateTemperatures.AutoSize = true;
            this.ckValidateTemperatures.Location = new System.Drawing.Point(27, 15);
            this.ckValidateTemperatures.Name = "ckValidateTemperatures";
            this.ckValidateTemperatures.Size = new System.Drawing.Size(183, 17);
            this.ckValidateTemperatures.TabIndex = 0;
            this.ckValidateTemperatures.Text = "Warn about missing temperatures";
            this.ckValidateTemperatures.UseVisualStyleBackColor = true;
            // 
            // ckValidateEpriRecommended
            // 
            this.ckValidateEpriRecommended.AutoSize = true;
            this.ckValidateEpriRecommended.Location = new System.Drawing.Point(27, 38);
            this.ckValidateEpriRecommended.Name = "ckValidateEpriRecommended";
            this.ckValidateEpriRecommended.Size = new System.Drawing.Size(259, 17);
            this.ckValidateEpriRecommended.TabIndex = 1;
            this.ckValidateEpriRecommended.Text = "Warn about grid sizes above EPRI recommended";
            this.ckValidateEpriRecommended.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(195, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Note: Warnings will always be displayed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(44, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "for grid sizes above EPRI maximum.";
            // 
            // ckValidateNoUpMain
            // 
            this.ckValidateNoUpMain.AutoSize = true;
            this.ckValidateNoUpMain.Location = new System.Drawing.Point(27, 90);
            this.ckValidateNoUpMain.Name = "ckValidateNoUpMain";
            this.ckValidateNoUpMain.Size = new System.Drawing.Size(202, 17);
            this.ckValidateNoUpMain.TabIndex = 4;
            this.ckValidateNoUpMain.Text = "Warn about no Upstream Main in grid";
            this.ckValidateNoUpMain.UseVisualStyleBackColor = true;
            // 
            // ckValidateNoUpExt
            // 
            this.ckValidateNoUpExt.AutoSize = true;
            this.ckValidateNoUpExt.Location = new System.Drawing.Point(27, 113);
            this.ckValidateNoUpExt.Name = "ckValidateNoUpExt";
            this.ckValidateNoUpExt.Size = new System.Drawing.Size(225, 17);
            this.ckValidateNoUpExt.TabIndex = 5;
            this.ckValidateNoUpExt.Text = "Warn about no Upstream Extension in grid";
            this.ckValidateNoUpExt.UseVisualStyleBackColor = true;
            // 
            // ckValidateNoDnExt
            // 
            this.ckValidateNoDnExt.AutoSize = true;
            this.ckValidateNoDnExt.Location = new System.Drawing.Point(27, 136);
            this.ckValidateNoDnExt.Name = "ckValidateNoDnExt";
            this.ckValidateNoDnExt.Size = new System.Drawing.Size(239, 17);
            this.ckValidateNoDnExt.TabIndex = 6;
            this.ckValidateNoDnExt.Text = "Warn about no Downstream Extension in grid";
            this.ckValidateNoDnExt.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbValidateColsRound);
            this.groupBox1.Controls.Add(this.rbValidateColsCeiling);
            this.groupBox1.Location = new System.Drawing.Point(27, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 69);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calculation of the number of Grid Columns";
            // 
            // rbValidateColsCeiling
            // 
            this.rbValidateColsCeiling.AutoSize = true;
            this.rbValidateColsCeiling.Location = new System.Drawing.Point(20, 20);
            this.rbValidateColsCeiling.Name = "rbValidateColsCeiling";
            this.rbValidateColsCeiling.Size = new System.Drawing.Size(193, 17);
            this.rbValidateColsCeiling.TabIndex = 0;
            this.rbValidateColsCeiling.TabStop = true;
            this.rbValidateColsCeiling.Text = "Ceiling ( PI * OD / Radial Grid Size )";
            this.rbValidateColsCeiling.UseVisualStyleBackColor = true;
            // 
            // rbValidateColsRound
            // 
            this.rbValidateColsRound.AutoSize = true;
            this.rbValidateColsRound.Location = new System.Drawing.Point(20, 44);
            this.rbValidateColsRound.Name = "rbValidateColsRound";
            this.rbValidateColsRound.Size = new System.Drawing.Size(194, 17);
            this.rbValidateColsRound.TabIndex = 1;
            this.rbValidateColsRound.TabStop = true;
            this.rbValidateColsRound.Text = "Round ( PI * OD / Radial Grid Size )";
            this.rbValidateColsRound.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbValidateDsExtRowsRound);
            this.groupBox2.Controls.Add(this.rbValidateDsExtRowsCeiling);
            this.groupBox2.Location = new System.Drawing.Point(26, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 69);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Calculation of the number of Grid DS. Ext. Rows";
            // 
            // rbValidateDsExtRowsRound
            // 
            this.rbValidateDsExtRowsRound.AutoSize = true;
            this.rbValidateDsExtRowsRound.Location = new System.Drawing.Point(20, 44);
            this.rbValidateDsExtRowsRound.Name = "rbValidateDsExtRowsRound";
            this.rbValidateDsExtRowsRound.Size = new System.Drawing.Size(276, 17);
            this.rbValidateDsExtRowsRound.TabIndex = 1;
            this.rbValidateDsExtRowsRound.TabStop = true;
            this.rbValidateDsExtRowsRound.Text = "Round ( Grid Proc. DS Diam\'s * OD / Grid Axial Dist. )";
            this.rbValidateDsExtRowsRound.UseVisualStyleBackColor = true;
            // 
            // rbValidateDsExtRowsCeiling
            // 
            this.rbValidateDsExtRowsCeiling.AutoSize = true;
            this.rbValidateDsExtRowsCeiling.Location = new System.Drawing.Point(20, 20);
            this.rbValidateDsExtRowsCeiling.Name = "rbValidateDsExtRowsCeiling";
            this.rbValidateDsExtRowsCeiling.Size = new System.Drawing.Size(275, 17);
            this.rbValidateDsExtRowsCeiling.TabIndex = 0;
            this.rbValidateDsExtRowsCeiling.TabStop = true;
            this.rbValidateDsExtRowsCeiling.Text = "Ceiling ( Grid Proc. DS Diam\'s * OD / Grid Axial Dist. )";
            this.rbValidateDsExtRowsCeiling.UseVisualStyleBackColor = true;
            // 
            // Preferences_General
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 393);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MinimumSize = new System.Drawing.Size(352, 358);
            this.Name = "Preferences_General";
            this.Text = "General Preferences";
            this.Load += new System.EventHandler(this.GeneralPreferences_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnBrowse_MeterData;
        private System.Windows.Forms.TextBox txtMeterDataFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowse_Image;
        private System.Windows.Forms.TextBox txtDefaultImageFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFactotumDataFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox ckValidateEpriRecommended;
        private System.Windows.Forms.CheckBox ckValidateTemperatures;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckValidateNoDnExt;
        private System.Windows.Forms.CheckBox ckValidateNoUpExt;
        private System.Windows.Forms.CheckBox ckValidateNoUpMain;
        private System.Windows.Forms.RadioButton rbValidateColsRound;
        private System.Windows.Forms.RadioButton rbValidateColsCeiling;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbValidateDsExtRowsRound;
        private System.Windows.Forms.RadioButton rbValidateDsExtRowsCeiling;
	}
}