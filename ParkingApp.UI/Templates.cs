#region Imports
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
#endregion
namespace SyntaxHighlighter
{
    public class SyntaxRichTextBox : System.Windows.Forms.RichTextBox
    {
        private SyntaxSettings m_settings = new SyntaxSettings();
        private static bool m_bPaint = true;
        private string m_strLine = "";
        private int m_nContentLength = 0;
        private int m_nLineLength = 0;
        private int m_nLineStart = 0;
        private int m_nLineEnd = 0;
        private string m_strKeywords = "";
        private int m_nCurSelection = 0;

        /// <summary>
        /// The settings.
        /// </summary>
        public SyntaxSettings Settings
        {
            get { return m_settings; }
        }

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x00f)
            {
                if (m_bPaint)
                    base.WndProc(ref m);
                else
                    m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }
        /// <summary>
        /// OnTextChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            // Calculate shit here.
            m_nContentLength = this.TextLength;

            int nCurrentSelectionStart = SelectionStart;
            int nCurrentSelectionLength = SelectionLength;

            m_bPaint = false;

            // Find the start of the current line.
            m_nLineStart = nCurrentSelectionStart;
            while ((m_nLineStart > 0) && (Text[m_nLineStart - 1] != '\n'))
                m_nLineStart--;
            // Find the end of the current line.
            m_nLineEnd = nCurrentSelectionStart;
            while ((m_nLineEnd < Text.Length) && (Text[m_nLineEnd] != '\n'))
                m_nLineEnd++;
            // Calculate the length of the line.
            m_nLineLength = m_nLineEnd - m_nLineStart;
            // Get the current line.
            m_strLine = Text.Substring(m_nLineStart, m_nLineLength);

            // Process this line.
            ProcessLine();

            m_bPaint = true;
        }
        /// <summary>
        /// Process a line.
        /// </summary>
        private void ProcessLine()
        {
            // Save the position and make the whole line black
            int nPosition = SelectionStart;
            SelectionStart = m_nLineStart;
            SelectionLength = m_nLineLength;
            SelectionColor = Color.Black;

            // Process the keywords
            ProcessRegex(m_strKeywords, Settings.KeywordColor);
            // Process numbers
            if (Settings.EnableIntegers)
                ProcessRegex("\\b(?:[0-9]*\\.)?[0-9]+\\b", Settings.IntegerColor);
            // Process strings
            if (Settings.EnableStrings)
                ProcessRegex("\"[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\"", Settings.StringColor);
            // Process comments
            if (Settings.EnableComments && !string.IsNullOrEmpty(Settings.Comment))
                ProcessRegex(Settings.Comment + ".*$", Settings.CommentColor);

            SelectionStart = nPosition;
            SelectionLength = 0;
            SelectionColor = Color.Black;

            m_nCurSelection = nPosition;
        }
        /// <summary>
        /// Process a regular expression.
        /// </summary>
        /// <param name="strRegex">The regular expression.</param>
        /// <param name="color">The color.</param>
        private void ProcessRegex(string strRegex, Color color)
        {
            Regex regKeywords = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match regMatch;

            for (regMatch = regKeywords.Match(m_strLine); regMatch.Success; regMatch = regMatch.NextMatch())
            {
                // Process the words
                int nStart = m_nLineStart + regMatch.Index;
                int nLenght = regMatch.Length;
                SelectionStart = nStart;
                SelectionLength = nLenght;
                SelectionColor = color;
            }
        }
        /// <summary>
        /// Compiles the keywords as a regular expression.
        /// </summary>
        public void CompileKeywords()
        {
            for (int i = 0; i < Settings.Keywords.Count; i++)
            {
                string strKeyword = Settings.Keywords[i];

                if (i == Settings.Keywords.Count - 1)
                    m_strKeywords += "\\b" + strKeyword + "\\b";
                else
                    m_strKeywords += "\\b" + strKeyword + "\\b|";
            }
        }

        public void ProcessAllLines()
        {
            m_bPaint = false;

            int nStartPos = 0;
            int i = 0;
            int nOriginalPos = SelectionStart;
            while (i < Lines.Length)
            {
                m_strLine = Lines[i];
                m_nLineStart = nStartPos;
                m_nLineEnd = m_nLineStart + m_strLine.Length;

                ProcessLine();
                i++;

                nStartPos += m_strLine.Length + 1;
            }

            m_bPaint = true;
        }
    }

    /// <summary>
    /// Class to store syntax objects in.
    /// </summary>
    public class SyntaxList
    {
        public List<string> m_rgList = new List<string>();
        public Color m_color = new Color();
    }

    /// <summary>
    /// Settings for the keywords and colors.
    /// </summary>
    public class SyntaxSettings
    {
        SyntaxList m_rgKeywords = new SyntaxList();
        string m_strComment = "";
        Color m_colorComment = Color.Green;
        Color m_colorString = Color.Gray;
        Color m_colorInteger = Color.Red;
        bool m_bEnableComments = true;
        bool m_bEnableIntegers = true;
        bool m_bEnableStrings = true;

        #region Properties
        /// <summary>
        /// A list containing all keywords.
        /// </summary>
        public List<string> Keywords
        {
            get { return m_rgKeywords.m_rgList; }
        }
        /// <summary>
        /// The color of keywords.
        /// </summary>
        public Color KeywordColor
        {
            get { return m_rgKeywords.m_color; }
            set { m_rgKeywords.m_color = value; }
        }
        /// <summary>
        /// A string containing the comment identifier.
        /// </summary>
        public string Comment
        {
            get { return m_strComment; }
            set { m_strComment = value; }
        }
        /// <summary>
        /// The color of comments.
        /// </summary>
        public Color CommentColor
        {
            get { return m_colorComment; }
            set { m_colorComment = value; }
        }
        /// <summary>
        /// Enables processing of comments if set to true.
        /// </summary>
        public bool EnableComments
        {
            get { return m_bEnableComments; }
            set { m_bEnableComments = value; }
        }
        /// <summary>
        /// Enables processing of integers if set to true.
        /// </summary>
        public bool EnableIntegers
        {
            get { return m_bEnableIntegers; }
            set { m_bEnableIntegers = value; }
        }
        /// <summary>
        /// Enables processing of strings if set to true.
        /// </summary>
        public bool EnableStrings
        {
            get { return m_bEnableStrings; }
            set { m_bEnableStrings = value; }
        }
        /// <summary>
        /// The color of strings.
        /// </summary>
        public Color StringColor
        {
            get { return m_colorString; }
            set { m_colorString = value; }
        }
        /// <summary>
        /// The color of integers.
        /// </summary>
        public Color IntegerColor
        {
            get { return m_colorInteger; }
            set { m_colorInteger = value; }
        }
        #endregion
    }
}
namespace Ambiance
{

    #region RoundRectangle

