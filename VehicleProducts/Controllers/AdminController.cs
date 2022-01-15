using Microsoft.AspNetCore.Mvc;
using VehicleProducts.Db;
using VehicleProducts.Models;

namespace VehicleProducts.Controllers
{
    public class AdminController : Controller
    {

        private readonly ProductDbContext _db;

        public AdminController(ProductDbContext db)
        {
            _db = db;

        }// end AdminController()

        public IActionResult Index()
        {
            var vehicleList = _db.Vehicles.ToList(); 

            //if (vehicleList.Count <= 0)
            //{
            //    var vehicle = new VehicleModel()
            //    {
            //        Title = "Test",
            //        FilePath = "Test file path",
            //        ImageName_1 = "Test image name",
            //        ProductDescription = "Test Product Description",
            //        StoreDate = DateTime.Now,
            //    }; 

            //    _db.AddAsync(vehicle);
            //    _db.SaveChanges();  


            //}// end if 

           


            return View(vehicleList);
        }



    }// end class AdminController
}
