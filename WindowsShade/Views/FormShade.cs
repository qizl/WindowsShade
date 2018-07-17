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

        public byte Alpha { get; set; }
        #endregion

        #region Structures & Methods
        public FormShade()
        {
            InitializeComponent();

            this.BackColor = Color.Black;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
        }

        public void Show(ShadeTypes t)
        {
            base.Show();
            switch (t)
            {
                case ShadeTypes.D1920R:
                    {
                        this.Location = new Point(-1920, 0);
                        this.Width = 1920 * 2;
                        this.Height = 1080;
                    }
                    break;
                case ShadeTypes.D1920L:
                    {
                        this.Location = new Point(0, 0);
                        this.Width = 1920 * 2;
                        this.Height = 1080;
                    }
                    break;
                case ShadeTypes.S1920:
                    {
                        this.Location = new Point(0, 0);
                        this.Width = 1920;
                        this.Height = 1080;
                    }
                    break;
                case ShadeTypes.D1440L:
                    {
                        this.Location = new Point(0, 0);
                        this.Width = 1440 * 2;
                        this.Height = 900;
                    }
                    break;
                case ShadeTypes.S1440:
                    {
                        this.Location = new Point(0, 0);
                        this.Width = 1440;
                        this.Height = 900;
                    }
                    break;
            }

            SetWindowLong(this.Handle, GWL_EXSTYLE, GetWindowLong(this.Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(this.Handle, 0, this.Alpha, LWA_ALPHA);
        }
        #endregion

        #region Events - Form
        private void FormShade_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion
    }
}
