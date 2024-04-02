using _1640.Areas.Repository.IRepository;
using _1640.Models;
using _1640.Repository.IRepository;
using _1640.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _1640.Areas.Student.Controllers
{
    [Area("Student")]
	public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, SD.Role_Manager))
                {
                    return RedirectToAction("List", "Manager", new { area = "Manager" });
                }
                else if (await _userManager.IsInRoleAsync(user, SD.Role_Student))
                {
                    // Cast the user to User to access the FacultyId property
                    var student = user as User;
                    if (student != null)
                    {
                        // Get the articles that have the same FacultyId as the student
                        var articles = _unitOfWork.ArticleRepository.GetAll(a => a.FacultyId == student.FacultyId && a.Status == Article.StatusArticle.Approve).ToList();
                        return View(articles);
                    }
                }
            }
            // If the user is not logged in or is not a manager or a student, return all approved articles
            var allArticles = _unitOfWork.ArticleRepository.GetAllApprove().ToList();
            return View(allArticles);
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
