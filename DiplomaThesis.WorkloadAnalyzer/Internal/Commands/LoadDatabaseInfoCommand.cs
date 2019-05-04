using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class LoadDatabaseInfoCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IDatabasesRepository databasesRepository;

        public LoadDatabaseInfoCommand(WorkloadAnalysisContext context, IDatabasesRepository databasesRepository)
        {
            this.context = context;
            this.databasesRepository = databasesRepository;
        }

        protected override void OnExecute()
        {
            context.Database = databasesRepository.Get(context.Workload.DatabaseID);
        }
    }
}