using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public class ReportingSettings
    {
        public HashSet<string> Recipients { get; set; } = new HashSet<string>();
    }
}
