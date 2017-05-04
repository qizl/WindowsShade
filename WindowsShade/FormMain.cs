using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsShade
{
    public partial class FormMain : Form
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
        #endregion

        #region Structures & Initialize
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.initialize();

            //this.start();
        }
        private void initialize()
        {
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }
        #endregion

        #region Events
        private void btnStart_Click(object sender, EventArgs e)
        {
            this.start();
        }
        private void start()
        {
            this.plContent.Hide();

            this.BackColor = Color.Black;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
            if (this.rbtnD1.Checked)
                this.Location = new Point(0, 0);
            else if (this.rbtnD2.Checked)
                this.Location = new Point(-1920, 0);
            this.Width = 1920 * 2;
            this.Height = 1080;
            SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(Handle, 0, 128, LWA_ALPHA);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("http://enjoycodes.com/Home/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");
        }

        #region Methods cmxTray
        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #endregion
    }
}