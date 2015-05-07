namespace Factotum
{
	partial class InspectedComponentView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectedComponentView));
            this.btnEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.cboHasMin = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtComponentID = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboStatusComplete = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cboPrepComplete = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtReportID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboUtFieldCplt = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cboReviewer = new System.Windows.Forms.ComboBox();
            this.cboSubmitted = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboFinal = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.btnStatusRept = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.validateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentReportPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printComponentReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvReportList = new Factotum.DataGridViewStd();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(11, 298);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(64, 22);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Component Report List";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(81, 298);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(64, 22);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(151, 298);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(64, 22);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cboHasMin
            // 
            this.cboHasMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHasMin.FormattingEnabled = true;
            this.cboHasMin.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboHasMin.Location = new System.Drawing.Point(434, 41);
            this.cboHasMin.Name = "cboHasMin";
            this.cboHasMin.Size = new System.Drawing.Size(85, 21);
            this.cboHasMin.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(382, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Has Min";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Component ID";
            // 
            // txtComponentID
            // 
            this.txtComponentID.Location = new System.Drawing.Point(93, 42);
            this.txtComponentID.Name = "txtComponentID";
            this.txtComponentID.Size = new System.Drawing.Size(85, 20);
            this.txtComponentID.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cboStatusComplete);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.cboPrepComplete);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.txtReportID);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cboUtFieldCplt);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.cboReviewer);
            this.panel1.Controls.Add(this.cboSubmitted);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cboFinal);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtComponentID);
            this.panel1.Controls.Add(this.cboHasMin);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(15, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 101);
            this.panel1.TabIndex = 1;
            // 
            // cboStatusComplete
            // 
            this.cboStatusComplete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatusComplete.FormattingEnabled = true;
            this.cboStatusComplete.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboStatusComplete.Location = new System.Drawing.Point(272, 68);
            this.cboStatusComplete.Name = "cboStatusComplete";
            this.cboStatusComplete.Size = new System.Drawing.Size(85, 21);
            this.cboStatusComplete.TabIndex = 17;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(205, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Status Cplt.";
            // 
            // cboPrepComplete
            // 
            this.cboPrepComplete.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrepComplete.FormattingEnabled = true;
            this.cboPrepComplete.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboPrepComplete.Location = new System.Drawing.Point(272, 14);
            this.cboPrepComplete.Name = "cboPrepComplete";
            this.cboPrepComplete.Size = new System.Drawing.Size(85, 21);
            this.cboPrepComplete.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(212, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Prep Cplt.";
            // 
            // txtReportID
            // 
            this.txtReportID.Location = new System.Drawing.Point(93, 16);
            this.txtReportID.Name = "txtReportID";
            this.txtReportID.Size = new System.Drawing.Size(85, 20);
            this.txtReportID.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Report ID";
            // 
            // cboUtFieldCplt
            // 
            this.cboUtFieldCplt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUtFieldCplt.FormattingEnabled = true;
            this.cboUtFieldCplt.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboUtFieldCplt.Location = new System.Drawing.Point(272, 41);
            this.cboUtFieldCplt.Name = "cboUtFieldCplt";
            this.cboUtFieldCplt.Size = new System.Drawing.Size(85, 21);
            this.cboUtFieldCplt.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(195, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "UT Field Cplt.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(376, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Reviewer";
            // 
            // cboReviewer
            // 
            this.cboReviewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReviewer.FormattingEnabled = true;
            this.cboReviewer.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
            this.cboReviewer.Location = new System.Drawing.Point(434, 68);
            this.cboReviewer.Name = "cboReviewer";
            this.cboReviewer.Size = new System.Drawing.Size(85, 21);
            this.cboReviewer.TabIndex = 15;
            // 
            // cboSubmitted
            // 
            this.cboSubmitted.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubmitted.FormattingEnabled = true;
            this.cboSubmitted.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboSubmitted.Location = new System.Drawing.Point(93, 68);
            this.cboSubmitted.Name = "cboSubmitted";
            this.cboSubmitted.Size = new System.Drawing.Size(85, 21);
            this.cboSubmitted.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Submitted";
            // 
            // cboFinal
            // 
            this.cboFinal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFinal.FormattingEnabled = true;
            this.cboFinal.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboFinal.Location = new System.Drawing.Point(434, 14);
            this.cboFinal.Name = "cboFinal";
            this.cboFinal.Size = new System.Drawing.Size(85, 21);
            this.cboFinal.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(399, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Final";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(22, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "List Filters";
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.Location = new System.Drawing.Point(488, 298);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(64, 22);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreview.Location = new System.Drawing.Point(418, 298);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(64, 22);
            this.btnPreview.TabIndex = 8;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnValidate.Location = new System.Drawing.Point(348, 298);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(64, 22);
            this.btnValidate.TabIndex = 9;
            this.btnValidate.Text = "Validate";
            this.btnValidate.UseVisualStyleBackColor = true;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnStatusRept
            // 
            this.btnStatusRept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStatusRept.Location = new System.Drawing.Point(278, 298);
            this.btnStatusRept.Name = "btnStatusRept";
            this.btnStatusRept.Size = new System.Drawing.Size(64, 22);
            this.btnStatusRept.TabIndex = 10;
            this.btnStatusRept.Text = "Status";
            this.btnStatusRept.UseVisualStyleBackColor = true;
            this.btnStatusRept.Click += new System.EventHandler(this.btnStatusRept_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.validateToolStripMenuItem,
            this.editReportToolStripMenuItem,
            this.componentReportPreviewToolStripMenuItem,
            this.printComponentReportToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(218, 114);
            // 
            // validateToolStripMenuItem
            // 
            this.validateToolStripMenuItem.Name = "validateToolStripMenuItem";
            this.validateToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.validateToolStripMenuItem.Text = "Validate";
            this.validateToolStripMenuItem.Click += new System.EventHandler(this.validateToolStripMenuItem_Click);
            // 
            // editReportToolStripMenuItem
            // 
            this.editReportToolStripMenuItem.Name = "editReportToolStripMenuItem";
            this.editReportToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.editReportToolStripMenuItem.Text = "Edit";
            this.editReportToolStripMenuItem.Click += new System.EventHandler(this.editReportToolStripMenuItem_Click);
            // 
            // componentReportPreviewToolStripMenuItem
            // 
            this.componentReportPreviewToolStripMenuItem.Name = "componentReportPreviewToolStripMenuItem";
            this.componentReportPreviewToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.componentReportPreviewToolStripMenuItem.Text = "Preview Component Report";
            this.componentReportPreviewToolStripMenuItem.Click += new System.EventHandler(this.componentReportPreviewToolStripMenuItem_Click);
            // 
            // printComponentReportToolStripMenuItem
            // 
            this.printComponentReportToolStripMenuItem.Name = "printComponentReportToolStripMenuItem";
            this.printComponentReportToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.printComponentReportToolStripMenuItem.Text = "Print Component Report";
            this.printComponentReportToolStripMenuItem.Click += new System.EventHandler(this.printComponentReportToolStripMenuItem_Click);
            // 
            // dgvReportList
            // 
            this.dgvReportList.AllowUserToAddRows = false;
            this.dgvReportList.AllowUserToDeleteRows = false;
            this.dgvReportList.AllowUserToOrderColumns = true;
            this.dgvReportList.AllowUserToResizeRows = false;
            this.dgvReportList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReportList.Location = new System.Drawing.Point(12, 151);
            this.dgvReportList.MultiSelect = false;
            this.dgvReportList.Name = "dgvReportList";
            this.dgvReportList.ReadOnly = true;
            this.dgvReportList.RowHeadersVisible = false;
            this.dgvReportList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReportList.Size = new System.Drawing.Size(535, 141);
            this.dgvReportList.StandardTab = true;
            this.dgvReportList.TabIndex = 6;
            this.dgvReportList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvReportList_KeyDown);
            this.dgvReportList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvReportList_CellMouseClick);
            this.dgvReportList.DoubleClick += new System.EventHandler(this.btnEdit_Click);
            // 
            // InspectedComponentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(170)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(559, 332);
            this.Controls.Add(this.btnStatusRept);
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvReportList);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnEdit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(567, 302);
            this.Name = "InspectedComponentView";
            this.Text = "View Component Reports";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InspectedComponentView_FormClosed);
            this.SizeChanged += new System.EventHandler(this.InspectedComponentView_SizeChanged);
            this.Load += new System.EventHandler(this.InspectedComponentView_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.ComboBox cboHasMin;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtComponentID;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cboSubmitted;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cboFinal;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox cboReviewer;
		private System.Windows.Forms.ComboBox cboUtFieldCplt;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtReportID;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboPrepComplete;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnPrint;
		private System.Windows.Forms.Button btnPreview;
		private DataGridViewStd dgvReportList;
		private System.Windows.Forms.ComboBox cboStatusComplete;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button btnValidate;
		private System.Windows.Forms.Button btnStatusRept;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem validateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem componentReportPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printComponentReportToolStripMenuItem;
	}
}