using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsService1
{

    public partial class Vault : ServiceBase
    {
        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = "C:\\Logs\\";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }

        public Vault()
        {
            CanStop = true;
            CanShutdown = true;
            CanPauseAndContinue = true;
            InitializeComponent();
            //var count = 10000000;
            //while (count >0)
            //{

            //    count--;
            //    Console.WriteLine(DateTime.Now.ToString());
            //    WriteLog(DateTime.Now.ToString());
            //    Task.Delay(1000).Wait();
            //}
            WriteLog("InitializeComponent");

        }
        //protected override void OnServiceFailed(Object sender, EventArgs e)
        //{
        //    base.OnServiceFailed(sender, e);
        //    // Handle service failure due to an invalid user account
        //    Console.WriteLine("Service failed to start due to an invalid user account.");
        //    // Log the error, notify administrators, or take appropriate action
        //}
        protected override void OnPause()
        {
            // Your service pause logic here
            WriteLog("Service paused successfully.");
        }
        protected override void OnStart(string[] args)
        {
            WriteLog("OnStart");
            InitializeTimer();
        }
        private Timer _timer;
        private int _counter = 0;
        private int _intervalInSeconds = 1; // Interval in seconds

        private void InitializeTimer()
        {
            // Create a new timer with the specified interval
            _timer = new Timer(_intervalInSeconds * 1000); // Convert seconds to milliseconds
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true; // Set to true if you want the timer to fire repeatedly
            _timer.Start();
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // This method will be called each time the timer interval elapses
            // Increment the counter
            _counter++;

            // Do something based on the counter value
            WriteLog($"Counter: {_counter}");
            WriteLog(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            WriteLog("User : " + System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString());
            WriteLog("AuthenticationType : " + System.Security.Principal.WindowsIdentity.GetCurrent().AuthenticationType.ToString());
            WriteLog("AccessToken : " + System.Security.Principal.WindowsIdentity.GetCurrent().AccessToken.ToString());
            WriteLog("IsAuthenticated : " + System.Security.Principal.WindowsIdentity.GetCurrent().IsAuthenticated.ToString());

            // You can perform any other logic here based on your requirements
        }
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            WriteLog("OnSessionChange");
            WriteLog(changeDescription.ToString());
            try
            {
                var user = (changeDescription.SessionId);

                if (changeDescription.Reason == SessionChangeReason.SessionLock ||
                    changeDescription.Reason == SessionChangeReason.SessionLogoff ||
                    changeDescription.Reason == SessionChangeReason.ConsoleDisconnect)

                    WriteLog(String.Format("{0} locked at {1}\r\n", user, DateTime.Now));

                else if (changeDescription.Reason == SessionChangeReason.SessionUnlock ||
                         changeDescription.Reason == SessionChangeReason.SessionLogon ||
                         changeDescription.Reason == SessionChangeReason.ConsoleConnect)

                    WriteLog(String.Format("{0} unlocked at {1}\r\n", user, DateTime.Now));

                base.OnSessionChange(changeDescription);
            }
            catch (Exception e)
            {
                WriteLog("Exception: " + e);
            }

            base.OnSessionChange(changeDescription);
        }
        protected override void OnStop()
        {
            WriteLog("OnStop");
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
