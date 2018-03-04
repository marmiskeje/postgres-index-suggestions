using IndexSuggestions.Collector.Contracts;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class LogProcessingService : ILogProcessingService
    {
        private readonly ILogProcessingConfiguration configuration;
        private readonly ILogProcessor logProcessor;
        private readonly IContinuousFileProcessor continuousFileProcessor;
        private FileSystemWatcher fileSystemWatcher;
        public LogProcessingService(ILogProcessingConfiguration configuration, ILogProcessor logProcessor, IContinuousFileProcessor continuousFileProcessor)
        {
            this.configuration = configuration;
            this.logProcessor = logProcessor;
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
            FileInfo lastFile = null;
            foreach (var file in files)
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var encoding = CodePagesEncodingProvider.Instance.GetEncoding(configuration.Encoding);
                    using (var reader = new StreamReader(fileStream, encoding))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line != null)
                            {
                                logProcessor.ProcessLine(line, reader.EndOfStream);
                            }
                        }
                    }
                }
                lastFile = file;
            }
            continuousFileProcessor.ChangeCurrentFile(lastFile.FullName);
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
