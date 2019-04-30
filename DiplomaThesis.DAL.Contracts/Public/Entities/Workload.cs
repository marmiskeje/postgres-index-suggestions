using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class Workload : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [NotMapped]
        public WorkloadDefinition Definition { get; set; }
        [Required]
        public string DefinitionData { get; set; }
        public List<WorkloadAnalysis> WorkloadAnalyses { get; set; }
    }

    public class WorkloadDefinition
    {
        public uint DatabaseID { get; set; }
        public WorkloadPropertyValuesDefinition<string> Users { get; set; }
        public WorkloadPropertyValuesDefinition<WorkloadDateTimeSlot> DateTimeSlots { get; set; }
        public WorkloadPropertyValuesDefinition<uint> Relations { get; set; }
        public WorkloadQueryThresholds QueryThresholds { get; set; }
        public WorkloadPropertyValuesDefinition<string> Applications { get; set; }
    }

    public class WorkloadPropertyValuesDefinition<T>
    {
        public HashSet<T> ForbiddenValues { get; set; }

        public WorkloadPropertyValuesDefinition()
        {
            ForbiddenValues = new HashSet<T>();
        }
    }

    public class WorkloadQueryThresholds
    {
        public TimeSpan? MinDuration { get; set; }
        public int? MinExectutionCount { get; set; }
    }

    public class WorkloadDateTimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
