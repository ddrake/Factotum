using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	// A custom control that acts as a graphic design element.  It represents an
	// arrow that is selectable, draggable and whose start and endpoints can be dragged.
	// The user can specify physical characteristics like the shaft width, the barb offset,
	// the tip length, as well as design properties like the stroke color and width, 
	// the fill color, the text color and the text
	public partial class Boundary : DrawingControl
	{
		//*******************************************************
		// Private fields
		//*******************************************************
		private PointF[] gBoundaryPoints;
		private PointF[] boundaryPoints;

		private const int DefBoundPoints = 12;
		private const int DefSize = 75;
		private const int Delta = 20;
		// These are set in MouseDown, reset in MouseUp and used in MouseMove
		private bool isMoving = false;
		private bool[] isChanging;
		private int nBoundPoints;
		// The rectangles used to draw the handles and detect mouse clicks
		// when the control is selected
		private Rectangle[] adornments;

		// This is used by the mouse move logic.
		// For the case of dragging the move adornment, it is simply set
		// when the mouse button is pressed.
		// For the case of dragging the start or end adornments, it is recalculated
		// each time the mouse moves to maintain its same relative position in the new 
		// frame of reference.
		private Point lastDown;
		private int chgPtIdx;
		bool mouseDownInAdornment = false;
		private int alphaLevel;

	
		//*******************************************************
		// Public Properties
		//*******************************************************

		// This isn't really used since we added alpha level...
		public override bool HasTransparentBackground
		{
			get { return (!HasFill); }
			set
			{
				HasFill = !value;
			}
		}

		public int AlphaLevel
		{
			get { return alphaLevel; }
			set
			{
				if (alphaLevel != value)
				{
					alphaLevel = value;
				}
			}
		}
		public PointF[] GBoundaryPoints
		{
			get { return gBoundaryPoints; }
		}

		//*******************************************************
		// Constructors
		//*******************************************************

		// Creates a default boundary with its start point at 100,100 in the 
		// parent container reference frame.
		public Boundary() : this(new PointF(100, 100)) { }

		// Creates a default boundary at the specified start point in the 
		// parent container reference frame.
		public Boundary(PointF gMidPoint)
			:
			this(new Point((int)(gMidPoint.X-DefSize/2),(int)(gMidPoint.Y-DefSize/2)),
			new Size(DefSize, DefSize), DefBoundPoints, 2, "", false, 
			Color.Yellow, Color.Red, Color.Black, DefaultFont,200){	}

		// Creates a boundary with the specified attributes with its start point
		// specified in the parent container reference frame.
		// New boundaries will generally be added to the container in two steps.
		// In the first step, the user clicks the form and a circular boundary
		// is created with its center at the coordinates the user specified.
		// In the second step, the user drags the the boundary to move it, 
		// Shift-drags any handle to resize it, or drags individual handles as desired.
		public Boundary(Point gLocation, Size size, int nPoints, 
			float stroke, string text, bool hasText,
			Color fillColor, Color textColor, Color strokeColor, Font font, int opacity)
			:
			this(GetDefaultBoundaryPts(gLocation, size, nPoints), 
			stroke, text, hasText,
			fillColor, textColor, strokeColor,font,opacity){ }

		// Creates a boundary with the specified attributes with its point array
		// specified in the parent container reference frame.
		public Boundary(PointF[] gBoundaryPts, float stroke, string text, bool hasText,
			Color fillColor, Color textColor, Color strokeColor, Font font, int opacity)
		{
			// OptimizedDoubleBuffer doesn't seem to help
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, true);
			BackColor = Color.Transparent;
			Stroke = stroke;
			Text = text;
			HasText = hasText;
			HasFill = true;
			HasStroke = true;
			AlphaLevel = opacity;
			StrokeColor = strokeColor;
			FillColor = fillColor; 
			TextColor = textColor;
			Font = font;
			this.nBoundPoints = gBoundaryPts.Length;
			this.gBoundaryPoints = gBoundaryPts;
			this.boundaryPoints = new PointF[nBoundPoints];
			this.adornments = new Rectangle[nBoundPoints];
			this.isChanging = new bool[nBoundPoints];
			SetDimensionsFromGlobalBoundaryPts();
			try
			{
				string path;
				path = Application.StartupPath + "\\move.cur";
				MoveCursor = new Cursor(path);
				path = Application.StartupPath + "\\stretch.cur";
				StretchCursor = new Cursor(path);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to set custom cursors", ex);
			} 
		}

		// Fills the array with boundary points around a circular circumference
		private static PointF[] GetDefaultBoundaryPts(Point gLocation, Size size, int nPoints)
		{
			PointF[] boundaryPoints = new PointF[nPoints];
			float angle = 0;
			float radius = Math.Min(size.Width, size.Height);
			PointF center = new PointF(gLocation.X + size.Width / 2, gLocation.Y + size.Height / 2);
			for (int i = 0; i < nPoints; i++)
			{
				PointF pt = new PointF(center.X + radius * (float)Math.Cos(angle),
					center.Y + radius * (float)Math.Sin(angle));
				angle += 2 * (float)Math.PI / nPoints;
				boundaryPoints[i] = pt;
			}
			return boundaryPoints;
		}
		
		// If the global boundary points are defined, we can calculate
		// all the other dimensions of the control
		private void SetDimensionsFromGlobalBoundaryPts()
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			foreach (PointF p in gBoundaryPoints)
			{
				if (p.X < minX) minX = p.X;
				if (p.Y < minY) minY = p.Y;
				if (p.X > maxX) maxX = p.X;
				if (p.Y > maxY) maxY = p.Y;
			}
			Top = (int)minY - Delta;
			Left = (int)minX - Delta;
			Width = (int)(maxX - minX + 2 * Delta);
			Height = (int)(maxY - minY + 2 * Delta);
			for (int i = 0; i < boundaryPoints.Length; i++ )
			{
				boundaryPoints[i] = new PointF(gBoundaryPoints[i].X - Left, gBoundaryPoints[i].Y - Top);
			}
		}

		// This can be called either by the Paint event of the control or by an external 
		// function.  If it's called externally we need to specify the location where we
		// want the painting performed.
		public override void PaintGraphics(Graphics g, float scaleFactor)
		{
			// Note: antialiasing only applies to paths, not region fills, so the path must
			// be drawn last.
			g.SmoothingMode = SmoothingMode.AntiAlias;

			GraphicsPath gp = new GraphicsPath();
			// Add the boundary points
			gp.AddClosedCurve(boundaryPoints, 0.5F);

			// Fill the interior
			Color c = Color.FromArgb(alphaLevel, FillColor);
			g.FillRegion(new SolidBrush(c), new Region(gp));
			//g.FillRegion(new SolidBrush(FillColor), new Region(gp));

			if (HasStroke)
			{
				// Draw the stroke
				g.DrawPath(new Pen(new SolidBrush(StrokeColor), Stroke), gp);
			}
			gp.Dispose();

			if (IsSelected)
			{
				// Reset the transforms because we're using absolute coords now.
				CalcAdornments();
				DrawAdornments(g);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			PaintGraphics(g,1.0F);

			// Set the control's Region property to include the painted region plus a little.
			ShapeControl();
		}

		private void DrawAdornments(Graphics g)
		{
			foreach (Rectangle adorn in adornments)
			{
				ControlPaint.DrawGrabHandle(g, adorn, true, true);
			}
		}

		// Shape the control, by creating the region that bounds it and setting the 
		// Region property of the control to it.
		private void ShapeControl()
		{
			GraphicsPath gp = new GraphicsPath();

			// Add the arrow boundary points
			gp.AddClosedCurve(boundaryPoints);

			Region rgn = new Region(gp);
			// If the control is selected, we need to union in the handles
			if (IsSelected)
			{
				foreach (Rectangle adorn in adornments)
				{
					rgn.Union(adorn);
				}
			}
			// Set the control's region property
			this.Region = rgn;

		}
		// Calculate the three adornments (grab handle) rectangles for the control
		private void CalcAdornments()
		{
			for (int i = 0; i < boundaryPoints.Length; i++)
			{
				adornments[i] = CalcAdornment(boundaryPoints[i]);
			}
		}
		// Calculate an adornment (grab handle) rectangle, centered on the given point
		private Rectangle CalcAdornment(PointF pt)
		{
			return new Rectangle((int)(pt.X - adornmentSize / 2), (int)(pt.Y - adornmentSize / 2),
				adornmentSize, adornmentSize);
		}

		// If the control was clicked, make it selected by setting its property
		// and repainting to show the handles.
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			Focus(); // Set the focus even if it was already selected.
			if (!IsSelected)
			{
				IsSelected = true;
				Invalidate();
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if (IsSelected) Cursor = MoveCursor;
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Cursor = DefaultCursor;
		}

		private bool inAdornment(Point p)
		{
			bool result = false;
			for (int i = 0; i < adornments.Length; i++)
			{
				if (adornments[i].Contains(p))
				{
					isChanging[i] = true;
				}
			}
			return result;
		}

		// If the control was selected, check for mouse down inside one of the 
		// drag handles.
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (IsSelected)
			{
				for (int i = 0; i < adornments.Length; i++)
				{
					if (adornments[i].Contains(e.Location))
					{
						isChanging[i] = true;
						mouseDownInAdornment = true;
						chgPtIdx = i;
						break;
					}
				}
				if (!mouseDownInAdornment) isMoving = true;

				lastDown = new Point(e.X, e.Y);
			}	
		}

		// This handles moving the control and changing its endpoints.
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (IsSelected)
			{
				if (inAdornment(e.Location))
				{
					Cursor = StretchCursor;
				}
				else
				{
					Cursor = MoveCursor;
				}
			}
			if (e.Button == MouseButtons.Left)
			{
				if (isMoving)
				{
					for (int i = 0; i < gBoundaryPoints.Length; i++)
					{
						gBoundaryPoints[i] = new PointF(gBoundaryPoints[i].X + e.X - lastDown.X,
							gBoundaryPoints[i].Y + e.Y - lastDown.Y);
					}
					SetDimensionsFromGlobalBoundaryPts();
					
				}
				else if (mouseDownInAdornment)
				{
					// "errors" from center of target point
					float xe;
					float ye;
					// This is tricky because as we drag a start or end point, the position and
					// size of the control change.  
					// Also, the relative positions of the start and end point can change.  For
					// example the endpoint may be above and to the right of the startpoint before
					// the move, but end up to the left and below it.
					
					xe = lastDown.X - (gBoundaryPoints[chgPtIdx].X - Left);
					ye = lastDown.Y - (gBoundaryPoints[chgPtIdx].Y - Top);
					gBoundaryPoints[chgPtIdx] = new PointF(gBoundaryPoints[chgPtIdx].X + e.X - lastDown.X,
						gBoundaryPoints[chgPtIdx].Y + e.Y - lastDown.Y);

					SetDimensionsFromGlobalBoundaryPts();

					// Update lastDown because the frame of reference may have changed
					lastDown = new Point((int)(boundaryPoints[chgPtIdx].X + xe),
						(int)(boundaryPoints[chgPtIdx].Y + ye));
					Invalidate();
				}
			}
		}

		// Reset all drag state flags
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			isMoving = false;
			mouseDownInAdornment = false;
		}

		// Return the midpoint between two given points
		private PointF getMidpoint(PointF p1, PointF p2)
		{
			return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}

	}
}
