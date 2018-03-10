using IndexSuggestions.DAL.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Postgres
{
    internal abstract class BaseRepository<TKey, TEntity> : IBaseRepository<TKey, TEntity> where TEntity : class
    {

        protected Func<IndexSuggestionsContext> CreateContextFunc { get; private set; }

        public BaseRepository(Func<IndexSuggestionsContext> createContextFunc)
        {
            CreateContextFunc = createContextFunc;
        }

        public void Create(TEntity entity)
        {
            using (var context = CreateContextFunc())
            {
                context.Set<TEntity>().Add(entity);
                context.SaveChanges();
            }
        }

        public TEntity GetByPrimaryKey(TKey id)
        {
            return GetByIdLoadFromDb(id);
        }

        public void Update(TEntity entity)
        {
            using (var context = CreateContextFunc())
            {
                context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
        }

        protected virtual TEntity GetByIdLoadFromDb(TKey id)
        {
            using (var context = CreateContextFunc())
            {
                return context.Set<TEntity>().Find(id);
            }
        }

    }
}
