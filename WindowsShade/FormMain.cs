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
        /// <summary>
        /// 是否允许关闭主程序
        /// </summary>
        private bool _isClosed = false;
        #endregion

        #region Structures & Initialize
        public FormMain() => InitializeComponent();

        private void FormMain_Load(object sender, EventArgs e) => this.initialize();
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
            this._shade.Visible = true;
            this._shade.Show(Common.Config.ShadeType);
            this.menuItemHidden.Text = "隐藏(&H)";
        }
        private void hiddenShade()
        {
            this._shade.Visible = false;
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

        #region Events - Form
        private void btnStart_Click(object sender, EventArgs e) => this.showShade();

        private void btnMinimize_Click(object sender, EventArgs e) => this.Visible = false;

        private void btnHelp_Click(object sender, EventArgs e) => Process.Start("http://enjoycodes.com/Home/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");

        private void rbtnShadeTypes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbtnD1.Checked) Common.Config.ShadeType = ShadeTypes.D1024L;
            else if (this.rbtnD2.Checked) Common.Config.ShadeType = ShadeTypes.D1920R;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._isClosed) e.Cancel = true;
            this.Visible = false;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Common.Config.Save();
        }
        #endregion

        #region Events - cmxTray
        private void menuItemOpenMain_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this._isClosed = true;
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