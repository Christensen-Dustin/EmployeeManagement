using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    // Blocks access to all pages exept the login and register, Controller 
    // level authentication
    // [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment,
                              ILogger<HomeController> logger,
                              UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.userManager = userManager;
        }

        // Allows access to anonymous user with authentication is implemented 
        // at the Controller or at the configeration level (Startup.cs)
        // [AllowAnonymous]
        public ViewResult Index()
        {
            // Retrieves all employee list
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        // Allows access to anonymous user with authentication is implemented 
        // at the Controller or at the configeration level (Startup.cs)
        // [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            // throws and exception automatically
            // throw new Exception("Error in Details View");

            // Check if the employee exists
            Employee employee = _employeeRepository.GetEmployee(id.Value);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            // ViewModel create a class
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                // id??1 if ID is given it will be displayed, if not default vaule is set to 1
                Employee = employee,
                PageTitle = "Employee Details"
            };

            // Sending Strongly Typed View
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        // Limits access to the CREATE attribute
        // [Authorize]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        // Limits access to the EDIT attribute
        // [Authorize]
        public ViewResult Edit(int id)
        {
            // Inject the Employee List (IEmployeeRepository)
            Employee employee = _employeeRepository.GetEmployee(id);

            // Logging types
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            // Populate the variable
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        // Limits access to the CREATE attribute
        // [Authorize]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            // Verify if fields are valid
            if (ModelState.IsValid)
            {
                // Create a string to capture the fileName of the image file
                string uniqueFileName = ProcessUploadedFile(model);

                // Creates the new employee and fills the CLASS
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                // Adds new employee to the Employee Repository
                _employeeRepository.Add(newEmployee);

                // Redirects the information back at detail with new employee ID
                return RedirectToAction("Details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpPost]
        // Limits access to the EDIT attribute
        // [Authorize]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            // Verify if fields are valid
            if (ModelState.IsValid)
            {
                // Get the injected model employee to update
                Employee employee = _employeeRepository.GetEmployee(model.Id);

                // Load the variables
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                // Debug Logs
                logger.LogDebug("ID: " + employee.Id);
                logger.LogDebug("Name: " + employee.Name);
                logger.LogDebug("Email: " + employee.Email);
                logger.LogDebug("Department: " + employee.Department);
                logger.LogDebug("Existing Path: " + model.ExistingPhotoPath);

                // Check to see if a new img file is being uploaded
                if (model.Photo != null)
                {
                    // Check to see if an img exists
                    if(model.ExistingPhotoPath != null)
                    {
                        // Build path to existing img
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", 
                                                        Path.GetFileName(model.ExistingPhotoPath));

                        // Delete the img file
                        System.IO.File.Delete(filePath);
                    }

                    // Returns a string of the new file name
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                // Adds new employee to the Employee Repository
                _employeeRepository.Update(employee);

                // Debug Logs
                logger.LogDebug("Updated ID: " + employee.Id);
                logger.LogDebug("Updated Name: " + employee.Name);
                logger.LogDebug("Updated Email: " + employee.Email);
                logger.LogDebug("Updated Department: " + employee.Department);
                logger.LogDebug("Updated Path: " + employee.PhotoPath);

                // Redirects the information back at detail with new employee ID
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID: {id} cannot be found";

                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

        /**********************************************************************
         * ProcessUploadedFile()
         *  This method takes the parameter EMPLOYEECREATEVIEWMODEL from the 
         *  ViewModel folder and prepares the path, in this instance an IMAGE
         *  file and copies it to PHOTO column of the USER.
         **********************************************************************/
        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            // Create a string to capture the fileName of the image file
            string uniqueFileName = null;

            // Check to see if the Photo object (model.Photo) is not null
            if (model.Photo != null)
            {
                // Generates the path to the images folder
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                // Generates a unique fileName for each image file
                // string photoFileName = model.Photo.FileName;
                string photoFileName = Path.GetFileName(model.Photo.FileName);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFileName;

                // Combining both the path and the fileName to be stored on the DATABASE
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Creates the image at the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
                    
            }

            return uniqueFileName;
        }
    }
}