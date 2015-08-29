using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
//using Senparc.Weixin.MP.AdvancedAPIs;
//using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using System.Web;
using log4net;
//using Senparc.Weixin.MP.CommonAPIs;
using System.Drawing;
using System.IO;

namespace troll_ui_app
{
    class NotificationRoutines
    {
        static readonly ILog log = Log.Get();
        public static async Task SendUninstallNotification()
        {
            await WechatForm.SendUninstallNotificationAsync();
        }
        public static async Task SendPornDetectedNotification(string domain_name)
        {
            await WechatForm.SendPornDetectedNotificationAsync(domain_name);
        }
        public static async Task SendPornScannedNotification(int picNum)
        {
            await WechatForm.SendPornScannedNotificationAsync(picNum.ToString());
        }
        //public static async Task SendContactChangedNotification(string newEmail)
        //{
        //}
        //public static async Task SendExitNotification()
        //{ 
        //}
        //public static async Task SendStopNotification()
        //{
        //}
    }
}
