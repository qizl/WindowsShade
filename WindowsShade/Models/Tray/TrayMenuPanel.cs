using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsShade.Models
{
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
}
