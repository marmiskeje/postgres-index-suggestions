using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
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
            chain.Add(commands.LoadWorkloadStatementsDataCommand(context));
            chain.Add(commands.LoadWorkloadRelationsDataCommand(context));
            chain.Add(commands.ExtractStatementsQueryDataCommand(context));

            // horizontal partitioning design
            chain.Add(commands.InitializeHPartitioningEnvironmentCommand(context));
            chain.Add(commands.PrepareHPartitioningAttributeDefinitionsCommand(context));
            chain.Add(commands.GenerateBaseHPartitioningEnvironmentsCommand(context));
            chain.Add(commands.EvaluateHPartitioningEnvironmentsCommand(context));
            chain.Add(commands.CleanUpNotImprovingHPartitioningAndTheirEnvsCommand(context));
            chain.Add(commands.PersistsHPartitioningsDesignDataCommand(context));

            // indices design
            chain.Add(commands.InitializeVirtualIndicesEnvironmentCommand(context));
            chain.Add(commands.LoadExistingIndicesCommand(context));
            chain.Add(commands.GenerateBaseBtreeIndicesCommand(context));
            chain.Add(commands.ExcludeExistingIndicesCommand(context));
            chain.Add(commands.GetRealExecutionPlansCommand(context));
            chain.Add(commands.GenerateBaseIndicesEnvironmentsCommand(context));
            chain.Add(commands.EvaluateIndicesEnvironmentsCommand(context));
            chain.Add(commands.CleanUpNotImprovingIndiciesAndTheirEnvsCommand(context));
            chain.Add(commands.GenerateCoveringIndicesCommand(context));
            chain.Add(commands.GenerateFinalIndicesEnvironmentsCommand(context));
            chain.Add(commands.EvaluateIndicesEnvironmentsCommand(context));
            chain.Add(commands.CleanUpNotImprovingIndiciesAndTheirEnvsCommand(context));
            chain.Add(commands.GenerateAndEvaluateFilteredIndicesCommand(context));
            chain.Add(commands.PersistsIndicesDesignDataCommand(context));

            chain.Add(commands.PersistsRealExecutionPlansCommand(context));
            chain.Add(commands.UpdateAnalysisStateCommand(context, WorkloadAnalysisStateType.EndedSuccesfully));
            return chain.FirstCommand;
        }

    }
}
