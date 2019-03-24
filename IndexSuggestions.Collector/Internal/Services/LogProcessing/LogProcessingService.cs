using IndexSuggestions.Collector.Contracts;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class LogProcessingService : ILogProcessingService
    {
        private readonly ILogProcessingConfiguration configuration;
        private readonly IFileProcessor oneFileProcessor;
        private readonly IContinuousFileProcessor continuousFileProcessor;
        private FileSystemWatcher fileSystemWatcher;
        public LogProcessingService(ILogProcessingConfiguration configuration, IFileProcessor oneFileProcessor, IContinuousFileProcessor continuousFileProcessor)
        {
            this.configuration = configuration;
            this.oneFileProcessor = oneFileProcessor;
            this.continuousFileProcessor = continuousFileProcessor;
        }

        public void Start()
        {
            Stop();
            var files = new List<FileInfo>();
            foreach (var f in Directory.GetFiles(configuration.Directory, configuration.FilenameFilter, SearchOption.TopDirectoryOnly))
            {
                files.Add(new FileInfo(f));
            }
            files.Sort((x, y) => x.CreationTime.CompareTo(y.CreationTime));

            for (int i = 0; i < files.Count - 1; i++)
            {
                oneFileProcessor.ProcessFile(files[i]);
            }
            if (files.Count > 0)
            {
                FileInfo lastFile = files[files.Count - 1];
                continuousFileProcessor.ChangeCurrentFile(lastFile.FullName); 
            }
            // enable
            fileSystemWatcher = new FileSystemWatcher(configuration.Directory, configuration.FilenameFilter);
            fileSystemWatcher.IncludeSubdirectories = false;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            fileSystemWatcher.Created += (x, y) => continuousFileProcessor.ChangeCurrentFile(y.FullPath);
            fileSystemWatcher.Renamed += (x, y) =>
            {
                if (y.FullPath.EndsWith(configuration.FilenameFilter.Replace("*", "")))
                {
                    continuousFileProcessor.ChangeCurrentFile(y.FullPath);
                }
            };
        }

        public void Stop()
        {
            if (continuousFileProcessor != null)
            {
                continuousFileProcessor.ChangeCurrentFile(null); 
            }
            if (fileSystemWatcher != null)
            {
                fileSystemWatcher.Dispose(); 
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
