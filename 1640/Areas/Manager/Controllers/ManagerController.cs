using _1640.Repository.IRepository;
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
            List<Faculity> faculities = _unitOfWork.FaculityRepository.GetAll().ToList();
            return View(faculities);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Faculity faculity)
        {
            if (ModelState.IsValid)
            {
                //_db.Faculities.Add(faculity);
                //_db.SaveChanges();
                _unitOfWork.FaculityRepository.Add(faculity);
                _unitOfWork.Save();
                TempData["success"] = "Faculity create successfully";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            Faculity faculity = new Faculity();
            if (id == null || id == 0)
            {
                return NotFound();
            }
            faculity = _unitOfWork.FaculityRepository.Get(f => f.Id == id);
            if (faculity == null)
            {
                return NotFound();
            }
            return View(faculity);
        }
        [HttpPost]
        public IActionResult Edit(Faculity faculity)
        {
            if (ModelState.IsValid)
            {
                //_db.Faculities.Update(faculity);
                //_db.SaveChanges();
                _unitOfWork.FaculityRepository.Update(faculity);
                _unitOfWork.Save();
                TempData["success"] = "Faculity update successfully";
                return RedirectToAction("Index");
            }
            return View(faculity);
        }
        public IActionResult Delete(int id)
        {
            Faculity faculity = new Faculity();
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            faculity = _unitOfWork.FaculityRepository.Get(f => f.Id == id);
            if (faculity == null)
            {
                return NotFound();
            }
            return View(faculity);
        }
        [HttpPost]
        public IActionResult Delete(Faculity faculity)
        {
            _unitOfWork.FaculityRepository.Delete(faculity);
            _unitOfWork.Save();
            TempData["success"] = "Faculity deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
