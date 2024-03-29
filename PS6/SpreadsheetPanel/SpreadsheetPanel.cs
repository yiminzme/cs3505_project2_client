﻿// Written by Joe Zachary for CS 3500, September 2011.
//Modified by Josh Christensen

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SS
{

    /// <summary>
    /// The type of delegate used to register for SelectionChanged events
    /// </summary>
    /// <param name="sender"></param>
    public delegate void SelectionChangedHandler(SpreadsheetPanel sender);


    /// <summary>
    /// A panel that displays a spreadsheet with 26 columns (labeled A-Z) and 99 rows
    /// (labeled 1-99).  Each cell on the grid can display a non-editable string.  One 
    /// of the cells is always selected (and highlighted).  When the selection changes, a 
    /// SelectionChanged event is fired.  Clients can register to be notified of
    /// such events.
    /// 
    /// None of the cells are editable.  They are for display purposes only.
    /// </summary>
    public partial class SpreadsheetPanel : UserControl
    {

        // The SpreadsheetPanel is composed of a DrawingPanel (where the grid is drawn),
        // a horizontal scroll bar, and a vertical scroll bar.
        private DrawingPanel drawingPanel;
        private HScrollBar hScroll;
        private VScrollBar vScroll;

        // These constants control the layout of the spreadsheet grid.  The height and
        // width measurements are in pixels.
        private const int DATA_COL_WIDTH = 80;
        private const int DATA_ROW_HEIGHT = 20;
        private const int LABEL_COL_WIDTH = 30;
        private const int LABEL_ROW_HEIGHT = 30;
        private const int PADDING = 2;
        private const int SCROLLBAR_WIDTH = 20;
        private const int COL_COUNT = 26;
        private const int ROW_COUNT = 99;


        /// <summary>
        /// Creates an empty SpreadsheetPanel
        /// </summary>
        public SpreadsheetPanel()
        {

            InitializeComponent();

            // The DrawingPanel is quite large, since it has 26 columns and 99 rows.  The
            // SpreadsheetPanel itself will usually be smaller, which is why scroll bars
            // are necessary.
            drawingPanel = new DrawingPanel(this);
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.AutoScroll = false;

            // A custom vertical scroll bar.  It is designed to scroll in multiples of rows.
            vScroll = new VScrollBar();
            vScroll.SmallChange = 1;
            vScroll.Maximum = ROW_COUNT;

            // A custom horizontal scroll bar.  It is designed to scroll in multiples of columns.
            hScroll = new HScrollBar();
            hScroll.SmallChange = 1;
            hScroll.Maximum = COL_COUNT;

            // Add the drawing panel and the scroll bars to the SpreadsheetPanel.
            Controls.Add(drawingPanel);
            Controls.Add(vScroll);
            Controls.Add(hScroll);

            // Arrange for the drawing panel to be notified when it needs to scroll itself.
            hScroll.ValueChanged += drawingPanel.HScrollValueChanged;
            vScroll.ValueChanged += drawingPanel.VScrollValueChanged;

        }

        /// <summary>
        /// Clears the display.
        /// </summary>

        public void Clear()
        {
            drawingPanel.Clear();
        }


        /// <summary>
        /// If the zero-based column and row are in range, sets the value of that
        /// cell and returns true.  Otherwise, returns false.
        /// </summary>
        public bool SetValue(int col, int row, string value)
        {
            return drawingPanel.SetValue(col, row, value);
        }


        /// <summary>
        /// If the zero-based column and row are in range, assigns the value
        /// of that cell to the out parameter and returns true.  Otherwise,
        /// returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetValue(int col, int row, out string value)
        {
            return drawingPanel.GetValue(col, row, out value);
        }


        /// <summary>
        /// If the zero-based column and row are in range, uses them to set
        /// the current selection and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public bool SetSelection(int col, int row)
        {
            return drawingPanel.SetSelection(col, row);
        }


        /// <summary>
        /// Assigns the column and row of the current selection to the
        /// out parameters.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>

        public void GetSelection(out int col, out int row)
        {
            drawingPanel.GetSelection(out col, out row);
        }

        /// <summary>
        /// Moves the selected box up one cell
        /// </summary>
        public void SelectUp()
        {
            int col, row;
            this.GetSelection(out col, out row);
            if (this.SetSelection(col, row - 1))
            {
                this.SelectionChanged(this);
            }
        }
        /// <summary>
        /// Moves the selected box down one cell
        /// </summary>
        public void selectDown()
        {
            int col, row;
            this.GetSelection(out col, out row);
            if (this.SetSelection(col, row + 1))
            {
                this.SelectionChanged(this);
            }
        }
        /// <summary>
        /// Moves the selected box left one cell
        /// </summary>
        public void selectLeft()
        {
            int col, row;
            this.GetSelection(out col, out row);
            if (this.SetSelection(col - 1, row))
            {
                this.SelectionChanged(this);
            }
        }
        /// <summary>
        /// Moves the selected box right one cell
        /// </summary>
        public void selectRight()
        {
            int col, row;
            this.GetSelection(out col, out row);
            if (this.SetSelection(col + 1, row))
            {
                this.SelectionChanged(this);
            }
        }


        /// <summary>
        /// When the SpreadsheetPanel is resized, we set the size and locations of the three
        /// components that make it up.
        /// </summary>
        /// <param name="eventargs"></param>

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (FindForm() == null || FindForm().WindowState != FormWindowState.Minimized)
            {
                drawingPanel.Size = new Size(Width - SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
                vScroll.Location = new Point(Width - SCROLLBAR_WIDTH, 0);
                vScroll.Size = new Size(SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
                vScroll.LargeChange = (Height - SCROLLBAR_WIDTH) / DATA_ROW_HEIGHT;
                hScroll.Location = new Point(0, Height - SCROLLBAR_WIDTH);
                hScroll.Size = new Size(Width - SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
                hScroll.LargeChange = (Width - SCROLLBAR_WIDTH) / DATA_COL_WIDTH;
            }
        }


        /// <summary>
        /// The event used to send notifications of a selection change
        /// </summary>

        public event SelectionChangedHandler SelectionChanged;


        /// <summary>
        /// Used internally to keep track of cell addresses
        /// </summary>

        private class Address
        {

            public int Col { get; set; }
            public int Row { get; set; }

            public Address(int c, int r)
            {
                Col = c;
                Row = r;
            }

            public override int GetHashCode()
            {
                return Col.GetHashCode() ^ Row.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if ((obj == null) || !(obj is Address))
                {
                    return false;
                }
                Address a = (Address)obj;
                return Col == a.Col && Row == a.Row;
            }

        }


        /// <summary>
        /// The panel where the spreadsheet grid is drawn.  It keeps track of the
        /// current selection as well as what is supposed to be drawn in each cell.
        /// </summary>
        private class DrawingPanel : Panel
        {
            // Columns and rows are numbered beginning with 0.  This is the coordinate
            // of the selected cell.
            private int _selectedCol;
            private int _selectedRow;

            // Coordinate of cell in upper-left corner of display
            private int _firstColumn = 0;
            private int _firstRow = 0;

            // The strings contained by the spreadsheet
            private Dictionary<Address, String> _values;
            private Object valuesLocker;

            // The containing panel
            private SpreadsheetPanel _ssp;

            /// <summary>
            ///  Creates a new DrawingPanel using the specified SpreadsheetPanel
            /// </summary>
            /// <param name="ss"></param>
            public DrawingPanel(SpreadsheetPanel ss)
            {
                DoubleBuffered = true;
                _values = new Dictionary<Address, String>();
                _ssp = ss;

                // vinc: init my variables
                IDToCell = new Dictionary<string, object[]>();
                IDToColor = new Dictionary<string, Color>();
                valuesLocker = new object();
            }

            /// <summary>
            /// Returns true if the passed address is an invalid address, false if it's valid.
            /// </summary>
            /// <param name="col"></param>
            /// <param name="row"></param>
            /// <returns></returns>
            private bool InvalidAddress(int col, int row)
            {
                return col < 0 || row < 0 || col >= COL_COUNT || row >= ROW_COUNT;
            }

            /// <summary>
            /// Clears all the drawn values of the spreadsheet.
            /// </summary>
            public void Clear()
            {
                lock (valuesLocker)
                    _values.Clear();
                // vinc: erase highlighted cell
                IDToCell.Clear();
                IDToColor.Clear();

                Invalidate();
            }

            /// <summary>
            /// Attempts to set the value at the specified coordinates. Returns true if we were successful, false otherwise.
            /// </summary>
            /// <param name="col"></param>
            /// <param name="row"></param>
            /// <param name="c"></param>
            /// <returns></returns>
            public bool SetValue(int col, int row, string c)
            {
                if (InvalidAddress(col, row))
                {
                    return false;
                }

                Address a = new Address(col, row);
                if (c == null || c == "")
                {
                    lock (valuesLocker)
                        _values.Remove(a);
                }
                else
                {
                    lock (valuesLocker)
                        _values[a] = c;
                }
                Invalidate();
                return true;
            }

            /// <summary>
            /// Attemtps to return the value of a specific location. Sets c to be that value, if the inputs are valid. Empty cells return
            /// and empty string.
            /// </summary>
            /// <param name="col"></param>
            /// <param name="row"></param>
            /// <param name="c"></param>
            /// <returns></returns>
            public bool GetValue(int col, int row, out string c)
            {
                if (InvalidAddress(col, row))
                {
                    c = null;
                    return false;
                }
                lock (valuesLocker)
                    if (!_values.TryGetValue(new Address(col, row), out c))
                    {
                        c = "";
                    }
                return true;
            }

            /// <summary>
            /// Sets the selection to be the specified column and row, if the passed values are valid.
            /// Returns true if we succeeded, false otherwise.
            /// </summary>
            /// <param name="col"></param>
            /// <param name="row"></param>
            /// <returns></returns>
            public bool SetSelection(int col, int row)
            {
                if (InvalidAddress(col, row))
                {
                    return false;
                }
                _selectedCol = col;
                _selectedRow = row;
                //ADDED BY JOSH CHRISTENSEN (u0978248)
                //Correct the position of the scroll bars.
                int colsDisplayed = (this.Width - _ssp.vScroll.Width) / DATA_COL_WIDTH;
                int rowsDisplayed = (this.Height - _ssp.hScroll.Height) / DATA_ROW_HEIGHT - 1;
                //Scroll left if the box is outside our view, in that direction.
                while (_selectedCol < _firstColumn)
                {
                    _ssp.hScroll.Value -= 1;
                }
                //Scroll right if the box is outside our view, in that direction.
                while ((colsDisplayed + _firstColumn) < _selectedCol)
                {
                    _ssp.hScroll.Value += 1;
                }
                //Scroll up if the box is outside our view, in that direction.
                while (_selectedRow < _firstRow)
                {
                    _ssp.vScroll.Value -= 1;
                }
                //Scroll up if the box is outside our view, in that direction.
                while ((rowsDisplayed + _firstRow) < _selectedRow)
                {
                    _ssp.vScroll.Value += 1;
                }
                Invalidate();
                return true;
            }

            //Returns the currently selected column and row.
            public void GetSelection(out int col, out int row)
            {
                col = _selectedCol;
                row = _selectedRow;
            }

            //Sets the new first column based upon whatever condition the scrollbars of the panel are in. Registered with the ValueChanged scroll event.
            public void HScrollValueChanged(object sender, EventArgs args)
            {
                HScrollBar bar = (HScrollBar)sender;
                _firstColumn = bar.Value;
                Invalidate();
            }

            //Sets the new first row based upon whatever condition the scrollbars of the panel are in. Registered with the ValueChanged scroll event.
            public void VScrollValueChanged(object sender, EventArgs args)
            {
                VScrollBar bar = (VScrollBar)sender;
                _firstRow = bar.Value;
                Invalidate();
            }

            //Paints the panel.
            protected override void OnPaint(PaintEventArgs e)
            {

                // Clip based on what needs to be refreshed.
                Region clip = new Region(e.ClipRectangle);
                e.Graphics.Clip = clip;

                // Color the background of the data area white
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.White),
                    LABEL_COL_WIDTH,
                    LABEL_ROW_HEIGHT,
                    (COL_COUNT - _firstColumn) * DATA_COL_WIDTH,
                    (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT);

                // Pen, brush, and fonts to use
                Brush brush = new SolidBrush(Color.Black);
                Pen pen = new Pen(brush);
                Font regularFont = Font;
                Font boldFont = new Font(regularFont, FontStyle.Bold);

                // Draw the column lines
                int bottom = LABEL_ROW_HEIGHT + (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, bottom));
                for (int x = 0; x <= (COL_COUNT - _firstColumn); x++)
                {
                    e.Graphics.DrawLine(
                        pen,
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, 0),
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, bottom));
                }

                // Draw the column labels
                for (int x = 0; x < COL_COUNT - _firstColumn; x++)
                {
                    Font f = (_selectedCol - _firstColumn == x) ? boldFont : Font;
                    DrawColumnLabel(e.Graphics, x, f);
                }

                // Draw the row lines
                int right = LABEL_COL_WIDTH + (COL_COUNT - _firstColumn) * DATA_COL_WIDTH;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(right, 0));
                for (int y = 0; y <= ROW_COUNT - _firstRow; y++)
                {
                    e.Graphics.DrawLine(
                        pen,
                        new Point(0, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT),
                        new Point(right, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT));
                }

                // Draw the row labels
                for (int y = 0; y < (ROW_COUNT - _firstRow); y++)
                {
                    Font f = (_selectedRow - _firstRow == y) ? boldFont : Font;
                    DrawRowLabel(e.Graphics, y, f);
                }

                // Highlight the selection, if it is visible
                if ((_selectedCol - _firstColumn >= 0) && (_selectedRow - _firstRow >= 0))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        new Rectangle(LABEL_COL_WIDTH + (_selectedCol - _firstColumn) * DATA_COL_WIDTH + 1,
                                      LABEL_ROW_HEIGHT + (_selectedRow - _firstRow) * DATA_ROW_HEIGHT + 1,
                                      DATA_COL_WIDTH - 2,
                                      DATA_ROW_HEIGHT - 2));
                }

                foreach (var cell in IDToCell)
                {
                    Pen highlightPen = new Pen(new SolidBrush(IDToColor[cell.Key]));
                    if (((int)cell.Value[1] - _firstColumn >= 0) && ((int)cell.Value[0] - _firstRow >= 0))
                    {
                        e.Graphics.DrawRectangle(
                            highlightPen,
                            new Rectangle(LABEL_COL_WIDTH + ((int)cell.Value[1] - _firstColumn) * DATA_COL_WIDTH + 1,
                                          LABEL_ROW_HEIGHT + ((int)cell.Value[0] - _firstRow) * DATA_ROW_HEIGHT + 1,
                                          DATA_COL_WIDTH - 2,
                                          DATA_ROW_HEIGHT - 2));
                    }
                }

                // Draw the text
                lock (valuesLocker)
                    foreach (KeyValuePair<Address, String> address in _values)
                    {
                        String text = address.Value;
                        int x = address.Key.Col - _firstColumn;
                        int y = address.Key.Row - _firstRow;
                        float height = e.Graphics.MeasureString(text, regularFont).Height;
                        float width = e.Graphics.MeasureString(text, regularFont).Width;
                        if (x >= 0 && y >= 0)
                        {
                            Region cellClip = new Region(new Rectangle(LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                                                                       LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT,
                                                                       DATA_COL_WIDTH - 2 * PADDING,
                                                                       DATA_ROW_HEIGHT));
                            cellClip.Intersect(clip);
                            e.Graphics.Clip = cellClip;
                            e.Graphics.DrawString(
                                text,
                                regularFont,
                                brush,
                                LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                                LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
                        }
                    }
            }

            /// <summary>
            /// Draws a column label.  The columns are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="x"></param>
            /// <param name="f"></param>
            private void DrawColumnLabel(Graphics g, int x, Font f)
            {
                String label = ((char)('A' + x + _firstColumn)).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                      label,
                      f,
                      new SolidBrush(Color.Black),
                      LABEL_COL_WIDTH + x * DATA_COL_WIDTH + (DATA_COL_WIDTH - width) / 2,
                      (LABEL_ROW_HEIGHT - height) / 2);
            }


            /// <summary>
            /// Draws a row label.  The rows are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="y"></param>
            /// <param name="f"></param>
            private void DrawRowLabel(Graphics g, int y, Font f)
            {
                String label = (y + 1 + _firstRow).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                    label,
                    f,
                    new SolidBrush(Color.Black),
                    LABEL_COL_WIDTH - width - PADDING,
                    LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
            }


            /// <summary>
            /// Determines which cell, if any, was clicked.  Generates a SelectionChanged event.  All of
            /// the indexes are zero based.
            /// </summary>
            /// <param name="e"></param>

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnClick(e);
                int x = (e.X - LABEL_COL_WIDTH) / DATA_COL_WIDTH;
                int y = (e.Y - LABEL_ROW_HEIGHT) / DATA_ROW_HEIGHT;
                if (e.X > LABEL_COL_WIDTH && e.Y > LABEL_ROW_HEIGHT && (x + _firstColumn < COL_COUNT) && (y + _firstRow < ROW_COUNT))
                {
                    _selectedCol = x + _firstColumn;
                    _selectedRow = y + _firstRow;
                    if (_ssp.SelectionChanged != null)
                    {
                        _ssp.SelectionChanged(_ssp);
                    }
                }
                Invalidate();
            }

            Dictionary<string, Object[]> IDToCell;
            Dictionary<string, Color> IDToColor;
            /// <summary>
            /// add cell that need to be highlighted
            /// </summary>
            public void addHighlightCell(string ID, int row, int col)
            {
                IDToCell[ID] = new object[] { row, col };
                if (!IDToColor.ContainsKey(ID))
                {
                    IDToColor[ID] = RandomColorObject(ID);
                }
                Invalidate();
            }

            /// <summary>
            /// hide a highlighted cell
            /// </summary>
            public void hideHighlightCell(string ID)
            {
                IDToCell.Remove(ID);
                Invalidate();
            }

            /// <summary>
            /// generate random color
            /// </summary>
            /// <param name="ID"></param>
            /// <returns></returns>
            private Color RandomColorObject(string ID)
            {
                int hashCode = (ID.ToString() + "SaltyMcSaltPants6589195drtfhxdtfhgfh61495619854").GetHashCode();
                return Color.FromArgb(255, (hashCode & 0x00FF0000) >> 16, (hashCode & 0x0000FF00) >> 8, hashCode & 0x000000FF);
            }


            // VinC: the win32 msg code to wheel tilt message
            private const int WM_MOUSEHWHEEL = 0x020E;
            /// <summary>
            /// VinC override
            /// 
            /// 1. Processes Windows messages.
            /// 2. respond to mouse horizontal wheel by rolling spreadsheet left/right
            /// </summary>
            /// <param name="m">The Windows System.Windows.Forms.Message to process.</param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.HWnd != this.Handle)
                    return;
                switch (m.Msg)
                {
                    case WM_MOUSEHWHEEL: // if it is mouse horizontal wheel
                        if ((int)m.WParam > 0) // if wheel tilt to the right
                            moveCol(_ssp.hScroll.SmallChange);
                        else // if wheel tilt to the left
                            moveCol(-_ssp.hScroll.SmallChange);
                        m.Result = (IntPtr)1; // indicate msg handled
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// VinC override
            /// 
            /// override the OnMouseWheel:
            /// 1. Raises the System.Windows.Forms.Control.MouseWheel event.
            /// 2. Respond to up/down scroll wheel control
            /// </summary>
            /// <param name="e">A System.Windows.Forms.MouseEventArgs that contains the event data.</param>
            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);
                if (ModifierKeys == Keys.Shift)
                {// if Shift key is pressed
                    if (e.Delta > 0)
                        moveCol(-_ssp.hScroll.SmallChange);
                    else if (e.Delta < 0)
                        moveCol(_ssp.hScroll.SmallChange);
                }
                else
                {// if Shift key isn't pressed
                    if (e.Delta > 0)
                        moveRow(-_ssp.vScroll.SmallChange);
                    else if (e.Delta < 0)
                        moveRow(_ssp.vScroll.SmallChange);
                }
            }

            /// <summary>
            /// VinC API method
            /// 
            /// move spreadsheet by distance rows, with row overflow protection
            /// e.g. 
            /// when distance > 0, _firstRow increase
            /// when distance &lt; 0, _firstRow decrease
            /// </summary>
            /// <param name="distance">the number of rows to move</param>
            /// <returns>the value of _firstRow(0 base)</returns>
            private int moveRow(int distance)
            {
                int maxFirstRow = ROW_COUNT - _ssp.vScroll.LargeChange;
                _firstRow = Math.Min(maxFirstRow, Math.Max(_firstRow + distance, 0));
                _ssp.vScroll.Value = _firstRow;
                Invalidate();
                return _firstRow;
            }

            /// <summary>
            /// VinC API method
            /// 
            /// move spreadsheet by distance cols, with cols overflow protection
            /// e.g. 
            /// when distance > 0, _firstColumn increase
            /// when distance &lt; 0, _firstColumn decrease
            /// </summary>
            /// <param name="distance">the number of cols to move</param>
            /// <returns>the value of _firstColumn(0 base)</returns>
            private int moveCol(int distance)
            {
                int maxFirstCol = COL_COUNT - _ssp.hScroll.LargeChange;
                _firstColumn = Math.Min(maxFirstCol, Math.Max(_firstColumn + distance, 0));
                _ssp.hScroll.Value = _firstColumn;
                Invalidate();
                return _firstColumn;
            }

        }

        /// <summary>
        /// add cell that need to be highlighted
        /// </summary>
        public void addHighlightCell(string ID, int row, int col)
        {
            drawingPanel.addHighlightCell(ID, row, col);
        }

        /// <summary>
        /// hide a highlighted cell
        /// </summary>
        public void hideHighlightCell(string ID)
        {
            drawingPanel.hideHighlightCell(ID);
        }
    }
}
