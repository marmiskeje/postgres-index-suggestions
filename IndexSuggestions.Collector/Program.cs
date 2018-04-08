using IndexSuggestions.Collector.Contracts;
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
            var queue = new CommandProcessingQueue<IExecutableCommand>(log, "ProcessingQueue");
            var commandFactory = new Postgres.LogEntryProcessingCommandFactory(DBMS.Postgres.RepositoriesFactory.Instance);
            var repositoriesFactory = RepositoriesFactory.Instance;
            var lastProcessedEvidence = new LastProcessedLogEntryEvidence(log, repositoriesFactory.GetSettingPropertiesRepository());
            var chainFactory = new LogEntryProcessingChainFactory(log, commandFactory, repositoriesFactory, lastProcessedEvidence);
            var logEntryProcessingService = new LogEntryProcessingService(logEntryGroupBox, queue, chainFactory, lastProcessedEvidence);

            logProcessingService.Start();
            logEntryProcessingService.Start();
            Console.WriteLine("Collector is running. Pres any key to exit...");
            Console.ReadLine();
            logProcessingService.Dispose();
            continuousFileProcessor.Dispose();
            queue.Dispose();
            logEntryGroupBox.Dispose();
            logEntryProcessingService.Dispose();
            lastProcessedEvidence.Dispose();
        }
    }
}
