using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface ILastProcessedLogEntryEvidence
    {
        DateTime Provide();
        void Publish(DateTime date);
        void PersistCurrentState();
    }
}
