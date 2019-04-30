using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.TaskScheduling
{
    public interface IRegularTaskScheduler : IDisposable
    {
        void Start();
        void Stop();
    }
}
