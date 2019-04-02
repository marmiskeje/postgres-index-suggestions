using IndexSuggestions.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace IndexSuggestions.Collector
{
    internal class StatisticsCollectorService : IStatisticsCollectorService
    {
#if DEBUG
        private const int PERSISTENCE_THRESHOLD_ELAPSED_SECONDS = 2;
        private const int TIMER_INTERVAL = 2000;
#else
        private const int PERSISTENCE_THRESHOLD_ELAPSED_SECONDS = 60;
        private const int TIMER_INTERVAL = 30000;
#endif
        private readonly Timer timer = null;
        private readonly ICommandProcessingQueue<IExecutableCommand> queue = null;
        private readonly IStatisticsProcessingChainFactory chainFactory = null;
        private bool isDisposed = false;
        private DateTime lastStatePersistenceDate = DateTime.Now;

        public StatisticsCollectorService(ICommandProcessingQueue<IExecutableCommand> queue, IStatisticsProcessingChainFactory chainFactory)
        {
            this.queue = queue;
            this.chainFactory = chainFactory;
            timer = new Timer(TIMER_INTERVAL);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var context = new TotalStatisticsCollectNextSampleContext();
            queue.Enqueue(chainFactory.TotalStatisticsCollectNextSampleChain(context));
            DateTime now = DateTime.Now;
            if ((now - lastStatePersistenceDate).TotalSeconds >= PERSISTENCE_THRESHOLD_ELAPSED_SECONDS)
            {
                InitiatePersistence();
                lastStatePersistenceDate = now;
            }
        }

        private void InitiatePersistence()
        {
            queue.Enqueue(chainFactory.TotalStatisticsPersistenceChain());
        }

        public void Start()
        {
            if (!timer.Enabled)
            {
                timer.Start();
            }
        }

        public void Stop()
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
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
        ~StatisticsCollectorService()
        {
            Dispose(false);
        }
    }
}
