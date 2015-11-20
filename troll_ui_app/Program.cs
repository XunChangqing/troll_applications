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
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.Win32;
using Microsoft.VisualBasic.ApplicationServices;

namespace troll_ui_app
{
    static class Program
    {
        static readonly ILog log = Log.Get();
        public static bool FirstTime = false;
        public static bool RealBindingSucess = false;
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
        static System.Threading.Mutex systemMutex;
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            //AutoCloseMessageBox.ShowMessage(20, "执行此操作需要微信绑定者进行授权！\n请按照公众号提示操作后重试！");
            //return;
            //AutoCloseMessageBox acmb = new AutoCloseMessageBox(100, "123");
            //acmb.ShowDialog();
            //return;
            //PornClassifier.Init();
            //FFMPEGWrapper.Init();
            ////PornClassifier.Instance.ClassifyVideoFile("1.mp4");
            //PornClassifier.Instance.ClassifyVideoFile("D:\\迅雷下载\\国产真实母子视频做爰[连干4次].rmvb");
            //return;
            //FFMPEGWrapper ffmpeg = new FFMPEGWrapper();
            //ffmpeg.Open("1.mp4");
            //ffmpeg.ReadFrame(4000);
            //ffmpeg.Dispose();
            //return;
            //ProcessThreadCollection ptc = Process.GetCurrentProcess().Threads;
            //Console.WriteLine("Start Threads Num: " + ptc.Count);
            //Thread t = new Thread(() =>
            //{
            //    //try
            //    //{
            //        while (true)
            //        {
            //            Console.WriteLine("loop!");
            //        }
            //    //}
            //    //catch (Exception e)
            //    //{
            //    //    Console.WriteLine(e.ToString());
            //    //}
            //});
            //t.Start();
            //Console.WriteLine(t.ThreadState);
            //t.Abort();
            //t.Join();
            //return;
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
                //throw new Exception("test error 2");
                //when uninstall send email and return
                if (args.Contains("-uninstall"))
                {
                    //NotificationRoutines.SendUninstallNotification().Wait();
                    return;
                }
                RegisterApplicationRestart(null, 0);
                //Init();
                InitLogAndDirs();

                string systemMutexName = "masa_troll_guard_mutex";
                bool result;
                systemMutex = new System.Threading.Mutex(true, systemMutexName, out result);
                //if(!result)
                //{
                //    log.Error("Exit due to another running instance: " + systemMutex.ToString());
                //    return;
                //}
                //如果是开机启动，则延时两分钟，减少对开机启动时间的影响
                if (args.Contains("-notvisible"))
                {
                    //System.Threading.Thread.Sleep(120000);
                }
                //必须在绑定之前，这样才能正常访问web的绑定服务
                SSLInit();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                bool bindingSuccess = true;
                if (Properties.Settings.Default.openid == "" ||
                    Properties.Settings.Default.userNickname == "")
                {
                    WechatForm bindingForm = WechatForm.GetInstance();
                    //bindingForm.ShowDialog();
                    //使用以下一行也可以
                    //WechatSingleInstanceController controller = new WechatSingleInstanceController();
                    //controller.Run(args);
                    Application.Run(bindingForm);
                    bindingSuccess = bindingForm.BindingSuccess;
                    RealBindingSucess = bindingForm.BindingSuccess;
                }

