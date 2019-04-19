using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.IndexAnalysis
{
    internal class CommandFactory : ICommandFactory
    {
        private readonly IRepositoriesFactory dalRepositories;
        private readonly DBMS.Contracts.IRepositoriesFactory dbmsRepositories;
        private readonly IIndexSqlCreateStatementGenerator indexSqlCreateStatementGenerator;
        private readonly IToSqlValueStringConverter toSqlValueStringConverter;
        public CommandFactory(IRepositoriesFactory dalRepositories, DBMS.Contracts.IRepositoriesFactory dbmsRepositories,
                              IIndexSqlCreateStatementGenerator indexSqlCreateStatementGenerator, IToSqlValueStringConverter toSqlValueStringConverter)
        {
            this.dalRepositories = dalRepositories;
            this.dbmsRepositories = dbmsRepositories;
            this.indexSqlCreateStatementGenerator = indexSqlCreateStatementGenerator;
            this.toSqlValueStringConverter = toSqlValueStringConverter;
        }

        public IChainableCommand EvaluateIndicesEnvironmentsCommand(DesignIndicesContext context)
        {
            return new EvaluateIndicesEnvironmentsCommand(context, dbmsRepositories.GetVirtualIndicesRepository(), dbmsRepositories.GetExplainRepository(),
                                                      indexSqlCreateStatementGenerator);
        }

        public IChainableCommand GenerateBaseBtreeIndicesCommand(DesignIndicesContext context)
        {
            return new GenerateBaseBtreeIndicesCommand(context);
        }

        public IChainableCommand GetRealExecutionPlansCommand(DesignIndicesContext context)
        {
            return new GetExecutionPlansCommand(context, () => context.RealExecutionPlansForStatements, dbmsRepositories.GetExplainRepository());
        }

        public IChainableCommand InitializeVirtualIndicesEnvironmentCommand(DesignIndicesContext context)
        {
            return new InitializeVirtualIndicesEnvironmentCommand(context, dbmsRepositories.GetVirtualIndicesRepository());
        }

        public IChainableCommand LoadDatabaseInfoCommand(DesignIndicesContext context)
        {
            return new LoadDatabaseInfoCommand(context, dbmsRepositories.GetDatabasesRepository());
        }

        public IChainableCommand LoadWorkloadCommand(DesignIndicesContext context)
        {
            return new LoadWorkloadCommand(context, dalRepositories.GetWorkloadsRepository());
        }

        public IChainableCommand LoadWorkloadStatementsCommand(DesignIndicesContext context)
        {
            return new LoadWorkloadStatementsCommand(context, dalRepositories.GetNormalizedWorkloadStatementsRepository());
        }

        public IChainableCommand LoadExistingIndicesCommand(DesignIndicesContext context)
        {
            return new LoadExistingIndicesCommand(context, dbmsRepositories.GetIndicesRepository());
        }

        public IChainableCommand GenerateCoveringBtreeIndicesCommand(DesignIndicesContext context)
        {
            return new GenerateCoveringBtreeIndicesCommand(context);
        }

        public IChainableCommand GenerateBaseIndicesEnvironmentsCommand(DesignIndicesContext context)
        {
            return new GenerateBaseIndicesEnvironmentsCommand(context);
        }

        public IChainableCommand ExtractStatementsQueryDataCommand(DesignIndicesContext context)
        {
            return new ExtractStatementsQueryDataCommand(context, dbmsRepositories);
        }

        public IChainableCommand ExcludeExistingIndicesCommand(DesignIndicesContext context)
        {
            return new ExcludeExistingIndicesCommand(context);
        }

        public IChainableCommand CleanUpNotImprovingIndiciesAndTheirEnvsCommand(DesignIndicesContext context)
        {
            return new CleanUpNotImprovingIndiciesAndTheirEnvsCommand(context);
        }

        public IChainableCommand GenerateFinalIndicesEnvironmentsCommand(DesignIndicesContext context)
        {
            return new GenerateFinalIndicesEnvironmentsCommand(context);
        }

        public IChainableCommand GenerateFilteredBtreeIndicesCommand(DesignIndicesContext context)
        {
            return new GenerateFilteredBtreeIndicesCommand(context, toSqlValueStringConverter);
        }
    }
}
