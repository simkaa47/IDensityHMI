using IDensity.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace IDensity.DataAccess
{
    public class ApplicationContext : DbContext
    {
        public DbSet<MeasUnit> MeasUnits => Set<MeasUnit>();

        public ApplicationContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
               // .UseLazyLoadingProxies()
                .UseSqlite("Data Source=application.db");
        }
    }
}
