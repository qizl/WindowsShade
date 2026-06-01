using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsShade.Models
{
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
}
