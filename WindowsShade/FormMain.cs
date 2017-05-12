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
            // 1.初始化控件
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            this.initShadeTypes();
            this._shade.Text = this.Text;

            // 2.加载配置文件
            Common.Config = Config.Load(Common.ConfigPath);
            if (Common.Config == null) Common.Config = Common.DefaultConfig.Clone() as Config;
            else
            {
                // 读取到配置文件，则直接调整屏幕亮度
                this.cbxShadeTypes.Text = Common.Config.ShadeType.ToString();
                this.showShade();
            }
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();

            // 3.激活主窗体
            this.Activate();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 初始化遮罩类型相关控件
        /// </summary>
        private void initShadeTypes()
        {
            var shadeTypes = Enum.GetNames(typeof(ShadeTypes));

            // 1.下拉框
            this.cbxShadeTypes.Items.AddRange(shadeTypes);
            this.cbxShadeTypes.SelectedIndex = 0;

            // 2.托盘菜单
            foreach (var item in shadeTypes)
            {
                var t = new ToolStripMenuItem();
                t.Text = item;
                t.Click += this.menuItemShadeTypes_Click;
                this.cmxTray.Items.Insert(0, t);
            }
        }
        private void showShade()
        {
            Common.Config.ShadeType = (ShadeTypes)Enum.Parse(typeof(ShadeTypes), this.cbxShadeTypes.Text);
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
        #endregion

        #region Events - Form
        private void btnStart_Click(object sender, EventArgs e) => this.showShade();

        private void btnMinimize_Click(object sender, EventArgs e) => this.Visible = false;

        private void btnHelp_Click(object sender, EventArgs e) => Process.Start("http://enjoycodes.com/Home/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");

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
        private void menuItemShadeTypes_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            this.cbxShadeTypes.Text = item.Text;
            this.showShade();
        }

        private void menuItemOpenMain_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Activate();
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