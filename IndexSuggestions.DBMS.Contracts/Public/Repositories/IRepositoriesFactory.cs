using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRepositoriesFactory
    {
        IRelationAttributesRepository GetRelationAttributesRepository();
        IRelationsRepository GetRelationsRepository();
        IIndicesRepository GetIndicesRepository();
        IExplainRepository GetExplainRepository();
        IVirtualIndicesRepository GetVirtualIndicesRepository();
        IDatabasesRepository GetDatabasesRepository();
    }
}
