using System;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class LoadWorkloadRelationsDataCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IRelationsRepository relationsRepository;
        private readonly IRelationAttributesRepository attributesRepository;
        public LoadWorkloadRelationsDataCommand(WorkloadAnalysisContext context, IRelationsRepository relationsRepository, IRelationAttributesRepository attributesRepository)
        {
            this.context = context;
            this.relationsRepository = relationsRepository;
            this.attributesRepository = attributesRepository;
        }

        protected override void OnExecute()
        {
            HashSet<uint> allFromStatements = new HashSet<uint>();
            Dictionary<uint, uint> evaluationReplacements = new Dictionary<uint, uint>();
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                foreach (var kv in context.StatementsData.AllQueriesByRelation)
                {
                    var relationID = kv.Key;
                    allFromStatements.Add(relationID);
                }
                foreach (var kv in context.WorkloadAnalysis.RelationReplacements)
                {
                    var originalRelationID = kv.SourceId;
                    var replacementRelationID = kv.TargetId;
                    evaluationReplacements.Add(originalRelationID, replacementRelationID);
                }
            }
            context.RelationsData = new WorkloadRelationsData(relationsRepository, attributesRepository, allFromStatements, evaluationReplacements);
        }
    }
}