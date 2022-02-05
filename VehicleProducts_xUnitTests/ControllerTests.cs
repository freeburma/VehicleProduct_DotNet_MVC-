using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using VehicleProducts.Controllers;
using VehicleProducts.Db;
using VehicleProducts.Models;
using VehicleProducts_xUnitTests.Services;
using Xunit;


#nullable disable

namespace VehicleProducts_xUnitTests
{
    public class ControllerTests
    {
        /// <summary>
        /// We need to create the Mock dependencies for controller. Read the StackOverflow article at the following link. 
        /// Ref: https://stackoverflow.com/questions/43424095/how-to-unit-test-with-ilogger-in-asp-net-core
        /// </summary>


        [Fact]
        public async void TestController_Privacy_Test()
        {

            using (var db = new ProductDbContext(DatabaseService.TestDbContextOptions()))
            {
                //// Arrange 
                await db.Vehicles.AddRangeAsync(DatabaseService.DummyMemoryTestList);
                await db.SaveChangesAsync();

                var mockILogger = Mock.Of<ILogger<HomeController>>();
                var privacyController = new HomeController(mockILogger, db);

                //// Act 
                var result = privacyController.Privacy();
                var getActionName = Assert.IsType<ViewResult>(result);

                //// Assert 
                Assert.Equal("Privacy", getActionName.ViewName);

            }// end using 




        }// end TestController_Index_Test()

        [Fact]
        public async void HomeController_Index_Test()
        {

            using (var db = new ProductDbContext(DatabaseService.TestDbContextOptions()))
            {
                //// Arrange 
                var mockILogger = Mock.Of<ILogger<HomeController>>();


                await db.Vehicles.AddRangeAsync(DatabaseService.DummyMemoryTestList);
                await db.SaveChangesAsync();

                var homeController = new HomeController(mockILogger, db); // *** We need an empty constructor. We don't want to mess up with iLogger and DbContext. 

                //// Act
                var result = await homeController.Index();


                //// Assert 
                var viewResult = Assert.IsType<ViewResult>(result);
                var vehicleList = Assert.IsAssignableFrom<IEnumerable<VehicleModel>>(viewResult.ViewData.Model);
                Assert.Equal(5, vehicleList.Count());

            }// end using 


        }// end HomeController_Index_Test()


    }// end class ControllerTests
}
