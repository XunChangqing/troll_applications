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

        static TcpServer Server;
        static System.Threading.Timer delete_history_timer;
        static System.Threading.Timer update_domain_list_timer;
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            //AllocConsole();
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
                    NotificationRoutines.SendUninstallNotification().Wait();
                    return;
                }
                Init();

                bool result;
                //using named system mutex to ensure single instance of application
                var mutex = new System.Threading.Mutex(true, "masa_troll_guard_mutex", out result);

                if (!result)
                {
                    log.Error("Exit due to another running instance!");
                    return;
                }

                Application.ApplicationExit += OnApplicationExit;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(args));
                //Application.Run(new WechatForm());
                log.Info("Exit from Main!");
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
        static private void Init()
        {
            //change workdir to the path of executable
            var fi = new FileInfo(Application.ExecutablePath);
            Directory.SetCurrentDirectory(fi.DirectoryName);
#if DEBUG
            AllocConsole();
#endif
            //var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            //var companyName = versionInfo.CompanyName;
            AppLocalDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/masatek/trollwiz/";
            //upgrade user settings
            if (!Properties.Settings.Default.upgraded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.upgraded = true;
                Properties.Settings.Default.Save();
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
            Utils.Log_Init();
            PornDatabase.Init();
            PornClassifier.Init();
            //RegisterApplicationRestart("", 0);

            //PornDatabase.Test();
            //return;
            //init proxy server
            const bool bUseIPv6 = false;
            Server = new TcpServer(Properties.Settings.Default.bindPort, bUseIPv6);

            Server.Start(GreenProxy.CreateProxy);

            Server.InitListenFinished.WaitOne();
            if (Server.InitListenException != null)
                throw Server.InitListenException;

            update_domain_list_timer = new System.Threading.Timer(PornDatabase.UpdateDatabase, null, new TimeSpan(0, 0, 5), new TimeSpan(4, 0, 0));
            delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0, 1, 0), System.Threading.Timeout.InfiniteTimeSpan);
        }
        static void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                //unset proxy again to make sure
                ProxyRoutines.SetProxy(false);
                //dispose timer and wait for callback complete
                WaitHandle[] whs = new WaitHandle[]{
                new AutoResetEvent(false),
                new AutoResetEvent(false) };
                //WaitHandle wh = new AutoResetEvent(false);
                delete_history_timer.Dispose(whs[0]);
                update_domain_list_timer.Dispose(whs[1]);
                foreach (WaitHandle wh in whs)
                    wh.WaitOne();
                Server.Stop();
                //updateTask.Wait();
                //exitMailTask.Wait();
                log.Info("Exit gracefully!");
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }
    }
}
