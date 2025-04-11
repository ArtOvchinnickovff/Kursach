using Microsoft.EntityFrameworkCore;

namespace RealEstateMarketAnalysis.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users => Set<UserModel>();
    }
}
