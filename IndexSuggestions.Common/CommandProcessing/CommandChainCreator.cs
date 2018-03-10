using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public class CommandChainCreator
    {
        private IChainableCommand lastCommand;
        public IChainableCommand FirstCommand { get; private set; }

        public void Add(IChainableCommand command)
        {
            if (FirstCommand == null)
            {
                FirstCommand = command;
                lastCommand = command;
            }
            else
            {
                lastCommand.SetSuccessor(command);
                lastCommand = command;
            }
        }
    }
}
