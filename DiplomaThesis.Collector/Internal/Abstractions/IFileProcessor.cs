using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiplomaThesis.Collector
{
    internal interface IFileProcessor
    {
        void ProcessFile(FileInfo file);
    }
}
