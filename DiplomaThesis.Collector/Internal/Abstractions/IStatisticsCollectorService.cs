using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IStatisticsCollectorService : IDisposable
    {
        void Start();
        void Stop();
    }
}
