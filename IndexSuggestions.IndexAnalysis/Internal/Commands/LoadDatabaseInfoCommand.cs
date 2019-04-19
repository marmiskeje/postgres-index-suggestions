using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.IndexAnalysis
{
    internal class LoadDatabaseInfoCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        private readonly IDatabasesRepository databasesRepository;

        public LoadDatabaseInfoCommand(DesignIndicesContext context, IDatabasesRepository databasesRepository)
        {
            this.context = context;
            this.databasesRepository = databasesRepository;
        }

        protected override void OnExecute()
        {
            context.Database = databasesRepository.Get(context.Workload.Definition.DatabaseID);
        }
    }
}