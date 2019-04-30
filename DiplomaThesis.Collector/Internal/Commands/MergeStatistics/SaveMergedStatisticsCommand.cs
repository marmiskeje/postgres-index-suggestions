using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace DiplomaThesis.Collector
{
    internal class SaveMergedStatisticsCommand<TKey, TData> : ChainableCommand where TData : IEntity<long>
    {
        private readonly MergeStatisticsContext<TKey, TData> context;
        private readonly Action<TData> removeEntityAction;
        private readonly Action<TData> createEntityAction;
        public SaveMergedStatisticsCommand(MergeStatisticsContext<TKey, TData> context, Action<TData> removeEntityAction, Action<TData> createEntityAction)
        {
            this.context = context;
            this.removeEntityAction = removeEntityAction;
            this.createEntityAction = createEntityAction;
        }
        protected override void OnExecute()
        {
            foreach (var s in context.MergedStatistics)
            {
                var newSamples = s.Value;
                var oldSamples = context.LoadedStatistics[s.Key];
                if (true || newSamples.Count < oldSamples.Count) // only if merge decreased count of samples
                {
                    using (var scope = new TransactionScope())
                    {
                        oldSamples.ForEach(x => removeEntityAction(x));
                        foreach (var n in newSamples)
                        {
                            n.ID = 0;
                            createEntityAction(n);
                        }
                        scope.Complete();
                    }
                }
            }
        }
    }
}