                if (bindingSuccess)
                {
                    InitForBusinessLogic();
                    Application.ApplicationExit += OnApplicationExit;
                    //MainForm mainform = new MainForm(args);
                    //Application.Run(mainform);
                    SingleInstanceController controller = new SingleInstanceController();
                    controller.Run(args);
                    CleanUp();
                }
                log.Info("Exit from Main!");
            }
            catch (Exception e)
            {
                //如果意外退出，一定要记得关闭代理，否则会导致用户无法上网
                CleanUp();
                log.Error(e.ToString());
                ReportErrorOnline(e.ToString());
                MessageBox.Show("程序由于未知问题崩溃，我们尽快处理，给您带来的不便敬请谅解！",
                        "程序崩溃", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public class WechatSingleInstanceController : WindowsFormsApplicationBase
        {
            public WechatSingleInstanceController()
            {
                IsSingleInstance = true;
                StartupNextInstance += this_StartupNextInstance;
            }

            void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
            {
                WechatForm form = MainForm as WechatForm; //My derived form type
                form.Show();
            }

            protected override void OnCreateMainForm()
            {
                MainForm = WechatForm.GetInstance();
            }
        }
        public class SingleInstanceController : WindowsFormsApplicationBase
        {
            public SingleInstanceController()
            {
                IsSingleInstance = true;
                StartupNextInstance += this_StartupNextInstance;
            }

            void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
            {
                MainForm form = MainForm as MainForm; //My derived form type
                form.Show();
            }

            protected override void OnCreateMainForm()
            {
                MainForm = new MainForm(Environment.GetCommandLineArgs());
                //因为要操作主窗口，所以需要在主窗口生成以后
                if (Program.RealBindingSucess)
                    WechatForm.TurnOnAuth();
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
        }
        static void InitForBusinessLogic()
        {
            //如果数据库文件不存在，则建立数据库文件
            PornDatabase.CreateDatabase();
            PornClassifier.Init();
            FFMPEGWrapper.Init();
            WechatForm.Init();

            update_domain_list_timer = new System.Threading.Timer(PornDatabase.UpdateDatabase, null, new TimeSpan(0, 0, 5), new TimeSpan(4, 0, 0));
            delete_history_timer = new System.Threading.Timer(PornDatabase.DeleteHistroy, null, new TimeSpan(0, 1, 0), System.Threading.Timeout.InfiniteTimeSpan);
        }
        static void SSLInit()
        {
            // Override automatic validation of SSL server certificates.
            ServicePointManager.ServerCertificateValidationCallback =
                   ValidateServerCertficate;
        }
        static readonly byte[] apiCertHash = { 224, 179, 82, 226, 79, 100, 202, 56, 92, 237, 148, 186, 170, 31, 83, 148, 21, 150, 103, 52 };
        static bool ValidateServerCertficate(
                object sender,
                X509Certificate cert,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                // Good certificate.
                return true;
            }

            log.DebugFormat("SSL certificate error: {0}", sslPolicyErrors);

            bool certMatch = false; // Assume failure
            byte[] certHash = cert.GetCertHash();
            if (certHash.Length == apiCertHash.Length)
            {
                certMatch = true; // Now assume success.
                for (int idx = 0; idx < certHash.Length; idx++)
                {
                    if (certHash[idx] != apiCertHash[idx])
                    {
                        certMatch = false; // No match
                        break;
                    }
                }
            }

            // Return true => allow unauthenticated server,
            //        false => disallow unauthenticated server.
            return certMatch;
        }
        public static bool GetProxy()
        {
            try
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                if ((int)reg.GetValue("ProxyEnable") == 1 && reg.GetValue("ProxyServer").ToString().Contains("http=127.0.0.1:8090"))
                    return true;
                else
                    return false;
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
                return false;
            }
        }
        public static void EnableProxy()
        {
#if !DEBUG
            //if(System.Environment.OSVersion.Version <=  
            //if (System.Environment.OSVersion.Version.Major <= 6 && System.Environment.OSVersion.Version.Minor <= 1)
            //{
            //    log.Info("Enabel Proxy Win7!");
            //    try { ProxyRoutines.SetProxy("http=127.0.0.1:8090"); }
            //    catch (Exception exp) { log.Error(exp.ToString()); }
            //}
            //else
            {
                log.Info("Enabel Proxy!");
                try
                {
                    if (!GetProxy())
                    {
                        SystemProxyHelper.EnableProxyHTTP("127.0.0.1", 8090);
                        FireFoxHelper.AddFirefox();
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.ToString());
                }
            }
#endif
        }
        public static void DisableProxy()
        {
            //unset proxy again to make sure
#if !DEBUG
            //if (System.Environment.OSVersion.Version.Major<=6 && System.Environment.OSVersion.Version.Minor<=1)
            //{
            //    log.Info("Disable Proxy Win7!");
            //    try
            //    {
            //        ProxyRoutines.SetProxy(false);
            //    }
            //    catch (Exception exp)
            //    { log.Error(exp.ToString()); }
            //}
            //else
            {
                try
                {
                    if (GetProxy())
                    {
                        log.Info("Disable Proxy!");
                        //在关机时，不能refresh，否则会导致无法修改成功，其他时候则要刷新，否则会导致无法刷新
                        if (kCloseReason == CloseReason.WindowsShutDown)
                        {
                            log.Info("Disable proxy without Refresh!");
                            SystemProxyHelper.DisableAllProxyWithourRestoreAndRefresh();
                            //int tryTimes = 10;
                            //while (tryTimes-- > 0)
                            //{
                            //SystemProxyHelper.DisableAllProxyWithourRestore();
                            //reg.SetValue("ProxyEnable", 0);
                            //reg.Flush();
                            ////read again
                            //if ((int)reg.GetValue("ProxyEnable") != 0)
                            //    Thread.Sleep(50);
                            ////else
                            //    //break;
                            RegistryKey reg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                            log.Info("ProxyEnable Value: " + reg.GetValue("ProxyEnable"));
                            //}
                        }
                        else
                        {
                            log.Info("Disable proxy with Refresh!");
                            SystemProxyHelper.DisableAllProxyWithourRestore();
                        }
                        FireFoxHelper.RemoveFirefox();
                    }
                }
                catch (Exception exp) { log.Error(exp.ToString()); }
            }
#endif

        }
        static bool _cleanUp = false;
        public static CloseReason kCloseReason = CloseReason.None;
        public static void CleanUp()
        {
            try
            {
                if (_cleanUp)
                    return;
                //只有有相关功能打开时，才在退出时关闭代理设置，避免触发杀毒软件
                if(Properties.Settings.Default.IsNetworkImageTurnOn || Properties.Settings.Default.IsPornWebsiteProtectionTurnOn)
                    DisableProxy();
                try
                {
                    systemMutex.Dispose();
                }
                catch (Exception exp)
                { log.Error(exp.ToString()); }
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

        static readonly string webToken = "masa417";
        static void ReportErrorOnline(string errMsg)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);

                JObject errReportObj = new JObject();
                errReportObj["token"] = webToken;
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                Version curVersion = new Version(versionInfo.ProductVersion);
                errReportObj["version"] = curVersion.ToString();
                errReportObj["errmsg"] = errMsg;

                HttpResponseMessage msg = client.PostAsync(Properties.Settings.Default.errorReportUrl,
                    new StringContent(errReportObj.ToString(),
                    Encoding.UTF8, "application/json")).Result;
                msg.EnsureSuccessStatusCode();
                string retStr = msg.Content.ReadAsStringAsync().Result;
            }
            catch (Exception err)
            {
                log.Error("提交崩溃错误失败: " + err.ToString());
            }
        }
    }
}
