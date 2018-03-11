using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    class SaveStatementIndexUsageCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public SaveStatementIndexUsageCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            if (context.NormalizedStatement != null && context.QueryPlan != null)
            {
                HashSet<long> indices = new HashSet<long>();
                FillAllIndices(indices, context.QueryPlan);
                foreach (var indexId in indices)
                {
                    var indicesRepository = repositories.GetIndicesRepository();
                    var index = indicesRepository.GetByPrimaryKey(indexId, true);
                    if (index == null)
                    {
                        index = new Index() { ID = indexId };
                        indicesRepository.Create(index);
                    }
                    var statementsRepository = repositories.GetNormalizedStatementsRepository();
                    var statement = statementsRepository.GetByStatement(context.NormalizedStatement, true);
                    if (statement == null)
                    {
                        statement = new NormalizedStatement() { Statement = context.NormalizedStatement };
                        statementsRepository.Create(statement);
                    }
                    var usageRepository = repositories.GetNormalizedStatementIndexUsagesRepository();
                    var usage = usageRepository.Get(statement.ID, index.ID, context.Entry.Timestamp.Date, true);
                    if (usage == null)
                    {
                        usage = new NormalizedStatementIndexUsage() { Date = context.Entry.Timestamp.Date, IndexID = index.ID, NormalizedStatementID = statement.ID };
                        usageRepository.Create(usage);
                    }
                    usage.Count += 1;
                    usageRepository.Update(usage);
                }
            }
        }

        private void FillAllIndices(ISet<long> indices, QueryPlanNode node)
        {
            if (node.IndexId.HasValue)
            {
                indices.Add(node.IndexId.Value);
            }
            foreach (var item in node.Plans)
            {
                FillAllIndices(indices, item);
            }
        }
    }
}
