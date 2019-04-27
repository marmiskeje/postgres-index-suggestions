using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndexSuggestions.WorkloadAnalyzer
{
    internal enum IndexStructureType
    {
        Unknown = 0,
        BTree = 1,
        Hash = 2,
        Gist = 3,
        Gin = 4,
        SpGist = 5,
        Brin = 6
    }
    internal class IndexDefinition
    {
        private readonly string identificationString;
        public IndexStructureType StructureType { get;}
        public RelationData Relation { get; }
        public IReadOnlyList<AttributeData> Attributes { get; }
        public IReadOnlyList<AttributeData> IncludeAttributes { get; }
        public string Name
        {
            get { return identificationString; }
        }
        public IndexDefinition(IndexStructureType structureType, RelationData relation, IEnumerable<AttributeData> attributes, IEnumerable<AttributeData> includeAttributes)
        {
            StructureType = structureType;
            Relation = relation;
            Attributes = new List<AttributeData>(attributes);
            if (includeAttributes != null)
            {
                IncludeAttributes = new List<AttributeData>(includeAttributes);
            }
            else
            {
                IncludeAttributes = new List<AttributeData>(0);
            }
            this.identificationString = $"{StructureType}_{Relation.DatabaseName}_{Relation.SchemaName}.{Relation.Name}_{String.Join("_", Attributes.Concat(IncludeAttributes).Select(x => x.Name))}";
        }

        public override int GetHashCode()
        {
            return identificationString.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IndexDefinition))
            {
                return false;
            }
            IndexDefinition tmp = (IndexDefinition)obj;
            return string.Equals(identificationString, tmp.identificationString);
        }

        public override string ToString()
        {
            return identificationString;
        }

        public IndexDefinition WithReplacedRelation(RelationData relation)
        {
            return new IndexDefinition(StructureType, relation, Attributes, IncludeAttributes);
        }
    }
}
