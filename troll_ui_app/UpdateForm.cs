using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using log4net;
using System.Net;
using System.Web;

namespace troll_ui_app
{
    public partial class UpdateForm : Form
    {
        static readonly ILog log = Log.Get();
        static UpdateForm updateForm;
        private string updateUrl;
        public UpdateForm(string url)
        {
            updateUrl = url;
            InitializeComponent();
        }

        private WebClient updateDownloadClient;
        private Task updateDownloadTask;

        public static async Task UpdateProduct()
        {
            //if (newVersion > curVersion)
            //    MessageBox.Show("Update");
            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                Version curVersion = new Version(versionInfo.ProductVersion);
                WebClient client = new WebClient();
                XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load("trollwiz-vers.xml");
                string xmlstr = await client.DownloadStringTaskAsync(Properties.Settings.Default.updateXmlUrl);
                xmlDoc.LoadXml(xmlstr);
                Version latestVersion = curVersion;
                string latestUrl = null;
                XmlElement root = xmlDoc.DocumentElement;//取到根结点
                foreach (var element in root.ChildNodes)
                {
                    if (element.GetType() == typeof(XmlElement))
                    {
                        XmlElement ele = (XmlElement)element;
                        if (ele.Name == "ver")
                        {
                            Version cv = new Version(ele.Attributes["version"].Value);
                            if (cv > latestVersion)
                            {
                                latestVersion = cv;
                                latestUrl = ele.Attributes["url"].Value;
                            }
                        }
                    }
                }

                if(latestUrl != null)
                {
                    log.Info("Update from: " + latestUrl);
                    if (DialogResult.Yes == MessageBox.Show("有新版本：" + latestVersion + "，是否更新？", "版本更新", MessageBoxButtons.YesNo))
                    {
                        updateForm = new UpdateForm(latestUrl);
                        updateForm.Show();
                    }
                }

            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
            //XmlNode newNode = xmlDoc.CreateNode("element", "NewBook", "");
            //newNode.InnerText = "WPF";

            //添加为根元素的第一层子结点
            //root.AppendChild(newNode);
            //xmlDoc.Save(xmlPath);

            //var companyName = versionInfo.CompanyName;
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            try
            {
                updateDownloadClient = new WebClient();
                updateDownloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                updateDownloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                //client.DownloadFileAsync(new Uri("http://joshua-ferrara.com/luahelper/lua.syn"), @"C:\LUAHelper\Syntax Files\lua.syn");
                //client.DownloadFileTaskAsync("http://trollwiz-online-update.oss-cn-beijing.aliyuncs.com/trollwiz-vers.xml", "my.xml");
                //updateDownloadTask = updateDownloadClient.DownloadFileTaskAsync("http://skylineservers.dl.sourceforge.net/project/opencvlibrary/opencv-win/3.0.0/opencv-3.0.0.exe", "opencv.exe");
                updateDownloadTask = updateDownloadClient.DownloadFileTaskAsync(updateUrl, Program.AppLocalDir + "updates/" + HttpUtility.UrlEncode(updateUrl));
            }
            catch(Exception exception)
            {
                log.Error(exception.ToString());
                this.Close();
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            downloadProgressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Error!=null)
            {
                log.Error(e.Error.ToString());
                this.Close();
            }
            //e.Error;
            //may be triggered by cancellation
            else if (!e.Cancelled)
            {
                //start installer
                try
                {
                    Process process = new Process();
                    //check if the proxy server is still alive
                    process.StartInfo.FileName = Program.AppLocalDir + "updates/" + HttpUtility.UrlEncode(updateUrl);
                    //process.Exited += new EventHandler(myProcess_Exited);
                    //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //process.StartInfo.UseShellExecute = true;
                    //process.StartInfo.Arguments = "--workdir " + Program.kWorkDir;
                    //var fi = new FileInfo(Application.ExecutablePath);
                    //process.StartInfo.WorkingDirectory = fi.DirectoryName;
                    if (process.Start())
                        Application.Exit();
                    else
                        this.Close();
                }
                catch (Exception exception)
                {
                    log.Error(exception.ToString());
                }
                //and exit for update
            }
        }

        private void UpdateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            updateDownloadClient.CancelAsync();
        }
    }
}
