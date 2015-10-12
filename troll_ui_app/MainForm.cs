using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Http;
using System.Runtime.InteropServices;
using Titanium.Web.Proxy.Helpers;
using log4net;
using TrotiNet;

namespace troll_ui_app
{
    public class MainForm : Form
    {
        static readonly ILog log = Log.Get();
        public static readonly int MainFormWidth = 900;
        public static readonly int MainFormHeight = 600;
        public enum Effect { Roll, Slide, Center, Blend }

        public static void Animate(Control ctl, Effect effect, int msec, int angle)
        {
            int flags = effmap[(int)effect];
            if (ctl.Visible) { flags |= 0x10000; angle += 180; }
            else
            {
                if (ctl.TopLevelControl == ctl) flags |= 0x20000;
                else if (effect == Effect.Blend) throw new ArgumentException();
            }
            flags |= dirmap[(angle % 360) / 45];
            bool ok = AnimateWindow(ctl.Handle, msec, flags);
            if (!ok) throw new Exception("Animation failed");
            ctl.Visible = !ctl.Visible;
        }
        //public bool ForceToQuit { get; set;}

        private static int[] dirmap = { 1, 5, 4, 6, 2, 10, 8, 9 };
        private static int[] effmap = { 0, 0x40000, 0x10, 0x80000 };

        [DllImport("user32.dll")]
        private static extern bool AnimateWindow(IntPtr handle, int msec, int flags);

        public void SlideWindow(Control window)
        {
            Animate(window, Effect.Slide, 100, 180);
            //bool origVisible = window.Visible;
            //int sx, dx;
            //int stride = 180;
            //if(origVisible)
            //{
            //    sx = 0;
            //    dx = 901;
            //    window.Location = new Point(sx, 0);
            //}
            //else
            //{
            //    sx = 901;
            //    dx = 0;
            //    stride = -stride;
            //    window.Location = new Point(sx, 0);
            //    window.Visible = true;
            //}

            //for (int i = sx; Math.Abs(i-sx) < MainForm.MainFormWidth; i+=stride)
            //{
            //    window.Location = new Point(i, 0);
            //    if (origVisible)
            //        mainPanelControl.Refresh();
            //    window.Refresh();
            //    //System.Threading.Thread.Sleep(10);
            //}
            //window.Location = new Point(dx, 0);

            //if (origVisible)
            //    window.Visible = false;
        }

        public MainPanelControl mainPanelControl;
        public ScanPanelControl scanPanelControl;
        public ProtectionPanelControl _protectionPanelControl;
        public ActiveFileMonitor _activeFileMonitor;
        public TcpServer Server;
        public Progress<PornDatabase.PornItemType> TargetProcessedProgress;

        public static MainForm Instance;

