using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    public class LocalScan
    {
        static readonly ILog log = Log.Get();
        public class ScanProgress
        {
            public string Description{get; set;}
            int _percentage;
            public int Percentage
            {
                get
                {
                    return _percentage;
                }
                set
                {
                    //确保进度不会小于0或是大于100，否则会引发progressbar的异常
                    if (value < 0)
                        _percentage = 0;
                    else if (value > 100)
                        _percentage = 100;
                    else
                        _percentage = value;
                }
            }
            public string TargetFilePath { get; set; }
            //public PornClassifier.ImageType TargetType { get; set; }
            public PornDatabase.PornItemType ItemType { get; set; }
            public bool if_from_cache { get; set; }
        }
        public event EventHandler<ScanProgress> ScanProgressChanged;
        public event EventHandler ScanComplete;
        private int PercentageOffset { set; get; }
        private int PercentageRatio { set; get; }
        Task _scanTask;
        public Task ScanTask { get { return _scanTask; } }
        static Func<string, bool> FilesCanProcessed = s => (ActiveFileMonitor.IsFileExtWith(s, ActiveFileMonitor.ImageExts) ||
            ActiveFileMonitor.IsFileExtWith(s, ActiveFileMonitor.VideoExts));
        static Func<string, bool> ImageFilesCanProcessed = s => (ActiveFileMonitor.IsFileExtWith(s, ActiveFileMonitor.ImageExts));
        
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
            log.Info("Stop Current Scan Task!");
        }
        public void StartFastScan(bool incremental)
        {
            DateTime lastDateTime;
            if (incremental)
                lastDateTime = Properties.Settings.Default.lastFastLocalScanDateTime;
            else
                lastDateTime = DateTime.MinValue;
            _scanPauseEvent.Set();
            _scanCancellationTokenSource = new CancellationTokenSource();
            _scanTask = Task.Factory.StartNew(()=> {
                if(_scanProgressReport!=null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备上网记录扫描";
                    npro.if_from_cache = true;
                    _scanProgressReport.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 100;
                FastLocalScan(_scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport, lastDateTime);
            });
            _scanTask.ContinueWith(atask =>
            {
                //如果正常结束，则记录时间
                if(_scanCancellationTokenSource!=null && !_scanCancellationTokenSource.IsCancellationRequested)
                {
                    Properties.Settings.Default.lastFastLocalScanDateTime = DateTime.Now;
                    Properties.Settings.Default.Save();
                }
                if (ScanComplete != null)
                    ScanComplete(this, EventArgs.Empty);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void StartAllScan(bool incremental)
        {
            DateTime lastDateTime;
            if (incremental)
                lastDateTime = Properties.Settings.Default.lastAllLocalScanDateTime;
            else
                lastDateTime = DateTime.MinValue;

            _scanPauseEvent.Set();
            _scanCancellationTokenSource = new CancellationTokenSource();
            _scanTask = Task.Factory.StartNew(()=> {
                if (_scanProgressReport != null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备全盘扫描";
                    _scanProgressReport.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 30;
                FastLocalScan(_scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport, lastDateTime);
                PercentageOffset = 30;
                PercentageRatio = 70;
                AllLocalFileScan(_scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport, lastDateTime);
            });
            _scanTask.ContinueWith(atask =>
            {
                //如果正常结束，则记录时间
                if(_scanCancellationTokenSource!=null && !_scanCancellationTokenSource.IsCancellationRequested)
                {
                    //浏览器缓存和全盘文件都记录当前时间
                    Properties.Settings.Default.lastAllLocalScanDateTime = DateTime.Now;
                    Properties.Settings.Default.Save();
                }
                if (ScanComplete != null)
                    ScanComplete(this, EventArgs.Empty);
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
        public void StartCustomScan(string dir)
        {
            _scanPauseEvent.Set();
            _scanCancellationTokenSource = new CancellationTokenSource();
            _scanTask = Task.Factory.StartNew(()=> {
                if(_scanProgressReport!=null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备自定义扫描";
                    _scanProgressReport.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 100;
                CustomLocalScan(dir, _scanCancellationTokenSource.Token, _scanPauseEvent, _scanProgressReport);
            });
            _scanTask.ContinueWith(atask =>
            {
                if (ScanComplete != null)
                    ScanComplete(this, EventArgs.Empty);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        static List<string> sBrowserDirList;
        static bool removeBrowserDirs(string d)
        {
            foreach(string dir in sBrowserDirList)
            {
                if (d.Contains(dir))
                    return false;
            }
            return true;
        }
        //所有本地文件扫描，排除了浏览器缓存文件以防重复扫描
        void AllLocalFileScan(CancellationToken ct, ManualResetEvent pauseEvent, IProgress<ScanProgress> progress, DateTime lastDateTime)
        {
            try
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                //List<string> allTopDirs = new List<string>();
                //先扫描所有磁盘根目录下的文件
                IEnumerable<string> allTopDirs = Enumerable.Empty<string>();
                foreach(var drive in allDrives)
                {
                    if(drive.DriveType == DriveType.Fixed)
                    {
                        allTopDirs = allTopDirs.Concat(SafeFileEnumerator.EnumerateDirectories(
                            drive.RootDirectory.FullName, "*", SearchOption.TopDirectoryOnly, null
                            ));
                        //var files = SafeFileEnumerator.EnumerateFiles(drive.RootDirectory.FullName, "*.*", SearchOption.TopDirectoryOnly, null).Where<String>(FilesCanProcessed);
                        var fileinfos = SafeFileEnumerator.EnumerateFileInfos(drive.RootDirectory.FullName, "*.*", SearchOption.TopDirectoryOnly, null).Where<FileInfo>(finfo => FilesCanProcessed(finfo.Name));
                        if(lastDateTime != DateTime.MinValue)
                            fileinfos = fileinfos.Where<FileInfo>(finfo => finfo.CreationTime > lastDateTime);
                        //if (Properties.Settings.Default.isAllLocalScanIncremental)
                        //    fileinfos = fileinfos.Where<FileInfo>(finfo => finfo.CreationTime > Properties.Settings.Default.lastAllLocalScanDateTime);

                        foreach(FileInfo finfo in fileinfos)
                        {
                            AnalysisFile(finfo.FullName, ct, pauseEvent, progress, 0, 1, false, false);
                        }
                    }
                }

                String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                List<string> allTopDirsList = allTopDirs.ToList();
                int tcount = allTopDirsList.Count;
                int num = 0;
                //再分别扫描所有磁盘下根文件夹下所有文件
                foreach(var topdir in allTopDirsList)
                {
                    //var allfiles = SafeFileEnumerator.EnumerateFiles(topdir, "*.*", SearchOption.AllDirectories,
                    //    d => !(d.Contains(iecachepath) || d.Contains(localAppDataPath))
                    //    ).Where<String>(FilesCanProcessed);
                    //排除浏览器缓存搜索的范围
                    string homePath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                    //var allFileInfos = SafeFileEnumerator.EnumerateFileInfos(topdir, "*.*", SearchOption.AllDirectories,
                    //    d => !(d.Contains(iecachepath) || d.Contains(homePath))
                    //    ).Where<FileInfo>(finfo => FilesCanProcessed(finfo.Name));
                    var allFileInfos = SafeFileEnumerator.EnumerateFileInfos(topdir, "*.*", SearchOption.AllDirectories,
                        removeBrowserDirs
                        ).Where<FileInfo>(finfo => FilesCanProcessed(finfo.Name));

                    if (lastDateTime != DateTime.MinValue)
                        allFileInfos = allFileInfos.Where<FileInfo>(finfo => finfo.CreationTime > lastDateTime);
                    //if (Properties.Settings.Default.isAllLocalScanIncremental)
                    //    allFileInfos = allFileInfos.Where<FileInfo>(finfo => finfo.CreationTime > Properties.Settings.Default.lastAllLocalScanDateTime);

                    foreach(FileInfo finfo in allFileInfos)
                    {
                        AnalysisFile(finfo.FullName, ct, pauseEvent, progress, num, tcount, false, false);
                    }
                    num++;
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        //扫描本地浏览器缓存图片文件
        void FastLocalScan(CancellationToken ct, ManualResetEvent pauseEvent, IProgress<ScanProgress> progress, DateTime lastDateTime)
        {
            try
            {
                String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                log.Info("IE cache path: " + iecachepath);
                //var totalfiles = Directory.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));
                //var totalfiles = SafeFileEnumerator.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(ImageFilesCanProcessed);
                //var totalfiles = SafeFileEnumerator.EnumerateFileInfos(iecachepath, "*.*", SearchOption.AllDirectories).Where<FileInfo>(ImageFilesCanProcessed);

                //var totalfiles = SafeFileEnumerator.EnumerateFileInfos(iecachepath, "*.*", SearchOption.AllDirectories).Where<FileInfo>(ImageFileInfosCanProcessed);
                var totalfiles = Enumerable.Empty<FileInfo>();

                //此处不能使用directoryfilter，因为该filter是控制对那些dir枚举其下的dir，而不是文件
                //这会导致在win8下，ie的缓存目录为INetCache，该目录下的文件会被重复枚举并扫描
                //string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                //                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                //    ? Environment.GetEnvironmentVariable("HOME")
                //    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                string homePath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                var dirs = SafeFileEnumerator.EnumerateDirectories(homePath, "*Cache*", SearchOption.AllDirectories).Where<String>(s => !s.Contains(iecachepath));

                //string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                //var dirs = SafeFileEnumerator.EnumerateDirectories(roamingPath, "*Cache*", SearchOption.AllDirectories).Where<String>(s=>!s.Contains(iecachepath));

                //string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //dirs = dirs.Concat(SafeFileEnumerator.EnumerateDirectories(localAppDataPath, "*Cache*", SearchOption.AllDirectories).Where<String>(s=>!s.Contains(iecachepath)));
                List<string> cachedirs = dirs.ToList();
                List<string> tmpcachedirs = cachedirs;
                cachedirs.Add(iecachepath);
                //剔除路径重叠的目录，留下最高层的目录，防止重复扫描
                cachedirs.RemoveAll(s => tmpcachedirs.Exists(ss => ss != s && s.Contains(ss)));
                
                //枚举每个目录内的文件
                foreach (var dir in cachedirs)
                {
                    log.Info("Fast dir: " + dir);
                    totalfiles = totalfiles.Concat(SafeFileEnumerator.EnumerateFileInfos(dir, "*.*", SearchOption.AllDirectories));
                }

                sBrowserDirList = cachedirs;
                //sBrowserDirList= dirs.ToList();
                //sBrowserDirList.Add(iecachepath);

                //var files = GetFiles("C:\\SearchDirectory", d => !d.Contains("AvoidMe", StringComparison.OrdinalIgnoreCase), "*.*");
                //if(Properties.Settings.Default.isFastLocalScanIncremental)
                //    totalfiles = totalfiles.Where<FileInfo>(finfo => finfo.CreationTime > Properties.Settings.Default.lastFastLocalScanDateTime);
                if(lastDateTime != DateTime.MinValue)
                    totalfiles = totalfiles.Where<FileInfo>(finfo => finfo.CreationTime > lastDateTime);

                int tcount = totalfiles.Count();
                int num = 0;
                foreach (var file in totalfiles)
                {
                    AnalysisFile(file.FullName, ct, pauseEvent, progress, num, tcount, true, true);
                    num++;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }
        void CustomLocalScan(string dir, CancellationToken ct, ManualResetEvent pauseEvent, IProgress<ScanProgress> progress)
        {
            try
            {
                //要忽略程序自身的缓存
                string localTempDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\masatek\\trollwiz";
                var totalfiles = SafeFileEnumerator.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories,
                    d => !(d.Contains(localTempDir))
                    ).Where<String>(FilesCanProcessed);
                int tcount = totalfiles.Count();
                int num = 0;
                foreach (var file in totalfiles)
                {
                    AnalysisFile(file, ct, pauseEvent, progress, num, tcount, false, false);
                    num++;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }

        void AnalysisFile(string file, CancellationToken ct, ManualResetEvent pauseEvent, IProgress<ScanProgress> progress, int num, int tcount, bool onlyImage, bool if_fast_scan)
        {
            log.Info("Scan: " + file);
            Exception classifyException;
            PornDatabase.PornItemType itype = PornDatabase.PornItemType.Undefined;
            if (onlyImage || ActiveFileMonitor.IsFileExtWith(file, ActiveFileMonitor.ImageExts))
            {
                if (PornClassifier.Instance.Classify(file, out classifyException) == PornClassifier.ImageType.Porn)
                    itype = PornDatabase.PornItemType.LocalImage;
            }
            else if (ActiveFileMonitor.IsFileExtWith(file, ActiveFileMonitor.VideoExts))
            {
                if (PornClassifier.Instance.ClassifyVideoFile(file, out classifyException))
                    itype = PornDatabase.PornItemType.LocalVideo;
            }
            if (progress != null)
            {
                ScanProgress npro = new ScanProgress();
                npro.Percentage = PercentageOffset + PercentageRatio * num / tcount;
                npro.TargetFilePath = file;
                npro.Description = "正在扫描：" + npro.TargetFilePath;
                npro.ItemType = itype;
                npro.if_from_cache = if_fast_scan;
                progress.Report(npro);
            }
            pauseEvent.WaitOne();
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();
        }
    }
}
