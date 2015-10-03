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
    class ActiveFileMonitor
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
        List<FileSystemWatcher> _jpgFileSystemWatchers;
        List<FileSystemWatcher> _pngFileSystemWatchers;
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

        private void InitWatchers(string filter, out List<FileSystemWatcher> ofsWatchers)
        {
            DriveInfo[] allDrive = DriveInfo.GetDrives();
            ofsWatchers = new List<FileSystemWatcher>();
            foreach (DriveInfo dinfo in allDrive)
            {
                if (dinfo.DriveType == DriveType.Fixed)
                {
                    FileSystemWatcher fsWatcher = new FileSystemWatcher(dinfo.RootDirectory.ToString());
                    //fsWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                    //_fileSystemWatcher.NotifyFilter = | NotifyFilters.Size;
                    //NotifyFilters.
                    fsWatcher.Filter = filter;
                    fsWatcher.IncludeSubdirectories = true;
                    fsWatcher.Created += _fileSystemWatcherOnChanged;
                    fsWatcher.Changed += _fileSystemWatcherOnChanged;
                    fsWatcher.Deleted += _fileSystemWatcherOnDeleted;
                    fsWatcher.Renamed += _fileSystemWatcherOnRenamed;

                    ofsWatchers.Add(fsWatcher);
                }
            }
        }
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
                    Console.WriteLine(ex.ToString());
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
                    Console.WriteLine(ex.ToString());
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
            InitWatchers("*.jpg", out _jpgFileSystemWatchers);
            InitWatchers("*.png", out _pngFileSystemWatchers);
            _md5hash = MD5.Create();

            //start back worker
            _backWorker = Task.Factory.StartNew(() =>
                {
                    BackgroundWorker(_cts.Token, _progress);
                });
        }
        ~ActiveFileMonitor()
        {
            _cts.Cancel();
            _backWorker.Wait(2000);
        }
        public void EnableImageDetection()
        {
            EnableWatchers(_jpgFileSystemWatchers);
            EnableWatchers(_pngFileSystemWatchers);
        }
        public void DisableImageDetection()
        {
            DisableWatchers(_jpgFileSystemWatchers);
            DisableWatchers(_pngFileSystemWatchers);
        }
        void _fileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            if (DirFilter(e.FullPath))
                return;
            Console.WriteLine(e.ChangeType.ToString()+e.FullPath);
            _taskQueue.Enqueue(e.FullPath);
            _newFileEvent.Set();
        }
        void _fileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            if (DirFilter(e.FullPath))
                return;
            //从数据库中删除旧文件
            //将新文件送入分类器，为保持最新，这里有可能对新文件进行了重复分类
            Console.WriteLine("File Renamed: " + e.OldFullPath + " to " + e.FullPath);
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
            PornDatabase pdb = new PornDatabase();
            pdb.DeletePornFile(e.FullPath, PornDatabase.PornItemType.LocalImage);
            Console.WriteLine("Deleted: " + e.FullPath);
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
            Dictionary<string, int> md5Set = new Dictionary<string, int>();
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
                        Console.WriteLine("Process: " + key);
                        FileAttributes attr = File.GetAttributes(key);
                        if (!attr.HasFlag(FileAttributes.Directory))
                        {
                            FileInfo finfo = new FileInfo(key);
                            if (!_fileProcessed.ContainsKey(key) || _fileProcessed[key] != finfo.Length)
                            {
                                string shash = GetMd5Hash(_md5hash, File.ReadAllBytes(key));
                                Console.WriteLine("\tHash: " + shash + " Time: " + fileSet[key]);
                                PornClassifier.ImageType itype;
                                if(md5Set.ContainsKey(shash))
                                    itype = (PornClassifier.ImageType)md5Set[shash];
                                else
                                {
                                    itype = PornClassifier.Instance.Classify(key);
                                    md5Set[shash] = (int)itype;
                                }
                                if(itype == PornClassifier.ImageType.Porn)
                                {
                                    PornDatabase pdb = new PornDatabase();
                                    pdb.InsertPornFile(key, PornDatabase.PornItemType.LocalImage);
                                }
                                _fileProcessed[key] = (int)finfo.Length;
                            }
                            else
                            {
                                Console.WriteLine("\tIgnore same file: " + key);
                            }
                        }
                        tobeDeleted.Add(key);
                    }
                    catch (System.IO.IOException ioex)
                    {
                        int value = fileSet[key];
                        if (value + 1 > 5)
                        {
                            tobeDeleted.Add(key);
                            Console.WriteLine("Delete: " + key);
                        }
                        else
                            fileSet[key] = value + 1;
                        Console.WriteLine(ioex.ToString());
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.ToString());
                    }
                }
                foreach(string del in tobeDeleted)
                    fileSet.Remove(del);
            }
        }
    }
}
