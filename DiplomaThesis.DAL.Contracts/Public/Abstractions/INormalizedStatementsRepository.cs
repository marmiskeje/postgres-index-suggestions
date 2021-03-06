﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DAL.Contracts
{
    public interface INormalizedStatementsRepository : IBaseRepository<long, NormalizedStatement>
    {
        NormalizedStatement GetByStatementFingerprint(string fingerprint, bool useCache = false);
    }
}
