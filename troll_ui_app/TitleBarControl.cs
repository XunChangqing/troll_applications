using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace troll_ui_app
{
    public class TitleBarControl : UserControl
    {
        static readonly int titleBarHeight = 32;
        static readonly int leftPadding = 10;
        static readonly int rightPadding = 10;

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void MouseDownMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.ParentForm.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                //SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        ImageButton closeBtn;
        ImageButton minimizeBtn;
        ImageButton menuBtn;

        ContextMenuStrip contextMenu;
        //static readonly int 
        Control logo;
        OutlineLabel title;
        ToolStripItem _protectionCenterItem;
        public ToolStripItem ProtectionCenterItem { get { return _protectionCenterItem; } }

        public string TitleText
        {
            get
            {
                return title.Text;
            }
            set
            {
                title.Text = value;
            }
        }

        private bool hideWindow;
        public bool HideWindow
        {
            get
            { return hideWindow; }
            set
            {
                hideWindow = value;
            }
        }
        private bool showMinimizeBar= true;
        public bool ShowMinimizeBar
        {
            get { return showMinimizeBar; }
            set
            {
                showMinimizeBar = value;
                minimizeBtn.Visible = value;
            }
        }
        private bool showMenuBar = true;
        public bool ShowMenuBar
        {
            get { return showMenuBar; }
            set
            {
                showMenuBar = value;
                menuBtn.Visible = value;
            }
        }
        //public bool WithReturnButton { get; set; }

        public TitleBarControl()
        {
            hideWindow = true;
            Height = titleBarHeight;
            //Padding = new Padding(20, 0, 20, 0);
            BackgroundImage = Properties.Resources.home_fh_bg;

            Dock = DockStyle.Top;
            MouseDown += MouseDownMove;

            closeBtn = new ImageButton();
            closeBtn.Image = Properties.Resources.btn_max_n;
            closeBtn.Size = closeBtn.Image.Size;
            closeBtn.NormalBack = Properties.Resources.btn_max_n;
            closeBtn.HoverBack = Properties.Resources.btn_max_h;
            closeBtn.PressBack = Properties.Resources.btn_max_p;
            closeBtn.Click += closeBtnOnClick;
            //closeBtn.Dock = DockStyle.Right;
            closeBtn.Location = new Point(MainForm.MainFormWidth - rightPadding - closeBtn.Width, 0);

            minimizeBtn = new ImageButton();
            minimizeBtn.Image = Properties.Resources.btn_small_n;
            minimizeBtn.Size = minimizeBtn.Image.Size;
            minimizeBtn.NormalBack = Properties.Resources.btn_small_n;
            minimizeBtn.HoverBack = Properties.Resources.btn_small_h;
            minimizeBtn.PressBack = Properties.Resources.btn_small_p;
            minimizeBtn.Click += minimizeBtnOnClick;
            //minimizeBtn.Dock = DockStyle.Right;
            minimizeBtn.Location = new Point(closeBtn.Location.X - minimizeBtn.Width, 0);

            menuBtn = new ImageButton();
            menuBtn.Image = Properties.Resources.btn_menu_n;
            menuBtn.Size = menuBtn.Image.Size;
            menuBtn.NormalBack = Properties.Resources.btn_menu_n;
            menuBtn.HoverBack = Properties.Resources.btn_menu_h;
            menuBtn.PressBack = Properties.Resources.btn_menu_p;
            menuBtn.Click += menuBtnOnClick;
            menuBtn.Location = new Point(minimizeBtn.Location.X - menuBtn.Width, 0);
            //menuBtn.Dock = DockStyle.Right;

            contextMenu = new ContextMenuStrip();
            _protectionCenterItem = contextMenu.Items.Add("防护中心");
            ToolStripItem helpItem = contextMenu.Items.Add("在线帮助");
            helpItem.Click += TitleBarMenuHelpClick;
            ToolStripItem aboutItem = contextMenu.Items.Add("关于");
            aboutItem.Click += TitleBarMenuAboutClick;

            logo = new Control();
            logo.BackgroundImage = Properties.Resources.icon_big_png;
            logo.BackgroundImageLayout = ImageLayout.Zoom;
            //logo.MaximumSize = logo.MinimumSize = logo.BackgroundImage.Size;
            logo.MaximumSize = logo.MinimumSize = new System.Drawing.Size(24, 24);
            logo.MouseDown += MouseDownMove;
            logo.Location = new Point(leftPadding, (titleBarHeight - logo.Height) / 2);
            //logo.Dock = DockStyle.Left;

            title = new OutlineLabel();
            title.AutoSize = true;
            //title.BackColor = Color.Yellow;
            title.BackColor = Color.Transparent;
            title.Text = "山妖卫士";
            //title.MinimumSize = new Size(0, titleBarHeight);
            //title.TextAlign = ContentAlignment.MiddleCenter;
            //title.Location = new Point(logo.Location.X + logo.Width, 0);
            title.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Point);
            title.ForeColor = Color.White;
            title.OutlineForeColor = Color.FromArgb(0x54, 0xa5, 0x13);
            //title.OutlineWidth = 1;
            //title.Font = new System.Drawing.Font()
            //title.Dock = DockStyle.Left;
            title.MouseDown += MouseDownMove;
            Load += TitleBarControlOnLoad;

            Controls.Add(title);
            Controls.Add(logo);

            Controls.Add(menuBtn);
            Controls.Add(minimizeBtn);
            Controls.Add(closeBtn);
        }

        bool _withLogo = true;
        public bool WithLogo { get
            {
                return _withLogo;
            }
            set
            {
                _withLogo = value;
                if(_withLogo)
                {
                    logo.Visible = true;
                }
                else
                {
                    logo.Visible = false;
                }
            }
        }

        void TitleBarControlOnLoad(object sender, EventArgs e)
        {
            if(_withLogo)
                title.Location = new Point(logo.Location.X + logo.Width, titleBarHeight / 2 - title.Height / 2);
            else
                title.Location = new Point(leftPadding, titleBarHeight / 2 - title.Height / 2);
            //if(WithReturnButton)
            //{
            //    returnBtn = new ImageButton();
            //}
        }

        void TitleBarMenuHelpClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.helpUrl);
        }
        void TitleBarMenuAboutClick(object sender, EventArgs e)
        {
            FormAbout.ShowAbout();
        }

        void closeBtnOnClick(object sender, EventArgs e)
        {
            if (hideWindow)
                FindForm().Hide();
            else
                FindForm().Close();
        }
        void minimizeBtnOnClick(object sender, EventArgs e)
        {
            FindForm().WindowState = FormWindowState.Minimized;
        }
        void menuBtnOnClick(object sender, EventArgs e)
        {
            contextMenu.Show(menuBtn, 0, menuBtn.Height);
        }
        void TitleBarOnLoad(object sender, EventArgs e)
        {
        }
    }
}
