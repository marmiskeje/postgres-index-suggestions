using System;
using System.Collections.Generic;
using IndexSuggestions.Common.CommandProcessing;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal class LoadExistingIndicesCommand : ChainableCommand
    {
        private readonly WorkloadAnalysisContext context;
        private readonly IIndicesRepository indicesRepository;

        public LoadExistingIndicesCommand(WorkloadAnalysisContext context, IIndicesRepository indicesRepository)
        {
            this.context = context;
            this.indicesRepository = indicesRepository;
        }

        protected override void OnExecute()
        {
            using (var scope = new DatabaseScope(context.Database.Name))
            {
                var existingIndices = indicesRepository.GetAll();
                foreach (var i in existingIndices)
                {
                    var relation = new IndexRelation(i.RelationID, i.RelationName, i.SchemaName, i.DatabaseName);
                    var attributes = new List<IndexAttribute>();
                    foreach (var a in i.AttributesNames)
                    {
                        attributes.Add(new IndexAttribute(relation, a, System.Data.DbType.Object, 0, null, null));
                    }
                    context.IndicesDesignData.ExistingIndices.Add(new IndexDefinition(Convert(i.AccessMethod), relation, attributes, null));
                }
            }
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