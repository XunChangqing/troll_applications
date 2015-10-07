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
using Titanium.Web.Proxy.Helpers;

namespace troll_ui_app
{
    public partial class FormMain : Form
    {
        static readonly ILog log = Log.Get();
        //private FilterRecordDisplay record_display_form = new FilterRecordDisplay();
        //= new FilterRecordDisplay();
        private const int MOD_CTRL = 0x0002;
        private const int MOD_ALT = 0x0001;
        const String kAutoRunRegisstryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const String kAutoRunKey = "trollwiz";
        bool IsRealQuit = false;
        public FormMain(String []args)
        {
            InitializeComponent();
            Icon = Properties.Resources.TrollIcon;
            //settingsToolStripDropDownButton.Image = Properties.Resources.advancedsettings;
            //helpToolStripDropDownButton.Image = Properties.Resources.help;
            //localScanButton.Image = Properties.Resources.search_button;
            if (args.Contains("-notvisible"))
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
            //set hotkey as ctrl+alt+backspace
            //Boolean success = FormMain.RegisterHotKey(this.Handle, this.GetType().GetHashCode(), MOD_CTRL | MOD_ALT, 0x08);//Set hotkey as 'b'
            //set the owner to avoid the main form in atl-table window
            Form form1 = new Form();
            form1.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form1.ShowInTaskbar = false;
            Owner = form1;
        }
        private void RecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if(record_display_form.IsDisposed)
            //    record_display_form = new FilterRecordDisplay();
            //record_display_form.Show();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //authenticate by wechat
            log.Info("Auth for Quit!");
            if (WechatForm.Auth())
            {
                //exitMailTask = NotificationRoutines.SendExitNotification();
                log.Info("auth success!");
                IsRealQuit = true;
                Application.Exit();
            }
            //this.Close();
        }

        private void tool_strip_menu_item_toggle_onff_Click(object sender, EventArgs e)
        {
            toggleOnOff();
        }
        private void toggleOnOffButton_Click(object sender, EventArgs e)
        {
            toggleOnOff();
        }

        private void TurnOnProxy()
        {
#if !DEBUG
                //ProxyRoutines.SetProxy("http=127.0.0.1:8090", null);
                SystemProxyHelper.EnableProxyHTTP("127.0.0.1", 8090);
                FireFoxHelper.AddFirefox();
#endif
                //notify_icon_main.Icon = Properties.Resources.on;
            tool_strip_menu_item_toggle_onff.Checked = true;
            //toggleOnOffButton.Image = Properties.Resources.powerblue_small;
            Properties.Settings.Default.isProtected = true;
            Properties.Settings.Default.Save();
            //同时设置
            TurnOnAutoStart();
        }
        private void TurnOffProxy()
        {
            if (WechatForm.Auth())
            {
#if !DEBUG
                    //ProxyRoutines.SetProxy(false);
                    SystemProxyHelper.DisableAllProxy();
                    FireFoxHelper.RemoveFirefox();
#endif
                    //notify_icon_main.Icon = Properties.Resources.off;
                tool_strip_menu_item_toggle_onff.Checked = false;
                //toggleOnOffButton.Image = Properties.Resources.powerblack_small;
                Properties.Settings.Default.isProtected = false;
                Properties.Settings.Default.Save();
                //同时设置
                TurnOffAutoStart();
            }
        }

        private void toggleOnOff()
        {
            //Task mailTask = NotificationRoutines.SendStopNotification();
            if (Properties.Settings.Default.isProtected)
                TurnOffProxy();
            else
                TurnOnProxy();
        }

