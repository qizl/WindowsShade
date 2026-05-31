using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private List<FormShade> _shades = new List<FormShade>();
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

        /// <summary>
        /// 遮罩置顶检测计时器
        /// </summary>
        private Timer _timerSetTopMost = new Timer();
        private string _applicationTitle => $"多屏亮度调整工具 v{Application.ProductVersion}";
        #endregion

        #region Structures & Initialize
        public FormMain() => InitializeComponent();

        private void FormMain_Load(object sender, EventArgs e) => this.initialize();
        private void initialize()
        {
            // 1.加载配置文件
            Common.Config = Config.Load(Common.ConfigPath);
            if (Common.Config == null)
                Common.Config = new Config(true);
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();

            /*
             * 2.初始化控件
             */
            // 2.1 主窗体控件
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            this.btnHidden.Location = new System.Drawing.Point(0, -100);
            this._timerSetTopMost.Interval = 1000;
            this._timerSetTopMost.Tick += _timerSetTopMost_Tick;

            // 2.2 遮罩窗体
            //this._shade.Text = this.Text;

            // 2.3 tabMain
            this.tabMain_SelectedIndexChanged(this, null);

            // 2.4 tabMain - 亮度控制
            this.tbAlpha.Value = Common.Config.Alpha;
            this.lblAlphaValue.Text = Common.Config.Alpha.ToString();
            this.tbSystem.Enabled = this._screenBrightness.Initiazlie();
            if (this.tbSystem.Enabled)
            {
                this.tbSystem.Maximum = this._screenBrightness.Maximum;
                this._tbSystemToScreenBrightness = this._screenBrightness.GetBrightness();  // 读取系统亮度
                this.lblSystem.Text = this.tbSystem.Value.ToString();
            }

            // 2.5 tabMain - 遮罩设置
            this.updateShades();
            this.txtResolution.BorderStyle = BorderStyle.None;

            // 2.6 tabMain - 软件设置
            this.ckxAutoHidden.Checked = Common.Config.AutoHidden;
            this.ckxAutoShowShade.Checked = Common.Config.AutoShowShade;

            // 3.主窗体显示控制
            if (Common.Config.AutoHidden) // 隐藏主窗体
                this.Visible = false;
            else // 不自动隐藏主窗体时，激活主窗体
                this.Activate();

            // 4.注册显示器变更事件
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 设置遮罩
        /// </summary>
        private void updateShades()
        {
            this.listView1.Items.Clear();

            if (this._shades.Any())
            {
                foreach (var shade in this._shades)
                    shade.Close();
            }
            this._shades.Clear();

            var monitorCount = Math.Max(Common.Config.Monitors.Count, Screen.AllScreens.Length); // 读取配置文件和系统屏幕中屏幕数量多的，作为显示器控件数量添加到listview中
            var screens = Screen.AllScreens.OrderBy(m => m.Bounds.X).ToArray(); // 按屏幕X轴位置排序
            for (int i = 0; i < monitorCount; i++)
            {
                // 2.5.1 添加控件
                this.listView1.Items.Add(new ListViewItem() { ImageIndex = (i < Common.Config.Monitors.Count && Common.Config.Monitors[i].Enabled) || Common.Config.IsFirtConfig ? 0 : 1 });

                // 2.5.2 添加屏幕配置
                if (monitorCount > Common.Config.Monitors.Count)
                    Common.Config.Monitors.Add(new Monitor()); // 将缺少的屏幕添加到配置文件

                // 2.5.3 读取屏幕配置
                var m = Common.Config.Monitors[i];
                if (i < screens.Length)
                {
                    m.Primary = screens[i].Primary; // 设置主显
                    m.Width = screens[i].Bounds.Width;
                    m.Height = screens[i].Bounds.Height;
                    m.X = screens[i].Bounds.X;
                    m.Y = screens[i].Bounds.Y;
                }
                else
                {
                    if (m.Width == 0)
                    {
                        m.Width = Screen.PrimaryScreen.Bounds.Width;
                        m.Height = Screen.PrimaryScreen.Bounds.Height;
                    }
                }
                if (Common.Config.IsFirtConfig)
                    m.Enabled = true; // 第一次配置，启用所有屏幕

                // 2.5.4 添加遮罩窗体
                if (i < Screen.AllScreens.Length)
                    this._shades.Add(new FormShade() { Text = this.Text });
            }
            this.listView1.Items[0].Selected = true;

            this.setBrightness(); // 调整遮罩亮度

            this.showOrHiddenShade(Common.Config.Monitors.Any(m => m.Enabled) || Common.Config.AutoShowShade); // 显示遮罩
        }

        /// <summary>
        /// 设置遮罩亮度
        /// </summary>
        private void setBrightness()
        {
            for (int i = 0; i < this._shades.Count; i++)
                this._shades[i].AdjustBrightness(Common.Config.Monitors[i].Alpha);
            this.ensureShadesTopMost();
        }

        /// <summary>
        /// 保持可见遮罩位于顶层，但不抢占当前前台窗口焦点
        /// </summary>
        private void ensureShadesTopMost()
        {
            var hasVisibleShade = false;
            for (int i = 0; i < this._shades.Count; i++)
            {
                if (!this._shades[i].Visible)
                    continue;

                hasVisibleShade = true;
                this._shades[i].SetTopMost();
            }

            this._timerSetTopMost.Enabled = hasVisibleShade;
        }

        /// <summary>
        /// 显示遮罩
        /// </summary>
        /// <param name="showOrHidden">是否显示遮罩</param>
        private void showOrHiddenShade(bool showOrHidden = true)
        {
            // 同步复选框状态
            if (showOrHidden)
            {
                if (!this.ckxAlpha.Checked)
                {
                    this.ckxAlpha.Checked = true;
                    return;
                }
            }
            else
            {
                if (this.ckxAlpha.Checked)
                {
                    this.ckxAlpha.Checked = false;
                    return;
                }
            }

            if (showOrHidden == true)
            {
                /*
                 * 显示遮罩
                 */

                // 如果没有启用任何屏幕，则启用所有屏幕；否则按配置启用屏幕
                if (!Common.Config.Monitors.Any(m => m.Enabled))
                    Common.Config.Monitors.ForEach(m => m.Enabled = true);

                for (int i = 0; i < this._shades.Count; i++)
                    this._shades[i].AdjustShade(Common.Config.Monitors[i]);
                this.ensureShadesTopMost();
            }
            else
            {
                /*
                 * 隐藏遮罩
                 */

                // 禁用所有屏幕
                Common.Config.Monitors.ForEach(m => m.Enabled = false);
                for (int i = 0; i < this._shades.Count; i++)
                    this._shades[i].Visible = false;
                this._timerSetTopMost.Stop();
            }

            this.menuItemHidden.Text = showOrHidden ? "隐藏(&H)" : "显示(&D)"; // 托盘菜单

        }
        #endregion

        #region Events - FormMain
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            this.updateShades();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e) => this.apply();
        private void apply()
        {
            // 1.获取亮度控制参数
            Common.Config.Alpha = (byte)this.tbAlpha.Value;

            // 2.获取多屏设置参数：自动更新

            // 3.获取软件设置参数
            Common.Config.AutoHidden = this.ckxAutoHidden.Checked;
            Common.Config.AutoShowShade = this.ckxAutoShowShade.Checked;

            // 4.持久化配置
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();

            /*
             * 5.调整屏幕亮度
             */
            if (this.tabMain.SelectedIndex == 1)
                this.showOrHiddenShade(Common.Config.Monitors.Any(m => m.Enabled));

        }

        /// <summary>
        /// 隐藏主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHidden_Click(object sender, EventArgs e) => this.Visible = false;

        private void FormMain_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e) => Process.Start("http://enjoycodes.com/ViewNote/dc7e3d7e-c462-465e-b20e-e4726beafb81");

        private void _timerSetTopMost_Tick(object sender, EventArgs e)
        {
            this.ensureShadesTopMost();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 1.取消屏幕关闭
            if (!this._isClosed) e.Cancel = true;

            // 2.应用
            this.apply();

            // 3.隐藏主窗体
            this.Visible = false;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            this._timerSetTopMost.Stop(); // 停止遮罩置顶
        }
        #endregion

        #region Events - tabMain
        /// <summary>
        /// tab 切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = this._applicationTitle;

            switch (this.tabMain.SelectedIndex)
            {
                case 0: break;
                case 1:
                    {
                        if (this.listView1.SelectedIndices.Count == 0)
                        {
                            // 未选中屏幕，禁用控件
                            this.lblMonitorInfo.Text = "未选中显示器。";
                            this.plScreenSettings.Enabled = false;
                        }
                        else
                        {
                            // 选中屏幕，加载屏幕亮度
                            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

                            var monitor = Common.Config.Monitors[index];
                            this.tbAlphaChild.Value = monitor.Alpha;
                            this.lblAlphaChildValue.Text = monitor.Alpha.ToString();
                        }
                    }
                    break;
                case 2: break;
            }
        }

        #region tab1 亮度控制
        /// <summary>
        /// 调整遮罩亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAlpha_Scroll(object sender, EventArgs e) => this.changeTbAlpha(this.tbAlpha.Value);
        private void changeTbAlpha(int value)
        {
            if (this.tbAlpha.Value != value)
                this.tbAlpha.Value = value;
            this.lblAlphaValue.Text = value.ToString();

            var delta = value - Common.Config.Alpha;
            Common.Config.Monitors.ForEach(m =>
            {
                var v = m.Alpha + delta;
                if (v >= 0 && v <= 255) m.Alpha = (byte)v;
            });

            Common.Config.Alpha = (byte)value;
            this.setBrightness();
        }

        /// <summary>
        /// 调整系统亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSystem_Scroll(object sender, EventArgs e)
        {
            this.lblSystem.Text = this.tbSystem.Value.ToString();

            this._screenBrightness.SetBrightness(this._tbSystemToScreenBrightness);
        }

        /// <summary>
        /// 是否显示遮罩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckxAlpha_CheckedChanged(object sender, EventArgs e) => this.showOrHiddenShade(this.ckxAlpha.Checked);
        #endregion

        #region tab2 屏幕设置
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 0)
            {
                this.lblMonitorInfo.Text = "未选中显示器。";
                this.plScreenSettings.Enabled = false;
                return;
            }
            this.plScreenSettings.Enabled = true;

            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            this.plScreenSettings.Enabled = index < Screen.AllScreens.Length; // 当从配置文件中读取到的屏幕数在系统屏幕数范围内，允许操作

            this.lblMonitorInfo.Text = $"当前配置第{index + 1}屏，\r\n共启用{Math.Min(Common.Config.Monitors.Count(m => m.Enabled), Screen.AllScreens.Length)}屏";

            var monitor = Common.Config.Monitors[index];
            this.ckxEnabled.Checked = monitor.Enabled;
            this.ckxIsMainScreen.Checked = monitor.Primary;
            this.tbAlphaChild.Value = monitor.Alpha;
            this.lblAlphaChildValue.Text = monitor.Alpha.ToString();
            this.txtResolution.Text = $"{monitor.Width}x{monitor.Height}";
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.plScreenSettings.Enabled)
                this.ckxEnabled.Checked = !this.ckxEnabled.Checked;
        }

        private void ckxEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 0) return;

            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            // 1.保存屏幕配置
            Common.Config.Monitors[index].Enabled = this.ckxEnabled.Checked;
            this.listView1.SelectedItems[0].ImageIndex = this.ckxEnabled.Checked ? 0 : 1; // 启用显示器时，图标彩色，否则灰色

            // 2.更新屏幕配置信息
            this.lblMonitorInfo.Text = $"当前配置第{index + 1}屏，\r\n共启用{Common.Config.Monitors.Count(m => m.Enabled)}屏";

            // 3.应用
            this.apply();
        }

        private void tbAlphaChild_Scroll(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 0) return;

            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            // 1.保存屏幕配置
            Common.Config.Monitors[index].Alpha = (byte)this.tbAlphaChild.Value;
            Common.Config.Monitors[index].Enabled = true;
            this.ckxEnabled.Checked = true;
            this.listView1.SelectedItems[0].ImageIndex = 0; // 启用显示器时，图标彩色，否则灰色
            this.lblAlphaChildValue.Text = this.tbAlphaChild.Value.ToString();

            // 2.调整屏幕亮度
            this._shades[index].AdjustBrightness(Common.Config.Monitors[index].Alpha);
            this.ensureShadesTopMost();

            // 3.更新屏幕配置信息
            this.lblMonitorInfo.Text = $"当前配置第{index + 1}屏，\r\n共启用{Common.Config.Monitors.Count(m => m.Enabled)}屏";

            // 4.应用
            this.apply();
        }
        #endregion

        #endregion

        #region Events - 托盘菜单（cmxTray）
        private void menuItemHidden_Click(object sender, EventArgs e)
        {
            var isChecked = this.menuItemHidden.Text == "显示(&D)";
            this.ckxAlpha.Checked = isChecked;
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
