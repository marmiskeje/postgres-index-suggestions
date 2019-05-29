using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.Cache;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.TaskScheduling;
using DiplomaThesis.DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DiplomaThesis.Collector
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var appSettings = configuration.Get<AppSettings>();

            var log = Common.Logging.NLog.NLog.Instace;
            NLog.LogManager.LoadConfiguration("nlog.config");
            var collectorConfiguration = new CollectorConfiguration(appSettings);
            var logProcessor = new Postgres.LogProcessor(collectorConfiguration.LogProcessing);
            var logEntryGroupBox = new Postgres.LogEntryGroupBox();
            var oneFileProcessor = new FileProcessor(log, collectorConfiguration.LogProcessing, logProcessor, logEntryGroupBox);
            var continuousFileProcessor = new ContinuousFileProcessor(log, collectorConfiguration.LogProcessing, logProcessor, logEntryGroupBox);
            var logProcessingService = new LogProcessingService(collectorConfiguration.LogProcessing, oneFileProcessor, continuousFileProcessor);
            var logEntriesProcessingQueue = new CommandProcessingQueue<IExecutableCommand>(log, "LogEntriesProcessingQueue");
            var statisticsProcessingQueue = new CommandProcessingQueue<IExecutableCommand>(log, "StatisticsProcessingQueue");
            var dateTimeSelectors = DateTimeSelectorsProvider.Instance;
            var statementDataAccumulator = new StatementsProcessingDataAccumulator(dateTimeSelectors.MinuteSelector);
            var statisticsDataAccumulator = new StatisticsProcessingDataAccumulator(dateTimeSelectors.MinuteSelector, CacheProvider.Instance.MemoryCache);
            var postgresRepositories = DBMS.Postgres.RepositoriesFactory.Instance;
            var dalRepositories = RepositoriesFactory.Instance;
            var lastProcessedEvidence = new LastProcessedLogEntryEvidence(log, dalRepositories.GetSettingPropertiesRepository());
            var dependencyHierarchyProvider = new DatabaseDependencyHierarchyProvider(log, new Postgres.DatabaseDependencyHierarchyBuilder(log, postgresRepositories), postgresRepositories.GetDatabasesRepository());
            var generalCommands = new GeneralProcessingCommandFactory(log, collectorConfiguration, statementDataAccumulator, postgresRepositories,
                                                                      dalRepositories, lastProcessedEvidence);
            var externalCommands = new Postgres.LogEntryProcessingCommandFactory(postgresRepositories);
            var statisticsCommands = new StatisticsProcessingCommandFactory(log, statisticsProcessingQueue, statisticsDataAccumulator, postgresRepositories, dalRepositories, dependencyHierarchyProvider);
            var logEntryProcessingChainFactory = new LogEntryProcessingChainFactory(log, generalCommands, statisticsCommands, externalCommands, dalRepositories);
            var statisticsChainFactory = new StatisticsProcessingChainFactory(statisticsCommands);
            var logEntryProcessingService = new LogEntryProcessingService(logEntryGroupBox, logEntriesProcessingQueue, logEntryProcessingChainFactory, lastProcessedEvidence);
            var statisticsCollectorService = new StatisticsCollectorService(statisticsProcessingQueue, statisticsChainFactory);

            var mergeStatisticsCommands = new MergeStatisticsCommandFactory(dalRepositories);
            var mergeStatisticsChainFactory = new MergeStatisticsChainFactory(mergeStatisticsCommands);
            var regularTasks = PlanRegularTasks(mergeStatisticsChainFactory, dateTimeSelectors);
            var taskScheduler = new RegularTaskScheduler(statisticsProcessingQueue, regularTasks);

            statisticsCollectorService.Start();
            logProcessingService.Start();
            logEntryProcessingService.Start();
            taskScheduler.Start();
            Console.WriteLine("Collector is running. Pres any key to exit...");
            Console.ReadLine();
            dependencyHierarchyProvider.Dispose();
            taskScheduler.Dispose();
            logProcessingService.Dispose();
            continuousFileProcessor.Dispose();
            logEntryGroupBox.Dispose();
            logEntryProcessingService.Dispose();
            statisticsCollectorService.Dispose();
            logEntriesProcessingQueue.Dispose();
            statisticsProcessingQueue.Dispose();
        }

        private static Dictionary<TimeSpan, IExecutableCommand> PlanRegularTasks(IMergeStatisticsChainFactory mergeStatisticsChainFactory, IDateTimeSelectorsProvider dateTimeSelectors)
        {
            var regularTasks = new Dictionary<TimeSpan, IExecutableCommand>();
            regularTasks.Add(new TimeSpan(1, 0, 0), new ActionCommand(() =>
            {
                DateTime now = DateTime.Now;
                DateTime from = now.AddDays(-7);
                DateTime to = now.AddDays(-9);
                MergeStatistics(from, to, dateTimeSelectors.HourSelector);
                return true;
            }));
            regularTasks.Add(new TimeSpan(2, 0, 0), new ActionCommand(() =>
            {
                DateTime now = DateTime.Now;
                DateTime from = now.AddDays(-14);
                DateTime to = now.AddDays(-16);
                MergeStatistics(from, to, dateTimeSelectors.DaySelector);
                return true;
            }));
            #region void MergeStatistics(DateTime from, DateTime to, IDateTimeSelector dateTimeSelector)
            void MergeStatistics(DateTime from, DateTime to, IDateTimeSelector dateTimeSelector)
            {
                ParallelCommandStepsCreator parallelSteps = new ParallelCommandStepsCreator();
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeNormalizedStatementStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeNormalizedStatementRelationStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeNormalizedStatementIndexStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeTotalRelationStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeTotalIndexStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeTotalStoredProcedureStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.AddParallelStep(mergeStatisticsChainFactory.MergeStatisticsChain(new MergeTotalViewStatisticsContext()
                {
                    CreatedDateFrom = from,
                    CreatedDateTo = to,
                    DateTimeSelector = dateTimeSelector
                }).AsChainableCommand());
                parallelSteps.CreateParallelCommand().Execute();
            } 
            #endregion
            return regularTasks;
        }
    }
}