        private void tool_strip_menu_item_auto_start_Click(object sender, EventArgs e)
        {
            autoStartToggle();
        }
        private void mainAutoStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoStartToggle();
        }
        private void TurnOnAutoStart()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            autorun_registry_key.SetValue(kAutoRunKey, Application.ExecutablePath + " -notvisible");
            ToolStripMenuItemAutoStartToggleOnff.Checked = true;
            //mainAutoStartToolStripMenuItem.Checked = true;
        }
        private void TurnOffAutoStart()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            if (WechatForm.Auth())
            {
                //autorun_registry_key.DeleteValue(kAutoRunKey);
                autorun_registry_key.SetValue(kAutoRunKey, "");
                ToolStripMenuItemAutoStartToggleOnff.Checked = false;
                //mainAutoStartToolStripMenuItem.Checked = false;
            }
        }
        private void InitAutoStart()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            var autostart = autorun_registry_key.GetValue(kAutoRunKey);
            if (autostart == null)
            {
                ToolStripMenuItemAutoStartToggleOnff.Checked = false;
                //mainAutoStartToolStripMenuItem.Checked = false;
            }
            else
            {
                ToolStripMenuItemAutoStartToggleOnff.Checked = true;
                //mainAutoStartToolStripMenuItem.Checked = true;
            }

        }
        private void autoStartToggle()
        {
            //if (mainAutoStartToolStripMenuItem.Checked == true)
            //    TurnOnAutoStart();
            //else
            //    TurnOffAutoStart();
        }
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
#if !DEBUG
//            if (Properties.Settings.Default.isProtected)
            //ProxyRoutines.SetProxy(false);
            SystemProxyHelper.DisableAllProxy();
            FireFoxHelper.RemoveFirefox();
#endif
            log.Info("FormMain_FormClosed Unset Proxy!");
        }

        private void toolStripMenuItemLocalScan_Click(object sender, EventArgs e)
        {
            localScan();
        }
        private void localScanButton_Click(object sender, EventArgs e)
        {
            localScan();
        }
        private void localScan()
        {
            //TemporaryFileScan.GetInstance().Show();
            //TemporaryFileScan.GetInstance().WindowState = FormWindowState.Normal;
        }
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkUpdate();
        }
        private void checkUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkUpdate();
        }
        private void checkUpdate()
        {
            UpdateInfoForm.GetInstance().Show();
            UpdateInfoForm.GetInstance().WindowState = FormWindowState.Normal;
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.helpUrl);
        }
        private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.helpUrl);
        }

        private void hideToolStripButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void notifyIconMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
                Show();
            }
            else
                base.OnMouseClick(e);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //init ui
            //helpToolStripDropDownButton.Image = Properties.Resources.help;
            //settingsToolStripDropDownButton.Image = Properties.Resources.advancedsettings;
            //hideToolStripButton.Image = Properties.Resources.symbol_delete_icon;

            notifyIconMain.Icon = Properties.Resources.TrollIcon;

            if (Properties.Settings.Default.isProtected)
                TurnOnProxy();
            //else
                //toggleOnOffButton.Image = Properties.Resources.powerblack_small;
               
            InitAutoStart();
            //binding, update and local scan
            if(Properties.Settings.Default.openid == "")
            {
                log.Info("binding wechat, openid: "+Properties.Settings.Default.openid);
                //binding wechat
                //WechatForm wechatForm = new WechatForm(false);
                //wechatForm.ShowDialog();
            }

            //new instance and get update info asyncly
            UpdateInfoForm.GetInstance();

            if (Program.FirstTime)
            {
                //we need non modal here
                //if (DialogResult.Yes == MessageBox.Show(Properties.Resources.RunLocalScanFirstTime, Properties.Resources.LocalScan, MessageBoxButtons.YesNo))
                //    TemporaryFileScan.GetInstance().Show();
            }
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                notifyIconMain.Visible = !notifyIconMain.Visible;
            }
            base.WndProc(ref m);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsRealQuit)
            {
                log.Info("just hide!");
                Hide();
                e.Cancel = true;
            }
            else
                log.Info("real quit!");
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout.ShowAbout();
        }
        //private void SetEmail()
        //{
        //    bool tryAgain = true;
        //    string prompt = "请输入Email地址";
        //    string title = "当前Email: "+Properties.Settings.Default.email;
        //    while (tryAgain)
        //    {
        //        try
        //        {
        //            string ret = Interaction.InputBox(prompt, title);
        //            MailAddress ma = new MailAddress(ret);
        //            //send mail to notify email changed if new email address is valid
        //            //Task n = MailRoutines.SendMailChangedNotification(ma.Address);
        //            Properties.Settings.Default.email = ma.Address;
        //            Properties.Settings.Default.Save();
        //            tryAgain = false;
        //        }
        //        catch (FormatException fe)
        //        {
        //            prompt = "请输入有效Email地址";
        //            tryAgain = true;
        //        }
        //        catch(ArgumentException ae)
        //        {
        //            log.Info(ae.ToString());
        //            tryAgain = false;
        //        }
        //    }
        //}

        //private void toolStripMenuItemChangeEmail_Click(object sender, EventArgs e)
        //{
        //    SetEmail();
        //}
    }
}
