using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomaThesis.WebUI
{
    public class BaseDateTimeFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class BasePagingDateTimeFilter : BasePagingFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
