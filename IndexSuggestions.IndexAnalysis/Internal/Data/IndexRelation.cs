using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.IndexAnalysis
{
    internal class IndexRelation : IEquatable<IndexRelation>
    {
        public uint ID { get;}
        public string Name { get; }
        public string SchemaName { get; }
        public string DatabaseName { get; }

        public IndexRelation(uint id, string name, string schemaName, string databaseName)
        {
            ID = id;
            Name = name;
            SchemaName = schemaName;
            DatabaseName = databaseName;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IndexRelation))
            {
                return false;
            }
            return Equals((IndexRelation)obj);
        }

        public bool Equals(IndexRelation other)
        {
            if (other == null)
            {
                return false;
            }
            return ID.Equals(other.ID);
        }
    }
}
