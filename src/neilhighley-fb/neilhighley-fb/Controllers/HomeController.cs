using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using neilhighley_fb.Models;

namespace neilhighley_fb.Controllers
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

        public ActionResult Facebook()
        {
            ViewBag.Message = "Your Facebook page.";

            var fbm = new FacebookUserModel() {Name = User.Identity.Name};
            fbm.Friends=new List<FriendModel>()
            {
                new FriendModel(){Name = "Neil",Profile=""},
                new FriendModel(){Name="Fred",Profile = ""}
            };

            return View(fbm);
        }
    }
}