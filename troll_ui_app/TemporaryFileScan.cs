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
using System.Threading;

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
            Icon = Properties.Resources.TrollIcon;
            progressChanged = new ProgressChanged(ProgressChangedMethod);
        }
        public static TemporaryFileScan GetInstance()
        {
            if (SingleInstance == null || SingleInstance.IsDisposed)
                SingleInstance = new TemporaryFileScan();
            return SingleInstance;
        }

        CancellationTokenSource cancelTokenSource;
        delegate void ProgressChanged(int percentage, string filename, PornClassifier.ImageType type);
        ProgressChanged progressChanged;
        private void ProgressChangedMethod(int percentage, string filename, PornClassifier.ImageType type)
        {
            progressBar.Value = percentage;
            currentFileName.Text = filename;
            if (type == PornClassifier.ImageType.Porn)
                bindingSource.Add(new PornFile(filename, type));
        }
        Task scanTask;
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

            //backgroundWorker.RunWorkerAsync();
            cancelTokenSource = new CancellationTokenSource();
            //Progress<int> progress = new Progress<int>();
            //progress.ProgressChanged += progress_ProgressChanged;
            scanTask = Task.Factory.StartNew(() =>
            {
                backgroundWorker_DoWork(cancelTokenSource.Token);
            },
            cancelTokenSource.Token
            );
        }

        private void pornGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void TemporaryFileScan_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (cancelTokenSource != null)
                cancelTokenSource.Cancel();
            try 
            {
                scanTask.Wait();
            }
            catch (AggregateException exception)
            {
                log.Info("AggregateException thrown with the following inner exceptions: " + exception.ToString());
            }
            finally
            {
                cancelTokenSource.Dispose();
            }

            //backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_DoWork(CancellationToken ct)
        {
            try
            {
                String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                log.Info("IE cache path: " + iecachepath);
                var totalfiles = SafeFileEnumerator.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories);
                //Directory.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));

                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dirs = SafeFileEnumerator.EnumerateDirectories(roamingPath, "*Cache*", SearchOption.AllDirectories);

                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                dirs = dirs.Concat(SafeFileEnumerator.EnumerateDirectories(localAppDataPath, "*Cache*", SearchOption.AllDirectories));

                foreach (var dir in dirs)
                    totalfiles = totalfiles.Concat(SafeFileEnumerator.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories));

                int tcount = totalfiles.Count();
                int num = 0;
                foreach (var file in totalfiles)
                {
                    log.Info("Scan: " + file);
                    PornClassifier.ImageType t;
                    try
                    {
                        t = PornClassifier.Instance.Classify(file);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception.ToString());
                        t = PornClassifier.ImageType.Normal;
                    }
                    num++;
                    //backgroundWorker.ReportProgress(100 * num / tcount, new PornFile(file, t));
                    //progressBar.Invoke(backgroundWorker_ProgressChanged, (int)100*num/tcount, file, t);
                    int percentage = 100 * num / tcount;
                    if (percentage > 100)
                        percentage = 100;
                    progressBar.BeginInvoke(progressChanged, percentage, file, t);
                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();
                    //if (backgroundWorker.CancellationPending)
                    //    break;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }

        //private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //private void backgroundWorker_ProgressChanged(int percentage, string file, PornClassifier.ImageType t)
        //{
        //    progressBar.Value = percentage;
        //    currentFileName.Text = file;
        //    if (t == PornClassifier.ImageType.Porn)
        //        bindingSource.Add(file);
        //}
    }
}
