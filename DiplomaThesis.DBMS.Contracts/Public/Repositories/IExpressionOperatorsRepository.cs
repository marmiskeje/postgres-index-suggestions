using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IExpressionOperatorsRepository
    {
        IExpressionOperator Get(uint operatorID);
    }
}
