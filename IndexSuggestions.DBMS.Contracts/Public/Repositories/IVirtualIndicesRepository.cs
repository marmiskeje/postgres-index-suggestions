using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IVirtualIndicesRepository
    {
        IVirtualIndex Create(VirtualIndexDefinition definition);
        void DestroyAll();
    }

}
