using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.IndexAnalysis
{
    internal interface ICommandFactory
    {
        IChainableCommand LoadWorkloadCommand(DesignIndicesContext context);
        IChainableCommand LoadWorkloadStatementsCommand(DesignIndicesContext context);
        IChainableCommand GenerateBaseBtreeIndicesCommand(DesignIndicesContext context);
        IChainableCommand LoadExistingIndicesCommand(DesignIndicesContext context);
        IChainableCommand LoadDatabaseInfoCommand(DesignIndicesContext context);
        IChainableCommand GetRealExecutionPlansCommand(DesignIndicesContext context);
        IChainableCommand EvaluateIndicesEnvironmentsCommand(DesignIndicesContext context);
        IChainableCommand InitializeVirtualIndicesEnvironmentCommand(DesignIndicesContext context);
        IChainableCommand ExtractStatementsQueryDataCommand(DesignIndicesContext context);
        IChainableCommand GenerateCoveringBtreeIndicesCommand(DesignIndicesContext context);
        IChainableCommand ExcludeExistingIndicesCommand(DesignIndicesContext context);
        IChainableCommand GenerateBaseIndicesEnvironmentsCommand(DesignIndicesContext context);
        IChainableCommand CleanUpNotImprovingIndiciesAndTheirEnvsCommand(DesignIndicesContext context);
        IChainableCommand GenerateFinalIndicesEnvironmentsCommand(DesignIndicesContext context);
        IChainableCommand GenerateFilteredBtreeIndicesCommand(DesignIndicesContext context);
    }
}
