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
            this._shade.Text = this.Text;

            Common.Config = Config.Load(Common.ConfigPath);
            if (Common.Config == null) Common.Config = Common.DefaultConfig.Clone() as Config;
            else this.showShade(); // 读取到配置文件，则直接调整屏幕亮度
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();
            this.selectShadeType(Common.Config.ShadeType);

            this.Activate();
        }
        #endregion

        #region Methods
        private void showShade()
        {
            this.Visible = false;
            this._shade.WindowState = FormWindowState.Normal;
            this._shade.Show(Common.Config.ShadeType);
            this.menuItemHidden.Text = "隐藏(&H)";

            Common.Config.Save();
        }
        private void hiddenShade()
        {
            this._shade.WindowState = FormWindowState.Minimized;
            this.menuItemHidden.Text = "显示(&D)";
        }

        private void selectShadeType(ShadeTypes type)
        {
            switch (type)
            {
            case ShadeTypes.D1024L: this.rbtnD1.Checked = true; break;
            case ShadeTypes.D1920R: this.rbtnD2.Checked = true; break;
            }
        }
        #endregion

        #region Events - FormMain
        private void btnStart_Click(object sender, EventArgs e) => this.showShade();

        private void btnMinimize_Click(object sender, EventArgs e) => this.Visible = false;

        private void btnHelp_Click(object sender, EventArgs e) => Process.Start("http://enjoycodes.com/Home/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");

        private void rbtnShadeTypes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbtnD1.Checked) Common.Config.ShadeType = ShadeTypes.D1024L;
            else if (this.rbtnD2.Checked) Common.Config.ShadeType = ShadeTypes.D1920R;
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