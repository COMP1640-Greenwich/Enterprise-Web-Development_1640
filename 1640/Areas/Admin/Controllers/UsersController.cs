using System.Security.Claims;
using _1640.Data;
using _1640.Models;
using _1640.Models.VM;
using _1640.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace _1640.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManger;
    private readonly ApplicationDbContext _context;

    // GET
    public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _db = db;
        _userManager = userManager;
        _roleManger = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // taking current login user id
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        // exception itself admin
        var userList = await _db.Users
            .Include(u => u.Faculty) // Include the Faculty data
            .Where(u => u.Id != claims.Value)
            .ToListAsync();

        foreach (var user in userList)
        {
            var userTemp = await _userManager.FindByIdAsync(user.Id);
            var roleTemp = await _userManager.GetRolesAsync(userTemp);
            user.Role = roleTemp.FirstOrDefault();
        }

        return View(userList);
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
        adminStoreOwnerCustomer.Campus = user.Campus;

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
    [HttpGet]
    public async Task<IActionResult> ResetPasswordAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        ResetPasswordVM model = new ResetPasswordVM()
        {
            Email = user.Email,
            Password = user.PasswordHash,
            ConfirmPassword = user.PasswordHash
        };
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        var email = Request.Form["email"];
        var password = Request.Form["password"];
        var user = await _userManager.FindByEmailAsync(email);
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        if (user == null)
            return NotFound();
        var result = await _userManager.ResetPasswordAsync(user, code, password);
        if (result.Succeeded)
        {
            TempData["success"] = $"Password reset successfully! for (Email:  {user.Email})";
            return RedirectToAction("Index");
        }
        //error
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        model = new ResetPasswordVM()
        {
            Email = user.Email,
            Password = user.PasswordHash,
            ConfirmPassword = user.PasswordHash,
            Token = user.Id
        };
        TempData["error"] = "Something wrong!";
        return View(model);
    }
    [HttpGet]
    public IActionResult Login(string? ReturnUrl = null)
    {
        ViewData["ReturnUrl"] = ReturnUrl;
        return View();
    }


}