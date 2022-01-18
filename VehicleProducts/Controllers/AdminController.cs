using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // *** Do not use -> using System.Data.Entity;
using VehicleProducts.Db;
using VehicleProducts.Models;
using VehicleProducts.ViewModels;

namespace VehicleProducts.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment; // wwwroot folder


        public AdminController(ProductDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Add(VehicleViewModel model)
        {
            if (! ModelState.IsValid)
            {
                return await Task.Run(() => View());
            }// end if

            var vehicleModel = new VehicleModel();
            vehicleModel.Title = model.VehicleModel.Title;
            vehicleModel.ProductDescription = model.VehicleModel.ProductDescription;

            //// File path
            var filePath = @"\images";          // Hardcoded file path. Should be coming form db. 
            vehicleModel.FilePath = filePath;

            vehicleModel.ImageName_1 = model.Image_1.FileName; 
            vehicleModel.StoreDate = DateTime.Now;

            //// Uploading an image on the server
            
            UploadFile(model.Image_1);

            await _db.AddAsync(vehicleModel);
            await _db.SaveChangesAsync(); 

            return await Task.Run(() => RedirectToAction(nameof(Index)));
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




        #region Helpers Methods 
        private async void UploadFile (IFormFile file)
        {

            //// Checking "images" directory 
            //if (!Directory.Exists("images"))
            //{
            //    Directory.CreateDirectory(imagePath);
            //}

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileName);

            if ( file != null )
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                }// end using 

            }// end if 


             
        }// end UploadFile()
        #endregion




    }// end class AdminController
}
