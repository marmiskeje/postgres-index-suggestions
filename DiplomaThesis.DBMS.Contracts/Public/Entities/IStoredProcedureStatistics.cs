using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IStoredProcedureStatistics : IEquatable<IStoredProcedureStatistics>
    {
        uint ID { get; set; }
        uint DatabaseID { get; set; }
        long CallsCount { get; set; }
        decimal TotalDurationInMs { get; set; }
        decimal SelfDurationInMs { get; set; }
    }
}
