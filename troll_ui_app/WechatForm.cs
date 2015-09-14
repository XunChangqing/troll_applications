using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using System.IO;
using System.Net.Http;
//using Senparc.Weixin.MP.CommonAPIs;
//using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
//using Senparc.Weixin.MP.AdvancedAPIs;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using QRCoder;

namespace troll_ui_app
{
    public partial class WechatForm : Form
    {
        static readonly string webToken = "masa417";
        static readonly ILog log = Log.Get();
        static bool hasBeenAuth = false;
        public static bool Auth()
        {
            if (hasBeenAuth || Properties.Settings.Default.openid == "")
                return true;
            else
            {
                log.Info("auth wechat, openid new version: "+Properties.Settings.Default.openid);
                WechatForm wechatForm = new WechatForm(true);
                wechatForm.ShowDialog();
                if (wechatForm.authSuccess)
                {
                    hasBeenAuth = true;
                    return true;
                }
                else
                    return false;
            }
        }
        public bool authSuccess { get; set; }
        //binding mode if false;
        bool authMode = false;
        private Task getQrCodeTask;
        CancellationTokenSource cancellationTokenSource;
        public WechatForm(bool mode)
        {
            authMode = mode;
            authSuccess = false;
            InitializeComponent();
            Icon = Properties.Resources.TrollIcon;
        }

        private async Task SetQrcodeAsync(CancellationToken cancellationToken)
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
                string qrcodeUrl = retObj["url"].ToString();

                //msg = await client.GetAsync(string.Format(Properties.Settings.Default.wechatGetQrcodeUrl, HttpUtility.UrlEncode(ticket)), cancellationToken);
                //msg.EnsureSuccessStatusCode();
                //Stream img = await msg.Content.ReadAsStreamAsync();
                //Bitmap bmp = new Bitmap(img);

                //generate qrcode myself to decrease the latency
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(qrcodeUrl, QRCodeGenerator.ECCLevel.Q);
                qrCodePictureBox.Image = qrCode.GetGraphic(20);
                //qrCodePictureBox.Image = bmp;

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
                                break;
                            }
                            else
                            {
                                tipLabel.Text = "您的微信与绑定微信不一致，授权失败！";
                                authSuccess = false;
                                await Task.Delay(2000);
                                Close();
                                break;
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
                //ControlBox = false;
                Text = "微信绑定";
            }
            else
                Text = "您需要验证一次身份才能进行停止和退出操作！";
            cancellationTokenSource = new CancellationTokenSource();
            getQrCodeTask = SetQrcodeAsync(cancellationTokenSource.Token);
        }
        private void WechatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //stop task, otherwise it will get the openid forever
            cancellationTokenSource.Cancel();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            //getQrCodeTask.Wait();
            getQrCodeTask = SetQrcodeAsync(cancellationTokenSource.Token);
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
            //{{first.DATA}}
            //计算机名称：{{keyword1.DATA}}
            //不良网站域名：{{keyword2.DATA}}
            //{{remark.DATA}}
            JObject msgObj = new JObject();
            msgObj["touser"] = Properties.Settings.Default.openid;
            msgObj["template_id"] = Properties.Settings.Default.wechatTemplatePornDetected;
            //msgObj["url"] = "http://www.shanyaows.com";
            msgObj["topcolor"] = "#FF0000";
            JObject dataObj = new JObject();
            msgObj["data"] = dataObj;
            dataObj["first"] = new JObject();
            dataObj["first"]["value"] = "您好，域名访问被禁止。";
            dataObj["first"]["color"] = "#173177";
            dataObj["keyword1"] = new JObject();
            dataObj["keyword1"]["value"] = Environment.MachineName;
            dataObj["keyword1"]["color"] = "#173177";
            dataObj["keyword2"] = new JObject();
            dataObj["keyword2"]["value"] = domainName;
            dataObj["keyword2"]["color"] = "#173177";
            dataObj["remark"] = new JObject();
            dataObj["remark"]["value"] = "该计算机频繁访问上述不良网站，现已暂时禁止对该域名的访问。";
            dataObj["remark"]["color"] = "#173177";

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
