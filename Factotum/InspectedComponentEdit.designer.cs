namespace Factotum
{
	partial class InspectedComponentEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectedComponentEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.cboReviewer = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cboGridProcedure = new System.Windows.Forms.ComboBox();
			this.lblSiteName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.ckPrepComplete = new System.Windows.Forms.CheckBox();
			this.ckFinal = new System.Windows.Forms.CheckBox();
			this.lblMinCount = new System.Windows.Forms.Label();
			this.lblReportSubmittedOn = new System.Windows.Forms.Label();
			this.lblEdsNumber = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.ckUtFieldComplete = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.cboComponent = new System.Windows.Forms.ComboBox();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.ckReportSubmitted = new System.Windows.Forms.CheckBox();
			this.ckCompletionReported = new System.Windows.Forms.CheckBox();
			this.lblCompletionReportedOn = new System.Windows.Forms.Label();
			this.dgvReportSections = new Factotum.DataGridViewStd();
			this.txtPageCountOverride = new Factotum.TextBoxWithUndo();
			this.txtWorkOrder = new Factotum.TextBoxWithUndo();
			this.txtSpecificArea = new Factotum.TextBoxWithUndo();
			this.txtReportID = new Factotum.TextBoxWithUndo();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvReportSections)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(366, 336);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 22;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(49, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Report #";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(436, 336);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 23;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// cboReviewer
			// 
			this.cboReviewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboReviewer.FormattingEnabled = true;
			this.cboReviewer.Location = new System.Drawing.Point(104, 118);
			this.cboReviewer.Name = "cboReviewer";
			this.cboReviewer.Size = new System.Drawing.Size(147, 21);
			this.cboReviewer.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(47, 121);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Reviewer";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(20, 148);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Grid Procedure";
			// 
			// cboGridProcedure
			// 
			this.cboGridProcedure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboGridProcedure.FormattingEnabled = true;
			this.cboGridProcedure.Location = new System.Drawing.Point(104, 145);
			this.cboGridProcedure.Name = "cboGridProcedure";
			this.cboGridProcedure.Size = new System.Drawing.Size(147, 21);
			this.cboGridProcedure.TabIndex = 9;
			// 
			// lblSiteName
			// 
			this.lblSiteName.AutoSize = true;
			this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
			this.lblSiteName.Location = new System.Drawing.Point(86, 9);
			this.lblSiteName.Name = "lblSiteName";
			this.lblSiteName.Size = new System.Drawing.Size(80, 16);
			this.lblSiteName.TabIndex = 13;
			this.lblSiteName.Text = "Component:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 175);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Specific Area";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(26, 69);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Work Order #";
			// 
			// ckPrepComplete
			// 
			this.ckPrepComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ckPrepComplete.AutoSize = true;
			this.ckPrepComplete.Location = new System.Drawing.Point(274, 65);
			this.ckPrepComplete.Name = "ckPrepComplete";
			this.ckPrepComplete.Size = new System.Drawing.Size(95, 17);
			this.ckPrepComplete.TabIndex = 12;
			this.ckPrepComplete.Text = "Prep Complete";
			this.ckPrepComplete.UseVisualStyleBackColor = true;
			// 
			// ckFinal
			// 
			this.ckFinal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ckFinal.AutoSize = true;
			this.ckFinal.Location = new System.Drawing.Point(276, 174);
			this.ckFinal.Name = "ckFinal";
			this.ckFinal.Size = new System.Drawing.Size(48, 17);
			this.ckFinal.TabIndex = 14;
			this.ckFinal.Text = "Final";
			this.ckFinal.UseVisualStyleBackColor = true;
			// 
			// lblMinCount
			// 
			this.lblMinCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblMinCount.AutoSize = true;
			this.lblMinCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblMinCount.ForeColor = System.Drawing.Color.Firebrick;
			this.lblMinCount.Location = new System.Drawing.Point(336, 42);
			this.lblMinCount.Name = "lblMinCount";
			this.lblMinCount.Size = new System.Drawing.Size(118, 13);
			this.lblMinCount.TabIndex = 21;
			this.lblMinCount.Text = "### below Tscreen";
			// 
			// lblReportSubmittedOn
			// 
			this.lblReportSubmittedOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblReportSubmittedOn.AutoSize = true;
			this.lblReportSubmittedOn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblReportSubmittedOn.Location = new System.Drawing.Point(400, 112);
			this.lblReportSubmittedOn.Name = "lblReportSubmittedOn";
			this.lblReportSubmittedOn.Size = new System.Drawing.Size(83, 13);
			this.lblReportSubmittedOn.TabIndex = 22;
			this.lblReportSubmittedOn.Text = "12/12/08 14:23";
			// 
			// lblEdsNumber
			// 
			this.lblEdsNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblEdsNumber.AutoSize = true;
			this.lblEdsNumber.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblEdsNumber.Location = new System.Drawing.Point(273, 42);
			this.lblEdsNumber.Name = "lblEdsNumber";
			this.lblEdsNumber.Size = new System.Drawing.Size(57, 13);
			this.lblEdsNumber.TabIndex = 23;
			this.lblEdsNumber.Text = "EDS #: 12";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(20, 203);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(146, 13);
			this.label13.TabIndex = 24;
			this.label13.Text = "Inspections (Report Sections)";
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(348, 175);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(64, 13);
			this.label14.TabIndex = 15;
			this.label14.Text = "Total Pages";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Location = new System.Drawing.Point(89, 303);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(64, 22);
			this.btnAdd.TabIndex = 18;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnEdit.Location = new System.Drawing.Point(19, 303);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(64, 22);
			this.btnEdit.TabIndex = 17;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDelete.Location = new System.Drawing.Point(159, 303);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(64, 22);
			this.btnDelete.TabIndex = 19;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// ckUtFieldComplete
			// 
			this.ckUtFieldComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ckUtFieldComplete.AutoSize = true;
			this.ckUtFieldComplete.Location = new System.Drawing.Point(274, 88);
			this.ckUtFieldComplete.Name = "ckUtFieldComplete";
			this.ckUtFieldComplete.Size = new System.Drawing.Size(113, 17);
			this.ckUtFieldComplete.TabIndex = 13;
			this.ckUtFieldComplete.Text = "UT Field Complete";
			this.ckUtFieldComplete.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(37, 42);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(61, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Component";
			// 
			// cboComponent
			// 
			this.cboComponent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboComponent.FormattingEnabled = true;
			this.cboComponent.Location = new System.Drawing.Point(104, 39);
			this.cboComponent.Name = "cboComponent";
			this.cboComponent.Size = new System.Drawing.Size(147, 21);
			this.cboComponent.TabIndex = 1;
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveDown.Location = new System.Drawing.Point(411, 303);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(75, 22);
			this.btnMoveDown.TabIndex = 21;
			this.btnMoveDown.Text = "Move Down";
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveUp.Location = new System.Drawing.Point(330, 303);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(75, 22);
			this.btnMoveUp.TabIndex = 20;
			this.btnMoveUp.Text = "Move Up";
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// ckReportSubmitted
			// 
			this.ckReportSubmitted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ckReportSubmitted.AutoSize = true;
			this.ckReportSubmitted.Location = new System.Drawing.Point(274, 111);
			this.ckReportSubmitted.Name = "ckReportSubmitted";
			this.ckReportSubmitted.Size = new System.Drawing.Size(108, 17);
			this.ckReportSubmitted.TabIndex = 26;
			this.ckReportSubmitted.Text = "Report Submitted";
			this.ckReportSubmitted.UseVisualStyleBackColor = true;
			// 
			// ckCompletionReported
			// 
			this.ckCompletionReported.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ckCompletionReported.AutoSize = true;
			this.ckCompletionReported.Location = new System.Drawing.Point(274, 134);
			this.ckCompletionReported.Name = "ckCompletionReported";
			this.ckCompletionReported.Size = new System.Drawing.Size(125, 17);
			this.ckCompletionReported.TabIndex = 27;
			this.ckCompletionReported.Text = "Completion Reported";
			this.ckCompletionReported.UseVisualStyleBackColor = true;
			// 
			// lblCompletionReportedOn
			// 
			this.lblCompletionReportedOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblCompletionReportedOn.AutoSize = true;
			this.lblCompletionReportedOn.ForeColor = System.Drawing.SystemColors.ControlText;
			this.lblCompletionReportedOn.Location = new System.Drawing.Point(400, 135);
			this.lblCompletionReportedOn.Name = "lblCompletionReportedOn";
			this.lblCompletionReportedOn.Size = new System.Drawing.Size(83, 13);
			this.lblCompletionReportedOn.TabIndex = 28;
			this.lblCompletionReportedOn.Text = "12/12/08 14:23";
			// 
			// dgvReportSections
			// 
			this.dgvReportSections.AllowUserToAddRows = false;
			this.dgvReportSections.AllowUserToDeleteRows = false;
			this.dgvReportSections.AllowUserToOrderColumns = true;
			this.dgvReportSections.AllowUserToResizeRows = false;
			this.dgvReportSections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvReportSections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvReportSections.Location = new System.Drawing.Point(23, 219);
			this.dgvReportSections.MultiSelect = false;
			this.dgvReportSections.Name = "dgvReportSections";
			this.dgvReportSections.ReadOnly = true;
			this.dgvReportSections.RowHeadersVisible = false;
			this.dgvReportSections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvReportSections.Size = new System.Drawing.Size(460, 78);
			this.dgvReportSections.StandardTab = true;
			this.dgvReportSections.TabIndex = 25;
			this.dgvReportSections.DoubleClick += new System.EventHandler(this.btnEdit_Click);
			this.dgvReportSections.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvReportSections_KeyDown);
			this.dgvReportSections.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgvReportSections_ColumnAdded);
			// 
			// txtPageCountOverride
			// 
			this.txtPageCountOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPageCountOverride.Location = new System.Drawing.Point(418, 172);
			this.txtPageCountOverride.Name = "txtPageCountOverride";
			this.txtPageCountOverride.Size = new System.Drawing.Size(65, 20);
			this.txtPageCountOverride.TabIndex = 16;
			this.txtPageCountOverride.TextChanged += new System.EventHandler(this.txtPageCountOverride_TextChanged);
			this.txtPageCountOverride.Validating += new System.ComponentModel.CancelEventHandler(this.txtPageCountOverride_Validating);
			// 
			// txtWorkOrder
			// 
			this.txtWorkOrder.Location = new System.Drawing.Point(104, 66);
			this.txtWorkOrder.Name = "txtWorkOrder";
			this.txtWorkOrder.Size = new System.Drawing.Size(147, 20);
			this.txtWorkOrder.TabIndex = 3;
			this.txtWorkOrder.TextChanged += new System.EventHandler(this.txtWorkOrder_TextChanged);
			this.txtWorkOrder.Validating += new System.ComponentModel.CancelEventHandler(this.txtWorkOrder_Validating);
			// 
			// txtSpecificArea
			// 
			this.txtSpecificArea.Location = new System.Drawing.Point(104, 172);
			this.txtSpecificArea.Name = "txtSpecificArea";
			this.txtSpecificArea.Size = new System.Drawing.Size(147, 20);
			this.txtSpecificArea.TabIndex = 11;
			this.txtSpecificArea.TextChanged += new System.EventHandler(this.txtSpecificArea_TextChanged);
			this.txtSpecificArea.Validating += new System.ComponentModel.CancelEventHandler(this.txtSpecificArea_Validating);
			// 
			// txtReportID
			// 
			this.txtReportID.Location = new System.Drawing.Point(104, 92);
			this.txtReportID.Name = "txtReportID";
			this.txtReportID.Size = new System.Drawing.Size(147, 20);
			this.txtReportID.TabIndex = 5;
			this.txtReportID.TextChanged += new System.EventHandler(this.txtReportID_TextChanged);
			this.txtReportID.Validating += new System.ComponentModel.CancelEventHandler(this.txtReportID_Validating);
			// 
			// InspectedComponentEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(177)))), ((int)(((byte)(150)))));
			this.ClientSize = new System.Drawing.Size(512, 370);
			this.Controls.Add(this.lblCompletionReportedOn);
			this.Controls.Add(this.ckCompletionReported);
			this.Controls.Add(this.ckReportSubmitted);
			this.Controls.Add(this.dgvReportSections);
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.cboComponent);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.txtPageCountOverride);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.lblEdsNumber);
			this.Controls.Add(this.lblReportSubmittedOn);
			this.Controls.Add(this.lblMinCount);
			this.Controls.Add(this.ckUtFieldComplete);
			this.Controls.Add(this.ckFinal);
			this.Controls.Add(this.ckPrepComplete);
			this.Controls.Add(this.txtWorkOrder);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtSpecificArea);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cboGridProcedure);
			this.Controls.Add(this.txtReportID);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cboReviewer);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(520, 368);
			this.Name = "InspectedComponentEdit";
			this.Text = "Edit Component Report";
			this.Load += new System.EventHandler(this.InspectedComponentEdit_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InspectedComponentEdit_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvReportSections)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cboReviewer;
		private TextBoxWithUndo txtReportID;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cboGridProcedure;
		private System.Windows.Forms.Label lblSiteName;
		private TextBoxWithUndo txtWorkOrder;
		private System.Windows.Forms.Label label4;
		private TextBoxWithUndo txtSpecificArea;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblReportSubmittedOn;
		private System.Windows.Forms.Label lblMinCount;
		private System.Windows.Forms.CheckBox ckFinal;
		private System.Windows.Forms.CheckBox ckPrepComplete;
		private System.Windows.Forms.Label lblEdsNumber;
		private System.Windows.Forms.Label label13;
		private TextBoxWithUndo txtPageCountOverride;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.CheckBox ckUtFieldComplete;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox cboComponent;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private DataGridViewStd dgvReportSections;
		private System.Windows.Forms.CheckBox ckCompletionReported;
		private System.Windows.Forms.CheckBox ckReportSubmitted;
		private System.Windows.Forms.Label lblCompletionReportedOn;
	}
}