    static class RoundRectangle
    {
        public static GraphicsPath RoundRect(Rectangle Rectangle, int Curve)
        {
            GraphicsPath P = new GraphicsPath();
            int ArcRectangleWidth = Curve * 2;
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90);
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90);
            P.AddLine(new Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));
            return P;
        }
        public static GraphicsPath RoundRect(int X, int Y, int Width, int Height, int Curve)
        {
            Rectangle Rectangle = new Rectangle(X, Y, Width, Height);
            GraphicsPath P = new GraphicsPath();
            int ArcRectangleWidth = Curve * 2;
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90);
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90);
            P.AddLine(new Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));
            return P;
        }
        public static GraphicsPath RoundedTopRect(Rectangle Rectangle, int Curve)
        {
            GraphicsPath P = new GraphicsPath();
            int ArcRectangleWidth = Curve * 2;
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
            P.AddLine(new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + ArcRectangleWidth), new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 1));
            P.AddLine(new Point(Rectangle.X, Rectangle.Height - 1 + Rectangle.Y), new Point(Rectangle.X, Rectangle.Y + Curve));
            return P;
        }
    }

    #endregion

    #region  ThemeContainer

    public class Ambiance_ThemeContainer : ContainerControl
    {

        #region  Enums

        public enum MouseState
        {
            None = 0,
            Over = 1,
            Down = 2,
            Block = 3
        }

        #endregion
        #region  Variables

        private Rectangle HeaderRect;
        protected MouseState State;
        private int MoveHeight;
        private Point MouseP = new Point(0, 0);
        private bool Cap = false;
        private bool HasShown;

        #endregion
        #region  Properties

        private bool _Sizable = true;
        public bool Sizable
        {
            get
            {
                return _Sizable;
            }
            set
            {
                _Sizable = value;
            }
        }

        private bool _SmartBounds = true;
        public bool SmartBounds
        {
            get
            {
                return _SmartBounds;
            }
            set
            {
                _SmartBounds = value;
            }
        }

        private bool _RoundCorners = true;
        public bool RoundCorners
        {
            get
            {
                return _RoundCorners;
            }
            set
            {
                _RoundCorners = value;
                Invalidate();
            }
        }

        private bool _IsParentForm;
        protected bool IsParentForm
        {
            get
            {
                return _IsParentForm;
            }
        }

        protected bool IsParentMdi
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }
                return Parent.Parent != null;
            }
        }

        private bool _ControlMode;
        protected bool ControlMode
        {
            get
            {
                return _ControlMode;
            }
            set
            {
                _ControlMode = value;
                Invalidate();
            }
        }

        private FormStartPosition _StartPosition;
        public FormStartPosition StartPosition
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                {
                    return ParentForm.StartPosition;
                }
                else
                {
                    return _StartPosition;
                }
            }
            set
            {
                _StartPosition = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.StartPosition = value;
                }
            }
        }

        #endregion
        #region  EventArgs

        protected sealed override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent == null)
            {
                return;
            }
            _IsParentForm = Parent is Form;

            if (!_ControlMode)
            {
                InitializeMessages();

                if (_IsParentForm)
                {
                    this.ParentForm.FormBorderStyle = FormBorderStyle.None;
                    this.ParentForm.TransparencyKey = Color.Fuchsia;

                    if (!DesignMode)
                    {
                        ParentForm.Shown += FormShown;
                    }
                }
                Parent.BackColor = BackColor;
                Parent.MinimumSize = new Size(261, 65);
            }
        }

        protected sealed override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!_ControlMode)
            {
                HeaderRect = new Rectangle(0, 0, Width - 14, MoveHeight - 7);
            }
            Invalidate();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                SetState(MouseState.Down);
            }
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
            {
                if (HeaderRect.Contains(e.Location))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[0]);
                }
                else if (_Sizable && !(Previous == 0))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[Previous]);
                }
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cap = false;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
            {
                if (_Sizable && !_ControlMode)
                {
                    InvalidateMouse();
                }
            }
            if (Cap)
            {
                Parent.Location = (System.Drawing.Point)((object)(System.Convert.ToDouble(MousePosition) - System.Convert.ToDouble(MouseP)));
            }
        }

        protected override void OnInvalidated(System.Windows.Forms.InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            ParentForm.Text = Text;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        private void FormShown(object sender, EventArgs e)
        {
            if (_ControlMode || HasShown)
            {
                return;
            }

            if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
            {
                Rectangle SB = Screen.PrimaryScreen.Bounds;
                Rectangle CB = ParentForm.Bounds;
                ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
            }
            HasShown = true;
        }

        #endregion
        #region  Mouse & Size

        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        private Point GetIndexPoint;
        private bool B1x;
        private bool B2x;
        private bool B3;
        private bool B4;
        private int GetIndex()
        {
            GetIndexPoint = PointToClient(MousePosition);
            B1x = GetIndexPoint.X < 7;
            B2x = GetIndexPoint.X > Width - 7;
            B3 = GetIndexPoint.Y < 7;
            B4 = GetIndexPoint.Y > Height - 7;

            if (B1x && B3)
            {
                return 4;
            }
            if (B1x && B4)
            {
                return 7;
            }
            if (B2x && B3)
            {
                return 5;
            }
            if (B2x && B4)
            {
                return 8;
            }
            if (B1x)
            {
                return 1;
            }
            if (B2x)
            {
                return 2;
            }
            if (B3)
            {
                return 3;
            }
            if (B4)
            {
                return 6;
            }
            return 0;
        }

        private int Current;
        private int Previous;
        private void InvalidateMouse()
        {
            Current = GetIndex();
            if (Current == Previous)
            {
                return;
            }

            Previous = Current;
            switch (Previous)
            {
                case 0:
                    Cursor = Cursors.Default;
                    break;
                case 6:
                    Cursor = Cursors.SizeNS;
                    break;
                case 8:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case 7:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        private Message[] Messages = new Message[9];
        private void InitializeMessages()
        {
            Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
            for (int I = 1; I <= 8; I++)
            {
                Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
            }
        }

        private void CorrectBounds(Rectangle bounds)
        {
            if (Parent.Width > bounds.Width)
            {
                Parent.Width = bounds.Width;
            }
            if (Parent.Height > bounds.Height)
            {
                Parent.Height = bounds.Height;
            }

            int X = Parent.Location.X;
            int Y = Parent.Location.Y;

            if (X < bounds.X)
            {
                X = bounds.X;
            }
            if (Y < bounds.Y)
            {
                Y = bounds.Y;
            }

            int Width = bounds.X + bounds.Width;
            int Height = bounds.Y + bounds.Height;

            if (X + Parent.Width > Width)
            {
                X = Width - Parent.Width;
            }
            if (Y + Parent.Height > Height)
            {
                Y = Height - Parent.Height;
            }

            Parent.Location = new Point(X, Y);
        }

        private bool WM_LMBUTTONDOWN;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (WM_LMBUTTONDOWN && m.Msg == 513)
            {
                WM_LMBUTTONDOWN = false;

                SetState(MouseState.Over);
                if (!_SmartBounds)
                {
                    return;
                }

                if (IsParentMdi)
                {
                    CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
                }
                else
                {
                    CorrectBounds(Screen.FromControl(Parent).WorkingArea);
                }
            }
        }

        #endregion

        protected override void CreateHandle()
        {
            base.CreateHandle();
        }

        public Ambiance_ThemeContainer()
        {
            SetStyle((ControlStyles)(139270), true);
            BackColor = Color.FromArgb(244, 241, 243);
            Padding = new Padding(20, 56, 20, 16);
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            MoveHeight = 48;
            Font = new Font("Segoe UI", 9);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            G.Clear(Color.FromArgb(69, 68, 63));

            G.DrawRectangle(new Pen(Color.FromArgb(38, 38, 38)), new Rectangle(0, 0, Width - 1, Height - 1));
            // Use [Color.FromArgb(87, 86, 81), Color.FromArgb(60, 59, 55)] for a darker taste
            // And replace each (60, 59, 55) with (69, 68, 63)
            G.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, 36), Color.FromArgb(87, 85, 77), Color.FromArgb(69, 68, 63)), new Rectangle(1, 1, Width - 2, 36));
            G.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(69, 68, 63), Color.FromArgb(69, 68, 63)), new Rectangle(1, 36, Width - 2, Height - 46));

            G.DrawRectangle(new Pen(Color.FromArgb(38, 38, 38)), new Rectangle(9, 47, Width - 19, Height - 55));
            G.FillRectangle(new SolidBrush(Color.FromArgb(244, 241, 243)), new Rectangle(10, 48, Width - 20, Height - 56));

            if (_RoundCorners == true)
            {

                // Draw Left upper corner
                G.FillRectangle(Brushes.Fuchsia, 0, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 2, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 3, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, 1, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 1, 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 1, 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 2, 1, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 3, 1, 1, 1);

                // Draw right upper corner
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 3, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 4, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, 1, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 2, 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 2, 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 3, 1, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 4, 1, 1, 1);

                // Draw Left bottom corner
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 4, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 2, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 3, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, Height - 2, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 1, Height - 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 1, Height - 4, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 3, Height - 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), 2, Height - 2, 1, 1);

                // Draw right bottom corner
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 3, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 4, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 3, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 4, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 4, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, Height - 2, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 2, Height - 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 2, Height - 4, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 4, Height - 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 38)), Width - 3, Height - 2, 1, 1);
            }

            G.DrawString(Text, new Font("Tahoma", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 219, 210)), new Rectangle(0, 14, Width - 1, Height), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });
        }
    }

    #endregion
    #region  ControlBox

    public class Ambiance_ControlBox : Control
    {

        #region  Enums

        public enum MouseState
        {
            None = 0,
            Over = 1,
            Down = 2
        }

        #endregion
        #region  MouseStates
        MouseState State = MouseState.None;
        int X;
        Rectangle CloseBtn = new Rectangle(3, 2, 17, 17);
        Rectangle MinBtn = new Rectangle(23, 2, 17, 17);
        Rectangle MaxBtn = new Rectangle(43, 2, 17, 17);

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            State = MouseState.Down;
            Invalidate();
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (X > 3 && X < 20)
            {
                FindForm().Close();
            }
            else if (X > 23 && X < 40)
            {
                FindForm().WindowState = FormWindowState.Minimized;
            }
            else if (X > 43 && X < 60)
            {
                if (_EnableMaximize == true)
                {
                    if (FindForm().WindowState == FormWindowState.Maximized)
                    {
                        FindForm().WindowState = FormWindowState.Minimized;
                        FindForm().WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        FindForm().WindowState = FormWindowState.Minimized;
                        FindForm().WindowState = FormWindowState.Maximized;
                    }
                }
            }
            State = MouseState.Over;
            Invalidate();
        }
        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            State = MouseState.Over;
            Invalidate();
        }
        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            State = MouseState.None;
            Invalidate();
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            X = e.Location.X;
            Invalidate();
        }
        #endregion
        #region  Properties

        bool _EnableMaximize = true;
        public bool EnableMaximize
        {
            get
            {
                return _EnableMaximize;
            }
            set
            {
                _EnableMaximize = value;
                if (_EnableMaximize == true)
                {
                    this.Size = new Size(64, 22);
                }
                else
                {
                    this.Size = new Size(44, 22);
                }
                Invalidate();
            }
        }

        #endregion

        public Ambiance_ControlBox()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer), true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Font = new Font("Marlett", 7);
            Anchor = (System.Windows.Forms.AnchorStyles)(AnchorStyles.Top | AnchorStyles.Right);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_EnableMaximize == true)
            {
                this.Size = new Size(64, 22);
            }
            else
            {
                this.Size = new Size(44, 22);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            // Auto-decide control location on the theme container
            Location = new Point(5, 13);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            base.OnPaint(e);
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            LinearGradientBrush LGBClose = new LinearGradientBrush(CloseBtn, Color.FromArgb(242, 132, 99), Color.FromArgb(224, 82, 33), 90);
            G.FillEllipse(LGBClose, CloseBtn);
            G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), CloseBtn);
            G.DrawString("r", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle((int)6.5, 8, 0, 0));

            LinearGradientBrush LGBMinimize = new LinearGradientBrush(MinBtn, Color.FromArgb(130, 129, 123), Color.FromArgb(103, 102, 96), 90);
            G.FillEllipse(LGBMinimize, MinBtn);
            G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), MinBtn);
            G.DrawString("0", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle(26, (int)4.4, 0, 0));

            if (_EnableMaximize == true)
            {
                LinearGradientBrush LGBMaximize = new LinearGradientBrush(MaxBtn, Color.FromArgb(130, 129, 123), Color.FromArgb(103, 102, 96), 90);
                G.FillEllipse(LGBMaximize, MaxBtn);
                G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), MaxBtn);
                G.DrawString("1", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle(46, 7, 0, 0));
            }

            switch (State)
            {
                case MouseState.None:
                    LinearGradientBrush xLGBClose_1 = new LinearGradientBrush(CloseBtn, Color.FromArgb(242, 132, 99), Color.FromArgb(224, 82, 33), 90);
                    G.FillEllipse(xLGBClose_1, CloseBtn);
                    G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), CloseBtn);
                    G.DrawString("r", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle((int)6.5, 8, 0, 0));

                    LinearGradientBrush xLGBMinimize_1 = new LinearGradientBrush(MinBtn, Color.FromArgb(130, 129, 123), Color.FromArgb(103, 102, 96), 90);
                    G.FillEllipse(xLGBMinimize_1, MinBtn);
                    G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), MinBtn);
                    G.DrawString("0", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle(26, (int)4.4, 0, 0));

                    if (_EnableMaximize == true)
                    {
                        LinearGradientBrush xLGBMaximize = new LinearGradientBrush(MaxBtn, Color.FromArgb(130, 129, 123), Color.FromArgb(103, 102, 96), 90);
                        G.FillEllipse(xLGBMaximize, MaxBtn);
                        G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), MaxBtn);
                        G.DrawString("1", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle(46, 7, 0, 0));
                    }
                    break;
                case MouseState.Over:
                    if (X > 3 && X < 20)
                    {
                        LinearGradientBrush xLGBClose = new LinearGradientBrush(CloseBtn, Color.FromArgb(248, 152, 124), Color.FromArgb(231, 92, 45), 90);
                        G.FillEllipse(xLGBClose, CloseBtn);
                        G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), CloseBtn);
                        G.DrawString("r", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle((int)6.5, 8, 0, 0));
                    }
                    else if (X > 23 && X < 40)
                    {
                        LinearGradientBrush xLGBMinimize = new LinearGradientBrush(MinBtn, Color.FromArgb(196, 196, 196), Color.FromArgb(173, 173, 173), 90);
                        G.FillEllipse(xLGBMinimize, MinBtn);
                        G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), MinBtn);
                        G.DrawString("0", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle(26, (int)4.4, 0, 0));
                    }
                    else if (X > 43 && X < 60)
                    {
                        if (_EnableMaximize == true)
                        {
                            LinearGradientBrush xLGBMaximize = new LinearGradientBrush(MaxBtn, Color.FromArgb(196, 196, 196), Color.FromArgb(173, 173, 173), 90);
                            G.FillEllipse(xLGBMaximize, MaxBtn);
                            G.DrawEllipse(new Pen(Color.FromArgb(57, 56, 53)), MaxBtn);
                            G.DrawString("1", new Font("Marlett", 7), new SolidBrush(Color.FromArgb(52, 50, 46)), new Rectangle(46, 7, 0, 0));
                        }
                    }
                    break;
            }

            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region Button 1

    class Ambiance_Button_1 : Control
    {

        #region Variables

        private int MouseState;
        private GraphicsPath Shape;
        private LinearGradientBrush InactiveGB;
        private LinearGradientBrush PressedGB;
        private LinearGradientBrush PressedContourGB;
        private Rectangle R1;
        private Pen P1;
        private Pen P3;
        private Image _Image;
        private Size _ImageSize;
        private StringAlignment _TextAlignment = StringAlignment.Center;
        private Color _TextColor = Color.FromArgb(150, 150, 150);
        private ContentAlignment _ImageAlign = ContentAlignment.MiddleLeft;

        #endregion
        #region Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = default(PointF);
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = Convert.ToSingle((Area.Width - ImageArea.Width) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.X = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.X = Area.Width - ImageArea.Width - 2;

                    break;
            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = Convert.ToSingle((Area.Height - ImageArea.Height) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.Y = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.Y = Area.Height - ImageArea.Height - 2;
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Far;
                    break;
            }
            return SF;
        }

        #endregion
        #region Properties

        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        public ContentAlignment ImageAlign
        {
            get { return _ImageAlign; }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        public StringAlignment TextAlignment
        {
            get { return this._TextAlignment; }
            set
            {
                this._TextAlignment = value;
                this.Invalidate();
            }
        }

        public override Color ForeColor
        {
            get { return this._TextColor; }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseState = 1;
            Focus();
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion

        public Ambiance_Button_1()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 12);
            ForeColor = Color.FromArgb(76, 76, 76);
            Size = new Size(177, 30);
            _TextAlignment = StringAlignment.Center;
            P1 = new Pen(Color.FromArgb(180, 180, 180));
            // P1 = Border color
        }

        protected override void OnResize(System.EventArgs e)
        {

            if (Width > 0 && Height > 0)
            {
                Shape = new GraphicsPath();
                R1 = new Rectangle(0, 0, Width, Height);

                InactiveGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(253, 252, 252), Color.FromArgb(239, 237, 236), 90f);
                PressedGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(226, 226, 226), Color.FromArgb(237, 237, 237), 90f);
                PressedContourGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(167, 167, 167), Color.FromArgb(167, 167, 167), 90f);

                P3 = new Pen(PressedContourGB);
            }

            var MyDrawer = Shape;
            MyDrawer.AddArc(0, 0, 10, 10, 180, 90);
            MyDrawer.AddArc(Width - 11, 0, 10, 10, -90, 90);
            MyDrawer.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            MyDrawer.AddArc(0, Height - 11, 10, 10, 90, 90);
            MyDrawer.CloseAllFigures();
            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            PointF ipt = ImageLocation(GetStringFormat(ImageAlign), Size, ImageSize);

            switch (MouseState)
            {
                case 0:
                    //Inactive
                    G.FillPath(InactiveGB, Shape);
                    // Fill button body with InactiveGB color gradient
                    G.DrawPath(P1, Shape);
                    // Draw button border [InactiveGB]
                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
                case 1:
                    //Pressed
                    G.FillPath(PressedGB, Shape);
                    // Fill button body with PressedGB color gradient
                    G.DrawPath(P3, Shape);
                    // Draw button border [PressedGB]

                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
            }
            base.OnPaint(e);
        }
    }

    #endregion
    #region Button 2

    class Ambiance_Button_2 : Control
    {

        #region Variables

        private int MouseState;
        private GraphicsPath Shape;
        private LinearGradientBrush InactiveGB;
        private LinearGradientBrush PressedGB;
        private LinearGradientBrush PressedContourGB;
        private Rectangle R1;
        private Pen P1;
        private Pen P3;
        private Image _Image;
        private Size _ImageSize;
        private StringAlignment _TextAlignment = StringAlignment.Center;
        private Color _TextColor = Color.FromArgb(150, 150, 150);
        private ContentAlignment _ImageAlign = ContentAlignment.MiddleLeft;

        #endregion
        #region Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = default(PointF);
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = Convert.ToSingle((Area.Width - ImageArea.Width) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.X = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.X = Area.Width - ImageArea.Width - 2;

                    break;
            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = Convert.ToSingle((Area.Height - ImageArea.Height) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.Y = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.Y = Area.Height - ImageArea.Height - 2;
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Far;
                    break;
            }
            return SF;
        }

        #endregion
        #region Properties

        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        public ContentAlignment ImageAlign
        {
            get { return _ImageAlign; }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        public StringAlignment TextAlignment
        {
            get { return this._TextAlignment; }
            set
            {
                this._TextAlignment = value;
                this.Invalidate();
            }
        }

        public override Color ForeColor
        {
            get { return this._TextColor; }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseState = 1;
            Focus();
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion

        public Ambiance_Button_2()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            ForeColor = Color.FromArgb(76, 76, 76);
            Size = new Size(177, 30);
            _TextAlignment = StringAlignment.Center;
            P1 = new Pen(Color.FromArgb(162, 120, 101));
            // P1 = Border color
        }

        protected override void OnResize(System.EventArgs e)
        {

            if (Width > 0 && Height > 0)
            {
                Shape = new GraphicsPath();
                R1 = new Rectangle(0, 0, Width, Height);

                InactiveGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(253, 175, 143), Color.FromArgb(244, 146, 106), 90f);
                PressedGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(244, 146, 106), Color.FromArgb(244, 146, 106), 90f);
                PressedContourGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(162, 120, 101), Color.FromArgb(162, 120, 101), 90f);

                P3 = new Pen(PressedContourGB);
            }

            var MyDrawer = Shape;
            MyDrawer.AddArc(0, 0, 10, 10, 180, 90);
            MyDrawer.AddArc(Width - 11, 0, 10, 10, -90, 90);
            MyDrawer.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            MyDrawer.AddArc(0, Height - 11, 10, 10, 90, 90);
            MyDrawer.CloseAllFigures();
            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            PointF ipt = ImageLocation(GetStringFormat(ImageAlign), Size, ImageSize);

            switch (MouseState)
            {
                case 0:
                    //Inactive
                    G.FillPath(InactiveGB, Shape);
                    // Fill button body with InactiveGB color gradient
                    G.DrawPath(P1, Shape);
                    // Draw button border [InactiveGB]
                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
                case 1:
                    //Pressed
                    G.FillPath(PressedGB, Shape);
                    // Fill button body with PressedGB color gradient
                    G.DrawPath(P3, Shape);
                    // Draw button border [PressedGB]

                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
            }
            base.OnPaint(e);
        }
    }

    #endregion
    #region Label

    class Ambiance_Label : Label
    {

        public Ambiance_Label()
        {
            Font = new Font("Segoe UI", 11);
            ForeColor = Color.FromArgb(76, 76, 77);
            BackColor = Color.Transparent;
        }
    }

    #endregion
    #region Link Label
    class Ambiance_LinkLabel : LinkLabel
    {

        public Ambiance_LinkLabel()
        {
            Font = new Font("Segoe UI", 11, FontStyle.Regular);
            BackColor = Color.Transparent;
            LinkColor = Color.FromArgb(240, 119, 70);
            ActiveLinkColor = Color.FromArgb(221, 72, 20);
            VisitedLinkColor = Color.FromArgb(240, 119, 70);
            LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
        }
    }

    #endregion
    #region Header Label

    class Ambiance_HeaderLabel : Label
    {

        public Ambiance_HeaderLabel()
        {
            Font = new Font("Segoe UI", 11, FontStyle.Bold);
            ForeColor = Color.FromArgb(76, 76, 77);
            BackColor = Color.Transparent;
        }
    }

    #endregion
    #region Separator

    public class Ambiance_Separator : Control
    {

        public Ambiance_Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = new Size(120, 10);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(224, 222, 220)), 0, 5, Width, 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(250, 249, 249)), 0, 6, Width, 6);
        }
    }

    #endregion
    #region ProgressBar

    public class Ambiance_ProgressBar : Control
    {

        #region Enums

        public enum Alignment
        {
            Right,
            Center
        }

        #endregion
        #region Variables

        private int _Minimum;
        private int _Maximum = 100;
        private int _Value = 0;
        private Alignment ALN;
        private bool _DrawHatch;

        private bool _ShowPercentage;
        private GraphicsPath GP1;
        private GraphicsPath GP2;
        private GraphicsPath GP3;
        private Rectangle R1;
        private Rectangle R2;
        private LinearGradientBrush GB1;
        private LinearGradientBrush GB2;
        private int I1;

        #endregion
        #region Properties

        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < 1)
                    value = 1;
                if (value < _Value)
                    _Value = value;
                _Maximum = value;
                Invalidate();
            }
        }

        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                _Minimum = value;

                if (value > _Maximum)
                    _Maximum = value;
                if (value > _Value)
                    _Value = value;

                Invalidate();
            }
        }

        public int Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum)
                    value = Maximum;
                _Value = value;
                Invalidate();
            }
        }

        public Alignment ValueAlignment
        {
            get { return ALN; }
            set
            {
                ALN = value;
                Invalidate();
            }
        }

        public bool DrawHatch
        {
            get { return _DrawHatch; }
            set
            {
                _DrawHatch = value;
                Invalidate();
            }
        }

        public bool ShowPercentage
        {
            get { return _ShowPercentage; }
            set
            {
                _ShowPercentage = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Height = 20;
            Size minimumSize = new Size(58, 20);
            this.MinimumSize = minimumSize;
        }

        #endregion

        public Ambiance_ProgressBar()
        {
            _Maximum = 100;
            _ShowPercentage = true;
            _DrawHatch = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
        }

        public void Increment(int value)
        {
            this._Value += value;
            Invalidate();
        }

        public void Deincrement(int value)
        {
            this._Value -= value;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.Clear(Color.Transparent);
            G.SmoothingMode = SmoothingMode.HighQuality;

            GP1 = RoundRectangle.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 4);
            GP2 = RoundRectangle.RoundRect(new Rectangle(1, 1, Width - 3, Height - 3), 4);

            R1 = new Rectangle(0, 2, Width - 1, Height - 1);
            GB1 = new LinearGradientBrush(R1, Color.FromArgb(255, 255, 255), Color.FromArgb(230, 230, 230), 90f);

            // Draw inside background
            G.FillRectangle(new SolidBrush(Color.FromArgb(244, 241, 243)), R1);
            G.SetClip(GP1);
            G.FillPath(new SolidBrush(Color.FromArgb(244, 241, 243)), RoundRectangle.RoundRect(new Rectangle(1, 1, Width - 3, Height / 2 - 2), 4));


            I1 = (int)Math.Round(((double)(this._Value - this._Minimum) / (double)(this._Maximum - this._Minimum)) * (double)(this.Width - 3));
            if (I1 > 1)
            {
                GP3 = RoundRectangle.RoundRect(new Rectangle(1, 1, I1, Height - 3), 4);

                R2 = new Rectangle(1, 1, I1, Height - 3);
                GB2 = new LinearGradientBrush(R2, Color.SeaGreen, Color.YellowGreen, 90f);

                // Fill the value with its gradient
                G.FillPath(GB2, GP3);

                // Draw diagonal lines
                if (_DrawHatch == true)
                {
                    for (var i = 0; i <= (Width - 1) * _Maximum / _Value; i += 20)
                    {
                        G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(25, Color.White)), 10.0F), new Point(System.Convert.ToInt32(i), 0), new Point((int)(i - 10), Height));
                    }
                }

                G.SetClip(GP3);
                G.SmoothingMode = SmoothingMode.None;
                G.SmoothingMode = SmoothingMode.AntiAlias;
                G.ResetClip();
            }

            // Draw value as a string
            string DrawString = Convert.ToString(Convert.ToInt32(Value)) + "%";
            int textX = (int)(this.Width - G.MeasureString(DrawString, Font).Width - 1);
            int textY = (int)((this.Height / 2) - (System.Convert.ToInt32(G.MeasureString(DrawString, Font).Height / 2) - 2));

            if (_ShowPercentage == true)
            {
                switch (ValueAlignment)
                {
                    case Alignment.Right:
                        G.DrawString(DrawString, new Font("Segoe UI", 8), Brushes.DimGray, new Point(textX, textY));
                        break;
                    case Alignment.Center:
                        G.DrawString(DrawString, new Font("Segoe UI", 8), Brushes.DimGray, new Rectangle(0, 0, Width, Height + 2), new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                        break;
                }
            }

            // Draw border
            G.DrawPath(new Pen(Color.FromArgb(180, 180, 180)), GP2);

            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region Progress Indicator

    class Ambiance_ProgressIndicator : Control
    {

        #region Variables

        private readonly SolidBrush BaseColor = new SolidBrush(Color.FromArgb(76, 76, 76));
        private readonly SolidBrush AnimationColor = new SolidBrush(Color.Gray);

        private readonly Timer AnimationSpeed = new Timer();
        private PointF[] FloatPoint;
        private BufferedGraphics BuffGraphics;
        private int IndicatorIndex;
        private readonly BufferedGraphicsContext GraphicsContext = BufferedGraphicsManager.Current;

        #endregion
        #region Custom Properties

        public Color P_BaseColor
        {
            get { return BaseColor.Color; }
            set { BaseColor.Color = value; }
        }

        public Color P_AnimationColor
        {
            get { return AnimationColor.Color; }
            set { AnimationColor.Color = value; }
        }

        public int P_AnimationSpeed
        {
            get { return AnimationSpeed.Interval; }
            set { AnimationSpeed.Interval = value; }
        }

        #endregion
        #region EventArgs

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
            UpdateGraphics();
            SetPoints();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            AnimationSpeed.Enabled = this.Enabled;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AnimationSpeed.Tick += AnimationSpeed_Tick;
            AnimationSpeed.Start();
        }

        private void AnimationSpeed_Tick(object sender, EventArgs e)
        {
            if (IndicatorIndex.Equals(0))
            {
                IndicatorIndex = FloatPoint.Length - 1;
            }
            else
            {
                IndicatorIndex -= 1;
            }
            this.Invalidate(false);
        }

        #endregion

        public Ambiance_ProgressIndicator()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(80, 80);
            Text = string.Empty;
            MinimumSize = new Size(80, 80);
            SetPoints();
            AnimationSpeed.Interval = 100;
        }

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
        }

        private void SetPoints()
        {
            Stack<PointF> stack = new Stack<PointF>();
            PointF startingFloatPoint = new PointF(((float)this.Width) / 2f, ((float)this.Height) / 2f);
            for (float i = 0f; i < 360f; i += 45f)
            {
                this.SetValue(startingFloatPoint, (int)Math.Round((double)((((double)this.Width) / 2.0) - 15.0)), (double)i);
                PointF endPoint = this.EndPoint;
                endPoint = new PointF(endPoint.X - 7.5f, endPoint.Y - 7.5f);
                stack.Push(endPoint);
            }
            this.FloatPoint = stack.ToArray();
        }

        private void UpdateGraphics()
        {
            if ((this.Width > 0) && (this.Height > 0))
            {
                Size size2 = new Size(this.Width + 1, this.Height + 1);
                this.GraphicsContext.MaximumBuffer = size2;
                this.BuffGraphics = this.GraphicsContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
                this.BuffGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.BuffGraphics.Graphics.Clear(this.BackColor);
            int num2 = this.FloatPoint.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (this.IndicatorIndex == i)
                {
                    this.BuffGraphics.Graphics.FillEllipse(this.AnimationColor, this.FloatPoint[i].X, this.FloatPoint[i].Y, 15f, 15f);
                }
                else
                {
                    this.BuffGraphics.Graphics.FillEllipse(this.BaseColor, this.FloatPoint[i].X, this.FloatPoint[i].Y, 15f, 15f);
                }
            }
            this.BuffGraphics.Render(e.Graphics);
        }


        private double Rise;
        private double Run;
        private PointF _StartingFloatPoint;

        private X AssignValues<X>(ref X Run, X Length)
        {
            Run = Length;
            return Length;
        }

        private void SetValue(PointF StartingFloatPoint, int Length, double Angle)
        {
            double CircleRadian = Math.PI * Angle / 180.0;

            _StartingFloatPoint = StartingFloatPoint;
            Rise = AssignValues(ref Run, Length);
            Rise = Math.Sin(CircleRadian) * Rise;
            Run = Math.Cos(CircleRadian) * Run;
        }

        private PointF EndPoint
        {
            get
            {
                float LocationX = Convert.ToSingle(_StartingFloatPoint.Y + Rise);
                float LocationY = Convert.ToSingle(_StartingFloatPoint.X + Run);

                return new PointF(LocationY, LocationX);
            }
        }
    }

    #endregion
    #region  Toggle Button

    [DefaultEvent("ToggledChanged")]
    public class Ambiance_Toggle : Control
    {

        #region  Enums

        public enum _Type
        {
            OnOff,
            YesNo,
            IO
        }

        #endregion
        #region  Variables

        public delegate void ToggledChangedEventHandler();
        private ToggledChangedEventHandler ToggledChangedEvent;

        public event ToggledChangedEventHandler ToggledChanged
        {
            add
            {
                ToggledChangedEvent = (ToggledChangedEventHandler)System.Delegate.Combine(ToggledChangedEvent, value);
            }
            remove
            {
                ToggledChangedEvent = (ToggledChangedEventHandler)System.Delegate.Remove(ToggledChangedEvent, value);
            }
        }

        private bool _Toggled;
        private _Type ToggleType;
        private Rectangle Bar;
        private Size cHandle = new Size(15, 20);

        #endregion
        #region  Properties

        public bool Toggled
        {
            get
            {
                return _Toggled;
            }
            set
            {
                _Toggled = value;
                Invalidate();
                if (ToggledChangedEvent != null)
                    ToggledChangedEvent();
            }
        }

        public _Type Type
        {
            get
            {
                return ToggleType;
            }
            set
            {
                ToggleType = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = 79;
            Height = 27;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Toggled = !Toggled;
            Focus();
        }

        #endregion

        public Ambiance_Toggle()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint), true);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;

            G.SmoothingMode = SmoothingMode.HighQuality;
            G.Clear(Parent.BackColor);

            int SwitchXLoc = 3;
            Rectangle ControlRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath ControlPath = RoundRectangle.RoundRect(ControlRectangle, 4);

            LinearGradientBrush BackgroundLGB = default(LinearGradientBrush);
            if (_Toggled)
            {
                SwitchXLoc = 37;
                BackgroundLGB = new LinearGradientBrush(ControlRectangle, Color.FromArgb(231, 108, 58), Color.FromArgb(236, 113, 63), 90.0F);
            }
            else
            {
                SwitchXLoc = 0;
                BackgroundLGB = new LinearGradientBrush(ControlRectangle, Color.FromArgb(208, 208, 208), Color.FromArgb(226, 226, 226), 90.0F);
            }

            // Fill inside background gradient
            G.FillPath(BackgroundLGB, ControlPath);

            // Draw string
            switch (ToggleType)
            {
                case _Type.OnOff:
                    if (Toggled)
                    {
                        G.DrawString("ON", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 18, (float)(Bar.Y + 13.5), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("OFF", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.DimGray, Bar.X + 59, (float)(Bar.Y + 13.5), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
                case _Type.YesNo:
                    if (Toggled)
                    {
                        G.DrawString("YES", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 18, (float)(Bar.Y + 13.5), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("NO", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.DimGray, Bar.X + 59, (float)(Bar.Y + 13.5), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
                case _Type.IO:
                    if (Toggled)
                    {
                        G.DrawString("I", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 18, (float)(Bar.Y + 13.5), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("O", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.DimGray, Bar.X + 59, (float)(Bar.Y + 13.5), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
            }

            Rectangle SwitchRectangle = new Rectangle(SwitchXLoc, 0, Width - 38, Height);
            GraphicsPath SwitchPath = RoundRectangle.RoundRect(SwitchRectangle, 4);
            LinearGradientBrush SwitchButtonLGB = new LinearGradientBrush(SwitchRectangle, Color.FromArgb(253, 253, 253), Color.FromArgb(240, 238, 237), LinearGradientMode.Vertical);

            // Fill switch background gradient
            G.FillPath(SwitchButtonLGB, SwitchPath);

            // Draw borders
            if (_Toggled == true)
            {
                G.DrawPath(new Pen(Color.FromArgb(185, 89, 55)), SwitchPath);
                G.DrawPath(new Pen(Color.FromArgb(185, 89, 55)), ControlPath);
            }
            else
            {
                G.DrawPath(new Pen(Color.FromArgb(181, 181, 181)), SwitchPath);
                G.DrawPath(new Pen(Color.FromArgb(181, 181, 181)), ControlPath);
            }
        }
    }

    #endregion
    #region CheckBox

    [DefaultEvent("CheckedChanged")]
    class Ambiance_CheckBox : Control
    {

        #region Variables

        private GraphicsPath Shape;
        private LinearGradientBrush GB;
        private Rectangle R1;
        private Rectangle R2;
        private bool _Checked;
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        #endregion
        #region Properties

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        #endregion

        public Ambiance_CheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            // Reduce control flicker
            Font = new Font("Segoe UI", 12);
            Size = new Size(171, 26);
        }

        protected override void OnClick(EventArgs e)
        {
            _Checked = !_Checked;
            if (CheckedChanged != null)
            {
                CheckedChanged(this);
            }
            Focus();
            Invalidate();
            base.OnClick(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnResize(System.EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                Shape = new GraphicsPath();

                R1 = new Rectangle(17, 0, Width, Height + 1);
                R2 = new Rectangle(0, 0, Width, Height);
                GB = new LinearGradientBrush(new Rectangle(0, 0, 25, 25), Color.FromArgb(213, 85, 32), Color.FromArgb(224, 123, 82), 90);

                var MyDrawer = Shape;
                MyDrawer.AddArc(0, 0, 7, 7, 180, 90);
                MyDrawer.AddArc(7, 0, 7, 7, -90, 90);
                MyDrawer.AddArc(7, 7, 7, 7, 0, 90);
                MyDrawer.AddArc(0, 7, 7, 7, 90, 90);
                MyDrawer.CloseAllFigures();
                Height = 15;
            }

            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var MyDrawer = e.Graphics;
            MyDrawer.Clear(Parent.BackColor);
            MyDrawer.SmoothingMode = SmoothingMode.AntiAlias;

            MyDrawer.FillPath(GB, Shape);
            // Fill the body of the CheckBox
            MyDrawer.DrawPath(new Pen(Color.FromArgb(182, 88, 55)), Shape);
            // Draw the border

            MyDrawer.DrawString(Text, Font, new SolidBrush(Color.FromArgb(76, 76, 95)), new Rectangle(17, 0, Width, Height - 1), new StringFormat { LineAlignment = StringAlignment.Center });

            if (Checked)
            {
                MyDrawer.DrawString("ü", new Font("Wingdings", 12), new SolidBrush(Color.FromArgb(255, 255, 255)), new Rectangle(-2, 1, Width, Height + 2), new StringFormat { LineAlignment = StringAlignment.Center });
            }
            e.Dispose();
        }
    }

    #endregion
    #region RadioButton

    [DefaultEvent("CheckedChanged")]
    class Ambiance_RadioButton : Control
    {

        #region Enums

        public enum MouseState : byte
        {
            None = 0,
            Over = 1,
            Down = 2,
            Block = 3
        }

        #endregion
        #region Variables

        private bool _Checked;
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        #endregion
        #region Properties

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                InvalidateControls();
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = 15;
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!_Checked)
                Checked = true;
            base.OnMouseDown(e);
            Focus();
        }

        #endregion

        public Ambiance_RadioButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 12);
            Width = 193;
        }

        private void InvalidateControls()
        {
            if (!IsHandleCreated || !_Checked)
                return;

            foreach (Control _Control in Parent.Controls)
            {
                if (!object.ReferenceEquals(_Control, this) && _Control is Ambiance_RadioButton)
                {
                    ((Ambiance_RadioButton)_Control).Checked = false;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var MyDrawer = e.Graphics;

            MyDrawer.Clear(Parent.BackColor);
            MyDrawer.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill the body of the ellipse with a gradient
            LinearGradientBrush LGB = new LinearGradientBrush(new Rectangle(new Point(0, 0), new Size(14, 14)), Color.FromArgb(213, 85, 32), Color.FromArgb(224, 123, 82), 90);
            MyDrawer.FillEllipse(LGB, new Rectangle(new Point(0, 0), new Size(14, 14)));

            GraphicsPath GP = new GraphicsPath();
            GP.AddEllipse(new Rectangle(0, 0, 14, 14));
            MyDrawer.SetClip(GP);
            MyDrawer.ResetClip();

            // Draw ellipse border
            MyDrawer.DrawEllipse(new Pen(Color.FromArgb(182, 88, 55)), new Rectangle(new Point(0, 0), new Size(14, 14)));

            // Draw an ellipse inside the body
            if (_Checked)
            {
                SolidBrush EllipseColor = new SolidBrush(Color.FromArgb(255, 255, 255));
                MyDrawer.FillEllipse(EllipseColor, new Rectangle(new Point(4, 4), new Size(6, 6)));
            }
            MyDrawer.DrawString(Text, Font, new SolidBrush(Color.FromArgb(76, 76, 95)), 16, 7, new StringFormat { LineAlignment = StringAlignment.Center });
            e.Dispose();
        }
    }

    #endregion
    #region  ComboBox

    public class Ambiance_ComboBox : ComboBox
    {

        #region  Variables

        private int _StartIndex = 0;
        private Color _HoverSelectionColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        #endregion
        #region  Custom Properties

        public int StartIndex
        {
            get
            {
                return _StartIndex;
            }
            set
            {
                _StartIndex = value;
                try
                {
                    base.SelectedIndex = value;
                }
                catch
                {
                }
                Invalidate();
            }
        }

        public Color HoverSelectionColor
        {
            get
            {
                return _HoverSelectionColor;
            }
            set
            {
                _HoverSelectionColor = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            LinearGradientBrush LGB = new LinearGradientBrush(e.Bounds, Color.FromArgb(246, 132, 85), Color.FromArgb(231, 108, 57), 90.0F);

            if (System.Convert.ToInt32((e.State & DrawItemState.Selected)) == (int)DrawItemState.Selected)
            {
                if (!(e.Index == -1))
                {
                    e.Graphics.FillRectangle(LGB, e.Bounds);
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, Brushes.WhiteSmoke, e.Bounds);
                }
            }
            else
            {
                if (!(e.Index == -1))
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(242, 241, 240)), e.Bounds);
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, Brushes.DimGray, e.Bounds);
                }
            }
            LGB.Dispose();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            SuspendLayout();
            Update();
            ResumeLayout();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            try
            {

                if (!Focused)
                {
                    SelectionLength = 0;
                }
            }
            catch (Exception) { }
        }

        #endregion

        public Ambiance_ComboBox()
        {
            SetStyle((ControlStyles)(139286), true);
            SetStyle(ControlStyles.Selectable, false);

            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;

            BackColor = Color.FromArgb(246, 246, 246);
            ForeColor = Color.FromArgb(142, 142, 142);
            Size = new Size(135, 26);
            ItemHeight = 20;
            DropDownHeight = 100;
            Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            LinearGradientBrush LGB = default(LinearGradientBrush);
            GraphicsPath GP = default(GraphicsPath);

            e.Graphics.Clear(Parent.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create a curvy border
            GP = RoundRectangle.RoundRect(0, 0, Width - 1, Height - 1, 5);
            // Fills the body of the rectangle with a gradient
            LGB = new LinearGradientBrush(ClientRectangle, Color.FromArgb(253, 252, 252), Color.FromArgb(239, 237, 236), 90.0F);

            e.Graphics.SetClip(GP);
            e.Graphics.FillRectangle(LGB, ClientRectangle);
            e.Graphics.ResetClip();

            // Draw rectangle border
            e.Graphics.DrawPath(new Pen(Color.FromArgb(180, 180, 180)), GP);
            // Draw string
            e.Graphics.DrawString(Text, Font, new SolidBrush(Color.FromArgb(76, 76, 97)), new Rectangle(3, 0, Width - 20, Height), new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            });
            e.Graphics.DrawString("6", new Font("Marlett", 13, FontStyle.Regular), new SolidBrush(Color.FromArgb(119, 119, 118)), new Rectangle(3, 0, Width - 4, Height), new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Far
            });
            e.Graphics.DrawLine(new Pen(Color.FromArgb(224, 222, 220)), Width - 24, 4, Width - 24, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(250, 249, 249)), Width - 25, 4, Width - 25, this.Height - 5);

            GP.Dispose();
            LGB.Dispose();
        }
    }

    #endregion
    #region  NumericUpDown

    public class Ambiance_NumericUpDown : Control
    {

        #region  Enums

        public enum _TextAlignment
        {
            Near,
            Center
        }

        #endregion
        #region  Variables

        private GraphicsPath Shape;
        private Pen P1;

        private long _Value;
        private long _Minimum;
        private long _Maximum;
        private int Xval;
        private bool KeyboardNum;
        private _TextAlignment MyStringAlignment;

        private Timer LongPressTimer = new Timer();

        #endregion
        #region  Properties

        public long Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value <= _Maximum & value >= _Minimum)
                {
                    _Value = value;
                }
                Invalidate();
            }
        }

        public long Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                if (value < _Maximum)
                {
                    _Minimum = value;
                }
                if (_Value < _Minimum)
                {
                    _Value = Minimum;
                }
                Invalidate();
            }
        }

        public long Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                if (value > _Minimum)
                {
                    _Maximum = value;
                }
                if (_Value > _Maximum)
                {
                    _Value = _Maximum;
                }
                Invalidate();
            }
        }

        public _TextAlignment TextAlignment
        {
            get
            {
                return MyStringAlignment;
            }
            set
            {
                MyStringAlignment = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            Height = 28;
            MinimumSize = new Size(93, 28);
            Shape = new GraphicsPath();
            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Xval = e.Location.X;
            Invalidate();

            if (e.X < Width - 50)
            {
                Cursor = Cursors.IBeam;
            }
            else
            {
                Cursor = Cursors.Default;
            }
            if (e.X > this.Width - 25 && e.X < this.Width - 10)
            {
                Cursor = Cursors.Hand;
            }
            if (e.X > this.Width - 44 && e.X < this.Width - 33)
            {
                Cursor = Cursors.Hand;
            }
        }

        private void ClickButton()
        {
            if (Xval > this.Width - 25 && Xval < this.Width - 10)
            {
                if ((Value + 1) <= _Maximum)
                {
                    _Value++;
                }
            }
            else
            {
                if (Xval > this.Width - 44 && Xval < this.Width - 33)
                {
                    if ((Value - 1) >= _Minimum)
                    {
                        _Value--;
                    }
                }
                KeyboardNum = !KeyboardNum;
            }
            Focus();
            Invalidate();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            ClickButton();
            LongPressTimer.Start();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            LongPressTimer.Stop();
        }
        private void LongPressTimer_Tick(object sender, EventArgs e)
        {
            ClickButton();
        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            try
            {
                if (KeyboardNum == true)
                {
                    _Value = long.Parse((_Value).ToString() + e.KeyChar.ToString().ToString());
                }
                if (_Value > _Maximum)
                {
                    _Value = _Maximum;
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Back)
            {
                string TemporaryValue = _Value.ToString();
                TemporaryValue = TemporaryValue.Remove(Convert.ToInt32(TemporaryValue.Length - 1));
                if (TemporaryValue.Length == 0)
                {
                    TemporaryValue = "0";
                }
                _Value = Convert.ToInt32(TemporaryValue);
            }
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                if ((Value + 1) <= _Maximum)
                {
                    _Value++;
                }
                Invalidate();
            }
            else
            {
                if ((Value - 1) >= _Minimum)
                {
                    _Value--;
                }
                Invalidate();
            }
        }

        #endregion

        public Ambiance_NumericUpDown()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            P1 = new Pen(Color.FromArgb(180, 180, 180));
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(76, 76, 76);
            _Minimum = 0;
            _Maximum = 100;
            Font = new Font("Tahoma", 11);
            Size = new Size(70, 28);
            MinimumSize = new Size(62, 28);
            DoubleBuffered = true;

            LongPressTimer.Tick += LongPressTimer_Tick;
            LongPressTimer.Interval = 300;
        }

        public void Increment(int Value)
        {
            this._Value += Value;
            Invalidate();
        }

        public void Decrement(int Value)
        {
            this._Value -= Value;
            Invalidate();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            LinearGradientBrush BackgroundLGB = default(LinearGradientBrush);

            BackgroundLGB = new LinearGradientBrush(ClientRectangle, Color.FromArgb(246, 246, 246), Color.FromArgb(254, 254, 254), 90.0F);

            G.SmoothingMode = SmoothingMode.AntiAlias;

            G.Clear(Color.Transparent); // Set control background color
            G.FillPath(BackgroundLGB, Shape); // Draw background
            G.DrawPath(P1, Shape); // Draw border

            G.DrawString("+", new Font("Tahoma", 14), new SolidBrush(Color.FromArgb(75, 75, 75)), new Rectangle(Width - 25, 1, 19, 30));
            G.DrawLine(new Pen(Color.FromArgb(229, 228, 227)), Width - 28, 1, Width - 28, this.Height - 2);
            G.DrawString("-", new Font("Tahoma", 14), new SolidBrush(Color.FromArgb(75, 75, 75)), new Rectangle(Width - 44, 1, 19, 30));
            G.DrawLine(new Pen(Color.FromArgb(229, 228, 227)), Width - 48, 1, Width - 48, this.Height - 2);

            switch (MyStringAlignment)
            {
                case _TextAlignment.Near:
                    G.DrawString(System.Convert.ToString(Value), Font, new SolidBrush(ForeColor), new Rectangle(5, 0, Width - 1, Height - 1), new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                    break;
                case _TextAlignment.Center:
                    G.DrawString(System.Convert.ToString(Value), Font, new SolidBrush(ForeColor), new Rectangle(0, 0, Width - 1, Height - 1), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    break;
            }
            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region  TrackBar

    [DefaultEvent("ValueChanged")]
    public class Ambiance_TrackBar : Control
    {

        #region  Enums

        public enum ValueDivisor
        {
            By1 = 1,
            By10 = 10,
            By100 = 100,
            By1000 = 1000
        }

        #endregion
        #region  Variables

        private GraphicsPath PipeBorder;
        private GraphicsPath FillValue;
        private Rectangle TrackBarHandleRect;
        private bool Cap;
        private int ValueDrawer;

        private Size ThumbSize = new Size(15, 15);
        private Rectangle TrackThumb;

        private int _Minimum = 0;
        private int _Maximum = 10;
        private int _Value = 0;

        private bool _DrawValueString = false;
        private bool _JumpToMouse = false;
        private ValueDivisor DividedValue = ValueDivisor.By1;

        #endregion
        #region  Properties

        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {

                if (value >= _Maximum)
                {
                    value = _Maximum - 10;
                }
                if (_Value < value)
                {
                    _Value = value;
                }

                _Minimum = value;
                Invalidate();
            }
        }

        public int Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {

                if (value <= _Minimum)
                {
                    value = _Minimum + 10;
                }
                if (_Value > value)
                {
                    _Value = value;
                }

                _Maximum = value;
                Invalidate();
            }
        }

        public delegate void ValueChangedEventHandler();
        private ValueChangedEventHandler ValueChangedEvent;

        public event ValueChangedEventHandler ValueChanged
        {
            add
            {
                ValueChangedEvent = (ValueChangedEventHandler)System.Delegate.Combine(ValueChangedEvent, value);
            }
            remove
            {
                ValueChangedEvent = (ValueChangedEventHandler)System.Delegate.Remove(ValueChangedEvent, value);
            }
        }

        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != value)
                {
                    if (value < _Minimum)
                    {
                        _Value = _Minimum;
                    }
                    else
                    {
                        if (value > _Maximum)
                        {
                            _Value = _Maximum;
                        }
                        else
                        {
                            _Value = value;
                        }
                    }
                    Invalidate();
                    if (ValueChangedEvent != null)
                        ValueChangedEvent();
                }
            }
        }

        public ValueDivisor ValueDivison
        {
            get
            {
                return DividedValue;
            }
            set
            {
                DividedValue = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public float ValueToSet
        {
            get
            {
                return _Value / (int)DividedValue;
            }
            set
            {
                Value = (int)(value * (int)DividedValue);
            }
        }

        public bool JumpToMouse
        {
            get
            {
                return _JumpToMouse;
            }
            set
            {
                _JumpToMouse = value;
                Invalidate();
            }
        }

        public bool DrawValueString
        {
            get
            {
                return _DrawValueString;
            }
            set
            {
                _DrawValueString = value;
                if (_DrawValueString == true)
                {
                    Height = 35;
                }
                else
                {
                    Height = 22;
                }
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            checked
            {
                bool flag = this.Cap && e.X > -1 && e.X < this.Width + 1;
                if (flag)
                {
                    this.Value = this._Minimum + (int)Math.Round((double)(this._Maximum - this._Minimum) * ((double)e.X / (double)this.Width));
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            bool flag = e.Button == MouseButtons.Left;
            checked
            {
                if (flag)
                {
                    this.ValueDrawer = (int)Math.Round(((double)(this._Value - this._Minimum) / (double)(this._Maximum - this._Minimum)) * (double)(this.Width - 11));
                    this.TrackBarHandleRect = new Rectangle(this.ValueDrawer, 0, 25, 25);
                    this.Cap = this.TrackBarHandleRect.Contains(e.Location);
                    this.Focus();
                    flag = this._JumpToMouse;
                    if (flag)
                    {
                        this.Value = this._Minimum + (int)Math.Round((double)(this._Maximum - this._Minimum) * ((double)e.X / (double)this.Width));
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cap = false;
        }

        #endregion

        public Ambiance_TrackBar()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer), true);

            Size = new Size(80, 22);
            MinimumSize = new Size(47, 22);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_DrawValueString == true)
            {
                Height = 35;
            }
            else
            {
                Height = 22;
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;

            G.Clear(Parent.BackColor);
            G.SmoothingMode = SmoothingMode.AntiAlias;
            TrackThumb = new Rectangle(8, 10, Width - 16, 2);
            PipeBorder = RoundRectangle.RoundRect(1, 8, Width - 3, 5, 2);

            try
            {
                this.ValueDrawer = (int)Math.Round(((double)(this._Value - this._Minimum) / (double)(this._Maximum - this._Minimum)) * (double)(this.Width - 11));
            }
            catch (Exception)
            {
            }

            TrackBarHandleRect = new Rectangle(ValueDrawer, 0, 10, 20);

            G.SetClip(PipeBorder); // Set the clipping region of this Graphics to the specified GraphicsPath
            G.FillPath(new SolidBrush(Color.FromArgb(221, 221, 221)), PipeBorder);
            FillValue = RoundRectangle.RoundRect(1, 8, TrackBarHandleRect.X + TrackBarHandleRect.Width - 4, 5, 2);

            G.ResetClip(); // Reset the clip region of this Graphics to an infinite region

            G.SmoothingMode = SmoothingMode.HighQuality;
            G.DrawPath(new Pen(Color.FromArgb(200, 200, 200)), PipeBorder); // Draw pipe border
            G.FillPath(new SolidBrush(Color.FromArgb(217, 99, 50)), FillValue);

            G.FillEllipse(new SolidBrush(Color.FromArgb(244, 244, 244)), this.TrackThumb.X + (int)Math.Round(unchecked((double)this.TrackThumb.Width * ((double)this.Value / (double)this.Maximum))) - (int)Math.Round((double)this.ThumbSize.Width / 2.0), this.TrackThumb.Y + (int)Math.Round((double)this.TrackThumb.Height / 2.0) - (int)Math.Round((double)this.ThumbSize.Height / 2.0), this.ThumbSize.Width, this.ThumbSize.Height);
            G.DrawEllipse(new Pen(Color.FromArgb(180, 180, 180)), this.TrackThumb.X + (int)Math.Round(unchecked((double)this.TrackThumb.Width * ((double)this.Value / (double)this.Maximum))) - (int)Math.Round((double)this.ThumbSize.Width / 2.0), this.TrackThumb.Y + (int)Math.Round((double)this.TrackThumb.Height / 2.0) - (int)Math.Round((double)this.ThumbSize.Height / 2.0), this.ThumbSize.Width, this.ThumbSize.Height);

            if (_DrawValueString == true)
            {
                G.DrawString(System.Convert.ToString(ValueToSet), Font, Brushes.DimGray, 1, 20);
            }
        }
    }

    #endregion
    #region  Panel

    public class Ambiance_Panel : ContainerControl
    {
        public Ambiance_Panel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, false);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Graphics G = e.Graphics;

            this.Font = new Font("Tahoma", 9);
            this.BackColor = Color.White;
            G.SmoothingMode = SmoothingMode.AntiAlias;
            G.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Width, Height));
            G.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Width - 1, Height - 1));
            G.DrawRectangle(new Pen(Color.FromArgb(211, 208, 205)), 0, 0, Width - 1, Height - 1);
        }
    }

    #endregion
    #region TextBox

    [DefaultEvent("TextChanged")]
    class Ambiance_TextBox : Control
    {
        #region Variables

        public TextBox AmbianceTB = new TextBox();
        private GraphicsPath Shape;
        private int _maxchars = 32767;
        private bool _ReadOnly;
        private bool _Multiline;
        private HorizontalAlignment ALNType;
        private bool isPasswordMasked = false;
        private Pen P1;
        private SolidBrush B1;

        #endregion
        #region Properties

        public HorizontalAlignment TextAlignment
        {
            get { return ALNType; }
            set
            {
                ALNType = value;
                Invalidate();
            }
        }
        public int MaxLength
        {
            get { return _maxchars; }
            set
            {
                _maxchars = value;
                AmbianceTB.MaxLength = MaxLength;
                Invalidate();
            }
        }

        public bool UseSystemPasswordChar
        {
            get { return isPasswordMasked; }
            set
            {
                AmbianceTB.UseSystemPasswordChar = UseSystemPasswordChar;
                isPasswordMasked = value;
                Invalidate();
            }
        }
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                if (AmbianceTB != null)
                {
                    AmbianceTB.ReadOnly = value;
                }
            }
        }
        public bool Multiline
        {
            get { return _Multiline; }
            set
            {
                _Multiline = value;
                if (AmbianceTB != null)
                {
                    AmbianceTB.Multiline = value;

                    if (value)
                    {
                        AmbianceTB.Height = Height - 10;
                    }
                    else
                    {
                        Height = AmbianceTB.Height + 10;
                    }
                }
            }
        }
        [DefaultValue(CharacterCasing.Normal)]
        [IODescriptionAttribute("TextBoxCharacterCasingDescr")]
        public CharacterCasing Casing { get; set; }
        #endregion
        #region EventArgs

        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
           
            AmbianceTB.Text = Text;
            Invalidate();
        }

        private void OnBaseTextChanged(object s, EventArgs e)
        {
            Text = AmbianceTB.Text;
        }

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            AmbianceTB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
            AmbianceTB.Font = Font;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        private void _OnKeyDown(object Obj, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                AmbianceTB.SelectAll();
                e.SuppressKeyPress = true;
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                AmbianceTB.Copy();
                e.SuppressKeyPress = true;
            }
        }

        private void _Enter(object Obj, EventArgs e)
        {
            P1 = new Pen(Color.FromArgb(205, 87, 40));
            Refresh();
        }

        private void _Leave(object Obj, EventArgs e)
        {
            P1 = new Pen(Color.FromArgb(180, 180, 180));
            Refresh();
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (_Multiline)
            {
                AmbianceTB.Height = Height - 10;
            }
            else
            {
                Height = AmbianceTB.Height + 10;
            }

            Shape = new GraphicsPath();
            var _with1 = Shape;
            _with1.AddArc(0, 0, 10, 10, 180, 90);
            _with1.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _with1.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _with1.AddArc(0, Height - 11, 10, 10, 90, 90);
            _with1.CloseAllFigures();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            AmbianceTB.Focus();
        }

        #endregion
        public void AddTextBox()
        {
            var _TB = AmbianceTB;
            _TB.Size = new Size(Width - 10, 33);
            _TB.Location = new Point(7, 4);
            _TB.Text = String.Empty;
            _TB.BorderStyle = BorderStyle.None;
            _TB.TextAlign = HorizontalAlignment.Left;
            _TB.Font = new Font("Tahoma", 11);
            _TB.UseSystemPasswordChar = UseSystemPasswordChar;
            _TB.Multiline = false;
            AmbianceTB.KeyDown += _OnKeyDown;
            AmbianceTB.Enter += _Enter;
            AmbianceTB.Leave += _Leave;
            AmbianceTB.TextChanged += OnBaseTextChanged;
            AmbianceTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckEnter);

        }

        public Ambiance_TextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            AddTextBox();
            Controls.Add(AmbianceTB);

            P1 = new Pen(Color.FromArgb(180, 180, 180)); // P1 = Border color
            B1 = new SolidBrush(Color.White); // B1 = Rect Background color
            BackColor = Color.Transparent;
            ForeColor = Color.DimGray;

            Text = null;
            Font = new Font("Tahoma", 11);
            Size = new Size(135, 33);
            DoubleBuffered = true;
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.AntiAlias;

            var _TB = AmbianceTB;
            _TB.Width = Width - 10;
            _TB.TextAlign = TextAlignment;
            _TB.UseSystemPasswordChar = UseSystemPasswordChar;

            G.Clear(Color.Transparent);
            G.FillPath(B1, Shape); // Draw background
            G.DrawPath(P1, Shape); // Draw border

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }

    }

    #endregion
    #region RichTextBox

    [DefaultEvent("TextChanged")]
    class Ambiance_RichTextBox : Control
    {

        #region Variables

        public RichTextBox AmbianceRTB = new RichTextBox();
        private bool _ReadOnly;
        private bool _WordWrap;
        private bool _AutoWordSelection;
        private GraphicsPath Shape;
        private Pen P1;

        #endregion
        #region Properties

        public override string Text
        {
            get { return AmbianceRTB.Text; }
            set
            {
                AmbianceRTB.Text = value;
                Invalidate();
            }
        }
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                if (AmbianceRTB != null)
                {
                    AmbianceRTB.ReadOnly = value;
                }
            }
        }
        public bool WordWrap
        {
            get { return _WordWrap; }
            set
            {
                _WordWrap = value;
                if (AmbianceRTB != null)
                {
                    AmbianceRTB.WordWrap = value;
                }
            }
        }
        public bool AutoWordSelection
        {
            get { return _AutoWordSelection; }
            set
            {
                _AutoWordSelection = value;
                if (AmbianceRTB != null)
                {
                    AmbianceRTB.AutoWordSelection = value;
                }
            }
        }
        #endregion
        #region EventArgs

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            AmbianceRTB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
            AmbianceRTB.Font = Font;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            AmbianceRTB.Size = new Size(Width - 13, Height - 11);
        }

        private void _Enter(object Obj, EventArgs e)
        {
            P1 = new Pen(Color.FromArgb(205, 87, 40));
            Refresh();
        }

        private void _Leave(object Obj, EventArgs e)
        {
            P1 = new Pen(Color.FromArgb(180, 180, 180));
            Refresh();
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            Shape = new GraphicsPath();
            var _Shape = Shape;
            _Shape.AddArc(0, 0, 10, 10, 180, 90);
            _Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            _Shape.CloseAllFigures();
        }

        public void _TextChanged(object s, EventArgs e)
        {
            AmbianceRTB.Text = Text;
        }

        #endregion

        public void AddRichTextBox()
        {
            var _RTB = AmbianceRTB;
            _RTB.BackColor = Color.White;
            _RTB.Size = new Size(Width - 10, 100);
            _RTB.Location = new Point(7, 5);
            _RTB.Text = string.Empty;
            _RTB.BorderStyle = BorderStyle.None;
            _RTB.Font = new Font("Tahoma", 10);
            _RTB.Multiline = true;
        }

        public Ambiance_RichTextBox()
            : base()
        {

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            AddRichTextBox();
            Controls.Add(AmbianceRTB);
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(76, 76, 76);

            P1 = new Pen(Color.FromArgb(180, 180, 180));
            Text = null;
            Font = new Font("Tahoma", 10);
            Size = new Size(150, 100);
            WordWrap = true;
            AutoWordSelection = false;
            DoubleBuffered = true;

            AmbianceRTB.Enter += _Enter;
            AmbianceRTB.Leave += _Leave;
            TextChanged += _TextChanged;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(B);
            G.SmoothingMode = SmoothingMode.AntiAlias;
            G.Clear(Color.Transparent);
            G.FillPath(Brushes.White, this.Shape);
            G.DrawPath(P1, this.Shape);
            G.Dispose();
            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            B.Dispose();
        }
    }

    #endregion
    #region  ListBox

    public class Ambiance_ListBox : ListBox
    {

        public Ambiance_ListBox()
        {
            this.SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint), true);
            this.DrawMode = DrawMode.OwnerDrawFixed;
            IntegralHeight = false;
            ItemHeight = 18;
            Font = new Font("Seoge UI", 11, FontStyle.Regular);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawBackground();
            LinearGradientBrush LGB = new LinearGradientBrush(e.Bounds, Color.FromArgb(246, 132, 85), Color.FromArgb(231, 108, 57), 90.0F);
            if (System.Convert.ToInt32((e.State & DrawItemState.Selected)) == (int)DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(LGB, e.Bounds);
            }
            using (SolidBrush b = new SolidBrush(e.ForeColor))
            {
                if (base.Items.Count == 0)
                {
                    return;
                }
                else
                {
                    e.Graphics.DrawString(base.GetItemText(base.Items[e.Index]), e.Font, b, e.Bounds);
                }
            }

            LGB.Dispose();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Region MyRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), MyRegion);

            if (this.Items.Count > 0)
            {
                for (int i = 0; i <= this.Items.Count - 1; i++)
                {
                    System.Drawing.Rectangle RegionRect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(RegionRect))
                    {
                        if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i) || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i)) || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, RegionRect, i, DrawItemState.Selected, this.ForeColor, this.BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, RegionRect, i, DrawItemState.Default, Color.FromArgb(60, 60, 60), this.BackColor));
                        }
                        MyRegion.Complement(RegionRect);
                    }
                }
            }
        }
    }

    #endregion
    #region  TabControl

    public class Ambiance_TabControl : TabControl
    {

        public Ambiance_TabControl()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint), true);
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();

            ItemSize = new Size(80, 24);
            Alignment = TabAlignment.Top;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            Rectangle ItemBoundsRect = new Rectangle();
            G.Clear(Parent.BackColor);
            for (int TabIndex = 0; TabIndex <= TabCount - 1; TabIndex++)
            {
                ItemBoundsRect = GetTabRect(TabIndex);
                if (!(TabIndex == SelectedIndex))
                {
                    G.DrawString(TabPages[TabIndex].Text, new Font(Font.Name, Font.Size - 1, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 76, 76)), new Rectangle(GetTabRect(TabIndex).Location, GetTabRect(TabIndex).Size), new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center
                    });
                }
            }

            // Draw container rectangle
            G.FillPath(new SolidBrush(Color.FromArgb(247, 246, 246)), RoundRectangle.RoundRect(0, 23, Width - 1, Height - 24, 2));
            G.DrawPath(new Pen(Color.FromArgb(201, 198, 195)), RoundRectangle.RoundRect(0, 23, Width - 1, Height - 24, 2));

            for (int ItemIndex = 0; ItemIndex <= TabCount - 1; ItemIndex++)
            {
                ItemBoundsRect = GetTabRect(ItemIndex);
                if (ItemIndex == SelectedIndex)
                {

                    // Draw header tabs
                    G.DrawPath(new Pen(Color.FromArgb(201, 198, 195)), RoundRectangle.RoundedTopRect(new Rectangle(new Point(ItemBoundsRect.X - 2, ItemBoundsRect.Y - 2), new Size(ItemBoundsRect.Width + 3, ItemBoundsRect.Height)), 7));
                    G.FillPath(new SolidBrush(Color.FromArgb(247, 246, 246)), RoundRectangle.RoundedTopRect(new Rectangle(new Point(ItemBoundsRect.X - 1, ItemBoundsRect.Y - 1), new Size(ItemBoundsRect.Width + 2, ItemBoundsRect.Height)), 7));

                    try
                    {
                        G.DrawString(TabPages[ItemIndex].Text, new Font(Font.Name, Font.Size - 1, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 76, 76)), new Rectangle(GetTabRect(ItemIndex).Location, GetTabRect(ItemIndex).Size), new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        });
                        TabPages[ItemIndex].BackColor = Color.FromArgb(247, 246, 246);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    #endregion

}
namespace iTalk
{

