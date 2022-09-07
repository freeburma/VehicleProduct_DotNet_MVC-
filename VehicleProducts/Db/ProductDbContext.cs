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

        public DbSet<VehicleModel> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<VehicleModel>()
            //    .HasData(
            //                new VehicleModel() { Title = "Test 1", ProductDescription = "Test 1 Description" },
            //                new VehicleModel() { Title = "Test 2", ProductDescription = "Test 2 Description" },
            //                new VehicleModel() { Title = "Test 3", ProductDescription = "Test 3 Description" },
            //                new VehicleModel() { Title = "Test 4", ProductDescription = "Test 4 Description" },
            //                new VehicleModel() { Title = "Test 5", ProductDescription = "Test 5 Description" }
            //            )
            //    ;

        }

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
