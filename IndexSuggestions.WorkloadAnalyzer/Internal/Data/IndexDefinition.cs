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
        public IndexRelation Relation { get; }
        public IReadOnlyList<IndexAttribute> Attributes { get; }
        public IReadOnlyList<IndexAttribute> IncludeAttributes { get; }
        public string Name
        {
            get { return identificationString; }
        }
        public IndexDefinition(IndexStructureType structureType, IndexRelation relation, IEnumerable<IndexAttribute> attributes, IEnumerable<IndexAttribute> includeAttributes)
        {
            StructureType = structureType;
            Relation = relation;
            Attributes = new List<IndexAttribute>(attributes);
            if (includeAttributes != null)
            {
                IncludeAttributes = new List<IndexAttribute>(includeAttributes);
            }
            else
            {
                IncludeAttributes = new List<IndexAttribute>(0);
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
    }
}
