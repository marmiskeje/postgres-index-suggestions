using IndexSuggestions.Common.Cache;
using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL
{
    internal abstract class BaseRepository<TKey, TEntity> : IBaseRepository<TKey, TEntity> where TEntity : class, IEntity<TKey>
    {
        protected Type ThisType { get; private set; }
        protected ICache Cache
        {
            get { return CacheProvider.Instance.MemoryCache; }
        }
        protected TimeSpan CacheExpiration { get; set; }

        protected Func<IndexSuggestionsContext> CreateContextFunc { get; private set; }

        public BaseRepository(Func<IndexSuggestionsContext> createContextFunc)
        {
            ThisType = GetType();
            CreateContextFunc = createContextFunc;
            CacheExpiration = TimeSpan.FromMinutes(1);
        }

        public void Create(TEntity entity)
        {
            FillEntitySet(entity);
            using (var context = CreateContextFunc())
            {
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
            }
        }

        public TEntity GetByPrimaryKey(TKey id, bool useCache = false)
        {
            return Get(() => GetByIdLoadFromDb(id), id.ToString(), useCache);
        }

        protected virtual TEntity Get(Func<TEntity> loadFromDbFunc, string cacheKey, bool useCache = false)
        {
            var cacheKeyToUse = CreateCacheKeyForThisType(cacheKey);
            if (useCache)
            {
                TEntity value;
                if (!Cache.TryGetValue(cacheKeyToUse, out value))
                {
                    value = loadFromDbFunc();
                    if (value != null)
                    {
                        FillEntityGet(value);
                        Cache.Save(cacheKeyToUse, value, CacheExpiration);
                    }
                }
                return value;
            }
            return loadFromDbFunc();
        }

        protected virtual void FillEntityGet(TEntity entity)
        {

        }

        protected virtual void FillEntitySet(TEntity entity)
        {

        }

        public void Update(TEntity entity)
        {
            FillEntitySet(entity);
            using (var context = CreateContextFunc())
            {
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                var cacheKeys = GetAllCacheKeys(entity.ID, entity);
                foreach (var cacheKey in cacheKeys)
                {
                    string cacheKeyToUse = CreateCacheKeyForThisType(cacheKey);
                    TEntity value;
                    if (Cache.TryGetValue(cacheKeyToUse, out value))
                    {
                        Cache.Save(cacheKeyToUse, entity, CacheExpiration);
                    }
                }
            }
        }

        protected virtual TEntity GetByIdLoadFromDb(TKey id)
        {
            using (var context = CreateContextFunc())
            {
                return context.Set<TEntity>().Find(id);
            }
        }

        protected string CreateCacheKeyForThisType(string uniqueKey)
        {
            return String.Format("{0}:{1}_{2}", ThisType.Assembly.FullName, ThisType.FullName, uniqueKey);
        }

        protected virtual ISet<string> GetAllCacheKeys(TKey key, TEntity entity)
        {
            return new HashSet<string>(new string[] { key.ToString() });
        }
    }
}
