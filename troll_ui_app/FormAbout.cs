using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    public partial class FormAbout : Form
    {
        static FormAbout SingleInstance;
        private FormAbout()
        {
            InitializeComponent();
            Icon = Properties.Resources.TrollIcon;
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            Version curVersion = new Version(versionInfo.ProductVersion);
            VersionLabel.Text = curVersion.ToString();
        }

        static public void ShowAbout()
        {
            if (SingleInstance == null || SingleInstance.IsDisposed)
                SingleInstance = new FormAbout();
            SingleInstance.Show();
        }
    }
}
