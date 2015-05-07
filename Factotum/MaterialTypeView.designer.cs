namespace Factotum
{
	partial class MaterialTypeView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialTypeView));
			this.btnEdit = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.cboStatusFilter = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.dgvMaterialTypeList = new Factotum.DataGridViewStd();
			this.label3 = new System.Windows.Forms.Label();
			this.txtNameFilter = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cboSiteFilter = new System.Windows.Forms.ComboBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvMaterialTypeList)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnEdit.Location = new System.Drawing.Point(11, 234);
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
			this.label1.Location = new System.Drawing.Point(12, 105);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Material Types List";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Location = new System.Drawing.Point(81, 234);
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
			this.btnDelete.Location = new System.Drawing.Point(151, 234);
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
			this.cboStatusFilter.Location = new System.Drawing.Point(228, 40);
			this.cboStatusFilter.Name = "cboStatusFilter";
			this.cboStatusFilter.Size = new System.Drawing.Size(85, 21);
			this.cboStatusFilter.TabIndex = 5;
			this.cboStatusFilter.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(185, 43);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Status";
			// 
			// dgvMaterialTypeList
			// 
			this.dgvMaterialTypeList.AllowUserToAddRows = false;
			this.dgvMaterialTypeList.AllowUserToDeleteRows = false;
			this.dgvMaterialTypeList.AllowUserToOrderColumns = true;
			this.dgvMaterialTypeList.AllowUserToResizeRows = false;
			this.dgvMaterialTypeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvMaterialTypeList.Location = new System.Drawing.Point(12, 121);
			this.dgvMaterialTypeList.MultiSelect = false;
			this.dgvMaterialTypeList.Name = "dgvMaterialTypeList";
			this.dgvMaterialTypeList.ReadOnly = true;
			this.dgvMaterialTypeList.RowHeadersVisible = false;
			this.dgvMaterialTypeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvMaterialTypeList.Size = new System.Drawing.Size(331, 107);
			this.dgvMaterialTypeList.StandardTab = true;
			this.dgvMaterialTypeList.TabIndex = 6;
			this.dgvMaterialTypeList.DoubleClick += new System.EventHandler(this.btnEdit_Click);
			this.dgvMaterialTypeList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvMaterialTypeList_KeyDown);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Material Type";
			// 
			// txtNameFilter
			// 
			this.txtNameFilter.Location = new System.Drawing.Point(84, 41);
			this.txtNameFilter.Name = "txtNameFilter";
			this.txtNameFilter.Size = new System.Drawing.Size(81, 20);
			this.txtNameFilter.TabIndex = 3;
			this.txtNameFilter.TextChanged += new System.EventHandler(this.txtNameFilter_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(25, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Site";
			// 
			// cboSiteFilter
			// 
			this.cboSiteFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboSiteFilter.FormattingEnabled = true;
			this.cboSiteFilter.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
			this.cboSiteFilter.Location = new System.Drawing.Point(38, 14);
			this.cboSiteFilter.Name = "cboSiteFilter";
			this.cboSiteFilter.Size = new System.Drawing.Size(127, 21);
			this.cboSiteFilter.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.txtNameFilter);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.cboStatusFilter);
			this.panel1.Controls.Add(this.cboSiteFilter);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label3);
			this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.panel1.Location = new System.Drawing.Point(15, 18);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(324, 75);
			this.panel1.TabIndex = 1;
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
			// MaterialTypeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 268);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.dgvMaterialTypeList);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnEdit);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(363, 302);
			this.Name = "MaterialTypeView";
			this.Text = " View Component Material Types";
			this.Load += new System.EventHandler(this.MaterialTypeView_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MaterialTypeView_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvMaterialTypeList)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
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
		private System.Windows.Forms.ComboBox cboSiteFilter;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label5;
		private DataGridViewStd dgvMaterialTypeList;
	}
}