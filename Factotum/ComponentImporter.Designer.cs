namespace Factotum
{
	partial class ComponentImporter
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentImporter));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnSelectAndPreview = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.lblSiteName = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.tvwComponentChange = new System.Windows.Forms.TreeView();
			this.tvwItemAdd = new System.Windows.Forms.TreeView();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 369);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(490, 22);
			this.statusStrip1.TabIndex = 0;
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
			// btnSelectAndPreview
			// 
			this.btnSelectAndPreview.Location = new System.Drawing.Point(23, 331);
			this.btnSelectAndPreview.Name = "btnSelectAndPreview";
			this.btnSelectAndPreview.Size = new System.Drawing.Size(247, 23);
			this.btnSelectAndPreview.TabIndex = 2;
			this.btnSelectAndPreview.Text = "Select File For Import and Preview Changes";
			this.btnSelectAndPreview.UseVisualStyleBackColor = true;
			this.btnSelectAndPreview.Click += new System.EventHandler(this.btnSelectAndPreview_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(303, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Items To Be Added";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Components To Be Changed";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(381, 331);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(97, 23);
			this.btnCancel.TabIndex = 15;
			this.btnCancel.Text = "Cancel Import";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnImport
			// 
			this.btnImport.Location = new System.Drawing.Point(276, 331);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(99, 23);
			this.btnImport.TabIndex = 16;
			this.btnImport.Text = "Perform Import";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// lblSiteName
			// 
			this.lblSiteName.AutoSize = true;
			this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
			this.lblSiteName.Location = new System.Drawing.Point(215, 7);
			this.lblSiteName.Name = "lblSiteName";
			this.lblSiteName.Size = new System.Drawing.Size(53, 16);
			this.lblSiteName.TabIndex = 17;
			this.lblSiteName.Text = "Facility:";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// tvwComponentChange
			// 
			this.tvwComponentChange.CheckBoxes = true;
			this.tvwComponentChange.Location = new System.Drawing.Point(23, 55);
			this.tvwComponentChange.Name = "tvwComponentChange";
			this.tvwComponentChange.Size = new System.Drawing.Size(266, 270);
			this.tvwComponentChange.TabIndex = 18;
			this.tvwComponentChange.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwComponentChange_AfterCheck);
			// 
			// tvwItemAdd
			// 
			this.tvwItemAdd.Location = new System.Drawing.Point(306, 55);
			this.tvwItemAdd.Name = "tvwItemAdd";
			this.tvwItemAdd.Size = new System.Drawing.Size(172, 270);
			this.tvwItemAdd.TabIndex = 19;
			// 
			// ComponentImporter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(490, 391);
			this.Controls.Add(this.tvwItemAdd);
			this.Controls.Add(this.tvwComponentChange);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSelectAndPreview);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ComponentImporter";
			this.Text = "Component Importer";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Button btnSelectAndPreview;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.Label lblSiteName;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TreeView tvwComponentChange;
		private System.Windows.Forms.TreeView tvwItemAdd;
	}
}