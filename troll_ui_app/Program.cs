using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace troll_ui_app
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            bool result;
            //using named system mutex to ensure single instance of application
            var mutex = new System.Threading.Mutex(true, "masa_troll_guard_mutex", out result);

            if (!result)
            {
                //MessageBox.Show("Another instance is already running.");
                return;
            }

            ProxyProcess proxy_process = new ProxyProcess();
            proxy_process.StartProxyServer();

            //init timer to update database after each start of our application
            //update_timer.AutoReset = false;
            //update_timer.Elapsed += new System.Timers.ElapsedEventHandler(DatabaseUpdate);
            //update_timer.Interval = 2000;
            //update_timer.Enabled = true;

            System.Threading.Timer delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0,0,10), System.Threading.Timeout.InfiniteTimeSpan);
            System.Threading.Timer update_domain_list_timer = new System.Threading.Timer(PornDatabase.MaintainDatabase, null, new TimeSpan(0,0,20), new TimeSpan(0,60,0));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(args));

            proxy_process.CloseProxyServer();
            //dispose timer and wait for callback complete
            WaitHandle[] whs = new WaitHandle[]{
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };
            //WaitHandle wh = new AutoResetEvent(false);
            delete_history_timer.Dispose(whs[0]);
            update_domain_list_timer.Dispose(whs[1]);
            foreach (WaitHandle wh in whs)
                wh.WaitOne();
        }

        //eventhandler of system timer will be called in threadpool
        //static void DatabaseUpdate(object state)
        //{
        //    Debug.WriteLine("timer!");
        //    //Thread.Sleep(System.Threading.Timeout.Infinite);
        //    Debug.WriteLine("timer out!");
        //}
    }
}
