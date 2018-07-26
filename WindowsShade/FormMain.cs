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
        private ScreenBrightness _screenBrightness = new ScreenBrightness();
        private int _tbSystemToScreenBrightness
        {
            get { return this.tbSystem.Maximum - this.tbSystem.Value; }
            set { this.tbSystem.Value = this.tbSystem.Maximum - value; }
        }
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
            this.btnApply.Location = new System.Drawing.Point(-100, -100);
            this.tbAlpha.Enabled = false;

            // 初始化系统亮度控件
            this.tbSystem.Maximum = this._screenBrightness.Maximum;
            //this.tbSystem.Update();
            //this.tbSystem.Refresh();
            this._tbSystemToScreenBrightness = this._screenBrightness.GetBrightness();
            this.lblSystem.Text = this.tbSystem.Value.ToString();

            // 2.加载配置文件
            Common.Config = Config.Load(Common.ConfigPath);
            if (Common.Config == null) Common.Config = new Config();
            else
            {
                // 2.1 读取到配置文件，则直接调整屏幕亮度
                this.cbxShadeTypes.Text = Common.Config.ShadeType.ToString();
                this.changeCbxAlpha(true);
                //this.showShade(Common.Config.Alpha);

                // 2.2 透明度
                this.tbAlpha.Value = Common.Config.Alpha;
                this.lblAlphaValue.Text = Common.Config.Alpha.ToString();
            }
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();

            // 3.激活主窗体
            //this.Activate();
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
        private void showShade(byte alpha = 128, bool autoHiddenFormMain = true)
        {
            Common.Config.ShadeType = (ShadeTypes)Enum.Parse(typeof(ShadeTypes), this.cbxShadeTypes.Text);
            this.Visible = !autoHiddenFormMain;
            this._shade.Alpha = alpha;
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
        private void btnApply_Click(object sender, EventArgs e) => this.showShade(Common.Config.Alpha);

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

        /// <summary>
        /// 调整遮罩亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAlpha_Scroll(object sender, EventArgs e)
        {
            Common.Config.Alpha = (byte)this.tbAlpha.Value;
            this.lblAlphaValue.Text = this.tbAlpha.Value.ToString();

            this.showShade(Common.Config.Alpha, false);
        }

        /// <summary>
        /// 调整系统亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSystem_Scroll(object sender, EventArgs e)
        {
            this._screenBrightness.SetBrightness(this._tbSystemToScreenBrightness);
            this.lblSystem.Text = this.tbSystem.Value.ToString();
        }

        /// <summary>
        /// 是否显示遮罩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxAlpha_CheckedChanged(object sender, EventArgs e) => this.changeCbxAlpha(this.cbxAlpha.Checked, false);
        private void changeCbxAlpha(bool isChecked, bool autoHiddenFormMain = true)
        {
            if (this.tbAlpha.Enabled == isChecked) return;

            this.tbAlpha.Enabled = isChecked; // 是否可以调整遮罩alpha
            this.cbxAlpha.Checked = isChecked;

            if (isChecked)
                this.showShade(Common.Config.Alpha, autoHiddenFormMain);
            else
                this.hiddenShade();
        }
        #endregion

        #region Events - cmxTray
        private void menuItemShadeTypes_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            this.cbxShadeTypes.Text = item.Text;
            this.showShade(Common.Config.Alpha);
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
            var isChecked = this.menuItemHidden.Text == "显示(&D)";
            this.changeCbxAlpha(isChecked);
        }
        #endregion

        #region Events - Nofify
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if ((e as MouseEventArgs).Button == MouseButtons.Left)
            {
                this.Visible = !this.Visible;
                if (this.Visible) this.Activate();
            }
        }
        #endregion
    }
}
