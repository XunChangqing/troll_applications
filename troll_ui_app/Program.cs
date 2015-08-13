using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Web;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace troll_ui_app
{
    static class Program
    {
        public const String kEventSource = "trollwiz";
        public const String kEventLog = "Application";
        public static String kWorkDir;
        public static String kImagesDir;
        public static String kConstPornDBPath;
        public static String kPornDBPath;
        public static String connectionString;
        /// <summary>
        /// 应用程序的主入口点。

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            AllocConsole();
            try
            {
                //var fi = new FileInfo(Application.ExecutablePath);
                //EventLog.WriteEntry(kEventSource, "ExecutablePath: " + Application.ExecutablePath, EventLogEntryType.Information);
                //Directory.SetCurrentDirectory(fi.DirectoryName);

                kWorkDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\masa\\trollwiz";
                kImagesDir = kWorkDir + "\\images";
                kPornDBPath = kWorkDir + "\\porn.db";
                kConstPornDBPath = Application.ExecutablePath + "\\porn.db";
                connectionString = "Integrated Security=SSPI;Persist Security Info=False;" +
            "Initial Catalog=Northwind;Data Source=" + kPornDBPath;
                //Properties.Settings.Default.Reset();

                PornDatabase.InitWorkDir();

                bool result;
                //using named system mutex to ensure single instance of application
                var mutex = new System.Threading.Mutex(true, "masa_troll_guard_mutex", out result);

                if (!result)
                {
                    EventLog.WriteEntry(kEventSource, "Exit due another running instance", EventLogEntryType.Warning);
                    return;
                }

                System.Threading.Timer delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0, 0, 10), System.Threading.Timeout.InfiniteTimeSpan);
                System.Threading.Timer update_domain_list_timer = new System.Threading.Timer(PornDatabase.MaintainDatabase, null, new TimeSpan(0, 0, 20), new TimeSpan(0, 60, 0));

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
                //EventLog.WriteEntry(kEventSource, "Exit", EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                //EventLog.WriteEntry(kEventSource, e.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
