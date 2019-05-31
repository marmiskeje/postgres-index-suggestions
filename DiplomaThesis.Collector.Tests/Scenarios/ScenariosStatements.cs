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
        private DatabaseScope CreateDatabaseScope(string databaseName = null)
        {
            return new DatabaseScope(databaseName ?? "tpcc");
        }

        private IRawSqlExecutionRepository Repository
        {
            get { return RepositoriesFactory.Instance.GetRawSqlExecutionRepository(); }
        }

        public void ExecuteQuery(string query)
        {
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }

        [Test]
        public void Scenario_Base1()
        {
            string query = "select * from public.customer where c_balance > 0";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base2()
        {
            string query = "select * from public.orders where o_created_date >= '2019-05-14' and o_created_date < '2019-05-15'";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base3()
        {
            string query = "select * from public.orders where o_state_id = 0";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base4()
        {
            string query = "select * from public.history where h_w_id in (1,2,3) and h_d_id = 1";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base5()
        {
            string query = "select c_first, c_last from public.customer where c_balance > 0";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base6()
        {
            string query = "select max(c_credit) from public.customer";
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base7()
        {
            string query = "delete from public.history where h_date >= '2018-1-1' and h_date < '2018-2-1'";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base8()
        {
            string query = "select * from vw_customers_history where h_date < '2018-1-1'";
            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base8Inline()
        {
            string query = "select distinct c.c_first, c.c_last, h.h_date, h.h_amount from public.history h inner join public.customer c on h.h_c_id = c.c_id and h.h_c_d_id = c.c_id and h.h_c_w_id = c.c_w_id where h.h_date < '2018-1-1'";

            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
        [Test]
        public void Scenario_Base8InlineNoJoin()
        {
            string query = "select h.h_date, h.h_amount from public.history h where h.h_date < '2018-1-1'";

            Console.WriteLine(query);
            using (var scope = CreateDatabaseScope())
            {
                var result = Repository.ExecuteQuery<dynamic>(query);
            }
        }
    }
}
