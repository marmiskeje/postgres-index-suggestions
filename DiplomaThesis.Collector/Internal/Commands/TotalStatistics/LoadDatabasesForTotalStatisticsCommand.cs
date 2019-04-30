using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal class LoadDatabasesForTotalStatisticsCommand : ChainableCommand
    {
        private readonly TotalStatisticsCollectNextSampleContext context;
        private readonly IDatabasesRepository databasesRepository;
        public LoadDatabasesForTotalStatisticsCommand(TotalStatisticsCollectNextSampleContext context, IDatabasesRepository databasesRepository)
        {
            this.context = context;
            this.databasesRepository = databasesRepository;
        }
        protected override void OnExecute()
        {
            context.Databases.AddRange(databasesRepository.GetAll());
        }
    }
}
