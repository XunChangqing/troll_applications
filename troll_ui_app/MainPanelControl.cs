using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using System.Diagnostics;
using System.Reflection;

namespace troll_ui_app
{
    public class MainPanelControl : UserControl
    {
        static readonly ILog log = Log.Get();
        static readonly int scanBtnTopPadding = 84;
        static readonly int scanBtnBottomPadding = 106;

        public static System.Timers.Timer BindingDetectTimer = new System.Timers.Timer();
        public static TimeSpan BindingDetectTimeLeft = TimeSpan.Zero;
        //public delegate void ScanEventHandler(object sender, ScanEventArgs e);
        //public event ScanEventHandler ScanEvent;
        TitleBarControl titleBar;
        Label mainFuncBtn;
        Panel guradNormalPanel;
        Label guardDescLabel;
        Label analysisItemsDescLabel;
        Label analysisItemsNumLabel;
        Label analysisResultDescLabel;
        TextButton analysisResultViewBtn;
        TextButton wechatBindingBtn;
        PictureBox wechatHeadImage;
        Label wechatNickname;
        TextButton wechatAuthCancelBtn;

        Panel scanningPanel;
        Label scanningLabel;
        TextButton viewScanningBtn;

        Panel scanBtnsPanel;
        ImageButton allScanBtn, fastScanBtn, customScanBtn;
        Label allScanLabel, fastScanLabel, customScanLabel;
        Panel statusPanel;
        Label versionLabel;
        Label wechatStatusLabel;
        Label wechatStatusIcon;
        TextButton checkUpdateBtn;

        TextButton qqGroupLabel;

        Label edgeAboveStatusPanel;
        Label edgeBelowStatusPanel;

        ContextMenuStrip mainPanelRightClickMenu;
        ToolStripMenuItem openProtectionPanel;
        ToolStripMenuItem openAllScan;
        ToolStripMenuItem openFastScan;
        ToolStripMenuItem openCustomScan;

        string qqGroupNum = "514824046";


        public enum ScanType { FastScan, AllScan, CustomScan };
        public class ScanEventArgs
        {
            public ScanEventArgs(ScanType st, List<string> dirs) { TypeScan = st; Dirs = dirs; }
            public List<String> Dirs { get; private set; } // readonly
            public ScanType TypeScan { get; private set; } // readonly
        }

        public MainPanelControl()
        {
            Location = new Point(0,0);
            Size = new System.Drawing.Size(MainForm.MainFormWidth, MainForm.MainFormHeight);
            //Dock = DockStyle.Fill;
            titleBar = new TitleBarControl();
            titleBar.Height = 252;

            mainFuncBtn = new Label();
            mainFuncBtn.BackColor = Color.Transparent;
            mainFuncBtn.Image = Properties.Resources.home_fh_icon_animation_h;
            mainFuncBtn.Size = mainFuncBtn.Image.Size;
            mainFuncBtn.Location = new Point(24, 60);
            mainFuncBtn.MouseEnter += mainFuncBtnOnMouseEnter;
            mainFuncBtn.MouseLeave += mainFuncBtnOnMouseLeave;
            titleBar.Controls.Add(mainFuncBtn);
            ToolTip mainFuncTip = new ToolTip();
            mainFuncTip.SetToolTip(mainFuncBtn, "设置防护功能和查看防护记录");
            titleBar.ProtectionCenterItem.Click += ProtectionCenterItemOnClick;

            wechatBindingBtn = new TextButton();
            wechatBindingBtn.BackColor = Color.Transparent;
            wechatBindingBtn.Text = "绑定微信";
            wechatBindingBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel); ;
            wechatBindingBtn.ForeColor = Color.FromArgb(0x4f, 0x4f, 0x00);
            wechatBindingBtn.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            wechatBindingBtn.Location = new Point(788, 214);
            titleBar.Controls.Add(wechatBindingBtn);
            wechatBindingBtn.Click += wechatBindingBtnOnClick;

