using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal interface IAnalysisRequestsLoader : IDisposable
    {
        void Start();
        void Stop();
    }
}
