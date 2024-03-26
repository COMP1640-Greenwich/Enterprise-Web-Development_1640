using _1640.Repository.IRepository;
using _1640.Data;
using _1640.Models.VM;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using _1640.Areas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace _1640.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class CoordinatorController : Controller
    {
        //private readonly ISemesterRepository SemesterRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        public CoordinatorController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
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
            List<Comment> comments = _dbContext.Comments.ToList();
            return View(comments);
        }
        [HttpPost]
        public ActionResult AddFeedBack(int id, string articleFB)
        {
            Comment comment = new Comment();
            comment.ArticleId = id;
            comment.Text = articleFB;
            comment.CommentOn = DateTime.Now;
            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();
            return RedirectToAction("AddFeedBack");
        }

    }
}