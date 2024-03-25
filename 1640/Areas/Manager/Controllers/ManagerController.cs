using _1640.Areas.Repository.IRepository;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace _1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ManagerController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //public ManagerController(ApplicationDbContext db)
        //{
        //    _db = db;
        //}
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ManagerController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            List<Faculty> faculities = _unitOfWork.FacultyRepository.GetAll().ToList();
            return View(faculities);
        }

        public IActionResult List()
        {
            List<Article> articles = _unitOfWork.ArticleRepository.GetAll().ToList();
            return View(articles);
        }

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

        public IActionResult Create()
        {
            return View();
        }
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
        public IActionResult Edit(int? id)
        {
            Faculty faculity = new Faculty();
            if (id == null || id == 0)
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
        [HttpPost]
        public IActionResult Delete(Faculty faculity)
        {
            _unitOfWork.FacultyRepository.Delete(faculity);
            _unitOfWork.Save();
            TempData["success"] = "Faculity deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
