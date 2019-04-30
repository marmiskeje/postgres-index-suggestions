using DiplomaThesis.DBMS.Contracts;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DiplomaThesis.DBMS.Postgres
{
    internal class VirtualHPartitioningsRepository : BaseRepository, IVirtualHPartitioningsRepository
    {
        public VirtualHPartitioningsRepository(string connectionString) : base(connectionString)
        {

        }

        public IVirtualHPartitioning Create(VirtualHPartitioningDefinition definition)
        {
            string partitioningQuery = "SELECT hypopg_partition_table(@RelationName, @PartitioningStatement)";
            var param = new
            {
                RelationName = definition.RelationName,
                PartitioningStatement = definition.PartitioningStatement
            };
            Execute(partitioningQuery, param);
            int counter = 1;
            foreach (var partitionStatement in definition.PartitionStatements)
            {
                string query = "SELECT * FROM hypopg_add_partition(@Name, @PartitionStatement)";
                string partitionName = $"hypo_partition_{definition.RelationName}_{counter})";
                var param2 = new
                {
                    Name = partitionName,
                    PartitionStatement = partitionStatement
                };
                Execute(query, param2);
                counter++;
            }
            return null;
        }

        public void DestroyAll()
        {
            string query = "select * from hypopg_reset_table()";
            Execute(query, null);
        }

        public void InitializeEnvironment()
        {
            Execute("CREATE EXTENSION IF NOT EXISTS hypopg", null);
        }
    }
}
