using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class WorkloadData
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public uint DatabaseID { get; set; }
        public DateTime CreatedDate { get; set; }
        public WorkloadDefinitionData Definition { get; set; }
    }

    public class WorkloadDefinitionData
    {
        public HashSet<string> ForbiddenUsers { get; set; }
        public List<WorkloadDateTimeSlotData> ForbiddenDateTimeSlots { get; set; }
        public HashSet<uint> ForbiddenRelations { get; set; }
        public int StatementMinDurationInMs { get; set; }
        public int StatementMinExectutionCount { get; set; }
        public HashSet<string> ForbiddenApplications { get; set; }
    }

    public class WorkloadDateTimeSlotData
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}
