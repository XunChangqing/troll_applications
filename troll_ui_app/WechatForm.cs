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
using System.Runtime.InteropServices;
using System.Net;

namespace troll_ui_app
{
    public partial class WechatForm : Form
    {
        static readonly string webToken = "masa417";
        static readonly ILog log = Log.Get();
        //static bool hasBeenAuth = false;
        public static DateTime LastAuthDateTime = DateTime.MinValue;
        public static TimeSpan AuthExpiredTimeLeft = TimeSpan.Zero;
        static System.Timers.Timer AuthExpiredTimer = new System.Timers.Timer();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void MouseDownMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                //SendMessage(this.ParentForm.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        public bool BindingSuccess { get; set; }
        static public void Init()
        {
            AuthExpiredTimer = new System.Timers.Timer(1000);
            AuthExpiredTimer.AutoReset = true;
            AuthExpiredTimer.Enabled = false;
            AuthExpiredTimer.Elapsed += AuthExpiredTimerOnElapsed;
            UpdateMainFormAuthStatus = new UpdateMainFormAuthStatusDelegate(UpdateMainFormAuthStatusMethod);
        }

        static void AuthExpiredTimerOnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AuthExpiredTimeLeft = AuthExpiredTimeLeft.Subtract(TimeSpan.FromSeconds(1));
            MainForm.Instance.mainPanelControl.Invoke(UpdateMainFormAuthStatus);
            if (AuthExpiredTimeLeft <= TimeSpan.Zero)
                AuthExpiredTimer.Stop();
        }
        delegate void UpdateMainFormAuthStatusDelegate();
        static UpdateMainFormAuthStatusDelegate UpdateMainFormAuthStatus;

        static void UpdateMainFormAuthStatusMethod()
        {
            MainForm.Instance.mainPanelControl.UpdateAuthStatus(AuthExpiredTimeLeft);
        }
        static public void TurnOnAuth()
        {
            //AuthExpiredTimeLeft = TimeSpan.FromSeconds(40);
            AuthExpiredTimeLeft = TimeSpan.FromHours(2);
            MainForm.Instance.mainPanelControl.UpdateAuthStatus(AuthExpiredTimeLeft);
            AuthExpiredTimer.Start();
        }
        static public bool Auth()
        {
            return true;
            //if (hasBeenAuth)
            if (AuthExpiredTimeLeft>TimeSpan.Zero)
                return true;
            else
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                bool networkException = false;
                DateTime n = UpdateAuth(cts.Token, out networkException);
                if(networkException)
                {
                    //MessageBox.Show("获取授权失败，请检查网络是否连接！",
                    //        "获取授权失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AutoCloseMessageBox.ShowMessage(20, "获取授权失败，请检查网络是否连接！");
                    return false;
                }
                else if (n.AddMinutes(20) > DateTime.Now)
                {
                    TurnOnAuth();
                    return true;
                }
                else
                {
                    Task t = SendAuthRequestNotificationAsync();
                    AutoCloseMessageBox.ShowMessage(20, "执行此操作需要微信绑定者进行授权！\n请按照公众号提示操作后重试！");
                    //MessageBox.Show("执行此操作需要微信绑定者点击公众号授权按钮进行授权！\n请在公众号菜单中点击授权按钮后重试！",
                    //    "操作未授权", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        public static DateTime UpdateAuth(CancellationToken cancellationToken, out bool networkException)
        {
            try
            {
                networkException = false;
                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);

                JObject getAuthInfoObj = new JObject();
                getAuthInfoObj["token"] = webToken;
                getAuthInfoObj["openid"] = Properties.Settings.Default.openid;

                HttpResponseMessage msg = client.PostAsync(Properties.Settings.Default.getAuthInfoUrl,
                    new StringContent(getAuthInfoObj.ToString(),
                    Encoding.UTF8, "application/json"), cancellationToken).Result;
                msg.EnsureSuccessStatusCode();
                string retStr = msg.Content.ReadAsStringAsync().Result;
                JObject retObj = JObject.Parse(retStr);
                JToken usedToken = retObj["used"];
                JToken updatedatToken = retObj["updated_at"];
                if (usedToken != null && !bool.Parse(usedToken.ToString()))
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    Task t = UpdateUserInfo(cts.Token);
                    return DateTime.Parse(updatedatToken.ToString()).ToLocalTime();
                }
                else
                    return DateTime.MinValue;
            }
            catch(Exception err)
            {
                log.Error(err.ToString());
                networkException = true;
                return DateTime.MinValue;
            }
        }
        public bool authSuccess { get; set; }
        //binding mode if false;
        private Task getQrCodeTask;
        CancellationTokenSource cancellationTokenSource;
        //Label _whyLabel;
        TextButton _whyLabel;
        TextButton _refreshButton;
        Panel _closeBackImage;
        Label _closeBtn;
        Label _testLabel;
        public WechatForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon_main_icon;
            Text = "山妖卫士";
            MouseDown += MouseDownMove;
            titleLabel.MouseDown += MouseDownMove;

