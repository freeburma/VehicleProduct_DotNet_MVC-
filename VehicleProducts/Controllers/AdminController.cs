using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // *** Do not use -> using System.Data.Entity;
using System.Net.Mime;
using VehicleProducts.Db;
using VehicleProducts.Models;
using VehicleProducts.ViewModels;

namespace VehicleProducts.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProductDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment; // wwwroot folder


        public AdminController(ProductDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }// end AdminController()

        //private VehicleModel VehicleModel { get; set; }

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

        //public async Task<IActionResult> Detail(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }// end if 

        //    VehicleViewModel vehicleViewModel = new VehicleViewModel();

        //    var vehicleModel = await _db.Vehicles.FindAsync(id);
        //    vehicleViewModel.VehicleModel = vehicleModel;

        //    //return await Task.Run(() => View(vehicleViewModel));
        //    return View(vehicleViewModel);

        //}// end Detail()

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
            var filePath = @"images";          // Hardcoded file path. Should be coming form db. 
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

            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }// end if 

            VehicleViewModel vehicleViewModel = new VehicleViewModel();

            var vehicleModel = await _db.Vehicles.FindAsync(id);
            vehicleViewModel.VehicleModel = vehicleModel;

            //return await Task.Run(() => View(vehicleViewModel));
            return View(vehicleViewModel);

        }// end Edit() -> HTTP_GET


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, VehicleViewModel model)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }// end if 


            if (ModelState.IsValid )
            {
                var vehicleModel = await _db.Vehicles.FindAsync(id);

                vehicleModel.Title = model.VehicleModel.Title;  
                vehicleModel.ProductDescription = model.VehicleModel.ProductDescription;

                //// TODO: File Upload is not working yet. 
                /// *** We can't set file from Controller to View due to the security reasons. 
                /// *** Never tried to set file through Model. 
                /// We assume that if the file name is equal, it is the same image. We won't be uploading the image to save CPU resources. 
                if (model.Image_1 != null)
                {
                    //// File path
                    var filePath = @"images";          // Hardcoded file path. Should be coming form dbin the future. 
                    
                    //// Remove the old image
                    DeleteFile(filePath, vehicleModel.ImageName_1);

                    //// Updating file info in db 
                    vehicleModel.FilePath = filePath;
                    vehicleModel.ImageName_1 = model.Image_1.FileName;
                    vehicleModel.StoreDate = DateTime.Now;

                    //// Uploading an image on the server            
                    UploadFile(model.Image_1);

                }// end if 


                //// Saving it to db
                _db.Update(vehicleModel);
                await _db.SaveChangesAsync();


            }// end if 

            return RedirectToAction(nameof(Index));

        }// end Edit() -> HTTP_POST

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index)); 
            }

            var vehicleModel = await _db.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
            
            if (vehicleModel == null)
            {
                return RedirectToAction(nameof(Index));
            }


            VehicleViewModel vehicleViewModel = new VehicleViewModel()
            {
                VehicleModel = vehicleModel
            }; 


            return await Task.Run(() => View(vehicleViewModel));

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

            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var vehicleModel = await _db.Vehicles.FindAsync(id);

            //// DON'T FORGET TO DELETE THE IMAGES
            DeleteFile(vehicleModel.FilePath, vehicleModel.ImageName_1);

            _db.Vehicles.Remove(vehicleModel);
            await _db.SaveChangesAsync();
            


            return RedirectToAction(nameof(Index));


        }// end Delete() -> HTTP_POST




        #region Helpers Methods 
        private async void UploadFile (IFormFile file, string directoryPath=@"images")
        {

            var fileName = Path.GetFileName(file.FileName);
            //var filePath = Path.Combine(Path.GetDirectoryName(@"wwwroot/images"),fileName);
            var filePath = _webHostEnvironment.WebRootPath + "\\" + directoryPath;

            //// Checking "images" directory 
            if ( ! Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }// end if 

            filePath = filePath + "\\" + fileName;

            //// Creating file 
            if ( file != null )
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                }// end using 

            }// end if 

             
        }// end UploadFile()


        private FormFile GetFile(string directoryPath, string fileName)
        {

            var filePath = _webHostEnvironment.WebRootPath + "\\" + directoryPath + "\\" + fileName;

            ContentDisposition contentDisposition = new ContentDisposition()
            {
                FileName = fileName,
                Inline = true
            };

            var getFileType = fileName.Split(".");

            var ct = new ContentType()
            {
                MediaType = "image/" + getFileType[getFileType.Length - 1]
            };


            if (System.IO.File.Exists(filePath))
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    var file = new FormFile(stream, 0, stream.Length, fileName, Path.GetFileName(fileName))
                    {
                        Headers = new HeaderDictionary(),

                        ContentDisposition = contentDisposition.ToString(),
                        ContentType = "image/" + getFileType[getFileType.Length - 1],
                    };


                    //file.ContentDisposition = contentDisposition.ToString();
                    //file.ContentType = ct.ToString(); 

                    return file;
                }// end using 
                
            }
            else
            {
                return null; 
            }// end if 

        }// end UploadFile()

        private void DeleteFile(string directoryPath, string fileName)
        {
            string fullPath = _webHostEnvironment.WebRootPath + "\\" + directoryPath + "\\" + fileName; 

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }// end if 
        }// end DeleteFile()


        #endregion




    }// end class AdminController
}
