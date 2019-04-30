using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    internal static class SqlReaderSupport
    {

        public static T GetValueOrDefault<T>(object item)
        {
            return GetValueOrDefault<T>(item, default(T));
        }

        public static T GetValueOrDefault<T>(object item, T defaultValue)
        {
            if (DBNull.Value.Equals(item))
            {
                return defaultValue;
            }
            if (!(item is T))
            {
                throw new InvalidCastException();
            }
            return (T)item;
        }
    }
}
