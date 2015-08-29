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
using System.Diagnostics;
using log4net;
using Newtonsoft.Json.Linq;
//using System.Json;

namespace troll_ui_app
{
    public enum Operations { AddWhite=1, DeleteWhite, AddBlack, DeleteBlack };
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
        static readonly string webToken = "masa417";
        static readonly ILog log = Log.Get();
        //select * from porn_pics where date(created_at)<date('now', '-10 day');
        static readonly String kConnectionString = "Data Source={0}porn.db";
        static readonly String kGeneralSelect = "select * from {0}";
        static readonly String kDomainListInsert = "insert or ignore into domain_list (domain_name, type) values ('{0}', {1})";
        static readonly String kDomainListGetType = "select type from domain_list where domain_name='{0}'";
        static readonly String kDomainListDelete = "delete from domain_list where domain_name='{0}'";
        static readonly String kDomainListInsertOrReplace = "insert or replace into domain_list (domain_name, type) values ('{0}', {1})";
        static readonly String kDomainListInsertOrIgnore = "insert or ignore into domain_list (domain_name, type) values ('{0}', {1})";
        static readonly String kPornPicsInsert = "insert or ignore into porn_pics (url, type) values ('{0}', {1})";
        static readonly String kPornPagesInsert = "insert or  ignore into porn_pages (domain_name, page_url, porn_pic_url) values ('{0}', '{1}', '{2}')";
        static readonly String kPornPagesCountSelect = "select count(*) from porn_pages where domain_name='{0}'";
        static readonly String kPornPagesDelete = "delete from porn_pages where domain_name='{0}'";
        static readonly String kBlockedPagesInsert = "insert or ignore into blocked_pages (url) values ('{0}')";

        static readonly String kHistoryPornPicSelect = "select url from porn_pics where date(created_at)<=date('now', '-{0} day')";
        static readonly String kHistoryDelete = "delete from {0} where date(created_at)<=date('now', '-{1} day')";

        static readonly String kGetLastIDSelect = "select value from configs where name='last_id'";
        static readonly String kUpdateLastIDInsert = "insert or replace into configs (name, value) values ('last_id', '{0}')";

        //static readonly String kBlackListTableName = "black_list";
        public enum DomainType {Undefined, White, Black, TmpBlack };

        //static private PornDatabase instance;
        //public static PornDatabase Instance
        //{
        //    get { return instance; }
        //}
        //private DataSet PornDataset = new DataSet();
        private SQLiteConnection PornDBConnection;

        //private DataTable WhiteDomainTable;
        //private DataTable BlackDomainTable;
        //private DataTable TmpDomainTable;
        //private DataTable DomainTable;
        //private DataTable PornPagesTable;

        //private DataTable PornPicsTable;
        //private DataTable BlockedPagesTable;

