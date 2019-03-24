using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface ILogProcessor
    {
        IReadOnlyList<LoggedEntry> ProcessLine(string line, bool eof);
    }
}
