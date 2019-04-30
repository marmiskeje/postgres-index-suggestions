using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Common.CommandProcessing;

namespace DiplomaThesis.Collector
{
    internal class MergeStatisticsChainFactory : IMergeStatisticsChainFactory
    {
        private readonly IMergeStatisticsCommandFactory commands;
        public MergeStatisticsChainFactory(IMergeStatisticsCommandFactory commands)
        {
            this.commands = commands;
        }
        public IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementRelationStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand MergeStatisticsChain(MergeNormalizedStatementIndexStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand MergeStatisticsChain(MergeTotalRelationStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand MergeStatisticsChain(MergeTotalIndexStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand MergeStatisticsChain(MergeTotalStoredProcedureStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand MergeStatisticsChain(MergeTotalViewStatisticsContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadStatisticsForMergeCommand(context));
            chain.Add(commands.MergeStatisticsCommand(context));
            chain.Add(commands.SaveMergedStatisticsCommand(context));
            return chain.FirstCommand;
        }
    }
}
