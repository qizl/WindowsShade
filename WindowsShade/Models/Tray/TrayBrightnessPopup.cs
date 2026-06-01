using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsShade.Models
{
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
}
