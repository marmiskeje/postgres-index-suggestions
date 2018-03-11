﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface INormalizedStatementsRepository : IBaseRepository<long, NormalizedStatement>
    {
        NormalizedStatement GetByStatement(string statement, bool useCache = false);
    }
}
