namespace Factotum
{
	partial class ActivationKeyGenerator
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivationKeyGenerator));
			this.btnGenerate = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnClose = new System.Windows.Forms.Button();
			this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.lblActivationKey = new System.Windows.Forms.Label();
			this.txtNumberOfDays = new Factotum.TextBoxWithUndo();
			this.lblKeyLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerate.Location = new System.Drawing.Point(175, 98);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(64, 22);
			this.btnGenerate.TabIndex = 8;
			this.btnGenerate.Text = "Generate";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of Days";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Location = new System.Drawing.Point(245, 98);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(64, 22);
			this.btnClose.TabIndex = 9;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// dtpStartDate
			// 
			this.dtpStartDate.Location = new System.Drawing.Point(109, 12);
			this.dtpStartDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
			this.dtpStartDate.MinDate = new System.DateTime(2008, 6, 1, 0, 0, 0, 0);
			this.dtpStartDate.Name = "dtpStartDate";
			this.dtpStartDate.Size = new System.Drawing.Size(200, 20);
			this.dtpStartDate.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(48, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(55, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Start Date";
			// 
			// lblActivationKey
			// 
			this.lblActivationKey.AutoSize = true;
			this.lblActivationKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblActivationKey.Location = new System.Drawing.Point(106, 71);
			this.lblActivationKey.Name = "lblActivationKey";
			this.lblActivationKey.Size = new System.Drawing.Size(116, 16);
			this.lblActivationKey.TabIndex = 4;
			this.lblActivationKey.Text = "Click To Generate";
			// 
			// txtNumberOfDays
			// 
			this.txtNumberOfDays.Location = new System.Drawing.Point(109, 38);
			this.txtNumberOfDays.Name = "txtNumberOfDays";
			this.txtNumberOfDays.Size = new System.Drawing.Size(40, 20);
			this.txtNumberOfDays.TabIndex = 1;
			this.txtNumberOfDays.TextChanged += new System.EventHandler(this.txtNumberOfDays_TextChanged);
			// 
			// lblKeyLabel
			// 
			this.lblKeyLabel.AutoSize = true;
			this.lblKeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblKeyLabel.Location = new System.Drawing.Point(8, 71);
			this.lblKeyLabel.Name = "lblKeyLabel";
			this.lblKeyLabel.Size = new System.Drawing.Size(95, 16);
			this.lblKeyLabel.TabIndex = 2;
			this.lblKeyLabel.Text = "Activation Key:";
			// 
			// ActivationKeyGenerator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(321, 132);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.dtpStartDate);
			this.Controls.Add(this.lblActivationKey);
			this.Controls.Add(this.lblKeyLabel);
			this.Controls.Add(this.txtNumberOfDays);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnGenerate);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(329, 166);
			this.Name = "ActivationKeyGenerator";
			this.Text = "Activation Key Generator";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnClose;
		private TextBoxWithUndo txtNumberOfDays;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker dtpStartDate;
		private System.Windows.Forms.Label lblActivationKey;
		private System.Windows.Forms.Label lblKeyLabel;
	}
}

