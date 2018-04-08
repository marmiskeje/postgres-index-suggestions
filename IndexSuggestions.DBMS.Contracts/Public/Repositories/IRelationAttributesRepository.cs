using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IRelationAttributesRepository
    {
        RelationAttribute Get(long relationID, long position);
    }
}
