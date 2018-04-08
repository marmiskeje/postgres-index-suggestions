using IndexSuggestions.Collector.Contracts;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class SaveStatementDefinitionIfNotExistsCommand : ChainableCommand
    {
        private readonly LogEntryProcessingContext context;
        private readonly IRepositoriesFactory repositories;
        public SaveStatementDefinitionIfNotExistsCommand(LogEntryProcessingContext context, IRepositoriesFactory repositories)
        {
            this.context = context;
            this.repositories = repositories;
        }
        protected override void OnExecute()
        {
            if (context.PersistedData.NormalizedStatement.StatementDefinition == null)
            {
                StatementDefinition definition = new StatementDefinition();
                definition.CommandType = Convert(context.QueryTree.QueryCommandType);
                definition.Predicates = new List<StatementPredicate>();
                definition.Relations = new List<StatementRelation>();
                foreach (var p in context.QueryTree.Predicates)
                {
                    definition.Predicates.Add(Convert(p));
                }
                foreach (var r in context.QueryTree.Relations)
                {
                    definition.Relations.Add(Convert(r));
                }
                context.PersistedData.NormalizedStatement.StatementDefinition = definition;
                var statements = repositories.GetNormalizedStatementsRepository();
                statements.Update(context.PersistedData.NormalizedStatement);
            }
        }

        private StatementRelation Convert(QueryTreeRelation r)
        {
            StatementRelation result = new StatementRelation();
            result.ID = r.ID;
            return result;
        }

        private StatementPredicate Convert(QueryTreePredicate p)
        {
            StatementPredicate result = new StatementPredicate();
            result.Operands = new List<StatementPredicateOperand>();
            foreach (var o in p.Operands)
            {
                result.Operands.Add(Convert(o));
            }
            result.OperatorID = p.OperatorID;
            return result;
        }

        private StatementPredicateOperand Convert(QueryTreePredicateOperand o)
        {
            StatementPredicateOperand result = new StatementPredicateOperand();
            result.AttributeName = o.AttributeName;
            result.ConstValue = o.ConstValue;
            result.RelationID = o.RelationID;
            result.Type = o.Type;
            result.TypeId = o.TypeId;
            return result;
        }

        private StatementCommandType Convert(QueryCommandType source)
        {
            return (StatementCommandType)((int)source);
        }
    }
}
