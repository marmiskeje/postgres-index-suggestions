using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal class DateTimeSelectorsProvider : IDateTimeSelectorsProvider
    {
        private static readonly Lazy<IDateTimeSelectorsProvider> instance = new Lazy<IDateTimeSelectorsProvider>(new DateTimeSelectorsProvider());
        private readonly Lazy<IDateTimeSelector> minuteSelector = new Lazy<IDateTimeSelector>(new MinuteDateTimeSelector());
        private readonly Lazy<IDateTimeSelector> hourSelector = new Lazy<IDateTimeSelector>(new HourDateTimeSelector());
        private readonly Lazy<IDateTimeSelector> daySelector = new Lazy<IDateTimeSelector>(new DayDateTimeSelector());
        private DateTimeSelectorsProvider()
        {

        }

        public static IDateTimeSelectorsProvider Instance
        {
            get { return instance.Value; }
        }

        public IDateTimeSelector MinuteSelector
        {
            get { return minuteSelector.Value; }
        }

        public IDateTimeSelector HourSelector
        {
            get { return hourSelector.Value; }
        }

        public IDateTimeSelector DaySelector
        {
            get { return daySelector.Value; }
        }
    }
}
