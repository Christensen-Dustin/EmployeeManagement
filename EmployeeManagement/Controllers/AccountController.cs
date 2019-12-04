using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }

        [HttpGet]
        // Allow access to anonymous user when authenication is set up in 
        // Startup.cs at the configuration level
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        // Allow access to anonymous user when authenication is set up in 
        // Startup.cs at the configuration level
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Connect user to password
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password,
                                    model.RememberMe, false);

                // Check if connection between user and password is successful
                if (result.Succeeded)
                {
                    // Checks to see if the ReturnUrl is avaible and is a local URL
                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        // LocalRedirect only redirects to a local page on the 
                        // site, not to a page off the site, that is available
                        // through using Redirect.
                        // return LocalRedirect(returnUrl); // More Secure
                        // return Redirect(returnUrl);      // Less Secure
                        
                        // In combination with the Url.IsLocalUrl is secure 
                        // and does not throw exception
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        // Redirect the user to the home/index page
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        [HttpGet]
        // Allow access to anonymous user when authenication is set up in 
        // Startup.cs at the configuration level
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Does the same as 
        // [HttpPost][HttpGet]
        [AcceptVerbs("Get", "Post")]
        // Allow access to anonymous user when authenication is set up in 
        // Startup.cs at the configuration level
        [AllowAnonymous]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            // Check new user email against the database
            var name = await userManager.FindByNameAsync(userName);

            // if the email is not on the database
            if (name == null)
            {
                return Json(true);
            }
            // if the email is on the database
            else
            {
                return Json($"User Name {name} is already in use");
            }
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            // Check new user email against the database
            var userEmail = await userManager.FindByEmailAsync(email);

            // if the email is not on the database
            if (userEmail == null)
            {
                return Json(true);
            }
            // if the email is on the database
            else
            {
                return Json($"Email {userEmail} is already in use");
            }
        }

        [HttpPost]
        // Allow access to anonymous user when authenication is set up in 
        // Startup.cs at the configuration level
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create new user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    City = model.City
                };
                
                // Connect user to password
                var result = await userManager.CreateAsync(user, model.Password);

                // Check if connection between user and password is successful
                if (result.Succeeded)
                {
                    // If USER has role "Admin" User will not be logged out
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        // Redirecting ADMIN User to the List of Users
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    // Sign in user with a session cookie
                    await signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect the user to the home/index page
                    return RedirectToAction("Index", "Home");
                }

                // if not successful loop through each error
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
