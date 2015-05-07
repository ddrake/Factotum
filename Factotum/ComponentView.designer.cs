namespace Factotum
{
	partial class ComponentView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentView));
            this.btnEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.cboStatusFilter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNameFilter = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboUnitFilter = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.cboSystemFilter = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cboLineFilter = new System.Windows.Forms.ComboBox();
            this.cboDataCompleteFilter = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboInOutageFilter = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnComponentListing = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dgvComponentList = new Factotum.DataGridViewStd();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponentList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.Location = new System.Drawing.Point(11, 318);
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
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Component List";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(81, 318);
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
            this.btnDelete.Location = new System.Drawing.Point(151, 318);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(64, 22);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cboStatusFilter
            // 
            this.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatusFilter.FormattingEnabled = true;
            this.cboStatusFilter.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
            this.cboStatusFilter.Location = new System.Drawing.Point(463, 41);
            this.cboStatusFilter.Name = "cboStatusFilter";
            this.cboStatusFilter.Size = new System.Drawing.Size(85, 21);
            this.cboStatusFilter.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(420, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Status";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Component ID";
            // 
            // txtNameFilter
            // 
            this.txtNameFilter.Location = new System.Drawing.Point(93, 42);
            this.txtNameFilter.Name = "txtNameFilter";
            this.txtNameFilter.Size = new System.Drawing.Size(85, 20);
            this.txtNameFilter.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(204, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Unit";
            // 
            // cboUnitFilter
            // 
            this.cboUnitFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUnitFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboUnitFilter.FormattingEnabled = true;
            this.cboUnitFilter.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
            this.cboUnitFilter.Location = new System.Drawing.Point(240, 68);
            this.cboUnitFilter.Name = "cboUnitFilter";
            this.cboUnitFilter.Size = new System.Drawing.Size(125, 21);
            this.cboUnitFilter.TabIndex = 15;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.cboSystemFilter);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.cboLineFilter);
            this.panel1.Controls.Add(this.cboDataCompleteFilter);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cboInOutageFilter);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtNameFilter);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cboStatusFilter);
            this.panel1.Controls.Add(this.cboUnitFilter);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(15, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(567, 101);
            this.panel1.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(193, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "System";
            // 
            // cboSystemFilter
            // 
            this.cboSystemFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSystemFilter.FormattingEnabled = true;
            this.cboSystemFilter.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
            this.cboSystemFilter.Location = new System.Drawing.Point(240, 41);
            this.cboSystemFilter.Name = "cboSystemFilter";
            this.cboSystemFilter.Size = new System.Drawing.Size(125, 21);
            this.cboSystemFilter.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(207, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Line";
            // 
            // cboLineFilter
            // 
            this.cboLineFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLineFilter.FormattingEnabled = true;
            this.cboLineFilter.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
            this.cboLineFilter.Location = new System.Drawing.Point(240, 13);
            this.cboLineFilter.Name = "cboLineFilter";
            this.cboLineFilter.Size = new System.Drawing.Size(125, 21);
            this.cboLineFilter.TabIndex = 3;
            // 
            // cboDataCompleteFilter
            // 
            this.cboDataCompleteFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataCompleteFilter.FormattingEnabled = true;
            this.cboDataCompleteFilter.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboDataCompleteFilter.Location = new System.Drawing.Point(93, 15);
            this.cboDataCompleteFilter.Name = "cboDataCompleteFilter";
            this.cboDataCompleteFilter.Size = new System.Drawing.Size(85, 21);
            this.cboDataCompleteFilter.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Data Complete";
            // 
            // cboInOutageFilter
            // 
            this.cboInOutageFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInOutageFilter.FormattingEnabled = true;
            this.cboInOutageFilter.Items.AddRange(new object[] {
            "All",
            "Yes",
            "No"});
            this.cboInOutageFilter.Location = new System.Drawing.Point(463, 14);
            this.cboInOutageFilter.Name = "cboInOutageFilter";
            this.cboInOutageFilter.Size = new System.Drawing.Size(85, 21);
            this.cboInOutageFilter.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(380, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "W/O Assigned";
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
            // btnComponentListing
            // 
            this.btnComponentListing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnComponentListing.Location = new System.Drawing.Point(480, 318);
            this.btnComponentListing.Name = "btnComponentListing";
            this.btnComponentListing.Size = new System.Drawing.Size(102, 23);
            this.btnComponentListing.TabIndex = 7;
            this.btnComponentListing.Text = "Component Listing";
            this.btnComponentListing.UseVisualStyleBackColor = true;
            this.btnComponentListing.Click += new System.EventHandler(this.btnComponentListing_Click);
            // 
            // dgvComponentList
            // 
            this.dgvComponentList.AllowUserToAddRows = false;
            this.dgvComponentList.AllowUserToDeleteRows = false;
            this.dgvComponentList.AllowUserToOrderColumns = true;
            this.dgvComponentList.AllowUserToResizeRows = false;
            this.dgvComponentList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvComponentList.Location = new System.Drawing.Point(12, 151);
            this.dgvComponentList.MultiSelect = false;
            this.dgvComponentList.Name = "dgvComponentList";
            this.dgvComponentList.ReadOnly = true;
            this.dgvComponentList.RowHeadersVisible = false;
            this.dgvComponentList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComponentList.Size = new System.Drawing.Size(570, 161);
            this.dgvComponentList.StandardTab = true;
            this.dgvComponentList.TabIndex = 6;
            this.dgvComponentList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvComponentList_KeyDown);
            this.dgvComponentList.DoubleClick += new System.EventHandler(this.btnEdit_Click);
            // 
            // ComponentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(189)))), ((int)(((byte)(140)))));
            this.ClientSize = new System.Drawing.Size(594, 352);
            this.Controls.Add(this.btnComponentListing);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvComponentList);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnEdit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(602, 298);
            this.Name = "ComponentView";
            this.Text = " View Components";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ComponentView_FormClosed);
            this.Load += new System.EventHandler(this.ComponentView_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponentList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.ComboBox cboStatusFilter;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtNameFilter;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboUnitFilter;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cboDataCompleteFilter;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cboInOutageFilter;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox cboSystemFilter;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox cboLineFilter;
		private DataGridViewStd dgvComponentList;
		private System.Windows.Forms.Button btnComponentListing;
        private System.Windows.Forms.ToolTip toolTip1;
	}
}