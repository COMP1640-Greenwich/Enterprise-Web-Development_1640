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
        private readonly int _recordsPerPage = 4;
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
        public IActionResult List(int id, string searchString = "")
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
                TempData["success"] = "Faculity create successfully";
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
                TempData["success"] = "Faculity update successfully";
                return RedirectToAction("Index");
            }
            return View(faculity);
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
            TempData["success"] = "Faculity deleted successfully";
            return RedirectToAction("Index");
        }
        [Route("")]
        [Route("Dashboard1")]
        public IActionResult Dashboard1()
        {
            Dictionary<int, int> articleCounts = new Dictionary<int, int>();
            //Count Article
            List<Semester> semesters = _dbContext.Semesters.ToList();
            foreach (var semester in semesters)
            {
                var articleCount = _dbContext.Articles.Where(u => u.SemesterId == semester.Id).Count();
                articleCounts.Add(semester.Id, articleCount);
            }
            ViewBag.ArticleList = articleCounts;
            return View(semesters);

        }
        [Route("Dashboard2")]
        public IActionResult Dashboard2()
        {
            Dictionary<int, int> studentCounts = new Dictionary<int, int>();
            //Count User
            List<Faculty> faculties = _dbContext.Faculties.ToList();
            foreach (var faculty in faculties)
            {
                var studentCount = _dbContext.Users.Where(u => u.Role == "Student").Where(u => u.FacultyId == faculty.Id).Count();
                studentCounts.Add(faculty.Id, studentCount);
            }
            ViewBag.StudentList = studentCounts;
            return View(faculties);
        }
        [Route("Dashboard3")]
        public IActionResult Dashboard3()
        {
            Dictionary<int, int> articleCounts1 = new Dictionary<int, int>();
            //Count User
            List<Faculty> faculties1 = _dbContext.Faculties.ToList();
            foreach (var faculty1 in faculties1)
            {
                var articleCount1 = _dbContext.Articles.Where(u => u.FacultyId == faculty1.Id).Count();
                articleCounts1.Add(faculty1.Id, articleCount1);
            }
            ViewBag.ArticleFacultyList = articleCounts1;
            return View(faculties1);
        }



    }
}
