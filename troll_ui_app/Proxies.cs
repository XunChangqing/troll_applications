using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using log4net;

namespace troll_ui_app
{
    //public class Proxies
    //{
    //    static readonly ILog log = Log.Get();
    //    [DllImport("wininet.dll")]
    //    public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
    //    public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
    //    public const int INTERNET_OPTION_REFRESH = 37;
    //    static bool settingsReturn, refreshReturn;
    //    static object origin_proxy_enable;
    //    static object origin_proxy_server;

    //    public static void SetProxy(string proxy = "http=127.0.0.1:8090")
    //    {
    //        //return;
    //        try
    //        {
    //            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
    //            //origin_proxy_enable = registry.GetValue("ProxyEnable");
    //            //origin_proxy_server = registry.GetValue("ProxyServer");
    //            log.Info(registry.GetValue("ProxyEnable").ToString());
    //            log.Info(registry.GetValue("ProxyServer").ToString());
    //            registry.SetValue("ProxyEnable", 1);
    //            registry.SetValue("ProxyServer", proxy);

    //            // These lines implement the Interface in the beginning of program 
    //            // They cause the OS to refresh the settings, causing IP to realy update
    //            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
    //            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
    //            log.Info("Enable Proxy!");
    //        }
    //        catch(Exception e)
    //        {
    //            log.Info(e.ToString());
    //        }
    //    }

    //    public static void UnsetProxy()
    //    {
    //        //return;
    //        try
    //        {
    //            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
    //            log.Info(registry.GetValue("ProxyEnable").ToString());
    //            log.Info(registry.GetValue("ProxyServer").ToString());
    //            registry.DeleteValue("ProxyServer", false);
    //            registry.SetValue("ProxyEnable", 0);
    //            //registry.SetValue("ProxyServer", "0.0.0.0:80");
    //            //registry.SetValue("ProxyEnable", origin_proxy_enable);
    //            //registry.SetValue("ProxyServer", origin_proxy_server);

    //            // These lines implement the Interface in the beginning of program 
    //            // They cause the OS to refresh the settings, causing IP to realy update
    //            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
    //            log.Info("InternetSetOption Ret: " + settingsReturn);
    //            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
    //            log.Info("InternetSetOption Ret: " + refreshReturn);
    //        }
    //        catch(Exception e)
    //        {
    //            log.Error(e.ToString());
    //        }
    //    }
    //}
}
