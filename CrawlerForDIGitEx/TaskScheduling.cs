using Microsoft.Win32.TaskScheduler;
using System;

namespace CrawlerForDIGitEx
{
    static class TaskScheduling
    {
        const String sTaskName = "DIGitCrawler";
        public static void ScheduleTask()
        {
            DateTime dtSchedule = DateTime.Now.Date + new TimeSpan(7, 0, 0);
            using (TaskService ts = new TaskService())
            {
                if (TaskExist()) return;

                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "This task crawls Cushman Wakefield and dtzVancouver.";
                td.Principal.LogonType = TaskLogonType.InteractiveToken;

                //creating a daily task
                td.Data = "D";
                DailyTrigger dt = (DailyTrigger)td.Triggers.Add(
                new DailyTrigger
                {
                    StartBoundary = dtSchedule,
                    DaysInterval = 1
                });

                String sAppPath = FileIO.GetDefaultDirPath() + "\\CrawlerForDIGitEx.exe";

                td.Actions.Add(new ExecAction(sAppPath, "c:\\CrawlerForDIGitEx.log", null));

                ts.RootFolder.RegisterTaskDefinition(sTaskName, td);
            }

        }

        static Boolean TaskExist()
        {
            Boolean bResult = false;
            using (TaskService ts = new TaskService())
            { 
                TaskDefinition td = ts.NewTask();
                td.Principal.LogonType = TaskLogonType.InteractiveToken;

                try
                {
                    Microsoft.Win32.TaskScheduler.Task t = ts.GetTask(sTaskName);
                    bResult = !(t == null);
                }
                catch (Exception)
                {
                    bResult = false;
                }
            }

            return bResult;
        }
    }
}
