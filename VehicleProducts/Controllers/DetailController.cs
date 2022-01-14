using Microsoft.AspNetCore.Mvc;

namespace VehicleProducts.Controllers
{
    public class DetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
