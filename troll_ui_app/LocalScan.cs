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
        public enum ScanType{AllScan, FastScan, CustomScan};
        private int PercentageOffset { set; get; }
        private int PercentageRatio { set; get; }
        public void AllLocalFileScan(CancellationToken ct, IProgress<ScanProgress> progress)
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
        public void FastLocalScan(CancellationToken ct, IProgress<ScanProgress> progress)
        {
            try
            {
                String iecachepath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
                log.Info("IE cache path: " + iecachepath);
                var totalfiles = Directory.EnumerateFiles(iecachepath, "*.*", SearchOption.AllDirectories).Where<String>(s => (s.EndsWith(".jpg") || s.EndsWith(".png")));

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
                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }
        public async Task FastLocalScanAsync(CancellationToken ct, IProgress<ScanProgress> progress)
        {
            await Task.Factory.StartNew(()=> {
                if(progress!=null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备快速扫描";
                    progress.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 100;
                FastLocalScan(ct, progress);
            });
        }
        public async Task AllLocalScanAsync(CancellationToken ct, IProgress<ScanProgress> progress)
        {
            await Task.Factory.StartNew(()=> {
                if(progress!=null)
                {
                    ScanProgress npro = new ScanProgress();
                    npro.Percentage = 0;
                    npro.TargetFilePath = null;
                    npro.Description = "正在准备全盘扫描";
                    progress.Report(npro);
                }
                PercentageOffset = 0;
                PercentageRatio = 30;
                FastLocalScan(ct, progress);
                PercentageOffset = 30;
                PercentageRatio = 70;
                AllLocalFileScan(ct, progress);
            });
            //Task UITask = task.ContinueWith(() =>
            //{
            //    this.TextBlock1.Text = "Complete";
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
