using Microsoft.AspNetCore.Mvc;

namespace _1640.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
