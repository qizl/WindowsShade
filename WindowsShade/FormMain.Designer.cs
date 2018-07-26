namespace WindowsShade
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
            this.btnApply = new System.Windows.Forms.Button();
            this.plContent = new System.Windows.Forms.Panel();
            this.cbxAlpha = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSystem = new System.Windows.Forms.Label();
            this.tbSystem = new System.Windows.Forms.TrackBar();
            this.lblAlphaValue = new System.Windows.Forms.Label();
            this.tbAlpha = new System.Windows.Forms.TrackBar();
            this.cbxShadeTypes = new System.Windows.Forms.ComboBox();
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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.plFoot = new System.Windows.Forms.Panel();
            this.plBody = new System.Windows.Forms.Panel();
            this.btnHidden = new System.Windows.Forms.Button();
            this.cbxAutoHidden = new System.Windows.Forms.CheckBox();
            this.plContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).BeginInit();
            this.cmxTray.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
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
            this.plContent.Controls.Add(this.cbxAlpha);
            this.plContent.Controls.Add(this.label2);
            this.plContent.Controls.Add(this.label4);
            this.plContent.Controls.Add(this.lblSystem);
            this.plContent.Controls.Add(this.tbSystem);
            this.plContent.Controls.Add(this.lblAlphaValue);
            this.plContent.Controls.Add(this.tbAlpha);
            this.plContent.Controls.Add(this.cbxShadeTypes);
            this.plContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plContent.Font = new System.Drawing.Font("宋体", 16F);
            this.plContent.Location = new System.Drawing.Point(3, 3);
            this.plContent.Name = "plContent";
            this.plContent.Size = new System.Drawing.Size(536, 242);
            this.plContent.TabIndex = 1;
            // 
            // cbxAlpha
            // 
            this.cbxAlpha.AutoSize = true;
            this.cbxAlpha.Location = new System.Drawing.Point(81, 98);
            this.cbxAlpha.Name = "cbxAlpha";
            this.cbxAlpha.Size = new System.Drawing.Size(84, 26);
            this.cbxAlpha.TabIndex = 8;
            this.cbxAlpha.Text = "亮度:";
            this.cbxAlpha.UseVisualStyleBackColor = true;
            this.cbxAlpha.CheckedChanged += new System.EventHandler(this.cbxAlpha_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(89, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Shade:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(56, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 22);
            this.label4.TabIndex = 7;
            this.label4.Text = "系统亮度:";
            // 
            // lblSystem
            // 
            this.lblSystem.AutoSize = true;
            this.lblSystem.Location = new System.Drawing.Point(410, 160);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(21, 22);
            this.lblSystem.TabIndex = 6;
            this.lblSystem.Text = "0";
            // 
            // tbSystem
            // 
            this.tbSystem.Location = new System.Drawing.Point(201, 149);
            this.tbSystem.Maximum = 255;
            this.tbSystem.Name = "tbSystem";
            this.tbSystem.Size = new System.Drawing.Size(209, 45);
            this.tbSystem.TabIndex = 5;
            this.tbSystem.Scroll += new System.EventHandler(this.tbSystem_Scroll);
            // 
            // lblAlphaValue
            // 
            this.lblAlphaValue.AutoSize = true;
            this.lblAlphaValue.Location = new System.Drawing.Point(410, 100);
            this.lblAlphaValue.Name = "lblAlphaValue";
            this.lblAlphaValue.Size = new System.Drawing.Size(21, 22);
            this.lblAlphaValue.TabIndex = 6;
            this.lblAlphaValue.Text = "0";
            // 
            // tbAlpha
            // 
            this.tbAlpha.Location = new System.Drawing.Point(201, 89);
            this.tbAlpha.Maximum = 255;
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(209, 45);
            this.tbAlpha.TabIndex = 5;
            this.tbAlpha.Scroll += new System.EventHandler(this.tbAlpha_Scroll);
            // 
            // cbxShadeTypes
            // 
            this.cbxShadeTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxShadeTypes.FormattingEnabled = true;
            this.cbxShadeTypes.Location = new System.Drawing.Point(201, 37);
            this.cbxShadeTypes.Name = "cbxShadeTypes";
            this.cbxShadeTypes.Size = new System.Drawing.Size(121, 29);
            this.cbxShadeTypes.TabIndex = 4;
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
            this.tabMain.Size = new System.Drawing.Size(550, 278);
            this.tabMain.TabIndex = 2;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.plContent);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(542, 248);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "亮度调整";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(542, 248);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "多屏设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.cbxAutoHidden);
            this.tabPage4.Font = new System.Drawing.Font("宋体", 16F);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(542, 248);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "软件设置";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // plFoot
            // 
            this.plFoot.Controls.Add(this.btnHidden);
            this.plFoot.Controls.Add(this.btnApply);
            this.plFoot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plFoot.Font = new System.Drawing.Font("宋体", 9.75F);
            this.plFoot.Location = new System.Drawing.Point(0, 278);
            this.plFoot.Name = "plFoot";
            this.plFoot.Size = new System.Drawing.Size(550, 66);
            this.plFoot.TabIndex = 3;
            // 
            // plBody
            // 
            this.plBody.Controls.Add(this.tabMain);
            this.plBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plBody.Location = new System.Drawing.Point(0, 0);
            this.plBody.Name = "plBody";
            this.plBody.Size = new System.Drawing.Size(550, 278);
            this.plBody.TabIndex = 4;
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
            // cbxAutoHidden
            // 
            this.cbxAutoHidden.AutoSize = true;
            this.cbxAutoHidden.Location = new System.Drawing.Point(89, 40);
            this.cbxAutoHidden.Name = "cbxAutoHidden";
            this.cbxAutoHidden.Size = new System.Drawing.Size(117, 26);
            this.cbxAutoHidden.TabIndex = 0;
            this.cbxAutoHidden.Text = "自动隐藏";
            this.cbxAutoHidden.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnHidden;
            this.ClientSize = new System.Drawing.Size(550, 344);
            this.Controls.Add(this.plBody);
            this.Controls.Add(this.plFoot);
            this.Font = new System.Drawing.Font("宋体", 20F);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(566, 383);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WindowsShade";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.FormMain_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plContent.ResumeLayout(false);
            this.plContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSystem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).EndInit();
            this.cmxTray.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
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
        private System.Windows.Forms.ComboBox cbxShadeTypes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TrackBar tbAlpha;
        private System.Windows.Forms.Label lblAlphaValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSystem;
        private System.Windows.Forms.TrackBar tbSystem;
        private System.Windows.Forms.CheckBox cbxAlpha;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel plFoot;
        private System.Windows.Forms.Panel plBody;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnHidden;
        private System.Windows.Forms.CheckBox cbxAutoHidden;
    }
}

