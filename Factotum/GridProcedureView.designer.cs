namespace Factotum
{
	partial class GridProcedureView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridProcedureView));
			this.btnEdit = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.cboStatusFilter = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.dgvGridProcedureList = new Factotum.DataGridViewStd();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cboInOutageFilter = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvGridProcedureList)).BeginInit();
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
			this.label1.Location = new System.Drawing.Point(15, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Grid Procedures List";
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
			this.cboStatusFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboStatusFilter.FormattingEnabled = true;
			this.cboStatusFilter.Items.AddRange(new object[] {
            "Active",
            "Inactive"});
			this.cboStatusFilter.Location = new System.Drawing.Point(235, 5);
			this.cboStatusFilter.Name = "cboStatusFilter";
			this.cboStatusFilter.Size = new System.Drawing.Size(85, 21);
			this.cboStatusFilter.TabIndex = 3;
			this.cboStatusFilter.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(192, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Status";
			// 
			// dgvGridProcedureList
			// 
			this.dgvGridProcedureList.AllowUserToAddRows = false;
			this.dgvGridProcedureList.AllowUserToDeleteRows = false;
			this.dgvGridProcedureList.AllowUserToOrderColumns = true;
			this.dgvGridProcedureList.AllowUserToResizeRows = false;
			this.dgvGridProcedureList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvGridProcedureList.Location = new System.Drawing.Point(15, 86);
			this.dgvGridProcedureList.MultiSelect = false;
			this.dgvGridProcedureList.Name = "dgvGridProcedureList";
			this.dgvGridProcedureList.ReadOnly = true;
			this.dgvGridProcedureList.RowHeadersVisible = false;
			this.dgvGridProcedureList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvGridProcedureList.Size = new System.Drawing.Size(328, 128);
			this.dgvGridProcedureList.StandardTab = true;
			this.dgvGridProcedureList.TabIndex = 6;
			this.dgvGridProcedureList.DoubleClick += new System.EventHandler(this.btnEdit_Click);
			this.dgvGridProcedureList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvGridProcedureList_KeyDown);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.cboInOutageFilter);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.cboStatusFilter);
			this.panel1.Controls.Add(this.label2);
			this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.panel1.Location = new System.Drawing.Point(15, 18);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(327, 34);
			this.panel1.TabIndex = 1;
			// 
			// cboInOutageFilter
			// 
			this.cboInOutageFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboInOutageFilter.FormattingEnabled = true;
			this.cboInOutageFilter.Items.AddRange(new object[] {
            "All",
            "Yes"});
			this.cboInOutageFilter.Location = new System.Drawing.Point(65, 5);
			this.cboInOutageFilter.Name = "cboInOutageFilter";
			this.cboInOutageFilter.Size = new System.Drawing.Size(85, 21);
			this.cboInOutageFilter.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(5, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(54, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "In Outage";
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
			// GridProcedureView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 268);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.dgvGridProcedureList);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnEdit);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(363, 302);
			this.Name = "GridProcedureView";
			this.Text = " View Grid Procedures";
			this.Load += new System.EventHandler(this.GridProcedureView_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GridProcedureView_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvGridProcedureList)).EndInit();
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
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cboInOutageFilter;
		private System.Windows.Forms.Label label6;
		private DataGridViewStd dgvGridProcedureList;
	}
}