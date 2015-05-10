using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Imaging;

// Todo: how can we prevent the user from dragging a control outside the panel?
// Todo: redesign the cursors and make some new ones.
// Todo: figure out how to store all the graphic information to the database.
// Todo: figure out how to print a graphic as part of a report.
// Note: I originally designed this so that you could enter the text for the control you were
// ABOUT TO DRAW, and not be limited to editing the text of the currently selected item.
// This didn't work too well because I didn't have a separate buffer for "default text".
// In the end it's overkill for this app so I scrapped it.
namespace Factotum
{
	public partial class GraphicEdit : Form, IEntityEditForm
	{
		// **************************************************************
		// MEMBER VARIABLES AND ENUMERATIONS
		// **************************************************************
		private EGraphic curGraphic;

        private bool isImageNotFound = false;
        private bool isImageAtDefaultPath = false;

		// Allow the calling form to access the entity
		public IEntity Entity
		{
			get { return curGraphic; }
		}

		// The two User Interface modes
		public enum UiMode {
			SelectMode,
			DrawMode
		};
		// Currently just these drawing tools
		public enum DrawingTool {
			DoubleBlackArrowSm,
			HeadlessBlackArrowSm,
			SimpleBlackArrow,
			SimpleBlackArrowSm,
			YellowTextArrow,
			Notation,
			Region
		};

		// The current modes
		private UiMode currentUiMode;
		private DrawingTool currentDrawingTool;
		
		// The current default font for all new drawing controls that have text
		private Font defaultFont;


		// **************************************************************
		// FORM CONSTRUCTOR
		// **************************************************************
		public GraphicEdit(Guid? ID)
			: this(ID, null){}

		public GraphicEdit(Guid? ID, Guid? inspectionID)
		{
			InitializeComponent();
			curGraphic = new EGraphic();
			curGraphic.Load(ID);
			if (inspectionID != null) curGraphic.GraphicIspID = inspectionID;
			InitializeControls(ID == null);
		}

		private void InitializeControls(bool newRecord)
		{
			this.Text = newRecord ? "New Graphic" : "Edit Graphic";
			// Todo: there should be a default image that possibly directs the user to 
			// load an image via the load image button
					
			// Set default mode and tool
			CurrentUiMode = UiMode.DrawMode;
			CurrentDrawingTool = DrawingTool.SimpleBlackArrow;

			// Set the default DrawingControl font to the Form's default font.
			defaultFont = Font;
			tsbFont.ToolTipText = "Set default font";

			// Initially no drawing objects, so disable any buttons which require a selection
			DisEnableSelectedItemButtons(false);
			DisEnableZButtons(false);
			SetControlValues();
			btnSaveGraphic.Enabled = curGraphic.GraphicBgImageFileName != null;

		}


		private FontFamily GetFontFamilyForName(string name)
		{
			FontFamily[] families = FontFamily.Families;
			foreach (FontFamily family in families)
			{
				if (family.Name == name) return family;
			}
			return FontFamily.GenericSerif;
		}

        private string SubstituteDefaultImagePath(string imagePath)
        {
            return UserSettings.sets.DefaultImageFolder + "\\" + Path.GetFileName(imagePath);
        }

        private Bitmap GetImageDeepClone(Bitmap srcImg)
        {
            Bitmap srcImgCopy;
            PixelFormat fmt;
            if (srcImg.PixelFormat == PixelFormat.Format4bppIndexed ||
                srcImg.PixelFormat == PixelFormat.Format8bppIndexed ||
                srcImg.PixelFormat == PixelFormat.Format1bppIndexed)
                fmt = PixelFormat.Format16bppRgb565;
            else
                fmt = srcImg.PixelFormat;
            srcImgCopy = new Bitmap(srcImg.Width, srcImg.Height);

            // Make sure the resolution is the same as the original image...
            srcImgCopy.SetResolution(srcImg.HorizontalResolution, srcImg.VerticalResolution);

            Graphics g = Graphics.FromImage(srcImgCopy);

            if (srcImg.Height > srcImg.Width)
            {
                int sx = srcImg.Height * pnlDrawingPanel.Width / pnlDrawingPanel.Height;
                Region reg = new Region(new Rectangle(new Point(0, 0), new Size(sx, srcImg.Height)));
                g.Clip = new Region(new Rectangle(new Point(0, 0), new Size(sx, srcImg.Height)));
                srcImgCopy = new Bitmap((int)g.ClipBounds.Width, (int)g.ClipBounds.Height, g);
                g = Graphics.FromImage(srcImgCopy);
                g.FillRegion(Brushes.White, reg);
                Point pt = new Point((int)(sx - srcImg.Width) / 2, 0);
                g.DrawImage(srcImg, pt);
            }
            else
            {
                g.DrawImage(srcImg, new Point(0, 0));
            }

            g.Dispose();
            srcImg.Dispose();
            return srcImgCopy; 
        }

