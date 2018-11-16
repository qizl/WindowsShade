using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsShade.Models;

namespace WindowsShade.Views
{
    public partial class FormShade : Form
    {
        #region Members
        //[DllImport("user32.dll")]
        //public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

        [DllImport("user32")]
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
        public void AdjustShade(Monitor m)
        {
            var border = 7;
            var title = 40;
            this.Location = new Point(m.X - border, m.Y - title);
            this.Width = m.Width + border * 2;
            this.Height = m.Height + title;

            this.Visible = m.Enabled;
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