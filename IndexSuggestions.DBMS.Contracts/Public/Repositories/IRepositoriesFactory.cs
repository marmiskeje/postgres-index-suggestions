using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRepositoriesFactory
    {
        IRelationAttributesRepository GetRelationAttributesRepository();
    }
}
