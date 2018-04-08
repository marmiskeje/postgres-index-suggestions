using IndexSuggestions.DBMS.Contracts;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class RelationAttributesRepository : IRelationAttributesRepository
    {
        protected String ConnectionString { get; private set; }
        public RelationAttributesRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public RelationAttribute Get(long relationID, long position)
        {
            // TODO - cache all and use cache for data retrieval
            RelationAttribute result = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT attrelid, attname, atttypid, attnum FROM pg_catalog.pg_attribute WHERE attrelid = @relationID and attnum = @position";
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new NpgsqlParameter("@relationID", relationID));
                    command.Parameters.Add(new NpgsqlParameter("@position", position));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new RelationAttribute();
                            result.Name = SqlReaderSupport.GetValueOrDefault<string>(reader["attname"]);
                            result.Position = position;
                            result.RelationID = relationID;
                        }
                    }
                }
            }
            return result;
        }
    }
}
