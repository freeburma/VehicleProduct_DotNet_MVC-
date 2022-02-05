using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using VehicleProducts.Db;
using VehicleProducts.Models;

/// <summary>
/// Creating a database service class for xUnit Testing in Memory database. 
/// *** This is the recommand way of testing application. Read the article at the following link. 
/// https://docs.microsoft.com/en-us/aspnet/core/test/razor-pages-tests?view=aspnetcore-6.0
/// 
/// Required Package: You can add a as one of the following commands 
/// PM > Install-Package Microsoft.EntityFrameworkCore.InMemory
/// or 
/// $ dotnet add package Microsoft.EntityFrameworkCore.InMemory
/// </summary>
namespace VehicleProducts_xUnitTests.Services
{
    public static class DatabaseService
    {
        public static DbContextOptions<ProductDbContext> TestDbContextOptions()
        {
            //// Create a new service provider to create a new in-memory database.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase() 
                .BuildServiceProvider();

            // Create a new options instance using an in-memory database and 
            // IServiceProvider that the context should resolve all of its 
            // services from.
            var builder = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);



            return builder.Options; 

        }// end TestDbContextOptions()


        public static Mock<ProductDbContext> MockTestDbContextOptions()
        {
            var optionBuilder = new DbContextOptionsBuilder<ProductDbContext>()
                                    .UseInMemoryDatabase("InMmeoroyDb");

            var mockDbContext = new Mock<ProductDbContext>(optionBuilder.Options);


            return mockDbContext;

        }// end MockTestDbContextOptions()

        //// Creatting new list
        public static List<VehicleModel> DummyMemoryTestList = new List<VehicleModel>()
        {
            new VehicleModel() { Title = "Test 1", ProductDescription = "Test 1 Description"},
            new VehicleModel() { Title = "Test 2", ProductDescription = "Test 2 Description"},
            new VehicleModel() { Title = "Test 3", ProductDescription = "Test 3 Description"},
            new VehicleModel() { Title = "Test 4", ProductDescription = "Test 4 Description"},
            new VehicleModel() { Title = "Test 5", ProductDescription = "Test 5 Description"},
        };

    }// end class DatabaseService
}
