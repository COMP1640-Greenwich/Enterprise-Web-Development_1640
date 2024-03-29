using _1640.Repository.IRepository;
using _1640.Data;
using _1640.Models.VM;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using _1640.Utility;
using Microsoft.EntityFrameworkCore;
using _1640.Areas.Repository.IRepository;

namespace _1640.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _db;
        //private readonly ISemesterRepository SemesterRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CoordinatorController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            List<Semester> semesters = _unitOfWork.SemesterRepository.GetAll("Faculty").ToList();
            return View(semesters);
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Create(int? id)
        {
            SemesterVM semesterVM = new SemesterVM()
            {
                Faculties = _unitOfWork.FacultyRepository.GetAll().Select(f => new SelectListItem
                {
                    Text = f.Name,
                    Value = f.Id.ToString(),
                }),
                Semester = new Semester()
            };
            if (id == null || id == 0)
            {
                return View(semesterVM);
            }
            return View(semesterVM);

        }
        [HttpPost]
        public IActionResult Create(SemesterVM semesterVM)
        {
            if (ModelState.IsValid)
            {
                if (semesterVM.Semester.Id == 0)
                {
                    _unitOfWork.SemesterRepository.Add(semesterVM.Semester);
                    _unitOfWork.Save();
                    TempData["success"] = "Semester created successfully";
                }

            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            SemesterVM semesterVM = new SemesterVM()
            {
                Faculties = _unitOfWork.FacultyRepository.GetAll().Select(f => new SelectListItem
                {
                    Text = f.Name,
                    Value = f.Id.ToString(),
                }),
                Semester = new Semester()
            };
            if (id == null || id == 0)
            {
                return View(semesterVM);
            }
            else
            {
                semesterVM.Semester = _unitOfWork.SemesterRepository.Get(s => s.Id == id);

            }
            return View(semesterVM);
        }
        [HttpPost]
        public IActionResult Edit(SemesterVM semesterVM)
        {
            if (semesterVM.Semester.Id != 0)
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.SemesterRepository.Update(semesterVM.Semester);
                    _unitOfWork.Save();
                    TempData["success"] = "Semester updated successfully";
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Semester semester = _unitOfWork.SemesterRepository.Get(s => s.Id == id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }
        [HttpPost]
        public IActionResult Delete(Semester semester)
        {
            _unitOfWork.SemesterRepository.Delete(semester);
            _unitOfWork.Save();
            TempData["success"] = "Delete semester successfully";
            return RedirectToAction("Index");
        }
        public ActionResult AddFeedBack(int id)
        {
            //if (id == null || id == 0)
            //{
            //    return NotFound();
            //}
            Comment comment = new Comment();
            comment.ArticleId = id;
            List<Comment> comments = _db.Comments.Where(c => c.ArticleId == id).ToList();
            ViewBag.ArticleId = id;
            return View(comments);

        }
        [HttpPost]
        public ActionResult AddFeedBack(int id, string articleFB)
        {
            Comment comment = new Comment();
            comment.ArticleId = id;
            comment.Text = articleFB;
            comment.CommentOn = DateTime.Now;
            _db.Comments.Add(comment);
            _db.SaveChanges();
            return RedirectToAction("Requests");
        }
        // list of request article for Coordinator
        [Authorize(Roles = Constraintt.CoordinatorRole)]
        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var request = await _db.Articles.Where(a => a.Status == Article.StatusArticle.Pending).ToListAsync();
            if (request.Count == 0 || request.Count == null)
            {
                return NotFound("You don't have any request");
            }
            return View(request);
        }

        // đồng ý và từ chối bài viết
        //Approve the article
        [Authorize(Roles =Constraintt.CoordinatorRole)]
        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var approveArticle = await _db.Articles.FindAsync(id);
            if(approveArticle == null)
            {
                return NotFound("The request not found");
            }
            approveArticle.Status = Article.StatusArticle.Approve;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Aprrove for Create Article successfully";
            return RedirectToAction("Index");
        }
        //Reject the article
        [Authorize(Roles = Constraintt.CoordinatorRole)]
        [HttpGet]
        public async Task<IActionResult> Reject(int id) 
        {
            var rejectArticle = await _db.Articles.FindAsync(id);
            if (rejectArticle == null)
            {
                return NotFound("The request not found");
            }
            rejectArticle.Status = Article.StatusArticle.Reject;
            _db.Remove(rejectArticle); // delelte the article from list
            await _db.SaveChangesAsync();
            TempData["Success"] = "Reject for Create Article successfully";
            return RedirectToAction("Index");
        }

    }
}