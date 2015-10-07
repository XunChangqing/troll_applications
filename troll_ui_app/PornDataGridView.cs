using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using System.Data;

namespace troll_ui_app
{
    class PornDataGridView : DataGridView
    {
        static readonly ILog log = Log.Get();
        Color _selectionColor = Color.FromArgb(0xe7, 0xfc, 0xc0);
        //bool _selectionEnable = true;
        //public bool SelectionEnable
        //{
        //    get
        //    {
        //        return _selectionEnable;
        //    }
        //    set
        //    {
        //        _selectionEnable = value;
        //        if(_selectionEnable)
        //        {
        //            SelectionChanged -= PornDataGridViewOnSelectionChanged;
        //            CellMouseEnter -= PornDataGridViewOnCellMouseEnter;
        //            CellMouseLeave -= PornDataGridViewOnCellMouseLeave;
        //        }
        //        else
        //        {
        //            SelectionChanged += PornDataGridViewOnSelectionChanged;
        //            CellMouseEnter += PornDataGridViewOnCellMouseEnter;
        //            CellMouseLeave += PornDataGridViewOnCellMouseLeave;
        //            ClearSelection();
        //        }
        //    }
        //}

        //void PornDataGridViewOnCellMouseLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    if(e.RowIndex>=0)
        //        Rows[e.RowIndex].DefaultCellStyle.BackColor = BackgroundColor;
        //}

        //void PornDataGridViewOnCellMouseEnter(object sender, DataGridViewCellEventArgs e)
        //{
        //    if(e.RowIndex>=0)
        //        Rows[e.RowIndex].DefaultCellStyle.BackColor = _selectionColor;
        //}

        //void PornDataGridViewOnSelectionChanged(object sender, EventArgs e)
        //{
        //    ClearSelection();
        //}
        bool _withCheckCol = false;
        public PornDataGridView(bool withCheckCol)
        {
            _withCheckCol = withCheckCol;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            AllowUserToResizeColumns = false;
            AllowDrop = false;
            AllowUserToOrderColumns = false;
            //ReadOnly = true;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = 24;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            BackgroundColor = Color.White;
            RowHeadersVisible = false;
            GridColor = Color.FromArgb(0xc6, 0xc6, 0xc6);
            EnableHeadersVisualStyles = false;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            //ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 12, GraphicsUnit.Point);
            ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(0x26, 0x26, 0x26);
            //ColumnHeadersDefaultCellStyle.Padding;
            //RowsDefaultCellStyle.BackColor = ;
            RowsDefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 10, GraphicsUnit.Point);
            RowsDefaultCellStyle.ForeColor = Color.FromArgb(0x46, 0x46, 0x46);
            RowsDefaultCellStyle.SelectionBackColor = _selectionColor;
            RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(0x46, 0x46, 0x46);
            RowTemplate.Height = 30;
            //RowsDefaultCellStyle.Padding = new Padding(20,2,0,2);
            //RowsDefaultCellStyle.Padding = ;

            CellFormatting += PornDataGridViewOnCellFormatting;
            CellContentClick += PornDataGridViewOnCellContentClick;
        }

        void PornDataGridViewOnCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn
                   && e.RowIndex != -1)
            {
                CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        public void Init()
        {
            //_checkBoxCol = new DataGridViewCheckBoxColumn();
            //_checkBoxCol.Width = 20;
            //_checkBoxCol.DataPropertyName = "checked";
            //Columns.Add(_checkBoxCol);

            //AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            int didx = 1;
            if (_withCheckCol)
            {
                Columns["checked"].Width = 20;
                Columns["checked"].HeaderText = "";
                Columns["checked"].DisplayIndex = didx++;
            }
            Columns["info"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Columns["item_type"].Width = 80;
            Columns["status"].Width = 100;
            Columns["created_at"].Width = 150;

            Columns["info"].HeaderText = "项目";
            Columns["item_type"].HeaderText = "类型";
            Columns["status"].HeaderText = "状态";
            Columns["created_at"].HeaderText = "时间";

            Columns["id"].Visible = false;
            Columns["desc"].Visible = false;
            //Columns["status"].ReadOnly = true;
            Columns["status"].Visible = false;
            if (_withCheckCol)
                Columns["created_at"].Visible = false;
            else
                Columns["created_at"].DisplayIndex = didx++;
            Columns["created_at"].ReadOnly = true;
            Columns["info"].ReadOnly = true;
            Columns["item_type"].ReadOnly = true;

            Columns["info"].DisplayIndex = didx++;
            Columns["item_type"].DisplayIndex = didx++;
            Columns["status"].DisplayIndex = didx++;
        }

        void PornDataGridViewOnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            DataGridView dgView = (DataGridView)(sender);
            // no need to add TaskTime...
            if (e.ColumnIndex == dgView.Columns["created_at"].Index)
            {
                try
                {
                    DateTime dsutc = (DateTime)e.Value;
                    e.Value = TimeZoneInfo.ConvertTimeFromUtc(dsutc, TimeZoneInfo.Local);
                }
                catch (Exception ex)
                {
                    log.Equals(ex.ToString());
                }
            }
            else if (e.ColumnIndex == dgView.Columns["item_type"].Index)
            {
                PornDatabase.PornItemType type = (PornDatabase.PornItemType)(Int64)e.Value;
                if (type == PornDatabase.PornItemType.Undefined)
                    e.Value = "未知";
                else if (type == PornDatabase.PornItemType.PornDomain)
                    e.Value = "不良网站";
                else if (type == PornDatabase.PornItemType.NetworkImage)
                    e.Value = "网络图片";
                else if (type == PornDatabase.PornItemType.NetworkVideo)
                    e.Value = "网络视频";
                else if (type == PornDatabase.PornItemType.LocalImage)
                    e.Value = "本地图片";
                else if (type == PornDatabase.PornItemType.LocalVideo)
                    e.Value = "本地视频";
            }
            else if (e.ColumnIndex == dgView.Columns["status"].Index)
            {
                PornDatabase.PornItemStatus status = (PornDatabase.PornItemStatus)(Int64)e.Value;
                if (status == PornDatabase.PornItemStatus.Undefined)
                    e.Value = "未知";
                else if (status == PornDatabase.PornItemStatus.Normal)
                    e.Value = "已屏蔽";
                else if (status == PornDatabase.PornItemStatus.Trusted)
                    e.Value = "已信任";
            }
        }
    }
}
