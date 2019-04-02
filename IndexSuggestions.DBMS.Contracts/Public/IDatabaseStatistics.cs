using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Contracts
{
    public interface IDatabaseStatistics
    {
        uint ID { get; set; }
        DateTime? LastResetDate { get; set; }
    }
}
