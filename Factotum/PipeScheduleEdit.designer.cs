namespace Factotum
{
	partial class PipeScheduleEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipeScheduleEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtNomDia = new Factotum.TextBoxWithUndo();
			this.txtPipeSchedule = new Factotum.TextBoxWithUndo();
			this.txtNomWall = new Factotum.TextBoxWithUndo();
			this.txtOD = new Factotum.TextBoxWithUndo();
			this.label5 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(134, 133);
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
			this.label1.Location = new System.Drawing.Point(41, 68);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Outside Diameter";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(204, 133);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 94);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Nominal Wall Thickness";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(53, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Pipe Schedule";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(39, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Nominal Diameter";
			// 
			// txtNomDia
			// 
			this.txtNomDia.Location = new System.Drawing.Point(135, 39);
			this.txtNomDia.Name = "txtNomDia";
			this.txtNomDia.Size = new System.Drawing.Size(100, 20);
			this.txtNomDia.TabIndex = 3;
			this.txtNomDia.TextChanged += new System.EventHandler(this.txtNomDia_TextChanged);
			this.txtNomDia.Validating += new System.ComponentModel.CancelEventHandler(this.txtNomDia_Validating);
			// 
			// txtPipeSchedule
			// 
			this.txtPipeSchedule.Location = new System.Drawing.Point(135, 13);
			this.txtPipeSchedule.Name = "txtPipeSchedule";
			this.txtPipeSchedule.Size = new System.Drawing.Size(100, 20);
			this.txtPipeSchedule.TabIndex = 1;
			this.txtPipeSchedule.TextChanged += new System.EventHandler(this.txtPipeSchedule_TextChanged);
			this.txtPipeSchedule.Validating += new System.ComponentModel.CancelEventHandler(this.txtPipeSchedule_Validating);
			// 
			// txtNomWall
			// 
			this.txtNomWall.Location = new System.Drawing.Point(135, 91);
			this.txtNomWall.Name = "txtNomWall";
			this.txtNomWall.Size = new System.Drawing.Size(100, 20);
			this.txtNomWall.TabIndex = 7;
			this.txtNomWall.TextChanged += new System.EventHandler(this.txtNomWall_TextChanged);
			this.txtNomWall.Validating += new System.ComponentModel.CancelEventHandler(this.txtNomWall_Validating);
			// 
			// txtOD
			// 
			this.txtOD.Location = new System.Drawing.Point(135, 65);
			this.txtOD.Name = "txtOD";
			this.txtOD.Size = new System.Drawing.Size(100, 20);
			this.txtOD.TabIndex = 5;
			this.txtOD.TextChanged += new System.EventHandler(this.txtOD_TextChanged);
			this.txtOD.Validating += new System.ComponentModel.CancelEventHandler(this.txtOD_Validating);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(254, 42);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(18, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "in.";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(254, 94);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(18, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "in.";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(254, 68);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(18, 13);
			this.label8.TabIndex = 12;
			this.label8.Text = "in.";
			// 
			// PipeScheduleEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(293, 167);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtNomDia);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtPipeSchedule);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtNomWall);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtOD);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(284, 201);
			this.Name = "PipeScheduleEdit";
			this.Text = "Edit Pipe Schedule Item";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnCancel;
		private TextBoxWithUndo txtOD;
		private TextBoxWithUndo txtNomDia;
		private System.Windows.Forms.Label label4;
		private TextBoxWithUndo txtPipeSchedule;
		private System.Windows.Forms.Label label3;
		private TextBoxWithUndo txtNomWall;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
	}
}

