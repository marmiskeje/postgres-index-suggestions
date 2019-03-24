using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IDateTimeSelector
    {
        DateTime Select(DateTime date);
    }
}
