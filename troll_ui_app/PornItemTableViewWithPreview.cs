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
        //是否总是显示最新的一个目标
        bool _showLastestItem = false;
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
            //_previewPictureBox.ImageLocation = null;
            DisposeCurrentImage();
            _previewPictureBox.Image = null;
            _showLastestItem = false;
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
            if (_showLastestItem)
            {
                try { _dataGridView.CurrentCell = _dataGridView.Rows[0].Cells["info"]; }
                catch (Exception exp) { log.Error(exp.ToString()); }
            }
        }

        public int ClearCheckedFiles()
        {
            //释放当前显示的图像，以防无法删除
            DisposeCurrentImage();
            int ret = 0;
            //foreach (DataRow x in _pornItemDataTable.Rows)
            foreach (DataRowView x in _pornItemBindingSource.List)
            {
                //if(x.Field<bool>("checked"))
                if (x.Row.Field<bool>("checked"))
                {
                    string fname = x.Row.Field<string>("info");
                    try { 
                        File.Delete(fname);
                        log.Info("Delete local porn file: " + fname);
                        ret++; 
                    }
                    catch (Exception e) { log.Error(e.ToString()); }
                    //如果在这里删除该行，则会触发selectionChange时间，导致再次打开图片文件，是的图片无法被删除
                    //x.Delete();
                }
            }
            //将所有文件删除以后，统一清空
            Clear();
            return ret;
        }

        public void ClearAllPornItems()
        {
            _pornDB.DeleteAllPornItmes();
            _pornItemDataTable.Clear();
            _previewPictureBox.ImageLocation = null;
            _previewPictureBox.Image = null;
        }

        void PornItemTableViewWithPreviewOnSizeChanged(object sender, EventArgs e)
        {
            _dataGridView.Height = Height;
        }

        void _dataGridViewOnSelectionChanged(object sender, EventArgs e)
        {
            //log.Info(sender.ToString());
            DataRowView currentRow = (DataRowView)_pornItemBindingSource.Current;
            if(currentRow != null)
            {
                PornDatabase.PornItemType type = (PornDatabase.PornItemType)currentRow.Row.Field<Int64>("item_type");
                string info = currentRow.Row.Field<string>("info");
                string fname;
                if (type == PornDatabase.PornItemType.LocalImage)
                    //|| type == PornDatabase.PornItemType.LocalVideo)
                {
                    //_previewPictureBox.ImageLocation = info;
                    fname = info;
                }
                else if (type == PornDatabase.PornItemType.NetworkImage)
                {
                    string filename = Program.AppLocalDir + Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(info);
                    //_previewPictureBox.ImageLocation = filename;
                    fname = filename;
                }
                else
                {
                    //_previewPictureBox.ImageLocation = null;
                    fname = null;
                }
                log.Info("PornPreview image location: " + _previewPictureBox.ImageLocation);
                //_previewPictureBox.ImageLocation = fname;
                //这里没有直接设置ImageLocation，因为发现如果文件名为httpurl编码形式，在文件拷贝以后无法显示
                try
                {
                    DisposeCurrentImage();
                    if (fname != null)
                    {
                        using (Image x = Image.FromFile(fname))
                        {
                            //通过拷贝构造函数生成新的bitmap，可以解除对文件的锁定
                            //这样正在打开的文件也可以被删除了
                            _previewPictureBox.Image = new Bitmap(x);
                            x.Dispose();
                        }
                    }
                    else
                        _previewPictureBox.Image = null;
                }
                catch (Exception ex)
                {
                    log.Info(ex.ToString());
                    _previewPictureBox.Image = null;
                }
            }
            //if (data_grid_view_porn_pics.SelectedRows.Count > 0)
            //{
            //    string value2 = data_grid_view_porn_pics.SelectedRows[0].Cells["url"].Value.ToString();
            //    string filename = Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(value2);
            //    picture_box.ImageLocation = filename;
            //    picture_box.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }
        void DisposeImage(Image img)
        {
            try
            {
                if (img != null)
                    img.Dispose();
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
        }
        void DisposeCurrentImage()
        {
            //必须释放旧图像，要先将picturebox的image设为null，否则会导致picturebox出现异常
            //picturebox访问已经被dispose的image出现异常
            _previewPictureBox.Image = null;
            DisposeImage(_previewPictureBox.Image);
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
