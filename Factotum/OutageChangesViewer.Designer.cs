namespace Factotum
{
	partial class OutageChangesViewer
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabChanges = new System.Windows.Forms.TabPage();
			this.dgvChanges = new System.Windows.Forms.DataGridView();
			this.tabReassignments = new System.Windows.Forms.TabPage();
			this.dgvReassignments = new System.Windows.Forms.DataGridView();
			this.tabAdditions = new System.Windows.Forms.TabPage();
			this.dgvAdditions = new System.Windows.Forms.DataGridView();
			this.btnPreview = new System.Windows.Forms.Button();
			this.btnPrint = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabChanges.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvChanges)).BeginInit();
			this.tabReassignments.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvReassignments)).BeginInit();
			this.tabAdditions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAdditions)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabChanges);
			this.tabControl1.Controls.Add(this.tabReassignments);
			this.tabControl1.Controls.Add(this.tabAdditions);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(546, 296);
			this.tabControl1.TabIndex = 0;
			// 
			// tabChanges
			// 
			this.tabChanges.Controls.Add(this.dgvChanges);
			this.tabChanges.Location = new System.Drawing.Point(4, 22);
			this.tabChanges.Name = "tabChanges";
			this.tabChanges.Padding = new System.Windows.Forms.Padding(3);
			this.tabChanges.Size = new System.Drawing.Size(538, 270);
			this.tabChanges.TabIndex = 0;
			this.tabChanges.Text = "Changes";
			this.tabChanges.UseVisualStyleBackColor = true;
			// 
			// dgvChanges
			// 
			this.dgvChanges.AllowUserToAddRows = false;
			this.dgvChanges.AllowUserToDeleteRows = false;
			this.dgvChanges.AllowUserToOrderColumns = true;
			this.dgvChanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvChanges.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvChanges.Location = new System.Drawing.Point(3, 3);
			this.dgvChanges.Name = "dgvChanges";
			this.dgvChanges.ReadOnly = true;
			this.dgvChanges.Size = new System.Drawing.Size(532, 264);
			this.dgvChanges.TabIndex = 0;
			// 
			// tabReassignments
			// 
			this.tabReassignments.Controls.Add(this.dgvReassignments);
			this.tabReassignments.Location = new System.Drawing.Point(4, 22);
			this.tabReassignments.Name = "tabReassignments";
			this.tabReassignments.Padding = new System.Windows.Forms.Padding(3);
			this.tabReassignments.Size = new System.Drawing.Size(538, 270);
			this.tabReassignments.TabIndex = 1;
			this.tabReassignments.Text = "Reassignments";
			this.tabReassignments.UseVisualStyleBackColor = true;
			// 
			// dgvReassignments
			// 
			this.dgvReassignments.AllowUserToAddRows = false;
			this.dgvReassignments.AllowUserToDeleteRows = false;
			this.dgvReassignments.AllowUserToOrderColumns = true;
			this.dgvReassignments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvReassignments.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvReassignments.Location = new System.Drawing.Point(3, 3);
			this.dgvReassignments.Name = "dgvReassignments";
			this.dgvReassignments.ReadOnly = true;
			this.dgvReassignments.Size = new System.Drawing.Size(532, 264);
			this.dgvReassignments.TabIndex = 1;
			// 
			// tabAdditions
			// 
			this.tabAdditions.Controls.Add(this.dgvAdditions);
			this.tabAdditions.Location = new System.Drawing.Point(4, 22);
			this.tabAdditions.Name = "tabAdditions";
			this.tabAdditions.Size = new System.Drawing.Size(538, 270);
			this.tabAdditions.TabIndex = 2;
			this.tabAdditions.Text = "Additions";
			this.tabAdditions.UseVisualStyleBackColor = true;
			// 
			// dgvAdditions
			// 
			this.dgvAdditions.AllowUserToAddRows = false;
			this.dgvAdditions.AllowUserToDeleteRows = false;
			this.dgvAdditions.AllowUserToOrderColumns = true;
			this.dgvAdditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAdditions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAdditions.Location = new System.Drawing.Point(0, 0);
			this.dgvAdditions.Name = "dgvAdditions";
			this.dgvAdditions.ReadOnly = true;
			this.dgvAdditions.Size = new System.Drawing.Size(538, 270);
			this.dgvAdditions.TabIndex = 1;
			// 
			// btnPreview
			// 
			this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPreview.Location = new System.Drawing.Point(15, 314);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(75, 23);
			this.btnPreview.TabIndex = 1;
			this.btnPreview.Text = "Preview";
			this.btnPreview.UseVisualStyleBackColor = true;
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			// 
			// btnPrint
			// 
			this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPrint.Location = new System.Drawing.Point(96, 314);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(75, 23);
			this.btnPrint.TabIndex = 2;
			this.btnPrint.Text = "Print";
			this.btnPrint.UseVisualStyleBackColor = true;
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Location = new System.Drawing.Point(483, 314);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// OutageChangesViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(570, 341);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnPrint);
			this.Controls.Add(this.btnPreview);
			this.Controls.Add(this.tabControl1);
			this.Name = "OutageChangesViewer";
			this.Text = "Outage Changes Viewer";
			this.Load += new System.EventHandler(this.OutageChangesViewer_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabChanges.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvChanges)).EndInit();
			this.tabReassignments.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvReassignments)).EndInit();
			this.tabAdditions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAdditions)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabChanges;
		private System.Windows.Forms.TabPage tabReassignments;
		private System.Windows.Forms.TabPage tabAdditions;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.Button btnPrint;
		private System.Windows.Forms.DataGridView dgvChanges;
		private System.Windows.Forms.DataGridView dgvReassignments;
		private System.Windows.Forms.DataGridView dgvAdditions;
		private System.Windows.Forms.Button btnClose;
	}
}