		// Set the form controls to the inspected component object values.
		private void SetControlValues()
		{
            Bitmap backImage = null;
			if (curGraphic.GraphicIspID != null)
			{
				EInspection inspection =
					new EInspection(curGraphic.GraphicIspID);
				lblSiteName.Text = "Graphic for Inspection: '" + inspection.InspectionName + "'";
			}
			else lblSiteName.Text = "Graphic for Unknown Inspection";
			DowUtils.Util.CenterControlHorizInForm(lblSiteName, this);
			EDrawingControlCollection ctrls = EDrawingControl.ListForGraphicByZindex(curGraphic.ID);
			if (curGraphic.GraphicBgImageFileName != null)
			{
                if (File.Exists(curGraphic.GraphicBgImageFileName))
                {
					backImage = (Bitmap)Image.FromFile(curGraphic.GraphicBgImageFileName);
                }
                else
                {
                    string imgDefaultPath = SubstituteDefaultImagePath(curGraphic.GraphicBgImageFileName);
                    if (File.Exists(imgDefaultPath))
                    {
                        isImageAtDefaultPath = true;
                        backImage = (Bitmap)Image.FromFile(imgDefaultPath);
                    }
                    else
                    {
                        isImageNotFound = true;
 					    MessageBox.Show("The background image file: '" + curGraphic.GraphicBgImageFileName
						    + "' could not be opened.\r\nIf it has been moved, try changing the Default Image Folder in Preferences > General.",
                            "Factotum");
                    }
                }
                if (backImage != null)
                {
                    backImage = GetImageDeepClone(backImage);
                    pnlDrawingPanel.BackgroundImage = backImage;
                }
			}
			foreach (EDrawingControl eCtrl in ctrls)
			{
				FontStyle style = FontStyle.Regular 
					| (eCtrl.DrawingControlFontIsBold ? FontStyle.Bold : 0) 
					| (eCtrl.DrawingControlFontIsItalic ? FontStyle.Italic : 0)
					| (eCtrl.DrawingControlFontIsUnderlined ? FontStyle.Underline : 0);


				FontFamily family = GetFontFamilyForName(eCtrl.DrawingControlFontFamily);
				Font font = new Font(family,eCtrl.DrawingControlFontPoints,style);
				Color fillColor = Color.FromArgb(eCtrl.DrawingControlFillColor);
				Color strokeColor = Color.FromArgb(eCtrl.DrawingControlStrokeColor);
				Color textColor = Color.FromArgb(eCtrl.DrawingControlTextColor);
				switch (eCtrl.DrawingControlType)
				{
					case (byte)EDrawingControlType.Arrow:
						EArrow eArrow = new EArrow();
						eArrow.LoadForDrawingControl((Guid)eCtrl.ID);
						Arrow arrow = new Arrow(
							new PointF((float)eArrow.ArrowStartX, (float)eArrow.ArrowStartY),
							new PointF((float)eArrow.ArrowEndX, (float)eArrow.ArrowEndY),
							eArrow.ArrowHeadCount,
							eArrow.ArrowShaftWidth,
							eArrow.ArrowBarb,
							eArrow.ArrowTip,
							eCtrl.DrawingControlStroke,
							eCtrl.DrawingControlText,
							eCtrl.DrawingControlHasText,
							fillColor,
							textColor,
							strokeColor,
							eCtrl.DrawingControlHasTranspBackground,
							font);
						pnlDrawingPanel.Controls.Add(arrow);
						arrow.StateChange +=
							new DrawingControl.SelectStateChangedEventHandler(AnyDwgControlSelectedStateChanged);
						arrow.KeyDown += new KeyEventHandler(AnyDwgControlKeyDown);
						break;
					case (byte)EDrawingControlType.Notation:
						ENotation eNotation = new ENotation();
						eNotation.LoadForDrawingControl((Guid)eCtrl.ID);
						Notation notation = new Notation(
							new Point(eNotation.NotationLeft, eNotation.NotationTop),
							new Size(eNotation.NotationWidth, eNotation.NotationHeight),
							eCtrl.DrawingControlStroke,
							eCtrl.DrawingControlText,
							fillColor,
							textColor,
							strokeColor,
							eCtrl.DrawingControlHasTranspBackground,
							font);
						pnlDrawingPanel.Controls.Add(notation);
						notation.StateChange +=
							new DrawingControl.SelectStateChangedEventHandler(AnyDwgControlSelectedStateChanged);
						notation.KeyDown += new KeyEventHandler(AnyDwgControlKeyDown);
						break;
					case (byte)EDrawingControlType.Boundary:
						EBoundary eBoundary = new EBoundary();
						eBoundary.LoadForDrawingControl((Guid)eCtrl.ID);
						PointF[] boundaryPts = EBoundaryPoint.GetArrayByIndexForBoundary((Guid)eBoundary.ID);
						Boundary boundary = new Boundary(
							boundaryPts,
							eCtrl.DrawingControlStroke,
							eCtrl.DrawingControlText,
							eCtrl.DrawingControlHasText,
							fillColor,
							textColor,
							strokeColor,
							font,
							eBoundary.BoundaryAlpha);
						pnlDrawingPanel.Controls.Add(boundary);
						boundary.StateChange +=
							new DrawingControl.SelectStateChangedEventHandler(AnyDwgControlSelectedStateChanged);
						boundary.KeyDown += new KeyEventHandler(AnyDwgControlKeyDown);
						break;

					default:
						break;
				} 

			}
		}


