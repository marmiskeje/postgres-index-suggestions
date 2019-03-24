using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface IExternalSqlNormalizationConfiguration
    {
        bool IsEnabled { get; }
        string ProcessFilename { get; }
        string ProcessArguments { get; }
    }
}
