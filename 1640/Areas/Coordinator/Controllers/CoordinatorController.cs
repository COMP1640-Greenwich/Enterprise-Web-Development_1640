using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models.VM;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            List<Semester> semesters = _unitOfWork.SemesterRepository.GetAll("Faculity").ToList();
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
                Faculities = _unitOfWork.FaculityRepository.GetAll().Select(f => new SelectListItem
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
                Faculities = _unitOfWork.FaculityRepository.GetAll().Select(f => new SelectListItem
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
    }
}