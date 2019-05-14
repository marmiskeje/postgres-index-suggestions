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
                case Guid v:
                    return $"'{v}'";
                case DateTime v:
                    return $"'{v.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            }
            return null;
        }

        public string ConvertStringRepresentation(DbType dbType, string value)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                case DbType.Guid:
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return $"'{value}'";
                case DbType.Boolean:
                case DbType.Decimal:
                case DbType.Single:
                case DbType.Double:
                case DbType.VarNumeric:
                case DbType.Byte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return value;
            }
            return null;
        }
    }
}
