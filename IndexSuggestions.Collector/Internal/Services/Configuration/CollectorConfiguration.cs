using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class CollectorConfiguration : ICollectorConfiguration
    {
        public ILogProcessingConfiguration LogProcessing { get; private set; }

        private class LogProcessingConfiguration : ILogProcessingConfiguration
        {
            public string Directory { get; private set; }
            public string FilenameFilter { get; private set; }
            public string Encoding { get; private set; }
            public string LogEntryStartingCharacter { get; private set; }

            public string LogEntryColumnSeparator { get; private set; }

            public LogProcessingConfiguration(LogProcessingSettings settings)
            {
                Directory = settings.Directory;
                FilenameFilter = settings.FilenameFilter;
                Encoding = settings.Encoding;
                LogEntryColumnSeparator = settings.LogEntryColumnSeparator;
                LogEntryStartingCharacter = settings.LogEntryStartingCharacter;
            }
        }

        public CollectorConfiguration(AppSettings appSettings)
        {
            LogProcessing = new LogProcessingConfiguration(appSettings.CollectorSettings.LogProcessing);
        }
    }
}
