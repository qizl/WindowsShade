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
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32")]
        private static extern bool SetLayeredWindowAttributes(IntPtr handle, int crKey, byte bAlpha, int dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int LWA_ALPHA = 2;
        private const uint WDA_MONITOR = 0x00000001;
        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private byte _alpha = 128;
        #endregion

        #region Structures & Methods
        public FormShade()
        {
            InitializeComponent();

            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;

            this.Visible = false;
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.applyLayeredAlpha();
            this.applyCaptureExclusion();
        }

        /// <summary>
        /// Adjust shade bounds and visibility.
        /// </summary>
        /// <param name="m"></param>
        public void AdjustShade(Monitor m)
        {
            var bounds = new Rectangle(m.X, m.Y, m.Width, m.Height);
            if (this.Bounds != bounds)
                this.Bounds = bounds;

            if (this.Visible != m.Enabled)
                this.Visible = m.Enabled;
        }

        /// <summary>
        /// Adjust shade opacity.
        /// </summary>
        /// <param name="alpha"></param>
        public void AdjustBrightness(byte alpha)
        {
            if (this._alpha == alpha && this.IsHandleCreated)
                return;

            this._alpha = alpha;
            this.applyLayeredAlpha();
        }

        public void SetTopMost()
        {
            if (!this.IsHandleCreated)
                return;

            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        private void applyLayeredAlpha()
        {
            if (!this.IsHandleCreated)
                return;

            SetLayeredWindowAttributes(this.Handle, 0, this._alpha, LWA_ALPHA);
        }

        private void applyCaptureExclusion()
        {
            if (!this.IsHandleCreated)
                return;

            if (!SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE))
                SetWindowDisplayAffinity(this.Handle, WDA_MONITOR);
        }
        #endregion
    }
}
