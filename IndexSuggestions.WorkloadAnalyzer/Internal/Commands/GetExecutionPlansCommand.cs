using System;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class GetExecutionPlansCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly Func<Dictionary<long, IExplainResult>> getDictionaryFunc;
        private readonly IExplainRepository explainRepository;

        public GetExecutionPlansCommand(WorkloadAnalysisContext context, Func<Dictionary<long, IExplainResult>> getDictionaryFunc, IExplainRepository explainRepository)
        {
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
                foreach (var s in context.StatementsData.AllSelects)
                {
                    var explainResult = explainRepository.Eplain(s.Value.RepresentativeStatistics.RepresentativeStatement);
                    dictionary.Add(s.Key, explainResult);
                }
            }
        }
    }
}