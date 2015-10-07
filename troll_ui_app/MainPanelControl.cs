using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    public class MainPanelControl : UserControl
    {
        static readonly int scanBtnTopPadding = 84;
        static readonly int scanBtnBottomPadding = 106;
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

        Label edgeAboveStatusPanel;
        Label edgeBelowStatusPanel;

        public enum ScanType { FastScan, AllScan, CustomScan };
        public class ScanEventArgs
        {
            public ScanEventArgs(ScanType st, List<string> dirs) { TypeScan = st; Dirs = dirs; }
            public List<String> Dirs { get; private set; } // readonly
            public ScanType TypeScan { get; private set; } // readonly
        }

        public MainPanelControl()
        {
            Dock = DockStyle.Fill;
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

            analysisResultDescLabel = new Label();
            analysisResultDescLabel.AutoSize = true;
            analysisResultDescLabel.Text = "检测到不良项目！";
            analysisResultDescLabel.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            analysisResultDescLabel.ForeColor = Color.FromArgb(0x4f, 0x4f, 0x00);
            guradNormalPanel.Controls.Add(analysisResultDescLabel);

            analysisResultViewBtn = new TextButton();
            analysisResultViewBtn.Text = "立即查看";
            analysisResultViewBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel); ;
            analysisResultViewBtn.ForeColor = Color.FromArgb(0x4f, 0x4f, 0x00);
            analysisResultViewBtn.HoverColor = Color.FromArgb(0xff, 0xa4, 0x05);
            guradNormalPanel.Controls.Add(analysisResultViewBtn);
            analysisResultViewBtn.Click += mainFuncBtnOnClick;

            analysisItemsDescLabel.Visible = false;
            analysisItemsNumLabel.Visible = false;
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

            fastScanBtn = new ImageButton();
            fastScanBtn .Image = Properties.Resources.home_scanner_ks_n;
            fastScanBtn .Size = fastScanBtn .Image.Size;
            fastScanBtn .NormalBack = Properties.Resources.home_scanner_ks_n;
            fastScanBtn .HoverBack = Properties.Resources.home_scanner_ks_h;
            fastScanBtn .PressBack = Properties.Resources.home_scanner_ks_n;
            fastScanBtn .Location = new Point(allScanBtn.Location.X+allScanBtn.Width*2, scanBtnTopPadding);

            customScanBtn = new ImageButton();
            customScanBtn.Image = Properties.Resources.home_scanner_zdy_n;
            customScanBtn.Size = customScanBtn .Image.Size;
            customScanBtn.NormalBack = Properties.Resources.home_scanner_zdy_n;
            customScanBtn.HoverBack = Properties.Resources.home_scanner_zdy_h;
            customScanBtn.PressBack = Properties.Resources.home_scanner_zdy_n;
            customScanBtn.Location = new Point(fastScanBtn.Location.X+fastScanBtn.Width*2, scanBtnTopPadding);
            customScanBtn.Enabled = false;

            allScanLabel = new Label();
            allScanLabel.AutoSize = true;
            allScanLabel.Text = "全盘扫描";
            allScanLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);

            fastScanLabel = new Label();
            fastScanLabel.AutoSize = true;
            fastScanLabel.Text = "快速扫描";
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
            versionLabel.Text = "程序版本 " + "0.9.0.23";
            versionLabel.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            versionLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            statusPanel.Controls.Add(versionLabel);

            wechatStatusIcon = new Label();
            wechatStatusIcon.Image = Properties.Resources.wechatauth;
            wechatStatusIcon.Size = wechatStatusIcon.Image.Size;
            wechatStatusIcon.Location = new Point(statusPanel.Width-12-wechatStatusIcon.Width, statusPanel.Height/2-wechatStatusIcon.Height/2);
            statusPanel.Controls.Add(wechatStatusIcon);

            wechatStatusLabel = new Label();
            wechatStatusLabel.AutoSize = true;
            wechatStatusLabel.Text = "微信已授权";
            wechatStatusLabel.Font = new System.Drawing.Font("微软雅黑", 11, GraphicsUnit.Pixel);
            wechatStatusLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            statusPanel.Controls.Add(wechatStatusLabel);

            Controls.Add(titleBar);
            Controls.Add(statusPanel);

            Load += MainPanelControlOnLoad;
            fastScanBtn.Click += fastScanBtnOnClick;
            allScanBtn.Click += allScanBtnOnClick;
            mainFuncBtn.Click += mainFuncBtnOnClick;
            //Paint += MainPanelOnPaint;
        }

        void checkUpdateBtnOnClick(object sender, EventArgs e)
        {
            UpdateInfoForm.GetInstance().Show();
            UpdateInfoForm.GetInstance().WindowState = FormWindowState.Normal;
        }

        public void EnterScanStatus()
        {
            guradNormalPanel.Visible = false;
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
            MainForm.Animate(MainForm.Instance.scanPanelControl, MainForm.Effect.Slide, 200, 180);
        }

        void MainPanelControlOnLoad(object sender, EventArgs e)
        {
            checkUpdateBtn.Location = new Point(statusPanel.Width / 2 - checkUpdateBtn.Width / 2, statusPanel.Height/2 - checkUpdateBtn.Height/2);
            versionLabel.Location = new Point(24, statusPanel.Height / 2 - versionLabel.Height / 2);
            wechatStatusLabel.Location = new Point(wechatStatusIcon.Location.X - wechatStatusLabel.Width, statusPanel.Height / 2 - wechatStatusLabel.Height / 2);

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
            //ScanEvent(this, new ScanEventArgs(ScanType.FastScan, null));
            //MainForm.Animate(MainForm.Instance.scanPanelControl, MainForm.Effect.Slide, 200, 180);
            MainForm.Instance.scanPanelControl.StartFastScan();
        }
        void allScanBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Instance.scanPanelControl.StartAllScan();
        }

        void mainFuncBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Animate(MainForm.Instance._protectionPanelControl, MainForm.Effect.Slide, 200, 180);
        }
    }
}
