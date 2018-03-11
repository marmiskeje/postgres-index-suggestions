using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface ILastProcessedLogEntryEvidence : IDisposable
    {
        DateTime Provide();
        void Publish(DateTime date);
    }
}