            wechatHeadImage = new PictureBox();
            wechatHeadImage.SizeMode = PictureBoxSizeMode.Zoom;
            wechatHeadImage.Size = new System.Drawing.Size(32,32);
            wechatHeadImage.ImageLocation = Program.AppLocalDir + "UserHeadImage";
            //wechatHeadImage.Location = new Point(wechatxLocation, 
            //    mainFuncBtn.Location.Y+mainFuncBtn.Height/2-wechatHeadImage.Height/2);
            wechatHeadImage.Location = new Point(788, 214);
            titleBar.Controls.Add(wechatHeadImage);
            ToolTip wechatHeadImageTip = new ToolTip();
            wechatHeadImageTip.SetToolTip(wechatHeadImage, "绑定者微信头像");

            wechatNickname = new Label();
            wechatNickname.BackColor = Color.Transparent;
            wechatNickname.AutoSize = true;
            //wechatNickname.TextAlign = ContentAlignment.MiddleCenter;
            wechatNickname.Text = Properties.Settings.Default.userNickname;
            wechatNickname.Font = new System.Drawing.Font("微软雅黑", 10, GraphicsUnit.Pixel);
            wechatNickname.ForeColor = Color.White;
            wechatNickname.MaximumSize = new System.Drawing.Size(80,0);
            wechatNickname.AutoEllipsis = true;
            titleBar.Controls.Add(wechatNickname);
            ToolTip wechatNicknameTip = new ToolTip();
            wechatNicknameTip.SetToolTip(wechatNickname, "绑定者微信昵称");

            guradNormalPanel = new Panel();
            guradNormalPanel.BackColor = Color.Transparent;
            guradNormalPanel.AutoSize = true;
            guradNormalPanel.Location = new Point(mainFuncBtn.Location.X+mainFuncBtn.Width, 120);
            titleBar.Controls.Add(guradNormalPanel);

            guardDescLabel = new Label();
            guardDescLabel.AutoSize = true;
            guardDescLabel.Text = "山妖卫士正在守护您的电脑";
            guardDescLabel.Font = new System.Drawing.Font("微软雅黑", 22, GraphicsUnit.Pixel);
            guardDescLabel.ForeColor = Color.White;
            guradNormalPanel.Controls.Add(guardDescLabel);

            analysisItemsDescLabel = new Label();
            analysisItemsDescLabel.AutoSize = true;
            analysisItemsDescLabel.Text = "已监控项目：";
            analysisItemsDescLabel.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            analysisItemsDescLabel.ForeColor = Color.White;
            guradNormalPanel.Controls.Add(analysisItemsDescLabel);

            analysisItemsNumLabel = new Label();
            analysisItemsNumLabel.AutoSize = true;
            analysisItemsNumLabel.Text = "0";
            analysisItemsNumLabel.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            analysisItemsNumLabel.ForeColor = Color.White;
            guradNormalPanel.Controls.Add(analysisItemsNumLabel);

            analysisResultViewBtn = new TextButton();
            analysisResultViewBtn.Text = "立即查看";
            analysisResultViewBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel); ;
            analysisResultViewBtn.ForeColor = Color.FromArgb(0x4f, 0x4f, 0x00);
            analysisResultViewBtn.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            guradNormalPanel.Controls.Add(analysisResultViewBtn);
            analysisResultViewBtn.Click += mainFuncBtnOnClick;

            analysisResultDescLabel = new Label();
            analysisResultDescLabel.AutoSize = true;
            analysisResultDescLabel.Text = "新检测到不良项目！";
            analysisResultDescLabel.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            analysisResultDescLabel.ForeColor = Color.FromArgb(0x4f, 0x4f, 0x00);
            analysisResultDescLabel.SizeChanged += analysisResultDescLabelOnSizeChanged;
            guradNormalPanel.Controls.Add(analysisResultDescLabel);

            //analysisItemsDescLabel.Visible = false;
            //analysisItemsNumLabel.Visible = false;
            analysisResultDescLabel.Visible = false;
            analysisResultViewBtn.Visible = false;

