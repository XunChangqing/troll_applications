﻿namespace troll_ui_app
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
            this.notify_icon_main = new System.Windows.Forms.NotifyIcon(this.components);
            this.context_menu_main = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tool_strip_menu_item_toggle_onff = new System.Windows.Forms.ToolStripMenuItem();
            this.RecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemAutoStartToggleOnff = new System.Windows.Forms.ToolStripMenuItem();
            this.QuitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.context_menu_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // notify_icon_main
            // 
            this.notify_icon_main.ContextMenuStrip = this.context_menu_main;
            this.notify_icon_main.Text = "山妖卫士";
            this.notify_icon_main.Visible = true;
            this.notify_icon_main.DoubleClick += new System.EventHandler(this.notify_icon_main_DoubleClick);
            // 
            // context_menu_main
            // 
            this.context_menu_main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_strip_menu_item_toggle_onff,
            this.RecordToolStripMenuItem,
            this.ToolStripMenuItemAutoStartToggleOnff,
            this.QuitToolStripMenuItem});
            this.context_menu_main.Name = "context_menu_main";
            this.context_menu_main.Size = new System.Drawing.Size(125, 92);
            // 
            // tool_strip_menu_item_toggle_onff
            // 
            this.tool_strip_menu_item_toggle_onff.Name = "tool_strip_menu_item_toggle_onff";
            this.tool_strip_menu_item_toggle_onff.Size = new System.Drawing.Size(124, 22);
            this.tool_strip_menu_item_toggle_onff.Text = "开启防护";
            this.tool_strip_menu_item_toggle_onff.Click += new System.EventHandler(this.tool_strip_menu_item_toggle_onff_Click);
            // 
            // RecordToolStripMenuItem
            // 
            this.RecordToolStripMenuItem.Name = "RecordToolStripMenuItem";
            this.RecordToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.RecordToolStripMenuItem.Text = "拦截记录";
            this.RecordToolStripMenuItem.Click += new System.EventHandler(this.RecordToolStripMenuItem_Click);
            // 
            // ToolStripMenuItemAutoStartToggleOnff
            // 
            this.ToolStripMenuItemAutoStartToggleOnff.CheckOnClick = true;
            this.ToolStripMenuItemAutoStartToggleOnff.Name = "ToolStripMenuItemAutoStartToggleOnff";
            this.ToolStripMenuItemAutoStartToggleOnff.Size = new System.Drawing.Size(124, 22);
            this.ToolStripMenuItemAutoStartToggleOnff.Text = "开机启动";
            this.ToolStripMenuItemAutoStartToggleOnff.Click += new System.EventHandler(this.tool_strip_menu_item_auto_start_Click);
            // 
            // QuitToolStripMenuItem
            // 
            this.QuitToolStripMenuItem.Name = "QuitToolStripMenuItem";
            this.QuitToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.QuitToolStripMenuItem.Text = "退出";
            this.QuitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 0);
            this.Name = "FormMain";
            this.ShowInTaskbar = false;
            this.Text = "山妖卫士";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.SizeChanged += new System.EventHandler(this.FormMain_SizeChanged);
            this.context_menu_main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notify_icon_main;
        private System.Windows.Forms.ContextMenuStrip context_menu_main;
        private System.Windows.Forms.ToolStripMenuItem RecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem QuitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tool_strip_menu_item_toggle_onff;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAutoStartToggleOnff;
    }
}
