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
        private readonly DBMS.Contracts.IDbObjectDefinitionGenerator dbObjectDefinitionGenerator;
        private readonly DBMS.Contracts.IToSqlValueStringConverter toSqlValueStringConverter;
        private readonly IAttributeHPartitioningDesigner attributeHPartitioningDesigner;
        public CommandFactory(IRepositoriesFactory dalRepositories, DBMS.Contracts.IRepositoriesFactory dbmsRepositories,
                              DBMS.Contracts.IDbObjectDefinitionGenerator dbObjectDefinitionGenerator, DBMS.Contracts.IToSqlValueStringConverter toSqlValueStringConverter,
                              IAttributeHPartitioningDesigner attributeHPartitioningDesigner)
        {
            this.dalRepositories = dalRepositories;
            this.dbmsRepositories = dbmsRepositories;
            this.dbObjectDefinitionGenerator = dbObjectDefinitionGenerator;
            this.toSqlValueStringConverter = toSqlValueStringConverter;
            this.attributeHPartitioningDesigner = attributeHPartitioningDesigner;
        }
        public IChainableCommand HandleExceptionCommand(IExecutableCommand onExceptionCommand = null, IExecutableCommand finallyCommand = null)
        {
            return new HandleExceptionCommand(ex => onExceptionCommand?.Execute(), () => finallyCommand?.Execute());
        }
        public IChainableCommand EvaluateIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            return new EvaluateIndicesEnvironmentsCommand(context, dbmsRepositories.GetVirtualIndicesRepository(), dbmsRepositories.GetExplainRepository(),
                                                      dbObjectDefinitionGenerator);
        }

        public IChainableCommand GenerateBaseBtreeIndicesCommand(WorkloadAnalysisContext context)
        {
            return new GenerateBaseBtreeIndicesCommand(context);
        }

        public IChainableCommand GetRealExecutionPlansCommand(WorkloadAnalysisContext context)
        {
            return new GetExecutionPlansCommand(context, () => context.RealExecutionPlansForStatements, dbmsRepositories.GetExplainRepository());
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

        public IChainableCommand LoadWorkloadStatementsDataCommand(WorkloadAnalysisContext context)
        {
            return new LoadWorkloadStatementsDataCommand(context, dalRepositories.GetNormalizedWorkloadStatementsRepository());
        }

        public IChainableCommand LoadExistingIndicesCommand(WorkloadAnalysisContext context)
        {
            return new LoadExistingIndicesCommand(context, dbmsRepositories.GetIndicesRepository(), dbmsRepositories.GetRelationsRepository(),
                                                  dbmsRepositories.GetRelationAttributesRepository());
        }

        public IChainableCommand GenerateCoveringIndicesCommand(WorkloadAnalysisContext context)
        {
            return new GenerateCoveringIndicesCommand(context);
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

        public IChainableCommand GenerateAndEvaluateFilteredIndicesCommand(WorkloadAnalysisContext context)
        {
            return new GenerateAndEvaluateFilteredIndicesCommand(context, toSqlValueStringConverter, dbmsRepositories.GetVirtualIndicesRepository(), dbObjectDefinitionGenerator);
        }

        public IChainableCommand UpdateAnalysisStateCommand(WorkloadAnalysisContext context, WorkloadAnalysisStateType state)
        {
            return new UpdateAnalysisStateCommand(context, state, dalRepositories.GetWorkloadAnalysesRepository());
        }

        public IChainableCommand PersistsIndicesDesignDataCommand(WorkloadAnalysisContext context)
        {
            return new PersistsIndicesDesignDataCommand(context, dalRepositories, dbObjectDefinitionGenerator);
        }

        public IChainableCommand LoadWorkloadRelationsDataCommand(WorkloadAnalysisContext context)
        {
            return new LoadWorkloadRelationsDataCommand(context, dbmsRepositories.GetRelationsRepository(), dbmsRepositories.GetRelationAttributesRepository());
        }

        public IChainableCommand InitializeHPartitioningEnvironmentCommand(WorkloadAnalysisContext context)
        {
            return new InitializeHPartitioningEnvironmentCommand(context, dbmsRepositories.GetVirtualHPartitioningsRepository());
        }

        public IChainableCommand PrepareHPartitioningAttributeDefinitionsCommand(WorkloadAnalysisContext context)
        {
            return new PrepareHPartitioningAttributeDefinitionsCommand(context, attributeHPartitioningDesigner);
        }

        public IChainableCommand GenerateBaseHPartitioningEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            return new GenerateBaseHPartitioningEnvironmentsCommand(context);
        }

        public IChainableCommand EvaluateHPartitioningEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            return new EvaluateHPartitioningEnvironmentsCommand(context, dbmsRepositories.GetVirtualHPartitioningsRepository(), dbmsRepositories.GetExplainRepository(), dbObjectDefinitionGenerator);
        }

        public IChainableCommand CleanUpNotImprovingHPartitioningAndTheirEnvsCommand(WorkloadAnalysisContext context)
        {
            return new CleanUpNotImprovingHPartitioningAndTheirEnvsCommand(context);
        }

        public IChainableCommand PersistsHPartitioningsDesignDataCommand(WorkloadAnalysisContext context)
        {
            return new PersistsHPartitioningsDesignDataCommand(context, dalRepositories, dbObjectDefinitionGenerator);
        }

        public IChainableCommand PersistsRealExecutionPlansCommand(WorkloadAnalysisContext context)
        {
            return new PersistsRealExecutionPlansCommand(context, dalRepositories);
        }
    }
}
