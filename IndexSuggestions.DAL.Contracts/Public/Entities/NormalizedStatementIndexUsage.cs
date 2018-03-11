using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class NormalizedStatementIndexUsage : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long NormalizedStatementID { get; set; }
        public NormalizedStatement NormalizedStatement { get; set; }
        public long IndexID { get; set; }
        public Index Index { get; set; }
        public DateTime Date { get; set; }
        public long Count { get; set; }
    }
}
