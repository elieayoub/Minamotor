using Admin.Controllers;
using Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Admin.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipAuthentication), false).Any())
            {
                // The controller action is decorated with the [DontValidate]
                // custom attribute => don't do anything.
                return;
            }

            if (HttpContext.Current.Session["Global_UserID"] == null ||
                HttpContext.Current.Session["Global_UserID"].ToString() == "")
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", 
                    action = "Login" }));
            }
        }
    }
}