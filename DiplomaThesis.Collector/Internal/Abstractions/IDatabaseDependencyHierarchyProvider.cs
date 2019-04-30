using DiplomaThesis.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IDatabaseDependencyHierarchyProvider : IDisposable
    {
        IDatabaseDependencyHierarchy ProvideForDatabaseFilteredForViewOccurence(uint databaseId);
    }
}
