using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class NormalizedStatement : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [MaxLength(8000)]
        [Required]
        public string Statement { get; set; }
        [MaxLength(100)]
        [Required]
        public string StatementFingerprint { get; set; }
        public string StatementDefinitionData { get; set; }
        [NotMapped]
        public StatementDefinition StatementDefinition { get; set; }
        public List<NormalizedStatementIndexUsage> NormalizedStatementIndexUsages { get; set; }
        public List<NormalizedWorkloadStatement> NormalizedWorkloadStatements { get; set; }
    }

    public class StatementDefinition
    {
        public StatementCommandType CommandType { get; set; }
    }

    public enum StatementCommandType
    {
        Unknown = 0,
        Select = 1,
        Insert = 2,
        Update = 3,
        Delete = 4,
        Utility = 5
    }
}
