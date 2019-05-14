using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IToSqlValueStringConverter
    {
        string Convert(DbType dbType, object value);
        string ConvertStringRepresentation(DbType dbType, string value);
    }
}
