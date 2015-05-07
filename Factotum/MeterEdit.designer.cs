namespace Factotum
{
	partial class MeterEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeterEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.ckActive = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtName = new Factotum.TextBoxWithUndo();
			this.cboModel = new System.Windows.Forms.ComboBox();
			this.cboKit = new System.Windows.Forms.ComboBox();
			this.dtpCalibrationDueOn = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(163, 128);
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
			this.label1.Location = new System.Drawing.Point(17, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Serial Number";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(233, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// ckActive
			// 
			this.ckActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ckActive.AutoSize = true;
			this.ckActive.Location = new System.Drawing.Point(27, 128);
			this.ckActive.Name = "ckActive";
			this.ckActive.Size = new System.Drawing.Size(56, 17);
			this.ckActive.TabIndex = 10;
			this.ckActive.TabStop = false;
			this.ckActive.Text = "Active";
			this.ckActive.UseVisualStyleBackColor = true;
			this.ckActive.Click += new System.EventHandler(this.ckActive_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(54, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(36, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Model";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(71, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(19, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Kit";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(96, 12);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(113, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// cboModel
			// 
			this.cboModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboModel.FormattingEnabled = true;
			this.cboModel.Location = new System.Drawing.Point(96, 38);
			this.cboModel.Name = "cboModel";
			this.cboModel.Size = new System.Drawing.Size(121, 21);
			this.cboModel.TabIndex = 3;
			// 
			// cboKit
			// 
			this.cboKit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboKit.FormattingEnabled = true;
			this.cboKit.Location = new System.Drawing.Point(96, 65);
			this.cboKit.Name = "cboKit";
			this.cboKit.Size = new System.Drawing.Size(121, 21);
			this.cboKit.TabIndex = 5;
			// 
			// dtpCalibrationDueOn
			// 
			this.dtpCalibrationDueOn.Location = new System.Drawing.Point(96, 92);
			this.dtpCalibrationDueOn.Name = "dtpCalibrationDueOn";
			this.dtpCalibrationDueOn.Size = new System.Drawing.Size(200, 20);
			this.dtpCalibrationDueOn.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Calibration Due";
			// 
			// MeterEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(309, 162);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.dtpCalibrationDueOn);
			this.Controls.Add(this.cboKit);
			this.Controls.Add(this.cboModel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.ckActive);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(317, 187);
			this.Name = "MeterEdit";
			this.Text = "Edit Meter";
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
		private TextBoxWithUndo txtName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cboKit;
		private System.Windows.Forms.ComboBox cboModel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker dtpCalibrationDueOn;
	}
}

