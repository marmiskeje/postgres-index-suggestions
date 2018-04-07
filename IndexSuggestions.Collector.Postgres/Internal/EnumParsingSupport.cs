using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Postgres
{
    internal static class EnumParsingSupport
    {
        public static T ConvertFromNumericOrDefault<T>(int n) where T : struct
        {
            if (Enum.IsDefined(typeof(T), n))
            {
                return (T)Enum.ToObject(typeof(T), n);
            }
            return default(T);
        }
    }
}
