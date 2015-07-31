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

namespace troll_ui_app
{
    public partial class FormMain : Form
    {
        private FilterRecordDisplay record_display_form = new FilterRecordDisplay();
        //= new FilterRecordDisplay();
        private bool onoff = true;
        private const int MOD_CTRL = 0x0002;
        private const int MOD_ALT = 0x0001;
        public FormMain(String []args)
        {
            InitializeComponent();
            if (args.Contains("-notvisible"))
                notify_icon_main.Visible = false;
            //set hotkey as ctrl+alt+backspace
            Boolean success = FormMain.RegisterHotKey(this.Handle, this.GetType().GetHashCode(), MOD_CTRL | MOD_ALT, 0x08);//Set hotkey as 'b'
            //record_display_form.Show();
            Proxies.SetProxy();
            tool_strip_menu_item_toggle_onff.Text = "停止保护";

            //ShowInTaskbar = false;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            var autostart = registryKey.GetValue("troll_guard");
            if (autostart == null)
                ToolStripMenuItemAutoStartToggleOnff.Checked = false;
            else
                ToolStripMenuItemAutoStartToggleOnff.Checked = true;

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
            this.Close();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //    this.Hide();
        }

        private void tool_strip_menu_item_toggle_onff_Click(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            if (onoff)
            {
                Proxies.UnsetProxy();
                //notify_icon_main.Icon = new Icon("Resources/on.ico");
                notify_icon_main.Icon = ((System.Drawing.Icon)(resources.GetObject("off.Icon")));
                tool_strip_menu_item_toggle_onff.Text = "开始保护";
            }
            else
            {
                Proxies.SetProxy();
                notify_icon_main.Icon = ((System.Drawing.Icon)(resources.GetObject("on.Icon")));
                tool_strip_menu_item_toggle_onff.Text = "停止保护";
            }
            onoff = !onoff;
        }

        private void tool_strip_menu_item_auto_start_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if(mi.Checked)
            {
                registryKey.SetValue("troll_guard", Application.ExecutablePath+" -notvisible");
            }
            else
            {
                registryKey.DeleteValue("troll_guard");
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Proxies.UnsetProxy();
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
    }
}
