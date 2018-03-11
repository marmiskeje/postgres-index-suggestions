using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface IEntity<TKey>
    {
        TKey ID { get; set; }
    }
}
