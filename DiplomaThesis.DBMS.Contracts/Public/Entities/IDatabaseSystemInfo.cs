using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IDatabaseSystemInfo
    {
        string VersionNumberString { get; }
        bool SupportsIncludeInIndices { get; }
    }
}
