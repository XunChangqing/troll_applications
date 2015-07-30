using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            PornDatabase.DeleteHistoryBlockedPages();
            PornDatabase.DeleteHistoryPornPics();

            ProxyProcess proxy_process = new ProxyProcess();
            proxy_process.StartProxyServer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());

            proxy_process.CloseProxyServer();

        }
    }
}
