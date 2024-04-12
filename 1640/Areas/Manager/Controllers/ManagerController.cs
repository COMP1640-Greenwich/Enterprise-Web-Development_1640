using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Models.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.IO.Compression;

namespace _1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Route("Manager")]
    //[Route("Manager/Manager")]
    public class ManagerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _dbContext;
        private readonly int _recordsPerPage = 3;
        public ManagerController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, ApplicationDbContext dBContext)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dBContext;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            List<Faculty> faculities = _unitOfWork.FacultyRepository.GetAll().ToList();
            return View(faculities);
        }

        [Route("List")]
        public IActionResult List(int id)
        {
            // Fetch all approved articles
            List<Article> articles = _unitOfWork.ArticleRepository.GetAll(a => a.Status == Article.StatusArticle.Approve).ToList();

            int numberOfRecords = articles.Count();
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _recordsPerPage);

            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;

            var articlesList = articles.Skip(id * _recordsPerPage).Take(_recordsPerPage).ToList();
            return View(articlesList);
        }


        [Route("List/id")]
        public IActionResult DownloadDocInZip(int id)
        {
            var article = _unitOfWork.ArticleRepository.Get(a => a.Id == id);
            if (article == null || (string.IsNullOrEmpty(article.DocxUrl) && string.IsNullOrEmpty(article.ImageUrl)))
            {
                return NotFound();
            }

            var zipPath = Path.GetTempFileName() + ".zip";
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                if (!string.IsNullOrEmpty(article.DocxUrl))
                {
                    var docPath = Path.Combine(_hostingEnvironment.WebRootPath, article.DocxUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(docPath))
                    {
                        zip.CreateEntryFromFile(docPath, Path.GetFileName(docPath));
                    }
                }

                if (!string.IsNullOrEmpty(article.ImageUrl))
                {
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, article.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        zip.CreateEntryFromFile(imagePath, Path.GetFileName(imagePath));
                    }
                }
            }

            var zipBytes = System.IO.File.ReadAllBytes(zipPath);
            return File(zipBytes, "application/zip", "doc.zip");
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        public IActionResult Create(Faculty faculity)
        {
            if (ModelState.IsValid)
            {
                //_db.Faculities.Add(faculity);
                //_db.SaveChanges();
                _unitOfWork.FacultyRepository.Add(faculity);
                _unitOfWork.Save();
                TempData["CreateFaculty"] = "Faculty created successfully";
                TempData["ShowMessage"] = true;
            }
            return RedirectToAction("Index");
        }
        [Route("Edit/id")]
        public IActionResult Edit(int? id)
        {
            Faculty faculty = new Faculty();
            if (id == null || id == 0)
            {
                return NotFound();
            }
            faculty = _unitOfWork.FacultyRepository.Get(f => f.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }
        [Route("Edit/id")]
        [HttpPost]
        public IActionResult Edit(Faculty faculity)
        {
            if (ModelState.IsValid)
            {
                //_db.Faculities.Update(faculity);
                //_db.SaveChanges();
                _unitOfWork.FacultyRepository.Update(faculity);
                _unitOfWork.Save();
                TempData["EditFaculty"] = "Faculity update successfully";
                TempData["ShowMessage"] = true;
            }
            return RedirectToAction("Index");
        }
        [Route("Delete/id")]
        public IActionResult Delete(int id)
        {
            Faculty faculity = new Faculty();
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            faculity = _unitOfWork.FacultyRepository.Get(f => f.Id == id);
            if (faculity == null)
            {
                return NotFound();
            }
            return View(faculity);
        }
        [Route("Delete/id")]
        [HttpPost]
        public IActionResult Delete(Faculty faculity)
        {
            _unitOfWork.FacultyRepository.Delete(faculity);
            _unitOfWork.Save();
            TempData["DeleteFaculty"] = "Faculity deleted successfully";
            TempData["ShowMessage"] = true;
            return RedirectToAction("Index");
        }
        [Route("")]
        [Route("Dashboard1")]
        public IActionResult Dashboard1()
        {
            List<Semester> semesters = _dbContext.Semesters.ToList();
            List<SemesterArticleVM> semesterArticleViewModels = new List<SemesterArticleVM>();

            foreach (var semester in semesters)
            {
                var articleCount = _dbContext.Articles.Count(a => a.SemesterId == semester.Id && a.Status != Article.StatusArticle.Reject && a.Status != Article.StatusArticle.Pending);

                // Create a view model object for each semester
                var semesterArticleVM = new SemesterArticleVM
                {
                    SemesterId = semester.Id,
                    SemesterName = semester.Name,
                    ArticleCount = articleCount
                };

                semesterArticleViewModels.Add(semesterArticleVM);
            }

            return View(semesterArticleViewModels);

        }
        [Route("Dashboard2")]
        public IActionResult Dashboard2()
        {
            List<FacultyVM> facultyViewModels = new List<FacultyVM>();

            // Get the faculties
            List<Faculty> faculties = _dbContext.Faculties.ToList();

            foreach (var faculty in faculties)
            {
                // Count students for each faculty
                var studentCount = _dbContext.Users.Where(u => u.Role == "Student" && u.FacultyId == faculty.Id).Count();

                // Create a view model object for each faculty
                var facultyViewModel = new FacultyVM
                {
                    FacultyId = faculty.Id,
                    FacultyName = faculty.Name,
                    StudentCount = studentCount
                };

                facultyViewModels.Add(facultyViewModel);
            }

            // Pass the list of faculty view models to the view
            return View(facultyViewModels);
        }
        [Route("Dashboard3")]
        public IActionResult Dashboard3()
        {
            List<Faculty> faculties = _dbContext.Faculties.ToList();
            List<FacultyVM> facultyArticleViewModels = new List<FacultyVM>();

            foreach (var faculty in faculties)
            {
                var articleCount = _dbContext.Articles.Count(a => a.FacultyId == faculty.Id && a.Status != Article.StatusArticle.Reject && a.Status != Article.StatusArticle.Pending);

                // Create a view model object for each faculty
                var facultyArticleViewModel = new FacultyVM
                {
                    FacultyId = faculty.Id,
                    FacultyName = faculty.Name,
                    ArticleCount = articleCount
                };

                facultyArticleViewModels.Add(facultyArticleViewModel);
            }

            return View(facultyArticleViewModels);
        }



    }
}
