using DiplomaThesis.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Postgres
{
    internal class DatabaseDependencyHierarchy : IDatabaseDependencyHierarchy
    {
        public ISet<IDependencyHierarchyObject> Views { get; } = new HashSet<IDependencyHierarchyObject>();

        public ISet<IDependencyHierarchyObject> StoredProcedures { get; } = new HashSet<IDependencyHierarchyObject>();
    }
}
