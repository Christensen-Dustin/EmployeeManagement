﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        // Constructor to inject the ILoggerInterface
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch(statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested cannot be found.";
                    logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath}"
                        + $"and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
                case 503:
                    ViewBag.ErrorMessage = "Sorry, something weird happened.";
                    logger.LogWarning($"503 Error Occured. Path = {statusCodeResult.OriginalPath}"
                        + $"and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
                default:
                    ViewBag.ErrorMessage = "Sorry, Something unexpected happened.";
                    logger.LogWarning($"An Error Occured. Path = {statusCodeResult.OriginalPath}"
                        + $"and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        }
        
        //Handles errors when not in "Development"
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            logger.LogError($"The path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");
            ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;

            return View("Error");
        }
    }
}
