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

namespace troll_ui_app
{
    public class ScanPanelControl : UserControl
    {
        static readonly ILog log = Log.Get();
        TitleBarControl titleBar;
        Panel _scanningStatusPanel;
        Label _scanningLogo;
        Label _progressLabel;
        ProgressBar _progressBar;
        Panel _returnBackImage;
        Label _returnBtn;
        ImageButton _pauseBtn;
        ImageButton _stopBtn;
        Label _scanStatusSummary;

        Panel _scanningResultPanel;
        Label _scanningResultLogo;
        Label _scanningResultTitleWarning;
        Label _scanningResultTitleSucceed;
        ImageButton _handleBtn;
        ImageButton _giveupBtn;
        ImageButton _confirmBtn;
        Label _scanResultSummary;

        PornItemTableViewWithPreview _pornItemResultView;

        Label _divider;
        Panel _footerPanel;
        //CheckBox _autoShutdownCheckBox;
        //Label _autoShutdownDescLabel;
        public LocalScan _localScan;

        int _totalTargetNum;
        int _totalPornNum;
        int _totalHandledNum;
        public enum ScanStatus {Idle, AllScan, FastScan, CustomScan};
        public ScanStatus Status { get; set; }
        public ScanPanelControl()
        {
            //InitializeComponent();
            //Dock = DockStyle.Fill;
            Location = new Point(0,0);
            Size = new System.Drawing.Size(MainForm.MainFormWidth, MainForm.MainFormHeight);
            titleBar = new TitleBarControl();
            Controls.Add(titleBar);

            _scanningStatusPanel = new Panel();
            _scanningStatusPanel.BackgroundImage = Properties.Resources.top_gray_bg;
            _scanningStatusPanel.BackgroundImageLayout = ImageLayout.Tile;
            _scanningStatusPanel.Size = new System.Drawing.Size(MainForm.MainFormWidth, 106);
            _scanningStatusPanel.Location = new Point(0, titleBar.Height);
            Controls.Add(_scanningStatusPanel);

            _scanningLogo = new Label();
            //_scanningLogo.Image = Properties.Resources.scanning;
            _scanningLogo.Image = Properties.Resources.scanning_work;
            _scanningLogo.Size = _scanningLogo.Image.Size;
            _scanningLogo.BackColor = Color.Transparent;
            _scanningLogo.Location = new Point(32, 20);
            _scanningStatusPanel.Controls.Add(_scanningLogo);

            _progressLabel = new Label();
            _progressLabel.Text = "正在准备";
            _progressLabel.ForeColor = Color.FromArgb(0x47, 0x47, 0x47);
            _progressLabel.BackColor = Color.Transparent;
            _progressLabel.Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Pixel);
            _progressLabel.TextAlign = ContentAlignment.MiddleLeft;
            _progressLabel.AutoEllipsis = true;
            _progressLabel.Width = 750;
            //_progressLabel.MaximumSize = new System.Drawing.Size(750, 0);
            //_progressLabel.AutoSize = true;
            _progressLabel.SizeChanged += _progressLabelOnSizeChanged;
            _scanningStatusPanel.Controls.Add(_progressLabel);

            _progressBar = new ProgressBar();
            _progressBar.Value = 90;
            _progressBar.Size = new System.Drawing.Size(600, 14);
            _progressBar.Location = new Point(20, 52);
            _scanningStatusPanel.Controls.Add(_progressBar);

            _pauseBtn = new ImageButton();
            _pauseBtn.Text = "暂停";
            _pauseBtn.Image = Properties.Resources.btn_primary_n;
            _pauseBtn.Size = _pauseBtn.Image.Size;
            _pauseBtn.NormalBack = Properties.Resources.btn_primary_n;
            _pauseBtn.HoverBack = Properties.Resources.btn_primary_h;
            _pauseBtn.PressBack = Properties.Resources.btn_primary_p;
            _pauseBtn.ForeColor = Color.WhiteSmoke;
            _pauseBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            _pauseBtn.TextAlign = ContentAlignment.MiddleCenter;
            _pauseBtn.Location = new Point(_progressBar.Location.X + _progressBar.Width + 32,
                52 + _progressBar.Height / 2 - _pauseBtn.Height / 2);
            _scanningStatusPanel.Controls.Add(_pauseBtn);

