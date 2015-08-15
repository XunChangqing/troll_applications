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
using TrotiNet;
using log4net;
using System.Reflection;

namespace troll_ui_app
{
    static class Program
    {
        static readonly ILog log = Log.Get();
        public static bool FirstTime = false;
        /// <summary>
        /// 应用程序的主入口点。

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            try
            {
                if(Properties.Settings.Default.firstTime)
                {
                    FirstTime = true;
                    Properties.Settings.Default.firstTime = false;
                    Properties.Settings.Default.Save();
                }
                Init();
                Utils.Log_Init();
                PornDatabase.Init();
                //var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                //return;
                //PornDatabase.Test();
                //return;
                PornClassifier.Init();

                const bool bUseIPv6 = false;
                var Server = new TcpServer(Properties.Settings.Default.bindPort, bUseIPv6);

                Server.Start(GreenProxy.CreateProxy);

                Server.InitListenFinished.WaitOne();
                if (Server.InitListenException != null)
                    throw Server.InitListenException;

                //Properties.Settings.Default.Reset();

                bool result;
                //using named system mutex to ensure single instance of application
                var mutex = new System.Threading.Mutex(true, "masa_troll_guard_mutex", out result);

                if (!result)
                {
                    log.Error("Exit due to another running instance!");
                    return;
                }

                System.Threading.Timer delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0, 0, 10), System.Threading.Timeout.InfiniteTimeSpan);
                System.Threading.Timer update_domain_list_timer = new System.Threading.Timer(PornDatabase.UpdateDatabase, null, new TimeSpan(0, 0, 20), new TimeSpan(0, 60, 0));

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(args));

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

            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }

        static private void Init()
        {
#if DEBUG
            AllocConsole();
#endif
            ////change workdir to the path of executable
            var fi = new FileInfo(Application.ExecutablePath);
            Directory.SetCurrentDirectory(fi.DirectoryName);

            ////check if the first time running
            //Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            //if (configuration.AppSettings.Settings["FirstTime"].Value == "yes")
            //{
            //    configuration.AppSettings.Settings["FirstTime"].Value = "no";
            //    configuration.Save();
            //    ConfigurationManager.RefreshSection("appSettings");

            //    //reset user settings
            //    Properties.Settings.Default.Reset();
            //}

            //create directory for images
            if (!Directory.Exists(Properties.Settings.Default.imagesDir))
                Directory.CreateDirectory(Properties.Settings.Default.imagesDir);
        }
    }
}
