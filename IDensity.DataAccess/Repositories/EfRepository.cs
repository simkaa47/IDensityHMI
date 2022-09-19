using IDensity.DataAccess.Extentions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace IDensity.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class, IDataBased
    { 
        public void Add(T entity)
        {
            using(var dbContext = new ApplicationContext())
            {
                dbContext.Set<T>().Add(entity);
            }
        }

        public void Delete(T entity)
        {
            using (var dbContext = new ApplicationContext())
            {
                T entityFromDb = dbContext.Set<T>().Find(entity.Id);
                if (entityFromDb != null)
                {
                    dbContext.Set<T>().Remove(entityFromDb);
                    dbContext.SaveChanges();
                }
            }            
        }

        public List<T> GetAll()
        {
            using (var dbContext = new ApplicationContext())
            {
                return dbContext.Set<T>().ToList();
            }
            
        }

        public T GetById(int id)
        {
            using (var dbContext = new ApplicationContext())
            {
                return dbContext.Set<T>()
                 .Where(e => e.Id == id)
                 .FirstOrDefault();
            }
            
        }

        public void Init(IEnumerable<T> collection)
        {
            using (var dbContext = new ApplicationContext())
            {
                dbContext.Init(collection);
            }
        }

        public void Update(T entity)
        {
            using (var dbContext = new ApplicationContext())
            {
                dbContext.Entry(entity).State = EntityState.Modified;
                dbContext.SaveChangesAsync();
            }
        }
    }
}
