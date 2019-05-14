using DiplomaThesis.DBMS.Contracts;
using DiplomaThesis.DBMS.Postgres;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Tests.Scenarios
{
    public class ScenariosStatements
    {
        private DatabaseScope CreateDatabaseScope()
        {
            return new DatabaseScope("tpcc");
        }

        private IRawSqlExecutionRepository Repository
        {
            get { return RepositoriesFactory.Instance.GetRawSqlExecutionRepository(); }
        }

        [Test]
        public void Scenario_Base1()
        {
            string query = "select * from public.customer where c_balance > 0";
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base2()
        {
            string query = "select * from public.orders where o_created_date >= '2019-05-14' and o_created_date < '2019-05-15'";
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base3()
        {
            string query = "select * from public.orders where o_state_id = 0";
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base4()
        {
            string query = "select * from public.history where h_w_id in (1,2,3) and h_d_id = 1";
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
    }
}
