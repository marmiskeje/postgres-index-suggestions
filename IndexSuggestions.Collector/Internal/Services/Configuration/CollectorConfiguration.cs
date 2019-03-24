using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class CollectorConfiguration : ICollectorConfiguration
    {
        public ILogProcessingConfiguration LogProcessing { get; private set; }

        public IExternalSqlNormalizationConfiguration ExternalSqlNormalization { get; private set; }

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

        private class ExternalSqlNormalizationConfiguration : IExternalSqlNormalizationConfiguration
        {
            public bool IsEnabled { get; private set; }

            public string ProcessFilename { get; private set; }

            public string ProcessArguments { get; private set; }
            public ExternalSqlNormalizationConfiguration(ExternalSqlNormalizationSettings settings)
            {
                IsEnabled = settings.IsEnabled;
                ProcessFilename = settings.ProcessFilename;
                ProcessArguments = settings.ProcessArguments;
            }
        }

        public CollectorConfiguration(AppSettings appSettings)
        {
            LogProcessing = new LogProcessingConfiguration(appSettings.CollectorSettings.LogProcessing);
            ExternalSqlNormalization = new ExternalSqlNormalizationConfiguration(appSettings.CollectorSettings.ExternalSqlNormalization);
        }
    }
}