        //private SQLiteDataAdapter DomainDataAdapter;
        //private SQLiteDataAdapter PornPagesDataAdatper;
        //private SQLiteDataAdapter WhiteDataAdapter;
        //private SQLiteDataAdapter BlackDataAdapter;
        //private SQLiteDataAdapter TmpBlackDataAdapter;
        //private SQLiteDataAdapter PornPicsDataAdapter;
        //private SQLiteDataAdapter BlockedPagesDataAdapter;
        public PornDatabase()
        {
            PornDBConnection = new SQLiteConnection(string.Format(kConnectionString, Program.AppLocalDir));
            PornDBConnection.Open();

            //{
            //    DomainTable = new DataTable();
            //    DomainDataAdapter = new SQLiteDataAdapter(String.Format(kGeneralSelect, "domain_list"), PornDBConnection);
            //    SQLiteCommand cmd = new SQLiteCommand(String.Format(kDomainListInsert, "domain_list"));
            //    cmd.Parameters.Add("@domain_name", DbType.String, 1024, "domain_name");
            //    cmd.Parameters.Add("@type", DbType.Int64, 8, "type");
            //    DomainDataAdapter.InsertCommand = cmd;
            //    SQLiteCommandBuilder cmdBuilder = new SQLiteCommandBuilder(DomainDataAdapter);
            //    DomainDataAdapter.Fill(DomainTable);
            //    //UniqueConstraint custUnique = new UniqueConstraint(TmpDomainTable.Columns["domain_name"]);
            //    //TmpDomainTable.Constraints.Add(custUnique);
            //    DomainTable.PrimaryKey = new DataColumn[] { DomainTable.Columns["domain_name"] };
            //    Int64 x = 1;
            //    DomainTable.Columns["type"].DataType = x.GetType();
            //}

            //{
            //    PornPagesTable = new DataTable();
            //    PornPagesDataAdatper = new SQLiteDataAdapter(String.Format(kGeneralSelect, "porn_pages"), PornDBConnection);
            //    SQLiteCommand porn_pages_cmd = new SQLiteCommand(kPornPagesInsert);
            //    porn_pages_cmd.Parameters.Add("@domain_name", DbType.String, 1024, "domain_name");
            //    porn_pages_cmd.Parameters.Add("@page_url", DbType.String, 1024, "page_url");
            //    porn_pages_cmd.Parameters.Add("@porn_pic_url", DbType.String, 1024, "porn_pic_url");
            //    PornPagesDataAdatper.InsertCommand = porn_pages_cmd;
            //    SQLiteCommandBuilder porn_pages_cmd_builder = new SQLiteCommandBuilder(PornPagesDataAdatper);
            //    PornPagesDataAdatper.Fill(PornPagesTable);
            //    UniqueConstraint custUnique = new UniqueConstraint(
            //        new DataColumn[]{PornPagesTable.Columns["page_url"], PornPagesTable.Columns["porn_pic_url"]});
            //    PornPagesTable.Constraints.Add(custUnique);
            //}

            //{
            //    PornPicsTable = new DataTable();
            //    PornPicsDataAdapter = new SQLiteDataAdapter(String.Format(kGeneralSelect, "porn_pics"), PornDBConnection);
            //    SQLiteCommand porn_pics_cmd = new SQLiteCommand(kPornPicsInsert);
            //    porn_pics_cmd.Parameters.Add("@url", DbType.String, 1024, "url");
            //    porn_pics_cmd.Parameters.Add("@type", DbType.Int64, 8, "type");
            //    PornPicsDataAdapter.InsertCommand = porn_pics_cmd;
            //    SQLiteCommandBuilder porn_pics_cmd_builder = new SQLiteCommandBuilder(PornPicsDataAdapter);
            //    PornPicsDataAdapter.Fill(PornPicsTable);
            //}

            //{
            //    BlockedPagesTable = new DataTable();
            //    BlockedPagesDataAdapter = new SQLiteDataAdapter(String.Format(kGeneralSelect, "blocked_pages"), PornDBConnection);
            //    SQLiteCommand blocked_pages_cmd = new SQLiteCommand(kBlockedPagesInsert);
            //    blocked_pages_cmd.Parameters.Add("@url", DbType.String, 1024, "url");
            //    BlockedPagesDataAdapter.InsertCommand = blocked_pages_cmd;
            //    SQLiteCommandBuilder cmdBuilder = new SQLiteCommandBuilder(BlockedPagesDataAdapter);
            //    BlockedPagesDataAdapter.Fill(BlockedPagesTable);
            //}
        }
        public DataTable CreatePornPicsDataTable()
        {
            DataTable pornPicsTable = new DataTable();
            SQLiteDataAdapter pornPicsDataAdapter = new SQLiteDataAdapter(String.Format(kGeneralSelect, "porn_pics"), PornDBConnection);
            //SQLiteCommand porn_pics_cmd = new SQLiteCommand(kPornPicsInsert);
            //porn_pics_cmd.Parameters.Add("@url", DbType.String, 1024, "url");
            //porn_pics_cmd.Parameters.Add("@type", DbType.Int64, 8, "type");
            //pornPicsDataAdapter.InsertCommand = porn_pics_cmd;
            //SQLiteCommandBuilder porn_pics_cmd_builder = new SQLiteCommandBuilder(PornPicsDataAdapter);
            pornPicsDataAdapter.Fill(pornPicsTable);
            return pornPicsTable;
        }

