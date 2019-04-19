using IndexSuggestions.DBMS.Contracts;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class VirtualIndicesRepository : BaseRepository, IVirtualIndicesRepository
    {
        public VirtualIndicesRepository(string connectionString) : base("Server=127.0.0.1;Port=6633;Database=test;User Id=postgres;Password = root;Application Name=IndexSuggestions;") // temporary to virtual machine
        {

        }
        public IVirtualIndex Create(VirtualIndexDefinition definition)
        {
            string query = "SELECT * FROM hypopg_create_index(@CreateStatement)";
            var param = new
            {
                CreateStatement = definition.CreateStatement
            };
            return ExecuteQuery<VirtualIndex>(query, param).Single();
        }

        public void DestroyAll()
        {
            string query = "select * from hypopg_reset()";
            Execute(query, null);
        }

        public long GetVirtualIndexSize(uint indexID)
        {
            var query = "select * from hypopg_relation_size(@IndexID)";
            var param = new
            {
                IndexID = indexID
            };
            return ExecuteQuery<VirtualIndexSize>(query, param).Single().Bytes;
        }

        public void InitializeEnvironment()
        {
            Execute("CREATE EXTENSION IF NOT EXISTS hypopg", null);
        }
    }
}
