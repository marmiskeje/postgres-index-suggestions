using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    public enum PostgresDbType
    { // Note that it's important to never change the numeric values of this enum, since user applications
        // compile them in.

        #region Numeric Types

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte "bigint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [PostgresDbTypeIdentification("int8", 20)]
        Bigint = 1,

        /// <summary>
        /// Corresponds to the PostgreSQL 8-byte floating-point "double" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [PostgresDbTypeIdentification("float8", 701)]
        Double = 8,

        /// <summary>
        /// Corresponds to the PostgreSQL 4-byte "integer" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [PostgresDbTypeIdentification("int4", 23)]
        Integer = 9,

        /// <summary>
        /// Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [PostgresDbTypeIdentification("numeric", 1700)]
        Numeric = 13,

        /// <summary>
        /// Corresponds to the PostgreSQL floating-point "real" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [PostgresDbTypeIdentification("float4", 700)]
        Real = 17,

        /// <summary>
        /// Corresponds to the PostgreSQL 2-byte "smallint" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-numeric.html</remarks>
        [PostgresDbTypeIdentification("int2", 21)]
        Smallint = 18,

        /// <summary>
        /// Corresponds to the PostgreSQL "money" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-money.html</remarks>
        [PostgresDbTypeIdentification("money", 790)]
        Money = 12,

        #endregion

        #region Boolean Type

        /// <summary>
        /// Corresponds to the PostgreSQL "boolean" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        [PostgresDbTypeIdentification("bool", 16)]
        Boolean = 2,

        #endregion

        #region Geometric types

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "box" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("box", 603)]
        Box = 3,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "circle" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("circle", 718)]
        Circle = 5,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "line" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("line", 628)]
        Line = 10,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "lseg" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("lseg", 601)]
        LSeg = 11,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "path" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("path", 602)]
        Path = 14,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "point" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("point", 600)]
        Point = 15,

        /// <summary>
        /// Corresponds to the PostgreSQL geometric "polygon" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-geometric.html</remarks>
        [PostgresDbTypeIdentification("polygon", 604)]
        Polygon = 16,

        #endregion

        #region Character Types

        /// <summary>
        /// Corresponds to the PostgreSQL "char(n)" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [PostgresDbTypeIdentification("bpchar", 1042)]
        Char = 6,

        /// <summary>
        /// Corresponds to the PostgreSQL "text" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [PostgresDbTypeIdentification("text", 25)]
        Text = 19,

        /// <summary>
        /// Corresponds to the PostgreSQL "varchar" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [PostgresDbTypeIdentification("varchar", 1043)]
        Varchar = 22,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "name" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-character.html</remarks>
        [PostgresDbTypeIdentification("name", 19)]
        Name = 32,

        /// <summary>
        /// Corresponds to the PostgreSQL "citext" type for the citext module.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/citext.html</remarks>
        Citext = 51,   // Extension type

        /// <summary>
        /// Corresponds to the PostgreSQL "char" type.
        /// </summary>
        /// <remarks>
        /// This is an internal field and should normally not be used for regular applications.
        ///
        /// See http://www.postgresql.org/docs/current/static/datatype-text.html
        /// </remarks>
        [PostgresDbTypeIdentification("char", 18)]
        InternalChar = 38,

        #endregion

        #region Binary Data Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-binary.html</remarks>
        [PostgresDbTypeIdentification("bytea", 17)]
        Bytea = 4,

        #endregion

        #region Date/Time Types

        /// <summary>
        /// Corresponds to the PostgreSQL "date" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [PostgresDbTypeIdentification("date", 1082)]
        Date = 7,

        /// <summary>
        /// Corresponds to the PostgreSQL "time" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [PostgresDbTypeIdentification("time", 1083)]
        Time = 20,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [PostgresDbTypeIdentification("timestamp", 1114)]
        Timestamp = 21,

        /// <summary>
        /// Corresponds to the PostgreSQL "timestamp with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [PostgresDbTypeIdentification("timestamptz", 1184)]
        TimestampTz = 26,

        /// <summary>
        /// Corresponds to the PostgreSQL "interval" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [PostgresDbTypeIdentification("interval", 1186)]
        Interval = 30,

        /// <summary>
        /// Corresponds to the PostgreSQL "time with time zone" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-datetime.html</remarks>
        [PostgresDbTypeIdentification("timetz", 1266)]
        TimeTz = 31,

        #endregion

        #region Network Address Types

        /// <summary>
        /// Corresponds to the PostgreSQL "inet" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [PostgresDbTypeIdentification("inet", 869)]
        Inet = 24,

        /// <summary>
        /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [PostgresDbTypeIdentification("cidr", 650)]
        Cidr = 44,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [PostgresDbTypeIdentification("macaddr", 829)]
        MacAddr = 34,

        /// <summary>
        /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-net-types.html</remarks>
        [PostgresDbTypeIdentification("macaddr8", 774)]
        MacAddr8 = 54,

        #endregion

        #region Bit String Types

        /// <summary>
        /// Corresponds to the PostgreSQL "bit" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-bit.html</remarks>
        [PostgresDbTypeIdentification("bit", 1560)]
        Bit = 25,

        /// <summary>
        /// Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length string of bits.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-boolean.html</remarks>
        [PostgresDbTypeIdentification("varbit", 1562)]
        Varbit = 39,

        #endregion

        #region Text Search Types

        /// <summary>
        /// Corresponds to the PostgreSQL "tsvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [PostgresDbTypeIdentification("tsvector", 3614)]
        TsVector = 45,

        /// <summary>
        /// Corresponds to the PostgreSQL "tsquery" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-textsearch.html</remarks>
        [PostgresDbTypeIdentification("tsquery", 3615)]
        TsQuery = 46,

        #endregion

        #region UUID Type

        /// <summary>
        /// Corresponds to the PostgreSQL "uuid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-uuid.html</remarks>
        [PostgresDbTypeIdentification("uuid", 2950)]
        Uuid = 27,

        #endregion

        #region XML Type

        /// <summary>
        /// Corresponds to the PostgreSQL "xml" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-xml.html</remarks>
        [PostgresDbTypeIdentification("xml", 142)]
        Xml = 28,

        #endregion

        #region JSON Types

        /// <summary>
        /// Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-json.html</remarks>
        /// <seealso cref="Jsonb"/>
        [PostgresDbTypeIdentification("json", 114)]
        Json = 35,

        /// <summary>
        /// Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized binary
        /// format.
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.4.
        /// See http://www.postgresql.org/docs/current/static/datatype-json.html
        /// </remarks>
        [PostgresDbTypeIdentification("jsonb", 3802)]
        Jsonb = 36,

        #endregion

        #region HSTORE Type

        /// <summary>
        /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value pairs.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/hstore.html</remarks>
        Hstore = 37,    // Extension type

        #endregion

        #region Arrays

        /// <summary>
        /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
        /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/arrays.html</remarks>
        Array = int.MinValue,

        #endregion

        #region Range Types

        /// <summary>
        /// Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of
        /// another type. This value must be combined with another value from <see cref="NpgsqlDbType"/>
        /// via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
        /// </summary>
        /// <remarks>
        /// Supported since PostgreSQL 9.2.
        /// See http://www.postgresql.org/docs/9.2/static/rangetypes.html
        /// </remarks>
        Range = 0x40000000,

        #endregion

        #region Internal Types

        /// <summary>
        /// Corresponds to the PostgreSQL "refcursor" type.
        /// </summary>
        [PostgresDbTypeIdentification("refcursor", 1790)]
        Refcursor = 23,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "oidvector" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [PostgresDbTypeIdentification("oidvector", 30)]
        Oidvector = 29,

        /// <summary>
        /// Corresponds to the PostgreSQL internal "int2vector" type.
        /// </summary>
        [PostgresDbTypeIdentification("int2vector", 22)]
        Int2Vector = 52,

        /// <summary>
        /// Corresponds to the PostgreSQL "oid" type.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [PostgresDbTypeIdentification("oid", 26)]
        Oid = 41,

        /// <summary>
        /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [PostgresDbTypeIdentification("xid", 28)]
        Xid = 42,

        /// <summary>
        /// Corresponds to the PostgreSQL "cid" type, an internal command identifier.
        /// </summary>
        /// <remarks>See http://www.postgresql.org/docs/current/static/datatype-oid.html</remarks>
        [PostgresDbTypeIdentification("cid", 29)]
        Cid = 43,

        /// <summary>
        /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
        /// </summary>
        [PostgresDbTypeIdentification("regtype", 2206)]
        Regtype = 49,

        /// <summary>
        /// Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
        /// </summary>
        [PostgresDbTypeIdentification("tid", 27)]
        Tid = 53,

        #endregion

        #region Special

        /// <summary>
        /// A special value that can be used to send parameter values to the database without
        /// specifying their type, allowing the database to cast them to another value based on context.
        /// The value will be converted to a string and send as text.
        /// </summary>
        /// <remarks>
        /// This value shouldn't ordinarily be used, and makes sense only when sending a data type
        /// unsupported by Npgsql.
        /// </remarks>
        [PostgresDbTypeIdentification("unknown", 705)]
        Unknown = 40,

        #endregion

        #region Postgis

        /// <summary>
        /// The geometry type for PostgreSQL spatial extension PostGIS.
        /// </summary>
        Geometry = 50     // Extension type

        #endregion
    }
}
