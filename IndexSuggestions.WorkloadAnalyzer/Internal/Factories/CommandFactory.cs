using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
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
        public IChainableCommand HandleExceptionCommand(IExecutableCommand onExceptionCommand = null, IExecutableCommand finallyCommand = null)
        {
            return new HandleExceptionCommand(ex => onExceptionCommand?.Execute(), () => finallyCommand?.Execute());
        }
        public IChainableCommand EvaluateIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            return new EvaluateIndicesEnvironmentsCommand(context, dbmsRepositories.GetVirtualIndicesRepository(), dbmsRepositories.GetExplainRepository(),
                                                      indexSqlCreateStatementGenerator);
        }

        public IChainableCommand GenerateBaseBtreeIndicesCommand(WorkloadAnalysisContext context)
        {
            return new GenerateBaseBtreeIndicesCommand(context);
        }

        public IChainableCommand GetRealExecutionPlansCommand(WorkloadAnalysisContext context)
        {
            return new GetExecutionPlansCommand(context, () => context.IndicesDesignData.RealExecutionPlansForStatements, dbmsRepositories.GetExplainRepository());
        }

        public IChainableCommand InitializeVirtualIndicesEnvironmentCommand(WorkloadAnalysisContext context)
        {
            return new InitializeVirtualIndicesEnvironmentCommand(context, dbmsRepositories.GetVirtualIndicesRepository());
        }

        public IChainableCommand LoadDatabaseInfoCommand(WorkloadAnalysisContext context)
        {
            return new LoadDatabaseInfoCommand(context, dbmsRepositories.GetDatabasesRepository());
        }

        public IChainableCommand LoadWorkloadCommand(WorkloadAnalysisContext context)
        {
            return new LoadWorkloadCommand(context, dalRepositories.GetWorkloadsRepository());
        }

        public IChainableCommand LoadWorkloadStatementsCommand(WorkloadAnalysisContext context)
        {
            return new LoadWorkloadStatementsCommand(context, dalRepositories.GetNormalizedWorkloadStatementsRepository());
        }

        public IChainableCommand LoadExistingIndicesCommand(WorkloadAnalysisContext context)
        {
            return new LoadExistingIndicesCommand(context, dbmsRepositories.GetIndicesRepository());
        }

        public IChainableCommand GenerateCoveringBtreeIndicesCommand(WorkloadAnalysisContext context)
        {
            return new GenerateCoveringBtreeIndicesCommand(context);
        }

        public IChainableCommand GenerateBaseIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            return new GenerateBaseIndicesEnvironmentsCommand(context);
        }

        public IChainableCommand ExtractStatementsQueryDataCommand(WorkloadAnalysisContext context)
        {
            return new ExtractStatementsQueryDataCommand(context, dbmsRepositories);
        }

        public IChainableCommand ExcludeExistingIndicesCommand(WorkloadAnalysisContext context)
        {
            return new ExcludeExistingIndicesCommand(context);
        }

        public IChainableCommand CleanUpNotImprovingIndiciesAndTheirEnvsCommand(WorkloadAnalysisContext context)
        {
            return new CleanUpNotImprovingIndiciesAndTheirEnvsCommand(context);
        }

        public IChainableCommand GenerateFinalIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            return new GenerateFinalIndicesEnvironmentsCommand(context);
        }

        public IChainableCommand GenerateAndEvaluateFilteredBtreeIndicesCommand(WorkloadAnalysisContext context)
        {
            return new GenerateAndEvaluateFilteredBtreeIndicesCommand(context, toSqlValueStringConverter, dbmsRepositories.GetVirtualIndicesRepository(), indexSqlCreateStatementGenerator);
        }

        public IChainableCommand UpdateAnalysisStateCommand(WorkloadAnalysisContext context, WorkloadAnalysisStateType state)
        {
            return new UpdateAnalysisStateCommand(context, state, dalRepositories.GetWorkloadAnalysesRepository());
        }

        public IChainableCommand PersistsIndicesDesignDataCommand(WorkloadAnalysisContext context)
        {
            return new PersistsIndicesDesignDataCommand(context, dalRepositories, indexSqlCreateStatementGenerator);
        }
    }
}
