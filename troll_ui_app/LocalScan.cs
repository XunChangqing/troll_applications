using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace troll_ui_app
{
    public class LocalScan
    {
        static readonly ILog log = Log.Get();
        public class ScanProgress
        {
            public string Description{get; set;}
            public int Percentage { get; set; }
            public string TargetFilePath { get; set; }
            public PornClassifier.ImageType TargetType { get; set; }
        }
        public event EventHandler<ScanProgress> ScanProgressChanged;
        public event EventHandler ScanComplete;
        private int PercentageOffset { set; get; }
        private int PercentageRatio { set; get; }
        Task _scanTask;
        public Task ScanTask { get { return _scanTask; } }
        
        Progress<ScanProgress> _scanProgress;
        IProgress<ScanProgress> _scanProgressReport;
        ManualResetEvent _scanPauseEvent;
        CancellationTokenSource _scanCancellationTokenSource;
        //ScanStatus _scanStatus;
        public LocalScan()
        {
            _scanPauseEvent = new ManualResetEvent(true);
            _scanProgress = new Progress<ScanProgress>();
            _scanProgressReport = _scanProgress;
            _scanProgress.ProgressChanged += _scanProgressOnProgressChanged;
        }
        void _scanProgressOnProgressChanged(object sender, LocalScan.ScanProgress e)
        {
            if (ScanProgressChanged != null)
                ScanProgressChanged(sender, e);
        }
        public void Pause()
        {
            _scanPauseEvent.Reset();
        }
        public void Resume()
        {
            _scanPauseEvent.Set();
        }
        public void Stop()
        {
            _scanPauseEvent.Set();
            _scanCancellationTokenSource.Cancel();
            _scanTask.Wait();
        }
        public void StartFastScan()
        {
            _scanPauseEvent.Set();
            _scanCancellationTokenSource = new CancellationTokenSource();
            _scanTask = Task.Factory.StartNew(()=> {
                if(_scanProgressReport!=null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备快速扫描";
                    _scanProgressReport.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 100;
                FastLocalScan(_scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport);
            });
            _scanTask.ContinueWith(atask =>
            {
                if (ScanComplete != null)
                    ScanComplete(this, EventArgs.Empty);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void StartAllScan()
        {
            _scanPauseEvent.Set();
            _scanCancellationTokenSource = new CancellationTokenSource();
            _scanTask = Task.Factory.StartNew(()=> {
                if(_scanProgressReport!=null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备全盘扫描";
                    _scanProgressReport.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 30;
                FastLocalScan(_scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport);
                PercentageOffset = 30;
                PercentageRatio = 70;
                AllLocalFileScan(_scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport);
            });
            _scanTask.ContinueWith(atask =>
            {
                if (ScanComplete != null)
                    ScanComplete(this, EventArgs.Empty);
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
        void AllLocalFileScan(CancellationToken ct, ManualResetEvent pauseEvent, IProgress<ScanProgress> progress)
        {
            try
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                //List<string> allTopDirs = new List<string>();
                IEnumerable<string> allTopDirs = Enumerable.Empty<string>();
                foreach(var drive in allDrives)
                {
                    if(drive.DriveType == DriveType.Fixed)
                    {
                        allTopDirs = allTopDirs.Concat(SafeFileEnumerator.EnumerateDirectories(
                            drive.RootDirectory.FullName, null, SearchOption.TopDirectoryOnly, null
                            ));
                        var files = SafeFileEnumerator.EnumerateFiles(drive.RootDirectory.FullName, "*.*", SearchOption.TopDirectoryOnly, null).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));
                        foreach(string file in files)
                        {
                            PornClassifier.ImageType t = PornClassifier.Instance.Classify(file);
                            if (progress != null)
                            {
                                ScanProgress npro = new ScanProgress();
                                npro.Percentage = PercentageOffset;
                                npro.TargetFilePath = file;
                                npro.Description = "正在扫描：" + npro.TargetFilePath;
                                npro.TargetType = t;
                                progress.Report(npro);
                            }
                            pauseEvent.WaitOne();
                            if (ct.IsCancellationRequested)
                                ct.ThrowIfCancellationRequested();
                        }
                    }
                }

                String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                List<string> allTopDirsList = allTopDirs.ToList();
                int tcount = allTopDirsList.Count;
                int num = 0;
                foreach(var topdir in allTopDirsList)
                {
                    var allfiles = SafeFileEnumerator.EnumerateFiles(topdir, "*.*", SearchOption.AllDirectories,
                        d => !(d.Contains(iecachepath) || d.Contains(localAppDataPath))
                        ).Where<String>(s => (s.EndsWith("*.jpg") || s.EndsWith("*.png")));

                    foreach(string file in allfiles)
                    {
                        PornClassifier.ImageType t = PornClassifier.Instance.Classify(file);
                        if(progress != null)
                        {
                            ScanProgress npro = new ScanProgress();
                            npro.Percentage = PercentageOffset + PercentageRatio * num / tcount;
                            npro.TargetFilePath = file;
                            npro.Description = "正在扫描："+npro.TargetFilePath;
                            npro.TargetType = t;
                            progress.Report(npro);
                        }
                        if (ct.IsCancellationRequested)
                            ct.ThrowIfCancellationRequested();
                    }
                    num++;
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        void FastLocalScan(CancellationToken ct, ManualResetEvent pauseEvent, IProgress<ScanProgress> progress)
        {
            try
            {
                String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                log.Info("IE cache path: " + iecachepath);
                //var totalfiles = Directory.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));
                var totalfiles = SafeFileEnumerator.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));

                string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dirs = SafeFileEnumerator.EnumerateDirectories(roamingPath, "*Cache*", SearchOption.AllDirectories);

                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                dirs = dirs.Concat(SafeFileEnumerator.EnumerateDirectories(localAppDataPath, "*Cache*", SearchOption.AllDirectories));
                foreach (var dir in dirs)
                    totalfiles = totalfiles.Concat(SafeFileEnumerator.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories));

                var browserDirs = dirs.ToList();
                browserDirs.Add(iecachepath);

                //var files = GetFiles("C:\\SearchDirectory", d => !d.Contains("AvoidMe", StringComparison.OrdinalIgnoreCase), "*.*");

                int tcount = totalfiles.Count();
                int num = 0;
                foreach (var file in totalfiles)
                {
                    log.Info("Scan: " + file);
                    PornClassifier.ImageType t = PornClassifier.Instance.Classify(file);
                    num++;
                    if (progress != null)
                    {
                        ScanProgress npro = new ScanProgress();
                        npro.Percentage = PercentageOffset + PercentageRatio * num / tcount;
                        npro.TargetFilePath = file;
                        npro.Description = "正在扫描："+npro.TargetFilePath;
                        npro.TargetType = t;
                        progress.Report(npro);
                    }
                    pauseEvent.WaitOne();
                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }

    }
}
