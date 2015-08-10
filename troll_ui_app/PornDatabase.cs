using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Configuration;
using System.Windows.Forms;
//using System.Json;

namespace troll_ui_app
{
    public enum Operations { AddWhite, DeleteWhite, AddBlack, DeleteBlack };
    [DataContract]
    public class DomainUpdateLogItem
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "operation")]
        public Operations Operation { get; set; }
        [DataMember(Name = "domain_name")]
        public String DomainName { get; set; }
        [DataMember(Name = "url")]
        public String Url { get; set; }
    }
    [DataContract]
    public class DomainSubmitItem
    {
        [DataMember(Name = "tmp_domain_name")]
        public DomainNameItem dni {set; get;}
    }
    [DataContract]
    public class DomainNameItem
    {
        [DataMember(Name = "domain_name")]
        public String domain_name { set; get; }
    }
    //public class DomainUpdateLogResponse
    //{
    //    public DomainUpdateLogItem[] Logs { get; set; }
    //}
    class PornDatabase
    {
        //select * from porn_pics where date(created_at)<date('now', '-10 day');
        const string kHistorySelect = "select * from {0} where date(created_at)<=date('now', '-{1} day')";
        const string kHistoryDelete = "delete from {0} where date(created_at)<=date('now', '-{1} day')";
        const String kDomainListSelect = "select * from {0}";
        const String kDomainListInsert = "insert or ignore into {0} (domain_name) values (@domain_name)";
        const int kHistoryDays = 30;
        const String connectionString =
            "Integrated Security=SSPI;Persist Security Info=False;" +
            "Initial Catalog=Northwind;Data Source=porn.db";
        PornDatabase()
        {
        }
        static public void DeleteHistoryPornPics()
        {
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
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kHistoryDelete, "blocked_pages", kHistoryDays), conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        static public void UpdateWhiteList()
        { 
        
        }

        static public void UpdateBlackList()
        { 
        }

        static public void DeleteSpecificRow(DataTable dt, String domain_name)
        {
            DataRow[] result_rows;
            result_rows = dt.Select("domain_name='" + domain_name + "'");
            if (result_rows.Length > 0)
                result_rows[0].Delete();
        }
        static public void DeleteHistroy(Object state)
        {
            //delete too old histories
            DeleteHistoryPornPics();
            DeleteHistoryBlockedPages();
        }
        static public void MaintainDatabase(Object state) 
        {
            int last_id = int.Parse(ConfigurationManager.AppSettings["last_id"]);
            //update black, white and tmp lists
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseProxy = false
            };
            HttpClient client = new HttpClient(handler); 
            try
            {
                //var builder = new UriBuilder();
                //builder.Host = "192.168.0.105";
                //builder.Port = 3000;
                //builder.Path = "domain_update_logs.json";
                //var query = HttpUtility.ParseQueryString(builder.Query);
                //query["start_id"] = last_id.ToString();
                //builder.Query = query.ToString();
                //string url = builder.ToString();

                //HttpResponseMessage response = await client.GetAsync(builder.Uri);
                //HttpResponseMessage response = client.GetAsync(builder.Uri).Result;
                //response.EnsureSuccessStatusCode();
                //String response_body = response.Content.ReadAsStringAsync().Result;
                List<DomainUpdateLogItem> log_items = new List<DomainUpdateLogItem>();

                //DataContractJsonSerializer json_serializer = new DataContractJsonSerializer(log_items.GetType());
                //using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(response_body)))
                //{
                //    log_items = json_serializer.ReadObject(stream) as List<DomainUpdateLogItem>;
                //}

                DomainUpdateLogItem new_item = new DomainUpdateLogItem();
                new_item.Id = 1;
                new_item.DomainName = "add_white.com";
                new_item.Operation = Operations.AddWhite;
                log_items.Add(new_item);
                new_item = new DomainUpdateLogItem();
                new_item.Id = 2;
                new_item.DomainName = "add_white2.com";
                new_item.Operation = Operations.AddWhite;
                log_items.Add(new_item);
                new_item = new DomainUpdateLogItem();
                new_item.Id = 3;
                new_item.DomainName = "add_white.com";
                new_item.Operation = Operations.DeleteWhite;
                log_items.Add(new_item);

                new_item = new DomainUpdateLogItem();
                new_item.Id = 4;
                new_item.DomainName = "add_black.com";
                new_item.Operation = Operations.AddBlack;
                log_items.Add(new_item);
                new_item = new DomainUpdateLogItem();
                new_item.Id = 5;
                new_item.DomainName = "add_black2.com";
                new_item.Operation = Operations.AddBlack;
                log_items.Add(new_item);
                new_item = new DomainUpdateLogItem();
                new_item.Id = 6;
                new_item.DomainName = "add_black.com";
                new_item.Operation = Operations.DeleteBlack;
                log_items.Add(new_item);

                SQLiteDataAdapter white_data_adapter = new SQLiteDataAdapter(String.Format(kDomainListSelect, "white_list"), connectionString);
                SQLiteCommand white_insert_cmd = new SQLiteCommand(String.Format(kDomainListInsert, "white_list"));
                white_insert_cmd.Parameters.Add("@domain_name", DbType.String, 40, "domain_name");
                white_data_adapter.InsertCommand = white_insert_cmd;
                SQLiteCommandBuilder white_cmd_builder = new SQLiteCommandBuilder(white_data_adapter);
                DataTable white_table = new DataTable();
                white_data_adapter.Fill(white_table);

                SQLiteDataAdapter black_data_adapter = new SQLiteDataAdapter(String.Format(kDomainListSelect, "black_list"), connectionString);
                SQLiteCommand black_insert_cmd = new SQLiteCommand(String.Format(kDomainListInsert, "black_list"));
                black_insert_cmd.Parameters.Add("@domain_name", DbType.String, 40, "domain_name");
                black_data_adapter.InsertCommand = black_insert_cmd;
                SQLiteCommandBuilder black_cmd_builder = new SQLiteCommandBuilder(black_data_adapter);
                DataTable black_table = new DataTable();
                black_data_adapter.Fill(black_table);

                SQLiteDataAdapter tmp_black_data_adapter = new SQLiteDataAdapter(String.Format(kDomainListSelect, "tmp_black_list"), connectionString);
                SQLiteCommand tmp_black_insert_cmd = new SQLiteCommand(String.Format(kDomainListInsert, "tmp_black_list"));
                tmp_black_insert_cmd.Parameters.Add("@domain_name", DbType.String, 40, "domain_name");
                tmp_black_data_adapter.InsertCommand = tmp_black_insert_cmd;
                SQLiteCommandBuilder tmp_black_cmd_builder = new SQLiteCommandBuilder(tmp_black_data_adapter);
                DataTable tmp_black_table = new DataTable();
                tmp_black_data_adapter.Fill(tmp_black_table);

                foreach (DomainUpdateLogItem item in log_items)
                {
                    if (item.Id > last_id)
                        last_id = item.Id;
                    switch (item.Operation)
                    {
                        case Operations.AddWhite:
                            white_table.Rows.Add(null, item.DomainName);
                            DeleteSpecificRow(tmp_black_table, item.DomainName);
                            break;
                        case Operations.DeleteWhite:
                            DeleteSpecificRow(white_table, item.DomainName);
                            break;
                        case Operations.AddBlack:
                            black_table.Rows.Add(null, item.DomainName);
                            DeleteSpecificRow(tmp_black_table, item.DomainName);
                            break;
                        case Operations.DeleteBlack:
                            DeleteSpecificRow(black_table, item.DomainName);
                            break;
                        default:
                            break;
                    }

                }
                white_data_adapter.Update(white_table);
                black_data_adapter.Update(black_table);
                tmp_black_data_adapter.Update(tmp_black_table);

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings["last_id"].Value = last_id.ToString();
                config.Save();

                //String data = "{\"domain_update_log\":{\"operation\":\"10\",\"domain_name\":\"sjk.com\"}}";
                //DomainSubmitItem submit_item = new DomainSubmitItem();
                //submit_item.dni = new DomainNameItem();
                //submit_item.dni.domain_name = "iii.com";
                //DataContractJsonSerializer submit_serializer = new DataContractJsonSerializer(submit_item.GetType());
                //MemoryStream ms = new MemoryStream();
                //submit_serializer.WriteObject(ms, submit_item);
                //String request_str = Encoding.Default.GetString(ms.ToArray());
                //////client.PostAsync("domain_update_logs.json", new StringContent();myObject.ToString()
                //HttpResponseMessage post_response = await client.PostAsync("tmp_domain_names.json", new StringContent(request_str, Encoding.UTF8, "application/json"));
                //post_response.EnsureSuccessStatusCode();
                //String post_response_body = await post_response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                //String x = e.ToString();
                Console.WriteLine(e.ToString());
            }
        }

        //static public void SubmitPornHost(String hostname)
        //{

        //}
    }
}
