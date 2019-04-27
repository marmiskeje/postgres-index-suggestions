using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal interface ICommandFactory
    {
        IChainableCommand HandleExceptionCommand(IExecutableCommand onExceptionCommand = null, IExecutableCommand finallyCommand = null);
        IChainableCommand LoadWorkloadCommand(WorkloadAnalysisContext context);
        IChainableCommand LoadWorkloadStatementsDataCommand(WorkloadAnalysisContext context);
        IChainableCommand GenerateBaseBtreeIndicesCommand(WorkloadAnalysisContext context);
        IChainableCommand LoadExistingIndicesCommand(WorkloadAnalysisContext context);
        IChainableCommand LoadDatabaseInfoCommand(WorkloadAnalysisContext context);
        IChainableCommand UpdateAnalysisStateCommand(WorkloadAnalysisContext context, WorkloadAnalysisStateType state);
        IChainableCommand GetRealExecutionPlansCommand(WorkloadAnalysisContext context);
        IChainableCommand EvaluateIndicesEnvironmentsCommand(WorkloadAnalysisContext context);
        IChainableCommand LoadWorkloadRelationsDataCommand(WorkloadAnalysisContext context);
        IChainableCommand InitializeVirtualIndicesEnvironmentCommand(WorkloadAnalysisContext context);
        IChainableCommand InitializeHPartitioningEnvironmentCommand(WorkloadAnalysisContext context);
        IChainableCommand PrepareHPartitioningAttributeDefinitionsCommand(WorkloadAnalysisContext context);
        IChainableCommand GenerateBaseHPartitioningEnvironmentsCommand(WorkloadAnalysisContext context);
        IChainableCommand CleanUpNotImprovingHPartitioningAndTheirEnvsCommand(WorkloadAnalysisContext context);
        IChainableCommand PersistsHPartitioningsDesignDataCommand(WorkloadAnalysisContext context);
        IChainableCommand EvaluateHPartitioningEnvironmentsCommand(WorkloadAnalysisContext context);
        IChainableCommand ExtractStatementsQueryDataCommand(WorkloadAnalysisContext context);
        IChainableCommand GenerateCoveringIndicesCommand(WorkloadAnalysisContext context);
        IChainableCommand ExcludeExistingIndicesCommand(WorkloadAnalysisContext context);
        IChainableCommand GenerateBaseIndicesEnvironmentsCommand(WorkloadAnalysisContext context);
        IChainableCommand CleanUpNotImprovingIndiciesAndTheirEnvsCommand(WorkloadAnalysisContext context);
        IChainableCommand GenerateFinalIndicesEnvironmentsCommand(WorkloadAnalysisContext context);
        IChainableCommand GenerateAndEvaluateFilteredIndicesCommand(WorkloadAnalysisContext context);
        IChainableCommand PersistsIndicesDesignDataCommand(WorkloadAnalysisContext context);
        IChainableCommand PersistsRealExecutionPlansCommand(WorkloadAnalysisContext context);
    }
}
