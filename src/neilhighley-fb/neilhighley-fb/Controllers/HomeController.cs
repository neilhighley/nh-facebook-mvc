using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Facebook;
using neilhighley_fb.Models;
using NLog;
using NLog.Fluent;

using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace neilhighley_fb.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        public async Task<ActionResult> Facebook(string code)
        {
            //CheckAuthorisation(code);
            ViewBag.Message = "Your Facebook page.";

            var fbm = new FacebookUserModel() {Name = User.Identity.Name};
            fbm.Friends=new List<FriendModel>()
            {
                new FriendModel(){Name = "Neil",Profile=""},
                new FriendModel(){Name="Fred",Profile = ""}
            };

            fbm = GetData(Session["access_token"] as Claim,User.Identity);
            

            return View(fbm);
        }

        private FacebookUserModel GetData(Claim access_token, IIdentity identity)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            
            
            try
            {
                var email = "";//claimsIdentity.FindAll("urn:facebook:email").First().Value;

                var fb = new FacebookClient(access_token.Value);
                dynamic myFriends = fb.Get("/me/taggable_friends");
                dynamic myStatuses = fb.Get("/me/statuses");

                logger.Debug(myFriends);

                var fbm = new FacebookUserModel()
                {
                    Name = identity.Name, Email = email
                };
                var friendsList = new List<FriendModel>();
                foreach (dynamic friend in myFriends.data)
                {
                    friendsList.Add(new FriendModel()
                    {
                        Name = friend.name,
                        Image = friend.picture.data.url
                    });
                    //Response.Write("Name: " + friend.name + "<br/>Facebook id: " + friend.id + "<br/><br/>");
                }
                fbm.Friends = friendsList;
                List<StatusModel> sm=new List<StatusModel>();
                foreach (dynamic s in myStatuses.data)
                {
                    sm.Add(new StatusModel(s));
                }
                fbm.Statuses = sm;
                return fbm;
            }
            catch(Exception e)
            {
                logger.Error(e);
                return new FacebookUserModel();
            }


        }

        //public ActionResult FBredirect(string code)
        //{
        //    CheckAuthorisation(code);
        //    return Facebook(code);
        //}

        //private void CheckAuthorisation(string code)
        //{
        //    if (code == null)
        //    {
        //        Response.Clear();
        //        Response.Redirect("https://www.facebook.com/dialog/oauth?" +
        //       "client_id=" +  ConfigurationManager.AppSettings["Facebook:AppId"]+
        //       "&redirect_uri=" + "https://localhost:44302/Home/FBredirect" +
        //       "&scope=email,publish_stream,email,rsvp_event,read_stream,user_likes,user_birthday"+
        //       "&response_type=code" );
        //    }
        //    else
        //    {
        //        FacebookClient client = new FacebookClient();
        //        dynamic result = client.Get("oauth/access_token", new
        //        {
        //            client_id = ConfigurationManager.AppSettings["Facebook:AppId"],
        //            redirect_uri = "https://localhost:44302/Home/FBredirect",
        //            client_secret = ConfigurationManager.AppSettings["Facebook:AppSecret"],
        //            code = code
        //        });
        //        var r = result;

        //    }

        //}

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static HomeController.ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new HomeController.ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("urn:facebook:access_token"),
                };
            }
        }
    }

   
}