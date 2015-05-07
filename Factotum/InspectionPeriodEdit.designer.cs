namespace Factotum
{
	partial class InspectionPeriodEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InspectionPeriodEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.dtpCalIn = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.dtpCalOut = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.dtpCalCheck1 = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.dtpCalCheck2 = new System.Windows.Forms.DateTimePicker();
			this.ckCheck1 = new System.Windows.Forms.CheckBox();
			this.ckCheck2 = new System.Windows.Forms.CheckBox();
			this.lblSiteName = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(228, 168);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(298, 168);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// dtpCalIn
			// 
			this.dtpCalIn.CustomFormat = "MM-dd-yyyy H:mm";
			this.dtpCalIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dtpCalIn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpCalIn.Location = new System.Drawing.Point(118, 44);
			this.dtpCalIn.Name = "dtpCalIn";
			this.dtpCalIn.Size = new System.Drawing.Size(134, 22);
			this.dtpCalIn.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(44, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Calibration In";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(36, 130);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Calibration Out";
			// 
			// dtpCalOut
			// 
			this.dtpCalOut.CustomFormat = "MM-dd-yyyy H:mm";
			this.dtpCalOut.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
			this.dtpCalOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dtpCalOut.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpCalOut.Location = new System.Drawing.Point(118, 126);
			this.dtpCalOut.Name = "dtpCalOut";
			this.dtpCalOut.Size = new System.Drawing.Size(134, 22);
			this.dtpCalOut.TabIndex = 11;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Calibration Check 1";
			// 
			// dtpCalCheck1
			// 
			this.dtpCalCheck1.CustomFormat = "MM-dd-yyyy H:mm";
			this.dtpCalCheck1.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
			this.dtpCalCheck1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dtpCalCheck1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpCalCheck1.Location = new System.Drawing.Point(118, 72);
			this.dtpCalCheck1.Name = "dtpCalCheck1";
			this.dtpCalCheck1.Size = new System.Drawing.Size(134, 22);
			this.dtpCalCheck1.TabIndex = 13;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 102);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Calibration Check 2";
			// 
			// dtpCalCheck2
			// 
			this.dtpCalCheck2.CustomFormat = "MM-dd-yyyy H:mm";
			this.dtpCalCheck2.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
			this.dtpCalCheck2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dtpCalCheck2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpCalCheck2.Location = new System.Drawing.Point(118, 98);
			this.dtpCalCheck2.Name = "dtpCalCheck2";
			this.dtpCalCheck2.Size = new System.Drawing.Size(134, 22);
			this.dtpCalCheck2.TabIndex = 15;
			// 
			// ckCheck1
			// 
			this.ckCheck1.AutoSize = true;
			this.ckCheck1.Location = new System.Drawing.Point(307, 77);
			this.ckCheck1.Name = "ckCheck1";
			this.ckCheck1.Size = new System.Drawing.Size(57, 17);
			this.ckCheck1.TabIndex = 16;
			this.ckCheck1.Text = "Check";
			this.ckCheck1.UseVisualStyleBackColor = true;
			this.ckCheck1.CheckedChanged += new System.EventHandler(this.ckCheck1_CheckedChanged);
			// 
			// ckCheck2
			// 
			this.ckCheck2.AutoSize = true;
			this.ckCheck2.Location = new System.Drawing.Point(307, 103);
			this.ckCheck2.Name = "ckCheck2";
			this.ckCheck2.Size = new System.Drawing.Size(57, 17);
			this.ckCheck2.TabIndex = 17;
			this.ckCheck2.Text = "Check";
			this.ckCheck2.UseVisualStyleBackColor = true;
			this.ckCheck2.CheckedChanged += new System.EventHandler(this.ckCheck2_CheckedChanged);
			// 
			// lblSiteName
			// 
			this.lblSiteName.AutoSize = true;
			this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
			this.lblSiteName.Location = new System.Drawing.Point(115, 9);
			this.lblSiteName.Name = "lblSiteName";
			this.lblSiteName.Size = new System.Drawing.Size(80, 16);
			this.lblSiteName.TabIndex = 18;
			this.lblSiteName.Text = "Component:";
			// 
			// InspectionPeriodEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(374, 202);
			this.Controls.Add(this.lblSiteName);
			this.Controls.Add(this.ckCheck2);
			this.Controls.Add(this.ckCheck1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.dtpCalCheck2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.dtpCalCheck1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dtpCalOut);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.dtpCalIn);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(382, 236);
			this.Name = "InspectionPeriodEdit";
			this.Text = "Edit Inspection Period";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker dtpCalIn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DateTimePicker dtpCalCheck2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dtpCalCheck1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtpCalOut;
		private System.Windows.Forms.CheckBox ckCheck1;
		private System.Windows.Forms.CheckBox ckCheck2;
		private System.Windows.Forms.Label lblSiteName;
	}
}

