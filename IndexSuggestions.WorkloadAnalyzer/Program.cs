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
            var toSqlValueStringConverter = new ToSqlValueStringConverter();
            var sqlCreateStatementGenerator = new SqlCreateStatementGenerator(dbSystemInfo.SupportsIncludeInIndices, toSqlValueStringConverter);
            var attributeHPartitioningDesigner = new AttributeHPartitioningDesigner();
            var commandFactory = new CommandFactory(dalRepositories, dbmsRepositories, sqlCreateStatementGenerator, toSqlValueStringConverter, attributeHPartitioningDesigner);
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
