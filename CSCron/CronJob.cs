using Cronos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CSCron
{
    class CronJob
    {
        static readonly Regex regex = new Regex(@"\S+");

        string exprstr = null;
        string command = null;
        CronExpression exprobj = null;
        DateTime? next = null;

        public CronJob(string line)
        {
            int fieldcount = 0;
            foreach (Match match in regex.Matches(line))
            {
                fieldcount++;
                if (fieldcount == 6)
                {
                    try
                    {
                        exprstr = line.Substring(0, match.Index - 1).Trim();
                        command = line.Substring(match.Index).Trim();
                        exprobj = CronExpression.Parse(exprstr);
                        next = exprobj.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local)?.DateTime;
                    }
                    catch { }
                    break;
                }
            }
            if (next == null)
            {
                CronService.Logging(string.Format("unable to create job: {0}", line));
            }
            else
            {
                CronService.Logging(string.Format("job created successfully: {0} {1}", exprstr, command));
            }
        }

        public void Execute()
        {
            if (DateTime.Now > next)
            {
                CronService.Logging(string.Format("run: {0}", command));
                ThreadPool.QueueUserWorkItem(RunCommand);
                next = exprobj.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local)?.DateTime;
            }
        }

        void RunCommand(object state)
        {
            var proc = new Process();
            var info = new ProcessStartInfo();
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.FileName = "cmd.exe";
            info.Arguments = string.Format("/C {0}", command);
            proc.StartInfo = info;
            proc.Start();
        }
    }
}
