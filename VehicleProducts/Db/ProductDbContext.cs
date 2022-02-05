using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleProducts.Models;

namespace VehicleProducts.Db
{
    public class ProductDbContext : IdentityDbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }

        public DbSet<VehicleModel> Vehicles { get; set; } = null !;

        #region DB CRUD operations 

        public async virtual Task AddProductAsync(VehicleModel model)
        {
            await Vehicles.AddAsync(model);
            await SaveChangesAsync();
        }// end AddProductAsync()

        public async virtual Task UpdateProductAsync(VehicleModel model)
        {
            Vehicles.Update(model);
            await SaveChangesAsync();
        }// end UpdateProductAsync()

        public async virtual Task DeleteProductAsync(VehicleModel model)
        {
            Vehicles.Remove(model);
            await SaveChangesAsync();
        }// end DeleteProductAsync()

        #endregion


    }// end class DbContext
}
