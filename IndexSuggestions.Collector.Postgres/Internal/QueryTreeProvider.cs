using IndexSuggestions.Collector.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Linq;
using IndexSuggestions.DBMS.Contracts;

namespace IndexSuggestions.Collector.Postgres
{
    internal class QueryTreeProvider
    {
        private readonly IRepositoriesFactory repositories;
        public QueryTreeProvider(IRepositoriesFactory repositories)
        {
            this.repositories = repositories;
        }
        
        public QueryTreeData Provide(JObject jObject)
        {
            QueryTreeData result = new QueryTreeData();
            result.QueryCommandType = ParseCommandType(jObject);
            var rtes = ParseRTEs(jObject);
            var relations = new List<QueryTreeRelation>();
            foreach (var r in rtes)
            {
                if (r.RteKind == RteKind.Relation && r.RelKind == RelKind.Relation)
                {
                    relations.Add(new QueryTreeRelation() { ID = r.RelId.Value });
                }
            }
            result.Relations.AddRange(relations.Distinct()); // same relation can be here multiple times if the same relation is in from and (in join or subquery)
            result.Predicates.AddRange(ParsePredicates(jObject, rtes));
            return result;
        }

        private QueryCommandType ParseCommandType(JObject jObject)
        {
            var cmdType = jObject.SelectToken("QUERY.commandType");
            return Convert(EnumParsingSupport.ConvertFromNumericOrDefault<CmdType>(cmdType.Value<int>()));
        }

        private List<Rte> ParseRTEs(JObject jObject)
        {
            List<Rte> result = new List<Rte>();
            var rtes = jObject.SelectTokens("..RTE");
            foreach (var rte in rtes)
            {
                var rteKind = EnumParsingSupport.ConvertFromNumericOrDefault<RteKind>(rte.SelectToken("rtekind").Value<int>());
                Rte toAdd = new Rte();
                if (rte.SelectToken("relid") != null)
                {
                    toAdd.RelId = rte.SelectToken("relid").Value<long>();
                    toAdd.RelKind = EnumParsingSupport.ConvertUsingAttributeOrDefault<RelKind, EnumMemberAttribute, string>(rte.SelectToken("relkind").Value<string>(), x => x.Value);
                }
                toAdd.RteKind = rteKind;
                result.Add(toAdd);
            }
            return result;
        }

        private List<QueryTreePredicate> ParsePredicates(JObject jObject, List<Rte> rtes)
        {
            // TODO subqueries je nutne spracovavat samostatne, maju vytvorene Query
            List<QueryTreePredicate> result = new List<QueryTreePredicate>();
            var conditions = jObject.SelectTokens("..OPEXPR");
            // TODO SCALARARRAYOPEXPR for IN clause, NULLTEST for IS NULL
            // TODO const value conversion
            foreach (var condition in conditions)
            {
                long operatorResultTypeId = condition.SelectToken("opresulttype").Value<long>();
                if (operatorResultTypeId != 16) // we want only predicates (pg_type oid = 16)
                {
                    continue;
                }
                QueryTreePredicate predicate = new QueryTreePredicate();
                predicate.OperatorID = condition.SelectToken("opno").Value<long>();
                var constants = condition.SelectTokens("..CONST");
                foreach (var c in constants)
                {
                    QueryTreePredicateOperand cToAdd = new QueryTreePredicateOperand();
                    cToAdd.TypeId = c.SelectToken("consttype").Value<long>();
                    var constValue = c.SelectToken("constvalue").Value<string>();
                    cToAdd.Type = Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(cToAdd.TypeId, x => x.OID));
                    //cToAdd.Value = new ConstantValueConverter().Convert(cToAdd.Type, constValue);
                    predicate.Operands.Add(cToAdd);
                }
                var vars = condition.SelectTokens("..VAR");
                foreach (var v in vars)
                {
                    QueryTreePredicateOperand toAdd = new QueryTreePredicateOperand();
                    var rteIndex = v.SelectToken("varnoold").Value<int>() - 1;
                    toAdd.RelationID = rtes[rteIndex].RelId;
                    toAdd.TypeId = v.SelectToken("vartype").Value<long>();
                    toAdd.Type = Convert(EnumParsingSupport.ConvertUsingAttributeOrDefault<PostgresDbType, PostgresDbTypeIdentificationAttribute, long>(toAdd.TypeId, x => x.OID));
                    var attno = v.SelectToken("varoattno").Value<long>();
                    var attribute = repositories.GetRelationAttributesRepository().Get(toAdd.RelationID.Value, attno);
                    toAdd.AttributeName = attribute.Name;
                    predicate.Operands.Add(toAdd);
                }
                result.Add(predicate);
            }
            return result;
        }
        #region private class Rte
        private class Rte
        {
            public RteKind RteKind { get; set; }

