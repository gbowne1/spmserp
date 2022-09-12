using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace spmserp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    ViewBag.QueryString = statusCodeResult.OriginalQueryString;
                    break;
                default:
                    break;
            }

            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            //ViewBag.ExceptionPath = exceptionDetails.Path;
            //ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            //ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;

            //return View("Error");

            ErrorViewModel errorViewModel = new ErrorViewModel();

            if (exceptionDetails != null)
            {
                errorViewModel.ExceptionPath = exceptionDetails.Path;
                errorViewModel.ExceptionMessage = exceptionDetails.Error.Message;
                errorViewModel.Stacktrace = exceptionDetails.Error.StackTrace;
            }
            else
            {
                errorViewModel.ExceptionPath = "";
                errorViewModel.ExceptionMessage = "Unhandled Error";
                errorViewModel.Stacktrace = "";
            }
            return View(errorViewModel);
        }
    }
}