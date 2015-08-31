using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace troll_ui_app
{
    public partial class TemporaryFileScan : Form
    {
        static readonly ILog log = Log.Get();
        static TemporaryFileScan SingleInstance;
        class PornFile
        {
            public PornFile(String p, PornClassifier.ImageType t)
            {
                path = p;
                type = t;
            }
            public String path { get; set; }
            public PornClassifier.ImageType type { get; set; }
        }

        private BindingSource bindingSource = new BindingSource();
        private TemporaryFileScan()
        {
            InitializeComponent();
        }
        public static TemporaryFileScan GetInstance()
        {
            if (SingleInstance == null || SingleInstance.IsDisposed)
                SingleInstance = new TemporaryFileScan();
            return SingleInstance;
        }

        private void TemporaryFileScan_Load(object sender, EventArgs e)
        {
            bindingSource.DataSource = typeof(PornFile);
            pornGridView.DataSource = bindingSource;
            //bindingSource.DataMember = "path";
            //bindingSource = typeof(PornFile);
            pornGridView.Columns["type"].Visible = false;
            //pornGridView.Columns["path"].HeaderText = "路径";
            //pornGridView.Columns["type"].HeaderText = "类型";
            //pornGridView.AutoGenerateColumns = true;
            pictureBox.DataBindings.Add("ImageLocation", bindingSource, "path");
            backgroundWorker.RunWorkerAsync();
        }

        private void pornGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void TemporaryFileScan_FormClosed(object sender, FormClosedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            var totalfiles = Directory.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));

            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dirs = SafeFileEnumerator.EnumerateDirectories(roamingPath, "Cache", SearchOption.AllDirectories);
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dirs = dirs.Concat(SafeFileEnumerator.EnumerateDirectories(localAppDataPath, "Cache", SearchOption.AllDirectories));
            foreach(var dir in dirs)
                totalfiles = totalfiles.Concat(SafeFileEnumerator.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories));

            int tcount = totalfiles.Count();
            int num = 0;
            foreach (var file in totalfiles)
            {
                log.Info("Scan: " + file);
                PornClassifier.ImageType t = PornClassifier.Instance.Classify(file);
                num++;
                backgroundWorker.ReportProgress(100 * num / tcount, new PornFile(file, t));
                if (backgroundWorker.CancellationPending)
                    break;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            PornFile pf = (PornFile)e.UserState;
            currentFileName.Text = pf.path;
            if (pf.type == PornClassifier.ImageType.Porn)
                bindingSource.Add(pf);
        }
    }
}
