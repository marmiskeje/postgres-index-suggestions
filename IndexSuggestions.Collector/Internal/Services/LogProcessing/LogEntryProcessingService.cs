using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace IndexSuggestions.Collector
{
    class LogEntryProcessingService : ILogEntryProcessingService
    {
        private const int PERSISTENCE_THRESHOLD_ENTRIES_COUNT = 1000;
        private const int PERSISTENCE_THRESHOLD_ELAPSED_SECONDS = 60;
        private readonly ILogEntryGroupBox groupBox;
        private readonly ILogEntryProcessingChainFactory chainFactory;
        private readonly ICommandProcessingQueue<IExecutableCommand> queue;
        private readonly ILastProcessedLogEntryEvidence lastProcessedEvidence;
        private readonly Timer timer;
        private bool isDisposed = false;
        private DateTime lastStatePersistenceDate = DateTime.Now;
        private int entriesCounter = 0;
        public LogEntryProcessingService(ILogEntryGroupBox groupBox, ICommandProcessingQueue<IExecutableCommand> queue, ILogEntryProcessingChainFactory chainFactory,
                                         ILastProcessedLogEntryEvidence lastProcessedEvidence)
        {
            this.groupBox = groupBox;
            this.queue = queue;
            this.chainFactory = chainFactory;
            this.lastProcessedEvidence = lastProcessedEvidence;
            this.timer = new Timer(2000);
            this.timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var entries = groupBox.Provide();
            var lastTimestamp = lastProcessedEvidence.Provide();
            foreach (var entry in entries)
            {
                if (entry.Timestamp >= lastTimestamp)
                {
                    LogEntryProcessingContext context = new LogEntryProcessingContext();
                    context.Entry = entry;
                    queue.Enqueue(chainFactory.LogEntryProcessingChain(context));
                    lastProcessedEvidence.Publish(entry.Timestamp);
                    entriesCounter++;
                }
                else
                {
                    // already processed
                }
            }
            DateTime now = DateTime.Now;
            if (entriesCounter >= PERSISTENCE_THRESHOLD_ENTRIES_COUNT || (now - lastStatePersistenceDate).TotalSeconds >= PERSISTENCE_THRESHOLD_ELAPSED_SECONDS)
            {
                InitiatePersistence();
                entriesCounter = 0;
                lastStatePersistenceDate = now;
            }
        }

        private void InitiatePersistence()
        {
            queue.Enqueue(chainFactory.LogEntryPersistenceChain());
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
            InitiatePersistence();
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    Stop();
                    this.timer.Dispose();
                }
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~LogEntryProcessingService()
        {
            Dispose(false);
        }
    }
}