		// **************************************************************
		// PROPERTIES (Should they really be public?)
		// **************************************************************
		// Current User Interface Mode Property
		public UiMode CurrentUiMode
		{
			get { return currentUiMode; }
			set 
			{
				if (currentUiMode != value)
				{
					// the ui mode changed
					currentUiMode = value;
					switch (value)
					{
						// the mode changed to Select Mode.
						// so toggle the button checked states and update the status bar label.
						case UiMode.SelectMode:
							
							tsbDraw.Checked = false;
							tsbSelect.Checked = true;
							lblCurrentMode.Text = "Select Mode";
							// Also set the the textbox text to match that of the selected item 
							// if there is one or clear it if not.
							DrawingControl dc = getSelectedDrawingControl();
							txtDrawingControlText.Text = ((dc != null && dc.HasText) ? dc.Text : "");
							break;
						// the mode changed to Draw Mode.
						// so toggle the button checked states and update the status bar label.
						case UiMode.DrawMode:
							tsbDraw.Checked = true;
							tsbSelect.Checked = false;
							lblCurrentMode.Text = "Drawing Mode";
							// Also make sure no drawing objects are selected
							deselectAllDrawingControls();
							break;
						default:
							break;
					}
					// Enable or disable the textbox based on the mode and current tool or
					// selected DrawingObject.
					DisEnableTextbox();
				}
			}
		}
		
		// Current Drawing tool property
		public DrawingTool CurrentDrawingTool
		{
			get { return currentDrawingTool; }
			set
			{
				if (currentDrawingTool != value)
				{
					currentDrawingTool = value;
				}
				// Any time the user accesses the drawing tools, ensure that no control is selected,
				// Set the textbox text to the default text for the chosen tool, and 
				// disable the textbox.  We don't allow edits until after it's drawn.
				deselectAllDrawingControls();
				SetDefaultText(currentDrawingTool);
				DisEnableTextbox();

				// We don't do anything with the font here, because fonts are handled differently.
				// There are no default fonts assigned to specific tools, 
				// just a current control font and a current default font.
			}
		}

		// **************************************************************
		// GENERAL USER INTERFACE EVENT HANDLERS
		// **************************************************************

		// Handle the user clicking on the drawing panel
		private void pnlDrawingPanel_MouseClick(object sender, MouseEventArgs e)
		{
			DrawingControl dc;
			if (CurrentUiMode == UiMode.SelectMode)
			{
				// If we're in Select mode, clicking on the form deselects any selected item
				// and clears the textbox text.
				deselectAllDrawingControls();
				txtDrawingControlText.Text = "";
			}
			else if (CurrentUiMode == UiMode.DrawMode)
			{
				// If we're in Drawing mode, clicking on the form draws a new DrawingControl
				// of the CurrentDrawingTool type.

				// Set up the default colors for the control of the CurrentDrawingTool type.
				Color fillColor, textColor, strokeColor;
				switch (CurrentDrawingTool)
				{
					case DrawingTool.DoubleBlackArrowSm:
						fillColor = Color.Black;
						textColor = Color.Black;
						strokeColor = Color.Black;
						break;
					case DrawingTool.HeadlessBlackArrowSm:
						fillColor = Color.Black;
						textColor = Color.Black;
						strokeColor = Color.Black;
						break;
					case DrawingTool.SimpleBlackArrow:
						fillColor = Color.Black;
						textColor = Color.Black;
						strokeColor = Color.Black;
						break;
					case DrawingTool.SimpleBlackArrowSm:
						fillColor = Color.Black;
						textColor = Color.Black;
						strokeColor = Color.Black;
						break;
					case DrawingTool.YellowTextArrow:
						fillColor = Color.Yellow;
						textColor = Color.Red;
						strokeColor = Color.Black;
						break;
					case DrawingTool.Notation:
						fillColor = Color.White;
						textColor = Color.Black;
						strokeColor = Color.Black;
						break;
					case DrawingTool.Region:
						fillColor = Color.AliceBlue;
						textColor = Color.Black;
						strokeColor = Color.Black;
						break;
					default:
						throw new Exception("Unknown drawing tool");
				}
				// Create the new DrawingControl
				switch (CurrentDrawingTool)
				{
					case DrawingTool.SimpleBlackArrow:
						dc = new Arrow(new PointF(e.X, e.Y), 1, 6, 4, 14, 1, "", false,
							fillColor, textColor, strokeColor, false, defaultFont);
						break;
					case DrawingTool.DoubleBlackArrowSm:
						dc = new Arrow(new PointF(e.X, e.Y),2, 2, 2, 10, 1, "", false,
							fillColor, textColor, strokeColor, false, defaultFont);
						break;
					case DrawingTool.SimpleBlackArrowSm:
						dc = new Arrow(new PointF(e.X, e.Y), 1, 2, 2, 10, 1, "", false,
							fillColor, textColor, strokeColor, false, defaultFont);
						break;
					case DrawingTool.HeadlessBlackArrowSm:
						dc = new Arrow(new PointF(e.X, e.Y),0, 2, 0 , 0, 1, "", false,
							fillColor, textColor, strokeColor, false, defaultFont);
						break;
					case DrawingTool.YellowTextArrow:
						dc = new Arrow(new PointF(e.X, e.Y),1, 20, 6, 15, 1, txtDrawingControlText.Text, true,
							fillColor, textColor, strokeColor, false, defaultFont);
						break;
					case DrawingTool.Notation:
						dc = new Notation(new Point(e.X, e.Y), 1, txtDrawingControlText.Text,
							fillColor, textColor, strokeColor, tsbTransparent.Checked, defaultFont);
						break;
					case DrawingTool.Region:
						dc = new Boundary(new Point(e.X, e.Y), new Size(50,50),12,1,"",false,fillColor,textColor,strokeColor,defaultFont,150);
						break;
					default:
						throw new Exception("Unknown drawing tool");
				}
				// Add the new drawing control to the panel.
				pnlDrawingPanel.Controls.Add(dc);
				dc.BringToFront();
				// Set to receive selection state change events raised by the new DrawingControl
				dc.StateChange +=
					new DrawingControl.SelectStateChangedEventHandler(AnyDwgControlSelectedStateChanged);
				dc.KeyDown += new KeyEventHandler(AnyDwgControlKeyDown);
				// Set the new drawing control's state to selected and the mode to select mode
				// so the user can edit if desired.
				dc.IsSelected = true;
				CurrentUiMode = UiMode.SelectMode;
			}
			else throw new Exception("Unknown User Interface Mode");
			// Enable the textbox only if we've just created a DrawingControl that has text
			DisEnableTextbox();
		}

