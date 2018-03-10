using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public interface IExecutableCommand
    {
        void Execute();
    }
}
