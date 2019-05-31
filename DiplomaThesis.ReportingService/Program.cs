using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.TaskScheduling;
using System;
using System.Collections.Generic;

namespace DiplomaThesis.ReportingService
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = Common.Logging.NLog.NLog.Instace;
            var queue = new CommandProcessingQueue<IExecutableCommand>(log, "ProcessingQueue");
            var dalRepositories = DAL.RepositoriesFactory.Instance;
            var dbmsRepositories = DBMS.Postgres.RepositoriesFactory.Instance;
            var razorEngine = new RazorEngine();
            var commands = new CommandFactory(log, dalRepositories, dbmsRepositories, razorEngine);
            var chains = new CommandChainFactory(commands);
            var regularTasks = PlanRegularTasks(chains);
            var taskScheduler = new RegularTaskScheduler(queue, regularTasks);
            taskScheduler.Start();
            Console.WriteLine("ReportingService is running. Type in S to immediately send current report. Type in any other key to exit...");
            bool canQuit = false;
            do
            {
                var line = (Console.ReadLine() ?? "").ToLower();
                if (line == "s")
                {
                    DateTime now = DateTime.Now;
                    var context = new ReportContextWithModel<SummaryEmailModel>();
                    context.DateFromInclusive = now.AddDays(-1);
                    context.DateToExclusive = now;
                    context.TemplateId = DAL.Contracts.SettingPropertyKeys.EMAIL_TEMPLATE_SUMMARY_REPORT;
                    try
                    {
                        chains.SummaryReportChain(context).Execute();
                        Console.WriteLine("Current report sent.");
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                    canQuit = false;
                }
                else
                {
                    canQuit = true;
                }
            } while (!canQuit);
            taskScheduler.Stop();
            taskScheduler.Dispose();
            queue.Dispose();
            razorEngine.Dispose();
        }

        private static Dictionary<TimeSpan, IExecutableCommand> PlanRegularTasks(ICommandChainFactory chains)
        {
            var regularTasks = new Dictionary<TimeSpan, IExecutableCommand>();
            regularTasks.Add(new TimeSpan(2, 0, 0), new ActionCommand(() =>
              {
                  DateTime previousDay = DateTime.Now.Date.AddDays(-1);
                  var context = new ReportContextWithModel<SummaryEmailModel>();
                  context.DateFromInclusive = previousDay;
                  context.DateToExclusive = previousDay.AddDays(1);
                  context.TemplateId = DAL.Contracts.SettingPropertyKeys.EMAIL_TEMPLATE_SUMMARY_REPORT;
                  chains.SummaryReportChain(context).Execute();
                  return true;
              }));
            return regularTasks;
        }
    }
}
