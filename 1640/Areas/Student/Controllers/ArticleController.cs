using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Models.VM;
using _1640.Repository.IRepository;
using _1640.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace _1640.Areas.Student.Controllers
{
    [Area("Student")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly int _recordsPerPage = 4;
        public ArticleController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        //public IActionResult Index()
        //{
        //    List<Article> articles = _unitOfWork.ArticleRepository.GetAllApprove("Semester").ToList();
        //    return View(articles);
        //}

        [Authorize(Roles = Constraintt.StudentRole)]
        public async Task<IActionResult> MyArticles(string searchString = "")
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Get the articles of the current user
            List<Article> articles = _unitOfWork.ArticleRepository.GetAll(a => a.UserId == user.Id).ToList();
            if (articles.Count == 0)
            {
                ViewBag.Message = "You don't have any an Contribution";
            }
            // If a search string is provided, filter the articles based on their title
            if (!string.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(a => a.Title.Contains(searchString)).ToList();
            }
            return View(articles);

        }

        public IActionResult Create(string id, int semesterId)
        {
            var userFacultyId = _unitOfWork.UserRepository.Get(f => f.Id == id).FacultyId.Value;

            ArticleVM articleVM = new ArticleVM()
            {
                Article = new Article()
                {
                    IsBlogActive = false,
                    SemesterId = semesterId
                },
                UserName = _unitOfWork.UserRepository.Get(f => f.Id == id).FullName.ToUpper(),
                FacultyId = userFacultyId
            };
            articleVM.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == articleVM.FacultyId).Name.ToString();
            ViewBag.UserId = id;
            return View(articleVM);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync(string id, ArticleVM articleVM, IFormFile? file, IFormFile? file1)
        {

            if (ModelState.IsValid)
            {

                var isBlogActive = articleVM.Article.IsBlogActive;
                if (isBlogActive == true)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string imagePath = Path.Combine(wwwRootPath, @"images\articles");

                        if (!string.IsNullOrEmpty(articleVM.Article.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, articleVM.Article.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        articleVM.Article.ImageUrl = @"\images\articles\" + fileName;
                    }
                    else
                    {
                        TempData ["error"] = "You must insert file image.";
                        return View(articleVM);
                    }
                    if (file1 != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file1.FileName);
                        string docxPath = Path.Combine(wwwRootPath, @"docx");
                        if (!string.IsNullOrEmpty(articleVM.Article.DocxUrl))
                        {
                            var old1ImagePath = Path.Combine(wwwRootPath, articleVM.Article.DocxUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(old1ImagePath))
                            {
                                System.IO.File.Delete(old1ImagePath);
                            }
                        }
                        using (var fileStream = new FileStream(Path.Combine(docxPath, fileName), FileMode.Create))
                        {
                            file1.CopyTo(fileStream);
                        }
                        articleVM.Article.DocxUrl = @"\docx\" + fileName;
                    }
                    else
                    {
                        TempData["error"] = "You must insert file doxc.";
                        return View(articleVM);
                    }
                    //set a new article to pending status
                    articleVM.Article.UserName = articleVM.UserName;
                    articleVM.Article.UserId = id;
                    articleVM.Article.FacultyId = (int)articleVM.FacultyId;
                    articleVM.Article.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == articleVM.FacultyId).Name.ToString();
                    articleVM.Article.Status = Article.StatusArticle.Pending;
                    

                    _unitOfWork.ArticleRepository.Add(articleVM.Article);
                    _unitOfWork.Save();

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("tdm0982480826@gmail.com", "xnej ojsl etxa euki"),
                        EnableSsl = true,
                    };
                    var message = $"Student {articleVM.Article.UserName} have requested an article";
                    smtpClient.Send("tdm0982480826@gmail.com", "tabthien18@gmail.com", "New article created", message);


                    TempData["success"] = "Contribution Created successfully";
                    return RedirectToAction("MyArticles");
                }
                else
                {

                    TempData["error"] = "You must agree to our Terms and Conditions.";
                    return View(articleVM);
                }
            }
            ArticleVM articleVMNew = new ArticleVM()
            {

                Semesters = _unitOfWork.SemesterRepository.GetAllOpening().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                }),
                Article = new Article()
                {
                    IsBlogActive = false,
                    UserId = id,


                },
                UserName = _unitOfWork.UserRepository.Get(f => f.Id == id).FullName.ToUpper(),
                FacultyId = _unitOfWork.UserRepository.Get(f => f.Id == id).FacultyId.Value
            };
            articleVM.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == articleVM.FacultyId).Name.ToString();
            ViewBag.UserId = id;
            return View(articleVMNew);

        }


        public ActionResult ViewFeedBack(int id)
        {
            Comment comment = new Comment();
            comment.ArticleId = id;
            List<Comment> comments = _dbContext.Comments.Where(c => c.ArticleId == id).ToList();
            ViewBag.ArticleId = id;
            if (comments.Count == 0)
            {
                ViewBag.Message = "Contribution don't have a feedback from Coordinator";
            }
            return View(comments);

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Get the article to be edited
            var article = _unitOfWork.ArticleRepository.Get(a => a.Id == id);

            if (article == null)
            {
                return NotFound("Article not found");
            }

            // Get the semester of the article
            var semester = _unitOfWork.SemesterRepository.Get(s => s.Id == article.SemesterId);

            if (DateTime.Now > semester.EndDate)
            {
                TempData["error"] = "The final deadline has passed. You cannot update this Contribution.";
                return RedirectToAction("MyArticles");
            }

            // Create the view model
            ArticleVM articleVM = new ArticleVM()
            {
                Article = article,
                Semesters = _unitOfWork.SemesterRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    })
            };

            return View(articleVM);
        }

        [HttpPost]
        public IActionResult Edit(ArticleVM articleVM, IFormFile? file, IFormFile? file1)
        {
            if (ModelState.IsValid)
            {
                // Get the current article from the database
                var currentArticle = _unitOfWork.ArticleRepository.Get(a => a.Id == articleVM.Article.Id);

                if (currentArticle == null)
                {
                    return NotFound("Article not found");
                }

                // Only allow editing if the article is in pending status
                if (currentArticle.Status != Article.StatusArticle.Pending)
                {
                    TempData["error"] = "You can only edit Contribution that are in pending status.";
                    return RedirectToAction("MyArticles");
                }

                // Update the article
                currentArticle.Title = articleVM.Article.Title;
                currentArticle.Description = articleVM.Article.Description;

                // Handle file uploads
                if (file != null)
                {
                    // Handle image file upload
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\articles");

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    currentArticle.ImageUrl = @"\images\articles\" + fileName;
                }

                if (file1 != null)
                {
                    // Handle docx file upload
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file1.FileName);
                    string docxPath = Path.Combine(wwwRootPath, @"docx");

                    using (var fileStream = new FileStream(Path.Combine(docxPath, fileName), FileMode.Create))
                    {
                        file1.CopyTo(fileStream);
                    }
                    currentArticle.DocxUrl = @"\docx\" + fileName;
                }

                _unitOfWork.ArticleRepository.Update(currentArticle);
                _unitOfWork.Save();

                TempData["success"] = "Contribution updated successfully";
                return RedirectToAction("MyArticles");
            }

            // If model state is not valid, return the view with the current articleVM
            return View(articleVM);
        }


        [HttpGet]
