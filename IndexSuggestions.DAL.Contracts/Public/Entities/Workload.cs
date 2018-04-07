using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public class Workload : IEntity<long>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [NotMapped]
        public WorkloadDefinition Definition { get; set; }
        [Required]
        public string DefinitionData { get; set; }
        public List<NormalizedWorkloadStatement> NormalizedWorkloadStatements { get; set; }
    }

    public class WorkloadDefinition
    {
        public string DatabaseName { get; set; }
        public WorkloadPropertyValuesDefinition<String> Users { get; set; }
        public WorkloadPropertyValuesDefinition<WorkloadDateTimeSlot> DateTimeSlots { get; set; }
        public WorkloadPropertyValuesDefinition<WorkloadRelation> Relations { get; set; }
        public WorkloadQueryThresholds QueryThresholds { get; set; }
        public WorkloadPropertyValuesDefinition<String> Applications { get; set; }
    }

    public class WorkloadPropertyValuesDefinition<T>
    {
        public HashSet<T> Values { get; set; }

        public WorkloadPropertyRestrictionType RestrictionType { get; set; }

        public WorkloadPropertyValuesDefinition()
        {
            Values = new HashSet<T>();
        }
    }

    public class WorkloadQueryThresholds
    {
        public TimeSpan? MinDuration { get; set; }
        public int? MinExectutionCount { get; set; }
    }

    public class WorkloadRelation
    {
        public long ID { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
    }

    public class WorkloadDateTimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }

    public enum WorkloadPropertyRestrictionType
    {
        Allowed = 0,
        Disallowed = 1
    }
}
