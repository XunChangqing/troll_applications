using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace troll_ui_app
{
    class AutoCloseMessageBox : Form
    {
        TitleBarControl _titleBar;
        int _seconds;
        string _message;
        Label _messageLabel;
        Button _yesButton;
        Timer _autoCloseTimer;
        public AutoCloseMessageBox(int seconds, string message)
        {
            Icon = Properties.Resources.icon_main_icon;
            Width = 400;
            Height = 120;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Azure;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            _titleBar = new TitleBarControl();
            _titleBar.WithLogo = false;
            Controls.Add(_titleBar);

            _seconds = seconds;
            _message = message;
            _messageLabel = new Label();
            _messageLabel.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Point);
            _messageLabel.Text = _message;
            _messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            _messageLabel.AutoSize = true;
            _messageLabel.Location = new System.Drawing.Point(Width/2-_messageLabel.Width/2, _titleBar.Height+10);
            Controls.Add(_messageLabel);
            _yesButton = new Button();
            _yesButton.AutoSize = true;
            _yesButton.Font = new System.Drawing.Font("微软雅黑", 14, GraphicsUnit.Point);
            //_yesButton.Text = string.Format("确认({0})", _seconds);
            _yesButton.Text = string.Format("确认");
            _yesButton.Location = new System.Drawing.Point(0, 100);
            Controls.Add(_yesButton);
            AcceptButton = _yesButton;
            Load += AutoCloseMessageBoxOnLoad;
            _yesButton.Click += _yesButtonOnClick;
        }
        void AutoCloseMessageBoxOnLoad(object sender, EventArgs e)
        {
            if (_messageLabel.Width > 400)
                Width = _messageLabel.Width;
            else
                Width = 400;
            _messageLabel.Location = new System.Drawing.Point(Width/2-_messageLabel.Width/2, _titleBar.Height+10);
            _yesButton.Location = new System.Drawing.Point(Width / 2 - _yesButton.Width / 2,
                _messageLabel.Location.Y + _messageLabel.Height + 5);
            Height = _yesButton.Location.Y + _yesButton.Height + 20;
            if (_seconds > 0)
            {
                _autoCloseTimer = new Timer();
                _autoCloseTimer.Interval = 1000;
                _autoCloseTimer.Tick += _autoCloseTimerOnTick;
                _autoCloseTimer.Enabled = true;
            }
        }

        void _autoCloseTimerOnTick(object sender, EventArgs e)
        {
            _yesButton.Text = string.Format("确认({0})", --_seconds);
            if (_seconds <= 0)
            {
                _autoCloseTimer.Stop();
                Close();
            }
        }
        void _yesButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }
        public static void ShowMessage(int seconds, string message)
        {
            AutoCloseMessageBox acmb = new AutoCloseMessageBox(seconds, message);
            acmb.ShowDialog();
        }
    }
}
