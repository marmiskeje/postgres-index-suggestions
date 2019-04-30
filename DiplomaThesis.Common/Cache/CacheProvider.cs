using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private static readonly Lazy<ICacheProvider> instance = new Lazy<ICacheProvider>(() => new CacheProvider()); // default thread-safe
        private readonly ICache memoryCache = new MemoryCache();

        public static ICacheProvider Instance { get { return instance.Value; } }

        private CacheProvider()
        {

        }

        public ICache MemoryCache
        {
            get { return memoryCache; }
        }
    }
}
