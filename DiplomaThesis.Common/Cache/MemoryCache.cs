using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiplomaThesis.Common.Cache
{
    internal class MemoryCache : ICache
    {
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache cache = null;

        public MemoryCache()
        {
            cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
        }

        public void Save<T>(string key, T value, TimeSpan expiration)
        {
            SaveToCachePrivate(key, value, expiration);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            object result = null;
            if (cache.TryGetValue(key, out result) && result is T)
            {
                value = (T)result;
                return true;
            }
            return false;
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        private void SaveToCachePrivate<T>(String key, T item, TimeSpan expiration)
        {
            cache.Set<T>(key, item, expiration);
        }
    }
}
