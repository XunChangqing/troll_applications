using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

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
            //PornDatabase.DeleteHistoryBlockedPages();
            //PornDatabase.DeleteHistoryPornPics();
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(args));

            proxy_process.CloseProxyServer();
        }
    }
}
