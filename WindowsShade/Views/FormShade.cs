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

        public void Show(ShadeTypes t)
        {
            int x = 0, y = 0, width = 0, height = 0;
            switch (t)
            {
                case ShadeTypes.D1920R:
                    {
                        x = -1920;
                        y = 0;
                        width = 1920 * 2;
                        height = 1080;
                    }
                    break;
                case ShadeTypes.D1920L:
                    {
                        x = 0;
                        y = 0;
                        width = 1920 * 2;
                        height = 1080;
                    }
                    break;
                case ShadeTypes.S1920:
                    {
                        x = 0;
                        y = 0;
                        width = 1920;
                        height = 1080;
                    }
                    break;
                case ShadeTypes.D1440L:
                    {
                        x = 0;
                        y = 0;
                        width = 1440 * 2;
                        height = 900;
                    }
                    break;
                case ShadeTypes.S1440:
                    {
                        x = 0;
                        y = 0;
                        width = 1440;
                        height = 900;
                    }
                    break;
            }
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
        public void Brightness(byte alpha)
        {
            SetWindowLong(this.Handle, GWL_EXSTYLE, GetWindowLong(this.Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(this.Handle, 0, alpha, LWA_ALPHA);
        }
        #endregion
    }
}