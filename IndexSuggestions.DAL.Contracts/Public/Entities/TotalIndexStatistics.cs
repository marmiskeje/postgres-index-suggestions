using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class TotalIndexStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public uint IndexID { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public long IndexScanCount { get; set; }
        public long IndexTupleReadCount { get; set; }
        public long IndexTupleFetchCount { get; set; }
    }

    public class TotalIndexStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint RelationID { get; set; }
        public uint IndexID { get; set; }
        public DateTime Date { get; set; }
    }
}
