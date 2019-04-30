using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface ITotalDatabaseStatistics
    {
        DateTime CollectedDate { get; }
        IDatabaseStatistics Database { get; }
        IEnumerable<IRelationStatistics> Relations { get; }
        IEnumerable<IIndexStatistics> Indices { get; }
        IEnumerable<IStoredProcedureStatistics> StoredProcedures { get; }
    }
}
