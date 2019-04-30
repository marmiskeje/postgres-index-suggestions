using System;
using System.Collections.Generic;
using System.Text;
using DiplomaThesis.Common.CommandProcessing;

namespace DiplomaThesis.Collector
{
    internal class StatisticsProcessingChainFactory : IStatisticsProcessingChainFactory
    {
        private readonly IStatisticsProcessingCommandFactory commands;
        public StatisticsProcessingChainFactory(IStatisticsProcessingCommandFactory commands)
        {
            this.commands = commands;
        }
        public IExecutableCommand TotalStatisticsCollectNextSampleChain(TotalStatisticsCollectNextSampleContext context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            // TODO: load only filtered databases
            chain.Add(commands.LoadDatabasesForTotalStatisticsCommand(context));
            chain.Add(commands.CollectTotalDatabaseStatisticsCommand(context));
            chain.Add(commands.PublishTotalDatabaseStatisticsCommand(context));
            return chain.FirstCommand;
        }

        public IExecutableCommand TotalStatisticsPersistenceChain()
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.PersistDataAccumulatorsCommand());
            return chain.FirstCommand;
        }
    }
}
