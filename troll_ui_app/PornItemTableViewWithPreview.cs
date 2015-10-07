using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Data.SQLite;
using log4net;
using System.IO;

namespace troll_ui_app
{
    public class PornItemTableViewWithPreview : UserControl
    {
        static readonly ILog log = Log.Get();
        BindingSource _pornItemBindingSource = new BindingSource();
        PornDatabase _pornDB;
        SQLiteDataAdapter _sqliteDataAdapter;
        PornDataGridView _dataGridView;
        PictureBox _previewPictureBox;
        DataTable _pornItemDataTable;
        bool _viewProtectionLogs;
        public PornItemTableViewWithPreview(bool viewProtectionLogs)
        {
            _viewProtectionLogs = viewProtectionLogs;
            //InitializeComponent();
            _pornDB = new PornDatabase();

            BackColor = Color.White;
            _dataGridView = new PornDataGridView(!viewProtectionLogs);
            _previewPictureBox = new PictureBox();
            _previewPictureBox.BackColor = Color.FromArgb(0xf3, 0xf3, 0xf3);
            _previewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            //Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            Controls.Add(_dataGridView);
            Controls.Add(_previewPictureBox);
            Load+=PornItemTabelViewWithPreviewOnLoad;
            _dataGridView.SelectionChanged += _dataGridViewOnSelectionChanged;
            SizeChanged += PornItemTableViewWithPreviewOnSizeChanged;

            DataTable dta = _pornDB.CreatePornItemsDataTable(out _sqliteDataAdapter);

            if (_viewProtectionLogs)
                _pornItemDataTable = dta;
            else
            {
                _pornItemDataTable = dta.Clone();
                DataColumn dc = new DataColumn("checked");
                dc.DataType = typeof(bool);
                dc.DefaultValue = true;
                _pornItemDataTable.Columns.Add(dc);
            }

            _dataGridView.DataSource = _pornItemBindingSource;
            _pornItemBindingSource.DataSource = _pornItemDataTable;
            //_pornItemBindingSource.DataSource = _pornItemDataTable;
            if(_viewProtectionLogs)
                PornDatabase.TableChangedProgress.ProgressChanged += TableChangedProgressOnProgressChanged;
        }

        public void Clear()
        {
            _pornItemDataTable.Clear();
            _previewPictureBox.ImageLocation = null;
        }
        void TableChangedProgressOnProgressChanged(object sender, string e)
        {
            //refresh data
            if (e == "porn_items")
            {
                _pornItemDataTable.Clear();
                _sqliteDataAdapter.Fill(_pornItemDataTable);
            }
        }

        public void AddRow(string info, PornDatabase.PornItemType itype, PornDatabase.PornItemStatus status)
        {
            DataRow dr = _pornItemDataTable.NewRow();
            if(!_viewProtectionLogs)
                dr.SetField<bool>("checked", true);
            dr.SetField<string>("info", info);
            //dr.SetField<string>("desc", "12");
            dr.SetField<PornDatabase.PornItemStatus>("status", status);
            dr.SetField<PornDatabase.PornItemType>("item_type", itype);
            dr.SetField<DateTime>("created_at", DateTime.Now);
            _pornItemDataTable.Rows.Add(dr);
        }

        public int ClearCheckedFiles()
        {
            int ret = 0;
            //foreach (DataRow x in _pornItemDataTable.Rows)
            foreach (DataRowView x in _pornItemBindingSource.List)
            {
                //if(x.Field<bool>("checked"))
                if (x.Row.Field<bool>("checked"))
                {
                    string fname = x.Row.Field<string>("info");
                    try { File.Delete(fname); ret++; }
                    catch (Exception e) { log.Error(e.ToString()); }
                    x.Delete();
                }
            }
            return ret;
        }

        public void ClearAllPornItems()
        {
            _pornDB.DeleteAllPornItmes();
            _pornItemDataTable.Clear();
        }

        void PornItemTableViewWithPreviewOnSizeChanged(object sender, EventArgs e)
        {
            _dataGridView.Height = Height;
        }

        void _dataGridViewOnSelectionChanged(object sender, EventArgs e)
        {
            DataRowView currentRow = (DataRowView)_pornItemBindingSource.Current;
            if(currentRow != null)
            {
                PornDatabase.PornItemType type = (PornDatabase.PornItemType)currentRow.Row.Field<Int64>("item_type");
                string info = currentRow.Row.Field<string>("info");
                if (type == PornDatabase.PornItemType.LocalImage ||
                    type == PornDatabase.PornItemType.LocalVideo)
                {
                    _previewPictureBox.ImageLocation = info;
                }
                else if (type == PornDatabase.PornItemType.NetworkImage)
                {
                    string filename = Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(info);
                    _previewPictureBox.ImageLocation = filename;
                }
                else
                    _previewPictureBox.ImageLocation = null;
            }
            //if (data_grid_view_porn_pics.SelectedRows.Count > 0)
            //{
            //    string value2 = data_grid_view_porn_pics.SelectedRows[0].Cells["url"].Value.ToString();
            //    string filename = Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(value2);
            //    picture_box.ImageLocation = filename;
            //    picture_box.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }

        void PornItemTabelViewWithPreviewOnLoad(object sender, EventArgs e)
        {

            _dataGridView.Width = MainForm.MainFormWidth - 260;
            _dataGridView.Height = Height;
            _dataGridView.Location = new Point(12, 0);
            _dataGridView.Init();

            _previewPictureBox.Width = 200;
            _previewPictureBox.Height = 200;
            _previewPictureBox.Location = new Point(_dataGridView.Width + 20, 52);
        }
    }
}
