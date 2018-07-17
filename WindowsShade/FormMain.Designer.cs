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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAlphaValue = new System.Windows.Forms.Label();
            this.tbAlpha = new System.Windows.Forms.TrackBar();
            this.cbxShadeTypes = new System.Windows.Forms.ComboBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmxTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemHidden = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemOpenMain = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.plContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).BeginInit();
            this.cmxTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnApply.Location = new System.Drawing.Point(213, 188);
            this.btnApply.Margin = new System.Windows.Forms.Padding(7);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(105, 35);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // plContent
            // 
            this.plContent.Controls.Add(this.label2);
            this.plContent.Controls.Add(this.label1);
            this.plContent.Controls.Add(this.lblAlphaValue);
            this.plContent.Controls.Add(this.tbAlpha);
            this.plContent.Controls.Add(this.cbxShadeTypes);
            this.plContent.Controls.Add(this.btnHelp);
            this.plContent.Controls.Add(this.btnApply);
            this.plContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plContent.Font = new System.Drawing.Font("宋体", 16F);
            this.plContent.Location = new System.Drawing.Point(0, 0);
            this.plContent.Name = "plContent";
            this.plContent.Size = new System.Drawing.Size(487, 262);
            this.plContent.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Shade:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 22);
            this.label1.TabIndex = 7;
            this.label1.Text = "Alpha:";
            // 
            // lblAlphaValue
            // 
            this.lblAlphaValue.AutoSize = true;
            this.lblAlphaValue.Location = new System.Drawing.Point(406, 123);
            this.lblAlphaValue.Name = "lblAlphaValue";
            this.lblAlphaValue.Size = new System.Drawing.Size(21, 22);
            this.lblAlphaValue.TabIndex = 6;
            this.lblAlphaValue.Text = "0";
            // 
            // tbAlpha
            // 
            this.tbAlpha.Location = new System.Drawing.Point(197, 112);
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
            this.cbxShadeTypes.Location = new System.Drawing.Point(197, 47);
            this.cbxShadeTypes.Name = "cbxShadeTypes";
            this.cbxShadeTypes.Size = new System.Drawing.Size(121, 29);
            this.cbxShadeTypes.TabIndex = 4;
            // 
            // btnHelp
            // 
            this.btnHelp.Font = new System.Drawing.Font("宋体", 14F);
            this.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHelp.Location = new System.Drawing.Point(89, 191);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 31);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
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
            this.cmxTray.Size = new System.Drawing.Size(181, 104);
            // 
            // menuItemHidden
            // 
            this.menuItemHidden.Name = "menuItemHidden";
            this.menuItemHidden.Size = new System.Drawing.Size(180, 22);
            this.menuItemHidden.Text = "显示(&D)";
            this.menuItemHidden.Click += new System.EventHandler(this.menuItemHidden_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // menuItemOpenMain
            // 
            this.menuItemOpenMain.Image = ((System.Drawing.Image)(resources.GetObject("menuItemOpenMain.Image")));
            this.menuItemOpenMain.Name = "menuItemOpenMain";
            this.menuItemOpenMain.Size = new System.Drawing.Size(180, 22);
            this.menuItemOpenMain.Text = "打开主界面(&M)";
            this.menuItemOpenMain.Click += new System.EventHandler(this.menuItemOpenMain_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.Size = new System.Drawing.Size(180, 22);
            this.menuItemClose.Text = "退出(&C)";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // FormMain
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnApply;
            this.ClientSize = new System.Drawing.Size(487, 262);
            this.Controls.Add(this.plContent);
            this.Font = new System.Drawing.Font("宋体", 20F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WindowsShade";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plContent.ResumeLayout(false);
            this.plContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbAlpha)).EndInit();
            this.cmxTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel plContent;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip cmxTray;
        private System.Windows.Forms.ToolStripMenuItem menuItemClose;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemHidden;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpenMain;
        private System.Windows.Forms.ComboBox cbxShadeTypes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TrackBar tbAlpha;
        private System.Windows.Forms.Label lblAlphaValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

