using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IDatabaseSystemInfoRepository
    {
        IDatabaseSystemInfo LoadInfo();
    }
}
