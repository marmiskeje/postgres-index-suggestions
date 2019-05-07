using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IRelationsRepository
    {
        IRelation Get(uint relationID);
        IEnumerable<IRelation> GetAll();
        IEnumerable<IRelation> GetAllNonSystems();
    }
}
