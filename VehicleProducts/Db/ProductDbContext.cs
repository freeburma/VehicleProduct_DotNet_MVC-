using Microsoft.EntityFrameworkCore;
using VehicleProducts.Models;

namespace VehicleProducts.Db
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }

        public DbSet<VehicleModel> Vehicles { get; set; } = null !; 


    }// end class DbContext
}
