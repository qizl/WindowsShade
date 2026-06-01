using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsShade.Models
{
    internal sealed class TrayBrightnessMenuView : IDisposable
    {
        private readonly Action _openMainClick;
        private readonly Action _closeClick;
        private readonly Action<bool> _shadeEnabledChanged;
        private readonly Action<int> _alphaBrightnessChanged;
        private readonly Action<int> _systemBrightnessChanged;
        private TrayBrightnessPopup _popup;
        private TrayCheckBox _shadeEnabledCheckBox;
        private Label _alphaCaption;
        private TrayBrightnessSlider _alphaTrackBar;
        private Label _alphaValue;
        private Label _systemCaption;
        private TrayBrightnessSlider _systemTrackBar;
        private Label _systemValue;
        private bool _isRefreshing;

        public TrayBrightnessMenuView(
            Action openMainClick,
            Action closeClick,
            Action<bool> shadeEnabledChanged,
            Action<int> alphaBrightnessChanged,
            Action<int> systemBrightnessChanged)
        {
            this._openMainClick = openMainClick;
            this._closeClick = closeClick;
            this._shadeEnabledChanged = shadeEnabledChanged;
            this._alphaBrightnessChanged = alphaBrightnessChanged;
            this._systemBrightnessChanged = systemBrightnessChanged;
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

                this._alphaCaption = this.createCaptionLabel("亮度", ContentAlignment.MiddleLeft);
                this._alphaCaption.Cursor = Cursors.Hand;
                this._alphaCaption.Click += (sender, e) =>
                {
                    this._popup.KeepVisibleTemporarily();
                    this._shadeEnabledCheckBox.Checked = !this._shadeEnabledCheckBox.Checked;
                };
                panel.Controls.Add(this._alphaCaption);
                this._alphaValue = this.createCaptionValueLabel();
                this._alphaTrackBar = this.createTrackBar(255);
                panel.Controls.Add(this._alphaTrackBar);
                panel.Controls.Add(this._alphaValue);

                this._systemCaption = this.createCaptionLabel("系统亮度", ContentAlignment.MiddleLeft);
                panel.Controls.Add(this._systemCaption);
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
                    this._shadeEnabledChanged(this._shadeEnabledCheckBox.Checked);
                };

                this._alphaTrackBar.ValueChanged += (sender, e) =>
                {
                    if (this._isRefreshing)
                        return;

                    this._alphaValue.Text = this._alphaTrackBar.Value.ToString();
                    this._alphaBrightnessChanged(this._alphaTrackBar.Value);
                };

                this._systemTrackBar.ValueChanged += (sender, e) =>
                {
                    if (this._isRefreshing || !this._systemTrackBar.Enabled)
                        return;

                    this._systemBrightnessChanged(this._systemTrackBar.Value);
                };

                this._popup = popup;
                this.applyLayout(1F, Screen.PrimaryScreen.WorkingArea);
                this._shadeEnabledCheckBox.PrepareForDisplay();
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
            this._shadeEnabledCheckBox.PrepareForDisplay();

            this._alphaCaption.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.CaptionFontSize);
            this._alphaCaption.Location = layout.AlphaCaptionBounds.Location;
            this._alphaCaption.Size = layout.AlphaCaptionBounds.Size;

            this._alphaValue.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.ValueFontSize);
            this._alphaValue.Location = layout.AlphaValueBounds.Location;
            this._alphaValue.Size = layout.AlphaValueBounds.Size;

            this._alphaTrackBar.Location = layout.AlphaTrackBarBounds.Location;
            this._alphaTrackBar.Size = layout.AlphaTrackBarBounds.Size;
            this._alphaTrackBar.TrackHeight = layout.SliderTrackHeight;
            this._alphaTrackBar.ThumbRadius = layout.SliderThumbRadius;

            this._systemCaption.Font = TrayNativeWindowHelper.CreateTrayCaptionFont(layout.CaptionFontSize);
            this._systemCaption.Location = layout.SystemCaptionBounds.Location;
            this._systemCaption.Size = layout.SystemCaptionBounds.Size;

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
}
