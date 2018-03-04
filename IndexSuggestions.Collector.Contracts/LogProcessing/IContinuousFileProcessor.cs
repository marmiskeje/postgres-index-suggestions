using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Collector.Contracts
{
    public interface IContinuousFileProcessor : IDisposable
    {
        void ChangeCurrentFile(string file);
    }
}
