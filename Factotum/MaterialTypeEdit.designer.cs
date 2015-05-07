namespace Factotum
{
	partial class MaterialTypeEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialTypeEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.cboCalBlockMatlType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.ckActive = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lblSiteName = new System.Windows.Forms.Label();
			this.txtName = new Factotum.TextBoxWithUndo();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(144, 139);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(37, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Material Type";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(214, 139);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// cboCalBlockMatlType
			// 
			this.cboCalBlockMatlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboCalBlockMatlType.FormattingEnabled = true;
			this.cboCalBlockMatlType.Location = new System.Drawing.Point(114, 73);
			this.cboCalBlockMatlType.Name = "cboCalBlockMatlType";
			this.cboCalBlockMatlType.Size = new System.Drawing.Size(147, 21);
			this.cboCalBlockMatlType.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 73);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Calibration Block";
			// 
			// ckActive
			// 
			this.ckActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ckActive.AutoSize = true;
			this.ckActive.Location = new System.Drawing.Point(27, 139);
			this.ckActive.Name = "ckActive";
			this.ckActive.Size = new System.Drawing.Size(56, 17);
			this.ckActive.TabIndex = 6;
			this.ckActive.TabStop = false;
			this.ckActive.Text = "Active";
			this.ckActive.UseVisualStyleBackColor = true;
			this.ckActive.Click += new System.EventHandler(this.ckActive_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(37, 86);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Material Type";
			// 
			// lblSiteName
			// 
			this.lblSiteName.AutoSize = true;
			this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
			this.lblSiteName.Location = new System.Drawing.Point(61, 9);
			this.lblSiteName.Name = "lblSiteName";
			this.lblSiteName.Size = new System.Drawing.Size(34, 16);
			this.lblSiteName.TabIndex = 11;
			this.lblSiteName.Text = "Site:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(114, 47);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(147, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// MaterialTypeEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(290, 173);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.ckActive);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cboCalBlockMatlType);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(298, 207);
			this.Name = "MaterialTypeEdit";
			this.Text = "Edit Component Material Type";
			this.Resize += new System.EventHandler(this.MaterialTypeEdit_Resize);
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
		private System.Windows.Forms.ComboBox cboCalBlockMatlType;
		private TextBoxWithUndo txtName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblSiteName;
	}
}

