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

        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
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
            if(Properties.Settings.Default.guid == "")
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
            RegisterApplicationRestart("", 0);

            //PornDatabase.Test();
            //return;
        }
    }
}
