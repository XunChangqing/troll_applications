using System.IO;

namespace troll_ui_app
{
    public static class Utils
    {
        public static void Log_Init()
        {
#if DEBUG
            string cfg = @"<?xml version='1.0' encoding='utf-8'?>
<log4net>
  <root>
    <level value='DEBUG' />
    <appender-ref ref='ConsoleLog' />
    <appender-ref ref='FileLog' />
  </root>

  <logger name='Trollwiz'>
    <level value='DEBUG'/>
  </logger>

  <appender name='ConsoleLog' type='log4Net.Appender.ColoredConsoleAppender'>
    <layout type='log4net.Layout.PatternLayout'>
      <conversionPattern value='[%t] %-5p %c - %m%n'  />
    </layout>
    <mapping>
      <level value='ERROR' />
      <foreColor value='White' />
      <backColor value='Red' />
    </mapping>
    <mapping>
      <level value='INFO' />
      <foreColor value='White' />
    </mapping>
    <mapping>
      <level value='DEBUG' />
      <foreColor value='Green' />
    </mapping>
  </appender>

  <appender name='FileLog' type='log4net.Appender.RollingFileAppender'>
    <file value='{0}/logs/trollwiz.txt' />
    <appendToFile value='true' />
    <maximumFileSize value='1000KB' />
    <rollingStyle value='Size' />
    <maxSizeRollBackups value='3' />
    <layout type='log4net.Layout.PatternLayout'>
      <conversionPattern value='%d [%t] %-5p %c - %m%n'  />
    </layout>
  </appender>
</log4net>";
#else
            string cfg = @"<?xml version='1.0' encoding='utf-8'?>
<log4net>
  <root>
    <level value='DEBUG' />
    <appender-ref ref='FileLog' />
  </root>

  <logger name='Trollwiz'>
    <level value='DEBUG'/>
  </logger>

  <appender name='FileLog' type='log4net.Appender.RollingFileAppender'>
    <file value='{0}/logs/trollwiz.txt' />
    <appendToFile value='true' />
    <maximumFileSize value='1000KB' />
    <rollingStyle value='Size' />
    <maxSizeRollBackups value='3' />
    <layout type='log4net.Layout.PatternLayout'>
      <conversionPattern value='%d [%t] %-5p %c - %m%n'  />
    </layout>
  </appender>
</log4net>";

#endif
            string ccfg = string.Format(cfg, Program.AppLocalDir);
            log4net.Config.XmlConfigurator.Configure(
                new MemoryStream(System.Text.Encoding.ASCII.GetBytes(ccfg)));
        }
    }
}
