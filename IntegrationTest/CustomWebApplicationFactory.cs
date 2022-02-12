
using IntegrationTest.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VehicleProducts;
using VehicleProducts.Db;

namespace IntegrationTest
{
    #nullable disable
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProductDbContext>)); 

                services.Remove(descriptor);

                //// Adding Dependency Injection and Injection InMemory Db
                //services.AddDbContext<ProductDbContext>(options =>
                services.AddDbContextPool<ProductDbContext>(options =>
                {
                    options.UseInMemoryDatabase("VehcielMemobryDb"); 
                });

                
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider; 

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<Program>>>();


                    var db = scopedServices.GetRequiredService<ProductDbContext>();


                    try
                    {
                        db.Database.EnsureCreated();

                        //// Initialize the data
                        db.AddRange(DatabaseUtilitiesService.DummyMemoryTestList);
                        db.SaveChanges();


                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " + "database with test messages. Error: {Message}", ex.Message);
                    }// end try 



                }// end using 

            });// end builder.ConfigureServices()
        
        }// end ConfigureWebHost()

    }// end class CustomWebApplicationFactory
}
