using DiplomaThesis.Collector.Contracts;
using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class DependencyHierarchyObject<TObject> : IDependencyHierarchyObject where TObject : IDatabaseDependencyObject
    {
        public uint ID { get; set; }
        public uint DatabaseID { get; set; }
        public string Name { get; set; }

        public DependencyHierarchyObjectType ObjectType { get; set; }

        public ISet<IDependencyHierarchyObject> Dependencies { get; } = new HashSet<IDependencyHierarchyObject>();

        public string SchemaName { get; set; }
        public string SearchOccurencePattern { get; set; }
        internal TObject DatabaseDependencyObject { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DependencyHierarchyObject<TObject>))
            {
                return false;
            }
            DependencyHierarchyObject<TObject> tmp = (DependencyHierarchyObject<TObject>)obj;
            return ID == tmp.ID && DatabaseID == tmp.DatabaseID;
        }

        public override int GetHashCode()
        {
            return $"{ID}_{DatabaseID}".GetHashCode();
        }
    }
}