    #region RoundRect

    // [CREDIT][DO NOT REMOVE]
    //
    // This module was written by Aeonhack
    //
    // [CREDIT][DO NOT REMOVE]

    static class RoundRectangle
    {
        public static GraphicsPath RoundRect(Rectangle Rectangle, int Curve)
        {
            GraphicsPath GP = new GraphicsPath();
            int EndArcWidth = Curve * 2;
            GP.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -180, 90);
            GP.AddArc(new Rectangle(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -90, 90);
            GP.AddArc(new Rectangle(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 0, 90);
            GP.AddArc(new Rectangle(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 90, 90);
            GP.AddLine(new Point(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));
            return GP;
        }

        public static GraphicsPath RoundRect(int X, int Y, int Width, int Height, int Curve)
        {
            Rectangle Rectangle = new Rectangle(X, Y, Width, Height);
            GraphicsPath GP = new GraphicsPath();
            int EndArcWidth = Curve * 2;
            GP.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -180, 90);
            GP.AddArc(new Rectangle(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -90, 90);
            GP.AddArc(new Rectangle(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 0, 90);
            GP.AddArc(new Rectangle(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 90, 90);
            GP.AddLine(new Point(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));
            return GP;
        }
    }

    #endregion

    #region  Control Renderer

    #region  Color Table

    public abstract class xColorTable
    {
        public abstract Color TextColor { get; }
        public abstract Color Background { get; }
        public abstract Color SelectionBorder { get; }
        public abstract Color SelectionTopGradient { get; }
        public abstract Color SelectionMidGradient { get; }
        public abstract Color SelectionBottomGradient { get; }
        public abstract Color PressedBackground { get; }
        public abstract Color CheckedBackground { get; }
        public abstract Color CheckedSelectedBackground { get; }
        public abstract Color DropdownBorder { get; }
        public abstract Color Arrow { get; }
        public abstract Color OverflowBackground { get; }
    }

    public abstract class ColorTable
    {
        public abstract xColorTable CommonColorTable { get; }
        public abstract Color BackgroundTopGradient { get; }
        public abstract Color BackgroundBottomGradient { get; }
        public abstract Color DroppedDownItemBackground { get; }
        public abstract Color DropdownTopGradient { get; }
        public abstract Color DropdownBottomGradient { get; }
        public abstract Color Separator { get; }
        public abstract Color ImageMargin { get; }
    }

    public class MSColorTable : ColorTable
    {

        private xColorTable _CommonColorTable;

        public MSColorTable()
        {
            _CommonColorTable = new DefaultCColorTable();
        }

        public override xColorTable CommonColorTable
        {
            get
            {
                return _CommonColorTable;
            }
        }

        public override System.Drawing.Color BackgroundTopGradient
        {
            get
            {
                return Color.FromArgb(246, 246, 246);
            }
        }

        public override System.Drawing.Color BackgroundBottomGradient
        {
            get
            {
                return Color.FromArgb(226, 226, 226);
            }
        }

        public override System.Drawing.Color DropdownTopGradient
        {
            get
            {
                return Color.FromArgb(246, 246, 246);
            }
        }

        public override System.Drawing.Color DropdownBottomGradient
        {
            get
            {
                return Color.FromArgb(246, 246, 246);
            }
        }

        public override System.Drawing.Color DroppedDownItemBackground
        {
            get
            {
                return Color.FromArgb(240, 240, 240);
            }
        }

        public override System.Drawing.Color Separator
        {
            get
            {
                return Color.FromArgb(190, 195, 203);
            }
        }

        public override System.Drawing.Color ImageMargin
        {
            get
            {
                return Color.FromArgb(240, 240, 240);
            }
        }
    }

    public class DefaultCColorTable : xColorTable
    {

        public override System.Drawing.Color CheckedBackground
        {
            get
            {
                return Color.FromArgb(230, 230, 230);
            }
        }

        public override System.Drawing.Color CheckedSelectedBackground
        {
            get
            {
                return Color.FromArgb(230, 230, 230);
            }
        }

        public override System.Drawing.Color SelectionBorder
        {
            get
            {
                return Color.FromArgb(180, 180, 180);
            }
        }

        public override System.Drawing.Color SelectionTopGradient
        {
            get
            {
                return Color.FromArgb(240, 240, 240);
            }
        }

        public override System.Drawing.Color SelectionMidGradient
        {
            get
            {
                return Color.FromArgb(235, 235, 235);
            }
        }

        public override System.Drawing.Color SelectionBottomGradient
        {
            get
            {
                return Color.FromArgb(230, 230, 230);
            }
        }

        public override System.Drawing.Color PressedBackground
        {
            get
            {
                return Color.FromArgb(232, 232, 232);
            }
        }

        public override System.Drawing.Color TextColor
        {
            get
            {
                return Color.FromArgb(80, 80, 80);
            }
        }

        public override System.Drawing.Color Background
        {
            get
            {
                return Color.FromArgb(188, 199, 216);
            }
        }

        public override System.Drawing.Color DropdownBorder
        {
            get
            {
                return Color.LightGray;
            }
        }

        public override System.Drawing.Color Arrow
        {
            get
            {
                return Color.Black;
            }
        }

        public override System.Drawing.Color OverflowBackground
        {
            get
            {
                return Color.FromArgb(213, 220, 232);
            }
        }
    }

    #endregion
    #region  Renderer

    public class ControlRenderer : ToolStripProfessionalRenderer
    {

        public ControlRenderer()
            : this(new MSColorTable())
        {
        }

        public ControlRenderer(ColorTable ColorTable)
        {
            this.ColorTable = ColorTable;
        }

        private ColorTable _ColorTable;
        public new ColorTable ColorTable
        {
            get
            {
                if (_ColorTable == null)
                {
                    _ColorTable = new MSColorTable();
                }
                return _ColorTable;
            }
            set
            {
                _ColorTable = value;
            }
        }

        protected override void OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);

            // Menu strip bar gradient
            using (LinearGradientBrush LGB = new LinearGradientBrush(e.AffectedBounds, this.ColorTable.BackgroundTopGradient, this.ColorTable.BackgroundBottomGradient, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(LGB, e.AffectedBounds);
            }

        }

        protected override void OnRenderToolStripBorder(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip.Parent == null)
            {
                // Draw border around the menu drop-down
                Rectangle Rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                using (Pen P1 = new Pen(this.ColorTable.CommonColorTable.DropdownBorder))
                {
                    e.Graphics.DrawRectangle(P1, Rect);
                }


                // Fill the gap between menu drop-down and owner item
                using (SolidBrush B1 = new SolidBrush(this.ColorTable.DroppedDownItemBackground))
                {
                    e.Graphics.FillRectangle(B1, e.ConnectedArea);
                }

            }
        }

        protected override void OnRenderMenuItemBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Enabled)
            {
                if (e.Item.Selected)
                {
                    if (!e.Item.IsOnDropDown)
                    {
                        Rectangle SelRect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                        RectDrawing.DrawSelection(e.Graphics, this.ColorTable.CommonColorTable, SelRect);
                    }
                    else
                    {
                        Rectangle SelRect = new Rectangle(2, 0, e.Item.Width - 4, e.Item.Height - 1);
                        RectDrawing.DrawSelection(e.Graphics, this.ColorTable.CommonColorTable, SelRect);
                    }
                }

                if (((ToolStripMenuItem)e.Item).DropDown.Visible && !e.Item.IsOnDropDown)
                {
                    Rectangle BorderRect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height);
                    // Fill the background
                    Rectangle BackgroundRect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height + 2);
                    using (SolidBrush B1 = new SolidBrush(this.ColorTable.DroppedDownItemBackground))
                    {
                        e.Graphics.FillRectangle(B1, BackgroundRect);
                    }


                    // Draw border
                    using (Pen P1 = new Pen(this.ColorTable.CommonColorTable.DropdownBorder))
                    {
                        RectDrawing.DrawRoundedRectangle(e.Graphics, P1, System.Convert.ToSingle(BorderRect.X), System.Convert.ToSingle(BorderRect.Y), System.Convert.ToSingle(BorderRect.Width), System.Convert.ToSingle(BorderRect.Height), 2);
                    }

                }
                e.Item.ForeColor = this.ColorTable.CommonColorTable.TextColor;
            }
        }

        protected override void OnRenderItemText(System.Windows.Forms.ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = this.ColorTable.CommonColorTable.TextColor;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderItemCheck(System.Windows.Forms.ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);

            Rectangle rect = new Rectangle(3, 1, e.Item.Height - 3, e.Item.Height - 3);
            Color c = default(Color);

            if (e.Item.Selected)
            {
                c = this.ColorTable.CommonColorTable.CheckedSelectedBackground;
            }
            else
            {
                c = this.ColorTable.CommonColorTable.CheckedBackground;
            }

            using (SolidBrush b = new SolidBrush(c))
            {
                e.Graphics.FillRectangle(b, rect);
            }


            using (Pen p = new Pen(this.ColorTable.CommonColorTable.SelectionBorder))
            {
                e.Graphics.DrawRectangle(p, rect);
            }


            e.Graphics.DrawString("ü", new Font("Wingdings", 13, FontStyle.Regular), Brushes.Black, new Point(4, 2));
        }

        protected override void OnRenderSeparator(System.Windows.Forms.ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
            int PT1 = 28;
            int PT2 = System.Convert.ToInt32(e.Item.Width);
            int Y = 3;
            using (Pen P1 = new Pen(this.ColorTable.Separator))
            {
                e.Graphics.DrawLine(P1, PT1, Y, PT2, Y);
            }

        }

        protected override void OnRenderImageMargin(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);

            Rectangle BackgroundRect = new Rectangle(0, -1, e.ToolStrip.Width, e.ToolStrip.Height + 1);
            using (LinearGradientBrush LGB = new LinearGradientBrush(BackgroundRect,
                    this.ColorTable.DropdownTopGradient,
                    this.ColorTable.DropdownBottomGradient,
                    LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(LGB, BackgroundRect);
            }


            using (SolidBrush B1 = new SolidBrush(this.ColorTable.ImageMargin))
            {
                e.Graphics.FillRectangle(B1, e.AffectedBounds);
            }

        }

        protected override void OnRenderButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
            bool @checked = System.Convert.ToBoolean(((ToolStripButton)e.Item).Checked);
            bool drawBorder = false;

            if (@checked)
            {
                drawBorder = true;

                if (e.Item.Selected && !e.Item.Pressed)
                {
                    using (SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.CheckedSelectedBackground))
                    {
                        e.Graphics.FillRectangle(b, rect);
                    }

                }
                else
                {
                    using (SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.CheckedBackground))
                    {
                        e.Graphics.FillRectangle(b, rect);
                    }

                }

            }
            else
            {

                if (e.Item.Pressed)
                {
                    drawBorder = true;
                    using (SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.PressedBackground))
                    {
                        e.Graphics.FillRectangle(b, rect);
                    }

                }
                else if (e.Item.Selected)
                {
                    drawBorder = true;
                    RectDrawing.DrawSelection(e.Graphics, this.ColorTable.CommonColorTable, rect);
                }

            }

            if (drawBorder)
            {
                using (Pen p = new Pen(this.ColorTable.CommonColorTable.SelectionBorder))
                {
                    e.Graphics.DrawRectangle(p, rect);
                }

            }
        }

        protected override void OnRenderDropDownButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
            bool drawBorder = false;

            if (e.Item.Pressed)
            {
                drawBorder = true;
                using (SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.PressedBackground))
                {
                    e.Graphics.FillRectangle(b, rect);
                }

            }
            else if (e.Item.Selected)
            {
                drawBorder = true;
                RectDrawing.DrawSelection(e.Graphics, this.ColorTable.CommonColorTable, rect);
            }

            if (drawBorder)
            {
                using (Pen p = new Pen(this.ColorTable.CommonColorTable.SelectionBorder))
                {
                    e.Graphics.DrawRectangle(p, rect);
                }

            }
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderSplitButtonBackground(e);
            bool drawBorder = false;
            bool drawSeparator = true;
            ToolStripSplitButton item = (ToolStripSplitButton)e.Item;
            checked
            {
                Rectangle btnRect = new Rectangle(0, 0, item.ButtonBounds.Width - 1, item.ButtonBounds.Height - 1);
                Rectangle borderRect = new Rectangle(0, 0, item.Bounds.Width - 1, item.Bounds.Height - 1);
                bool flag = item.DropDownButtonPressed;
                if (flag)
                {
                    drawBorder = true;
                    drawSeparator = false;
                    SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.PressedBackground);
                    try
                    {
                        e.Graphics.FillRectangle(b, borderRect);
                    }
                    finally
                    {
                        flag = (b != null);
                        if (flag)
                        {
                            ((IDisposable)b).Dispose();
                        }
                    }
                }
                else
                {
                    flag = item.DropDownButtonSelected;
                    if (flag)
                    {
                        drawBorder = true;
                        RectDrawing.DrawSelection(e.Graphics, this.ColorTable.CommonColorTable, borderRect);
                    }
                }
                flag = item.ButtonPressed;
                if (flag)
                {
                    SolidBrush b2 = new SolidBrush(this.ColorTable.CommonColorTable.PressedBackground);
                    try
                    {
                        e.Graphics.FillRectangle(b2, btnRect);
                    }
                    finally
                    {
                        flag = (b2 != null);
                        if (flag)
                        {
                            ((IDisposable)b2).Dispose();
                        }
                    }
                }
                flag = drawBorder;
                if (flag)
                {
                    Pen p = new Pen(this.ColorTable.CommonColorTable.SelectionBorder);
                    try
                    {
                        e.Graphics.DrawRectangle(p, borderRect);
                        flag = drawSeparator;
                        if (flag)
                        {
                            e.Graphics.DrawRectangle(p, btnRect);
                        }
                    }
                    finally
                    {
                        flag = (p != null);
                        if (flag)
                        {
                            ((IDisposable)p).Dispose();
                        }
                    }
                    this.DrawCustomArrow(e.Graphics, item);
                }
            }
        }


        private void DrawCustomArrow(Graphics g, ToolStripSplitButton item)
        {
            int dropWidth = System.Convert.ToInt32(item.DropDownButtonBounds.Width - 1);
            int dropHeight = System.Convert.ToInt32(item.DropDownButtonBounds.Height - 1);
            float triangleWidth = dropWidth / 2.0F + 1;
            float triangleLeft = System.Convert.ToSingle(item.DropDownButtonBounds.Left + (dropWidth - triangleWidth) / 2.0F);
            float triangleHeight = triangleWidth / 2.0F;
            float triangleTop = System.Convert.ToSingle(item.DropDownButtonBounds.Top + (dropHeight - triangleHeight) / 2.0F + 1);
            RectangleF arrowRect = new RectangleF(triangleLeft, triangleTop, triangleWidth, triangleHeight);

            this.DrawCustomArrow(g, item, Rectangle.Round(arrowRect));
        }

        private void DrawCustomArrow(Graphics g, ToolStripItem item, Rectangle rect)
        {
            ToolStripArrowRenderEventArgs arrowEventArgs = new ToolStripArrowRenderEventArgs(g, item, rect, this.ColorTable.CommonColorTable.Arrow, ArrowDirection.Down);
            base.OnRenderArrow(arrowEventArgs);
        }

        protected override void OnRenderOverflowButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            Rectangle rect = default(Rectangle);
            Rectangle rectEnd = default(Rectangle);
            rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 2);
            rectEnd = new Rectangle(rect.X - 5, rect.Y, rect.Width - 5, rect.Height);

            if (e.Item.Pressed)
            {
                using (SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.PressedBackground))
                {
                    e.Graphics.FillRectangle(b, rect);
                }

            }
            else if (e.Item.Selected)
            {
                RectDrawing.DrawSelection(e.Graphics, this.ColorTable.CommonColorTable, rect);
            }
            else
            {
                using (SolidBrush b = new SolidBrush(this.ColorTable.CommonColorTable.OverflowBackground))
                {
                    e.Graphics.FillRectangle(b, rect);
                }

            }

            using (Pen P1 = new Pen(this.ColorTable.CommonColorTable.Background))
            {
                RectDrawing.DrawRoundedRectangle(e.Graphics, P1, System.Convert.ToSingle(rectEnd.X), System.Convert.ToSingle(rectEnd.Y), System.Convert.ToSingle(rectEnd.Width), System.Convert.ToSingle(rectEnd.Height), 3);
            }


            // Icon
            int w = System.Convert.ToInt32(rect.Width - 1);
            int h = System.Convert.ToInt32(rect.Height - 1);
            float triangleWidth = w / 2.0F + 1;
            float triangleLeft = System.Convert.ToSingle(rect.Left + (w - triangleWidth) / 2.0F + 3);
            float triangleHeight = triangleWidth / 2.0F;
            float triangleTop = System.Convert.ToSingle(rect.Top + (h - triangleHeight) / 2.0F + 7);
            RectangleF arrowRect = new RectangleF(triangleLeft, triangleTop, triangleWidth, triangleHeight);
            this.DrawCustomArrow(e.Graphics, e.Item, Rectangle.Round(arrowRect));

            using (Pen p = new Pen(this.ColorTable.CommonColorTable.Arrow))
            {
                e.Graphics.DrawLine(p, triangleLeft + 2, triangleTop - 2, triangleLeft + triangleWidth - 2, triangleTop - 2);
            }

        }
    }

    #endregion
    #region  Drawing

    public class RectDrawing
    {

        public static void DrawSelection(Graphics G, xColorTable ColorTable, Rectangle Rect)
        {
            Rectangle TopRect = default(Rectangle);
            Rectangle BottomRect = default(Rectangle);
            Rectangle FillRect = new Rectangle(Rect.X + 1, Rect.Y + 1, Rect.Width - 1, Rect.Height - 1);

            TopRect = FillRect;
            TopRect.Height -= System.Convert.ToInt32(TopRect.Height / 2);
            BottomRect = new Rectangle(TopRect.X, TopRect.Bottom, TopRect.Width, FillRect.Height - TopRect.Height);

            // Top gradient
            using (LinearGradientBrush LGB = new LinearGradientBrush(TopRect, ColorTable.SelectionTopGradient, ColorTable.SelectionMidGradient, LinearGradientMode.Vertical))
            {
                G.FillRectangle(LGB, TopRect);
            }


            // Bottom
            using (SolidBrush B1 = new SolidBrush(ColorTable.SelectionBottomGradient))
            {
                G.FillRectangle(B1, BottomRect);
            }


            // Border
            using (Pen P1 = new Pen(ColorTable.SelectionBorder))
            {
                RectDrawing.DrawRoundedRectangle(G, P1, System.Convert.ToSingle(Rect.X), System.Convert.ToSingle(Rect.Y), System.Convert.ToSingle(Rect.Width), System.Convert.ToSingle(Rect.Height), 2);
            }

        }

        public static void DrawRoundedRectangle(Graphics G, Pen P, float X, float Y, float W, float H, float Rad)
        {

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(X + Rad, Y, X + W - (Rad * 2), Y);
                gp.AddArc(X + W - (Rad * 2), Y, Rad * 2, Rad * 2, 270, 90);
                gp.AddLine(X + W, Y + Rad, X + W, Y + H - (Rad * 2));
                gp.AddArc(X + W - (Rad * 2), Y + H - (Rad * 2), Rad * 2, Rad * 2, 0, 90);
                gp.AddLine(X + W - (Rad * 2), Y + H, X + Rad, Y + H);
                gp.AddArc(X, Y + H - (Rad * 2), Rad * 2, Rad * 2, 90, 90);
                gp.AddLine(X, Y + H - (Rad * 2), X, Y + Rad);
                gp.AddArc(X, Y, Rad * 2, Rad * 2, 180, 90);
                gp.CloseFigure();

                G.SmoothingMode = SmoothingMode.AntiAlias;
                G.DrawPath(P, gp);
                G.SmoothingMode = SmoothingMode.Default;
            }

        }
    }

    #endregion

    #endregion
    #region  ThemeContainer

    public class iTalk_ThemeContainer : ContainerControl
    {


        #region  Variables

        private Point MouseP = new Point(0, 0);
        private bool Cap = false;
        private int MoveHeight;
        private string _TextBottom = null;
        const int BorderCurve = 7;
        protected MouseState State;
        private bool HasShown;
        private Rectangle HeaderRect;

        #endregion
        #region  Enums

        public enum MouseState
        {
            None = 0,
            Over = 1,
            Down = 2
        }

        #endregion
        #region  Properties

        private bool _Sizable = true;
        public bool Sizable
        {
            get
            {
                return _Sizable;
            }
            set
            {
                _Sizable = value;
            }
        }

        private bool _SmartBounds = false;
        public bool SmartBounds
        {
            get
            {
                return _SmartBounds;
            }
            set
            {
                _SmartBounds = value;
            }
        }

        private bool _IsParentForm;
        protected bool IsParentForm
        {
            get
            {
                return _IsParentForm;
            }
        }

        protected bool IsParentMdi
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }
                return Parent.Parent != null;
            }
        }

        private bool _ControlMode;
        protected bool ControlMode
        {
            get
            {
                return _ControlMode;
            }
            set
            {
                _ControlMode = value;
                Invalidate();
            }
        }

        private FormStartPosition _StartPosition;
        public FormStartPosition StartPosition
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                {
                    return ParentForm.StartPosition;
                }
                else
                {
                    return _StartPosition;
                }
            }
            set
            {
                _StartPosition = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.StartPosition = value;
                }
            }
        }

        #endregion
        #region  EventArgs

        protected sealed override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent == null)
            {
                return;
            }
            _IsParentForm = Parent is Form;

            if (!_ControlMode)
            {
                InitializeMessages();

                if (_IsParentForm)
                {
                    this.ParentForm.FormBorderStyle = FormBorderStyle.None;
                    this.ParentForm.TransparencyKey = Color.Fuchsia;

                    if (!DesignMode)
                    {
                        ParentForm.Shown += FormShown;
                    }
                }
                Parent.BackColor = BackColor;
                Parent.MinimumSize = new Size(126, 39);
            }
        }

        protected sealed override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!_ControlMode)
            {
                HeaderRect = new Rectangle(0, 0, Width - 14, MoveHeight - 7);
            }
            Invalidate();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                SetState(MouseState.Down);
            }
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
            {
                if (HeaderRect.Contains(e.Location))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[0]);
                }
                else if (_Sizable && !(Previous == 0))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[Previous]);
                }
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cap = false;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
            {
                if (_Sizable && !_ControlMode)
                {
                    InvalidateMouse();
                }
            }
            if (Cap)
            {
                Parent.Location = (System.Drawing.Point)((object)(System.Convert.ToDouble(MousePosition) - System.Convert.ToDouble(MouseP)));
            }
        }

        protected override void OnInvalidated(System.Windows.Forms.InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            ParentForm.Text = Text;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        private void FormShown(object sender, EventArgs e)
        {
            if (_ControlMode || HasShown)
            {
                return;
            }

            if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
            {
                Rectangle SB = Screen.PrimaryScreen.Bounds;
                Rectangle CB = ParentForm.Bounds;
                ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
            }
            HasShown = true;
        }

        #endregion
        #region  Mouse & Size

        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        private Point GetIndexPoint;
        private bool B1x;
        private bool B2x;
        private bool B3;
        private bool B4;
        private int GetIndex()
        {
            GetIndexPoint = PointToClient(MousePosition);
            B1x = GetIndexPoint.X < 7;
            B2x = GetIndexPoint.X > Width - 7;
            B3 = GetIndexPoint.Y < 7;
            B4 = GetIndexPoint.Y > Height - 7;

            if (B1x && B3)
            {
                return 4;
            }
            if (B1x && B4)
            {
                return 7;
            }
            if (B2x && B3)
            {
                return 5;
            }
            if (B2x && B4)
            {
                return 8;
            }
            if (B1x)
            {
                return 1;
            }
            if (B2x)
            {
                return 2;
            }
            if (B3)
            {
                return 3;
            }
            if (B4)
            {
                return 6;
            }
            return 0;
        }

        private int Current;
        private int Previous;
        private void InvalidateMouse()
        {
            Current = GetIndex();
            if (Current == Previous)
            {
                return;
            }

            Previous = Current;
            switch (Previous)
            {
                case 0:
                    Cursor = Cursors.Default;
                    break;
                case 6:
                    Cursor = Cursors.SizeNS;
                    break;
                case 8:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case 7:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        private Message[] Messages = new Message[9];
        private void InitializeMessages()
        {
            Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
            for (int I = 1; I <= 8; I++)
            {
                Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
            }
        }

        private void CorrectBounds(Rectangle bounds)
        {
            if (Parent.Width > bounds.Width)
            {
                Parent.Width = bounds.Width;
            }
            if (Parent.Height > bounds.Height)
            {
                Parent.Height = bounds.Height;
            }

            int X = Parent.Location.X;
            int Y = Parent.Location.Y;

            if (X < bounds.X)
            {
                X = bounds.X;
            }
            if (Y < bounds.Y)
            {
                Y = bounds.Y;
            }

            int Width = bounds.X + bounds.Width;
            int Height = bounds.Y + bounds.Height;

            if (X + Parent.Width > Width)
            {
                X = Width - Parent.Width;
            }
            if (Y + Parent.Height > Height)
            {
                Y = Height - Parent.Height;
            }

            Parent.Location = new Point(X, Y);
        }

        private bool WM_LMBUTTONDOWN;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (WM_LMBUTTONDOWN && m.Msg == 513)
            {
                WM_LMBUTTONDOWN = false;

                SetState(MouseState.Over);
                if (!_SmartBounds)
                {
                    return;
                }

                if (IsParentMdi)
                {
                    CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
                }
                else
                {
                    CorrectBounds(Screen.FromControl(Parent).WorkingArea);
                }
            }
        }

        #endregion

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.ParentForm.FormBorderStyle = FormBorderStyle.None;
            this.ParentForm.TransparencyKey = Color.Fuchsia;
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();
        }

        public iTalk_ThemeContainer()
        {
            SetStyle((ControlStyles)(139270), true);
            Dock = DockStyle.Fill;
            MoveHeight = 25;
            Padding = new Padding(3, 28, 3, 28);
            Font = new Font("Segoe UI", 8, FontStyle.Regular);
            ForeColor = Color.FromArgb(142, 142, 142);
            BackColor = Color.FromArgb(246, 246, 246);
            DoubleBuffered = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            Rectangle ClientRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            Color TransparencyKey = this.ParentForm.TransparencyKey;

            G.SmoothingMode = SmoothingMode.Default;
            G.Clear(TransparencyKey);

            // Draw the container borders
            G.FillPath(new SolidBrush(Color.FromArgb(52, 52, 52)), RoundRectangle.RoundRect(ClientRectangle, BorderCurve));
            // Draw a rectangle in which the controls should be added on
            G.FillPath(new SolidBrush(Color.FromArgb(246, 246, 246)), RoundRectangle.RoundRect(new Rectangle(2, 20, Width - 5, Height - 42), BorderCurve));

            // Patch the header with a rectangle that has a curve so its border will remain within container bounds
            G.FillPath(new SolidBrush(Color.FromArgb(52, 52, 52)), RoundRectangle.RoundRect(new Rectangle(2, 2, (int)(Width / 2 + 2), 16), BorderCurve));
            G.FillPath(new SolidBrush(Color.FromArgb(52, 52, 52)), RoundRectangle.RoundRect(new Rectangle((int)(Width / 2 - 3), 2, (int)(Width / 2), 16), BorderCurve));
            // Fill the header rectangle below the patch
            G.FillRectangle(new SolidBrush(Color.FromArgb(52, 52, 52)), new Rectangle(2, 15, Width - 5, 10));

            // Increase the thickness of the container borders
            G.DrawPath(new Pen(Color.FromArgb(52, 52, 52)), RoundRectangle.RoundRect(new Rectangle(2, 2, Width - 5, Height - 5), BorderCurve));
            G.DrawPath(new Pen(Color.FromArgb(52, 52, 52)), RoundRectangle.RoundRect(ClientRectangle, BorderCurve));

            // Draw the string from the specified 'Text' property on the header rectangle
            G.DrawString(Text, new Font("Trebuchet MS", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(221, 221, 221)), new Rectangle(BorderCurve, BorderCurve - 4, Width - 1, 22), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });


            // Draws a rectangle at the bottom of the container
            G.FillRectangle(new SolidBrush(Color.FromArgb(52, 52, 52)), 0, Height - 25, Width - 3, 22 - 2);
            G.DrawLine(new Pen(Color.FromArgb(52, 52, 52)), 5, Height - 5, Width - 6, Height - 5);
            G.DrawLine(new Pen(Color.FromArgb(52, 52, 52)), 7, Height - 4, Width - 7, Height - 4);

            G.DrawString(_TextBottom, new Font("Trebuchet MS", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(221, 221, 221)), 5, Height - 23);

            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region ControlBox

    public class iTalk_ControlBox : Control
    {

        #region Enums

        public enum MouseState : byte
        {
            None = 0,
            Over = 1,
            Down = 2,
            Block = 3
        }

        #endregion
        #region Variables

        MouseState State = MouseState.None;
        int i;
        Rectangle CloseRect = new Rectangle(28, 0, 47, 18);
        Rectangle MinimizeRect = new Rectangle(0, 0, 28, 18);

        #endregion
        #region EventArgs

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (i > 0 & i < 28)
            {
                this.FindForm().WindowState = FormWindowState.Minimized;
            }
            else if (i > 30 & i < 75)
            {
                this.FindForm().Close();
            }

            State = MouseState.Down;
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            State = MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            State = MouseState.None;
            Invalidate();
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            i = e.Location.X;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = 77;
            Height = 19;
        }

        #endregion

        public iTalk_ControlBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Point location = new Point(checked(this.FindForm().Width - 81), -1);
            this.Location = location;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            GraphicsPath GP_MinimizeRect = new GraphicsPath();
            GraphicsPath GP_CloseRect = new GraphicsPath();

            GP_MinimizeRect.AddRectangle(MinimizeRect);
            GP_CloseRect.AddRectangle(CloseRect);
            G.Clear(BackColor);

            switch (State)
            {
                case MouseState.None:
                NonePoint:
                    LinearGradientBrush MinimizeGradient = new LinearGradientBrush(MinimizeRect, Color.FromArgb(73, 73, 73), Color.FromArgb(58, 58, 58), 90);
                    G.FillPath(MinimizeGradient, GP_MinimizeRect);
                    G.DrawPath(new Pen(Color.FromArgb(40, 40, 40)), GP_MinimizeRect);
                    G.DrawString("0", new Font("Marlett", 11, FontStyle.Regular), new SolidBrush(Color.FromArgb(221, 221, 221)), MinimizeRect.Width - 22, MinimizeRect.Height - 16);

                    LinearGradientBrush CloseGradient = new LinearGradientBrush(CloseRect, Color.FromArgb(73, 73, 73), Color.FromArgb(58, 58, 58), 90);
                    G.FillPath(CloseGradient, GP_CloseRect);
                    G.DrawPath(new Pen(Color.FromArgb(40, 40, 40)), GP_CloseRect);
                    G.DrawString("r", new Font("Marlett", 11, FontStyle.Regular), new SolidBrush(Color.FromArgb(221, 221, 221)), CloseRect.Width - 4, CloseRect.Height - 16);
                    break;
                case MouseState.Over:
                    if (i > 0 & i < 28)
                    {
                        LinearGradientBrush xMinimizeGradient = new LinearGradientBrush(this.MinimizeRect, Color.FromArgb(76, 76, 76, 76), Color.FromArgb(48, 48, 48), 90f);
                        G.FillPath(xMinimizeGradient, GP_MinimizeRect);
                        G.DrawPath(new Pen(Color.FromArgb(40, 40, 40)), GP_MinimizeRect);
                        G.DrawString("0", new Font("Marlett", 11, FontStyle.Regular), new SolidBrush(Color.FromArgb(221, 221, 221)), MinimizeRect.Width - 22, MinimizeRect.Height - 16);

                        LinearGradientBrush xCloseGradient = new LinearGradientBrush(CloseRect, Color.FromArgb(73, 73, 73), Color.FromArgb(58, 58, 58), 90);
                        G.FillPath(xCloseGradient, GP_CloseRect);
                        G.DrawPath(new Pen(Color.FromArgb(40, 40, 40)), GP_CloseRect);
                        G.DrawString("r", new Font("Marlett", 11, FontStyle.Regular), new SolidBrush(Color.FromArgb(221, 221, 221)), CloseRect.Width - 4, CloseRect.Height - 16);
                    }
                    else if (i > 30 & i < 75)
                    {
                        LinearGradientBrush xCloseGradient = new LinearGradientBrush(CloseRect, Color.FromArgb(76, 76, 76, 76), Color.FromArgb(48, 48, 48), 90);
                        G.FillPath(xCloseGradient, GP_CloseRect);
                        G.DrawPath(new Pen(Color.FromArgb(40, 40, 40)), GP_CloseRect);
                        G.DrawString("r", new Font("Marlett", 11, FontStyle.Regular), new SolidBrush(Color.FromArgb(221, 221, 221)), CloseRect.Width - 4, CloseRect.Height - 16);

                        LinearGradientBrush xMinimizeGradient = new LinearGradientBrush(MinimizeRect, Color.FromArgb(73, 73, 73), Color.FromArgb(58, 58, 58), 90);
                        G.FillPath(xMinimizeGradient, RoundRectangle.RoundRect(MinimizeRect, 1));
                        G.DrawPath(new Pen(Color.FromArgb(40, 40, 40)), GP_MinimizeRect);
                        G.DrawString("0", new Font("Marlett", 11, FontStyle.Regular), new SolidBrush(Color.FromArgb(221, 221, 221)), MinimizeRect.Width - 22, MinimizeRect.Height - 16);
                    }
                    else
                    {
                        goto NonePoint; // Return to [MouseState = None]     
                    }
                    break;
            }

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            GP_CloseRect.Dispose();
            GP_MinimizeRect.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region Button 1

    public class iTalk_Button_1 : Control
    {

        #region Variables

        private int MouseState;
        private GraphicsPath Shape;
        private LinearGradientBrush InactiveGB;
        private LinearGradientBrush PressedGB;
        private LinearGradientBrush PressedContourGB;
        private Rectangle R1;
        private Pen P1;
        private Pen P3;
        private Image _Image;
        private Size _ImageSize;
        private StringAlignment _TextAlignment = StringAlignment.Center;
        private Color _TextColor = Color.FromArgb(150, 150, 150);
        private ContentAlignment _ImageAlign = ContentAlignment.MiddleLeft;

        #endregion
        #region Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = default(PointF);
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = Convert.ToSingle((Area.Width - ImageArea.Width) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.X = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.X = Area.Width - ImageArea.Width - 2;

                    break;
            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = Convert.ToSingle((Area.Height - ImageArea.Height) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.Y = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.Y = Area.Height - ImageArea.Height - 2;
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Far;
                    break;
            }
            return SF;
        }

        #endregion
        #region Properties

        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        public ContentAlignment ImageAlign
        {
            get { return _ImageAlign; }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        public StringAlignment TextAlignment
        {
            get { return this._TextAlignment; }
            set
            {
                this._TextAlignment = value;
                this.Invalidate();
            }
        }

        public override Color ForeColor
        {
            get { return this._TextColor; }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseState = 1;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion

        public iTalk_Button_1()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 12);
            ForeColor = Color.FromArgb(150, 150, 150);
            Size = new Size(166, 40);
            _TextAlignment = StringAlignment.Center;
            P1 = new Pen(Color.FromArgb(190, 190, 190)); // P1 = Border color
        }

        protected override void OnResize(System.EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                Shape = new GraphicsPath();
                R1 = new Rectangle(0, 0, Width, Height);

                InactiveGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(251, 251, 251), Color.FromArgb(225, 225, 225), 90f);
                PressedGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(235, 235, 235), Color.FromArgb(223, 223, 223), 90f);
                PressedContourGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(167, 167, 167), Color.FromArgb(167, 167, 167), 90f);

                P3 = new Pen(PressedContourGB);
            }

            var _Shape = Shape;
            _Shape.AddArc(0, 0, 10, 10, 180, 90);
            _Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            _Shape.CloseAllFigures();

            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var _G = e.Graphics;
            _G.SmoothingMode = SmoothingMode.HighQuality;
            PointF ipt = ImageLocation(GetStringFormat(ImageAlign), Size, ImageSize);

            switch (MouseState)
            {
                case 0:
                    _G.FillPath(InactiveGB, Shape);
                    _G.DrawPath(P1, Shape);
                    if ((Image == null))
                    {
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        _G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
                case 1:
                    _G.FillPath(PressedGB, Shape);
                    _G.DrawPath(P3, Shape);

                    if ((Image == null))
                    {
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        _G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
            }
            base.OnPaint(e);
        }
    }

    #endregion
    #region Button 2

    class iTalk_Button_2 : Control
    {

        #region Variables

        private int MouseState;
        private GraphicsPath Shape;
        private LinearGradientBrush InactiveGB;
        private LinearGradientBrush PressedGB;
        private LinearGradientBrush PressedContourGB;
        private Rectangle R1;
        private Pen P1;
        private Pen P3;
        private Image _Image;
        private Size _ImageSize;
        private StringAlignment _TextAlignment = StringAlignment.Center;
        private ContentAlignment _ImageAlign = ContentAlignment.MiddleLeft;

        #endregion
        #region Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = default(PointF);
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = Convert.ToSingle((Area.Width - ImageArea.Width) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.X = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.X = Area.Width - ImageArea.Width - 2;
                    break;
            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = Convert.ToSingle((Area.Height - ImageArea.Height) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.Y = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.Y = Area.Height - ImageArea.Height - 2;
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Far;
                    break;
            }
            return SF;
        }

        #endregion
        #region Properties

        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        public StringAlignment TextAlignment
        {
            get { return this._TextAlignment; }
            set
            {
                this._TextAlignment = value;
                this.Invalidate();
            }
        }

        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        public ContentAlignment ImageAlign
        {
            get { return _ImageAlign; }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseState = 1;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = 0;
            // [Inactive]
            Invalidate();
            // Update control
            base.OnMouseLeave(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion

        public iTalk_Button_2()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 14);
            ForeColor = Color.White;
            Size = new Size(166, 40);
            _TextAlignment = StringAlignment.Center;
            P1 = new Pen(Color.FromArgb(0, 118, 176));
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {
                Shape = new GraphicsPath();
                R1 = new Rectangle(0, 0, Width, Height);

                InactiveGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(0, 176, 231), Color.FromArgb(0, 152, 224), 90f);
                PressedGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(0, 118, 176), Color.FromArgb(0, 149, 222), 90f);
                PressedContourGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(0, 118, 176), Color.FromArgb(0, 118, 176), 90f);

                P3 = new Pen(PressedContourGB);
            }

            var _Shape = Shape;
            _Shape.AddArc(0, 0, 10, 10, 180, 90);
            _Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            _Shape.CloseAllFigures();

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var _G = e.Graphics;
            _G.SmoothingMode = SmoothingMode.HighQuality;

            PointF ipt = ImageLocation(GetStringFormat(ImageAlign), Size, ImageSize);

            switch (MouseState)
            {
                case 0:
                    _G.FillPath(InactiveGB, Shape);
                    _G.DrawPath(P1, Shape);
                    if ((Image == null))
                    {
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        _G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
                case 1:
                    _G.FillPath(PressedGB, Shape);
                    _G.DrawPath(P3, Shape);
                    if ((Image == null))
                    {
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        _G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        _G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
            }
            base.OnPaint(e);
        }
    }

    #endregion
    #region Toggle Button

    [DefaultEvent("ToggledChanged")]
    class iTalk_Toggle : Control
    {

        #region Designer

        //|------DO-NOT-REMOVE------|
        //|---------CREDITS---------|

        // Pill class and functions were originally created by Tedd
        // Last edited by Tedd on: 12/20/2013
        // Modified by HazelDev on: 1/4/2014

        //|---------CREDITS---------|
        //|------DO-NOT-REMOVE------|

        public class PillStyle
        {
            public bool Left;
            public bool Right;
        }

        public GraphicsPath Pill(Rectangle Rectangle, PillStyle PillStyle)
        {
            GraphicsPath functionReturnValue = default(GraphicsPath);
            functionReturnValue = new GraphicsPath();

            if (PillStyle.Left)
            {
                functionReturnValue.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Height, Rectangle.Height), -270, 180);
            }
            else
            {
                functionReturnValue.AddLine(Rectangle.X, Rectangle.Y + Rectangle.Height, Rectangle.X, Rectangle.Y);
            }

            if (PillStyle.Right)
            {
                functionReturnValue.AddArc(new Rectangle(Rectangle.X + Rectangle.Width - Rectangle.Height, Rectangle.Y, Rectangle.Height, Rectangle.Height), -90, 180);
            }
            else
            {
                functionReturnValue.AddLine(Rectangle.X + Rectangle.Width, Rectangle.Y, Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height);
            }

            functionReturnValue.CloseAllFigures();
            return functionReturnValue;
        }

        public object Pill(int X, int Y, int Width, int Height, PillStyle PillStyle)
        {
            return Pill(new Rectangle(X, Y, Width, Height), PillStyle);
        }

        #endregion
        #region Enums

        public enum _Type
        {
            YesNo,
            OnOff,
            IO
        }

        #endregion
        #region Variables

        private Timer AnimationTimer = new Timer { Interval = 1 };
        private int ToggleLocation = 0;
        public event ToggledChangedEventHandler ToggledChanged;
        public delegate void ToggledChangedEventHandler();
        private bool _Toggled;
        private _Type ToggleType;
        private Rectangle Bar;
        private Size cHandle = new Size(15, 20);

        #endregion
        #region Properties

        public bool Toggled
        {
            get { return _Toggled; }
            set
            {
                _Toggled = value;
                Invalidate();

                if (ToggledChanged != null)
                {
                    ToggledChanged();
                }
            }
        }

        public _Type Type
        {
            get { return ToggleType; }
            set
            {
                ToggleType = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = 41;
            Height = 23;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Toggled = !Toggled;
        }

        #endregion

        public iTalk_Toggle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            AnimationTimer.Tick += new EventHandler(AnimationTimer_Tick);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AnimationTimer.Start();
        }

        void AnimationTimer_Tick(object sender, EventArgs e)
        {
            //  Create a slide animation when toggled on/off
            if ((_Toggled == true))
            {
                if ((ToggleLocation < 100))
                {
                    ToggleLocation += 10;
                    this.Invalidate(false);
                }
            }
            else if ((ToggleLocation > 0))
            {
                ToggleLocation -= 10;
                this.Invalidate(false);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            G.Clear(Parent.BackColor);
            checked
            {
                Point point = new Point(0, (int)Math.Round(unchecked((double)this.Height / 2.0 - (double)this.cHandle.Height / 2.0)));
                Point arg_A8_0 = point;
                Point point2 = new Point(0, (int)Math.Round(unchecked((double)this.Height / 2.0 + (double)this.cHandle.Height / 2.0)));
                LinearGradientBrush Gradient = new LinearGradientBrush(arg_A8_0, point2, Color.FromArgb(250, 250, 250), Color.FromArgb(240, 240, 240));
                this.Bar = new Rectangle(8, 10, this.Width - 21, this.Height - 21);

                G.SmoothingMode = SmoothingMode.AntiAlias;
                G.FillPath(Gradient, (GraphicsPath)this.Pill(0, (int)Math.Round(unchecked((double)this.Height / 2.0 - (double)this.cHandle.Height / 2.0)), this.Width - 1, this.cHandle.Height - 5, new iTalk_Toggle.PillStyle
                {
                    Left = true,
                    Right = true
                }));
                G.DrawPath(new Pen(Color.FromArgb(177, 177, 176)), (GraphicsPath)this.Pill(0, (int)Math.Round(unchecked((double)this.Height / 2.0 - (double)this.cHandle.Height / 2.0)), this.Width - 1, this.cHandle.Height - 5, new iTalk_Toggle.PillStyle
                {
                    Left = true,
                    Right = true
                }));
                Gradient.Dispose();
                switch (this.ToggleType)
                {
                    case iTalk_Toggle._Type.YesNo:
                        {
                            bool toggled = this.Toggled;
                            if (toggled)
                            {
                                G.DrawString("Yes", new Font("Segoe UI", 7f, FontStyle.Regular), Brushes.Gray, (float)(this.Bar.X + 7), (float)this.Bar.Y, new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                            }
                            else
                            {
                                G.DrawString("No", new Font("Segoe UI", 7f, FontStyle.Regular), Brushes.Gray, (float)(this.Bar.X + 18), (float)this.Bar.Y, new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                            }
                            break;
                        }
                    case iTalk_Toggle._Type.OnOff:
                        {
                            bool toggled = this.Toggled;
                            if (toggled)
                            {
                                G.DrawString("On", new Font("Segoe UI", 7f, FontStyle.Regular), Brushes.Gray, (float)(this.Bar.X + 7), (float)this.Bar.Y, new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                            }
                            else
                            {
                                G.DrawString("Off", new Font("Segoe UI", 7f, FontStyle.Regular), Brushes.Gray, (float)(this.Bar.X + 18), (float)this.Bar.Y, new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                            }
                            break;
                        }
                    case iTalk_Toggle._Type.IO:
                        {
                            bool toggled = this.Toggled;
                            if (toggled)
                            {
                                G.DrawString("I", new Font("Segoe UI", 7f, FontStyle.Regular), Brushes.Gray, (float)(this.Bar.X + 7), (float)this.Bar.Y, new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                            }
                            else
                            {
                                G.DrawString("O", new Font("Segoe UI", 7f, FontStyle.Regular), Brushes.Gray, (float)(this.Bar.X + 18), (float)this.Bar.Y, new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                });
                            }
                            break;
                        }
                }
                G.FillEllipse(new SolidBrush(Color.FromArgb(249, 249, 249)), this.Bar.X + (int)Math.Round(unchecked((double)this.Bar.Width * ((double)this.ToggleLocation / 80.0))) - (int)Math.Round((double)this.cHandle.Width / 2.0), this.Bar.Y + (int)Math.Round((double)this.Bar.Height / 2.0) - (int)Math.Round(unchecked((double)this.cHandle.Height / 2.0 - 1.0)), this.cHandle.Width, this.cHandle.Height - 5);
                G.DrawEllipse(new Pen(Color.FromArgb(177, 177, 176)), this.Bar.X + (int)Math.Round(unchecked((double)this.Bar.Width * ((double)this.ToggleLocation / 80.0) - (double)checked((int)Math.Round((double)this.cHandle.Width / 2.0)))), this.Bar.Y + (int)Math.Round((double)this.Bar.Height / 2.0) - (int)Math.Round(unchecked((double)this.cHandle.Height / 2.0 - 1.0)), this.cHandle.Width, this.cHandle.Height - 5);
            }
        }
    }
    #endregion
    #region Label

    class iTalk_Label : Label
    {

        public iTalk_Label()
        {
            Font = new Font("Segoe UI", 8);
            ForeColor = Color.FromArgb(142, 142, 142);
            BackColor = Color.Transparent;
        }
    }

    #endregion
    #region Link Label

    class iTalk_LinkLabel : LinkLabel
    {

        public iTalk_LinkLabel()
        {
            Font = new Font("Segoe UI", 8, FontStyle.Regular);
            BackColor = Color.Transparent;
            LinkColor = Color.FromArgb(51, 153, 225);
            ActiveLinkColor = Color.FromArgb(0, 101, 202);
            VisitedLinkColor = Color.FromArgb(0, 101, 202);
            LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
        }
    }

    #endregion
    #region Header Label

    class iTalk_HeaderLabel : Label
    {

        public iTalk_HeaderLabel()
        {
            Font = new Font("Segoe UI", 25, FontStyle.Regular);
            ForeColor = Color.FromArgb(80, 80, 80);
            BackColor = Color.Transparent;
        }
    }

    #endregion
    #region Big TextBox

    [DefaultEvent("TextChanged")]
    class iTalk_TextBox_Big : Control
    {
        #region Variables

        public TextBox iTalkTB = new TextBox();
        private GraphicsPath Shape;
        private int _maxchars = 32767;
        private bool _ReadOnly;
        private bool _Multiline;
        private Image _Image;
        private Size _ImageSize;
        private HorizontalAlignment ALNType;
        private bool isPasswordMasked = false;
        private Pen P1;
        private SolidBrush B1;

        #endregion
        #region Properties

        public HorizontalAlignment TextAlignment
        {
            get { return ALNType; }
            set
            {
                ALNType = value;
                Invalidate();
            }
        }
        public int MaxLength
        {
            get { return _maxchars; }
            set
            {
                _maxchars = value;
                iTalkTB.MaxLength = MaxLength;
                Invalidate();
            }
        }

        public bool UseSystemPasswordChar
        {
            get { return isPasswordMasked; }
            set
            {
                iTalkTB.UseSystemPasswordChar = UseSystemPasswordChar;
                isPasswordMasked = value;
                Invalidate();
            }
        }
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                if (iTalkTB != null)
                {
                    iTalkTB.ReadOnly = value;
                }
            }
        }
        public bool Multiline
        {
            get { return _Multiline; }
            set
            {
                _Multiline = value;
                if (iTalkTB != null)
                {
                    iTalkTB.Multiline = value;

                    if (value)
                    {
                        iTalkTB.Height = Height - 23;
                    }
                    else
                    {
                        Height = iTalkTB.Height + 23;
                    }
                }

            }
        }

        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;

                if (Image == null)
                {
                    iTalkTB.Location = new Point(8, 10);
                }
                else
                {
                    iTalkTB.Location = new Point(35, 11);
                }
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
        }

        #endregion
        #region EventArgs

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            iTalkTB.Text = Text;
            Invalidate();
        }

        private void OnBaseTextChanged(object s, EventArgs e)
        {
            Text = iTalkTB.Text;
        }

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            iTalkTB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
            iTalkTB.Font = Font;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        private void _OnKeyDown(object Obj, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                iTalkTB.SelectAll();
                e.SuppressKeyPress = true;
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                iTalkTB.Copy();
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (_Multiline)
            {
                iTalkTB.Height = Height - 23;
            }
            else
            {
                Height = iTalkTB.Height + 23;
            }

            Shape = new GraphicsPath();
            var _with1 = Shape;
            _with1.AddArc(0, 0, 10, 10, 180, 90);
            _with1.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _with1.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _with1.AddArc(0, Height - 11, 10, 10, 90, 90);
            _with1.CloseAllFigures();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            iTalkTB.Focus();
        }

        #endregion
        public void AddTextBox()
        {
            var _TB = iTalkTB;
            _TB.Location = new Point(7, 10);
            _TB.Text = string.Empty;
            _TB.BorderStyle = BorderStyle.None;
            _TB.TextAlign = HorizontalAlignment.Left;
            _TB.Font = new Font("Tahoma", 11);
            _TB.UseSystemPasswordChar = UseSystemPasswordChar;
            _TB.Multiline = false;
            _TB.CharacterCasing = CharacterCasing.Normal;
            iTalkTB.KeyDown += _OnKeyDown;
            iTalkTB.TextChanged += OnBaseTextChanged;
        }

        public iTalk_TextBox_Big()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            AddTextBox();
            Controls.Add(iTalkTB);

            P1 = new Pen(Color.FromArgb(180, 180, 180)); // P1 = Border color
            B1 = new SolidBrush(Color.White); // B1 = Rect Background color
            BackColor = Color.Transparent;
            ForeColor = Color.DimGray;

            Text = null;
            Font = new Font("Tahoma", 11);
            Size = new Size(135, 43);
            DoubleBuffered = true;
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.AntiAlias;

            if (Image == null)
            {
                iTalkTB.Width = Width - 18;
            }
            else
            {
                iTalkTB.Width = Width - 45;
            }

            iTalkTB.TextAlign = TextAlignment;
            iTalkTB.UseSystemPasswordChar = UseSystemPasswordChar;

            G.Clear(Color.Transparent);
            G.FillPath(B1, Shape); // Draw background
            G.DrawPath(P1, Shape); // Draw border


            if (Image != null)
            {
                G.DrawImage(_Image, 5, 8, 24, 24);
                // 24x24 is the perfect size of the image
            }

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region Small TextBox

    [DefaultEvent("TextChanged")]
    class iTalk_TextBox_Small : Control
    {
        #region Variables

        public TextBox iTalkTB = new TextBox();
        private GraphicsPath Shape;
        private int _maxchars = 32767;
        private bool _ReadOnly;
        private bool _Multiline;
        private HorizontalAlignment ALNType;
        private bool isPasswordMasked = false;
        private Pen P1;
        private SolidBrush B1;

        #endregion
        #region Properties

        public HorizontalAlignment TextAlignment
        {
            get { return ALNType; }
            set
            {
                ALNType = value;
                Invalidate();
            }
        }
        public int MaxLength
        {
            get { return _maxchars; }
            set
            {
                _maxchars = value;
                iTalkTB.MaxLength = MaxLength;
                Invalidate();
            }
        }

        public bool UseSystemPasswordChar
        {
            get { return isPasswordMasked; }
            set
            {
                iTalkTB.UseSystemPasswordChar = UseSystemPasswordChar;
                isPasswordMasked = value;
                Invalidate();
            }
        }
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                if (iTalkTB != null)
                {
                    iTalkTB.ReadOnly = value;
                }
            }
        }
        public bool Multiline
        {
            get { return _Multiline; }
            set
            {
                _Multiline = value;
                if (iTalkTB != null)
                {
                    iTalkTB.Multiline = value;

                    if (value)
                    {
                        iTalkTB.Height = Height - 10;
                    }
                    else
                    {
                        Height = iTalkTB.Height + 10;
                    }
                }
            }
        }

        #endregion
        #region EventArgs

        private void CheckEnter(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            iTalkTB.Text = Text;
            Invalidate();
        }

        private void OnBaseTextChanged(object s, EventArgs e)
        {
            Text = iTalkTB.Text;
        }

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            iTalkTB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
            iTalkTB.Font = Font;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        private void _OnKeyDown(object Obj, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                iTalkTB.SelectAll();
                e.SuppressKeyPress = true;
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                iTalkTB.Copy();
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (_Multiline)
            {
                iTalkTB.Height = Height - 10;
            }
            else
            {
                Height = iTalkTB.Height + 10;
            }

            Shape = new GraphicsPath();
            var _with1 = Shape;
            _with1.AddArc(0, 0, 10, 10, 180, 90);
            _with1.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _with1.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _with1.AddArc(0, Height - 11, 10, 10, 90, 90);
            _with1.CloseAllFigures();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            iTalkTB.Focus();
        }

        #endregion
        public void AddTextBox()
        {
            var _TB = iTalkTB;
            _TB.Size = new Size(Width - 10, 33);
            _TB.Location = new Point(7, 5);
            _TB.Text = string.Empty;
            _TB.BorderStyle = BorderStyle.None;
            _TB.TextAlign = HorizontalAlignment.Left;
            _TB.Font = new Font("Tahoma", 11);
            _TB.UseSystemPasswordChar = UseSystemPasswordChar;
            _TB.Multiline = false;
            iTalkTB.KeyDown += _OnKeyDown;
            iTalkTB.TextChanged += OnBaseTextChanged;
            iTalkTB.KeyPress += CheckEnter;
        }

        public iTalk_TextBox_Small()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            AddTextBox();
            Controls.Add(iTalkTB);

            P1 = new Pen(Color.FromArgb(180, 180, 180)); // P1 = Border color
            B1 = new SolidBrush(Color.White); // B1 = Rect Background color
            BackColor = Color.Transparent;
            ForeColor = Color.DimGray;

            Text = null;
            Font = new Font("Tahoma", 11);
            Size = new Size(135, 33);
            DoubleBuffered = true;
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.AntiAlias;

            var _TB = iTalkTB;
            _TB.Width = Width - 10;
            _TB.TextAlign = TextAlignment;
            _TB.UseSystemPasswordChar = UseSystemPasswordChar;

            G.Clear(Color.Transparent);
            G.FillPath(B1, Shape); // Draw background
            G.DrawPath(P1, Shape); // Draw border

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }

    }

    #endregion
    #region RichTextBox

    [DefaultEvent("TextChanged")]
    class iTalk_RichTextBox : Control
    {

        #region Variables

        public RichTextBox iTalkRTB = new RichTextBox();
        private bool _ReadOnly;
        private bool _WordWrap;
        private bool _AutoWordSelection;
        private GraphicsPath Shape;

        #endregion
        #region Properties

        [Localizable(false)]
        public override string Text
        {
            get { return iTalkRTB.Text; }
            set
            {
                iTalkRTB.Text = value;
                Invalidate();
            }
        }
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                if (iTalkRTB != null)
                {
                    iTalkRTB.ReadOnly = value;
                }
            }
        }
        public bool WordWrap
        {
            get { return _WordWrap; }
            set
            {
                _WordWrap = value;
                if (iTalkRTB != null)
                {
                    iTalkRTB.WordWrap = value;
                }
            }
        }
        public bool AutoWordSelection
        {
            get { return _AutoWordSelection; }
            set
            {
                _AutoWordSelection = value;
                if (iTalkRTB != null)
                {
                    iTalkRTB.AutoWordSelection = value;
                }
            }
        }
        #endregion
        #region EventArgs

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            iTalkRTB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
            iTalkRTB.Font = Font;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            iTalkRTB.Size = new Size(Width - 13, Height - 11);
        }


        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            Shape = new GraphicsPath();
            var _Shape = Shape;
            _Shape.AddArc(0, 0, 10, 10, 180, 90);
            _Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            _Shape.CloseAllFigures();
        }

        public void _TextChanged(object s, EventArgs e)
        {
            iTalkRTB.Text = Text;
        }

        #endregion

        public void AddRichTextBox()
        {
            var _RTB = iTalkRTB;
            _RTB.BackColor = Color.White;
            _RTB.Size = new Size(Width - 10, 100);
            _RTB.Location = new Point(7, 5);
            _RTB.Text = string.Empty;
            _RTB.BorderStyle = BorderStyle.None;
            _RTB.Font = new Font("Tahoma", 10);
            _RTB.Multiline = true;
        }

        public iTalk_RichTextBox()
            : base()
        {

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            AddRichTextBox();
            Controls.Add(iTalkRTB);
            BackColor = Color.Transparent;
            ForeColor = Color.DimGray;

            Text = null;
            Font = new Font("Tahoma", 10);
            Size = new Size(150, 100);
            WordWrap = true;
            AutoWordSelection = false;
            DoubleBuffered = true;

            TextChanged += _TextChanged;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(B);
            G.SmoothingMode = SmoothingMode.AntiAlias;
            G.Clear(Color.Transparent);
            G.FillPath(Brushes.White, this.Shape);
            G.DrawPath(new Pen(Color.FromArgb(180, 180, 180)), this.Shape);
            G.Dispose();
            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            B.Dispose();
        }
    }

    #endregion
    #region  NumericUpDown

    public class iTalk_NumericUpDown : Control
    {

        #region  Enums

        public enum _TextAlignment
        {
            Near,
            Center
        }

        #endregion
        #region  Variables

        private GraphicsPath Shape;
        private Pen P1;
        private SolidBrush B1;

        private long _Value;
        private long _Minimum;
        private long _Maximum;
        private int Xval;
        private int Yval;
        private bool KeyboardNum;
        private _TextAlignment MyStringAlignment;

        #endregion
        #region  Properties

        public long Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value <= _Maximum & value >= _Minimum)
                {
                    _Value = value;
                }
                Invalidate();
            }
        }

        public long Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                if (value < _Maximum)
                {
                    _Minimum = value;
                }
                if (_Value < _Minimum)
                {
                    _Value = Minimum;
                }
                Invalidate();
            }
        }

        public long Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                if (value > _Minimum)
                {
                    _Maximum = value;
                }
                if (_Value > _Maximum)
                {
                    _Value = _Maximum;
                }
                Invalidate();
            }
        }

        public _TextAlignment TextAlignment
        {
            get
            {
                return MyStringAlignment;
            }
            set
            {
                MyStringAlignment = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            Height = 28;
            Shape = new GraphicsPath();
            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Xval = e.Location.X;
            Yval = e.Location.Y;
            Invalidate();

            if (e.X < Width - 24)
            {
                Cursor = Cursors.IBeam;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (Xval > this.Width - 23 && Xval < this.Width - 3)
            {
                if (Yval < 15)
                {
                    if ((Value + 1) <= _Maximum)
                    {
                        _Value++;
                    }
                }
                else
                {
                    if ((Value - 1) >= _Minimum)
                    {
                        _Value--;
                    }
                }
            }
            else
            {
                KeyboardNum = !KeyboardNum;
                Focus();
            }
            Invalidate();
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            try
            {
                if (KeyboardNum == true)
                {
                    _Value = long.Parse((_Value).ToString() + e.KeyChar.ToString().ToString());
                }
                if (_Value > _Maximum)
                {
                    _Value = _Maximum;
                }
            }
            catch (Exception)
            {
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Back)
            {
                string TemporaryValue = _Value.ToString();
                TemporaryValue = TemporaryValue.Remove(Convert.ToInt32(TemporaryValue.Length - 1));
                if (TemporaryValue.Length == 0)
                {
                    TemporaryValue = "0";
                }
                _Value = Convert.ToInt32(TemporaryValue);
            }
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                if ((Value + 1) <= _Maximum)
                {
                    _Value++;
                }
                Invalidate();
            }
            else
            {
                if ((Value - 1) >= _Minimum)
                {
                    _Value--;
                }
                Invalidate();
            }
        }

        #endregion

        public iTalk_NumericUpDown()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            P1 = new Pen(Color.FromArgb(180, 180, 180)); // P1 = Border color
            B1 = new SolidBrush(Color.White); // B1 = Rect Background color
            BackColor = Color.Transparent;
            ForeColor = Color.DimGray;

            _Minimum = 0;
            _Maximum = 100;

            Font = new Font("Tahoma", 11);
            Size = new Size(70, 28);
            MinimumSize = new Size(62, 28);
            DoubleBuffered = true;
        }

        public void Increment(int Value)
        {
            this._Value += Value;
            Invalidate();
        }

        public void Decrement(int Value)
        {
            this._Value -= Value;
            Invalidate();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.AntiAlias;

            G.Clear(Color.Transparent); // Set control background color
            G.FillPath(B1, Shape); // Draw background
            G.DrawPath(P1, Shape); // Draw border

            LinearGradientBrush ColorGradient = new LinearGradientBrush(new Rectangle(Width - 23, 4, 19, 19), Color.FromArgb(241, 241, 241), Color.FromArgb(241, 241, 241), 90.0F);
            G.FillRectangle(ColorGradient, ColorGradient.Rectangle); // Fills the body of the rectangle

            G.DrawRectangle(new Pen(Color.FromArgb(252, 252, 252)), new Rectangle(Width - 22, 5, 17, 17));
            G.DrawRectangle(new Pen(Color.FromArgb(180, 180, 180)), new Rectangle(Width - 23, 4, 19, 19));

            G.DrawLine(new Pen(Color.FromArgb(250, 252, 250)), new Point(Width - 22, Height - 16), new Point(Width - 5, Height - 16));
            G.DrawLine(new Pen(Color.FromArgb(180, 180, 180)), new Point(Width - 22, Height - 15), new Point(Width - 5, Height - 15));
            G.DrawLine(new Pen(Color.FromArgb(250, 250, 250)), new Point(Width - 22, Height - 14), new Point(Width - 5, Height - 14));

            G.DrawString("+", new Font("Tahoma", 8), Brushes.Gray, Width - 19, Height - 26);
            G.DrawString("-", new Font("Tahoma", 12), Brushes.Gray, Width - 19, Height - 20);

            switch (MyStringAlignment)
            {
                case _TextAlignment.Near:
                    G.DrawString(System.Convert.ToString(Value), Font, new SolidBrush(ForeColor), new Rectangle(5, 0, Width - 1, Height - 1), new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                    break;
                case _TextAlignment.Center:
                    G.DrawString(System.Convert.ToString(Value), Font, new SolidBrush(ForeColor), new Rectangle(0, 0, Width - 1, Height - 1), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    break;
            }

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region Left Chat Bubble

    public class iTalk_ChatBubble_L : Control
    {

        #region Variables

        private GraphicsPath Shape;
        private Color _TextColor = Color.FromArgb(52, 52, 52);
        private Color _BubbleColor = Color.FromArgb(217, 217, 217);
        private bool _DrawBubbleArrow = true;

        #endregion
        #region Properties

        public override Color ForeColor
        {
            get { return this._TextColor; }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        public Color BubbleColor
        {
            get { return this._BubbleColor; }
            set
            {
                this._BubbleColor = value;
                this.Invalidate();
            }
        }

        public bool DrawBubbleArrow
        {
            get { return _DrawBubbleArrow; }
            set
            {
                _DrawBubbleArrow = value;
                Invalidate();
            }
        }

        #endregion

        public iTalk_ChatBubble_L()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Size = new Size(152, 38);
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(52, 52, 52);
            Font = new Font("Segoe UI", 10);
        }

        protected override void OnResize(System.EventArgs e)
        {
            Shape = new GraphicsPath();

            var _Shape = Shape;
            _Shape.AddArc(9, 0, 10, 10, 180, 90);
            _Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _Shape.AddArc(9, Height - 11, 10, 10, 90, 90);
            _Shape.CloseAllFigures();

            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(B);
            var _G = G;
            _G.SmoothingMode = SmoothingMode.HighQuality;
            _G.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _G.Clear(BackColor);

            // Fill the body of the bubble with the specified color
            _G.FillPath(new SolidBrush(_BubbleColor), Shape);
            // Draw the string specified in 'Text' property
            _G.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(15, 4, Width - 17, Height - 5));

            // Draw a polygon on the right side of the bubble
            if (_DrawBubbleArrow == true)
            {
                Point[] p = {
                            new Point(9, Height - 19),
                            new Point(0, Height - 25),
                            new Point(9, Height - 30)
                        };
                _G.FillPolygon(new SolidBrush(_BubbleColor), p);
                _G.DrawPolygon(new Pen(new SolidBrush(_BubbleColor)), p);
            }
            G.Dispose();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(B, 0, 0);
            B.Dispose();
        }
    }

    #endregion
    #region Right Chat Bubble

    public class iTalk_ChatBubble_R : Control
    {

        #region Variables

        private GraphicsPath Shape;
        private Color _TextColor = Color.FromArgb(52, 52, 52);
        private Color _BubbleColor = Color.FromArgb(192, 206, 215);
        private bool _DrawBubbleArrow = true;

        #endregion
        #region Properties

        public override Color ForeColor
        {
            get { return this._TextColor; }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        public Color BubbleColor
        {
            get { return this._BubbleColor; }
            set
            {
                this._BubbleColor = value;
                this.Invalidate();
            }
        }

        public bool DrawBubbleArrow
        {
            get { return _DrawBubbleArrow; }
            set
            {
                _DrawBubbleArrow = value;
                Invalidate();
            }
        }

        #endregion

        public iTalk_ChatBubble_R()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Size = new Size(152, 38);
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(52, 52, 52);
            Font = new Font("Segoe UI", 10);
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            Shape = new GraphicsPath();

            var _with1 = Shape;
            _with1.AddArc(0, 0, 10, 10, 180, 90);
            _with1.AddArc(Width - 18, 0, 10, 10, -90, 90);
            _with1.AddArc(Width - 18, Height - 11, 10, 10, 0, 90);
            _with1.AddArc(0, Height - 11, 10, 10, 90, 90);
            _with1.CloseAllFigures();

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(B);

            var _G = G;
            _G.SmoothingMode = SmoothingMode.HighQuality;
            _G.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _G.Clear(BackColor);

            // Fill the body of the bubble with the specified color
            _G.FillPath(new SolidBrush(_BubbleColor), Shape);
            // Draw the string specified in 'Text' property
            _G.DrawString(Text, Font, new SolidBrush(ForeColor), (new Rectangle(6, 4, Width - 15, Height)));

            // Draw a polygon on the right side of the bubble
            if (_DrawBubbleArrow == true)
            {
                Point[] p = {
            new Point(Width - 8, Height - 19),
            new Point(Width, Height - 25),
            new Point(Width - 8, Height - 30)
        };
                _G.FillPolygon(new SolidBrush(_BubbleColor), p);
                _G.DrawPolygon(new Pen(new SolidBrush(_BubbleColor)), p);
            }

            G.Dispose();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(B, 0, 0);
            B.Dispose();
        }
    }

    #endregion
    #region Separator

    public class iTalk_Separator : Control
    {

        public iTalk_Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = new Size(120, 10);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(184, 183, 188)), 0, 5, Width, 5);
        }
    }

    #endregion
    #region Panel

    class iTalk_Panel : ContainerControl
    {


        private GraphicsPath Shape;
        public iTalk_Panel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            this.Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            Shape = new GraphicsPath();
            var _with1 = Shape;
            _with1.AddArc(0, 0, 10, 10, 180, 90);
            _with1.AddArc(Width - 11, 0, 10, 10, -90, 90);
            _with1.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            _with1.AddArc(0, Height - 11, 10, 10, 90, 90);
            _with1.CloseAllFigures();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.HighQuality;

            G.Clear(Color.Transparent);
            G.FillPath(Brushes.White, Shape); // Draw RTB background
            G.DrawPath(new Pen(Color.FromArgb(180, 180, 180)), Shape); // Draw border

            G.Dispose();
            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            B.Dispose();
        }
    }

    #endregion
    #region GroupBox

    public class iTalk_GroupBox : ContainerControl
    {

        public iTalk_GroupBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            this.Size = new Size(212, 104);
            this.MinimumSize = new Size(136, 50);
            this.Padding = new Padding(5, 28, 5, 5);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            Rectangle TitleBox = new Rectangle(51, 3, Width - 103, 18);
            Rectangle box = new Rectangle(0, 0, Width - 1, Height - 10);

            G.Clear(Color.Transparent);
            G.SmoothingMode = SmoothingMode.HighQuality;

            // Draw the body of the GroupBox
            G.FillPath(Brushes.White, RoundRectangle.RoundRect(new Rectangle(1, 12, Width - 3, box.Height - 1), 8));
            // Draw the border of the GroupBox
            G.DrawPath(new Pen(Color.FromArgb(159, 159, 161)), RoundRectangle.RoundRect(new Rectangle(1, 12, Width - 3, Height - 13), 8));

            // Draw the background of the title box
            G.FillPath(Brushes.White, RoundRectangle.RoundRect(TitleBox, 1));
            // Draw the border of the title box
            G.DrawPath(new Pen(Color.FromArgb(182, 180, 186)), RoundRectangle.RoundRect(TitleBox, 4));
            // Draw the specified string from 'Text' property inside the title box
            G.DrawString(Text, new Font("Tahoma", 9, FontStyle.Regular), new SolidBrush(Color.FromArgb(53, 53, 53)), TitleBox, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region CheckBox

    [DefaultEvent("CheckedChanged")]
    class iTalk_CheckBox : Control
    {

        #region Variables

        private GraphicsPath Shape;
        private LinearGradientBrush GB;
        private Rectangle R1;
        private Rectangle R2;
        private bool _Checked;
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        #endregion
        #region Properties

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        #endregion

        public iTalk_CheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 10);
            Size = new Size(120, 26);
            ForeColor = Color.Black;
        }

        protected override void OnClick(EventArgs e)
        {
            _Checked = !_Checked;
            if (CheckedChanged != null)
            {
                CheckedChanged(this);
            }
            Invalidate();
            base.OnClick(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnResize(System.EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                Shape = new GraphicsPath();

                R1 = new Rectangle(17, 0, Width, Height + 1);
                R2 = new Rectangle(0, 0, Width, Height);
                GB = new LinearGradientBrush(new Rectangle(0, 0, 25, 25), Color.FromArgb(250, 250, 250), Color.FromArgb(240, 240, 240), 90);

                var _Shape = Shape;
                _Shape.AddArc(0, 0, 7, 7, 180, 90);
                _Shape.AddArc(7, 0, 7, 7, -90, 90);
                _Shape.AddArc(7, 7, 7, 7, 0, 90);
                _Shape.AddArc(0, 7, 7, 7, 90, 90);
                _Shape.CloseAllFigures();
                Height = 15;
            }

            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var _G = e.Graphics;
            _G.Clear(Color.FromArgb(246, 246, 246));
            _G.SmoothingMode = SmoothingMode.AntiAlias;
            // Fill the body of the CheckBox
            _G.FillPath(GB, Shape);
            // Draw the border
            _G.DrawPath(new Pen(Color.FromArgb(160, 160, 160)), Shape);
            // Draw the string
            _G.DrawString(Text, Font, new SolidBrush(Color.Black), R1, new StringFormat { LineAlignment = StringAlignment.Center });

            if (Checked)
            {
                _G.DrawString("ü", new Font("Wingdings", 14), new SolidBrush(Color.FromArgb(231, 53, 53)), new Rectangle(-2, 1, Width, Height), new StringFormat { LineAlignment = StringAlignment.Center });
            }
            e.Dispose();
        }
    }

    #endregion
    #region RadioButton

    [DefaultEvent("CheckedChanged")]
    class iTalk_RadioButton : Control
    {

        #region Enums

        public enum MouseState : byte
        {
            None = 0,
            Over = 1,
            Down = 2,
            Block = 3
        }

        #endregion
        #region Variables

        private bool _Checked;
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        #endregion
        #region Properties

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                InvalidateControls();
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = 15;
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!_Checked)
                Checked = true;
            base.OnMouseDown(e);
        }

        #endregion

        public iTalk_RadioButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 10);
            Width = 132;
        }

        private void InvalidateControls()
        {
            if (!IsHandleCreated || !_Checked)
                return;

            foreach (Control _Control in Parent.Controls)
            {
                if (!object.ReferenceEquals(_Control, this) && _Control is iTalk_RadioButton)
                {
                    ((iTalk_RadioButton)_Control).Checked = false;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var _G = e.Graphics;

            _G.Clear(Color.FromArgb(246, 246, 246));
            _G.SmoothingMode = SmoothingMode.AntiAlias;

            LinearGradientBrush LGB = new LinearGradientBrush(new Rectangle(new Point(0, 0), new Size(14, 14)), Color.FromArgb(250, 250, 250), Color.FromArgb(240, 240, 240), 90);
            _G.FillEllipse(LGB, new Rectangle(new Point(0, 0), new Size(14, 14)));

            GraphicsPath GP = new GraphicsPath();
            GP.AddEllipse(new Rectangle(0, 0, 14, 14));
            _G.SetClip(GP);
            _G.ResetClip();

            // Draw ellipse border
            _G.DrawEllipse(new Pen(Color.FromArgb(160, 160, 160)), new Rectangle(new Point(0, 0), new Size(14, 14)));

            // Draw an ellipse inside the body
            if (_Checked)
            {
                SolidBrush EllipseColor = new SolidBrush(Color.FromArgb(142, 142, 142));
                _G.FillEllipse(EllipseColor, new Rectangle(new Point(4, 4), new Size(6, 6)));
            }
            // Draw the string specified in 'Text' property
            _G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(142, 142, 142)), 16, 8, new StringFormat { LineAlignment = StringAlignment.Center });

            e.Dispose();
        }
    }

    #endregion
    #region Notification Number

    class iTalk_NotificationNumber : Control
    {
        #region Variables

        private int _Value = 0;
        private int _Maximum = 99;

        #endregion
        #region Properties

        public int Value
        {
            get
            {
                if (this._Value == 0)
                {
                    return 0;
                }
                return this._Value;
            }
            set
            {
                if (value > this._Maximum)
                {
                    value = this._Maximum;
                }
                this._Value = value;
                this.Invalidate();
            }
        }

        public int Maximum
        {
            get
            {
                return this._Maximum;
            }
            set
            {
                if (value < this._Value)
                {
                    this._Value = value;
                }
                this._Maximum = value;
                this.Invalidate();
            }
        }



        #endregion

        public iTalk_NotificationNumber()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            Text = null;
            DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = 20;
            Width = 20;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var _G = e.Graphics;
            string myString = _Value.ToString();
            _G.Clear(BackColor);
            _G.SmoothingMode = SmoothingMode.AntiAlias;
            LinearGradientBrush LGB = new LinearGradientBrush(new Rectangle(new Point(0, 0), new Size(18, 20)), Color.FromArgb(197, 69, 68), Color.FromArgb(176, 52, 52), 90f);

            // Fills the body with LGB gradient
            _G.FillEllipse(LGB, new Rectangle(new Point(0, 0), new Size(18, 18)));
            // Draw border
            _G.DrawEllipse(new Pen(Color.FromArgb(205, 70, 66)), new Rectangle(new Point(0, 0), new Size(18, 18)));
            _G.DrawString(myString, new Font("Segoe UI", 8, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 255, 253)), new Rectangle(0, 0, Width - 2, Height), new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
            e.Dispose();
        }

    }

    #endregion
    #region ListView

    class iTalk_Listview : ListView
    {

        [DllImport("uxtheme", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string textSubAppName, string textSubIdList);

        public iTalk_Listview()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            iTalk_Listview.SetWindowTheme(this.Handle, "explorer", null);
            base.OnHandleCreated(e);
        }
    }

    #endregion
    #region ComboBox

    public class iTalk_ComboBox : ComboBox
    {

        #region Variables

        private int _StartIndex = 0;
        private Color _HoverSelectionColor = Color.FromArgb(241, 241, 241);

        #endregion
        #region Custom Properties

        public int StartIndex
        {
            get { return _StartIndex; }
            set
            {
                _StartIndex = value;
                try
                {
                    base.SelectedIndex = value;
                }
                catch
                {
                }
                Invalidate();
            }
        }

        public Color HoverSelectionColor
        {
            get { return _HoverSelectionColor; }
            set
            {
                _HoverSelectionColor = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(_HoverSelectionColor), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }

            if (!(e.Index == -1))
            {
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, Brushes.DimGray, e.Bounds);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            SuspendLayout();
            Update();
            ResumeLayout();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        #endregion

        public iTalk_ComboBox()
        {
            SetStyle((ControlStyles)139286, true);
            SetStyle(ControlStyles.Selectable, false);

            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;

            BackColor = Color.FromArgb(246, 246, 246);
            ForeColor = Color.FromArgb(142, 142, 142);
            Size = new Size(135, 26);
            ItemHeight = 20;
            DropDownHeight = 100;
            Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            LinearGradientBrush LGB = default(LinearGradientBrush);
            GraphicsPath GP = default(GraphicsPath);

            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create a curvy border
            GP = RoundRectangle.RoundRect(0, 0, Width - 1, Height - 1, 5);
            // Fills the body of the rectangle with a gradient
            LGB = new LinearGradientBrush(ClientRectangle, Color.FromArgb(241, 241, 241), Color.FromArgb(241, 241, 241), 90f);

            e.Graphics.SetClip(GP);
            e.Graphics.FillRectangle(LGB, ClientRectangle);
            e.Graphics.ResetClip();

            // Draw rectangle border
            e.Graphics.DrawPath(new Pen(Color.FromArgb(204, 204, 204)), GP);
            // Draw string
            e.Graphics.DrawString(Text, Font, new SolidBrush(Color.FromArgb(142, 142, 142)), new Rectangle(3, 0, Width - 20, Height), new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            });

            // Draw the dropdown arrow
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160), 2), new Point(Width - 18, 10), new Point(Width - 14, 14));
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160), 2), new Point(Width - 14, 14), new Point(Width - 10, 10));
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), new Point(Width - 14, 15), new Point(Width - 14, 14));

            GP.Dispose();
            LGB.Dispose();
        }
    }

    #endregion
    #region Circular ProgressBar

    public class iTalk_ProgressBar : Control
    {

        #region Enums

        public enum _ProgressShape
        {
            Round,
            Flat
        }

        #endregion
        #region Variables

        private long _Value;
        private long _Maximum = 100;
        private Color _ProgressColor1 = Color.FromArgb(92, 92, 92);
        private Color _ProgressColor2 = Color.FromArgb(92, 92, 92);
        private _ProgressShape ProgressShapeVal;

        #endregion
        #region Custom Properties

        public long Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum)
                    value = _Maximum;
                _Value = value;
                Invalidate();
            }
        }

        public long Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < 1)
                    value = 1;
                _Maximum = value;
                Invalidate();
            }
        }

        public Color ProgressColor1
        {
            get { return _ProgressColor1; }
            set
            {
                _ProgressColor1 = value;
                Invalidate();
            }
        }

        public Color ProgressColor2
        {
            get { return _ProgressColor2; }
            set
            {
                _ProgressColor2 = value;
                Invalidate();
            }
        }

        public _ProgressShape ProgressShape
        {
            get { return ProgressShapeVal; }
            set
            {
                ProgressShapeVal = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetStandardSize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
        }

        protected override void OnPaintBackground(PaintEventArgs p)
        {
            base.OnPaintBackground(p);
        }

        #endregion

        public iTalk_ProgressBar()
        {
            Size = new Size(130, 130);
            Font = new Font("Segoe UI", 15);
            //MinimumSize = new Size(100, 100);
            DoubleBuffered = true;
        }

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
        }

        public void Increment(int Val)
        {
            this._Value += Val;
            Invalidate();
        }

        public void Decrement(int Val)
        {
            this._Value -= Val;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.Clear(this.BackColor);
                    using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, this._ProgressColor1, this._ProgressColor2, LinearGradientMode.ForwardDiagonal))
                    {
                        using (Pen pen = new Pen(brush, 10f))
                        {
                            switch (this.ProgressShapeVal)
                            {
                                case _ProgressShape.Round:
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;
                                    break;

                                case _ProgressShape.Flat:
                                    pen.StartCap = LineCap.Flat;
                                    pen.EndCap = LineCap.Flat;
                                    break;
                            }
                            graphics.DrawArc(pen, 0x12, 0x12, (this.Width - 0x23) - 2, (this.Height - 0x23) - 2, -90, (int)Math.Round((double)((360.0 / ((double)this._Maximum)) * this._Value)));
                        }
                    }
                    using (LinearGradientBrush brush2 = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(0x34, 0x34, 0x34), Color.FromArgb(0x34, 0x34, 0x34), LinearGradientMode.Vertical))
                    {
                        graphics.FillEllipse(brush2, 0x18, 0x18, (this.Width - 0x30) - 1, (this.Height - 0x30) - 1);
                    }
                    SizeF MS = graphics.MeasureString(Convert.ToString(Convert.ToInt32((100 / _Maximum) * _Value)), Font);
                    graphics.DrawString(Convert.ToString(Convert.ToInt32((100 / _Maximum) * _Value)), Font, Brushes.White, Convert.ToInt32(Width / 2 - MS.Width / 2), Convert.ToInt32(Height / 2 - MS.Height / 2));
                    e.Graphics.DrawImage(bitmap, 0, 0);
                    graphics.Dispose();
                    bitmap.Dispose();
                }
            }
        }
    }

    #endregion
    #region Progress Indicator

    class iTalk_ProgressIndicator : Control
    {

        #region Variables

        private readonly SolidBrush BaseColor = new SolidBrush(Color.DarkGray);
        private readonly SolidBrush AnimationColor = new SolidBrush(Color.DimGray);

        private readonly Timer AnimationSpeed = new Timer();
        private PointF[] FloatPoint;
        private BufferedGraphics BuffGraphics;
        private int IndicatorIndex;
        private readonly BufferedGraphicsContext GraphicsContext = BufferedGraphicsManager.Current;

        #endregion
        #region Custom Properties

        public Color P_BaseColor
        {
            get { return BaseColor.Color; }
            set { BaseColor.Color = value; }
        }

        public Color P_AnimationColor
        {
            get { return AnimationColor.Color; }
            set { AnimationColor.Color = value; }
        }

        public int P_AnimationSpeed
        {
            get { return AnimationSpeed.Interval; }
            set { AnimationSpeed.Interval = value; }
        }

        #endregion
        #region EventArgs

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
            UpdateGraphics();
            SetPoints();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            AnimationSpeed.Enabled = this.Enabled;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AnimationSpeed.Tick += AnimationSpeed_Tick;
            AnimationSpeed.Start();
        }

        private void AnimationSpeed_Tick(object sender, EventArgs e)
        {
            if (IndicatorIndex.Equals(0))
            {
                IndicatorIndex = FloatPoint.Length - 1;
            }
            else
            {
                IndicatorIndex -= 1;
            }
            this.Invalidate(false);
        }

        #endregion

        public iTalk_ProgressIndicator()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(80, 80);
            Text = string.Empty;
            MinimumSize = new Size(80, 80);
            SetPoints();
            AnimationSpeed.Interval = 100;
        }

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
        }

        private void SetPoints()
        {
            Stack<PointF> stack = new Stack<PointF>();
            PointF startingFloatPoint = new PointF(((float)this.Width) / 2f, ((float)this.Height) / 2f);
            for (float i = 0f; i < 360f; i += 45f)
            {
                this.SetValue(startingFloatPoint, (int)Math.Round((double)((((double)this.Width) / 2.0) - 15.0)), (double)i);
                PointF endPoint = this.EndPoint;
                endPoint = new PointF(endPoint.X - 7.5f, endPoint.Y - 7.5f);
                stack.Push(endPoint);
            }
            this.FloatPoint = stack.ToArray();
        }

        private void UpdateGraphics()
        {
            if ((this.Width > 0) && (this.Height > 0))
            {
                Size size2 = new Size(this.Width + 1, this.Height + 1);
                this.GraphicsContext.MaximumBuffer = size2;
                this.BuffGraphics = this.GraphicsContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
                this.BuffGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.BuffGraphics.Graphics.Clear(this.BackColor);
            int num2 = this.FloatPoint.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (this.IndicatorIndex == i)
                {
                    this.BuffGraphics.Graphics.FillEllipse(this.AnimationColor, this.FloatPoint[i].X, this.FloatPoint[i].Y, 15f, 15f);
                }
                else
                {
                    this.BuffGraphics.Graphics.FillEllipse(this.BaseColor, this.FloatPoint[i].X, this.FloatPoint[i].Y, 15f, 15f);
                }
            }
            this.BuffGraphics.Render(e.Graphics);
        }


        private double Rise;
        private double Run;
        private PointF _StartingFloatPoint;

        private X AssignValues<X>(ref X Run, X Length)
        {
            Run = Length;
            return Length;
        }

        private void SetValue(PointF StartingFloatPoint, int Length, double Angle)
        {
            double CircleRadian = Math.PI * Angle / 180.0;

            _StartingFloatPoint = StartingFloatPoint;
            Rise = AssignValues(ref Run, Length);
            Rise = Math.Sin(CircleRadian) * Rise;
            Run = Math.Cos(CircleRadian) * Run;
        }

        private PointF EndPoint
        {
            get
            {
                float LocationX = Convert.ToSingle(_StartingFloatPoint.Y + Rise);
                float LocationY = Convert.ToSingle(_StartingFloatPoint.X + Run);

                return new PointF(LocationY, LocationX);
            }
        }
    }

    #endregion
    #region TabControl

    class iTalk_TabControl : TabControl
    {

        // NOTE: For best quality icons/images on the TabControl; from the associated ImageList, set
        // the image size (24,24) so it can fit in the tab rectangle. However, to ensure a
        // high-quality image drawing, make sure you only add (32,32) images and not (24,24) as
        // determined in the ImageList

        // INFO: A free, non-commercial icon list that would fit in perfectly with the TabControl is
        // Wireframe Toolbar Icons by Gentleface. Licensed under Creative Commons Attribution.
        // Check it out from here: http://www.gentleface.com/free_icon_set.html

        public iTalk_TabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer, true);

            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(44, 135);
            DrawMode = TabDrawMode.OwnerDrawFixed;

            foreach (TabPage Page in this.TabPages)
            {
                Page.BackColor = Color.FromArgb(246, 246, 246);
            }
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();

            base.DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;
            Alignment = TabAlignment.Left;
        }


        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is TabPage)
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.Controls.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        TabPage current = (TabPage)enumerator.Current;
                        current = new TabPage();
                    }
                }
                finally
                {
                    e.Control.BackColor = Color.FromArgb(246, 246, 246);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            var _Graphics = G;

            _Graphics.Clear(Color.FromArgb(246, 246, 246));
            _Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            _Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            _Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            // Draw tab selector background
            _Graphics.FillRectangle(new SolidBrush(Color.FromArgb(54, 57, 64)), new Rectangle(-5, 0, ItemSize.Height + 4, Height));
            // Draw vertical line at the end of the tab selector rectangle
            _Graphics.DrawLine(new Pen(Color.FromArgb(25, 26, 28)), ItemSize.Height - 1, 0, ItemSize.Height - 1, Height);

            for (int TabIndex = 0; TabIndex <= TabCount - 1; TabIndex++)
            {
                if (TabIndex == SelectedIndex)
                {
                    Rectangle TabRect = new Rectangle(new Point(GetTabRect(TabIndex).Location.X - 2, GetTabRect(TabIndex).Location.Y - 2), new Size(GetTabRect(TabIndex).Width + 3, GetTabRect(TabIndex).Height - 8));

                    // Draw background of the selected tab
                    _Graphics.FillRectangle(new SolidBrush(Color.FromArgb(35, 36, 38)), TabRect.X, TabRect.Y, TabRect.Width - 4, TabRect.Height + 3);
                    // Draw a tab highlighter on the background of the selected tab
                    Rectangle TabHighlighter = new Rectangle(new Point(GetTabRect(TabIndex).X - 2, GetTabRect(TabIndex).Location.Y - (TabIndex == 0 ? 1 : 1)), new Size(4, GetTabRect(TabIndex).Height - 7));
                    _Graphics.FillRectangle(new SolidBrush(Color.FromArgb(89, 169, 222)), TabHighlighter);
                    // Draw tab text
                    _Graphics.DrawString(TabPages[TabIndex].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), new SolidBrush(Color.FromArgb(254, 255, 255)), new Rectangle(TabRect.Left + 40, TabRect.Top + 12, TabRect.Width - 40, TabRect.Height), new StringFormat { Alignment = StringAlignment.Near });

                    if (this.ImageList != null)
                    {
                        int Index = TabPages[TabIndex].ImageIndex;
                        if (!(Index == -1))
                        {
                            _Graphics.DrawImage(ImageList.Images[TabPages[TabIndex].ImageIndex], TabRect.X + 9, TabRect.Y + 6, 24, 24);
                        }
                    }
                }
                else
                {
                    Rectangle TabRect = new Rectangle(new Point(GetTabRect(TabIndex).Location.X - 2, GetTabRect(TabIndex).Location.Y - 2), new Size(GetTabRect(TabIndex).Width + 3, GetTabRect(TabIndex).Height - 8));
                    _Graphics.DrawString(TabPages[TabIndex].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), new SolidBrush(Color.FromArgb(159, 162, 167)), new Rectangle(TabRect.Left + 40, TabRect.Top + 12, TabRect.Width - 40, TabRect.Height), new StringFormat { Alignment = StringAlignment.Near });

                    if (this.ImageList != null)
                    {
                        int Index = TabPages[TabIndex].ImageIndex;
                        if (!(Index == -1))
                        {
                            _Graphics.DrawImage(ImageList.Images[TabPages[TabIndex].ImageIndex], TabRect.X + 9, TabRect.Y + 6, 24, 24);
                        }
                    }

                }
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region TrackBar

    [DefaultEvent("ValueChanged")]
    class iTalk_TrackBar : Control
    {

        #region Enums

        public enum ValueDivisor
        {
            By1 = 1,
            By10 = 10,
            By100 = 100,
            By1000 = 1000
        }

        #endregion
        #region Variables

        private GraphicsPath PipeBorder;
        private GraphicsPath TrackBarHandle;
        private Rectangle TrackBarHandleRect;
        private Rectangle ValueRect;
        private LinearGradientBrush VlaueLGB;
        private LinearGradientBrush TrackBarHandleLGB;
        private bool Cap;

        private int ValueDrawer;
        private int _Minimum = 0;
        private int _Maximum = 10;
        private int _Value = 0;
        private Color _ValueColour = Color.FromArgb(224, 224, 224);
        private bool _DrawHatch = true;
        private bool _DrawValueString = false;
        private bool _JumpToMouse = false;
        private ValueDivisor DividedValue = ValueDivisor.By1;

        #endregion
        #region Custom Properties

        public int Minimum
        {
            get { return _Minimum; }

            set
            {
                if (value >= _Maximum)
                    value = _Maximum - 10;
                if (_Value < value)
                    _Value = value;

                _Minimum = value;
                Invalidate();
            }
        }

        public int Maximum
        {
            get { return _Maximum; }

            set
            {
                if (value <= _Minimum)
                    value = _Minimum + 10;
                if (_Value > value)
                    _Value = value;

                _Maximum = value;
                Invalidate();
            }
        }

        public event ValueChangedEventHandler ValueChanged;
        public delegate void ValueChangedEventHandler();
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    if (value < _Minimum)
                    {
                        _Value = _Minimum;
                    }
                    else
                    {
                        if (value > _Maximum)
                        {
                            _Value = _Maximum;
                        }
                        else
                        {
                            _Value = value;
                        }
                    }
                    Invalidate();
                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }

        public ValueDivisor ValueDivison
        {
            get
            {
                return this.DividedValue;
            }
            set
            {
                this.DividedValue = value;
                this.Invalidate();
            }
        }

        [Browsable(false)]
        public float ValueToSet
        {
            get
            {
                return (float)(((double)this._Value) / ((double)this.DividedValue));
            }
            set
            {
                this.Value = (int)Math.Round((double)(value * ((float)this.DividedValue)));
            }
        }

        public Color ValueColour
        {
            get { return _ValueColour; }
            set
            {
                _ValueColour = value;
                Invalidate();
            }
        }

        public bool DrawHatch
        {
            get { return _DrawHatch; }
            set
            {
                _DrawHatch = value;
                Invalidate();
            }
        }

        public bool DrawValueString
        {
            get { return _DrawValueString; }
            set
            {
                _DrawValueString = value;
                if (_DrawValueString == true)
                {
                    Height = 40;
                }
                else
                {
                    Height = 22;
                }
                Invalidate();
            }
        }

        public bool JumpToMouse
        {
            get
            {
                return this._JumpToMouse;
            }
            set
            {
                this._JumpToMouse = value;
            }
        }

        #endregion
        #region EventArgs

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if ((this.Cap && (e.X > -1)) && (e.X < (this.Width + 1)))
            {
                this.Value = this._Minimum + ((int)Math.Round((double)((this._Maximum - this._Minimum) * (((double)e.X) / ((double)this.Width)))));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.ValueDrawer = (int)Math.Round((double)((((double)(this._Value - this._Minimum)) / ((double)(this._Maximum - this._Minimum))) * (this.Width - 11)));
                this.TrackBarHandleRect = new Rectangle(this.ValueDrawer, 0, 10, 20);
                this.Cap = this.TrackBarHandleRect.Contains(e.Location);
                if (this._JumpToMouse)
                {
                    this.Value = this._Minimum + ((int)Math.Round((double)((this._Maximum - this._Minimum) * (((double)e.X) / ((double)this.Width)))));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.Cap = false;
        }


        #endregion

        public iTalk_TrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer, true);

            _DrawHatch = true;
            Size = new Size(80, 22);
            MinimumSize = new Size(37, 22);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_DrawValueString == true)
            {
                Height = 40;
            }
            else
            {
                Height = 22;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            HatchBrush Hatch = new HatchBrush(HatchStyle.WideDownwardDiagonal, Color.FromArgb(20, Color.Black), Color.Transparent);
            G.Clear(Parent.BackColor);
            G.SmoothingMode = SmoothingMode.AntiAlias;
            checked
            {
                this.PipeBorder = RoundRectangle.RoundRect(1, 6, this.Width - 3, 8, 3);
                try
                {
                    this.ValueDrawer = (int)Math.Round(unchecked(checked((double)(this._Value - this._Minimum) / (double)(this._Maximum - this._Minimum)) * (double)checked(this.Width - 11)));
                }
                catch (Exception)
                {
                }
                this.TrackBarHandleRect = new Rectangle(this.ValueDrawer, 0, 10, 20);
                G.SetClip(this.PipeBorder);
                this.ValueRect = new Rectangle(1, 7, this.TrackBarHandleRect.X + this.TrackBarHandleRect.Width - 2, 7);
                this.VlaueLGB = new LinearGradientBrush(this.ValueRect, this._ValueColour, this._ValueColour, 90f);
                G.FillRectangle(this.VlaueLGB, this.ValueRect);

                if (_DrawHatch == true)
                {
                    G.FillRectangle(Hatch, this.ValueRect);
                }

                G.ResetClip();
                G.SmoothingMode = SmoothingMode.AntiAlias;
                G.DrawPath(new Pen(Color.FromArgb(180, 180, 180)), this.PipeBorder);
                this.TrackBarHandle = RoundRectangle.RoundRect(this.TrackBarHandleRect, 3);
                this.TrackBarHandleLGB = new LinearGradientBrush(this.ClientRectangle, SystemColors.Control, SystemColors.Control, 90f);
                G.FillPath(this.TrackBarHandleLGB, this.TrackBarHandle);
                G.DrawPath(new Pen(Color.FromArgb(180, 180, 180)), this.TrackBarHandle);

                if (_DrawValueString == true)
                {
                    G.DrawString(System.Convert.ToString(ValueToSet), Font, Brushes.Gray, 0, 25);
                }
            }
        }
    }

    #endregion
    #region MenuStrip

    public class iTalk_MenuStrip : MenuStrip
    {

        public iTalk_MenuStrip()
        {
            this.Renderer = new ControlRenderer();
        }

        public new ControlRenderer Renderer
        {
            get { return (ControlRenderer)base.Renderer; }
            set { base.Renderer = value; }
        }

    }

    #endregion
    #region ContextMenuStrip

    public class iTalk_ContextMenuStrip : ContextMenuStrip
    {

        public iTalk_ContextMenuStrip()
        {
            this.Renderer = new ControlRenderer();
        }

        public new ControlRenderer Renderer
        {
            get { return (ControlRenderer)base.Renderer; }
            set { base.Renderer = value; }
        }
    }

    #endregion
    #region StatusStrip

    public class iTalk_StatusStrip : StatusStrip
    {

        public iTalk_StatusStrip()
        {
            this.Renderer = new ControlRenderer();
            SizingGrip = false;
        }

        public new ControlRenderer Renderer
        {
            get { return (ControlRenderer)base.Renderer; }
            set { base.Renderer = value; }
        }
    }

    #endregion
    #region Info Icon

    class iTalk_Icon_Info : Control
    {
        public iTalk_Icon_Info()
        {
            this.ForeColor = Color.DimGray;
            this.BackColor = Color.FromArgb(246, 246, 246);
            this.Size = new Size(33, 33);
            DoubleBuffered = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            e.Graphics.FillEllipse(new SolidBrush(Color.Gray), new Rectangle(1, 1, 29, 29));
            e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(246, 246, 246)), new Rectangle(3, 3, 25, 25));

            e.Graphics.DrawString("¡", new Font("Segoe UI", 25, FontStyle.Bold), new SolidBrush(Color.Gray), new Rectangle(4, -14, Width, 43), new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            });
        }
    }

    #endregion
    #region  Tick Icon

    class iTalk_Icon_Tick : Control
    {

        public iTalk_Icon_Tick()
        {
            this.ForeColor = Color.DimGray;
            this.BackColor = Color.FromArgb(246, 246, 246);
            this.Size = new Size(33, 33);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            e.Graphics.FillEllipse(new SolidBrush(Color.Gray), new Rectangle(1, 1, 29, 29));
            e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(246, 246, 246)), new Rectangle(3, 3, 25, 25));

            e.Graphics.DrawString("ü", new Font("Wingdings", 25, FontStyle.Bold), new SolidBrush(Color.Gray), new Rectangle(0, -3, Width, 43), new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            });
        }
    }

    #endregion

}
namespace MonoFlat
{

