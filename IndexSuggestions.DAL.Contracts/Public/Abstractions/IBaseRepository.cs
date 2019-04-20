using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface IBaseSimpleRepository<TEntity>
    {
        void Create(TEntity entity);
    }
    public interface IBaseRepository<TKey, TEntity> : IBaseSimpleRepository<TEntity>
    {
        void Update(TEntity entity);
        void Remove(TEntity entity);
        TEntity GetByPrimaryKey(TKey key, bool useCache = false);
    }
}
