using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Titanium.Web.Proxy.Helpers;
using log4net;

namespace troll_ui_app
{
    public class ProtectionPanelControl : UserControl
    {
        static readonly ILog log = Log.Get();
        TitleBarControl _titleBar;

        Panel _settingPanel;
        Label _logoLabel;
        ImageSwitchButton _websiteBtn;
        Label _websiteDescLabel;
        ImageSwitchButton _activeImageBtn;
        Label _activeImageDescLabel;
        ImageSwitchButton _activeVideoBtn;
        Label _activeVideoDescLabel;
        ImageSwitchButton _networkImageBtn;
        Label _networkImageDescLabel;
        CheckBox _autoStartCheckbox;
        CheckBox _fastLocalScanIncrementalCheckbox;
        CheckBox _allLocalScanIncrementalCheckbox;
        CheckBox _strongNetworkImageFilter;
        Panel _returnBackImage;
        Label _returnBtn;

        Panel _tabHeaderPanel;

        PornItemTableViewWithPreview _pornItemTableView;
        //ImageButton _activeTabBtn;
        //ImageButton _pornWebsiteTabBtn;
        //ImageButton _pornFileTabBtn;

        //Panel _tabPornWebsiteContentPanel;
        //PornDataGridView _pornWebsiteTableView;

        //Panel _tabPornFileContentPanel;
        //PornItemTabelViewWithPreview _networkPornPicView;

