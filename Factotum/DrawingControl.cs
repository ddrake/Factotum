using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public abstract class DrawingControl : Control
	{
		// Used to turn on adornments (handles) for resizing, changing orientation, etc.
		private bool isSelected = false;

		// These flags control whether or not text, stroke, and fill are painted for the control
		// Note: the BackColor property inherited from Control should be set by all
		// drawing controls to Color.Transparent.  
		// BackColor is the background color of the control surface which is mostly hidden 
		// after filling and setting the control region.
		// used for fill of interior of drawing control.		
		private bool hasText;
		private bool hasStroke;
		private bool hasFill;

		// These attributes are ignored if their respective has* flags are not set.
		private Color fillColor; 
		private Color textColor;
		// used for stroke around drawing control.
		private float stroke;	
		private Color strokeColor;

		// Custom cursors -- todo: clean up and add more.
		private Cursor moveCursor;
		private Cursor stretchCursor;

		// Could make this user accessible, but maybe unnecessary.
		protected const int adornmentSize = 10;

		// Design attributes
		public float Stroke
		{
			get { return stroke; }
			set
			{
				if (stroke != value)
				{
					stroke = value;
				}
			}
		}
		public Color StrokeColor
		{
			get { return strokeColor; }
			set
			{
				if (strokeColor != value)
				{
					strokeColor = value;
				}
			}
		}
		public Color FillColor
		{
			get { return fillColor; }
			set
			{
				if (fillColor != value)
				{
					fillColor = value;
				}
			}
		}
		public Color TextColor
		{
			get { return textColor; }
			set
			{
				if (textColor != value)
				{
					textColor = value;
				}
			}
		}
		
		public bool HasText
		{
			get { return hasText; }
			set
			{
				if (hasText != value)
				{
					hasText = value;
				}
			}
		}
		public bool HasStroke
		{
			get { return hasStroke; }
			set
			{
				if (hasStroke != value)
				{
					hasStroke = value;
				}
			}
		}
		public bool HasFill
		{
			get { return hasFill; }
			set
			{
				if (hasFill != value)
				{
					hasFill = value;
				}
			}
		}

		// This is declared abstract so that child controls are required to implement it
		// but can do so any way they like.  For example, for a notation, hiding the background
		// means turning off both fill and border.  For an arrow, it just means turning off fill.
		public abstract bool HasTransparentBackground { get; set; }

		public abstract void PaintGraphics(Graphics g, float scaleFactor);

		// When set, causes adornments to be drawn which can be dragged by the user.
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				if (isSelected != value)
				{
					isSelected = value;
					// Set the focus if it became selected -- even if it wasn't clicked on.
					if (value) Focus();
					OnSelectStateChange(EventArgs.Empty);
					Invalidate();
				}
			}
		}

		public Cursor StretchCursor
		{
			get { return stretchCursor; }
			set
			{
				if (stretchCursor != value)
				{
					stretchCursor = value;
				}
			}
		}
		public Cursor MoveCursor
		{
			get { return moveCursor; }
			set
			{
				if (moveCursor != value)
				{
					moveCursor = value;
				}
			}
		}

		public delegate void SelectStateChangedEventHandler(object sender, EventArgs e);
		public event SelectStateChangedEventHandler StateChange;

		// Handler for a custom event that is raised when the IsSelected property
		// changes.
		protected virtual void OnSelectStateChange(EventArgs e)
		{
			if (StateChange != null)
			{
				StateChange(this, e);
			}
		}
	}
}
