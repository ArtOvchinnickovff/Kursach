using Microsoft.EntityFrameworkCore;
using RealEstateMarketAnalysis.Models;

namespace RealEstateMarketAnalysis.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users => Set<UserModel>();
        public DbSet<CianListing> CianListings { get; set; }
    }
}
