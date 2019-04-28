using System;
using System.Collections.Generic;
using System.Text;
using IndexSuggestions.Common.CommandProcessing;

namespace IndexSuggestions.ReportingService
{
    internal class CommandChainFactory : ICommandChainFactory
    {
        private readonly ICommandFactory commands;
        public CommandChainFactory(ICommandFactory commands)
        {
            this.commands = commands;
        }
        public IExecutableCommand SummaryReportChain(ReportContextWithModel<SummaryEmailModel> context)
        {
            CommandChainCreator chain = new CommandChainCreator();
            chain.Add(commands.LoadDataAndCreateEmailModelCommand(context));
            chain.Add(commands.GenerateEmailCommand(context));
            chain.Add(commands.SendEmailCommand(context));
            return chain.FirstCommand;
        }
    }
}
