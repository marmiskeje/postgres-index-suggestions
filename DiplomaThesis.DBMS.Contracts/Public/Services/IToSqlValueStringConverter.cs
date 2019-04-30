using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IToSqlValueStringConverter
    {
        string Convert(DbType dbType, object value);
    }
}
