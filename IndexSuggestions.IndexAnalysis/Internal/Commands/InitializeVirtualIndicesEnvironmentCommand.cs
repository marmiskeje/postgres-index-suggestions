using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.IndexAnalysis
{
    internal class InitializeVirtualIndicesEnvironmentCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        private readonly IVirtualIndicesRepository repository;

        public InitializeVirtualIndicesEnvironmentCommand(DesignIndicesContext context, IVirtualIndicesRepository repository)
        {
            this.context = context;
            this.repository = repository;
        }

        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                repository.InitializeEnvironment();
            }
        }
    }
}