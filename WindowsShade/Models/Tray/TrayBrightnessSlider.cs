using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsShade.Models
{
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
}
