using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IDateTimeSelectorsProvider
    {
        IDateTimeSelector MinuteSelector { get; }
        IDateTimeSelector HourSelector { get; }
        IDateTimeSelector DaySelector { get; }
    }
}
