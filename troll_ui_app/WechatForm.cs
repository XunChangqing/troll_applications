using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Senparc.Weixin.MP.CommonAPIs;
using log4net;
using System.IO;
using System.Net.Http;
//using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
//using Senparc.Weixin.MP.AdvancedAPIs;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace troll_ui_app
{
    public partial class WechatForm : Form
    {
        static readonly string webToken = "masa417";
        static readonly ILog log = Log.Get();
        public bool authSuccess { get; set; }
        //binding mode if false;
        bool authMode = false;
        private Task getQrCodeTask;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken;
        public WechatForm(bool mode)
        {
            authMode = mode;
            authSuccess = false;
            cancellationToken = cancellationTokenSource.Token;
            InitializeComponent();
        }


        private async Task SetQrcodeAsync()
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);

                JObject createUserObj = new JObject();
                createUserObj["token"] = webToken;
                //createUserObj["uuid"] = "dummy-uuid";

                //HttpResponseMessage msg = client.PostAsync(Properties.Settings.Default.createUserUrl, formContent).Result;
                HttpResponseMessage msg = await client.PostAsync(Properties.Settings.Default.createSceneUrl, new StringContent(createUserObj.ToString(),
                    Encoding.UTF8, "application/json"), cancellationToken);
                msg.EnsureSuccessStatusCode();
                string retStr = await msg.Content.ReadAsStringAsync();
                JObject retObj = JObject.Parse(retStr);
                int userid = int.Parse(retObj["scene_id"].ToString());

                string qrcodeCreateUrl = string.Format(Properties.Settings.Default.wechatCreateQrcodeUrl, retObj["access_token"]);
                JObject requestObj = new JObject();
                requestObj["expire_seconds"] = 604800;
                requestObj["action_name"] = "QR_SCENE";
                requestObj["action_info"] = new JObject();
                requestObj["action_info"]["scene"] = new JObject();
                requestObj["action_info"]["scene"]["scene_id"] = userid;
                string x = requestObj.ToString();
                msg = await client.PostAsync(qrcodeCreateUrl,
                    new StringContent(requestObj.ToString(), Encoding.UTF8, "application/json"),
                    cancellationToken);
                retStr = await msg.Content.ReadAsStringAsync();
                retObj = JObject.Parse(retStr);
                string ticket = retObj["ticket"].ToString();
                msg = await client.GetAsync(string.Format(Properties.Settings.Default.wechatGetQrcodeUrl, HttpUtility.UrlEncode(ticket)), cancellationToken);
                msg.EnsureSuccessStatusCode();
                Stream img = await msg.Content.ReadAsStreamAsync();
                Bitmap bmp = new Bitmap(img);
                qrCodePictureBox.Image = bmp;

                //qrCodePictureBox.ImageLocation = string.Format(Properties.Settings.Default.wechatGetQrcodeUrl, HttpUtility.UrlEncode(ticket));
                //qrCodePictureBox.LoadAsync(string.Format(Properties.Settings.Default.wechatGetQrcodeUrl, HttpUtility.UrlEncode(ticket)));

                JObject sceneRequestObj = new JObject();
                sceneRequestObj["token"] = webToken;
                sceneRequestObj["scene_id"] = userid;
                while(true)
                {
                    msg = await client.PostAsync(Properties.Settings.Default.getSceneUrl,
                        new StringContent(sceneRequestObj.ToString(), Encoding.UTF8, "application/json"),
                        cancellationToken);
                    msg.EnsureSuccessStatusCode();
                    retStr = await msg.Content.ReadAsStringAsync();
                    retObj = JObject.Parse(retStr);
                    string openid = retObj["openid"].ToString();
                    if (openid != "")
                    {
                        if (authMode)
                        {
                            if(openid == Properties.Settings.Default.openid)
                            {
                                authSuccess = true;
                                tipLabel.Text = "授权成功！";
                                await Task.Delay(2000);
                                Close();
                            }
                            else
                            {
                                tipLabel.Text = "您的微信与绑定微信不一致，授权失败！";
                                authSuccess = false;
                                await Task.Delay(2000);
                                Close();
                            }

                        }
                        else
                        {
                            log.Info("scaned by: " + openid);
                            tipLabel.Text = "微信绑定成功！";
                            Properties.Settings.Default.openid = openid;
                            Properties.Settings.Default.Save();
                            ////注册用户信息到服务器
                            //JObject registerRequestObj = new JObject();
                            //sceneRequestObj["token"] = webToken;
                            //sceneRequestObj["guid"] = Properties.Settings.Default.guid;
                            //sceneRequestObj["machine-name"] = Environment.MachineName;
                            //sceneRequestObj["openid"] = openid;
                            //msg = await client.PostAsync(Properties.Settings.Default.registerTrollwizUserUrl,
                            //    new StringContent(sceneRequestObj.ToString(), Encoding.UTF8, "application/json"),
                            //    cancellationToken);
                            //msg.EnsureSuccessStatusCode();
                            await Task.Delay(2000);
                            Close();
                            break;
                        }
                    }
                    await Task.Delay(2000);
                    log.Info("one time again!");
                }
            }
            catch(Exception err)
            {
                log.Error(err.ToString());
            }
        }

        private void WechatForm_Load(object sender, EventArgs e)
        {
            if (!authMode)
            {
                ControlBox = false;
                Text = "微信绑定";
            }
            else
            {
                Text = "微信授权";
            }
            getQrCodeTask = SetQrcodeAsync();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            getQrCodeTask.Wait();
            getQrCodeTask = SetQrcodeAsync();
        }
        private static async Task SendTemplateMessageAsync(JObject msgObj)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);

                JObject templateMsgObj = new JObject();
                templateMsgObj["token"] = webToken;

                HttpResponseMessage msg = await client.PostAsync(Properties.Settings.Default.getAccessTokenUrl, new StringContent(templateMsgObj.ToString(),
                    Encoding.UTF8, "application/json"));
                msg.EnsureSuccessStatusCode();
                string retStr = await msg.Content.ReadAsStringAsync();
                JObject retObj = JObject.Parse(retStr);

                string sendTemplateMsgUrl = string.Format(Properties.Settings.Default.wechatSendTemplateMessage, retObj["access_token"].ToString());
                msg = await client.PostAsync(sendTemplateMsgUrl, new StringContent(msgObj.ToString(),
                    Encoding.UTF8, "application/json"));
                retStr = await msg.Content.ReadAsStringAsync();
                log.Info(retStr);
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
        }

        public static async Task SendUninstallNotificationAsync()
        {
            JObject msgObj = new JObject();
            msgObj["touser"] = Properties.Settings.Default.openid;
            msgObj["template_id"] = Properties.Settings.Default.wechatTemplateUninstall;
            //msgObj["url"] = "http://www.shanyaows.com";
            msgObj["topcolor"] = "#FF0000";
            JObject dataObj = new JObject();
            msgObj["data"] = dataObj;
            dataObj["machine-name"] = new JObject();
            dataObj["machine-name"]["value"] = Environment.MachineName;
            dataObj["machine-name"]["color"] = "#173177";
            await SendTemplateMessageAsync(msgObj);
        }

        public static async Task SendPornDetectedNotificationAsync(string domainName)
        {
            JObject msgObj = new JObject();
            msgObj["touser"] = Properties.Settings.Default.openid;
            msgObj["template_id"] = Properties.Settings.Default.wechatTemplatePornDetected;
            //msgObj["url"] = "http://www.shanyaows.com";
            msgObj["topcolor"] = "#FF0000";
            JObject dataObj = new JObject();
            msgObj["data"] = dataObj;
            dataObj["machine-name"] = new JObject();
            dataObj["machine-name"]["value"] = Environment.MachineName;
            dataObj["machine-name"]["color"] = "#173177";
            dataObj["domain-name"] = new JObject();
            dataObj["domain-name"]["value"] = domainName;
            dataObj["domain-name"]["color"] = "#173177";
            await SendTemplateMessageAsync(msgObj);
        }

        public static async Task SendPornScannedNotificationAsync(string picNumber)
        {
            JObject msgObj = new JObject();
            msgObj["touser"] = Properties.Settings.Default.openid;
            msgObj["template_id"] = Properties.Settings.Default.wechatTemplatePornScanned;
            //msgObj["url"] = "http://www.shanyaows.com";
            msgObj["topcolor"] = "#FF0000";
            JObject dataObj = new JObject();
            msgObj["data"] = dataObj;
            dataObj["machine-name"] = new JObject();
            dataObj["machine-name"]["value"] = Environment.MachineName;
            dataObj["machine-name"]["color"] = "#173177";
            dataObj["pic-number"] = new JObject();
            dataObj["pic-number"]["value"] = picNumber;
            dataObj["pic-number"]["color"] = "#173177";
            await SendTemplateMessageAsync(msgObj);
        }
    }
}
