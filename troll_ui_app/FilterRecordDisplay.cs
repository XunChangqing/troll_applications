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

namespace troll_ui_app
{
    public partial class FilterRecordDisplay : Form
    {
        private BindingSource porn_pics_binding_source = new BindingSource();
        private SQLiteDataAdapter porn_pics_data_adapter = new SQLiteDataAdapter();
        private BindingSource blocked_pages_binding_source = new BindingSource();
        private SQLiteDataAdapter blocked_pages_data_adapter = new SQLiteDataAdapter();

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
                // Specify a connection string. Replace the given value with a  
                // valid connection string for a Northwind SQL Server sample 
                // database accessible to your system.
                String connectionString =
                    "Integrated Security=SSPI;Persist Security Info=False;" +
                    "Initial Catalog=Northwind;Data Source=porn.db";

                // Create a new data adapter based on the specified query.
                //porn_pics_data_adapter = new SQLiteDataAdapter("select id, url, type, datetime(created_at,'localtime') from porn_pics", connectionString);
                //blocked_pages_data_adapter = new SQLiteDataAdapter("select id, url, datetime(created_at, 'localtime') from blocked_pages", connectionString);
                porn_pics_data_adapter = new SQLiteDataAdapter("select id, url, type, created_at from porn_pics", connectionString);
                blocked_pages_data_adapter = new SQLiteDataAdapter("select id, url, created_at from blocked_pages", connectionString);

                // Create a command builder to generate SQL update, insert, and 
                // delete commands based on selectCommand. These are used to 
                // update the database.
                SQLiteCommandBuilder porn_pics_command_builder = new SQLiteCommandBuilder(porn_pics_data_adapter);
                SQLiteCommandBuilder blocked_pages_command_builder = new SQLiteCommandBuilder(blocked_pages_data_adapter);

                // Populate a new data table and bind it to the BindingSource.
                DataTable porn_pics_table = new DataTable();
                porn_pics_table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                porn_pics_data_adapter.Fill(porn_pics_table);
                porn_pics_binding_source.DataSource = porn_pics_table;

                // Resize the DataGridView columns to fit the newly loaded content.
                data_grid_view_porn_pics.Columns["id"].Visible = false;
                data_grid_view_porn_pics.Columns["created_at"].DefaultCellStyle.Format = "HH:mm:ss, MMMM dd, yyyy (dddd)";
                data_grid_view_porn_pics.Columns["url"].HeaderText = "地址";
                data_grid_view_porn_pics.Columns["type"].HeaderText = "类型";
                data_grid_view_porn_pics.Columns["created_at"].HeaderText = "时间";

                data_grid_view_porn_pics.Sort(data_grid_view_porn_pics.Columns["created_at"], ListSortDirection.Descending);

                DataTable blocked_pages_table = new DataTable();
                blocked_pages_table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                blocked_pages_data_adapter.Fill(blocked_pages_table);
                blocked_pages_binding_source.DataSource = blocked_pages_table;

                data_grid_view_pages.Columns["id"].Visible = false;
                data_grid_view_pages.Columns["created_at"].DefaultCellStyle.Format = "HH:mm:ss MMMM dd, yyyy (dddd)";
                data_grid_view_pages.Columns["url"].HeaderText = "地址";
                data_grid_view_pages.Columns["created_at"].HeaderText = "时间";

                data_grid_view_pages.Sort(data_grid_view_pages.Columns["created_at"], ListSortDirection.Descending);

                data_grid_view_porn_pics.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                data_grid_view_pages.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                //DataRow myrow = table.NewRow();
                //myrow["url"] = "abcdef";
                //table.Rows.Add(myrow);
                //table.Rows.Find();
                //dataAdapter.Update(table);
            }
            catch (SqlException)
            {
                MessageBox.Show("To run this example, replace the value of the " +
                    "connectionString variable with a connection string that is " +
                    "valid for your system.");
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
                //...
                string filename = "images/" + HttpUtility.UrlEncode(value2);
                picture_box.ImageLocation = filename;
                //picture_box.SizeMode = PictureBoxSizeMode.AutoSize;
                picture_box.SizeMode = PictureBoxSizeMode.Zoom;
                //picture_box.SizeMode = PictureBoxSizeMode.StretchImage;
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
                Int64 type = (Int64)e.Value;
                if (type == 3)
                    e.Value = "色情";
                else if (type == 2)
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
