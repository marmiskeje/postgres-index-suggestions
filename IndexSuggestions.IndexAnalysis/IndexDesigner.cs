using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndexSuggestions.IndexAnalysis
{
    public class IndexDesigner
    {
        private readonly DAL.Contracts.IRepositoriesFactory dalRepositories;
        public IndexDesigner(DAL.Contracts.IRepositoriesFactory dalRepositories)
        {
            this.dalRepositories = dalRepositories;
        }
        public Task Run(long workloadId)
        {
            return Task.Run(() =>
            {
                var workloadStatements = dalRepositories.GetNormalizedWorkloadStatementsRepository().GetAllByWorkloadId(workloadId);
                var workloadSelects = workloadStatements.Where(x => x.NormalizedStatement.StatementDefinition != null && x.NormalizedStatement.StatementDefinition.CommandType == DAL.Contracts.StatementCommandType.Select);
                // aplikuj najskor workload filtrovanie na pocet vykonani !!!
                // key: relationId, value set of attributes
                Dictionary<long, HashSet<string>> relationWithPredicateAttributes = new Dictionary<long, HashSet<string>>();
                foreach (var s in workloadSelects)
                {
                    foreach (var p in s.NormalizedStatement.StatementDefinition.Predicates)
                    {
                        if (p.OperatorID > 0) // TODO filter operator: only for btree index:
                        {
                            var attributes = p.Operands.Where(x => x.AttributeName != null && x.RelationID.HasValue); // TODO: filter type: only for btree index (numerics, date etc.)
                            if (attributes.Count() > 0)
                            {
                                foreach (var a in attributes)
                                {
                                    if (!relationWithPredicateAttributes.ContainsKey(a.RelationID.Value))
                                    {
                                        relationWithPredicateAttributes.Add(a.RelationID.Value, new HashSet<string>());
                                    }
                                    relationWithPredicateAttributes[a.RelationID.Value].Add(a.AttributeName);
                                }
                            }
                        }
                    }
                }
                // vygeneruj vsetky kombinacie (1prvkove az n)
                // potom pridaj atributy zo selektu, orderby, etc. pre covering index
            });
        }
    }
}