    #region  RoundRectangle

    sealed class RoundRectangle
    {
        public static GraphicsPath RoundRect(Rectangle Rectangle, int Curve)
        {
            GraphicsPath P = new GraphicsPath();
            int ArcRectangleWidth = Curve * 2;
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
            P.AddArc(new Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90);
            P.AddArc(new Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90);
            P.AddLine(new Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));
            return P;
        }
    }

    #endregion

    #region  ThemeContainer

    public class MonoFlat_ThemeContainer : ContainerControl
    {

        #region  Enums

        public enum MouseState
        {
            None = 0,
            Over = 1,
            Down = 2,
            Block = 3
        }

        #endregion
        #region  Variables

        private Rectangle HeaderRect;
        protected MouseState State;
        private int MoveHeight;
        private Point MouseP = new Point(0, 0);
        private bool Cap = false;
        private bool HasShown;

        #endregion
        #region  Properties

        private bool _Sizable = true;
        public bool Sizable
        {
            get
            {
                return _Sizable;
            }
            set
            {
                _Sizable = value;
            }
        }

        private bool _SmartBounds = true;
        public bool SmartBounds
        {
            get
            {
                return _SmartBounds;
            }
            set
            {
                _SmartBounds = value;
            }
        }

        private bool _RoundCorners = true;
        public bool RoundCorners
        {
            get
            {
                return _RoundCorners;
            }
            set
            {
                _RoundCorners = value;
                Invalidate();
            }
        }

        private bool _IsParentForm;
        protected bool IsParentForm
        {
            get
            {
                return _IsParentForm;
            }
        }

        protected bool IsParentMdi
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }
                return Parent.Parent != null;
            }
        }

        private bool _ControlMode;
        protected bool ControlMode
        {
            get
            {
                return _ControlMode;
            }
            set
            {
                _ControlMode = value;
                Invalidate();
            }
        }

        private FormStartPosition _StartPosition;
        public FormStartPosition StartPosition
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                {
                    return ParentForm.StartPosition;
                }
                else
                {
                    return _StartPosition;
                }
            }
            set
            {
                _StartPosition = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.StartPosition = value;
                }
            }
        }

        #endregion
        #region  EventArgs

        protected sealed override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent == null)
            {
                return;
            }
            _IsParentForm = Parent is Form;

            if (!_ControlMode)
            {
                InitializeMessages();

                if (_IsParentForm)
                {
                    this.ParentForm.FormBorderStyle = FormBorderStyle.None;
                    this.ParentForm.TransparencyKey = Color.Fuchsia;

                    if (!DesignMode)
                    {
                        ParentForm.Shown += FormShown;
                    }
                }
                Parent.BackColor = BackColor;
                //   Parent.MinimumSize = New Size(261, 65)
            }
        }

        protected sealed override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!_ControlMode)
            {
                HeaderRect = new Rectangle(0, 0, Width - 14, MoveHeight - 7);
            }
            Invalidate();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (e.Button == MouseButtons.Left)
            {
                SetState(MouseState.Down);
            }
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
            {
                if (HeaderRect.Contains(e.Location))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[0]);
                }
                else if (_Sizable && !(Previous == 0))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[Previous]);
                }
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cap = false;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
            {
                if (_Sizable && !_ControlMode)
                {
                    InvalidateMouse();
                }
            }
            if (Cap)
            {
                Parent.Location = (System.Drawing.Point)((object)(System.Convert.ToDouble(MousePosition) - System.Convert.ToDouble(MouseP)));
            }
        }

        protected override void OnInvalidated(System.Windows.Forms.InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            ParentForm.Text = Text;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        private void FormShown(object sender, EventArgs e)
        {
            if (_ControlMode || HasShown)
            {
                return;
            }

            if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
            {
                Rectangle SB = Screen.PrimaryScreen.Bounds;
                Rectangle CB = ParentForm.Bounds;
                ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
            }
            HasShown = true;
        }

        #endregion
        #region  Mouse & Size

        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        private Point GetIndexPoint;
        private bool B1x;
        private bool B2x;
        private bool B3;
        private bool B4;
        private int GetIndex()
        {
            GetIndexPoint = PointToClient(MousePosition);
            B1x = GetIndexPoint.X < 7;
            B2x = GetIndexPoint.X > Width - 7;
            B3 = GetIndexPoint.Y < 7;
            B4 = GetIndexPoint.Y > Height - 7;

            if (B1x && B3)
            {
                return 4;
            }
            if (B1x && B4)
            {
                return 7;
            }
            if (B2x && B3)
            {
                return 5;
            }
            if (B2x && B4)
            {
                return 8;
            }
            if (B1x)
            {
                return 1;
            }
            if (B2x)
            {
                return 2;
            }
            if (B3)
            {
                return 3;
            }
            if (B4)
            {
                return 6;
            }
            return 0;
        }

        private int Current;
        private int Previous;
        private void InvalidateMouse()
        {
            Current = GetIndex();
            if (Current == Previous)
            {
                return;
            }

            Previous = Current;
            switch (Previous)
            {
                case 0:
                    Cursor = Cursors.Default;
                    break;
                case 6:
                    Cursor = Cursors.SizeNS;
                    break;
                case 8:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case 7:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        private Message[] Messages = new Message[9];
        private void InitializeMessages()
        {
            Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
            for (int I = 1; I <= 8; I++)
            {
                Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
            }
        }

        private void CorrectBounds(Rectangle bounds)
        {
            if (Parent.Width > bounds.Width)
            {
                Parent.Width = bounds.Width;
            }
            if (Parent.Height > bounds.Height)
            {
                Parent.Height = bounds.Height;
            }

            int X = Parent.Location.X;
            int Y = Parent.Location.Y;

            if (X < bounds.X)
            {
                X = bounds.X;
            }
            if (Y < bounds.Y)
            {
                Y = bounds.Y;
            }

            int Width = bounds.X + bounds.Width;
            int Height = bounds.Y + bounds.Height;

            if (X + Parent.Width > Width)
            {
                X = Width - Parent.Width;
            }
            if (Y + Parent.Height > Height)
            {
                Y = Height - Parent.Height;
            }

            Parent.Location = new Point(X, Y);
        }

        private bool WM_LMBUTTONDOWN;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (WM_LMBUTTONDOWN && m.Msg == 513)
            {
                WM_LMBUTTONDOWN = false;

                SetState(MouseState.Over);
                if (!_SmartBounds)
                {
                    return;
                }

                if (IsParentMdi)
                {
                    CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
                }
                else
                {
                    CorrectBounds(Screen.FromControl(Parent).WorkingArea);
                }
            }
        }

        #endregion

        protected override void CreateHandle()
        {
            base.CreateHandle();
        }

        public MonoFlat_ThemeContainer()
        {
            SetStyle((ControlStyles)(139270), true);
            BackColor = Color.FromArgb(32, 41, 50);
            Padding = new Padding(10, 70, 10, 9);
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            MoveHeight = 66;
            Font = new Font("Segoe UI", 9);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;

            G.Clear(Color.FromArgb(32, 41, 50));
            G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), new Rectangle(0, 0, Width, 60));

            if (_RoundCorners == true)
            {
                // Draw Left upper corner
                G.FillRectangle(Brushes.Fuchsia, 0, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 2, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 3, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, 1, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), 1, 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), 1, 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), 2, 1, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), 3, 1, 1, 1);

                // Draw right upper corner
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 3, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 4, 0, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, 1, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), Width - 2, 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), Width - 2, 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), Width - 3, 1, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), Width - 4, 1, 1, 1);

                // Draw Left bottom corner
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 0, Height - 4, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 2, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 3, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, 1, Height - 2, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), 1, Height - 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), 1, Height - 4, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), 3, Height - 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), 2, Height - 2, 1, 1);

                // Draw right bottom corner
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 3, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 4, Height, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 2, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 3, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 3, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 4, Height - 1, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 1, Height - 4, 1, 1);
                G.FillRectangle(Brushes.Fuchsia, Width - 2, Height - 2, 1, 1);

                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), Width - 2, Height - 3, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), Width - 2, Height - 4, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), Width - 4, Height - 2, 1, 1);
                G.FillRectangle(new SolidBrush(Color.FromArgb(32, 41, 50)), Width - 3, Height - 2, 1, 1);
            }

            G.DrawString(Text, new Font("Microsoft Sans Serif", 12, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 254, 255)), new Rectangle(20, 20, Width - 1, Height), new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near });
        }
    }

    #endregion
    #region ControlBox
    class MonoFlat_ControlBox : Control
    {

        #region Enums

        public enum ButtonHoverState
        {
            Minimize,
            Maximize,
            Close,
            None
        }

        #endregion
        #region Variables

        private ButtonHoverState ButtonHState = ButtonHoverState.None;

        #endregion
        #region Properties

        private bool _EnableMaximize = true;
        public bool EnableMaximizeButton
        {
            get { return _EnableMaximize; }
            set
            {
                _EnableMaximize = value;
                Invalidate();
            }
        }

        private bool _EnableMinimize = true;
        public bool EnableMinimizeButton
        {
            get { return _EnableMinimize; }
            set
            {
                _EnableMinimize = value;
                Invalidate();
            }
        }

        private bool _EnableHoverHighlight = false;
        public bool EnableHoverHighlight
        {
            get { return _EnableHoverHighlight; }
            set
            {
                _EnableHoverHighlight = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(100, 25);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int X = e.Location.X;
            int Y = e.Location.Y;
            if (Y > 0 && Y < (Height - 2))
            {
                if (X > 0 && X < 34)
                {
                    ButtonHState = ButtonHoverState.Minimize;
                }
                else if (X > 33 && X < 65)
                {
                    ButtonHState = ButtonHoverState.Maximize;
                }
                else if (X > 64 && X < Width)
                {
                    ButtonHState = ButtonHoverState.Close;
                }
                else
                {
                    ButtonHState = ButtonHoverState.None;
                }
            }
            else
            {
                ButtonHState = ButtonHoverState.None;
            }
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            switch (ButtonHState)
            {
                case ButtonHoverState.Close:
                    Parent.FindForm().Close();
                    break;
                case ButtonHoverState.Minimize:
                    if (_EnableMinimize == true)
                    {
                        Parent.FindForm().WindowState = FormWindowState.Minimized;
                    }
                    break;
                case ButtonHoverState.Maximize:
                    if (_EnableMaximize == true)
                    {
                        if (Parent.FindForm().WindowState == FormWindowState.Normal)
                        {
                            Parent.FindForm().WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            Parent.FindForm().WindowState = FormWindowState.Normal;
                        }
                    }
                    break;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ButtonHState = ButtonHoverState.None;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        #endregion

        public MonoFlat_ControlBox()
            : base()
        {
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            try
            {
                Location = new Point(Parent.Width - 112, 15);
            }
            catch (Exception)
            {
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            G.Clear(Color.FromArgb(181, 41, 42));

            if (_EnableHoverHighlight == true)
            {
                switch (ButtonHState)
                {
                    case ButtonHoverState.None:
                        G.Clear(Color.FromArgb(181, 41, 42));
                        break;
                    case ButtonHoverState.Minimize:
                        if (_EnableMinimize == true)
                        {
                            G.FillRectangle(new SolidBrush(Color.FromArgb(156, 35, 35)), new Rectangle(3, 0, 30, Height));
                        }
                        break;
                    case ButtonHoverState.Maximize:
                        if (_EnableMaximize == true)
                        {
                            G.FillRectangle(new SolidBrush(Color.FromArgb(156, 35, 35)), new Rectangle(35, 0, 30, Height));
                        }
                        break;
                    case ButtonHoverState.Close:
                        G.FillRectangle(new SolidBrush(Color.FromArgb(156, 35, 35)), new Rectangle(66, 0, 35, Height));
                        break;
                }
            }

            //Close
            G.DrawString("r", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(255, 254, 255)), new Point(Width - 16, 8), new StringFormat { Alignment = StringAlignment.Center });

            //Maximize
            switch (Parent.FindForm().WindowState)
            {
                case FormWindowState.Maximized:
                    if (_EnableMaximize == true)
                    {
                        G.DrawString("2", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(255, 254, 255)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("2", new Font("Marlett", 12), new SolidBrush(Color.LightGray), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    break;
                case FormWindowState.Normal:
                    if (_EnableMaximize == true)
                    {
                        G.DrawString("1", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(255, 254, 255)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("1", new Font("Marlett", 12), new SolidBrush(Color.LightGray), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    break;
            }

            //Minimize
            if (_EnableMinimize == true)
            {
                G.DrawString("0", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(255, 254, 255)), new Point(20, 7), new StringFormat { Alignment = StringAlignment.Center });
            }
            else
            {
                G.DrawString("0", new Font("Marlett", 12), new SolidBrush(Color.LightGray), new Point(20, 7), new StringFormat { Alignment = StringAlignment.Center });
            }
        }
    }
    class MonoFlat_ControlBox2 : Control
    {

        #region Enums

        public enum ButtonHoverState
        {
            Minimize,
            Maximize,
            Close,
            None
        }

        #endregion
        #region Variables

        private ButtonHoverState ButtonHState = ButtonHoverState.None;

        #endregion
        #region Properties

        private bool _EnableMaximize = true;
        public bool EnableMaximizeButton
        {
            get { return _EnableMaximize; }
            set
            {
                _EnableMaximize = value;
                Invalidate();
            }
        }

        private bool _EnableMinimize = true;
        public bool EnableMinimizeButton
        {
            get { return _EnableMinimize; }
            set
            {
                _EnableMinimize = value;
                Invalidate();
            }
        }

        private bool _EnableHoverHighlight = false;
        public bool EnableHoverHighlight
        {
            get { return _EnableHoverHighlight; }
            set
            {
                _EnableHoverHighlight = value;
                Invalidate();
            }
        }

        #endregion
        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(100, 25);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int X = e.Location.X;
            int Y = e.Location.Y;
            if (Y > 0 && Y < (Height - 2))
            {
                if (X > 0 && X < 34)
                {
                    ButtonHState = ButtonHoverState.Minimize;
                }
                else if (X > 33 && X < 65)
                {
                    ButtonHState = ButtonHoverState.Maximize;
                }
                else if (X > 64 && X < Width)
                {
                    ButtonHState = ButtonHoverState.Close;
                }
                else
                {
                    ButtonHState = ButtonHoverState.None;
                }
            }
            else
            {
                ButtonHState = ButtonHoverState.None;
            }
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            switch (ButtonHState)
            {
                case ButtonHoverState.Close:
                    Parent.FindForm().Close();
                    break;
                case ButtonHoverState.Minimize:
                    if (_EnableMinimize == true)
                    {
                        Parent.FindForm().WindowState = FormWindowState.Minimized;
                    }
                    break;
                case ButtonHoverState.Maximize:
                    if (_EnableMaximize == true)
                    {
                        if (Parent.FindForm().WindowState == FormWindowState.Normal)
                        {
                            Parent.FindForm().WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            Parent.FindForm().WindowState = FormWindowState.Normal;
                        }
                    }
                    break;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ButtonHState = ButtonHoverState.None;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        #endregion

        public MonoFlat_ControlBox2()
            : base()
        {
            DoubleBuffered = true;
            Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            try
            {
                Location = new Point(Parent.Width - 112, 15);
            }
            catch (Exception)
            {
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            G.Clear(Color.FromArgb(245, 245, 245));

            if (_EnableHoverHighlight == true)
            {
                switch (ButtonHState)
                {
                    case ButtonHoverState.None:
                        G.Clear(Color.FromArgb(245, 245, 245));
                        break;
                    case ButtonHoverState.Minimize:
                        if (_EnableMinimize == true)
                        {
                            G.FillRectangle(new SolidBrush(Color.FromArgb(82, 82, 82)), new Rectangle(3, 0, 30, Height));
                        }
                        break;
                    case ButtonHoverState.Maximize:
                        if (_EnableMaximize == true)
                        {
                            G.FillRectangle(new SolidBrush(Color.FromArgb(82, 82, 82)), new Rectangle(35, 0, 30, Height));
                        }
                        break;
                    case ButtonHoverState.Close:
                        G.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), new Rectangle(66, 0, 35, Height));
                        break;
                }
            }

            //Close
            G.DrawString("r", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(170, 0, 4)), new Point(Width - 16, 8), new StringFormat { Alignment = StringAlignment.Center });

            //Maximize
            switch (Parent.FindForm().WindowState)
            {
                case FormWindowState.Maximized:
                    if (_EnableMaximize == true)
                    {
                        G.DrawString("2", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(82, 82, 82)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("2", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(82, 82, 82)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    break;
                case FormWindowState.Normal:
                    if (_EnableMaximize == true)
                    {
                        G.DrawString("1", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(82, 82, 82)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("1", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(82, 82, 82)), new Point(51, 7), new StringFormat { Alignment = StringAlignment.Center });
                    }
                    break;
            }

            //Minimize
            if (_EnableMinimize == true)
            {
                G.DrawString("0", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(82, 82, 82)), new Point(20, 7), new StringFormat { Alignment = StringAlignment.Center });
            }
            else
            {
                G.DrawString("0", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(82, 82, 82)), new Point(20, 7), new StringFormat { Alignment = StringAlignment.Center });
            }
        }
    }

    #endregion
    #region  Button

    public class MonoFlat_Button : Control
    {

        #region  Variables

        private int MouseState;
        private GraphicsPath Shape;
        private LinearGradientBrush InactiveGB;
        private LinearGradientBrush PressedGB;
        private Rectangle R1;
        private Pen P1;
        private Pen P3;
        private Image _Image;
        private Size _ImageSize;
        private StringAlignment _TextAlignment = StringAlignment.Center;
        private Color _TextColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private ContentAlignment _ImageAlign = ContentAlignment.MiddleLeft;

        #endregion
        #region  Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = new PointF();
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = (float)((Area.Width - ImageArea.Width) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.X = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.X = Area.Width - ImageArea.Width - 2;
                    break;

            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = (float)((Area.Height - ImageArea.Height) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.Y = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.Y = Area.Height - ImageArea.Height - 2;
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Far;
                    break;
            }
            return SF;
        }

        #endregion
        #region  Properties

        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
        }

        public ContentAlignment ImageAlign
        {
            get
            {
                return _ImageAlign;
            }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        public StringAlignment TextAlignment
        {
            get
            {
                return this._TextAlignment;
            }
            set
            {
                this._TextAlignment = value;
                this.Invalidate();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return this._TextColor;
            }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseState = 1;
            Focus();
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion

        public MonoFlat_Button()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 12);
            ForeColor = Color.FromArgb(255, 255, 255);
            Size = new Size(146, 41);
            _TextAlignment = StringAlignment.Center;
            P1 = new Pen(Color.FromArgb(181, 41, 42)); // P1 = Border color
            P3 = new Pen(Color.FromArgb(165, 37, 37)); // P3 = Border color when pressed
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {

                Shape = new GraphicsPath();
                R1 = new Rectangle(0, 0, Width, Height);

                InactiveGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(181, 41, 42), Color.FromArgb(181, 41, 42), 90.0F);
                PressedGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(165, 37, 37), Color.FromArgb(165, 37, 37), 90.0F);
            }

            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            PointF ipt = ImageLocation(GetStringFormat(ImageAlign), Size, ImageSize);

            switch (MouseState)
            {
                case 0:
                    //Inactive
                    G.FillPath(InactiveGB, Shape);
                    // Fill button body with InactiveGB color gradient
                    G.DrawPath(P1, Shape);
                    // Draw button border [InactiveGB]
                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
                case 1:
                    //Pressed
                    G.FillPath(PressedGB, Shape);
                    // Fill button body with PressedGB color gradient
                    G.DrawPath(P3, Shape);
                    // Draw button border [PressedGB]

                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
            }
            base.OnPaint(e);
        }
    }
    public class MonoFlat_Button2 : Control
    {

        #region  Variables

        private int MouseState;
        private GraphicsPath Shape;
        private LinearGradientBrush InactiveGB;
        private LinearGradientBrush PressedGB;
        private Rectangle R1;
        private Pen P1;
        private Pen P3;
        private Image _Image;
        private Size _ImageSize;
        private StringAlignment _TextAlignment = StringAlignment.Center;
        private Color _TextColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private ContentAlignment _ImageAlign = ContentAlignment.MiddleLeft;

        #endregion
        #region  Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = new PointF();
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = (float)((Area.Width - ImageArea.Width) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.X = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.X = Area.Width - ImageArea.Width - 2;
                    break;

            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = (float)((Area.Height - ImageArea.Height) / 2);
                    break;
                case StringAlignment.Near:
                    MyPoint.Y = 2;
                    break;
                case StringAlignment.Far:
                    MyPoint.Y = Area.Height - ImageArea.Height - 2;
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.TopCenter:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopLeft:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    SF.LineAlignment = StringAlignment.Near;
                    SF.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                    SF.LineAlignment = StringAlignment.Far;
                    SF.Alignment = StringAlignment.Far;
                    break;
            }
            return SF;
        }

        #endregion
        #region  Properties

        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
        }

        public ContentAlignment ImageAlign
        {
            get
            {
                return _ImageAlign;
            }
            set
            {
                _ImageAlign = value;
                Invalidate();
            }
        }

        public StringAlignment TextAlignment
        {
            get
            {
                return this._TextAlignment;
            }
            set
            {
                this._TextAlignment = value;
                this.Invalidate();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return this._TextColor;
            }
            set
            {
                this._TextColor = value;
                this.Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseState = 1;
            Focus();
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseState = 0;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        #endregion

        public MonoFlat_Button2()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint), true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 12);
            ForeColor = Color.FromArgb(255, 255, 255);
            Size = new Size(146, 41);
            _TextAlignment = StringAlignment.Center;
            P1 = new Pen(Color.FromArgb(39, 75, 240)); // P1 = Border color
            P3 = new Pen(Color.FromArgb(165, 37, 37)); // P3 = Border color when pressed
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {

                Shape = new GraphicsPath();
                R1 = new Rectangle(0, 0, Width, Height);

                InactiveGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(39, 75, 240), Color.FromArgb(39, 75, 120), 90.0F);
                PressedGB = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(165, 37, 37), Color.FromArgb(165, 37, 37), 90.0F);
            }

            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            PointF ipt = ImageLocation(GetStringFormat(ImageAlign), Size, ImageSize);

            switch (MouseState)
            {
                case 0:
                    //Inactive
                    G.FillPath(InactiveGB, Shape);
                    // Fill button body with InactiveGB color gradient
                    G.DrawPath(P1, Shape);
                    // Draw button border [InactiveGB]
                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
                case 1:
                    //Pressed
                    G.FillPath(PressedGB, Shape);
                    // Fill button body with PressedGB color gradient
                    G.DrawPath(P3, Shape);
                    // Draw button border [PressedGB]

                    if ((Image == null))
                    {
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    else
                    {
                        G.DrawImage(_Image, ipt.X, ipt.Y, ImageSize.Width, ImageSize.Height);
                        G.DrawString(Text, Font, new SolidBrush(ForeColor), R1, new StringFormat
                        {
                            Alignment = _TextAlignment,
                            LineAlignment = StringAlignment.Center
                        });
                    }
                    break;
            }
            base.OnPaint(e);
        }
    }

    #endregion
    #region  Social Button

    public class MonoFlat_SocialButton : Control
    {

        #region  Variables

        private Image _Image;
        private Size _ImageSize;
        private Color EllipseColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        #endregion
        #region  Properties

        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(54, 54);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            EllipseColor = Color.FromArgb(181, 41, 42);
            Refresh();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            EllipseColor = Color.FromArgb(66, 76, 85);
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            EllipseColor = Color.FromArgb(153, 34, 34);
            Focus();
            Refresh();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            EllipseColor = Color.FromArgb(181, 41, 42);
            Refresh();
        }

        #endregion
        #region  Image Designer

        private static PointF ImageLocation(StringFormat SF, SizeF Area, SizeF ImageArea)
        {
            PointF MyPoint = new PointF();
            switch (SF.Alignment)
            {
                case StringAlignment.Center:
                    MyPoint.X = (float)((Area.Width - ImageArea.Width) / 2);
                    break;
            }

            switch (SF.LineAlignment)
            {
                case StringAlignment.Center:
                    MyPoint.Y = (float)((Area.Height - ImageArea.Height) / 2);
                    break;
            }
            return MyPoint;
        }

        private StringFormat GetStringFormat(ContentAlignment _ContentAlignment)
        {
            StringFormat SF = new StringFormat();
            switch (_ContentAlignment)
            {
                case ContentAlignment.MiddleCenter:
                    SF.LineAlignment = StringAlignment.Center;
                    SF.Alignment = StringAlignment.Center;
                    break;
            }
            return SF;
        }

        #endregion

        public MonoFlat_SocialButton()
        {
            DoubleBuffered = true;
            EllipseColor = Color.FromArgb(66, 76, 85);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.Clear(Parent.BackColor);
            G.SmoothingMode = SmoothingMode.HighQuality;

            PointF ImgPoint = ImageLocation(GetStringFormat(ContentAlignment.MiddleCenter), Size, ImageSize);
            G.FillEllipse(new SolidBrush(EllipseColor), new Rectangle(0, 0, 53, 53));

            // HINTS:
            // The best size for the drawn image is 32x32\
            // The best matching color of drawn image is (RGB: 31, 40, 49)
            if (Image != null)
            {
                G.DrawImage(_Image, (int)ImgPoint.X, (int)ImgPoint.Y, ImageSize.Width, ImageSize.Height);
            }
        }
    }

    #endregion
    #region  Label

    public class MonoFlat_Label : Label
    {

        public MonoFlat_Label()
        {
            Font = new Font("Segoe UI", 9);
            ForeColor = Color.FromArgb(116, 125, 132);
            BackColor = Color.Transparent;
        }
    }

    #endregion
    #region  Link Label
    public class MonoFlat_LinkLabel : LinkLabel
    {

        public MonoFlat_LinkLabel()
        {
            Font = new Font("Segoe UI", 9, FontStyle.Regular);
            BackColor = Color.Transparent;
            LinkColor = Color.FromArgb(181, 41, 42);
            ActiveLinkColor = Color.FromArgb(153, 34, 34);
            VisitedLinkColor = Color.FromArgb(181, 41, 42);
            LinkBehavior = LinkBehavior.NeverUnderline;
        }
    }

    #endregion
    #region  Header Label

    public class MonoFlat_HeaderLabel : Label
    {

        public MonoFlat_HeaderLabel()
        {
            Font = new Font("Segoe UI", 11, FontStyle.Bold);
            ForeColor = Color.FromArgb(255, 255, 255);
            BackColor = Color.Transparent;
        }
    }

    #endregion
    #region  Toggle Button

    [DefaultEvent("ToggledChanged")]
    public class MonoFlat_Toggle : Control
    {

        #region  Enums

        public enum _Type
        {
            CheckMark,
            OnOff,
            YesNo,
            IO
        }

        #endregion
        #region  Variables

        public delegate void ToggledChangedEventHandler();
        private ToggledChangedEventHandler ToggledChangedEvent;

        public event ToggledChangedEventHandler ToggledChanged
        {
            add
            {
                ToggledChangedEvent = (ToggledChangedEventHandler)System.Delegate.Combine(ToggledChangedEvent, value);
            }
            remove
            {
                ToggledChangedEvent = (ToggledChangedEventHandler)System.Delegate.Remove(ToggledChangedEvent, value);
            }
        }

        private bool _Toggled;
        private _Type ToggleType;
        private Rectangle Bar;
        private int _Width;
        private int _Height;

        #endregion
        #region  Properties

        public bool Toggled
        {
            get
            {
                return _Toggled;
            }
            set
            {
                _Toggled = value;
                Invalidate();
                if (ToggledChangedEvent != null)
                    ToggledChangedEvent();
            }
        }

        public _Type Type
        {
            get
            {
                return ToggleType;
            }
            set
            {
                ToggleType = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(76, 33);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Toggled = !Toggled;
            Focus();
        }

        #endregion

        public MonoFlat_Toggle()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint), true);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            System.Drawing.Graphics G = e.Graphics;

            G.SmoothingMode = SmoothingMode.HighQuality;
            G.Clear(Parent.BackColor);
            _Width = Width - 1;
            _Height = Height - 1;

            GraphicsPath GP = default(GraphicsPath);
            GraphicsPath GP2 = new GraphicsPath();
            Rectangle BaseRect = new Rectangle(0, 0, _Width, _Height);
            Rectangle ThumbRect = new Rectangle(_Width / 2, 0, 38, _Height);

            G.SmoothingMode = (System.Drawing.Drawing2D.SmoothingMode)2;
            G.PixelOffsetMode = (System.Drawing.Drawing2D.PixelOffsetMode)2;
            G.TextRenderingHint = (System.Drawing.Text.TextRenderingHint)5;
            G.Clear(BackColor);

            GP = RoundRectangle.RoundRect(BaseRect, 4);
            ThumbRect = new Rectangle(4, 4, 36, _Height - 8);
            GP2 = RoundRectangle.RoundRect(ThumbRect, 4);
            G.FillPath(new SolidBrush(Color.FromArgb(66, 76, 85)), GP);
            G.FillPath(new SolidBrush(Color.FromArgb(32, 41, 50)), GP2);

            if (_Toggled)
            {
                GP = RoundRectangle.RoundRect(BaseRect, 4);
                ThumbRect = new Rectangle((_Width / 2) - 2, 4, 36, _Height - 8);
                GP2 = RoundRectangle.RoundRect(ThumbRect, 4);
                G.FillPath(new SolidBrush(Color.FromArgb(181, 41, 42)), GP);
                G.FillPath(new SolidBrush(Color.FromArgb(32, 41, 50)), GP2);
            }

            // Draw string
            switch (ToggleType)
            {
                case _Type.CheckMark:
                    if (Toggled)
                    {
                        G.DrawString("ü", new Font("Wingdings", 18, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 18, Bar.Y + 19, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("r", new Font("Marlett", 14, FontStyle.Regular), Brushes.DimGray, Bar.X + 59, Bar.Y + 18, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
                case _Type.OnOff:
                    if (Toggled)
                    {
                        G.DrawString("ON", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 18, Bar.Y + 16, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("OFF", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.DimGray, Bar.X + 57, Bar.Y + 16, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
                case _Type.YesNo:
                    if (Toggled)
                    {
                        G.DrawString("YES", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 19, Bar.Y + 16, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("NO", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.DimGray, Bar.X + 56, Bar.Y + 16, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
                case _Type.IO:
                    if (Toggled)
                    {
                        G.DrawString("I", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.WhiteSmoke, Bar.X + 18, Bar.Y + 16, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    else
                    {
                        G.DrawString("O", new Font("Segoe UI", 12, FontStyle.Regular), Brushes.DimGray, Bar.X + 57, Bar.Y + 16, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                    break;
            }
        }
    }

    #endregion
    #region  CheckBox

    [DefaultEvent("CheckedChanged")]
    public class MonoFlat_CheckBox : Control
    {

        #region  Variables

        private int X;
        private bool _Checked = false;
        private GraphicsPath Shape;

        #endregion
        #region  Properties

        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        public delegate void CheckedChangedEventHandler(object sender);
        private CheckedChangedEventHandler CheckedChangedEvent;

        public event CheckedChangedEventHandler CheckedChanged
        {
            add
            {
                CheckedChangedEvent = (CheckedChangedEventHandler)System.Delegate.Combine(CheckedChangedEvent, value);
            }
            remove
            {
                CheckedChangedEvent = (CheckedChangedEventHandler)System.Delegate.Remove(CheckedChangedEvent, value);
            }
        }


        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            X = e.Location.X;
            Invalidate();
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _Checked = !_Checked;
            Focus();
            if (CheckedChangedEvent != null)
                CheckedChangedEvent(this);
            base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Height = 16;

            Shape = new GraphicsPath();
            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
            Invalidate();
        }

        #endregion

        public MonoFlat_CheckBox()
        {
            Width = 148;
            Height = 16;
            Font = new Font("Microsoft Sans Serif", 9);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            G.Clear(Parent.BackColor);

            if (_Checked)
            {
                G.FillRectangle(new SolidBrush(Color.FromArgb(66, 76, 85)), new Rectangle(0, 0, 16, 16));
                G.FillRectangle(new SolidBrush(Color.FromArgb(66, 76, 85)), new Rectangle(1, 1, 16 - 2, 16 - 2));
            }
            else
            {
                G.FillRectangle(new SolidBrush(Color.FromArgb(66, 76, 85)), new Rectangle(0, 0, 16, 16));
                G.FillRectangle(new SolidBrush(Color.FromArgb(66, 76, 85)), new Rectangle(1, 1, 16 - 2, 16 - 2));
            }

            if (Enabled == true)
            {
                if (_Checked)
                {
                    G.DrawString("a", new Font("Marlett", 16), new SolidBrush(Color.FromArgb(181, 41, 42)), new Point(-5, -3));
                }
            }
            else
            {
                if (_Checked)
                {
                    G.DrawString("a", new Font("Marlett", 16), new SolidBrush(Color.Gray), new Point(-5, -3));
                }
            }

            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(116, 125, 132)), new Point(20, 0));
        }
    }
    #endregion
    #region  Radio Button

    [DefaultEvent("CheckedChanged")]
    public class MonoFlat_RadioButton : Control
    {

        #region  Variables

        private int X;
        private bool _Checked;

        #endregion
        #region  Properties

        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                InvalidateControls();
                if (CheckedChangedEvent != null)
                    CheckedChangedEvent(this);
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        public delegate void CheckedChangedEventHandler(object sender);
        private CheckedChangedEventHandler CheckedChangedEvent;

        public event CheckedChangedEventHandler CheckedChanged
        {
            add
            {
                CheckedChangedEvent = (CheckedChangedEventHandler)System.Delegate.Combine(CheckedChangedEvent, value);
            }
            remove
            {
                CheckedChangedEvent = (CheckedChangedEventHandler)System.Delegate.Remove(CheckedChangedEvent, value);
            }
        }


        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!_Checked)
            {
                @Checked = true;
            }
            Focus();
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            X = e.X;
            Invalidate();
        }
        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            int textSize = 0;
            textSize = (int)(this.CreateGraphics().MeasureString(Text, Font).Width);
            this.Width = 28 + textSize;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Height = 17;
        }

        #endregion

        public MonoFlat_RadioButton()
        {
            Width = 159;
            Height = 17;
            DoubleBuffered = true;
        }

        private void InvalidateControls()
        {
            if (!IsHandleCreated || !_Checked)
            {
                return;
            }

            foreach (Control _Control in Parent.Controls)
            {
                if (_Control != this && _Control is MonoFlat_RadioButton)
                {
                    ((MonoFlat_RadioButton)_Control).Checked = false;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;
            G.Clear(Parent.BackColor);
            G.SmoothingMode = SmoothingMode.HighQuality;

            G.FillEllipse(new SolidBrush(Color.FromArgb(66, 76, 85)), new Rectangle(0, 0, 16, 16));

            if (_Checked)
            {
                G.DrawString("a", new Font("Marlett", 15), new SolidBrush(Color.FromArgb(181, 41, 42)), new Point(-3, -2));
            }

            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(116, 125, 132)), new Point(20, 0));
        }
    }

    #endregion
    #region  TextBox

    [DefaultEvent("TextChanged")]
    public class MonoFlat_TextBox : Control
    {

        #region  Variables

        public TextBox MonoFlatTB = new TextBox();
        private int _maxchars = 32767;
        private bool _ReadOnly;
        private bool _Multiline;
        private Image _Image;
        private Size _ImageSize;
        private HorizontalAlignment ALNType;
        private bool isPasswordMasked = false;
        private Pen P1;
        private SolidBrush B1;
        private GraphicsPath Shape;

        #endregion
        #region  Properties

        public HorizontalAlignment TextAlignment
        {
            get
            {
                return ALNType;
            }
            set
            {
                ALNType = value;
                Invalidate();
            }
        }
        public int MaxLength
        {
            get
            {
                return _maxchars;
            }
            set
            {
                _maxchars = value;
                MonoFlatTB.MaxLength = MaxLength;
                Invalidate();
            }
        }

        public bool UseSystemPasswordChar
        {
            get
            {
                return isPasswordMasked;
            }
            set
            {
                MonoFlatTB.UseSystemPasswordChar = UseSystemPasswordChar;
                isPasswordMasked = value;
                Invalidate();
            }
        }
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                _ReadOnly = value;
                if (MonoFlatTB != null)
                {
                    MonoFlatTB.ReadOnly = value;
                }
            }
        }
        public bool Multiline
        {
            get
            {
                return _Multiline;
            }
            set
            {
                _Multiline = value;
                if (MonoFlatTB != null)
                {
                    MonoFlatTB.Multiline = value;

                    if (value)
                    {
                        MonoFlatTB.Height = Height - 23;
                    }
                    else
                    {
                        Height = MonoFlatTB.Height + 23;
                    }
                }
            }
        }

        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;

                if (Image == null)
                {
                    MonoFlatTB.Location = new Point(8, 10);
                }
                else
                {
                    MonoFlatTB.Location = new Point(35, 11);
                }
                Invalidate();
            }
        }

        protected Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
        }

        #endregion
        #region  EventArgs

        private void _Enter(object Obj, EventArgs e)
        {
            P1 = new Pen(Color.FromArgb(181, 41, 42));
            Refresh();
        }

        private void _Leave(object Obj, EventArgs e)
        {
            P1 = new Pen(Color.FromArgb(32, 41, 50));
            Refresh();
        }

        private void OnBaseTextChanged(object s, EventArgs e)
        {
            Text = MonoFlatTB.Text;
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            MonoFlatTB.Text = Text;
            Invalidate();
        }

        protected override void OnForeColorChanged(System.EventArgs e)
        {
            base.OnForeColorChanged(e);
            MonoFlatTB.ForeColor = ForeColor;
            Invalidate();
        }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
            MonoFlatTB.Font = Font;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        private void _OnKeyDown(object Obj, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                MonoFlatTB.SelectAll();
                e.SuppressKeyPress = true;
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                MonoFlatTB.Copy();
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (_Multiline)
            {
                MonoFlatTB.Height = Height - 23;
            }
            else
            {
                Height = MonoFlatTB.Height + 23;
            }

            Shape = new GraphicsPath();
            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            MonoFlatTB.Focus();
        }

        public void _TextChanged(System.Object sender, System.EventArgs e)
        {
            Text = MonoFlatTB.Text;
        }

        public void _BaseTextChanged(System.Object sender, System.EventArgs e)
        {
            MonoFlatTB.Text = Text;
        }

        #endregion

        public void AddTextBox()
        {
            MonoFlatTB.Location = new Point(8, 10);
            MonoFlatTB.Text = String.Empty;
            MonoFlatTB.BorderStyle = BorderStyle.None;
            MonoFlatTB.TextAlign = HorizontalAlignment.Left;
            MonoFlatTB.Font = new Font("Tahoma", 11);
            MonoFlatTB.UseSystemPasswordChar = UseSystemPasswordChar;
            MonoFlatTB.Multiline = false;
            MonoFlatTB.BackColor = Color.FromArgb(66, 76, 85);
            MonoFlatTB.ScrollBars = ScrollBars.None;
            MonoFlatTB.KeyDown += _OnKeyDown;
            MonoFlatTB.Enter += _Enter;
            MonoFlatTB.Leave += _Leave;
            MonoFlatTB.TextChanged += OnBaseTextChanged;
        }

        public MonoFlat_TextBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            AddTextBox();
            Controls.Add(MonoFlatTB);

            P1 = new Pen(Color.FromArgb(32, 41, 50));
            B1 = new SolidBrush(Color.FromArgb(66, 76, 85));
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(176, 183, 191);

            Text = null;
            Font = new Font("Tahoma", 11);
            Size = new Size(135, 43);
            DoubleBuffered = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.AntiAlias;


            if (Image == null)
            {
                MonoFlatTB.Width = Width - 18;
            }
            else
            {
                MonoFlatTB.Width = Width - 45;
            }

            MonoFlatTB.TextAlign = TextAlignment;
            MonoFlatTB.UseSystemPasswordChar = UseSystemPasswordChar;

            G.Clear(Color.Transparent);

            G.FillPath(B1, Shape);
            G.DrawPath(P1, Shape);

            if (Image != null)
            {
                G.DrawImage(_Image, 5, 8, 24, 24);
                // 24x24 is the perfect size of the image
            }

            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    #endregion
    #region  Panel

    public class MonoFlat_Panel : ContainerControl
    {

        private GraphicsPath Shape;

        public MonoFlat_Panel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);

            BackColor = Color.FromArgb(39, 51, 63);
            this.Size = new Size(187, 117);
            Padding = new Padding(5, 5, 5, 5);
            DoubleBuffered = true;
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            Shape = new GraphicsPath();
            Shape.AddArc(0, 0, 10, 10, 180, 90);
            Shape.AddArc(Width - 11, 0, 10, 10, -90, 90);
            Shape.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
            Shape.AddArc(0, Height - 11, 10, 10, 90, 90);
            Shape.CloseAllFigures();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Bitmap B = new Bitmap(Width, Height);
            var G = Graphics.FromImage(B);

            G.SmoothingMode = SmoothingMode.HighQuality;

            G.Clear(Color.FromArgb(32, 41, 50)); // Set control background to transparent
            G.FillPath(new SolidBrush(Color.FromArgb(39, 51, 63)), Shape); // Draw RTB background
            G.DrawPath(new Pen(Color.FromArgb(39, 51, 63)), Shape); // Draw border

            G.Dispose();
            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            B.Dispose();
        }
    }

    #endregion
    #region  Separator

    public class MonoFlat_Separator : Control
    {

        public MonoFlat_Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = (System.Drawing.Size)(new Point(120, 10));
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(45, 57, 68)), 0, 5, Width, 5);
        }
    }

    #endregion
    #region  TrackBar

    [DefaultEvent("ValueChanged")]
    public class MonoFlat_TrackBar : Control
    {

        #region  Enums

        public enum ValueDivisor
        {
            By1 = 1,
            By10 = 10,
            By100 = 100,
            By1000 = 1000
        }

        #endregion
        #region  Variables

        private Rectangle FillValue;
        private Rectangle PipeBorder;
        private Rectangle TrackBarHandleRect;
        private bool Cap;
        private int ValueDrawer;

        private Size ThumbSize = new Size(14, 14);
        private Rectangle TrackThumb;

        private int _Minimum = 0;
        private int _Maximum = 10;
        private int _Value = 0;

        private bool _JumpToMouse = false;
        private ValueDivisor DividedValue = ValueDivisor.By1;

        #endregion
        #region  Properties

        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {

                if (value >= _Maximum)
                {
                    value = _Maximum - 10;
                }
                if (_Value < value)
                {
                    _Value = value;
                }

                _Minimum = value;
                Invalidate();
            }
        }

        public int Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {

                if (value <= _Minimum)
                {
                    value = _Minimum + 10;
                }
                if (_Value > value)
                {
                    _Value = value;
                }

                _Maximum = value;
                Invalidate();
            }
        }

        public delegate void ValueChangedEventHandler();
        private ValueChangedEventHandler ValueChangedEvent;

        public event ValueChangedEventHandler ValueChanged
        {
            add
            {
                ValueChangedEvent = (ValueChangedEventHandler)System.Delegate.Combine(ValueChangedEvent, value);
            }
            remove
            {
                ValueChangedEvent = (ValueChangedEventHandler)System.Delegate.Remove(ValueChangedEvent, value);
            }
        }

        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != value)
                {
                    if (value < _Minimum)
                    {
                        _Value = _Minimum;
                    }
                    else
                    {
                        if (value > _Maximum)
                        {
                            _Value = _Maximum;
                        }
                        else
                        {
                            _Value = value;
                        }
                    }
                    Invalidate();
                    if (ValueChangedEvent != null)
                        ValueChangedEvent();
                }
            }
        }

        public ValueDivisor ValueDivison
        {
            get
            {
                return DividedValue;
            }
            set
            {
                DividedValue = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public float ValueToSet
        {
            get
            {
                return _Value / (int)DividedValue;
            }
            set
            {
                Value = (int)(value * (int)DividedValue);
            }
        }

        public bool JumpToMouse
        {
            get
            {
                return _JumpToMouse;
            }
            set
            {
                _JumpToMouse = value;
                Invalidate();
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            checked
            {
                bool flag = this.Cap && e.X > -1 && e.X < this.Width + 1;
                if (flag)
                {
                    this.Value = this._Minimum + (int)Math.Round((double)(this._Maximum - this._Minimum) * ((double)e.X / (double)this.Width));
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                this.ValueDrawer = (int)Math.Round(((double)(this._Value - this._Minimum) / (double)(this._Maximum - this._Minimum)) * (double)(this.Width - 11));
                TrackBarHandleRect = new Rectangle(ValueDrawer, 0, 25, 25);
                Cap = TrackBarHandleRect.Contains(e.Location);
                Focus();
                if (_JumpToMouse)
                {
                    this.Value = this._Minimum + (int)Math.Round((double)(this._Maximum - this._Minimum) * ((double)e.X / (double)this.Width));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cap = false;
        }

        #endregion

        public MonoFlat_TrackBar()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer), true);

            Size = new Size(80, 22);
            MinimumSize = new Size(47, 22);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Height = 22;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics G = e.Graphics;

            G.Clear(Parent.BackColor);
            G.SmoothingMode = SmoothingMode.AntiAlias;
            TrackThumb = new Rectangle(7, 10, Width - 16, 2);
            PipeBorder = new Rectangle(1, 10, Width - 3, 2);

            try
            {
                this.ValueDrawer = (int)Math.Round(((double)(this._Value - this._Minimum) / (double)(this._Maximum - this._Minimum)) * (double)(this.Width));
            }
            catch (Exception)
            {
            }

            TrackBarHandleRect = new Rectangle(ValueDrawer, 0, 3, 20);

            G.FillRectangle(new SolidBrush(Color.FromArgb(124, 131, 137)), PipeBorder);
            FillValue = new Rectangle(0, 10, TrackBarHandleRect.X + TrackBarHandleRect.Width - 4, 3);

            G.ResetClip();

            G.SmoothingMode = SmoothingMode.Default;
            G.DrawRectangle(new Pen(Color.FromArgb(124, 131, 137)), PipeBorder); // Draw pipe border
            G.FillRectangle(new SolidBrush(Color.FromArgb(181, 41, 42)), FillValue);

            G.ResetClip();

            G.SmoothingMode = SmoothingMode.HighQuality;

            G.FillEllipse(new SolidBrush(Color.FromArgb(181, 41, 42)), this.TrackThumb.X + (int)Math.Round(unchecked((double)this.TrackThumb.Width * ((double)this.Value / (double)this.Maximum))) - (int)Math.Round((double)this.ThumbSize.Width / 2.0), this.TrackThumb.Y + (int)Math.Round((double)this.TrackThumb.Height / 2.0) - (int)Math.Round((double)this.ThumbSize.Height / 2.0), this.ThumbSize.Width, this.ThumbSize.Height);
            G.DrawEllipse(new Pen(Color.FromArgb(181, 41, 42)), this.TrackThumb.X + (int)Math.Round(unchecked((double)this.TrackThumb.Width * ((double)this.Value / (double)this.Maximum))) - (int)Math.Round((double)this.ThumbSize.Width / 2.0), this.TrackThumb.Y + (int)Math.Round((double)this.TrackThumb.Height / 2.0) - (int)Math.Round((double)this.ThumbSize.Height / 2.0), this.ThumbSize.Width, this.ThumbSize.Height);
        }
    }

    #endregion
    #region  NotificationBox

    public class MonoFlat_NotificationBox : Control
    {

        #region  Variables

        private Point CloseCoordinates;
        private bool IsOverClose;
        private int _BorderCurve = 8;
        private GraphicsPath CreateRoundPath;
        private string NotificationText = null;
        private Type _NotificationType;
        private bool _RoundedCorners;
        private bool _ShowCloseButton;
        private Image _Image;
        private Size _ImageSize;

        #endregion
        #region  Enums

        // Create a list of Notification Types
        public enum Type
        {
            @Notice,
            @Success,
            @Warning,
            @Error
        }

        #endregion
        #region  Custom Properties

        // Create a NotificationType property and add the Type enum to it
        public Type NotificationType
        {
            get
            {
                return _NotificationType;
            }
            set
            {
                _NotificationType = value;
                Invalidate();
            }
        }
        // Boolean value to determine whether the control should use border radius
        public bool RoundCorners
        {
            get
            {
                return _RoundedCorners;
            }
            set
            {
                _RoundedCorners = value;
                Invalidate();
            }
        }
        // Boolean value to determine whether the control should draw the close button
        public bool ShowCloseButton
        {
            get
            {
                return _ShowCloseButton;
            }
            set
            {
                _ShowCloseButton = value;
                Invalidate();
            }
        }
        // Integer value to determine the curve level of the borders
        public int BorderCurve
        {
            get
            {
                return _BorderCurve;
            }
            set
            {
                _BorderCurve = value;
                Invalidate();
            }
        }
        // Image value to determine whether the control should draw an image before the header
        public Image Image
        {
            get
            {
                return _Image;
            }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }
        // Size value - returns the image size
        protected Size ImageSize
        {
            get
            {
                return _ImageSize;
            }
        }

        #endregion
        #region  EventArgs

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Decides the location of the drawn ellipse. If mouse is over the correct coordinates, "IsOverClose" boolean will be triggered to draw the ellipse
            if (e.X >= Width - 19 && e.X <= Width - 10 && e.Y > CloseCoordinates.Y && e.Y < CloseCoordinates.Y + 12)
            {
                IsOverClose = true;
            }
            else
            {
                IsOverClose = false;
            }
            // Updates the control
            Invalidate();
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Disposes the control when the close button is clicked
            if (_ShowCloseButton == true)
            {
                if (IsOverClose)
                {
                    Dispose();
                }
            }
        }

        #endregion

        internal GraphicsPath CreateRoundRect(Rectangle r, int curve)
        {
            // Draw a border radius
            try
            {
                CreateRoundPath = new GraphicsPath(FillMode.Winding);
                CreateRoundPath.AddArc(r.X, r.Y, curve, curve, 180.0F, 90.0F);
                CreateRoundPath.AddArc(r.Right - curve, r.Y, curve, curve, 270.0F, 90.0F);
                CreateRoundPath.AddArc(r.Right - curve, r.Bottom - curve, curve, curve, 0.0F, 90.0F);
                CreateRoundPath.AddArc(r.X, r.Bottom - curve, curve, curve, 90.0F, 90.0F);
                CreateRoundPath.CloseFigure();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Value must be either \'1\' or higher", "Invalid Integer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Return to the default border curve if the parameter is less than "1"
                _BorderCurve = 8;
                BorderCurve = 8;
            }
            return CreateRoundPath;
        }

        public MonoFlat_NotificationBox()
        {
            SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw), true);

            Font = new Font("Tahoma", 9);
            this.MinimumSize = new Size(100, 0);
            RoundCorners = false;
            ShowCloseButton = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            // Declare Graphics to draw the control
            Graphics GFX = e.Graphics;
            // Declare Color to paint the control's Text, Background and Border
            Color ForeColor = new Color();
            Color BackgroundColor = new Color();
            Color BorderColor = new Color();
            // Determine the header Notification Type font
            Font TypeFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            // Decalre a new rectangle to draw the control inside it
            Rectangle MainRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            // Declare a GraphicsPath to create a border radius
            GraphicsPath CrvBorderPath = CreateRoundRect(MainRectangle, _BorderCurve);

            GFX.SmoothingMode = SmoothingMode.HighQuality;
            GFX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            GFX.Clear(Parent.BackColor);

            switch (_NotificationType)
            {
                case Type.Notice:
                    BackgroundColor = Color.FromArgb(111, 177, 199);
                    BorderColor = Color.FromArgb(111, 177, 199);
                    ForeColor = Color.White;
                    break;
                case Type.Success:
                    BackgroundColor = Color.FromArgb(91, 195, 162);
                    BorderColor = Color.FromArgb(91, 195, 162);
                    ForeColor = Color.White;
                    break;
                case Type.Warning:
                    BackgroundColor = Color.FromArgb(254, 209, 108);
                    BorderColor = Color.FromArgb(254, 209, 108);
                    ForeColor = Color.DimGray;
                    break;
                case Type.Error:
                    BackgroundColor = Color.FromArgb(217, 103, 93);
                    BorderColor = Color.FromArgb(217, 103, 93);
                    ForeColor = Color.White;
                    break;
            }

            if (_RoundedCorners == true)
            {
                GFX.FillPath(new SolidBrush(BackgroundColor), CrvBorderPath);
                GFX.DrawPath(new Pen(BorderColor), CrvBorderPath);
            }
            else
            {
                GFX.FillRectangle(new SolidBrush(BackgroundColor), MainRectangle);
                GFX.DrawRectangle(new Pen(BorderColor), MainRectangle);
            }

            switch (_NotificationType)
            {
                case Type.Notice:
                    NotificationText = "DİKKAT!";
                    break;
                case Type.Success:
                    NotificationText = "BİLGİ";
                    break;
                case Type.Warning:
                    NotificationText = "UYARI!";
                    break;
                case Type.Error:
                    NotificationText = "KRİTİK UYARI!";
                    break;
            }

            if (Image == null)
            {
                GFX.DrawString(NotificationText, TypeFont, new SolidBrush(ForeColor), new Point(10, 5));
                GFX.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(10, 21, Width - 17, Height - 5));
            }
            else
            {
                GFX.DrawImage(_Image, 12, 4, 16, 16);
                GFX.DrawString(NotificationText, TypeFont, new SolidBrush(ForeColor), new Point(30, 5));
                GFX.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(10, 21, Width - 17, Height - 5));
            }

            CloseCoordinates = new Point(Width - 26, 4);

            if (_ShowCloseButton == true)
            {
                // Draw the close button
                GFX.DrawString("r", new Font("Marlett", 7, FontStyle.Regular), new SolidBrush(Color.FromArgb(130, 130, 130)), new Rectangle(Width - 20, 10, Width, Height), new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near });
            }

            CrvBorderPath.Dispose();
        }
    }

    #endregion

}