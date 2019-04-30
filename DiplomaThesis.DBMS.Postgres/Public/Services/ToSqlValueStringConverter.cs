using DiplomaThesis.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    public class ToSqlValueStringConverter : IToSqlValueStringConverter
    {
        public string Convert(DbType dbType, object value)
        {
            var cultureInfo = new CultureInfo("en-US");
            switch (value)
            {
                case byte v:
                    return v.ToString(cultureInfo);
                case sbyte v:
                    return v.ToString(cultureInfo);
                case short v:
                    return v.ToString(cultureInfo);
                case ushort v:
                    return v.ToString(cultureInfo);
                case int v:
                    return v.ToString(cultureInfo);
                case uint v:
                    return v.ToString(cultureInfo);
                case long v:
                    return v.ToString(cultureInfo);
                case ulong v:
                    return v.ToString(cultureInfo);
                case float v:
                    return v.ToString(cultureInfo);
                case double v:
                    return v.ToString(cultureInfo);
                case decimal v:
                    return v.ToString(cultureInfo);
                case bool v:
                    return v ? "1" : "0";
                case char v:
                    return $"'{v}'";
                case string v:
                    return $"'{v}'";
                case DateTime v:
                    return $"'{v.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            }
            return null;
        }
    }
}
