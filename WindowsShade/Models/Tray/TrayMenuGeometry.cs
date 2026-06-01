using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsShade.Models
{
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
