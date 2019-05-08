using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class UnusedObjectsData
    {
        public List<UnusedObjectData> Objects { get; set; }
    }

    public class UnusedObjectData
    {
        public string SchemaName { get; set; }
        public string Name { get; set; }
        public string ObjectType { get; set; }
        public DateTime? LastKnownUsageDate { get; set; }
        public bool? IsUnused { get; set; }
    }
}
