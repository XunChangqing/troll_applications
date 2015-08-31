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
        //private FilterRecordDisplay record_display_form = new FilterRecordDisplay();
        //= new FilterRecordDisplay();
        private const int MOD_CTRL = 0x0002;
        private const int MOD_ALT = 0x0001;
        const String kAutoRunRegisstryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const String kAutoRunKey = "trollwiz";
        bool isProxyOn = false;
        public FormMain(String []args)
        {
            InitializeComponent();
            if (args.Contains("-notvisible"))
                WindowState = FormWindowState.Minimized;
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
            if (WechatForm.Auth())
            {
                //exitMailTask = NotificationRoutines.SendExitNotification();
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

        private void toggleOnOff()
        {
            if (isProxyOn)
            {
                //Task mailTask = NotificationRoutines.SendStopNotification();
                if (WechatForm.Auth())
                {
#if !DEBUG
                    ProxyRoutines.SetProxy(false);
#endif
                    //notify_icon_main.Icon = Properties.Resources.off;
                    tool_strip_menu_item_toggle_onff.Checked = false;
                    toggleOnOffButton.Image = Properties.Resources.powerblack_small;
                    isProxyOn = !isProxyOn;
                }
            }
            else
            {
#if !DEBUG
                ProxyRoutines.SetProxy("http=127.0.0.1:8090", null);
#endif
                //notify_icon_main.Icon = Properties.Resources.on;
                tool_strip_menu_item_toggle_onff.Checked = true;
                toggleOnOffButton.Image = Properties.Resources.powerblue_small;
                isProxyOn = !isProxyOn;
            }
        }

        private void tool_strip_menu_item_auto_start_Click(object sender, EventArgs e)
        {
            autoStartToggle(false);
        }
        private void mainAutoStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoStartToggle(false);
        }
        private void autoStartToggle(bool isInit)
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            var autostart = autorun_registry_key.GetValue(kAutoRunKey);
            if (autostart == null)
            {
                if (isInit)
                {
                    ToolStripMenuItemAutoStartToggleOnff.Checked = false;
                    mainAutoStartToolStripMenuItem.Checked = false;
                }
                else
                {
                    autorun_registry_key.SetValue(kAutoRunKey, Application.ExecutablePath + " -notvisible");
                    ToolStripMenuItemAutoStartToggleOnff.Checked = true;
                    mainAutoStartToolStripMenuItem.Checked = true;
                }
            }
            else
            {
                ToolStripMenuItemAutoStartToggleOnff.Checked = true;
                mainAutoStartToolStripMenuItem.Checked = true;
                if (WechatForm.Auth())
                {
                    autorun_registry_key.DeleteValue(kAutoRunKey);
                    ToolStripMenuItemAutoStartToggleOnff.Checked = false;
                    mainAutoStartToolStripMenuItem.Checked = false;
                }
            }
        }
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isProxyOn)
                toggleOnOff();
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
            TemporaryFileScan.GetInstance().Show();
            TemporaryFileScan.GetInstance().WindowState = FormWindowState.Normal;
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
                Show();
            }
            else
                base.OnMouseClick(e);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //init ui
            helpToolStripDropDownButton.Image = Properties.Resources.help;
            settingsToolStripDropDownButton.Image = Properties.Resources.advancedsettings;
            hideToolStripButton.Image = Properties.Resources.symbol_delete_icon;
            notifyIconMain.Icon = Properties.Resources.main_icon;

            toggleOnOff();
            autoStartToggle(true);
            //binding, update and local scan
            if(Properties.Settings.Default.openid == "")
            {
                //binding wechat
                WechatForm wechatForm = new WechatForm(false);
                wechatForm.ShowDialog();
            }

            //new instance and get update info asyncly
            UpdateInfoForm.GetInstance();

            if (Program.FirstTime)
            {
                //we need non modal here
                if (DialogResult.Yes == MessageBox.Show("是否扫描浏览器记录？", "本地扫描", MessageBoxButtons.YesNo))
                    TemporaryFileScan.GetInstance().Show();
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
