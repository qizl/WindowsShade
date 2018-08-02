using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsShade.Models;

namespace WindowsShade.Views
{
    public partial class FormShade : Form
    {
        #region Members
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(IntPtr Handle, int crKey, byte bAlpha, int dwFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x20;
        const int WS_EX_LAYERED = 0x80000;
        const int LWA_ALPHA = 2;

        const int HWND_TOP = 0;
        const int HWND_BOTTOM = 1;
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;
        #endregion

        #region Structures & Methods
        public FormShade()
        {
            InitializeComponent();

            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ControlBox = false;
            this.TopMost = true;
            this.ShowInTaskbar = false;
        }
        
        /// <summary>
        /// 调整遮罩
        /// </summary>
        /// <param name="monitors"></param>
        public void AdjustShade(List<Monitor> monitors)
        {
            monitors = monitors.Where(m => m.Enabled).ToList();
            if (!monitors.Any()) return;
            if (!monitors.Any(m => m.IsMain))
                monitors[0].IsMain = true;

            var mainMonitor = monitors.First(m => m.IsMain); // 主显
            var mainMonitorIndex = monitors.IndexOf(mainMonitor); // 主显位置
            var d = mainMonitor.Resolution.Y - monitors.Max(m => m.Resolution.Y); // 主显与最大高度显示器高度差

            int x = mainMonitorIndex == 0 ? 0 : monitors.Take(mainMonitorIndex).Sum(m => m.Resolution.X) * -1;
            int y = d >= 0 ? 0 : d; // 默认用户多显示器配置为底部对齐
            int width = monitors.Sum(m => m.Resolution.X);
            int height = monitors.Max(m => m.Resolution.Y);

            var border = 7;
            var title = 40;
            this.Location = new Point(x - border, y - title);
            this.Width = width + border * 2;
            this.Height = height + title;

            //SetWindowPos(this.Handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, 1 | 2);
        }

        /// <summary>
        /// 调整亮度
        /// </summary>
        /// <param name="alpha"></param>
        public void AdjustBrightness(byte alpha)
        {
            try
            {
                SetWindowLong(this.Handle, GWL_EXSTYLE, GetWindowLong(this.Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
                SetLayeredWindowAttributes(this.Handle, 0, alpha, LWA_ALPHA);
            }
            catch { }
        }
        #endregion
    }
}