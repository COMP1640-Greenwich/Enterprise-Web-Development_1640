using System.Security.Claims;
using _1640.Data;
using _1640.Models;
using _1640.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1640.Controllers;

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;

        // GET
        public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManger = roleManager;
        }
        
        public async Task<IActionResult> Index()
        {
            // taking current login user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // exception itself admin
            var userList = _db.Users.Where(u => u.Id != claims.Value);

            foreach (var user in userList)
            {
                var userTemp = await _userManager.FindByIdAsync(user.Id);
                var roleTemp = await _userManager.GetRolesAsync(userTemp);
                user.Role = roleTemp.FirstOrDefault();
            }


            return View(userList.ToList());
        }
        
   
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            TempData["DeleteUserMessage"] = "User Deleted!";
            TempData["ShowMessage"] = true; //Set flag to show message in the view
            return RedirectToAction(nameof(Index));
        }

       
        
        [HttpGet]
        public IActionResult EditUser(string id)
        {
            var adminStoreOwnerCustomer = _db.Users.Find(id);
            return View(adminStoreOwnerCustomer);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var adminStoreOwnerCustomer = _db.Users.Find(user.Id);
            if (adminStoreOwnerCustomer == null)
            {
                return NotFound("User is null");
            }

            adminStoreOwnerCustomer.FullName = user.FullName;
            adminStoreOwnerCustomer.PhoneNumber = user.PhoneNumber;
            adminStoreOwnerCustomer.HomeAddress = user.HomeAddress;
                
            _db.Users.Update(adminStoreOwnerCustomer);
            _db.SaveChanges();
            
            TempData["EditUserMessage"] = "The information of User updated!";
            TempData["ShowMessage"] = true; //Set flag to show message in the view
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = _db.Users.Find(id);
            var roletemp = await _userManager.GetRolesAsync(user);
            var role = roletemp.First();

            return RedirectToAction("EditUser", new { id });
        }
        
    }