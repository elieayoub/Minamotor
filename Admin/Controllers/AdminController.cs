using Admin.Filters;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class AdminController : AdminMainController
    {
        public ActionResult Index()
        {
            return View();
        }

        [SkipAuthentication]
        public ActionResult Login()
        {
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"].ToString();
            return View();
        }

        [SkipAuthentication]
        public ActionResult Authenticate(string username, string password)
        {
            try
            {
                User _user = functionHelper.AuthenticateUser(username, password);
                if (_user != null)
                {
                    dataHelper.User_IsLoggedIn = true;
                    dataHelper.UserID = _user.UserID.ToString();
                    dataHelper.UserFullName = _user.Name;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                functionHelper.InsertErrorLog(ex, "Authenticate", "Home");
            }
            TempData["Message"] = "Username/Password are invalid";
            return RedirectToAction("Login", "Home");
        }

        [SkipAuthentication]
        public ActionResult Logout()
        {
            dataHelper.User_IsLoggedIn = false;
            dataHelper.UserID = null;
            dataHelper.UserFullName = "";
            return RedirectToAction("Login", "Home");
        }

    }
}