using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IDateTimeSelector
    {
        DateTime Select(DateTime date);
    }
}
