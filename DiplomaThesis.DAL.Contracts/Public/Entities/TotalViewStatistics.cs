using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class TotalViewStatistics : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public uint DatabaseID { get; set; }
        [Required]
        public uint ViewID { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long RewrittenCount { get; set; }
    }

    public class TotalViewStatisticsUniqueKey
    {
        public uint DatabaseID { get; set; }
        public uint ViewID { get; set; }
        public DateTime Date { get; set; }
    }
}
