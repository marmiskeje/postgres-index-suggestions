using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface IDatabaseDependencyHierarchy
    {
        ISet<IDependencyHierarchyObject> Views { get; }
        ISet<IDependencyHierarchyObject> StoredProcedures { get; }
    }
}
