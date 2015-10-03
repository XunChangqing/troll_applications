namespace troll_ui_app
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
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tool_strip_menu_item_toggle_onff = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemLocalScan = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemAutoStartToggleOnff = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.打开面板ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.QuitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.toggleOnOffButton = new System.Windows.Forms.Button();
            this.localScanButton = new System.Windows.Forms.Button();
            this.contextMenuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_strip_menu_item_toggle_onff,
            this.toolStripMenuItemLocalScan,
            this.ToolStripMenuItemAutoStartToggleOnff,
            this.toolStripSeparator2,
            this.打开面板ToolStripMenuItem,
            this.toolStripSeparator1,
            this.QuitToolStripMenuItem});
            this.contextMenuMain.Name = "context_menu_main";
            this.contextMenuMain.Size = new System.Drawing.Size(125, 126);
            // 
            // tool_strip_menu_item_toggle_onff
            // 
            this.tool_strip_menu_item_toggle_onff.CheckOnClick = true;
            this.tool_strip_menu_item_toggle_onff.Name = "tool_strip_menu_item_toggle_onff";
            this.tool_strip_menu_item_toggle_onff.Size = new System.Drawing.Size(124, 22);
            this.tool_strip_menu_item_toggle_onff.Text = "实时防护";
            this.tool_strip_menu_item_toggle_onff.Click += new System.EventHandler(this.tool_strip_menu_item_toggle_onff_Click);
            // 
            // toolStripMenuItemLocalScan
            // 
            this.toolStripMenuItemLocalScan.Name = "toolStripMenuItemLocalScan";
            this.toolStripMenuItemLocalScan.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemLocalScan.Text = "本地扫描";
            this.toolStripMenuItemLocalScan.Click += new System.EventHandler(this.toolStripMenuItemLocalScan_Click);
            // 
            // ToolStripMenuItemAutoStartToggleOnff
            // 
            this.ToolStripMenuItemAutoStartToggleOnff.CheckOnClick = true;
            this.ToolStripMenuItemAutoStartToggleOnff.Name = "ToolStripMenuItemAutoStartToggleOnff";
            this.ToolStripMenuItemAutoStartToggleOnff.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemAutoStartToggleOnff.Text = "开机启动";
            this.ToolStripMenuItemAutoStartToggleOnff.Visible = false;
            this.ToolStripMenuItemAutoStartToggleOnff.Click += new System.EventHandler(this.tool_strip_menu_item_auto_start_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(121, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // 打开面板ToolStripMenuItem
            // 
            this.打开面板ToolStripMenuItem.Name = "打开面板ToolStripMenuItem";
            this.打开面板ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打开面板ToolStripMenuItem.Text = "打开面板";
            this.打开面板ToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            // 
            // QuitToolStripMenuItem
            // 
            this.QuitToolStripMenuItem.Name = "QuitToolStripMenuItem";
            this.QuitToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.QuitToolStripMenuItem.Text = "退出";
            this.QuitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // notifyIconMain
            // 
            this.notifyIconMain.ContextMenuStrip = this.contextMenuMain;
            this.notifyIconMain.Text = "山妖卫士";
            this.notifyIconMain.Visible = true;
            this.notifyIconMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconMain_MouseClick);
            // 
            // toggleOnOffButton
            // 
            this.toggleOnOffButton.Location = new System.Drawing.Point(55, 47);
            this.toggleOnOffButton.Name = "toggleOnOffButton";
            this.toggleOnOffButton.Size = new System.Drawing.Size(100, 90);
            this.toggleOnOffButton.TabIndex = 0;
            this.toggleOnOffButton.TabStop = false;
            this.toggleOnOffButton.Text = "实时防护";
            this.toggleOnOffButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toggleOnOffButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toggleOnOffButton.UseVisualStyleBackColor = true;
            this.toggleOnOffButton.Click += new System.EventHandler(this.toggleOnOffButton_Click);
            // 
            // localScanButton
            // 
            this.localScanButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.localScanButton.Location = new System.Drawing.Point(189, 47);
            this.localScanButton.Name = "localScanButton";
            this.localScanButton.Size = new System.Drawing.Size(100, 90);
            this.localScanButton.TabIndex = 2;
            this.localScanButton.TabStop = false;
            this.localScanButton.Text = "本地扫描";
            this.localScanButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.localScanButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.localScanButton.UseVisualStyleBackColor = true;
            this.localScanButton.Click += new System.EventHandler(this.localScanButton_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(708, 442);
            this.ContextMenuStrip = this.contextMenuMain;
            this.Controls.Add(this.toggleOnOffButton);
            this.Controls.Add(this.localScanButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "山妖卫士";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.contextMenuMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem QuitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tool_strip_menu_item_toggle_onff;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAutoStartToggleOnff;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLocalScan;
        private System.Windows.Forms.NotifyIcon notifyIconMain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 打开面板ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button toggleOnOffButton;
        private System.Windows.Forms.Button localScanButton;
    }
}

