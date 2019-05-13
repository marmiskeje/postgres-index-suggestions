using DiplomaThesis.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiplomaThesis.DAL
{
    internal class SettingPropertiesRepository : BaseRepository<long, SettingProperty>, ISettingPropertiesRepository
    {
        public SettingPropertiesRepository(Func<IndexSuggestionsContext> createContextFunc) : base(createContextFunc)
        {

        }

        public SettingProperty Get(string key, bool useCache = false)
        {
            SettingProperty LoadDataFromDb()
            {
                using (var context = CreateContextFunc())
                {
                    return context.SettingProperties.Where(x => x.Key == key).SingleOrDefault();
                }
            }
            if (useCache)
            {
                SettingProperty result = null;
                var cacheKeyToUse = CreateCacheKeyForThisType(key);
                if (!Cache.TryGetValue(cacheKeyToUse, out result))
                {
                    result = LoadDataFromDb();
                    if (result != null)
                    {
                        Cache.Save(cacheKeyToUse, result, CacheExpiration);
                    }
                }
                return result;
            }
            return LoadDataFromDb();
        }

        public TObject GetObject<TObject>(string key, bool useCache = false) where TObject : class
        {
            TObject LoadData()
            {
                TObject result = default(TObject);
                var setting = Get(key, useCache);
                if (setting != null && !String.IsNullOrEmpty(setting.StrValue))
                {
                    result = JsonSerializationUtility.Deserialize<TObject>(setting.StrValue);
                }
                return result;
            }
            if (useCache)
            {
                string cacheKeyToUse = CreateCacheKeyForThisType(key + "_object");
                TObject result = default(TObject);
                if (!Cache.TryGetValue(cacheKeyToUse, out result))
                {
                    result = LoadData();
                    if (result != null)
                    {
                        Cache.Save(cacheKeyToUse, result, CacheExpiration);
                    }
                }
                return result;
            }
            return LoadData();
        }

        public void SetObject<TObject>(string key, TObject data) where TObject : class
        {
            using (var context = CreateContextFunc())
            {
                string strData = null;
                if (data != null)
                {
                    strData = JsonSerializationUtility.Serialize(data);
                }
                lock (String.Intern(key))
                {
                    var entity = context.SettingProperties.Where(x => x.Key == key).SingleOrDefault();
                    if (entity != null)
                    {
                        entity.StrValue = strData;
                        Update(entity);
                    }
                    else
                    {
                        entity = new SettingProperty() { Key = key, StrValue = strData };
                        Create(entity);
                    }
                    Cache.Remove(CreateCacheKeyForThisType(key));
                    Cache.Remove(CreateCacheKeyForThisType(key + "_object"));
                }
            }
        }
    }
}
