namespace Factotum
{
	partial class PipeScheduleView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipeScheduleView));
			this.btnEdit = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.dgvPipeSchedulesList = new Factotum.DataGridViewStd();
			((System.ComponentModel.ISupportInitialize)(this.dgvPipeSchedulesList)).BeginInit();
			this.SuspendLayout();
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnEdit.Location = new System.Drawing.Point(11, 234);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(64, 22);
			this.btnEdit.TabIndex = 0;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Pipe Schedules List";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Location = new System.Drawing.Point(81, 234);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(64, 22);
			this.btnAdd.TabIndex = 1;
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
			this.btnDelete.TabIndex = 2;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// dgvPipeSchedulesList
			// 
			this.dgvPipeSchedulesList.AllowUserToAddRows = false;
			this.dgvPipeSchedulesList.AllowUserToDeleteRows = false;
			this.dgvPipeSchedulesList.AllowUserToOrderColumns = true;
			this.dgvPipeSchedulesList.AllowUserToResizeRows = false;
			this.dgvPipeSchedulesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvPipeSchedulesList.Location = new System.Drawing.Point(11, 35);
			this.dgvPipeSchedulesList.MultiSelect = false;
			this.dgvPipeSchedulesList.Name = "dgvPipeSchedulesList";
			this.dgvPipeSchedulesList.ReadOnly = true;
			this.dgvPipeSchedulesList.RowHeadersVisible = false;
			this.dgvPipeSchedulesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvPipeSchedulesList.Size = new System.Drawing.Size(425, 193);
			this.dgvPipeSchedulesList.StandardTab = true;
			this.dgvPipeSchedulesList.TabIndex = 4;
			this.dgvPipeSchedulesList.DoubleClick += new System.EventHandler(this.btnEdit_Click);
			this.dgvPipeSchedulesList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvPipeSchedulesList_KeyDown);
			// 
			// PipeScheduleView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(449, 268);
			this.Controls.Add(this.dgvPipeSchedulesList);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnEdit);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(363, 302);
			this.Name = "PipeScheduleView";
			this.Text = " View Pipe Schedule Info";
			this.Load += new System.EventHandler(this.PipeScheduleView_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PipeScheduleView_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvPipeSchedulesList)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private DataGridViewStd dgvPipeSchedulesList;
	}
}