		// Handle a selected state change of any drawing control
		private void AnyDwgControlSelectedStateChanged(object sender, EventArgs e)
		{
			DrawingControl dc = (DrawingControl)sender;
			if (dc.IsSelected)
			{
				// Deselect all the other drawing controls.  Only allow selecting one at a time.
				deselectAllDrawingControlsExcept(dc);

				// Change the textbox text to match that of the newly selected control
				txtDrawingControlText.Text = (dc.HasText ? dc.Text : "");

				// Set the state of the Transparent background button to match
				// the setting of the newly selected control.
				if (dc is Boundary)
				{
					tsbTransparent.Checked = false;
					tsbTransparent.ToolTipText = "Set Background Opacity";
				}
				else
				{
					tsbTransparent.Checked = dc.HasTransparentBackground;
					tsbTransparent.ToolTipText = tsbTransparent.Checked ? "Show Background" : "Hide Background";
				}
				// Let the user know that the font button is now affecting just the selected item.
				tsbFont.ToolTipText = "Set font for item";

				// Enable the textbox if the selected control has text
				DisEnableTextbox();

				// Enable the Z-order buttons if we have more than one control
				if (pnlDrawingPanel.Controls.Count > 1) DisEnableZButtons(true);

				// Enable the buttons that require a selection (e.g. color btns, delete btn)
				DisEnableSelectedItemButtons(true, dc.HasText);

				// Enable the font button if and only if the selected control has text.
				tsbFont.Enabled = dc.HasText;

				// Set the current mode to Select mode.  
				// If we were in drawing mode and clicked on an existing control 
				// instead of drawing on the form, we should go back to Select mode
				CurrentUiMode = UiMode.SelectMode;
			}
		}

		// Pass on textbox text changes to the selected control
		private void txtDrawingControlText_TextChanged(object sender, EventArgs e)
		{
			DrawingControl dc = getSelectedDrawingControl();
			if (dc != null)
			{
				dc.Text = txtDrawingControlText.Text;
				dc.Invalidate();
			}
		}

