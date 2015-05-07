namespace Factotum
{
	partial class CustomerConfigView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerConfigView));
            this.tvwCustomerConfig = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ckShowInactive = new System.Windows.Forms.CheckBox();
            this.btnAddCustomer = new System.Windows.Forms.Button();
            this.btnAddSelected = new System.Windows.Forms.Button();
            this.btnEditSelected = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.btnToggleStatusSelected = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tvwCustomerConfig
            // 
            this.tvwCustomerConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwCustomerConfig.ImageIndex = 0;
            this.tvwCustomerConfig.ImageList = this.imageList1;
            this.tvwCustomerConfig.Location = new System.Drawing.Point(12, 12);
            this.tvwCustomerConfig.Name = "tvwCustomerConfig";
            this.tvwCustomerConfig.SelectedImageIndex = 0;
            this.tvwCustomerConfig.Size = new System.Drawing.Size(302, 224);
            this.tvwCustomerConfig.TabIndex = 0;
            this.tvwCustomerConfig.DoubleClick += new System.EventHandler(this.tvwCustomerConfig_DoubleClick);
            this.tvwCustomerConfig.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwCustomerConfig_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "CustomerActive.gif");
            this.imageList1.Images.SetKeyName(1, "SiteActive.gif");
            this.imageList1.Images.SetKeyName(2, "UnitActive.gif");
            this.imageList1.Images.SetKeyName(3, "SystemActive.gif");
            this.imageList1.Images.SetKeyName(4, "LineActive.gif");
            this.imageList1.Images.SetKeyName(5, "CustomerInActive.gif");
            this.imageList1.Images.SetKeyName(6, "SiteInActive.gif");
            this.imageList1.Images.SetKeyName(7, "UnitInactive.gif");
            this.imageList1.Images.SetKeyName(8, "SystemInActive.gif");
            this.imageList1.Images.SetKeyName(9, "LineInActive.gif");
            this.imageList1.Images.SetKeyName(10, "Outage.gif");
            // 
            // ckShowInactive
            // 
            this.ckShowInactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckShowInactive.AutoSize = true;
            this.ckShowInactive.Checked = true;
            this.ckShowInactive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckShowInactive.Location = new System.Drawing.Point(12, 242);
            this.ckShowInactive.Name = "ckShowInactive";
            this.ckShowInactive.Size = new System.Drawing.Size(94, 17);
            this.ckShowInactive.TabIndex = 8;
            this.ckShowInactive.TabStop = false;
            this.ckShowInactive.Text = "Show Inactive";
            this.ckShowInactive.UseVisualStyleBackColor = true;
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddCustomer.Location = new System.Drawing.Point(320, 12);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(127, 27);
            this.btnAddCustomer.TabIndex = 1;
            this.btnAddCustomer.Text = "Add Customer";
            this.btnAddCustomer.UseVisualStyleBackColor = true;
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // btnAddSelected
            // 
            this.btnAddSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSelected.Location = new System.Drawing.Point(320, 57);
            this.btnAddSelected.Name = "btnAddSelected";
            this.btnAddSelected.Size = new System.Drawing.Size(127, 27);
            this.btnAddSelected.TabIndex = 2;
            this.btnAddSelected.Text = "Add Selected";
            this.btnAddSelected.UseVisualStyleBackColor = true;
            this.btnAddSelected.Click += new System.EventHandler(this.btnAddSelected_Click);
            // 
            // btnEditSelected
            // 
            this.btnEditSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditSelected.Location = new System.Drawing.Point(320, 90);
            this.btnEditSelected.Name = "btnEditSelected";
            this.btnEditSelected.Size = new System.Drawing.Size(127, 27);
            this.btnEditSelected.TabIndex = 5;
            this.btnEditSelected.Text = "Edit Selected";
            this.btnEditSelected.UseVisualStyleBackColor = true;
            this.btnEditSelected.Click += new System.EventHandler(this.btnEditSelected_Click);
            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSelected.Location = new System.Drawing.Point(320, 123);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(127, 27);
            this.btnDeleteSelected.TabIndex = 6;
            this.btnDeleteSelected.Text = "Delete Selected";
            this.btnDeleteSelected.UseVisualStyleBackColor = true;
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            // 
            // btnToggleStatusSelected
            // 
            this.btnToggleStatusSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleStatusSelected.Location = new System.Drawing.Point(320, 210);
            this.btnToggleStatusSelected.Name = "btnToggleStatusSelected";
            this.btnToggleStatusSelected.Size = new System.Drawing.Size(127, 27);
            this.btnToggleStatusSelected.TabIndex = 7;
            this.btnToggleStatusSelected.Text = "Inactivate Selected";
            this.btnToggleStatusSelected.UseVisualStyleBackColor = true;
            this.btnToggleStatusSelected.Click += new System.EventHandler(this.btnToggleStatusSelected_Click);
            // 
            // CustomerConfigView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 271);
            this.Controls.Add(this.btnToggleStatusSelected);
            this.Controls.Add(this.btnDeleteSelected);
            this.Controls.Add(this.btnEditSelected);
            this.Controls.Add(this.btnAddSelected);
            this.Controls.Add(this.btnAddCustomer);
            this.Controls.Add(this.ckShowInactive);
            this.Controls.Add(this.tvwCustomerConfig);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(388, 305);
            this.Name = "CustomerConfigView";
            this.Text = "Customer Configuration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CustomerConfigView_FormClosed);
            this.Load += new System.EventHandler(this.CustomerConfigView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView tvwCustomerConfig;
		private System.Windows.Forms.CheckBox ckShowInactive;
		private System.Windows.Forms.Button btnAddCustomer;
		private System.Windows.Forms.Button btnAddSelected;
		private System.Windows.Forms.Button btnEditSelected;
		private System.Windows.Forms.Button btnDeleteSelected;
		private System.Windows.Forms.Button btnToggleStatusSelected;
		private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip1;
	}
}