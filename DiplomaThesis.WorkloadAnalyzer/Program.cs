using DiplomaThesis.Common.CommandProcessing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
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
            var queue = new CommandProcessingQueue<IExecutableCommand>(log, "ProcessingQueue");
            var dalRepositories = DAL.RepositoriesFactory.Instance;
            var dbmsRepositories = DBMS.Postgres.RepositoriesFactory.Instance;
            var dbSystemInfo = dbmsRepositories.GetDatabaseSystemInfoRepository().LoadInfo();
            var toSqlValueStringConverter = new DBMS.Postgres.ToSqlValueStringConverter();
            var dbObjectDefinitionGenerator = new DBMS.Postgres.DbObjectDefinitionGenerator(dbSystemInfo.SupportsIncludeInIndices, toSqlValueStringConverter);
            var attributeHPartitioningDesigner = new AttributeHPartitioningDesigner(dbSystemInfo.SupportsHashHPartitioning);
            var commandFactory = new CommandFactory(log, appSettings, dalRepositories, dbmsRepositories, dbObjectDefinitionGenerator, toSqlValueStringConverter, attributeHPartitioningDesigner);
            var chainFactory = new CommandChainFactory(commandFactory);
            var requestsLoader = new AnalysisRequestsLoader(log, queue, dalRepositories.GetWorkloadAnalysesRepository(), chainFactory);
            requestsLoader.Start();
            Console.WriteLine("Analyzer is running. Press any key to exit...");
            Console.ReadLine();
            requestsLoader.Stop();
            requestsLoader.Dispose();
            queue.Dispose();
        }
    }
}
