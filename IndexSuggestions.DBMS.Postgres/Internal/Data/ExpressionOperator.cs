using IndexSuggestions.DBMS.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IndexSuggestions.DBMS.Postgres
{
    internal class ExpressionOperator : IExpressionOperator
    {
        [Column("op_id")]
        public uint ID { get; set; }
        [Column("op_name")]
        public string Name { get; set; }
    }
}
