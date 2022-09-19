using Microsoft.EntityFrameworkCore;

namespace IDensity.DataAccess
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                //.UseLazyLoadingProxies()
                .UseSqlite("Data Source=settings.db");

        }
    }
}
