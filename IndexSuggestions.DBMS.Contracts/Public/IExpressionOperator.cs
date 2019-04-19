using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IExpressionOperator
    {
        uint ID { get; set; }
        string Name { get; set; }
    }
}
