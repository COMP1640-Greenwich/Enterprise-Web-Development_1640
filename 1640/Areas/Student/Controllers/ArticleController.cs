using _1640.Data;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ArticleController(ApplicationDbContext dBContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dBContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Create()        
        {
            var model = new Article()
            {
                IsBlogActive = false
            };

            return View(model);

        }
        [HttpPost]
        public IActionResult Create(Article article, IFormFile? file, IFormFile? file1)
        {
            

            if (ModelState.IsValid)
            {
                var isBlogActive = article.IsBlogActive;
                if (isBlogActive == true)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\articles");

                    if (!string.IsNullOrEmpty(article.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, article.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    article.ImageUrl = @"\images\articles\" + fileName;
                }
                if (file1 != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file1.FileName);
                    string docxPath = Path.Combine(wwwRootPath, @"docx");
                    if (!string.IsNullOrEmpty(article.DocxUrl))
                    {
                        var old1ImagePath = Path.Combine(wwwRootPath, article.DocxUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(old1ImagePath))
                        {
                            System.IO.File.Delete(old1ImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(docxPath, fileName), FileMode.Create))
                    {
                        file1.CopyTo(fileStream);
                    }
                    article.DocxUrl = @"\docx\" + fileName;
                }
                else
                {
                    TempData["error"] = "You must insert file doxc.";
                    return View(article);
                }

                
                    _dbContext.Add(article);
                    _dbContext.SaveChanges();
                    TempData["success"] = "Article Created successfully";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["error"] = "You must agree to our Terms and Conditions.";
                    return View(article);
                }
            }

                return View(article); 
            
        }

    }
}