		// Handle the user pressing the 'Delete' key.
		void AnyDwgControlKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete) DeleteSelectedControl();
		}

		// **************************************************************
		// TOOLBAR EVENT HANDLERS
		// **************************************************************

		// Handle the user clicking to select a new background image.
		private void tsbGetBackgroundImage_Click(object sender, EventArgs e)
		{
            Bitmap backImage;
			if (curGraphic.GraphicBgImageFileName != null)
			{
				FileInfo fi = new FileInfo(curGraphic.GraphicBgImageFileName);
				openFileDialog1.FileName = fi.Name;
				openFileDialog1.InitialDirectory = fi.DirectoryName;
			}
			else
			{
				openFileDialog1.FileName = "";
				openFileDialog1.InitialDirectory = Globals.ImageFolder;
			}
			// Todo: what file types should we allow?
			openFileDialog1.Filter = "Image Files *.jpg, *.gif, *.png, *.bmp|*.jpg;*.gif;*.png;*.bmp";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				// Dispose of the previous background image if there was any, otherwise that file will be
				// locked to the user until such time as garbage disposal gets around to its job.
				if (pnlDrawingPanel.BackgroundImage != null) pnlDrawingPanel.BackgroundImage.Dispose();
				curGraphic.GraphicBgImageFileName = openFileDialog1.FileName;

                backImage = (Bitmap)Image.FromFile(openFileDialog1.FileName);

                // free up the image file.
                backImage = GetImageDeepClone(backImage);
				pnlDrawingPanel.BackgroundImage = backImage;

				if (pnlDrawingPanel.BackgroundImage.Height > pnlDrawingPanel.BackgroundImage.Width)
					MessageBox.Show("The selected background image is in portrait mode.  To avoid distortion, it must be cropped or rotated." + 
						"\r\nPlease close the Graphic Editor before making adjustments to the image file.","Factotum: Background Image In Portrait Mode");
			}
			curGraphic.GraphicBgImageType = (byte)BackgroundImageFileType.Jpeg;
			if (curGraphic.GraphicBgImageFileName != null) btnSaveGraphic.Enabled = true;

            // Clear these flag if it was set, since a new image has now been selected
            isImageAtDefaultPath = false;
            isImageNotFound = false;
        }

		// Handle user click on the select mode button
		private void tsbSelect_Click(object sender, EventArgs e)
		{
			CurrentUiMode = UiMode.SelectMode;
		}

		// Handle user click on the draw mode button
		private void tsbDraw_Click(object sender, EventArgs e)
		{
			CurrentUiMode = UiMode.DrawMode;
			// Set the textbox text to the default text for the current tool
			SetDefaultText(CurrentDrawingTool);
		}

		// If the user clicks to select any tool, switch to draw mode
		private void tsbToolSelector_Click(object sender, EventArgs e)
		{
			CurrentUiMode = UiMode.DrawMode;
		}

		// Handle user selecting a drawing tool
		private void tsbToolSelector_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			// Change the selector's image to the currently selected image
			tsbToolSelector.Image = e.ClickedItem.Image;

			// Set the current tool to the selected item
			switch (e.ClickedItem.Name)
			{
				case "miDoubleBlackArrowSm":
					CurrentDrawingTool = DrawingTool.DoubleBlackArrowSm;
					break;
				case "miHeadlessBlackArrowSm":
					CurrentDrawingTool = DrawingTool.HeadlessBlackArrowSm;
					break;
				case "miSimpleBlackArrow":
					CurrentDrawingTool = DrawingTool.SimpleBlackArrow;
					break;
				case "miSimpleBlackArrowSm":
					CurrentDrawingTool = DrawingTool.SimpleBlackArrowSm;
					break;
				case "miYellowTextArrow":
					CurrentDrawingTool = DrawingTool.YellowTextArrow;
					break;
				case "miNotation":
					CurrentDrawingTool = DrawingTool.Notation;
					break;
				case "miRegion":
					CurrentDrawingTool = DrawingTool.Region;
					break;
				default:
					throw new Exception("Invalid drop-down menu item name");
			}
		}

		// If the user clicks to toggle the background transparency, do so.
		private void tsbTransparent_Click(object sender, EventArgs e)
		{
			DrawingControl dc = getSelectedDrawingControl();
			System.Diagnostics.Debug.Assert(dc != null);
			if (dc is Boundary)
			{
				Boundary b = (Boundary)dc;
				TransparencySelector ts = new TransparencySelector();
				ts.AlphaLevel = b.AlphaLevel;
				ts.ShowDialog();
				if (ts.DialogResult == DialogResult.OK)
				{
					b.AlphaLevel = ts.AlphaLevel;
				}
				// Force it back to unchecked.
				tsbTransparent.Checked = false;
			}
			else
			{
				dc.HasTransparentBackground = tsbTransparent.Checked;

				// Change the tool tip appropriately
				tsbTransparent.ToolTipText = tsbTransparent.Checked ? "Show Background" : "Hide Background";
			}
			dc.Invalidate();
		}

		// If the user clicks to select a fill color, show a dialog to get a new color
		private void tsbFillColor_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Assert(CurrentUiMode == UiMode.SelectMode);
			
			DrawingControl dc = getSelectedDrawingControl();
			System.Diagnostics.Debug.Assert(dc != null);
			
			// Initialize the dialog color to the user's selected color
			colorDialog1.Color = dc.FillColor;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				dc.FillColor = colorDialog1.Color;
				dc.Invalidate();
			}
		}

		// If the user clicks to select a stroke color, show a dialog to get a new color
		private void tsbStrokeColor_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Assert(CurrentUiMode == UiMode.SelectMode);

			DrawingControl dc = getSelectedDrawingControl();
			System.Diagnostics.Debug.Assert(dc != null);
			// Initialize the dialog color to the user's selected color
			colorDialog1.Color = dc.StrokeColor;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				dc.StrokeColor = colorDialog1.Color;
				dc.Invalidate();
			}
		}

		// If the user clicks to select a text color, show a dialog to get a new color
		private void tsbTextColor_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Assert(CurrentUiMode == UiMode.SelectMode);

			DrawingControl dc = getSelectedDrawingControl();
			System.Diagnostics.Debug.Assert(dc != null);
			// Initialize the dialog color to the user's selected color
			colorDialog1.Color = dc.TextColor;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				dc.TextColor = colorDialog1.Color;
				dc.Invalidate();
			}
		}

		// Handle the user clicking to change font.
		private void tsbFont_Click(object sender, EventArgs e)
		{
			DrawingControl dc = getSelectedDrawingControl();

			// If there's a drawing control selected, we're just changing its font
			// Otherwise, we're changing the default font for new drawing controls.

			// Initialize the font dialog to the current font and disallow script change
			// just because it seems unnecessarily complicated.
			fontDialog1.Font = (dc == null ? defaultFont : dc.Font);
			fontDialog1.AllowScriptChange = false;
			if (fontDialog1.ShowDialog() == DialogResult.OK)
			{
				if (dc != null)
				{
					// There was a control selected, so set its font
					dc.Font = fontDialog1.Font;
					dc.Invalidate();
				}
				else
				{
					// No control selected, so change the default font
					defaultFont = fontDialog1.Font;

					if (existDrawingControlsWithText())
					{
						// If there are existing controls with text check whether or not the user 
						// wants to update them to the new font.
						if (MessageBox.Show("Update existing drawing elements to this new default font?", "Factotum: Setting New Default Font",
							MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							foreach (DrawingControl d in pnlDrawingPanel.Controls)
							{
								d.Font = defaultFont;
								d.Invalidate();
							}
						}
					}
				}
			}
		}

		// If the user clicks to bring the selected item to the front, do so.
		private void tsbBringToFront_Click(object sender, EventArgs e)
		{
			DrawingControl dc = getSelectedDrawingControl();
			System.Diagnostics.Debug.Assert(dc != null);
			dc.BringToFront();
		}

		// If the user clicks to send the selected item to the back, do so.
		private void tsbSendToBack_Click(object sender, EventArgs e)
		{
			DrawingControl dc = getSelectedDrawingControl();
			System.Diagnostics.Debug.Assert(dc != null);
			dc.SendToBack();
		}

		// Handle the user clicking the delete button
		private void tsbDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedControl();
		}

		// **************************************************************
		// UTILITY FUNCTIONS 
		// **************************************************************

		// Set the textbox text to the default text for the current tool
		private void SetDefaultText(DrawingTool dt)
		{
			switch (dt)
			{
				case DrawingTool.DoubleBlackArrowSm:
				case DrawingTool.HeadlessBlackArrowSm:
				case DrawingTool.SimpleBlackArrow:
				case DrawingTool.SimpleBlackArrowSm:
				case DrawingTool.Region:
					txtDrawingControlText.Text = "";
					break;
				case DrawingTool.YellowTextArrow:
					txtDrawingControlText.Text = "FLOW";
					break;
				case DrawingTool.Notation:
					txtDrawingControlText.Text = "ENTER TEXT";
					break;
				default:
					throw new Exception("Invalid drop-down menu item name");
			}
		}
		
		// Check if we have any drawing controls that have text.
		private bool existDrawingControlsWithText()
		{
			foreach (DrawingControl d in pnlDrawingPanel.Controls)
				if (d.HasText) return true;

			return false;
		}

		// Delete the selected control, if there is one.
		private void DeleteSelectedControl()
		{
			DrawingControl dc = getSelectedDrawingControl();
			if (dc != null)
			{
				pnlDrawingPanel.Controls.Remove(dc);
				dc.Dispose();
				// After we delete, there won't be any selected items
				DisEnableSelectedItemButtons(false);
				DisEnableZButtons(false);
				tsbFont.ToolTipText = "Set default font";
				// After we delete, there won't be any selected text
				txtDrawingControlText.Text = "";
				DisEnableTextbox();
			}
		}

		// De-select all the drawing controls except the specified one.
		private void deselectAllDrawingControlsExcept(Control ctl)
		{
			foreach (DrawingControl c in pnlDrawingPanel.Controls)
				if (!c.Equals(ctl)) c.IsSelected = false;
		}

		// De-select all the drawing controls.
		private void deselectAllDrawingControls()
		{
			foreach (DrawingControl c in pnlDrawingPanel.Controls)
				c.IsSelected = false;

			// Since there is no longer a selected control, disable some buttons
			DisEnableSelectedItemButtons(false);
			DisEnableZButtons(false);

			// Let the user know that the font button now sets the default font
			// and make sure it's enabled.
			tsbFont.ToolTipText = "Set default font";
			tsbFont.Enabled = true;
		}

		// Handle enabling and disabling of buttons that should only be enabled if there is
		// a drawing control selected.
		private void DisEnableSelectedItemButtons(bool status)
		{
			tsbDelete.Enabled = status;
			tsbFillColor.Enabled = status;
			tsbStrokeColor.Enabled = status;
			tsbTransparent.Enabled = status;
			tsbTextColor.Enabled = status;
		}

		// Handle default enabling and disabling of buttons that need a drawing control selected.
		// Then handle the special case of the text color button.
		private void DisEnableSelectedItemButtons(bool status, bool hasText)
		{
			DisEnableSelectedItemButtons(status);
			tsbTextColor.Enabled = status && hasText;
		}

		// Handle enabling and disabling of the bringToFront and SendToBack buttons
		// These should only be enabled if there is a drawing control selected 
		// AND there are at least two drawing controls in the container
		private void DisEnableZButtons(bool status)
		{
			tsbBringToFront.Enabled = status;
			tsbSendToBack.Enabled = status;
		}

		// Handle enabling and disabling of the textbox
		private void DisEnableTextbox()
		{
			if (CurrentUiMode == UiMode.SelectMode)
			{
				// If we're in select mode and a drawing control is selected,
				// enable the textbox if the control has text.
				DrawingControl dc = getSelectedDrawingControl();
				if (dc != null) txtDrawingControlText.Enabled = (dc.HasText);

				// If no drawing controls are selected, disable the textbox.
				else txtDrawingControlText.Enabled = false;
			}
			// If we're in drawing mode, disable the textbox.
			else txtDrawingControlText.Enabled = false;
		}

		// Return the selected drawing control or null if none are selected.
		private DrawingControl getSelectedDrawingControl()
		{
			foreach (DrawingControl dc in pnlDrawingPanel.Controls)
				if (dc.IsSelected) return dc;

			return null;
		}

		private void tsbPrint_Click(object sender, EventArgs e)
		{
			//CaptureScreen();
			//printDocument1.Print();
		}

		private Bitmap graphicImage;
