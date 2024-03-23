using _1640.Areas.Repository.IRepository;
using _1640.Models;
using Microsoft.AspNetCore.Mvc;

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
