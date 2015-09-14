namespace troll_ui_app
{
    partial class FilterRecordDisplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tab_control_record = new System.Windows.Forms.TabControl();
            this.context_menu_grid_view = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.刷新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tab_pics = new System.Windows.Forms.TabPage();
            this.data_grid_view_porn_pics = new System.Windows.Forms.DataGridView();
            this.tab_page_pages = new System.Windows.Forms.TabPage();
            this.data_grid_view_pages = new System.Windows.Forms.DataGridView();
            this.picture_box = new System.Windows.Forms.PictureBox();
            this.tab_control_record.SuspendLayout();
            this.context_menu_grid_view.SuspendLayout();
            this.tab_pics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view_porn_pics)).BeginInit();
            this.tab_page_pages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view_pages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture_box)).BeginInit();
            this.SuspendLayout();
            // 
            // tab_control_record
            // 
            this.tab_control_record.ContextMenuStrip = this.context_menu_grid_view;
            this.tab_control_record.Controls.Add(this.tab_pics);
            this.tab_control_record.Controls.Add(this.tab_page_pages);
            this.tab_control_record.Dock = System.Windows.Forms.DockStyle.Left;
            this.tab_control_record.Location = new System.Drawing.Point(0, 0);
            this.tab_control_record.Name = "tab_control_record";
            this.tab_control_record.SelectedIndex = 0;
            this.tab_control_record.Size = new System.Drawing.Size(846, 480);
            this.tab_control_record.TabIndex = 0;
            // 
            // context_menu_grid_view
            // 
            this.context_menu_grid_view.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.刷新ToolStripMenuItem});
            this.context_menu_grid_view.Name = "context_menu_grid_view";
            this.context_menu_grid_view.Size = new System.Drawing.Size(101, 26);
            // 
            // 刷新ToolStripMenuItem
            // 
            this.刷新ToolStripMenuItem.Name = "刷新ToolStripMenuItem";
            this.刷新ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.刷新ToolStripMenuItem.Text = "刷新";
            this.刷新ToolStripMenuItem.Click += new System.EventHandler(this.刷新ToolStripMenuItem_Click);
            // 
            // tab_pics
            // 
            this.tab_pics.Controls.Add(this.data_grid_view_porn_pics);
            this.tab_pics.Location = new System.Drawing.Point(4, 22);
            this.tab_pics.Name = "tab_pics";
            this.tab_pics.Padding = new System.Windows.Forms.Padding(3);
            this.tab_pics.Size = new System.Drawing.Size(838, 454);
            this.tab_pics.TabIndex = 0;
            this.tab_pics.Text = "拦截的图片";
            this.tab_pics.UseVisualStyleBackColor = true;
            // 
            // data_grid_view_porn_pics
            // 
            this.data_grid_view_porn_pics.AllowUserToAddRows = false;
            this.data_grid_view_porn_pics.AllowUserToDeleteRows = false;
            this.data_grid_view_porn_pics.AllowUserToResizeRows = false;
            this.data_grid_view_porn_pics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.data_grid_view_porn_pics.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.data_grid_view_porn_pics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.data_grid_view_porn_pics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.data_grid_view_porn_pics.Location = new System.Drawing.Point(3, 3);
            this.data_grid_view_porn_pics.Name = "data_grid_view_porn_pics";
            this.data_grid_view_porn_pics.ReadOnly = true;
            this.data_grid_view_porn_pics.RowHeadersVisible = false;
            this.data_grid_view_porn_pics.RowTemplate.Height = 23;
            this.data_grid_view_porn_pics.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.data_grid_view_porn_pics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.data_grid_view_porn_pics.Size = new System.Drawing.Size(832, 448);
            this.data_grid_view_porn_pics.TabIndex = 0;
            this.data_grid_view_porn_pics.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.data_grid_view_porn_pics_CellFormatting);
            this.data_grid_view_porn_pics.SelectionChanged += new System.EventHandler(this.data_grid_view_porn_pics_SelectionChanged);
            this.data_grid_view_porn_pics.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.data_grid_view_porn_pics_MouseDoubleClick);
            // 
            // tab_page_pages
            // 
            this.tab_page_pages.Controls.Add(this.data_grid_view_pages);
            this.tab_page_pages.Location = new System.Drawing.Point(4, 22);
            this.tab_page_pages.Name = "tab_page_pages";
            this.tab_page_pages.Padding = new System.Windows.Forms.Padding(3);
            this.tab_page_pages.Size = new System.Drawing.Size(838, 454);
            this.tab_page_pages.TabIndex = 1;
            this.tab_page_pages.Text = "拦截的网页";
            this.tab_page_pages.UseVisualStyleBackColor = true;
            // 
            // data_grid_view_pages
            // 
            this.data_grid_view_pages.AllowUserToAddRows = false;
            this.data_grid_view_pages.AllowUserToDeleteRows = false;
            this.data_grid_view_pages.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.data_grid_view_pages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.data_grid_view_pages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.data_grid_view_pages.Location = new System.Drawing.Point(3, 3);
            this.data_grid_view_pages.Name = "data_grid_view_pages";
            this.data_grid_view_pages.ReadOnly = true;
            this.data_grid_view_pages.RowHeadersVisible = false;
            this.data_grid_view_pages.RowTemplate.Height = 23;
            this.data_grid_view_pages.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.data_grid_view_pages.Size = new System.Drawing.Size(832, 448);
            this.data_grid_view_pages.TabIndex = 0;
            this.data_grid_view_pages.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.data_grid_view_pages_CellFormatting);
            // 
            // picture_box
            // 
            this.picture_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picture_box.Location = new System.Drawing.Point(846, 0);
            this.picture_box.Name = "picture_box";
            this.picture_box.Size = new System.Drawing.Size(249, 480);
            this.picture_box.TabIndex = 1;
            this.picture_box.TabStop = false;
            // 
            // FilterRecordDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1095, 480);
            this.Controls.Add(this.picture_box);
            this.Controls.Add(this.tab_control_record);
            this.Name = "FilterRecordDisplay";
            this.Text = "拦截记录";
            this.Load += new System.EventHandler(this.FilterRecordDisplay_Load);
            this.tab_control_record.ResumeLayout(false);
            this.context_menu_grid_view.ResumeLayout(false);
            this.tab_pics.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view_porn_pics)).EndInit();
            this.tab_page_pages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view_pages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture_box)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tab_control_record;
        private System.Windows.Forms.TabPage tab_pics;
        private System.Windows.Forms.TabPage tab_page_pages;
        private System.Windows.Forms.DataGridView data_grid_view_porn_pics;
        private System.Windows.Forms.DataGridView data_grid_view_pages;
        private System.Windows.Forms.ContextMenuStrip context_menu_grid_view;
        private System.Windows.Forms.ToolStripMenuItem 刷新ToolStripMenuItem;
        private System.Windows.Forms.PictureBox picture_box;
    }
}