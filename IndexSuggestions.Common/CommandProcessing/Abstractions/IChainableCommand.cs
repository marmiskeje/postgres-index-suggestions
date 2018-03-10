using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public interface IChainableCommand : IExecutableCommand, ICommandSuccessorInfo
    {
        void SetSuccessor(IExecutableCommand successor);
    }
}