            _stopBtn = new ImageButton();
            _stopBtn.Text = "停止";
            _stopBtn.Image = Properties.Resources.btn_primary_n;
            _stopBtn.Size = _stopBtn.Image.Size;
            _stopBtn.NormalBack = Properties.Resources.btn_primary_n;
            _stopBtn.HoverBack = Properties.Resources.btn_primary_h;
            _stopBtn.PressBack = Properties.Resources.btn_primary_p;
            _stopBtn.ForeColor = Color.WhiteSmoke;
            _stopBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            _stopBtn.TextAlign = ContentAlignment.MiddleCenter;
            _stopBtn.Location = new Point(_pauseBtn.Location.X + _pauseBtn.Width + 12,
                _pauseBtn.Location.Y);
            _scanningStatusPanel.Controls.Add(_stopBtn);

            _scanStatusSummary = new Label();
            _scanStatusSummary.Text = "共扫描sdfsdfs";
            _scanStatusSummary.AutoSize = true;
            _scanStatusSummary.Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Pixel);
            _scanStatusSummary.BackColor = Color.Transparent;
            _scanStatusSummary.TextAlign = ContentAlignment.MiddleLeft;
            _scanStatusSummary.ForeColor = Color.FromArgb(0x47, 0x47, 0x47);
            _scanStatusSummary.Location = new Point(32,
                _progressBar.Location.Y + _progressBar.Height);
            _scanningStatusPanel.Controls.Add(_scanStatusSummary);

            _returnBackImage = new Panel();
            _returnBackImage.BackColor = Color.Transparent;
            _returnBackImage.BackgroundImage = Properties.Resources.btn_return_n;
            _returnBackImage.Width = _returnBackImage.Height = 64;
            _returnBackImage.Location = new Point(MainForm.MainFormWidth - _returnBackImage.Width, 0);
            _scanningStatusPanel.Controls.Add(_returnBackImage);

            _returnBtn = new Label();
            //_returnBtn.BackColor = Color.Red;
            _returnBtn.Width = _returnBtn.Height = 24;
            _returnBtn.Location = new Point(_returnBackImage.Width - 8 - _returnBtn.Width, 6);
            _returnBackImage.Controls.Add(_returnBtn);
            _returnBtn.MouseEnter += _returnBtnOnMouseEnter;
            _returnBtn.MouseLeave += _returnBtnOnMouseLeave;
            _returnBtn.MouseDown += _returnBtnOnMouseDown;
            _returnBtn.MouseUp += _returnBtnOnMouseUp;

            //scanning result panel
            _scanningResultPanel = new Panel();
            _scanningResultPanel .BackgroundImage = Properties.Resources.top_gray_bg;
            _scanningResultPanel .BackgroundImageLayout = ImageLayout.Tile;
            _scanningResultPanel .Size = new System.Drawing.Size(MainForm.MainFormWidth, 106);
            _scanningResultPanel .Location = new Point(0, titleBar.Height);
            Controls.Add(_scanningResultPanel);

            _scanningResultLogo = new Label();
            _scanningResultLogo.Image = Properties.Resources.warning;
            _scanningResultLogo.Image = Properties.Resources.Complete;
            _scanningResultLogo.Size = _scanningResultLogo.Image.Size;
            _scanningResultLogo.Location = new Point(32, 20);
            _scanningResultPanel.Controls.Add(_scanningResultLogo);

            _scanningResultTitleWarning = new Label();
            _scanningResultTitleWarning.Text = "本次扫描发现0个待处理文件！";
            _scanningResultTitleWarning.AutoSize = true;
            _scanningResultTitleWarning.BackColor = Color.Transparent;
            _scanningResultTitleWarning.TextAlign = ContentAlignment.MiddleLeft;
            _scanningResultTitleWarning.Font = new System.Drawing.Font("微软雅黑", 18, GraphicsUnit.Pixel);
            _scanningResultTitleWarning.ForeColor = Color.FromArgb(0xcf, 0x49, 0x2c);
            _scanningResultTitleWarning.Location = new Point(120, 30);
            _scanningResultPanel.Controls.Add(_scanningResultTitleWarning);

            _scanningResultTitleSucceed = new Label();
            _scanningResultTitleSucceed.Text = "已成功处理扫描中发现的文件！";
            _scanningResultTitleSucceed.AutoSize = true;
            _scanningResultTitleSucceed.BackColor = Color.Transparent;
            _scanningResultTitleSucceed.TextAlign = ContentAlignment.MiddleLeft;
            _scanningResultTitleSucceed.Font = new System.Drawing.Font("微软雅黑", 18, GraphicsUnit.Pixel);
            _scanningResultTitleSucceed.ForeColor = Color.FromArgb(0x4f, 0xb5, 0x2c);
            _scanningResultTitleSucceed.Location = new Point(120, 30);
            _scanningResultPanel.Controls.Add(_scanningResultTitleSucceed);

