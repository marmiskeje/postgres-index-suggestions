using DiplomaThesis.Common.CommandProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    /// <summary>
    /// One environment with all possible indices
    /// </summary>
    internal class GenerateBaseIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public GenerateBaseIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
        {
            this.context = context;
        }
        protected override void OnExecute()
        {
            context.IndicesDesignData.Environments.Clear();
            foreach (var index in context.IndicesDesignData.PossibleIndices.All)
            {
                foreach (var kv in context.StatementsData.AllQueriesByRelation[index.Relation.ID])
                {
                    var query = kv.Query;
                    var normalizedStatement = context.StatementsData.All[kv.NormalizedStatementID].NormalizedStatement;
                    var queryExtractedDataForCovering = context.StatementsExtractedData.DataPerQuery[kv];
                    if (IndexApplicability.IsIndexApplicableForQuery(queryExtractedDataForCovering, index))
                    {
                        context.IndicesDesignData.PossibleIndices.TryAddPossibleIndices(new[] { index }, normalizedStatement, query);
                    }
                }
            }
            var possibleIndices = context.IndicesDesignData.PossibleIndices.Clone();
            context.IndicesDesignData.Environments.Add(new VirtualIndicesEnvironment(possibleIndices));
        }
    }
}
