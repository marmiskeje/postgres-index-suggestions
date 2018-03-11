using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.Cache
{
    public interface ICacheProvider
    {
        ICache MemoryCache { get; }
    }
}
