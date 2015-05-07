namespace Factotum
{
	partial class GridSizeEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridSizeEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.ckActive = new System.Windows.Forms.CheckBox();
			this.txtName = new Factotum.TextBoxWithUndo();
			this.txtAxialDistance = new Factotum.TextBoxWithUndo();
			this.label3 = new System.Windows.Forms.Label();
			this.txtRadialDistance = new Factotum.TextBoxWithUndo();
			this.label2 = new System.Windows.Forms.Label();
			this.txtMaxOD = new Factotum.TextBoxWithUndo();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(144, 143);
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
			this.label1.Location = new System.Drawing.Point(16, 15);
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
			this.btnCancel.Location = new System.Drawing.Point(214, 143);
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
			this.ckActive.Location = new System.Drawing.Point(27, 143);
			this.ckActive.Name = "ckActive";
			this.ckActive.Size = new System.Drawing.Size(56, 17);
			this.ckActive.TabIndex = 10;
			this.ckActive.TabStop = false;
			this.ckActive.Text = "Active";
			this.ckActive.UseVisualStyleBackColor = true;
			this.ckActive.Click += new System.EventHandler(this.ckActive_Click);
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(57, 12);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(147, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// txtAxialDistance
			// 
			this.txtAxialDistance.Location = new System.Drawing.Point(138, 38);
			this.txtAxialDistance.Name = "txtAxialDistance";
			this.txtAxialDistance.Size = new System.Drawing.Size(66, 20);
			this.txtAxialDistance.TabIndex = 3;
			this.txtAxialDistance.TextChanged += new System.EventHandler(this.txtAxialDistance_TextChanged);
			this.txtAxialDistance.Validating += new System.ComponentModel.CancelEventHandler(this.txtAxialDistance_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(61, 41);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "AxialDistance";
			// 
			// txtRadialDistance
			// 
			this.txtRadialDistance.Location = new System.Drawing.Point(138, 64);
			this.txtRadialDistance.Name = "txtRadialDistance";
			this.txtRadialDistance.Size = new System.Drawing.Size(66, 20);
			this.txtRadialDistance.TabIndex = 5;
			this.txtRadialDistance.TextChanged += new System.EventHandler(this.txtRadialDistance_TextChanged);
			this.txtRadialDistance.Validating += new System.ComponentModel.CancelEventHandler(this.txtRadialDistance_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(50, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Radial Distance";
			// 
			// txtMaxOD
			// 
			this.txtMaxOD.Location = new System.Drawing.Point(138, 90);
			this.txtMaxOD.Name = "txtMaxOD";
			this.txtMaxOD.Size = new System.Drawing.Size(66, 20);
			this.txtMaxOD.TabIndex = 7;
			this.txtMaxOD.TextChanged += new System.EventHandler(this.txtMaxOD_TextChanged);
			this.txtMaxOD.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaxOD_Validating);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(62, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Max Pipe OD";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(220, 93);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(18, 13);
			this.label5.TabIndex = 13;
			this.label5.Text = "in.";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(220, 67);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(18, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "in.";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(220, 41);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(18, 13);
			this.label7.TabIndex = 11;
			this.label7.Text = "in.";
			// 
			// GridSizeEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(290, 177);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.txtMaxOD);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtRadialDistance);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtAxialDistance);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.ckActive);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(298, 207);
			this.Name = "GridSizeEdit";
			this.Text = "Edit Grid Size";
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
		private TextBoxWithUndo txtAxialDistance;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private TextBoxWithUndo txtMaxOD;
		private System.Windows.Forms.Label label4;
		private TextBoxWithUndo txtRadialDistance;
		private System.Windows.Forms.Label label2;
	}
}