            //AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            //Width = 230;
            //Height = 320;
            _testLabel = new Label();
            _testLabel.Location = new Point(230, 0);
            _testLabel.Text = "测试";
            Controls.Add(_testLabel);

            tipLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            tipLabel.ForeColor = Color.FromArgb(0x4f, 0xb5, 0x2c);

            _whyLabel = new TextButton();
            _whyLabel.Text = "为什么要绑定？";
            _whyLabel.AutoSize = true;
            _whyLabel.Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Pixel);
            _whyLabel.ForeColor = Color.FromArgb(0x4f, 0xb5, 0x2c);
            Controls.Add(_whyLabel);
            _whyLabel.Location = new Point(Width / 2 - _whyLabel.Width / 2,
                tipLabel.Location.Y + tipLabel.Height);
            _whyLabel.Click += _whyLabelOnClick;
            //ToolTip whyTip = new ToolTip();
            //whyTip.SetToolTip(_whyLabel,
            //    "绑定以后，非绑定者将无法设置软件");

            _refreshButton = new TextButton();
            _refreshButton.Text = "刷新二维码";
            _refreshButton.Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Pixel);
            _refreshButton.ForeColor = Color.FromArgb(0x4f, 0xb5, 0x2c);
            Controls.Add(_refreshButton);
            _refreshButton.Location = new Point(Width / 2 - _refreshButton.Width / 2,
                _whyLabel.Location.Y + _whyLabel.Height);
            _refreshButton.Click += _refreshButtonOnClick;

            _closeBackImage = new Panel();
            _closeBackImage.BackColor = Color.Transparent;
            _closeBackImage.BackgroundImage = Properties.Resources.login_btn_Close_n;
            _closeBackImage.Size = _closeBackImage.BackgroundImage.Size;
            _closeBackImage.Location = new Point(Width - _closeBackImage.Width, 0);
            Controls.Add(_closeBackImage);

            _closeBtn = new Label();
            //_closeBtn.BackColor = Color.Red;
            _closeBtn.Width = _closeBtn.Height = 24;
            _closeBtn.Location = new Point(_closeBackImage.Width - 4 - _closeBtn.Width, 4);
            _closeBackImage.Controls.Add(_closeBtn);
            _closeBtn.MouseEnter += _closeBtnOnMouseEnter;
            _closeBtn.MouseLeave += _closeBtnOnMouseLeave;
            _closeBtn.MouseDown += _closeBtnOnMouseDown;
            _closeBtn.MouseUp += _closeBtnOnMouseUp;
            _closeBtn.Click += _closeBtnOnClick;
            tipLabel.SizeChanged += tipLabelOnSizeChanged;

            Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Pixel);
        }
        private void WechatForm_Load(object sender, EventArgs e)
        {
            //防止由于系统放大，这里自动缩放
            Width = 230;
            Height = 320;
            cancellationTokenSource = new CancellationTokenSource();
            getQrCodeTask = SetQrcodeAsync(cancellationTokenSource.Token);
        }

        void _whyLabelOnClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.whyWechatBindingUrl);
        }

        void tipLabelOnSizeChanged(object sender, EventArgs e)
        {
            tipLabel.Location = new Point(Width / 2 - tipLabel.Width / 2,
                qrCodePictureBox.Location.Y + qrCodePictureBox.Height + 5);
        }

        void _closeBtnOnClick(object sender, EventArgs e)
        {
            Close();
        }

        void _closeBtnOnMouseUp(object sender, MouseEventArgs e)
        {
            _closeBackImage.BackgroundImage = Properties.Resources.login_btn_Close_n;
        }

        void _closeBtnOnMouseDown(object sender, MouseEventArgs e)
        {
            _closeBackImage.BackgroundImage = Properties.Resources.login_btn_Close_p;
        }

        void _closeBtnOnMouseLeave(object sender, EventArgs e)
        {
            _closeBackImage.BackgroundImage = Properties.Resources.login_btn_Close_n;
        }

        void _closeBtnOnMouseEnter(object sender, EventArgs e)
        {
            _closeBackImage.BackgroundImage = Properties.Resources.login_btn_Close_h;
        }

        void _refreshButtonOnClick(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            //getQrCodeTask.Wait();
            getQrCodeTask = SetQrcodeAsync(cancellationTokenSource.Token);
        }

        static async Task UpdateUserInfo(CancellationToken cancellationToken)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);
                JObject userInfoRequestObj = new JObject();
                userInfoRequestObj["token"] = webToken;
                userInfoRequestObj["openid"] = Properties.Settings.Default.openid;
                HttpResponseMessage msg = await client.PostAsync(Properties.Settings.Default.getUserInfoUrl,
                    new StringContent(userInfoRequestObj.ToString(), Encoding.UTF8, "application/json"),
                    cancellationToken);
                msg.EnsureSuccessStatusCode();
                string retStr = await msg.Content.ReadAsStringAsync();
                JObject retObj = JObject.Parse(retStr);
                JToken openidToken = retObj["openid"];
                JToken nicknameToken = retObj["nickname"];
                JToken headimgurlToken = retObj["headimgurl"];
                if (nicknameToken != null && nicknameToken.ToString() != "")
                {
                    string nickname = nicknameToken.ToString();
                    string headimgurl = headimgurlToken.ToString();
                    Properties.Settings.Default.userNickname = nickname;
                    Properties.Settings.Default.userHeadimgurl = headimgurl;
                    Properties.Settings.Default.Save();
                    WebClient downloadClient = new WebClient();
                    downloadClient.Proxy = null;
                    downloadClient.DownloadFile(headimgurl, Program.AppLocalDir + "UserHeadImage");
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.ToString());
            }
        }

        private async Task SetQrcodeAsync(CancellationToken cancellationToken)
        {
            try
            {
                qrCodePictureBox.Cursor = Cursors.WaitCursor;
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
                qrCodePictureBox.Cursor = Cursors.Default;
                //qrCodePictureBox.Image = bmp;

                //qrCodePictureBox.ImageLocation = string.Format(Properties.Settings.Default.wechatGetQrcodeUrl, HttpUtility.UrlEncode(ticket));
                //qrCodePictureBox.LoadAsync(string.Format(Properties.Settings.Default.wechatGetQrcodeUrl, HttpUtility.UrlEncode(ticket)));

                JObject sceneRequestObj = new JObject();
                sceneRequestObj["token"] = webToken;
                sceneRequestObj["scene_id"] = userid;
                while(true)
                {
                    //防止由于某次连接异常，导致无法继续的问题
                    //try
                    //{
                        msg = await client.PostAsync(Properties.Settings.Default.getSceneUrl,
                            new StringContent(sceneRequestObj.ToString(), Encoding.UTF8, "application/json"),
                            cancellationToken);
                        msg.EnsureSuccessStatusCode();
                        retStr = await msg.Content.ReadAsStringAsync();
                        retObj = JObject.Parse(retStr);
                        if (retObj["openid"] != null && retObj["openid"].ToString() != "")
                        {
                            string openid = retObj["openid"].ToString();
                            log.Info("scaned by: " + openid);
                            tipLabel.Text = "扫码成功，正在获取用户信息！";
                            Properties.Settings.Default.openid = openid;
                            Properties.Settings.Default.Save();
                            break;
                        }
                    //}
                    //catch(Exception exp)
                    //{ log.Error("error of getting scene: " + exp.ToString()); }
                    //await Task.Delay(2000);
                    log.Info("one time again for getting scene!");
                }
                while (true)
                {
                    //防止由于某次连接异常，导致无法继续的问题
                    //try
                    //{
                        JObject userInfoRequestObj = new JObject();
                        userInfoRequestObj["token"] = webToken;
                        userInfoRequestObj["openid"] = Properties.Settings.Default.openid;
                        msg = await client.PostAsync(Properties.Settings.Default.getUserInfoUrl,
                            new StringContent(userInfoRequestObj.ToString(), Encoding.UTF8, "application/json"),
                            cancellationToken);
                        msg.EnsureSuccessStatusCode();
                        retStr = await msg.Content.ReadAsStringAsync();
                        retObj = JObject.Parse(retStr);
                        JToken openidToken = retObj["openid"];
                        JToken nicknameToken = retObj["nickname"];
                        JToken headimgurlToken = retObj["headimgurl"];
                        if (nicknameToken != null && nicknameToken.ToString() != "")
                        {
                            string nickname = nicknameToken.ToString();
                            string headimgurl = headimgurlToken.ToString();
                            log.Info("binding by: " + nickname);
                            tipLabel.Text = "绑定成功！";
                            Properties.Settings.Default.userNickname = nickname;
                            Properties.Settings.Default.userHeadimgurl = headimgurl;
                            Properties.Settings.Default.Save();
                            try
                            {
                                WebClient downloadClient = new WebClient();
                                downloadClient.Proxy = null;
                                downloadClient.DownloadFile(headimgurl, Program.AppLocalDir + "UserHeadImage");
                            }
                            catch (Exception excep)
                            {
                                log.Error(excep.ToString());
                            }
                            BindingSuccess = true;
                            break;
                        }
                        else
                            tipLabel.Text = "请点击公众号绑定按钮完成绑定！";
                    //}
                    //catch(Exception exp)
                    //{ log.Error("Error of getting user info: " + exp.ToString()); }
                    log.Info("one time again for getting user info!");
                }
                //await Task.Delay(2000);
                log.Info("Binding Success!");
                Close();
            }
            catch (System.Net.Http.HttpRequestException reqErr)
            {
                AutoCloseMessageBox.ShowMessage(-1, "网络无法连接，请检查网络后点击刷新二维码重新绑定！");
                log.Error(reqErr.ToString());
            }
            catch(Exception err)
            {
                log.Error(err.ToString());
            }
        }

        private void WechatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //stop task, otherwise it will get the openid forever
            cancellationTokenSource.Cancel();
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
        public static async Task SendAuthRequestNotificationAsync()
        {
            //{{first.DATA}}
            //计算机名称：{{keyword1.DATA}}
            //请求时间：{{keyword2.DATA}}
            //{{remark.DATA}}
            JObject msgObj = new JObject();
            msgObj["touser"] = Properties.Settings.Default.openid;
            msgObj["template_id"] = Properties.Settings.Default.wechatTemplateAuthRequest;
            msgObj["url"] = Properties.Settings.Default.wechatAuthUrl;
            //msgObj["url"] = "http://www.shanyaows.com";
            msgObj["topcolor"] = "#FF0000";
            JObject dataObj = new JObject();
            msgObj["data"] = dataObj;
            dataObj["first"] = new JObject();
            dataObj["first"]["value"] = "您好，有特权操作需要授权。";
            dataObj["first"]["color"] = "#173177";
            dataObj["keyword1"] = new JObject();
            dataObj["keyword1"]["value"] = Environment.MachineName;
            dataObj["keyword1"]["color"] = "#173177";
            dataObj["keyword2"] = new JObject();
            dataObj["keyword2"]["value"] = DateTime.Now.ToString();
            dataObj["keyword2"]["color"] = "#173177";
            dataObj["remark"] = new JObject();
            dataObj["remark"]["value"] = "请点击本消息进行授权，然后在电脑上重新操作。";
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
