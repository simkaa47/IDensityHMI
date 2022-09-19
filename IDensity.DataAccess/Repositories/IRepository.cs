using System.Collections.Generic;

namespace IDensity.DataAccess.Repositories
{
    public interface IRepository<T> where T : class, IDataBased
    {
        T GetById(int id);
        List<T> GetAll();
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);

        void Init(IEnumerable<T> collection);

    }
    
}
