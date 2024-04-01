using _1640.Areas.Repository.IRepository;
using _1640.Models;
using _1640.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<HomeController> _logger;
		private readonly int _recordsPerPage = 4;

		public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index(int id, string searchString = "")
        {
            List<Article> articles = _unitOfWork.ArticleRepository.GetAllApprove("Semester")
                .Where(b => b.Title.Contains(searchString))
                .ToList();

			int numberOfRecords = articles.Count();
			int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _recordsPerPage);

			ViewBag.numberOfPages = numberOfPages;
			ViewBag.currentPage = id;
			ViewData["Current Filter"] = searchString;

			var articlesList = articles.Skip(id * numberOfPages).Take(_recordsPerPage).ToList();
			return View(articlesList);
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
        public IActionResult Login()
        {
            return Redirect("Identity/Account/Login");
        }
    }
}
