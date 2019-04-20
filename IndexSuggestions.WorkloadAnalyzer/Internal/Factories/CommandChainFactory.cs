using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class CommandChainFactory : ICommandChainFactory
    {
        private readonly ICommandFactory commands;
        public CommandChainFactory(ICommandFactory commands)
        {
            this.commands = commands;
        }
        public IExecutableCommand WorkloadAnalysisChain(WorkloadAnalysisContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.UpdateAnalysisStateCommand(context, WorkloadAnalysisStateType.InProgress));
            chain.Add(commands.HandleExceptionCommand(commands.UpdateAnalysisStateCommand(context, WorkloadAnalysisStateType.EndedWithError)));
            chain.Add(commands.LoadWorkloadCommand(context));
            chain.Add(commands.LoadDatabaseInfoCommand(context));
            chain.Add(commands.LoadWorkloadStatementsCommand(context));
            chain.Add(commands.InitializeVirtualIndicesEnvironmentCommand(context));
            chain.Add(commands.LoadExistingIndicesCommand(context));
            chain.Add(commands.ExtractStatementsQueryDataCommand(context));
            chain.Add(commands.GenerateBaseBtreeIndicesCommand(context));
            chain.Add(commands.ExcludeExistingIndicesCommand(context));
            chain.Add(commands.GetRealExecutionPlansCommand(context));
            chain.Add(commands.GenerateBaseIndicesEnvironmentsCommand(context));
            chain.Add(commands.EvaluateIndicesEnvironmentsCommand(context));
            chain.Add(commands.CleanUpNotImprovingIndiciesAndTheirEnvsCommand(context));
            chain.Add(commands.GenerateCoveringBtreeIndicesCommand(context));
            chain.Add(commands.GenerateFinalIndicesEnvironmentsCommand(context));
            chain.Add(commands.EvaluateIndicesEnvironmentsCommand(context));
            chain.Add(commands.CleanUpNotImprovingIndiciesAndTheirEnvsCommand(context));
            chain.Add(commands.GenerateAndEvaluateFilteredBtreeIndicesCommand(context));
            chain.Add(commands.PersistsIndicesDesignDataCommand(context));
            chain.Add(commands.UpdateAnalysisStateCommand(context, WorkloadAnalysisStateType.EndedSuccesfully));
            return chain.FirstCommand;
        }

    }
}
