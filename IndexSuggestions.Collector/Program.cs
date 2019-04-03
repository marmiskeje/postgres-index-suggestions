using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.Cache;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace IndexSuggestions.Collector
{
    class Program
    {
        /// <summary>
        /// Treba si ujasnit spracovanie logov. Nestaci iba spracovavat logovane statementy? Normalizovat ich a ulozit aspon jednu verziu, aby bolo mozne ziskat exekucny plan?
        /// Naco je dobre logovat plan a parse tree? Parse tree netreba - pouzije sa pg query. PLAN TREBA!! Ako inak namapovat dotaz a pouzity index?!!
        /// Je mozne, ze idealne riesenie bude: sledovat iba statements - vieme dobu vykonavania aa potom ked bude spustene sledovanie kvoli indexom, tak ukladat aj vsetky vykonane dotazy na analyzu - pre kazdy dotaz sa ziska normovana verzia a k nej sa ulozia vsetky realne
        /// Pozor - ale pri funkciach sa nam zide parse tree, lebo inak nevieme aky statement bol vykonany. PgQuery to nevie? Ze nam vrati rovno prepisany statement? (bez anonymizacie)
        /// 
        /// Log nam garantuje, ze su do neho vyliate zaznamy vztahujuce sa k statementu hned po sebe? Ak ano, tak grupuj hned v LogProcesore!!!! Toto treba overit.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var appSettings = configuration.Get<AppSettings>();

            var log = Common.Logging.NLog.NLog.Instace;
            var collectorConfiguration = new CollectorConfiguration(appSettings);
            var logProcessor = new Postgres.LogProcessor(collectorConfiguration.LogProcessing);
            var logEntryGroupBox = new Postgres.LogEntryGroupBox();
            var oneFileProcessor = new FileProcessor(log, collectorConfiguration.LogProcessing, logProcessor, logEntryGroupBox);
            var continuousFileProcessor = new ContinuousFileProcessor(log, collectorConfiguration.LogProcessing, logProcessor, logEntryGroupBox);
            var logProcessingService = new LogProcessingService(collectorConfiguration.LogProcessing, oneFileProcessor, continuousFileProcessor);
            var logEntriesProcessingQueue = new CommandProcessingQueue<IExecutableCommand>(log, "LogEntriesProcessingQueue");
            var dateTimeSelectors = DateTimeSelectorsProvider.Instance;
            var statementDataAccumulator = new StatementsProcessingDataAccumulator(dateTimeSelectors.MinuteSelector);
            var postgresRepositories = DBMS.Postgres.RepositoriesFactory.Instance;
            var dalRepositories = RepositoriesFactory.Instance;
            var lastProcessedEvidence = new LastProcessedLogEntryEvidence(log, dalRepositories.GetSettingPropertiesRepository());
            var generalCommands = new GeneralProcessingCommandFactory(log, collectorConfiguration, statementDataAccumulator, postgresRepositories,
                                                                      dalRepositories, lastProcessedEvidence);
            var externalCommands = new Postgres.LogEntryProcessingCommandFactory(postgresRepositories);
            var chainFactory = new LogEntryProcessingChainFactory(log, generalCommands, externalCommands, dalRepositories, statementDataAccumulator);
            var logEntryProcessingService = new LogEntryProcessingService(logEntryGroupBox, logEntriesProcessingQueue, chainFactory, lastProcessedEvidence);
            var statisticsProcessingQueue = new CommandProcessingQueue<IExecutableCommand>(log, "StatisticsProcessingQueue");
            var statisticsDataAccumulator = new StatisticsProcessingDataAccumulator(dateTimeSelectors.MinuteSelector, CacheProvider.Instance.MemoryCache);
            var statisticsCommands = new StatisticsProcessingCommandFactory(log, statisticsDataAccumulator, postgresRepositories, dalRepositories);
            var statisticsChainFactory = new StatisticsProcessingChainFactory(statisticsCommands);
            var statisticsCollectorService = new StatisticsCollectorService(statisticsProcessingQueue, statisticsChainFactory);

            var mergeStatisticsCommands = new MergeStatisticsCommandFactory(dalRepositories);
            var mergeStatisticsChainFactory = new MergeStatisticsChainFactory(mergeStatisticsCommands);
            Dictionary<TimeSpan, IExecutableCommand> regularTasks = new Dictionary<TimeSpan, IExecutableCommand>();
            regularTasks.Add(new TimeSpan(1,0,0), new ActionCommand(() =>
            {
                DateTime now = DateTime.Now;
                var context = new MergeNormalizedStatementStatisticsContext() { CreatedDateFrom = now.AddDays(-7), CreatedDateTo = now.AddDays(-9), DateTimeSelector = dateTimeSelectors.HourSelector };
                mergeStatisticsChainFactory.MergeStatisticsChain(context).Execute();
                var context2 = new MergeNormalizedStatementRelationStatisticsContext() { CreatedDateFrom = now.AddDays(-7), CreatedDateTo = now.AddDays(-9), DateTimeSelector = dateTimeSelectors.HourSelector };
                mergeStatisticsChainFactory.MergeStatisticsChain(context2).Execute();
                var context3 = new MergeNormalizedStatementIndexStatisticsContext() { CreatedDateFrom = now.AddDays(-7), CreatedDateTo = now.AddDays(-9), DateTimeSelector = dateTimeSelectors.HourSelector };
                mergeStatisticsChainFactory.MergeStatisticsChain(context3).Execute();
                return true;
            }));
            regularTasks.Add(new TimeSpan(2, 0, 0), new ActionCommand(() =>
            {
                DateTime now = DateTime.Now;
                var context = new MergeNormalizedStatementStatisticsContext() { CreatedDateFrom = now.AddDays(-14), CreatedDateTo = now.AddDays(-16), DateTimeSelector = dateTimeSelectors.DaySelector };
                mergeStatisticsChainFactory.MergeStatisticsChain(context).Execute();
                var context2 = new MergeNormalizedStatementRelationStatisticsContext() { CreatedDateFrom = now.AddDays(-14), CreatedDateTo = now.AddDays(-16), DateTimeSelector = dateTimeSelectors.DaySelector };
                mergeStatisticsChainFactory.MergeStatisticsChain(context2).Execute();
                var context3 = new MergeNormalizedStatementIndexStatisticsContext() { CreatedDateFrom = now.AddDays(-14), CreatedDateTo = now.AddDays(-16), DateTimeSelector = dateTimeSelectors.DaySelector };
                mergeStatisticsChainFactory.MergeStatisticsChain(context3).Execute();
                return true;
            }));
            var taskScheduler = new RegularTaskScheduler(statisticsProcessingQueue, regularTasks);

            statisticsCollectorService.Start();
            logProcessingService.Start();
            logEntryProcessingService.Start();
            taskScheduler.Start();
            Console.WriteLine("Collector is running. Pres any key to exit...");
            Console.ReadLine();
            taskScheduler.Dispose();
            logProcessingService.Dispose();
            continuousFileProcessor.Dispose();
            logEntryGroupBox.Dispose();
            logEntryProcessingService.Dispose();
            statisticsCollectorService.Dispose();
            logEntriesProcessingQueue.Dispose();
            statisticsProcessingQueue.Dispose();
        }
    }
}
