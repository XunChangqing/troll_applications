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

namespace troll_ui_app
{
    public class ProtectionPanelControl : UserControl
    {
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
        public ProtectionPanelControl()
        {
            //InitializeComponent();
            Dock = DockStyle.Fill;
            _titleBar = new TitleBarControl();
            _titleBar.TitleText = "山妖卫士-安全防护中心";
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
            _activeVideoBtn.Enabled = false;

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

            //_pornWebsiteTabBtn = new ImageButton();
            //_pornWebsiteTabBtn.Size = Properties.Resources.btn_blwz_n.Size;
            //_pornWebsiteTabBtn.Image = _pornWebsiteTabBtn.NormalBack = Properties.Resources.btn_blwz_h;
            //_pornWebsiteTabBtn.HoverBack = Properties.Resources.btn_blwz_h;
            //_pornWebsiteTabBtn.PressBack = Properties.Resources.btn_blwz_p;
            //_pornWebsiteTabBtn.Location = new Point(32, 0);
            //_tabHeaderPanel.Controls.Add(_pornWebsiteTabBtn);
            //_activeTabBtn = _pornWebsiteTabBtn;

            //_pornFileTabBtn = new ImageButton();
            //_pornFileTabBtn.Size = Properties.Resources.btn_blwj_n.Size;
            //_pornFileTabBtn.Image = _pornFileTabBtn.NormalBack = Properties.Resources.btn_blwj_n;
            //_pornFileTabBtn.HoverBack = Properties.Resources.btn_blwj_h;
            //_pornFileTabBtn.PressBack = Properties.Resources.btn_blwj_p;
            //_pornFileTabBtn.Location = new Point(32+_pornWebsiteTabBtn.Width, 0);
            //_tabHeaderPanel.Controls.Add(_pornFileTabBtn);
            //_pornWebsiteTabBtn.Click += _pornWebsiteTabBtnOnClick;
            //_pornFileTabBtn.Click += _pornFileTabBtnOnClick;

            //_tabPornWebsiteContentPanel = new Panel();
            //_tabPornWebsiteContentPanel.Height = 264 + 30;
            //_tabPornWebsiteContentPanel.Width = MainForm.MainFormWidth;
            //_tabPornWebsiteContentPanel.Location = new Point(0, _tabHeaderPanel.Location.Y + _tabHeaderPanel.Height);
            //_tabPornWebsiteContentPanel.BackColor = Color.White;
            ////_tabPornWebsiteContentPanel.BackColor = Color.Red;
            //Controls.Add(_tabPornWebsiteContentPanel);
            //_tabPornWebsiteContentPanel.Visible = true;

            //_pornWebsiteTableView = new PornDataGridView();
            //_pornWebsiteTableView.Location = new Point(12, 0);
            //_pornWebsiteTableView.Width = _tabPornWebsiteContentPanel.Width-24;
            //_pornWebsiteTableView.Height = _tabPornWebsiteContentPanel.Height;
            //_pornWebsiteTableView.SelectionEnable = false;

            //_pornWebsiteTableView.Rows.Add(true, 1, "www.ifeng.com");
            //_pornWebsiteTableView.Rows.Add(true, "20150102 19", "www.ifeng.com");

            ////_pornWebsiteTableView.Rows.Add("20150102 19", "www.ifeng.com");
            ////_pornWebsiteTableView.Rows.Add("20150102 19", "www.ifeng.com");

            //_tabPornWebsiteContentPanel.Controls.Add(_pornWebsiteTableView);
    
            //_tabPornFileContentPanel = new Panel();
            //_tabPornFileContentPanel.Height = _tabPornWebsiteContentPanel.Height;
            //_tabPornFileContentPanel.Width = _tabPornWebsiteContentPanel.Width;
            //_tabPornFileContentPanel.Location = _tabPornWebsiteContentPanel.Location;
            ////_tabPornFileContentPanel.BackColor = Color.Brown;
            //Controls.Add(_tabPornFileContentPanel);
            //_tabPornFileContentPanel.Visible = false;

            //_networkPornPicView = new PornItemTableViewWithPreview();
            //_pornFileTableView.Width = ;
            //_pornFileTableView.Height = ;
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
        }

        static readonly string kAutoRunRegisstryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        static readonly string kAutoRunKey = "trollwiz";
        void TurnOnAutoStart()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            autorun_registry_key.SetValue(kAutoRunKey, Application.ExecutablePath + " -notvisible");
        }
        void TurnOffAutoStart()
        {
            RegistryKey autorun_registry_key = Registry.CurrentUser.OpenSubKey(kAutoRunRegisstryKey, true);
            autorun_registry_key.SetValue(kAutoRunKey, "");
        }
        void SetLogoImage()
        {
            if (Properties.Settings.Default.IsNetworkImageTurnOn ||
                Properties.Settings.Default.IsLocalActiveImageTurnOn ||
                Properties.Settings.Default.IsPornWebsiteProtectionTurnOn ||
                Properties.Settings.Default.IsLocalActiveVideoTurnOn)
            {
                _logoLabel.Image = Properties.Resources.fh_icon_animation_n;
                TurnOnAutoStart();
            }
            else
            {
                _logoLabel.Image = Properties.Resources.fh_icon_animation_d;
                TurnOffAutoStart();
            }
        }
        void _websiteBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsPornWebsiteProtectionTurnOn = e;
            Properties.Settings.Default.Save();
            SetLogoImage();
//#if !DEBUG
            //if (e)
            //{
            //    SystemProxyHelper.EnableProxyHTTP("127.0.0.1", 8090);
            //    FireFoxHelper.AddFirefox();
            //}
//#endif
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
        }
        void _activeVideoBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsLocalActiveVideoTurnOn = e;
            Properties.Settings.Default.Save();
            SetLogoImage();
        }
        void _networkImageBtnOnSwitchChanged(object sender, bool e)
        {
            Properties.Settings.Default.IsNetworkImageTurnOn = e;
            Properties.Settings.Default.Save();
            SetLogoImage();
        }

        void _returnBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Animate(this, MainForm.Effect.Slide, 200, 180);
            MainForm.Instance.mainPanelControl.Refresh();
        }

        void _clearAllBtnOnClick(object sender, EventArgs e)
        {
            _pornItemTableView.ClearAllPornItems();
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
