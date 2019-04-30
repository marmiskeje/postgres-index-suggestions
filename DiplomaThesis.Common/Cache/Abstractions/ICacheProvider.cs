using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.Cache
{
    public interface ICacheProvider
    {
        ICache MemoryCache { get; }
    }
}
