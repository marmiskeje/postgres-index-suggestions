using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class RelationData
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string SchemaName { get; set; }
        public string FullName
        {
            get { return SchemaName + "." + Name; }
        }
    }
}
