using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.Common.Cache
{
    public interface ICache
    {
        void Save<T>(String key, T value, TimeSpan expiration);

        bool TryGetValue<T>(String key, out T value);
        void Remove(String key);
    }
}
