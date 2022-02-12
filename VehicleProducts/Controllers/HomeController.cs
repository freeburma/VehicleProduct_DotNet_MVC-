using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System.Diagnostics;
using VehicleProducts.Db;
using VehicleProducts.Models;

namespace VehicleProducts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductDbContext _db;

       

        public HomeController(ILogger<HomeController> logger, ProductDbContext db)
        {
            _logger = logger;
            _db = db;   
        }

        public async Task<IActionResult> Index()
        {
            List<VehicleModel> vehicleList = await _db.Vehicles.ToListAsync();


            return View(vehicleList);
        }

        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> Detail(int? id)
        {
            var vehicle = await _db.Vehicles.FindAsync(id);


            return View(vehicle);
        }

        public IActionResult Privacy()
        {
            return View(nameof(Privacy));
        }

      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}