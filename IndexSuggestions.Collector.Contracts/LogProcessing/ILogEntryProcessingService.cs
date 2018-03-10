using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface ILogEntryProcessingService : IDisposable
    {
        void Start();
        void Stop();
    }
}
