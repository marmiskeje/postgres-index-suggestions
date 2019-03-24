using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class FileProcessor : IFileProcessor
    {
        protected ILog Log { get; private set; }
        protected Encoding Encoding { get; private set; }
        private ILogProcessor LogProcessor { get; set; }
        private ILogEntryGroupBox GroupBox { get; set; }
        
        public FileProcessor(ILog log, ILogProcessingConfiguration configuration, ILogProcessor logProcessor, ILogEntryGroupBox groupBox)
        {
            Log = log;
            LogProcessor = logProcessor;
            GroupBox = groupBox;
            Encoding = CodePagesEncodingProvider.Instance.GetEncoding(configuration.Encoding) ?? Encoding.UTF8;
        }

        protected void ProcessLine(string line, bool eof)
        {
            if (line != null)
            {
                try
                {
                    var entries = LogProcessor.ProcessLine(line, eof);
                    if (entries.Count > 0)
                    {
                        GroupBox.Publish(entries); 
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }

        public void ProcessFile(FileInfo file)
        {
            using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fileStream, Encoding))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        ProcessLine(line, reader.EndOfStream);
                    }
                }
            }
        }
    }
}
