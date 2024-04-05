// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using _1640.Areas.Repository.IRepository;
using _1640.Data;
using _1640.Models;
using _1640.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _1640.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
             RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            //_emailStore = GetEmailStore();
            _logger = logger;
            //_emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _db = db;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            [Display(Name = "Your phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Faculty")]
            public int SelectedFacultyId { get; set; }

            public IEnumerable<SelectListItem> Faculties { get; set; }

        // address and rolelist
        [Required]
            [Display(Name = "Your Campus")]
            public string Campus { get; set; }
            public IEnumerable<SelectListItem> SelectYourRole { get; set; }
            [Required]
            public string Role { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            GetRoles();
            Input.Faculties = _unitOfWork.FacultyRepository.GetAll()
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                });
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {               

                var user = new User()
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    EmailConfirmed = true,
                    Campus = Input.Campus,
                    PhoneNumber = Input.PhoneNumber,
                    Role = Input.Role,
                    FacultyId = Input.SelectedFacultyId,
                };

                if (Input.Role == SD.Role_Coordinator && _db.Users.Any(u => u.FacultyId == Input.SelectedFacultyId && u.Role == SD.Role_Coordinator))
                {
                    ModelState.AddModelError(string.Empty, "A coordinator for the selected faculty already exists.");
                    GetRoles();
                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (Input.Role == "Student")
                    {
                        await _userManager.AddToRolesAsync(user, new[] { "Student" });
                        TempData["success"] = "Adding Account Student Successfully";
                    }
                    if (Input.Role == "User")
                    {
                        await _userManager.AddToRolesAsync(user, new[] { "User" });
                        TempData["success"] = "Adding Account Guest Successfully";
                    }
                    if (Input.Role == "Coordinator")
                    {
                        await _userManager.AddToRolesAsync(user, new[] { "Coordinator" });
                        TempData["success"] = "Adding Account Coordinator Successfully";
                    }
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        //await _signInManager.SignInAsync(user, isPersistent: true);
                        //return LocalRedirect(returnUrl);
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            GetRoles();
            return Page();
        }

        //private IdentityUser CreateUser()
        //{
        //    try
        //    {
        //        return Activator.CreateInstance<IdentityUser>();
        //    }
        //    catch
        //    {
        //        throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
        //            $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
        //            $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        //    }
        //}
        //private IUserEmailStore<IdentityUser> GetEmailStore()
        //{
        //    if (!_userManager.SupportsUserEmail)
        //    {
        //        throw new NotSupportedException("The default UI requires a user store with email support.");
        //    }
        //    return (IUserEmailStore<IdentityUser>)_userStore;
        //}

        private void GetRoles()
        {
            Input = new InputModel()
            {
                SelectYourRole = _roleManager.Roles.Where(x => x.Name != "Admin").Where(x=>x.Name !="Manager")
                    .Select(x => x.Name).Select(x => new SelectListItem()
                    {
                        Text = x,
                        Value = x
                    })
            };
        }
    }
}
