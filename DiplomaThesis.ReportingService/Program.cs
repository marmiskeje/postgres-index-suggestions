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
            Console.WriteLine("ReportingService is running. Pres any key to exit...");
            Console.ReadLine();
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
