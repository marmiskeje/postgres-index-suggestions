using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IViewsRepository
    {
        IEnumerable<IView> GetAllNonSystems();
    }
}
