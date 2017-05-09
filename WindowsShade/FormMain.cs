using System;
using System.Diagnostics;
using System.Windows.Forms;
using WindowsShade.Models;
using WindowsShade.Views;

namespace WindowsShade
{
    public partial class FormMain : Form
    {
        #region Members & Properties
        public ShadeTypes ShadeType { get; private set; }

        private FormShade _shade = new FormShade();
        #endregion

        #region Structures & Initialize
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.initialize();
        }
        private void initialize()
        {
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            this.rbtnShadeTypes_CheckedChanged(this, null);

            this.Activate();
        }
        #endregion

        #region Methods
        private void showShade()
        {
            this.Visible = false;
            this._shade.WindowState = FormWindowState.Normal;
            this._shade.Show(this.ShadeType);
            this.menuItemHidden.Text = "隐藏(&H)";
        }
        private void hiddenShade()
        {
            this._shade.WindowState = FormWindowState.Minimized;
            this.menuItemHidden.Text = "显示(&D)";
        }
        #endregion

        #region Events - FormMain
        private void btnStart_Click(object sender, EventArgs e) => this.showShade();

        private void btnExit_Click(object sender, EventArgs e) => this.Close();

        private void btnHelp_Click(object sender, EventArgs e) => Process.Start("http://enjoycodes.com/Home/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");

        private void rbtnShadeTypes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbtnD1.Checked) this.ShadeType = ShadeTypes.D1024L;
            else if (this.rbtnD2.Checked) this.ShadeType = ShadeTypes.D1920R;
        }

        #endregion

        #region Events - cmxTray
        private void menuItemOpenMain_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItemHidden_Click(object sender, EventArgs e)
        {
            switch (this.menuItemHidden.Text)
            {
            case "隐藏(&H)": this.hiddenShade(); break;
            case "显示(&D)": this.showShade(); break;
            }
        }
        #endregion
    }
}