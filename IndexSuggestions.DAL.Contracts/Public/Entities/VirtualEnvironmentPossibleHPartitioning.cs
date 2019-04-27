using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class VirtualEnvironmentPossibleHPartitioning : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public uint RelationID { get; set; }
        [Required]
        public long VirtualEnvironmentID { get; set; }
        public VirtualEnvironment VirtualEnvironment { get; set; }
        [Required]
        public string PartitioningStatement { get; set; }
        [Required]
        public string PartitionStatementsData { get; set; }
        [NotMapped]
        public HashSet<string> PartitionStatements { get; set; } = new HashSet<string>();
    }
}
