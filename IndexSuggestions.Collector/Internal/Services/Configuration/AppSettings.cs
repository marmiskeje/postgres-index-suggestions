using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class AppSettings
    {
        public CollectorSettings CollectorSettings { get; set; }
    }
    internal class CollectorSettings
    {
        public LogProcessingSettings LogProcessing { get; set; }
        public ExternalSqlNormalizationSettings ExternalSqlNormalization { get; set; }
    }

    internal class LogProcessingSettings
    {
        public string Directory { get; set; }
        public string FilenameFilter { get; set; }
        public string Encoding { get; set; }
        public string LogEntryStartingCharacter { get; set; }
        public string LogEntryColumnSeparator { get; set; }
    }

    internal class ExternalSqlNormalizationSettings
    {
        public bool IsEnabled { get; set; }
        public string ProcessFilename { get; set; }
        public string ProcessArguments { get; set; }
    }
}