        Panel _processPanel;
        ImageButton _clearAllBtn;
        static readonly int CheckBoxLocationX = 336 + 138 + 200;
        public ProtectionPanelControl()
        {
            //InitializeComponent();
            //Dock = DockStyle.Fill;
            Location = new Point(0,0);
            Size = new System.Drawing.Size(MainForm.MainFormWidth, MainForm.MainFormHeight);
            _titleBar = new TitleBarControl();
            //_titleBar.TitleText = "山妖卫士-安全防护中心";
            _titleBar.Height = 44;
            Controls.Add(_titleBar);

            _settingPanel = new Panel();
            Controls.Add(_settingPanel);
            //_settingPanel.BackColor = Color.Red;
            _settingPanel.BackgroundImage = Properties.Resources.top_green_bg;
            _settingPanel.BackgroundImageLayout = ImageLayout.Tile;
            _settingPanel.Height = 204;
            _settingPanel.Width = MainForm.MainFormWidth;
            _settingPanel.Location = new Point(0, _titleBar.Height);

            _logoLabel = new Label();
            //_logoLabel.Image = Properties.Resources.fh_icon_animation_n;
            SetLogoImage();
            _logoLabel.BackColor = Color.Transparent;
            _logoLabel.Height = 150;
            _logoLabel.Width = 174;
            _logoLabel.Location = new Point(54, 30);
            _settingPanel.Controls.Add(_logoLabel);

            int btnsX = 300;
            int heightDist = 30;
            _websiteBtn = new ImageSwitchButton(Properties.Settings.Default.IsPornWebsiteProtectionTurnOn,
                Properties.Resources.switch_on,
                Properties.Resources.switch_off);
            _websiteBtn.Location = new Point(btnsX, 36);
            _websiteDescLabel = new Label();
            _websiteDescLabel.AutoSize = true;
            _websiteDescLabel.Text = "不良网站检测防护";
            _websiteDescLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _websiteDescLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _websiteDescLabel.Location = new Point(btnsX + _websiteBtn.Width + 4,
                _websiteBtn.Location.Y+_websiteBtn.Height/2-_websiteDescLabel.Height/2);
            _websiteDescLabel.BackColor = Color.Transparent;
            _settingPanel.Controls.Add(_websiteBtn);
            _settingPanel.Controls.Add(_websiteDescLabel);
            _websiteBtn.SwitchChanged += _websiteBtnOnSwitchChanged;
            ToolTip websiteTip = new ToolTip();
            websiteTip.AutoPopDelay = 32000;
            websiteTip.SetToolTip(_websiteDescLabel, "自动检测色情网站并屏蔽");

            _activeImageBtn = new ImageSwitchButton(Properties.Settings.Default.IsLocalActiveImageTurnOn,
                Properties.Resources.switch_on,
                Properties.Resources.switch_off);
            _activeImageBtn.Location = new Point(btnsX, _websiteBtn.Location.Y+_websiteBtn.Height+heightDist);
            _activeImageDescLabel = new Label();
            _activeImageDescLabel.AutoSize = true;
            _activeImageDescLabel.Text = "本地活跃图片文件监控";
            _activeImageDescLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _activeImageDescLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _activeImageDescLabel.BackColor = Color.Transparent;
            _activeImageDescLabel.Location = new Point(btnsX + _activeImageBtn.Width + 4,
                _activeImageBtn.Location.Y+_activeImageBtn.Height/2-_activeImageDescLabel.Height/2);
            _settingPanel.Controls.Add(_activeImageBtn);
            _settingPanel.Controls.Add(_activeImageDescLabel);
            _activeImageBtn.SwitchChanged += _activeImageBtnOnSwitchChanged;
            if (Properties.Settings.Default.IsLocalActiveImageTurnOn)
                MainForm.Instance._activeFileMonitor.EnableImageDetection();
            ToolTip activImageTip = new ToolTip();
            activImageTip.AutoPopDelay = 32000;
            activImageTip.SetToolTip(_activeImageDescLabel, "监控本地活跃图片文件，记录本地色情图片文件的创建");

            _activeVideoBtn = new ImageSwitchButton(Properties.Settings.Default.IsLocalActiveVideoTurnOn,
                Properties.Resources.switch_on,
                Properties.Resources.switch_off);
            _activeVideoBtn.Location = new Point(btnsX, _activeImageBtn.Location.Y+_activeImageBtn.Height+heightDist);
            _activeVideoDescLabel = new Label();
            _activeVideoDescLabel.AutoSize = true;
            _activeVideoDescLabel.Text = "本地活跃视频文件监控";
            _activeVideoDescLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _activeVideoDescLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _activeVideoDescLabel.BackColor = Color.Transparent;
            _activeVideoDescLabel.Location = new Point(btnsX + _activeVideoBtn.Width + 4,
                _activeVideoBtn.Location.Y+_activeVideoBtn.Height/2-_activeVideoDescLabel.Height/2);
            _settingPanel.Controls.Add(_activeVideoBtn);
            _settingPanel.Controls.Add(_activeVideoDescLabel);
            _activeVideoBtn.SwitchChanged += _activeVideoBtnOnSwitchChanged;
            //_activeVideoBtn.Enabled = false;
            ToolTip activeVideoTip = new ToolTip();
            activeVideoTip.AutoPopDelay = 32000;
            activeVideoTip .SetToolTip(_activeVideoDescLabel, "监控本地活跃视频文件，记录本地色情视频文件的创建");
            if (Properties.Settings.Default.IsLocalActiveVideoTurnOn)
                MainForm.Instance._activeFileMonitor.EnableVideoDetection();

            _networkImageBtn = new ImageSwitchButton(Properties.Settings.Default.IsNetworkImageTurnOn,
                Properties.Resources.switch_on,
                Properties.Resources.switch_off);
            _networkImageBtn.Location = new Point(btnsX, _activeVideoBtn.Location.Y+_activeVideoBtn.Height+heightDist);
            _networkImageDescLabel = new Label();
            _networkImageDescLabel.AutoSize = true;
            _networkImageDescLabel.Text = "网页不良图片过滤";
            _networkImageDescLabel.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _networkImageDescLabel.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _networkImageDescLabel.BackColor = Color.Transparent;
            _networkImageDescLabel.Location = new Point(btnsX + _networkImageBtn.Width + 4,
                _networkImageBtn.Location.Y+_networkImageBtn.Height/2-_networkImageDescLabel.Height/2);
            _settingPanel.Controls.Add(_networkImageBtn);
            _settingPanel.Controls.Add(_networkImageDescLabel);
            _networkImageBtn.SwitchChanged += _networkImageBtnOnSwitchChanged;
            ToolTip networkImageTip = new ToolTip();
            networkImageTip.AutoPopDelay = 32000;
            networkImageTip.SetToolTip(_networkImageDescLabel, "过滤网页中的色情图片，此项功能在开启或关闭后，如页面未刷新，请使用Ctrl+F5强制刷新");

            _strongNetworkImageFilter = new CheckBox();
            _strongNetworkImageFilter.Location = new Point(CheckBoxLocationX,
                _networkImageDescLabel.Location.Y);
            _strongNetworkImageFilter.BackColor = Color.Transparent;
            _strongNetworkImageFilter.Text = "强力过滤";
            _strongNetworkImageFilter.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _strongNetworkImageFilter.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _strongNetworkImageFilter.BackColor = Color.Transparent;
            _strongNetworkImageFilter.Checked = Properties.Settings.Default.IsStrongNetworkImageFilter;
            if (!Properties.Settings.Default.IsNetworkImageTurnOn)
                _strongNetworkImageFilter.Enabled = false;
            _strongNetworkImageFilter.CheckedChanged += _strongNetworkImageFilterOnCheckedChanged;
            _settingPanel.Controls.Add(_strongNetworkImageFilter);
            ToolTip strongNetworkImageFilterTip = new ToolTip();
            strongNetworkImageFilterTip.AutoPopDelay = 32000;
            strongNetworkImageFilterTip.SetToolTip(_strongNetworkImageFilter, "包含擦边球色情图片，此项功能在开启或关闭后，如页面未刷新，请使用Ctrl+F5强制刷新");

            //自启动checkbox
            _autoStartCheckbox = new CheckBox();
            _autoStartCheckbox.Location = new Point(CheckBoxLocationX, _websiteDescLabel.Location.Y);
            _autoStartCheckbox.BackColor = Color.Transparent;
            _autoStartCheckbox.Text = "开机启动";
            _autoStartCheckbox.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _autoStartCheckbox.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _autoStartCheckbox.BackColor = Color.Transparent;
            _autoStartCheckbox.Checked = GetAutoStart();
            _autoStartCheckbox.CheckedChanged += _autoStartCheckboxOnCheckedChanged;
            _settingPanel.Controls.Add(_autoStartCheckbox);
            //ToolTip strongNetworkImageFilterTip = new ToolTip();
            //strongNetworkImageFilterTip.SetToolTip(_strongNetworkImageFilter, "包含擦边球色情图片");

            //增量快速扫描
            _fastLocalScanIncrementalCheckbox = new CheckBox();
            _fastLocalScanIncrementalCheckbox.Location = new Point(CheckBoxLocationX, _activeImageDescLabel.Location.Y);
            _fastLocalScanIncrementalCheckbox.BackColor = Color.Transparent;
            _fastLocalScanIncrementalCheckbox.Text = "增量上网记录扫描";
            _fastLocalScanIncrementalCheckbox.AutoSize = true;
            _fastLocalScanIncrementalCheckbox.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _fastLocalScanIncrementalCheckbox.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _fastLocalScanIncrementalCheckbox.BackColor = Color.Transparent;
            _fastLocalScanIncrementalCheckbox.Checked = Properties.Settings.Default.isFastLocalScanIncremental;
            _fastLocalScanIncrementalCheckbox.CheckedChanged += _fastLocalScanIncrementalCheckboxOnCheckedChanged;
            _settingPanel.Controls.Add(_fastLocalScanIncrementalCheckbox);

            //增量全盘扫描
            _allLocalScanIncrementalCheckbox = new CheckBox();
            _allLocalScanIncrementalCheckbox.Location = new Point(CheckBoxLocationX, _activeVideoDescLabel.Location.Y);
            _allLocalScanIncrementalCheckbox.BackColor = Color.Transparent;
            _allLocalScanIncrementalCheckbox.Text = "增量全盘扫描";
            _allLocalScanIncrementalCheckbox.AutoSize = true;
            _allLocalScanIncrementalCheckbox.ForeColor = Color.FromArgb(0x5e, 0x5e, 0x5e);
            _allLocalScanIncrementalCheckbox.Font = new System.Drawing.Font("微软雅黑", 16, GraphicsUnit.Pixel);
            _allLocalScanIncrementalCheckbox.BackColor = Color.Transparent;
            _allLocalScanIncrementalCheckbox.Checked = Properties.Settings.Default.isAllLocalScanIncremental;
            _allLocalScanIncrementalCheckbox.CheckedChanged += _allLocalScanIncrementalCheckboxOnCheckedChanged;
            _settingPanel.Controls.Add(_allLocalScanIncrementalCheckbox );

            _returnBackImage = new Panel();
            _returnBackImage.BackColor = Color.Transparent;
            _returnBackImage.BackgroundImage = Properties.Resources.btn_return_n;
            _returnBackImage.Width = _returnBackImage.Height = 64;
            _returnBackImage.Location = new Point(MainForm.MainFormWidth - _returnBackImage.Width, 0);
            _settingPanel.Controls.Add(_returnBackImage);

            _returnBtn = new Label();
            //_returnBtn.BackColor = Color.Red;
            _returnBtn.Width = _returnBtn.Height = 24;
            _returnBtn.Location = new Point(_returnBackImage.Width - 8 - _returnBtn.Width, 6);
            _returnBackImage.Controls.Add(_returnBtn);
            _returnBtn.MouseEnter += _returnBtnOnMouseEnter;
            _returnBtn.MouseLeave += _returnBtnOnMouseLeave;
            _returnBtn.MouseDown += _returnBtnOnMouseDown;
            _returnBtn.MouseUp += _returnBtnOnMouseUp;

            _tabHeaderPanel = new Panel();
            _tabHeaderPanel.BackColor = Color.FromArgb(0xf3, 0xf3, 0xf3);
            //_tabHeaderPanel.BackColor = Color.Green;
            _tabHeaderPanel.Height = 24;
            _tabHeaderPanel.Width = MainForm.MainFormWidth;
            _tabHeaderPanel.Location = new Point(0, _settingPanel.Location.Y + _settingPanel.Height);
            Controls.Add(_tabHeaderPanel);

            _pornItemTableView = new PornItemTableViewWithPreview(true);
            _pornItemTableView.Height = 264 + 30;
            _pornItemTableView.Width = MainForm.MainFormWidth;
            _pornItemTableView.Location = new Point(0, _tabHeaderPanel.Location.Y + _tabHeaderPanel.Height);

            Controls.Add(_pornItemTableView);

            _processPanel = new Panel();
            _processPanel.Width = MainForm.MainFormWidth;
            _processPanel.Height = 34;
            _processPanel.BackColor = Color.FromArgb(0xf3, 0xf3, 0xf3);
            _processPanel.Location = new Point(0, _pornItemTableView.Location.Y + _pornItemTableView.Height);
            Controls.Add(_processPanel);

            _clearAllBtn = new ImageButton();
            _clearAllBtn.Size = Properties.Resources.btn_Clear_n.Size;
            _clearAllBtn.Image = _clearAllBtn.NormalBack = Properties.Resources.btn_Clear_n;
            _clearAllBtn.HoverBack = Properties.Resources.btn_Clear_h;
            _clearAllBtn.PressBack = Properties.Resources.btn_Clear_p;
            _clearAllBtn.Location = new Point(32, _processPanel.Height / 2 - _clearAllBtn.Height/2);
            _processPanel.Controls.Add(_clearAllBtn);

            _clearAllBtn.Click += _clearAllBtnOnClick;
            _returnBtn.Click += _returnBtnOnClick;

            Load += ProtectionPanelControlOnLoad;
            SetSystemProxy();
        }
        void SetSystemProxy()
        {
            if (Properties.Settings.Default.IsNetworkImageTurnOn || Properties.Settings.Default.IsPornWebsiteProtectionTurnOn)
            {
#if !DEBUG
                try
                {
                    SystemProxyHelper.EnableProxyHTTP("127.0.0.1", 8090);
                    FireFoxHelper.AddFirefox();
                }
                catch(Exception e)
                {
                    log.Error(e.ToString());
                }
#endif
            }
            else
            {
#if !DEBUG
                try
                {
                    log.Info("Disable proxy!");
                    SystemProxyHelper.DisableAllProxyWithourRestore();
                    FireFoxHelper.RemoveFirefox();
                }
                catch (Exception exp) { log.Error(exp.ToString()); }
                try { ProxyRoutines.SetProxy(false); }
                catch (Exception exp) { log.Equals(exp.ToString()); }
#endif
            }
        }

