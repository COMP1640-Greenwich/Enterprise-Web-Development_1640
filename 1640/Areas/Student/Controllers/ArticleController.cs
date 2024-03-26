using _1640.Data;
using _1640.Models;
using _1640.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        private readonly IWebHostEnvironment _webHostEnvironment;
     
        public ArticleController( IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment )
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
  
        }
        public IActionResult Index()
        {
            List<Article> articles = _unitOfWork.ArticleRepository.GetAll().ToList();
            return View(articles);
        }
            
        public IActionResult Create()        
        {
            var model = new Article()
            {
                IsBlogActive = false
            };

            return View(model);

        }
        //create a request
        [HttpPost]
        public async Task<IActionResult> Create(Article article, IFormFile? file, IFormFile? file1)
        {
            if (ModelState.IsValid)
            {
                //set a new article to pending status
                article.Status = Article.StatusArticle.Pending;
                _unitOfWork.ArticleRepository.Add(article);
                _unitOfWork.Save();
                TempData["ShowMessage"] = true;
                
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
                else
                {
                    TempData["error"] = "You must insert file image.";
                    return View(article);
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

                    
                    _unitOfWork.ArticleRepository.Update(article);
                    _unitOfWork.Save();
                    TempData["success"] = "Send add article request successfully";
                    TempData["ShowMessage"] = true;
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

        public IActionResult Detail(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }

            Article? article = _unitOfWork.ArticleRepository.Get(a => a.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }



    }
}
