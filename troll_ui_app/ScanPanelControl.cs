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
    public class ScanPanelControl : UserControl
    {
        public class ReturnEventArgs
        {
            public ReturnEventArgs() { }
        }
        public delegate void ReturnEventHandler(object sender, ReturnEventArgs e);
        public event ReturnEventHandler ReturnEvent;
        TitleBarControl titleBar;
        ImageButton returnBtn;
        Panel listPanel;
        Panel progressPanel;

        Label progressLabel;
        Label totalScanedDesc;
        Label totalScaned;
        ProgressBar progressBar;
        Label targetScanedDesc;
        Label targetScaned;
        Label timeElapsed;
        Label timeElapsedDesc;
        Button pauseBtn;
        Button stopBtn;

        Panel processPanel;

        Panel summaryPanel;

        public ScanPanelControl()
        {
            //InitializeComponent();
            Dock = DockStyle.Fill;
            titleBar = new TitleBarControl();
            Controls.Add(titleBar);

            listPanel = new Panel();
            Controls.Add(listPanel);

            progressPanel = new Panel();
            progressPanel.BackColor = Color.LightGray;
            listPanel.Controls.Add(progressPanel);

            progressLabel = new Label();
            progressLabel.Text = "正在准备";
            progressPanel.Controls.Add(progressLabel);
            progressBar = new ProgressBar();
            progressBar.Value = 90;
            progressPanel.Controls.Add(progressBar);

            totalScanedDesc = new Label();
            totalScanedDesc .AutoSize = true;
            totalScanedDesc .Text = "共扫描对象：";
            progressPanel.Controls.Add(totalScanedDesc);

            totalScaned = new Label();
            totalScaned .AutoSize = true;
            totalScaned.Text = "1";
            progressPanel.Controls.Add(totalScaned);

            targetScanedDesc = new Label();
            targetScanedDesc.AutoSize = true;
            targetScanedDesc.Text = "共检出项：";
            progressPanel.Controls.Add(targetScanedDesc);

            targetScaned = new Label();
            targetScaned.ForeColor = Color.Red;
            targetScaned .AutoSize = true;
            targetScaned.Text = "1";
            progressPanel.Controls.Add(targetScaned);

            timeElapsedDesc = new Label();
            timeElapsedDesc.AutoSize = true;
            timeElapsedDesc.Text = "已用时间：";
            progressPanel.Controls.Add(timeElapsedDesc);

            timeElapsed = new Label();
            timeElapsed.AutoSize = true;
            timeElapsed.Text = "0:20";
            progressPanel.Controls.Add(timeElapsed);

            pauseBtn = new Button();
            pauseBtn.Text = "暂停";

            stopBtn = new Button();
            stopBtn.Text = "停止";

            Load += ScanPanelControlOnLoad;
        }

        void ScanPanelControlOnLoad(object sender, EventArgs e)
        {
            listPanel.Location = new Point(0, titleBar.Height);
            listPanel.Size = new System.Drawing.Size(Width, Height - titleBar.Height);
            progressPanel.Location = new Point(0, 0);
            progressPanel.Size = new Size(listPanel.Width, 100);
            //summaryPanel.Height = Height - titleBar.Height;
            progressLabel.Location = new Point(10, 10);
            progressLabel.AutoSize = true;
            progressBar.Location = new Point(progressLabel.Location.X, progressLabel.Location.Y+progressLabel.Height+5);
            progressBar.Size = new Size(progressPanel.Width - 10 - 100, 20);

            totalScanedDesc.Location = new Point(progressBar.Location.X, progressBar.Location.Y + progressBar.Height + 5);
            totalScaned.Location = new Point(totalScanedDesc.Location.X + totalScanedDesc.Width + 5, totalScanedDesc.Location.Y);

            targetScanedDesc.Location = new Point(totalScaned.Location.X + totalScaned.Width + 5, totalScaned.Location.Y);
            targetScaned.Location = new Point(targetScanedDesc.Location.X + targetScanedDesc.Width + 5, targetScanedDesc.Location.Y);

            timeElapsedDesc.Location = new Point(targetScaned.Location.X + targetScaned.Width + 5, targetScaned.Location.Y);
            timeElapsed.Location = new Point(timeElapsedDesc.Location.X + timeElapsedDesc.Width + 5, timeElapsedDesc.Location.Y);

            pauseBtn.Location = new Point(progressBar.Location.X + progressBar.Width + 5,
                progressBar.Location.Y + (progressBar.Height - pauseBtn.Height) / 2);
        }
    }
}
