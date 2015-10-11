using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using System.Reflection;
using System.Net.Mail;
using TrotiNet;
using Titanium.Web.Proxy.Helpers;
using System.Drawing;

namespace troll_ui_app
{
    static class Program
    {
        static readonly ILog log = Log.Get();
        public static bool FirstTime = false;
        public static string AppLocalDir;
        /// <summary>
        /// 应用程序的主入口点。

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern int RegisterApplicationRestart(
            [MarshalAs(UnmanagedType.BStr)] string commandLineArgs,
            int flags);

        static System.Threading.Timer delete_history_timer;
        static System.Threading.Timer update_domain_list_timer;
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            //Bitmap nbm = new Bitmap(400, 400);
            //Graphics g = Graphics.FromImage(nbm);
            //SolidBrush solidBrush = new SolidBrush(Color.Red);
            //g.FillRectangle(solidBrush, 0, 0, nbm.Width, nbm.Height);
            //SolidBrush stringBrush = new SolidBrush(Color.Yellow);
            //g.DrawString("山妖卫士", new Font("微软雅黑", nbm.Width/10, GraphicsUnit.Pixel), stringBrush, new Point(0, 0));
            //g.Flush();
            //nbm.Save("test.jpg");
            //return;
            //for (int i = 0; i < 1000;i++ )
            //{
            //    //pdb.InsertPornPic("http://www.ifeng.com/yxz.jpg", PornClassifier.ImageType.Porn);
            //    PornDatabase pdb = new PornDatabase();
            //}
            //return;
            //FileSystemWatcher watcher = new FileSystemWatcher();
            //LocalScan.LocalScanWork();
            //SystemProxyHelper.EnableProxyHTTP("127.0.0.1", 8090);
            //FireFoxHelper.AddFirefox();
            //string x = Console.ReadLine();
            //SystemProxyHelper.DisableAllProxy();
            //FireFoxHelper.RemoveFirefox();
            //return;
            try
            {
                //when uninstall send email and return
                if (args.Contains("-uninstall"))
                {
                    //NotificationRoutines.SendUninstallNotification().Wait();
                    return;
                }
                RegisterApplicationRestart(null, 0);
                //Init();
                InitLogAndDirs();

                bool result;
                //using named system mutex to ensure single instance of application
                var mutex = new System.Threading.Mutex(true, "masa_troll_guard_mutex", out result);

                if (!result)
                {
                    log.Error("Exit due to another running instance!");
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                bool bindingSuccess = true;
                if (Properties.Settings.Default.openid == "" ||
                    Properties.Settings.Default.userNickname == "")
                {
                    WechatForm bindingForm = new WechatForm();
                    //bindingForm.ShowDialog();
                    //使用以下一行也可以
                    Application.Run(bindingForm);
                    bindingSuccess = bindingForm.BindingSuccess;
                }
                if (bindingSuccess)
                {
                    InitForBusinessLogic();
                    Application.ApplicationExit += OnApplicationExit;
                    MainForm mainform = new MainForm(args);
                    Application.Run(mainform);
                    CleanUp();
                }
                log.Info("Exit from Main!");
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
        static private void InitLogAndDirs()
        {
            //change workdir to the path of executable
#if !DEBUG
            var fi = new FileInfo(Application.ExecutablePath);
            Directory.SetCurrentDirectory(fi.DirectoryName);
#endif
#if DEBUG
            AllocConsole();
#endif
            //var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            //var companyName = versionInfo.CompanyName;
            AppLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/masatek/trollwiz/";
            //upgrade user settings
            Utils.Log_Init();
            //#if !DEBUG
            if (!Properties.Settings.Default.upgraded)
            {
                log.Info("Upgrade settings!");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.upgraded = true;
                Properties.Settings.Default.Save();
                PornDatabase.DeleteOldDatabase();
            }
            if (Properties.Settings.Default.firstTime)
            {
                FirstTime = true;
                Properties.Settings.Default.firstTime = false;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.guid == "")
            {
                Properties.Settings.Default.guid = System.Guid.NewGuid().ToString();
                Properties.Settings.Default.Save();
            }

            //create directory for work
            if (!Directory.Exists(Program.AppLocalDir))
                Directory.CreateDirectory(Program.AppLocalDir);
            //create directory for images
            if (!Directory.Exists(Program.AppLocalDir + Properties.Settings.Default.imagesDir))
                Directory.CreateDirectory(Program.AppLocalDir + Properties.Settings.Default.imagesDir);
            //create directory for updates
            if (!Directory.Exists(Program.AppLocalDir + Properties.Settings.Default.updateDir))
                Directory.CreateDirectory(Program.AppLocalDir + Properties.Settings.Default.updateDir);
        }
        static void InitForBusinessLogic()
        {
            //如果数据库文件不存在，则建立数据库文件
            PornDatabase.CreateDatabase();
            PornClassifier.Init();

            update_domain_list_timer = new System.Threading.Timer(PornDatabase.UpdateDatabase, null, new TimeSpan(0, 0, 5), new TimeSpan(4, 0, 0));
            delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0, 1, 0), System.Threading.Timeout.InfiniteTimeSpan);
        }
        static bool _cleanUp = false;
        public static void CleanUp()
        {
            try
            {
                if (_cleanUp)
                    return;
                //unset proxy again to make sure
//#if !DEBUG
                //try
                //{
                //    log.Info("Disable proxy!");
                //    SystemProxyHelper.DisableAllProxyWithourRestore();
                //    FireFoxHelper.RemoveFirefox();
                //}
                //catch (Exception exp) { log.Error(exp.ToString()); }
//#endif
                //dispose timer and wait for callback complete
                WaitHandle[] whs = new WaitHandle[]{
                new AutoResetEvent(false),
                new AutoResetEvent(false) };
                //WaitHandle wh = new AutoResetEvent(false);
                delete_history_timer.Dispose(whs[0]);
                update_domain_list_timer.Dispose(whs[1]);
                foreach (WaitHandle wh in whs)
                    wh.WaitOne();
                //updateTask.Wait();
                //exitMailTask.Wait();
                _cleanUp = true;
                log.Info("Exit gracefully!");
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }
        static void OnApplicationExit(object sender, EventArgs e)
        {
            CleanUp();
        }
    }
}
