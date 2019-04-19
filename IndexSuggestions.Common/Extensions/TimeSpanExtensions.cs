using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Min(TimeSpan x1, TimeSpan x2)
        {
            return x1 < x2 ? x1 : x2;
        }
        public static TimeSpan Max(TimeSpan x1, TimeSpan x2)
        {
            return x1 > x2 ? x1 : x2;
        }
    }
}
