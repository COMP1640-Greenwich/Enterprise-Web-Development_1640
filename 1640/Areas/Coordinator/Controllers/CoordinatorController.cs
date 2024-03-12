using _1640.Data;
using Microsoft.AspNetCore.Mvc;

namespace _1640.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _db;
    public CoordinatorController(ApplicationDbContext db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        
      
    }
}
