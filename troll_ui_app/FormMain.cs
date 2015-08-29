using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using log4net;
using TrotiNet;
using System.Threading;
using System.Net.Mail;
using Microsoft.VisualBasic;

namespace troll_ui_app
{
    public partial class FormMain : Form
    {
        static readonly ILog log = Log.Get();
        private FilterRecordDisplay record_display_form = new FilterRecordDisplay();
        //= new FilterRecordDisplay();
        private bool onoff = true;
        private const int MOD_CTRL = 0x0002;
        private const int MOD_ALT = 0x0001;
        const String kAutoRunRegisstryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const String kAutoRunKey = "trollwiz";
        private TcpServer Server;
        System.Threading.Timer delete_history_timer;
        System.Threading.Timer update_domain_list_timer;
        Task updateTask;
        Task exitMailTask;
        public FormMain(String []args)
        {
            InitializeComponent();
            notify_icon_main.Icon = Properties.Resources.on;
            //if (args.Contains("-notvisible"))
            //    notify_icon_main.Visible = false;
            //set hotkey as ctrl+alt+backspace
            //Boolean success = FormMain.RegisterHotKey(this.Handle, this.GetType().GetHashCode(), MOD_CTRL | MOD_ALT, 0x08);//Set hotkey as 'b'
            //set the owner to avoid the main form in atl-table window
            Form form1 = new Form();
            form1.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form1.ShowInTaskbar = false;
            Owner = form1;
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var Params = base.CreateParams;
        //        Params.ExStyle |= 0x80;
        //        return Params;
        //    }
        //}

        private void notify_icon_main_DoubleClick(object sender, EventArgs e)
        {
            //this.Visible = true;
            //this.WindowState = FormWindowState.Normal;
        }

        private void RecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(record_display_form.IsDisposed)
                record_display_form = new FilterRecordDisplay();
            record_display_form.Show();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //authenticate by wechat
            WechatForm wechatForm = new WechatForm(true);
            wechatForm.ShowDialog();
            if (wechatForm.authSuccess)
            {
                //exitMailTask = NotificationRoutines.SendExitNotification();
                Application.Exit();
            }
            //this.Close();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //    this.Hide();
        }

        private void tool_strip_menu_item_toggle_onff_Click(object sender, EventArgs e)
        {
            if (onoff)
            {
                //Task mailTask = NotificationRoutines.SendStopNotification();
                WechatForm wechatForm = new WechatForm(true);
                wechatForm.ShowDialog();
                if (wechatForm.authSuccess)
                {
#if !DEBUG
                    ProxyRoutines.SetProxy(false);
#endif
                    notify_icon_main.Icon = Properties.Resources.off;
                    tool_strip_menu_item_toggle_onff.Text = "开始保护";
                    onoff = !onoff;
                }
            }
            else
            {
#if !DEBUG
                ProxyRoutines.SetProxy("http=127.0.0.1:8090", null);
#endif
                notify_icon_main.Icon = Properties.Resources.on;
                tool_strip_menu_item_toggle_onff.Text = "停止保护";
                onoff = !onoff;
            }
        }

        private void tool_strip_menu_item_auto_start_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            if (mi.Checked)
            {
                autorun_registry_key.SetValue(kAutoRunKey, Application.ExecutablePath + " -notvisible");
            }
            else
            {
                WechatForm wechatForm = new WechatForm(true);
                wechatForm.ShowDialog();
                if (wechatForm.authSuccess)
                    autorun_registry_key.DeleteValue(kAutoRunKey);
                else
                    mi.Checked = true;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ProxyRoutines.SetProxy(false);
            log.Info("FormMain_FormClosed Unset Proxy!");
            //dispose timer and wait for callback complete
            WaitHandle[] whs = new WaitHandle[]{
                new AutoResetEvent(false),
                new AutoResetEvent(false) };
            //WaitHandle wh = new AutoResetEvent(false);
            delete_history_timer.Dispose(whs[0]);
            update_domain_list_timer.Dispose(whs[1]);
            foreach (WaitHandle wh in whs)
                wh.WaitOne();
            Server.Stop();
            updateTask.Wait();
            //exitMailTask.Wait();
            log.Info("Exit gracefully!");
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                notify_icon_main.Visible = !notify_icon_main.Visible;
                //MessageBox.Show("Catched");//You can replace this statement with your desired response to the Hotkey.
            }
            base.WndProc(ref m);
        }

        private void toolStripMenuItemLocalScan_Click(object sender, EventArgs e)
        {
            Form scan = new TemporaryFileScan();
            scan.Show();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //check update
            log.Info("Load FormMain!");
            
            const bool bUseIPv6 = false;
            Server = new TcpServer(Properties.Settings.Default.bindPort, bUseIPv6);

            Server.Start(GreenProxy.CreateProxy);

            Server.InitListenFinished.WaitOne();
            if (Server.InitListenException != null)
                throw Server.InitListenException;

            update_domain_list_timer = new System.Threading.Timer(PornDatabase.UpdateDatabase, null, new TimeSpan(0, 0, 5), new TimeSpan(4, 0, 0));
            delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0, 1, 0), System.Threading.Timeout.InfiniteTimeSpan);

            //setup
#if !DEBUG
            ProxyRoutines.SetProxy("http=127.0.0.1:8090", null);
#endif
            tool_strip_menu_item_toggle_onff.Text = "停止保护";

            //ShowInTaskbar = false;
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            var autostart = autorun_registry_key.GetValue(kAutoRunKey);
            if (autostart == null)
                ToolStripMenuItemAutoStartToggleOnff.Checked = false;
            else
            {
                //autorun_registry_key.SetValue(kAutoRunKey, Application.ExecutablePath+" -notvisible");
                ToolStripMenuItemAutoStartToggleOnff.Checked = true;
            }

            if(Properties.Settings.Default.openid == "")
            {
                //binding wechat
                WechatForm wechatForm = new WechatForm(false);
                wechatForm.ShowDialog();
            }

            updateTask = UpdateForm.UpdateProduct();

            //if(Properties.Settings.Default.email == "")
            //{
            //    SetEmail();
            //}

            if (Program.FirstTime)
            {
                //we need non modal here
                if (DialogResult.Yes == MessageBox.Show("是否扫描浏览器记录？", "本地扫描", MessageBoxButtons.YesNo))
                {
                    Form scan = new TemporaryFileScan();
                    scan.Show();
                }
            }
        }

        private void SetEmail()
        {
            bool tryAgain = true;
            string prompt = "请输入Email地址";
            string title = "当前Email: "+Properties.Settings.Default.email;
            while (tryAgain)
            {
                try
                {
                    string ret = Interaction.InputBox(prompt, title);
                    MailAddress ma = new MailAddress(ret);
                    //send mail to notify email changed if new email address is valid
                    //Task n = MailRoutines.SendMailChangedNotification(ma.Address);
                    Properties.Settings.Default.email = ma.Address;
                    Properties.Settings.Default.Save();
                    tryAgain = false;
                }
                catch (FormatException fe)
                {
                    prompt = "请输入有效Email地址";
                    tryAgain = true;
                }
                catch(ArgumentException ae)
                {
                    log.Info(ae.ToString());
                    tryAgain = false;
                }
            }
        }

        private void toolStripMenuItemChangeEmail_Click(object sender, EventArgs e)
        {
            SetEmail();
        }
    }
}