        NotifyIcon _mainNotifyIcon;
        ContextMenuStrip _mainContextMenuTrip;
        bool bindingSuccess = true;
        public MainForm(String []args)
        {
            Instance = this;
            //ForceToQuit = false;
            Icon = Properties.Resources.icon_main_icon;

            //必须在UI内部初始化，保证progress对象是由UI线程初始化的
            PornDatabase.Init();

            TargetProcessedProgress = new Progress<PornDatabase.PornItemType>();

            const bool bUseIPv6 = false;
            Server = new TcpServer(Properties.Settings.Default.bindPort, bUseIPv6);
            Server.Start(GreenProxy.CreateProxy);
            Server.InitListenFinished.WaitOne();
            if (Server.InitListenException != null)
                throw Server.InitListenException;
#if !DEBUG
            SystemProxyHelper.EnableProxyHTTP("127.0.0.1", 8090);
            FireFoxHelper.AddFirefox();
#endif
            //PornDatabase pdb = new PornDatabase();
            //pdb.InsertPornFile("C:/xyz", PornDatabase.PornItemType.LocalImage);

            _activeFileMonitor = new ActiveFileMonitor();

            m_aeroEnabled = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //InitializeComponent();
            Size = new System.Drawing.Size(900, 600);
            mainPanelControl = new MainPanelControl();
            scanPanelControl = new ScanPanelControl();
            _protectionPanelControl = new ProtectionPanelControl();
            mainPanelControl.Size = Size;
            scanPanelControl.Size = Size;
            _protectionPanelControl.Size = Size;
            Controls.Add(_protectionPanelControl);
            Controls.Add(scanPanelControl);
            Controls.Add(mainPanelControl);

            _mainContextMenuTrip = new System.Windows.Forms.ContextMenuStrip();
            ToolStripItem openMainPanel = _mainContextMenuTrip.Items.Add("打开主界面");
            ToolStripItem quitItem = _mainContextMenuTrip.Items.Add("退出");
            openMainPanel.Click += openMainPanelOnClick;
            quitItem.Click += quitItemOnClick;

            _mainNotifyIcon = new NotifyIcon();
            _mainNotifyIcon.ContextMenuStrip = _mainContextMenuTrip;
            _mainNotifyIcon.Icon = Properties.Resources.icon_main_icon;
            _mainNotifyIcon.Visible = true;
            _mainNotifyIcon.DoubleClick += _mainNotifyIconOnDoubleClick;

            //mainPanelControl.ScanEvent += mainPanelOnScanEvent;
            Load += MainFormOnLoad;
            FormClosed += MainFormOnFormClosed;
            FormClosing += MainFormOnFormClosing;
            Disposed += MainFormOnDisposed;

            if (args.Contains("-notvisible"))
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
            //set hotkey as ctrl+alt+backspace
            //Boolean success = FormMain.RegisterHotKey(this.Handle, this.GetType().GetHashCode(), MOD_CTRL | MOD_ALT, 0x08);//Set hotkey as 'b'
            //set the owner to avoid the main form in atl-table window
            //Form form1 = new Form();
            //form1.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            //form1.ShowInTaskbar = false;
            //Owner = form1;
        }

        void MainFormOnDisposed(object sender, EventArgs e)
        {
            _mainNotifyIcon.Visible = false;
            Server.Stop();
            log.Info("Mainform Disposed!");
        }

        void MainFormOnFormClosing(object sender, FormClosingEventArgs e)
        {
            log.Info("MainForm Closing: " + e.CloseReason.ToString());
            //if (!(e.CloseReason == CloseReason.) !ForceToQuit && !WechatForm.Auth())
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.TaskManagerClosing)
            //|| e.CloseReason == CloseReason.WindowsShutDown)
            {
                if (!WechatForm.Auth())
                {
                    e.Cancel = true;
                }
            }
            //else if (e.CloseReason == CloseReason.WindowsShutDown)
            //    Application.Exit();
        }

        void MainFormOnFormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.Exit();
            if(e.CloseReason == CloseReason.WindowsShutDown)
            {
                Program.CleanUp();
            }
            log.Info("MainForm Closed!");
            //确保触发application.exit事件，系统关机时不会自动触发Application.Exit事件
        }

        void _mainNotifyIconOnDoubleClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Show();
        }


        void MainFormOnLoad(object sender, EventArgs e)
        {
            log.Info("MainForm Load!");
            scanPanelControl.Visible = false;
            _protectionPanelControl.Visible = false;
            UpdateInfoForm.GetInstance();
        }
        //void scanPanelOnReturnEvent(object sender, troll_ui_app.ScanPanelControl.ReturnEventArgs e)
        //{
        //    //scanPanel.Visible = false;
        //    Animate(scanPanelControl, Effect.Slide, 200, 180);
        //    mainPanelControl.Refresh();
        //}
        void mainPanelOnScanEvent(object sender, troll_ui_app.MainPanelControl.ScanEventArgs e)
        {
            Console.WriteLine("Scan Event!");
            MainForm.Instance.SlideWindow(scanPanelControl);
            //Animate(scanPanelControl, Effect.Slide, 200, 180);
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1 
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        void quitItemOnClick(object sender, EventArgs e)
        {
            if (WechatForm.Auth())
            {
                Application.Exit();
            }
        }
        void openMainPanelOnClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Show();
        }
    }
}
