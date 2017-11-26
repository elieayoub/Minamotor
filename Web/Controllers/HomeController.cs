using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : MainController
    {
        public ActionResult Index()
        {
            ViewBag.MakeList = new SelectList(dataHelper.MakeList.Select(obj => new SelectListItem { Text = obj.Name, Value = obj.MakeID.ToString() }).AsEnumerable(), "Value", "Text");
            ViewBag.ModelList = dataHelper.ModelList;
            ViewBag.VehicleTrimList = dataHelper.VehicleTrimList;

            Guid specialListId = Guid.Parse(functionHelper.GetFromConfig("SpecialListID"));
            Guid featuredListId = Guid.Parse(functionHelper.GetFromConfig("FeaturedListID"));
            ViewBag.WebCarListViewListSpecial = dataHelper.WebCarListViewList.Where(obj => obj.listID == specialListId).ToList();
            ViewBag.WebCarListViewListFeatured = dataHelper.WebCarListViewList.Where(obj => obj.listID == featuredListId).ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}