            public RelKind RelKind { get; set; }
            public long? RelId { get; set; }
        }
        #endregion
        #region private QueryCommandType Convert(CmdType source)
        private QueryCommandType Convert(CmdType source)
        {
            switch (source)
            {
                case CmdType.Select:
                    return QueryCommandType.Select;
                case CmdType.Update:
                    return QueryCommandType.Update;
                case CmdType.Insert:
                    return QueryCommandType.Insert;
                case CmdType.Delete:
                    return QueryCommandType.Delete;
                case CmdType.Utility:
                    return QueryCommandType.Utility;
                default:
                    return QueryCommandType.Unknown;
            }
        }
        #endregion

        #region private DbType Convert(PostgresDbType source)
        //http://www.npgsql.org/doc/types/basic.html
        private DbType Convert(PostgresDbType source)
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

        /*nodes.h
     *     CMD_UNKNOWN,
	CMD_SELECT,					// select stmt 
	CMD_UPDATE,					// update stmt 
	CMD_INSERT,					// insert stmt 
	CMD_DELETE,
	CMD_UTILITY,				// cmds like create, destroy, copy, vacuum,
								 * etc. 
	CMD_NOTHING					// dummy command for instead nothing rules
								 * with qual 
     */
        private enum CmdType
        {
            Unknown = 0,
            Select = 1,
            Update = 2,
            Insert = 3,
            Delete = 4,
            Utility = 5,
            Nothing = 6
        }

        /*parsenodes.h
    RTE_RELATION,				// ordinary relation reference 
    RTE_SUBQUERY,				// subquery in FROM 
	RTE_JOIN,					// join 
	RTE_FUNCTION,				// function in FROM 
	RTE_TABLEFUNC,				// TableFunc(.., column list) 
	RTE_VALUES,					// VALUES (<exprlist>), (<exprlist>), ... 
	RTE_CTE,					// common table expr (WITH list element) 
	RTE_NAMEDTUPLESTORE			// tuplestore, e.g. for AFTER triggers 
 */
        private enum RteKind
        {
            Relation = 0,
            Subquery = 1,
            Join = 2,
            Function = 3,
            TableFunc = 4,
            Values = 5,
            CTE = 6,
            NamedTupleStore = 7
        }

        /*pg_class.h
#define RELKIND_RELATION		  'r'	// ordinary table
#define RELKIND_INDEX			  'i'	// secondary index
#define RELKIND_SEQUENCE		  'S'	// sequence object
#define RELKIND_TOASTVALUE	  't'	// for out-of-line values
#define RELKIND_VIEW			  'v'	// view
#define RELKIND_MATVIEW		  'm'	// materialized view
#define RELKIND_COMPOSITE_TYPE  'c'	// composite type
#define RELKIND_FOREIGN_TABLE   'f'	// foreign table
#define RELKIND_PARTITIONED_TABLE 'p' // partitioned table
*/
        private enum RelKind
        {
            Unknown = 0,
            [EnumMember(Value = "r")]
            Relation = 1,
            [EnumMember(Value = "i")]
            Index = 2,
            [EnumMember(Value = "S")]
            Sequence = 3,
            [EnumMember(Value = "t")]
            ToastValue = 4,
            [EnumMember(Value = "v")]
            View = 5,
            [EnumMember(Value = "m")]
            MaterializedView = 6,
            [EnumMember(Value = "c")]
            CompositeType = 7,
            [EnumMember(Value = "f")]
            ForeignTable = 8,
            [EnumMember(Value = "p")]
            PartitionedTable = 9
        }
    }
}
