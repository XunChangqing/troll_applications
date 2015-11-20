using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using log4net;

namespace troll_ui_app
{
    public class ActiveFileMonitor : IDisposable
    {
        static readonly ILog log = Log.Get();
        public class PornActiveFile
        {
            public string Path { get; set; }
        }
        public delegate void PornDetectedEventHandler(object sender, PornActiveFile e);
        public event PornDetectedEventHandler PornDetected;

        ConcurrentDictionary<string, int> _fileProcessed = new ConcurrentDictionary<string, int>();
        ConcurrentQueue<string> _taskQueue = new ConcurrentQueue<string>();
        AutoResetEvent _newFileEvent = new AutoResetEvent(true);
        Task _backWorker;
        CancellationTokenSource _cts = new CancellationTokenSource();
        Progress<PornActiveFile> _progress = new Progress<PornActiveFile>();
        //FileSystemWatcher _fileSystemWatcher;
        public static string []ImageExts = {".jpg", ".png"};
        public static string []VideoExts = { ".mkv", ".flv", ".vob", ".ogg", ".ogv", ".avi", ".mov", ".qt", ".wmv", ".rm", ".rmvb", ".asf", ".mp4", ".m4p", ".m4v", ".mpg", ".mpeg", ".m4v", ".3gp", ".3g2", ".mxf" };
        public static bool IsFileExtWith(string fname, string[] exts)
        {
            foreach(string ext in exts)
            {
                if (fname.EndsWith(ext))
                    return true;
            }
            return false;
        }
        //图像
        List<FileSystemWatcher> _imageFileSystemWatchers;
        //视频
        List<FileSystemWatcher> _videoFileSystemWatchers;

        List<string> _dirFilters;
        MD5 _md5hash;

        private bool DirFilter(string path)
        {
            foreach (string filter in _dirFilters)
            {
                if (path.Contains(filter))
                    return true;
            }
            return false;
        }

        private void InitWatchers(string[] filters, out List<FileSystemWatcher> ofsWatchers)
        {
            DriveInfo[] allDrive = DriveInfo.GetDrives();
            ofsWatchers = new List<FileSystemWatcher>();
            foreach (DriveInfo dinfo in allDrive)
            {
                if (dinfo.DriveType == DriveType.Fixed)
                {
                    try
                    {
                        foreach (string filter in filters)
                        {
                            FileSystemWatcher fsWatcher = new FileSystemWatcher(dinfo.RootDirectory.ToString());
                            //fsWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                            //_fileSystemWatcher.NotifyFilter = | NotifyFilters.Size;
                            //NotifyFilters.
                            fsWatcher.Filter = "*" + filter;
                            fsWatcher.IncludeSubdirectories = true;
                            fsWatcher.Created += _fileSystemWatcherOnChanged;
                            fsWatcher.Changed += _fileSystemWatcherOnChanged;
                            //fsWatcher.Deleted += _fileSystemWatcherOnDeleted;
                            //fsWatcher.Renamed += _fileSystemWatcherOnRenamed;

                            ofsWatchers.Add(fsWatcher);
                        }
                    }
                    catch(Exception exp)
                    {
                        log.Error(exp.ToString());
                    }
                }
            }
        }
        //private void InitWatchers(string filter, out List<FileSystemWatcher> ofsWatchers)
        //{
        //    DriveInfo[] allDrive = DriveInfo.GetDrives();
        //    ofsWatchers = new List<FileSystemWatcher>();
        //    foreach (DriveInfo dinfo in allDrive)
        //    {
        //        if (dinfo.DriveType == DriveType.Fixed)
        //        {
        //            try
        //            {
        //                FileSystemWatcher fsWatcher = new FileSystemWatcher(dinfo.RootDirectory.ToString());
        //                //fsWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        //                //_fileSystemWatcher.NotifyFilter = | NotifyFilters.Size;
        //                //NotifyFilters.
        //                fsWatcher.Filter = filter;
        //                fsWatcher.IncludeSubdirectories = true;
        //                fsWatcher.Created += _fileSystemWatcherOnChanged;
        //                fsWatcher.Changed += _fileSystemWatcherOnChanged;
        //                //fsWatcher.Deleted += _fileSystemWatcherOnDeleted;
        //                //fsWatcher.Renamed += _fileSystemWatcherOnRenamed;

