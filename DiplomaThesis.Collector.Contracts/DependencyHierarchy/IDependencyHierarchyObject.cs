using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public interface IDependencyHierarchyObject
    {
        uint ID { get; }
        uint DatabaseID { get; }
        string SchemaName { get; }
        string Name { get; }
        string SearchOccurencePattern { get; }
        DependencyHierarchyObjectType ObjectType { get; }
        ISet<IDependencyHierarchyObject> Dependencies { get; }
    }
    public enum DependencyHierarchyObjectType
    {
        View = 0,
        StoredProcedure = 1
    }
}
