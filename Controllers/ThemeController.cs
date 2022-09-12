using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing.RequestContext;
using System.Web.Mvc;
 
namespace Theme.Controllers
{
    public abstract class ThemeController : Controller
    {
        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            // Set default masterpage to use
            requestContext.HttpContext.Items["themeName"] = "Default";

            // Allow the Theme to be overriden via the querystring
            // If a Theme Name is Passed in the querystring then use it and override the previously set Theme Name
            // http://localhost/Default.aspx?theme=Red
            var previewTheme = requestContext.HttpContext.Request.QueryString["theme"];
            if (!string.IsNullOrEmpty(previewTheme))
            {
                requestContext.HttpContext.Items["themeName"] = previewTheme;
            }
            
            base.Execute(requestContext);
        }

    }
}