        void ProtectionPanelControlOnLoad(object sender, EventArgs e)
        {
            //确保加载时像素尺寸不发生变
            Size = new System.Drawing.Size(MainForm.MainFormWidth, MainForm.MainFormHeight);
            _pornItemTableView.Height = 264 + 30;
            _pornItemTableView.Width = MainForm.MainFormWidth;
        }
        void _allLocalScanIncrementalCheckboxOnCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.isAllLocalScanIncremental = _allLocalScanIncrementalCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        void _fastLocalScanIncrementalCheckboxOnCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.isFastLocalScanIncremental = _fastLocalScanIncrementalCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        static readonly string kAutoRunRegisstryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        static readonly string kAutoRunKey = "trollwiz";
        bool GetAutoStart()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            var autostart = autorun_registry_key.GetValue(kAutoRunKey);
            if (autostart == null)
                return false;
            else
                return true;
        }
        void TurnOnAutoStartReg()
        {
            try
            {
                RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
                autorun_registry_key.SetValue(kAutoRunKey, "\"" + Application.ExecutablePath + " \"" + " -notvisible");
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
            //暂时断开消息处理函数，防止反复处理
            //_autoStartCheckbox.CheckedChanged -= _autoStartCheckboxOnCheckedChanged;
            //_autoStartCheckbox.Checked = true;
            //_autoStartCheckbox.CheckedChanged += _autoStartCheckboxOnCheckedChanged;
        }
        void TurnOffAutoStartReg()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            autorun_registry_key.DeleteValue(kAutoRunKey);
            //autorun_registry_key.SetValue(kAutoRunKey, "");
            //暂时断开消息处理函数，防止反复处理
            //_autoStartCheckbox.CheckedChanged -= _autoStartCheckboxOnCheckedChanged;
            //_autoStartCheckbox.Checked = false; ;
            //_autoStartCheckbox.CheckedChanged += _autoStartCheckboxOnCheckedChanged;
        }
        //void TurnOnAutoStart()
        //{
        //    _autoStartCheckbox.Checked = true;
        //}
        //void TurnOffAutoStart()
        //{
        //    _autoStartCheckbox.Checked = false;
        //}
        void _autoStartCheckboxOnCheckedChanged(object sender, EventArgs e)
        {
            log.Info("auto start checkbox checked changed!");
            if (_autoStartCheckbox.Checked)
                TurnOnAutoStartReg();
            else
                TurnOffAutoStartReg();
        }
        bool HasOneFuncWork()
        {
            if (Properties.Settings.Default.IsNetworkImageTurnOn ||
                Properties.Settings.Default.IsLocalActiveImageTurnOn ||
                Properties.Settings.Default.IsPornWebsiteProtectionTurnOn ||
                Properties.Settings.Default.IsLocalActiveVideoTurnOn)
                return true;
            else
                return false;
        }
        void SetLogoImage()
        {
            if(HasOneFuncWork())
                _logoLabel.Image = Properties.Resources.fh_icon_animation_n;
            else
                _logoLabel.Image = Properties.Resources.fh_icon_animation_d;
        }
        //当有功能设置变化时重新设置开机自启动
        //void SetAutoStart()
        //{
        //    if(HasOneFuncWork())
        //        TurnOnAutoStart();
        //    else
        //        TurnOffAutoStart();
        //}
        void _websiteBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsPornWebsiteProtectionTurnOn = e;
            Properties.Settings.Default.Save();
            SetSystemProxy();
            SetLogoImage();
            //SetAutoStart();
        }
        void _activeImageBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsLocalActiveImageTurnOn = e;
            Properties.Settings.Default.Save();
            SetLogoImage();
            if (e)
                MainForm.Instance._activeFileMonitor.EnableImageDetection();
            else
                MainForm.Instance._activeFileMonitor.DisableImageDetection();
            //SetAutoStart();
        }
        void _activeVideoBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsLocalActiveVideoTurnOn = e;
            Properties.Settings.Default.Save();
            SetLogoImage();
            if (e)
                MainForm.Instance._activeFileMonitor.EnableVideoDetection();
            else
                MainForm.Instance._activeFileMonitor.DisableVideoDetection();
            //SetAutoStart();
        }
        void _networkImageBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsNetworkImageTurnOn = e;
            Properties.Settings.Default.Save();
            SetSystemProxy();
            SetLogoImage();
            if (e)
                _strongNetworkImageFilter.Enabled = true;
            else
                _strongNetworkImageFilter.Enabled = false;
            //SetAutoStart();
        }
        void _strongNetworkImageFilterOnCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsStrongNetworkImageFilter = _strongNetworkImageFilter.Checked;
            Properties.Settings.Default.Save();
            //SetAutoStart();
        }

        void _returnBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Instance.SlideWindow(this);
            MainForm.Instance.mainPanelControl.Refresh();
        }

        void _clearAllBtnOnClick(object sender, EventArgs e)
        {
            _pornItemTableView.ClearAllPornItems();
            MainForm.Instance.mainPanelControl.ClearPornLogs();
        }

        //void _pornFileTabBtnOnClick(object sender, EventArgs e)
        //{
        //    if(_activeTabBtn != _pornFileTabBtn)
        //    {
        //        _activeTabBtn = _pornFileTabBtn;
        //        _pornWebsiteTabBtn.Image = _pornWebsiteTabBtn.NormalBack = Properties.Resources.btn_blwz_n;
        //        _pornFileTabBtn.Image = _pornFileTabBtn.NormalBack = Properties.Resources.btn_blwj_h;
        //        _tabPornWebsiteContentPanel.Visible = false;
        //        _tabPornFileContentPanel.Visible = true;
        //    }
        //}

        //void _pornWebsiteTabBtnOnClick(object sender, EventArgs e)
        //{
        //    if(_activeTabBtn != _pornWebsiteTabBtn)
        //    {
        //        _activeTabBtn = _pornWebsiteTabBtn;
        //        _pornWebsiteTabBtn.Image = _pornWebsiteTabBtn.NormalBack = Properties.Resources.btn_blwz_h;
        //        _pornFileTabBtn.Image = _pornFileTabBtn.NormalBack = Properties.Resources.btn_blwj_n;
        //        _tabPornWebsiteContentPanel.Visible = true;
        //        _tabPornFileContentPanel.Visible = false;
        //    }
        //}

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
