using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.CommandProcessing
{
    public interface IChainableCommand : IExecutableCommand, ICommandSuccessorInfo
    {
        void SetSuccessor(IExecutableCommand successor);
    }
}
