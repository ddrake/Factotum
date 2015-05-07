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
	public partial class Arrow : DrawingControl
	{
		//*******************************************************
		// Private fields
		//*******************************************************
		private PointF startPoint;
		private PointF endPoint;

		private float shaftWidth;	// the nominal shaft width. The stroke also affects the actual width
		private float halfWidth;	// half the nominal shaft width, more convenient to work with.
		private float barb;			// The barb offset
		private float tip;			// The tip length


		// These are set in MouseDown, reset in MouseUp and used in MouseMove
		private bool isMoving = false;
		private bool isStartChanging = false;
		private bool isEndChanging = false;

		// The rectangles used to draw the handles and detect mouse clicks
		// when the control is selected
		private Rectangle startAdornment;
		private Rectangle endAdornment;

		// This is used by the mouse move logic.
		// For the case of dragging the move adornment, it is simply set
		// when the mouse button is pressed.
		// For the case of dragging the start or end adornments, it is recalculated
		// each time the mouse moves to maintain its same relative position in the new 
		// frame of reference.
		private Point lastDown;

		private PointF gStartPoint;
		private PointF gEndPoint;

		private int headCount;
	
		//*******************************************************
		// Public Properties
		//*******************************************************

		// Physical attributes
		public float ShaftWidth
		{
			get { return shaftWidth; }
			set 
			{
				if (shaftWidth != value)
				{
					shaftWidth = value;
					halfWidth = shaftWidth / 2;
				}
			}
		}
		public float Barb
		{
			get { return barb; }
			set
			{
				if (barb != value)
				{
					barb = value;
				}
			}
		}
		public float Tip
		{
			get { return tip; }
			set
			{
				if (tip != value)
				{
					tip = value;
				}
			}
		}

		public PointF GStartPoint
		{
			get { return gStartPoint; }
			set
			{
				if (gStartPoint != value)
				{
					gStartPoint = value;
					if (!gEndPoint.IsEmpty && !gStartPoint.IsEmpty)
					{
						SetDimensionsFromGlobalStartAndEndPoints();
					}
				}
			}
		}
		public PointF GEndPoint
		{
			get { return gEndPoint; }
			set
			{
				if (gEndPoint != value)
				{
					gEndPoint = value;
					if (!gEndPoint.IsEmpty && !gStartPoint.IsEmpty)
					{
						SetDimensionsFromGlobalStartAndEndPoints();
					}
				}
			}
		}

		public override bool HasTransparentBackground
		{
			get { return (!HasFill); }
			set
			{
				HasFill = !value;
			}
		}
		public int HeadCount
		{
			get { return headCount; }
			set { headCount = value; }
		}

		//*******************************************************
		// Constructors
		//*******************************************************

		// Creates a default arrow with its start point at 100,100 in the 
		// parent container reference frame.
		public Arrow() : this(new PointF(100, 100)) { }

		// Creates a default arrow at the specified start point in the 
		// parent container reference frame.
		public Arrow(PointF gStartPoint)
			:
			this(gStartPoint,1, 12F, 6F, 20F, 2F, "FLOW", true,
			Color.Yellow, Color.Red, Color.Black, false, DefaultFont){	}

		// Creates an arrow with the specified attributes with its start point
		// specified in the parent container reference frame.
		// New arrows will generally be added to the container in two steps.
		// In the first step, the user clicks the form and an arrow with default length
		// is created with its start point at the coordinates the user specified.
		// In the second step, the user drags the endpoint of the arrow as desired.
		public Arrow(PointF gStartPoint, int headCount, float shaftWidth, 
			float barb, float tip, float stroke, string text, bool hasText,
			Color fillColor, Color textColor, Color strokeColor,
			bool hasTransparentBackground, Font font)
			:
			this(gStartPoint, new PointF(gStartPoint.X + 100, gStartPoint.Y), 
			headCount, shaftWidth, barb, tip, stroke, text, hasText,
			fillColor, textColor, strokeColor, hasTransparentBackground, font) { }

		// Creates an arrow with the specified attributes with both start and end points
		// specified in the parent container reference frame.
		public Arrow(PointF gStartPoint, PointF gEndPoint, int headCount, float shaftWidth, 
			float barb, float tip, float stroke, string text, bool hasText,
			Color fillColor, Color textColor, Color strokeColor,
			bool hasTransparentBackground, Font font)
		{
			// OptimizedDoubleBuffer doesn't seem to help
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, true);
			BackColor = Color.Transparent;
			HeadCount = headCount;
			ShaftWidth = shaftWidth;
			Barb = barb;
			Tip = tip;
			Stroke = stroke;
			Text = text;
			HasText = hasText;
			HasTransparentBackground = hasTransparentBackground;
			HasStroke = true;

			StrokeColor = strokeColor;
			FillColor = fillColor; 
			TextColor = textColor;
			Font = font;
			GStartPoint = gStartPoint;
			GEndPoint = gEndPoint;
			SetDimensionsFromGlobalStartAndEndPoints();
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
		// If gStartPoint and gEndPoint are defined, we can calculate
		// all the other dimensions of the control
		private void SetDimensionsFromGlobalStartAndEndPoints()
		{
			float delta = getDelta();
			float minx = (int)Math.Min(gStartPoint.X, gEndPoint.X);
			float miny = (int)Math.Min(gStartPoint.Y, gEndPoint.Y);
			Top = (int)(miny - delta);
			Left = (int)(minx - delta);
			startPoint = new PointF(gStartPoint.X - minx + delta, gStartPoint.Y - miny + delta);
			endPoint = new PointF(gEndPoint.X - minx + delta, gEndPoint.Y - miny + delta);
			Width = (int)(Math.Abs(gStartPoint.X - gEndPoint.X) + 2 * delta);
			Height = (int)(Math.Abs(gStartPoint.Y - gEndPoint.Y) + 2 * delta);		
		}

		// This can be called either by the Paint event of the control or by an external 
		// function.  If it's called externally we need to specify the location where we
		// want the painting performed.
		public override void PaintGraphics(Graphics g, float scaleFactor)
		{
			// Note: antialiasing only applies to paths, not region fills, so the path must
			// be drawn last.
			g.SmoothingMode = SmoothingMode.AntiAlias;
			// The distance from the startpoint to the endpoint (where the barb begins)
			float length = getDistance(startPoint, endPoint);
			// The absolute angle of the vector from startPoint to endPoint
			float angle = getAngle(startPoint, endPoint);
			GraphicsPath gp = new GraphicsPath();
			// Translate and rotate to put the startPoint at the origin and the endpoint on 
			// the positive x axis
			g.TranslateTransform(startPoint.X, startPoint.Y);
			g.RotateTransform(angle);
			float halfStroke = HasStroke ? Stroke / 2 : 0;
			switch (headCount)
			{
				case 0:
					// Add the arrow boundary points
					// In this simple case it's really just a rectangle...
					gp.AddPolygon(new PointF[] {
						new PointF(halfStroke,halfStroke),
						new PointF(halfStroke,halfWidth-halfStroke),
						new PointF(length,halfWidth-halfStroke),
						new PointF(length, -(halfWidth-halfStroke)),
						new PointF(halfStroke,-(halfWidth-halfStroke))
					});
					break;

				case 1:
					// Add the arrow boundary points
					gp.AddPolygon(new PointF[] {
						new PointF(halfStroke,halfStroke),
						new PointF(halfStroke,halfWidth-halfStroke),
						new PointF(length,halfWidth-halfStroke),
						new PointF(length,halfWidth+barb-halfStroke),
						new PointF(length+tip-halfStroke,0),
						new PointF(length,-(halfWidth+barb-halfStroke)),
						new PointF(length, -(halfWidth-halfStroke)),
						new PointF(halfStroke,-(halfWidth-halfStroke))
					});
					break;

				case 2:
					// Add the arrow boundary points
					gp.AddPolygon(new PointF[] {
						new PointF(0,halfWidth-halfStroke),
						new PointF(length,halfWidth-halfStroke),
						new PointF(length,halfWidth+barb-halfStroke),
						new PointF(length+tip-halfStroke,0),
						new PointF(length,-(halfWidth+barb-halfStroke)),
						new PointF(length, -(halfWidth-halfStroke)),
						new PointF(0,-(halfWidth-halfStroke)),
						new PointF(0,-(halfWidth+barb-halfStroke)),
						new PointF(-(tip-halfStroke),0),
						new PointF(0,halfWidth+barb-halfStroke)
					});
					break;

				default:
					throw new Exception("An arrow must have zero, one or two heads.");
			}
			if (HasFill)
			{
				// Fill the interior
				g.FillRegion(new SolidBrush(FillColor), new Region(gp));
			}
			if (HasStroke)
			{
				// Draw the stroke
				g.DrawPath(new Pen(new SolidBrush(StrokeColor), Stroke), gp);
			}
			gp.Dispose();

			// If we have some text to draw 
			if (HasText && Text.Length > 0)
			{
				float topPadding = 2;
				Font fnt = new Font(Font.FontFamily, 2 * halfWidth - Stroke - topPadding,
					Font.Style, GraphicsUnit.Pixel);
				SizeF fntSize = g.MeasureString(Text, fnt);
				PointF txtLocation;
				// Draw the text. If the angle is pointing to the left, rotate 180 degrees first
				// so the text isn't upside down.
				if (angle <= 90 && angle >= -90)
				{
					txtLocation = new PointF((length - fntSize.Width) / 2, (-fntSize.Height / 2) + topPadding);
					g.DrawString(Text, fnt, new SolidBrush(TextColor), txtLocation);
				}
				else
				{
					g.RotateTransform(180);
					txtLocation = new PointF((-length - fntSize.Width) / 2, (-fntSize.Height / 2) + topPadding);
					g.DrawString(Text, fnt, new SolidBrush(TextColor), txtLocation);
					g.RotateTransform(-180);
				}

				fnt.Dispose();
			}

			g.RotateTransform(-angle);
			g.TranslateTransform(-startPoint.X, -startPoint.Y);

			if (IsSelected)
			{
				// Reset the transforms because we're using absolute coords now.
				CalcAdornments();
				ControlPaint.DrawGrabHandle(g, startAdornment, true, true);
				ControlPaint.DrawGrabHandle(g, endAdornment, true, true);
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

		// See the geometry notes for these two functions
		// Get the additional distance to the tip for a given "stroke half-width" s
		private float getTipOffset(float s)
		{
			return (float)(s * Math.Sqrt(1 + Math.Pow(tip / (halfWidth + barb), 2)));
		}
		// Get the additional distance to the outer edge of the barb 
		// for a given tipOffset and "stroke half-width" s
		private float getBarbOffset(float tipOffset, float s)
		{
			float dy = barb + halfWidth;
			return dy * ((s + tipOffset + tip) / tip - 1);
		}

		// Shape the control, by creating the region that bounds it and setting the 
		// Region property of the control to it.
		private void ShapeControl()
		{
			GraphicsPath gp = new GraphicsPath();
			float length = getDistance(startPoint, endPoint);
			// may want to add an extra pixel to the half-stroke width for better smoothing.
			float s = Stroke / 2; 
			float tipOffset = getTipOffset(s);
			float barbOffset = getBarbOffset(tipOffset,s);

			// Add the boundary points

			switch (headCount)
			{
				case 0:
					// Add the arrow boundary points
					// In this simple case it's really just a rectangle...
					gp.AddPolygon(new PointF[] {
					new PointF(-s,halfWidth+s),
					new PointF(length-s,halfWidth+s),
					new PointF(length-s,-halfWidth-s),
					new PointF(-s,-halfWidth-s)});
					break;

				case 1:
					// Add the arrow boundary points
					gp.AddPolygon(new PointF[] {
					new PointF(-s,0),
					new PointF(-s,halfWidth+s),
					new PointF(length-s,halfWidth+s),
					new PointF(length-s,halfWidth+barb+barbOffset),
					new PointF(length+tip+tipOffset,0),
					new PointF(length-s,-halfWidth-barb-barbOffset),
					new PointF(length-s,-halfWidth-s),
					new PointF(-s,-halfWidth-s)});
					break;

				case 2:
					// Add the arrow boundary points
					gp.AddPolygon(new PointF[] {
						new PointF(s,halfWidth+s),
						new PointF(length-s,halfWidth+s),
						new PointF(length-s,halfWidth+barb+barbOffset),
						new PointF(length+tip+tipOffset,0),
						new PointF(length-s,-halfWidth-barb-barbOffset),
						new PointF(length-s,-halfWidth-s),
						new PointF(s,-halfWidth-s),
						new PointF(s,-halfWidth-barb-barbOffset),
						new PointF(-tip-tipOffset,0),
						new PointF(s,halfWidth+barb+barbOffset)				
					});
					break;

				default:
					throw new Exception("An arrow must have zero, one or two heads.");
			}



			// We're not painting anything, so we don't need a graphics.
			// Use a matrix to transform.
			Matrix mat = new Matrix();
			mat.Translate(startPoint.X, startPoint.Y);
			mat.Rotate(getAngle(startPoint, endPoint));
			gp.Transform(mat);

			Region rgn = new Region(gp);
			// If the control is selected, we need to union in the handles
			if (IsSelected)
			{
				rgn.Union(startAdornment);
				rgn.Union(endAdornment);
			}
			// Set the control's region property
			this.Region = rgn;

		}
		// Calculate the three adornments (grab handle) rectangles for the control
		private void CalcAdornments()
		{
			startAdornment = CalcAdornment(startPoint);
			endAdornment = CalcAdornment(endPoint);
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
		// If the control was selected, check for mouse down inside one of the 
		// drag handles.
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (IsSelected)
			{
				if (startAdornment.Contains(e.Location)) isStartChanging = true;
				else if (endAdornment.Contains(e.Location)) isEndChanging = true;
				else isMoving = true;

				lastDown = new Point(e.X, e.Y);
			}	
		}
#if true
		// This handles moving the control and changing its endpoints.
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (IsSelected)
			{
				if (startAdornment.Contains(e.Location) || endAdornment.Contains(e.Location))
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
					gStartPoint = new PointF(gStartPoint.X + e.X - lastDown.X,
						gStartPoint.Y + e.Y - lastDown.Y);
					GEndPoint = new PointF(gEndPoint.X + e.X - lastDown.X,
						gEndPoint.Y + e.Y - lastDown.Y);

					
					//Left += e.X - lastDown.X;
					//Top += e.Y - lastDown.Y;
				}
				else if (isStartChanging || isEndChanging)
				{
					// "errors" from center of target point
					float xe;
					float ye;
					// This is tricky because as we drag a start or end point, the position and
					// size of the control change.  
					// Also, the relative positions of the start and end point can change.  For
					// example the endpoint may be above and to the right of the startpoint before
					// the move, but end up to the left and below it.
					if (isStartChanging)
					{
						xe = lastDown.X - startPoint.X;
						ye = lastDown.Y - startPoint.Y;
						// Setting this property automatically updates all the other dimensions.
						GStartPoint = new PointF(gStartPoint.X + e.X - lastDown.X, 
							gStartPoint.Y + e.Y - lastDown.Y);
						// Update lastDown because the frame of reference may have changed
						lastDown = new Point((int)(startPoint.X + xe),(int)(startPoint.Y+ye));
					}
					else // endpoint must be changing
					{
						xe = lastDown.X - endPoint.X;
						ye = lastDown.Y - endPoint.Y;
						// Setting this property automatically updates all the other dimensions.
						GEndPoint = new PointF(gEndPoint.X + e.X - lastDown.X, 
							gEndPoint.Y + e.Y - lastDown.Y);
						// Update lastDown because the frame of reference may have changed
						lastDown = new Point((int)(endPoint.X + xe),(int)(endPoint.Y+ye));
					}
					Invalidate();
				}
			}
		}
#endif

#if false
		// This handles moving the control and changing its endpoints.
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (IsSelected)
			{
				if (startAdornment.Contains(e.Location) || endAdornment.Contains(e.Location))
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
					System.Diagnostics.Debug.Print((e.X - lastDown.X) + " " + (e.Y - lastDown.Y));
					Left += e.X - lastDown.X;
					Top += e.Y - lastDown.Y;
				}
				else if (isStartChanging || isEndChanging)
				{
					// This is tricky because as we drag a start or end point, the position and
					// size of the control change.  
					// Also, the relative positions of the start and end point can change.  For
					// example the endpoint may be above and to the right of the startpoint before
					// the move, but end up to the left and below it.

					float p1xp, p1yp;		// New start point coords in the OLD coord system
					float p2xp, p2yp;		// New end point coords in the OLD coord system
					float p1xdp, p1ydp;	// New start point coords in the NEW coord system
					float p2xdp, p2ydp;	// New end point coords in the NEW coord system

					// The control area consists of the rectangle formed by the start and end points
					// offset by delta.  This tries to ensure that the control area will
					// always be large enough to contain the arrow head, barbs, and adornments.
					float delta = getDelta(); 

					float widthp, leftp; // New width and horizontal position of the control
					float heightp, topp; // New height and vertical position of the control

					if (isStartChanging)
					{
						p1xp = startPoint.X + e.X - lastDown.X;
						p1yp = startPoint.Y + e.Y - lastDown.Y;
						p2xp = endPoint.X;
						p2yp = endPoint.Y;
					}
					else
					{
						p2xp = endPoint.X + e.X - lastDown.X;
						p2yp = endPoint.Y + e.Y - lastDown.Y;
						p1xp = startPoint.X;
						p1yp = startPoint.Y;
					}

					// Get new X-related coords
					if (p1xp < p2xp)
					{
						p1xdp = delta;
						p2xdp = delta + p2xp - p1xp;
						widthp = 2 * delta + p2xp - p1xp;
						if (startPoint.X < endPoint.X)
							leftp = Left + p1xp - startPoint.X;
						else
							leftp = Left + p1xp - endPoint.X;
					}
					else
					{
						p2xdp = delta;
						p1xdp = delta + p1xp - p2xp;
						widthp = 2 * delta + p1xp - p2xp;
						if (endPoint.X < startPoint.X)
							leftp = Left + p2xp - endPoint.X;
						else
							leftp = Left + p2xp - startPoint.X;
					}

					// Get new Y-related coords
					if (p1yp < p2yp)
					{
						p1ydp = delta;
						p2ydp = delta + p2yp - p1yp;
						heightp = 2 * delta + p2yp - p1yp;
						if (startPoint.Y < endPoint.Y)
							topp = Top + p1yp - startPoint.Y;
						else
							topp = Top + p1yp - endPoint.Y;
					}
					else
					{
						p2ydp = delta;
						p1ydp = delta + p1yp - p2yp;
						heightp = 2 * delta + p1yp - p2yp;
						if (endPoint.Y < startPoint.Y)
							topp = Top + p2yp - endPoint.Y;
						else
							topp = Top + p2yp - startPoint.Y;
					}
					// Since the coordinate system may have changed, 
					// recalculate the "last down" point.  
					if (isStartChanging)
					{
						lastDown.X = (int)(p1xdp + lastDown.X - startPoint.X);
						lastDown.Y = (int)(p1ydp + lastDown.Y - startPoint.Y);
					}
					else
					{
						lastDown.X = (int)(p2xdp + lastDown.X - endPoint.X);
						lastDown.Y = (int)(p2ydp + lastDown.Y - endPoint.Y);
					}

					// Define the new start and end points
					startPoint = new PointF(p1xdp, p1ydp);
					endPoint = new PointF(p2xdp,p2ydp);

					Top = (int)topp;
					Left = (int)leftp;
					Width = (int)widthp;
					Height = (int)heightp;
					Invalidate();

				}
			}
		}
