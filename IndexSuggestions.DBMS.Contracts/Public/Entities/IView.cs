using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IView : IDatabaseDependencyObject
    {
        uint ID { get; }
        string Name { get; }
        string SchemaName { get; }
        uint DatabaseID { get; }
        string Definition { get; }
    }
}
