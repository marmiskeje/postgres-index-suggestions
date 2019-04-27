using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IVirtualHPartitioningsRepository
    {
        void InitializeEnvironment();
        IVirtualHPartitioning Create(VirtualHPartitioningDefinition definition);
        void DestroyAll();
    }

}
