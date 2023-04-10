using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IDensity.DataAccess.Repositories
{
    public interface IRepository<T> where T : class, IDataBased
    {
        T GetById(int id);
        List<T> GetAll();
        List<T> GetWhere(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);

        IEnumerable<T> Init(IEnumerable<T> collection);

    }
    
}