        //                ofsWatchers.Add(fsWatcher);
        //            }
        //            catch(Exception exp)
        //            {
        //                log.Error(exp.ToString());
        //            }
        //        }
        //    }
        //}
        private void EnableWatchers(List<FileSystemWatcher> watchers)
        {
            foreach (FileSystemWatcher w in watchers)
            {
                try
                {
                    w.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }
        }
        private void DisableWatchers(List<FileSystemWatcher> watchers)
        {
            foreach (FileSystemWatcher w in watchers)
            {
                try
                {
                    w.EnableRaisingEvents = false;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }
        }
        public ActiveFileMonitor()
        {
            _dirFilters = new List<string>();
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.CDBurning));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
            _dirFilters.Add(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            _dirFilters.Add("$");

            DriveInfo[] allDrive = DriveInfo.GetDrives();
            //InitWatchers("*.jpg", out _jpgFileSystemWatchers);
            //InitWatchers("*.png", out _pngFileSystemWatchers);
            InitWatchers(ImageExts, out _imageFileSystemWatchers);
            InitWatchers(VideoExts, out _videoFileSystemWatchers);
            //_md5hash = MD5.Create();

            //start back worker
            _backWorker = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        BackgroundWorker(_cts.Token, _progress);
                    }
                    catch (TaskCanceledException tce)
                    {
                        log.Info("Active File Monitor Backgroud Worker Task Canceled: "+tce.ToString());
                    }
                    catch(Exception exp)
                    {
                        log.Info("Active File Monitor Backgroud Worker Task Error: "+exp.ToString());
                    }
                });
        }
        public void Dispose()
        {
            _cts.Cancel();
            _backWorker.Wait();
            //_backWorker.Wait(2000);
            log.Info("Dispose Active File Monitor!");
        }
        //~ActiveFileMonitor()
        //{
        //}
        public void EnableImageDetection()
        {
            EnableWatchers(_imageFileSystemWatchers);
        }
        public void DisableImageDetection()
        {
            DisableWatchers(_imageFileSystemWatchers);
        }
        public void EnableVideoDetection()
        {
            EnableWatchers(_videoFileSystemWatchers);
        }
        public void DisableVideoDetection()
        {
            DisableWatchers(_videoFileSystemWatchers);
        }
        void _fileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (DirFilter(e.FullPath))
                return;
            log.Info(e.ChangeType.ToString() + e.FullPath);
            _taskQueue.Enqueue(e.FullPath);
            _newFileEvent.Set();
        }
        void _fileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            if (DirFilter(e.FullPath))
                return;
            //从数据库中删除旧文件
            //将新文件送入分类器，为保持最新，这里有可能对新文件进行了重复分类
            log.Info("File Renamed: " + e.OldFullPath + " to " + e.FullPath);
            _taskQueue.Enqueue(e.FullPath);
            _newFileEvent.Set();
        }
        void _fileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            if (DirFilter(e.FullPath))
                return;
            int dummySize;
            _fileProcessed.TryRemove(e.FullPath, out dummySize);
            //从数据库中删除
            //PornDatabase pdb = new PornDatabase();
            //pdb.DeletePornFile(e.FullPath, PornDatabase.PornItemType.LocalImage);
            log.Info("Deleted: " + e.FullPath);
        }
        string GetMd5Hash(MD5 md5Hash, byte[] input)
        {
            // Convert the input string to a byte array and compute the hash.
            //byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            byte[] data = md5Hash.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        void BackgroundWorker(CancellationToken ct, IProgress<PornActiveFile> progress)
        {
            Dictionary<string, PornDatabase.PornItemType> md5Set = new Dictionary<string, PornDatabase.PornItemType>();
            //Dictionary<byte[], int> md5Set = new Dictionary<byte[], int>();
            Dictionary<string, int> fileSet = new Dictionary<string,int>();
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                if (_taskQueue.IsEmpty)
                    _newFileEvent.WaitOne(1000);
                string nfile;
                while (_taskQueue.TryDequeue(out nfile))
                {
                    fileSet[nfile] = 0;
                }
                List<string> tobeDeleted = new List<string>();
                foreach (string key in fileSet.Keys.ToList())
                {
                    try
                    {
                        log.Info("Process: " + key);
                        FileAttributes attr = File.GetAttributes(key);
                        if (!attr.HasFlag(FileAttributes.Directory))
                        {
                            FileInfo finfo = new FileInfo(key);
                            if (!_fileProcessed.ContainsKey(key) || _fileProcessed[key] != finfo.Length)
                            {
                                //string shash = GetMd5Hash(_md5hash, File.ReadAllBytes(key));
                                //log.Info("\tHash: " + shash + " Time: " + fileSet[key]);
                                //PornClassifier.ImageType itype;
                                PornDatabase.PornItemType itype = PornDatabase.PornItemType.Undefined;
                                //if (md5Set.ContainsKey(shash))
                                //{
                                //    itype = md5Set[shash];
                                //}
                                //else
                                {
                                    Exception classifyException;
                                    if (IsFileExtWith(key, ImageExts))
                                    {
                                        var t = PornClassifier.Instance.Classify(key, out classifyException);
                                        if (classifyException != null)
                                            throw classifyException;
                                        else if (t == PornClassifier.ImageType.Porn)
                                        {
                                            itype = PornDatabase.PornItemType.LocalImage;
                                            //md5Set[shash] = PornDatabase.PornItemType.LocalImage;
                                        }
                                    }
                                    else if(IsFileExtWith(key, VideoExts))
                                    {
                                        var t = PornClassifier.Instance.ClassifyVideoFile(key, out classifyException);
                                        if (classifyException != null)
                                            throw classifyException;
                                        else if (t)
                                        {
                                            itype = PornDatabase.PornItemType.LocalVideo;
                                            //md5Set[shash] = PornDatabase.PornItemType.LocalVideo;
                                        }
                                    }
                                }
                                IProgress<PornDatabase.PornItemType> ip = MainForm.Instance.TargetProcessedProgress as IProgress<PornDatabase.PornItemType>;
                                if(itype != PornDatabase.PornItemType.Undefined)
                                {
                                    log.Info("Detect Active File: " + key);
                                    PornDatabase pdb = new PornDatabase();
                                    pdb.InsertPornFile(key, itype);
                                    ip.Report(itype);
                                }
                                else
                                    ip.Report(PornDatabase.PornItemType.Undefined);

                                _fileProcessed[key] = (int)finfo.Length;
                            }
                            else
                            {
                                log.Info("\tIgnore same file: " + key);
                            }
                        }
                        tobeDeleted.Add(key);
                    }
                    //catch (System.IO.IOException ioex)
                    catch (Exception ex)
                    {
                        int value = fileSet[key];
                        if (value + 1 > 10)
                        {
                            tobeDeleted.Add(key);
                            log.Info("Delete: " + key);
                        }
                        else
                            fileSet[key] = value + 1;
                        log.Info(ex.ToString());
                    }
                    //{
                    //    log.Error(ex.ToString());
                    //}
                }
                foreach(string del in tobeDeleted)
                    fileSet.Remove(del);
            }
        }
    }
}
