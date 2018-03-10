using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.CommandProcessing
{
    public interface ICommandProcessingQueue<T> : IDisposable where T : IExecutableCommand
    {
        int Count { get; }
        void Enqueue(T command);
    }
}
