using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.WorkloadAnalyzer
{
    internal class WorkloadRelationsData
    {
        private readonly string databaseName = null;
        private readonly IRelationsRepository relationsRepository;
        private readonly IRelationAttributesRepository attributesRepository;
        private readonly Dictionary<uint, RelationData> allRelations = new Dictionary<uint, RelationData>();
        private readonly Dictionary<uint, RelationData> evaluationReplacements = new Dictionary<uint, RelationData>();
        private readonly Dictionary<uint, PrimaryKeyData> primaryKeyPerRelation = new Dictionary<uint, PrimaryKeyData>();
        public WorkloadRelationsData(string databaseName, IRelationsRepository relationsRepository, IRelationAttributesRepository attributesRepository,
                                     HashSet<uint> allRelationIdsFromStatements, IDictionary<uint, uint> evaluationReplacements)
        {
            this.databaseName = databaseName;
            this.relationsRepository = relationsRepository;
            this.attributesRepository = attributesRepository;
            foreach (var relationId in allRelationIdsFromStatements)
            {
                var relation = GetRelation(relationId);
            }
            foreach (var kv in evaluationReplacements)
            {
                var sourceId = kv.Key;
                var targetId = kv.Value;
                var sourceRelation = GetRelation(sourceId);
                var targetRelation = GetRelation(targetId);
                this.evaluationReplacements.Add(sourceId, targetRelation);
            }
        }
        private RelationData GetRelation(uint relationId)
        {
            if (!allRelations.ContainsKey(relationId))
            {
                using (var scope = new DatabaseScope(databaseName))
                {
                    var loadedRelation = relationsRepository.Get(relationId);
                    RelationData relationData = null;
                    if (loadedRelation != null)
                    {
                        relationData = new RelationData(loadedRelation);
                    }
                    allRelations.Add(relationId, relationData);
                }
            }
            return allRelations[relationId];
        }
        public bool TryGetRelation(uint relationId, out RelationData relation)
        {
            relation = GetRelation(relationId);
            return relation != null;
        }

        public RelationData GetReplacementOrOriginal(uint originalRelationId)
        {
            RelationData result = null;
            if (!evaluationReplacements.TryGetValue(originalRelationId, out result))
            {
                result = GetRelation(originalRelationId);
            }
            return result;
        }
        public bool TryGetPrimaryKey(uint relationId, out PrimaryKeyData primaryKey)
        {
            if (!primaryKeyPerRelation.ContainsKey(relationId))
            {
                using (var scope = new DatabaseScope(databaseName))
                {
                    var relation = relationsRepository.Get(relationId);
                    PrimaryKeyData primaryKeyToAdd = null;
                    if (relation != null)
                    {
                        List<AttributeData> primaryKeyAttributes = new List<AttributeData>();
                        if (relation.PrimaryKeyAttributeNames != null)
                        {
                            foreach (var attributeName in relation.PrimaryKeyAttributeNames)
                            {
                                primaryKeyAttributes.Add(new AttributeData(GetRelation(relationId), attributesRepository.Get(relationId, attributeName)));
                            }
                        }
                        if (primaryKeyAttributes.Count > 0)
                        {
                            primaryKeyToAdd = new PrimaryKeyData(primaryKeyAttributes);
                        }
                    }
                    primaryKeyPerRelation.Add(relationId, primaryKeyToAdd);
                }
            }
            primaryKey = primaryKeyPerRelation[relationId];
            return primaryKey != null;
        }
    }
}
