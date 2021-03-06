﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.DBMS.Contracts
{
    public interface IStoredProcedure : IDatabaseDependencyObject
    {
        uint ID { get; }
        uint DatabaseID { get; }
        string Name { get; }
        uint SchemaID { get; }
        string SchemaName { get; }
        string LanguageName { get; }
        int ArgumentsCount { get; }
        string Definition { get; }
    }
}
