using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auction.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!User.IsInRole("Admin"))
            {
                ViewBag.DisplayAdminPanel = "hidden";
            }
            else
            {
                ViewBag.DisplayAdminPanel = "visible";
            }
        }

        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Index", "Auctions");
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            //return View();
            return RedirectToAction("Index", "Auctions");
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            //if (!User.IsInRole("Admin"))
            //{
            //    ViewBag.DisplayLInk = "hidden";
            //}
            //else
            //{
            //    ViewBag.DisplayLInk = "visible";
            //}

            //return View();

            return RedirectToAction("Index", "Auctions");
        }
    }
}