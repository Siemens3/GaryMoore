using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketSystem.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
       [Authorize]
        public ActionResult Merchandise()
        {
            return View();
        }

        public ActionResult WhyBrighton()
        {
            return View();
        }
        public ActionResult GaryMoore()
        {
            return View();
        }
        public ActionResult PlacesOfInterest()
        {
            return View();
        }
        public ActionResult PastGatherings()
        {
            return View();
        }
        public ActionResult UsefulLinks()
        {
            return View();
        }
    }
}