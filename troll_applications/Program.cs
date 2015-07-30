using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace troll_applications
{
    struct PornPic
    {
        public int id;
        public string url;
        public DateTime created_at;
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=porn.db"))
            {
                conn.Open();
                SQLiteCommand comm = conn.CreateCommand();
                comm.CommandText = "select * from porn_pics";
                //comm.CommandType = comm.CommandText;  

                //PornPic []pics;
                using (SQLiteDataReader reader = comm.ExecuteReader())
                {
                    using (DataTable dt = new DataTable())
                    {
                        dt.Load(reader);
                        //foreach (DataColumn col in dt)
                        //{

                        //}
                        foreach (DataRow row in dt.Rows) 
                        {
                            foreach (DataColumn column in dt.Columns)
                            {
                                Console.Write("\t{0}", row[column]);
                            }
                            Console.WriteLine("\t" + row.RowState);
                        }
                        Console.WriteLine(dt.Rows.Count);
                    }
                    //while (reader.Read())
                    //{
                    //    PornPic npic;
                    //    npic.id = reader.GetInt32(reader.GetOrdinal("id"));
                    //    npic.url = reader.GetString(reader.GetOrdinal("url"));
                    //    npic.created_at = reader.GetDateTime(reader.GetOrdinal("created_at"));
                    //    pics.p
                    //    //Console.WriteLine(reader[1].ToString());
                    //    Console.WriteLine("{0}{1}{2}", x, url, created_at);
                    //}
                }
            }
            //Proxies.SetProxy("http=127.0.0.1:9000");
            //Proxies.UnsetProxy();
        }
    }
}
