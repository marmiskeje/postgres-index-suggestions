using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.Logging;
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
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var appSettings = configuration.Get<AppSettings>();

            var log = Common.Logging.NLog.NLog.Instace;
            var collectorConfiguration = new CollectorConfiguration(appSettings);
            var logProcessor = new Postgres.LogProcessor(collectorConfiguration.LogProcessing);
            var continuousFileProcessor = new ContinuousFileProcessor(log, collectorConfiguration.LogProcessing, logProcessor);
            var logProcessingService = new LogProcessingService(collectorConfiguration.LogProcessing, logProcessor, continuousFileProcessor);


            logProcessingService.Start();
            Console.WriteLine("Collector is running. Pres any key to exit...");
            Console.ReadLine();
            logProcessingService.Dispose();
            continuousFileProcessor.Dispose();
        }
    }
}
