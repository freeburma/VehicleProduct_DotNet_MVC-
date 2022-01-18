using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // *** Do not use -> using System.Data.Entity;
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

        private VehicleModel VehicleModel { get; set; }

        public async Task<IActionResult> Index()
        {
            List<VehicleModel> vehicleList = await _db.Vehicles.ToListAsync();

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

        public async Task<IActionResult> Detail(int? id)
        {
            return await Task.Run(() => View());

        }// end Detail()

        public async Task<IActionResult> Add()
        {
            return await Task.Run(() => View());

        }// end Add()

        [HttpPost]
        [ValidateAntiForgeryToken, ActionName("Add")]
        public async Task<IActionResult> Add(VehicleModel model)
        {

            return await Task.Run(() => View());
        }

        public async Task<IActionResult> Edit(int? id)
        {

            return await Task.Run(() => View());

        }// end Edit() -> HTTP_GET


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, VehicleModel model)
        {
            return await Task.Run(() => View());

        }// end Edit() -> HTTP_POST

        public async Task<IActionResult> Delete(int? id)
        {

            return await Task.Run(() => View());

        }// end Delete() -> HTTP_GET

        /// <summary>
        /// Delete 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            return await Task.Run(() => View());

        }// end Delete() -> HTTP_POST







    }// end class AdminController
}
