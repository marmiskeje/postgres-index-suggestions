using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRelationsRepository
    {
        IRelation Get(uint relationID);
    }
}
