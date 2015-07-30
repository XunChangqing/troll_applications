using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Web;
using System.IO;

namespace troll_ui_app
{
    class PornDatabase
    {
        //select * from porn_pics where date(created_at)<date('now', '-10 day');
        static private string kHistorySelect = "select * from {0} where date(created_at)<=date('now', '-{1} day')";
        static private string kHistoryDelete = "delete from {0} where date(created_at)<=date('now', '-{1} day')";
        static private int kHistoryDays = 30;
        PornDatabase()
        {
        }
        static public void DeleteHistoryPornPics()
        {
            String connectionString =
                "Integrated Security=SSPI;Persist Security Info=False;" +
                "Initial Catalog=Northwind;Data Source=porn.db";

            SQLiteDataAdapter porn_pics_data_adapter = new SQLiteDataAdapter(String.Format(kHistorySelect, "porn_pics", kHistoryDays), connectionString);
            SQLiteCommandBuilder cmd_builder = new SQLiteCommandBuilder(porn_pics_data_adapter);
            DataTable dt = new DataTable();
            porn_pics_data_adapter.Fill(dt);
            foreach(DataRow each_row in dt.Rows)
            {
                String url = (String)each_row["url"];
                String url_encoded = "images/"+HttpUtility.UrlEncode(url);
                try
                {
                    File.Delete(url_encoded);
                }
                catch
                {
                }
                each_row.Delete();
            }
            porn_pics_data_adapter.Update(dt);
        }

        static public void DeleteHistoryBlockedPages()
        {
            String connectionString =
                "Integrated Security=SSPI;Persist Security Info=False;" +
                "Initial Catalog=Northwind;Data Source=porn.db";
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kHistoryDelete, "blocked_pages", kHistoryDays), conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
