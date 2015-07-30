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

namespace troll_ui_app
{
    public partial class FormMain : Form
    {
        private FilterRecordDisplay record_display_form = new FilterRecordDisplay();
        //= new FilterRecordDisplay();
        private bool onoff = true;
        public FormMain()
        {
            InitializeComponent();
            //record_display_form.Show();
            Proxies.SetProxy();
            tool_strip_menu_item_toggle_onff.Text = "停止保护";

            //ShowInTaskbar = false;

            //set the owner to avoid the main form in atl-table window
            Form form1 = new Form();
            form1.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form1.ShowInTaskbar = false;
            Owner = form1;
            //Form fw = new Form();
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
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Proxies.UnsetProxy();
        }
    }
}
