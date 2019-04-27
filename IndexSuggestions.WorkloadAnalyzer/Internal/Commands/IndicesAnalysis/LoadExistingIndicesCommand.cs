using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;
using System.Linq;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class LoadExistingIndicesCommand : ChainableCommand
    {
        private static readonly Regex INDEX_DEFINITION_INCLUDED_PART = new Regex(@"include\(.*\)|INCLUDE\(.*\)");
        private readonly WorkloadAnalysisContext context;
        private readonly IIndicesRepository indicesRepository;
        private readonly IRelationsRepository relationsRepository;
        private readonly IRelationAttributesRepository attributesRepository;

        public LoadExistingIndicesCommand(WorkloadAnalysisContext context, IIndicesRepository indicesRepository, IRelationsRepository relationsRepository,
                                          IRelationAttributesRepository attributesRepository)
        {
            this.context = context;
            this.indicesRepository = indicesRepository;
            this.relationsRepository = relationsRepository;
            this.attributesRepository = attributesRepository;
        }

        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                var existingIndices = indicesRepository.GetAll();
                foreach (var i in existingIndices)
                {
                    var relation = relationsRepository.Get(i.RelationID);
                    var relationData = new RelationData(relation);
                    var attributes = new List<AttributeData>();
                    var includedAttributes = new List<AttributeData>();
                    foreach (var atrributeName in i.AttributesNames)
                    {
                        var attribute = attributesRepository.Get(relation.ID, atrributeName);
                        var toAdd = new AttributeData(relationData, attribute);
                        if (IsIncludedAttribute(i.CreateDefinition, attribute.Name))
                        {
                            includedAttributes.Add(toAdd);
                        }
                        else
                        {
                            attributes.Add(toAdd);
                        }
                    }
                    var existingIndexToAdd = new IndexDefinition(Convert(i.AccessMethod), relationData, attributes, includedAttributes);
                    if (context.StatementsData.AllQueriesByRelation.TryGetValue(relation.ID, out var possibleQueriesForIndex))
                    {
                        foreach (var statementQueryPair in possibleQueriesForIndex)
                        {
                            var normalizedStatementID = statementQueryPair.NormalizedStatementID;
                            var normalizedStatement = context.StatementsData.All[normalizedStatementID].NormalizedStatement;
                            var query = statementQueryPair.Query;
                            var extractedData = context.StatementsExtractedData.DataPerQuery[statementQueryPair];
                            if (IsIndexApplicableForQuery(extractedData, existingIndexToAdd))
                            {
                                context.IndicesDesignData.ExistingIndices.TryAddPossibleIndices(new[] { existingIndexToAdd }, normalizedStatement, query);
                            }
                        }
                    }
                }
            }
        }

        private bool IsIndexApplicableForQuery(StatementQueryExtractedData extractedData, IndexDefinition index)
        {
            return extractedData.WhereAttributes
                .Union(extractedData.JoinAttributes)
                .Union(extractedData.GroupByAttributes)
                .Union(extractedData.OrderByAttributes).Contains(index.Attributes.First());
        }

        private bool IsIncludedAttribute(string indexDefinition, string attributeName)
        {
            var match = INDEX_DEFINITION_INCLUDED_PART.Match(indexDefinition);
            if (match != null && match.Success)
            {
                return match.Value.Contains(attributeName);
            }
            return false;
        }

        private IndexStructureType Convert(IndexAccessMethodType accessMethod)
        {
            switch (accessMethod)
            {
                case IndexAccessMethodType.BTree:
                    return IndexStructureType.BTree;
                case IndexAccessMethodType.Hash:
                    return IndexStructureType.Hash;
                case IndexAccessMethodType.Gist:
                    return IndexStructureType.Gist;
                case IndexAccessMethodType.Gin:
                    return IndexStructureType.Gin;
                case IndexAccessMethodType.SpGist:
                    return IndexStructureType.SpGist;
                case IndexAccessMethodType.Brin:
                    return IndexStructureType.Brin;
            }
            return IndexStructureType.Unknown;
        }
    }
}