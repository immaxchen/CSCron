using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace CSCron
{
    static class CronService
    {
        private static string CronFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSCron.txt");
        private static DateTime LastWrite = DateTime.MaxValue;
        private static List<CronJob> Jobs = new List<CronJob>();
        private static Timer Timer = new Timer(5000);
        private static DateTime LastExecute = DateTime.Now;
        private static string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSCron.log");

        static CronService()
        {
            if (!File.Exists(CronFile)) File.WriteAllText(CronFile, string.Empty);
            LastWrite = File.GetLastWriteTime(CronFile);
            UpdateJobs();
            Timer.AutoReset = true;
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        private static void TimerElapsed(object sender, EventArgs e)
        {
            if (DateTime.Now.Minute != LastExecute.Minute)
            {
                LastExecute = DateTime.Now;
                foreach (var job in Jobs) job.Execute();
            }
            if (File.GetLastWriteTime(CronFile) > LastWrite)
            {
                LastWrite = File.GetLastWriteTime(CronFile);
                UpdateJobs();
            }
        }

        private static void UpdateJobs()
        {
            Logging("updating job list");
            Jobs = new List<CronJob>();
            foreach (var line in File.ReadAllLines(CronFile))
            {
                if (line.Trim().StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;
                Jobs.Add(new CronJob(line));
            }
        }

        public static void Logging(string content)
        {
            File.AppendAllText(LogFile, string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}\n", DateTime.Now, content));
        }

        public static void OpenCronFile()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(o => System.Diagnostics.Process.Start("notepad.exe", CronFile));
        }

        public static void OpenLogFile()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(o => System.Diagnostics.Process.Start("notepad.exe", LogFile));
        }

        public static void ClearLogFile()
        {
            File.WriteAllText(LogFile, string.Empty);
        }
    }
}
