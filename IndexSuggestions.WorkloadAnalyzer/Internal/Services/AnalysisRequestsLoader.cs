using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.Common.Logging;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class AnalysisRequestsLoader : IAnalysisRequestsLoader
    {
        private const int MAX_LOADED_REQUESTS = 5;
        private bool isDisposed = false;
        private readonly ILog log;
        private readonly ICommandProcessingQueue<IExecutableCommand> queue;
        private readonly IWorkloadAnalysesRepository workloadAnalysesRepository;
        private readonly ICommandChainFactory chainFactory;
        private readonly Timer timer = null;
        public AnalysisRequestsLoader(ILog log, ICommandProcessingQueue<IExecutableCommand> queue,
                                      IWorkloadAnalysesRepository workloadAnalysesRepository, ICommandChainFactory chainFactory)
        {
            this.log = log;
            this.queue = queue;
            this.workloadAnalysesRepository = workloadAnalysesRepository;
            this.chainFactory = chainFactory;
            this.timer = new Timer(60000);
            this.timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var analysesForProcessing = workloadAnalysesRepository.LoadForProcessing(MAX_LOADED_REQUESTS);
                foreach (var a in analysesForProcessing)
                {
                    var context = new WorkloadAnalysisContext() { WorkloadAnalysis = a };
                    queue.Enqueue(chainFactory.WorkloadAnalysisChain(context));
                }
            }
            catch (Exception ex)
            {
                log.Write(ex);
            }
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
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    Stop();
                    timer.Dispose();
                }
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AnalysisRequestsLoader()
        {
            Dispose(false);
        }
    }
}
