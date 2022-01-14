using Microsoft.AspNetCore.Mvc;

namespace VehicleProducts.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }// end class AdminController
}