#if false
		private void CaptureScreen()
		{
			graphicImage = new Bitmap(pnlDrawingPanel.Width, pnlDrawingPanel.Height);

			pnlDrawingPanel.DrawToBitmap(graphicImage,
				new Rectangle(0, 0, pnlDrawingPanel.Width, pnlDrawingPanel.Height));
		}
#endif
		// This function creates a graphics from the background image and then paints each control
		// into it in the correct sequence.  This way the transparency is always handled correctly
		// without the controls needing to be aware of each other.
		// If we are called with 'forStoring' true, we double the width and height of the image and 
		// paint the controls at twice their actual size.  This gives us high enough density of
		// graphical data for the printed image to look reasonably good.
		private Image CaptureScreen(bool forStoring)
		{
			Graphics g;
			if (forStoring)
			{
				Size sz = new Size(pnlDrawingPanel.DisplayRectangle.Width * 2,
					pnlDrawingPanel.DisplayRectangle.Height * 2);
				graphicImage = new Bitmap(pnlDrawingPanel.BackgroundImage, sz);
				g = Graphics.FromImage(graphicImage);
				g.PageUnit = GraphicsUnit.Pixel;
				g.PageScale = 2.0F;
			}
			else
			{
				Size sz = new Size(pnlDrawingPanel.DisplayRectangle.Width,
					pnlDrawingPanel.DisplayRectangle.Height);
				graphicImage = new Bitmap(pnlDrawingPanel.BackgroundImage, sz);
				g = Graphics.FromImage(graphicImage);
			}
			int ctrlCount = pnlDrawingPanel.Controls.Count;
			for (int i = ctrlCount - 1; i>=0; i-- )
			{
				DrawingControl d = (DrawingControl)pnlDrawingPanel.Controls[i];	
				g.TranslateTransform(d.Left, d.Top);
				d.PaintGraphics(g,forStoring?2.0F:1.0F);
				g.TranslateTransform(-d.Left, -d.Top);
			}
            g.Dispose();
			//graphicImage.Save(@"c:\test1.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
			return graphicImage;
		}

		//private void printDocument1_PrintPage(Object sender, PrintPageEventArgs e)
		//{
		//   int x = e.MarginBounds.Left;
		//   int y = e.MarginBounds.Top;

		//   e.Graphics.DrawString("Hello World",
		//      Font,new SolidBrush(Color.Black),
		//      new PointF(x,y));
		//   y += 15;
		//   e.Graphics.DrawImage(graphicImage, new Point(x, y));
		//}

		private void previewToolStripButton_Click(object sender, EventArgs e)
		{
            if (pnlDrawingPanel.BackgroundImage == null)
            {
                MessageBox.Show("A background image must be set before the graphic can be previewed", "Factotum");
                return;
            }
            deselectAllDrawingControls();
			PreviewGraphic pv = new	PreviewGraphic();
			pv.SetPreviewImage(CaptureScreen(false));
			pv.ShowDialog();
		}
		private void tsbExport_Click(object sender, EventArgs e)
		{
            if (pnlDrawingPanel.BackgroundImage == null)
            {
                MessageBox.Show("A background image must be set before the graphic can be exported","Factotum");
                return;
            }
			deselectAllDrawingControls();
			Image img = CaptureScreen(true);
			saveFileDialog1.Filter = "Jpeg Files|*.jpg";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				img.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			}
		}

		private void btnSaveGraphic_Click(object sender, EventArgs e)
		{
			int i = 0;
			deselectAllDrawingControls();

            if (isImageNotFound)
            {
                if (MessageBox.Show("Are you sure that you want to save the current graphic?  If so, the original background image file name will be lost!",
                    "Factotum", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            }
            if (isImageAtDefaultPath)
            {
                if (MessageBox.Show("Would you like to update the location of the background image to the current Default Image Folder?",
                    "Factotum", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    curGraphic.GraphicBgImageFileName = SubstituteDefaultImagePath(curGraphic.GraphicBgImageFileName);
                }
            }
            curGraphic.GraphicImage = CaptureScreen(true);

			curGraphic.Save();

			EDrawingControl.DeleteAllForGraphic((Guid)curGraphic.ID);
			foreach (DrawingControl dctl in pnlDrawingPanel.Controls)
			{
				EDrawingControl eDrawingControl = new EDrawingControl();
				eDrawingControl.DrawingControlGphID = curGraphic.ID;
				eDrawingControl.DrawingControlFillColor = dctl.FillColor.ToArgb();
				eDrawingControl.DrawingControlFontFamily = dctl.Font.FontFamily.Name;
				eDrawingControl.DrawingControlFontIsBold = dctl.Font.Bold;
				eDrawingControl.DrawingControlFontIsItalic = dctl.Font.Italic;
				eDrawingControl.DrawingControlFontIsUnderlined = dctl.Font.Underline;
				eDrawingControl.DrawingControlFontPoints = dctl.Font.SizeInPoints;
				eDrawingControl.DrawingControlHasFill = dctl.HasFill;
				eDrawingControl.DrawingControlHasStroke = dctl.HasStroke;
				eDrawingControl.DrawingControlHasText = dctl.HasText;
				eDrawingControl.DrawingControlHasTranspBackground = dctl.HasTransparentBackground;
				eDrawingControl.DrawingControlStroke = (int)dctl.Stroke;
				eDrawingControl.DrawingControlStrokeColor = dctl.StrokeColor.ToArgb();
				eDrawingControl.DrawingControlText = dctl.Text;
				eDrawingControl.DrawingControlTextColor = dctl.TextColor.ToArgb();
				eDrawingControl.DrawingControlZindex = i;
				if (dctl is Arrow)
					eDrawingControl.DrawingControlType = (byte)EDrawingControlType.Arrow;
				else if (dctl is Notation)
					eDrawingControl.DrawingControlType = (byte)EDrawingControlType.Notation;
				else if (dctl is Boundary)
					eDrawingControl.DrawingControlType = (byte)EDrawingControlType.Boundary;
				else
					throw new Exception("Unexpected drawing control type");
				
				Guid? drawingControlID = eDrawingControl.Save();
				if (drawingControlID == null)
				{
					MessageBox.Show("Unable to save graphic.  Contact support.", "Factotum");
				}
				if (dctl is Arrow)
				{
					Arrow arrow = (Arrow)dctl;
					EArrow eArrow = new EArrow();
					eArrow.ArrowBarb = (int)arrow.Barb;
					eArrow.ArrowDctID = drawingControlID;
					eArrow.ArrowEndX = (int)arrow.GEndPoint.X;
					eArrow.ArrowEndY = (int)arrow.GEndPoint.Y;
					eArrow.ArrowHeadCount = (byte)arrow.HeadCount;
					eArrow.ArrowShaftWidth = (int)arrow.ShaftWidth;
					eArrow.ArrowStartX = (int)arrow.GStartPoint.X;
					eArrow.ArrowStartY = (int)arrow.GStartPoint.Y;
					eArrow.ArrowTip = (int)arrow.Tip;
					eArrow.Save();
				}
				else if (dctl is Notation)
				{
					Notation notation = (Notation)dctl;
					ENotation eNotation = new ENotation();
					eNotation.NotationDctID = drawingControlID;
					eNotation.NotationHeight = notation.Height;
					eNotation.NotationLeft = notation.Left;
					eNotation.NotationTop = notation.Top;
					eNotation.NotationWidth = notation.Width;
					eNotation.Save();
				}
				else if (dctl is Boundary)
				{
					Boundary boundary = (Boundary)dctl;
					EBoundary eBoundary = new EBoundary();
					eBoundary.BoundaryAlpha = (byte)boundary.AlphaLevel;
					eBoundary.BoundaryDctID = drawingControlID;
					Guid? boundaryID = eBoundary.Save();
					if (boundaryID == null)
					{
						MessageBox.Show("Unable to save graphic.  Please contact support.", "Factotum");
					}
					EBoundaryPoint.SaveForBoundaryFromArray((Guid)boundaryID, boundary.GBoundaryPoints);
				}
				i++;
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void GraphicEdit_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Free up the background image file in case the user wants to edit it.
            if (pnlDrawingPanel.BackgroundImage != null)
                pnlDrawingPanel.BackgroundImage = null;
		}

	}
}