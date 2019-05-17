using DiplomaThesis.Common;
using DiplomaThesis.Common.CommandProcessing;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    /// <summary>
    /// Generates combinations of indices
    /// </summary>
    internal class GenerateFinalIndicesEnvironmentsCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        public GenerateFinalIndicesEnvironmentsCommand(WorkloadAnalysisContext context)
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
            foreach (var kv in context.IndicesDesignData.PossibleIndices.AllPerStatement)
            {
                var indices = kv.Value;
                for (int combinationLength = 1; combinationLength <= indices.Count; combinationLength++)
                {
                    var combinations = new Combinations<IndexDefinition>(indices, combinationLength);
                    foreach (var c in combinations)
                    {
                        context.IndicesDesignData.Environments.Add(new VirtualIndicesEnvironment(context.IndicesDesignData.PossibleIndices.ToSubSetOf(c)));
                    }
                }
            }
            /*
            var allIndices = new ElementSet<IndexDefinition>(context.IndicesDesignData.PossibleIndices.All);
            for (int combinationLength = 1; combinationLength <= allIndices.Count; combinationLength++)
            {
                var combinations = new Combinations<IndexDefinition>(allIndices, combinationLength);
                foreach (var c in combinations)
                {
                    context.IndicesDesignData.Environments.Add(new VirtualIndicesEnvironment(context.IndicesDesignData.PossibleIndices.ToSubSetOf(c)));
                }
            }
            */
        }
    }
}
