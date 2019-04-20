using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = Common.Logging.NLog.NLog.Instace;
            var queue = new CommandProcessingQueue<IExecutableCommand>(log, "ProcessingQueue");
            var dalRepositories = DAL.RepositoriesFactory.Instance;
            var dbmsRepositories = DBMS.Postgres.RepositoriesFactory.Instance;
            var dbSystemInfo = dbmsRepositories.GetDatabaseSystemInfoRepository().LoadInfo();
            var indexCreateStatementGenerator = new IndexSqlCreateStatementGenerator(dbSystemInfo.SupportsIncludeInIndices);
            var toSqlValueStringConverter = new ToSqlValueStringConverter();
            var commandFactory = new CommandFactory(dalRepositories, dbmsRepositories, indexCreateStatementGenerator, toSqlValueStringConverter);
            var chainFactory = new CommandChainFactory(commandFactory);
            var requestsLoader = new AnalysisRequestsLoader(log, queue, dalRepositories.GetWorkloadAnalysesRepository(), chainFactory);
            requestsLoader.Start();
            Console.WriteLine("Analyzer is running. Pres any key to exit...");
            Console.ReadLine();
            requestsLoader.Stop();
            requestsLoader.Dispose();
            queue.Dispose();
        }
    }
}
