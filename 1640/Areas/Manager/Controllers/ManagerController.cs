using Microsoft.AspNetCore.Mvc;

namespace _1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