            scanningPanel = new Panel();
            scanningPanel .BackColor = Color.Transparent;
            scanningPanel .AutoSize = true;
            scanningPanel .Location = new Point(mainFuncBtn.Location.X+mainFuncBtn.Width, 128);
            titleBar.Controls.Add(scanningPanel);

            scanningLabel = new Label();
            scanningLabel.AutoSize = true;
            scanningLabel.Text = "正在进行本地扫描...";
            scanningLabel.Font = new System.Drawing.Font("微软雅黑", 22, GraphicsUnit.Pixel);
            scanningLabel.ForeColor = Color.White;
            scanningLabel.Location = new Point(0, 0);
            scanningPanel.Controls.Add(scanningLabel);

            viewScanningBtn = new TextButton();
            viewScanningBtn.Text = "查看详情";
            viewScanningBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);;
            viewScanningBtn.ForeColor = Color.FromArgb(0x4f, 0x4f, 0x00);
            viewScanningBtn.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            scanningPanel.Controls.Add(viewScanningBtn);
            viewScanningBtn.Click += viewScanningBtnOnClick;

            scanBtnsPanel = new Panel();
            scanBtnsPanel.BackColor = Color.FromArgb(0xf8, 0xf8, 0xf8);
            scanBtnsPanel.Size = new System.Drawing.Size(MainForm.MainFormWidth, 318);
            scanBtnsPanel.Location = new Point(0, titleBar.Height);
            Controls.Add(scanBtnsPanel);

            allScanBtn = new ImageButton();
            allScanBtn.Image = Properties.Resources.home_scanner_qp_n;
            allScanBtn.Size = allScanBtn.Image.Size;
            allScanBtn.NormalBack = Properties.Resources.home_scanner_qp_n;
            allScanBtn.HoverBack = Properties.Resources.home_scanner_qp_h;
            allScanBtn.PressBack = Properties.Resources.home_scanner_qp_n;
            allScanBtn.Location = new Point(130, scanBtnTopPadding);
            ToolTip allScanTip = new ToolTip();
            allScanTip.AutoPopDelay = 32000;
            allScanTip.SetToolTip(allScanBtn, "扫描所有磁盘上的图片和视频");

            fastScanBtn = new ImageButton();
            fastScanBtn .Image = Properties.Resources.home_scanner_ks_n;
            fastScanBtn .Size = fastScanBtn .Image.Size;
            fastScanBtn .NormalBack = Properties.Resources.home_scanner_ks_n;
            fastScanBtn .HoverBack = Properties.Resources.home_scanner_ks_h;
            fastScanBtn .PressBack = Properties.Resources.home_scanner_ks_n;
            fastScanBtn .Location = new Point(allScanBtn.Location.X+allScanBtn.Width*2, scanBtnTopPadding);
            ToolTip fastScanTip = new ToolTip();
            fastScanTip.AutoPopDelay = 32000;
            fastScanTip.SetToolTip(fastScanBtn, "扫描上网记录中的图片");

            customScanBtn = new ImageButton();
            customScanBtn.Image = Properties.Resources.home_scanner_zdy_n;
            customScanBtn.Size = customScanBtn .Image.Size;
            customScanBtn.NormalBack = Properties.Resources.home_scanner_zdy_n;
            customScanBtn.HoverBack = Properties.Resources.home_scanner_zdy_h;
            customScanBtn.PressBack = Properties.Resources.home_scanner_zdy_n;
            customScanBtn.Location = new Point(fastScanBtn.Location.X+fastScanBtn.Width*2, scanBtnTopPadding);
            ToolTip customScanTip = new ToolTip();
            customScanTip.AutoPopDelay = 32000;
            customScanTip.SetToolTip(customScanBtn, "扫描指定文件夹内的图片和视频");

            allScanLabel = new Label();
            allScanLabel.AutoSize = true;
            allScanLabel.Text = "全盘扫描";
            allScanLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);

            fastScanLabel = new Label();
            fastScanLabel.AutoSize = true;
            fastScanLabel.Text = "上网记录扫描";
            fastScanLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);

            customScanLabel = new Label();
            customScanLabel.AutoSize = true;
            customScanLabel.Text = "自定义扫描";
            customScanLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);

            scanBtnsPanel.Controls.Add(allScanBtn);
            scanBtnsPanel.Controls.Add(fastScanBtn);
            scanBtnsPanel.Controls.Add(customScanBtn);
            scanBtnsPanel.Controls.Add(allScanLabel);
            scanBtnsPanel.Controls.Add(fastScanLabel);
            scanBtnsPanel.Controls.Add(customScanLabel);

            edgeAboveStatusPanel = new Label();
            edgeAboveStatusPanel.BackColor = Color.FromArgb(0xee, 0xee, 0xee);
            edgeAboveStatusPanel.Size = new System.Drawing.Size(MainForm.MainFormWidth, 3);
            edgeAboveStatusPanel.Location = new Point(0, scanBtnsPanel.Location.Y + scanBtnsPanel.Height);

            edgeBelowStatusPanel = new Label();
            edgeBelowStatusPanel.BackColor = Color.FromArgb(0xee, 0xee, 0xee);
            edgeBelowStatusPanel.Size = new System.Drawing.Size(MainForm.MainFormWidth, 3);
            edgeBelowStatusPanel.Location = new Point(0, MainForm.MainFormHeight - edgeBelowStatusPanel.Height);

            statusPanel = new Panel();
            statusPanel.Location = new Point(0, edgeAboveStatusPanel.Location.Y + edgeAboveStatusPanel.Height);
            statusPanel.Size = new System.Drawing.Size(MainForm.MainFormWidth, edgeBelowStatusPanel.Location.Y - edgeAboveStatusPanel.Location.Y - edgeAboveStatusPanel.Height);
            statusPanel.BackColor = Color.White;

            checkUpdateBtn = new TextButton();
            //checkUpdateBtn.BackColor = Color.Red;
            checkUpdateBtn.Text = "检查更新";
            checkUpdateBtn.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            checkUpdateBtn.ForeColor = Color.FromArgb(0x4f, 0xb5, 0x2c);
            checkUpdateBtn.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            checkUpdateBtn.Click += checkUpdateBtnOnClick;
            statusPanel.Controls.Add(checkUpdateBtn);

            versionLabel = new Label();
            versionLabel.AutoSize = true;
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            Version curVersion = new Version(versionInfo.ProductVersion);
            versionLabel.Text = "程序版本 " + curVersion.ToString();
            versionLabel.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            versionLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            statusPanel.Controls.Add(versionLabel);

            qqGroupLabel = new TextButton();
            //qqGroupLabel.AutoSize = true;
            qqGroupLabel.Text = "反馈QQ群："+ qqGroupNum;
            qqGroupLabel.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            qqGroupLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            qqGroupLabel.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            //qqGroupLabel.Location = new Point(, statusPanel.Height / 2 - wechatStatusIcon.Height / 2);
            ToolTip qqGroupTip = new ToolTip();
            qqGroupTip.AutoPopDelay = 32000;
            qqGroupTip.InitialDelay = 10;
            qqGroupTip.SetToolTip(qqGroupLabel, "点击复制QQ群号");
            qqGroupLabel.Click += qqGroupLabelOnClick;
            statusPanel.Controls.Add(qqGroupLabel);

            wechatStatusIcon = new Label();
            //wechatStatusIcon.Image = Properties.Resources.wechatauth;
            wechatStatusIcon.Image = Properties.Resources.wechatnotauth;
            wechatStatusIcon.Size = wechatStatusIcon.Image.Size;
            wechatStatusIcon.Location = new Point(statusPanel.Width-12-wechatStatusIcon.Width, statusPanel.Height/2-wechatStatusIcon.Height/2);
            statusPanel.Controls.Add(wechatStatusIcon);

            wechatStatusLabel = new Label();
            wechatStatusLabel.AutoSize = true;
            wechatStatusLabel.Text = "微信未授权";
            wechatStatusLabel.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            wechatStatusLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            statusPanel.Controls.Add(wechatStatusLabel);
            wechatStatusLabel.SizeChanged += wechatStatusLabelOnSizeChanged;

            wechatAuthCancelBtn = new TextButton();
            wechatAuthCancelBtn.AutoSize = true;
            wechatAuthCancelBtn.Text = "取消微信授权";
            wechatAuthCancelBtn.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            wechatAuthCancelBtn.ForeColor = Color.FromArgb(0xee, 0x00, 0x00);
            wechatAuthCancelBtn.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            ToolTip wechatAuthCancelTip = new ToolTip();
            wechatAuthCancelTip.AutoPopDelay = 32000;
            wechatAuthCancelTip.InitialDelay = 10;
            wechatAuthCancelTip.SetToolTip(wechatAuthCancelBtn, "立刻取消微信授权");           
            wechatAuthCancelBtn.Click += wechatAuthCancelBtnOnClick;
            statusPanel.Controls.Add(wechatAuthCancelBtn);

            Controls.Add(titleBar);
            Controls.Add(statusPanel);

            Load += MainPanelControlOnLoad;
            fastScanBtn.Click += fastScanBtnOnClick;
            allScanBtn.Click += allScanBtnOnClick;
            customScanBtn.Click += customScanBtnOnClick;
            mainFuncBtn.Click += mainFuncBtnOnClick;
            MainForm.Instance.TargetProcessedProgress.ProgressChanged += TargetProcessedProgressOnProgressChanged;
            //Paint += MainPanelOnPaint;
            //RefreshWechatInfo();

            mainPanelRightClickMenu = new ContextMenuStrip();
            openProtectionPanel = new ToolStripMenuItem("防护中心");
            openAllScan = new ToolStripMenuItem("全盘扫描");
            openFastScan = new ToolStripMenuItem("上网记录扫描");
            openCustomScan = new ToolStripMenuItem("自定义扫描");
            openProtectionPanel.Click += mainFuncBtnOnClick;
            openAllScan.Click += allScanBtnOnClick;
            openFastScan.Click += fastScanBtnOnClick;
            openCustomScan.Click += customScanBtnOnClick;
            mainPanelRightClickMenu.Items.AddRange(new ToolStripItem[] { openProtectionPanel, openAllScan, openFastScan, openCustomScan });
            this.ContextMenuStrip = mainPanelRightClickMenu;

        }

        void wechatBindingBtnOnClick(object sender, EventArgs e)
        {
            WechatForm.GetInstance().Show();
            //StartBindingDetect();
        }
        public static void StartBindingDetect()
        {
            BindingDetectTimeLeft = TimeSpan.FromMinutes(2);
            BindingDetectTimer.Start();
        }
        public void RefreshWechatInfo()
        {
            if (Program.RealBindingSucess)
            {
                wechatNickname.Text = Properties.Settings.Default.userNickname;
                wechatHeadImage.ImageLocation = Program.AppLocalDir + "UserHeadImage";
                wechatNickname.Visible = true;
                wechatHeadImage.Visible = true;
                wechatBindingBtn.Visible = false;
                BindingDetectTimer.Stop();
            }
        }
        void wechatStatusLabelOnSizeChanged(object sender, EventArgs e)
        {
            wechatStatusLabel.Location = new Point(wechatStatusIcon.Location.X - wechatStatusLabel.Width, statusPanel.Height / 2 - wechatStatusLabel.Height / 2);
        }
        void analysisResultDescLabelOnSizeChanged(object sender, EventArgs e)
        {
            analysisResultViewBtn.Location = new Point(analysisResultDescLabel.Width, analysisResultDescLabel.Location.Y);
        }
        int TargetProcessedNum = 0;
        int PornDetectedNum = 0;
        void TargetProcessedProgressOnProgressChanged(object sender, PornDatabase.PornItemType e)
        {
            TargetProcessedNum++;
            analysisItemsNumLabel.Text = TargetProcessedNum.ToString();
            if(e != PornDatabase.PornItemType.Undefined)
            {
                log.Info("Show analysisview button!");
                PornDetectedNum++;
                string cont = "新检测到{0}个不良项目！";
                analysisResultDescLabel.Text = string.Format(cont, PornDetectedNum);
                analysisResultDescLabel.Visible = true;
                analysisResultViewBtn.Visible = true;
            }
            log.Info("Target Processed Progress, Item Type: " + e.ToString() + " Total Num: " + TargetProcessedNum + " Porn Num: " + PornDetectedNum);
        }
        public void ClearPornLogs()
        {
            PornDetectedNum = 0;
            analysisResultDescLabel.Visible = false;
            analysisResultViewBtn.Visible = false;
        }

        //public void AuthWechat(bool st)
        //{
        //    if (st)
        //    {
        //        wechatStatusLabel.Text = "微信已授权";
        //        wechatStatusIcon.Image = Properties.Resources.wechatauth;
        //    }
        //    else
        //    { 
        //        wechatStatusLabel.Text = "微信未授权";
        //        wechatStatusIcon.Image = Properties.Resources.wechatnotauth;
        //    }
        //}
        public void UpdateAuthStatus(TimeSpan authExpiredTimeLeft)
        {
            if (authExpiredTimeLeft>TimeSpan.Zero)
            {
                wechatStatusLabel.Text = "微信已授权 "+authExpiredTimeLeft.ToString();
                wechatStatusIcon.Image = Properties.Resources.wechatauth;
                wechatAuthCancelBtn.Visible = true;
            }
            else
            { 
                wechatStatusLabel.Text = "微信未授权";
                wechatStatusIcon.Image = Properties.Resources.wechatnotauth;
                wechatAuthCancelBtn.Visible = false;
            }
        }

        void checkUpdateBtnOnClick(object sender, EventArgs e)
        {
            UpdateInfoForm updateInfoForm = new UpdateInfoForm();
            Task t = updateInfoForm.UpdateProduct();
            updateInfoForm.Show();
            //UpdateInfoForm.GetInstance().Show();
            //UpdateInfoForm.GetInstance().WindowState = FormWindowState.Normal;
        }

        void qqGroupLabelOnClick(object sender, EventArgs e)
        {
            //复制qq群号到剪贴板
            Clipboard.SetDataObject(qqGroupNum);
            MessageBox.Show("QQ群号已复制，反馈群期待您的到来！");
        }

        void wechatAuthCancelBtnOnClick(object sender, EventArgs e)
        {
            WechatForm.AuthExpiredTimeLeft = TimeSpan.Zero;
        }

        

        public void EnterScanStatus(string desc)
        {
            guradNormalPanel.Visible = false;
            scanningLabel.Text = desc;
            scanningPanel.Visible = true;
            mainFuncBtn.Image = Properties.Resources.home_fh_icon_animation_n;
        }

        public void ExitScanStatus()
        {
            scanningPanel.Visible = false;
            guradNormalPanel.Visible = true;
            mainFuncBtn.Image = Properties.Resources.home_fh_icon_animation_h;
        }

        void viewScanningBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Instance.SlideWindow(MainForm.Instance.scanPanelControl);
        }

        void MainPanelControlOnLoad(object sender, EventArgs e)
        {
            //确保加载时像素尺寸不发生变
            Size = new System.Drawing.Size(MainForm.MainFormWidth, MainForm.MainFormHeight);

            wechatNickname.Location = new Point(wechatHeadImage.Location.X+wechatHeadImage.Width+2,
                wechatHeadImage.Location.Y+wechatHeadImage.Height/2-wechatNickname.Height/2);

            wechatAuthCancelBtn.Visible = false;

            if (Properties.Settings.Default.openid == "" ||
                Properties.Settings.Default.userNickname == "")
            {
                wechatNickname.Visible = false;
                wechatHeadImage.Visible = false;
                wechatBindingBtn.Visible = true;
            }
            else
            {
                wechatNickname.Visible = true;
                wechatHeadImage.Visible = true;
                wechatBindingBtn.Visible = false;
            }

            checkUpdateBtn.Location = new Point(statusPanel.Width / 2 - checkUpdateBtn.Width / 2, statusPanel.Height/2 - checkUpdateBtn.Height/2);
            versionLabel.Location = new Point(10, statusPanel.Height / 2 - versionLabel.Height / 2);
            qqGroupLabel.Location = new Point(versionLabel.Location.X + versionLabel.Width + 20, statusPanel.Height / 2 - qqGroupLabel.Height / 2);
            wechatStatusLabel.Location = new Point(wechatStatusIcon.Location.X - wechatStatusLabel.Width, statusPanel.Height / 2 - wechatStatusLabel.Height / 2);
            wechatAuthCancelBtn.Location = new Point(wechatStatusLabel.Location.X - wechatAuthCancelBtn.Width - 55, statusPanel.Height / 2 - wechatAuthCancelBtn.Height / 2);

            allScanLabel.Location = new Point(allScanBtn.Location.X + allScanBtn.Width / 2 - allScanLabel.Width / 2, allScanBtn.Location.Y+allScanBtn.Height);
            fastScanLabel.Location = new Point(fastScanBtn.Location.X + fastScanBtn.Width / 2 - fastScanLabel.Width / 2, fastScanBtn.Location.Y+fastScanBtn.Height);
            customScanLabel.Location = new Point(customScanBtn.Location.X + customScanBtn.Width / 2 - customScanLabel.Width / 2, customScanBtn.Location.Y+customScanBtn.Height);

            guardDescLabel.Location = new Point(0,0);
            analysisItemsDescLabel.Location = new Point(0, guardDescLabel.Location.Y + guardDescLabel.Height+5);
            analysisItemsNumLabel.Location = new Point(analysisItemsDescLabel.Width, analysisItemsDescLabel.Location.Y);

            analysisResultDescLabel.Location = new Point(0, analysisItemsDescLabel.Location.Y + analysisItemsDescLabel.Height + 5);
            analysisResultViewBtn.Location = new Point(analysisResultDescLabel.Width, analysisResultDescLabel.Location.Y);

            viewScanningBtn.Location = new Point(0, scanningLabel.Height);
            //guradNormalPanel.Visible = false;
            scanningPanel.Visible = false;

            BindingDetectTimer = new System.Timers.Timer(500);
            BindingDetectTimer.AutoReset = true;
            BindingDetectTimer.Enabled = false;
            BindingDetectTimer.Elapsed += BindingDectectTimerOnElapsed;
            WechatForm.StartAuthDetect();
        }
        static void BindingDectectTimerOnElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            BindingDetectTimeLeft = BindingDetectTimeLeft.Subtract(TimeSpan.FromSeconds(0.5));
            MainForm.Instance.mainPanelControl.RefreshWechatInfo();
            if (BindingDetectTimeLeft <= TimeSpan.Zero)
                BindingDetectTimer.Stop();      
        }
        void mainFuncBtnOnMouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }
        void mainFuncBtnOnMouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }
        void MainPanelOnPaint(object sender, PaintEventArgs e)
        {
            // Create pen.
            Pen blackPen = new Pen(Color.Black, 1);

            // Create points that define line.
            Point point1 = new Point(0, FindForm().Height - 20);
            Point point2 = new Point(FindForm().Width, FindForm().Height - 20);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
        }

        void fastScanBtnOnClick(object sender, EventArgs e)
        {
            if (WechatForm.Auth())
                MainForm.Instance.scanPanelControl.StartFastScan();
        }
        void allScanBtnOnClick(object sender, EventArgs e)
        {
            if (WechatForm.Auth())
                MainForm.Instance.scanPanelControl.StartAllScan();
        }
        void customScanBtnOnClick(object sender, EventArgs e)
        {
            if (WechatForm.Auth())
            {
                MainForm.Instance.scanPanelControl.StartCustomScan();
            }
        }
        void mainFuncBtnOnClick(object sender, EventArgs e)
        {
            if(WechatForm.Auth())
                MainForm.Instance.SlideWindow(MainForm.Instance._protectionPanelControl);
        }
        void ProtectionCenterItemOnClick(object sender, EventArgs e)
        {
            if(WechatForm.Auth())
                MainForm.Instance.SlideWindow(MainForm.Instance._protectionPanelControl);
        }

    }
}