#endif
		// Reset all drag state flags
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			isMoving = false;
			isStartChanging = false;
			isEndChanging = false;
		}
		
		// The control area consists of the rectangle formed by the start and end points
		// offset by delta.  This tries to ensure that the control area will
		// always be large enough to contain the arrow head, barbs, and adornments.
		private float getDelta()
		{
			return Math.Max(Math.Max(Tip, halfWidth + Barb) + Stroke, adornmentSize);
		}

		// Calculate the absolute angle of a vector from pStart to pEnd
		// Angles returned should be in the range from -90 to +269.999
		// If the start and endpoint are the same, the angle is arbitrarily set to zero.
		private float getAngle(PointF pStart, PointF pEnd)
		{
			const float eps = float.Epsilon;
			float dx = pEnd.X - pStart.X;
			float dy = pEnd.Y - pStart.Y;
			float adx = Math.Abs(dx);
			float ady = Math.Abs(dy);
			if (adx <= eps)
			{
				if (ady <= eps) return 0; // arbitrarily set to zero
				else if (dy > eps) return 90;
				else return -90;
			}
			else if (dx > eps)
			{
				// Main branch of the arctangent function (-90 to 90)
				if (ady <= eps) return 0;
				else return (float)(180 / Math.PI * Math.Atan(dy / dx));
			}
			else // (dx < eps)
			{
				if (ady <= eps) return 180;
				// Second branch of the arctangent function (add PI to Main branch result)
				else return (float)(180 / Math.PI * (Math.Atan(dy / dx) + Math.PI));
			}
		}

		// Return the distance between two points.
		private float getDistance(PointF p1, PointF p2)
		{
			return (float)Math.Sqrt(
				(p2.X - p1.X) * (p2.X - p1.X) +
				(p2.Y - p1.Y) * (p2.Y - p1.Y));
		}

		// Return the midpoint between two given points
		private PointF getMidpoint(PointF p1, PointF p2)
		{
			return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}

	}
}
