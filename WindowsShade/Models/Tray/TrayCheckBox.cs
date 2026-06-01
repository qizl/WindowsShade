using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsShade.Models
{
    internal sealed class TrayCheckBox : Control
    {
        private const int BoxSize = 15;
        private bool _checked;
        public const int PreferredControlWidth = BoxSize + 3;
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
            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;
            this.TabStop = false;
        }

        public void PrepareForDisplay()
        {
            if (!this.IsHandleCreated)
                this.CreateControl();

            this.Invalidate();
            this.Update();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(brush, this.ClientRectangle);

            var boxBounds = new Rectangle(1, (this.Height - BoxSize) / 2, BoxSize, BoxSize);
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
}