public IActionResult Delete(int id)
{
    // Get the article to be deleted
    var article = _unitOfWork.ArticleRepository.Get(a => a.Id == id);

    if (article == null)
    {
        return NotFound("Article not found");
    }

    // Get the semester of the article
    var semester = _unitOfWork.SemesterRepository.Get(s => s.Id == article.SemesterId);

    if (DateTime.Now > semester.EndDate)
    {
        TempData["error"] = "The final deadline has passed. You cannot delete this Contribution.";
        return RedirectToAction("MyArticles");
    }

    // Pass the article to the view
    return View(article);
}



        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // Get the article to be deleted
            var article = _unitOfWork.ArticleRepository.Get(a => a.Id == id);

            if (article == null)
            {
                return NotFound("Article not found");
            }

            // Get the semester of the article
            var semester = _unitOfWork.SemesterRepository.Get(s => s.Id == article.SemesterId);

            if (DateTime.Now > semester.EndDate)
            {
                TempData["error"] = "The final deadline has passed. You cannot delete this Contribution.";
                return RedirectToAction("MyArticles");
            }

            // Delete the article
            _unitOfWork.ArticleRepository.Delete(article);
            _unitOfWork.Save();

            TempData["success"] = "Contribution deleted successfully";
            return RedirectToAction("MyArticles");
        }

        public async Task<IActionResult> SemesterList()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Get the FacultyId of the current user
            var userFacultyId = _unitOfWork.UserRepository.Get(f => f.Id == user.Id).FacultyId.Value;

            // Get the semesters of the current user's faculty
            var semesters = _unitOfWork.SemesterRepository.GetAllOpeningByFaculty(s => s.FacultyId == userFacultyId).ToList();

            return View(semesters);
        }

    }
}
