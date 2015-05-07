namespace Factotum
{
	partial class SiteEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiteEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.cboCustomer = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.ckActive = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtSiteFullName = new Factotum.TextBoxWithUndo();
			this.txtName = new Factotum.TextBoxWithUndo();
			this.label5 = new System.Windows.Forms.Label();
			this.cboDefaultCalProcedure = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(125, 156);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(54, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Site Name";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(195, 156);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// cboCustomer
			// 
			this.cboCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCustomer.FormattingEnabled = true;
			this.cboCustomer.Location = new System.Drawing.Point(111, 95);
			this.cboCustomer.Name = "cboCustomer";
			this.cboCustomer.Size = new System.Drawing.Size(147, 21);
			this.cboCustomer.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(54, 98);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Customer";
			// 
			// ckActive
			// 
			this.ckActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ckActive.AutoSize = true;
			this.ckActive.Location = new System.Drawing.Point(27, 156);
			this.ckActive.Name = "ckActive";
			this.ckActive.Size = new System.Drawing.Size(56, 17);
			this.ckActive.TabIndex = 10;
			this.ckActive.TabStop = false;
			this.ckActive.Text = "Active";
			this.ckActive.UseVisualStyleBackColor = true;
			this.ckActive.Click += new System.EventHandler(this.ckActive_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(56, 41);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Full Name";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(48, 54);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(62, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "For Reports";
			// 
			// txtSiteFullName
			// 
			this.txtSiteFullName.Location = new System.Drawing.Point(111, 39);
			this.txtSiteFullName.Multiline = true;
			this.txtSiteFullName.Name = "txtSiteFullName";
			this.txtSiteFullName.Size = new System.Drawing.Size(147, 50);
			this.txtSiteFullName.TabIndex = 3;
			this.txtSiteFullName.TextChanged += new System.EventHandler(this.txtSiteFullName_TextChanged);
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(111, 12);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(147, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 119);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(93, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Default Calibration";
			// 
			// cboDefaultCalProcedure
			// 
			this.cboDefaultCalProcedure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboDefaultCalProcedure.FormattingEnabled = true;
			this.cboDefaultCalProcedure.Location = new System.Drawing.Point(111, 122);
			this.cboDefaultCalProcedure.Name = "cboDefaultCalProcedure";
			this.cboDefaultCalProcedure.Size = new System.Drawing.Size(147, 21);
			this.cboDefaultCalProcedure.TabIndex = 7;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(49, 132);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "Procedure";
			// 
			// SiteEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(271, 190);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cboDefaultCalProcedure);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtSiteFullName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.ckActive);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cboCustomer);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(279, 224);
			this.Name = "SiteEdit";
			this.Text = "Edit Site";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox ckActive;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cboCustomer;
		private TextBoxWithUndo txtName;
		private System.Windows.Forms.Label label4;
		private TextBoxWithUndo txtSiteFullName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cboDefaultCalProcedure;
	}
}

