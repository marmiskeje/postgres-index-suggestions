using System;
using System.Collections.Generic;
using System.Text;

namespace IndexSuggestions.DAL.Contracts
{
    public interface IBaseRepository<TKey, TEntity>
    {
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        TEntity GetByPrimaryKey(TKey key, bool useCache = false);
    }
}
