using DiplomaThesis.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DiplomaThesis.Common.CommandProcessing
{
    public class CommandProcessingQueue<T> : ICommandProcessingQueue<T> where T : IExecutableCommand
    {
        protected bool isDisposed = false;
        private readonly object lockObject = new object();
        private ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private ManualResetEvent enqueueEvent = new ManualResetEvent(false);
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private Thread worker = null;

        protected ILog Log { get; private set; }

        public int Count
        {
            get { return queue.Count; }
        }

        public CommandProcessingQueue(ILog log, string queueName)
        {
            Log = log;
            worker = new Thread(new ThreadStart(Worker_Work));
            worker.Name = String.Format("{0} worker thread", queueName ?? "CommandProcessingQueue");
            worker.Start();
        }

        public void Enqueue(T command)
        {
            try
            {
                if (!isDisposed)
                {
                    queue.Enqueue(command);
                    enqueueEvent.Set();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void Worker_Work()
        {
            while (!isDisposed && !shutdownEvent.WaitOne(0, false))
            {
                enqueueEvent.WaitOne(Timeout.Infinite, false);

                ProcessQueueData();
            }
            ProcessQueueData();
        }

        private void ProcessQueueData()
        {
            while (!isDisposed && true)
            {
                T command = default(T);
                if (!queue.TryDequeue(out command))
                {
                    enqueueEvent.Reset();
                    return;
                }
                TryExecuteCommand(command);
            }
        }

        protected virtual void TryExecuteCommand(T command)
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
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
                shutdownEvent.Set();
                enqueueEvent.Set();
                if (worker != null)
                {
                    if (!worker.Join(5000))
                    {
                        worker.Abort();
                        worker.Join(100);
                    }
                }
                shutdownEvent.Close();
                enqueueEvent.Close();
                DisposeProtected(isDisposing);
            }
            isDisposed = true;
        }

        protected virtual void DisposeProtected(bool isDisposing)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CommandProcessingQueue()
        {
            Dispose(false);
        }
    }
}
