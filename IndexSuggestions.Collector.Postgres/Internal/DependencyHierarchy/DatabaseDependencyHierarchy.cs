using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    internal class DatabaseDependencyHierarchy : IDatabaseDependencyHierarchy
    {
        public ISet<IDependencyHierarchyObject> Views { get; } = new HashSet<IDependencyHierarchyObject>();

        public ISet<IDependencyHierarchyObject> StoredProcedures { get; } = new HashSet<IDependencyHierarchyObject>();
    }
}
