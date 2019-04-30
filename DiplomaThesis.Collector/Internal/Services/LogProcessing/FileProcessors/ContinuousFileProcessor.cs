using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DiplomaThesis.Collector
{
    class ContinuousFileProcessor : FileProcessor, IContinuousFileProcessor
    {
        private readonly object lockObject = new object();
        private volatile string currentFile = null;
        private volatile bool isDisposed = false;
        private Thread worker;
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private ManualResetEvent changedFileEvent = new ManualResetEvent(false);
        private ManualResetEvent changedFileOldProcessedEvent = new ManualResetEvent(false);

        public ContinuousFileProcessor(ILog log, ILogProcessingConfiguration configuration, ILogProcessor logProcessor, ILogEntryGroupBox groupBox)
            : base(log, configuration, logProcessor, groupBox)
        {
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
                            Log.Write(SeverityType.Info, "Processed log file changed to: {0}", file ?? "NONE");
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
                        using (var reader = new StreamReader(fileStream, Encoding))
                        {
                            while (true)
                            {
                                string line = reader.ReadLine();
                                if (line != null)
                                {
                                    ProcessLine(line, reader.EndOfStream);
                                }
                                else
                                {
                                    if (isDisposed || file != this.currentFile)
                                    {
                                        changedFileOldProcessedEvent.Set();
                                        break;
                                    }
                                    changedFileEvent.WaitOne(10000);
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
