using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IRelationAttributesRepository
    {
        IRelationAttribute Get(uint relationID, int attributeNumber);
        IRelationAttribute Get(uint relationID, string attributeName);
    }
}
