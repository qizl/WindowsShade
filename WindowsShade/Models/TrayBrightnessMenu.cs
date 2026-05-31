using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsShade.Models
{
    internal sealed class TrayBrightnessMenu : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _contextMenu;
        private readonly Action _openMainClick;
        private readonly Action _closeClick;
        private TrayBrightnessPopup _popup;
        private TrayCheckBox _shadeEnabledCheckBox;
        private TrayBrightnessSlider _alphaTrackBar;
        private Label _alphaValue;
        private TrayBrightnessSlider _systemTrackBar;
        private Label _systemValue;
        private bool _isRefreshing;

        public event Action<bool> ShadeEnabledChanged;
        public event Action<int> AlphaBrightnessChanged;
        public event Action<int> SystemBrightnessChanged;

        public TrayBrightnessMenu(NotifyIcon notifyIcon, ContextMenuStrip contextMenu, Action openMainClick, Action closeClick)
        {
            this._notifyIcon = notifyIcon;
            this._contextMenu = contextMenu;
            this._openMainClick = openMainClick;
            this._closeClick = closeClick;
        }

        public void Initialize()
        {
            this._contextMenu.Opening += (sender, e) => e.Cancel = true;
            this._notifyIcon.ContextMenuStrip = null;
            this.ensurePopup();
        }

        public void ShowAtCursor()
        {
            this.ensurePopup();

            using (TrayNativeWindowHelper.EnterTrayDpiAwareness())
            {
                var bounds = this.getBounds(TrayNativeWindowHelper.GetCursorPosition(), this._popup.Size);
                if (this._popup.Visible)
                    this._popup.Hide();

                this._popup.Bounds = bounds;
                this._popup.PrepareForDisplay();
                this._popup.Show();
                this._popup.Refresh();
                TrayNativeWindowHelper.ShowTopMostNoActivate(this._popup.Handle, bounds);
            }
        }

        public void SetState(int alpha, bool shadeEnabled, bool systemEnabled, int systemMaximum, int systemValue)
        {
            this.ensurePopup();

            this._isRefreshing = true;
            try
            {
                this._shadeEnabledCheckBox.Checked = shadeEnabled;
                this._alphaTrackBar.Value = this.clamp(this._alphaTrackBar, alpha);
                this._alphaValue.Text = alpha.ToString();

                this._systemTrackBar.Enabled = systemEnabled;
                if (systemEnabled)
                {
                    this._systemTrackBar.Maximum = systemMaximum;
                    this._systemTrackBar.Value = this.clamp(this._systemTrackBar, systemValue);
                    this._systemValue.Text = systemValue.ToString();
                }
                else
                {
                    this._systemTrackBar.Value = this.clamp(this._systemTrackBar, 0);
                    this._systemValue.Text = "不可用";
                }
            }
            finally
            {
                this._isRefreshing = false;
            }
        }

        public void Invalidate()
        {
            if (this._popup != null && !this._popup.IsDisposed)
                this._popup.Invalidate();
        }

        public void Dispose()
        {
            if (this._popup != null)
                this._popup.Dispose();
        }

        private void ensurePopup()
        {
            if (this._popup != null && !this._popup.IsDisposed)
                return;

            using (TrayNativeWindowHelper.EnterTrayDpiAwareness())
            {
                var popup = new TrayBrightnessPopup
                {
                    Size = new Size(344, 310)
                };

                popup.Deactivate += (sender, e) =>
                {
                    if (!popup.ShouldIgnoreDeactivate())
                        popup.Hide();
                };

                var panel = popup.ContentPanel;
                panel.Padding = new Padding(26, 14, 26, 14);

                const int captionY = 12;
                const int captionHeight = 30;

                this._shadeEnabledCheckBox = this.createCheckBox(30, captionY + 2, captionHeight);
                panel.Controls.Add(this._shadeEnabledCheckBox);

                panel.Controls.Add(this.createCaptionLabel("亮度", 55, captionY, 60, captionHeight, ContentAlignment.MiddleLeft));
                this._alphaValue = this.createCaptionValueLabel(264, captionY, captionHeight);
                this._alphaTrackBar = this.createTrackBar(26, 58, 255);
                panel.Controls.Add(this._alphaTrackBar);
                panel.Controls.Add(this._alphaValue);

                panel.Controls.Add(this.createCaptionLabel("系统亮度", 26, 102, 210, captionHeight, ContentAlignment.MiddleLeft));
                this._systemValue = this.createCaptionValueLabel(264, 102, captionHeight);
                this._systemTrackBar = this.createTrackBar(26, 148, 100);
                panel.Controls.Add(this._systemTrackBar);
                panel.Controls.Add(this._systemValue);

                this.createButton(panel, "打开主界面(&M)", 188, this._openMainClick);
                this.createButton(panel, "退出(&C)", 243, this._closeClick);

                this._shadeEnabledCheckBox.CheckedChanged += (sender, e) =>
                {
                    if (this._isRefreshing)
                        return;

                    this._popup.KeepVisibleTemporarily();
                    this.ShadeEnabledChanged?.Invoke(this._shadeEnabledCheckBox.Checked);
                };

                this._alphaTrackBar.ValueChanged += (sender, e) =>
                {
                    if (this._isRefreshing)
                        return;

                    this._alphaValue.Text = this._alphaTrackBar.Value.ToString();
                    this.AlphaBrightnessChanged?.Invoke(this._alphaTrackBar.Value);
                };

                this._systemTrackBar.ValueChanged += (sender, e) =>
                {
                    if (this._isRefreshing || !this._systemTrackBar.Enabled)
                        return;

                    this.SystemBrightnessChanged?.Invoke(this._systemTrackBar.Value);
                };

                this._popup = popup;
            }
        }

        private TrayCheckBox createCheckBox(int x, int y, int rowHeight)
        {
            const int checkBoxWidth = 22;
            return new TrayCheckBox
            {
                Location = new Point(x, y + (rowHeight - TrayCheckBox.PreferredControlHeight) / 2),
                Size = new Size(checkBoxWidth, TrayCheckBox.PreferredControlHeight)
            };
        }

        private TrayMenuButton createButton(Control parent, string text, int y, Action clickHandler)
        {
            var button = new TrayMenuButton
            {
                Location = new Point(8, y),
                Size = new Size(parent.Width - 16, 55),
                Font = TrayNativeWindowHelper.CreateTrayMenuFont(),
                Text = text
            };
            button.Click += (sender, e) =>
            {
                this._popup.Hide();
                clickHandler();
            };
            parent.Controls.Add(button);
            return button;
        }

        private Label createCaptionLabel(string text, int x, int y, int width, int height, ContentAlignment textAlign)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Font = TrayNativeWindowHelper.CreateTrayCaptionFont(),
                ForeColor = Color.FromArgb(88, 88, 88),
                Size = new Size(width, height),
                Text = text,
                TextAlign = textAlign
            };
        }

        private Label createCaptionValueLabel(int x, int y, int height)
        {
            return new Label
            {
                AutoSize = false,
                Location = new Point(x, y),
                Font = TrayNativeWindowHelper.CreateTrayCaptionFont(),
                ForeColor = Color.FromArgb(88, 88, 88),
                Size = new Size(54, height),
                TextAlign = ContentAlignment.MiddleRight
            };
        }

        private TrayBrightnessSlider createTrackBar(int x, int y, int maximum)
        {
            return new TrayBrightnessSlider
            {
                Location = new Point(x, y),
                Maximum = maximum,
                Minimum = 0,
                Size = new Size(292, 22)
            };
        }

        private Rectangle getBounds(Point anchor, Size size)
        {
            var area = TrayNativeWindowHelper.GetMonitorWorkingArea(anchor);
            var width = size.Width;
            var height = size.Height;
            var showAbove = anchor.Y >= area.Top + area.Height / 2;
            var alignRight = anchor.X >= area.Left + area.Width / 2;
            var x = alignRight ? anchor.X - width : anchor.X;
            var y = showAbove ? anchor.Y - height : anchor.Y;

            if (x + width > area.Right)
                x = area.Right - width;
            if (x < area.Left)
                x = area.Left;
            if (y + height > area.Bottom)
                y = area.Bottom - height;
            if (y < area.Top)
                y = area.Top;

            return new Rectangle(x, y, width, height);
        }

        private int clamp(TrayBrightnessSlider trackBar, int value)
        {
            if (value < trackBar.Minimum)
                return trackBar.Minimum;
            if (value > trackBar.Maximum)
                return trackBar.Maximum;
            return value;
        }
    }

    internal sealed class TrayBrightnessSlider : Control
    {
        private const int ThumbRadius = 7;
        private const int TrackHeight = 4;
        private bool _dragging;
        private bool _hovering;
        private int _minimum;
        private int _maximum = 100;
        private int _value;

        public event EventHandler ValueChanged;

        public bool IsDragging
        {
            get { return this._dragging; }
        }

        public int Minimum
        {
            get { return this._minimum; }
            set
            {
                this._minimum = value;
                if (this._maximum < this._minimum)
                    this._maximum = this._minimum;
                this.Value = this._value;
                this.Invalidate();
            }
        }

        public int Maximum
        {
            get { return this._maximum; }
            set
            {
                this._maximum = Math.Max(value, this._minimum);
                this.Value = this._value;
                this.Invalidate();
            }
        }

        public int Value
        {
            get { return this._value; }
            set
            {
                var newValue = Math.Max(this._minimum, Math.Min(this._maximum, value));
                if (this._value == newValue)
                    return;

                this._value = newValue;
                this.Invalidate();
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public TrayBrightnessSlider()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.UserPaint, true);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Cursor = this.Enabled ? Cursors.Hand : Cursors.Default;
            this.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this._hovering = true;
            this.Focus();
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this._hovering = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!this.Enabled || e.Button != MouseButtons.Left)
                return;

            this.Focus();
            this._dragging = true;
            this.Capture = true;
            this.updateValueFromX(e.X);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!this.Enabled || e.Delta == 0)
                return;

            var step = Math.Max(1, SystemInformation.MouseWheelScrollLines);
            this.Value += e.Delta > 0 ? step : -step;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this._dragging)
                this.updateValueFromX(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != MouseButtons.Left)
                return;

            this._dragging = false;
            this.Capture = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = this.getTrackBounds();
            var radius = TrackHeight / 2;
            var thumbX = this.valueToX();
            var activeBounds = new Rectangle(bounds.Left, bounds.Top, Math.Max(0, thumbX - bounds.Left), bounds.Height);
            var disabled = !this.Enabled;
            var backColor = disabled ? Color.FromArgb(232, 232, 232) : Color.FromArgb(226, 229, 233);
            var activeColor = disabled ? Color.FromArgb(190, 190, 190) : Color.FromArgb(0, 120, 215);
            var thumbColor = disabled ? Color.FromArgb(178, 178, 178) : Color.FromArgb(0, 120, 215);

            using (var path = TrayMenuGeometry.CreateRoundRect(bounds, radius))
            using (var brush = new SolidBrush(backColor))
                e.Graphics.FillPath(brush, path);

            if (activeBounds.Width > 0)
            {
                using (var path = TrayMenuGeometry.CreateRoundRect(activeBounds, radius))
                using (var brush = new SolidBrush(activeColor))
                    e.Graphics.FillPath(brush, path);
            }

            var currentThumbRadius = this._dragging || this._hovering ? ThumbRadius + 1 : ThumbRadius;
            var thumbBounds = new Rectangle(thumbX - currentThumbRadius, bounds.Top + bounds.Height / 2 - currentThumbRadius, currentThumbRadius * 2, currentThumbRadius * 2);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0)))
                e.Graphics.FillEllipse(shadowBrush, new Rectangle(thumbBounds.X, thumbBounds.Y + 1, thumbBounds.Width, thumbBounds.Height));
            using (var brush = new SolidBrush(thumbColor))
                e.Graphics.FillEllipse(brush, thumbBounds);
        }

        private Rectangle getTrackBounds()
        {
            var left = ThumbRadius + 2;
            var width = Math.Max(1, this.Width - (ThumbRadius + 2) * 2);
            return new Rectangle(left, (this.Height - TrackHeight) / 2, width, TrackHeight);
        }

        private int valueToX()
        {
            var bounds = this.getTrackBounds();
            if (this._maximum == this._minimum)
                return bounds.Left;

            var percent = (double)(this._value - this._minimum) / (this._maximum - this._minimum);
            return bounds.Left + (int)Math.Round(bounds.Width * percent);
        }

        private void updateValueFromX(int x)
        {
            var bounds = this.getTrackBounds();
            var clampedX = Math.Max(bounds.Left, Math.Min(bounds.Right, x));
            var percent = (double)(clampedX - bounds.Left) / bounds.Width;
            this.Value = this._minimum + (int)Math.Round((this._maximum - this._minimum) * percent);
        }
    }

    internal sealed class TrayBrightnessPopup : Form
    {
        private const int AutoHideMargin = 6;
        private readonly Panel _contentPanel;
        private readonly Timer _autoHideTimer;
        private bool _hasMouseEntered;
        private DateTime _ignoreDeactivateUntil = DateTime.MinValue;

        public Panel ContentPanel => this._contentPanel;

        public void KeepVisibleTemporarily()
        {
            this._ignoreDeactivateUntil = DateTime.Now.AddMilliseconds(350);
        }

        public bool ShouldIgnoreDeactivate()
        {
            return DateTime.Now <= this._ignoreDeactivateUntil;
        }

        public void PrepareForDisplay()
        {
            if (!this.IsHandleCreated)
                this.CreateControl();

            this._contentPanel.Refresh();
            this.Refresh();
        }

        public TrayBrightnessPopup()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.Fuchsia;
            this.TransparencyKey = Color.Fuchsia;
            this.Padding = new Padding(0);
            this.TopMost = true;
            this.AutoScaleMode = AutoScaleMode.None;

            this._autoHideTimer = new Timer
            {
                Interval = 120
            };
            this._autoHideTimer.Tick += this.autoHideTimer_Tick;

            this._contentPanel = new Panel
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(this._contentPanel);
        }

        protected override bool ShowWithoutActivation => true;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this._autoHideTimer.Start();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                this._hasMouseEntered = false;
                this._autoHideTimer.Start();
            }
            else
            {
                this._hasMouseEntered = false;
                this._autoHideTimer.Stop();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this._autoHideTimer.Dispose();

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Fuchsia);
            var bounds = new Rectangle(1, 1, this.Width - 3, this.Height - 3);
            using (var path = TrayMenuGeometry.CreateRoundRect(bounds, 12))
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.FillPath(brush, path);

            using (var path = TrayMenuGeometry.CreateRoundRect(bounds, 12))
            using (var pen = new Pen(Color.FromArgb(212, 212, 212)))
                e.Graphics.DrawPath(pen, path);
        }

        private void autoHideTimer_Tick(object sender, EventArgs e)
        {
            if (!this.Visible || this.isSliderDragging() || MouseButtons == MouseButtons.Left)
                return;

            var bounds = this.Bounds;
            bounds.Inflate(AutoHideMargin, AutoHideMargin);
            if (bounds.Contains(TrayNativeWindowHelper.GetCursorPosition()))
            {
                this._hasMouseEntered = true;
                return;
            }

            if (this._hasMouseEntered)
                this.Hide();
        }

        private bool isSliderDragging()
        {
            foreach (Control control in this._contentPanel.Controls)
            {
                var slider = control as TrayBrightnessSlider;
                if (slider != null && slider.IsDragging)
                    return true;
            }

            return false;
        }
    }

    internal sealed class TrayMenuButton : Control
    {
        private const int TextVerticalPadding = 8;
        private const int HoverVerticalPadding = 1;
        private bool _hovering;

        public TrayMenuButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this._hovering = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this._hovering = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(brush, this.ClientRectangle);

            if (this._hovering)
            {
                var shadowBounds = new Rectangle(4, HoverVerticalPadding + 1, this.Width - 8, this.Height - HoverVerticalPadding * 2);
                var hoverBounds = new Rectangle(3, HoverVerticalPadding, this.Width - 6, this.Height - HoverVerticalPadding * 2);
                using (var path = TrayMenuGeometry.CreateRoundRect(shadowBounds, 5))
                using (var brush = new SolidBrush(Color.FromArgb(22, 0, 0, 0)))
                    e.Graphics.FillPath(brush, path);
                using (var path = TrayMenuGeometry.CreateRoundRect(hoverBounds, 5))
                using (var brush = new SolidBrush(Color.FromArgb(244, 244, 244)))
                    e.Graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                this.Font,
                new Rectangle(18, TextVerticalPadding, this.Width - 36, this.Height - TextVerticalPadding * 2),
                Color.FromArgb(32, 32, 32),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }

    internal sealed class TrayCheckBox : CheckBox
    {
        private const int BoxSize = 15;
        public const int PreferredControlHeight = 30;

        public TrayCheckBox()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.Cursor = Cursors.Hand;
            this.TabStop = false;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(brush, this.ClientRectangle);

            var boxBounds = new Rectangle(0, (this.Height - BoxSize) / 2, BoxSize, BoxSize);
            var borderColor = this.Checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(118, 118, 118);
            var fillColor = this.Checked ? Color.FromArgb(0, 120, 215) : Color.White;

            using (var path = TrayMenuGeometry.CreateRoundRect(boxBounds, 3))
            using (var brush = new SolidBrush(fillColor))
                e.Graphics.FillPath(brush, path);

            using (var path = TrayMenuGeometry.CreateRoundRect(boxBounds, 3))
            using (var pen = new Pen(borderColor))
                e.Graphics.DrawPath(pen, path);

            if (!this.Checked)
                return;

            using (var pen = new Pen(Color.White, 2F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                e.Graphics.DrawLines(pen, new[]
                {
                    new Point(boxBounds.Left + 4, boxBounds.Top + 8),
                    new Point(boxBounds.Left + 7, boxBounds.Top + 11),
                    new Point(boxBounds.Left + 12, boxBounds.Top + 5)
                });
            }
        }
    }

    internal static class TrayNativeWindowHelper
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        public static void ShowTopMostNoActivate(IntPtr handle, Rectangle bounds)
        {
            if (handle == IntPtr.Zero || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            SetWindowPos(handle, HWND_TOPMOST, bounds.X, bounds.Y, bounds.Width, bounds.Height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        public static IDisposable EnterTrayDpiAwareness()
        {
            try
            {
                return new DpiAwarenessScope(SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2));
            }
            catch
            {
                return new DpiAwarenessScope(IntPtr.Zero);
            }
        }

        public static Font CreateTrayMenuFont()
        {
            return new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public static Font CreateTrayCaptionFont()
        {
            return new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public static Point GetCursorPosition()
        {
            POINT point;
            if (GetCursorPos(out point))
                return new Point(point.X, point.Y);

            return Cursor.Position;
        }

        public static Rectangle GetMonitorWorkingArea(Point point)
        {
            var monitor = MonitorFromPoint(new POINT(point), MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                if (GetMonitorInfo(monitor, ref monitorInfo))
                {
                    return Rectangle.FromLTRB(
                        monitorInfo.rcWork.Left,
                        monitorInfo.rcWork.Top,
                        monitorInfo.rcWork.Right,
                        monitorInfo.rcWork.Bottom);
                }
            }

            return Screen.FromPoint(point).WorkingArea;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(Point point)
            {
                this.X = point.X;
                this.Y = point.Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        private sealed class DpiAwarenessScope : IDisposable
        {
            private readonly IntPtr _previousContext;

            public DpiAwarenessScope(IntPtr previousContext)
            {
                this._previousContext = previousContext;
            }

            public void Dispose()
            {
                if (this._previousContext != IntPtr.Zero)
                    SetThreadDpiAwarenessContext(this._previousContext);
            }
        }
    }

    internal static class TrayMenuGeometry
    {
        public static GraphicsPath CreateRoundRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var diameter = Math.Max(1, radius * 2);
            if (bounds.Width <= diameter || bounds.Height <= diameter)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
