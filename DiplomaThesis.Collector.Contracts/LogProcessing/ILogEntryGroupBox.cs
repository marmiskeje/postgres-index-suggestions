using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public interface ILogEntryGroupBox : IDisposable
    {
        void Publish(IEnumerable<LoggedEntry> entries);
        IEnumerable<LoggedEntry> Provide();
    }
}
