namespace Factotum
{
	partial class GraphicEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphicEdit));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbGetBackgroundImage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSelect = new System.Windows.Forms.ToolStripButton();
            this.tsbDraw = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToolSelector = new System.Windows.Forms.ToolStripDropDownButton();
            this.miSimpleBlackArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miSimpleBlackArrowSm = new System.Windows.Forms.ToolStripMenuItem();
            this.miDoubleBlackArrowSm = new System.Windows.Forms.ToolStripMenuItem();
            this.miHeadlessBlackArrowSm = new System.Windows.Forms.ToolStripMenuItem();
            this.miYellowTextArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miNotation = new System.Windows.Forms.ToolStripMenuItem();
            this.miRegion = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTransparent = new System.Windows.Forms.ToolStripButton();
            this.tsbFillColor = new System.Windows.Forms.ToolStripButton();
            this.tsbStrokeColor = new System.Windows.Forms.ToolStripButton();
            this.tsbTextColor = new System.Windows.Forms.ToolStripButton();
            this.tsbFont = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbBringToFront = new System.Windows.Forms.ToolStripButton();
            this.tsbSendToBack = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.previewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tsbExport = new System.Windows.Forms.ToolStripButton();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.pnlDrawingPanel = new System.Windows.Forms.Panel();
            this.txtDrawingControlText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.btnSaveGraphic = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblCurrentMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.lblSiteName = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbGetBackgroundImage,
            this.toolStripSeparator3,
            this.tsbSelect,
            this.tsbDraw,
            this.toolStripSeparator,
            this.tsbToolSelector,
            this.toolStripSeparator1,
            this.tsbTransparent,
            this.tsbFillColor,
            this.tsbStrokeColor,
            this.tsbTextColor,
            this.tsbFont,
            this.toolStripSeparator2,
            this.tsbBringToFront,
            this.tsbSendToBack,
            this.toolStripSeparator4,
            this.tsbDelete,
            this.toolStripSeparator5,
            this.previewToolStripButton,
            this.tsbExport});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(580, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // tsbGetBackgroundImage
            // 
            this.tsbGetBackgroundImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGetBackgroundImage.Image = ((System.Drawing.Image)(resources.GetObject("tsbGetBackgroundImage.Image")));
            this.tsbGetBackgroundImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGetBackgroundImage.Name = "tsbGetBackgroundImage";
            this.tsbGetBackgroundImage.Size = new System.Drawing.Size(23, 22);
            this.tsbGetBackgroundImage.ToolTipText = "Set Background Image";
            this.tsbGetBackgroundImage.Click += new System.EventHandler(this.tsbGetBackgroundImage_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSelect
            // 
            this.tsbSelect.Checked = true;
            this.tsbSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSelect.Image = ((System.Drawing.Image)(resources.GetObject("tsbSelect.Image")));
            this.tsbSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSelect.Name = "tsbSelect";
            this.tsbSelect.Size = new System.Drawing.Size(23, 22);
            this.tsbSelect.Text = "&New";
            this.tsbSelect.ToolTipText = "Select Mode";
            this.tsbSelect.Click += new System.EventHandler(this.tsbSelect_Click);
            // 
            // tsbDraw
            // 
            this.tsbDraw.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDraw.Image = ((System.Drawing.Image)(resources.GetObject("tsbDraw.Image")));
            this.tsbDraw.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDraw.Name = "tsbDraw";
            this.tsbDraw.Size = new System.Drawing.Size(23, 22);
            this.tsbDraw.Text = "&Open";
            this.tsbDraw.ToolTipText = "Drawing Mode";
            this.tsbDraw.Click += new System.EventHandler(this.tsbDraw_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbToolSelector
            // 
            this.tsbToolSelector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToolSelector.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSimpleBlackArrow,
            this.miSimpleBlackArrowSm,
            this.miDoubleBlackArrowSm,
            this.miHeadlessBlackArrowSm,
            this.miYellowTextArrow,
            this.miNotation,
            this.miRegion});
            this.tsbToolSelector.Image = ((System.Drawing.Image)(resources.GetObject("tsbToolSelector.Image")));
            this.tsbToolSelector.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbToolSelector.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToolSelector.Name = "tsbToolSelector";
            this.tsbToolSelector.Size = new System.Drawing.Size(57, 22);
            this.tsbToolSelector.Text = "toolStripDropDownButton1";
            this.tsbToolSelector.ToolTipText = "Select a drawing tool";
            this.tsbToolSelector.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsbToolSelector_DropDownItemClicked);
            this.tsbToolSelector.Click += new System.EventHandler(this.tsbToolSelector_Click);
            // 
            // miSimpleBlackArrow
            // 
            this.miSimpleBlackArrow.AutoSize = false;
            this.miSimpleBlackArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miSimpleBlackArrow.Image = ((System.Drawing.Image)(resources.GetObject("miSimpleBlackArrow.Image")));
            this.miSimpleBlackArrow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miSimpleBlackArrow.Name = "miSimpleBlackArrow";
            this.miSimpleBlackArrow.Size = new System.Drawing.Size(55, 22);
            this.miSimpleBlackArrow.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // miSimpleBlackArrowSm
            // 
            this.miSimpleBlackArrowSm.AutoSize = false;
            this.miSimpleBlackArrowSm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miSimpleBlackArrowSm.Image = ((System.Drawing.Image)(resources.GetObject("miSimpleBlackArrowSm.Image")));
            this.miSimpleBlackArrowSm.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miSimpleBlackArrowSm.Name = "miSimpleBlackArrowSm";
            this.miSimpleBlackArrowSm.Size = new System.Drawing.Size(55, 22);
            this.miSimpleBlackArrowSm.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // miDoubleBlackArrowSm
            // 
            this.miDoubleBlackArrowSm.AutoSize = false;
            this.miDoubleBlackArrowSm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miDoubleBlackArrowSm.Image = ((System.Drawing.Image)(resources.GetObject("miDoubleBlackArrowSm.Image")));
            this.miDoubleBlackArrowSm.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miDoubleBlackArrowSm.Name = "miDoubleBlackArrowSm";
            this.miDoubleBlackArrowSm.Size = new System.Drawing.Size(55, 22);
            this.miDoubleBlackArrowSm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // miHeadlessBlackArrowSm
            // 
            this.miHeadlessBlackArrowSm.AutoSize = false;
            this.miHeadlessBlackArrowSm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miHeadlessBlackArrowSm.Image = ((System.Drawing.Image)(resources.GetObject("miHeadlessBlackArrowSm.Image")));
            this.miHeadlessBlackArrowSm.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miHeadlessBlackArrowSm.Name = "miHeadlessBlackArrowSm";
            this.miHeadlessBlackArrowSm.Size = new System.Drawing.Size(55, 22);
            this.miHeadlessBlackArrowSm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // miYellowTextArrow
            // 
            this.miYellowTextArrow.AutoSize = false;
            this.miYellowTextArrow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miYellowTextArrow.Image = ((System.Drawing.Image)(resources.GetObject("miYellowTextArrow.Image")));
            this.miYellowTextArrow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miYellowTextArrow.Name = "miYellowTextArrow";
            this.miYellowTextArrow.Size = new System.Drawing.Size(55, 22);
            this.miYellowTextArrow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // miNotation
            // 
            this.miNotation.AutoSize = false;
            this.miNotation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miNotation.Image = ((System.Drawing.Image)(resources.GetObject("miNotation.Image")));
            this.miNotation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miNotation.Name = "miNotation";
            this.miNotation.Size = new System.Drawing.Size(50, 22);
            this.miNotation.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // miRegion
            // 
            this.miRegion.AutoSize = false;
            this.miRegion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miRegion.Image = ((System.Drawing.Image)(resources.GetObject("miRegion.Image")));
            this.miRegion.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.miRegion.Name = "miRegion";
            this.miRegion.Size = new System.Drawing.Size(50, 22);
            this.miRegion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbTransparent
            // 
            this.tsbTransparent.CheckOnClick = true;
            this.tsbTransparent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTransparent.Image = ((System.Drawing.Image)(resources.GetObject("tsbTransparent.Image")));
            this.tsbTransparent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTransparent.Name = "tsbTransparent";
            this.tsbTransparent.Size = new System.Drawing.Size(23, 22);
            this.tsbTransparent.Tag = "";
            this.tsbTransparent.Text = "toolStripButton1";
            this.tsbTransparent.ToolTipText = "Hide Background";
            this.tsbTransparent.Click += new System.EventHandler(this.tsbTransparent_Click);
            // 
            // tsbFillColor
            // 
            this.tsbFillColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFillColor.Image = ((System.Drawing.Image)(resources.GetObject("tsbFillColor.Image")));
            this.tsbFillColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFillColor.Name = "tsbFillColor";
            this.tsbFillColor.Size = new System.Drawing.Size(23, 22);
            this.tsbFillColor.Text = "toolStripButton1";
            this.tsbFillColor.ToolTipText = "Choose Fill Color";
            this.tsbFillColor.Click += new System.EventHandler(this.tsbFillColor_Click);
            // 
            // tsbStrokeColor
            // 
            this.tsbStrokeColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStrokeColor.Image = ((System.Drawing.Image)(resources.GetObject("tsbStrokeColor.Image")));
            this.tsbStrokeColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStrokeColor.Name = "tsbStrokeColor";
            this.tsbStrokeColor.Size = new System.Drawing.Size(23, 22);
            this.tsbStrokeColor.Text = "toolStripButton1";
            this.tsbStrokeColor.ToolTipText = "Choose Border Color";
            this.tsbStrokeColor.Click += new System.EventHandler(this.tsbStrokeColor_Click);
            // 
            // tsbTextColor
            // 
            this.tsbTextColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTextColor.Image = ((System.Drawing.Image)(resources.GetObject("tsbTextColor.Image")));
            this.tsbTextColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTextColor.Name = "tsbTextColor";
            this.tsbTextColor.Size = new System.Drawing.Size(23, 22);
            this.tsbTextColor.Text = "toolStripButton1";
            this.tsbTextColor.ToolTipText = "Choose Text Color";
            this.tsbTextColor.Click += new System.EventHandler(this.tsbTextColor_Click);
            // 
            // tsbFont
            // 
            this.tsbFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFont.Image = ((System.Drawing.Image)(resources.GetObject("tsbFont.Image")));
            this.tsbFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFont.Name = "tsbFont";
            this.tsbFont.Size = new System.Drawing.Size(23, 22);
            this.tsbFont.Text = "toolStripButton1";
            this.tsbFont.ToolTipText = "Select Font";
            this.tsbFont.Click += new System.EventHandler(this.tsbFont_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbBringToFront
            // 
            this.tsbBringToFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBringToFront.Image = ((System.Drawing.Image)(resources.GetObject("tsbBringToFront.Image")));
            this.tsbBringToFront.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBringToFront.Name = "tsbBringToFront";
            this.tsbBringToFront.Size = new System.Drawing.Size(23, 22);
            this.tsbBringToFront.Text = "toolStripButton1";
            this.tsbBringToFront.ToolTipText = "Bring to Front";
            this.tsbBringToFront.Click += new System.EventHandler(this.tsbBringToFront_Click);
            // 
            // tsbSendToBack
            // 
            this.tsbSendToBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSendToBack.Image = ((System.Drawing.Image)(resources.GetObject("tsbSendToBack.Image")));
            this.tsbSendToBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSendToBack.Name = "tsbSendToBack";
            this.tsbSendToBack.Size = new System.Drawing.Size(23, 22);
            this.tsbSendToBack.Text = "toolStripButton2";
            this.tsbSendToBack.ToolTipText = "Send to Back";
            this.tsbSendToBack.Click += new System.EventHandler(this.tsbSendToBack_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbDelete
            // 
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelete.Image")));
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(23, 22);
            this.tsbDelete.Text = "toolStripButton1";
            this.tsbDelete.ToolTipText = "Delete Selected Item";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // previewToolStripButton
            // 
            this.previewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previewToolStripButton.Image = global::Factotum.Properties.Resources.PrintPreview;
            this.previewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previewToolStripButton.Name = "previewToolStripButton";
            this.previewToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.previewToolStripButton.ToolTipText = "Preview Graphic";
            this.previewToolStripButton.Click += new System.EventHandler(this.previewToolStripButton_Click);
            // 
            // tsbExport
            // 
            this.tsbExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbExport.Image = ((System.Drawing.Image)(resources.GetObject("tsbExport.Image")));
            this.tsbExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExport.Name = "tsbExport";
            this.tsbExport.Size = new System.Drawing.Size(23, 22);
            this.tsbExport.Text = "toolStripButton1";
            this.tsbExport.ToolTipText = "Export Graphic to Jpeg";
            this.tsbExport.Click += new System.EventHandler(this.tsbExport_Click);
            // 
            // pnlDrawingPanel
            // 
            this.pnlDrawingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDrawingPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlDrawingPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlDrawingPanel.Location = new System.Drawing.Point(128, 75);
            this.pnlDrawingPanel.Name = "pnlDrawingPanel";
            this.pnlDrawingPanel.Size = new System.Drawing.Size(440, 330);
            this.pnlDrawingPanel.TabIndex = 1;
            this.pnlDrawingPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlDrawingPanel_MouseClick);
            // 
            // txtDrawingControlText
            // 
            this.txtDrawingControlText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDrawingControlText.Location = new System.Drawing.Point(5, 75);
            this.txtDrawingControlText.Multiline = true;
            this.txtDrawingControlText.Name = "txtDrawingControlText";
            this.txtDrawingControlText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDrawingControlText.Size = new System.Drawing.Size(107, 272);
            this.txtDrawingControlText.TabIndex = 2;
            this.txtDrawingControlText.TextChanged += new System.EventHandler(this.txtDrawingControlText_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Edit Selected Item Text";
            // 
            // btnSaveGraphic
            // 
            this.btnSaveGraphic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveGraphic.Location = new System.Drawing.Point(5, 353);
            this.btnSaveGraphic.Name = "btnSaveGraphic";
            this.btnSaveGraphic.Size = new System.Drawing.Size(107, 23);
            this.btnSaveGraphic.TabIndex = 4;
            this.btnSaveGraphic.Text = "OK";
            this.btnSaveGraphic.UseVisualStyleBackColor = true;
            this.btnSaveGraphic.Click += new System.EventHandler(this.btnSaveGraphic_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(5, 382);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(107, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCurrentMode});
            this.statusStrip1.Location = new System.Drawing.Point(0, 420);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(580, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblCurrentMode
            // 
            this.lblCurrentMode.Name = "lblCurrentMode";
            this.lblCurrentMode.Size = new System.Drawing.Size(0, 17);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblSiteName
            // 
            this.lblSiteName.AutoSize = true;
            this.lblSiteName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSiteName.ForeColor = System.Drawing.Color.DimGray;
            this.lblSiteName.Location = new System.Drawing.Point(141, 33);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(80, 16);
            this.lblSiteName.TabIndex = 14;
            this.lblSiteName.Text = "Component:";
            // 
            // GraphicEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 442);
            this.Controls.Add(this.lblSiteName);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveGraphic);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDrawingControlText);
            this.Controls.Add(this.pnlDrawingPanel);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(588, 326);
            this.Name = "GraphicEdit";
            this.Text = "Edit Graphic";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GraphicEdit_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tsbSelect;
		private System.Windows.Forms.ToolStripButton tsbDraw;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
		private System.Windows.Forms.ToolStripDropDownButton tsbToolSelector;
		private System.Windows.Forms.ToolStripMenuItem miDoubleBlackArrowSm;
		private System.Windows.Forms.ToolStripMenuItem miHeadlessBlackArrowSm;
		private System.Windows.Forms.ToolStripMenuItem miYellowTextArrow;
		private System.Windows.Forms.ToolStripButton tsbGetBackgroundImage;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.ToolStripButton tsbFillColor;
		private System.Windows.Forms.ToolStripButton tsbStrokeColor;
		private System.Windows.Forms.ToolStripButton tsbTextColor;
		private System.Windows.Forms.ToolStripButton tsbBringToFront;
		private System.Windows.Forms.ToolStripButton tsbSendToBack;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton tsbDelete;
		private System.Windows.Forms.ToolStripMenuItem miSimpleBlackArrow;
		private System.Windows.Forms.ToolStripMenuItem miSimpleBlackArrowSm;
		private System.Windows.Forms.Panel pnlDrawingPanel;
		private System.Windows.Forms.TextBox txtDrawingControlText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolStripMenuItem miNotation;
		private System.Windows.Forms.ToolStripMenuItem miRegion;
		private System.Windows.Forms.ToolStripButton tsbTransparent;
		private System.Windows.Forms.ToolStripButton tsbFont;
		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.Button btnSaveGraphic;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel lblCurrentMode;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Drawing.Printing.PrintDocument printDocument1;
		private System.Windows.Forms.ToolStripButton previewToolStripButton;
		private System.Windows.Forms.Label lblSiteName;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton tsbExport;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;


	}
}