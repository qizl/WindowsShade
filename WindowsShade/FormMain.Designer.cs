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
            this.btnStart = new System.Windows.Forms.Button();
            this.plContent = new System.Windows.Forms.Panel();
            this.btnHelp = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtnD2 = new System.Windows.Forms.RadioButton();
            this.rbtnD1 = new System.Windows.Forms.RadioButton();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmxTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHidden = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.plContent.SuspendLayout();
            this.panel1.SuspendLayout();
            this.cmxTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(208, 128);
            this.btnStart.Margin = new System.Windows.Forms.Padding(7);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(125, 51);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // plContent
            // 
            this.plContent.Controls.Add(this.btnHelp);
            this.plContent.Controls.Add(this.panel1);
            this.plContent.Controls.Add(this.btnStart);
            this.plContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plContent.Location = new System.Drawing.Point(0, 0);
            this.plContent.Name = "plContent";
            this.plContent.Size = new System.Drawing.Size(540, 306);
            this.plContent.TabIndex = 1;
            // 
            // btnHelp
            // 
            this.btnHelp.Font = new System.Drawing.Font("宋体", 14F);
            this.btnHelp.Location = new System.Drawing.Point(47, 254);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(85, 31);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtnD2);
            this.panel1.Controls.Add(this.rbtnD1);
            this.panel1.Location = new System.Drawing.Point(113, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 59);
            this.panel1.TabIndex = 1;
            // 
            // rbtnD2
            // 
            this.rbtnD2.AutoSize = true;
            this.rbtnD2.Location = new System.Drawing.Point(125, 15);
            this.rbtnD2.Name = "rbtnD2";
            this.rbtnD2.Size = new System.Drawing.Size(58, 31);
            this.rbtnD2.TabIndex = 1;
            this.rbtnD2.TabStop = true;
            this.rbtnD2.Text = "D2";
            this.rbtnD2.UseVisualStyleBackColor = true;
            // 
            // rbtnD1
            // 
            this.rbtnD1.AutoSize = true;
            this.rbtnD1.Checked = true;
            this.rbtnD1.Location = new System.Drawing.Point(22, 15);
            this.rbtnD1.Name = "rbtnD1";
            this.rbtnD1.Size = new System.Drawing.Size(58, 31);
            this.rbtnD1.TabIndex = 0;
            this.rbtnD1.TabStop = true;
            this.rbtnD1.Text = "D1";
            this.rbtnD1.UseVisualStyleBackColor = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.cmxTray;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // cmxTray
            // 
            this.cmxTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHidden,
            this.toolStripSeparator1,
            this.menuItemClose});
            this.cmxTray.Name = "cmxTray";
            this.cmxTray.Size = new System.Drawing.Size(153, 76);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.Size = new System.Drawing.Size(152, 22);
            this.menuItemClose.Text = "退出(&C)";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // menuItemHidden
            // 
            this.menuItemHidden.Name = "menuItemHidden";
            this.menuItemHidden.Size = new System.Drawing.Size(152, 22);
            this.menuItemHidden.Text = "隐藏(&H)";
            this.menuItemHidden.Click += new System.EventHandler(this.menuItemHidden_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // FormMain
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 306);
            this.Controls.Add(this.plContent);
            this.Font = new System.Drawing.Font("宋体", 20F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WindowsShade";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.plContent.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.cmxTray.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel plContent;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip cmxTray;
        private System.Windows.Forms.ToolStripMenuItem menuItemClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtnD2;
        private System.Windows.Forms.RadioButton rbtnD1;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemHidden;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

