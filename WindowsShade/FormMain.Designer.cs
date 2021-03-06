﻿namespace WindowsShade
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, 0, System.Drawing.Color.Empty, System.Drawing.Color.Empty, null);
            this.btnApply = new System.Windows.Forms.Button();
            this.plContent = new System.Windows.Forms.Panel();
            this.ckxAlpha = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSystem = new System.Windows.Forms.Label();
            this.tbSystem = new System.Windows.Forms.TrackBar();
            this.lblAlphaValue = new System.Windows.Forms.Label();
            this.tbAlpha = new System.Windows.Forms.TrackBar();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmxTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemHidden = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemOpenMain = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.plScreens = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.plScreenSettings = new System.Windows.Forms.Panel();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.tbAlphaChild = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblMonitorInfo = new System.Windows.Forms.Label();
            this.ckxEnabled = new System.Windows.Forms.CheckBox();
            this.ckxIsMainScreen = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.ckxAutoAdjust = new System.Windows.Forms.CheckBox();
            this.ckxAutoShowShade = new System.Windows.Forms.CheckBox();
            this.ckxAutoHidden = new System.Windows.Forms.CheckBox();
            this.plFoot = new System.Windows.Forms.Panel();
            this.btnHidden = new System.Windows.Forms.Button();
            this.plBody = new System.Windows.Forms.Panel();
            this.lblAlphaChildValue = new System.Windows.Forms.Label();
            this.plContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).BeginInit();
            this.cmxTray.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.plScreens.SuspendLayout();
            this.plScreenSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlphaChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.plFoot.SuspendLayout();
            this.plBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnApply.Font = new System.Drawing.Font("宋体", 9.75F);
            this.btnApply.Location = new System.Drawing.Point(87, 10);
            this.btnApply.Margin = new System.Windows.Forms.Padding(7);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(85, 31);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "应用";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // plContent
            // 
            this.plContent.Controls.Add(this.ckxAlpha);
            this.plContent.Controls.Add(this.label4);
            this.plContent.Controls.Add(this.lblSystem);
            this.plContent.Controls.Add(this.tbSystem);
            this.plContent.Controls.Add(this.lblAlphaValue);
            this.plContent.Controls.Add(this.tbAlpha);
            this.plContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plContent.Font = new System.Drawing.Font("宋体", 16F);
            this.plContent.Location = new System.Drawing.Point(3, 3);
            this.plContent.Name = "plContent";
            this.plContent.Size = new System.Drawing.Size(536, 289);
            this.plContent.TabIndex = 1;
            // 
            // ckxAlpha
            // 
            this.ckxAlpha.AutoSize = true;
            this.ckxAlpha.Location = new System.Drawing.Point(81, 86);
            this.ckxAlpha.Name = "ckxAlpha";
            this.ckxAlpha.Size = new System.Drawing.Size(84, 26);
            this.ckxAlpha.TabIndex = 8;
            this.ckxAlpha.Text = "亮度:";
            this.ckxAlpha.UseVisualStyleBackColor = true;
            this.ckxAlpha.CheckedChanged += new System.EventHandler(this.ckxAlpha_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(56, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 22);
            this.label4.TabIndex = 7;
            this.label4.Text = "系统亮度:";
            // 
            // lblSystem
            // 
            this.lblSystem.AutoSize = true;
            this.lblSystem.Location = new System.Drawing.Point(410, 148);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(21, 22);
            this.lblSystem.TabIndex = 6;
            this.lblSystem.Text = "0";
            // 
            // tbSystem
            // 
            this.tbSystem.Location = new System.Drawing.Point(201, 137);
            this.tbSystem.Maximum = 255;
            this.tbSystem.Name = "tbSystem";
            this.tbSystem.Size = new System.Drawing.Size(209, 45);
            this.tbSystem.TabIndex = 5;
            this.tbSystem.Scroll += new System.EventHandler(this.tbSystem_Scroll);
            // 
            // lblAlphaValue
            // 
            this.lblAlphaValue.AutoSize = true;
            this.lblAlphaValue.Location = new System.Drawing.Point(410, 88);
            this.lblAlphaValue.Name = "lblAlphaValue";
            this.lblAlphaValue.Size = new System.Drawing.Size(21, 22);
            this.lblAlphaValue.TabIndex = 6;
            this.lblAlphaValue.Text = "0";
            // 
            // tbAlpha
            // 
            this.tbAlpha.Location = new System.Drawing.Point(201, 77);
            this.tbAlpha.Maximum = 255;
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(209, 45);
            this.tbAlpha.TabIndex = 5;
            this.tbAlpha.Scroll += new System.EventHandler(this.tbAlpha_Scroll);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.cmxTray;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Window Shade";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // cmxTray
            // 
            this.cmxTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHidden,
            this.toolStripSeparator2,
            this.menuItemOpenMain,
            this.toolStripSeparator1,
            this.menuItemClose});
            this.cmxTray.Name = "cmxTray";
            this.cmxTray.Size = new System.Drawing.Size(157, 82);
            // 
            // menuItemHidden
            // 
            this.menuItemHidden.Name = "menuItemHidden";
            this.menuItemHidden.Size = new System.Drawing.Size(156, 22);
            this.menuItemHidden.Text = "隐藏(&H)";
            this.menuItemHidden.Click += new System.EventHandler(this.menuItemHidden_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(153, 6);
            // 
            // menuItemOpenMain
            // 
            this.menuItemOpenMain.Image = ((System.Drawing.Image)(resources.GetObject("menuItemOpenMain.Image")));
            this.menuItemOpenMain.Name = "menuItemOpenMain";
            this.menuItemOpenMain.Size = new System.Drawing.Size(156, 22);
            this.menuItemOpenMain.Text = "打开主界面(&M)";
            this.menuItemOpenMain.Click += new System.EventHandler(this.menuItemOpenMain_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.Size = new System.Drawing.Size(156, 22);
            this.menuItemClose.Text = "退出(&C)";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // tabMain
            // 
            this.tabMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Controls.Add(this.tabPage4);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(550, 325);
            this.tabMain.TabIndex = 2;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.plContent);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(542, 295);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "亮度调整";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.plScreens);
            this.tabPage2.Controls.Add(this.plScreenSettings);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(542, 295);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "屏幕设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // plScreens
            // 
            this.plScreens.Controls.Add(this.listView1);
            this.plScreens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plScreens.Location = new System.Drawing.Point(3, 3);
            this.plScreens.Name = "plScreens";
            this.plScreens.Size = new System.Drawing.Size(536, 147);
            this.plScreens.TabIndex = 1;
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewItem1.StateImageIndex = 0;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(536, 147);
            this.listView1.TabIndex = 0;
            this.listView1.TileSize = new System.Drawing.Size(128, 128);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "monitor.jpg");
            this.imageList1.Images.SetKeyName(1, "monitor_gray.jpg");
            // 
            // plScreenSettings
            // 
            this.plScreenSettings.Controls.Add(this.lblAlphaChildValue);
            this.plScreenSettings.Controls.Add(this.txtResolution);
            this.plScreenSettings.Controls.Add(this.tbAlphaChild);
            this.plScreenSettings.Controls.Add(this.trackBar1);
            this.plScreenSettings.Controls.Add(this.lblMonitorInfo);
            this.plScreenSettings.Controls.Add(this.ckxEnabled);
            this.plScreenSettings.Controls.Add(this.ckxIsMainScreen);
            this.plScreenSettings.Controls.Add(this.label2);
            this.plScreenSettings.Controls.Add(this.label1);
            this.plScreenSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plScreenSettings.Font = new System.Drawing.Font("宋体", 12F);
            this.plScreenSettings.Location = new System.Drawing.Point(3, 150);
            this.plScreenSettings.Name = "plScreenSettings";
            this.plScreenSettings.Size = new System.Drawing.Size(536, 142);
            this.plScreenSettings.TabIndex = 0;
            // 
            // txtResolution
            // 
            this.txtResolution.Location = new System.Drawing.Point(201, 108);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.ReadOnly = true;
            this.txtResolution.Size = new System.Drawing.Size(121, 26);
            this.txtResolution.TabIndex = 4;
            // 
            // tbAlphaChild
            // 
            this.tbAlphaChild.Location = new System.Drawing.Point(194, 68);
            this.tbAlphaChild.Maximum = 255;
            this.tbAlphaChild.Name = "tbAlphaChild";
            this.tbAlphaChild.Size = new System.Drawing.Size(121, 45);
            this.tbAlphaChild.TabIndex = 3;
            this.tbAlphaChild.Scroll += new System.EventHandler(this.tbAlphaChild_Scroll);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(198, 74);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(0, 45);
            this.trackBar1.TabIndex = 6;
            // 
            // lblMonitorInfo
            // 
            this.lblMonitorInfo.Location = new System.Drawing.Point(5, 14);
            this.lblMonitorInfo.Name = "lblMonitorInfo";
            this.lblMonitorInfo.Size = new System.Drawing.Size(133, 50);
            this.lblMonitorInfo.TabIndex = 4;
            this.lblMonitorInfo.Text = "当前配置第1屏，\r\n共启用2屏。";
            // 
            // ckxEnabled
            // 
            this.ckxEnabled.AutoSize = true;
            this.ckxEnabled.Location = new System.Drawing.Point(201, 14);
            this.ckxEnabled.Name = "ckxEnabled";
            this.ckxEnabled.Size = new System.Drawing.Size(59, 20);
            this.ckxEnabled.TabIndex = 1;
            this.ckxEnabled.Text = "启用";
            this.ckxEnabled.UseVisualStyleBackColor = true;
            this.ckxEnabled.CheckedChanged += new System.EventHandler(this.ckxEnabled_CheckedChanged);
            // 
            // ckxIsMainScreen
            // 
            this.ckxIsMainScreen.AutoSize = true;
            this.ckxIsMainScreen.Enabled = false;
            this.ckxIsMainScreen.Location = new System.Drawing.Point(201, 39);
            this.ckxIsMainScreen.Name = "ckxIsMainScreen";
            this.ckxIsMainScreen.Size = new System.Drawing.Size(75, 20);
            this.ckxIsMainScreen.TabIndex = 2;
            this.ckxIsMainScreen.Text = "主屏幕";
            this.ckxIsMainScreen.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "亮度微调：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(123, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "分辨率：";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.ckxAutoAdjust);
            this.tabPage4.Controls.Add(this.ckxAutoShowShade);
            this.tabPage4.Controls.Add(this.ckxAutoHidden);
            this.tabPage4.Font = new System.Drawing.Font("宋体", 16F);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(542, 295);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "软件设置";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // ckxAutoAdjust
            // 
            this.ckxAutoAdjust.AutoSize = true;
            this.ckxAutoAdjust.Location = new System.Drawing.Point(89, 146);
            this.ckxAutoAdjust.Name = "ckxAutoAdjust";
            this.ckxAutoAdjust.Size = new System.Drawing.Size(161, 26);
            this.ckxAutoAdjust.TabIndex = 2;
            this.ckxAutoAdjust.Text = "自动调整亮度";
            this.ckxAutoAdjust.UseVisualStyleBackColor = true;
            this.ckxAutoAdjust.CheckedChanged += new System.EventHandler(this.ckxAutoAdjust_CheckedChanged);
            // 
            // ckxAutoShowShade
            // 
            this.ckxAutoShowShade.AutoSize = true;
            this.ckxAutoShowShade.Location = new System.Drawing.Point(89, 92);
            this.ckxAutoShowShade.Name = "ckxAutoShowShade";
            this.ckxAutoShowShade.Size = new System.Drawing.Size(183, 26);
            this.ckxAutoShowShade.TabIndex = 1;
            this.ckxAutoShowShade.Text = "启动时调整亮度";
            this.ckxAutoShowShade.UseVisualStyleBackColor = true;
            // 
            // ckxAutoHidden
            // 
            this.ckxAutoHidden.AutoSize = true;
            this.ckxAutoHidden.Location = new System.Drawing.Point(89, 40);
            this.ckxAutoHidden.Name = "ckxAutoHidden";
            this.ckxAutoHidden.Size = new System.Drawing.Size(205, 26);
            this.ckxAutoHidden.TabIndex = 0;
            this.ckxAutoHidden.Text = "启动时隐藏主界面";
            this.ckxAutoHidden.UseVisualStyleBackColor = true;
            // 
            // plFoot
            // 
            this.plFoot.Controls.Add(this.btnHidden);
            this.plFoot.Controls.Add(this.btnApply);
            this.plFoot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plFoot.Font = new System.Drawing.Font("宋体", 9.75F);
            this.plFoot.Location = new System.Drawing.Point(0, 325);
            this.plFoot.Name = "plFoot";
            this.plFoot.Size = new System.Drawing.Size(550, 66);
            this.plFoot.TabIndex = 3;
            // 
            // btnHidden
            // 
            this.btnHidden.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnHidden.Location = new System.Drawing.Point(224, 10);
            this.btnHidden.Name = "btnHidden";
            this.btnHidden.Size = new System.Drawing.Size(85, 31);
            this.btnHidden.TabIndex = 3;
            this.btnHidden.Text = "Hidden";
            this.btnHidden.UseVisualStyleBackColor = true;
            this.btnHidden.Click += new System.EventHandler(this.btnHidden_Click);
            // 
            // plBody
            // 
            this.plBody.Controls.Add(this.tabMain);
            this.plBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plBody.Location = new System.Drawing.Point(0, 0);
            this.plBody.Name = "plBody";
            this.plBody.Size = new System.Drawing.Size(550, 325);
            this.plBody.TabIndex = 4;
            // 
            // lblAlphaChildValue
            // 
            this.lblAlphaChildValue.AutoSize = true;
            this.lblAlphaChildValue.Font = new System.Drawing.Font("宋体", 16F);
            this.lblAlphaChildValue.Location = new System.Drawing.Point(316, 79);
            this.lblAlphaChildValue.Name = "lblAlphaChildValue";
            this.lblAlphaChildValue.Size = new System.Drawing.Size(21, 22);
            this.lblAlphaChildValue.TabIndex = 7;
            this.lblAlphaChildValue.Text = "0";
            // 
            // FormMain
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnHidden;
            this.ClientSize = new System.Drawing.Size(550, 391);
            this.Controls.Add(this.plBody);
            this.Controls.Add(this.plFoot);
            this.Font = new System.Drawing.Font("宋体", 20F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(566, 420);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WindowsShade";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.FormMain_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plContent.ResumeLayout(false);
            this.plContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSystem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).EndInit();
            this.cmxTray.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.plScreens.ResumeLayout(false);
            this.plScreenSettings.ResumeLayout(false);
            this.plScreenSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlphaChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.plFoot.ResumeLayout(false);
            this.plBody.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel plContent;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip cmxTray;
        private System.Windows.Forms.ToolStripMenuItem menuItemClose;
        private System.Windows.Forms.ToolStripMenuItem menuItemHidden;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpenMain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TrackBar tbAlpha;
        private System.Windows.Forms.Label lblAlphaValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSystem;
        private System.Windows.Forms.TrackBar tbSystem;
        private System.Windows.Forms.CheckBox ckxAlpha;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel plFoot;
        private System.Windows.Forms.Panel plBody;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnHidden;
        private System.Windows.Forms.CheckBox ckxAutoHidden;
        private System.Windows.Forms.CheckBox ckxAutoShowShade;
        private System.Windows.Forms.Panel plScreens;
        private System.Windows.Forms.Panel plScreenSettings;
        private System.Windows.Forms.CheckBox ckxIsMainScreen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.CheckBox ckxEnabled;
        private System.Windows.Forms.Label lblMonitorInfo;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.CheckBox ckxAutoAdjust;
        private System.Windows.Forms.TrackBar tbAlphaChild;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblAlphaChildValue;
    }
}

