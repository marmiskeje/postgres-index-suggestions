using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IRegularTaskScheduler : IDisposable
    {
        void Start();
        void Stop();
    }
}
