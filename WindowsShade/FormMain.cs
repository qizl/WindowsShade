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
        private readonly List<FormShade> _shades = new List<FormShade>();
        private readonly MonitorConfiguration _monitorConfiguration = new MonitorConfiguration();
        private Screen[] _shadeScreens = Array.Empty<Screen>();
        private HashSet<Monitor> _enabledScreensBeforeHide;
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
        private readonly Timer _timerSetTopMost = new Timer();
        private string _applicationTitle => $"多屏亮度调整工具 v{Application.ProductVersion}";
        private TrayBrightnessMenu _trayBrightnessMenu;
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

            /*
             * 2.初始化控件
             */
            // 2.1 主窗体控件
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
            this.btnHidden.Location = new System.Drawing.Point(0, -100);
            this._timerSetTopMost.Interval = 1000;
            this._timerSetTopMost.Tick += _timerSetTopMost_Tick;
            this.initializeTrayBrightnessMenu();

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
            this.refreshTrayBrightnessPanel();

            // 2.5 tabMain - 遮罩设置
            this.updateShades();
            this.showOrHiddenShade(Common.Config.IsFirtConfig || this.hasEnabledMonitor() || Common.Config.AutoShowShade);
            this.txtResolution.BorderStyle = BorderStyle.None;

            // 2.6 tabMain - 软件设置
            this.ckxAutoHidden.Checked = Common.Config.AutoHidden;
            this.ckxAutoShowShade.Checked = Common.Config.AutoShowShade;
            this.ckxAutoStart.Checked = false;
            this.ckxAutoStart.Enabled = false;

            // 3.主窗体显示控制
            if (Common.Config.AutoHidden) // 隐藏主窗体
                this.Visible = false;
            else // 不自动隐藏主窗体时，激活主窗体
                this.Activate();

            // 4.注册显示器变更事件
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 设置遮罩
        /// </summary>
        private void updateShades()
        {
            var monitors = Common.Config.Monitors;
            var selectedIndex = this.listView1.SelectedIndices.Count > 0 ? this.listView1.SelectedIndices[0] : -1;
            var selectedMonitor = selectedIndex >= 0 && selectedIndex < monitors.Count ? monitors[selectedIndex] : null;
            var existingShades = new Dictionary<Monitor, FormShade>();
            for (int i = 0; i < this._shades.Count; i++)
                existingShades.Add(monitors[i], this._shades[i]);

            var screens = Screen.AllScreens.OrderBy(screen => screen.Bounds.X).ToArray();
            this._monitorConfiguration.Reconcile(monitors, screens.Select(screen => screen.DeviceName), Common.Config.Alpha);
            this._shadeScreens = screens;
            this._shades.Clear();

            this.listView1.BeginUpdate();
            try
            {
                this.listView1.Items.Clear();
                for (int i = 0; i < monitors.Count; i++)
                {
                    var monitor = monitors[i];
                    var connected = i < screens.Length;
                    this.listView1.Items.Add(new ListViewItem { ImageIndex = connected && monitor.Enabled ? 0 : 1 });
                    monitor.Primary = connected && screens[i].Primary;
                    if (!connected)
                    {
                        if (monitor.Width == 0 && Screen.PrimaryScreen != null)
                        {
                            monitor.Width = Screen.PrimaryScreen.Bounds.Width;
                            monitor.Height = Screen.PrimaryScreen.Bounds.Height;
                        }
                        continue;
                    }

                    var bounds = screens[i].Bounds;
                    monitor.Width = bounds.Width;
                    monitor.Height = bounds.Height;
                    monitor.X = bounds.X;
                    monitor.Y = bounds.Y;

                    // 继续使用仍连接的遮罩，避免分辨率或排列变化时重建窗口。
                    if (existingShades.TryGetValue(monitor, out var shade))
                        existingShades.Remove(monitor);
                    else
                        shade = new FormShade { Text = this.Text };
                    shade.AdjustBrightness(monitor.Alpha);
                    this._shades.Add(shade);
                }
                if (monitors.Count > 0)
                    this.listView1.Items[Math.Max(0, monitors.IndexOf(selectedMonitor))].Selected = true;
            }
            finally
            {
                this.listView1.EndUpdate();
            }

            foreach (var shade in existingShades.Values)
                shade.Dispose();
        }

        /// <summary>
        /// 设置遮罩亮度
        /// </summary>
        private void setBrightness()
        {
            for (int i = 0; i < this._shades.Count; i++)
                this._shades[i].AdjustBrightness(Common.Config.Monitors[i].Alpha);
            this.ensureShadesTopMost();
            this.refreshTrayBrightnessPanel();
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
        private void showOrHiddenShade(bool showOrHidden)
        {
            // 程序设置复选框时，由 CheckedChanged 统一执行切换。
            if (this.ckxAlpha.Checked != showOrHidden)
            {
                this.ckxAlpha.Checked = showOrHidden;
                return;
            }

            var monitors = Common.Config.Monitors;
            if (showOrHidden)
            {
                // 若用户在屏幕设置中直接启用了某一屏，采用当前的逐屏设置。
                if (!this.hasEnabledMonitor())
                {
                    var enableAll = this._enabledScreensBeforeHide == null || this._enabledScreensBeforeHide.Count == 0;
                    for (int i = 0; i < monitors.Count; i++)
                        monitors[i].Enabled = enableAll ? i < this._shadeScreens.Length : this._enabledScreensBeforeHide.Contains(monitors[i]);
                }
                this._enabledScreensBeforeHide = null;
            }
            else
            {
                // 显示器配置会随设备重排，保留配置对象可恢复同一台显示器。
                if (this._enabledScreensBeforeHide == null)
                    this._enabledScreensBeforeHide = new HashSet<Monitor>(monitors.Where(m => m.Enabled));
                monitors.ForEach(m => m.Enabled = false);
            }

            this.refreshShadeWindows();
            for (int i = 0; i < this.listView1.Items.Count; i++)
                this.listView1.Items[i].ImageIndex = i < this._shadeScreens.Length && monitors[i].Enabled ? 0 : 1;
            this.refreshSelectedMonitorEnabledState();

            this.menuItemHidden.Text = showOrHidden ? "隐藏(&H)" : "显示(&D)"; // 托盘菜单
            this.refreshTrayBrightnessPanel();
        }

        private bool hasEnabledMonitor() => Common.Config.Monitors.Exists(m => m.Enabled);

        private void refreshShadeWindows()
        {
            for (int i = 0; i < this._shades.Count; i++)
                this._shades[i].AdjustShade(Common.Config.Monitors[i]);
            this.ensureShadesTopMost();
        }

        private void initializeTrayBrightnessMenu()
        {
            this._trayBrightnessMenu = new TrayBrightnessMenu(
                this.notifyIcon1,
                this.cmxTray,
                () => this.menuItemOpenMain_Click(this.menuItemOpenMain, EventArgs.Empty),
                () => this.menuItemClose_Click(this.menuItemClose, EventArgs.Empty));
            this._trayBrightnessMenu.ShadeEnabledChanged += this.applyTrayShadeEnabled;
            this._trayBrightnessMenu.AlphaBrightnessChanged += this.applyTrayAlphaBrightness;
            this._trayBrightnessMenu.SystemBrightnessChanged += this.applyTraySystemBrightness;
            this._trayBrightnessMenu.Initialize();
        }

        private void refreshTrayBrightnessPanel()
        {
            if (this._trayBrightnessMenu == null || Common.Config == null)
                return;

            this._trayBrightnessMenu.SetState(Common.Config.Alpha, this.ckxAlpha.Checked, this.tbSystem.Enabled, this.tbSystem.Maximum, this.tbSystem.Value);
        }

        private void applyTrayShadeEnabled(bool enabled)
        {
            this.ckxAlpha.Checked = enabled;
        }

        private void applyTrayAlphaBrightness(int value)
        {
            this.changeTbAlpha(value);
        }

        private void applyTraySystemBrightness(int value)
        {
            this.tbSystem.Value = value;
            this.changeSystemBrightness();
            this.refreshTrayBrightnessPanel();
        }

        private void changeSystemBrightness()
        {
            this.lblSystem.Text = this.tbSystem.Value.ToString();

            try
            {
                this._screenBrightness.SetBrightness(this._tbSystemToScreenBrightness);
            }
            catch
            {
                this.tbSystem.Enabled = false;
                this.lblSystem.Text = "不可用";
            }
        }
        #endregion

        #region Events - FormMain
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Action(this.refreshDisplays));
                }
                catch (InvalidOperationException) when (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
                {
                    // 显示器事件可能在主窗体释放句柄时仍在派发。
                }
                return;
            }

            this.refreshDisplays();
        }

        private void refreshDisplays()
        {
            if (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
                return;

            this.updateShades();
            this.refreshShadeWindows();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e) => this.apply();
        private void apply()
        {
            Common.Config.Alpha = (byte)this.tbAlpha.Value;
            Common.Config.AutoHidden = this.ckxAutoHidden.Checked;
            Common.Config.AutoShowShade = this.ckxAutoShowShade.Checked;
            Common.Config.UpdateTime = DateTime.Now;
            Common.Config.Save();
        }

        /// <summary>
        /// 隐藏主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHidden_Click(object sender, EventArgs e) => this.Visible = false;

        private void FormMain_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e) => Process.Start("https://www.enjoycodes.com/Share/GINXR8nZheKDVFYJN9Iqxvo9");

        internal void ShowMainWindow()
        {
            this.Show();
            this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.BringToFront();
        }

        private void _timerSetTopMost_Tick(object sender, EventArgs e)
        {
            this.ensureShadesTopMost();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 只有用户关闭主界面时才隐藏到托盘，系统关机和应用退出照常关闭。
            if (!this._isClosed && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }

            if (Common.Config != null)
                this.apply();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            this._timerSetTopMost.Dispose();
            this._trayBrightnessMenu?.Dispose();
            foreach (var shade in this._shades)
                shade.Dispose();
            this._shades.Clear();
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

            var newAlpha = (byte)value;
            foreach (var monitor in Common.Config.Monitors)
                monitor.ApplyGlobalAlpha(Common.Config.Alpha, newAlpha);

            Common.Config.Alpha = newAlpha;
            this.setBrightness();
        }

        /// <summary>
        /// 调整系统亮度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSystem_Scroll(object sender, EventArgs e)
        {
            this.changeSystemBrightness();
            this.refreshTrayBrightnessPanel();
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

            this.plScreenSettings.Enabled = index < this._shadeScreens.Length;
            this.refreshSelectedMonitorEnabledState();

            var monitor = Common.Config.Monitors[index];
            this.ckxIsMainScreen.Checked = monitor.Primary;
            this.tbAlphaChild.Value = monitor.Alpha;
            this.lblAlphaChildValue.Text = monitor.Alpha.ToString();
            this.txtResolution.Text = $"{monitor.Width}x{monitor.Height}";
        }

        private void refreshSelectedMonitorEnabledState()
        {
            if (this.listView1.SelectedIndices.Count == 0)
                return;

            var index = this.listView1.SelectedIndices[0];
            var monitors = Common.Config.Monitors;
            var enabledCount = monitors.Take(this._shadeScreens.Length).Count(m => m.Enabled);
            this.lblMonitorInfo.Text = $"当前配置第{index + 1}屏，\r\n共启用{enabledCount}屏";
            this.ckxEnabled.Checked = monitors[index].Enabled;
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

            var monitor = Common.Config.Monitors[index];
            if (monitor.Enabled == this.ckxEnabled.Checked)
                return; // 刷新复选框状态时无需再次应用配置。

            monitor.Enabled = this.ckxEnabled.Checked;
            this.showOrHiddenShade(this.hasEnabledMonitor());
            this.apply();
        }

        private void tbAlphaChild_Scroll(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count == 0) return;

            var index = this.listView1.SelectedItems[0].Index; // 当前选中屏幕索引

            var monitor = Common.Config.Monitors[index];
            monitor.Alpha = (byte)this.tbAlphaChild.Value;
            monitor.Enabled = true;
            this.lblAlphaChildValue.Text = monitor.Alpha.ToString();
            this._shades[index].AdjustBrightness(monitor.Alpha);
            this.showOrHiddenShade(true);
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
            this.ShowMainWindow();
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this._isClosed = true;
            this.Close();
        }
        #endregion

        #region Events - 托盘图标（notifyIcon1）
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = !this.Visible;
                if (this.Visible) this.Activate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.refreshTrayBrightnessPanel();
                this._trayBrightnessMenu.ShowAtCursor();
            }
        }
        #endregion
    }
}
