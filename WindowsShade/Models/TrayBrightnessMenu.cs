using System;
using System.Collections.Generic;
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
            // 托盘菜单使用自绘弹窗，避免 ContextMenuStrip 在遮罩置顶和 DPI 切换时出现缩放、阴影或定位不一致。
            this._contextMenu.Opening += (sender, e) => e.Cancel = true;
            this._notifyIcon.ContextMenuStrip = null;
            this.ensurePopup();
        }

        public void ShowAtCursor()
        {
            this.ensurePopup();

            using (TrayNativeWindowHelper.EnterTrayDpiAwareness())
            {
                var cursor = TrayNativeWindowHelper.GetCursorPosition();
                var workingArea = TrayNativeWindowHelper.GetMonitorWorkingArea(cursor);
                // 每次打开都按鼠标所在屏幕重新布局，支持 1080p 与 4K 缩放屏幕来回切换。
                this.applyLayout(TrayNativeWindowHelper.GetDpiScale(cursor), workingArea);

                var bounds = this.getBounds(cursor, this._popup.Size, workingArea);
                if (this._popup.Visible)
                    this._popup.Hide();

                this._popup.Bounds = bounds;
                this._popup.PrepareForDisplay();
                this._popup.Show();
                TrayNativeWindowHelper.ShowTopMostNoActivate(this._popup.Handle, bounds);
                this._popup.Invalidate(true);
                this._popup.Update();
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
                var popup = new TrayBrightnessPopup();

                popup.Deactivate += (sender, e) =>
                {
                    if (!popup.ShouldIgnoreDeactivate())
                        popup.Hide();
                };

                var panel = popup.ContentPanel;

                this._shadeEnabledCheckBox = this.createCheckBox();
                panel.Controls.Add(this._shadeEnabledCheckBox);

                var alphaCaption = this.createCaptionLabel("亮度", ContentAlignment.MiddleLeft);
                alphaCaption.Cursor = Cursors.Hand;
                alphaCaption.Click += (sender, e) =>
                {
                    this._popup.KeepVisibleTemporarily();
                    this._shadeEnabledCheckBox.Checked = !this._shadeEnabledCheckBox.Checked;
                };
                panel.Controls.Add(alphaCaption);
                this._alphaValue = this.createCaptionValueLabel();
                this._alphaTrackBar = this.createTrackBar(255);
                panel.Controls.Add(this._alphaTrackBar);
                panel.Controls.Add(this._alphaValue);

                panel.Controls.Add(this.createCaptionLabel("系统亮度", ContentAlignment.MiddleLeft));
                this._systemValue = this.createCaptionValueLabel();
                this._systemTrackBar = this.createTrackBar(100);
                panel.Controls.Add(this._systemTrackBar);
                panel.Controls.Add(this._systemValue);

                panel.AddMenuItem("打开主界面(&M)", new Rectangle(8, 188, panel.Width - 16, 55), () =>
                {
                    this._popup.Hide();
                    this._openMainClick();
                });
                panel.AddMenuItem("退出(&C)", new Rectangle(8, 243, panel.Width - 16, 55), () =>
                {
                    this._popup.Hide();
                    this._closeClick();
                });

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
                this.applyLayout(1F, Screen.PrimaryScreen.WorkingArea);
            }
        }

        private void applyLayout(float scale, Rectangle workingArea)
        {
            var layout = TrayLayout.Create(scale, workingArea);

            // 所有坐标由 TrayLayout 统一下发，避免 1080 compact 和 4K regular 的微调互相影响。
            var panel = this._popup.ContentPanel;
            this._popup.ClientSize = layout.PopupSize;
            panel.Padding = layout.PanelPadding;

            this._shadeEnabledCheckBox.Location = layout.ShadeCheckBoxBounds.Location;
            this._shadeEnabledCheckBox.Size = layout.ShadeCheckBoxBounds.Size;

            var alphaCaption = (Label)panel.Controls[1];
            alphaCaption.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.CaptionFontSize);
            alphaCaption.Location = layout.AlphaCaptionBounds.Location;
            alphaCaption.Size = layout.AlphaCaptionBounds.Size;

            this._alphaValue.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.ValueFontSize);
            this._alphaValue.Location = layout.AlphaValueBounds.Location;
            this._alphaValue.Size = layout.AlphaValueBounds.Size;

            this._alphaTrackBar.Location = layout.AlphaTrackBarBounds.Location;
            this._alphaTrackBar.Size = layout.AlphaTrackBarBounds.Size;
            this._alphaTrackBar.TrackHeight = layout.SliderTrackHeight;
            this._alphaTrackBar.ThumbRadius = layout.SliderThumbRadius;

            var systemCaption = (Label)panel.Controls[4];
            systemCaption.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.CaptionFontSize);
            systemCaption.Location = layout.SystemCaptionBounds.Location;
            systemCaption.Size = layout.SystemCaptionBounds.Size;

            this._systemValue.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.ValueFontSize);
            this._systemValue.Location = layout.SystemValueBounds.Location;
            this._systemValue.Size = layout.SystemValueBounds.Size;

            this._systemTrackBar.Location = layout.SystemTrackBarBounds.Location;
            this._systemTrackBar.Size = layout.SystemTrackBarBounds.Size;
            this._systemTrackBar.TrackHeight = layout.SliderTrackHeight;
            this._systemTrackBar.ThumbRadius = layout.SliderThumbRadius;

            panel.Font = TrayNativeWindowHelper.CreateTrayMenuFont(layout.MenuFontSize);
            panel.TextVerticalPadding = layout.MenuTextVerticalPadding;
            panel.TextLeftPadding = layout.MenuTextLeftPadding;
            panel.SetMenuItemBounds(0, layout.OpenMainBounds);
            panel.SetMenuItemBounds(1, layout.CloseBounds);
        }

        private TrayCheckBox createCheckBox()
        {
            return new TrayCheckBox
            {
                Size = new Size(22, TrayCheckBox.PreferredControlHeight)
            };
        }

        private Label createCaptionLabel(string text, ContentAlignment textAlign)
        {
            return new Label
            {
                AutoSize = false,
                Font = TrayNativeWindowHelper.CreateTrayCaptionFont(),
                ForeColor = Color.FromArgb(88, 88, 88),
                Text = text,
                TextAlign = textAlign
            };
        }

        private Label createCaptionValueLabel()
        {
            return new Label
            {
                AutoSize = false,
                Font = TrayNativeWindowHelper.CreateTrayCaptionFont(),
                ForeColor = Color.FromArgb(88, 88, 88),
                TextAlign = ContentAlignment.MiddleRight
            };
        }

        private TrayBrightnessSlider createTrackBar(int maximum)
        {
            return new TrayBrightnessSlider
            {
                Maximum = maximum,
                Minimum = 0,
                Size = new Size(292, 22)
            };
        }

        private Rectangle getBounds(Point anchor, Size size, Rectangle area)
        {
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

    internal struct TrayLayout
    {
        private const float BaseDpi = 144F;

        public Size PopupSize;
        public Padding PanelPadding;
        public Rectangle ShadeCheckBoxBounds;
        public Rectangle AlphaCaptionBounds;
        public Rectangle AlphaValueBounds;
        public Rectangle AlphaTrackBarBounds;
        public Rectangle SystemCaptionBounds;
        public Rectangle SystemValueBounds;
        public Rectangle SystemTrackBarBounds;
        public Rectangle OpenMainBounds;
        public Rectangle CloseBounds;
        public float CaptionFontSize;
        public float ValueFontSize;
        public float MenuFontSize;
        public int MenuTextVerticalPadding;
        public int MenuTextLeftPadding;
        public int SliderTrackHeight;
        public int SliderThumbRadius;

        public static TrayLayout Create(float dpiScale, Rectangle workingArea)
        {
            // 低 DPI 与高 DPI 菜单的视觉调参完全拆开，后续修 1080 不应改动 4K 布局。
            if (dpiScale < 1.25F)
                return CreateCompact(workingArea);

            return CreateRegular(dpiScale, workingArea);
        }

        private static TrayLayout CreateCompact(Rectangle workingArea)
        {
            // 1080p/100% 菜单需要更小字体、更细滑块和更紧凑高度，避免托盘附近显得过大。
            const float scale = 0.70F;
            var minWidth = scaleValue(292, scale);
            var preferredWidth = scaleValue(310, scale);
            var maxWidth = Math.Max(minWidth, workingArea.Width - scaleValue(24, scale));
            var popupWidth = clamp(preferredWidth, minWidth, maxWidth);
            var horizontalPadding = scaleValue(22, scale);
            var checkWidth = scaleValue(22, scale);
            var contentLeft = horizontalPadding + scaleValue(10, scale);
            var captionLeft = contentLeft;
            var alphaCaptionLeft = contentLeft + checkWidth + scaleValue(6, scale);
            var valueWidth = scaleValue(54, scale);
            var sliderWidth = Math.Max(scaleValue(150, scale), popupWidth - captionLeft - horizontalPadding);
            var captionY = scaleValue(14, scale);
            var captionHeight = scaleValue(32, scale);
            var alphaTrackY = scaleValue(52, scale);
            var systemCaptionY = scaleValue(94, scale);
            var systemTrackY = scaleValue(134, scale);
            var menuTop = scaleValue(174, scale);
            var menuHeight = scaleValue(49, scale);
            var checkHeight = scaleValue(TrayCheckBox.PreferredControlHeight, scale);
            var contentHeight = menuTop + menuHeight * 2 + scaleValue(8, scale);
            var popupHeight = Math.Min(contentHeight, Math.Max(contentHeight, workingArea.Height - scaleValue(24, scale)));
            var trackBarHeight = scaleValue(22, scale);
            var trackBarBounds = new Rectangle(captionLeft, 0, sliderWidth, trackBarHeight);
            var valueX = popupWidth - horizontalPadding - valueWidth;

            return new TrayLayout
            {
                PopupSize = new Size(popupWidth, popupHeight),
                PanelPadding = new Padding(horizontalPadding, scaleValue(10, scale), horizontalPadding, scaleValue(10, scale)),
                ShadeCheckBoxBounds = new Rectangle(contentLeft, captionY + scaleValue(2, scale) + (captionHeight - checkHeight) / 2, checkWidth, checkHeight),
                AlphaCaptionBounds = new Rectangle(alphaCaptionLeft, captionY, scaleValue(80, scale), captionHeight),
                AlphaValueBounds = new Rectangle(valueX, captionY, valueWidth, captionHeight),
                AlphaTrackBarBounds = new Rectangle(trackBarBounds.X, alphaTrackY, trackBarBounds.Width, trackBarBounds.Height),
                SystemCaptionBounds = new Rectangle(captionLeft, systemCaptionY, valueX - captionLeft, captionHeight),
                SystemValueBounds = new Rectangle(valueX, systemCaptionY, valueWidth, captionHeight),
                SystemTrackBarBounds = new Rectangle(trackBarBounds.X, systemTrackY, trackBarBounds.Width, trackBarBounds.Height),
                OpenMainBounds = new Rectangle(scaleValue(8, scale), menuTop, popupWidth - scaleValue(16, scale), menuHeight),
                CloseBounds = new Rectangle(scaleValue(8, scale), menuTop + menuHeight, popupWidth - scaleValue(16, scale), menuHeight),
                CaptionFontSize = 9F,
                ValueFontSize = 9F,
                MenuFontSize = 10F,
                MenuTextVerticalPadding = 8,
                MenuTextLeftPadding = contentLeft - scaleValue(8, scale),
                SliderTrackHeight = 3,
                SliderThumbRadius = 5
            };
        }

        private static TrayLayout CreateRegular(float dpiScale, Rectangle workingArea)
        {
            // 4K/150% 等高 DPI 菜单保持较宽松布局，优先保证字体和控件观感稳定。
            var scale = Math.Max(1F, dpiScale / (BaseDpi / 96F));
            var minWidth = scaleValue(300, scale);
            var preferredWidth = scaleValue(344, scale);
            var maxWidth = Math.Max(minWidth, workingArea.Width - scaleValue(24, scale));
            var popupWidth = clamp(preferredWidth, minWidth, maxWidth);
            var horizontalPadding = scaleValue(26, scale);
            var checkWidth = scaleValue(22, scale);
            var contentLeft = scaleValue(30, scale);
            var captionLeft = scaleValue(55, scale);
            var valueWidth = scaleValue(54, scale);
            var valueGap = scaleValue(10, scale);
            var sliderWidth = Math.Max(scaleValue(150, scale), popupWidth - horizontalPadding * 2);
            var captionY = scaleValue(12, scale);
            var captionHeight = scaleValue(30, scale);
            var alphaTrackY = scaleValue(58, scale);
            var systemCaptionY = scaleValue(102, scale);
            var systemTrackY = scaleValue(148, scale);
            var menuTop = scaleValue(188, scale);
            var menuHeight = scaleValue(55, scale);
            var checkHeight = scaleValue(TrayCheckBox.PreferredControlHeight, scale);
            var contentHeight = menuTop + menuHeight * 2 + scaleValue(12, scale);
            var popupHeight = Math.Min(scaleValue(310, scale), Math.Max(contentHeight, workingArea.Height - scaleValue(24, scale)));
            var trackBarHeight = scaleValue(22, scale);
            var trackBarWidth = sliderWidth;
            var valueX = popupWidth - horizontalPadding - valueWidth;

            return new TrayLayout
            {
                PopupSize = new Size(popupWidth, popupHeight),
                PanelPadding = new Padding(horizontalPadding, scaleValue(14, scale), horizontalPadding, scaleValue(14, scale)),
                ShadeCheckBoxBounds = new Rectangle(contentLeft, captionY + scaleValue(2, scale) + (captionHeight - checkHeight) / 2, checkWidth, checkHeight),
                AlphaCaptionBounds = new Rectangle(captionLeft, captionY, scaleValue(60, scale), captionHeight),
                AlphaValueBounds = new Rectangle(valueX, captionY, valueWidth, captionHeight),
                AlphaTrackBarBounds = new Rectangle(horizontalPadding, alphaTrackY, trackBarWidth, trackBarHeight),
                SystemCaptionBounds = new Rectangle(horizontalPadding, systemCaptionY, valueX - horizontalPadding, captionHeight),
                SystemValueBounds = new Rectangle(valueX, systemCaptionY, valueWidth, captionHeight),
                SystemTrackBarBounds = new Rectangle(horizontalPadding, systemTrackY, sliderWidth, trackBarHeight),
                OpenMainBounds = new Rectangle(scaleValue(8, scale), menuTop, popupWidth - scaleValue(16, scale), menuHeight),
                CloseBounds = new Rectangle(scaleValue(8, scale), menuTop + menuHeight, popupWidth - scaleValue(16, scale), menuHeight),
                CaptionFontSize = 11F,
                ValueFontSize = 11F,
                MenuFontSize = 13F,
                MenuTextVerticalPadding = 8,
                MenuTextLeftPadding = 22,
                SliderTrackHeight = 4,
                SliderThumbRadius = 7
            };
        }

        private static int clamp(int value, int minimum, int maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private static int scaleValue(int value, float scale)
        {
            return Math.Max(1, (int)Math.Round(value * scale));
        }
    }

    internal sealed class TrayBrightnessSlider : Control
    {
        private int _thumbRadius = 7;
        private int _trackHeight = 4;
        private bool _dragging;
        private bool _hovering;
        private int _minimum;
        private int _maximum = 100;
        private int _value;

        public event EventHandler ValueChanged;

        public int ThumbRadius
        {
            get { return this._thumbRadius; }
            set
            {
                this._thumbRadius = Math.Max(3, value);
                this.Invalidate();
            }
        }

        public int TrackHeight
        {
            get { return this._trackHeight; }
            set
            {
                this._trackHeight = Math.Max(2, value);
                this.Invalidate();
            }
        }

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
            var radius = this._trackHeight / 2;
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

            var currentThumbRadius = this._dragging || this._hovering ? this._thumbRadius + 1 : this._thumbRadius;
            var thumbBounds = new Rectangle(thumbX - currentThumbRadius, bounds.Top + bounds.Height / 2 - currentThumbRadius, currentThumbRadius * 2, currentThumbRadius * 2);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0)))
                e.Graphics.FillEllipse(shadowBrush, new Rectangle(thumbBounds.X, thumbBounds.Y + 1, thumbBounds.Width, thumbBounds.Height));
            using (var brush = new SolidBrush(thumbColor))
                e.Graphics.FillEllipse(brush, thumbBounds);
        }

        private Rectangle getTrackBounds()
        {
            var left = this._thumbRadius + 2;
            var width = Math.Max(1, this.Width - (this._thumbRadius + 2) * 2);
            return new Rectangle(left, (this.Height - this._trackHeight) / 2, width, this._trackHeight);
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
        private readonly TrayMenuPanel _contentPanel;
        private readonly Timer _autoHideTimer;
        private bool _hasMouseEntered;
        private DateTime _ignoreDeactivateUntil = DateTime.MinValue;

        public TrayMenuPanel ContentPanel => this._contentPanel;

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

            this.UpdateRoundedRegion();
            this.PerformLayout();
            this.Invalidate(true);
        }

        public TrayBrightnessPopup()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.White;
            this.Padding = new Padding(0);
            this.TopMost = true;
            this.AutoScaleMode = AutoScaleMode.None;

            this._autoHideTimer = new Timer
            {
                Interval = 120
            };
            this._autoHideTimer.Tick += this.autoHideTimer_Tick;

            this._contentPanel = new TrayMenuPanel
            {
                BackColor = Color.White,
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
                this.UpdateRoundedRegion();
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
            e.Graphics.Clear(Color.White);
            var bounds = new Rectangle(1, 1, this.Width - 3, this.Height - 3);
            using (var path = TrayMenuGeometry.CreateRoundRect(bounds, 12))
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.FillPath(brush, path);

            using (var path = TrayMenuGeometry.CreateRoundRect(bounds, 12))
            using (var pen = new Pen(Color.FromArgb(212, 212, 212)))
                e.Graphics.DrawPath(pen, path);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.UpdateRoundedRegion();
        }

        public void UpdateRoundedRegion()
        {
            if (this.Width <= 0 || this.Height <= 0)
                return;

            using (var path = TrayMenuGeometry.CreateRoundRect(new Rectangle(0, 0, this.Width, this.Height), 12))
                this.Region = new Region(path);
        }

        private void autoHideTimer_Tick(object sender, EventArgs e)
        {
            if (!this.Visible || this.isSliderDragging() || MouseButtons == MouseButtons.Left)
                return;

            // 初次打开时鼠标可能还没进入弹窗，只有进入后再移出才自动隐藏。
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

    internal sealed class TrayMenuPanel : Panel
    {
        private const int HoverVerticalPadding = 1;
        private readonly List<TrayMenuItem> _items = new List<TrayMenuItem>();
        private int _hoveredIndex = -1;

        public int TextVerticalPadding { get; set; } = 8;
        public int TextLeftPadding { get; set; } = 22;

        public TrayMenuPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.ContainerControl, true);
            this.Font = TrayNativeWindowHelper.CreateTrayMenuFont();
        }

        public void AddMenuItem(string text, Rectangle bounds, Action clickHandler)
        {
            this._items.Add(new TrayMenuItem(text, bounds, clickHandler));
            this.Invalidate(bounds);
        }

        public void SetMenuItemBounds(int index, Rectangle bounds)
        {
            if (index < 0 || index >= this._items.Count)
                return;

            this.Invalidate(this._items[index].Bounds);
            this._items[index].Bounds = bounds;
            this.Invalidate(bounds);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.setHoveredIndex(-1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var index = this.getItemIndex(e.Location);
            this.setHoveredIndex(index);
            this.Cursor = index >= 0 ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
                return;

            var index = this.getItemIndex(e.Location);
            if (index >= 0)
                this._items[index].ClickHandler();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Color.FromArgb(222, 222, 222)))
            {
                foreach (var item in this._items)
                    e.Graphics.DrawLine(pen, item.Bounds.Left, item.Bounds.Top, item.Bounds.Right, item.Bounds.Top);
            }

            for (var i = 0; i < this._items.Count; i++)
            {
                var item = this._items[i];
                if (i == this._hoveredIndex)
                {
                    // hover 背景与阴影由同一面板绘制，避免子控件透明背景造成首帧样式不一致。
                    var shadowBounds = new Rectangle(item.Bounds.Left + 4, item.Bounds.Top + HoverVerticalPadding + 1, item.Bounds.Width - 8, item.Bounds.Height - HoverVerticalPadding * 2);
                    var hoverBounds = new Rectangle(item.Bounds.Left + 3, item.Bounds.Top + HoverVerticalPadding, item.Bounds.Width - 6, item.Bounds.Height - HoverVerticalPadding * 2);
                    using (var path = TrayMenuGeometry.CreateRoundRect(shadowBounds, 5))
                    using (var brush = new SolidBrush(Color.FromArgb(22, 0, 0, 0)))
                        e.Graphics.FillPath(brush, path);
                    using (var path = TrayMenuGeometry.CreateRoundRect(hoverBounds, 5))
                    using (var brush = new SolidBrush(Color.FromArgb(244, 244, 244)))
                        e.Graphics.FillPath(brush, path);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    item.Text,
                    this.Font,
                    new Rectangle(item.Bounds.Left + this.TextLeftPadding, item.Bounds.Top + this.TextVerticalPadding, item.Bounds.Width - this.TextLeftPadding * 2, item.Bounds.Height - this.TextVerticalPadding * 2),
                    Color.FromArgb(32, 32, 32),
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
        }

        private int getItemIndex(Point point)
        {
            for (var i = 0; i < this._items.Count; i++)
            {
                if (this._items[i].Bounds.Contains(point))
                    return i;
            }

            return -1;
        }

        private void setHoveredIndex(int index)
        {
            if (this._hoveredIndex == index)
                return;

            this.invalidateItem(this._hoveredIndex);
            this._hoveredIndex = index;
            this.invalidateItem(this._hoveredIndex);
        }

        private void invalidateItem(int index)
        {
            if (index >= 0 && index < this._items.Count)
                this.Invalidate(this._items[index].Bounds);
        }

        private sealed class TrayMenuItem
        {
            public readonly string Text;
            public readonly Action ClickHandler;
            public Rectangle Bounds;

            public TrayMenuItem(string text, Rectangle bounds, Action clickHandler)
            {
                this.Text = text;
                this.Bounds = bounds;
                this.ClickHandler = clickHandler;
            }
        }
    }

    internal sealed class TrayCheckBox : Control
    {
        private const int BoxSize = 15;
        private bool _checked;
        public const int PreferredControlHeight = 30;

        public event EventHandler CheckedChanged;

        public bool Checked
        {
            get { return this._checked; }
            set
            {
                if (this._checked == value)
                    return;

                this._checked = value;
                this.Invalidate();
                this.CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public TrayCheckBox()
        {
            // 纯自绘控件可避免原生 CheckBox 首次创建句柄时闪现凹陷系统样式。
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.Selectable, true);
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
            var borderColor = this._checked ? Color.FromArgb(0, 120, 215) : Color.FromArgb(118, 118, 118);
            var fillColor = this._checked ? Color.FromArgb(0, 120, 215) : Color.White;

            using (var path = TrayMenuGeometry.CreateRoundRect(boxBounds, 3))
            using (var brush = new SolidBrush(fillColor))
                e.Graphics.FillPath(brush, path);

            using (var path = TrayMenuGeometry.CreateRoundRect(boxBounds, 3))
            using (var pen = new Pen(borderColor))
                e.Graphics.DrawPath(pen, path);

            if (!this._checked)
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

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Checked = !this.Checked;
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

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const int MDT_EFFECTIVE_DPI = 0;
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

        public static float GetDpiScale(Point point)
        {
            try
            {
                var monitor = MonitorFromPoint(new POINT(point), MONITOR_DEFAULTTONEAREST);
                uint dpiX;
                uint dpiY;
                if (monitor != IntPtr.Zero && GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out dpiX, out dpiY) == 0 && dpiX > 0)
                    return dpiX / 96F;
            }
            catch
            {
            }

            return 1F;
        }

        public static Font CreateTrayMenuFont()
        {
            return CreateTrayMenuFont(13F);
        }

        public static Font CreateTrayMenuFont(float size)
        {
            return new Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public static Font CreateTrayCaptionFont()
        {
            return CreateTrayCaptionFont(11F);
        }

        public static Font CreateTrayCaptionFont(float size)
        {
            return new Font("Segoe UI", size, FontStyle.Regular, GraphicsUnit.Pixel);
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
