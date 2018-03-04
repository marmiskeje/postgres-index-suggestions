using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface ILogProcessor
    {
        IEnumerable<LoggedEntry> ProcessLine(string line, bool eof);
    }
}
