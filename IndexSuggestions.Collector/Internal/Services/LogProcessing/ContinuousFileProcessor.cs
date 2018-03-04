using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace IndexSuggestions.Collector
{
    class ContinuousFileProcessor : IContinuousFileProcessor
    {
        private readonly object lockObject = new object();
        private readonly ILog log;
        private readonly ILogProcessor logProcessor;
        private readonly ILogProcessingConfiguration configuration;
        private volatile string currentFile = null;
        private volatile bool isDisposed = false;
        private Thread worker;
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private ManualResetEvent changedFileEvent = new ManualResetEvent(false);
        private ManualResetEvent changedFileOldProcessedEvent = new ManualResetEvent(false);

        public ContinuousFileProcessor(ILog log, ILogProcessingConfiguration configuration, ILogProcessor logProcessor)
        {
            this.log = log;
            this.configuration = configuration;
            this.logProcessor = logProcessor;
            worker = new Thread(Thread_Job);
            worker.Start();
        }
        public void ChangeCurrentFile(string file)
        {
            if (!isDisposed)
            {
                if (currentFile != file)
                {
                    lock (lockObject)
                    {
                        if (!isDisposed)
                        {
                            this.currentFile = file;
                            changedFileEvent.Set();
                            changedFileOldProcessedEvent.WaitOne(5000);
                            changedFileOldProcessedEvent.Reset();
                            log.Write(SeverityType.Info, "Processed log file changed to: {0}", file ?? "NONE");
                        }
                    }
                }
            }
        }

        private void Thread_Job(object obj)
        {
            while (!isDisposed && !shutdownEvent.WaitOne(0, false))
            {
                var file = this.currentFile;
                if (file != null)
                {
                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        var encoding = CodePagesEncodingProvider.Instance.GetEncoding(configuration.Encoding);
                        using (var reader = new StreamReader(fileStream, encoding))
                        {
                            while (true)
                            {
                                string line = reader.ReadLine();
                                if (line != null)
                                {
                                    try
                                    {
                                        logProcessor.ProcessLine(line, reader.EndOfStream);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Write(ex);
                                    }
                                }
                                else
                                {
                                    if (isDisposed || file != this.currentFile)
                                    {
                                        changedFileOldProcessedEvent.Set();
                                        break;
                                    }
                                    changedFileEvent.WaitOne(60000);
                                    changedFileEvent.Reset();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (isDisposed)
                    {
                        break;
                    }
                    changedFileEvent.WaitOne(60000);
                }
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    // Managed resources
                }
                ChangeCurrentFile(null);
                isDisposed = true;
                shutdownEvent.Set();
                changedFileOldProcessedEvent.Set();
                changedFileEvent.Set();
                if (worker != null)
                {
                    if (!worker.Join(5000))
                    {
                        worker.Abort();
                        worker.Join(100);
                    }
                }
                shutdownEvent.Close();
                changedFileOldProcessedEvent.Close();
                changedFileEvent.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ContinuousFileProcessor()
        {
            Dispose(false);
        }
    }
}
