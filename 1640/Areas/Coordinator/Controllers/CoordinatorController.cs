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
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace _1640.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ApplicationDbContext _db;
        //private readonly ISemesterRepository SemesterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public CoordinatorController(IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var coordinator = user as User;
            List<Semester> semesters = _unitOfWork.SemesterRepository.GetAll(a => a.FacultyId == coordinator.FacultyId).ToList();
            return View(semesters);
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Create(string id)
        {
            var userFacultyId = _unitOfWork.UserRepository.Get(f => f.Id == id).FacultyId.Value;
            SemesterVM semesterVM = new SemesterVM()
            {
                Semester = new Semester(),
                FacultyId = userFacultyId,
            };
            semesterVM.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == semesterVM.FacultyId).Name.ToString();
            return View(semesterVM);

        }
        [HttpPost]
        public IActionResult Create(SemesterVM semesterVM)
        {
            if (ModelState.IsValid)
            {
                if (semesterVM.Semester.Id == 0)
                {
                    semesterVM.Semester.FacultyId = semesterVM.FacultyId;
                    semesterVM.Semester.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == semesterVM.FacultyId).Name.ToString();
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
                Semester = new Semester(),
                //FacultyId = _unitOfWork.SemesterRepository.Get(filter => filter.Id == id).FacultyId,
                //FacultyName = _unitOfWork.SemesterRepository.Get(f => f.Id == id).FacultyName.ToString(),
            };
            
            if (id == null || id == 0)
            {
                return NotFound();
            }
            else
            {
                semesterVM.Semester = _unitOfWork.SemesterRepository.Get(s => s.Id == id);
                semesterVM.FacultyId = _unitOfWork.SemesterRepository.Get(x => x.Id == id).FacultyId.Value;
                semesterVM.FacultyName = semesterVM.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == semesterVM.FacultyId).Name.ToString();
            }
            return View(semesterVM);
        }

        [HttpPost]
        public IActionResult Edit(SemesterVM semesterVM)
        {
                if (ModelState.IsValid)
                {
                    semesterVM.Semester.FacultyId = semesterVM.FacultyId;
                    semesterVM.Semester.FacultyName = _unitOfWork.FacultyRepository.Get(f => f.Id == semesterVM.FacultyId).Name.ToString();
                    _unitOfWork.SemesterRepository.Update(semesterVM.Semester);
                    _unitOfWork.Save();
                    TempData["success"] = "Semester updated successfully";
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
            TempData["error"] = "Delete semester successfully";
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
            return RedirectToAction("AddFeedBack", new { id = comment.ArticleId });
        }

        // list of request article for Coordinator
        [Authorize(Roles = Constraintt.CoordinatorRole)]
        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            // Get the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _db.Users.FindAsync(userId);

            // Get the articles that have the same FacultyId as the coordinator and are pending
            var request = await _db.Articles
                .Where(a => a.Status == Article.StatusArticle.Pending && a.FacultyId == user.FacultyId)
                .ToListAsync();

            if (request.Count == 0)
            {
                ViewBag.Message = "You don't have any request";
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

            var user = await _db.Users.FindAsync(approveArticle.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Send email to the student who created the article
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tdm0982480826@gmail.com", "xnej ojsl etxa euki"),
                EnableSsl = true,
            };
            smtpClient.Send("tdm0982480826@gmail.com", user.Email, "Your article was approved", "Congratulations, your article was approved.");


            TempData["Success"] = "Aprrove for Create Megazine successfully";
            return RedirectToAction("Requests");
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
            //_db.Remove(rejectArticle); // delelte the article from list
            await _db.SaveChangesAsync();

            var user = await _db.Users.FindAsync(rejectArticle.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Send email to the student who created the article
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tdm0982480826@gmail.com", "xnej ojsl etxa euki"),
                EnableSsl = true,
            };
            smtpClient.Send("tdm0982480826@gmail.com", user.Email, "Your article was rejected", "We're sorry, but your article was rejected.");

            TempData["Success"] = "Reject for Create Megazine successfully";
            return RedirectToAction("Requests");
        }

    }
}