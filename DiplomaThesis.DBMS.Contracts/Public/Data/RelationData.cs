using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public class RelationData : IEquatable<RelationData>
    {
        private readonly IRelation relation;
        public uint ID => relation.ID;
        public string Name => relation.Name;
        public string SchemaName => relation.SchemaName;
        public string DatabaseName => relation.DatabaseName;
        public long TuplesCount => relation.TuplesCount;

        public RelationData(IRelation relation)
        {
            this.relation = relation;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RelationData))
            {
                return false;
            }
            return Equals((RelationData)obj);
        }

        public bool Equals(RelationData other)
        {
            if (other == null)
            {
                return false;
            }
            return ID.Equals(other.ID);
        }
    }
}
