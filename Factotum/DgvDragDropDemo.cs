using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace FactotumMaster
{
	/// 
	/// This class extends the DataGridView to allow for custom
	/// drag and drop reordering operations on both columns and rows.  
	/// The sequence of user interaction events occurs as follows: 
	/// 1. The user clicks (and releases) the left mouse button 
	/// on a row or column header cell to select the row or column. 
	/// 2. The user then clicks (and holds down) the left mouse 
	/// button to initiate a drag and drop operation which will allow 
	/// him/her to reorder the selected row or column within the 
	/// DataGridView. 
	/// 3. As the drag and drop operation begins, a horizontal (for 
	/// rows) or vertical (for columns) red line is displayed on the 
	/// DataGridView to indicate the target of the drag and drop operation 
	/// (i.e., to indicate where on the grid the selected row or column 
	/// will be dropped). 
	/// 4. When the user has selected the new target location for the 
	/// selected row/column, he/she releases the left mouse button, and 
	/// the appropriate reordering of columns or rows is carried out.
	/// ******************************************************************
	///  AUTHOR: Daniel S. Soper
	///     URL: http://www.danielsoper.com
	///    DATE: 02 February 2007
	/// LICENSE: Public Domain. Enjoy!   :-)
	/// ******************************************************************
	/// 
	class MyDGV : DataGridView
	{
		//vars for custom column/row drag/drop operations
		private Rectangle DragDropRectangle;
		private int DragDropSourceIndex;
		private int DragDropTargetIndex;
		private int DragDropCurrentIndex = -1;
		private int DragDropType; //0=column, 1=row

		public MyDGV()
		{
			this.AllowUserToResizeRows = false;
			this.SelectionMode = DataGridViewSelectionMode.CellSelect;
			this.AllowUserToOrderColumns = false;
			this.AllowDrop = true;
			this.Size = new Size(150, 80);
		} //end default constructor

		protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
		{
			//runs when a new column is added to the DGV
			e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
			e.Column.HeaderText = "column " + e.Column.Index;
			base.OnColumnAdded(e);
		} //end OnColumnAdded

		protected override void OnRowHeaderMouseClick(DataGridViewCellMouseEventArgs e)
		{
			//runs when the mouse is clicked over a row header cell
			if (e.RowIndex > -1)
			{
				if (e.Button == MouseButtons.Left)
				{
					//single-click left mouse button
					if (this.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
					{
						this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
						this.Rows[e.RowIndex].Selected = true;
						this.CurrentCell = this[0, e.RowIndex];
					} //end if
				}
			} //end if
			base.OnRowHeaderMouseClick(e);
		} //end OnRowHeaderMouseClick

		protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
		{
			//runs when the mouse is clicked over a cell
			if (e.RowIndex > -1 && e.ColumnIndex > -1)
			{
				if (e.Button == MouseButtons.Left)
				{
					//single-click left mouse button
					if (this.SelectionMode != DataGridViewSelectionMode.CellSelect)
					{
						this.SelectionMode = DataGridViewSelectionMode.CellSelect;
					} //end if
				}

			} //end if
			base.OnCellMouseClick(e);
		} //end OnCellMouseClick

		protected override void OnMouseDown(MouseEventArgs e)
		{
			//stores values for drag/drop operations if necessary
			if (this.AllowDrop)
			{
				if (this.HitTest(e.X, e.Y).ColumnIndex == -1 && this.HitTest(e.X, e.Y).RowIndex > -1)
				{
					//if this is a row header cell
					if (this.Rows[this.HitTest(e.X, e.Y).RowIndex].Selected)
					{
						//if this row is selected
						DragDropType = 1;
						Size DragSize = SystemInformation.DragSize;
						DragDropRectangle = new Rectangle(new Point(e.X - (DragSize.Width / 2), e.Y - (DragSize.Height / 2)), DragSize);
						DragDropSourceIndex = this.HitTest(e.X, e.Y).RowIndex;
					}
					else
					{
						DragDropRectangle = Rectangle.Empty;
					} //end if
				}
				else
				{
					DragDropRectangle = Rectangle.Empty;
				} //end if
			}
			else
			{
				DragDropRectangle = Rectangle.Empty;
			}//end if
			base.OnMouseDown(e);
		} //end OnMouseDown

		protected override void OnMouseMove(MouseEventArgs e)
		{
			//handles drag/drop operations if necessary
			if (this.AllowDrop)
			{
				if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
				{
					if (DragDropRectangle != Rectangle.Empty && !DragDropRectangle.Contains(e.X, e.Y))
					{
						if (DragDropType == 1)
						{
							//row drag/drop
							DragDropEffects DropEffect = this.DoDragDrop(this.Rows[DragDropSourceIndex], DragDropEffects.Move);
						} //end if
					} //end if
				} //end if
			} //end if
			base.OnMouseMove(e);
		} //end OnMouseMove

		protected override void OnDragOver(DragEventArgs e)
		{
			//runs while the drag/drop is in progress
			if (this.AllowDrop)
			{
				e.Effect = DragDropEffects.Move;
				if (DragDropType == 1)
				{
					//row drag/drop
					int CurRow = this.HitTest(this.PointToClient(new Point(e.X, e.Y)).X, this.PointToClient(new Point(e.X, e.Y)).Y).RowIndex;
					if (DragDropCurrentIndex != CurRow)
					{
						DragDropCurrentIndex = CurRow;
						this.Invalidate(); //repaint
					} //end if
				} //end if
			} //end if
			base.OnDragOver(e);
		} //end OnDragOver

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			//runs after a drag/drop operation for column/row has completed
			if (this.AllowDrop)
			{
				if (drgevent.Effect == DragDropEffects.Move)
				{
					Point ClientPoint = this.PointToClient(new Point(drgevent.X, drgevent.Y));
					if (DragDropType == 1)
					{
						//if this is a row drag/drop operation
						DragDropTargetIndex = this.HitTest(ClientPoint.X, ClientPoint.Y).RowIndex;
						int rowCount = this.RowCount;
						if (DragDropTargetIndex > -1 && DragDropCurrentIndex < rowCount - 1)
						{
							DragDropCurrentIndex = -1;
							DataGridViewRow SourceRow = drgevent.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
							this.Rows.RemoveAt(DragDropSourceIndex);
							int effectiveTargetIndex = DragDropTargetIndex > DragDropSourceIndex ?
								DragDropTargetIndex - 1 :
								DragDropTargetIndex;
							this.Rows.Insert(effectiveTargetIndex, SourceRow);
							this.Rows[effectiveTargetIndex].Selected = true;
							this.CurrentCell = this[0, effectiveTargetIndex];
						} //end if
					} //end if
				} //end if
			} //end if
			base.OnDragDrop(drgevent);
		} //end OnDragDrop

		protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
		{
			//draws red drag/drop target indicator lines if necessary
			if (DragDropCurrentIndex > -1)
			{
				if (DragDropType == 1)
				{
					//row drag/drop
					if (e.RowIndex == DragDropCurrentIndex && DragDropCurrentIndex < this.RowCount - 1)
					{
						//if this cell is in the same row as the mouse cursor
						Pen p = new Pen(Color.Red, 1);
						e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top - 1, e.CellBounds.Right, e.CellBounds.Top - 1);
					} //end if
				} //end if
			} //end if
			base.OnCellPainting(e);
		} //end OnCellPainting

	} //end class
}
