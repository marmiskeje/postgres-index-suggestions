using System;
using System.Collections.Generic;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.Common.Logging;
using DiplomaThesis.DBMS.Contracts;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class GetExecutionPlansCommand : ChainableCommand
    {
        private readonly ILog log;
        private readonly WorkloadAnalysisContext context;
        private readonly Func<Dictionary<long, IExplainResult>> getDictionaryFunc;
        private readonly IExplainRepository explainRepository;

        private class EmptyExplainResult : IExplainResult
        {
            public string PlanJson { get; }

            public QueryPlanNode Plan { get; }

            public ISet<string> UsedIndexScanIndices { get; }
            public EmptyExplainResult()
            {
                PlanJson = "";
                Plan = new QueryPlanNode();
            }
        }

        public GetExecutionPlansCommand(ILog log, WorkloadAnalysisContext context, Func<Dictionary<long, IExplainResult>> getDictionaryFunc, IExplainRepository explainRepository)
        {
            this.log = log;
            this.context = context;
            this.getDictionaryFunc = getDictionaryFunc;
            this.explainRepository = explainRepository;
        }

        protected override void OnExecute()
        {
            var dictionary = getDictionaryFunc();
            dictionary.Clear();
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var s in context.StatementsData.All)
                {
                    IExplainResult explainResult = null;
                    try
                    {
                        explainResult = explainRepository.Eplain(s.Value.RepresentativeStatistics.RepresentativeStatement);
                    }
                    catch (Exception ex)
                    {
                        log.Write(ex);
                    }
                    dictionary.Add(s.Key, explainResult ?? new EmptyExplainResult());
                }
            }
        }
    }
}