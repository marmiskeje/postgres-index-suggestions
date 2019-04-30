using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class TotalDatabaseStatistics : ITotalDatabaseStatistics
    {
        public DateTime CollectedDate { get; set; }
        public IDatabaseStatistics Database { get; set; }
        public IEnumerable<IRelationStatistics> Relations { get; set; }
        public IEnumerable<IIndexStatistics> Indices { get; set; }
        public IEnumerable<IStoredProcedureStatistics> StoredProcedures { get; set; }
    }

    internal class TotalDatabaseCollectionInfo
    {
        [Column("collected_date")]
        public DateTime CollectedDate { get; set; }
    }
}
