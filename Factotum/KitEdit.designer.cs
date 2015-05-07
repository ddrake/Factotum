namespace Factotum
{
	partial class KitEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KitEdit));
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.TabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.clbInspectors = new System.Windows.Forms.CheckedListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.clbMeters = new System.Windows.Forms.CheckedListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.clbTransducers = new System.Windows.Forms.CheckedListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.clbCalBlocks = new System.Windows.Forms.CheckedListBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.clbThermos = new System.Windows.Forms.CheckedListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.txtName = new Factotum.TextBoxWithUndo();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.TabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(206, 203);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 22);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Kit Name";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(276, 203);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 22);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// TabControl1
			// 
			this.TabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.TabControl1.Controls.Add(this.tabPage1);
			this.TabControl1.Controls.Add(this.tabPage2);
			this.TabControl1.Controls.Add(this.tabPage3);
			this.TabControl1.Controls.Add(this.tabPage4);
			this.TabControl1.Controls.Add(this.tabPage5);
			this.TabControl1.Location = new System.Drawing.Point(17, 37);
			this.TabControl1.Name = "TabControl1";
			this.TabControl1.SelectedIndex = 0;
			this.TabControl1.Size = new System.Drawing.Size(323, 156);
			this.TabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.clbInspectors);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(315, 130);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Inspector";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// clbInspectors
			// 
			this.clbInspectors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.clbInspectors.FormattingEnabled = true;
			this.clbInspectors.Location = new System.Drawing.Point(11, 11);
			this.clbInspectors.Name = "clbInspectors";
			this.clbInspectors.Size = new System.Drawing.Size(292, 109);
			this.clbInspectors.TabIndex = 0;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(52, 67);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(141, 13);
			this.label7.TabIndex = 2;
			this.label7.Text = "currently have kits assigned.";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(52, 54);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(203, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "All active Inspectors in the current outage";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.clbMeters);
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.label8);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(315, 130);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Meter";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// clbMeters
			// 
			this.clbMeters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.clbMeters.FormattingEnabled = true;
			this.clbMeters.Location = new System.Drawing.Point(11, 11);
			this.clbMeters.Name = "clbMeters";
			this.clbMeters.Size = new System.Drawing.Size(292, 109);
			this.clbMeters.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(72, 65);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(171, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "are currently assigned to other kits.";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(72, 52);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(85, 13);
			this.label8.TabIndex = 1;
			this.label8.Text = "All active Meters";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.clbTransducers);
			this.tabPage3.Controls.Add(this.label2);
			this.tabPage3.Controls.Add(this.label9);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(315, 130);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Transducers";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// clbTransducers
			// 
			this.clbTransducers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.clbTransducers.FormattingEnabled = true;
			this.clbTransducers.Location = new System.Drawing.Point(11, 11);
			this.clbTransducers.Name = "clbTransducers";
			this.clbTransducers.Size = new System.Drawing.Size(292, 109);
			this.clbTransducers.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(72, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(171, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "are currently assigned to other kits.";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(72, 52);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(112, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "All active Transducers";
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.clbCalBlocks);
			this.tabPage4.Controls.Add(this.label6);
			this.tabPage4.Controls.Add(this.label11);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(315, 130);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Cal Blocks";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// clbCalBlocks
			// 
			this.clbCalBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.clbCalBlocks.FormattingEnabled = true;
			this.clbCalBlocks.Location = new System.Drawing.Point(11, 11);
			this.clbCalBlocks.Name = "clbCalBlocks";
			this.clbCalBlocks.Size = new System.Drawing.Size(292, 109);
			this.clbCalBlocks.TabIndex = 0;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(73, 65);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(171, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "are currently assigned to other kits.";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(73, 52);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(137, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "All active Calibration Blocks";
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.clbThermos);
			this.tabPage5.Controls.Add(this.label5);
			this.tabPage5.Controls.Add(this.label12);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Size = new System.Drawing.Size(315, 130);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Thermos";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// clbThermos
			// 
			this.clbThermos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.clbThermos.FormattingEnabled = true;
			this.clbThermos.Location = new System.Drawing.Point(11, 11);
			this.clbThermos.Name = "clbThermos";
			this.clbThermos.Size = new System.Drawing.Size(292, 109);
			this.clbThermos.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(72, 65);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(171, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "are currently assigned to other kits.";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(72, 52);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(137, 13);
			this.label12.TabIndex = 1;
			this.label12.Text = "All active Thermometers";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(76, 11);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(75, 20);
			this.txtName.TabIndex = 1;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
			// 
			// KitEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(204)))), ((int)(((byte)(128)))));
			this.ClientSize = new System.Drawing.Size(352, 237);
			this.Controls.Add(this.TabControl1);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(360, 271);
			this.Name = "KitEdit";
			this.Text = "Edit Kit";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.TabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnCancel;
		private TextBoxWithUndo txtName;
		private System.Windows.Forms.TabControl TabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.CheckedListBox clbTransducers;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckedListBox clbInspectors;
		private System.Windows.Forms.CheckedListBox clbMeters;
		private System.Windows.Forms.CheckedListBox clbCalBlocks;
		private System.Windows.Forms.CheckedListBox clbThermos;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label12;
	}
}

