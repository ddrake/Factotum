namespace Factotum
{
	partial class ComponentListing
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
			Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
			this.TmpComponentListingBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.ComponentListingDataSet = new Factotum.ComponentListingDataSet();
			this.cboUnit = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.rvComponentListing = new Microsoft.Reporting.WinForms.ReportViewer();
			this.TmpComponentListingTableAdapter = new Factotum.ComponentListingDataSetTableAdapters.TmpComponentListingTableAdapter();
			this.btnGenerate = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.TmpComponentListingBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ComponentListingDataSet)).BeginInit();
			this.SuspendLayout();
			// 
			// TmpComponentListingBindingSource
			// 
			this.TmpComponentListingBindingSource.DataMember = "TmpComponentListing";
			this.TmpComponentListingBindingSource.DataSource = this.ComponentListingDataSet;
			// 
			// ComponentListingDataSet
			// 
			this.ComponentListingDataSet.DataSetName = "ComponentListingDataSet";
			this.ComponentListingDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// cboUnit
			// 
			this.cboUnit.FormattingEnabled = true;
			this.cboUnit.Location = new System.Drawing.Point(90, 9);
			this.cboUnit.Name = "cboUnit";
			this.cboUnit.Size = new System.Drawing.Size(186, 21);
			this.cboUnit.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(58, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Unit";
			// 
			// rvComponentListing
			// 
			this.rvComponentListing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			reportDataSource1.Name = "ComponentListingDataSet_TmpComponentListing";
			reportDataSource1.Value = this.TmpComponentListingBindingSource;
			this.rvComponentListing.LocalReport.DataSources.Add(reportDataSource1);
			this.rvComponentListing.LocalReport.ReportEmbeddedResource = "Factotum.ComponentListing.rdlc";
			this.rvComponentListing.Location = new System.Drawing.Point(12, 48);
			this.rvComponentListing.Name = "rvComponentListing";
			this.rvComponentListing.Size = new System.Drawing.Size(635, 250);
			this.rvComponentListing.TabIndex = 2;
			// 
			// TmpComponentListingTableAdapter
			// 
			this.TmpComponentListingTableAdapter.ClearBeforeFill = true;
			// 
			// btnGenerate
			// 
			this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerate.Location = new System.Drawing.Point(532, 12);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(115, 23);
			this.btnGenerate.TabIndex = 3;
			this.btnGenerate.Text = "Generate Report";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// ComponentListing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(659, 310);
			this.Controls.Add(this.btnGenerate);
			this.Controls.Add(this.rvComponentListing);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboUnit);
			this.MinimumSize = new System.Drawing.Size(424, 149);
			this.Name = "ComponentListing";
			this.Text = "Component Listing";
			this.Load += new System.EventHandler(this.ComponentListing_Load);
			((System.ComponentModel.ISupportInitialize)(this.TmpComponentListingBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ComponentListingDataSet)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboUnit;
		private System.Windows.Forms.Label label1;
		private Microsoft.Reporting.WinForms.ReportViewer rvComponentListing;
		private System.Windows.Forms.BindingSource TmpComponentListingBindingSource;
		private ComponentListingDataSet ComponentListingDataSet;
		private Factotum.ComponentListingDataSetTableAdapters.TmpComponentListingTableAdapter TmpComponentListingTableAdapter;
		private System.Windows.Forms.Button btnGenerate;
	}
}