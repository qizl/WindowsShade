using System;
using System.Diagnostics;
using System.Linq;
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
            // 1.加载配置文件
            Common.Config = Config.Load(Common.ConfigPath);
            if (Common.Config == null)
                Common.Config = new Config();
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();

            /*
             * 2.初始化控件
             */
            // 2.1 主窗体控件
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            this.btnHidden.Location = new System.Drawing.Point(0, -100);

            // 2.2 遮罩窗体
            this._shade.Text = this.Text;

            // 2.3 tabMain
            this.tabMain_SelectedIndexChanged(this, null);

            // 2.4 tabMain - 亮度调整
            this.tbAlpha.Enabled = false;
            this.tbAlpha.Value = Common.Config.Alpha;
            this.lblAlphaValue.Text = Common.Config.Alpha.ToString();
            this.tbSystem.Enabled = this._screenBrightness.Initiazlie();
            if (this.tbSystem.Enabled)
            {
                this.tbSystem.Maximum = this._screenBrightness.Maximum;
                this._tbSystemToScreenBrightness = this._screenBrightness.GetBrightness();
                this.lblSystem.Text = this.tbSystem.Value.ToString();
            }

            // 2.5 tabMain - 多屏设置
            this.cbxResolution.Items.Clear();
            foreach (var item in Common.Config.Resolutions)
                this.cbxResolution.Items.Add($"{item.X}x{item.Y}");
            this.listView1.MultiSelect = false;
            this.listView1.Items[0].Selected = true;

            // 2.6 tabMain - 软件设置
            this.ckxAutoHidden.Checked = Common.Config.AutoHidden;
            this.ckxAutoShowShade.Checked = Common.Config.AutoShowShade;

            // 3.托盘菜单
            this.menuItemHidden.Text = "显示(&D)";

            // 4.自动调整亮度
            if (Common.Config.AutoShowShade)
            {
                // 显示遮罩，调整亮度
                this.changeCkxAlpha(true, Common.Config.AutoHidden); // 调用遮罩选中事件，显示遮罩
                this.tbAlpha_Scroll(this, null); // 调用调整亮度事件，调整亮度
            }
            else
            {
                // 调整亮度
                this._shade.AdjustBrightness(Common.Config.Alpha);
            }
            this._shade.AdjustShade(Common.Config.Monitors); // 设置遮罩大小

            // 5.主窗体显示控制
            if (Common.Config.AutoHidden) // 隐藏主窗体
                this.Visible = false;
            else // 不自动隐藏主窗体时，激活主窗体
                this.Activate();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 显示遮罩
        /// </summary>
        /// <param name="hiddenFormMain">隐藏主窗体</param>
        private void showShade(bool hiddenFormMain = true)
        {
            this.Visible = !hiddenFormMain;

            this._shade.Visible = true;
            this.menuItemHidden.Text = "隐藏(&H)";
        }
        /// <summary>
        /// 隐藏遮罩
        /// </summary>
        private void hiddenShade()
        {
            this._shade.Visible = false;
            this.menuItemHidden.Text = "显示(&D)";
        }
        #endregion

        #region Events - FormMain
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            // 1.获取亮度调整参数
            Common.Config.Alpha = (byte)this.tbAlpha.Value;

            // 2.获取多屏设置参数

            // 3.获取软件设置参数
            Common.Config.AutoHidden = this.ckxAutoHidden.Checked;
            Common.Config.AutoShowShade = this.ckxAutoShowShade.Checked;

            // 4.持久化配置
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();

            // 5.调整屏幕亮度
            if (this.tabMain.SelectedIndex == 1)
            {
                // 5.1 调整遮罩
                this._shade.AdjustShade(Common.Config.Monitors);

                // 5.2 显示遮罩
                if (!this.ckxAlpha.Checked)
                    this.ckxAlpha.Checked = true;
                this.showShade(false);
            }
        }

        /// <summary>
        /// 隐藏主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHidden_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.btnApply_Click(sender, e);
        }

        private void FormMain_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e) => Process.Start("http://enjoycodes.com/Home/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._isClosed) e.Cancel = true;
            this.Visible = false;
        }
        #endregion

        #region Events - tabMain
        #region tab1 亮度调整
        /// <summary>
        /// 调整遮罩亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAlpha_Scroll(object sender, EventArgs e)
        {
            var alpha = (byte)this.tbAlpha.Value;

            this.lblAlphaValue.Text = alpha.ToString();

            Common.Config.Alpha = alpha;

            this._shade.AdjustBrightness(alpha);
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
        private void ckxAlpha_CheckedChanged(object sender, EventArgs e) => this.changeCkxAlpha(this.ckxAlpha.Checked, hiddenFormMain: false);
        private void changeCkxAlpha(bool isChecked, bool hiddenFormMain = true)
        {
            if (this.tbAlpha.Enabled == isChecked) return;

            this.tbAlpha.Enabled = isChecked; // 是否可以调整遮罩alpha
            this.ckxAlpha.Checked = isChecked;

            if (isChecked)
                this.showShade(hiddenFormMain);
            else
                this.hiddenShade();
        }
        #endregion

        #region tab2 多屏设置
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 0) return;

            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引
            var monitor = Common.Config.Monitors.Count > index ? Common.Config.Monitors[index] : new Monitor();

            this.lblMonitorInfo.Text = $"当前配置第{index + 1}屏，\r\n共启用{Common.Config.Monitors.Count(m => m.Enabled)}屏";
            this.ckxEnabled.Checked = monitor.Enabled;
            this.ckxIsMainScreen.Checked = monitor.Primary;
            this.ckxIsMainScreen.Enabled = monitor.Enabled;
            this.cbxResolution.Text = $"{monitor.Resolution.X}x{monitor.Resolution.Y}";
            this.cbxResolution.Enabled = monitor.Enabled;
        }

        private void ckxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            // 1.屏幕参数控件状态设置
            this.ckxIsMainScreen.Enabled = this.ckxEnabled.Checked;
            this.cbxResolution.Enabled = this.ckxEnabled.Checked;

            // 2.保存屏幕配置
            if (Common.Config.Monitors.Count <= index)
                for (int i = 0; i < index + 1 - Common.Config.Monitors.Count; i++)
                    Common.Config.Monitors.Add(new Monitor(0, 1920, 1080));
            Common.Config.Monitors[index].Enabled = this.ckxEnabled.Checked;
            if (!this.ckxEnabled.Checked) // 禁用当前屏幕时，设置当前屏幕不为主屏
                Common.Config.Monitors[index].Primary = false;

            // 3.更新屏幕配置信息
            this.lblMonitorInfo.Text = $"当前配置第{index + 1}屏，\r\n共启用{Common.Config.Monitors.Count(m => m.Enabled)}屏";
        }

        private void ckxIsMainScreen_CheckedChanged(object sender, EventArgs e)
        {
            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            if (this.ckxIsMainScreen.Checked)
                Common.Config.Monitors.ForEach(m => m.Primary = false);

            Common.Config.Monitors[index].Primary = this.ckxIsMainScreen.Checked;
        }

        private void cbxResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            var r = this.cbxResolution.Text.Split('x');
            Common.Config.Monitors[index].Resolution.X = Convert.ToInt32(r[0]);
            Common.Config.Monitors[index].Resolution.Y = Convert.ToInt32(r[1]);
        }
        #endregion

        #region tab3 软件设置
        #endregion

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = $"WindowsShade - {this.tabMain.SelectedTab.Text}";
        }
        #endregion

        #region Events - 托盘菜单（cmxTray）
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
            this.changeCkxAlpha(isChecked);
        }
        #endregion

        #region Events - 托盘图标（notifyIcon1）
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
