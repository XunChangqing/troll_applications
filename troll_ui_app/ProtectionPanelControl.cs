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
    public class ProtectionPanelControl : UserControl
    {
        TitleBarControl _titleBar;

        Panel _settingPanel;
        Label _logoLabel;
        ImageButton _activeImageBtn;
        ImageButton _networkImageBtn;
        ImageButton _websiteBtn;
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
            //_settingPanel.BackColor = Color.Red;
            _settingPanel.BackgroundImage = Properties.Resources.top_green_bg;
            _settingPanel.BackgroundImageLayout = ImageLayout.Tile;
            _settingPanel.Height = 204;
            _settingPanel.Width = MainForm.MainFormWidth;
            _settingPanel.Location = new Point(0, _titleBar.Height);

            _logoLabel = new Label();
            _logoLabel.Image = Properties.Resources.fh_icon_animation_;
            _logoLabel.BackColor = Color.Transparent;
            _logoLabel.Height = 150;
            _logoLabel.Width = 174;
            _logoLabel.Location = new Point(54, 30);
            _settingPanel.Controls.Add(_logoLabel);

            Controls.Add(_settingPanel);

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

            //_activeImageBtn = new ImageButton();
            //_activeImageBtn.Height = ;
            //_activeImageBtn.Width = ;
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

        void _returnBtnOnClick(object sender, EventArgs e)
        {
            MainForm.Animate(this, MainForm.Effect.Slide, 200, 180);
            MainForm.Instance.mainPanelControl.Refresh();
        }

        void _clearAllBtnOnClick(object sender, EventArgs e)
        {
            _pornItemTableView.ClearChecked();
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
