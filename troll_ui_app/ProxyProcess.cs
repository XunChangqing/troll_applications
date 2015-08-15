using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
namespace troll_ui_app
{
    class ProxyProcess
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        private Process process = new Process();
        const string process_name = "proxy-console.exe";
        public void StartProxyServer()
        {
            try
            {
                //check if the proxy server is still alive
                process.StartInfo.FileName = process_name;
                process.Exited += new EventHandler(myProcess_Exited);
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = true;
                //process.StartInfo.Arguments = "--workdir " + Program.kWorkDir;
                var fi = new FileInfo(Application.ExecutablePath);
                process.StartInfo.WorkingDirectory = fi.DirectoryName;
                process.Start();
            }
            catch(Exception e)
            {
                process = null;
                //EventLog.WriteEntry(Program.kEventSource, "Cannot start proxy process: "+e.ToString(), EventLogEntryType.Warning);
            }
            //record the id
        }
        public void CloseProxyServer()
        {
            if(process != null)
                StopProcess(process);
        }
        private void StopProcess(Process proc)
        {
            //This does not require the console window to be visible.
            if (AttachConsole((uint)proc.Id))
            {
                // Disable Ctrl-C handling for our program
                SetConsoleCtrlHandler(null, true);
                GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);

                // Must wait here. If we don't and re-enable Ctrl-C
                // handling below too fast, we might terminate ourselves.
                proc.WaitForExit(2000);
                if (!proc.HasExited)
                    proc.Kill();

                FreeConsole();

                //Re-enable Ctrl-C handling or any subsequently started
                //programs will inherit the disabled state.
                SetConsoleCtrlHandler(null, false);
            }
        }
        // Handle Exited event and display process information.
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            //EventLog.WriteEntry(Program.kEventSource, "Proxy process exit!", EventLogEntryType.Information);
            //eventHandled = true;
            //Console.WriteLine("Exit time:    {0}\r\n" + "Exit code:    {1}\r\n", process.ExitTime, process.ExitCode);
        }
        //processes = Process.GetProcessesByName(procName);
        //foreach (Process proc in processes)
    }
}
