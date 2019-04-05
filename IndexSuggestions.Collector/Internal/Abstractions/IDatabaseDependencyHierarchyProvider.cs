using IndexSuggestions.Collector.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IDatabaseDependencyHierarchyProvider : IDisposable
    {
        IDatabaseDependencyHierarchy ProvideForDatabaseFilteredForViewOccurence(uint databaseId);
    }
}