            _scanResultSummary = new Label();
            _scanResultSummary.Text = "";
            _scanResultSummary.AutoSize = true;
            _scanResultSummary.Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Pixel);
            _scanResultSummary.BackColor = Color.Transparent;
            _scanResultSummary.TextAlign = ContentAlignment.MiddleLeft;
            _scanResultSummary.ForeColor = Color.FromArgb(0x84, 0x84, 0x84);
            _scanResultSummary.Location = new Point(120,
                _scanningResultTitleWarning.Location.Y + _scanningResultTitleWarning.Height);
            _scanningResultPanel.Controls.Add(_scanResultSummary);

            _handleBtn = new ImageButton();
            _handleBtn.Text = "一键处理";
            _handleBtn.Image = Properties.Resources.btn_primary_n;
            _handleBtn.Size = _handleBtn.Image.Size;
            _handleBtn.NormalBack = Properties.Resources.btn_primary_n;
            _handleBtn.HoverBack = Properties.Resources.btn_primary_h;
            _handleBtn.PressBack = Properties.Resources.btn_primary_p;
            _handleBtn.ForeColor = Color.WhiteSmoke;
            _handleBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            _handleBtn.TextAlign = ContentAlignment.MiddleCenter;
            _handleBtn.Location = new Point(652, 42);
            _scanningResultPanel.Controls.Add(_handleBtn);

            _giveupBtn = new ImageButton();
            _giveupBtn.Text = "暂不处理";
            _giveupBtn.Image = Properties.Resources.btn_primary_n;
            _giveupBtn.Size = _giveupBtn.Image.Size;
            _giveupBtn.NormalBack = Properties.Resources.btn_primary_n;
            _giveupBtn.HoverBack = Properties.Resources.btn_primary_h;
            _giveupBtn.PressBack = Properties.Resources.btn_primary_p;
            _giveupBtn.ForeColor = Color.WhiteSmoke;
            _giveupBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            _giveupBtn.TextAlign = ContentAlignment.MiddleCenter;
            _giveupBtn.Location = new Point(_handleBtn.Location.X + _handleBtn.Width + 12, 42);
            _scanningResultPanel.Controls.Add(_giveupBtn);

            _confirmBtn = new ImageButton();
            _confirmBtn.Text = "确认";
            _confirmBtn.Image = Properties.Resources.btn_primary_n;
            _confirmBtn.Size = _confirmBtn.Image.Size;
            _confirmBtn.NormalBack = Properties.Resources.btn_primary_n;
            _confirmBtn.HoverBack = Properties.Resources.btn_primary_h;
            _confirmBtn.PressBack = Properties.Resources.btn_primary_p;
            _confirmBtn.ForeColor = Color.WhiteSmoke;
            _confirmBtn.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Pixel);
            _confirmBtn.TextAlign = ContentAlignment.MiddleCenter;
            _confirmBtn.Location = new Point(710, 42);
            _scanningResultPanel.Controls.Add(_confirmBtn);

            _divider = new Label();
            _divider.Size = new System.Drawing.Size(MainForm.MainFormWidth, 20);
            _divider.Location = new Point(0, _scanningStatusPanel.Location.Y + _scanningStatusPanel.Height);
            _divider.BackColor = Color.FromArgb(0xf8, 0xf8, 0xf8);
            Controls.Add(_divider);

            _pornItemResultView = new PornItemTableViewWithPreview(false);
            _pornItemResultView.Location = new Point(0, _divider.Location.Y + _divider.Height);
            _pornItemResultView.Height = MainForm.MainFormHeight - _pornItemResultView.Location.Y - 30;
            _pornItemResultView.Width = MainForm.MainFormWidth;
            Controls.Add(_pornItemResultView);

            _footerPanel = new Panel();
            _footerPanel.Size = new System.Drawing.Size(MainForm.MainFormWidth, 30);
            _footerPanel.Location = new Point(0, _pornItemResultView.Location.Y + _pornItemResultView.Height);
            _footerPanel.BackColor = Color.FromArgb(0xf8, 0xf8, 0xf8);
            Controls.Add(_footerPanel);

            //_autoShutdownCheckBox = new CheckBox();
            //_autoShutdownCheckBox.FlatStyle = FlatStyle.Flat;
            //_autoShutdownCheckBox.CheckAlign = ContentAlignment.MiddleCenter;
            //_autoShutdownCheckBox.Location = new Point(20, 12);
            //_footerPanel.Controls.Add(_autoShutdownCheckBox);

            //_autoShutdownDescLabel = new Label();
            //_autoShutdownDescLabel.Text = "扫描完成自动处理并关机";

            Load += ScanPanelControlOnLoad;
            _returnBtn.Click += _returnBtnOnClick;

            //_scanningStatusPanel.Visible = false;
            //_handleBtn.Visible = false;
            //_giveupBtn.Visible = false;
            _scanningResultPanel.Visible = false;
            _confirmBtn.Visible = false;

            _localScan = new LocalScan();
            _localScan.ScanProgressChanged += _localScanOnScanProgressChanged;
            _localScan.ScanComplete += _localScanOnScanComplete;

            _pauseBtn.Click += _pauseBtnOnClick;
            _stopBtn.Click += _stopBtnOnClick;
            _giveupBtn.Click += _giveupBtnOnClick;
            _handleBtn.Click += _handleBtnOnClick;
            _confirmBtn.Click += _confirmBtnOnClick;

            Disposed += ScanPanelControlOnDisposed;
        }

        void ScanPanelControlOnDisposed(object sender, EventArgs e)
        {
            log.Info("Dispose Scan Panel Control!");
            if(Status != ScanStatus.Idle)
            {
                log.Info("Stop Running Scan Task When Quit!");
                _localScan.Stop();
            }
        }

        void _progressLabelOnSizeChanged(object sender, EventArgs e)
        {
            _progressLabel.Location = new Point(_scanningLogo.Location.X + _scanningLogo.Width + 5,
                20 + _scanningLogo.Height / 2 - _progressLabel.Height / 2);
        }

        void _confirmBtnOnClick(object sender, EventArgs e)
        {
            Status = ScanStatus.Idle;
            MainForm.Instance.mainPanelControl.ExitScanStatus();
            MainForm.Instance.SlideWindow(this);
            MainForm.Instance.mainPanelControl.Refresh();
        }

        void AskForConfirmation()
        {
            _handleBtn.Visible = false;
            _giveupBtn.Visible = false;
            _confirmBtn.Visible = true;
            string summary = "共扫描对象：{0} 共检出项：{1} 共处理项：{2}";
            _scanResultSummary.Text = string.Format(summary, _totalTargetNum, _totalPornNum, _totalHandledNum);
        }
        void _handleBtnOnClick(object sender, EventArgs e)
        {
            _totalHandledNum = _pornItemResultView.ClearCheckedFiles();
            AskForConfirmation();
        }

        void _giveupBtnOnClick(object sender, EventArgs e)
        {
            AskForConfirmation();
        }

        bool _pause = false;

        void _stopBtnOnClick(object sender, EventArgs e)
        {
            _localScan.Stop();
        }

        void _pauseBtnOnClick(object sender, EventArgs e)
        {
            if (_pause)
            {
                _scanningLogo.Image = Properties.Resources.scanning_work;
                _localScan.Resume();
                _pauseBtn.Text = "暂停";
            }
            else
            {
                _scanningLogo.Image = Properties.Resources.scanning;
                _localScan.Pause();
                _pauseBtn.Text = "继续";
            }
            _pause = !_pause;
        }

        void _localScanOnScanComplete(object sender, EventArgs e)
        {
            if (_totalPornNum > 0)
            {
                _scanningStatusPanel.Visible = false;
                _scanningResultPanel.Visible = true;
                string summary = "共扫描对象：{0} 共检出项：{1}";
                _scanResultSummary.Text = string.Format(summary, _totalTargetNum, _totalPornNum);
                _scanningResultTitleWarning.Text = string.Format("本次扫描发现{0}个待处理文件！", _totalPornNum);
                _scanningResultPanel.Visible = true;
            }
            else
            {
                _scanningStatusPanel.Visible = false;
                _scanningResultPanel.Visible = true;
                AskForConfirmation();
            }
        }

        void _localScanOnScanProgressChanged(object sender, LocalScan.ScanProgress e)
        {
            _progressLabel.Text = e.Description;
            try
            { _progressBar.Value = e.Percentage; }
            catch(Exception exp)
            { log.Error(exp.ToString()); }
            if (e.TargetFilePath != null)
            {
                _totalTargetNum++;
                //if (e.TargetType == PornClassifier.ImageType.Porn)
                if (e.ItemType != PornDatabase.PornItemType.Undefined)
                {
                    _totalPornNum++;
                    _pornItemResultView.AddRow(e.TargetFilePath, e.ItemType,
                        PornDatabase.PornItemStatus.Normal);
                }
            }
            string summary = "共扫描对象：{0}    共检出项：{1}";
            _scanStatusSummary.Text = string.Format(summary, _totalTargetNum, _totalPornNum);
        }

        void InitForNewScan()
        {
            _pauseBtn.Text = "暂停";
            _totalTargetNum = 0;
            _totalPornNum = 0;
            _totalHandledNum = 0;
            _progressBar.Value = 0;
            _progressLabel.Text = "";
            _scanningStatusPanel.Visible = true;
            _scanningResultPanel.Visible = false;
            _handleBtn.Visible = true;
            _giveupBtn.Visible = true;
            _confirmBtn.Visible = false;
            _pornItemResultView.Clear();
            _scanningResultTitleWarning.Text = string.Format("本次扫描发现{0}个待处理文件！", _totalPornNum);
            string summary = "共扫描对象：{0}    共检出项：{1}";
            _scanStatusSummary.Text = string.Format(summary, _totalTargetNum, _totalPornNum);
        }

        public void StartFastScan()
        {
            if(Status == ScanStatus.Idle)
            {
                Status = ScanStatus.FastScan;
                SetTitle("山妖卫士-上网记录扫描");
                InitForNewScan();
                MainForm.Instance.mainPanelControl.EnterScanStatus("正在进行上网记录扫描...");
                MainForm.Instance.SlideWindow(this);
                MainForm.Instance.mainPanelControl.Refresh();
                _localScan.StartFastScan();
            }
            else if(Status == ScanStatus.FastScan)
            {
                MainForm.Instance.SlideWindow(this);
                MainForm.Instance.mainPanelControl.Refresh();
            }
            else
            {
                MessageBox.Show("有扫描任务正在进行，请等待扫描结束再启动新的扫描任务！");
            }
        }
        public void StartAllScan()
        {
            if(Status == ScanStatus.Idle)
            {
                Status = ScanStatus.AllScan;
                SetTitle("山妖卫士-全盘扫描");
                InitForNewScan();
                MainForm.Instance.mainPanelControl.EnterScanStatus("正在进行全盘扫描...");
                MainForm.Instance.SlideWindow(this);
                _localScan.StartAllScan();
            }
            else if(Status == ScanStatus.AllScan)
            {
                MainForm.Instance.SlideWindow(this);
            }
            else
            {
                MessageBox.Show("有扫描任务正在进行，请等待扫描结束再启动新的扫描任务！");
            }
        }

        public void StartCustomScan()
        {
            if(Status == ScanStatus.Idle)
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.ShowNewFolderButton = false;
                    DialogResult dr = folderDialog.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        log.Info("Custom Scan: " + folderDialog.SelectedPath);
                        //MainForm.Instance.scanPanelControl.StartCustomScan(folderDialog.SelectedPath);

                        Status = ScanStatus.CustomScan;
                        SetTitle("山妖卫士-自定义扫描");
                        InitForNewScan();
                        MainForm.Instance.mainPanelControl.EnterScanStatus("正在进行自定义扫描...");
                        MainForm.Instance.SlideWindow(this);
                        MainForm.Instance.mainPanelControl.Refresh();
                        _localScan.StartCustomScan(folderDialog.SelectedPath);
                    }
                }
            }
            else if(Status == ScanStatus.CustomScan)
            {
                MainForm.Instance.SlideWindow(this);
            }
            else
            {
                MessageBox.Show("有扫描任务正在进行，请等待扫描结束再启动新的扫描任务！");
            }
        }

        public void SetTitle(string title)
        {
            titleBar.TitleText = title;
        }

        void ScanPanelControlOnLoad(object sender, EventArgs e)
        {
            //确保加载时像素尺寸不发生变
            Size = new System.Drawing.Size(MainForm.MainFormWidth, MainForm.MainFormHeight);

            _pornItemResultView.Height = MainForm.MainFormHeight - _pornItemResultView.Location.Y - 30;
            _pornItemResultView.Width = MainForm.MainFormWidth;

            _progressLabel.Location = new Point(_scanningLogo.Location.X + _scanningLogo.Width + 5,
                20 + _scanningLogo.Height / 2 - _progressLabel.Height / 2);
        }

        void _returnBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Instance.SlideWindow(this);
            MainForm.Instance.mainPanelControl.Refresh();
        }
        void _returnBtnOnMouseUp(object sender, MouseEventArgs e)
        {
            _returnBackImage.BackgroundImage = Properties.Resources.btn_return_n;
        }

        void _returnBtnOnMouseDown(object sender, MouseEventArgs e)
        {
            _returnBackImage.BackgroundImage = Properties.Resources.btn_return_p;
        }

        void _returnBtnOnMouseLeave(object sender, EventArgs e)
        {
            _returnBackImage.BackgroundImage = Properties.Resources.btn_return_n;
        }

        void _returnBtnOnMouseEnter(object sender, EventArgs e)
        {
            _returnBackImage.BackgroundImage = Properties.Resources.btn_return_h;
        }
    }
}
