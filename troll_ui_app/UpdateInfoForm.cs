using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using log4net;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Reflection;

namespace troll_ui_app
{
    public partial class UpdateInfoForm : Form
    {
        static readonly ILog log = Log.Get();
        static UpdateInfoForm Singleton;
        Version newVersion;
        //string verInfo;
        string verUrl;
        private UpdateInfoForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.TrollIcon;
        }
        private async Task UpdateProduct()
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
                string latestInfo = null;
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
                                latestInfo = ele.InnerText;
                            }
                        }
                    }
                }

                Version skipVersion = new Version(Properties.Settings.Default.skipVersion);

                if (latestVersion > curVersion && latestVersion > skipVersion && latestUrl != null)
                {
                    log.Info("New Version: " + latestVersion.ToString() + ", URL: " + latestUrl);
                    updateInfoRichTextBox.Text = latestInfo;
                    this.Text += latestVersion.ToString();
                    verUrl = latestUrl;
                    newVersion = latestVersion;
                    Show();
                    //UpdateVersionInfo retInfo = new UpdateVersionInfo();
                    //retInfo.newVersion = latestVersion;
                    //retInfo.verInfo = latestInfo;
                    //retInfo.verUrl = latestUrl;
                    //return retInfo;
                }
                else
                {
                    updateInfoRichTextBox.Text = "没有检测新版本！";
                    log.Info("No new version");
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

        private void UpdateInfoForm_Load(object sender, EventArgs e)
        {
        }

        public static UpdateInfoForm GetInstance()
        {
            if (Singleton == null || Singleton.IsDisposed)
            {
                Singleton = new UpdateInfoForm();
                Task t = Singleton.UpdateProduct();
            }
            return Singleton;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateForm.GetInstance(verUrl).Show();
        }

        private void updateNextTimeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void skipVersionButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.skipVersion = newVersion.ToString();
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
