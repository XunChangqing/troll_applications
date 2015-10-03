using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Web;
using System.Diagnostics;
using System.IO;
using log4net;

namespace troll_ui_app
{
    public partial class FilterRecordDisplay : Form
    {
        static readonly ILog log = Log.Get();
        private BindingSource porn_pics_binding_source = new BindingSource();
        private BindingSource blocked_pages_binding_source = new BindingSource();
        private PornDatabase PornDB;

        public FilterRecordDisplay()
        {
            InitializeComponent();
        }

        //public ~FilterRecordDisplay()
        //{
        //    Console.WriteLine("xx");
        //}

        private void FilterRecordDisplay_Load(object sender, EventArgs e)
        {
            data_grid_view_porn_pics.DataSource = porn_pics_binding_source;
            data_grid_view_pages.DataSource = blocked_pages_binding_source;
            GetData();
        }

        private void GetData()
        {
            try
            {
                //PornDB = new PornDatabase();
                PornDB = PornDatabase.Instance;
                porn_pics_binding_source.DataSource = PornDB.CreatePornPicsDataTable();

                // Resize the DataGridView columns to fit the newly loaded content.
                data_grid_view_porn_pics.Columns["id"].Visible = false;
                data_grid_view_porn_pics.Columns["created_at"].DefaultCellStyle.Format = "HH:mm:ss, MMMM dd, yyyy (dddd)";
                data_grid_view_porn_pics.Columns["url"].HeaderText = "地址";
                data_grid_view_porn_pics.Columns["type"].HeaderText = "类型";
                data_grid_view_porn_pics.Columns["created_at"].HeaderText = "时间";

                data_grid_view_porn_pics.Sort(data_grid_view_porn_pics.Columns["created_at"], ListSortDirection.Descending);

                blocked_pages_binding_source.DataSource = PornDB.CreateBlockedPagesDataTable();

                data_grid_view_pages.Columns["id"].Visible = false;
                data_grid_view_pages.Columns["created_at"].DefaultCellStyle.Format = "HH:mm:ss MMMM dd, yyyy (dddd)";
                data_grid_view_pages.Columns["url"].HeaderText = "地址";
                data_grid_view_pages.Columns["created_at"].HeaderText = "时间";

                data_grid_view_pages.Sort(data_grid_view_pages.Columns["created_at"], ListSortDirection.Descending);

                data_grid_view_porn_pics.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                data_grid_view_pages.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (SqlException e)
            {
                log.Error(e.ToString());
            }
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void data_grid_view_porn_pics_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //foreach (DataGridViewRow row in data_grid_view_porn_pics.SelectedRows)
            //{
            //    string value2 = row.Cells["url"].Value.ToString();
            //    //...
            //    string filename = "images/"+HttpUtility.UrlEncode(value2);
            //    PictureBox pb = new PictureBox();
            //    pb.ImageLocation = filename;
            //    pb.SizeMode = PictureBoxSizeMode.AutoSize;
            //    pb.Show();
            //}
        }

        private void data_grid_view_porn_pics_SelectionChanged(object sender, EventArgs e)
        {
            if (data_grid_view_porn_pics.SelectedRows.Count > 0)
            {
                string value2 = data_grid_view_porn_pics.SelectedRows[0].Cells["url"].Value.ToString();
                string filename = Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(value2);
                picture_box.ImageLocation = filename;
                picture_box.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void data_grid_view_porn_pics_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            DataGridView dgView = (DataGridView)(sender);
            // no need to add TaskTime...
            if (e.ColumnIndex == dgView.Columns["created_at"].Index)
            {
                DateTime dsutc = (DateTime)e.Value;
                e.Value = TimeZoneInfo.ConvertTimeFromUtc(dsutc, TimeZoneInfo.Local);
            }
            else if (e.ColumnIndex == dgView.Columns["type"].Index)
            {
                PornClassifier.ImageType type = (PornClassifier.ImageType)(Int64)e.Value;
                if (type == PornClassifier.ImageType.Porn)
                    e.Value = "色情";
                else if (type == PornClassifier.ImageType.Disguise)
                    e.Value = "暴露";
            }
        }

        private void data_grid_view_pages_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            DataGridView dgView = (DataGridView)(sender);
            // no need to add TaskTime...
            if (e.ColumnIndex == dgView.Columns["created_at"].Index)
            {
                DateTime dsutc = (DateTime)e.Value;
                e.Value = TimeZoneInfo.ConvertTimeFromUtc(dsutc, TimeZoneInfo.Local);
            }
        }

        //private void reloadButton_Click(object sender, System.EventArgs e)
        //{
        //    // Reload the data from the database.
        //    GetData(dataAdapter.SelectCommand.CommandText);
        //}

        //private void submitButton_Click(object sender, System.EventArgs e)
        //{
        //    // Update the database with the user's changes.
        //    dataAdapter.Update((DataTable)bindingSource1.DataSource);
        //}
    }
}
