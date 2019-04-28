using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.TaskScheduling
{
    public interface IRegularTaskScheduler : IDisposable
    {
        void Start();
        void Stop();
    }
}
