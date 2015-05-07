namespace Factotum
{
	partial class InspectionEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectionEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblSiteName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.dgvDatasets = new Factotum.DataGridViewStd();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.btnAddDset = new System.Windows.Forms.Button();
			this.btnEditDset = new System.Windows.Forms.Button();
			this.btnDeleteDset = new System.Windows.Forms.Button();
			this.btnAddEditGraphic = new System.Windows.Forms.Button();
			this.btnAddEditGrid = new System.Windows.Forms.Button();
			this.btnDeleteGrid = new System.Windows.Forms.Button();
			this.btnDeleteGraphic = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.txtInspectorHours = new Factotum.TextBoxWithUndo();
			this.txtNotes = new Factotum.TextBoxWithUndo();
			this.txtName = new Factotum.TextBoxWithUndo();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvDatasets)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(343, 290);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 15;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(413, 290);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 16;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
			this.label3.Location = new System.Drawing.Point(9, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Notes";
			// 
			// dgvDatasets
			// 
			this.dgvDatasets.AllowUserToAddRows = false;
			this.dgvDatasets.AllowUserToDeleteRows = false;
			this.dgvDatasets.AllowUserToOrderColumns = true;
			this.dgvDatasets.AllowUserToResizeRows = false;
			this.dgvDatasets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvDatasets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDatasets.Location = new System.Drawing.Point(12, 145);
			this.dgvDatasets.MultiSelect = false;
			this.dgvDatasets.Name = "dgvDatasets";
			this.dgvDatasets.ReadOnly = true;
			this.dgvDatasets.RowHeadersVisible = false;
			this.dgvDatasets.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvDatasets.Size = new System.Drawing.Size(365, 106);
			this.dgvDatasets.StandardTab = true;
			this.dgvDatasets.TabIndex = 18;
			this.dgvDatasets.DoubleClick += new System.EventHandler(this.btnEditDset_Click);
			this.dgvDatasets.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvDatasets_KeyDown);
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(9, 131);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(49, 13);
			this.label13.TabIndex = 17;
			this.label13.Text = "Datasets";
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(334, 41);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(73, 13);
			this.label14.TabIndex = 2;
			this.label14.Text = "Inspector Hrs.";
			// 
			// btnAddDset
			// 
			this.btnAddDset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAddDset.Location = new System.Drawing.Point(82, 257);
			this.btnAddDset.Name = "btnAddDset";
			this.btnAddDset.Size = new System.Drawing.Size(64, 22);
			this.btnAddDset.TabIndex = 7;
			this.btnAddDset.Text = "Add";
			this.btnAddDset.UseVisualStyleBackColor = true;
			this.btnAddDset.Click += new System.EventHandler(this.btnAddDset_Click);
			// 
			// btnEditDset
			// 
			this.btnEditDset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnEditDset.Location = new System.Drawing.Point(12, 257);
			this.btnEditDset.Name = "btnEditDset";
			this.btnEditDset.Size = new System.Drawing.Size(64, 22);
			this.btnEditDset.TabIndex = 6;
			this.btnEditDset.Text = "Edit";
			this.btnEditDset.UseVisualStyleBackColor = true;
			this.btnEditDset.Click += new System.EventHandler(this.btnEditDset_Click);
			// 
			// btnDeleteDset
			// 
			this.btnDeleteDset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDeleteDset.Location = new System.Drawing.Point(152, 257);
			this.btnDeleteDset.Name = "btnDeleteDset";
			this.btnDeleteDset.Size = new System.Drawing.Size(64, 22);
			this.btnDeleteDset.TabIndex = 8;
			this.btnDeleteDset.Text = "Delete";
			this.btnDeleteDset.UseVisualStyleBackColor = true;
			this.btnDeleteDset.Click += new System.EventHandler(this.btnDeleteDset_Click);
			// 
			// btnAddEditGraphic
			// 
			this.btnAddEditGraphic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddEditGraphic.Location = new System.Drawing.Point(383, 143);
			this.btnAddEditGraphic.Name = "btnAddEditGraphic";
			this.btnAddEditGraphic.Size = new System.Drawing.Size(94, 22);
			this.btnAddEditGraphic.TabIndex = 11;
			this.btnAddEditGraphic.Text = "Add/Edit Graphic";
			this.btnAddEditGraphic.UseVisualStyleBackColor = true;
			this.btnAddEditGraphic.Click += new System.EventHandler(this.btnAddEditGraphic_Click);
			// 
			// btnAddEditGrid
			// 
			this.btnAddEditGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddEditGrid.Location = new System.Drawing.Point(383, 202);
			this.btnAddEditGrid.Name = "btnAddEditGrid";
			this.btnAddEditGrid.Size = new System.Drawing.Size(94, 22);
			this.btnAddEditGrid.TabIndex = 13;
			this.btnAddEditGrid.Text = "Add/Edit Grid";
			this.btnAddEditGrid.UseVisualStyleBackColor = true;
			this.btnAddEditGrid.Click += new System.EventHandler(this.btnAddEditGrid_Click);
			// 
			// btnDeleteGrid
			// 
			this.btnDeleteGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDeleteGrid.Location = new System.Drawing.Point(383, 230);
			this.btnDeleteGrid.Name = "btnDeleteGrid";
			this.btnDeleteGrid.Size = new System.Drawing.Size(94, 22);
			this.btnDeleteGrid.TabIndex = 14;
			this.btnDeleteGrid.Text = "Delete Grid";
			this.btnDeleteGrid.UseVisualStyleBackColor = true;
			this.btnDeleteGrid.Click += new System.EventHandler(this.btnDeleteGrid_Click);
			// 
			// btnDeleteGraphic
			// 
			this.btnDeleteGraphic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDeleteGraphic.Location = new System.Drawing.Point(383, 171);
			this.btnDeleteGraphic.Name = "btnDeleteGraphic";
			this.btnDeleteGraphic.Size = new System.Drawing.Size(94, 22);
			this.btnDeleteGraphic.TabIndex = 12;
			this.btnDeleteGraphic.Text = "Delete Graphic";
			this.btnDeleteGraphic.UseVisualStyleBackColor = true;
			this.btnDeleteGraphic.Click += new System.EventHandler(this.btnDeleteGraphic_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveDown.Location = new System.Drawing.Point(302, 257);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(75, 22);
			this.btnMoveDown.TabIndex = 10;
			this.btnMoveDown.Text = "Move Down";
			this.btnMoveDown.UseVisualStyleBackColor = true;
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveUp.Location = new System.Drawing.Point(221, 257);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(75, 22);
			this.btnMoveUp.TabIndex = 9;
			this.btnMoveUp.Text = "Move Up";
			this.btnMoveUp.UseVisualStyleBackColor = true;
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// txtInspectorHours
			// 
			this.txtInspectorHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtInspectorHours.Location = new System.Drawing.Point(412, 38);
			this.txtInspectorHours.Name = "txtInspectorHours";
			this.txtInspectorHours.Size = new System.Drawing.Size(65, 20);
			this.txtInspectorHours.TabIndex = 3;
			this.txtInspectorHours.Validating += new System.ComponentModel.CancelEventHandler(this.txtInspectorHours_Validating);
			// 
			// txtNotes
			// 
			this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.txtNotes.Location = new System.Drawing.Point(12, 81);
			this.txtNotes.Multiline = true;
			this.txtNotes.Name = "txtNotes";
			this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtNotes.Size = new System.Drawing.Size(465, 47);
			this.txtNotes.TabIndex = 5;
			this.txtNotes.TextChanged += new System.EventHandler(this.txtNotes_TextChanged);
			this.txtNotes.Validating += new System.ComponentModel.CancelEventHandler(this.txtNotes_Validating);
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(50, 38);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(147, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// InspectionEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(199)))), ((int)(((byte)(180)))));
			this.ClientSize = new System.Drawing.Size(489, 324);
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this.btnDeleteGraphic);
			this.Controls.Add(this.btnDeleteGrid);
			this.Controls.Add(this.btnAddEditGrid);
			this.Controls.Add(this.btnAddEditGraphic);
			this.Controls.Add(this.btnDeleteDset);
			this.Controls.Add(this.btnAddDset);
			this.Controls.Add(this.btnEditDset);
			this.Controls.Add(this.txtInspectorHours);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.dgvDatasets);
			this.Controls.Add(this.txtNotes);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(497, 340);
			this.Name = "InspectionEdit";
			this.Text = "Edit Inspection (Report Section)";
			this.Load += new System.EventHandler(this.InspectedComponentEdit_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InspectionEdit_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvDatasets)).EndInit();
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
		private TextBoxWithUndo txtNotes;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label13;
		private TextBoxWithUndo txtInspectorHours;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button btnDeleteDset;
		private System.Windows.Forms.Button btnAddDset;
		private System.Windows.Forms.Button btnEditDset;
		private System.Windows.Forms.Button btnDeleteGraphic;
		private System.Windows.Forms.Button btnDeleteGrid;
		private System.Windows.Forms.Button btnAddEditGrid;
		private System.Windows.Forms.Button btnAddEditGraphic;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnMoveUp;
		private DataGridViewStd dgvDatasets;
	}
}

