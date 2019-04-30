using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public interface IDatabaseDependencyHierarchyBuilder
    {
        IDatabaseDependencyHierarchy Build();
    }
}
