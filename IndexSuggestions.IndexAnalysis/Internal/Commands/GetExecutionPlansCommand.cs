using System;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.IndexAnalysis
{
    internal class GetExecutionPlansCommand : ChainableCommand
    {
        private readonly DesignIndicesContext context;
        private readonly Func<Dictionary<long, IExplainResult>> getDictionaryFunc;
        private readonly IExplainRepository explainRepository;

        public GetExecutionPlansCommand(DesignIndicesContext context, Func<Dictionary<long, IExplainResult>> getDictionaryFunc, IExplainRepository explainRepository)
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
                foreach (var s in context.Statements)
                {
                    var explainResult = explainRepository.Eplain(s.Value.RepresentativeStatistics.RepresentativeStatement);
                    dictionary.Add(s.Key, explainResult);
                }
            }
        }
    }
}