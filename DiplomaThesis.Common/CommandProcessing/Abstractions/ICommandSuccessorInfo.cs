using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.CommandProcessing
{
    public interface ICommandSuccessorInfo
    {
        IExecutableCommand Successor { get; }
    }
}
