using _1640.Repository.IRepository;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;
using _1640.Areas.Repository.IRepository;

namespace _1640.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class ManagerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManagerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Faculty> faculities = _unitOfWork.FacultyRepository.GetAll().ToList();
            return View(faculities);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Add(faculty);
                _unitOfWork.Save();
                TempData["success"] = "Faculty create successfully";
            }
            return RedirectToAction("Index");
        }
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
        [HttpPost]
        public IActionResult Edit(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Update(faculty);
                _unitOfWork.Save();
                TempData["success"] = "Faculty update successfully";
                return RedirectToAction("Index");
            }
            return View(faculty);
        }
        public IActionResult Delete(int id)
        {
            Faculty faculty = new Faculty();
            if (id == 0 || id == null)
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
        [HttpPost]
        public IActionResult Delete(Faculty faculty)
        {
            _unitOfWork.FacultyRepository.Delete(faculty);
            _unitOfWork.Save();
            TempData["success"] = "Faculty deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
