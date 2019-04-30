using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiplomaThesis.DBMS.Postgres
{
    public static class PostgresDbTypeCovertUtility
    {
        #region private DbType Convert(PostgresDbType source)
        //http://www.npgsql.org/doc/types/basic.html
        public static DbType Convert(PostgresDbType source)
        {
            switch (source)
            {
                case PostgresDbType.Bigint:
                    return DbType.Int64;
                case PostgresDbType.Double:
                    return DbType.Double;
                case PostgresDbType.Integer:
                    return DbType.Int32;
                case PostgresDbType.Numeric:
                    return DbType.Decimal;
                case PostgresDbType.Real:
                    return DbType.Single;
                case PostgresDbType.Smallint:
                    return DbType.Int16;
                case PostgresDbType.Money:
                    return DbType.Currency;
                case PostgresDbType.Boolean:
                    return DbType.Boolean;
                case PostgresDbType.Char:
                    return DbType.String;
                case PostgresDbType.Text:
                    return DbType.String;
                case PostgresDbType.Varchar:
                    return DbType.String;
                case PostgresDbType.Name:
                    return DbType.String;
                case PostgresDbType.Citext:
                    return DbType.String;
                case PostgresDbType.InternalChar:
                    return DbType.String;
                case PostgresDbType.Bytea:
                    return DbType.Binary;
                case PostgresDbType.Date:
                    return DbType.Date;
                case PostgresDbType.Time:
                    return DbType.Time;
                case PostgresDbType.Timestamp:
                    return DbType.DateTime;
                case PostgresDbType.TimestampTz:
                    return DbType.DateTimeOffset;
                case PostgresDbType.TimeTz:
                    return DbType.DateTimeOffset;
                case PostgresDbType.Bit:
                    return DbType.Boolean;
                case PostgresDbType.Varbit:
                    return DbType.Boolean;
                case PostgresDbType.Uuid:
                    return DbType.Guid;
                case PostgresDbType.Xml:
                    return DbType.String;
                case PostgresDbType.Json:
                    return DbType.String;
                case PostgresDbType.Jsonb:
                    return DbType.Binary;
                case PostgresDbType.Oid:
                    return DbType.UInt32;
                case PostgresDbType.Xid:
                    return DbType.UInt32;
                case PostgresDbType.Cid:
                    return DbType.UInt32;
                default:
                    return DbType.Object;
            }
        }
        #endregion
    }
}
