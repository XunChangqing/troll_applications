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
    class MailRoutines
    {
        static readonly ILog log = Log.Get();
        private static async Task SendMail(string subject, string body)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential("14917793", "x152300");
                client.Host = "smtp.qq.com";

                MailMessage mail = new MailMessage("xunchangqing@qq.com", Properties.Settings.Default.email);
                mail.Subject = subject;
                mail.Body = body;
                //client.Send(mail);
                await client.SendMailAsync(mail);
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
            return;
        }

        public static async Task SendUninstallMail()
        {
            await SendMail("山妖卫士-卸载提醒", "您安装于机器：" + Environment.MachineName + "上的山妖卫士即将被卸载，感谢您的使用！");
        }
        public static async Task SendMailChangedNotification(string newEmail)
        { 
            await SendMail("山妖卫士-邮件地址修改提醒", "您安装于机器： "+Environment.MachineName +"上的山妖卫士修改了通知邮件地址，原地址为："+Properties.Settings.Default.email+"，新地址为："+newEmail);
        }
        public static async Task SendExitNotification()
        { 
            await SendMail("山妖卫士-退出提醒", "您安装于机器： "+Environment.MachineName +"上的山妖卫士正在退出!");
        }
        public static async Task SendShutdownNotification()
        {
            await SendMail("山妖卫士-关闭防护", "您安装于机器： "+Environment.MachineName +"上的山妖卫士已关闭防护!");
        }
    }
}
