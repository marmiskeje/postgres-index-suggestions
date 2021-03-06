﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Collector.Contracts
{
    public interface ILogProcessingConfiguration
    {
        string Directory { get; }
        string FilenameFilter { get; }
        string Encoding { get; }
        string LogEntryStartingCharacter { get; }
        string LogEntryColumnSeparator { get; }
    }
}
