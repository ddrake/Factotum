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
	public partial class Notation : DrawingControl
	{
		//*******************************************************
		// Private fields
		//*******************************************************

		// These are set in MouseDown, reset in MouseUp and used in MouseMove
		private bool isMoving = false;
		private bool isTopLeftChanging = false;
		private bool isLeftChanging = false;
		private bool isBottomLeftChanging = false;
		private bool isBottomChanging = false;
		private bool isBottomRightChanging = false;
		private bool isRightChanging = false;
		private bool isTopRightChanging = false;
		private bool isTopChanging = false;
		private const int Delta = adornmentSize/2;

		// The rectangles used to draw the handles and detect mouse clicks
		// when the control is selected
		private Rectangle topLeftAdornment;
		private Rectangle leftAdornment;
		private Rectangle bottomLeftAdornment;
		private Rectangle topRightAdornment;
		private Rectangle rightAdornment;
		private Rectangle bottomRightAdornment;
		private Rectangle topAdornment;
		private Rectangle bottomAdornment;

		// This is used by the mouse move logic.
		// For the case of dragging the move adornment, it is simply set
		// when the mouse button is pressed.
		// For the case of dragging the start or end adornments, it is recalculated
		// each time the mouse moves to maintain its same relative position in the new 
		// frame of reference.
		private Point lastDown;

		//*******************************************************
		// Public Properties
		//*******************************************************
		public override bool HasTransparentBackground
		{
			get { return (!HasFill && !HasStroke); }
			set
			{
				HasFill = !value;
				HasStroke = !value;
			}
		}
		//*******************************************************
		// Constructors
		//*******************************************************

		// Creates a default notation with its start point at 100,100 in the 
		// parent container reference frame.
		public Notation() : this(new Point(100, 100)) { }

		// Creates a default arrow at the specified start point in the 
		// parent container reference frame.
		public Notation(Point location)
			:
			this(location, 2, "FLOW", Color.Yellow, Color.Red, Color.Black, false, DefaultFont) { }

		// Creates a notation with the specified attributes at the location
		// specified in the parent container reference frame.
		// New notations will generally be added to the container in two steps.
		// In the first step, the user clicks the form and a notation with default size
		// is created at the location specified.
		// In the second step, the user resizes the notation as desired.
		public Notation(Point location, float stroke, string text, 
			Color fillColor, Color textColor, Color strokeColor, 
			bool hasTransparentBackground, Font font)
			:
			this(location, new Size(150,100), stroke, text,
			fillColor, textColor, strokeColor, hasTransparentBackground,font) { }

		// Creates a notation with the specified attributes with both start and end points
		// specified in the parent container reference frame.
		public Notation(Point location, Size size, float stroke, string text,
			Color fillColor, Color textColor, Color strokeColor,
			bool hasTransparentBackground, Font font)
		{
			// This doesn't seem to help
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, true);
			BackColor = Color.Transparent;
			// Adjust location and size.  We need the control surface to be a little bigger than
			// the actual notation to make room for the handles.
			//Location = new Point(location.X-Delta, location.Y-Delta);
			//Size = new Size(size.Width+2*Delta, size.Height+2*Delta);
			Location = location;
			Size = size;
			Stroke = stroke;
			Text = text;
			TextColor = textColor;
			Font = font;
			StrokeColor = strokeColor;
			FillColor = fillColor;
			HasTransparentBackground = hasTransparentBackground;
			HasText = true;
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

		// This can be called either by the Paint event of the control or by an external 
		// function.  If it's called externally we need to specify the location where we
		// want the painting performed.
		public override void PaintGraphics(Graphics g, float scaleFactor)
		{
			// Note, antialiasing only applies to paths, not region fills, so the path must
			// be drawn last.
			g.SmoothingMode = SmoothingMode.AntiAlias;
			float halfStroke = HasStroke ? Stroke / 2 : 0;
			// Fill the interior
			RectangleF r = new RectangleF(Delta+halfStroke, Delta+halfStroke, 
				Width - 2 * (Delta+halfStroke), Height - 2 * (Delta+halfStroke));
			if (HasFill)
			{
				// Draw the stroke
				g.FillRegion(new SolidBrush(FillColor), new Region(r));
			}
			if (HasStroke)
			{
				// Draw the stroke
				g.DrawRectangle(new Pen(new SolidBrush(StrokeColor), Stroke),r.X,r.Y,r.Width,r.Height);
			}
			// If we have some text to draw 
			if (Text.Length > 0)
			{
				// back up the current font graphics unit.
				Font fnt = new Font(Font.FontFamily, this.Font.Size * scaleFactor,
					this.Font.Style);

				SizeF fntSize = g.MeasureString(Text, fnt);
				PointF txtLocation;
				// Draw the text. 
				txtLocation = new PointF((Width - fntSize.Width) / 2, (Height - fntSize.Height) / 2);
				g.DrawString(Text, fnt, new SolidBrush(TextColor), txtLocation);
			}

			if (IsSelected)
			{
				// Reset the transforms because we're using absolute coords now.
				CalcAdornments();
				ControlPaint.DrawGrabHandle(g, topLeftAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, bottomLeftAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, topRightAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, bottomRightAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, leftAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, rightAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, topAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, bottomAdornment, true, true);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			PaintGraphics(g,1.0F);
			ShapeControl();
		}

		// Shape the control, by creating the region that bounds it and setting the 
		// Region property of the control to it.
		private void ShapeControl()
		{
			Rectangle r = new Rectangle(Delta, Delta, Width - 2 * Delta, Height - 2 * Delta);
			GraphicsPath gp = new GraphicsPath();

			// Add the arrow boundary points
			gp.AddRectangle(r);

			Region rgn = new Region(gp);
			// If the control is selected, we need to union in the handles
			if (IsSelected)
			{
				rgn.Union(topLeftAdornment);
				rgn.Union(bottomLeftAdornment);
				rgn.Union(topRightAdornment);
				rgn.Union(bottomRightAdornment);
				rgn.Union(leftAdornment);
				rgn.Union(rightAdornment);
				rgn.Union(topAdornment);
				rgn.Union(bottomAdornment);
			}
			// Set the control's region property
			this.Region = rgn;

		}

		// Calculate the three adornments (grab handle) rectangles for the control
		private void CalcAdornments()
		{
			topLeftAdornment = CalcAdornment(new PointF(Delta, Delta));
			bottomLeftAdornment = CalcAdornment(new PointF(Delta, Height-Delta));
			topRightAdornment = CalcAdornment(new PointF(Width-Delta, Delta));
			bottomRightAdornment = CalcAdornment(new PointF(Width - Delta, Height - Delta));
			leftAdornment = CalcAdornment(new PointF(Delta, Height / 2));
			rightAdornment = CalcAdornment(new PointF(Width - Delta, Height / 2));
			topAdornment = CalcAdornment(new PointF(Width / 2, Delta));
			bottomAdornment = CalcAdornment(new PointF(Width / 2, Height - Delta));
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
			Focus();
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
		// If the control was selected, check for mouse down inside one of the 
		// drag handles.
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (IsSelected)
			{
				if (topLeftAdornment.Contains(e.Location)) isTopLeftChanging = true;
				else if (leftAdornment.Contains(e.Location)) isLeftChanging = true;
				else if (bottomLeftAdornment.Contains(e.Location)) isBottomLeftChanging = true;
				else if (bottomAdornment.Contains(e.Location)) isBottomChanging = true;
				else if (bottomRightAdornment.Contains(e.Location)) isBottomRightChanging = true;
				else if (rightAdornment.Contains(e.Location)) isRightChanging = true;
				else if (topRightAdornment.Contains(e.Location)) isTopRightChanging = true;
				else if (topAdornment.Contains(e.Location)) isTopChanging = true;
				else isMoving = true;
				lastDown = new Point(e.X, e.Y);
			}
		}
		// This handles moving the control and changing its endpoints.
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (IsSelected)
			{
				if (topLeftAdornment.Contains(e.Location) || bottomRightAdornment.Contains(e.Location))
				{
					Cursor = StretchCursor;
				}
				else if (bottomLeftAdornment.Contains(e.Location) || topRightAdornment.Contains(e.Location))
				{
					Cursor = StretchCursor;
				}
				else if (topAdornment.Contains(e.Location) || bottomAdornment.Contains(e.Location))
				{
					Cursor = StretchCursor;
				}
				else if (leftAdornment.Contains(e.Location) || rightAdornment.Contains(e.Location))
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
					Left += e.X - lastDown.X;
					Top += e.Y - lastDown.Y;
				}
				else if (isTopLeftChanging)
				{
					Left += e.X - lastDown.X;
					Top += e.Y - lastDown.Y;
					Width -= e.X - lastDown.X;
					Height -= e.Y - lastDown.Y;
					Invalidate();
				}
				else if (isLeftChanging)
				{
					Left += e.X - lastDown.X;
					Width -= e.X - lastDown.X;
					Invalidate();
				}
				else if (isBottomLeftChanging)
				{
					Left += e.X - lastDown.X;
					Width -= e.X - lastDown.X;
					Height += e.Y - lastDown.Y;
					lastDown.Y = e.Y;
					Invalidate();
				}
				else if (isBottomChanging)
				{
					Height += e.Y - lastDown.Y;
					lastDown.Y = e.Y;
					Invalidate();
				}
				else if (isBottomRightChanging)
				{
					Width += e.X - lastDown.X;
					Height += e.Y - lastDown.Y;
					lastDown = new Point(e.X, e.Y);
					Invalidate();
				}
				else if (isRightChanging)
				{
					Width += e.X - lastDown.X;
					lastDown.X = e.X;
					Invalidate();
				}
				else if (isTopRightChanging)
				{
					Top += e.Y - lastDown.Y;
					Width += e.X - lastDown.X;
					Height -= e.Y - lastDown.Y;
					lastDown.X = e.X;
					Invalidate();
				}
				else if (isTopChanging)
				{
					Top += e.Y - lastDown.Y;
					Height -= e.Y - lastDown.Y;
					Invalidate();
				}
			}
		}

		// Reset all drag state flags
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			isMoving = false;
			isTopLeftChanging = false;
			isLeftChanging = false;
			isBottomLeftChanging = false;
			isBottomChanging = false;
			isBottomRightChanging = false;
			isRightChanging = false;
			isTopRightChanging = false;
			isTopChanging = false;
		}

		// Return the midpoint between two given points
		private PointF getMidpoint(PointF p1, PointF p2)
		{
			return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}

	}
}
