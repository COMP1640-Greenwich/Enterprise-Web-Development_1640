using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(IUnitOfWork unitOfWork,ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Articles.Where(x => x.Status == Article.StatusArticle.Approve).ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
