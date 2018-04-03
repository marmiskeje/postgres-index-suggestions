using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IndexSuggestions.Collector
{
    internal interface IFileProcessor
    {
        void ProcessFile(FileInfo file);
    }
}