        public DataTable CreateBlockedPagesDataTable()
        {
            DataTable blockedPagesTable = new DataTable();
            SQLiteDataAdapter blockedPagesDataAdapter = new SQLiteDataAdapter(String.Format(kGeneralSelect, "blocked_pages"), PornDBConnection);
            //SQLiteCommand blocked_pages_cmd = new SQLiteCommand(kBlockedPagesInsert);
            //blocked_pages_cmd.Parameters.Add("@url", DbType.String, 1024, "url");
            //BlockedPagesDataAdapter.InsertCommand = blocked_pages_cmd;
            //SQLiteCommandBuilder cmdBuilder = new SQLiteCommandBuilder(BlockedPagesDataAdapter);
            blockedPagesDataAdapter.Fill(blockedPagesTable);
            return blockedPagesTable;
        }

        public void InsertBlockedPage(String url)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kBlockedPagesInsert, url), PornDBConnection);
                cmd.ExecuteNonQuery();
                //lock (BlockedPagesTable)
                //{
                //    BlockedPagesTable.Rows.Add(null, url);
                //    BlockedPagesDataAdapter.Update(BlockedPagesTable);
                //}
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
        }
        public void InsertPornPic(String url, PornClassifier.ImageType prop)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kPornPicsInsert, url, (Int64)prop), PornDBConnection);
                cmd.ExecuteNonQuery();
                //lock (PornPicsTable)
                //{
                //    PornPicsTable.Rows.Add(null, url, (int)prop);
                //    PornPicsDataAdapter.Update(PornPicsTable);
                //}
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
        }

        public void InsertPornPage(String domainName, String pageUrl, String pornUrl, bool isBlack)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kPornPagesInsert, domainName, pageUrl, pornUrl), PornDBConnection);
                cmd.ExecuteNonQuery();

                SQLiteCommand countCmd = new SQLiteCommand(String.Format(kPornPagesCountSelect, domainName), PornDBConnection);
                Object row = countCmd.ExecuteScalar();
                Int64 count = (Int64)row;
                log.Info("Domain: " + domainName + " contains porn: " + count);
                int thd = Properties.Settings.Default.thdPornPicsOfDomain;
                if (isBlack)
                    thd = Properties.Settings.Default.thdPornPicsOfBlackDomain;
                if (count > thd)
                {
                    log.Info("Add domain to tmp black: " + domainName);
                    Task tmpTask = InsertBlackDomainDetecedAsync(domainName);
                    //DeletePagesInList(domainName);
                }
                
                //lock (PornPagesTable.Rows.SyncRoot)
                //{
                //    PornPagesTable.Rows.Add(null, domainName, pageUrl, pornUrl);
                //    PornPagesDataAdatper.Update(PornPagesTable);
                //}
                //check if the number of porn pics of this domain exceed the threshold
                //DataRow[] rows = PornPagesTable.Select(String.Format("domain_name='{0}'", domainName));
                //log.Info("Domain name: " + domainName + " times: " + rows.Length);
                //if (rows.Length > Properties.Settings.Default.thdPornPicsOfDomain)
                //    InsertBlackDomainDeteced(domainName);
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
        }
        //delete porn pages belongs to a domain name which is already in black, white or tmp black
        private void DeletePagesInList(String domainName)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kPornPagesDelete, domainName), PornDBConnection);
                cmd.ExecuteNonQuery();
                //lock (PornPagesTable.Rows.SyncRoot)
                //{
                //    DataRow[] rows = PornPagesTable.Select(String.Format("domain_name='{0}'", domainName));
                //    foreach (DataRow eachrow in rows)
                //        eachrow.Delete();
                //    PornPagesDataAdatper.Update(PornPagesTable);
                //}
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
        public async Task InsertBlackDomainDetecedAsync(String domainName)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kDomainListInsertOrReplace, domainName, (Int64)DomainType.TmpBlack), PornDBConnection);
                cmd.ExecuteNonQuery();

                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);

                JObject submitTmpDomainObj = new JObject();
                submitTmpDomainObj["token"] = webToken;
                submitTmpDomainObj["domain_name"] = domainName;

                HttpResponseMessage msg = await client.PostAsync(Properties.Settings.Default.submitTmpDomainUrl, 
                    new StringContent(submitTmpDomainObj.ToString(),
                    Encoding.UTF8, "application/json"));
                //msg.EnsureSuccessStatusCode();
                //string retStr = await msg.Content.ReadAsStringAsync();
                //JObject retObj = JObject.Parse(retStr);
                await NotificationRoutines.SendPornDetectedNotification(domainName);
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
        public DomainType GetDomainType(String domainName)
        {
            SQLiteCommand cmd = new SQLiteCommand(String.Format(kDomainListGetType, domainName), PornDBConnection);
            Object t = cmd.ExecuteScalar();
            if (t == null)
                return DomainType.Undefined;
            return (DomainType)(Int64)t;
            
            //DataRow row = DomainTable.Rows.Find(domainName);
            //if (row == null)
            //    return DomainType.Undefined;
            //return (DomainType)(Int64)row["type"];
        }

        static public void Test()
        {
            PornDatabase porndb = new PornDatabase();
            SQLiteCommand ncmd = new SQLiteCommand(kGetLastIDSelect, porndb.PornDBConnection);
            int last_id = int.Parse(ncmd.ExecuteScalar().ToString());
            ncmd = new SQLiteCommand(String.Format(kUpdateLastIDInsert, 2), porndb.PornDBConnection);
            ncmd.ExecuteNonQuery();
            //DomainType t = porndb.GetDomainType("haoskys.com");
            //PornDatabase.DeleteHistroy(null);
            //porndb.InsertPornPage("feng.com", "sdfsdf", "xxxx");

            //porndb.MaintainDatabase();
            Task t = porndb.InsertBlackDomainDetecedAsync("kkk.com");
        }
        static public void Init()
        {
            try
            {
                //if (!Directory.Exists(Program.kWorkDir))
                //    Directory.CreateDirectory(Program.kWorkDir);
                if (!File.Exists(Program.AppLocalDir+Properties.Settings.Default.pornDbFileName))
                {
                    File.Copy(Properties.Settings.Default.pornDbFileName, Program.AppLocalDir + Properties.Settings.Default.pornDbFileName);
    //                //create database
    //                SQLiteConnection.CreateFile(Properties.Settings.Default.pornDbFileName);
    //                SQLiteConnection conn = new SQLiteConnection(kConnectionString);
    //                conn.Open();

    //                String sql =
    //"create table porn_pics(id integer primary key autoincrement, url text not null, type integer not null default 0, created_at datetime default current_timestamp);" +
    //"create index porn_pics_created_at_index on porn_pics(created_at);" +
    //"create table blocked_pages(id integer primary key autoincrement, url text not null, created_at datetime default current_timestamp);" +
    //"create index blocked_pages_created_at_index on blocked_pages(created_at);" +
    //"create table porn_pages(id integer primary key autoincrement, domain_name text not null, page_url text not null, porn_pic_url text not null, created_at datetime default current_timestamp, unique(page_url, porn_pic_url));" +
    //"create table domain_list(id integer primary key autoincrement, domain_name text not null unique, type integer not null default 0, created_at datetime default current_timestamp);";

    //                //"create table white_list(id integer primary key autoincrement, domain_name text not null unique, created_at datetime default current_timestamp);" +
    //                //"create table black_list(id integer primary key autoincrement, domain_name text not null unique, created_at datetime default current_timestamp);" +
    //                //"create table tmp_black_list(id integer primary key autoincrement, domain_name text not null unique, created_at datetime default current_timestamp);" +
                    //create table configs (id integer primary key autoincrement, name text not null unique, value text)

    //                SQLiteCommand command = new SQLiteCommand(sql, conn);
    //                command.ExecuteNonQuery();
    //                conn.Close();
                }
                //instance = new PornDatabase();
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
        public void DeleteHistoryPornPics()
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kHistoryPornPicSelect, Properties.Settings.Default.maxHistoryDays), PornDBConnection);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        String url = reader.GetString(0);
                        String url_encoded = Program.AppLocalDir + Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(url);
                        try { File.Delete(url_encoded); }
                        catch { }
                    }
                }
                SQLiteCommand dcmd = new SQLiteCommand(String.Format(kHistoryDelete, "porn_pics", Properties.Settings.Default.maxHistoryDays), PornDBConnection);
                dcmd.ExecuteNonQuery();
                //lock (PornPicsTable)
                //{
                //    DateTime nt = DateTime.Now.ToUniversalTime().AddDays(-Properties.Settings.Default.maxHistoryDays);
                //    DataRow[] rows = PornPicsTable.Select(String.Format("created_at<#{0}#", nt.ToString()));
                //    foreach (DataRow each_row in rows)
                //    {
                //        String url = (String)each_row["url"];
                //        String url_encoded = Properties.Settings.Default.imagesDir + "\\" + HttpUtility.UrlEncode(url);
                //        try { File.Delete(url_encoded); }
                //        catch { }
                //        each_row.Delete();
                //    }
                //    PornPicsDataAdapter.Update(PornPicsTable);
                //}
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }

        public void DeleteHistoryBlockedPages()
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(String.Format(kHistoryDelete, "blocked_pages", Properties.Settings.Default.maxHistoryDays), PornDBConnection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }

        static public void DeleteHistroy(Object state)
        {
            log.Info("Delete history!");
            try
            {
                PornDatabase db = new PornDatabase();
                db.DeleteHistoryPornPics();
                db.DeleteHistoryBlockedPages();
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
        static public void UpdateDatabase(Object state)
        {
            log.Info("Update Database");
            PornDatabase db = new PornDatabase();
            db.MaintainDatabase();
        }
        public void MaintainDatabase()
        {
            try
            {
                //SQLiteCommand ncmd = new SQLiteCommand(String.Format(kGetLastIDSelect, item.DomainName, (Int64)DomainType.White), PornDBConnection);
                SQLiteCommand ncmd = new SQLiteCommand(kGetLastIDSelect, PornDBConnection);
                int last_id = int.Parse(ncmd.ExecuteScalar().ToString());
                //update black, white and tmp lists
                HttpClientHandler handler = new HttpClientHandler() { UseProxy = false };
                HttpClient client = new HttpClient(handler);

                JObject updateDomainObj = new JObject();
                updateDomainObj["token"] = webToken;
                updateDomainObj["last_id"] = last_id;

                HttpResponseMessage msg = client.PostAsync(Properties.Settings.Default.domainUpdateLogsUrl, 
                    new StringContent(updateDomainObj.ToString(),
                    Encoding.UTF8, "application/json")).Result;
                msg.EnsureSuccessStatusCode();
                //string retStr = await msg.Content.ReadAsStringAsync();
                //JObject retObj = JObject.Parse(retStr);

                ////HttpResponseMessage response = await client.GetAsync(builder.Uri);
                List<DomainUpdateLogItem> log_items = new List<DomainUpdateLogItem>();
                String response_body = msg.Content.ReadAsStringAsync().Result;

                DataContractJsonSerializer json_serializer = new DataContractJsonSerializer(log_items.GetType());
                using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(response_body)))
                {
                    log_items = json_serializer.ReadObject(stream) as List<DomainUpdateLogItem>;
                }

                //DomainUpdateLogItem new_item = new DomainUpdateLogItem();
                //new_item.Id = 1;
                //new_item.DomainName = "add_white.com";
                //new_item.Operation = Operations.AddWhite;
                //log_items.Add(new_item);
                //new_item = new DomainUpdateLogItem();
                //new_item.Id = 2;
                //new_item.DomainName = "add_white2.com";
                //new_item.Operation = Operations.AddWhite;
                //log_items.Add(new_item);
                //new_item = new DomainUpdateLogItem();
                //new_item.Id = 3;
                //new_item.DomainName = "add_white.com";
                //new_item.Operation = Operations.AddBlack;
                //log_items.Add(new_item);

                //new_item = new DomainUpdateLogItem();
                //new_item.Id = 4;
                //new_item.DomainName = "add_black.com";
                //new_item.Operation = Operations.AddBlack;
                //log_items.Add(new_item);
                //new_item = new DomainUpdateLogItem();
                //new_item.Id = 5;
                //new_item.DomainName = "add_black2.com";
                //new_item.Operation = Operations.AddBlack;
                //log_items.Add(new_item);
                //new_item = new DomainUpdateLogItem();
                //new_item.Id = 6;
                //new_item.DomainName = "add_black.com";
                //new_item.Operation = Operations.AddWhite;
                //log_items.Add(new_item);
                if (log_items.Count > 0)
                {
                    SQLiteCommand cmd;
                    foreach (DomainUpdateLogItem item in log_items)
                    {
                        if (item.Id > last_id)
                            last_id = item.Id;
                        switch (item.Operation)
                        {
                            case Operations.AddWhite:
                                log.Info("Add white: " + item.DomainName);
                                DeletePagesInList(item.DomainName);
                                cmd = new SQLiteCommand(String.Format(kDomainListInsertOrReplace, item.DomainName, (Int64)DomainType.White), PornDBConnection);
                                cmd.ExecuteNonQuery();
                                //row = DomainTable.Rows.Find(item.DomainName);
                                //if (row != null)
                                //    row["type"] = DomainType.White;
                                //else
                                //    DomainTable.Rows.Add(null, item.DomainName, DomainType.White);
                                break;
                            case Operations.DeleteWhite:
                                log.Info("Delete white: " + item.DomainName);
                                cmd = new SQLiteCommand(String.Format(kDomainListDelete, item.DomainName), PornDBConnection);
                                cmd.ExecuteNonQuery();
                                //row = DomainTable.Rows.Find(item.DomainName);
                                //if (row != null)
                                //    row.Delete();
                                break;
                            case Operations.AddBlack:
                                log.Info("Add black: " + item.DomainName);
                                //DeletePagesInList(item.DomainName);
                                cmd = new SQLiteCommand(String.Format(kDomainListInsertOrIgnore, item.DomainName, (Int64)DomainType.Black), PornDBConnection);
                                cmd.ExecuteNonQuery();
                                //row = DomainTable.Rows.Find(item.DomainName);
                                //if (row != null)
                                //    row["type"] = DomainType.Black;
                                //else
                                //    DomainTable.Rows.Add(null, item.DomainName, DomainType.Black);
                                break;
                            case Operations.DeleteBlack:
                                log.Info("Delete black: " + item.DomainName);
                                cmd = new SQLiteCommand(String.Format(kDomainListDelete, item.DomainName), PornDBConnection);
                                cmd.ExecuteNonQuery();
                                //row = DomainTable.Rows.Find(item.DomainName);
                                //if (row != null)
                                //    row.Delete();
                                break;
                            default:
                                break;
                        }
                    }
                    //DomainDataAdapter.Update(DomainTable);

                    log.Info("Last id after update: " + last_id);
                    ncmd = new SQLiteCommand(String.Format(kUpdateLastIDInsert, last_id), PornDBConnection);
                    ncmd.ExecuteNonQuery();
                    //int last_id = int.Parse(ncmd.ExecuteScalar().ToString());
                    //Properties.Settings.Default.Save();
                }
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
        }
    }
}
